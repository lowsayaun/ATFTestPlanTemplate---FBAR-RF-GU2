using System;

namespace SparamTestLib
{
    public class SparamRipple_Between : SparamTestCase.TestCaseAbstract 
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
            double Rslt_Max;
            double Rslt_Min;
            double tmpRslt;


            if (childErrorRaise == true)
            {
                tmpResult[TestNo].Result[0] = -999;
                return;
            }


            Rslt_Max = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStartCnt].DB;
            Rslt_Min = Rslt_Max;

            for (int iArr = iStartCnt; iArr < iStopCnt; iArr++)
            {
                tmpRslt = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iArr].DB;
                MaxMinComparator(ref Rslt_Min, ref Rslt_Max, tmpRslt);
            }

            if (Abs)
            {
                _Result.Result[0] = Math.Abs(Rslt_Max - Rslt_Min);
            }
            else
            {
                _Result.Result[0] = Rslt_Max - Rslt_Min;
            }
        }

        private void MaxMinComparator(ref double MinValue, ref double MaxValue, double parseValue)
        {
            if (parseValue > MaxValue)
            {
                MaxValue = parseValue;
            }
            if (parseValue < MinValue)
            {
                MinValue = parseValue;
            }
        }
    }
}
