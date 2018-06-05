using System;

namespace SparamTestLib
{
    public class SparamMagBetween : SparamTestCase.TestCaseAbstract 
    {
        private int iStartCnt, iStopCnt;
        private bool blnInterStart, blnInterStop;
        public override void Initialize()
        {
            bool blnDoneStart = false , blnDoneStop = false;

            _Result = new SResult();

            for (int seg = 0; seg < tmpSparamRaw[ChannelNumber - 1].Freq.Length; seg++)
            {
                if (StartFreq >= tmpSparamRaw[ChannelNumber - 1].Freq[seg] && StartFreq <= tmpSparamRaw[ChannelNumber - 1].Freq[seg + 1])
                {
                    iStartCnt = seg;
                    blnInterStart = true;
                    blnDoneStart = true;
                    if (StartFreq == tmpSparamRaw[ChannelNumber - 1].Freq[seg])
                        blnInterStart = false;
                }
                if (StopFreq >= tmpSparamRaw[ChannelNumber - 1].Freq[seg] && StopFreq <= tmpSparamRaw[ChannelNumber - 1].Freq[seg + 1])
                {
                    iStopCnt = seg;
                    blnInterStop = true;
                    blnDoneStop = true;
                    if (StopFreq == tmpSparamRaw[ChannelNumber - 1].Freq[seg])
                        blnInterStop = false;
                }
                if (blnDoneStart && blnDoneStop)
                    break;
            }

            Array.Resize(ref _Result.Result, 1);
            Array.Resize(ref _Result.Header, 1);

            _Result.Enable = true;
            _Result.Header[0] = TcfHeader + "_C" + ChannelNumber + "_" + SParam +
                                "_" + StartFreq/1e6 + "_M_" + StopFreq/1e6 + "_M";
            _Result.Result[0] = -9999;
        }
        public override void RunTest()
        {
            bool b_PositiveValue;
            double tmpSearchResult;
            double Rslt1, Rslt2;

            if (childErrorRaise == true)
            {
                tmpResult[TestNo].Result[0] = -999;
                return;
            }

            if (tmpSparamRaw[ChannelNumber - 1].SParamData[GetSparamIndex()].SParam[iStartCnt].DB > 0)
            {
                b_PositiveValue = true;
            }
            else
            {
                b_PositiveValue = false;
            }

            tmpSearchResult = SearchData(iStartCnt, iStopCnt, b_PositiveValue);

            Rslt1 = SearchInterpolatedData(blnInterStart,StartFreq, iStartCnt);
            Rslt2 = SearchInterpolatedData(blnInterStop, StopFreq, iStopCnt);

            _Result.Result[0] = ProcessData(tmpSearchResult, Rslt1, Rslt2, b_PositiveValue);
        }
        
        private double ProcessData(double Rslt, double Rslt1, double Rslt2, bool PositiveValue)
        {
            double rtnRslt;
            //rtnRslt = -999;
            rtnRslt = Rslt;
            switch (SearchType)
            {
                case ESearchType.MAX:
                    if (PositiveValue)
                    {
                        if (Rslt < Rslt1)
                        {
                            rtnRslt = Rslt1;
                        }
                        if (Rslt < Rslt2)
                        {
                            rtnRslt = Rslt2;
                        }
                    }
                    else
                    {
                        if (Rslt > Rslt1)
                        {
                            rtnRslt = Rslt1;
                        }
                        if (Rslt > Rslt2)
                        {
                            rtnRslt = Rslt2;
                        }
                    }
                    break;
                case ESearchType.MIN:
                    if (PositiveValue)
                    {
                        if (Rslt > Rslt1)
                        {
                            rtnRslt = Rslt1;
                        }
                        if (Rslt > Rslt2)
                        {
                            rtnRslt = Rslt2;
                        }
                    }
                    else
                    {
                        if (Rslt < Rslt1)
                        {
                            rtnRslt = Rslt1;
                        }
                        if (Rslt < Rslt2)
                        {
                            rtnRslt = Rslt2;
                        }
                    }
                    break;
            }
            return (rtnRslt);
        }
    }
}
