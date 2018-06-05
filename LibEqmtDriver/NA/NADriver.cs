using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace LibEqmtDriver.NA
{
    public abstract class NetworkAnalyzerAbstract : LibEqmtCommon
    {
        private string ClassName = "NetworkAnalyzerAbstract class";
        private static bool _swUse;
        private static bool _handlerUse;
        internal int IntTotalChannel;
        internal LibMath.MathLib objMath = new LibMath.MathLib();
        internal LibMath.MathLib.DataType objMathData;

        public bool switchUse
        {
            get { return _swUse; }
            set { _swUse = value; }
        }
        public bool handlerUse
        {
            get { return _handlerUse; }
            set { _handlerUse = value; }
        }

        public void UnInit() { }
        public virtual void LoadState(string filename) { }
        public virtual void SaveState(string filename) { }

        public virtual void DisplayOn(bool state) { }

        public virtual void Calibrate(SCalibrationProcedure[] CalProcedure, string stateFile)
        {
            int tmpChannelNo = -1;
            int totalChannel = 0;
            string tmpLabel;
            bool CalKit_FailCheck = false;
            bool bNext;

            Reset();
            waitTimer.Wait(500);
            LoadState(stateFile);
            waitTimer.Wait(5000);

            for (int i = 0; i < CalProcedure.Length; i++)
            {
                if (tmpChannelNo != CalProcedure[i].ChannelNumber)
                {

                    SetCalKitLabel(CalProcedure[i].ChannelNumber, CalProcedure[i].CKitLabel);
                    waitTimer.Wait(30);
                    //Only verify the 1st OPEN cal statement only. Must ensure that your Cal Kit Label define correctly for this row
                    if (naEnum.ECalibrationStandard.OPEN == CalProcedure[i].CalStandard)
                    {
                        if (CalProcedure[i].CKitLocNum != 0) //Only check if user define the cal kit location number, undefine will assume no check required
                        {
                            char[] trimChar = { '\"', '\n' };
                            tmpLabel = GetCalKitLabel(CalProcedure[i].ChannelNumber);
                            tmpLabel = tmpLabel.Trim(trimChar);
                            if (tmpLabel != CalProcedure[i].CKitLabel)
                            {
                                CalKit_FailCheck = true;  // set flag to true, cal program will not proceed if flag true
                                DisplayError(ClassName, "Error Cal Kit Verification", "Unrecognize ENA CalKit Label = " + tmpLabel + '\n' +
                                    "Define Cal Kit Label in config file = " + CalProcedure[i].CKitLabel + '\n' +
                                    "Please checked your configuration file !!!!!" + '\n' +
                                    " ***** Calibration Procedure will STOP and EXIT *****");
                            }
                        }
                        tmpChannelNo = CalProcedure[i].ChannelNumber;
                        totalChannel++;
                    }
                }
            }
            if (!CalKit_FailCheck)
            {
                string tempDef = GetTraceInfo(1);
                string[] parts = (tempDef.Split(','));

                DisplayMessage(ClassName + " --> ", "Start Calibration", "Start Calibration");
                int ChannelNo = -1;
                int pStep = 0;
                for (int iCal = 0; iCal < CalProcedure.Length; iCal++)
                {
                    //on the ZNBT averaging
                    if (ChannelNo != -1 && ChannelNo == CalProcedure[iCal].ChannelNumber)
                        Averaging(CalProcedure[iCal].ChannelNumber, CalProcedure[iCal].AvgState);

                    string tmpStr;
                    do
                    {
                        bNext = false;


                        if (_handlerUse)
                        {
                            if (CalProcedure[iCal].Move)
                            {
                                // put in PnP handler movement control code
                            }
                        }
                        if (_swUse)
                        {
                            //do switching for PAD
                        }
                        #region "Calibration Message"
                        if (CalProcedure[iCal].Message.Trim() != "" & CalProcedure[iCal].Message.Trim() != "0")
                        {
                            if (pStep > 0 | iCal > 0)
                            {
                                switch (MessageBox.Show("Do you want to re-measure STD?\n" + CalProcedure[pStep].Message, "Penang NPI", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    case DialogResult.Cancel:
                                        bNext = true;
                                        break;
                                    case DialogResult.Yes:
                                        bNext = false;
                                        iCal = pStep;
                                        break;
                                }
                            }
                            tmpStr = CalProcedure[iCal].Message;
                            pStep = iCal;
                            DisplayMessage(ClassName + " --> ", CalProcedure[iCal].CalType.ToString() + " Calibration (Cal Kit)", tmpStr);
                        }
                        else
                        {
                            tmpStr = "Calibrating " + CalProcedure[iCal].CalType.ToString() + " (Cal Kit) procedure for : "
                                            + "\r\n\tChannel Number : " + CalProcedure[iCal].ChannelNumber.ToString() + "\r\n\t";
                            switch (CalProcedure[iCal].CalStandard)
                            {
                                case naEnum.ECalibrationStandard.ECAL:
                                    for (int iPort = 0; iPort < CalProcedure[iCal].NoPorts; iPort++)
                                    {
                                        switch (iPort)
                                        {
                                            case 0:
                                                tmpStr += "Port " + CalProcedure[iCal].Port1.ToString();
                                                break;
                                            case 1:
                                                tmpStr += "," + CalProcedure[iCal].Port2.ToString();
                                                break;
                                            case 2:
                                                tmpStr += "," + CalProcedure[iCal].Port3.ToString();
                                                break;
                                            case 3:
                                                tmpStr += "," + CalProcedure[iCal].Port4.ToString();
                                                break;

                                        }
                                    }
                                    break;
                                case naEnum.ECalibrationStandard.ISOLATION:
                                case naEnum.ECalibrationStandard.THRU:
                                case naEnum.ECalibrationStandard.TRLLINE:
                                case naEnum.ECalibrationStandard.TRLTHRU:
                                    tmpStr += "Port " + CalProcedure[iCal].Port1.ToString() + "," + CalProcedure[iCal].Port2.ToString();
                                    break;
                                case naEnum.ECalibrationStandard.LOAD:
                                case naEnum.ECalibrationStandard.OPEN:
                                case naEnum.ECalibrationStandard.SHORT:
                                case naEnum.ECalibrationStandard.SUBCLASS:
                                case naEnum.ECalibrationStandard.TRLREFLECT:
                                    tmpStr += "Port " + CalProcedure[iCal].Port1.ToString();
                                    break;
                            }
                        }

                        #endregion

                        waitTimer.Wait(CalProcedure[iCal].Sleep);

                        if (CalProcedure[iCal].BCalKit)
                        {
                            #region "Cal Kit Procedure"

                            if (ChannelNo != CalProcedure[iCal].ChannelNumber)
                            {
                                if (ChannelNo != -1)
                                {
                                    SaveCorr(ChannelNo);
                                    waitTimer.Wait(500);
                                }
                                ChannelNo = CalProcedure[iCal].ChannelNumber;
                                
                                ActiveChannel(ChannelNo);
                                SetCorrProperty(ChannelNo, false);
                                ClearCorr(ChannelNo);//ENA only
                                ChannelMax(ChannelNo, true);//ENA only
                                waitTimer.Wait(500);
                                SelectSubClass(CalProcedure[iCal].ChannelNumber);
                                Operation_Complete();
                                SetCalKitLabel(CalProcedure[iCal].ChannelNumber, CalProcedure[iCal].CKitLabel);
                                Operation_Complete();
                                SetCorrMethodSOLT(CalProcedure[iCal].ChannelNumber, GetCalPorts(ChannelNo, CalProcedure));

                                waitTimer.Wait(500);
                                Operation_Complete();

                            }


                            switch (CalProcedure[iCal].CalStandard)
                            {
                                case naEnum.ECalibrationStandard.OPEN:
                                case naEnum.ECalibrationStandard.SHORT:
                                case naEnum.ECalibrationStandard.LOAD:

                                    MeasCorr1PortStd(CalProcedure[iCal].ChannelNumber, CalProcedure[iCal].Port1, CalProcedure[iCal].CalKit, CalProcedure[iCal].CalStandard.ToString());

                                    waitTimer.Wait(CalProcedure[iCal].Sleep);
                                    Operation_Complete();

                                    break;

                                case naEnum.ECalibrationStandard.THRU:
                                case naEnum.ECalibrationStandard.TRLLINE:
                                case naEnum.ECalibrationStandard.TRLREFLECT:
                                case naEnum.ECalibrationStandard.TRLTHRU:
                                case naEnum.ECalibrationStandard.UTHRU:

                                    MeasCorr2PortStd(CalProcedure[iCal].ChannelNumber, CalProcedure[iCal].Port1, CalProcedure[iCal].Port2, CalProcedure[iCal].CalKit, CalProcedure[iCal].CalStandard.ToString());

                                    waitTimer.Wait(CalProcedure[iCal].Sleep);
                                    Operation_Complete();

                                    break;

                                default:
                                    DisplayError(ClassName, "Error in Normal Calibration Procedure", "Unrecognize Calibration Procedure = " + CalProcedure[iCal].CalType.ToString() + " for Cal Kit Standard " + CalProcedure[iCal].CalKit.ToString());
                                    break;
                            }
                            #endregion

                        }

                        waitTimer.Wait(500);
                        bNext = true;

                        if (CalProcedure.Length - 1 == iCal)
                        {
                            switch (MessageBox.Show("Do you want to re-measure STD?\n" + CalProcedure[pStep].Message, "Penang NPI", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                case DialogResult.No:
                                    bNext = true;
                                    break;
                                case DialogResult.Yes:
                                    bNext = false;
                                    iCal = pStep;
                                    break;
                            }
                        }                   
                        

                    } while (!bNext);

                    Averaging(ChannelNo, naEnum.EOnOff.Off);
                    SetCorrProperty(ChannelNo, true);
                    ChannelMax(ChannelNo, false);


                }
                //for (int i = 0; i < totalChannel; i++)
                //{
                    SaveCorr(ChannelNo);
                //}
                DisplayOn(true);
            }
            DisplayMessage(ClassName + " --> ", "Calibration Completed", "Calibration Complete");
        }

        public virtual void Averaging(int channelNumber, naEnum.EOnOff val) { }
        public virtual void AveragingMode(int channelNumber, string val) { }
        public virtual void AveragingFactor(int channelNumber, int val) { }
        public virtual void ActiveChannel(int channelNumber) { }
        public virtual void ChannelMax(int channelNumber, bool state) { }

        //Correction Method
        #region Correction

        public virtual void SetCorrProperty(int channelNumber, bool enable) { }
        public virtual void ClearCorr(int channelNumber) { }
        public virtual void SelectSubClass(int channelNumber) { }
        public virtual void SetCorrMethodSOLT(int channelNUmber, List<int> numberPorts) { }
        public virtual void MeasCorr1PortStd(int channelNumber, int portNumber, int stdNumber, string stdType) { }
        public virtual void MeasCorr2PortStd(int channelNumber, int port1, int port2, int stdNumber, string stdType) { }
        public virtual void SaveCorr(int channelNumber) { }
        #endregion

        //Source Function
        public virtual void SetPower(float level){}

        //Sense function
        #region Sense Function
        public virtual void SetFreqStart(int channelNumber, double Freq)
        { 
            SendCommand("SENS" + channelNumber.ToString() + ":FREQ:STAR " + Freq.ToString()); 
        }
        public virtual void SetFreqStop(int channelNumber, double Freq) 
        {
            SendCommand("SENS" + channelNumber.ToString() + ":FREQ:STOP " + Freq.ToString());
        }
        public virtual void SetFreqBw(int channelNumber, double BW) 
        {
            SendCommand("SENS" + channelNumber.ToString() + ":BAND " + BW.ToString());
        }

        public virtual void SetSwePoint(int channelNumber, int points) 
        {
            SendCommand("SENS" + channelNumber.ToString() + ":SWE:POIN " + points.ToString());
        }
        public virtual void SetSweType(int channelNumber, naEnum.ESweepType sweepType) 
        {
            SendCommand("SENS" + channelNumber.ToString() + ":SWE:TYPE " + sweepType.ToString());
        }
        #endregion


        //Trigger Function
        public virtual void TriggerSingle(int channelNumber) { }
        public virtual void TriggerMode(naEnum.ETriggerMode trigMode) { }
        public virtual void TriggerSource(naEnum.ETriggerSource trigSource) { }

        public virtual string GetTraceInfo(int channelNumber)
        {
            return string.Empty;
        }
        public virtual naEnum.ESFormat GetTraceFormat(int channelNumber, int traceNumber)
        {
            return naEnum.ESFormat.MLOG;
        }
        public virtual double[] GetFreqList(int channelNumber)
        {
            return new double[] { };
        }

        //Grab Data
        public virtual double[] GrabRealImagData(int channelNumber)
        {
            return new double[] { };
        }

        public SParam GrabSParamRiData(int channelNumber)
        {
            int offSet = 0;
            int sParamDef;
            string tmpVar;
            string[] arrTrace, traces;
            string[] traceMap;
            double[] rawData;
            SParam sparamData = new SParam();
            naEnum.ESFormat traceFormat;

            rawData = GrabRealImagData(channelNumber);
            
            tmpVar = GetTraceInfo(channelNumber);
            tmpVar = tmpVar.Replace("'", "").Replace("\n", "").Trim();
            arrTrace = tmpVar.Split(new char[] { ',' });

            //Get only odd number
            traces = arrTrace.Where((item, index) => index % 2 != 0).ToArray();
            traceMap = arrTrace.Where((item, index) => index % 2 == 0).ToArray(); //new int[traces.Length];

            
            double[] fList = GetFreqList(channelNumber);
            sparamData.Freq = fList;
            sparamData.NoPoints = fList.Length;
            int Basepoint = fList.Length;
            sparamData.SParamData = new SParamData[traces.Length];

            for (int i = 0; i < traces.Length; i++)
            {
                sParamDef = (int.Parse(traceMap[i].Replace("Trc", "")) - 1);
                traceFormat = GetTraceFormat(channelNumber, sParamDef + 1);
                offSet = i * Basepoint * 2;

                sparamData.SParamData[i].SParam = new LibMath.MathLib.DataType[Basepoint];
                sparamData.SParamData[i].Format = traceFormat;
                sparamData.SParamData[i].SParamDef = (naEnum.ESParametersDef)Enum.Parse(typeof(naEnum.ESParametersDef), traces[i]);

                for (int iPts = 0; iPts < Basepoint; iPts++)
                {

                    if (traceFormat == naEnum.ESFormat.GDEL)
                    {
                        sparamData.SParamData[i].SParam[iPts].DB = rawData[offSet + iPts];
                        sparamData.SParamData[i].SParam[iPts].Phase = 0;
                    }
                    else
                    {
                        objMathData.Real = rawData[offSet + (iPts * 2)];
                        objMathData.Imag = rawData[offSet + (iPts * 2) + 1];

                        objMath.conv_RealImag_to_dBAngle(ref objMathData);

                        sparamData.SParamData[i].SParam[iPts].Real = objMathData.Real;
                        sparamData.SParamData[i].SParam[iPts].Imag = objMathData.Imag;
                        sparamData.SParamData[i].SParam[iPts].DB = objMathData.DB;
                        sparamData.SParamData[i].SParam[iPts].Mag = objMathData.Mag;
                        sparamData.SParamData[i].SParam[iPts].Phase = objMathData.Phase;
                    }

                }

            }
            return sparamData;

        }

        //Setup Cal-kit
        public void SetupCalKit(SCalStdTable stdTable)
        {
            InsertCalKitStd(stdTable);
        }

        //Setup NA state file
        public void SetupTrace(int channelNumber, STraceMatching TraceMatching) 
        {
            SortedList<int, String> Traces = new SortedList<int, string>();
         
            ActiveChannel(channelNumber);
            ChannelMax(channelNumber, true);
            
            for (int i = 0; i < TraceMatching.TraceNumber.Length;i++ )
            {
                if (TraceMatching.SParamDefNumber[i] > -1)
                {
                    Traces.Add(TraceMatching.TraceNumber[i], Enum.GetName(typeof(naEnum.ESParametersDef), TraceMatching.SParamDefNumber[i]));
                }
            }
               
            InsertTrace(channelNumber, Traces);
            waitTimer.Wait(500);
        }
        
        public void SetupSegmentTable(SSegmentTable[] segmentTable)
        {  
            SetPower(0);//Set Power to 0 for ZNB
            waitTimer.Wait(500);
            for (int iChn = 0; iChn < segmentTable.Length ; iChn++)
            {
                 
                //if (segmentTable[iChn].Segm == 1)
                //{
                //    SetFreqStart(iChn + 1, segmentTable[iChn].SegmentData[0].Start);
                //    SetFreqStop (iChn + 1, segmentTable[iChn].SegmentData[0].Stop);
                //    SetSwePoint(iChn + 1, segmentTable[iChn].SegmentData[0].Points);
                //    SetFreqBw(iChn + 1, segmentTable[iChn].SegmentData[0].IfbwValue);
                //    SetSweType(iChn + 1, naEnum.ESweepType.LIN);
                //}
                //else
                //{
                    InsertSegmentTableData(iChn + 1, segmentTable[iChn]);
                    SetSweType(iChn + 1, naEnum.ESweepType.SEGM);
                //}
            }
        }
        public virtual void GetSegmentTable(out SSegmentTable segmentTable, int channelNumber)
        {
            segmentTable = new SSegmentTable();
        }
        public void ExportSnpFile(SParam sparamData, string FolderName, string FileName, string Unit, int iChn)
        {
            string[] OutputData;
            string OutputFileName;

            if (!Directory.Exists(FolderName))
                Directory.CreateDirectory(FolderName);


            OutputFileName = FolderName + "Unit" + Unit + "_Chan" + (iChn).ToString() + "_" + FileName;
            OutputData = new string[sparamData.Freq.Length + 3];
            OutputData[0] = "#\tHZ\tS\tdB\tR 50";
            OutputData[1] = "!\t" + DateTime.Now.ToShortDateString() + "\t" + DateTime.Now.ToLongTimeString();
            OutputData[2] = "!Freq\t";

            for (int i = 0; i < sparamData.SParamData.Length; i++)
            {
                OutputData[2] += sparamData.SParamData[i].SParamDef.ToString() + "\t\t";  
                for (int iPoint = 0; iPoint < sparamData.SParamData[i].SParam.Length; iPoint++)
                {
                    if (i == 0)
                        OutputData[iPoint + 3] += sparamData.Freq[iPoint].ToString();
                    OutputData[iPoint + 3] += "\t" + sparamData.SParamData[i].SParam[iPoint].DB +
                                             "\t" + sparamData.SParamData[i].SParam[iPoint].Phase;
                }
            }
            System.IO.File.WriteAllLines(OutputFileName + ".snp", OutputData);
        }

        public SParam ConvertTourchStone(STraceMatching TraceMatch, SParam sParamData)
        {
            SParam tsSparam = new SParam();

            int totalport = TraceMatch.NoPorts;
            int totalsparam = 1;
            int iCount = 0, i, x;
            //for (int counter = 1 ;counter <= totalport;counter ++)
            //{
            totalsparam = totalport * totalport;
            //}

            tsSparam.SParamData = new SParamData[totalsparam];
            tsSparam.Freq = sParamData.Freq;
            tsSparam.NoPoints = sParamData.NoPoints;

            for (i = 1; i <= totalport; i++)
            {
                for (x = 1; x <= totalport; x++)
                {
                    tsSparam.SParamData[iCount].SParamDef = (naEnum.ESParametersDef)
                        Enum.Parse(typeof(naEnum.ESParametersDef), "S" + i + x);
                    tsSparam.SParamData[iCount].SParam = new LibMath.MathLib.DataType[tsSparam.Freq.Length];
                    iCount++;
                }
            }

            for (i = 0; i < sParamData.SParamData.Length;i++)
            {
                for (x = 0;x<tsSparam.SParamData.Length;x++)
                {
                    if (tsSparam.SParamData[x].SParamDef == sParamData.SParamData[i].SParamDef)
                        tsSparam.SParamData[x].SParam = sParamData.SParamData[i].SParam;
                }
            }

            return tsSparam;
        }


        internal virtual void SetCalKitLabel(int channelNumber, string label) { }
        internal virtual string GetCalKitLabel(int channelNumber) { return string.Empty; }
        internal virtual void InsertTrace(int channelNumber, SortedList<int, String> Traces) { }
        internal virtual void InsertSegmentTableData(int channelNumber, SSegmentTable segmentTable) { }
        internal virtual void InsertCalKitStd(SCalStdTable stdTable){}

        private List<int> GetCalPorts(int ChannelNo, SCalibrationProcedure[] calProd)
        {
            //int[] result = new int[calProd.Length];

            List<int> calports = new List<int>();

            for (int i = 0; i < calProd.Length; i++)
            {
                if (!calports.Contains(calProd[i].Port1) & calProd[i].ChannelNumber == ChannelNo)
                {
                    calports.Add(calProd[i].Port1);
                }
            }

            return calports;
        }
    }
    public struct SCalibrationProcedure
    {
        public string CalType;
        public naEnum.ECalibrationStandard CalStandard;
        public naEnum.EOnOff AvgState;
        public int ChannelNumber;
        public int CKitLocNum;
        public string CKitLabel;
        public int NoPorts;
        public int Port1;
        public int Port2;
        public int Port3;
        public int Port4;
        public int Port5;
        public int Port6;
        public int Port7;
        public int Port8;
        public int Port9;
        public int Port10;
        public int Port11;
        public int Port12;
        public int Port13;
        public int Port14;
        public int Port15;
        public int Port16;
        public int Port17;
        public int Port18;
        public int Port19;
        public int Port20;
        public int Port21;
        public int Port22;
        public int Port23;
        public int Port24;
        public int CalKit;
        public bool BCalKit;
        public string Message;
        public int Sleep;
        public bool Move;
        public int MoveStep;
    }
    public struct SCalStdTable
    {
        public int channelNo;
        public naEnum.EOnOff enable;
        public int calkit_locnum;
        public string calkit_label;
        public int total_calstd;
        public SCalStdData[] CalStdData;
        public naEnum.EOnOff Avg_Enable;
        public string Avg_Mode;
        public int Avg_Factor;
    }
    public struct SCalStdData
    {
        public naEnum.ECalibrationStandard StdType;
        public string StdLabel;
        public int StdNo;
        public double C0_L0;
        public double C1_L1;
        public double C2_L2;
        public double C3_L3;
        public double OffsetDelay;
        public double OffsetZ0;
        public double OffsetLoss;
        public double ArbImp;
        public double MinFreq;
        public double MaxFreq;
        public naEnum.ECalStdMedia Media;
        public naEnum.ECalStdLengthType LengthType;
        public int Port1;
        public int Port2;
    }

    public struct STraceMatching
    {
        public int[] TraceNumber;
        public int[] SParamDefNumber;
        public int NoPorts;
    }

    public struct SParam
    {
        public SParamData[] SParamData;
        public double[] Freq;
        public int NoPoints;
        public bool[] SParamEnable;
    }
    public struct SParamData
    {
        public LibMath.MathLib.DataType[] SParam;
        public naEnum.ESFormat Format;
        public naEnum.ESParametersDef SParamDef;
    }
    //public struct SDataType
    //{
    //    public double Real;
    //    public double Imag;
    //    public double DB;
    //    public double Mag;
    //    public double Phase;
    //}
    public struct SSegmentTable
    {
        public naEnum.EModeSetting Mode;
        public naEnum.EOnOff Ifbw;
        public naEnum.EOnOff Pow;
        public naEnum.EOnOff Del;
        public naEnum.EOnOff Swp;
        public naEnum.EOnOff Time;
        public int Segm;
        public SSegmentData[] SegmentData;

    }
    public struct SSegmentData
    {
        public double Start;
        public double Stop;
        public int Points;
        public double IfbwValue;
        public double PowValue;
        public double DelValue;
        public naEnum.ESweepMode SwpValue;
        public double TimeValue;
    }

    public struct SStateFile
    {
        public string StateFile;
        public bool LoadState;
    }

}

