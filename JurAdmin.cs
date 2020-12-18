using System;
using System.Data;
using System.Configuration;

namespace suseso
{
    public class JurAdmin
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region PROPERTIES
        private string conStringSQL = ConfigurationManager.ConnectionStrings["conStringSQL"].ConnectionString;
        public string GUID { get; set; }
        public int tipoDocumento { get; set; }
        public string cita { get; set; }
        public string citaOnline { get; set; }
        public string rol { get; set; }
        public DateTime fechaSentencia { get; set; }
        public string titulo { get; set; }
        public string sumario { get; set; }
        public string textoSentencia { get; set; }
        public string linkOrigen { get; set; }
        public DateTime fechaRegistro { get; set; }
        public int migrado { get; set; }

        private DataManager _myDataManager;
        private DataManager myDataManager
        {
            get => _myDataManager;
            set => _myDataManager = new DataManager(conStringSQL);
        }

        public JurAdmin()
        {

        }
        #endregion
        /// <summary>
        ///  Validate AID
        ///  If the record exists, then it returns true
        ///  Compare ID with ROL
        /// </summary>
        /// <returns></returns>
        public bool validateRol()
        {
            DataTable dtTemp;
            this.myDataManager = new DataManager(this.conStringSQL);

            string sSQL = "SELECT TOP 1 ROL FROM JUR_ADMINISTRATIVA WHERE ROL='" + this.rol + "';";
            dtTemp = this.myDataManager.getDataSQL(sSQL);

            if (dtTemp.Rows.Count > 0)
            {
                if (dtTemp.Rows[0][0].ToString() != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// insert instance of JUR_ADMINISTRATIVA to SQL Database.
        /// </summary>
        /// <returns> OK/Error </returns>
        public string addElement()
        {
            try
            {
                this.myDataManager = new DataManager(this.conStringSQL);
                string sfechaRegistro = this.fechaRegistro.Date.ToString("yyyy/MM/dd HH:mm:ss");
                string sfechaSentencia= this.fechaSentencia.Date.ToString("yyyy/MM/dd HH:mm:ss");
                this.sumario=this.sumario.Replace("'", "");
                this.titulo=this.titulo.Replace("'", "");
                this.textoSentencia=this.textoSentencia.Replace("'", "");

                string SQL = "INSERT INTO JUR_ADMINISTRATIVA (" +
                             "[GUID]" +
                             ",[TIPODOCUMENTO_ID]" +
                             ",[CITA]" +
                             ",[CITAONLINE]" +
                             ",[ROL]" +
                             ",[FECHASENTENCIA]" +
                             ",[TITULO]" +
                              ",[SUMARIO]" +
                              ",[TEXTOSENTENCIA]" +
                              ",[LINKORIGEN]" +
                              ",[FECHAREGISTRO]" +
                              ",[MIGRADO]" +
                              ") VALUES(" + "'',1,'NO DISPONIBLE','NO DISPONIBLE',"+this.rol+ ",'" + sfechaSentencia+"','" + this.titulo+"','"+this.sumario+"','"+
                              this.textoSentencia + "','" +this.linkOrigen+ "','" + sfechaRegistro + "',0)";

                string sMsg = myDataManager.setDataSQL(SQL);
                if (sMsg == "ok")
                {
                    Console.WriteLine("El registro  \"{0}\" ingresado correctamente.", this.rol);
                    return "ok";
                }
                else
                {
                    return "error en el ingreso";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                return "error";
            }
        }
    }
}
