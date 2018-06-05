using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver.SG
{
    public class E8257D : IISiggen
    {
        public static string ClassName = "E8257D Siggen Class";
        private FormattedIO488 _myVisaSg = new FormattedIO488();
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
                return _myVisaSg;
            }
            set
            {
                _myVisaSg = ParseIo;
            }
        }
        public void OpenIo()
        {
            if (IoAddress.Length > 3)
            {
                try
                {
                    ResourceManager mgr = new ResourceManager();
                    _myVisaSg.IO = (IMessage)mgr.Open(IoAddress, AccessMode.NO_LOCK, 2000, "");
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("Class Name: " + ClassName + "\nParameters: OpenIO" + "\n\nErrorDesciption: \n"
                        + ex, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _myVisaSg.IO = null;
                    return;
                }
            }
        }

        public E8257D(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        ~E8257D() { }
        
        #region iSeggen
        void IISiggen.Initialize()
        {
            try
            {
                _myVisaSg.WriteString("*IDN?", true);
                string result = _myVisaSg.ReadString();
            }
            catch (Exception ex)
            {
                throw new Exception("EquipE8257D: Initialization -> " + ex.Message);
            }

        }
        void IISiggen.Reset()
        {
            try
            {
                _myVisaSg.WriteString("*CLS; *RST", true);
            }
            catch (Exception ex)
            {
                throw new Exception("EquipE8257D: RESET -> " + ex.Message);
            }
        }
        void IISiggen.EnableRf(InstrOutput onoff)
        {
            _myVisaSg.WriteString("POW:STAT " + onoff, true);

        }
        void IISiggen.EnableModulation(InstrOutput onoff)
        {
            _myVisaSg.WriteString("OUTP:MOD " + onoff, true);
        }
        void IISiggen.SetAmplitude(float dBm)
        {
            _myVisaSg.WriteString("POW:LEV:IMM:AMPL " + dBm.ToString(), true);
        }
        void IISiggen.SetFreq(double mHz)
        {
            _myVisaSg.WriteString("FREQ:FIX " + mHz.ToString() + "MHz", true);
        }
        void IISiggen.SetPowerMode(N5182PowerMode mode)
        {
            _myVisaSg.WriteString(":POW:MODE " + mode.ToString(), true);
        }
        void IISiggen.SetFreqMode(N5182FrequencyMode mode)
        {
            _myVisaSg.WriteString(":FREQ:MODE " + mode.ToString(), true);
        }
        void IISiggen.MOD_FORMAT_WITH_LOADING_CHECK(string strWaveform, string strWaveformName, bool waveformInitalLoad)
        {
            //Not applicable
        }
        void IISiggen.SELECT_WAVEFORM(N5182AWaveformMode mode)
        {
            //Not applicable
        }
        void IISiggen.SET_LIST_TYPE(N5182ListType mode)
        {
            //myVisaSg.WriteString(":LIST:TYPE " + _mode.ToString(), true);
        }
        void IISiggen.SET_LIST_MODE(InstrMode mode)
        {
            //myVisaSg.WriteString(":LIST:MODE " + _mode.ToString(), true);
        }
        void IISiggen.SET_LIST_TRIG_SOURCE(N5182TrigType mode)
        {
            //myVisaSg.WriteString(":LIST:TRIG:SOUR " + _mode.ToString(), true);
        }
        void IISiggen.SET_CONT_SWEEP(InstrOutput onoff)        // Set up for single sweep
        {
            _myVisaSg.WriteString(":INIT:CONT " + onoff.ToString(), true);
        }
        void IISiggen.SET_START_FREQUENCY(double mHz)
        {
            _myVisaSg.WriteString("FREQ:START " + mHz.ToString() + "MHz", true);
        }
        void IISiggen.SET_STOP_FREQUENCY(float mHz)
        {
            _myVisaSg.WriteString("FREQ:STOP " + mHz.ToString() + "MHz", true);
        }
        void IISiggen.SET_TRIG_TIMERPERIOD(double ms)
        {
            double second = ms / 1e3;
            _myVisaSg.WriteString("SWE:TIME " + second, true);
        }
        void IISiggen.SET_SWEEP_POINT(int points)
        {
            _myVisaSg.WriteString("SWE:POIN " + points.ToString(), true);
        }
        void IISiggen.SINGLE_SWEEP()
        {
            _myVisaSg.WriteString("INIT:IMM", true);
        }
        void IISiggen.SET_SWEEP_PARAM(int points, double ms, double startFreqMHz, double stopFreqMHz)
        {
            double totalSweepT = ms * points;              //calculate dwelltime(mS) per point to total sweepTime(mS)
            _myVisaSg.WriteString("FREQ:START " + startFreqMHz.ToString() + "MHz", true);
            _myVisaSg.WriteString("FREQ:STOP " + stopFreqMHz.ToString() + "MHz", true);
            _myVisaSg.WriteString("SWE:POIN " + points.ToString(), true);
            _myVisaSg.WriteString("SWE:TIME " + totalSweepT.ToString() + "ms", true);
        }
        bool IISiggen.OPERATION_COMPLETE()
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
                throw new Exception("E8257D: OPERATION_COMPLETE -> " + ex.Message);
            }
        }
        #endregion

        public string QUERY_ERROR()
        {
            string errMsg, tempErrMsg = "";
            int errNum;
            try
            {
                errMsg = WRITE_READ_STRING("SYST:ERR?");
                tempErrMsg = errMsg;
                // Remove the error number
                errNum = Convert.ToInt16(errMsg.Remove((errMsg.IndexOf(",")),
                    (errMsg.Length) - (errMsg.IndexOf(","))));
                if (errNum != 0)
                {
                    while (errNum != 0)
                    {
                        tempErrMsg = errMsg;

                        // Check for next error(s)
                        errMsg = WRITE_READ_STRING("SYST:ERR?");

                        // Remove the error number
                        errNum = Convert.ToInt16(errMsg.Remove((errMsg.IndexOf(",")),
                            (errMsg.Length) - (errMsg.IndexOf(","))));
                    }
                }
                return tempErrMsg;
            }
            catch (Exception ex)
            {
                throw new Exception("EquipE8257D: QUERY_ERROR --> " + ex.Message);
            }
        }

        #region generic READ and WRITE function
        public float WRITE_READ_SINGLE(string cmd)
        {
            _myVisaSg.WriteString(cmd, true);
            return Convert.ToSingle(_myVisaSg.ReadString());
        }
        public float[] READ_IEEEBlock(IEEEBinaryType type)
        {
            return (float[])_myVisaSg.ReadIEEEBlock(type, true, true);
        }
        public float[] WRITE_READ_IEEEBlock(string cmd, IEEEBinaryType type)
        {
            _myVisaSg.WriteString(cmd, true);
            return (float[])_myVisaSg.ReadIEEEBlock(type, true, true);
        }
        public void Write(string cmd)
        {
            _myVisaSg.WriteString(cmd, true);
        }
        public double WRITE_READ_DOUBLE(string cmd)
        {
            _myVisaSg.WriteString(cmd, true);
            return Convert.ToDouble(_myVisaSg.ReadString());
        }
        public string WRITE_READ_STRING(string cmd)
        {
            _myVisaSg.WriteString(cmd, true);
            return _myVisaSg.ReadString();
        }
        public void WriteInt16Array(string command, Int16[] data)
        {
            _myVisaSg.WriteIEEEBlock(command, data, true);
        }

        public void WriteByteArray(string command, byte[] data)
        {
            _myVisaSg.WriteIEEEBlock(command, data, true);
        }
        #endregion
    }
}
