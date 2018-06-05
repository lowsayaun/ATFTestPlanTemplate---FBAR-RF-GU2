
using System;
using System.Windows.Forms;
using LibEqmtDriver.NA;

namespace SparamTestLib
{
    public struct SResult
    {
        public int TestNumber;
        public bool Enable;
        public string[] Header;
        public double[] Result;
        public int Misc;
    }

    public enum ESparamTestCase
    {
        Trigger = 0,
        Freq_At,
        Mag_At,
        Real_At,
        Imag_At,
        Phase_At,
        Impedance_At,
        Mag_Between,
        CPL_Between,
        Ripple_Between,
        Impendace_Bewteen,
        Bandwidth,
        Balance,
        Channel_Averaging,
        Delta,
        Sum,
        Divide,
        Multiply,
        Relative_Gain
    }

    public enum ESearchDirection
    {
        NONE = 0,
        FROM_LEFT,
        FROM_RIGHT,
        FROM_MAX_LEFT,
        FROM_MAX_RIGHT,
        FROM_EXTREME_LEFT,
        FROM_EXTREME_RIGHT,
    }
    public enum ESearchType
    {
        MIN = 0,
        MAX,
        USER
    }

    public class SparamTestCase
    {
        public TestCaseAbstract[] TestCases;

        private static bool ErrorRaise;
        private int _totalTestNo;
        private int _totalChannel;
        private static NetworkAnalyzerAbstract _Eq;
        private static SResult[] _sParaResults;
        private static SParam[] _sParamRaw;

        public NetworkAnalyzerAbstract EqNa
        {
            get { return _Eq; }
            set { _Eq = value; }
        }

        public SResult[] Result
        {
            get { return _sParaResults; }
        }

        public int TotalTestNo
        {
            get { return _totalTestNo; }
            set
            {
                _totalTestNo = value;
                Array.Resize(ref _sParaResults, value);
            }
        }
        public int TotalChannel
        {
            get { return _totalChannel; }
            set
            {
                _totalChannel = value;
                Array.Resize(ref _sParamRaw, value);
            }
        }

        public SParam[] SparamRaw
        {
            get { return _sParamRaw; }
        }
        //public string SDIFolderPath
        //{
        //    get { return _SDIFolderPath; }
        //    set { _SDIFolderPath = value;}
        //}

        //public string SDIFileName
        //{
        //    get { return _SDIFileName; }
        //    set { _SDIFileName = value; }
        //}
        //public bool EnableDatalog
        //{
        //    get { return _blnDatalog; }
        //    set { _blnDatalog = value; }
        //}
        //public int DatalogCount
        //{
        //    get { return _DatalogCount; }
        //    set { _DatalogCount = value; }
        //}
        public abstract class TestCaseAbstract
        {
            //protected variable

            #region protected variable
            protected SResult _Result;
            private int _ChannelNumber;
            private string _TCFHeader;
            private naEnum.ESParametersDef _SParam;
            private naEnum.ESParametersDef _SParam2;
            private double _StartFreq;
            private double _StopFreq;
            private double _TargetFreq;
            private ESearchDirection _SearchDirection;
            private ESearchType _SearchType;
            private double _SearchValue;
            private bool _Interpolate;
            private bool _Abs;
            private int _TestNo;
            private ESparamTestCase _TestPara;
            private string _UsePrevious;
            private double _Z0;
            #endregion

            //Class Property
            #region Class Property

            public ESparamTestCase TestPara
            {
                get { return _TestPara; }
                set { _TestPara = value; }
            }

            public int ChannelNumber
            {
                get { return _ChannelNumber; }
                set { _ChannelNumber = value; }
            }

            public string TcfHeader
            {
                get { return _TCFHeader; }
                set { _TCFHeader = value.Replace(" ", "_"); }
            }

            public naEnum.ESParametersDef SParam
            {
                get { return _SParam; }
                set { _SParam = value; }
            }

            public naEnum.ESParametersDef SParam2
            {
                get { return _SParam2; }
                set { _SParam2 = value; }
            }

            public double StartFreq
            {
                get { return _StartFreq; }
                set { _StartFreq = value; }
            }

            public double StopFreq
            {
                get { return _StopFreq; }
                set { _StopFreq = value; }
            }

            public double TargetFreq
            {
                get { return _TargetFreq; }
                set { _TargetFreq = value; }
            }

            public ESearchDirection SearchDirection
            {
                get { return _SearchDirection; }
                set { _SearchDirection = value; }
            }

            public ESearchType SearchType
            {
                get { return _SearchType; }
                set { _SearchType = value; }
            }

            public double SearchValue
            {
                get { return _SearchValue; }
                set { _SearchValue = value; }
            }

            public bool Interpolate
            {
                get { return _Interpolate; }
                set { _Interpolate = value; }
            }

            public bool Abs
            {
                get { return _Abs; }
                set { _Abs = value; }
            }

            public int TestNo
            {
                get { return _TestNo; }
                set { _TestNo = value; }
            }

            public string UsePrevious
            {
                get { return _UsePrevious; }
                set { _UsePrevious = value; }
            }

            public double Z0
            {
                get { return _Z0; }
                set { _Z0 = value; }
            }
            #endregion

            protected NetworkAnalyzerAbstract _EqNa = SparamTestCase._Eq;
            protected SResult[] tmpResult = SparamTestCase._sParaResults;
            protected SParam[] tmpSparamRaw = SparamTestCase._sParamRaw;
            protected bool childErrorRaise = SparamTestCase.ErrorRaise;
            //protected bool EnableDatalog = SparamTestCase._blnDatalog;
            //protected int DatalogCount = SparamTestCase._DatalogCount;
            //protected string SDIFolder = SparamTestCase._SDIFolderPath;
            //protected string SDIFileName = SparamTestCase._SDIFileName;

            public virtual void Initialize()
            {
                throw new NotImplementedException();
            }

            public void BuildResults()
            {
                tmpResult[_TestNo] = _Result;
            }

            public virtual void RunTest()
            {
                ErrorRaise = true;
            }


            //Private Method
            protected double SearchData(int iStartCnt, int iStopCnt, bool PositiveValue)
            {
                double tmpResult;
                tmpResult = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStartCnt].DB; //SParamData[Channel_Number - 1].sParam_Data[SParam].sParam[StartFreqCnt].dBAng.dB;

                switch (SearchType)
                {
                    case ESearchType.MAX:
                        if (PositiveValue)
                        {
                            for (int i = iStartCnt; i <= iStopCnt; i++)
                            {
                                if (tmpResult < tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                {
                                    tmpResult = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB;
                                }
                            }
                        }
                        else
                        {
                            for (int i = iStartCnt; i <= iStopCnt; i++)
                            {
                                if (tmpResult > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                {
                                    tmpResult = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB;
                                }
                            }
                        }
                        break;
                    case ESearchType.MIN:
                        if (PositiveValue)
                        {
                            for (int i = iStartCnt; i <= iStopCnt; i++)
                            {
                                if (tmpResult > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                {
                                    tmpResult = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB;
                                }
                            }
                        }
                        else
                        {
                            for (int i = iStartCnt; i <= iStopCnt; i++)
                            {
                                if (tmpResult < tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                {
                                    tmpResult = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB;
                                }
                            }
                        }
                        break;
                    case ESearchType.USER:
                        tmpResult = iStartCnt;
                        if(PositiveValue)
                        {
                            if (SearchDirection != ESearchDirection.FROM_RIGHT)
                            {
                                for (int i = iStartCnt; i <= iStopCnt; i++)
                                {
                                    if (_SearchValue < tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i - 1].DB &&
                                        _SearchValue > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                    {
                                        tmpResult = i;
                                        break;
                                    }
                                }
                            }
                            else
                            {

                                for (int i = iStartCnt + (iStopCnt - iStartCnt); i != iStartCnt; i--)
                                {
                                    if (_SearchValue < tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i - 1].DB &&
                                        _SearchValue > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                    {
                                        tmpResult = i;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (SearchDirection != ESearchDirection.FROM_RIGHT)
                            {
                                for (int i = iStartCnt; i <= iStopCnt; i++)
                                {
                                    if (_SearchValue > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i - 1].DB &&
                                        _SearchValue < tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                    {
                                        tmpResult = i;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = iStartCnt +(iStopCnt - iStartCnt); i != iStartCnt ; i--)
                                {
                                    if (_SearchValue < tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i - 1].DB &&
                                        _SearchValue > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i].DB)
                                    {
                                        tmpResult = i;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }
                return (tmpResult);
            }
            protected int SearchMaxValue(int iStartCnt, int iStopCnt)
            {
                int tmpReturn = -999;
                double Peak_Value = -999999;

                for (int iArr = iStartCnt; iArr <= iStopCnt; iArr++)
                {
                    if (tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr].DB > Peak_Value)
                    {
                        Peak_Value = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr].DB;
                        tmpReturn = iArr;
                    }
                }

                return tmpReturn;
            }
            protected double SearchInterpolatedData(bool blnInterpolate, double Frequency, int FreqCnt)
            {
                //double tmpData;
                double y, y0, y1, x0, x1, x;
                if (blnInterpolate == false)
                {
                    y = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[FreqCnt].DB;
                    //tmpData = tmpSparamRaw[_ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStartCnt].DB;
                }
                else
                {
                    y0 = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[FreqCnt].DB;
                    y1 = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[FreqCnt + 1].DB;
                    x = Frequency;
                    x0 = tmpSparamRaw[ChannelNumber - 1].Freq[FreqCnt];
                    x1 = tmpSparamRaw[ChannelNumber - 1].Freq[FreqCnt + 1];

                    y = y0 + (x - x0) * ((y1 - y0) / (x1 - x0));

                    //tmpData = ((tmpSparamRaw[_ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[FreqCnt + 1].DB - tmpSparamRaw[_ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[FreqCnt].DB) /
                    //            (tmpSparamRaw[_ChannelNumber - 1].Freq[FreqCnt + 1] - tmpSparamRaw[_ChannelNumber - 1].Freq[FreqCnt]) *
                    //            (Frequency - tmpSparamRaw[_ChannelNumber - 1].Freq[FreqCnt])) + tmpSparamRaw[_ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[FreqCnt].DB;
                }
                return y;// tmpData;
            }
            protected int GetSparamIndex()
            {
                int val = -1;

                for (int t = 0; t < tmpSparamRaw[ChannelNumber - 1].SParamData.Length;t++ )
                {
                    if (tmpSparamRaw[ChannelNumber - 1].SParamData[t].SParamDef.Equals(_SParam))
                        val = t;
                }
                return val;
            }

            protected void DisplayError(string ClassName, string ErrParam, string ErrDesc)
            {
                MessageBox.Show("Class Name: " + ClassName + "\nParameters: " + ErrParam + "\n\nErrorDesciption: \n"
                    + ErrDesc, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }
    }
}
