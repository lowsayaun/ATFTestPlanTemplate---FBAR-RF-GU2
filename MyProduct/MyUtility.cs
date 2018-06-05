using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Avago.ATF.StandardLibrary;
using Avago.ATF.Shares;
using ICSharpCode.SharpZipLib.Zip;

namespace MyProduct
{
    public struct SMxaSetting
    {
        public double Rbw;
        public double Vbw;
        public double Span;
        public double Attenuation;
        public double RefLevel;
        public int NoPoints;
        public int SweepT;
    }

    public struct SCalSegmSetting
    {
        public string TxCalPath;
        public string TxCalSegm;
        public string Rx1CalPath;
        public string Rx1CalSegm;
        public string Rx2CalPath;
        public string Rx2CalSegm;
        public string AntCalPath;
        public string AntCalSegm;
    }

    public class MyUtility
    {

        public SMxaSetting MxaSetting;
        public SCalSegmSetting CalSegmSetting;

        private const string ConstExStartTxt = "#START";
        private const string ConstExEndTxt = "#END";
        private const string ConstExSkipTxt = "X";
        private const string ConstExLabelTxt = "#LABEL";
        private const string ConstExSegmentEndTxt = "#ENDSEGMENT";
        private const string CosntExTraceEndTxt = "#ENDTRACE";
        private const string ConstExSegmentTxt = "SEGMENT NO";

        private string ExcelGetValue(string sheetName, int iRow, int iColumn)
        {
            string tmpStr = string.Empty;

            

            return tmpStr;
        }

        public void ReadCalSheet(string sheetName, int indexColumnNo, int calParaColumnNo, ref Dictionary<string, string> dicCalInfo)
        {
            string strExInput = "";
            string strExTestItems = "";
            int intRow = 1;
            bool starCalcuteCalNo = false;

            dicCalInfo = new Dictionary<string, string>();

            while (true)
            {
                try
                {
                    strExInput = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, intRow, indexColumnNo);
                }
                catch (Exception)
                {

                    strExInput = "";
                }

                try
                {
                    strExTestItems = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, intRow, calParaColumnNo);
                }
                catch (Exception)
                {

                    strExTestItems = "";
                }
                

                if (strExInput.ToUpper() == ConstExEndTxt)
                {
                    break;
                }
                else if (starCalcuteCalNo && (strExInput.Trim().ToUpper() != ConstExSkipTxt))
                {
                    dicCalInfo.Add(strExInput, strExTestItems);
                }
                else if (strExInput.ToUpper() == ConstExStartTxt)
                {
                    starCalcuteCalNo = true;
                }
                intRow++;
            }
        }
    
        public void ReadTcf(string sheetNo, int indexColumnNo, int testParaColumnNo, ref Dictionary<string, string>[] dicTest)
        {
            string strExInput = "";
            string strExTestItems = "";
            int intRow = 1, intTotalTestNo = 0;
            int testStartRow = 0, intTotaltColumnNo = 0;
            int intTestCount = 0;
            bool starCalcuteTestNo = false;

            #region Calculate Test Parameter
            while (true)
            {
                try
                {
                    strExInput = ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, intRow, indexColumnNo);
                }
                catch (Exception)
                {

                    strExInput = "";
                }
                

                if (strExInput.ToUpper() == ConstExEndTxt)
                {
                    break;
                }
                else if (starCalcuteTestNo && (strExInput.Trim().ToUpper() != ConstExSkipTxt))
                {
                    try
                    {
                        strExTestItems = ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, intRow, testParaColumnNo);
                    }
                    catch (Exception)
                    {

                        strExTestItems = "";
                    }
                    
                    if (strExTestItems.Trim() != "")
                    {
                        intTotalTestNo++;
                    }
                }
                else if (strExInput.ToUpper() == ConstExStartTxt)
                {
                    testStartRow = intRow;
                    starCalcuteTestNo = true;
                }
                intRow++;
            }
            #endregion

            #region Calculate Excel Column
            while (true)
            {
                try
                {
                    strExInput = ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, testStartRow, intTotaltColumnNo);
                }
                catch (Exception)
                {

                    strExInput = "";
                }
                

                if (strExInput.Trim().ToUpper() == ConstExEndTxt)
                {
                    intTotaltColumnNo--;
                    break;
                }
                else
                    intTotaltColumnNo++;
            }
            #endregion

            #region Test Dictionary Generation
            try
            {
                intRow = testStartRow + 1;
                dicTest = new Dictionary<string, string>[intTotalTestNo];

                while (true)
                {
                    try
                    {
                        strExInput = ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, intRow, indexColumnNo).ToUpper();
                    }
                    catch (Exception)
                    {

                        strExInput = "";
                    }

                    try
                    {
                        strExTestItems = ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, intRow, testParaColumnNo).ToUpper();
                    }
                    catch (Exception)
                    {

                        strExTestItems = "";
                    }
                    

                    if (strExInput.ToUpper() == ConstExEndTxt)
                        break;
                    else if (strExInput.Trim().ToUpper() == ConstExSkipTxt)
                    {
                        intRow++;
                        continue;
                    }
                    else
                    {
                        if (strExTestItems.Trim() != "")
                        {
                            dicTest[intTestCount] = new Dictionary<string, string>();

                            string currentTestCondName = "";
                            string currentTestCondValue = "";

                            for (int i = 2; i <= intTotaltColumnNo; i++)
                            {
                                try
                                {
                                    currentTestCondName = ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, testStartRow, i).ToUpper();
                                }
                                catch (Exception)
                                {

                                    currentTestCondName = "";
                                }

                                try
                                {
                                    currentTestCondValue = ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, intRow, i);
                                }
                                catch (Exception)
                                {

                                    currentTestCondValue = "";
                                }
                                
                                if (currentTestCondValue == "")
                                    currentTestCondValue = "0";
                                if (currentTestCondName.Trim() != "")
                                    dicTest[intTestCount].Add(currentTestCondName, currentTestCondValue);
                            }
                            intTestCount++;
                        }
                    }
                    intRow++;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            #endregion
        }
        public void ReadTcf(string sheetName, int indexColumnNo, int testParaColumnNo, ref Dictionary<string, string>[] dicTest, ref Dictionary<string, string> dicLabel)
        {
            string strExInput = "";
            string strExTestItems = "";
            int intRow = 1, intTotalTestNo = 0;
            int testStartRow = 0, intTotaltColumnNo = 0;
            int intTestCount = 0;
            int testLabelRow = 0;
            bool starCalcuteTestNo = false;

            #region Calculate Test Parameter
            while (true)
            {
                try
                {
                    strExInput = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, intRow, indexColumnNo);                    
                }
                catch (Exception)
                {  
                    //meant is blank space - need to force it because of clotho ver 2.2.3 above
                    strExInput = "";
                }

                if (strExInput.ToUpper() == ConstExEndTxt)
                {
                    break;
                }
                else if (starCalcuteTestNo && (strExInput.Trim().ToUpper() != ConstExSkipTxt))
                {
                    try
                    {
                        strExTestItems = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, intRow, testParaColumnNo);
                    }
                    catch (Exception)
                    {

                        strExTestItems = "";
                    }
                    
                    if (strExTestItems.Trim() != "")
                    {
                        intTotalTestNo++;
                    }
                }
                else if (strExInput.ToUpper() == ConstExStartTxt)
                {
                    testStartRow = intRow;
                    starCalcuteTestNo = true;
                }
                else if (strExInput.ToUpper() == ConstExLabelTxt)
                {
                    testLabelRow = intRow;
                }
                intRow++;
            }
            #endregion

            #region Calculate Excel Column
            while (true)
            {
                try
                {
                    strExInput = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, testStartRow, intTotaltColumnNo);
                }
                catch (Exception)
                {
                    //meant is blank space - need to force it because of clotho ver 2.2.3 above
                    strExInput = "";
                }
                

                if (strExInput.Trim().ToUpper() == ConstExEndTxt)
                {
                    intTotaltColumnNo--;
                    break;
                }
                else
                    intTotaltColumnNo++;
            }
            #endregion

            #region Test Dictionary Generation
            try
            {
                intRow = testStartRow + 1;
                dicTest = new Dictionary<string, string>[intTotalTestNo];

                while (true)
                {
                    try
                    {
                        strExInput = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, intRow, indexColumnNo).ToUpper();
                    }
                    catch (Exception)
                    {

                        strExInput = "";
                    }
                    try
                    {
                        strExTestItems = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, intRow, testParaColumnNo).ToUpper();
                    }
                    catch (Exception)
                    {

                        strExTestItems = "";
                    }
                    

                    if (strExInput.ToUpper() == ConstExEndTxt)
                        break;
                    else if (strExInput.Trim().ToUpper() == ConstExSkipTxt)
                    {
                        intRow++;
                        continue;
                    }
                    else
                    {
                        if (strExTestItems.Trim() != "")
                        {
                            dicTest[intTestCount] = new Dictionary<string, string>();

                            string currentTestCondName = "";
                            string currentTestCondValue = "";

                            for (int i = 2; i <= intTotaltColumnNo; i++)
                            {
                                try
                                {
                                    currentTestCondName = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, testStartRow, i).ToUpper();
                                }
                                catch (Exception)
                                {

                                    currentTestCondName = "";
                                }

                                try
                                {
                                    currentTestCondValue = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, intRow, i);
                                }
                                catch (Exception)
                                {

                                    currentTestCondValue = "";
                                }
                                
                                if (currentTestCondValue == "")
                                    currentTestCondValue = "0";
                                if (currentTestCondName.Trim() != "")
                                    dicTest[intTestCount].Add(currentTestCondName, currentTestCondValue);
                            }
                            intTestCount++;
                        }
                    }
                    intRow++;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            #endregion

            #region Parameter Label Generation

            dicLabel = new Dictionary<string, string>();

            string currentLabelCondName = "";
            string currentLabelCondValue = "";

            for (int i = 2; i <= intTotaltColumnNo; i++)
            {
                try
                {
                    currentLabelCondName = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, testStartRow, i).ToUpper();
                }
                catch (Exception)
                {
                    //meant is blank space - need to force it because of clotho ver 2.2.3 above
                    currentLabelCondName = "";
                }
                try
                {
                    currentLabelCondValue = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, testLabelRow, i);
                }
                catch (Exception)
                {
                    //meant is blank space - need to force it because of clotho ver 2.2.3 above
                    currentLabelCondValue = "";
                }
                
                if (currentLabelCondValue == "")
                    currentLabelCondValue = "NA";
                if (currentLabelCondName.Trim() != "")
                    dicLabel.Add(currentLabelCondName, currentLabelCondValue);
            }

            #endregion
        }     
        public void ReadWaveformFilePath (string sheetName, int waveFormColumnNo, ref Dictionary<string, string> dicWaveForm)
        {
            int currentRow = 2;
            dicWaveForm = new Dictionary<string, string>();
            string waveform, waveformFilePath;
            while(true)
            {
                try
                {
                    waveform = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, currentRow, waveFormColumnNo).Trim().ToUpper();
                }
                catch (Exception)
                {

                    waveform = "";
                }

                try
                {
                    waveformFilePath = ATFCrossDomainWrapper.Excel_Get_Input(sheetName, currentRow, waveFormColumnNo + 1).Trim().ToUpper();
                }
                catch (Exception)
                {

                    waveformFilePath = "";
                }
                
                if (waveform.ToUpper() == ConstExEndTxt)
                    break;
                else
                {
                    dicWaveForm.Add(waveform, waveformFilePath);
                    currentRow++;
                }
            }
        }
        public void ReadTcfSegmentTable(string segmentSheetName, string traceSheetName,
            ref LibEqmtDriver.NA.SSegmentTable[] segmentParam, ref LibEqmtDriver.NA.SParam[] sParamData, ref LibEqmtDriver.NA.SStateFile[] stateFiles)
        {
            int channelNumber;
            int totalPoints;
            int totalSegment;
            string tmpStr, preChn = "0";
            int rowNo;
            bool segmentSettings;
            bool segmentTableSettings;
            int channelCnt = 0;
            int totalChannel = 1;
            string sheetName;

            #region Get Segment Table Total Channel

            rowNo = 3;
            sheetName = traceSheetName;
            do
            {
                tmpStr = ReadExcelString(sheetName, rowNo, 1);
                rowNo++;
                if (tmpStr != preChn)
                {
                    preChn = tmpStr;
                    channelCnt++;
                }

            } while (tmpStr.ToUpper() != CosntExTraceEndTxt);

            totalChannel = channelCnt - 1;

            if (((totalChannel < 1) || (rowNo > 40)))
            {
                DisplayError("MyUtility", "Error Total Channel Number",
                    "Total Channel Number Error in Init_Channel() function \r\nTotal Channel = " + totalChannel.ToString() + " (Less then 1!)");
            }

            #endregion


            rowNo = 1;
            sheetName = segmentSheetName;
            segmentSettings = false;
            segmentTableSettings = false;
            totalPoints = 0;
            totalSegment = 0;
            channelNumber = 0;


            segmentParam = new LibEqmtDriver.NA.SSegmentTable[totalChannel];
            stateFiles = new LibEqmtDriver.NA.SStateFile[totalChannel];
            //Init_StateFiles();
            if (sParamData == null)
            {
                sParamData = new LibEqmtDriver.NA.SParam [totalChannel];
            }
            do
            {
                tmpStr = ReadExcelString(sheetName, rowNo, 1);
            

                if (segmentSettings == true)
                {
                    channelNumber = int.Parse(ReadExcelString(sheetName, rowNo, 2)) - 1;
                    segmentParam[channelNumber].Mode = (LibEqmtDriver.NA.naEnum.EModeSetting)int.Parse(ReadExcelString(sheetName, rowNo + 1, 2));
                    segmentParam[channelNumber].Ifbw = (LibEqmtDriver.NA.naEnum.EOnOff)int.Parse(ReadExcelString(sheetName, rowNo + 2, 2));
                    segmentParam[channelNumber].Pow = (LibEqmtDriver.NA.naEnum.EOnOff)int.Parse(ReadExcelString(sheetName, rowNo + 3, 2));
                    segmentParam[channelNumber].Del = (LibEqmtDriver.NA.naEnum.EOnOff)int.Parse(ReadExcelString(sheetName, rowNo + 4, 2));
                    segmentParam[channelNumber].Swp = (LibEqmtDriver.NA.naEnum.EOnOff)int.Parse(ReadExcelString(sheetName, rowNo + 5, 2));
                    segmentParam[channelNumber].Time = (LibEqmtDriver.NA.naEnum.EOnOff)int.Parse(ReadExcelString(sheetName, rowNo + 6, 2));
                    segmentParam[channelNumber].Segm = int.Parse(ReadExcelString(sheetName, rowNo + 7, 2));
                    
                    // Parsing State Files
                    stateFiles[channelNumber].StateFile = ReadExcelString(sheetName, rowNo, 6);
                    stateFiles[channelNumber].LoadState = CStr2Bool(ReadExcelString(sheetName, rowNo, 4));

                    segmentSettings = false;
                }
                if (segmentTableSettings == true)
                {
                    if (totalPoints == 0) segmentParam[channelNumber].SegmentData = new LibEqmtDriver.NA.SSegmentData[segmentParam[channelNumber].Segm];
                    segmentParam[channelNumber].SegmentData[totalSegment].Start = CStr2Val(ReadExcelString(sheetName, rowNo, 2));
                    segmentParam[channelNumber].SegmentData[totalSegment].Stop = CStr2Val(ReadExcelString(sheetName, rowNo, 3));
                    segmentParam[channelNumber].SegmentData[totalSegment].Points = int.Parse(ReadExcelString(sheetName, rowNo, 4));
                    totalPoints += segmentParam[channelNumber].SegmentData[totalSegment].Points;
                    segmentParam[channelNumber].SegmentData[totalSegment].IfbwValue = CStr2Val(ReadExcelString(sheetName, rowNo, 5));
                    segmentParam[channelNumber].SegmentData[totalSegment].PowValue = double.Parse(ReadExcelString(sheetName, rowNo, 6));
                    segmentParam[channelNumber].SegmentData[totalSegment].DelValue = double.Parse(ReadExcelString(sheetName, rowNo, 7));
                    segmentParam[channelNumber].SegmentData[totalSegment].SwpValue = (LibEqmtDriver.NA.naEnum.ESweepMode)int.Parse(ReadExcelString(sheetName, rowNo, 8));
                    segmentParam[channelNumber].SegmentData[totalSegment].TimeValue = double.Parse(ReadExcelString(sheetName, rowNo, 9));
                    totalSegment++;
                    if (totalSegment == segmentParam[channelNumber].Segm) segmentTableSettings = false;
                }


                if (tmpStr.ToUpper() == ConstExStartTxt)
                {
                    segmentSettings = true;
                }
                if (tmpStr.ToUpper() == ConstExEndTxt)
                {
                    sParamData[channelNumber].NoPoints = totalPoints;
                    totalPoints = 0;
                    totalSegment = 0;
                    channelNumber++;
                }
                else if (tmpStr.ToUpper() == ConstExSegmentTxt)
                {
                    segmentTableSettings = true;
                }
                rowNo++;
                if (rowNo > 5000) break; //Force Exit if RowNo more than 5000 lines -> if user forget to add the #Endxxxxxx Statement in worksheet, program will be in infinite loop
            } while (tmpStr.ToUpper() != ConstExSegmentEndTxt);
        }
        public string ReadTcfData(Dictionary<string, string> testPara, string strHeader)
        {
            string temp = "";

            testPara.TryGetValue(strHeader.ToUpper(), out temp);
            return (temp != null ? temp : "");

        }

        public void ReadTcfFbarCalProcedure(string sheetName, ref LibEqmtDriver.NA.SCalibrationProcedure[] procedure, ref bool autoCal, ref string calType)
        {
            int calPortMethod = 0;
            int calPortCounter = 0;
            int channelNo = 0;
            int rowNo, colNo;
            int calProcedureNo;
            int calKitNo;
            int calKitLocation;
            bool found;
            string tmpStr;
            string tmpHeader;

            //cFBAR.cCalibrationClasses.s_CalibrationTotalPort[] CalTotalPort;
            //CalTotalPort = new cFBAR.cCalibrationClasses.s_CalibrationTotalPort[1];

            LibEqmtDriver.NA.naEnum.ECalibrationStandard calStandard;

            Dictionary<string, int> calHeader = new Dictionary<string, int>();

            found = false;
            rowNo = 1;
            colNo = 2;
            calProcedureNo = 0;
            procedure = new LibEqmtDriver.NA.SCalibrationProcedure[1];

            //FBAR.Calibration_Class = new cFBAR.cCalibrationClasses();
            //FBAR.Calibration_Class.Handler = Handler;
            //FBAR.Calibration_Class.CalType = "UOSM"; //Default to UOSM for ZNBT
            do
            {
                tmpStr = ReadExcelString (sheetName, rowNo, 1);

                if (tmpStr.ToUpper() == "MODE")
                {
                    if (ReadExcelString(sheetName, rowNo, 3).ToUpper() == "AUTO")
                        autoCal = true;
                    else
                        autoCal = false;
                }
                else if (tmpStr.ToUpper() == "TYPE")
                {
                    calType = ReadExcelString(sheetName, rowNo, 3);
                }
                else if (tmpStr.ToUpper() == ("#End").ToUpper())
                {
                    found = false;
                }

                if (found)
                {
                    if (calProcedureNo > 0)
                    {
                        Array.Resize(ref procedure, calProcedureNo + 1);
                        //Array.Resize(ref CalTotalPort, cFBAR.TotalChannel);
                    }

                    calKitLocation = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["CALKIT_LocNumber"]));
                    if (calKitLocation > 0)
                    {
                        procedure[calProcedureNo].CKitLocNum = calKitLocation;
                        procedure[calProcedureNo].CKitLabel = ReadExcelString(sheetName, rowNo, calHeader["CALKIT_Label"]);
                    }

                    calKitNo = int.Parse(ReadExcelString (sheetName, rowNo, calHeader["Standard_Number"]));
                    if (calKitNo > 0)
                    {
                        procedure[calProcedureNo].CalKit = calKitNo;
                        procedure[calProcedureNo].BCalKit = true;
                    }
                    procedure[calProcedureNo].ChannelNumber = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Channel"]));
                    if (channelNo != procedure[calProcedureNo].ChannelNumber)
                    {
                        if (channelNo == 0)
                        {
                            channelNo = procedure[calProcedureNo].ChannelNumber;
                        }
                        else
                        {
                            channelNo = procedure[calProcedureNo].ChannelNumber;
                            calPortMethod = 0;
                            calPortCounter = 0;
                        }
                    }

                    //add avg for cal
                    procedure[calProcedureNo].AvgState = (LibEqmtDriver.NA.naEnum.EOnOff)int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Avg_Enable"]));

                    procedure[calProcedureNo].Message = ReadExcelString(sheetName, rowNo, calHeader["Message_Remarks"]);
                    procedure[calProcedureNo].Move = CStr2Bool(ReadExcelString(sheetName, rowNo, calHeader["Move_Handler"]));
                    procedure[calProcedureNo].MoveStep = CStr2int_UniqueHandlerMove(ReadExcelString(sheetName, rowNo, calHeader["Move_Handler"]));
                    calStandard = (LibEqmtDriver.NA.naEnum.ECalibrationStandard)
                        Enum.Parse(typeof(LibEqmtDriver.NA.naEnum.ECalibrationStandard), ReadExcelString(sheetName, rowNo, calHeader["Calibration_Type"]),true);
                    procedure[calProcedureNo].CalStandard = calStandard;
                    procedure[calProcedureNo].Sleep = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Wait [ms]"]));
                    procedure[calProcedureNo].CalType = calType ;
                    procedure[calProcedureNo].NoPorts = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Number_of_Ports"]));
                    switch (calStandard)
                    {
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.ECAL:

                            procedure[calProcedureNo].NoPorts = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Number_of_Ports"]));

                            for (int iPort = 0; iPort < procedure[calProcedureNo].NoPorts; iPort++)
                            {
                                switch (iPort)
                                {
                                    case 0:
                                        procedure[calProcedureNo].Port1 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_1"]));
                                        break;
                                    case 1:
                                        procedure[calProcedureNo].Port2 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_2"]));
                                        break;
                                    case 2:
                                        procedure[calProcedureNo].Port3 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_3"]));
                                        break;
                                    case 3:
                                        procedure[calProcedureNo].Port4 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_4"]));
                                        break;
                                }
                            }
                            break;
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.SUBCLASS:
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.THRU:
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.TRLLINE:
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.TRLTHRU:
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.ISOLATION:
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.UTHRU:
                            procedure[calProcedureNo].Port1 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_1"]));
                            procedure[calProcedureNo].Port2 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_2"]));
                            if (procedure[calProcedureNo].NoPorts == 0)
                            {
                                if (procedure[calProcedureNo].Port1 > calPortMethod)
                                {
                                    calPortMethod = procedure[calProcedureNo].Port1;
                                    //CalTotalPort[ChannelNo - 1].No_Ports = CalPortMethod;
                          
                                }
                                if (procedure[calProcedureNo].Port2 > calPortMethod)
                                {
                                    calPortMethod = procedure[calProcedureNo].Port2;
                                    //CalTotalPort[ChannelNo - 1].No_Ports = CalPortMethod;
                          
                                }
                            }
                            else
                            {
                                //CalTotalPort[ChannelNo - 1].No_Ports = Procedure[CalProcedure_No].No_Ports;
                            }
                            break;

                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.OPEN:
                            procedure[calProcedureNo].Port1 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_1"]));
                            //switch (CalPortCounter)
                            //{
                            //    case 0:
                            //        CalTotalPort[ChannelNo - 1].PortNo_1 = Procedure[CalProcedure_No].Port_1;
                            //        break;
                            //    case 1:
                            //        CalTotalPort[ChannelNo - 1].PortNo_2 = Procedure[CalProcedure_No].Port_1;
                            //        break;
                            //    case 2:
                            //        CalTotalPort[ChannelNo - 1].PortNo_3 = Procedure[CalProcedure_No].Port_1;
                            //        break;
                            //    case 3:
                            //        CalTotalPort[ChannelNo - 1].PortNo_4 = Procedure[CalProcedure_No].Port_1;
                            //        break;
                            //}
                            //CalPortCounter++; //Note : Max Counter only 4 port for every channel
                            break;
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.LOAD:
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.SHORT:
                        case LibEqmtDriver.NA.naEnum.ECalibrationStandard.TRLREFLECT:
                            procedure[calProcedureNo].Port1 = int.Parse(ReadExcelString(sheetName, rowNo, calHeader["Port_1"]));
                            break;
                        default:
                            DisplayError("MyUtility.cs","Loading Calibration Procedure", "Unable to recognized calibration procedure " + (calProcedureNo + 1).ToString() + " : " + calStandard.ToString());
                            //RaiseError_Calibration = true;
                            break;
                    }
                    calProcedureNo++;
                }
                if (tmpStr.ToUpper() == ("#Start").ToUpper())
                {
                    found = true;
                    do
                    {
                        tmpHeader = ReadExcelString(sheetName, rowNo, colNo);
                        if (tmpHeader.Trim() != "")
                        {
                            calHeader.Add(tmpHeader, colNo);
                        }
                        colNo++;
                    } while (tmpHeader.ToUpper() != ("#End").ToUpper());
                }

                rowNo++;
            } while (tmpStr.ToUpper() != ("#End").ToUpper());
      
            //FBAR.Calibration_Class.parse_Procedure = Procedure;
            //FBAR.Calibration_Class.parse_CalTotalPort = CalTotalPort;

        }
        public void ReadTcfFbarTrace(string sheetName, ref LibEqmtDriver.NA.STraceMatching[] TraceMatch)
        {
            int ChannelNumber, ipreChn = 0;
            int PortNumbers;
            string TraceSetting;
            string tmpStr, preChn = "0";
            int totalChannel = 0;
            int totalTrace = 0;
            int rowNo = 3;

            do
            {
                tmpStr = ReadExcelString(sheetName,rowNo ,1);
                rowNo++;
                totalTrace++;
                if (tmpStr != preChn)
                {
                    preChn = tmpStr;
                    totalChannel++;
                }
            }while (tmpStr.ToUpper() != ("#EndTrace").ToUpper());

            totalChannel = totalChannel - 1;
            totalTrace = totalTrace - 1;

            TraceMatch = new LibEqmtDriver.NA.STraceMatching[totalChannel];
            //SParamData = new LibEqmtDriver.NA.SParam[totalChannel];
            
            //SBalanceParamData = new S_CMRRnBal_Param[totalChannel];

            for (int iChn = 0; iChn < totalChannel; iChn++)
            {
                //TraceMatch[iChn].TraceNumber = new int[Enum.GetValues(typeof(LibEqmtDriver.NA.naEnum.ESParametersDef)).Length];
                //TraceMatch[iChn].SParamDefNumber = new int[Enum.GetValues(typeof(LibEqmtDriver.NA.naEnum.ESParametersDef)).Length];
                //for (int iDef = 0; iDef < Enum.GetValues(typeof(LibEqmtDriver.NA.naEnum.ESParametersDef)).Length; iDef++)
                //{
                //    TraceMatch[iChn].TraceNumber[iDef] = -1;
                //    TraceMatch[iChn].SParamDefNumber[iDef] = -1;
                //}
                TraceMatch[iChn].TraceNumber = new int[totalTrace];
                TraceMatch[iChn].SParamDefNumber = new int[totalTrace];
            }


            rowNo = 3;
            int chTotalTrace = 0;
            //traceOffset = 0;
            for (int iTrace = 0; iTrace < totalTrace; iTrace++)
            {
                ChannelNumber = int.Parse(ReadExcelString(sheetName, (iTrace + rowNo), 1));
                if (ipreChn == ChannelNumber)
                {
                    chTotalTrace++;
                    //Array.Resize(ref TraceMatch[ChannelNumber - 1].TraceNumber, chTotalTrace);
                    //Array.Resize(ref TraceMatch[ChannelNumber - 1].SParamDefNumber, chTotalTrace);
                    //TraceMatch[ChannelNumber - 1].TraceNumber[chTotalTrace - 1] = int.Parse(ReadExcelString(sheetName, iTrace + rowNo, 4));
                    //TraceMatch[ChannelNumber - 1].SParamDefNumber[chTotalTrace - 1] = Enum.Parse(typeof(LibEqmtDriver.NA.naEnum.ESParametersDef),
                    //                                                                    ReadExcelString(sheetName, iTrace + rowNo, 5)).GetHashCode();
                }
                else
                {
                    TraceMatch[ChannelNumber - 1].NoPorts = int.Parse(ReadExcelString(sheetName, iTrace + rowNo, 2));
                    ipreChn = ChannelNumber;
                    chTotalTrace = 1;
                }

                Array.Resize(ref TraceMatch[ChannelNumber - 1].TraceNumber, chTotalTrace);
                Array.Resize(ref TraceMatch[ChannelNumber - 1].SParamDefNumber, chTotalTrace);
                TraceMatch[ChannelNumber - 1].TraceNumber[chTotalTrace - 1] = int.Parse(ReadExcelString(sheetName, iTrace + rowNo, 4));
                TraceMatch[ChannelNumber - 1].SParamDefNumber[chTotalTrace - 1] = Enum.Parse(typeof(LibEqmtDriver.NA.naEnum.ESParametersDef),
                                                                                    ReadExcelString(sheetName, iTrace + rowNo, 5)).GetHashCode();

                

            }


            //for (int iRow = 0; iRow < totalChannel; iRow++)
            //{
            //    ChannelNumber = int.Parse(ReadExcelString(sheetName, (iRow + rowNo), 1));
            //    PortNumbers = int.Parse(ReadExcelString(sheetName, (iRow + rowNo), 2));        //Excel Data
            //    //SParamData[iRow].NoPorts = PortNumbers;
            //    TraceSetting = ReadExcelString(sheetName, (iRow + rowNo), 3);                  //Excel Data
                
            //    //SParamData[iRow].SParamData = new LibEqmtDriver.NA.SParamData[Enum.GetValues(typeof(LibEqmtDriver.NA.naEnum.ESParametersDef)).Length];
            //    for(int iDef = 0;iDef < Enum.GetValues(typeof(LibEqmtDriver.NA.naEnum.ESParametersDef)).Length; iDef++)
            //    {
            //        TraceMatch[iRow].TraceNumber[iDef] = int.Parse(ReadExcelString(sheetName, iRow + rowNo, 4)) ;
            //        if (TraceMatch[iRow].TraceNumber[iDef] > 0)
            //            TraceMatch[iRow].SParamDefNumber[iDef] = iDef;
            //    }

                
            //    //Init_TraceMatch((ChannelNumber - 1), PortNumbers, General.convertAutoStr2Bool(TraceSetting));
            //    //int chanTrace = 0;
            //    //Set_ENATrace(ChannelNumber - 1, out chanTrace);
            //    //traceOffset += chanTrace;
            //    //Set_ZnbtGroup(ChannelNumber, PortNumbers);
            //    //Thread.Sleep(1000);
            //    //ENA.Display.Window.Channel_Max(false);
            //}
        }

        public void ReadTcfFbarCalStd(string sheetName,ref LibEqmtDriver.NA.SCalStdTable CalStdTable)
        {
            int ChannelNumber;
            int TotalStd;
            string tmpStr;
            int RowNo;
            bool CalStdSettings;
            bool CalStdTableSettings;
            string CalStdStr = "Define CalStd";

            RowNo = 1;
            CalStdSettings = false;
            CalStdTableSettings = false;
            TotalStd = 0;
            ChannelNumber = 0;

            CalStdTable = new LibEqmtDriver.NA.SCalStdTable();

            do
            {
                tmpStr = ReadExcelString(CalStdStr, RowNo, 1);

                if (CalStdSettings == true)
                {
                    ChannelNumber = (ReadExcelInt(CalStdStr, RowNo, 2));
                    CalStdTable.channelNo = ChannelNumber;
                    CalStdTable.enable = (LibEqmtDriver.NA.naEnum.EOnOff)ReadExcelInt(CalStdStr, RowNo + 1, 2);
                    CalStdTable.calkit_locnum = ReadExcelInt(CalStdStr, RowNo + 2, 2);
                    CalStdTable.calkit_label = ReadExcelString(CalStdStr, RowNo + 3, 2);
                    CalStdTable.total_calstd = ReadExcelInt(CalStdStr, RowNo + 4, 2);
                    CalStdTable.Avg_Mode = ReadExcelString(CalStdStr, RowNo + 5, 2);
                    CalStdTable.Avg_Factor = ReadExcelInt(CalStdStr, RowNo + 6, 2);

                    CalStdSettings = false;
                }
                if (CalStdTableSettings == true)
                {
                    if (TotalStd == 0) CalStdTable.CalStdData = new LibEqmtDriver.NA.SCalStdData[CalStdTable.total_calstd];
                    CalStdTable.CalStdData[TotalStd].StdType = (LibEqmtDriver.NA.naEnum.ECalibrationStandard)Enum.Parse(typeof(LibEqmtDriver.NA.naEnum.ECalibrationStandard), ReadExcelString(CalStdStr, RowNo, 1).ToUpper());
                    CalStdTable.CalStdData[TotalStd].StdLabel = ReadExcelString(CalStdStr, RowNo, 2);
                    CalStdTable.CalStdData[TotalStd].StdNo = ReadExcelInt(CalStdStr, RowNo, 3);
                    CalStdTable.CalStdData[TotalStd].C0_L0 = CStr2Val(ReadExcelString(CalStdStr, RowNo, 4));
                    CalStdTable.CalStdData[TotalStd].C1_L1 = CStr2Val(ReadExcelString(CalStdStr, RowNo, 5));
                    CalStdTable.CalStdData[TotalStd].C2_L2 = CStr2Val(ReadExcelString(CalStdStr, RowNo, 6));
                    CalStdTable.CalStdData[TotalStd].C3_L3 = CStr2Val(ReadExcelString(CalStdStr, RowNo, 7));
                    CalStdTable.CalStdData[TotalStd].OffsetDelay = CStr2Val(ReadExcelString(CalStdStr, RowNo, 8));
                    CalStdTable.CalStdData[TotalStd].OffsetZ0 = CStr2Val(ReadExcelString(CalStdStr, RowNo, 9));
                    CalStdTable.CalStdData[TotalStd].OffsetLoss = CStr2Val(ReadExcelString(CalStdStr, RowNo, 10));
                    CalStdTable.CalStdData[TotalStd].ArbImp = CStr2Val(ReadExcelString(CalStdStr, RowNo, 11));
                    CalStdTable.CalStdData[TotalStd].MinFreq = CStr2Val(ReadExcelString(CalStdStr, RowNo, 12));
                    CalStdTable.CalStdData[TotalStd].MaxFreq = CStr2Val(ReadExcelString(CalStdStr, RowNo, 13));
                    CalStdTable.CalStdData[TotalStd].Media = (LibEqmtDriver.NA.naEnum.ECalStdMedia)Enum.Parse(typeof(LibEqmtDriver.NA.naEnum.ECalStdMedia), ReadExcelString(CalStdStr, RowNo, 14).ToUpper());
                    CalStdTable.CalStdData[TotalStd].LengthType = (LibEqmtDriver.NA.naEnum.ECalStdLengthType)Enum.Parse(typeof(LibEqmtDriver.NA.naEnum.ECalStdLengthType), ReadExcelString(CalStdStr, RowNo, 15).ToUpper());
                    CalStdTable.CalStdData[TotalStd].Port1 = ReadExcelInt(CalStdStr, RowNo, 16);
                    CalStdTable.CalStdData[TotalStd].Port2 = ReadExcelInt(CalStdStr, RowNo, 17);
                    TotalStd++;
                    if (TotalStd == CalStdTable.total_calstd) CalStdTableSettings = false;
                }
                if (tmpStr.ToUpper() == ConstExStartTxt)
                {
                    CalStdSettings = true;
                }
                if (tmpStr.ToUpper() == ConstExEndTxt)
                {
                    TotalStd = 0;
                    ChannelNumber++;
                }
                else if (tmpStr.ToUpper() == ("Std_Type").ToUpper())
                {
                    CalStdTableSettings = true;
                }
                RowNo++;
                if (RowNo > 5000) break; //Force Exit if RowNo more than 5000 lines -> if user forget to add the #Endxxxxxx Statement in worksheet, program will be in infinite loop
            } while (tmpStr.ToUpper() != ("#EndCalStd").ToUpper());
        }

        public string ReadTextFile(string dirpath, string groupName, string targetName)
        {
            string tempSingleString = "NA";
            try
            {
                if (!File.Exists(@dirpath))
                {
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                }
                else
                {
                    using (StreamReader reader = File.OpenText(@dirpath))
                    {
                        string line = "";
                        string[] templine;
                        tempSingleString = "";

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line == "[" + groupName + "]")
                            {
                                char[] temp = { };
                                line = reader.ReadLine();
                                while (line != null && line != "")
                                {
                                    templine = line.ToString()
                                        .Split(new char[] { '=' });
                                    temp = line.ToCharArray();
                                    if (temp[0] == '[' && temp[temp.Length - 1] == ']')
                                        break;
                                    if (templine[0].TrimEnd() == targetName)
                                    {
                                        tempSingleString = templine[templine.Length - 1].ToString().TrimStart();
                                        break;
                                    }
                                    line = reader.ReadLine();
                                }
                                break;
                            }
                        }

                        reader.Close();
                    }
                }
                return tempSingleString;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(dirpath + " " + groupName + " " +
                    targetName + " Cannot Read from the file!");
            }
        }
        public ArrayList ReadCalProcedure(string dirpath)
        {
            ArrayList tempString = new ArrayList();
            try
            {
                if (!File.Exists(@dirpath))
                {
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                }
                else
                {
                    using (StreamReader reader = File.OpenText(@dirpath))
                    {
                        string line = "";
                        tempString.Clear();

                        while ((line = reader.ReadLine()) != null)
                        {
                            tempString.Add(line.ToString());
                        }
                        reader.Close();
                    }
                }
                return tempString;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(dirpath + " Cannot Read from the file!");
            }
        }

        public void CalFileGeneration(string strTargetCalDataFile)
        {
            // Checking and creating a new calibration data file
            DateTime d1 = DateTime.Now;
            StreamWriter swCalDataFile;
            string tempTime = d1.ToString();
            FileInfo fCalDataFile = new FileInfo(strTargetCalDataFile);

            if (fCalDataFile.Exists)
            {
                DialogResult result = MessageBox.Show("The Cal file, " + strTargetCalDataFile + ", exists. Do you want to replace it?", "Debug Mode", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    fCalDataFile.Delete();
                    fCalDataFile = new FileInfo(strTargetCalDataFile);
                    swCalDataFile = fCalDataFile.CreateText();
                    swCalDataFile.WriteLine("1D-Combined" + "," + tempTime);
                    swCalDataFile.Close();
                }
            }
            else
            {
                fCalDataFile = new FileInfo(strTargetCalDataFile);
                swCalDataFile = fCalDataFile.CreateText();
                swCalDataFile.WriteLine("1D-Combined" + "," + tempTime);
                swCalDataFile.Close();
            }
            fCalDataFile = null;

        }
        public void LoadCalFreqList(string strCalFreqList, ref string[] arrCalFreqList, ref int varNumOfCalFreqList)
        {
            // Loading the calibration freq list
            string tempStr;
            FileInfo fCalFreqList = new FileInfo(strCalFreqList);
            StreamReader srCalFreqList = new StreamReader(fCalFreqList.ToString());

            varNumOfCalFreqList = 0;
            while ((tempStr = srCalFreqList.ReadLine()) != null)
            {
                arrCalFreqList[varNumOfCalFreqList] = tempStr.Trim();    //tempStr.Trim();
                varNumOfCalFreqList++;
            }
            srCalFreqList.Close();
        }
        public void LoadMeasEquipCalFactor(string strMeasEquipCalFactor, ref bool varCalDataAvailableMeas)
        {
            // Loading the calibration data for the measurement equipment
            if (strMeasEquipCalFactor.ToUpper().Trim() == "NONE")
                varCalDataAvailableMeas = false;
            else
                varCalDataAvailableMeas = true;
        }
        private void Assign_Cal_File_Combined(string strTargetCalDataFile, ref StreamWriter swCalDataFile)
        {
            // Checking and creating a new calibration data file
            FileInfo fCalDataFile = new FileInfo(strTargetCalDataFile);
            swCalDataFile = fCalDataFile.AppendText();

        }
        public void LoadSourceData(string strTargetCalDataFile, string strSourceEquipCalFactor, string[] arrFreqList, ref string[] arrCalDataSource, ref bool varCalDataAvailableSource, ref StreamWriter swCalDataFile)
        {
            string errInformation = "";
            float calFactor = 0f;
            int varNumOfCalFreqList = 0;

            // Loading the calibration data for the source equipment
            if (strSourceEquipCalFactor.ToUpper().Trim() == "NONE")
                varCalDataAvailableSource = false;
            else
            {
                varCalDataAvailableSource = true;
                varNumOfCalFreqList = 0;
                try
                {
                    swCalDataFile.Close();
                }
                catch { }

                ATFCrossDomainWrapper.Cal_LoadCalData("CalData1D_", strTargetCalDataFile);

                try
                {
                    Assign_Cal_File_Combined(strTargetCalDataFile, ref swCalDataFile);
                }
                catch { }

                ATFCrossDomainWrapper.Cal_GetCalData1DCombined("CalData1D_", strSourceEquipCalFactor, Convert.ToSingle(arrFreqList[varNumOfCalFreqList]), ref calFactor, ref errInformation);
                while (arrFreqList[varNumOfCalFreqList] != null)
                {
                    ATFCrossDomainWrapper.Cal_GetCalData1DCombined("CalData1D_", strSourceEquipCalFactor, Convert.ToSingle(arrFreqList[varNumOfCalFreqList]), ref calFactor, ref errInformation);
                    arrCalDataSource[varNumOfCalFreqList] = calFactor.ToString(); ;
                    varNumOfCalFreqList++;
                }
                try
                {
                    ATFCrossDomainWrapper.Cal_ResetAll();
                }
                catch { }

            }
        }

        public bool SNP_SDI_Compression(string ProductTag, string source_directory, string sdi_inbox_wave)
        {
            bool bOK = false;
            
            

            try
            {
                string tmpStr = source_directory.Remove(source_directory.LastIndexOf("\\"));
                tmpStr = source_directory.Remove(tmpStr.LastIndexOf("\\"));

                if (Directory.Exists(source_directory))
                {
                    string[] waveTraceFolders = Directory.GetDirectories(tmpStr , ProductTag + "*", SearchOption.TopDirectoryOnly);

                    string tempFolderName = string.Empty;
                    string tempFileName = string.Empty;

                    foreach (string waveTraceFolder in waveTraceFolders)
                    {
                        tempFolderName = waveTraceFolder.Substring(waveTraceFolder.LastIndexOf("\\") + 1, waveTraceFolder.Length - 1 - waveTraceFolder.LastIndexOf("\\"));

                        tempFileName = sdi_inbox_wave  + tempFolderName + ".fmtrace";

                        if (File.Exists(tempFileName))
                        {
                            File.Delete(tempFileName);
                        }

                        //SDI
                        using (ZipFile zip = ZipFile.Create(tempFileName))
                        {
                            zip.NameTransform = new ZipNameTransform(waveTraceFolder.Replace(tempFolderName, ""));

                            string[] files = Directory.GetFiles(waveTraceFolder);

                            zip.BeginUpdate();

                            foreach (string file in files)
                            {
                                zip.Add(file, CompressionMethod.Deflated);
                            }
                            zip.CommitUpdate();
                        }

                        Directory.Delete(waveTraceFolder, true);
                    }
                    bOK = true;
                }
            }
            catch {
                DisplayError(this.ToString(), "Possible S Parameter Trace File",
                            "S Parameter Trace File Compress Error for " + ProductTag.ToString() + " mismatch!!!");
            }
            return bOK;
        }
        public void Generate_SNP_Header(string FilePath, List<string> Contains)
        {
            int i = 0,
                j = 0;
            StreamWriter writer = null;
            //IO_TextFile IO_TXT = new IO_TextFile();
            //IO_TXT.CreateFileInDirectory(FilePath);
            try
            {
                writer = File.CreateText(FilePath);
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException("Cannot Create file! " + e.Message);
            }
            finally
            {
                writer.Close();
            }


            j = Contains.Count;

            for (i = 0; i < j; i++)
            {
                //IO_TXT.WriteNewLineToExistTextFile(FilePath, Contains[i]);
                try
                {
                    if (!File.Exists(FilePath))
                        throw new FileNotFoundException("{0} does not exist."
                            , @FilePath);
                    else
                    {
                        using (writer = File.AppendText(@FilePath))
                        {
                            writer.Write(Contains[i]);
                            writer.WriteLine();
                            writer.Close();
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    throw new FileNotFoundException("Cannot Write/Append the file!");
                }
            }
        }

        public void Decode_MXA_Setting(string mxaData)
        {
            string[] tempdata1;
            string[] tempData2;

            MxaSetting = new SMxaSetting();

            tempdata1 = mxaData.Split(';');

            for (int i = 0; i < 7; i++)
            {
                tempData2 = tempdata1[i].Split('@');

                switch (tempData2[0].ToUpper())
                {
                    case "RBW":
                        MxaSetting.Rbw = Convert.ToDouble(tempData2[1]);
                        break;
                    case "VBW":
                        MxaSetting.Vbw = Convert.ToDouble(tempData2[1]);
                        break;
                    case "SPAN":
                        MxaSetting.Span = Convert.ToDouble(tempData2[1]);
                        break;
                    case "ATTN":
                        MxaSetting.Attenuation = Convert.ToDouble(tempData2[1]);
                        break;
                    case "REFLVL":
                        MxaSetting.RefLevel = Convert.ToDouble(tempData2[1]);
                        break;
                    case "NOPOINTS":
                        MxaSetting.NoPoints = Convert.ToInt32(tempData2[1]);
                        break;
                    case "SWEEPT":
                        MxaSetting.SweepT = Convert.ToInt16(tempData2[1]);
                        break;
                }
            }
        }
        public void Decode_CalSegm_Setting(string calSegmData)
        {
            string[] tempdata1;
            string[] tempData2;

            CalSegmSetting = new SCalSegmSetting();

            tempdata1 = calSegmData.Split(';');

            for (int i = 0; i < tempdata1.Length; i++)
            {
                tempData2 = tempdata1[i].Split('@');

                switch (tempData2[0].ToUpper())
                {
                    case "TX":
                        CalSegmSetting.TxCalPath = tempData2[0].ToUpper();
                        CalSegmSetting.TxCalSegm = tempData2[1].ToUpper();
                        break;
                    case "MXA1":
                        CalSegmSetting.Rx1CalPath = tempData2[0].ToUpper();
                        CalSegmSetting.Rx1CalSegm = tempData2[1].ToUpper();
                        break;
                    case "MXA2":
                        CalSegmSetting.Rx2CalPath = tempData2[0].ToUpper();
                        CalSegmSetting.Rx2CalSegm = tempData2[1].ToUpper();
                        break;
                    case "ANT":
                        CalSegmSetting.AntCalPath = tempData2[0].ToUpper();
                        CalSegmSetting.AntCalSegm = tempData2[1].ToUpper();
                        break;
                }
            }
        }

        private void DisplayError(string className, string errParam, string errDesc)
        {
            MessageBox.Show("Class Name: " + className + "\nParameters: " + errParam + "\n\nErrorDesciption: \n"
                + errDesc, "Error found in Class " + className, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
        private void DisplayMessage(string className, string descParam, string descDetail)
        {
            MessageBox.Show("Class Name: " + className + "\nParameters: " + descParam + "\n\nDesciption: \n"
                + descDetail, className, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public bool CStr2Bool(string input)
        {
            if (input.Trim() == "1" || input.ToUpper().Trim() == "YES" || input.ToUpper().Trim() == "ON" || input.ToUpper().Trim() == "V")
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
        public double CStr2Val(string input)
        {
            string[] tmpStr;
            double tmpVal;
            string tmpChar;
            tmpStr = input.Split(' ');

            tmpVal = Convert.ToDouble(tmpStr[0]);

            if (tmpStr.Length < 2)
            {
                return (Convert.ToDouble(tmpStr[0]));
            }
            else
            {
                if (tmpStr[1].Length == 1)
                {
                    tmpChar = tmpStr[1];
                }
                else
                {
                    tmpChar = tmpStr[1].Substring(0, 1);
                }
                switch (tmpChar)
                {
                    case "G":
                        return (Math.Round(tmpVal * 1000000000));
                    case "M":
                        return (Math.Round(tmpVal * Math.Pow(10, 6)));
                    case "K":
                    case "k":
                        return (Math.Round(tmpVal * Math.Pow(10, 3)));
                    case "m":
                        return ((tmpVal * Math.Pow(10, -3)));
                    case "u":
                        return ((tmpVal * Math.Pow(10, -6)));
                    case "n":
                        return ((tmpVal * Math.Pow(10, -9)));
                    case "p":
                        return ((tmpVal * Math.Pow(10, -12)));
                    default:
                        return (tmpVal);
                }
            }
        }

        private string ReadExcelString (int sheetNo, int rowNo, int column)
        {
            try
            {
                return ATFCrossDomainWrapper.Excel_Get_Input(sheetNo, rowNo, column);
            }
            catch
            {
                return "0";
            }

        }
        private string ReadExcelString(string sheetName, int rowNo, int column)
        {
            try
            {
                return ATFCrossDomainWrapper.Excel_Get_Input(sheetName, rowNo, column);
            }
            catch
            {
                return "0";
            }

        }
        private int ReadExcelInt(string sheetName, int rowNo, int column)
        {
            try
            {
                return int.Parse(ATFCrossDomainWrapper.Excel_Get_Input(sheetName, rowNo, column));
            }
            catch
            {
                return -1;
            }
        }
        
        private int CStr2int_UniqueHandlerMove(string input)
        {
            try
            {
                int test = int.Parse(input);
                return test;
            }
            catch
            {
                if (input.Trim() == "1" || input.ToUpper().Trim() == "YES" || input.ToUpper().Trim() == "ON" || input.ToUpper().Trim() == "V")
                {
                    return (1);
                }
                else
                {
                    return (0);
                }
            }
        }
    }
}
