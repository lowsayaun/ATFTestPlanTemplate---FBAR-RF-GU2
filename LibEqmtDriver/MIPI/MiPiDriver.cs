using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aemulus.Hardware.DM280e;

namespace LibEqmtDriver.MIPI
{
    public class LibVar
    {
        //MIPI
        public static bool ºMipiEnable { get; set; }
        public static string ºMipiReg0 { get; set; }
        public static string ºMipiReg1 { get; set; }
        public static string ºMipiReg2 { get; set; }
        public static string ºMipiReg3 { get; set; }
        public static string ºMipiReg4 { get; set; }
        public static string ºMipiReg5 { get; set; }
        public static string ºMipiReg6 { get; set; }
        public static string ºMipiReg7 { get; set; }
        public static string ºMipiReg8 { get; set; }
        public static string ºMipiReg9 { get; set; }
        public static string ºMipiRegA { get; set; }
        public static string ºMipiRegB { get; set; }
        public static string ºMipiRegC { get; set; }
        public static string ºMipiRegD { get; set; }
        public static string ºMipiRegE { get; set; }
        public static string ºMipiRegF { get; set; }

        public static int ºSlaveAddress { get; set; }
        public static int ºChannelUsed { get; set; }
        public static int ºPmTrig { get; set; }
        public static int ºPmTrigData { get; set; }
        public static int ºRegData { get; set; }
        public static bool ºReadSuccessful { get; set; }
        public static int ºDm280Ch0 { get; set; }
        public static int ºDm280Ch1 { get; set; }
        public static bool ºTestSwMarker { get; set; }
        public static bool ºOrfs { get; set; }
        public static bool ºReadFunction { get; set; }

        public static int ºDm482Ch0 { get; set; }
        public static int ºDm482Ch1 { get; set; }

        // DM280 MIPI
        public static string ºmyDm280Address { get; set; }
        public static AemulusDm280E ºmyDm280 { get; set; }
        public static int ºChnVio { get; set; }

        // DM482 MIPI
        public static string ºmyDm482Address { get; set; }
        public static AemulusDm482E ºmyDm482E { get; set; }
        public static string ºHwProfile { get; set; }

        public class TestResult
        {
            public static double[] IccArry, PoutArry, IccOnArry, IccOffArry;
            public static float
             LeakageVio, LeakageSdata, LeakageSclk, Ich1, Ich2, Ich3, Ich4, Isum,
             Pout, Gain, Pin, Pae, Aclr1L, Aclr1U, Aclr2L, Aclr2U, Aclr3L, Aclr3U,
             Evm, Coup, H2, H3, Ns, TxLeakage, TxLeakage2, TxLeakage3, MipiValue, MaxGain,
             GainP1dB, PoutP1dB, PinP1dB, IccP1dB, PaeP1dB,
             GainP2dB, PoutP2dB, PinP2dB, IccP2dB, PaeP2dB,
             GainP3dB, PoutP3dB, PinP3dB, IccP3dB, PaeP3dB,
             RfOnTime, DcOnTime, DcOffTime, FBurstTime, TempSensor;
            public static bool
                MipiBool;
            public static int
                VarLoop, IndexP1Db, IndexP2Db, IndexP3Db,
                MipiNumBitErrors, MipiOtpResult, MipiReg08Result, MipiReg09Result, MipiReg0FResult, MipiRegE3Result,
                MipiMid, MipiPid, MipiUsid, OtpStatReg08, OtpStatReg09, OtpStatReg0F, OtpStatRegE3;

        }
        public class TestParameter
        {
            public static bool
                 TestPout, TestPin, TestGain, TestCf, TestIch1, TestIch2, TestIch3, TestIch4, TestIsum, TestPae, TestAcp, TestNs07, TestNs12, TestNs15,
                TestTxleakage, TestTxleakage2, TestTxleakage3, TestH2, TestH3, TestEvm, TestMipiVio, TestMipiSclk, TestMipiSdata, TestP1dB, TestP2dB, TestP3dB, OtpQa;

        }
    }

    public interface IIMiPiCtrl
    {
        void Init();
        void SendAndReadMipiCodes(out bool readSuccessful, int mipiReg);
        void TurnOff_VIO(int miPiPair);
        void TurnOn_VIO(int miPiPair);
    }
}
