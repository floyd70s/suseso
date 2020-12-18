using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Pdf;
using System.Text.RegularExpressions;

namespace suseso
{
    public class Ocr
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Ocr()
        {
        }
        /// <summary>
        /// Convert PDF to Text
        /// </summary>
        /// <param name="sMode">win o mac mode</param>
        /// <param name="sPDFFilePath"></param>
        /// <param name="sTIFFFilePath"></param>
        /// <param name="sTXTFilePath"></param>
        /// <param name="sTIFFPath"></param>
        /// <param name="sTXTFPath"></param>
        /// <param name="sLanguage"></param>
        /// <returns></returns>
        public string PdfToText(string sMode, string sPDFFilePath, string sTIFFFilePath, string sTXTFilePath, string sTIFFPath, string sTXTFPath, string sLanguage, int iResolution)
        {
            string sText = "";           
            PdfToTiff(sPDFFilePath, sTIFFFilePath, iResolution);

            if (sMode == "win")
            {
                //convert TIFF file to Text
                ExecuteCommand("C:\\Program Files\\Tesseract-OCR\\tesseract.exe ", "-l " + sLanguage + " " + sTIFFFilePath + " " + sTXTFilePath);
                sText = FileToText(sTXTFilePath + ".txt");
            }
            else
            {
                ExecuteMacCommand("tesseract -l " + sLanguage + " " + sTIFFFilePath + " " + sTXTFilePath);
                sText = FileToText(sTXTFilePath + ".txt");
            }
            return sText;
        }
        /// <summary>
        /// delete files from folder in Mac
        /// </summary>
        /// <param name="sPath"></param>
        /// <param name="sExt"></param>
        /// <returns></returns>
        public string cleanFolderMac(string sPath, string sExt)
        {
            try
            {
                ExecuteMacCommand("rm -f " + sPath + "*." + sExt);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                log.Error("Exception: " + e.Message);
                return "no fue posible borrar los directorios temporales de conversion OCR.";
            }
            finally
            {
                Console.WriteLine("Error en la limpieza de directorios.");
            }
            return "Borrado de  directorios temporales de conversion OCR OK";
        }

        /// <summary>
        /// delete files from folder in Windows
        /// </summary>
        /// <param name="sPath"></param>
        /// <param name="sExt"></param>
        /// <returns></returns>
        public void cleanFolderWin(string sPath, string sExt)
        {
            try
            {
                foreach (var item in Directory.GetFiles(sPath + "\\", "*.*"))
                {
                    File.SetAttributes(item, FileAttributes.Normal);
                    File.Delete(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No fue posible limpiar el directorio {0} { Error: {1}", sPath, ex.Message);
                log.Error("No fue posible limpiar el directorio "+sPath+"  Error: "+ ex.Message);
            }
        }

        /// <summary>
        /// execute terminal command in Mac
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteMacCommand(string command)
        {
            Console.WriteLine("Inicio Conversion a TXT");
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                Console.WriteLine(proc.StandardOutput.ReadLine());
            }
        }
        /// <summary>
        /// execute CMD command terminal in windows
        /// </summary>
        /// <param name="sProgram"></param>
        /// <param name="command"></param>
        public static void ExecuteCommand(string sProgram, string command)
        {
            try
            {
                Console.WriteLine("Inicio Conversion OCR a TXT");
                ProcessStartInfo processStartInfo = new ProcessStartInfo(sProgram);
                processStartInfo.UseShellExecute = false;
                processStartInfo.Arguments = command;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                processStartInfo.FileName = @sProgram;

                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                Console.WriteLine("proceso en curso...");
                process.WaitForExit();  // wait until finish the process
                Console.WriteLine("Fin de Conversion OCR a TXT");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
            }
        }


        /// <summary>
        /// Convert PDF to Tiff
        /// </summary>
        /// <param name="sPathPdf">PDF file path</param>
        /// <param name="sPathTiff">Tiff file path</param>
        public static void PdfToTiff(string sPathPdf, string sPathTiff, int iResolution)
        {
            try
            {
                Console.WriteLine("Inicio Conversion a TIFF");
                GcPdfDocument doc = new GcPdfDocument();

                var fs = new FileStream(Path.Combine("Resources", sPathPdf), FileMode.Open, FileAccess.Read);
                doc.Load(fs, "");

                SaveAsImageOptions options = new SaveAsImageOptions();
                options.BackColor = Color.White;

                //The tiff file must have high resolution
                options.Resolution = iResolution;

                //Set range for page
                int paginas = doc.Pages.Count;
                List<int> pages = new List<int>();
                for (int i = 0; i < paginas; i++)
                {
                    if (i > 0)
                        pages.Add(i + 1);
                }

                doc.SaveAsTiff(sPathTiff, null, options);
            }
            catch (Exception ex)
            {
                // handle other web exceptions
                Console.WriteLine("No fue posible descargar el archivo a TIFF  Error: {1}", ex.Message);
                log.Error("No fue posible descargar el archivo a TIFF  Error: "+ ex.Message);
            }
        }

        /// <summary>
        /// obtain text from Text file
        /// </summary>
        /// <param name="sPathTxt">text file path</param>
        /// <returns>text from Txt file</returns>
        public string FileToText(string sPathTxt)
        {
            Console.WriteLine("Inicio de lectura de Texto desde TXT");
            String sTextFile = "";

            try
            {
                sTextFile = System.IO.File.ReadAllText(@sPathTxt.Trim());
                sTextFile = ReplaceHexadecimalSymbols(sTextFile);
                return sTextFile;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                log.Error("Exception: " + e.Message);
                return "no disponible";
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        /// <summary>
        /// replace text for identation
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string ReplaceHexadecimalSymbols(string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26\x91\x92]";
            txt = Regex.Replace(txt, r, " ", RegexOptions.Compiled);
            txt = txt.Replace("<", "&lt;");
            txt = txt.Replace(">", "&gt;");
            txt = txt.Replace("\n\n", "*SALTO*");
            txt = txt.Replace("\n", "");
            txt = txt.Replace("*SALTO*", "\n\n");

            return txt;
        }
    }
}
