using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver.TuneableFilter
{
    public class CKnLD5Bt : IITuneFilterDriver
    {
        public static string ClassName = "KnL D5BT Tuneable Filter Class";
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

        //Constructor
        public CKnLD5Bt(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        CKnLD5Bt() { }

        #region iSwitch Members
        void IITuneFilterDriver.Initialize()
        {
            try
            {
                _myVisaEq.WriteString("SYST:PRES DEF", true);
            }
            catch (Exception ex)
            {
                throw new Exception("KnL D5BT: Initialize -> " + ex.Message);
            }
        }
        double IITuneFilterDriver.ReadFreqMHz()
        {
            string result;
            double freqMHz;
            result = WRITE_READ_STRING("F?");
            result = result.TrimStart('?');               //Remove '?' from data of the 1st character
            freqMHz = (Convert.ToDouble(result))/1e3;     //Convert from KHz to MHz
            return freqMHz;
        }
        void IITuneFilterDriver.SetFreqMHz(double freqMHz)
        {
            double freqKHz = freqMHz * 1e3;
            _myVisaEq.WriteString("F" + freqKHz, true);
        }
        void IITuneFilterDriver.Reset()
        {
        }
        #endregion

        #region generic READ and WRITE function
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
        public void Write(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
        }
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
        public void WriteInt16Array(string command, Int16[] data)
        {
            _myVisaEq.WriteIEEEBlock(command, data, true);
        }

        public void WriteByteArray(string command, byte[] data)
        {
            _myVisaEq.WriteIEEEBlock(command, data, true);
        }
        #endregion
    }
}
