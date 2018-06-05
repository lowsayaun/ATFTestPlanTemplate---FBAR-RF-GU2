using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using Aemulus.Hardware.SMU;

namespace LibEqmtDriver.SMU
{
    public class AePxismu : IIPowerSupply
    {
        public AePxismu(string[] val)
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
        ~AePxismu() { }

        
        public static Dictionary<string, IIAeSmu> SmuResources = new Dictionary<string, IIAeSmu>();

        public static IIAeSmu GetSmu(string visaAlias, string chanNumber, string pinName, bool reset)
        {
            IIAeSmu smu;

            if (visaAlias.Contains("430"))
            {
                smu = new Am430E(visaAlias, chanNumber, pinName, reset);
            }
            else if (visaAlias.Contains("471"))
            {
                smu = new Am471E(visaAlias, chanNumber, pinName, reset);
            }
            else
            {
                throw new Exception("Visa Alias \"" + visaAlias + "\" is not in a recognized format.\nValid SMU Visa Aliases must include one of the following:\n"
                    + "\n\"430\""
                    + "\n\"471\""
                    + "\n\nFor example, Visa Alias \"SMU_AM471E_P1\" will be recognized as an AEMULUS AM471E module.");
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
            SmuResources[strSelection].OutputEnable(true, channel);
        }

        void IIPowerSupply.DcOff(string strSelection, EPSupplyChannel channel)
        {
            //SmuResources[strSelection].ForceVoltage(0.0, 1e-6);      //force voltage to 0V and very small current (cannot be zero)
            SmuResources[strSelection].OutputEnable(false, channel);
        }

        void IIPowerSupply.SetNplc(string strSelection, EPSupplyChannel channel, float val)
        {
            SmuResources[strSelection].SetNplc(channel, val);
        }

        void IIPowerSupply.SetVolt(string strSelection, EPSupplyChannel channel, double volt, double iLimit, EPSupplyVRange vRange)
        {
            SmuResources[strSelection].SetVoltage(channel, volt, iLimit, vRange);
        }

        float IIPowerSupply.MeasI(string strSelection, EPSupplyChannel channel, EPSupplyIRange range)
        {
            float imeas = -999;
            imeas = SmuResources[strSelection].MeasureCurrent(channel, range);
            return imeas;
        }

        float IIPowerSupply.MeasV(string strSelection, EPSupplyChannel channel, EPSupplyVRange vRange)
        {
            float vmeas = -999;
            vmeas = SmuResources[strSelection].MeasureVoltage(channel, vRange);
            return vmeas;
        }

        #endregion
    }

    #region iAeSMU class

    public class Am430E : IIAeSmu
    {
        public PxiSmu Smu;
        public string VisaAlias { get; set; }
        public string ChanNumber { get; set; }
        public string PinName { get; set; }

        public Am430E(string visaAlias, string chanNumber, string pinName, bool reset)
        {
            try
            {
                this.VisaAlias = visaAlias;
                this.ChanNumber = chanNumber;
                this.PinName = pinName;

                Smu = new PxiSmu(visaAlias, "0-3", 0xf, "Simulate=0, DriverSetup=Model:AM430e");

                int ret = 0;
                ret += Smu.Reset();
                ret += Smu.ConfigurePowerLineFrequency(50);
                ret += Smu.ConfigureOutputTransient("0-3", 1);
                ret += Smu.ConfigureSamplingTime("0-3", 0.1, 1);
                ret += Smu.ConfigureSense("0-3", PxiSmuConstants.SenseRemote);
                ret += Smu.ConfigureOutputFunction("0-3", PxiSmuConstants.DVCI);
                ret += Smu.ConfigureCurrentLimit("0-3".ToString(), 0, 100e-3);
                ret += Smu.ConfigureVoltageLevelAndRange("0-3", 0, 2);
                //ret += smu.ConfigureVoltageLevel("0-3", 0);
                ret += Smu.ConfigureOutputSwitch("0-3", 1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SMU Initialize");
            }
        }

        public void OutputEnable(bool state, EPSupplyChannel channel)
        {
            //int ret = 0;
            //if (state)
            //    ret += smu.ConfigureOutputSwitch(((int)Channel).ToString(), 1);
            //else
            //    smu.ConfigureOutputSwitch(((int)Channel).ToString(), 0);    
        }
        public void SetNplc(EPSupplyChannel channel, float val)
        {
            Smu.ConfigureSamplingTime(((int)channel).ToString(), val, 1);
        }
        public void SetVoltage(EPSupplyChannel channel, double volt, double iLimit, EPSupplyVRange EvRange)
        {
            double vRange = 0;
            switch (EvRange)
            {
                case EPSupplyVRange._1V:
                    vRange = 1; break;
                case EPSupplyVRange._10V:
                    vRange = 10; break;
                case EPSupplyVRange.Auto:
                    vRange = 10; break;
            }
            Smu.ConfigureOutputFunction(((int)channel).ToString(), PxiSmuConstants.DVCI);
            Smu.ConfigureCurrentLimit(((int)channel).ToString(), 0, iLimit);
            Smu.ConfigureVoltageLevelAndRange(((int)channel).ToString(), volt, vRange);
        }
        public float MeasureCurrent(EPSupplyChannel channel, EPSupplyIRange range)
        {
            //we don't need to set range for measure
            double[] current = new double[4];
            int ret = 0;
            ret += Smu.Measure(((int)channel).ToString(), PxiSmuConstants.MeasureCurrent, current);
            return (float)current[0];
        }
        public float MeasureVoltage(EPSupplyChannel channel, EPSupplyVRange vRange)
        {
            double[] volt = new double[4];
            int ret = 0;
            ret += Smu.Measure(((int)channel).ToString(), PxiSmuConstants.MeasureVoltage, volt);
            return (float)volt[0];
        }
    }

    public class Am471E : IIAeSmu
    {
        public PxiSmu Smu;
        public string VisaAlias { get; set; }
        public string ChanNumber { get; set; }
        public string PinName { get; set; }

        public Am471E(string visaAlias, string chanNumber, string pinName, bool reset)
        {
            try
            {
                this.VisaAlias = visaAlias;
                this.ChanNumber = chanNumber;
                this.PinName = pinName;

                Smu = new PxiSmu(visaAlias, "0", 0xf, "Simulate=0, DriverSetup=Model:AM471e");

                int ret = 0;
                ret += Smu.Reset();
                ret += Smu.ConfigurePowerLineFrequency(50);
                ret += Smu.ConfigureOutputTransient("0", 1);
                ret += Smu.ConfigureSamplingTime("0", 0.1, 1);
                ret += Smu.ConfigureSense("0", PxiSmuConstants.SenseRemote);
                ret += Smu.ConfigureOutputFunction("0", PxiSmuConstants.DVCI);
                ret += Smu.ConfigureCurrentLimit("0".ToString(), 0, 100e-3);
                ret += Smu.ConfigureVoltageLevelAndRange("0", 0, 1);
                //ret += smu.ConfigureVoltageLevel("0", 0);
                ret += Smu.ConfigureOutputSwitch("0", 1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "SMU Initialize");
            }
        }

        public void OutputEnable(bool state, EPSupplyChannel channel)
        {
            //if (state)
            //    smu.ConfigureOutputSwitch(((int)Channel).ToString(), 1);
            //else
            //    smu.ConfigureOutputSwitch(((int)Channel).ToString(), 0);      
        }
        public void SetNplc(EPSupplyChannel channel, float val)
        {
            Smu.ConfigureSamplingTime(((int)channel).ToString(), val, 1);
        }
        public void SetVoltage(EPSupplyChannel channel, double volt, double iLimit, EPSupplyVRange VRange)
        {
            double vRange = 0;
            switch (VRange)
            {
                case EPSupplyVRange._1V:
                    vRange = 1; break;
                case EPSupplyVRange._10V:
                    vRange = 10; break;
                case EPSupplyVRange.Auto:
                    vRange = 10; break;
            }
            Smu.ConfigureOutputFunction(((int)channel).ToString(), PxiSmuConstants.DVCI);
            Smu.ConfigureCurrentLimit(((int)channel).ToString(), 0, iLimit);
            Smu.ConfigureVoltageLevelAndRange(((int)channel).ToString(), volt, vRange);
            //smu.ConfigureVoltageLevel(((int)Channel).ToString(), Volt);
        }
        public float MeasureCurrent(EPSupplyChannel channel, EPSupplyIRange range)
        {
            //we don't need to set range for measure
            double[] current = new double[4];
            int ret = 0;
            ret += Smu.Measure(((int)channel).ToString(), PxiSmuConstants.MeasureCurrent, current);
            return (float)current[0];
        }
        public float MeasureVoltage(EPSupplyChannel channel, EPSupplyVRange vRange)
        {
            double[] volt = new double[4];
            int ret = 0;
            ret += Smu.Measure(((int)channel).ToString(), PxiSmuConstants.MeasureVoltage, volt);
            return (float)volt[0];
        }
    }

    #endregion

    public interface IIAeSmu
    {
        string VisaAlias { get; set; }
        string ChanNumber { get; set; }
        string PinName { get; set; }

        void OutputEnable(bool state, EPSupplyChannel channel);
        void SetNplc(EPSupplyChannel channel, float val);
        void SetVoltage(EPSupplyChannel channel, double volt, double iLimit, EPSupplyVRange vRange);
        float MeasureCurrent(EPSupplyChannel channel, EPSupplyIRange range);
        float MeasureVoltage(EPSupplyChannel channel, EPSupplyVRange vRange);
    }
}
 

