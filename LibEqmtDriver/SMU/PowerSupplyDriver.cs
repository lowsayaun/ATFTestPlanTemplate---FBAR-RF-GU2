using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibEqmtDriver.SMU
{
    
    public interface IIPowerSupply
    {
        void Init();
        void DcOn(string strSelection, EPSupplyChannel channel);
        void DcOff(string strSelection, EPSupplyChannel channel);
        void SetNplc(string strSelection, EPSupplyChannel channel, float val);
        void SetVolt(string strSelection, EPSupplyChannel channel, double volt, double iLimit, EPSupplyVRange vRange);
        float MeasI(string strSelection, EPSupplyChannel channel, EPSupplyIRange range);
        float MeasV(string strSelection, EPSupplyChannel channel, EPSupplyVRange vRange);

    }

    public class PowerSupplyDriver
    {
        private EPSupplyModel _eModel;
        private EPSupplyChannel _eChannel;
        private IIPowerSupply _objPs;

        public void Initialize(IIPowerSupply[] eqPSupply)
        {
            int i = 0;

            for (i = 0; i < eqPSupply.Length; i++)
            {
                eqPSupply[i].Init();
            }
        }
        public void DcOn(string[] arrSelection, IIPowerSupply[] eqPSupply)
        {
            int i = 0;

            for (i = 0; i < arrSelection.Length; i++)
            {
                PSupply_Selection(arrSelection[i], eqPSupply, out _objPs, out _eChannel, out _eModel);
                _objPs.DcOn(arrSelection[i], _eChannel);
            }
        }
        public void DcOff(string[] arrSelection, IIPowerSupply[] eqPSupply)
        {
            int i = 0;

            for (i = 0; i < arrSelection.Length; i++)
            {
                PSupply_Selection(arrSelection[i], eqPSupply, out _objPs, out _eChannel, out _eModel);
                _objPs.DcOff(arrSelection[i], _eChannel);
            }
        }
        public void SetVolt(string strSelection, IIPowerSupply[] eqPSupply, float volt, float iLimit)
        {
            PSupply_Selection(strSelection, eqPSupply, out _objPs, out _eChannel, out _eModel);
            _objPs.SetVolt(strSelection, _eChannel, volt, iLimit, EPSupplyVRange.Auto);
            
        }
        public float MeasI(string strSelection, IIPowerSupply[] eqPSupply, float nplc, EPSupplyIRange range)
        {
            float measVal = -999;
            PSupply_Selection(strSelection, eqPSupply, out _objPs, out _eChannel, out _eModel);
            SetNplc(strSelection, eqPSupply, nplc);
            measVal = _objPs.MeasI(strSelection, _eChannel, range);

            return measVal;
        }
        public float MeasV(string strSelection, IIPowerSupply[] eqPSupply, float nplc, EPSupplyVRange vRange)
        {
            float measVal = -999;
            PSupply_Selection(strSelection, eqPSupply, out _objPs, out _eChannel, out _eModel);
            SetNplc(strSelection, eqPSupply, nplc);
            measVal = _objPs.MeasV(strSelection, _eChannel, vRange);

            return measVal;
        }
        private void SetNplc(string strSelection, IIPowerSupply[] eqPSupply, float valNplc)
        {
            PSupply_Selection(strSelection, eqPSupply, out _objPs, out _eChannel, out _eModel);
            _objPs.SetNplc(strSelection, _eChannel, valNplc);
        }
        private void PSupply_Selection(string val, IIPowerSupply[] psAvailable, out IIPowerSupply objPs, out EPSupplyChannel eChannel, out EPSupplyModel eModel)
        {
            string[] arSelected = new string[4];
            arSelected = val.Split('_');

            const string
                preFixP1 = "P1",
                preFixP2 = "P2",
                preFixP3 = "P3",
                preFixP4 = "P4",
                preFixP5 = "P5",
                preFixP6 = "P6",
                preFixP7 = "P7",
                preFixCh0 = "CH0",
                preFixCh1 = "CH1",
                preFixCh2 = "CH2",
                preFixCh3 = "CH3",
                preFixCh4 = "CH4",
                preFixCh5 = "CH5",
                preFixCh6 = "CH6",
                preFixCh7 = "CH7",
                preFixCh8 = "CH8",
                preFixChA = "CHA",
                preFixChB = "CHB",
                preFixAe1340 = "AE1340",
                preFixAePxi = "AEPXI",
                preFixNiPxi = "NIPXI",
                preFixKeith = "KEITH",
                preFixAgilentPxi = "AGPXI",
                preFixAm471E = "AM471E",
                preFixAm430E = "AM430E",
                preFixNi4143 = "NI4143",
                preFixNi4139 = "NI4139",
                preFixNi4154 = "NI4154",
                preFixAgilent = "AG";

            switch (arSelected[0].ToUpper())
            {
                case preFixP1:
                    objPs = psAvailable[0];
                    break;
                case preFixP2:
                    objPs = psAvailable[1];
                    break;
                case preFixP3:
                    objPs = psAvailable[2];
                    break;
                case preFixP4:
                    objPs = psAvailable[3];
                    break;
                case preFixP5:
                    objPs = psAvailable[4];
                    break;
                case preFixP6:
                    objPs = psAvailable[5];
                    break;
                case preFixP7:
                    objPs = psAvailable[6];
                    break;
                default:
                    objPs = null;
                    MessageBox.Show("Power Supply in Local Setting file PowerSupply portion have invalid setting, P1-7 only");
                    break;
            }

            switch (arSelected[1].ToUpper())
            {
                case preFixCh0:
                    eChannel = EPSupplyChannel.Ch0;
                    break;
                case preFixCh1:
                    eChannel = EPSupplyChannel.Ch1;
                    break;
                case preFixCh2:
                    eChannel = EPSupplyChannel.Ch2;
                    break;
                case preFixCh3:
                    eChannel = EPSupplyChannel.Ch3;
                    break;
                case preFixCh4:
                    eChannel = EPSupplyChannel.Ch4;
                    break;
                case preFixCh5:
                    eChannel = EPSupplyChannel.Ch5;
                    break;
                case preFixCh6:
                    eChannel = EPSupplyChannel.Ch6;
                    break;
                case preFixCh7:
                    eChannel = EPSupplyChannel.Ch7;
                    break;
                case preFixCh8:
                    eChannel = EPSupplyChannel.Ch8;
                    break;
                case preFixChA:
                    eChannel = EPSupplyChannel.A;
                    break;
                case preFixChB:
                    eChannel = EPSupplyChannel.B;
                    break;
                default:
                    eChannel = new EPSupplyChannel();
                    MessageBox.Show("Power Supply channel in Local Setting file PowerSupply portion have invalid setting.");
                    break;
            }

            switch (arSelected[2].ToUpper())
            {
                case preFixKeith:
                    eModel = EPSupplyModel.Keithley;
                    break;
                case preFixNiPxi:
                    eModel = EPSupplyModel.NiPxi;
                    break;
                case preFixAgilentPxi:
                    eModel = EPSupplyModel.AgilentPxi;
                    break;
                case preFixAgilent:
                    eModel = EPSupplyModel.Agilent;
                    break;
                case preFixAePxi:
                    eModel = EPSupplyModel.AemulusPxi;
                    break;
                case preFixAe1340:
                    eModel = EPSupplyModel.Aemulus1340;
                    break;
                case preFixAm471E:
                    eModel = EPSupplyModel.Am471E;
                    break;
                case preFixAm430E:
                    eModel = EPSupplyModel.Am430E;
                    break;
                case preFixNi4143:
                    eModel = EPSupplyModel.Ni4143;
                    break;
                case preFixNi4139:
                    eModel = EPSupplyModel.Ni4139;
                    break;
                case preFixNi4154:
                    eModel = EPSupplyModel.Ni4154;
                    break;
                default:
                    eModel = new EPSupplyModel();
                    MessageBox.Show("Power Supply model in Local Setting file PowerSupply portion have invalid setting.");
                    break;
            }
        }
    }

    public enum EPSupplyVRange
    {
        //260xB range
        Auto,
        Keith260X100MV,
        Keith260X_1V,
        Keith260X_6V,
        Keith260X40V,
        //261xB range
        Keith261X200MV,
        Keith261X_2V,
        Keith261X20V,
        Keith261X200V,
        //Aemulus 
        _1V,
        _2V,
        _5V,
        _10V,
    }
    public enum EPSupplyIRange
    {
        //all model range
        Auto,
        All100NA,
        All1UA,
        All10UA,
        All100UA,
        All1MA,
        All10MA,
        All100MA,
        All_1A,

        //260x range
        _260x_3A,

        //261x range
        _261x_1_5A,
        _261x_10A,

    }
    public enum EPSupplyChannel
    {
        Ch0 = 0,
        Ch1 = 1,
        Ch2 = 2,
        Ch3 = 3,
        Ch4 = 4,
        Ch5 = 5,
        Ch6 = 6,
        Ch7 = 7,
        Ch8 = 8,
        A,
        B
    }
    public enum EPSupplyModel
    {
        Agilent,
        AgilentPxi,
        Aemulus1340,
        AemulusPxi,
        Keithley,
        Am471E,
        Am430E,
        Ni4143,
        Ni4139,
        Ni4154,
        NiPxi
    }
}
