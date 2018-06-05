using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace LibEqmtDriver.SCU
{
    public class AeWofer : IISwitch  
    {
      
        int _mRetVal, _session, _hSys;
        string _apiName = "";

        public AeWofer(int sysId)
        {
            Createsession(null);
            AMB1340C_CREATEINSTANCE(sysId, 0);
        }

        #region iSwitch Members

        void IISwitch.Initialize()
        {
            try
            {
                _apiName = "INITIALIZE"; RetVal = AMb1340C.INITIALIZE(_hSys);
                AMB1340C_SETPORTDIRECTION(65535);
            }
            catch (Exception ex)
            {
                throw new Exception("AeWofer: Initialize -> " + ex.Message);
            }
            
        }

        void IISwitch.SetPath(string val)
        {
            string[] tempdata;
            tempdata = val.Split(';');
            //string val0 = tempdata[0];
            //string val1 = tempdata[1];

            try
            {
                for (int i = 0; i < tempdata.Length; i++)
                {
                    //AMB1340C_DRIVETHISPORT(0, Convert.ToInt32(val0));
                    //AMB1340C_DRIVETHISPORT(1, Convert.ToInt32(val1));

                    AMB1340C_DRIVETHISPORT(i, Convert.ToInt32(tempdata[i]));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AeWofer: SetPath -> " + ex.Message);
            }
        }

        void IISwitch.Reset()
        {
            try
            {
                Resetboards();
            }
            catch (Exception ex)
            {
                throw new Exception("AeWofer: Reset -> " + ex.Message);
            }
        }

        #endregion


        private void Resetboards()
        {
            _apiName = "RESETBOARDS"; RetVal = AMb1340C.RESETBOARDS();
        }
        private void Createsession(string hostname)
        {
            _apiName = "CREATESESSION"; RetVal = AMb1340C.CREATESESSION(hostname, out _session);
        }
        private void Closesession()
        {
            _apiName = "CLOSESESSION"; RetVal = AMb1340C.CLOSESESSION(_session);
        }
        private void AMB1340C_CREATEINSTANCE(int sysId, int offlinemode)
        {
            _apiName = "AMB1340C_CREATEINSTANCE"; RetVal = AMb1340C.AMB1340C_CREATEINSTANCE(_session, sysId, offlinemode, out _hSys);
        }
        private void AMB1340C_DELETEINSTANCE()
        {
            _apiName = "AMB1340C_DELETEINSTANCE"; RetVal = AMb1340C.AMB1340C_DELETEINSTANCE(_hSys);
        }

        private void AMB1340C_DRIVEPORT(int value)
        {
            _apiName = "AMB1340C_DRIVEPORT"; RetVal = AMb1340C.DRIVEPORT(_hSys, value);
        }
        private void AMB1340C_DRIVETHISPORT(int port, int value)
        {
            _apiName = "AMB1340C_DRIVETHISPORT"; RetVal = AMb1340C.DRIVETHISPORT(_hSys, port, value);
        }
        private void AMB1340C_DRIVEPIN(int pin, int value)
        {
            _apiName = "AMB1340C_DRIVEPIN"; RetVal = AMb1340C.DRIVEPIN(_hSys, pin, value);
        }
        private void AMB1340C_READPORT(out int value)
        {
            _apiName = "AMB1340C_READPORT"; RetVal = AMb1340C.READPORT(_hSys, out value);
        }
        private void AMB1340C_READPIN(int pin, out int value)
        {
            _apiName = "AMB1340C_READPIN"; RetVal = AMb1340C.READPIN(_hSys, pin, out value);
        }
        private void AMB1340C_SETPORTDIRECTION(int value)
        {
            _apiName = "AMB1340C_SETPORTDIRECTION"; RetVal = AMb1340C.SETPORTDIRECTION(_hSys, value);
        }
        private void AMB1340C_SETPINDIRECTION(int pin, int value)
        {
            _apiName = "AMB1340C_SETPINDIRECTION"; RetVal = AMb1340C.SETPINDIRECTION(_hSys, pin, value);
        }
        private void AMB1340C_GETPORTDIRECTION(out int value)
        {
            _apiName = "AMB1340C_GETPORTDIRECTION"; RetVal = AMb1340C.GETPORTDIRECTION(_hSys, out value);
        }
        private void AMB1340C_GETPINDIRECTION(int pin, out int value)
        {
            _apiName = "AMB1340C_GETPINDIRECTION"; RetVal = AMb1340C.GETPINDIRECTION(_hSys, pin, out value);
        }

        private void Drivevoltage(int chset, int mVvalue, int sign)
        {
            _apiName = "DRIVEVOLTAGE"; RetVal = AMb1340C.DRIVEVOLTAGE(_hSys, chset, mVvalue, sign);
        }
        private void Drivecurrent(int chset, int nAvalue, int sign)
        {
            _apiName = "DRIVECURRENT"; RetVal = AMb1340C.DRIVECURRENT(_hSys, chset, nAvalue, sign);
        }
        private void Clampvoltage(int chset, int mVvalue)
        {
            _apiName = "CLAMPVOLTAGE"; RetVal = AMb1340C.CLAMPVOLTAGE(_hSys, chset, mVvalue);
        }
        private void Clampcurrent(int chset, int nAvalue)
        {
            _apiName = "CLAMPCURRENT"; RetVal = AMb1340C.CLAMPCURRENT(_hSys, chset, nAvalue);
        }

        private void Readvoltage(int chset, out int mVvalue)
        {
            _apiName = "READVOLTAGE"; RetVal = AMb1340C.READVOLTAGE(_hSys, chset, out mVvalue);
        }
        private void Readcurrent(int chset, out int nAvalue)
        {
            _apiName = "READCURRENT"; RetVal = AMb1340C.READCURRENT(_hSys, chset, out nAvalue);
        }
        private void Readcurrentrate(int chset, out int nAvalue)
        {
            _apiName = "READCURRENTRATE"; RetVal = AMb1340C.READCURRENTRATE(_hSys, chset, out nAvalue);
        }
        private void Readvoltagevolt(int chset, out float volt)
        {
            _apiName = "READVOLTAGEVOLT"; RetVal = AMb1340C.READVOLTAGEVOLT(_hSys, chset, out volt);
        }
        private void Readcurrentamp(int chset, out float ampere)
        {
            _apiName = "READCURRENTAMP"; RetVal = AMb1340C.READCURRENTAMP(_hSys, chset, out ampere);
        }
        private void Readcurrentamprate(int chset, out float ampere)
        {
            _apiName = "READCURRENTAMPRATE"; RetVal = AMb1340C.READCURRENTAMPRATE(_hSys, chset, out ampere);
        }
        private void Readvoltagewithaverage(int chset, int average, out int average_MV, out int every_MV)
        {
            _apiName = "READVOLTAGEWITHAVERAGE"; RetVal = AMb1340C.READVOLTAGEWITHAVERAGE(_hSys, chset, average, out average_MV, out every_MV);
        }
        private void Readcurrentwithaverage(int chset, int average, out int average_NA, out int every_NA)
        {
            _apiName = "READCURRENTWITHAVERAGE"; RetVal = AMb1340C.READCURRENTWITHAVERAGE(_hSys, chset, average, out average_NA, out every_NA);
        }
        private void Readcurrentautorange(int chset, out int nAvalue)
        {
            _apiName = "READCURRENTAUTORANGE"; RetVal = AMb1340C.READCURRENTAUTORANGE(_hSys, chset, out nAvalue);
        }
        private void Readcurrentfromrange(int chset, out int nAvalue)
        {
            _apiName = "READCURRENTFROMRANGE"; RetVal = AMb1340C.READCURRENTFROMRANGE(_hSys, chset, out nAvalue);
        }

        private void Onsmupin(int pin)
        {
            _apiName = "ONSMUPIN"; RetVal = AMb1340C.ONSMUPIN(_hSys, pin);
        }
        private void Offsmupin(int pin)
        {
            _apiName = "OFFSMUPIN"; RetVal = AMb1340C.OFFSMUPIN(_hSys, pin);
        }
        private void Setanapinbandwidth(int pin, int setting)
        {
            _apiName = "SETANAPINBANDWIDTH"; RetVal = AMb1340C.SETANAPINBANDWIDTH(_hSys, pin, setting);
        }
        private void Setintegration(int chdat)
        {
            _apiName = "SETINTEGRATION"; RetVal = AMb1340C.SETINTEGRATION(_hSys, chdat);
        }
        private void Setintegrationpowercycles(int setting, int powerCycles)
        {
            _apiName = "SETINTEGRATIONPOWERCYCLES"; RetVal = AMb1340C.SETINTEGRATIONPOWERCYCLES(_hSys, setting, powerCycles);
        }
       

        private void Biassmupin(int chset, out int chdat)
        {
            _apiName = "BIASSMUPIN"; RetVal = AMb1340C.BIASSMUPIN(_hSys, chset, out chdat);
        }
        private void Readsmupin(int chset, out int chdat, out int chRead)
        {
            _apiName = "READSMUPIN"; RetVal = AMb1340C.READSMUPIN(_hSys, chset, out chdat, out chRead);
        }

        //Modified by ChoonChin for IccCal
        private int READSMUPIN_int(int chset, out int chdat, out int chRead)
        {
            _apiName = "READSMUPIN"; int a  = AMb1340C.READSMUPIN(_hSys, chset, out chdat, out chRead);
            return a;
        }

        private void Readsmupinrate(int chset, out int chdat, out int chRead)
        {
            _apiName = "READSMUPINRATE"; RetVal = AMb1340C.READSMUPINRATE(_hSys, chset, out chdat, out chRead);
        }
        private void Onoffsmupin(int chset, out int chdat)
        {
            _apiName = "ONOFFSMUPIN"; RetVal = AMb1340C.ONOFFSMUPIN(_hSys, chset, out chdat);
        }

        private void Armreadsmupin(int measset, out int chdat)
        {
            _apiName = "ARMREADSMUPIN"; RetVal = AMb1340C.ARMREADSMUPIN(_hSys, measset, out chdat);
        }
        private void Retrievereadsmupin(int measset, out int chdat, out int chRead)
        {
            _apiName = "RETRIEVEREADSMUPIN"; RetVal = AMb1340C.RETRIEVEREADSMUPIN(_hSys, measset, out chdat, out chRead);
        }

        private void Sourcedelaymeasuresmupin(int chset, out int chdat, out int chRead, int sequence)
        {
            _apiName = "SOURCEDELAYMEASURESMUPIN";
            RetVal = AMb1340C.SOURCEDELAYMEASURESMUPIN(_hSys, chset, out chdat, out chRead, sequence);
        }

        private void AMB1340C_SOURCEDELAYMEASURESMUPIN(int pinset, out float pindat, out int measset, out float pinRead, int sequence)
        {
            _apiName = "AMB1340C_SOURCEDELAYMEASURESMUPIN";
            RetVal = AMb1340C.AMB1340C_SOURCEDELAYMEASURESMUPIN(_hSys, pinset, out pindat, out measset, out pinRead, sequence);
        }


        private void AM330_DRIVEPULSEVOLTAGE(int pin, float _base, float pulse, float pulseS, float holdS,
            int drVrange, int cycles, int measCh, int measSel, int measVrange, int trigPercentage,
            int armExtTriginH, float timeoutS)
        {
            _apiName = "AM330_DRIVEPULSEVOLTAGE"; RetVal = AMb1340C.AM330_DRIVEPULSEVOLTAGE(_hSys, pin, _base, pulse, pulseS, holdS, drVrange, cycles, measCh, measSel, measVrange, trigPercentage, armExtTriginH, timeoutS);
        }


        private void AM371_DRIVEVOLTAGE(int pin, float volt)
        {
            _apiName = "AM371_DRIVEVOLTAGE"; RetVal = AMb1340C.AM371_DRIVEVOLTAGE(_hSys, pin, volt);
        }
        private void AM371_DRIVECURRENT(int pin, float ampere)
        {
            _apiName = "AM371_DRIVECURRENT"; RetVal = AMb1340C.AM371_DRIVECURRENT(_hSys, pin, ampere);
        }
        private void AM371_DRIVEVOLTAGESETVRANGE(int pin, float volt, int vrange)
        {
            _apiName = "AM371_DRIVEVOLTAGESETVRANGE"; RetVal = AMb1340C.AM371_DRIVEVOLTAGESETVRANGE(_hSys, pin, volt, vrange);
        }
        private void AM371_DRIVECURRENTSETIRANGE(int pin, float ampere, int irange)
        {
            _apiName = "AM371_DRIVECURRENTSETIRANGE"; RetVal = AMb1340C.AM371_DRIVECURRENTSETIRANGE(_hSys, pin, ampere, irange);
        }
        private void AM371_CLAMPVOLTAGE(int pin, float volt)
        {
            _apiName = "AM371_CLAMPVOLTAGE"; RetVal = AMb1340C.AM371_CLAMPVOLTAGE(_hSys, pin, volt);
        }
        private void AM371_CLAMPCURRENT(int pin, float ampere)
        {
            _apiName = "AM371_CLAMPCURRENT"; RetVal = AMb1340C.AM371_CLAMPCURRENT(_hSys, pin, ampere);
        }
        private void AM371_CLAMPVOLTAGESETVRANGE(int pin, float volt, int vrange)
        {
            _apiName = "AM371_CLAMPVOLTAGESETVRANGE"; RetVal = AMb1340C.AM371_CLAMPVOLTAGESETVRANGE(_hSys, pin, volt, vrange);
        }
        private void AM371_CLAMPCURRENTSETIRANGE(int pin, float ampere, int irange)
        {
            _apiName = "AM371_CLAMPCURRENTSETIRANGE"; RetVal = AMb1340C.AM371_CLAMPCURRENTSETIRANGE(_hSys, pin, ampere, irange);
        }

        private void AM371_READVOLTAGE(int pin, out float volt)
        {
            _apiName = "AM371_READVOLTAGE"; RetVal = AMb1340C.AM371_READVOLTAGE(_hSys, pin, out volt);
        }
        private void AM371_READVOLTAGEGETVRANGE(int pin, out float volt, out int vrange)
        {
            _apiName = "AM371_READVOLTAGEGETVRANGE"; RetVal = AMb1340C.AM371_READVOLTAGEGETVRANGE(_hSys, pin, out volt, out vrange);
        }
        private void AM371_READCURRENT(int pin, out float ampere)
        {
            _apiName = "AM371_READCURRENT"; RetVal = AMb1340C.AM371_READCURRENT(_hSys, pin, out ampere);
        }
        private void AM371_READCURRENTRATE(int pin, out float ampere)
        {
            _apiName = "AM371_READCURRENTRATE"; RetVal = AMb1340C.AM371_READCURRENTRATE(_hSys, pin, out ampere);
        }
        private void AM371_READCURRENTGETIRANGE(int pin, out float ampere, out int irange)
        {
            _apiName = "AM371_READCURRENTGETIRANGE"; RetVal = AMb1340C.AM371_READCURRENTGETIRANGE(_hSys, pin, out ampere, out irange);
        }

        private void AM371_ONSMUPIN(int pin, int remoteSenseH)
        {
            _apiName = "AM371_ONSMUPIN"; RetVal = AMb1340C.AM371_ONSMUPIN(_hSys, pin, remoteSenseH);
        }
        private void AM371_OFFSMUPIN(int pin)
        {
            _apiName = "AM371_OFFSMUPIN"; RetVal = AMb1340C.AM371_OFFSMUPIN(_hSys, pin);
        }

        private int AM371_EXTTRIGARM_READCURRENTARRAY(int pin, int posedgeH, float delayS, int nsample, float sampleDelayS)
        {
            _apiName = "AM371_EXTTRIGARM_READCURRENTARRAY";
            return AMb1340C.AM371_EXTTRIGARM_READCURRENTARRAY(_hSys, pin, posedgeH, delayS, nsample, sampleDelayS);
        }
        private int AM371_EXTTRIGGET_READCURRENTARRAY_WITH_MINMAX(int pin, out int nsample, float[] iarray, out float min, out float max, out float average)
        {
            _apiName = "AM371_EXTTRIGGET_READCURRENTARRAY_WITH_MINMAX";
            return AMb1340C.AM371_EXTTRIGGET_READCURRENTARRAY_WITH_MINMAX(_hSys, pin, out nsample, iarray, out min, out max, out average);
        }

        private void AM371_EXTTRIGARM_RELEASE(int pin)
        {
            _apiName = "AM371_EXTTRIGARM_RELEASE"; RetVal = AMb1340C.AM371_EXTTRIGARM_RELEASE(_hSys, pin);
        }

        private void AM371_USERBWSEL(int pin, int drvCoarseBw, int drvBoostEn, int clmpCoarseBw, int clmpBoostEn)
        {
            _apiName = "AM371_USERBWSEL"; RetVal = AMb1340C.AM371_USERBWSEL(_hSys, pin, drvCoarseBw, drvBoostEn, clmpCoarseBw, clmpBoostEn);
        }
        private void AM371_READCURRENT10X(int pin, out float avalue)
        {
            _apiName = "AM371_READCURRENT10X"; RetVal = AMb1340C.AM371_READCURRENT10X(_hSys, pin, out avalue);
        }
        private void AM371_READCURRENT10XRATE(int pin, out float avalue)
        {
            _apiName = "AM371_READCURRENT10XRATE"; RetVal = AMb1340C.AM371_READCURRENT10XRATE(_hSys, pin, out avalue);
        }

        private void AM330_EXTTRIGARM_READSMUPIN(int measset, out int chdat, int trigMode, float delayAfterTrigS)
        {
            _apiName = "AM330_EXTTRIGARM_READSMUPIN"; RetVal = AMb1340C.AM330_EXTTRIGARM_READSMUPIN(_hSys, measset, out chdat, trigMode, delayAfterTrigS);
        }
        private int AM330_EXTTRIGARM_RETRIEVEREADSMUPIN(int measset, out int chdat, out int chRead)
        {
            _apiName = "AM330_EXTTRIGARM_RETRIEVEREADSMUPIN";
            return AMb1340C.AM330_EXTTRIGARM_RETRIEVEREADSMUPIN(_hSys, measset, out chdat, out chRead);
        }
        private void AM330_EXTTRIGARM_GETSTATUS(out int armedH, out int triggeredH, out int timeoutH)
        {
            _apiName = "AM330_EXTTRIGARM_GETSTATUS"; RetVal = AMb1340C.AM330_EXTTRIGARM_GETSTATUS(_hSys, out armedH, out triggeredH, out timeoutH);
        }
        private void AM330_EXTTRIGARM_RELEASE()
        {
            _apiName = "AM330_EXTTRIGARM_RELEASE"; RetVal = AMb1340C.AM330_EXTTRIGARM_RELEASE(_hSys);
        }
        private void AM330_EXTTRIGARM_SETTIMEOUTLIMIT(float timeoutS)
        {
            _apiName = "AM330_EXTTRIGARM_SETTIMEOUTLIMIT"; RetVal = AMb1340C.AM330_EXTTRIGARM_SETTIMEOUTLIMIT(_hSys, timeoutS);
        }

        private int RetVal
        {
            set
            {
                try
                {
                    _mRetVal = value;
                    if (_mRetVal != 0)
                        throw new Exception("AM1340c " + _apiName + " Error: " + String.Format("{0:x8}", _mRetVal).ToUpper());
                }
                catch (Exception ex)
                {
                    throw new System.Exception(ex.Message);
                }
                

            }
        }



   
    }

    abstract class AMb1340C
    {
        [DllImport("AMB1340C.dll", EntryPoint = "AM330_EXTTRIGARM_READSMUPIN")]
        public static extern int AM330_EXTTRIGARM_READSMUPIN(int hSys, int measset, out int chdat, int trigMode, float delayAfterTrigS);
        [DllImport("AMB1340C.dll", EntryPoint = "AM330_EXTTRIGARM_RETRIEVEREADSMUPIN")]
        public static extern int AM330_EXTTRIGARM_RETRIEVEREADSMUPIN(int hSys, int measset, out int chdat, out int chRead);
        [DllImport("AMB1340C.dll", EntryPoint = "AM330_EXTTRIGARM_GETSTATUS")]
        public static extern int AM330_EXTTRIGARM_GETSTATUS(int hSys, out int armedH, out int triggeredH, out int timeoutH);
        [DllImport("AMB1340C.dll", EntryPoint = "AM330_EXTTRIGARM_RELEASE")]
        public static extern int AM330_EXTTRIGARM_RELEASE(int hSys);
        [DllImport("AMB1340C.dll", EntryPoint = "AM330_EXTTRIGARM_SETTIMEOUTLIMIT")]
        public static extern int AM330_EXTTRIGARM_SETTIMEOUTLIMIT(int hSys, float timeoutS);

        [DllImport("AMB1340C.dll", EntryPoint = "INITIALIZE")]
        public static extern int INITIALIZE(int hSys);
        [DllImport("AMB1340C.dll", EntryPoint = "RESETBOARDS")]
        public static extern int RESETBOARDS();

        [DllImport("AMB1340C.dll", EntryPoint = "CREATESESSION")]
        public static extern int CREATESESSION(string hostname, out int session);
        [DllImport("AMB1340C.dll", EntryPoint = "CLOSESESSION")]
        public static extern int CLOSESESSION(int session);
        [DllImport("AMB1340C.dll", EntryPoint = "AMB1340C_CREATEINSTANCE")]
        public static extern int AMB1340C_CREATEINSTANCE(int session, int sysId, int offlinemode, out int hSys);
        [DllImport("AMB1340C.dll", EntryPoint = "AMB1340C_DELETEINSTANCE")]
        public static extern int AMB1340C_DELETEINSTANCE(int hSys);

        [DllImport("AMB1340C.dll", EntryPoint = "DRIVEPORT")]
        public static extern int DRIVEPORT(int hSys, int value);
        [DllImport("AMB1340C.dll", EntryPoint = "DRIVETHISPORT")]
        public static extern int DRIVETHISPORT(int hSys,int port, int value);
        [DllImport("AMB1340C.dll", EntryPoint = "DRIVEPIN")]
        public static extern int DRIVEPIN(int hSys, int pin, int value);
        [DllImport("AMB1340C.dll", EntryPoint = "READPORT")]
        public static extern int READPORT(int hSys, out int value);
        [DllImport("AMB1340C.dll", EntryPoint = "READPIN")]
        public static extern int READPIN(int hSys, int pin, out int value);
        [DllImport("AMB1340C.dll", EntryPoint = "SETPORTDIRECTION")]
        public static extern int SETPORTDIRECTION(int hSys, int value);
        [DllImport("AMB1340C.dll", EntryPoint = "SETPINDIRECTION")]
        public static extern int SETPINDIRECTION(int hSys, int pin, int value);
        [DllImport("AMB1340C.dll", EntryPoint = "GETPORTDIRECTION")]
        public static extern int GETPORTDIRECTION(int hSys, out int value);
        [DllImport("AMB1340C.dll", EntryPoint = "GETPINDIRECTION")]
        public static extern int GETPINDIRECTION(int hSys, int pin, out int value);

        [DllImport("AMB1340C.dll", EntryPoint = "DRIVEVOLTAGE")]
        public static extern int DRIVEVOLTAGE(int hSys, int pin, int mVvalue, int sign);
        [DllImport("AMB1340C.dll", EntryPoint = "DRIVECURRENT")]
        public static extern int DRIVECURRENT(int hSys, int pin, int nAvalue, int sign);
        [DllImport("AMB1340C.dll", EntryPoint = "CLAMPVOLTAGE")]
        public static extern int CLAMPVOLTAGE(int hSys, int pin, int mVvalue);
        [DllImport("AMB1340C.dll", EntryPoint = "CLAMPCURRENT")]
        public static extern int CLAMPCURRENT(int hSys, int pin, int nAvalue);

        [DllImport("AMB1340C.dll", EntryPoint = "READVOLTAGE")]
        public static extern int READVOLTAGE(int hSys, int pin, out int mVvalue);
        [DllImport("AMB1340C.dll", EntryPoint = "READCURRENT")]
        public static extern int READCURRENT(int hSys, int pin, out int nAvalue);
        [DllImport("AMB1340C.dll", EntryPoint = "READCURRENTABS")]
        public static extern int READCURRENTRATE(int hSys, int pin, out int nAvalue);
        [DllImport("AMB1340C.dll", EntryPoint = "READVOLTAGEVOLT")]
        public static extern int READVOLTAGEVOLT(int hSys, int pin, out float volt);
        [DllImport("AMB1340C.dll", EntryPoint = "READCURRENTAMP")]
        public static extern int READCURRENTAMP(int hSys, int pin, out float ampere);
        [DllImport("AMB1340C.dll", EntryPoint = "READCURRENTAMPABS")]
        public static extern int READCURRENTAMPRATE(int hSys, int pin, out float ampere);
        [DllImport("AMB1340C.dll", EntryPoint = "READVOLTAGEWITHAVERAGE")]
        public static extern int READVOLTAGEWITHAVERAGE(int hSys, int pin, int average, out int average_MV, out int every_MV);
        [DllImport("AMB1340C.dll", EntryPoint = "READCURRENTWITHAVERAGE")]
        public static extern int READCURRENTWITHAVERAGE(int hSys, int pin, int average, out int average_NA, out int every_NA);
        [DllImport("AMB1340C.dll", EntryPoint = "READCURRENTAUTORANGE")]
        public static extern int READCURRENTAUTORANGE(int hSys, int pin, out int nAvalue);
        [DllImport("AMB1340C.dll", EntryPoint = "READCURRENTFROMRANGE")]
        public static extern int READCURRENTFROMRANGE(int hSys, int channel, out int nAvalue);

        [DllImport("AMB1340C.dll", EntryPoint = "ONSMUPIN")]
        public static extern int ONSMUPIN(int hSys, int pin);
        [DllImport("AMB1340C.dll", EntryPoint = "OFFSMUPIN")]
        public static extern int OFFSMUPIN(int hSys, int pin);
        [DllImport("AMB1340C.dll", EntryPoint = "SETANAPINBANDWIDTH")]
        public static extern int SETANAPINBANDWIDTH(int hSys, int pin, int setting);
        [DllImport("AMB1340C.dll", EntryPoint = "SETINTEGRATION")]
        public static extern int SETINTEGRATION(int hSys, int chdat);
        [DllImport("AMB1340C.dll", EntryPoint = "SETINTEGRATIONPOWERCYCLES")]
        public static extern int SETINTEGRATIONPOWERCYCLES(int hSys, int setting, int powerCycles);
        [DllImport("AMB1340C.dll", EntryPoint = "SETNPLC")]
        public static extern int SETNPLC(int hSys, int pin, float nplc);    // 0.0009 ~ 60

        [DllImport("AMB1340C.dll", EntryPoint = "BIASSMUPIN")]
        public static extern int BIASSMUPIN(int hSys, int chset, out int chdat);
        [DllImport("AMB1340C.dll", EntryPoint = "READSMUPIN")]
        public static extern int READSMUPIN(int hSys, int chset, out int chdat, out int chRead);
        [DllImport("AMB1340C.dll", EntryPoint = "READSMUPINABS")]
        public static extern int READSMUPINRATE(int hSys, int chset, out int chdat, out int chRead);
        [DllImport("AMB1340C.dll", EntryPoint = "ONOFFSMUPIN")]
        public static extern int ONOFFSMUPIN(int hSys, int chset, out int chdat);

        [DllImport("AMB1340C.dll", EntryPoint = "ARMREADSMUPIN")]
        public static extern int ARMREADSMUPIN(int hSys, int measset, out int chdat);
        [DllImport("AMB1340C.dll", EntryPoint = "RETRIEVEREADSMUPIN")]
        public static extern int RETRIEVEREADSMUPIN(int hSys, int measset, out int chdat, out int chRead);


        [DllImport("AMB1340C.dll", EntryPoint = "SOURCEDELAYMEASURESMUPIN")]
        public static extern int SOURCEDELAYMEASURESMUPIN(int hSys, int chset, out int chdat, out int chRead, int sequence);
        [DllImport("AMB1340C.dll", EntryPoint = "AMB1340C_SOURCEDELAYMEASURESMUPIN")]
        public static extern int AMB1340C_SOURCEDELAYMEASURESMUPIN(int hSys, int pinset, out float pindat, out int measset, out float pinRead, int sequence);

        [DllImport("AMB1340C.dll", EntryPoint = "AM330_DRIVEPULSEVOLTAGE")]
        public static extern int AM330_DRIVEPULSEVOLTAGE(int hSys, int pin, float _base, float pulse, float pulseS, float holdS, int drVrange, int cycles, int measCh, int measSel, int measVrange, int trigPercentage, int armExtTriginH, float timeoutS);

        [DllImport("AMB1340C.dll", EntryPoint = "AM371_DRIVEVOLTAGE")]
        public static extern int AM371_DRIVEVOLTAGE(int hSys, int pin, float volt);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_DRIVEVOLTAGESETVRANGE")]
        public static extern int AM371_DRIVEVOLTAGESETVRANGE(int hSys, int pin, float volt, int vrange);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_DRIVECURRENT")]
        public static extern int AM371_DRIVECURRENT(int hSys, int pin, float ampere);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_DRIVECURRENTSETIRANGE")]
        public static extern int AM371_DRIVECURRENTSETIRANGE(int hSys, int pin, float ampere, int irange);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_CLAMPVOLTAGE")]
        public static extern int AM371_CLAMPVOLTAGE(int hSys, int pin, float volt);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_CLAMPVOLTAGESETVRANGE")]
        public static extern int AM371_CLAMPVOLTAGESETVRANGE(int hSys, int pin, float volt, int vrange);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_CLAMPCURRENT")]
        public static extern int AM371_CLAMPCURRENT(int hSys, int pin, float ampere);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_CLAMPCURRENTSETIRANGE")]
        public static extern int AM371_CLAMPCURRENTSETIRANGE(int hSys, int pin, float ampere, int irange);

        [DllImport("AMB1340C.dll", EntryPoint = "AM371_READVOLTAGE")]
        public static extern int AM371_READVOLTAGE(int hSys, int pin, out float volt);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_READVOLTAGEGETVRANGE")]
        public static extern int AM371_READVOLTAGEGETVRANGE(int hSys, int pin, out float volt, out int vrange);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_READCURRENT")]
        public static extern int AM371_READCURRENT(int hSys, int pin, out float ampere);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_READCURRENTABS")]
        public static extern int AM371_READCURRENTRATE(int hSys, int pin, out float ampere);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_READCURRENTGETIRANGE")]
        public static extern int AM371_READCURRENTGETIRANGE(int hSys, int pin, out float ampere, out int irange);

        [DllImport("AMB1340C.dll", EntryPoint = "AM371_ONSMUPIN")]
        public static extern int AM371_ONSMUPIN(int hSys, int pin, int remoteSenseH);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_OFFSMUPIN")]
        public static extern int AM371_OFFSMUPIN(int hSys, int pin);

        [DllImport("AMB1340C.dll", EntryPoint = "AM371_EXTTRIGARM_READCURRENTARRAY")]
        public static extern int AM371_EXTTRIGARM_READCURRENTARRAY(int hSys, int pin, int posedgeH, float delayS, int nsample, float sampleDelayS);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_EXTTRIGGET_READCURRENTARRAY_WITH_MINMAX")]
        public static extern int AM371_EXTTRIGGET_READCURRENTARRAY_WITH_MINMAX(int hSys, int pin, out int nsample, [MarshalAs(UnmanagedType.LPArray)] float[] iarray, out float min, out float max, out float average);

        [DllImport("AMB1340C.dll", EntryPoint = "AM371_EXTTRIGARM_RELEASE")]
        public static extern int AM371_EXTTRIGARM_RELEASE(int hSys, int pin);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_USERBWSEL")]
        public static extern int AM371_USERBWSEL(int hSys, int pin, int drvCoarseBw, int drvBoostEn, int clmpCoarseBw, int clmpBoostEn);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_READCURRENT10X")]
        public static extern int AM371_READCURRENT10X(int hSys, int pin, out float avalue);
        [DllImport("AMB1340C.dll", EntryPoint = "AM371_READCURRENT10XABS")]
        public static extern int AM371_READCURRENT10XRATE(int hSys, int pin, out float avalue);

        [DllImport("AMB1340C.dll", EntryPoint = "WLF_SETVOLTAGELEVEL")]
        public static extern int WLF_SETVOLTAGELEVEL(int hSys, int switchNum, int setting);
        [DllImport("AMB1340C.dll", EntryPoint = "WLF_DRIVESINGLESWITCH")]
        public static extern int WLF_DRIVESINGLESWITCH(int hSys, int switchNum, int val);
        [DllImport("AMB1340C.dll", EntryPoint = "WLF_DRIVEALLSWITCH")]
        public static extern int WLF_DRIVEALLSWITCH(int hSys, int val);

    }
}
