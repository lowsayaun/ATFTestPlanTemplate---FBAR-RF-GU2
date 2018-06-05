using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa.Interop;
using System.Threading;
using System.Windows.Forms;

namespace LibEqmtDriver.SG
{
    public class N5182A : IISiggen
    {
        public static string ClassName = "N5182A Siggen Class";
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
        public N5182A(string ioAddress)
        {
            Address = ioAddress;
            OpenIo();
        }
        ~N5182A() { }
        
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
                throw new Exception("EquipN5182A: Initialization -> " + ex.Message);
            }

        }
        void IISiggen.Reset()
        {
            try
            {
                _myVisaSg.WriteString("SYSTEM:PRESET", true);
                Thread.Sleep(1000);
                _myVisaSg.WriteString("*CLS; *RST", true);
            }
            catch (Exception ex)
            {
                throw new Exception("EquipN5182A: RESET -> " + ex.Message);
            }
        }
        void IISiggen.EnableRf(InstrOutput onoff)
        {
            _myVisaSg.WriteString("OUTP:STATE " + onoff, true);

        }
        void IISiggen.EnableModulation(InstrOutput onoff)
        {
            _myVisaSg.WriteString("OUTP:MOD " + onoff, true);
        }
        void IISiggen.SetAmplitude(float dBm)
        {
            _myVisaSg.WriteString(":POW " + dBm.ToString(), true);
        }
        void IISiggen.SetFreq(double mHz)
        {
            _myVisaSg.WriteString("FREQ " + mHz.ToString() + "MHz", true);
        }
        void IISiggen.SetPowerMode(N5182PowerMode mode)
        {
            _myVisaSg.WriteString(":SOUR:POW:MODE " + mode.ToString(), true);
        }
        void IISiggen.SetFreqMode(N5182FrequencyMode mode)
        {
            _myVisaSg.WriteString(":SOUR:FREQ:MODE " + mode.ToString(), true);
        }
        void IISiggen.MOD_FORMAT_WITH_LOADING_CHECK(string strWaveform, string strWaveformName, bool waveformInitalLoad)
        {
            while (true)
            {
                if (waveformInitalLoad)
                {
                    _myVisaSg.WriteString(":MEM:COPY \"" + strWaveformName + "@NVWFM\",\"" + strWaveformName + "@WFM1\"", true);
                }

                _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:" + strWaveformName + "\"", true);
                _myVisaSg.WriteString(":RAD:ARB ON;:OUTP:MOD ON", true);                  
                break;
            }
        }
        void IISiggen.SELECT_WAVEFORM(N5182AWaveformMode mode)
        {
            switch (mode)
            {
                case N5182AWaveformMode.Cdma2K:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:CDMA2K-WFM1\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Cdma2KRc1:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:CDMA2K_RC1_20100316\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm850:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK850\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm900:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK900\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm1800:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK1800\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm1900:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK1900\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm850A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GSM850A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm900A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK900A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm1800A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK1800A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gsm1900A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK1900A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Hsdpa:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:HSDPA_UL\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.HsupaTc3:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_UPLINK_HSUPA_TC3\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.HsupaSt2:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_HSUPA_ST2\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.HsupaSt3:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_HSUPA_ST3\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.HsupaSt4:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_HSUPA_ST4\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Is95A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:IS95A_20100608\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Is95AReWfm1:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:IS95A_RE-WFM1\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Is98:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:IS98_WFM\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Wcdma:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_1DPCH_WFM\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.WcdmaUl:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_UL\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.WcdmaGtc1:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_GTC1_20100208A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.WcdmaGtc3:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_GTC3_20100726A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.WcdmaGtc4:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_UPLINK_GTC4\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.WcdmaGtc1New:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:WCDMA_GTC1_NEW_20101111\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge850:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE850\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge900:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE900\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge1800:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE1800\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge1900:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE1900\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge850A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE850A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge900A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE900A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge1800A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE1800A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge1900A:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE1900A\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;

                case N5182AWaveformMode.Ltetd5M8Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTETU_QPSK_5M8RB\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Ltetd5M8Rb17S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTETU_QPSK_5M8RB17S\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Ltetd10M12Rb38S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTETU_QPSK_10M12RB38S\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Ltetd10M12Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTETU_QPSK_10M12RB\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Ltetd10M1Rb49S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTUQ_10M1R49S\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;

                case N5182AWaveformMode.Lte5M8Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_5M8RB_20091202\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M8Rb17S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_QPSK_5M8RB17S\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M25Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_5M25RB_091215\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M1Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_QPSK_10M1RB\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M1Rb49S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_QPSK10M1RB49S\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M12Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_10M12RB_091215\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M12Rb19S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK10M12RB19S_1220\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M12Rb38S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_10M12RB38S\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M48Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_10M48RB_091215\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M50Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_10M50RB_091215\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M20Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_QPSK_10M20RB\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte15M75Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_15M75RB_091215\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte15M18Rb57S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK15M18RB57S_1025\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte20M100Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_20M100RB091215\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte20M18Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_20M18RB_100408\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte20M48Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_QPSK_20M48RB_091215\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M12RbMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_10M12RB_ST0_MCS6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte10M12Rb38SMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_10M12RB_ST38_M6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M25RbMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_5M25RB_ST0_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M8Rb17SMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_5M8RB_ST17_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M8Rb17SMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_5M8RB_ST17_MCS6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte16Qam5M8Rb17S:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_16QAM_5M8RB17S\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M25RbMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_5M25RB_ST0_MCS6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M1Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_QPSK_5M1RB\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;

                case N5182AWaveformMode.Lte10M50RbMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_10M50RB_ST0_MCS6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte20M18RbMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_20M18RB0S_MCS6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte20M18Rb82SMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_20M18RB82S_MCS6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte20M100RbMcs2:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"LTEFUQ_20M100RB0S_MCS2\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte15M16RbMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_15M16RB0S_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte15M16Rb59SMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFUQ_15M16RB59S_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte15M75RbMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"LTEFUQ_15M75RB0S_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M8RbMcs6:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"LTEFUQ_5M8RB_ST0_MCS6\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte5M8RbMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"LTEFUQ_5M8RB_ST0_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;

                case N5182AWaveformMode.Lte1P4M5RbMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_1P4M5RB_ST0_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte1P4M5Rb1SMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_1P4M5RB_ST1_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte3M4RbMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_3M4RB_ST0_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte3M4Rb11SMcs5:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTEFU_3M4RB_ST11_MCS5\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;

                case N5182AWaveformMode.Lte16Qam5M25Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:LTE_16QAM_5M25RB\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte16Qam10M50Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"LTE_16QAM_10M50RB_0213\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte16Qam15M75Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"LTE_16QAM_15M75RB_0213\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Lte16Qam5M8Rb:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"LTE_16QAM_5M8RB\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;

                case N5182AWaveformMode.Gmsk900:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK900\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gmsk800:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK800\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gmsk850:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK850\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Edge800:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE800\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gmsk1700:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK1700\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Gmsk1900:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GMSK1900\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.GmskTs01:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:GSM_TIMESLOT01_20100107\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.EdgeTs01:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:EDGE_TS01_20100107\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Evdo4096:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:1XEVDO_REVA_TR4096_0816\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.EvdoB:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:1XEVDO_REVB_5MHZSEP_001\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;

                case N5182AWaveformMode.TdscdmaTs1:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:TDSCDMA_TS1_1P28MHZ\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Drep:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:DREP\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.Pulse:
                    _myVisaSg.WriteString(":RAD:ARB:WAV \"WFM1:PULSE\"", true);
                    _myVisaSg.WriteString(":OUTP:MOD ON", true);
                    break;
                case N5182AWaveformMode.None:
                    _myVisaSg.WriteString(":OUTP:MOD OFF", true);
                    break;
                case N5182AWaveformMode.Cw:
                    _myVisaSg.WriteString(":OUTP:MOD OFF", true);
                    break;
                default: throw new Exception("Not such a waveform!");
            }
        }
        void IISiggen.SET_LIST_TYPE(N5182ListType mode)
        {
            _myVisaSg.WriteString(":LIST:TYPE " + mode.ToString(), true);
        }
        void IISiggen.SET_LIST_MODE(InstrMode mode)
        {
            _myVisaSg.WriteString(":LIST:MODE " + mode.ToString(), true);
        }
        void IISiggen.SET_LIST_TRIG_SOURCE(N5182TrigType mode)
        {
            _myVisaSg.WriteString(":LIST:TRIG:SOUR " + mode.ToString(), true);
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
            _myVisaSg.WriteString("TRIG:SEQ:TIM " + ms.ToString() + "ms", true);
        }
        void IISiggen.SET_SWEEP_POINT(int points)
        {
            _myVisaSg.WriteString("SWE:POIN " + points.ToString(), true);
        }
        void IISiggen.SINGLE_SWEEP()
        {
            _myVisaSg.WriteString("SOUR:TSWEEP", true);
        }
        void IISiggen.SET_SWEEP_PARAM(int points, double ms, double startFreqMHz, double stopFreqMHz)
        {
            _myVisaSg.WriteString("FREQ:START " + startFreqMHz.ToString() + "MHz", true);
            _myVisaSg.WriteString("FREQ:STOP " + stopFreqMHz.ToString() + "MHz", true);
            _myVisaSg.WriteString("SWE:POIN " + points.ToString(), true);
            _myVisaSg.WriteString("TRIG:SEQ:TIM " + ms.ToString() + "ms", true);
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
                throw new Exception("N5182A: OPERATION_COMPLETE -> " + ex.Message);
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
                throw new Exception("EquipN5182A: QUERY_ERROR --> " + ex.Message);
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
