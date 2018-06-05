using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Aemulus.Hardware.DM280e;

namespace LibEqmtDriver.MIPI
{
    public class AemulusDm280E : IIMiPiCtrl
    {
        //static string DM280_Address = "PXI2::0::INSTR";
        static readonly int Dm280InitOpt = 0xF;
        static readonly string Dm280OptString = "Simulate = 0, DriverSetup=Model:DM280e";

        readonly DM280e _aemDm280 = new DM280e(LibVar.ºmyDm280Address, Dm280InitOpt, Dm280OptString);

        public void Initialization()
        {
            //Set CLK Freq
            int dm280Clk = 26000000;
            double dm280Vio = 1.8;
            Config_MIPI_CLK(dm280Clk);
            Config_MIPI_Voltage(dm280Vio);
        }
        public void Configure_Loopback(int channel, int loopback)
        {
            _aemDm280.MIPI_ConfigureLoopback(channel, loopback);	
        }
        public void Configure_Delay(int channel, int delay)
        {
            _aemDm280.MIPI_ConfigureDelay(channel, delay);
        }
        public void Config_MIPI_CLK(int freq)
        {
            _aemDm280.MIPI_ConfigureClock(freq);
        }
        public void Config_MIPI_Voltage(double voltage) //added 13 August 2013
        {
            double actualVoltage = 0;
            _aemDm280.MIPI_ConfigureVoltageSupply(voltage, out actualVoltage);
        }

        public void MIPI_Reg_Write(int channel, int slaveAdd, int dataAdd, int data)
        {
            int wrCommand = 0;
            int[] dataFrame = new int[16];
            if (dataAdd == 0)
                wrCommand = (slaveAdd << 8) | (0x1 << 7) | (data);
            else if (dataAdd > 0 && dataAdd < 32)
            {
                wrCommand = (slaveAdd << 8) | (0x2 << 5) | (dataAdd);
                dataFrame[0] = data;
            }

            _aemDm280.MIPI_Write(channel, wrCommand, dataFrame);
        }
        public void MIPI_Reg_Read(int channel, int speed, int slaveAdd, int data)
        {

            int reCommand = (slaveAdd << 8) | (0x3 << 5) | (data);
            int[] dummyData = new int[2];  //Dummy

            _aemDm280.MIPI_Read(channel, speed, reCommand, dummyData);
        }

        public void MIPI_Retrieve(int channel, out int rdByteData, out int[] data, out int[] parity)
        {
            int tempRdByteData = 0;
            int[] tempData = new int[256];
            int[] tempParity = new int[256];

            _aemDm280.MIPI_Retrieve(channel, out tempRdByteData, tempData, tempParity);

            rdByteData = tempRdByteData;
            data = tempData;
            parity = tempParity;
        }
        public void MIPI_PM_Trigger_MLEO(int thisChannel)
        {

            int thisSlaveAdd = LibVar.ºSlaveAddress;
            int thisDataAdd = LibVar.ºPmTrig;
            int thisData = LibVar.ºPmTrigData;

            MIPI_Reg_Write(thisChannel, thisSlaveAdd, thisDataAdd, thisData);
        }

        public void MIPI_WR_RegX_MLEO(int thisChannel, string mipiRegCond, int thisRegNo)
        {
            bool regEn = new bool();
            int strLength = 0;
            int regDataInteger = 999;

            int regSlaveAdd = LibVar.ºSlaveAddress;
            int regChannel = 999;
            int regDataAdd = thisRegNo;

            strLength = mipiRegCond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                regChannel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiRegCond == "X" || mipiRegCond == "x")
                        regEn = false;
                    else
                        throw new Exception("Write Register "  + thisRegNo + " : invalid string to disable " + mipiRegCond);
                }
                else if (strLength == 2)
                {
                    int mipiRegInt = 999;

                    mipiRegInt = Convert.ToInt32(mipiRegCond, 16);

                    if (mipiRegInt >= 0 && mipiRegInt <= 255)
                    {
                        regEn = true;
                        regDataInteger = mipiRegInt;
                    }

                    else
                        throw new Exception("Write Register " + thisRegNo + " : invalid data (not between 00 to FF) " + mipiRegCond);
                }

                else if (strLength == 4)
                {
                    string reg = mipiRegCond.Substring(0, 2);
                    string regDataStr = "";
                    //string compare_reg_data_str = "0" + this_reg_no.ToString(); // variable to double confirm the register is correctly define in TCF
                    string compareRegDataStr = thisRegNo.ToString("X2");

                    if (reg == compareRegDataStr)
                    {
                        regDataStr = mipiRegCond.Substring(2);

                        int mipiRegInt = 999;

                        mipiRegInt = Convert.ToInt32(regDataStr, 16);

                        if (mipiRegInt >= 0 && mipiRegInt <= 255)
                        {
                            regEn = true;
                            regDataInteger = mipiRegInt;
                        }
                        else
                            throw new Exception("Write Register " + thisRegNo + " : invalid data (not between 00 to FF) " + regDataStr);
                    }
                    else
                    {
                        throw new Exception("Write Register " + thisRegNo + " : invalid data for register " + thisRegNo + mipiRegCond);
                    }
                }
                else
                {
                    throw new Exception("Write Register " + thisRegNo + " : invalid number of string " + mipiRegCond);
                }
            }
            else
                throw new Exception("Write Register " + thisRegNo + " : invalid channel number (DM280 Channel only 0 or 1) " + thisChannel);

            if (regEn == true)
            {
                MIPI_Reg_Write(regChannel, regSlaveAdd, regDataAdd, regDataInteger);
            }

            regEn = new bool();
        }
        public void MIPI_WR_Reg0_MLEO(int thisChannel, string mipiReg0Cond)
        {
            bool reg0En = new bool();
            int strLength = 0;
            int reg0DataInteger = 999;

            int reg0SlaveAdd = LibVar.ºSlaveAddress;
            int reg0Channel = 999;
            int reg0DataAdd = 0;

            strLength = mipiReg0Cond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                reg0Channel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiReg0Cond == "X" || mipiReg0Cond == "x")
                        reg0En = false;
                    else
                        throw new Exception("Write Register 0 : invalid string to disable " + mipiReg0Cond);
                }
                else if (strLength == 2)
                {
                    int mipiReg0Int = 999;

                    mipiReg0Int = Convert.ToInt32(mipiReg0Cond, 16);

                    if (mipiReg0Int >= 0 && mipiReg0Int <= 255)
                    {
                        reg0En = true;
                        reg0DataInteger = mipiReg0Int;
                    }

                    else
                        throw new Exception("Write Register 0 : invalid data (not between 00 to FF) " + mipiReg0Cond);
                }

                else if (strLength == 4)
                {
                    string reg = mipiReg0Cond.Substring(0, 2);
                    string reg0DataStr = "";

                    if (reg == "00")
                    {
                        reg0DataStr = mipiReg0Cond.Substring(2);

                        int mipiReg0Int = 999;

                        mipiReg0Int = Convert.ToInt32(reg0DataStr, 16);

                        if (mipiReg0Int >= 0 && mipiReg0Int <= 255)
                        {
                            reg0En = true;
                            reg0DataInteger = mipiReg0Int;
                        }
                        else
                            throw new Exception("Write Register 0 : invalid data (not between 00 to FF) " + reg0DataStr);
                    }
                    else
                    {
                        throw new Exception("Write Register 0 : invalid data for register 0" + mipiReg0Cond);
                    }
                }
                else
                {
                    throw new Exception("Write Register 0 : invalid number of string " + mipiReg0Cond);
                }
            }
            else
                throw new Exception("Write Register 0: invalid channel number (DM280 Channel only 0 or 1) " + thisChannel);

            if (reg0En == true)
            {
                MIPI_Reg_Write(reg0Channel, reg0SlaveAdd, reg0DataAdd, reg0DataInteger);
            }

            reg0En = new bool();
        }
        public void MIPI_WR_Reg1_MLEO(int thisChannel, string mipiReg1Cond)
        {
            bool reg1En = new bool();
            int strLength = 0;
            int reg1DataInteger = 999;

            int reg1SlaveAdd = LibVar.ºSlaveAddress;
            int reg1Channel = 999;
            int reg1DataAdd = 1;

            strLength = mipiReg1Cond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                reg1Channel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiReg1Cond == "X" || mipiReg1Cond == "x")
                        reg1En = false;
                    else
                        throw new Exception("Write Register 1 : invalid string to disable " + mipiReg1Cond);
                }
                else if (strLength == 2)
                {
                    int mipiReg1Int = 999;

                    mipiReg1Int = Convert.ToInt32(mipiReg1Cond, 16);

                    if (mipiReg1Int >= 0 && mipiReg1Int <= 255)
                    {
                        reg1En = true;
                        reg1DataInteger = mipiReg1Int;
                    }

                    else
                        throw new Exception("Write Register 1 : invalid data (not between 00 to FF) " + mipiReg1Cond);
                }

                else if (strLength == 4)
                {
                    string reg = mipiReg1Cond.Substring(0, 2);
                    string reg1DataStr = "";

                    if (reg == "01")
                    {
                        reg1DataStr = mipiReg1Cond.Substring(2);

                        int mipiReg1Int = 999;

                        mipiReg1Int = Convert.ToInt32(reg1DataStr, 16);

                        if (mipiReg1Int >= 0 && mipiReg1Int <= 255)
                        {
                            reg1En = true;
                            reg1DataInteger = mipiReg1Int;
                        }
                        else
                            throw new Exception("Write Register 1 : invalid data (not between 00 to FF) " + reg1DataStr);
                    }
                    else
                    {
                        throw new Exception("Write Register 1 : invalid data for register 0" + mipiReg1Cond);
                    }
                }
                else
                {
                    throw new Exception("Write Register 1 : invalid number of string " + mipiReg1Cond);
                }
            }
            else
                throw new Exception("Write Register 1: invalid channel number (DM280 Channel only 0 or 1) " + thisChannel);

            if (reg1En == true)
            {
                MIPI_Reg_Write(reg1Channel, reg1SlaveAdd, reg1DataAdd, reg1DataInteger);
            }

            reg1En = new bool();
        }
        public void MIPI_WR_Reg2_MLEO(int thisChannel, string mipiReg2Cond)
        {
            bool reg2En = new bool();
            int strLength = 0;
            int reg2DataInteger = 999;

            int reg2SlaveAdd = LibVar.ºSlaveAddress;
            int reg2Channel = 999;
            int reg2DataAdd = 2;      //be carefull - hardcoded reg address

            strLength = mipiReg2Cond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                reg2Channel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiReg2Cond == "X" || mipiReg2Cond == "x")
                        reg2En = false;
                    else
                        throw new Exception("Write Register 2 : invalid string to disable " + mipiReg2Cond);
                }
                else if (strLength == 2)
                {
                    int mipiReg2Int = 999;

                    mipiReg2Int = Convert.ToInt32(mipiReg2Cond, 16);

                    if (mipiReg2Int >= 0 && mipiReg2Int <= 255)
                    {
                        reg2En = true;
                        reg2DataInteger = mipiReg2Int;
                    }

                    else
                        throw new Exception("Write Register 1 : invalid data (not between 00 to FF) " + mipiReg2Cond);
                }

                else if (strLength == 4)
                {
                    string reg = mipiReg2Cond.Substring(0, 2);
                    string reg2DataStr = "";

                    if (reg == "02")
                    {
                        reg2DataStr = mipiReg2Cond.Substring(2);

                        int mipiReg2Int = 999;

                        mipiReg2Int = Convert.ToInt32(reg2DataStr, 16);

                        if (mipiReg2Int >= 0 && mipiReg2Int <= 255)
                        {
                            reg2En = true;
                            reg2DataInteger = mipiReg2Int;
                        }
                        else
                            throw new Exception("Write Register 2 : invalid data (not between 00 to FF) " + reg2DataStr);
                    }
                    else
                    {
                        throw new Exception("Write Register 2 : invalid data for register 0" + mipiReg2Cond);
                    }
                }
                else
                {
                    throw new Exception("Write Register 2 : invalid number of string " + mipiReg2Cond);
                }
            }
            else
                throw new Exception("Write Register 2: invalid channel number (DM280 Channel only 0 or 1) " + thisChannel);

            if (reg2En == true)
            {
                MIPI_Reg_Write(reg2Channel, reg2SlaveAdd, reg2DataAdd, reg2DataInteger);
            }

            reg2En = new bool();
        }
        public void MIPI_WR_Reg_MLEO(int thisChannel, string mipiRegCond)
        {
            bool regEn = new bool();
            int strLength = 0;
            int regDataInteger = 0;

            int regSlaveAdd = LibVar.ºSlaveAddress;
            int regChannel = 999;
            int regDataAdd = 999;

            strLength = mipiRegCond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                regChannel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiRegCond == "X" || mipiRegCond == "x")
                        regEn = false;
                    else
                        throw new Exception("Write Register : invalid string to disable " + mipiRegCond);
                }
                else if (strLength == 4)
                {
                    string reg = mipiRegCond.Substring(0, 2);
                    string regDataStr = "";

                    if (reg == "00")
                    {
                        regDataStr = mipiRegCond.Substring(2);

                        int mipiReg0Int = 999;

                        mipiReg0Int = Convert.ToInt32(regDataStr, 16);

                        if (mipiReg0Int >= 0 && mipiReg0Int <= 255)
                        {
                            regEn = true;
                            regDataInteger = mipiReg0Int;
                            regDataAdd = 0;
                        }
                        else
                            throw new Exception("Write Register 0 : Register 0 invalid data (not between 00 to FF) " + regDataStr);
                    }
                    else if (reg == "01")
                    {
                        regDataStr = mipiRegCond.Substring(2);

                        int mipiReg1Int = 999;

                        mipiReg1Int = Convert.ToInt32(regDataStr, 16);

                        if (mipiReg1Int >= 0 && mipiReg1Int <= 255)
                        {
                            regEn = true;
                            regDataInteger = mipiReg1Int;
                            regDataAdd = 1;
                        }
                        else
                            throw new Exception("Write Register : Register 1 invalid data (not between 00 to FF) " + regDataStr);

                    }
                    else
                    {
                        throw new Exception("Write Register : invalid string (only allow 4 charecters '0000' " + mipiRegCond);
                    }
                }
                else
                {
                    throw new Exception("Write Register : invalid number of string " + mipiRegCond);
                }
            }
            else
                throw new Exception("Write Register : invalid channel number " + thisChannel);

            if (regEn == true)
            {
                MIPI_Reg_Write(regChannel, regSlaveAdd, regDataAdd, regDataInteger);
            }

            regEn = new bool();
        }

        public void MIPI_Reg_RD_Array(int thisChannel, out string[] rdBkReg, int thisRegNo)
        {
            int regSlaveAdd = LibVar.ºSlaveAddress;
            int thisRdByteDataCount = 0;
            int[] retrieveData = new int[256];
            int[] retrieveParity = new int[256];
            rdBkReg = new string[thisRegNo];
            int i;

            //Config_MIPI_CLK(23000000);
            _aemDm280.MIPI_ConfigureDelay(thisChannel, 3);

            for (i = 0; i < thisRegNo; i++)
            {
                MIPI_Reg_Read(thisChannel, 1, regSlaveAdd, i);
            }
            MIPI_Retrieve(thisChannel, out thisRdByteDataCount, out retrieveData, out retrieveParity);

            for (i = 0; i < thisRegNo; i++)
            {
                //rd_bk_reg[i] = "0" + i.ToString() + retrieve_data[i].ToString("X2");
                rdBkReg[i] = i.ToString("X2") + retrieveData[i].ToString("X2");
            }

        }
        public void MIPI_Reg_RD(int thisChannel, out string rdBkReg0, out string rdBkReg1, out string rdBkReg2)
        {
            int regSlaveAdd = LibVar.ºSlaveAddress;
            int thisRdByteDataCount = 0;
            int[] retrieveData = new int[256];
            int[] retrieveParity = new int[256];

            //Config_MIPI_CLK(23000000);
            _aemDm280.MIPI_ConfigureDelay(thisChannel, 3);
            MIPI_Reg_Read(thisChannel, 1, regSlaveAdd, 0);
            MIPI_Reg_Read(thisChannel, 1, regSlaveAdd, 1);
            MIPI_Reg_Read(thisChannel, 1, regSlaveAdd, 2);
            MIPI_Retrieve(thisChannel, out thisRdByteDataCount, out retrieveData, out retrieveParity);

            rdBkReg0 = "00" + retrieveData[0].ToString("X2");
            rdBkReg1 = "01" + retrieveData[1].ToString("X2");
            rdBkReg2 = "02" + retrieveData[2].ToString("X2");

        }
        public void MIPI_Reg_RD(int thisChannel, out string rdBk_1C)
        {
            int regSlaveAdd = LibVar.ºSlaveAddress;
            int thisRdByteDataCount = 0;
            int[] retrieveData = new int[256];
            int[] retrieveParity = new int[256];

            //Config_MIPI_CLK(23000000);
            _aemDm280.MIPI_ConfigureDelay(thisChannel, 3);
            MIPI_Reg_Read(thisChannel, 1, regSlaveAdd, 0x1C);
            //MIPI_Reg_Read(this_channel, 1, reg_slave_add, 1);
            MIPI_Retrieve(thisChannel, out thisRdByteDataCount, out retrieveData, out retrieveParity);

            rdBk_1C = "1C" + retrieveData[0].ToString("X2");
        }


        public string MIPI_WR_Reg0_MLEO_Complete(int thisChannel, string mipiReg0Cond)
        {
            bool reg0En = new bool();
            int strLength = 0;
            int reg0DataInteger = 999;

            int reg0SlaveAdd = LibVar.ºSlaveAddress;
            int reg0Channel = 999;
            int reg0DataAdd = 0;

            int pmTrigDataAdd = LibVar.ºPmTrig;
            int pmTrigData = LibVar.ºPmTrigData;

            int thisRdByteDataCount = 0;
            int[] retrieveData = new int[256];
            int[] retrieveParity = new int[256];
            string ret = "";

            strLength = mipiReg0Cond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                reg0Channel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiReg0Cond == "X" || mipiReg0Cond == "x")
                    {
                        reg0En = false;
                        ret = mipiReg0Cond;
                    }
                    else
                        throw new Exception("Write Register 0 : invalid string to disable " + mipiReg0Cond);
                }
                else if (strLength == 2)
                {
                    int mipiReg0Int = 999;

                    mipiReg0Int = Convert.ToInt32(mipiReg0Cond, 16);

                    if (mipiReg0Int >= 0 && mipiReg0Int <= 255)
                    {
                        reg0En = true;
                        reg0DataInteger = mipiReg0Int;
                    }

                    else
                        throw new Exception("Write Register 0 : invalid data (not between 00 to FF) " + mipiReg0Cond);
                }

                else if (strLength == 4)
                {
                    string reg = mipiReg0Cond.Substring(0, 2);
                    string reg0DataStr = "";

                    if (reg == "00")
                    {
                        reg0DataStr = mipiReg0Cond.Substring(2);

                        int mipiReg0Int = 999;

                        mipiReg0Int = Convert.ToInt32(reg0DataStr, 16);

                        if (mipiReg0Int >= 0 && mipiReg0Int <= 255)
                        {
                            reg0En = true;
                            reg0DataInteger = mipiReg0Int;
                        }
                        else
                            throw new Exception("Write Register 0 : invalid data (not between 00 to FF) " + reg0DataStr);
                    }
                    else
                    {
                        throw new Exception("Write Register 0 : invalid data for register 0" + mipiReg0Cond);
                    }
                }
                else
                {
                    throw new Exception("Write Register 0 : invalid number of string " + mipiReg0Cond);
                }
            }
            else
                throw new Exception("Write Register 0: invalid channel number (DM280 Channel only 0 or 1) " + thisChannel);

            if (reg0En == true)
            {
                MIPI_Reg_Write(reg0Channel, reg0SlaveAdd, reg0DataAdd, reg0DataInteger);
                Thread.Sleep(1);
                MIPI_Reg_Write(thisChannel, reg0SlaveAdd, pmTrigDataAdd, pmTrigData);

                Config_MIPI_CLK(9000000);
                MIPI_Reg_Read(thisChannel, 1, reg0SlaveAdd, 0);
                MIPI_Retrieve(thisChannel, out thisRdByteDataCount, out retrieveData, out retrieveParity);
                Config_MIPI_CLK(26000000);

                if (retrieveData[0] == reg0DataInteger)
                {
                    reg0En = new bool();
                    ret = retrieveData[0].ToString("X2");
                }
                else
                {
                    reg0En = new bool();
                    ret = "999";
                }
            }
            return ret;
        }
        public string MIPI_WR_Reg1_MLEO_Complete(int thisChannel, string mipiReg1Cond)
        {
            bool reg1En = new bool();
            int strLength = 0;
            int reg1DataInteger = 999;

            int reg1SlaveAdd = LibVar.ºSlaveAddress;
            int reg1Channel = 999;
            int reg1DataAdd = 1;

            int pmTrigDataAdd = LibVar.ºPmTrig;
            int pmTrigData = LibVar.ºPmTrigData;

            int thisRdByteDataCount = 0;
            int[] retrieveData = new int[256];
            int[] retrieveParity = new int[256];
            string ret = "";

            strLength = mipiReg1Cond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                reg1Channel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiReg1Cond == "X" || mipiReg1Cond == "x")
                    {
                        reg1En = false;
                        ret = mipiReg1Cond;
                    }
                    else
                        throw new Exception("Write Register 1 : invalid string to disable " + mipiReg1Cond);
                }
                else if (strLength == 2)
                {
                    int mipiReg1Int = 999;

                    mipiReg1Int = Convert.ToInt32(mipiReg1Cond, 16);

                    if (mipiReg1Int >= 0 && mipiReg1Int <= 255)
                    {
                        reg1En = true;
                        reg1DataInteger = mipiReg1Int;
                    }

                    else
                        throw new Exception("Write Register 1 : invalid data (not between 00 to FF) " + mipiReg1Cond);
                }

                else if (strLength == 4)
                {
                    string reg = mipiReg1Cond.Substring(0, 2);
                    string reg1DataStr = "";

                    if (reg == "01")
                    {
                        reg1DataStr = mipiReg1Cond.Substring(2);

                        int mipiReg1Int = 999;

                        mipiReg1Int = Convert.ToInt32(reg1DataStr, 16);

                        if (mipiReg1Int >= 0 && mipiReg1Int <= 255)
                        {
                            reg1En = true;
                            reg1DataInteger = mipiReg1Int;
                        }
                        else
                            throw new Exception("Write Register 1 : invalid data (not between 00 to FF) " + reg1DataStr);
                    }
                    else
                    {
                        throw new Exception("Write Register 1 : invalid data for register 0" + mipiReg1Cond);
                    }
                }
                else
                {
                    throw new Exception("Write Register 1 : invalid number of string " + mipiReg1Cond);
                }
            }
            else
                throw new Exception("Write Register 1: invalid channel number (DM280 Channel only 0 or 1) " + thisChannel);

            if (reg1En == true)
            {
                MIPI_Reg_Write(reg1Channel, reg1SlaveAdd, reg1DataAdd, reg1DataInteger);
                Thread.Sleep(1);
                MIPI_Reg_Write(thisChannel, reg1SlaveAdd, pmTrigDataAdd, pmTrigData);

                Config_MIPI_CLK(9000000);
                MIPI_Reg_Read(thisChannel, 1, reg1SlaveAdd, 1);
                MIPI_Retrieve(thisChannel, out thisRdByteDataCount, out retrieveData, out retrieveParity);
                Config_MIPI_CLK(26000000);


                if (retrieveData[0] == reg1DataInteger)
                {
                    reg1En = new bool();
                    ret = retrieveData[0].ToString("X2");
                }
                else
                {
                    reg1En = new bool();
                    ret = "999";
                }
            }
            return ret;
        }
        public string MIPI_WR_Reg2_MLEO_Complete(int thisChannel, string mipiReg2Cond)
        {
            bool reg2En = new bool();
            int strLength = 0;
            int reg2DataInteger = 999;

            int reg2SlaveAdd = LibVar.ºSlaveAddress;
            int reg2Channel = 999;
            int reg2DataAdd = 1;

            int pmTrigDataAdd = LibVar.ºPmTrig;
            int pmTrigData = LibVar.ºPmTrigData;

            int thisRdByteDataCount = 0;
            int[] retrieveData = new int[256];
            int[] retrieveParity = new int[256];
            string ret = "";

            strLength = mipiReg2Cond.Length;

            if (thisChannel == 0 || thisChannel == 1)
            {
                reg2Channel = thisChannel;

                if (strLength == 1)
                {
                    if (mipiReg2Cond == "X" || mipiReg2Cond == "x")
                    {
                        reg2En = false;
                        ret = mipiReg2Cond;
                    }
                    else
                        throw new Exception("Write Register 2 : invalid string to disable " + mipiReg2Cond);
                }
                else if (strLength == 2)
                {
                    int mipiReg2Int = 999;

                    mipiReg2Int = Convert.ToInt32(mipiReg2Cond, 16);

                    if (mipiReg2Int >= 0 && mipiReg2Int <= 255)
                    {
                        reg2En = true;
                        reg2DataInteger = mipiReg2Int;
                    }

                    else
                        throw new Exception("Write Register 2 : invalid data (not between 00 to FF) " + mipiReg2Cond);
                }

                else if (strLength == 4)
                {
                    string reg = mipiReg2Cond.Substring(0, 2);
                    string reg2DataStr = "";

                    if (reg == "01")
                    {
                        reg2DataStr = mipiReg2Cond.Substring(2);

                        int mipiReg2Int = 999;

                        mipiReg2Int = Convert.ToInt32(reg2DataStr, 16);

                        if (mipiReg2Int >= 0 && mipiReg2Int <= 255)
                        {
                            reg2En = true;
                            reg2DataInteger = mipiReg2Int;
                        }
                        else
                            throw new Exception("Write Register 2 : invalid data (not between 00 to FF) " + reg2DataStr);
                    }
                    else
                    {
                        throw new Exception("Write Register 2 : invalid data for register 0" + mipiReg2Cond);
                    }
                }
                else
                {
                    throw new Exception("Write Register 2 : invalid number of string " + mipiReg2Cond);
                }
            }
            else
                throw new Exception("Write Register 2: invalid channel number (DM280 Channel only 0 or 1) " + thisChannel);

            if (reg2En == true)
            {
                MIPI_Reg_Write(reg2Channel, reg2SlaveAdd, reg2DataAdd, reg2DataInteger);
                Thread.Sleep(1);
                MIPI_Reg_Write(thisChannel, reg2SlaveAdd, pmTrigDataAdd, pmTrigData);

                Config_MIPI_CLK(9000000);
                MIPI_Reg_Read(thisChannel, 1, reg2SlaveAdd, 1);
                MIPI_Retrieve(thisChannel, out thisRdByteDataCount, out retrieveData, out retrieveParity);
                Config_MIPI_CLK(26000000);


                if (retrieveData[0] == reg2DataInteger)
                {
                    reg2En = new bool();
                    ret = retrieveData[0].ToString("X2");
                }
                else
                {
                    reg2En = new bool();
                    ret = "999";
                }
            }
            return ret;
        }

        #region iMipiCtrl interface

        public void Init()
        {
            Initialization();
        }
        public void TurnOn_VIO(int pair)
        {
            //not implemented
        }
        public void TurnOff_VIO(int pair)
        {
            //not implemented
        }
        public void SendAndReadMipiCodes(out bool boolreadSuccessful, int mipiReg)
        {
            int[] mipiArr = new int[mipiReg];
            boolreadSuccessful = true;
            bool[] readSuccessful = new bool[mipiReg];
            string[] regXValue = new string[mipiReg];
            string[] mipiRegCond = new string[mipiReg];
            int i;
            int regCnt;
            int passRd, failRd;

            //Initialize variable
            i = 0; regCnt = 0;
            for (regCnt = 0; regCnt < mipiReg; regCnt++)
            {
                readSuccessful[regCnt] = true;
                regXValue[regCnt] = "";
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

            for (i = 0; i < 10; i++)
            {
                regCnt = 0; passRd = 0; failRd = 0; //reset read success counter

                for (regCnt = 0; regCnt < mipiReg; regCnt++)
                {
                    MIPI_WR_RegX_MLEO(LibEqmtDriver.MIPI.LibVar.ºChannelUsed, mipiRegCond[regCnt], regCnt);
                    MIPI_PM_Trigger_MLEO(LibEqmtDriver.MIPI.LibVar.ºChannelUsed);
                }

                MIPI_Reg_RD_Array(LibEqmtDriver.MIPI.LibVar.ºChannelUsed, out regXValue, mipiReg);

                for (regCnt = 0; regCnt < mipiReg; regCnt++)
                {
                    if (mipiRegCond[regCnt].Length > 1)
                    {
                        if (mipiRegCond[regCnt].Length == 2)
                        {
                            string regDataStr = regXValue[regCnt].Substring(2, 2);
                            if (mipiRegCond[regCnt] != regDataStr && LibEqmtDriver.MIPI.LibVar.ºReadFunction == true)
                                readSuccessful[regCnt] = false;
                            else
                                readSuccessful[regCnt] = true;
                        }
                        if (mipiRegCond[regCnt].Length == 4)
                        {
                            if (mipiRegCond[regCnt] != regXValue[regCnt] && LibEqmtDriver.MIPI.LibVar.ºReadFunction == true)
                                readSuccessful[regCnt] = false;
                            else
                                readSuccessful[regCnt] = true;
                        }
                    }
                }

                for (regCnt = 0; regCnt < mipiReg; regCnt++)
                {
                    if (readSuccessful[regCnt] == true)
                        passRd++;
                    else
                        failRd++;
                }

                if (passRd == (mipiReg))
                {
                    boolreadSuccessful = true;
                    break;
                }
                else
                    boolreadSuccessful = false;
            }
        }
        #endregion
    }
}
