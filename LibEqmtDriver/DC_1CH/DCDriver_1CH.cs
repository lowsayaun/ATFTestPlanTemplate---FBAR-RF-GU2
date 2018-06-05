using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEqmtDriver.DC_1CH
{
    public interface IIDcSupply_1Ch
    {
        void Init();
        void DcOn(int channel);
        void DcOff(int channel);
        void SetVolt(int channel, double volt, double iLimit);
        float MeasI(int channel);
        float MeasV(int channel);
    }
}
