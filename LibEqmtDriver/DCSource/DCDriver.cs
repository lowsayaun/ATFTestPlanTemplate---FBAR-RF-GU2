using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEqmtDriver.DC
{
    public interface IIDcSupply
    {
        void Init();
        void DcOn(int channel);
        void DcOff(int channel);
        void SetVolt(int channel, double volt, double iLimit);
        float MeasI(int channel);
        float MeasV(int channel);
    }

}
