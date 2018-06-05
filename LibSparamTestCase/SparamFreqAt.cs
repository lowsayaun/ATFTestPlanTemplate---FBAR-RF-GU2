using System;

namespace SparamTestLib
{
    public class SparamFreqAt : SparamTestCase.TestCaseAbstract 
    {
        private int iStartCnt, iStopCnt;
        private bool blnInterStart, blnInterStop;

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

            Array.Resize(ref _Result.Result, 1);
            Array.Resize(ref _Result.Header, 1);

            _Result.Enable = true;
            _Result.Header[0] = TcfHeader + "_C" + ChannelNumber + "_" + SParam +
                                "_" + StartFreq / 1e6 + "_M_" + StopFreq / 1e6 + "_M";
            _Result.Result[0] = -9999;
        }

        public override void RunTest()
        {
            int intPeakCnt;
            int intResultCnt;
            double rtnFreq = 99e9;
            bool b_PositiveValue;
            int iStopCntR = iStopCnt, iStartCntR = iStartCnt;

            if (tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStartCnt].DB > 0)
            {
                b_PositiveValue = true;
            }
            else
            {
                b_PositiveValue = false;
            }

            switch (SearchDirection)
            {
                case ESearchDirection.FROM_MAX_LEFT:
                    intPeakCnt = SearchMaxValue(iStartCnt, iStopCnt);
                    iStopCntR = intPeakCnt;
                    if (tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStartCntR].DB > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStopCntR].DB)
                        b_PositiveValue = true;
                    else
                        b_PositiveValue = false;
                    break;
                case ESearchDirection.FROM_MAX_RIGHT:
                    intPeakCnt = SearchMaxValue(iStartCnt, iStopCnt);
                    iStartCntR = intPeakCnt;
                    if (tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStartCntR].DB > tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStopCntR].DB)
                        b_PositiveValue = true;
                    else
                        b_PositiveValue = false;
                    break;
            }

            intResultCnt = (int)SearchData(iStartCntR, iStopCntR, b_PositiveValue);

            if (tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[intResultCnt].DB != SearchValue)
            {
                double f1 = tmpSparamRaw[ChannelNumber - 1].Freq[intResultCnt];
                double f2 = tmpSparamRaw[ChannelNumber - 1].Freq[intResultCnt - 1];
                double p1 = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[intResultCnt].DB;
                double p2 = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[intResultCnt - 1].DB;
                double Gradient = (f2 - f1) / (p2 - p1);

                rtnFreq = Math.Round(((SearchValue - p1) * Gradient) + f1, 0);
            }
            else
                rtnFreq = tmpSparamRaw[ChannelNumber - 1].Freq[intResultCnt];

            _Result.Result[0] = rtnFreq;

            
        }
    }
}
