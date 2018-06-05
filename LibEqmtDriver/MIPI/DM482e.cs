using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace LibEqmtDriver.MIPI
{
    public class AemulusDm482E : IIMiPiCtrl
    {
        readonly int _slvaddr = LibVar.ºSlaveAddress;
        readonly int _pair = 0;
        readonly int _pair2 = 1;

        readonly Aemulus.Hardware.DM _myDm;

        int _ret = 0;
        bool[] _dataArrayBool;
        //public static OTP _OTP { get; set; }
        Stopwatch _speedo = new Stopwatch();
        LibEqmtDriver.Utility.HiPerfTimer _hiTimer = new LibEqmtDriver.Utility.HiPerfTimer();

        readonly string _moduleAlias = LibVar.ºmyDm482Address;

        private const string Sclk0 = "P0"; //chnl 0
        private const string Sdata0 = "P1"; //chnl 1
        private const string Svio0 = "P2"; //chnl 2

        private const string Sclk1 = "P4"; //chnl 4
        private const string Sdata1 = "P5"; //chnl 5
        private const string Svio1 = "P3"; //chnl 3

        private const string Sclk2 = "P6"; //chnl 6
        private const string Sdata2 = "P7"; //chnl 7
        private const string Svio2 = "P8"; //chnl 8

        private const string Sclk3 = "P10"; //chnl 10
        private const string Sdata3 = "P11"; //chnl 11
        private const string Svio3 = "P9"; //chnl 9

        private double _vih, _vil, _voh, _vol, _ioh, _iol, _vch, _vcl, _vt;

        public AemulusDm482E()
        {
            _myDm = new Aemulus.Hardware.DM(LibVar.ºHwProfile, 3, 0, 0, false, 0x0f); //3
        }

        public void Initialization()
        {
            //Actual DUT MIPI
            InitMipi(_pair, 26000000, 1.8);
            SetMipiInputDelay(_pair, 2); //depend on cable length
            OnOff_VIO(true, _pair);
            OnOff_CLKDATA(true, _pair);

            //On board DUT - Ref DUT for temperature control , ID etc mounted on test board
            InitMipi(_pair2, 26000000, 1.8); 
            SetMipiInputDelay(_pair2, 2); //depend on cable length
            OnOff_VIO(true, _pair2);
            OnOff_CLKDATA(true, _pair2);
        }

        public int InitMipi(int mipiPair, int freqHz, double mipiVoltage)
        {
            double threhold = 1.2;
            _ret += _myDm.MIPI_ConfigureClock(_moduleAlias, mipiPair, freqHz);

            _vih = mipiVoltage;
            _vil = 0;
            _voh = threhold; //(vih - vil) / 3;
            _vol = threhold; // (vih - vil) / 3;
            _ioh = 0.01;
            _iol = 0.01;
            _vch = mipiVoltage;
            _vcl = 0;
            _vt = 1.6;
            //DM482e spec
            //=====================================================
            //Driver (VIH, VIL)   -1.4V to 6V 
            //Comparator (VOH, VOL)      -2.0V to 7V
            //Current Load (IOH, IOL)    -12mA to 12mA
            //Clamp Voltage Range High Side (VCH)      -1.0V to 6.0V
            //Clamp Voltage Range Low Side (VCL)       -1.5V to 5.0V
            //Termination Voltage (VT)   -2.0V to 6.0V
            //=====================================================

            int state = 0; //Pin Electronics
            int statePmu = 1; //PMU
            int stateVio = 2; //DIO

            //DM482e_DPINForce state  
            //=====================================================
            //0 : DM482E_CONST_FORCE_STATE_VECTOR(Pin Electronics) 
            //1 : DM482E_CONST_FORCE_STATE_PMU (Pin Measurement Unit)
            //2 : DM482E_CONST_FORCE_STATE_DIO
            //5 : DM482E_CONST_FORCE_STATE_CLOCK
            //6 : DM482E_CONST_FORCE_STATE_INVERTED_CLOCK
            //=====================================================

            switch (mipiPair)
            {
                case 0:
                    //for Mipi
                    _ret += _myDm.Force(Svio0, stateVio);
                    _ret += _myDm.Force(Sclk0, state);
                    _ret += _myDm.Force(Sdata0, state);

                    _ret += _myDm.DPINLevel(Svio0, _vih, _vil, _voh, _vol, _ioh, _iol, _vch, _vcl, _vt);
                    _ret += _myDm.DPINLevel(Sclk0, _vih, _vil, _voh, _vol, _ioh, _iol, _vch, _vcl, _vt);
                    _ret += _myDm.DPINLevel(Sdata0, _vih, _vil, _voh, _vol, _ioh, _iol, _vch, _vcl, _vt);

                    _ret += _myDm.ConfigurePEAttribute(Svio0, false, false, false, false); //High Z
                    _ret += _myDm.ConfigurePEAttribute(Sclk0, false, false, false, false); //High Z
                    _ret += _myDm.ConfigurePEAttribute(Sdata0, true, false, false, false); //Termination Mode

                    _ret += _myDm.SetPinDirection(Svio0, 1);

                    //for PMU
                    _ret += _myDm.ConfigurePowerLineFrequency(_moduleAlias, 50);

                    _ret += _myDm.ConfigurePMUSense(Svio0, 0); //0 local, 1 remote 
                    _ret += _myDm.ConfigurePMUSense(Sclk0, 0); //0 local, 1 remote 
                    _ret += _myDm.ConfigurePMUSense(Sdata0, 0); //0 local, 1 remote 

                    _ret += _myDm.ConfigurePMUSamplingTime(Svio0, 0.0001, 0);
                    _ret += _myDm.ConfigurePMUSamplingTime(Sclk0, 0.0001, 0);
                    _ret += _myDm.ConfigurePMUSamplingTime(Sdata0, 0.0001, 0);
                    break;

                case 1:
                    //for Mipi
                    _ret += _myDm.Force(Svio1, stateVio);
                    _ret += _myDm.Force(Sclk1, state);
                    _ret += _myDm.Force(Sdata1, state);

                    _ret += _myDm.DPINLevel(Svio1, _vih, _vil, _voh, _vol, _ioh, _iol, _vch, _vcl, _vt);
                    _ret += _myDm.DPINLevel(Sclk1, _vih, _vil, _voh, _vol, _ioh, _iol, _vch, _vcl, _vt);
                    _ret += _myDm.DPINLevel(Sdata1, _vih, _vil, _voh, _vol, _ioh, _iol, _vch, _vcl, _vt);

                    _ret += _myDm.ConfigurePEAttribute(Svio1, false, false, false, false); //High Z
                    _ret += _myDm.ConfigurePEAttribute(Sclk1, false, false, false, false); //High Z
                    _ret += _myDm.ConfigurePEAttribute(Sdata1, true, false, false, false); //Termination Mode

                    _ret += _myDm.SetPinDirection(Svio1, 1);

                    //Drive voltage
                    _ret += _myDm.Force("P6", statePmu);
                    _ret += _myDm.ConfigurePMUOutputFunction("P6", 0); //0=>DVCI; 1=>DICV;
                    _ret += ClampCurrent("P6", 0.025);
                    _ret += DriveVoltage("P6", 0);
                    _ret += _myDm.DPINOn("P6");
                    break;
            }
            return _ret;
        }
        public int OnOff_VIO(bool isOn, int mipiPair)
        {
            if (isOn)
            {

                switch (mipiPair)
                {
                    case 0:
                        _ret += _myDm.DrivePin(Svio0, 1); //vio drive high
                        break;
                    case 1:
                        _ret += _myDm.DrivePin(Svio1, 1); //vio drive high
                        break;
                    case 2:
                        _ret += _myDm.DrivePin(Svio2, 1); //vio drive high
                        break;
                    case 3:
                        _ret += _myDm.DrivePin(Svio3, 1); //vio drive high
                        break;
                    default:
                        _ret += _myDm.DrivePin(Svio0, 1); //vio drive high
                        break;

                }
            }
            else
            {
                switch (mipiPair)
                {
                    case 0:
                        _ret += _myDm.DrivePin(Svio0, 0); //vio drive low
                        break;
                    case 1:
                        _ret += _myDm.DrivePin(Svio1, 0); //vio drive low
                        break;
                    case 2:
                        _ret += _myDm.DrivePin(Svio2, 0); //vio drive low
                        break;
                    case 3:
                        _ret += _myDm.DrivePin(Svio3, 0); //vio drive low
                        break;
                    default:
                        _ret += _myDm.DrivePin(Svio0, 0); //vio drive low
                        break;

                }
            }
            return _ret;
        }
        public int OnOff_CLKDATA(bool isOn, int mipiPair)
        {

            if (isOn)
            {
                //connect mipi
                //ret += myDM.MIPI_Connect(moduleAlias, mipi_pair, 1);
                _ret += _myDm.MIPI_Connect(_moduleAlias, mipiPair, 1);
                switch (mipiPair)
                {
                    case 0:
                        _ret += _myDm.DPINOn(Svio0);
                        _ret += _myDm.DPINOn(Sclk0);
                        _ret += _myDm.DPINOn(Sdata0);
                        break;
                    case 1:
                        _ret += _myDm.DPINOn(Svio1);
                        _ret += _myDm.DPINOn(Sclk1);
                        _ret += _myDm.DPINOn(Sdata1);
                        break;
                    case 2:
                        _ret += _myDm.DPINOn(Svio2);
                        _ret += _myDm.DPINOn(Sclk2);
                        _ret += _myDm.DPINOn(Sdata2);
                        break;
                    case 3:
                        _ret += _myDm.DPINOn(Svio3);
                        _ret += _myDm.DPINOn(Sclk3);
                        _ret += _myDm.DPINOn(Sdata3);
                        break;
                    default:
                        _ret += _myDm.DPINOn(Svio0);
                        _ret += _myDm.DPINOn(Sclk0);
                        _ret += _myDm.DPINOn(Sdata0);
                        break;

                }
            }
            else
            {
                //disconnect mipi
                _ret += _myDm.MIPI_Connect(_moduleAlias, mipiPair, 0);

                switch (mipiPair)
                {
                    case 0:
                        _ret += _myDm.DPINOff(Svio0);
                        _ret += _myDm.DPINOff(Sclk0);
                        _ret += _myDm.DPINOff(Sdata0);
                        break;
                    case 1:
                        _ret += _myDm.DPINOff(Svio1);
                        _ret += _myDm.DPINOff(Sclk1);
                        _ret += _myDm.DPINOff(Sdata1);
                        break;
                    case 2:
                        _ret += _myDm.DPINOff(Svio2);
                        _ret += _myDm.DPINOff(Sclk2);
                        _ret += _myDm.DPINOff(Sdata2);
                        break;
                    case 3:
                        _ret += _myDm.DPINOff(Svio3);
                        _ret += _myDm.DPINOff(Sclk3);
                        _ret += _myDm.DPINOff(Sdata3);
                        break;
                    default:
                        _ret += _myDm.DPINOff(Svio0);
                        _ret += _myDm.DPINOff(Sclk0);
                        _ret += _myDm.DPINOff(Sdata0);
                        break;

                }
            }

            return _ret;
        }
        public int SetMipiInputDelay(int mipiPair, int delay)
        {
            _ret += _myDm.MIPI_ConfigureDelay(_moduleAlias, mipiPair, delay);
            return _ret;
        }
        public int ClampCurrent(string pinAlias, double currentLevel)
        {
            // ret += myDM.ConfigurePMUOutputFunction(PinAlias, 0); //CIDV
            _ret += _myDm.ConfigurePMUCurrentLimitRange(pinAlias, currentLevel);
            return _ret;
        }
        public int DriveVoltage(string pinAlias, double voltageLevel)
        {
            _ret += _myDm.ConfigurePMUVoltageLevel(pinAlias, voltageLevel);
            return _ret;
        }
        public void Trig()
        {
            Mipi_Write(_pair, _slvaddr, 0x1c, 0x03);
        }
        public bool Register_Change(int mipiReg)
        {
            int limit = 0;
            int[] mipiArr = new int[mipiReg];
            bool readSuccessful = false;
            bool[] T_ReadSuccessful = new bool[mipiReg];
            string[] regXValue = new string[mipiReg];
            string[] mipiRegCond = new string[mipiReg];
            int i;
            int regCnt;
            int passRd, failRd;
            string result = "";

            //Initialize variable
            i = 0; regCnt = 0;
            for (regCnt = 0; regCnt < mipiReg; regCnt++)
            {

                switch (regCnt)
                {
                    case 0:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg0;
                        break;
                    case 1:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg1;
                        break;
                    case 2:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg2;
                        break;
                    case 3:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg3;
                        break;
                    case 4:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg4;
                        break;
                    case 5:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg5;
                        break;
                    case 6:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg6;
                        break;
                    case 7:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg7;
                        break;
                    case 8:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg8;
                        break;
                    case 9:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiReg9;
                        break;
                    case 10:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiRegA;
                        break;
                    case 11:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiRegB;
                        break;
                    case 12:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiRegC;
                        break;
                    case 13:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiRegD;
                        break;
                    case 14:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiRegE;
                        break;
                    case 15:
                        mipiRegCond[regCnt] = LibEqmtDriver.MIPI.LibVar.ºMipiRegF;
                        break;
                    default:
                        MessageBox.Show("Total Register Number : " + mipiReg + " not supported at this moment.", "MyDUT", MessageBoxButtons.OK);
                        break;
                }
            }

            while (true)
            {
                regCnt = 0; passRd = 0; failRd = 0; //reset read success counter

                for (regCnt = 0; regCnt < mipiReg; regCnt++)
                {
                    if (mipiRegCond[regCnt].ToUpper() != "X")
                        WRITE_Register_Address(mipiRegCond[regCnt], regCnt);
                }

                Trig();

                for (regCnt = 0; regCnt < mipiReg; regCnt++)
                {
                    T_ReadSuccessful[regCnt] = true;
                    regXValue[regCnt] = "";

                    if (mipiRegCond[regCnt].ToUpper() != "X")
                    {
                        Read_Register_Address(ref result, regCnt);
                        regXValue[regCnt] = result;
                    }
                    else
                    {
                        regXValue[regCnt] = mipiRegCond[regCnt];
                    }

                    if (mipiRegCond[regCnt] != regXValue[regCnt] && LibEqmtDriver.MIPI.LibVar.ºReadFunction == true)
                        T_ReadSuccessful[regCnt] = false;
                    else
                        T_ReadSuccessful[regCnt] = true;
                }

                for (regCnt = 0; regCnt < mipiReg; regCnt++)
                {
                    if (T_ReadSuccessful[regCnt] == true)
                        passRd++;
                    else
                        failRd++;
                }

                if (passRd == (mipiReg))
                {
                    readSuccessful = true;
                    break;
                }
                else
                    readSuccessful = false;

                limit = limit + 1;


                if (limit > 10) break;
            }
            return readSuccessful;
        }

        public void WRITE_Register_Address(string cmd, int regAddr)
        {
            int data = int.Parse(cmd, System.Globalization.NumberStyles.HexNumber);
            Mipi_Write(_pair, _slvaddr, regAddr, data);
        }
        public string Read_Register_Address(ref string x, int regAddr)
        {
            int bytecount = 1;
            int dum = 0x0;
            //read
            Mipi_Read(_pair, _slvaddr, regAddr, dum, true);

            //retrieve
            int count = 0;
            int[] dataarray = new int[bytecount + 1];
            int[] datahex = new int[bytecount + 1];
            int[] parityarray = new int[bytecount + 1];
            Mipi_Retrieve(_pair, out count, dataarray, out datahex, parityarray);

            //Mipi_Retrieve(1, out count, dataarray, out datahex, parityarray);

            string tempresult = "";
            // F -> 0F
            if (datahex[0] <= 15)
            {
                tempresult = "0" + datahex[0].ToString("X");
            }
            else
            {
                tempresult = datahex[0].ToString("X");
            }
            x = tempresult;
            return x;
        }

        /// <summary>
        /// Register Read
        /// </summary>
        /// <param name="mipiPair">pair 0 = pin 0,1,2 (CLK DATA VIO); pair 1 = pin 3,4,5 (CLK DATA VIO)...pair 3 max</param>
        /// <param name="slaveaddress">max 0x0f hex</param>
        /// <param name="address">max 0x1f hex [4:0]</param>
        /// <param name="data">max 0xff hex [7:0]</param>
        /// <param name="isfullSpeed">true = fullspeed(26MHz), false = halfspeed(13MHz)</param>
        /// <returns>0 = no error</returns>
        public int Mipi_Read(int mipiPair, int slaveaddress, int address, int data, bool isfullSpeed)
        {
            int speed = 0;
            //full speed of half speed read
            if (isfullSpeed)
                speed = 1;
            else
                speed = 0;

            //command frame
            int command = (slaveaddress << 8) + 0x60 + (address & 0x1f);

            //data frame
            int[] tempdata = new int[1];
            tempdata[0] = data;

            //reg read
            _ret += _myDm.MIPI_Read(_moduleAlias, mipiPair, speed, command, tempdata);

            return _ret;
        }
        /// <summary>
        /// Register Write
        /// </summary>
        /// <param name="mipiPair">pair 0 = pin 0,1,2 (CLK DATA VIO); pair 1 = pin 3,4,5 (CLK DATA VIO)...pair 3 max</param>
        /// <param name="slaveaddress">max 0x0f hex</param>
        /// <param name="address">max 0x1f hex [4:0]</param>
        /// <param name="data">max 0xff (1 byte data)[7:0]</param>
        /// <returns></returns>
        public int Mipi_Write(int mipiPair, int slaveaddress, int address, int data)
        {
            //command frame
            int command = ((slaveaddress & 0x1f) << 8) + 0x40 + (address & 0x1f);

            //data frame
            int[] tempdata = new int[1];
            tempdata[0] = data;

            //reg write
            _ret += _myDm.MIPI_Write(_moduleAlias, mipiPair, command, tempdata);//DM482.DM482e_MIPI_RFFE_WR(session, mipi_pair, command, tempdata);

            return _ret;
        }

        public int Mipi_Retrieve(int mipiPair, out int rdByteDataCount, int[] rdDataArray, out int[] rdDataArrayHex, int[] parityCheckArray)
        {
            rdByteDataCount = 0;
            _ret += _myDm.MIPI_Retrieve(_moduleAlias, mipiPair, ref rdByteDataCount, rdDataArray, parityCheckArray);

            //decode to hex value
            rdDataArrayHex = new int[rdDataArray.Length];
            for (int i = 0; i < rdDataArray.Length; i++)
            {
                rdDataArrayHex[i] = Decodetohexvalue(rdDataArray[i]);
            }
            return _ret;
        }
        private int Decodetohexvalue(int raw)
        {
            int result = 0;
            int[] tempdata = new int[2];
            tempdata[0] = (raw & 0xff00) >> 8;
            tempdata[1] = raw & 0xff;

            //raw to hex table 
            result = ((Rawtohex(tempdata[0])) << 4) | Rawtohex(tempdata[1]);

            return result;
        }
        private int Rawtohex(int rawbyte)
        {
            int result = 0;
            switch (rawbyte)
            {
                case 0x00:
                    result = 0x00;
                    break;
                case 0x01:
                    result = 0x01;
                    break;
                case 0x04:
                    result = 0x02;
                    break;
                case 0x05:
                    result = 0x03;
                    break;
                case 0x10:
                    result = 0x04;
                    break;
                case 0x11:
                    result = 0x05;
                    break;
                case 0x14:
                    result = 0x06;
                    break;
                case 0x15:
                    result = 0x07;
                    break;
                case 0x40:
                    result = 0x08;
                    break;
                case 0x41:
                    result = 0x09;
                    break;
                case 0x44:
                    result = 0x0A;
                    break;
                case 0x45:
                    result = 0x0B;
                    break;
                case 0x50:
                    result = 0x0C;
                    break;
                case 0x51:
                    result = 0x0D;
                    break;
                case 0x54:
                    result = 0x0E;
                    break;
                case 0x55:
                    result = 0x0F;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }
        public void VIO_OFF(int mipiPair)
        {
            switch (mipiPair)
            {
                case 0:
                    _ret += _myDm.DrivePin(Svio0, 0); //vio drive high
                    break;

                case 1:
                    _ret += _myDm.DrivePin(Svio1, 0); //vio drive high
                    break;
            }
        }
        public void VIO_ON(int mipiPair)
        {
            switch (mipiPair)
            {
                case 0:
                    _ret += _myDm.DrivePin(Svio0, 1); //vio drive high
                    break;

                case 1:
                    _ret += _myDm.DrivePin(Svio1, 1); //vio drive high
                    break;
            }
        }
        #region iMipiCtrl interface


        public void Init()
        {
            Initialization();
        }
        public void TurnOn_VIO(int pair)
        {
            VIO_ON(pair);
        }
        public void TurnOff_VIO(int pair)
        {
            VIO_OFF(pair);
        }
        public void SendAndReadMipiCodes(out bool readSuccessful, int mipiReg)
        {
            readSuccessful = Register_Change(mipiReg);
        }
        #endregion
    }
}
