﻿using System;

namespace SparamTestLib
{
    public class SparamMagAt : SparamTestCase.TestCaseAbstract
    {
        private int iTargetCnt;
        private bool blnInterStart;

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
                        blnInterStart = true;                 
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
                rtnResult = SearchInterpolatedData(blnInterStart, StartFreq, iTargetCnt);      
            }

            _Result.Result[0] = rtnResult;
        }
    }
}
