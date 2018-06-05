using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver.DC
{
    public class Ps662xA : IIDcSupply
    {
        public static string ClassName = "PS662xA 4-Channel PowerSupply Class";
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
        public Ps662xA(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        ~Ps662xA() { }

        #region iDCSupply Members

        void IIDcSupply.Init()
        {
            Reset();
        }

        void IIDcSupply.DcOn(int channel)
        {
            try
            {
                _myVisaEq.IO.WriteString("OUT " + channel.ToString() + ",1");
            }
            catch (Exception ex)
            {
                throw new Exception("PS662xA: DcOn -> " + ex.Message);
            }
        }
        void IIDcSupply.DcOff(int channel)
        {
            try
            {
                _myVisaEq.IO.WriteString("OUT " + channel.ToString() + ",0");
            }
            catch (Exception ex)
            {
                throw new Exception("PS662xA: DcOff -> " + ex.Message);
            }
        }

        void IIDcSupply.SetVolt(int channel, double volt, double iLimit)
        {
            VSourceSet(channel, volt);
            SourceSet(channel, iLimit);
        }

        float IIDcSupply.MeasI(int channel)
        {
            try
            {
                return WRITE_READ_SINGLE("IOUT? " + channel.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PS662xA:: MeasI -> " + ex.Message);
            }
        }

        float IIDcSupply.MeasV(int channel)
        {
            try
            {
                return WRITE_READ_SINGLE("VOUT? " + channel.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PS662xA:: MeasV -> " + ex.Message);
            }
        }

        #endregion

        private void Reset()
        {
            try
            {
                _myVisaEq.WriteString("CLR", true);
                for (int i = 1; i < 5; i++)
                {
                    _myVisaEq.WriteString("OUT " + i.ToString() + ",0", true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PS662xA: Initialize -> " + ex.Message);
            }
        }

        private void VSourceSet(int val, double dblVoltage)
        {
            try
            {
                _myVisaEq.IO.WriteString("VSET " + val.ToString() + "," + dblVoltage.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PS662xA: VSourceSet -> " + ex.Message);
            }
        }
        private void SourceSet(int val, double dblAmps)
        {
            try
            {
                _myVisaEq.IO.WriteString("ISET " + val.ToString() + "," + dblAmps.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PS662xA: ISourceSet -> " + ex.Message);
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
