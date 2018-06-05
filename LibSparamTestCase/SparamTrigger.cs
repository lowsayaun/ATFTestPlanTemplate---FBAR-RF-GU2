using System;
using LibEqmtDriver.NA;

namespace SparamTestLib
{
    public class SparamTrigger : SparamTestCase.TestCaseAbstract
    {
        public override void Initialize()
        {
            _EqNa.TriggerSingle(ChannelNumber);
                
            tmpSparamRaw[ChannelNumber - 1] = new SParam
            {
                Freq = _EqNa.GetFreqList(ChannelNumber)
            };
            _EqNa.GrabSParamRiData(ChannelNumber);
            //_EqNa.TriggerMode(naEnum.ETriggerMode.Cont);
            _Result.Enable = false;
        }
        
        public override void RunTest()
        {
            tmpSparamRaw[ChannelNumber - 1] = new SParam();
            
            _EqNa.TriggerSingle(ChannelNumber);
            tmpSparamRaw[ChannelNumber - 1] = _EqNa.GrabSParamRiData(ChannelNumber);
        }
    }
}
