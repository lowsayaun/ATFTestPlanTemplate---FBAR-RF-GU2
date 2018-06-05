using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.Interop;

namespace LibEqmtDriver.SMU
{
    public class NiPxismu : IIPowerSupply
    {

        public NiPxismu(string[] val)
        {
            //Note : val data must be in this format Px_CHy_NIxxxx (eg P1_CH1_NI4143) , will decode the NI SMU aliasname (eg. NI4143_P1)
            //Where Px = part SMU aliasname
            //Where CHx = which SMU channel to set (eg NI4143 has 4x CH)
            //Where NIxxxx = which SMU model

            string tempVal = "";

            for (int i = 0; i < val.Length; i++)
            {
                string visaAlias;
                string chNum;
                string pinName;

                string[] arSelected = new string[4];
                tempVal = val[i];
                arSelected = tempVal.Split('_');

                visaAlias = arSelected[3];
                pinName = tempVal;
                chNum = arSelected[1].Substring(2, 1);

                GetSmu(visaAlias, chNum, pinName, true);
            }  
        }
        ~NiPxismu() { }

        
        public static Dictionary<string, IISmu> SmuResources = new Dictionary<string, IISmu>();

        public static IISmu GetSmu(string visaAlias, string chanNumber, string pinName, bool reset)
        {
            IISmu smu;

            if (visaAlias.Contains("4154"))
            {
                smu = new Ni4154(visaAlias, chanNumber, pinName, reset);
            }
            else if (visaAlias.Contains("4139"))
            {
                smu = new Ni4139(visaAlias, chanNumber, pinName, reset);
            }
            else if (visaAlias.Contains("4143") || visaAlias.Contains("4141"))
            {
                smu = new Ni4143(visaAlias, chanNumber, pinName, reset);
            }
            else
            {
                throw new Exception("Visa Alias \"" + visaAlias + "\" is not in a recognized format.\nValid SMU Visa Aliases must include one of the following:\n"
                    + "\n\"4154\""
                    + "\n\"4139\""
                    + "\n\"4143\""
                    + "\n\"4141\""
                    + "\n\nFor example, Visa Alias \"SMU_NI4143_02\" will be recognized as an NI 4143 module.");
            }

            SmuResources.Add(pinName, smu);

            return smu;
        }

        #region iPowerSupply

        void IIPowerSupply.Init()
        {
            throw new NotImplementedException();
        }

        void IIPowerSupply.DcOn(string strSelection, EPSupplyChannel channel)
        {
            SmuResources[strSelection].OutputEnable(true);
        }

        void IIPowerSupply.DcOff(string strSelection, EPSupplyChannel channel)
        {
            //SmuResources[strSelection].ForceVoltage(0.0, 1e-6);      //force voltage to 0V and very small current (cannot be zero)
            SmuResources[strSelection].OutputEnable(false);
        }

        void IIPowerSupply.SetNplc(string strSelection, EPSupplyChannel channel, float val)
        {

        }

        void IIPowerSupply.SetVolt(string strSelection, EPSupplyChannel channel, double volt, double iLimit, EPSupplyVRange vRange)
        {
            SmuResources[strSelection].ForceVoltage(volt, iLimit);
        }

        float IIPowerSupply.MeasI(string strSelection, EPSupplyChannel channel, EPSupplyIRange range)
        {
            double imeas = -999;
            SmuResources[strSelection].SetupCurrentMeasure(false, false);
            SmuResources[strSelection].MeasureCurrent(1, false, ref imeas);
            return Convert.ToSingle(imeas);
        }

        float IIPowerSupply.MeasV(string strSelection, EPSupplyChannel channel, EPSupplyVRange vRange)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #region iSMU class
    public class Ni4154 : IISmu
    {
        public nidcpower SmUsession;
        public string VisaAlias { get; set; }
        public string ChanNumber { get; set; }
        public string PinName { get; set; }
        public double PriorVoltage { get; set; }
        public double PriorCurrentLim { get; set; }

        public Ni4154(string visaAlias, string chanNumber, string pinName, bool reset)
        {
            try
            {
                this.SmUsession = new NationalInstruments.ModularInstruments.Interop.nidcpower(visaAlias, chanNumber, reset, "");

                this.VisaAlias = visaAlias;
                this.ChanNumber = chanNumber;
                this.PinName = pinName;

                string model = SmUsession.GetString(NationalInstruments.ModularInstruments.Interop.nidcpowerProperties.InstrumentModel);

                SmUsession.SetDouble(nidcpowerProperties.SourceDelay, chanNumber, 0.00003);
                SmUsession.ConfigureOutputEnabled(chanNumber, false);
                SmUsession.ConfigureOutputFunction(chanNumber, nidcpowerConstants.DcVoltage);
                SmUsession.ConfigureSense(chanNumber, nidcpowerConstants.Remote);
                SmUsession.SetInt32(nidcpowerProperties.CurrentLimitAutorange, chanNumber, nidcpowerConstants.On);
                SmUsession.SetInt32(nidcpowerProperties.VoltageLevelAutorange, chanNumber, nidcpowerConstants.On);
                SmUsession.ConfigureVoltageLevel(chanNumber, 0);
                SmUsession.ConfigureCurrentLimit(chanNumber, nidcpowerConstants.CurrentRegulate, 0.01);
                SmUsession.ConfigureOutputEnabled(chanNumber, true);
                SmUsession.Initiate();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SMU Initialize");
            }
        }

        public void ForceVoltage(double voltsForce, double currentLimit)
        {
            int error = -1;
            int chanInt = Convert.ToInt16(ChanNumber);

            try
            {
                if (currentLimit != PriorCurrentLim)
                {
                    // ConfigureCurrentLimit appears to also set the range automatically, because auto-range is on
                    error = SmUsession.ConfigureCurrentLimit(ChanNumber, nidcpowerConstants.CurrentRegulate, currentLimit);
                    PriorCurrentLim = currentLimit;

                    //double readRange = SMUsession.GetDouble(nidcpowerProperties.CurrentLimit);
                }

                if (voltsForce != PriorVoltage)
                {
                    error = SmUsession.ConfigureVoltageLevel(ChanNumber, voltsForce);
                    PriorVoltage = voltsForce;
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "ForceVoltage4154");
            }
        }

        public void SetupCurrentMeasure(bool useIQsettings, bool triggerFromSa)
        {
            try
            {
                SmUsession.Abort();

                double measureTimeLength = 0;


                measureTimeLength = 0.0005;
                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnDemand);
                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, 1);


                double dcSampleRate = 200e3;   // this is fixed for NI hardware
                int samplesToAvg = (int)(dcSampleRate * measureTimeLength);

                SmUsession.SetInt32(nidcpowerProperties.SamplesToAverage, samplesToAvg);

                SmUsession.Initiate();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupCurrentMeasure4154");

            }
        }
        public void MeasureCurrent(int numAverages, bool triggered, ref double result)
        {
            int error = -1;

            try
            {
                double[] volt = new double[numAverages];
                double[] curr = new double[numAverages];
                ushort[] measComp = new ushort[numAverages];
                int actCount = 0;
                double[] voltSingle = new double[1];
                double[] currSingle = new double[1];

                for (int avg = 0; avg < numAverages; avg++)
                {
                    if (triggered)
                    {
                        error = SmUsession.FetchMultiple(ChanNumber, 1, 1, voltSingle, currSingle, measComp, out actCount);  // "Count" doesn't wait to re-trigger, just measures all immediately after 1st trigger

                        volt[avg] = voltSingle[0];
                        curr[avg] = currSingle[0];
                    }
                    else
                    {
                        error = SmUsession.Measure(ChanNumber, nidcpowerConstants.MeasureCurrent, out curr[avg]);
                    }
                }

                result = curr.Average();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "MeasureCurrent4154");

            }
        }

        int _numTraceSamples;
        public void SetupCurrentTraceMeasurement(double measureTimeLength, double aperture, bool triggered, bool triggerFromSa = false)
        {
            int error = -1;

            try
            {
                SmUsession.Abort();

                double dcSampleRate = 200e3;   // this is fixed for NI hardware
                _numTraceSamples = (int)(dcSampleRate * measureTimeLength);

                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnMeasureTrigger);

                if (triggered)
                {
                    SmUsession.SetInt32(nidcpowerProperties.MeasureTriggerType, nidcpowerConstants.DigitalEdge);
                    if (triggerFromSa) SmUsession.SetString(nidcpowerProperties.DigitalEdgeMeasureTriggerInputTerminal, "PXI_Trig1");
                    else SmUsession.SetString(nidcpowerProperties.DigitalEdgeMeasureTriggerInputTerminal, "PXI_Trig0");
                }
                else
                {
                    SmUsession.SetInt32(nidcpowerProperties.MeasureTriggerType, nidcpowerConstants.SoftwareEdge);
                }

                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, _numTraceSamples);
                SmUsession.SetDouble(nidcpowerProperties.SourceDelay, ChanNumber, 0.001);

                SmUsession.SetInt32(nidcpowerProperties.SamplesToAverage, 1);

                SmUsession.Initiate();

                SmUsession.WaitForEvent(nidcpowerConstants.SourceCompleteEvent, 0.02);

                if (!triggered) SmUsession.SendSoftwareEdgeTrigger(nidcpowerConstants.MeasureTrigger);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupCurrentTraceMeasurement4154");

            }
        }
        public double[] MeasureCurrentTrace()
        {
            int error = -1;

            try
            {
                ushort[] measComp = new ushort[_numTraceSamples];
                int actCount = 0;
                double[] voltSingle = new double[_numTraceSamples];
                double[] currSingle = new double[_numTraceSamples];

                error = SmUsession.FetchMultiple(ChanNumber, 1, _numTraceSamples, voltSingle, currSingle, measComp, out actCount);  // "Count" doesn't wait to re-trigger, just measures all immediately after 1st trigger

                return currSingle;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "MeasureCurrentTrace4154");
                return new double[4];
            }
        }

        public void SetupVoltageMeasure()
        {
            int error = -1;

            try
            {
                SmUsession.Abort();

                double measureTimeLength = 0.001;

                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnDemand);
                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, 1);

                double dcSampleRate = 200e3;   // this is fixed for NI hardware
                int samplesToAvg = (int)(dcSampleRate * measureTimeLength);

                SmUsession.SetInt32(nidcpowerProperties.SamplesToAverage, samplesToAvg);

                SmUsession.Initiate();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupVoltageMeasure4154");

            }
        }
        public void MeasureVoltage(int numAverages, ref double result)
        {
            int error = -1;

            try
            {
                double[] volts = new double[numAverages];

                for (int avg = 0; avg < numAverages; avg++)
                {
                    error = SmUsession.Measure(ChanNumber, nidcpowerConstants.MeasureVoltage, out volts[avg]);
                }

                result = volts.Average();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "MeasureVoltage4154");

            }
        }

        public void SetupContinuity(double currentForce)
        {
            throw new NotImplementedException();
        }

        public void MeasureContinuity(int avgs, ref double result)
        {
            throw new NotImplementedException();
        }

        public void OutputEnable(bool state)
        {
            SmUsession.ConfigureOutputEnabled(ChanNumber, state);
        }

    }

    public class Ni4143 : IISmu
    {
        public nidcpower SmUsession;
        public string VisaAlias { get; set; }
        public string ChanNumber { get; set; }
        public string PinName { get; set; }
        public double PriorVoltage { get; set; }
        public double PriorCurrentLim { get; set; }

        public Ni4143(string visaAlias, string chanNumber, string pinName, bool reset)
        {
            try
            {
                this.SmUsession = new NationalInstruments.ModularInstruments.Interop.nidcpower(visaAlias, chanNumber, reset, "");

                this.VisaAlias = visaAlias;
                this.ChanNumber = chanNumber;
                this.PinName = pinName;

                string model = SmUsession.GetString(NationalInstruments.ModularInstruments.Interop.nidcpowerProperties.InstrumentModel);

                SmUsession.SetDouble(nidcpowerProperties.SourceDelay, chanNumber, 0);
                SmUsession.SetDouble(nidcpowerProperties.PowerLineFrequency, chanNumber, nidcpowerConstants._60Hertz);
                SmUsession.ConfigureOutputEnabled(chanNumber, false);
                SmUsession.ConfigureOutputFunction(chanNumber, nidcpowerConstants.DcVoltage);
                SmUsession.ConfigureSense(chanNumber, nidcpowerConstants.Remote);
                SmUsession.SetInt32(nidcpowerProperties.CurrentLimitAutorange, chanNumber, nidcpowerConstants.On);
                SmUsession.SetInt32(nidcpowerProperties.VoltageLevelAutorange, chanNumber, nidcpowerConstants.On);
                SmUsession.ConfigureVoltageLevel(chanNumber, 0);
                SmUsession.ConfigureCurrentLimit(chanNumber, nidcpowerConstants.CurrentRegulate, 0.001);
                SmUsession.ConfigureOutputEnabled(chanNumber, true);
                SmUsession.SetDouble(nidcpowerProperties.VoltageCompensationFrequency, 500);
                SmUsession.SetDouble(nidcpowerProperties.VoltageGainBandwidth, 20000);
                SmUsession.SetDouble(nidcpowerProperties.VoltagePoleZeroRatio, 8);
                SmUsession.Initiate();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SMU Initialize");
            }
        }

        public void ForceVoltage(double voltsForce, double currentLimit)
        {
            int error = -1;
            int chanInt = Convert.ToInt16(ChanNumber);

            try
            {
                if (currentLimit != PriorCurrentLim)
                {
                    // ConfigureCurrentLimit appears to also set the range automatically, because auto-range is on
                    error = SmUsession.ConfigureCurrentLimit(ChanNumber, nidcpowerConstants.CurrentRegulate, currentLimit);
                    PriorCurrentLim = currentLimit;

                    //double readRange = SMUsession.GetDouble(nidcpowerProperties.CurrentLimit);
                }
                if (voltsForce != PriorVoltage)
                {
                    error = SmUsession.ConfigureVoltageLevel(ChanNumber, voltsForce);
                    PriorVoltage = voltsForce;
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "ConfigureCurrentLimit");
            }
        }

        public void SetupCurrentMeasure(bool useIQsettings, bool triggerFromSa)
        {
            int error = -1;

            try
            {
                SmUsession.Abort();

                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, 1);

                SmUsession.SetDouble(nidcpowerProperties.ApertureTime, ChanNumber, 0.0005);
                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnDemand);

                SmUsession.Initiate();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupCurrentMeasure4143");

            }
        }
        public void MeasureCurrent(int numAverages, bool triggered, ref double result)
        {
            int error = -1;

            try
            {
                double[] volt = new double[numAverages];
                double[] curr = new double[numAverages];
                ushort[] measComp = new ushort[numAverages];
                int actCount = 0;
                double[] voltSingle = new double[1];
                double[] currSingle = new double[1];

                for (int avg = 0; avg < numAverages; avg++)
                {
                    if (triggered)
                    {
                        error = SmUsession.FetchMultiple(ChanNumber, 1, 1, voltSingle, currSingle, measComp, out actCount);  // "Count" doesn't wait to re-trigger, just measures all immediately after 1st trigger

                        volt[avg] = voltSingle[0];
                        curr[avg] = currSingle[0];
                    }
                    else
                    {
                        error = SmUsession.Measure(ChanNumber, nidcpowerConstants.MeasureCurrent, out curr[avg]);
                    }
                }

                result = curr.Average();


            }
            catch (Exception e)
            {
                MessageBox.Show("PinName: " + PinName + "\n\nVisaAlias: " + VisaAlias + "\n\nChannel: " + ChanNumber + "\n\n" + e.ToString(), "MeasureCurrent");
            }
        }

        int _numTraceSamples;
        public void SetupCurrentTraceMeasurement(double measureTimeLength, double aperture, bool triggered, bool triggerFromSa = false)
        {
            try
            {
                SmUsession.Abort();

                SmUsession.SetDouble(nidcpowerProperties.ApertureTime, ChanNumber, aperture);
                aperture = SmUsession.GetDouble(nidcpowerProperties.ApertureTime);

                _numTraceSamples = (int)(measureTimeLength / aperture);

                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnMeasureTrigger);

                if (triggered)
                {
                    SmUsession.SetInt32(nidcpowerProperties.MeasureTriggerType, nidcpowerConstants.DigitalEdge);
                    if (triggerFromSa) SmUsession.SetString(nidcpowerProperties.DigitalEdgeMeasureTriggerInputTerminal, "PXI_Trig1");
                    else SmUsession.SetString(nidcpowerProperties.DigitalEdgeMeasureTriggerInputTerminal, "PXI_Trig0");
                }
                else
                {
                    SmUsession.SetInt32(nidcpowerProperties.MeasureTriggerType, nidcpowerConstants.SoftwareEdge);
                }

                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, _numTraceSamples);
                SmUsession.SetInt32(nidcpowerProperties.SamplesToAverage, 1);

                SmUsession.Initiate();

                if (!triggered) SmUsession.SendSoftwareEdgeTrigger(nidcpowerConstants.MeasureTrigger);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupCurrentTraceMeasurement4143");

            }
        }
        public double[] MeasureCurrentTrace()
        {
            int error = -1;

            try
            {
                ushort[] measComp = new ushort[_numTraceSamples];
                int actCount = 0;
                double[] voltSingle = new double[_numTraceSamples];
                double[] currSingle = new double[_numTraceSamples];

                error = SmUsession.FetchMultiple(ChanNumber, 1, _numTraceSamples, voltSingle, currSingle, measComp, out actCount);  // "Count" doesn't wait to re-trigger, just measures all immediately after 1st trigger

                return currSingle;
            }
            catch (Exception e)
            {
                MessageBox.Show("PinName: " + PinName + "\n\nVisaAlias: " + VisaAlias + "\n\nChannel: " + ChanNumber + "\n\n" + e.ToString(), "MeasureCurrentTrace");
                return new double[4];
            }
        }

        public void SetupVoltageMeasure()
        {
            int error = -1;

            try
            {
                SmUsession.Abort();

                double measureTimeLength = 0.001;

                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnDemand);
                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, 1);

                double dcSampleRate = 200e3;   // this is fixed for NI hardware
                int samplesToAvg = (int)(dcSampleRate * measureTimeLength);

                SmUsession.SetInt32(nidcpowerProperties.SamplesToAverage, samplesToAvg);

                SmUsession.Initiate();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupVoltageMeasure4154");

            }
        }
        public void MeasureVoltage(int numAverages, ref double result)
        {
            int error = -1;

            try
            {
                double[] volts = new double[numAverages];

                for (int avg = 0; avg < numAverages; avg++)
                {
                    error = SmUsession.Measure(ChanNumber, nidcpowerConstants.MeasureVoltage, out volts[avg]);
                }

                result = volts.Average();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "MeasureVoltage");

            }
        }

        public void SetupContinuity(double currentForce)
        {
            SmUsession.Abort();
            SmUsession.ConfigureOutputFunction(ChanNumber, nidcpowerConstants.DcCurrent);
            SmUsession.ConfigureCurrentLevel(ChanNumber, currentForce);
            SmUsession.ConfigureCurrentLevelRange(ChanNumber, Math.Abs(currentForce));

            SetupCurrentMeasure(false, false);
        }

        public void MeasureContinuity(int avgs, ref double result)
        {
            MeasureVoltage(avgs, ref result);

            SmUsession.Abort();
            SmUsession.ConfigureOutputFunction(ChanNumber, nidcpowerConstants.DcVoltage);
        }

        public void OutputEnable(bool state)
        {
            SmUsession.ConfigureOutputEnabled(ChanNumber, state);
        }
    }

    public class Ni4139 : IISmu
    {
        public nidcpower SmUsession;
        public string VisaAlias { get; set; }
        public string ChanNumber { get; set; }
        public string PinName { get; set; }
        public double PriorVoltage { get; set; }
        public double PriorCurrentLim { get; set; }

        public Ni4139(string visaAlias, string chanNumber, string pinName, bool reset)
        {
            try
            {
                this.SmUsession = new NationalInstruments.ModularInstruments.Interop.nidcpower(visaAlias, chanNumber, reset, "");

                this.VisaAlias = visaAlias;
                this.ChanNumber = chanNumber;
                this.PinName = pinName;

                string model = SmUsession.GetString(NationalInstruments.ModularInstruments.Interop.nidcpowerProperties.InstrumentModel);

                SmUsession.SetDouble(nidcpowerProperties.SourceDelay, chanNumber, 0);
                SmUsession.SetDouble(nidcpowerProperties.PowerLineFrequency, chanNumber, nidcpowerConstants._60Hertz);
                SmUsession.ConfigureOutputEnabled(chanNumber, false);
                SmUsession.ConfigureOutputFunction(chanNumber, nidcpowerConstants.DcVoltage);
                SmUsession.ConfigureSense(chanNumber, nidcpowerConstants.Remote);
                SmUsession.SetInt32(nidcpowerProperties.CurrentLimitAutorange, chanNumber, nidcpowerConstants.On);
                SmUsession.SetInt32(nidcpowerProperties.VoltageLevelAutorange, chanNumber, nidcpowerConstants.On);
                SmUsession.ConfigureVoltageLevel(chanNumber, 0);
                SmUsession.ConfigureCurrentLimit(chanNumber, nidcpowerConstants.CurrentRegulate, 0.001);
                SmUsession.ConfigureOutputEnabled(chanNumber, true);
                SmUsession.SetDouble(nidcpowerProperties.VoltageCompensationFrequency, 500);
                SmUsession.SetDouble(nidcpowerProperties.VoltageGainBandwidth, 20000);
                SmUsession.SetDouble(nidcpowerProperties.VoltagePoleZeroRatio, 8);
                SmUsession.Initiate();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SMU Initialize");
            }
        }

        public void ForceVoltage(double voltsForce, double currentLimit)
        {
            int error = -1;
            int chanInt = Convert.ToInt16(ChanNumber);

            try
            {
                if (currentLimit != PriorCurrentLim)
                {
                    // ConfigureCurrentLimit appears to also set the range automatically, because auto-range is on
                    error = SmUsession.ConfigureCurrentLimit(ChanNumber, nidcpowerConstants.CurrentRegulate, currentLimit);
                    PriorCurrentLim = currentLimit;

                    //double readRange = SMUsession.GetDouble(nidcpowerProperties.CurrentLimit);
                }
                if (voltsForce != PriorVoltage)
                {
                    error = SmUsession.ConfigureVoltageLevel(ChanNumber, voltsForce);
                    PriorVoltage = voltsForce;
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "ConfigureCurrentLimit");
            }
        }

        public void SetupCurrentMeasure(bool useIQsettings, bool triggerFromSa)
        {
            int error = -1;

            try
            {
                SmUsession.Abort();

                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, 1);

                SmUsession.SetDouble(nidcpowerProperties.ApertureTime, ChanNumber, 0.0005);
                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnDemand);

                SmUsession.Initiate();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupCurrentMeasure4139");

            }
        }
        public void MeasureCurrent(int numAverages, bool triggered, ref double result)
        {
            int error = -1;

            try
            {
                double[] volt = new double[numAverages];
                double[] curr = new double[numAverages];
                ushort[] measComp = new ushort[numAverages];
                int actCount = 0;
                double[] voltSingle = new double[1];
                double[] currSingle = new double[1];

                for (int avg = 0; avg < numAverages; avg++)
                {
                    if (triggered)
                    {
                        error = SmUsession.FetchMultiple(ChanNumber, 1, 1, voltSingle, currSingle, measComp, out actCount);  // "Count" doesn't wait to re-trigger, just measures all immediately after 1st trigger

                        volt[avg] = voltSingle[0];
                        curr[avg] = currSingle[0];
                    }
                    else
                    {
                        error = SmUsession.Measure(ChanNumber, nidcpowerConstants.MeasureCurrent, out curr[avg]);
                    }
                }

                result = curr.Average();


            }
            catch (Exception e)
            {
                MessageBox.Show("PinName: " + PinName + "\n\nVisaAlias: " + VisaAlias + "\n\nChannel: " + ChanNumber + "\n\n" + e.ToString(), "MeasureCurrent");

            }
        }

        int _numTraceSamples;
        public void SetupCurrentTraceMeasurement(double measureTimeLength, double aperture, bool triggered, bool triggerFromSa = false)
        {
            try
            {
                SmUsession.Abort();

                SmUsession.SetDouble(nidcpowerProperties.ApertureTime, ChanNumber, aperture);
                aperture = SmUsession.GetDouble(nidcpowerProperties.ApertureTime);

                _numTraceSamples = (int)(measureTimeLength / aperture);

                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnMeasureTrigger);

                if (triggered)
                {
                    SmUsession.SetInt32(nidcpowerProperties.MeasureTriggerType, nidcpowerConstants.DigitalEdge);
                    if (triggerFromSa) SmUsession.SetString(nidcpowerProperties.DigitalEdgeMeasureTriggerInputTerminal, "PXI_Trig1");
                    else SmUsession.SetString(nidcpowerProperties.DigitalEdgeMeasureTriggerInputTerminal, "PXI_Trig0");
                }
                else
                {
                    SmUsession.SetInt32(nidcpowerProperties.MeasureTriggerType, nidcpowerConstants.SoftwareEdge);
                }

                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, _numTraceSamples);
                SmUsession.SetInt32(nidcpowerProperties.SamplesToAverage, 1);

                SmUsession.Initiate();

                if (!triggered) SmUsession.SendSoftwareEdgeTrigger(nidcpowerConstants.MeasureTrigger);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupCurrentTraceMeasurement4139");

            }
        }
        public double[] MeasureCurrentTrace()
        {
            int error = -1;

            try
            {
                ushort[] measComp = new ushort[_numTraceSamples];
                int actCount = 0;
                double[] voltSingle = new double[_numTraceSamples];
                double[] currSingle = new double[_numTraceSamples];

                error = SmUsession.FetchMultiple(ChanNumber, 1, _numTraceSamples, voltSingle, currSingle, measComp, out actCount);  // "Count" doesn't wait to re-trigger, just measures all immediately after 1st trigger

                return currSingle;
            }
            catch (Exception e)
            {
                MessageBox.Show("PinName: " + PinName + "\n\nVisaAlias: " + VisaAlias + "\n\nChannel: " + ChanNumber + "\n\n" + e.ToString(), "MeasureCurrentTrace");
                return new double[4];
            }
        }

        public void SetupVoltageMeasure()
        {
            int error = -1;

            try
            {
                SmUsession.Abort();

                double measureTimeLength = 0.001;

                SmUsession.SetInt32(nidcpowerProperties.MeasureWhen, nidcpowerConstants.OnDemand);
                SmUsession.SetInt32(nidcpowerProperties.MeasureRecordLength, 1);

                double dcSampleRate = 200e3;   // this is fixed for NI hardware
                int samplesToAvg = (int)(dcSampleRate * measureTimeLength);

                SmUsession.SetInt32(nidcpowerProperties.SamplesToAverage, samplesToAvg);

                SmUsession.Initiate();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SetupVoltageMeasure4154");

            }
        }
        public void MeasureVoltage(int numAverages, ref double result)
        {
            int error = -1;

            try
            {
                double[] volts = new double[numAverages];

                for (int avg = 0; avg < numAverages; avg++)
                {
                    error = SmUsession.Measure(ChanNumber, nidcpowerConstants.MeasureVoltage, out volts[avg]);
                }

                result = volts.Average();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "MeasureVoltage4154");

            }
        }

        public void SetupContinuity(double currentForce)
        {
            SmUsession.Abort();
            SmUsession.ConfigureOutputFunction(ChanNumber, nidcpowerConstants.DcCurrent);
            SmUsession.ConfigureCurrentLevel(ChanNumber, currentForce);
            SmUsession.ConfigureCurrentLevelRange(ChanNumber, Math.Abs(currentForce));

            SetupCurrentMeasure(false, false);
        }

        public void MeasureContinuity(int avgs, ref double result)
        {
            MeasureVoltage(avgs, ref result);

            SmUsession.Abort();
            SmUsession.ConfigureOutputFunction(ChanNumber, nidcpowerConstants.DcVoltage);
        }

        public void OutputEnable(bool state)
        {
            SmUsession.ConfigureOutputEnabled(ChanNumber, state);
        }
    }
    #endregion

    public interface IISmu
    {
        string VisaAlias { get; set; }
        string ChanNumber { get; set; }
        string PinName { get; set; }
        double PriorVoltage { get; set; }
        double PriorCurrentLim { get; set; }

        void ForceVoltage(double voltsForce, double currentLimit);
        void SetupCurrentMeasure(bool useIQsettings, bool triggerFromSa);
        void MeasureCurrent(int numAverages, bool triggered, ref double result);
        void SetupCurrentTraceMeasurement(double measureTimeLength, double aperture, bool triggered, bool triggerFromSa = false);
        double[] MeasureCurrentTrace();
        void SetupVoltageMeasure();
        void MeasureVoltage(int numAverages, ref double result);
        void SetupContinuity(double currentForce);
        void MeasureContinuity(int avgs, ref double result);
        void OutputEnable(bool state);
    }
}
