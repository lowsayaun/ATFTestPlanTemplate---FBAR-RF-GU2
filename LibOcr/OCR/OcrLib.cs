using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;

namespace LibOcr
{
    public class OCR
    {
        #region Class Property
        private string VisionDefaultPath = "";
        private string ResultDefault = "";
        private string Site = "";
        private string ImageServer = "";
        private string SdiServer = "";
        private string ResultFileName = "";
        private string lotId;
        private string SublotId;
        private DateTime DT;
        private string fileNameWithoutExtension;
        private string ResultZipFileName;
        private string ResultZipPath;
        private bool debug = false;

        public string SetVisionDefaultPath
        {
            set { VisionDefaultPath = value; }
        }
        public string SetResultDefault
        {
            set { ResultDefault = value; }
        }
        public string SetSite
        {
            set { Site = value; }
        }
        public string SetImageServer
        {
            set { ImageServer = value; }
        }
        public string SetSdiServer
        {
            set { SdiServer = value; }
        }
        public string SetResultFile
        {
            set { ResultFileName = value; }
        }
        public string SetLotID
        {
            set { lotId = value; }
        }
        public string SetSublotId
        {
            set { SublotId = value; }
        }
        public DateTime SetDateTime
        {
            set { DT = value; }
        }
        public bool SetDebugMode
        {
            set { debug = value; }
        }
        #endregion

        public void Merge()
        {

            string OCR_enable = "false",
            TempLocation = @"C:\Avago.ATF.Common\OCR\",
            VisionFileName = "",

            //

            ResultPath = "";


            if (OCR_enable.ToUpper() == "TRUE")
            {
                switch (MessageBox.Show("Do you want to merge OCR automatically?", "Penang NPI", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.Yes:

                        //Read Local setting file:
                        //VisionDefaultPath = Drivers.IO_TextFile.ReadTextFile(LocalSettingFile, "OCR", "VisionDefaultPath");
                        //ResultDefault = Drivers.IO_TextFile.ReadTextFile(LocalSettingFile, "OCR", "ResultDefaultPath");
                        //Site = Drivers.IO_TextFile.ReadTextFile(LocalSettingFile, "OCR", "Site");
                        //ImageServer = Drivers.IO_TextFile.ReadTextFile(LocalSettingFile, "OCR", "ImagePath");
                        //SdiServer = Drivers.IO_TextFile.ReadTextFile(LocalSettingFile, "OCR", "SdiserverPath");
                        //try  //CY, allow user to turn on/off the debug mode
                        //{
                        //    debug = Drivers.IO_TextFile.ReadTextFile(LocalSettingFile, "OCR", "debug");
                        //}
                        //catch
                        //{
                        //    debug = "false";
                        //}

                        //Generate file name
                        //ResultFileName = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_CUR_RESULT_FILE, "");

                        //Full result path:
                        ResultPath = ResultDefault + ResultFileName;

                        //Vision file name:
                        VisionFileName = lotId + "_" + SublotId + "_" + DT.ToString("yyyy-MM-dd") + "_" + Site + ".txt";

                        //Create Temporary OCR folder if not exist
                        if (!Directory.Exists(TempLocation))
                        {
                            Directory.CreateDirectory(TempLocation);
                        }

                        //Copy result to temp location due to Excel locked by Clotho
                        File.Copy(ResultPath, TempLocation + ResultFileName);

                        //CY- Rename the vision generated text file
                        System.IO.File.Move(VisionDefaultPath + "___1.txt", VisionDefaultPath + VisionFileName);
                        Thread.Sleep(1000);

                        //Start merge
                        if (ResultFileName != "")
                        {
                            if (debug)
                            {
                                MessageBox.Show("Start Merging\r\n" + "VisionDefaultPath: " + VisionDefaultPath + "\r\nVisionFileName: " + VisionFileName + "\r\nTempLocation: " + TempLocation + "\r\nResultFileName: " + ResultFileName);
                            }
                            MergeOCRWithFullResults(VisionDefaultPath + VisionFileName, TempLocation + ResultFileName);
                            if (debug)
                            {
                                MessageBox.Show("End Merging");
                            }
                            string imageName = lotId + "_" + SublotId + "_" + DT.ToString("yyyy-MM-dd") + "_" + Site;

                            //Move and delete
                            try
                            {
                                //Commented this 2 line because clotho 2.1.x will reports the result.csv example with IP192.10.10.10 instead IP192101010 - Shaz 22/03/2013
                                //string[] atemp = ResultFileName.Split('.');
                                //string NewResult = atemp[0] + "_WithOCR.CSV";

                                //CY_ - CLotho 2.2.6 not allow program to modify the filename. Hence, obsolete below 5 lines code.
                                //string tempchar = ResultFileName.Remove(ResultFileName.Length - 4, 4);
                                //string NewResult = tempchar + "_WithOCR.CSV";
                                //File.Move(TempLocation + NewResult, ResultDefault + NewResult);
                                //File.Delete(TempLocation + NewResult);
                                //File.Delete(TempLocation + ResultFileName);
                            }
                            catch { }

                            try
                            {
                                //Compressed and send to server
                                //CY- change the Image temporary store location from @"C:\Image\" to @"C:\Avago.ATF.Common\OCR\Image".
                                //This is because Inari production pc not allow create folder in C:\ drive.
                                string imageCompressedPath = TempLocation + @"Image\";  //CY
                                if (SublotId != "" && lotId != "")
                                {
                                    if (!Directory.Exists(imageCompressedPath))
                                    {
                                        Directory.CreateDirectory(imageCompressedPath);
                                    }
                                    else
                                    {
                                        Directory.Delete(imageCompressedPath, true);
                                        Directory.CreateDirectory(imageCompressedPath);
                                    }

                                    if (Directory.Exists(ImageServer))
                                    {
                                        string[] files = Directory.GetFiles(ImageServer);

                                        foreach (string file in files)
                                        {
                                            string fileName = Path.GetFileName(file);
                                            string destFile = Path.Combine(imageCompressedPath, fileName);
                                            //File.Copy(file, destFile, true);  //CY Inari request do not keep the image at the vision pc, so intead copy, i move it.
                                            File.Move(file, destFile);
                                        }
                                    }
                                    else
                                    {
                                        //CY
                                        MessageBox.Show("OCR Image path not exist!\r\n" + ImageServer);
                                    }

                                    CompressSDIFileInDirectory(TempLocation, "Image");
                                    if (debug)
                                    {
                                        MessageBox.Show("Compressed Image folder stored at :" + TempLocation + "Image.tar.bz2");
                                    }

                                    //CY - create the folder for store the image zip folder
                                    if (!Directory.Exists(SdiServer + @"Image\"))
                                    {
                                        Directory.CreateDirectory(SdiServer + @"Image\");
                                    }

                                    CopyFile(TempLocation + "Image.tar.bz2", SdiServer + @"Image\" + imageName + ".tar.bz2");
                                    File.Delete(TempLocation + "Image.tar.bz2");
                                    Directory.Delete(imageCompressedPath, true);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Compress file error!\r\n" + ex.Message);
                            }

                            //CY- create a thread to wait until the CLotho generated the result zip file, then further process it.
                            //Copy result file						
                            fileNameWithoutExtension = ResultFileName.Remove(ResultFileName.Length - 4, 4);
                            ResultZipFileName = fileNameWithoutExtension + ".zip";

                            //Full result backup path:
                            ResultZipPath = @"C:\Avago.ATF.Common\Results.Backup\" + ResultZipFileName;
                            if (debug)
                            {
                                MessageBox.Show("ResultZipPath (Result.BackUp): " + ResultZipPath);
                            }

                            Thread WaitReplaceUpload = new Thread(ProcessingFile);
                            WaitReplaceUpload.Start();

                        }

                        break;
                    case DialogResult.No:
                        break;
                    default:
                        break;
                }
            }


        }
        private void ProcessingFile()
        {
            string localOCRPath = @"C:\Avago.ATF.Common\OCR\";

            do
            {
                Thread.Sleep(2000);

            } while (!File.Exists(ResultZipPath));
            Thread.Sleep(1000);

            if (!Directory.Exists(localOCRPath + fileNameWithoutExtension))
            {
                Directory.CreateDirectory(localOCRPath + fileNameWithoutExtension);
            }

            //Move the original result zip file from @"C:\Avago.ATF.Common\Result.BackUp\ to @"C:\Avago.ATF.Common\OCR\:
            File.Move(ResultZipPath, localOCRPath + fileNameWithoutExtension + @"\" + ResultZipFileName);

            if (debug)
            {
                MessageBox.Show("Before Unzip Ori Clotho Result File at " + localOCRPath + "fileName");
            }
            using (Ionic.Zip.ZipFile zip1 = Ionic.Zip.ZipFile.Read(localOCRPath + fileNameWithoutExtension + @"\" + ResultZipFileName))
            {
                zip1.ExtractAll(localOCRPath + fileNameWithoutExtension + @"\",
                Ionic.Zip.ExtractExistingFileAction.DoNotOverwrite);
            }

            string[] resultFiles = Directory.GetFiles(localOCRPath + fileNameWithoutExtension);

            if (debug)
            {
                foreach (string file in resultFiles)
                {
                    MessageBox.Show("File length: " + resultFiles.Length.ToString() + "\r\n" + file);
                }
            }

            if (resultFiles.Length >= 3)
            {
                File.Delete(localOCRPath + fileNameWithoutExtension + @"\" + ResultZipFileName);  //delete original result zip file
                if (debug)
                {
                    MessageBox.Show("Original Zip file deleted.");
                }
                File.Copy(localOCRPath + ResultFileName, localOCRPath + fileNameWithoutExtension + @"\" + ResultFileName, true);  //copy OCR result file and ovewrite
                File.Delete(localOCRPath + ResultFileName);
                Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
                foreach (string file in resultFiles)
                {
                    string fileName = Path.GetFileName(file);
                    if (!(fileName.Contains(".exe") || fileName.Contains(".zip")))
                    {
                        if (debug)
                        {
                            MessageBox.Show("File to be zipped: " + fileName + "\r\n At: " + localOCRPath + fileNameWithoutExtension + @"\");
                        }

                        zip.AddFile(file, "");
                    }
                }

                zip.Save(localOCRPath + fileNameWithoutExtension + @"\" + fileNameWithoutExtension + ".zip");
                if (debug)
                {
                    MessageBox.Show("OCR result file successfully zipped at " + localOCRPath + fileNameWithoutExtension + @"\" + fileNameWithoutExtension + ".zip");
                }
            }
            else
            {
                MessageBox.Show("Unzip Clotho result file contain less that 3 files could be missing result.csv, summary.txt or OriClothoResult.zip file.");
            }

            //Move back Result zip file(contain OCR data) back to the Result.BackUp folder
            System.IO.File.Move(localOCRPath + fileNameWithoutExtension + @"\" + fileNameWithoutExtension + ".zip", ResultZipPath);
            if (debug)
            {
                MessageBox.Show("OCR zipped file successfully moved to C:\\Avago.ATF.Common\\Results.Backup!");
            }

            MessageBox.Show("OCR Process Completed!!");
        }
        private void CompressSDIFileInDirectory(string defaultPath, string folderName)
        {
            RunCMD("/C cd " + defaultPath + " && tar -cvf " + folderName + ".tar " + folderName);
            RunCMD("/C cd " + defaultPath + " && bzip2 " + folderName + ".tar");
        }
        private void RunCMD(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "CMD.exe";
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
        private void CopyFile(string source, string target)
        {
            File.Copy(source, target);
        }
        private void MergeOCRWithFullResults(string OCRFileLocation, string ResultFileLocation)
        {
            //Fields
            FileStream _file = new FileStream();
            ExcelApplication _excel = new ExcelApplication();
            string[] aOCR_Date;
            string[] aOCR_Time;
            List<string> lOCR_Date = new List<string>();
            List<string> lOCR_Time = new List<string>();
            List<string> lOCR_Serial = new List<string>();
            List<string> lOCR_CSV = new List<string>();
            List<string[]> lOCRNew = new List<string[]>();
            List<string[]> lDatum = new List<string[]>();

            //added by AunThiam 28-Nov 2012
            //ClothoLibStandard.IO_TextFile IO_TXT=new IO_TextFile();
            //IO_TextFile IO_TXT = new IO_TextFile();
            //OcrMethod OCR = new OcrMethod();


            int
                iUnitCount = 0,
                iFileNameCount = 0,
                iFileNameLoop = 0,
                iResultData = 0,
                iOCRData = 0,
                iOCRColumn = 0,
                iParam = 0,
                iParamData = 1,
                iLimits = 0,
                iPID = 0,
                iDatum = 0,
                iDatumCount = 0,
                iOCRRow = 0,
                iOCRReject = 0,
                iExcelColumn = 0,
                iExcelRowTotal = 0,
                iExcelRowDatum = 0;

            string
                sMisc = "",
                sParam = "",
                sOutputFileLocation = "",
                sCurrentLocation = "",
                sFileName = "",
                sCaptureResult = "PID",
                sResultFileName = "",
                ImageServer = "";


            string[]
                aOutputFileLocation = null,
                aFileName = null,
                aParam = null,
                aDatum = null,
                aDatumCondition = null;

            //CY -Try1
            try
            {
                //Create new file name
                aOutputFileLocation = ResultFileLocation.Split('\\');
                iFileNameCount = aOutputFileLocation.Count();

                //Commented this 2 line because clotho 2.1.x will reports the result.csv filename including IP address example with IP192.10.10.10 instead IP192101010 - Shaz 22/03/2013
                //aFileName = aOutputFileLocation[iFileNameCount - 1].Split('.');
                //sFileName = aFileName[0] + "_WithOCR.CSV";
                //CY - comment two line because clotho do not zip the file
                sResultFileName = aOutputFileLocation[iFileNameCount - 1].Remove(aOutputFileLocation[iFileNameCount - 1].Length - 4, 4);
                //sFileName = sResultFileName + "_WithOCR.CSV";
                sFileName = sResultFileName + ".csv";

                //Temporary location for file operation
                sCurrentLocation = "C:\\temp\\" + sFileName;
                while (iFileNameLoop != iFileNameCount - 1)
                {
                    sOutputFileLocation += aOutputFileLocation[iFileNameLoop] + "\\";
                    iFileNameLoop++;
                }
                sOutputFileLocation += sFileName;

                //Find Result's parameter and PID's row number
                iParam = _file.ReadTextLineNumberWithWord(ResultFileLocation, "Parameter");
                iPID = _file.ReadTextLineNumberWithWord(ResultFileLocation, "PID");

                //Find OCR row number
                iOCRRow = _file.CheckTextFileLineNumber(OCRFileLocation);
                lOCR_CSV = _file.ReadTextContentByLine_List(OCRFileLocation, iOCRRow);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CY Try1\r\n" + ex.Message);
            }

            //CY try2
            try
            {
                //Serialize OCR data, removes '/' and spaces for SDI  
                while (iOCRData != iOCRRow)
                {
                    lOCRNew.Add(lOCR_CSV[iOCRData].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    aOCR_Date = lOCRNew[iOCRData][0].Split('.');
                    aOCR_Time = lOCRNew[iOCRData][2].Split(':');
                    //Join string[]
                    lOCR_Date.Add(aOCR_Date[0] + aOCR_Date[1] + aOCR_Date[2]);
                    lOCR_Time.Add(aOCR_Time[0] + aOCR_Time[1] + aOCR_Time[2]);

                    if (lOCRNew[iOCRData][4].ToUpper().Contains("F"))
                    {
                        //For Post OCR
                        //lOCR_Serial.Add("NA");

                        //For Pre OCR
                        //Check for rejects
                        iOCRReject++;
                    }
                    else
                    {
                        ////Remove "f" from 00000f
                        //if (lOCRNew[iOCRData][4].ToUpper().Contains("F"))
                        //{
                        //    lOCRNew[iOCRData][4] = lOCRNew[iOCRData][4].Remove(5, 1);
                        //    lOCR_Serial.Add(lOCRNew[iOCRData][4]);
                        //}
                        //else
                        //{
                        lOCR_Serial.Add(lOCRNew[iOCRData][4]);
                        //}
                    }
                    iOCRData++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CY Try2\r\n" + ex.Message);
            }

            //CY try3
            try
            {

                //Split Parameter's csv into arrays
                sParam = _file.ReadTextFileLine(ResultFileLocation, iParam);

                //Check if CSV corrupted
                if (sParam.Contains(","))
                {
                    //Parameter split
                    aParam = sParam.Split(',');
                    //Count column number from array
                    iExcelColumn = aParam.Count();
                    iExcelRowTotal = _file.CheckTextFileLineNumber(ResultFileLocation);
                    iExcelRowDatum = iExcelRowTotal - iParam;

                    //Redim arrays
                    aDatum = new string[iExcelRowDatum];
                    aDatumCondition = new string[iExcelRowDatum];

                    ////Check for option, capture all or PASS only
                    //if (CaptureAll)
                    //{
                    //    sCaptureResult = "PID";
                    //}
                    //else
                    //{
                    //    sCaptureResult = "PASS_ALL+";
                    //}

                    //If OCR row == Result then proceed
                    if ((iExcelRowTotal - iPID + 1) == (iOCRRow - iOCRReject))
                    {
                        //Populate data with OCR, append OCR to result data
                        while (iResultData != iExcelRowDatum)
                        {
                            aDatum[iResultData] = _file.ReadTextFileLine(ResultFileLocation, iPID + iResultData);

                            if (aDatum[iResultData] != null && aDatum[iResultData].Contains(sCaptureResult) && aDatum[iResultData].Contains(",") && iUnitCount != iOCRRow)
                            {
                                lDatum.Add(aDatum[iUnitCount].Split(','));
                                iDatumCount = lDatum[iUnitCount].Count();
                                //Join string for SDI
                                aDatumCondition[iUnitCount] = string.Join(",", lDatum[iUnitCount], 0, iDatumCount - 5);
                                aDatumCondition[iUnitCount] += "," + lOCR_Date[iUnitCount] + "," + lOCR_Time[iUnitCount] + "," + lOCR_Serial[iUnitCount] + ",";
                                aDatumCondition[iUnitCount] += string.Join(",", lDatum[iUnitCount], iDatumCount - 5, 5);
                                iUnitCount++;
                            }
                            iResultData++;
                        }

                        //Delete if file exist
                        //if (File.Exists(sOutputFileLocation))  //CY comment
                        //{
                        //    File.Delete(sOutputFileLocation);
                        //}
                        sParam = "";

                        //Print Misc before Parameter
                        while (iParamData != iParam)
                        {
                            sMisc = _file.ReadTextFileLine(ResultFileLocation, iParamData);
                            _file.WriteLineToTextFile(sCurrentLocation, sMisc);
                            iParamData++;
                        }

                        //Print Parameter + OCR column
                        while (iOCRColumn != iExcelColumn - 5) //-5 if inserted between PassFail
                        {
                            sParam += aParam[iOCRColumn] + ",";
                            iOCRColumn++;
                        }
                        sParam = sParam + "OcrDate,OcrTime,OcrSerial,";

                        while (iOCRColumn != iExcelColumn - 1)
                        {
                            sParam += aParam[iOCRColumn] + ",";
                            iOCRColumn++;
                        }
                        sParam = sParam + "SWBinName";

                        _file.WriteLineToTextFile(sCurrentLocation, sParam);
                        iLimits = iParam + 1;

                        //CY
                        //MessageBox.Show("sCurrentLocation: " + sCurrentLocation + "\r\nsOutputFileLocation: " + sOutputFileLocation);
                        //Print limits
                        while (iLimits != iPID)
                        {
                            sMisc = _file.ReadTextFileLine(ResultFileLocation, iLimits);
                            _file.WriteLineToTextFile(sCurrentLocation, sMisc);
                            iLimits++;
                        }

                        //Print Datum
                        while (iDatum != iUnitCount)
                        {
                            _file.WriteLineToTextFile(sCurrentLocation, aDatumCondition[iDatum]);
                            iDatum++;
                        }

                        //Copy file to result directory
                        //_file.CopyFile(sCurrentLocation, sOutputFileLocation);  //CY comment, replace with copy with overwrite
                        File.Copy(sCurrentLocation, sOutputFileLocation, true);
                        try
                        {
                            _file.DeleteFile(sCurrentLocation);
                        }
                        catch
                        {
                        }

                        MessageBox.Show("OCR merging successful! Please check output file for details.", "Operation completed!", MessageBoxButtons.OK, MessageBoxIcon.Information);


                        if (!Directory.Exists(@"C:\Avago.ATF.Common\OCR\Image\"))
                        {
                            Directory.CreateDirectory(@"C:\Avago.ATF.Common\OCR\Image\");
                        }
                        else
                        {
                            Directory.Delete(@"C:\Avago.ATF.Common\OCR\Image\", true);
                            Directory.CreateDirectory(@"C:\Avago.ATF.Common\OCR\Image\");
                        }
                    }
                    else
                    {
                        if (((iExcelRowTotal - iPID + 1) - (iOCRRow - iOCRReject)) > 1)
                        {
                            MessageBox.Show("Unit count not matched, additional " + ((iExcelRowTotal - iPID + 1) - (iOCRRow - iOCRReject)).ToString() + " row(s) on TEST RESULT.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Unit count not matched, additional " + ((iOCRRow - iOCRReject) - (iExcelRowTotal - iPID + 1)).ToString() + " row(s) on OCR.", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                //Exit if CSV file corrupted
                else
                {
                    MessageBox.Show("Result file(csv) corrupted, please check!", "File reading error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CY Try3\r\n" + ex.Message);
            }


            //Garbage collector, flushes out used memories
            try
            {
                GC.Collect();
            }
            catch { }
        }

    }
    public class FileStream
    {
        StreamWriter sw;

        public void CreateNewFolder(string Path)
        {

            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }
        }

        public string FileBrowser()
        {
            string FileSelect = "";
            OpenFileDialog OFD = new OpenFileDialog();
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                FileSelect = OFD.FileName.ToString();
            }
            return FileSelect;
        }

        public void WriteLineToTextFile(string FileName, string Text)
        {
            sw = new StreamWriter(FileName, true);
            sw.WriteLine(Text);
            sw.Close();
        }

        public void RunApplication(string directory)
        {
            Process ProcessBat;
            ProcessBat = new Process();
            ProcessBat = Process.Start(directory);
            ProcessBat.WaitForExit(24000);
            ProcessBat.Close();
        }
        public void DeleteFile(string filename)
        {
            File.Delete(filename);
        }

        public void CopyFile(string Source, string Destination)
        {
            File.Copy(Source, Destination);
        }

        public int CheckTextFileLineNumber(string filename)
        {
            int linenumber = 0;
            using (StreamReader r = new StreamReader(filename))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    linenumber++;
                }
            }
            return linenumber;
        }

        public ArrayList ReadTextContentByLine_ArrayList(string filename, int linenumber)
        {
            ArrayList Result = new ArrayList();
            using (StreamReader r = new StreamReader(filename))
            {
                int line = 0;
                string text = "";
                while ((line < linenumber))
                {
                    text = r.ReadLine();
                    if (text != "")
                    {
                        Result.Add(text);
                    }
                    line++;
                }
            }
            return Result;
        }

        public List<string> ReadTextContentByLine_List(string filename, int linenumber)
        {
            List<string> Result = new List<string>();
            using (StreamReader r = new StreamReader(filename))
            {
                int line = 0;
                string text = "";
                while ((line < linenumber))
                {
                    text = r.ReadLine();
                    if (text != "")
                    {
                        Result.Add(text);
                    }
                    line++;
                }
            }
            return Result;
        }

        public string ReadTextFileLine(string filename, int linenumber)
        {
            string text = "";
            int line = 0;
            using (StreamReader r = new StreamReader(filename))
            {
                while ((line < linenumber - 1))
                {
                    text = r.ReadLine();
                    line++;
                }
                text = r.ReadLine();
            }
            return text;
        }

        public int ReadTextLineNumberWithWord(string filename, string word)
        {
            int linenumber = 1;
            string text = "";
            using (StreamReader r = new StreamReader(filename))
            {
                while (true)
                {
                    text = r.ReadLine();
                    if (text != null)
                    {
                        if (text.Contains(word))
                        {
                            break;
                        }
                    }
                    linenumber++;
                }
            }
            return linenumber;
        }
    }
    public class ExcelApplication
    {
        //Interop Method - Read
        public int ReadExcelRow(string filename)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            object str;
            int rCnt = 0;
            int cCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;

            for (rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
            {
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return rCnt;
        }

        public int ReadExcelRowStartWithWord(string filename, string word)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            object str;
            string svalue;
            bool done = false;
            int rCnt = 0;
            int cCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;

            for (rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
            {
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    if (str != null)
                    {
                        svalue = (string)str;
                        if (svalue.Contains(word))
                            done = true;
                    }
                }
                if (done)
                    break;
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return rCnt;
        }

        public string ReadExcelColumn(string filename, int row)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            object str;
            string value = "";
            int rCnt = row;
            int cCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;

            for (rCnt = row; rCnt <= row; rCnt++)
            {
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    value = (string)str;
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return value;
        }

        public ArrayList ReadExcelColumn_ArrayList(string filename, int row)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            object str;
            ArrayList value = new ArrayList();
            int rCnt = 0;
            int cCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;

            for (rCnt = row; rCnt <= row; rCnt++)
            {
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    value.Add(str);
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return value;
        }

        public ArrayList ReadExcelColumnWithSize_ArrayList(string filename, int startrow, int endrow)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            object str;
            ArrayList value = new ArrayList();
            int rCnt = 0;
            int cCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;

            for (rCnt = startrow; rCnt <= endrow; rCnt++)
            {
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    value.Add(str);
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return value;
        }

        public List<string> ReadExcelColumnWithSize_List(string filename, int startrow, int endrow)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            object str;
            List<string> value = new List<string>();
            int rCnt = 0;
            int cCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;

            for (rCnt = startrow; rCnt <= endrow; rCnt++)
            {
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    value.Add((string)str);
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return value;
        }

        public ArrayList ReadExcelColumnWithRowAndKeyword_ArrayList(string filename, int startrow, int endrow, string keyword)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            object str;
            ArrayList value = new ArrayList();
            int rCnt = 0;
            int cCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;

            for (rCnt = startrow; rCnt <= endrow; rCnt++)
            {
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    if (str != null && str.ToString().Contains(keyword))
                    {
                        value.Add(str);
                    }
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return value;
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        //OLEDB Method - Write
        public void CreateExcelWithIndex(string filename, string tablename, string index)
        {
            System.Data.OleDb.OleDbConnection CN = new System.Data.OleDb.OleDbConnection();
            CN.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties=Excel 8.0";
            CN.Open();
            System.Data.OleDb.OleDbCommand CM = CN.CreateCommand();
            string Command;
            //Create Table                
            Command = string.Format("CREATE TABLE {0} ({1})", tablename, index);
            CM.CommandText = Command;
            CM.ExecuteNonQuery();
            Command = "";
        }

        public void InsertIntoExcel(string filename, string tablename, string word)
        {
            System.Data.OleDb.OleDbConnection CN = new System.Data.OleDb.OleDbConnection();
            CN.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties=Excel 8.0";
            CN.Open();
            System.Data.OleDb.OleDbCommand CM = CN.CreateCommand();
            string Command;
            //Insert to Table                
            Command = string.Format("INSERT INTO [{0}] VALUES ({1})", tablename, word);
            CM.CommandText = Command;
            CM.ExecuteNonQuery();
            CN.Close();
        }

        public void InsertIntoExcel(string filename, string tablename, ArrayList Name, ArrayList Value)
        {
            System.Data.OleDb.OleDbConnection CN = new System.Data.OleDb.OleDbConnection();
            CN.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties=Excel 8.0";
            CN.Open();
            System.Data.OleDb.OleDbCommand CM = CN.CreateCommand();
            string Command;
            //Insert to Table
            int i, length = 0;
            length = Name.Count;
            for (i = 0; i < length; i++)
            {
                Command = string.Format("INSERT INTO [{0}] VALUES ('{1}', '{2}')", tablename, Name[i].ToString(), Value[i].ToString());
                CM.CommandText = Command;
                CM.ExecuteNonQuery();
            }

            CN.Close();
        }

        public void InsertIntoExcel(string filename, string tablename, ArrayList Name, ArrayList Value1, ArrayList Value2)
        {
            System.Data.OleDb.OleDbConnection CN = new System.Data.OleDb.OleDbConnection();
            CN.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties=Excel 8.0";
            CN.Open();
            System.Data.OleDb.OleDbCommand CM = CN.CreateCommand();
            string Command;

            //Insert to Table
            int i, length = 0;
            length = Name.Count;
            for (i = 0; i < length; i++)
            {
                Command = string.Format("INSERT INTO [{0}] VALUES ('{1}', '{2}', '{3}')", tablename, Name[i].ToString(), Value1[i].ToString(), Value2[i].ToString());
                CM.CommandText = Command;
                CM.ExecuteNonQuery();
            }

            CN.Close();
        }

        public void InsertIntoExcel(string filename, string tablename, List<string> Name, List<string> Value1, List<string> Value2)
        {
            System.Data.OleDb.OleDbConnection CN = new System.Data.OleDb.OleDbConnection();
            CN.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties=Excel 8.0";
            CN.Open();
            System.Data.OleDb.OleDbCommand CM = CN.CreateCommand();
            string Command;

            //Insert to Table
            int i, length = 0;
            length = Name.Count;
            for (i = 0; i < length; i++)
            {
                Command = string.Format("INSERT INTO [{0}] VALUES ('{1}', '{2}', '{3}')", tablename, Name[i].ToString(), Value1[i].ToString(), Value2[i].ToString());
                CM.CommandText = Command;
                CM.ExecuteNonQuery();
            }

            CN.Close();
        }

    }
}