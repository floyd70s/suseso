using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Globalization;

namespace suseso
{
    public class Email
    {
        private string conStringSQL = ConfigurationManager.ConnectionStrings["EmailCS"].ConnectionString;
        private DataManager _myDataManager;
        private DataManager myDataManager
        {
            get => _myDataManager;
            set => _myDataManager = new DataManager(conStringSQL);
        }
        public Email()
        {
        }
        private string getTemplateXML()
        {
            string text = string.Empty;
            System.IO.StreamReader sr = new System.IO.StreamReader(ConfigurationManager.AppSettings["EmailTemplate"].ToString(), System.Text.Encoding.Default);
            text = sr.ReadToEnd();
            sr.Close();

            return text;
        }

        public void sendEmail(int procesados, int sinprocesar, int importados)
        {
            string emailBody = this.getTemplateXML();
            string rowIndicador = string.Empty;
            IFormatProvider culture = new CultureInfo("es-ES", true);
            string fecha = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy", culture);

            emailBody = emailBody.Replace("[##Fecha##]", DateTime.Now.ToString("dd.MM.yyyy"));
            emailBody = emailBody.Replace("[##Cola##]", sinprocesar.ToString());

            //las carpetas de cada corte
            int i = 0;

            rowIndicador += this.getRowTemplate(i++, fecha, "SUSESO", "1", sinprocesar, procesados, importados);
            rowIndicador += this.getRowTemplate(i++, fecha, "Total", "0", sinprocesar, procesados, importados);

            emailBody = emailBody.Replace("[##FALLOS##]", rowIndicador);
            string sResultEmail = this.saveEmail(emailBody);
        }

        private string saveEmail(string msgHTML)
        {
            this.myDataManager = new DataManager(this.conStringSQL);
            string DeNombre = ConfigurationManager.AppSettings["EmaildeNombre"].ToString();
            string DeCorreo = ConfigurationManager.AppSettings["EmaildeCorreo"].ToString();
            string Titulo = ConfigurationManager.AppSettings["EmailTitulo"].ToString();
            string ResponderA = ConfigurationManager.AppSettings["EmailResponderA"].ToString();
            string ParaCC = ConfigurationManager.AppSettings["EmailParaCC"].ToString();
            string ContentHTML = "1";
            string EmailNombre = ConfigurationManager.AppSettings["EmailNombre"].ToString();
            string EmailEmail = ConfigurationManager.AppSettings["EmailEmail"].ToString();

            string SQL = "INSERT INTO ErrMail(ErrNombreDe, ErrMailDe, ErrNombrePara, ErrMailPara, ErrTitulo, ErrMensaje, ErrResponderA, ErrParaCC,ErrTipoMensaje, ErrFechaError, ErrFechaEnviado, ErrEnviado, ErrNroIntento) ";
            SQL += " VALUES('" + DeNombre + "',  '" + DeCorreo + "', '" + EmailNombre + "', '" + EmailEmail + "', '" + Titulo + "', '" + msgHTML + "', '" + ResponderA + "', '" + ParaCC + "', " + ContentHTML + ", GETDATE(), NULL, 0, 0 )";

            string sMsg = myDataManager.setDataSQL(SQL);
            if (sMsg == "ok")
            {
                Console.WriteLine("El correo fue ingresado correctamente.");
                return "ok";
            }
            else
            {
                return "error en el ingreso";
            }
        }
        private string getRowTemplate(int i, string fecha, string titulo, string id, int sinprocesar, int procesados, int grabados)
        {
            string color = "#FFF";
            string estatusTmp = string.Empty;

            if (i % 2 == 0)
                color = "EEE";
            else
                color = "FFF";

            string rTemplate = "<tr style=\"border-collapse:collapse; border:1px solid #666; background-color:#[##COLOR##];\">";
            rTemplate += "<td style=\"border-collapse:collapse; border:1px solid #666; padding:5px\" align=\"right\">";
            rTemplate += "<span style=\"color:#000000; font-size:11px; font-family:Arial;\">[##POS##]</span>";
            rTemplate += "</td>";
            rTemplate += "<td style=\"border-collapse:collapse; border:1px solid #666; padding:5px\" align=\"left\">";
            if (id != "0")
                rTemplate += "<span style=\"color:#000000; font-size:11px; font-family:Arial;\"><a href=\"http://bo.legalpublishing.cl/Intranet/Mant_Fallos/ListadoFallos.dev.asp?masivo=1&tribunal=" + id + "&fecha=" + fecha + "\">[##TRIBUNAL##]</a></span>";
            else
                rTemplate += "<span style=\"color:#000000; font-size:11px; font-family:Arial;\">[##TRIBUNAL##]</span>";
            rTemplate += "</td>";
            rTemplate += "<td style=\"border-collapse:collapse; border:1px solid #666; padding:5px\" align=\"right\">";
            rTemplate += "<span style=\"color:#000000; font-size:11px; font-family:Arial;\">[##SINPROCESAR##]</span>";
            rTemplate += "</td>";
            rTemplate += "<td style=\"border-collapse:collapse; border:1px solid #666; padding:5px\" align=\"right\">";
            rTemplate += "<span style=\"color:#000000; font-size:11px; font-family:Arial;\">[##PROCESADOS##]</span>";
            rTemplate += "</td>";
            rTemplate += "<td style=\"border-collapse:collapse; border:1px solid #666; padding:5px\" align=\"right\">";
            rTemplate += "<span style=\"color:#000000; font-size:11px; font-family:Arial;\">[##GRABADOS##]</span>";
            rTemplate += "</td>";
            rTemplate += "<td style=\"border-collapse:collapse; border:1px solid #666; padding:5px\" align=\"right\">";
            rTemplate += "<span style=\"color:#000000; font-size:11px; font-family:Arial;\">[##PORCENTAJE##]</span>";
            rTemplate += "</td>";
            rTemplate += "</tr>";

            rTemplate = rTemplate.Replace("[##COLOR##]", color);
            rTemplate = rTemplate.Replace("[##POS##]", (i + 1).ToString());
            rTemplate = rTemplate.Replace("[##TRIBUNAL##]", titulo);
            rTemplate = rTemplate.Replace("[##SINPROCESAR##]", sinprocesar.ToString());
            rTemplate = rTemplate.Replace("[##PROCESADOS##]", procesados.ToString());
            rTemplate = rTemplate.Replace("[##GRABADOS##]", grabados.ToString());
            rTemplate = rTemplate.Replace("[##PORCENTAJE##]", this.porcentaje(procesados, grabados));


            return rTemplate;
        }

        private string porcentaje(int procesados, int grabados)
        {

            double dProcesados = double.Parse(procesados.ToString());
            double dGrabados = double.Parse(grabados.ToString());

            double percent = 0;
            if (procesados != 0)
            {
                percent = (dGrabados * 100) / dProcesados;
            }
            else
            {
                percent = 0;
            }

            return percent.ToString("n2");
        }

        public string sendEmail()
        {
            try
            {
                var fromAddress = new MailAddress("claudioperezdiaz@gmail.com", "From Name");
                var toAddress = new MailAddress("floyd70s@gmail.com", "To Name");
                const string fromPassword = "Galloviejo1";
                const string subject = "Reporte SUSESO";
                const string body = "Detalle del informe";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                return "error";
            }


        }
    }
}
