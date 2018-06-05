using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver.DC_1CH
{
    public class E3633A : IIDcSupply_1Ch
    {
        public static string ClassName = "E3633A/E3644A 1-Channel PowerSupply Class";
        private FormattedIO488 _myVisaEq = new FormattedIO488();
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
                return _myVisaEq;
            }
            set
            {
                _myVisaEq = ParseIo;
            }
        }
        public void OpenIo()
        {
            if (IoAddress.Length > 3)
            {
                try
                {
                    ResourceManager mgr = new ResourceManager();
                    _myVisaEq.IO = (IMessage)mgr.Open(IoAddress, AccessMode.NO_LOCK, 2000, "");
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("Class Name: " + ClassName + "\nParameters: OpenIO" + "\n\nErrorDesciption: \n"
                        + ex, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _myVisaEq.IO = null;
                    return;
                }
            }
        }
        public E3633A(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        ~E3633A() { }

        #region iDCSupply_1CH Members

        void IIDcSupply_1Ch.Init()
        {
            Reset();
        }

        void IIDcSupply_1Ch.DcOn(int channel)
        {
            try
            {
                _myVisaEq.IO.WriteString("OUTP ON");
            }
            catch (Exception ex)
            {
                throw new Exception("E3633A: DcOn -> " + ex.Message);
            }
        }
        void IIDcSupply_1Ch.DcOff(int channel)
        {
            try
            {
                _myVisaEq.IO.WriteString("OUTP OFF");
            }
            catch (Exception ex)
            {
                throw new Exception("E3633A: DcOff -> " + ex.Message);
            }
        }

        void IIDcSupply_1Ch.SetVolt(int channel, double volt, double iLimit)
        {
            SourceSet(channel, iLimit);
            VSourceSet(channel, volt);
        }

        float IIDcSupply_1Ch.MeasI(int channel)
        {
            try
            {
                return WRITE_READ_SINGLE("MEAS:CURR?");
            }
            catch (Exception ex)
            {
                throw new Exception("E3633A:: MeasI -> " + ex.Message);
            }
        }

        float IIDcSupply_1Ch.MeasV(int channel)
        {
            try
            {
                return WRITE_READ_SINGLE("MEAS:VOLT?");
            }
            catch (Exception ex)
            {
                throw new Exception("E3633A:: MeasV -> " + ex.Message);
            }
        }

        #endregion

        private void Reset()
        {
            try
            {
                _myVisaEq.WriteString("*CLS; *RST", true);
            }
            catch (Exception ex)
            {
                throw new Exception("E3633A: Initialize -> " + ex.Message);
            }
        }

        private void VSourceSet(int val, double dblVoltage)
        {
            try
            {
                _myVisaEq.IO.WriteString("VOLT " + dblVoltage.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("E3633A: VSourceSet -> " + ex.Message);
            }
        }
        private void SourceSet(int val, double dblAmps)
        {
            try
            {
                _myVisaEq.IO.WriteString("CURR " + dblAmps.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("E3633A: ISourceSet -> " + ex.Message);
            }
        }

        #region generic READ function

        public double WRITE_READ_DOUBLE(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
            return Convert.ToDouble(_myVisaEq.ReadString());
        }
        public string WRITE_READ_STRING(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
            return _myVisaEq.ReadString();
        }
        public float WRITE_READ_SINGLE(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
            return Convert.ToSingle(_myVisaEq.ReadString());
        }
        public float[] READ_IEEEBlock(IEEEBinaryType type)
        {
            return (float[])_myVisaEq.ReadIEEEBlock(type, true, true);
        }
        public float[] WRITE_READ_IEEEBlock(string cmd, IEEEBinaryType type)
        {
            _myVisaEq.WriteString(cmd, true);
            return (float[])_myVisaEq.ReadIEEEBlock(type, true, true);
        }

        #endregion

    }
}
