using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver.PS
{
    public class E4417A : IIPowerSensor
    {
        public static string ClassName = "E4417A PowerMeter Class";
        private FormattedIO488 _myVisaPm = new FormattedIO488();
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
                return _myVisaPm;
            }
            set
            {
                _myVisaPm = ParseIo;
            }
        }
        public void OpenIo()
        {
            if (IoAddress.Length > 3)
            {
                try
                {
                    ResourceManager mgr = new ResourceManager();
                    _myVisaPm.IO = (IMessage)mgr.Open(IoAddress, AccessMode.NO_LOCK, 2000, "");
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("Class Name: " + ClassName + "\nParameters: OpenIO" + "\n\nErrorDesciption: \n"
                        + ex, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _myVisaPm.IO = null;
                    return;
                }
            }
        }
        //Constructor
        public E4417A(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        E4417A() { }

        #region iSwitch Members

        void IIPowerSensor.Initialize(int chNo)
        {
            try
            {
                _myVisaPm.WriteString("SYST:PRES DEF", true);
                bool dummy = OPERATION_COMPLETE();
            }
            catch (Exception ex)
            {
                throw new Exception("E4417A: Initialize -> " + ex.Message);
            }
        }

        void IIPowerSensor.SetFreq(int chNo, double freqMHz)
        {
            try
            {
                double freqHz = freqMHz * 1e6;
                _myVisaPm.WriteString("SENS" + chNo + ":FREQ " + freqHz, true);
            }
            catch (Exception ex)
            {
                throw new Exception("E4417A: SetFreq -> " + ex.Message);
            }
        }

        void IIPowerSensor.SetOffset(int chNo, double val)
        {
            try
            {
                _myVisaPm.WriteString("CALC" + chNo + ":GAIN " + val, true);
            }
            catch (Exception ex)
            {
                throw new Exception("E4417A: SetPath -> " + ex.Message);
            }
        }

        void IIPowerSensor.EnableOffset(int chNo, bool state)
        {
            try
            {
                switch (state)
                {
                    case true:
                        _myVisaPm.WriteString("CALC" + chNo + ":GAIN:STAT ON", true);
                        break;
                    case false:
                        _myVisaPm.WriteString("CALC" + chNo + ":GAIN:STAT OFF", true);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("E4417A: SetPath -> " + ex.Message);
            }
        }

        float IIPowerSensor.MeasPwr(int chNo)
        {
            try
            {
                return WRITE_READ_SINGLE("FETC" + chNo + "?");
            }
            catch (Exception ex)
            {
                throw new Exception("E4471A:: MeasPwr -> " + ex.Message);
            }
        }

        void IIPowerSensor.Reset()
        {
             try
             {
                 _myVisaPm.WriteString("*CLS; *RST", true);
             }
             catch (Exception ex)
             {
                 throw new Exception("E4417A: Reset -> " + ex.Message);
             }
        }

        #endregion

        public bool OPERATION_COMPLETE()
        {
            try
            {
                bool complete = false;
                double dummy = -99;
                do
                {
                    dummy = WRITE_READ_DOUBLE("*OPC?");
                } while (dummy == 0);
                complete = true;
                return complete;

            }
            catch (Exception ex)
            {
                throw new Exception("E4417A: OPERATION_COMPLETE -> " + ex.Message);
            }
        }

        #region generic READ function

        public double WRITE_READ_DOUBLE(string cmd)
        {
            _myVisaPm.WriteString(cmd, true);
            return Convert.ToDouble(_myVisaPm.ReadString());
        }
        public string WRITE_READ_STRING(string cmd)
        {
            _myVisaPm.WriteString(cmd, true);
            return _myVisaPm.ReadString();
        }
        public float WRITE_READ_SINGLE(string cmd)
        {
            _myVisaPm.WriteString(cmd, true);
            return Convert.ToSingle(_myVisaPm.ReadString());
        }
        public float[] READ_IEEEBlock(IEEEBinaryType type)
        {
            return (float[])_myVisaPm.ReadIEEEBlock(type, true, true);
        }
        public float[] WRITE_READ_IEEEBlock(string cmd, IEEEBinaryType type)
        {
            _myVisaPm.WriteString(cmd, true);
            return (float[])_myVisaPm.ReadIEEEBlock(type, true, true);
        }

        #endregion
    }

}
