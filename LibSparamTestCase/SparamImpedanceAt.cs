using System;

namespace SparamTestLib
{
    public class SparamImpedanceAt : SparamTestCase.TestCaseAbstract 
    {
        private int iTargetCnt;
        private bool blnInterStart;
        private double partialGradient;

        public override void Initialize()
        {
            _Result = new SResult();

            for (int seg = 0; seg < tmpSparamRaw[ChannelNumber - 1].Freq.Length; seg++)
            {
                if (TargetFreq >= tmpSparamRaw[ChannelNumber - 1].Freq[seg] && TargetFreq < tmpSparamRaw[ChannelNumber - 1].Freq[seg + 1])
                {
                    iTargetCnt = seg;
                    if (StartFreq == tmpSparamRaw[ChannelNumber - 1].Freq[seg])
                        blnInterStart = false;
                    else
                    {
                        blnInterStart = true;
                        partialGradient = (tmpSparamRaw[ChannelNumber - 1].Freq[iTargetCnt + 1] - 
                                            tmpSparamRaw[ChannelNumber - 1].Freq[iTargetCnt + 1]);
                        partialGradient = partialGradient * (TargetFreq - tmpSparamRaw[ChannelNumber - 1].Freq[iTargetCnt]);
                    }
                }
            }

            Array.Resize(ref _Result.Result, 1);
            Array.Resize(ref _Result.Header, 1);

            _Result.Enable = true;
            _Result.Header[0] = TcfHeader + "_C" + ChannelNumber + "_" + SParam +
                                "_" + TargetFreq / 1e6 + "_M";
            _Result.Result[0] = -9999;
        }
        public override void RunTest()
        {
            LibMath.MathLib objMath = new LibMath.MathLib();

            double rtnResult;
            int i_TargetFreqCnt = 0;
            int usePrevious = int.Parse(UsePrevious);

            if (childErrorRaise == true)
            {
                _Result.Result[0] = -999;
                return;
            }

            if (usePrevious > 0)
            {
                i_TargetFreqCnt = tmpResult[usePrevious].Misc;
                rtnResult = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[i_TargetFreqCnt].DB;
            }
            else
            {
                //need to calculate the iTargetCnt + 1 as the variable will be overwrite.
                if (blnInterStart)
                {
                    objMath.conv_SParam_to_Impedance(ref tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iTargetCnt + 1], Z0);
                    double p2 = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iTargetCnt + 1].Impedance;
                    objMath.conv_SParam_to_Impedance(ref tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iTargetCnt], Z0);
                    double p1 = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iTargetCnt].Impedance;
                    rtnResult = p1 + (partialGradient * (p2 - p1));
                }
                else
                {
                    objMath.conv_SParam_to_Impedance(ref tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iTargetCnt], Z0);
                    rtnResult = tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iTargetCnt].Impedance;
                }
            }

            _Result.Result[TestNo] = rtnResult;
        }
    }
}
