using System;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver.SMU
{
    public class KeithleySmu : IIPowerSupply 
    {
        public static string ClassName = "Keithly SMU Class";
        private FormattedIO488 _myVisaKeithley = new FormattedIO488();
        public string IoAddress;

        /// <summary>
        /// Parsing Equpment Address
        /// </summary>
        public string Address
        {
            get
            {
                return IoAddress;
            }
            set
            {
                IoAddress = value;
            }
        }
        public FormattedIO488 ParseIo
        {
            get
            {
                return _myVisaKeithley;
            }
            set
            {
                _myVisaKeithley = ParseIo;
            }
        }
        public void OpenIo()
        {
            if (IoAddress.Length > 3)
            {
                try
                {
                    ResourceManager mgr = new ResourceManager();
                    _myVisaKeithley.IO = (IMessage)mgr.Open(IoAddress, AccessMode.NO_LOCK, 2000, "");
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("Class Name: " + ClassName + "\nParameters: OpenIO" + "\n\nErrorDesciption: \n"
                        + ex, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _myVisaKeithley.IO = null;
                    return;
                }
            }
        }
        public KeithleySmu(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        ~KeithleySmu() { }

        #region iPowerSupply Members

        void IIPowerSupply.Init()
        {
            Reset();
        }

        void IIPowerSupply.DcOn(string strSelection, EPSupplyChannel channel)
        {
            try 
            {
                _myVisaKeithley.IO.WriteString("smu" + channel.ToString() + ".source.output = smu" + channel.ToString() + ".OUTPUT_ON");
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: DcOn -> " + ex.Message);
            }
        }

        void IIPowerSupply.DcOff(string strSelection, EPSupplyChannel channel)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + channel.ToString() + ".source.output = smu" + channel.ToString() + ".OUTPUT_OFF");
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: DcOff -> " + ex.Message);
            }
        }

        void IIPowerSupply.SetNplc(string strSelection, EPSupplyChannel channel, float val)
        {
            try
            {
                if ((val < 0.001) | (val > 25))
                {
                    throw new Exception("KeithleySMU: SetNPLC -> must in range: 0.001 < NPLC < 25");
                }
                else
                {
                    _myVisaKeithley.IO.WriteString("smu" + channel.ToString() + ".measure.nplc = " + val.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: SetNPLC -> " + ex.Message);
            }
        }

        void IIPowerSupply.SetVolt(string strSelection, EPSupplyChannel channel, double volt, double iLimit, EPSupplyVRange vRange)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + channel.ToString() + ".source.func = smu" + channel.ToString() + ".OUTPUT_DCVOLTS");

                VSourceAutoRange((EPSupplyChannel)channel, true);

                _myVisaKeithley.IO.WriteString("smu" + channel.ToString() + ".source.limiti = " + iLimit.ToString());
                _myVisaKeithley.IO.WriteString("smu" + channel.ToString() + ".source.levelv = " + volt.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: iPowerSupply.SetVolt -> " + ex.Message);
            }
        }

        float IIPowerSupply.MeasI(string strSelection, EPSupplyChannel channel, EPSupplyIRange range)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("print(smu" + channel.ToString() + ".measure.i())");
                return (float)Convert.ToDouble(_myVisaKeithley.ReadString());
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: MeasI -> " + ex.Message);
            }
        }

        float IIPowerSupply.MeasV(string strSelection, EPSupplyChannel channel, EPSupplyVRange vRange)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("print(smu" + channel.ToString() + ".measure.v())");
                return (float)Convert.ToDouble(_myVisaKeithley.ReadString());
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: MeasV -> " + ex.Message);
            }
        }

        #endregion



        private void Reset()
        {
            try
            {

                _myVisaKeithley.IO.WriteString("reset()" );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: RESET -> " + ex.Message);
            }
        }
        private void SentCmd(string strCmd)
        {
            try
            {
                _myVisaKeithley.IO.WriteString(strCmd );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: SentCmd -> " + ex.Message);
            }
        }

        private void SetOutput(EPSupplyChannel val, bool @on)
        {
            try
            {
                if (@on)
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.output = smu" + val.ToString() + ".OUTPUT_ON" );
                    
                }
                else
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.output = smu" + val.ToString() + ".OUTPUT_OFF" );
                }
            }
            catch(Exception ex)
            {
                throw new Exception("KeithleySMU: SetOutput -> " + ex.Message);
            }

        }
        private void VMeasAutoRange(EPSupplyChannel val, bool onOff)
        {
            try
            {
                if (onOff)
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".measure.autorangeI = smu" + val.ToString() + "AUTORANGE_ON");
                }
                else
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".measure.autorangeI = smu" + val.ToString() + "AUTORANGE_OFF");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: VMeasAutoRange -> " + ex.Message);
            }
        }
        private void MeasAutoRange(EPSupplyChannel val, bool onOff)
        {
            try
            {
                if (onOff)
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".measure.autorangei = smu" + val.ToString() + ".AUTORANGE_ON");
                }
                else
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".measure.autorangei = smu" + val.ToString() + ".AUTORANGE_OFF");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: IMeasAutoRange -> " + ex.Message);
            }
        }
        private void VSourceAutoRange(EPSupplyChannel val, bool onOff)
        {
            try
            {
                if (onOff)
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.autorangev = smu" + val.ToString() + ".AUTORANGE_ON");
                }
                else
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.autorangev = smu" + val.ToString() + ".AUTORANGE_OFF");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: VSourceAutoRange -> " + ex.Message);
            }
        }
        private void SourceAutoRange(EPSupplyChannel val, bool onOff)
        {
            try
            {
                if (onOff)
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.autorangeI = smu" + val.ToString() + "AUTORANGE_ON");
                }
                else
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.autorangeI = smu" + val.ToString() + "AUTORANGE_OFF");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: ISourceAutoRange -> " + ex.Message);
            }
        }
        private void VSourceSet(EPSupplyChannel val, double dblVoltage, double dblClampI, EPSupplyVRange range)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.func = smu" + val.ToString() + ".OUTPUT_DCVOLTS" );
                if (range != EPSupplyVRange.Auto)
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.rangev = " + VRange_String(range).ToString());
                }
                else
                {
                    VSourceAutoRange(val, true);
                }
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.limiti = " + dblClampI.ToString() );
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.levelv = " + dblVoltage.ToString() );
                
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: VSourceSet -> " + ex.Message);
            }
        }
        private void SourceSet(EPSupplyChannel val, double dblAmps, double dblClampV, EPSupplyIRange range)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.func = smu" + val.ToString() + ".OUTPUT_DCAMPS" );
                if (range != EPSupplyIRange.Auto)
                {
                    _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.rangei = " + IRange_String(range).ToString());
                }
                else
                {
                    SourceAutoRange(val, true);
                }
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.limitv = " + dblClampV.ToString() );
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.leveli = " + dblAmps.ToString() );
                
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: ISourceSet -> " + ex.Message);
            }
        }
        private void VChangeLevel(EPSupplyChannel val, double dblVoltage)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.levelv = " + dblVoltage.ToString() );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: VChangeLevel -> " + ex.Message);
            }
        }
        private void ChangeLevel(EPSupplyChannel val, double dblAmps)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.leveli = " + dblAmps.ToString() );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: IChangeLevel -> " + ex.Message);
            }
        }
        private void Limit(EPSupplyChannel val, double dblAmps)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.limiti = " + dblAmps.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: ILimit -> " + ex.Message);
            }
        }
        private void VLimit(EPSupplyChannel val, double dblVoltage)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("smu" + val.ToString() + ".source.limitv = " + dblVoltage.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: VLimit -> " + ex.Message);
            }
        }
       
        private void DisplayClear()
        {
            try
            {
                _myVisaKeithley.IO.WriteString("display.clear()" );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: DisplayClear -> " + ex.Message);
            }
        }
        private void DisplayVolt(EPSupplyChannel val)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("display.smu" + val.ToString() + ".measure.func = display.MEASURE_DCVOLTS" );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: DisplayVolt -> " + ex.Message);
            }

        }
        private void DisplayAmps(EPSupplyChannel val)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("display.smu" + val.ToString() + ".measure.func = display.MEASURE_DCAMPS");
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: DisplayAmps -> " + ex.Message);
            }

        }
        private void DisplayOhms(EPSupplyChannel val)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("display.smu" + val.ToString() + ".measure.func = display.MEASURE_OHMS" );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: DisplayOhms -> " + ex.Message);
            }

        }
        private void DisplayWatt(EPSupplyChannel val)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("display.smu" + val.ToString() + ".measure.func = display.MEASURE_WATTS" );
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: DisplayWatt -> " + ex.Message);
            }

        }

        private double MeasWatt(EPSupplyChannel val)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("print(smu" + val.ToString() + ".measure.p())" );
                return Convert.ToDouble(_myVisaKeithley.ReadString());
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: MeasWatt -> " + ex.Message);
            }
        }
        private double MeasOhms(EPSupplyChannel val)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("print(smu" + val.ToString() + ".measure.r())" );
                return Convert.ToDouble(_myVisaKeithley.ReadString());
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: MeasOhms -> " + ex.Message);
            }
        }
        private string ReadString(string strCmd)
        {
            try
            {
                _myVisaKeithley.IO.WriteString("print(" + strCmd + ")" );
                return _myVisaKeithley.ReadString();
            }
            catch (Exception ex)
            {
                throw new Exception("KeithleySMU: SentCmd -> " + ex.Message);
            }
        }

        private double VRange_String(EPSupplyVRange val)
        {
            if (val == EPSupplyVRange.Keith260X100MV) return 100e-3;
            if (val == EPSupplyVRange.Keith260X_1V) return 1;
            if (val == EPSupplyVRange.Keith260X40V) return 40;
            if (val == EPSupplyVRange.Keith260X_6V) return 6;
            if (val == EPSupplyVRange.Keith261X200MV) return 200e-3;
            if (val == EPSupplyVRange.Keith261X200V) return 200;
            if (val == EPSupplyVRange.Keith261X20V) return 20;
            if (val == EPSupplyVRange.Keith261X_2V) return 2;
            if (val == EPSupplyVRange.Auto) return 999;
            else return 0;
        }
        private double IRange_String(EPSupplyIRange val)
        {
            if (val == EPSupplyIRange._260x_3A) return 3;
            if (val == EPSupplyIRange._261x_1_5A) return 1.5;
            if (val == EPSupplyIRange._261x_10A) return 10;
            if (val == EPSupplyIRange.All100MA) return 100e-3;
            if (val == EPSupplyIRange.All100NA) return 100e-9;
            if (val == EPSupplyIRange.All100UA) return 100e-6;
            if (val == EPSupplyIRange.All10MA) return 10e-3;
            if (val == EPSupplyIRange.All10UA) return 10e-6;
            if (val == EPSupplyIRange.All_1A) return 1;
            if (val == EPSupplyIRange.All1MA) return 1e-3;
            if (val == EPSupplyIRange.All1UA) return 1e-6;
            if (val == EPSupplyIRange.Auto) return 999;
            else return 0;
        }

    }
}
