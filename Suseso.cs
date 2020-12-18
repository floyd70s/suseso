using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.IO;
//using Microsoft.Data.Sqlite;
using System.Net;
using System.Web;

namespace suseso
{
    public class Suseso
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region properties

        [JsonProperty("cid")]
        public int cid { get; set; }

        [JsonProperty("pid")]
        public string pid { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("property-value_532")]
        public string comment { get; set; }

        [JsonProperty("abstract")]
        public string abstrac { get; set; }

        [JsonProperty("property-value_620_name")]
        public string theme { get; set; }

        [JsonProperty("extended-property-value_pvid")]
        public string linkedCirculars { get; set; }

        [JsonProperty("property-value_620_pid")]
        public string property_value_620_pid { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("property-value_546_iso8601")]
        public DateTime sentenceDate { get; set; }

        [JsonProperty("binary_id")]
        public string binary_id { get; set; }

        [JsonProperty("aid")]
        public string aid { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("iid")]
        public string iid { get; set; }

        [JsonProperty("score")]
        public string score { get; set; }

        [JsonProperty("property-value_525")]
        public string property_value_525 { get; set; }

        [JsonProperty("hl1")]
        public string hl1 { get; set; }

        [JsonProperty("using_cids")]
        public string using_cids { get; set; }

        private string conStringSQLite = ConfigurationManager.ConnectionStrings["conStringSQLite"].ConnectionString;
        private DataManager _myDataManager;
        private DataManager myDataManager
        {
            get => _myDataManager;
            set => _myDataManager = new DataManager(conStringSQLite);
        }

        public Suseso()
        {
        }

        #endregion
        /// <summary>
        /// insert instance of suseso to SQLite Database.
        /// </summary>
        /// <returns> OK/Error </returns>
        public string addElement()
        {
            try
            {
                this.myDataManager = new DataManager(this.conStringSQLite);
                string SQL = "INSERT INTO  'SUSESO' ('aid', 'title', 'abstract', 'name', 'insertDate', 'status', 'sentenceDate','rol') VALUES (" +
                            this.aid + "," +
                            "'" + this.hl1 + "'," +
                            "'" + this.abstrac + "'," +
                            "'" + this.name + "'," +
                            "'" + System.DateTime.Now + "'," +
                            "0," +
                            "'" + this.sentenceDate.ToString("yyyy/MM/dd") + "'," +
                            "" + Convert.ToInt32(this.hl1.Substring(8).Trim()) + ");";

                string sMsg = myDataManager.setData(SQL);
                if (sMsg == "ok")
                {
                    Console.WriteLine("El registro  \"{0}\" ingresado correctamente.", this.aid);
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
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);

                Console.WriteLine("........Fail");
                return "error";
            }
        }

        /// <summary>
        /// update status in SUSESO SQLite DB
        /// </summary>
        /// <returns>string with  OK/Error</returns>
        public string update()
        {
            try
            {
                this.myDataManager = new DataManager(this.conStringSQLite);
                string SQL = "update SUSESO set status=1 where aid=" + this.aid;

                string sMsg = myDataManager.setData(SQL);
                if (sMsg == "ok")
                {
                    Console.WriteLine("El registro  \"{0}\" actualizado correctamente.", this.aid);
                    return "ok";
                }
                else
                {
                    return "error en la actualizacion.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                return "error";
            }
        }

        /// <summary>
        ///  validate AID
        ///  if the record exists, then it returns true
        /// </summary>
        /// <returns></returns>
        public bool validateAID()
        {
            try
            {
                DataTable dtTemp;
                this.myDataManager = new DataManager(this.conStringSQLite);

                string rol = this.hl1.Substring(8).Trim();
                string sSQL = "select AID from SUSESO where AID=" + this.aid + " LIMIT 1;";
                dtTemp = this.myDataManager.getData(sSQL);
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
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                return false;
            }
        }

        /// <summary>
        /// Obtain all records from suseso table with status "0" -->pending
        /// </summary>
        /// <returns>dataset with records from SUSESO with status 0 - pending </returns>
        public DataTable getAll()
        {
            this.myDataManager = new DataManager(this.conStringSQLite);
            //string SQL = "select aid,title,abstract,name,status,rol from SUSESO where status=0";
            string SQL = "select aid,title,abstract,name,insertDate,status,rol,sentenceDate from SUSESO where status=0";
            DataTable miDataTable = myDataManager.getDataTemp(SQL);
            return miDataTable;
        }

        /// <summary>
        /// save PDF from website SUSESO with AID
        /// </summary>
        /// <param name="PDFPath">path for save PDF file</param>
        /// <param name="sAid">unique ID for PDF file</param>
        /// <returns> string with link </returns>
        public string savePdf(string sAid)
        {
            string PDFPath = ConfigurationManager.AppSettings["PDFPath"];               //Path to save PDF
            string sUrlPDF = "https://www.suseso.cl/612/articles-" + sAid + "_archivo_01.pdf";
            string sLocalPDF = PDFPath + "\\" + sAid + "_archivo_01.pdf";
            Console.WriteLine("salida de sLocalPDF: " + sLocalPDF);

            try
            {
                var request = WebRequest.Create(sUrlPDF);
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    // Process the stream
                }
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
            {
                // handle 404 exceptions
                //Not found
                Console.WriteLine("El archivo {0} fue remapeado", sUrlPDF);
                log.Info("El archivo fue remapeado- "+sUrlPDF);
                sUrlPDF = "https://www.suseso.cl/612/articles-" + sAid + "_archivo_01.";
            }
            catch (WebException ex)
            {
                // handle other web exceptions
            }

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(sUrlPDF, sLocalPDF);
                return sUrlPDF;
            }
        }
        /// <summary>
        /// extract text from SUSESO Web Site
        /// </summary>
        /// <param name="URL">URL SUSESO Web</param>
        /// <returns>string with the suseso dump</returns>
        public string extractWebSuseso(string URL)
        {
            try
            {
                string docImportSrc = string.Empty;
                string infoBase = "";

                using (WebClient webClient = new WebClient())       //get JSON from suseso
                {
                    docImportSrc = URL;
                    webClient.Encoding = System.Text.Encoding.UTF8;
                    infoBase = webClient.DownloadString(URL);
                }
                Console.WriteLine("load website Ok");
                return infoBase;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                return "error";
            }
        }

        /// <summary>
        /// Extract text from PDFFile
        /// </summary>
        /// <param name="filePath">local file path </param>
        /// <returns>string with the content of PDF file</returns>
        public string extractTextFromPDF(string filePath)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);

                PdfReader pdfReader = new PdfReader(file);
                PdfDocument pdfDoc = new PdfDocument(pdfReader);


                string pageContent = "";
                for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    pageContent = pageContent + "-" + PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                }
                pdfDoc.Close();
                pdfReader.Close();
                return pageContent;

            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                return "";
            }
        }

        /// <summary>
        /// method for extract JSON from website string
        /// </summary>
        /// <param name="siteBase"></param>
        /// <returns></returns>
        public string extractJson(string siteBase)
        {

            int iniJson = siteBase.IndexOf("[");
            int endJson = siteBase.IndexOf("properties");
            return siteBase.Substring(iniJson, endJson - iniJson - 3);
        }
        /// <summary>
        /// obtain number of element from JSON
        /// </summary>
        /// <param name="siteBase"></param>
        /// <returns></returns>
        public int extractNumberOfElements(string siteBase)
        {
            try
            {
                int iniResult = siteBase.IndexOf("num_results");
                int endResult = siteBase.IndexOf("hits_number");
                string sNumber = siteBase.Substring(iniResult + 14, endResult - iniResult - 17).Trim();
                return Convert.ToInt32(sNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                return 0;
            }
        }
    }
}
