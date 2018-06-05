using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEqmtDriver.SCU
{
    public interface IISwitch
    {
        void Initialize();
        void SetPath(string val);
        void Reset();
    }
    
}
