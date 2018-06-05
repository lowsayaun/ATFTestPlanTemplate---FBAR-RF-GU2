using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibEqmtDriver.NA
{
    class Topaz : NetworkAnalyzerAbstract
    {
        readonly int _intTimeOut = 10000;

        public Topaz(string address = "TCPIP0::localhost::hislip0::instr")
        {
            try
            {
                base.OpenIo(address, _intTimeOut);
                base.Operation_Complete();
                SendCommand("FORM REAL, 64");
                SendCommand("FORM:BORD NORM");
            }
            catch { DisplayError(this.ToString(), "TOPAZ Init", "TOPAZ address mismatch -" + address); }
        }
    }
}
