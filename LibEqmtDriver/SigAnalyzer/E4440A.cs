using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa.Interop;
using System.Threading;
using System.Windows.Forms;

namespace LibEqmtDriver.SA
{
    public class E4440A : IISigAnalyzer
    {
        public static string ClassName = "E4440A MXA Class";
        private FormattedIO488 _myVisaSa = new FormattedIO488();
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
                return _myVisaSa;
            }
            set
            {
                _myVisaSa = ParseIo;
            }
        }
        public void OpenIo()
        {
            if (IoAddress.Length > 3)
            {
                try
                {
                    ResourceManager mgr = new ResourceManager();
                    _myVisaSa.IO = (IMessage)mgr.Open(IoAddress, AccessMode.NO_LOCK, 2000, "");
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("Class Name: " + ClassName + "\nParameters: OpenIO" + "\n\nErrorDesciption: \n"
                        + ex, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _myVisaSa.IO = null;
                    return;
                }
            }
        }
        public E4440A(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        ~E4440A() { }

        #region iSigAnalyzer
        public void Initialize(int equipId)
        {
            _myVisaSa.WriteString(":INST SA", true);

            Reset();
            if (equipId == 1)   // PSA E4440A
            {
                _myVisaSa.WriteString("CORR:CSET1 OFF", true);
                _myVisaSa.WriteString("CORR:CSET2 OFF", true);
                _myVisaSa.WriteString("CORR:CSET3 OFF", true);
                _myVisaSa.WriteString("CORR:CSET4 OFF", true);
                _myVisaSa.WriteString("CORR:CSET:ALL OFF", true);
                _myVisaSa.WriteString("CORR:OFFS:MAGN 0", true);
                _myVisaSa.WriteString("CALC:MARK:FUNC OFF", true);
            }
            else if (equipId == 3) // MXA
                _myVisaSa.WriteString(":CORR:SA:GAIN 0", true);

            _myVisaSa.WriteString(":FORM:DATA REAL,32", true);
            _myVisaSa.WriteString(":INIT:CONT 1", true);
            _myVisaSa.WriteString(":BWID:VID:RAT " + "10", true);
            _myVisaSa.WriteString("SWE:POIN 301", true);
            _myVisaSa.WriteString("SENS:ROSC:SOUR EXT", true);
            _myVisaSa.WriteString("SENS:ROSC:OUTP OFF", true);
        }

        public void Preset()
        {
            _myVisaSa.WriteString("SYST:PRES", true);
        }

        void IISigAnalyzer.Select_Instrument(N9020AInstrumentMode mode)
        {
            switch (mode)
            {
                case N9020AInstrumentMode.SpectrumAnalyzer: _myVisaSa.WriteString("INST:SEL SA", true); break;
                case N9020AInstrumentMode.Basic: _myVisaSa.WriteString("INST:SEL BASIC", true); break;
                case N9020AInstrumentMode.Wcdma: _myVisaSa.WriteString("INST:SEL WCDMA", true); break;
                case N9020AInstrumentMode.Wimax: _myVisaSa.WriteString("INST:SEL WIMAXOFDMA", true); break;
                case N9020AInstrumentMode.EdgeGsm: _myVisaSa.WriteString("INST:SEL EDGEGSM", true); break;
                default: throw new Exception("Not such a intrument mode!");
            }
        }

        void IISigAnalyzer.Select_Triggering(N9020ATriggeringType type)
        {
            switch (type)
            {
                ///******************************************
                /// SweptSA mode trigerring
                ///******************************************
                case N9020ATriggeringType.RfExt1: _myVisaSa.WriteString("TRIG:SOUR EXT1", true); break;
                case N9020ATriggeringType.RfExt2: _myVisaSa.WriteString("TRIG:SOUR EXT2", true); break;
                case N9020ATriggeringType.RfRfBurst: _myVisaSa.WriteString("TRIG:SOUR RFB", true); break;
                case N9020ATriggeringType.RfVideo: _myVisaSa.WriteString("TRIG:SOUR VID", true); break;
                case N9020ATriggeringType.RfFreeRun: _myVisaSa.WriteString("TRIG:SOUR IMM", true); break;

                ///******************************************
                /// EDGEGSM Transmit power type trigerring
                ///******************************************


                ///******************************************
                /// EDGEGSM Power Vs Time type trigerring
                ///******************************************


                ///******************************************
                /// EDGEGSM Power Vs Time type trigerring
                ///******************************************


                ///******************************************
                /// EDGEGSM Edge Power Vs Time type trigerring
                ///******************************************


                ///******************************************
                /// EDGEGSM Edge EVM type trigerring
                ///******************************************

                default: throw new Exception("Not such a Trigger Mode!");
            }
        }

        void IISigAnalyzer.Measure_Setup(N9020AMeasType type)
        {
            switch (type)
            {
                case N9020AMeasType.SweptSa: _myVisaSa.WriteString(":INIT:SAN", true); break;
                case N9020AMeasType.ChanPower: _myVisaSa.WriteString(":INIT:CHP", true); break;
                case N9020AMeasType.Acp: _myVisaSa.WriteString(":CONF:ACP:NDEF", true); break;
                case N9020AMeasType.BTxPow: _myVisaSa.WriteString(":INIT:TXP", true); break;
                case N9020AMeasType.GPowVtm: _myVisaSa.WriteString(":INIT:PVT", true); break;
                case N9020AMeasType.GpHaseFreq: _myVisaSa.WriteString(":INIT:PFER", true); break;
                case N9020AMeasType.GOutRfSpec: _myVisaSa.WriteString(":INIT:ORFS", true); break;
                case N9020AMeasType.GTxSpur: _myVisaSa.WriteString(":INIT:TSP", true); break;
                case N9020AMeasType.EPowVtm: _myVisaSa.WriteString(":INIT:EPVT", true); break;
                case N9020AMeasType.Eevm: _myVisaSa.WriteString(":INIT:EEVM", true); break;
                case N9020AMeasType.EOutRfSpec: _myVisaSa.WriteString(":INIT:EORF", true); break;
                case N9020AMeasType.ETxSpur: _myVisaSa.WriteString(":INIT:ETSP", true); break;
                case N9020AMeasType.MonitorSpec: break;

                default: throw new Exception("Not such a Measure setup type!");
            }
        }

        void IISigAnalyzer.Enable_Display(N9020ADisplay onoff)
        {
            _myVisaSa.WriteString(":DISP:ENAB " + onoff, true);
        }

        void IISigAnalyzer.VBW_RATIO(double ratio)
        {
            _myVisaSa.WriteString("BAND:VID:RAT " + ratio.ToString(), true);
        }

        void IISigAnalyzer.Span(double freqMHz)
        {
            _myVisaSa.WriteString("FREQ:SPAN " + freqMHz.ToString() + " MHz", true);
        }

        void IISigAnalyzer.MARKER_TURN_ON_NORMAL_POINT(int markerNum, float markerFreqMHz)
        {
            _myVisaSa.WriteString("CALC:MARK" + markerNum.ToString() + ":MODE POS", true);
            _myVisaSa.WriteString("CALC:MARK" + markerNum.ToString() + ":X " + markerFreqMHz.ToString() + " MHz", true);
        }

        void IISigAnalyzer.TURN_ON_INTERNAL_PREAMP()
        {
            _myVisaSa.WriteString("POW:GAIN ON", true);
            _myVisaSa.WriteString("POW:GAIN:BAND FULL", true);
        }

        void IISigAnalyzer.TURN_OFF_INTERNAL_PREAMP()
        {
            _myVisaSa.WriteString("POW:GAIN OFF", true);
        }

        void IISigAnalyzer.TURN_OFF_MARKER()
        {
            _myVisaSa.WriteString(":CALC:MARK:AOFF", true);
        }

        double IISigAnalyzer.READ_MARKER(int markerNum)
        {
            return WRITE_READ_DOUBLE("CALC:MARK" + markerNum.ToString() + ":Y?");
        }

        void IISigAnalyzer.SWEEP_TIMES(int sweeptimeMs)
        {
            _myVisaSa.WriteString(":SWE:TIME " + sweeptimeMs.ToString() + " ms", true);
        }

        void IISigAnalyzer.SWEEP_POINTS(int sweepPoints)
        {
            _myVisaSa.WriteString(":SWE:POIN " + sweepPoints.ToString(), true);
        }

        void IISigAnalyzer.CONTINUOUS_MEASUREMENT_ON()
        {
            _myVisaSa.WriteString("INIT:CONT ON", true);
        }

        void IISigAnalyzer.CONTINUOUS_MEASUREMENT_OFF()
        {
            _myVisaSa.WriteString("INIT:CONT OFF", true);
        }

        void IISigAnalyzer.RESOLUTION_BW(double bw)
        {
            _myVisaSa.WriteString(":BAND " + bw.ToString(), true);
        }

        double IISigAnalyzer.MEASURE_PEAK_POINT(int delayMs)
        {
            _myVisaSa.WriteString("CALC:MARK:MAX", true);
            Thread.Sleep(delayMs);
            bool status = Operation_Complete();
            return WRITE_READ_DOUBLE("CALC:MARK:Y?");
        }

        double IISigAnalyzer.MEASURE_PEAK_FREQ(int delayMs)
        {
            _myVisaSa.WriteString("CALC:MARK:MAX", true);
            Thread.Sleep(delayMs);
            bool status = Operation_Complete();
            return WRITE_READ_DOUBLE("CALC:MARK:X?");
        }

        void IISigAnalyzer.VIDEO_BW(double vbwHz)
        {
            _myVisaSa.WriteString(":BAND:VID " + vbwHz, true);
        }

        void IISigAnalyzer.TRIGGER_CONTINUOUS()
        {
            _myVisaSa.WriteString("INIT:CONT ON", true);
        }

        void IISigAnalyzer.TRIGGER_SINGLE()
        {
            _myVisaSa.WriteString("INIT:CONT OFF", true);
        }

        void IISigAnalyzer.TRIGGER_IMM()
        {
            _myVisaSa.WriteString("INIT:IMM", true);
        }

        void IISigAnalyzer.TRACE_AVERAGE(int avg)
        {
            _myVisaSa.WriteString(":AVERage:COUN " + avg.ToString(), true);
        }

        void IISigAnalyzer.AVERAGE_OFF()
        {
            _myVisaSa.WriteString(":AVER:STAT OFF", true);
        }

        void IISigAnalyzer.AVERAGE_ON()
        {
            _myVisaSa.WriteString(":AVER:STAT ON", true);
        }

        void IISigAnalyzer.SET_TRACE_DETECTOR(string mode)
        {
            switch (mode.ToUpper())
            {
                case "WRIT":
                case "WRITE":
                    _myVisaSa.WriteString("TRAC:MODE WRIT", true);
                    break;
                case "MAXH":
                case "MAXHOLD":
                    _myVisaSa.WriteString("TRAC:MODE MAXH", true);
                    break;
                case "MINH":
                case "MINHOLD":
                    _myVisaSa.WriteString("TRAC:MODE MINH", true);
                    break;
            }
        }

        void IISigAnalyzer.CLEAR_WRITE()
        {
            _myVisaSa.WriteString("TRAC:MODE WRIT", true);
        }

        void IISigAnalyzer.AMPLITUDE_REF_LEVEL_OFFSET(double refLvlOffset)
        {
            _myVisaSa.WriteString("DISP:WIND:TRAC:Y:RLEV:OFFS " + refLvlOffset, true);
        }

        void IISigAnalyzer.AMPLITUDE_REF_LEVEL(double refLvl)
        {
            _myVisaSa.WriteString("DISP:WIND:TRAC:Y:RLEV " + refLvl, true);
        }

        void IISigAnalyzer.AMPLITUDE_INPUT_ATTENUATION(double inputAttenuation)
        {
            _myVisaSa.WriteString("POW:ATT " + inputAttenuation, true);
        }

        void IISigAnalyzer.AUTO_ATTENUATION(bool state)
        {
            if (state)
            {
                _myVisaSa.WriteString(":SENS:POW:ATT:AUTO ON", true);
            }
            else
            {
                _myVisaSa.WriteString(":SENS:POW:ATT:AUTO OFF", true);
            }
        }

        void IISigAnalyzer.ELEC_ATTENUATION(float inputAttenuation)
        {
            _myVisaSa.WriteString("POW:EATT " + inputAttenuation, true);
        }

        void IISigAnalyzer.ELEC_ATTEN_ENABLE(bool inputStat)
        {
            if (inputStat)
                _myVisaSa.WriteString("POW:EATT:STAT ON", true);
            else
                _myVisaSa.WriteString("POW:EATT:STAT OFF", true);
        }

        void IISigAnalyzer.ALIGN_PARTIAL()
        {
            _myVisaSa.WriteString(":CAL:AUTO PART", true);
        }

        void IISigAnalyzer.ALIGN_ONCE()
        {
            _myVisaSa.WriteString(":CAL:AUTO ONCE", true);
        }

        void IISigAnalyzer.AUTOALIGN_ENABLE(bool inputStat)
        {
            if (inputStat)
                _myVisaSa.WriteString(":CAL:AUTO ON", true);
            else
                _myVisaSa.WriteString(":CAL:AUTO OFF", true);
        }

        void IISigAnalyzer.Cal()
        {
            _myVisaSa.WriteString(":CAL", true);
        }

        bool IISigAnalyzer.OPERATION_COMPLETE()
        {
            bool complete = false;
            double dummy = -9;
            do
            {
                //timer.wait(2);
                dummy = WRITE_READ_DOUBLE("*OPC?");
            } while (dummy == 0);
            complete = true;
            return complete;
        }

        void IISigAnalyzer.START_FREQ(string strFreq, string strUnit)
        {
            _myVisaSa.WriteString("SENS:FREQ:STAR " + strFreq + strUnit, true);
        }

        void IISigAnalyzer.STOP_FREQ(string strFreq, string strUnit)
        {
            _myVisaSa.WriteString("SENS:FREQ:STOP " + strFreq + strUnit, true);
        }

        void IISigAnalyzer.FREQ_CENT(string strSaFreq, string strUnit)
        {
            _myVisaSa.WriteString(":FREQ:CENT " + strSaFreq + strUnit, true);
        }

        string IISigAnalyzer.READ_MXATrace(int traceNum)
        {
            _myVisaSa.WriteString(":FORM ASC", true);
            return WRITE_READ_STRING(":TRAC? TRACE" + traceNum.ToString());
        }

        double IISigAnalyzer.READ_STARTFREQ()
        {
            return WRITE_READ_DOUBLE("SENS:FREQ:STAR?");
        }

        double IISigAnalyzer.READ_STOPFREQ()
        {
            return WRITE_READ_DOUBLE("SENS:FREQ:STOP?");
        }

        float IISigAnalyzer.READ_SWEEP_POINTS()
        {
            return WRITE_READ_SINGLE(":SWE:POIN?");
        }

        float[] IISigAnalyzer.IEEEBlock_READ_MXATrace(int traceNum)
        {
            float[] arrSaTraceData = null;
            _myVisaSa.WriteString(":FORM:DATA REAL,32", true);
            arrSaTraceData = WRITE_READ_IEEEBlock(":TRAC? TRACE" + traceNum.ToString(), IEEEBinaryType.BinaryType_R4);

            _myVisaSa.WriteString(":FORM ASC", true);
            return arrSaTraceData;
        }

        void IISigAnalyzer.EXT_AMP_GAIN(double gain)
        {
            _myVisaSa.WriteString("CORR:OFFS:MAGN " + gain, true);
        }

        void IISigAnalyzer.Select_MarkerFunc(N9020AMarkerFunc type)
        {
            switch (type)
            {
                case N9020AMarkerFunc.Off: _myVisaSa.WriteString("CALC:MARK:FUNC OFF", true); break;
                case N9020AMarkerFunc.Bandpow: _myVisaSa.WriteString("CALC:MARK:FUNC BPOW", true); break;
                case N9020AMarkerFunc.Banddensity: _myVisaSa.WriteString("CALC:MARK:FUNC BDEN", true); break;
                case N9020AMarkerFunc.Noise: _myVisaSa.WriteString("CALC:MARK:FUNC NOIS", true); break;

                default: throw new Exception("Not such a Marker Function type!");
            }
        }
        #endregion

        public bool Operation_Complete()
        {
            bool complete = false;
            double dummy = -9;
            do
            {
                //timer.wait(2);
                dummy = WRITE_READ_DOUBLE("*OPC?");
            } while (dummy == 0);
            complete = true;
            return complete;
        }
        public void Reset()
        {
            try
            {
                _myVisaSa.WriteString("*CLS; *RST", true);
            }
            catch (Exception ex)
            {
                throw new Exception("EquipSA: RESET -> " + ex.Message);
            }
        }
        #region READ and WRITE function
        public string READ_STRING()
        {
            return _myVisaSa.ReadString();
        }
        public void Write(string cmd)
        {
            _myVisaSa.WriteString(cmd, true);
        }
        public double WRITE_READ_DOUBLE(string cmd)
        {
            _myVisaSa.WriteString(cmd, true);
            return Convert.ToDouble(_myVisaSa.ReadString());
        }
        public string WRITE_READ_STRING(string cmd)
        {
            _myVisaSa.WriteString(cmd, true);
            return _myVisaSa.ReadString();
        }
        public float WRITE_READ_SINGLE(string cmd)
        {
            _myVisaSa.WriteString(cmd, true);
            return Convert.ToSingle(_myVisaSa.ReadString());
        }
        public float[] READ_IEEEBlock(IEEEBinaryType type)
        {
            return (float[])_myVisaSa.ReadIEEEBlock(type, true, true);
        }
        public float[] WRITE_READ_IEEEBlock(string cmd, IEEEBinaryType type)
        {
            _myVisaSa.WriteString(cmd, true);
            return (float[])_myVisaSa.ReadIEEEBlock(type, true, true);
        }
        #endregion
    }
}
