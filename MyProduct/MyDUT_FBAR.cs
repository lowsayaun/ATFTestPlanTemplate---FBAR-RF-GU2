using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Avago.ATF.StandardLibrary;
using LibEqmtDriver.NA;
using SparamTestLib;
using Enum = System.Enum;


namespace MyProduct
{
    public class MyDutFbar
    {
        private static SEqmtStatus _eqmtStatus;
        public SSnpFile SnpFile;
        private static readonly IoTextFile IoTxtFile = new IoTextFile();

        private readonly MyUtility _myUtility = new MyUtility();

        private readonly Stopwatch _speedo = new Stopwatch();

        private Dictionary<string, string>[] _dicTestPa;
        private Dictionary<string, string>[] _dicTestFBar;
        private Dictionary<string, string> _dicCalInfo;
        private Dictionary<string, string> _dicWaveForm;
        private Dictionary<string, string> _dicTestLabel;

        private List<string> _fileContains = new List<string>();

        private LibEqmtDriver.SMU.IIPowerSupply[] _eqSmu;
        private LibEqmtDriver.SMU.PowerSupplyDriver _eqSmuDriver;
        private LibEqmtDriver.DC.IIDcSupply _eqDc;

        private LibEqmtDriver.NA.NetworkAnalyzerAbstract _eqNa;
        private LibEqmtDriver.NA.SSegmentTable[] _segmentParam;
        private LibEqmtDriver.NA.SParam[] _sParamData;
        private LibEqmtDriver.NA.SStateFile[] _stateFiles;
        private LibEqmtDriver.NA.STraceMatching[] _traceSetting;

        private SparamTestCase objFBar = new SparamTestCase();

        //Ivan
        private static bool _firstDut = true;

        //Fbar Cal Variable
        private LibEqmtDriver.NA.SCalibrationProcedure[]  _fbarCalProcedure;
        private LibEqmtDriver.NA.SCalStdTable _fbarCalStdTable;

        private bool _fbarCalMode;
        private string _fbarCalType;

        #region Misc Variable

        public string ProductTag="";
        public int TmpUnitNo;
        public SparamTestLib.SResult[] Results;
        public int TestCount;
        public STraceData[] MxaTrace;
        public STraceNo ResultMxaTrace;
        public int TotalDcSupply = 4;        //max DC Supply 1 Channel is 4 (equal 4 channel in tcf)

        private bool blnSuccess;

        private List<string> FileContains = new List<string>();

        public List<string> FileFileContain
        {
            set { FileContains = value; }
        }

        public bool InitSuccess
        {
            get { return blnSuccess; }
            set { blnSuccess = value; }
        }

        #endregion

        public MyDutFbar(ref StringBuilder sb, SSegmentTable[] segmentParam, SParam[] sParamData, SStateFile[] stateFiles, SCalibrationProcedure[] fbarCalProcedure, bool fbarCalMode, string fbarCalType)
        {
            _segmentParam = segmentParam;
            _sParamData = sParamData;
            _stateFiles = stateFiles;
            _fbarCalProcedure = fbarCalProcedure;
            _fbarCalMode = fbarCalMode;
            _fbarCalType = fbarCalType;
            Init(ref sb);
        }

        public MyDutFbar(ref StringBuilder sb)
        {
            Init(ref sb);
        }

        ~MyDutFbar()
        {
            //UnInit();
        }
        public void RunTest(ref ATFReturnResult results)
        {               
            string strError = string.Empty;
            // ReSharper disable once JoinDeclarationAndInitializer
            long testTime;

            TestCount = 0; //reset to start
            _speedo.Reset();
            _speedo.Start();
            
            foreach (SparamTestCase.TestCaseAbstract tc in objFBar.TestCases)
            {
                tc.RunTest();
                tc.BuildResults();
            }

            #region Create FmTrace file
            if (SnpFile.FileOutputEnable & TmpUnitNo <= SnpFile.FileOutputCount)
            {
                
                for (int i = 0; i < objFBar.SparamRaw.Length;i++)
                {
                    if (SnpFile.ADSFormat)
                    {
                        LibEqmtDriver.NA.SParam tsSparam = _eqNa.ConvertTourchStone(_traceSetting[i], objFBar.SparamRaw[i]);
                        _eqNa.ExportSnpFile(tsSparam, SnpFile.FileSourcePath, SnpFile.FileOutputFileName, TmpUnitNo.ToString(), i + 1);
                    }
                    else
                    {
                        _eqNa.ExportSnpFile(objFBar.SparamRaw[i], SnpFile.FileSourcePath, SnpFile.FileOutputFileName, TmpUnitNo.ToString(), i + 1);
                    }
                }
            }
            #endregion

            #region Add Result to Clotho
            for (int i = 0; i < objFBar.Result.Length; i++)
            {
                if (objFBar.Result[i].Enable)
                {
                    for (int iRst = 0; iRst < objFBar.Result[i].Header.Length; iRst++)
                    {
                        ATFResultBuilder.AddResult(ref results, objFBar.Result[i].Header[iRst], "", objFBar.Result[i].Result[iRst]);
                    }
                }
            }
            #endregion

            _speedo.Stop();
            testTime = _speedo.ElapsedMilliseconds;

            ATFResultBuilder.AddResult(ref results, "SparamTestTime", "mS", testTime);
           
        }

        private void Init(ref StringBuilder sb)
        {
            try
            {
                #region Load TCF
                var doneEvents = new ManualResetEvent[6];
                doneEvents[0] = new ManualResetEvent(false);
                doneEvents[1] = new ManualResetEvent(false);
                doneEvents[2] = new ManualResetEvent(false);
                doneEvents[3] = new ManualResetEvent(false);
                doneEvents[4] = new ManualResetEvent(false);
                doneEvents[5] = new ManualResetEvent(false);

                ThreadWithDelegate thLoadFbarTcf = new ThreadWithDelegate(doneEvents[0]);
                thLoadFbarTcf.WorkExternal = ReadFbarTcf;
                ThreadPool.QueueUserWorkItem(thLoadFbarTcf.ThreadPoolCallback, 0);

                ThreadWithDelegate thReadSegment = new ThreadWithDelegate(doneEvents[1]);
                thReadSegment.WorkExternal = ReadSegmentTable;
                ThreadPool.QueueUserWorkItem(thReadSegment.ThreadPoolCallback, 0);

                ThreadWithDelegate thLoadCalInfoTcf = new ThreadWithDelegate(doneEvents[2]);
                thLoadCalInfoTcf.WorkExternal = ReadFbarCalInfoTcf;
                ThreadPool.QueueUserWorkItem(thLoadCalInfoTcf.ThreadPoolCallback, 0);

                ThreadWithDelegate thFbarCalProcTcf = new ThreadWithDelegate(doneEvents[3]);
                thFbarCalProcTcf.WorkExternal = ReadFbarCalProcedure;
                ThreadPool.QueueUserWorkItem(thFbarCalProcTcf.ThreadPoolCallback, 0);

                ThreadWithDelegate thFbarCalKitStd = new ThreadWithDelegate(doneEvents[4]);
                thFbarCalKitStd.WorkExternal = ReadTcfCalKitStd;
                ThreadPool.QueueUserWorkItem(thFbarCalKitStd.ThreadPoolCallback, 0);

                ThreadWithDelegate thFbarTraceSetting = new ThreadWithDelegate(doneEvents[5]);
                thFbarTraceSetting.WorkExternal = ReadTcfTraceSetting;
                ThreadPool.QueueUserWorkItem(thFbarTraceSetting.ThreadPoolCallback, 0);

                WaitHandle.WaitAll(doneEvents);
                //ReadFbarTCF();
                //ReadSegmentTable();
                //ReadFbarCalProcedure();
                //ReadTcfTraceSetting();
                //ReadTcfCalKitStd();
                //ReadTcfTraceSetting();
                #endregion

                #region Retrieve Cal Sheet Info

                //string CalFilePath = Convert.ToString(DicCalInfo[DataFilePath.CalPathRF]); //H2 Cal Path
                string locSetFilePath = Convert.ToString(_dicCalInfo[DataFilePath.LocSettingPath]);
                SnpFile.FileOutputEnable = Convert.ToBoolean(_dicCalInfo[DataFilePath.EnableDataLog]);
                SnpFile.FileOutputPath = TcfHeader.ConstFBarResultDir;
                SnpFile.FileOutputCount = int.Parse(_dicCalInfo[DataFilePath.DataLogCount]);
                SnpFile.ADSFormat = Convert.ToBoolean(_dicCalInfo[DataFilePath.AdsDataLog]);

                ProductTag = _dicCalInfo[DataFilePath.GuPartNo].ToUpper();
                if (ProductTag == string.Empty | !ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_PACKAGE_TAG, "").ToUpper().Contains(ProductTag))
                {
                    InitSuccess = false;
                    MessageBox.Show("TCF Main sheet GuPartNo " + ProductTag + " not match with test package load " +
                        ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_PACKAGE_TAG, "").ToUpper(), "Read Main Sheet Error",MessageBoxButtons.OK);
                }
                #endregion

                #region Instrument Init

                InstrInit(locSetFilePath);

                #endregion

                #region Verify ENA State 
                InitSuccess = VerifyTraceMatch();
                InitSuccess = VerifySegment();
                #endregion

                #region Configure Network Analyzer
                string cnt_str = Interaction.InputBox
                    ("Do you want to Configure Network Analyzer? If so, please enter \"Yes\".", "Configure Network Analyzer", "No", 100, 100);
                if (cnt_str.ToUpper() == "YES")
                {
                    _eqNa.Reset();
                    _eqNa.SetupCalKit(_fbarCalStdTable);
                    _eqNa.SetupSegmentTable(_segmentParam);
                    for (int i = 0; i < _traceSetting.Length; i++)
                    {
                        _eqNa.SetupTrace(i + 1, _traceSetting[i]);
                    }

                    _eqNa.TriggerSource(naEnum.ETriggerSource.BUS);
                    _eqNa.SaveState(_stateFiles[0].StateFile);
                    InitSuccess = true;
                    
                }

                cnt_str = Interaction.InputBox
                    ("Do you want to perform FBAR subcal? If so, please enter \"Yes\".", "Network Analyzer Calibration", "No", 100, 100);
               if(cnt_str.ToUpper() == "YES")
                {
                    _eqNa.Calibrate(_fbarCalProcedure, _stateFiles[0].StateFile);
                    _eqNa.SaveState(_stateFiles[0].StateFile);
                    InitSuccess = true;
                }

               DialogResult rslt = MessageBox.Show("Switch OFF ENA display?","ENA Display Setting", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
               if (rslt == DialogResult.Yes)
               {
                   _eqNa.DisplayOn(true);
               }
               else
               {
                   _eqNa.DisplayOn(false);
               }
                #endregion

                #region Init Sparam Test Case
                InitTestCase();
                #endregion

            }
            catch(Exception ex)
            {
                InitSuccess = false;
                MessageBox.Show(ex.Message);
            }

        }
        public void UnInit()
        {
            var processes = from p in System.Diagnostics.Process.GetProcessesByName("EXCEL") select p;
            DateTime DT = DateTime.Now;

            foreach (var process in processes)
            {
                // All those background un-release process will be closed
                //Application.IgnoreRemoteRequests = False
                if (process.MainWindowTitle == "")
                    process.Kill();
            }

            if (SnpFile.FileOutputFileName != null)
            {
                string strHeaderPath;
                
                FileContains.Add("DATE_END=" + DT.ToString("yyyyMMdd") + "\r\n" + "TIME_END=" + DT.ToString("HHmmss"));
                strHeaderPath = Path.Combine(SnpFile.FileSourcePath, SnpFile.LotID + ".txt");
                _myUtility.Generate_SNP_Header(strHeaderPath, FileContains);
                _myUtility.SNP_SDI_Compression(SnpFile.FileOutputFileName, SnpFile.FileSourcePath, SnpFile.FileOutputPath);
            }

            string locSetFilePath = Convert.ToString(_dicCalInfo[DataFilePath.LocSettingPath]);
            bool blnOcr = (_myUtility.ReadTextFile(locSetFilePath, "OCR", "Enable").ToUpper() == "TRUE")? true: false;
            if (blnOcr)
            {
                switch (MessageBox.Show("Do you want to merge OCR automatically?", "Penang NPI", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.Yes:
                        LibOcr.OCR objOcr = new LibOcr.OCR();
                        objOcr.SetDateTime = DT;
                        objOcr.SetVisionDefaultPath = _myUtility.ReadTextFile(locSetFilePath,"OCR", "VisionDefaultPath");
                        objOcr.SetResultDefault = _myUtility.ReadTextFile(locSetFilePath, "OCR", "ResultDefaultPath");
                        objOcr.SetSite = _myUtility.ReadTextFile(locSetFilePath, "OCR", "Site");
                        objOcr.SetImageServer = _myUtility.ReadTextFile(locSetFilePath, "OCR", "ImagePath");
                        objOcr.SetSdiServer = _myUtility.ReadTextFile(locSetFilePath, "OCR", "SdiserverPath");
                        objOcr.SetResultFile = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_CUR_RESULT_FILE, "");
                        objOcr.SetLotID = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_LOT_ID, "");
                        objOcr.SetSublotId = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_SUB_LOT_ID, "");
                        objOcr.Merge();
                        
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        break;
                }
            }
            InstrUnInit();
        }
        
        private void InstrInit(string locSetFilePath)
        {
            #region NA Init

            string na = _myUtility.ReadTextFile(locSetFilePath, "EQUIPMENT", "NA");
            string nAaddr = _myUtility.ReadTextFile(locSetFilePath, "ADDRESS", "NA");
            _eqmtStatus.Na01 = false;
            try
            {
                switch (na.ToUpper())
                {
                    case "E5071B":
                    case "E5071C":
                    case "E5071D":
                        _eqNa = new LibEqmtDriver.NA.Ena(nAaddr);
                        //EqNA.Preset("1");
                        _eqmtStatus.Na01 = true;
                        break;

                    case "ZNB":
                    case "ZNBT":
                        _eqNa = new LibEqmtDriver.NA.Znbt(nAaddr);
                        //EqNA.Preset("2");
                        _eqmtStatus.Na01 = true;
                        break;
                    case "NONE":
                    case "NA":
                        _eqmtStatus.Na01 = false;
                        break;
                }

                _eqNa.TriggerMode(LibEqmtDriver.NA.naEnum.ETriggerMode.Single);
                _eqNa.TriggerSource(LibEqmtDriver.NA.naEnum.ETriggerSource.BUS);
                _eqNa.LoadState(_stateFiles[0].StateFile);
                DelayMs(3000);
                _eqNa.Operation_Complete();
            }
            catch
            {
                DisplayError(this.ToString(), "Init Equipment", "NA with alias name " + nAaddr + " init error.");
            }
            #endregion
        }

        private void InstrUnInit()
        {        
            if(_eqmtStatus.Na01)
            {
                _eqNa.CloseIo();
                _eqNa = null;
            }
        }

        //Fbar Test Mode
        private void InitTestCase()
        {
            int i = 0;
            string tmpStr;
            
            Array.Resize(ref objFBar.TestCases , _dicTestFBar.Length);
            objFBar.EqNa = _eqNa;
            objFBar.TotalTestNo = _dicTestFBar.Length;
            objFBar.TotalChannel = _segmentParam.Length;
            

            foreach (Dictionary<string, string> t in _dicTestFBar)
            {
                try
                {
                    objFBar.TestCases[i] = CreateTestCaseObj(_myUtility.ReadTcfData(t, TcfHeader.ConstTestParam));
                    objFBar.TestCases[i].TcfHeader = _myUtility.ReadTcfData(t, TcfHeader.ConstParaHeader);
                    tmpStr = _myUtility.ReadTcfData(t, TcfHeader.ConstTestParam);
                    objFBar.TestCases[i].TestPara = (ESparamTestCase)Enum.Parse(typeof(ESparamTestCase), tmpStr);
                    objFBar.TestCases[i].ChannelNumber = int.Parse((_myUtility.ReadTcfData(t, TcfHeader.ConstChannelNo)));
                    objFBar.TestCases[i].UsePrevious = _myUtility.ReadTcfData(t, TcfHeader.ConstUsePrev);
                    tmpStr = _myUtility.ReadTcfData(t, TcfHeader.ConstSPara);
                    objFBar.TestCases[i].SParam = (naEnum.ESParametersDef)Enum.Parse(typeof(naEnum.ESParametersDef), tmpStr);
                    objFBar.TestCases[i].StartFreq = _myUtility.CStr2Val(_myUtility.ReadTcfData(t, TcfHeader.ConstStartFreq));
                    objFBar.TestCases[i].StopFreq = _myUtility.CStr2Val(_myUtility.ReadTcfData(t, TcfHeader.ConstStopFreq));
                    objFBar.TestCases[i].TargetFreq = _myUtility.CStr2Val(_myUtility.ReadTcfData(t, TcfHeader.ConstTargetFreq));
                    tmpStr = _myUtility.ReadTcfData(t, TcfHeader.ConstSearchMethod);
                    objFBar.TestCases[i].SearchType = (ESearchType)Enum.Parse(typeof(ESearchType), tmpStr.ToUpper());
                    tmpStr = _myUtility.ReadTcfData(t, TcfHeader.ConstSearchDirection);
                    objFBar.TestCases[i].SearchDirection = (ESearchDirection)Enum.Parse(typeof(ESearchDirection), tmpStr.ToUpper());
                    objFBar.TestCases[i].SearchValue = double.Parse(_myUtility.ReadTcfData(t, TcfHeader.ConstSearchValue));
                    objFBar.TestCases[i].Interpolate = _myUtility.CStr2Bool(_myUtility.ReadTcfData(t, TcfHeader.ConstInterpolation));
                    objFBar.TestCases[i].Abs = _myUtility.CStr2Bool(_myUtility.ReadTcfData(t, TcfHeader.ConstAbsValue));

                    

                    objFBar.TestCases[i].TestNo = i;
                    objFBar.TestCases[i].Initialize();
                    objFBar.TestCases[i].BuildResults();
                    i++;
                }
                catch
                {
                    MessageBox.Show(objFBar.TestCases[i].TcfHeader + " init error");
             
                }
            }
        }

        private SparamTestCase.TestCaseAbstract CreateTestCaseObj(string val)
        {
            ESparamTestCase valTestCase = (ESparamTestCase)Enum.Parse(typeof(ESparamTestCase), val);
            SparamTestCase.TestCaseAbstract obj = null;
            switch (valTestCase)
            {
                case ESparamTestCase.Trigger:
                    {
                        obj = new SparamTrigger();
                        break;
                    }
                case ESparamTestCase.Mag_At:
                    {
                        obj = new SparamMagAt();
                        break;
                    }
                case ESparamTestCase.Imag_At:
                    {
                        obj = new SparamImagAt();
                        break;
                    }
                case ESparamTestCase.Freq_At:
                    {
                        obj = new SparamFreqAt();
                        break;
                    }
                case ESparamTestCase.Impedance_At:
                    {
                        obj = new SparamImpedanceAt();
                        break;
                    }
                case ESparamTestCase.Phase_At:
                    {
                        obj = new SparamPhaseAt();
                        break;
                    }
                case ESparamTestCase.Real_At:
                    {
                        obj = new SparamRealAt();
                        break;
                    }
                case ESparamTestCase.CPL_Between:
                    {
                        obj = new SparamCPL_Between();
                        break;
                    }
                case ESparamTestCase.Impendace_Bewteen:
                    {
                        obj = new SparamImpedance_Between();
                        break;
                    }
                case ESparamTestCase.Mag_Between:
                    {
                        obj = new SparamMagBetween();
                        break;
                    }
                case ESparamTestCase.Ripple_Between:
                    {
                        obj = new SparamRipple_Between();
                        break;
                    }
                case ESparamTestCase.Channel_Averaging:
                    {
                        obj = new SparamChannel_Avg();
                        break;
                    }
                case ESparamTestCase.Bandwidth:
                    {
                        obj = new SparamBandwith();
                        break;
                    }
                case ESparamTestCase.Balance:
                    {
                        obj = new SparamBanlance();
                        break;
                    }
                case ESparamTestCase.Delta:
                    {
                        obj = new SparamDelta();
                        break;
                    }
                case ESparamTestCase.Divide:
                    {
                        obj = new SparamDivide();
                        break;
                    }
                case ESparamTestCase.Relative_Gain:
                    {
                        obj = new SparamRelativeGain();
                        break;
                    }
                case ESparamTestCase.Sum:
                    {
                        obj = new SparamSum();
                        break;
                    }
            }
            return obj;//no obj created
        }

        private void ReadPaTcf()
        {
            _myUtility.ReadTcf(TcfSheet.ConstPaSheet, TcfSheet.ConstPaIndexColumnNo, TcfSheet.ConstPaTestParaColumnNo, ref _dicTestPa, ref _dicTestLabel);
        }
        private void ReadFbarTcf()
        {
            _myUtility.ReadTcf(TcfSheet.ConstFbarSheet, TcfSheet.ConstFbarIndexColumnNo, TcfSheet.ConstFbarTestParaColumnNo, ref _dicTestFBar, ref _dicTestLabel);
        }
        private void ReadCalTcf()
        {
            _myUtility.ReadCalSheet(TcfSheet.ConstMainSheet, TcfSheet.ConstCalIndexColumnNo, TcfSheet.ConstCalParaColumnNo, ref _dicCalInfo);
        }
        private void ReadFbarCalInfoTcf()
        {
            _myUtility.ReadCalSheet(TcfSheet.ConstMainSheet, TcfSheet.ConstCalIndexColumnNo, TcfSheet.ConstCalParaColumnNo, ref _dicCalInfo);
        }
        private void ReadFbarCalProcedure()
        {
            _myUtility.ReadTcfFbarCalProcedure(TcfSheet.ConstFbarCalSheet, ref _fbarCalProcedure, ref _fbarCalMode, ref _fbarCalType);
        }
        private void ReadWafeForm()
        {
            _myUtility.ReadWaveformFilePath(TcfSheet.ConstKeyWordSheet, TcfSheet.ConstWaveFormColumnNo, ref _dicWaveForm);
        }
        private void ReadSegmentTable()
        {
            _myUtility.ReadTcfSegmentTable(TcfSheet.ConstSegmentTableSheet, TcfSheet.ConstTraceSheet, ref _segmentParam, ref _sParamData, ref _stateFiles);
        }
        private void ReadTcfTraceSetting()
        {
            _myUtility.ReadTcfFbarTrace(TcfSheet.ConstTraceSheet, ref _traceSetting);
        }

        private void ReadTcfCalKitStd()
        {
            _myUtility.ReadTcfFbarCalStd(TcfSheet.ConstCalStdSheet,ref _fbarCalStdTable);
        }
      
        private bool VerifyTraceMatch()
        {
            string[] arrTrace;
            string[] arrSparam;
            bool bOk = true;
            try
            {
                for (int iChn = 0; iChn < _traceSetting.Length; iChn++)
                {
                    string tmpVar = _eqNa.GetTraceInfo(iChn + 1);
                    tmpVar = tmpVar.Replace("'", "").Replace("\n", "").Trim();
                    string[] tmp = tmpVar.Split(new char[] { ',' });

                    arrTrace = new string[tmp.Length / 2];
                    arrSparam = new string[tmp.Length / 2];
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        arrTrace[i / 2] = tmp[i].Replace("Trc", "");
                        arrSparam[i / 2] = tmp[i + 1];
                        i++;
                    }

                    for (int i = 0; i < _traceSetting[iChn].TraceNumber.Length; i++)
                    {
                        try
                        {
                            if (_traceSetting[iChn].TraceNumber[i] == 0) continue;
                            if ((arrTrace[i ] != _traceSetting[iChn].TraceNumber[i].ToString()) |
                                (arrSparam[i] != Enum.GetName(typeof(naEnum.ESParametersDef), _traceSetting[iChn].SParamDefNumber[i])))
                            {
                                DisplayError(this.ToString(), "Possible S Parameter Mismatch",
                                    "S Parameter for Trace Number " + _traceSetting[iChn].TraceNumber[i].ToString() + " mismatch!!!");
                                bOk = false;
                            }
                        }
                        catch
                        {
                            DisplayError(this.ToString(), "Possible S Parameter Mismatch",
                                    "S Parameter for Trace Number " + _traceSetting[iChn].TraceNumber[i].ToString() + " mismatch!!!");
                            bOk = false;
                        }
                    }
                }
                return bOk;
            }
            catch
            {
                DisplayError(this.ToString(), "Possible S Parameter Mismatch",
                                   "TCF Trace Sheet Total Channel VS NA Total Channel mismatch!!!");
                return false;
            }
        }

        private bool VerifySegment()
        {
            SSegmentTable NASegment;
            string tmpStr = string.Empty;
            bool bOk = true;
            try
            {

                for (int i = 0; i < _segmentParam.Length; i++)
                {

                    _eqNa.GetSegmentTable(out NASegment, i + 1);

                    if (NASegment.Mode != _segmentParam[i].Mode)
                    {
                        tmpStr += "\r\nENA Segment Mode = " + NASegment.Mode + ", Segment Mode = " + _segmentParam[i].Mode;
                    }
                    if (NASegment.Ifbw != _segmentParam[i].Ifbw)
                    {
                        tmpStr += "\r\nENA Segment IF BW = " + NASegment.Ifbw + ", Segment IF BW = " + _segmentParam[i].Ifbw;
                    }
                    if (NASegment.Pow != _segmentParam[i].Pow)
                    {
                        tmpStr += "\r\nENA Segment Power = " + NASegment.Pow + ", Segment Power = " + _segmentParam[i].Pow;
                    }
                    if (NASegment.Del != _segmentParam[i].Del)
                    {
                        tmpStr += "\r\nENA Segment Delay = " + NASegment.Del + ", Segment Delay = " + _segmentParam[i].Del;
                    }
                    if (NASegment.Swp != _segmentParam[i].Swp)
                    {
                        tmpStr += "\r\nENA Segment Sweep Mode Setting = " + NASegment.Del + ", Segment Sweep Mode Setting = " + _segmentParam[i].Del;
                    }
                    if (NASegment.Time != _segmentParam[i].Time)
                    {
                        tmpStr += "\r\nENA Segment Time = " + NASegment.Time + ", Segment Time = " + _segmentParam[i].Time;
                    }
                    if (NASegment.Segm != _segmentParam[i].Segm)
                    {
                        tmpStr += "\r\nENA Segment Number = " + NASegment.Segm + ", Segment Number = " + _segmentParam[i].Segm;
                    }
                    else
                    {
                        for (int iSegm = 0; iSegm < NASegment.Segm; iSegm++)
                        {
                            if (NASegment.SegmentData[iSegm].Start != _segmentParam[i].SegmentData[iSegm].Start)
                            {
                                tmpStr += "\r\nSegment Number : " + (iSegm + 1) + "--> ENA Segment Start Frequency = " + NASegment.SegmentData[iSegm].Start + ", Segment Start Frequency = " + _segmentParam[i].SegmentData[iSegm].Start;
                            }
                            if (NASegment.SegmentData[iSegm].Stop != _segmentParam[i].SegmentData[iSegm].Stop)
                            {
                                tmpStr += "\r\nSegment Number : " + (iSegm + 1) + "--> ENA Segment Stop Frequency = " + NASegment.SegmentData[iSegm].Stop + ", Segment Stop Frequency = " + _segmentParam[i].SegmentData[iSegm].Stop;
                            }
                            if (NASegment.SegmentData[iSegm].Points != _segmentParam[i].SegmentData[iSegm].Points)
                            {
                                tmpStr += "\r\nSegment Number : " + (iSegm + 1) + "--> ENA Segment Points = " + NASegment.SegmentData[iSegm].Points + ", Segment Points = " + _segmentParam[i].SegmentData[iSegm].Points;
                            }
                            if ((NASegment.SegmentData[iSegm].IfbwValue != _segmentParam[i].SegmentData[iSegm].IfbwValue) & NASegment.Ifbw == naEnum.EOnOff.On)
                            {
                                tmpStr += "\r\nSegment Number : " + (iSegm + 1) + "--> ENA Segment IF Bandwidth = " + NASegment.SegmentData[iSegm].IfbwValue + ", Segment IF Bandwidth = " + _segmentParam[i].SegmentData[iSegm].IfbwValue;
                            }
                            if ((NASegment.SegmentData[iSegm].PowValue != _segmentParam[i].SegmentData[iSegm].PowValue) && NASegment.Pow == naEnum.EOnOff.On)
                            {
                                tmpStr += "\r\nSegment Number : " + (iSegm + 1) + "--> ENA Segment Power = " + NASegment.SegmentData[iSegm].PowValue + ", Segment Power = " + _segmentParam[i].SegmentData[iSegm].PowValue;
                            }
                            if ((NASegment.SegmentData[iSegm].DelValue != _segmentParam[i].SegmentData[iSegm].DelValue) && NASegment.Del == naEnum.EOnOff.On)
                            {
                                tmpStr += "\r\nSegment Number : " + (iSegm + 1) + "--> ENA Segment Delay = " + NASegment.SegmentData[iSegm].DelValue + ", Segment Delay = " + _segmentParam[i].SegmentData[iSegm].DelValue;
                            }
                            if ((NASegment.SegmentData[iSegm].TimeValue != _segmentParam[i].SegmentData[iSegm].TimeValue) && NASegment.Time == naEnum.EOnOff.On)
                            {
                                tmpStr += "\r\nSegment Number : " + (iSegm + 1) + "--> ENA Segment Time = " + NASegment.SegmentData[iSegm].TimeValue + ", Segment Time = " + _segmentParam[i].SegmentData[iSegm].TimeValue;
                            }
                        }
                    }
                }


                if (tmpStr != "")
                {
                    DisplayError(this.ToString(), "Error in verifying Segment Table", "Mistake in Segment Table \r\n" + tmpStr);
                    bOk = false;
                }
            }
            catch (Exception e)
            {
                DisplayError(this.ToString(), "Error in verifying Segment Table", "Mistake in Segment Table \r\n" + e.Message);
                bOk = false;
            }
            return bOk;
        }
            
        public void DelayMs(int mSec)
        {
            LibEqmtDriver.Utility.HiPerfTimer timer = new LibEqmtDriver.Utility.HiPerfTimer();
            timer.Wait(mSec);
        }
        public void DelayUs(int uSec)
        {
            LibEqmtDriver.Utility.HiPerfTimer timer = new LibEqmtDriver.Utility.HiPerfTimer();
            timer.wait_us(uSec);
        }
        public void BuildResults(ref ATFReturnResult results, string paraName, string unit, double value)
        {
            ATFResultBuilder.AddResult(ref results, paraName, unit, value);
        }

        private void DisplayError(string ClassName, string ErrParam, string ErrDesc)
        {
            MessageBox.Show("Class Name: " + ClassName + "\nParameters: " + ErrParam + "\n\nErrorDesciption: \n"
                + ErrDesc, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
    }
}
