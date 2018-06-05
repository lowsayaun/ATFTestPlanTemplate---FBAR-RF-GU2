using System;

namespace SparamTestLib
{
    public class SparamImpedance_Between : SparamTestCase.TestCaseAbstract 
    {
        private int iStartCnt, iStopCnt;
        private bool blnInterStart, blnInterStop;
        private double tmpVal = 0;

        public override void Initialize()
        {
            _Result = new SResult();

            for (int seg = 0; seg < tmpSparamRaw[ChannelNumber - 1].Freq.Length; seg++)
            {
                if (StartFreq >= tmpSparamRaw[ChannelNumber - 1].Freq[seg] && StartFreq < tmpSparamRaw[ChannelNumber - 1].Freq[seg + 1])
                {
                    iStartCnt = seg;
                    blnInterStart = true;
                    if (StartFreq == tmpSparamRaw[ChannelNumber - 1].Freq[seg])
                        blnInterStart = false;
                }
                if (StopFreq >= tmpSparamRaw[ChannelNumber - 1].Freq[seg] && StopFreq < tmpSparamRaw[ChannelNumber - 1].Freq[seg + 1])
                {
                    iStopCnt = seg;
                    blnInterStop = true;
                    if (StopFreq == tmpSparamRaw[ChannelNumber - 1].Freq[seg])
                        blnInterStop = false;
                }
            }

            if (SearchType == ESearchType.MAX)
                tmpVal = -99999;
            if (SearchType == ESearchType.MIN)
                tmpVal = 99999;

            Array.Resize(ref _Result.Result, 1);
            Array.Resize(ref _Result.Header, 1);

            _Result.Enable = true;
            _Result.Header[0] = TcfHeader + "_C" + ChannelNumber + "_" + SParam +
                                "_" + StartFreq / 1e6 + "_M_" + StopFreq / 1e6 + "_M";
            _Result.Result[0] = -9999;
        }
        public override void RunTest()
        {
            LibMath.MathLib objMath = new LibMath.MathLib();

            if (childErrorRaise == true)
            {
                tmpResult[TestNo].Result[0] = -999;
                return;
            }


            for (int iArr = iStartCnt; iArr <= iStopCnt; iArr++)
            {
                
                objMath.conv_SParam_to_Impedance(ref tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr], Z0);
                    
                if (SearchType == ESearchType.MAX)
                {
                    if (tmpVal < tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr].Impedance)
                    {
                        tmpVal = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr].Impedance;
                    }
                }
                if (SearchType == ESearchType.MIN)
                {
                    if (tmpVal > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr].Impedance)
                    {
                        tmpVal = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr].Impedance;
                    }
                }
            }
            _Result.Result[0] = tmpVal;
        }
    }
}
