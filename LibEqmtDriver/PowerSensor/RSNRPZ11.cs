using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstrumentDrivers;
using System.Windows.Forms;

namespace LibEqmtDriver.PS
{
    public class Rsnrpz11 : IIPowerSensor
    {
        public static Rsnrpz MyRSnrp;
        private static double _previousMeasLength = 0;
        private static int _previousNumAvgs = 0;
        private static bool _isInitialized = false;
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
        //Constructor
        public Rsnrpz11(string ioAddress)
        {
            Address = ioAddress;
        }
        Rsnrpz11() { }

        public void InitializeAndZero()
        {
            try
            {
                //if (isInitialized) return;

                //myRSnrp = new rsnrpz("*", true, true);
                MyRSnrp = new Rsnrpz();
                MyRSnrp.Init(Address, true, true);
                MyRSnrp.Reset();
                //myRSnrp.chan_reset(1);
                MyRSnrp.chan_zero(1);
                _previousMeasLength = 0;

                _isInitialized = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        #region iPowerSensor Members
        void IIPowerSensor.Initialize(int chNo)
        {
            InitializeAndZero();
        }
        void IIPowerSensor.SetFreq(int chNo, double freqMHz)
        {
            SetupMeasurement(freqMHz, 0.001, 10);
        }
        void IIPowerSensor.SetOffset(int chNo, double offset)
        {
            try
            {
                MyRSnrp.corr_setOffset(1, offset);
                MyRSnrp.corr_setOffsetEnabled(1, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        void IIPowerSensor.EnableOffset(int chNo, bool status)
        {
            try
            {
                MyRSnrp.corr_setOffsetEnabled(1, status);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        float IIPowerSensor.MeasPwr(int chNo)
        {
            double[] measDataWatts = new double[1];
            int readCount;
            float measValDbm = -2000;

            try
            {
                MyRSnrp.meass_readBufferMeasurement(1, 50, 1, measDataWatts, out readCount);

                measValDbm = 10f * (float)Math.Log10(1000.0 * measDataWatts[0]);
            }
            catch (Exception e)
            {
                MyRSnrp.chan_abort(1);
                //MessageBox.Show(measValDbm.ToString() + "/n/n" + e.ToString());
            }

            if (float.IsNaN(measValDbm) || (measValDbm < -100 || measValDbm > 100))    // need this in case of NAN or -inifinity
            {
                measValDbm = -999;
            }

            return measValDbm;
        }
        void IIPowerSensor.Reset()
        {

        }
        #endregion

        public static void SetupMeasurement(double measureFreqMHz, double measLengthS, int numAvgs)
        {
            try
            {

                MyRSnrp.chan_mode(1, InstrumentDrivers.RsnrpzConstants.SensorModeTimeslot);
                if (measureFreqMHz > 8000) MyRSnrp.chan_setCorrectionFrequency(1, 8000 * 1e6);
                else MyRSnrp.chan_setCorrectionFrequency(1, measureFreqMHz * 1e6); // Set corr frequency
                MyRSnrp.trigger_setSource(1, InstrumentDrivers.RsnrpzConstants.TriggerSourceImmediate);
                SetupMeasLength(measLengthS);
                SetupNumAverages(numAvgs);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public static void SetupBurstMeasurement(double measureFreqMHz, double measLengthS, double triggerLevDbm, int numAvgs)
        {
            try
            {
                MyRSnrp.chan_mode(1, InstrumentDrivers.RsnrpzConstants.SensorModeTimeslot);
                MyRSnrp.chan_setCorrectionFrequency(1, measureFreqMHz * 1e6); // Set corr frequency
                MyRSnrp.trigger_setSource(1, InstrumentDrivers.RsnrpzConstants.TriggerSourceInternal);
                SetupMeasLength(measLengthS);
                double trigLev = Math.Pow(10.0, triggerLevDbm / 10.0) / 1000.0;
                MyRSnrp.trigger_setLevel(1, trigLev);
                SetupNumAverages(numAvgs);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public static void SetupMeasLength(double measLengthS)
        {
            try
            {
                if (true | measLengthS != _previousMeasLength)
                {
                    MyRSnrp.tslot_configureTimeSlot(1, 1, measLengthS);
                    _previousMeasLength = measLengthS;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public static void SetupNumAverages(int numAvgs)
        {
            try
            {
                if (true | numAvgs != _previousNumAvgs)
                {
                    MyRSnrp.avg_configureAvgManual(1, numAvgs);
                    _previousNumAvgs = numAvgs;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
