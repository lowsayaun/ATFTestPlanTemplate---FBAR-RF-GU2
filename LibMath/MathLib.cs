using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMath
{
    public class MathLib
    {
        public struct DataType
        {
            public double Real;
            public double Imag;
            public double DB;
            public double Mag;
            public double Phase;
            public double Impedance;

        }

        
        public void conv_RealImag_to_dBAngle(ref DataType ret)
        {
            
            conv_RealImag_to_MagAngle(ref ret);
            ret.DB = 20 * (Math.Log10(ret.Mag));
          
        }
        private void conv_RealImag_to_MagAngle(ref DataType ret)
        {
            ret.Mag = Math.Sqrt((ret.Real * ret.Real) + (ret.Imag * ret.Imag));
            calc_Angle(ref ret);
        }
        private void calc_Angle(ref DataType ret)
        {
            double radian;
            radian = 0;
            if ((ret.Real == 0) && (ret.Imag == 0))
                radian = 0;
            else if (ret.Real > 0)
                radian = Math.Atan(ret.Imag / ret.Real);
            else if (ret.Real == 0)
                radian = Math.Sign(ret.Imag) * Math.PI / 2;
            else if (ret.Real < 0)
            {
                if (ret.Imag == 0)
                    radian = Math.Atan(ret.Imag / ret.Real) + Math.PI;
                else
                    radian = Math.Atan(ret.Imag / ret.Real) + (Math.Sign(ret.Imag) * Math.PI);
            }
            ret.Phase = radian * (180 / Math.PI);
        }
        public void conv_SParam_to_Impedance(ref DataType Zin, double Z0)
        {
            // This conversion only correct for Reflection Measurement (ie only S11,S22,S33 or S44)
            // Equation base on Application Note 2866 (AN2866)
            // Zin = Z0 * ((1 + S11)/(1 - S11))
            // Return will be impedence 

            Zin.Impedance = Z0 * ((1 - Math.Pow(Zin.Real, 2) - Math.Pow(Zin.Imag, 2)) / (Math.Pow((1 - Zin.Real), 2) + Math.Pow(Zin.Imag, 2)));

        }

    }
    

}
