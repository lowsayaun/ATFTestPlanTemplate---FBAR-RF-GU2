using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProduct
{
    public struct SEqmtStatus
    {
        public bool Mxg01;
        public bool Mxg02;
        public bool Mxa01;
        public bool Mxa02;
        public bool Na01;
        public bool Smu;
        public string SmuCh;
        public bool Dc;
        public string DcCh;
        public bool Switch;
        public bool Mipi;
        public bool Pm;
        public bool TuneFilter;
        public bool[] DcSupply;
    }
    public struct STraceData
    {
        public int TestNumber;
        public bool Enable;
        public STraceNo[] MultiTrace;
    }
    public struct STraceNo
    {
        public string MxaNo;
        public int NoPoints;
        public double[] FreqMHz;
        public double[] Ampl;
        public string ResultHeader;
    }
    public struct SResult
    {
        public int TestNumber;
        public bool Enable;
        public SmRslt[] MultiResults;
        public int Misc;
    }
    public struct SmRslt
    {
        public bool Enable;
        public string ResultHeader;
        public double ResultData;
    }
    public struct SOffBiasAfterTest
    {
        public bool Dc;
        public bool Smu;
    }
    public struct SSnpFile
    {
        public string FileOutputPath;
        public string FileOutputFileName;
        public string FileSourcePath;
        public bool FileOutputEnable;
        public int FileOutputCount;
        public bool ADSFormat;
        public string LotID;
    }
    //public struct SSnpDatalog
    //{
    //    public int Number;
    //    public bool Status;
    //    public string SnpFolderPath;
    //    public string SnpFileName;
    //}
    public struct SStopOnFail
    {
        public bool Enable;
        public bool TestFail;
    }

    #region static variable
    public enum EResultTag
    {
        HarmonicAmpl,
        HarmonicFreq,
        Pin,
        Pout,
        Pin1,
        Pout1,
        Pin2,
        Pout2,
    }
    #endregion

    #region dynamic variable
    #endregion

    public static class TcfHeader
    {
        #region TCF Header
        public const string ConstTestNum = "Test Number";
        public const string ConstTestMode = "Test Mode";
        public const string ConstTestParam = "Test Parameter";
        public const string ConstParaName = "Parameter Name";
        public const string ConstParaHeader = "Parameter Header";
        public const string ConstUsePrev = "Use Previous";

        //Single Freq Condition
        #region Single Freq Condition
        public const string ConstPout = "Pout";
        public const string ConstPin = "Pin";
        public const string ConstTxBand = "TX_Band";
        public const string ConstTxFreq = "TX_Freq";
        public const string ConstRxBand = "RX_Band";
        public const string ConstRxFreq = "RX_Freq";
        public const string ConstTunePwrTx = "TunePwr_TX";
        #endregion

        //Sweep Freq Condition
        #region Sweep Freq Condition
        public const string ConstPout1 = "Pout1";
        public const string ConstPin1 = "Pin1";
        public const string ConstTx1Band = "TX1_Band";
        public const string ConstStartTxFreq1 = "Start_TXFreq1";
        public const string ConstStopTxFreq1 = "Stop_TXFreq1";
        public const string ConstStepTxFreq1 = "Step_TXFreq1";
        public const string ConstDwellTime1 = "Dwell_Time1";
        public const string ConstTunePwrTx1 = "TunePwr_TX1";

        public const string ConstRx1Band = "RX1_Band";
        public const string ConstStartRxFreq1 = "Start_RXFreq1";
        public const string ConstStopRxFreq1 = "Stop_RXFreq1";
        public const string ConstStepRxFreq1 = "Step_RXFreq1";
        public const string ConstRx1SweepT = "RX1_SweepTime";
        public const string ConstSetRx1NDiag = "RX1_NDiag_Mode";

        public const string ConstPoutTolerance = "Pout_Tolerance";
        public const string ConstPinTolerance = "Pin_Tolerance";
        public const string ConstPowerMode = "PowerMode";
        public const string ConstCalTag = "Calibration Tag";
        public const string ConstSwBand = "Switching Band";
        public const string ConstModulation = "Modulation";
        public const string ConstWaveformName = "WaveFormName";
        #endregion

        //SMU Header
        #region SMU header
        public const string ConstSmuSetCh = "SMUSet_Channel";
        public const string ConstSmuMeasCh = "SMUMeasure_Channel";
        public const string ConstSmuvCh0 = "SMUV_CH0";
        public const string ConstSmuvCh1 = "SMUV_CH1";
        public const string ConstSmuvCh2 = "SMUV_CH2";
        public const string ConstSmuvCh3 = "SMUV_CH3";
        public const string ConstSmuvCh4 = "SMUV_CH4";
        public const string ConstSmuvCh5 = "SMUV_CH5";
        public const string ConstSmuvCh6 = "SMUV_CH6";
        public const string ConstSmuvCh7 = "SMUV_CH7";
        public const string ConstSmuvCh8 = "SMUV_CH8";

        public const string ConstSmuiCh0Limit = "SMUI_CH0";
        public const string ConstSmuiCh1Limit = "SMUI_CH1";
        public const string ConstSmuiCh2Limit = "SMUI_CH2";
        public const string ConstSmuiCh3Limit = "SMUI_CH3";
        public const string ConstSmuiCh4Limit = "SMUI_CH4";
        public const string ConstSmuiCh5Limit = "SMUI_CH5";
        public const string ConstSmuiCh6Limit = "SMUI_CH6";
        public const string ConstSmuiCh7Limit = "SMUI_CH7";
        public const string ConstSmuiCh8Limit = "SMUI_CH8";
        #endregion

        //DC Header
        #region DC header
        public const string ConstDcSetCh = "DCSet_Channel";
        public const string ConstDcMeasCh = "DCMeasure_Channel";
        public const string ConstDcPsSet = "DC_PS_Set";
        public const string ConstDcvCh1 = "DCV_CH1";
        public const string ConstDcvCh2 = "DCV_CH2";
        public const string ConstDcvCh3 = "DCV_CH3";
        public const string ConstDcvCh4 = "DCV_CH4";
        public const string ConstDciCh1Limit = "DCI_CH1";
        public const string ConstDciCh2Limit = "DCI_CH2";
        public const string ConstDciCh3Limit = "DCI_CH3";
        public const string ConstDciCh4Limit = "DCI_CH4";
        #endregion

        //MIPI Header
        #region MIPI header
        public const string ConstMipiRegNo = "MIPI_RegCount";        //total register used
        public const string ConstMipiReg0 = "MIPI_Reg_0";
        public const string ConstMipiReg1 = "MIPI_Reg_1";
        public const string ConstMipiReg2 = "MIPI_Reg_2";
        public const string ConstMipiReg3 = "MIPI_Reg_3";
        public const string ConstMipiReg4 = "MIPI_Reg_4";
        public const string ConstMipiReg5 = "MIPI_Reg_5";
        public const string ConstMipiReg6 = "MIPI_Reg_6";
        public const string ConstMipiReg7 = "MIPI_Reg_7";
        public const string ConstMipiReg8 = "MIPI_Reg_8";
        public const string ConstMipiReg9 = "MIPI_Reg_9";
        public const string ConstMipiRegA = "MIPI_Reg_A";
        public const string ConstMipiRegB = "MIPI_Reg_B";
        public const string ConstMipiRegC = "MIPI_Reg_C";
        public const string ConstMipiRegD = "MIPI_Reg_D";
        public const string ConstMipiRegE = "MIPI_Reg_E";
        public const string ConstMipiRegF = "MIPI_Reg_F";
        #endregion

        //Equipment Setting
        #region Equip setting
        public const string ConstSetSa1 = "SA1_Setting";
        public const string ConstSetSa2 = "SA2_Setting";
        public const string ConstSetSg1 = "SG1_Setting";
        public const string ConstSetSg2 = "SG2_Setting";
        public const string ConstSetSmu = "SMU_Setting";

        public const string ConstSa1Att = "SA1_Input_Atten";
        public const string ConstSa2Att = "SA2_Input_Atten";
        public const string ConstSg1MaxPwr = "SG1_MaxPwr";
        public const string ConstSg2MaxPwr = "SG2_MaxPwr";

        public const string ConstSg1DefaultFreq = "SG1_DefaultFreq";        //variable to preset default or initial freq
        public const string ConstSg2DefaultFreq = "SG2_DefaultFreq";        //variable to preset default or initial freq

        //Equipment Off State Flag
        public const string ConstOffSg1 = "SG1_Off";
        public const string ConstOffSg2 = "SG2_Off";
        public const string ConstOffSmu = "SMU_Off";
        public const string ConstOffDc = "DC_Off";
        #endregion

        //Fbar Para
        #region Fbar Para
        public const string ConstChannelNo = "Channel Number";
        public const string ConstChannelNoRename = "Channel Number Rename";
        public const string ConstSPara = "S-Parameter";
        public const string ConstSPara2 = "S-Parameter_2";
        public const string ConstSParaRename = "S-Parameter_Rename";
        public const string ConstTargetFreq = "Target_Freq";
        public const string ConstStartFreq = "Start_Freq";
        public const string ConstStopFreq = "Stop_Freq";
        #endregion

        //Result Header
        public const string ConstParaPout = "Para_Pout";
        public const string ConstParaPin = "Para_Pin";
        public const string ConstParaPout1 = "Para_Pout1";
        public const string ConstParaPin1 = "Para_Pin1";
        public const string ConstParaMxaTrace = "Para_MXA_Trace";
        public const string ConstParaMxaTraceFreq = "Para_MXA_TraceFreq";
        public const string ConstParaHarmonic = "Para_Harmonic";
        public const string ConstParaMipi = "Para_MIPI";
        public const string ConstParaSmu = "Para_SMU";
        public const string ConstParaDcSupply = "Para_DCSupply";
        public const string ConstParaSwitch = "Para_Switch";
        public const string ConstParaTestTime = "Para_TestTime";

        //Delay Header and Other Setting
        public const string ConstTrigDelay = "Trig_Delay";
        public const string ConstGenericDelay = "Generic_Delay";
        public const string ConstRdCurrDelay = "RdCurr_Delay";
        public const string ConstRdPwrDelay = "RdPwr_Delay";
        public const string ConstSetupDelay = "Setup_Delay";
        public const string ConstStartSyncDelay = "StartSync_Delay";
        public const string ConstEstimateTestTime = "Estimate_TestTime";
        public const string ConstSearchDirection = "Search_Direction";
        public const string ConstSearchMethod = "Search_Method";
        public const string ConstSearchValue = "Search_Value";
        public const string ConstInterpolation = "Interpolation";
        public const string ConstAbsValue = "Absolute Value";
        public const string ConstZ0 = "Impedance_Z0";
        public const string ConstSaveMxaTrace = "Save_MXATrace";

        public const string ConstFBarResultDir = @"C:\\Avago.ATF.Common\\Results\\";
        #endregion
    }
    public static class DataFilePath
    {
        public const string CalPathRf = "CalFile_Path_RF";
        public const string CalPathNf = "CalFile_Path_NF";
        public const string LocSettingPath = "LocalSettingFile_Path";
        public const string EnaStateFileName = "ENA_StateFile_Path";
        public const string EnableDataLog = "Datalog";
        public const string DataLogCount = "DatalogCount";
        public const string SDIFolder = "SDIFolderPath";
        public const string SDIFileName = "SDIFilename";
        public const string AdsDataLog = "AdsDataLog";
        public const string GuPartNo = "GuPartNo";
    }
    public static class TcfSheet
    {
        #region TCF

        public const string ConstPaSheet = "Test_Condition_PA",
                         //ConstCalSheet = 3,
                         ConstKeyWordSheet = "Keywords";

        public const string ConstSegmentTableSheet = "Segment";
        public const int ConstSegmentIndexColumnNo = 1, ConstSegmentParaColumnNo = 2;

        public const string ConstMainSheet = "Main";

        public const string ConstTraceSheet = "Trace";

        public const string ConstFbarSheet = "Test_Condition";
        
        public const string ConstFbarCalSheet = "Calibration Procedure";
        public const string ConstCalStdSheet = "Define CalStd";   
             
        public const int ConstPaIndexColumnNo = 1, ConstPaTestParaColumnNo = 2;
        public const int ConstCalIndexColumnNo = 1, ConstCalParaColumnNo = 2;
        public const int ConstFbarIndexColumnNo = 1, ConstFbarTestParaColumnNo = 2;

        public const int ConstWaveFormColumnNo = 3;

        

        #endregion
    }
    public static class LocalSetting
    {
        #region Cal File
        public const string CalTag = "CalData1D";
        #endregion

        #region Local Setting File
        public const string HeaderFilePath = "FilePath";
        public const string KeyCalEnable = "Cal_Enable";
        public const string HeaderOcr = "OCR";
        public const string KeyOcrEnable = "Enable";
        #endregion
    }
}
