using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEqmtDriver.SA
{
    public enum N9020AInstrumentMode
    {
        SpectrumAnalyzer,
        Basic,
        Wcdma,
        Wimax,
        EdgeGsm,
    }
    public enum N9020AMeasType
    {
        SweptSa,
        ChanPower,
        Acp,
        BTxPow,
        GPowVtm,
        GpHaseFreq,
        GOutRfSpec,
        GTxSpur,
        EPowVtm,
        Eevm,
        EOutRfSpec,
        ETxSpur,
        MonitorSpec,
    }
    public enum N9020ATriggeringType
    {
        RfFreeRun,
        RfExt1,
        RfExt2,
        RfRfBurst,
        RfVideo,
        TxpFreeRun,
        TxpExt1,
        TxpExt2,
        TxpRfBurst,
        TxpVideo,
        PvtFreeRun,
        PvtExt1,
        PvtExt2,
        PvtRfBurst,
        PvtVideo,
        EpvtFreeRun,
        EpvtExt1,
        EpvtExt2,
        EpvtRfBurst,
        EpvtVideo,
        EorfsFreeRun,
        EorfsExt1,
        EorfsExt2,
        EorfsRfBurst,
        EorfsVideo,
        EevmFreeRun,
        EevmExt1,
        EevmExt2,
        EevmRfBurst,
        EevmVideo,
    }
    public enum N9020ARadStd
    {
        None,
        Is95A,
        Jstd,
        Is97D,
        Gsm,
        W3Gpp,
        C2000Mc1,
        C20001X,
        Nadc,
        Pdc,
        BluEtooth,
        TetRa,
        Wl802Dot11A,
        Wl802Dot11B,
        Wl802Dot11G,
        Hiperlan2,
        Dvbtlsn,
        Dvbtgpn,
        Dvbtipn,
        Fcc15,
        Sdmbse,
        Uwbindoor,
    }
    public enum N9020ARadioStdBand
    {
        Egsm,
        Pgsm,
        Rgsm,
        Dcs1800,
        Pcs1900,
        Gsm450,
        Gsm480,
        Gsm700,
        Gsm850,
    }
    public enum N9020AAcpMethod
    {
        Ibw,
        IbwRange,
        Fast,
        Rbw,
    }
    public enum N9020AFrequencyListMode
    {
        Standard,
        Short,
        Custom,
    }
    public enum N9020ATraceAveType
    {
        Aver,
        Neg,
        Norm,
        Pos,
        Samp,
        Qpe,
        Eav,
        Epos,
        Mpos,
    }
    public enum N9020AAveType
    {
        Rms,
        Logarithm,
        Scalar,
        Txprms,
        Txplogarithm,
        Pvtrms,
        Pvtlogarithm,
        Epvtrms,
        Epvtlogrithm,
        Eorfrms,
        Eorflogrithm,
    }
    public enum N9020AGsmAverage
    {
        Txp,
        Pvt,
        Epvt,
        Eevm,
        Eorf
    }
    public enum N9020AAutoCouple
    {
        All,
        None,
    }
    public enum N9020AStandardDevice
    {
        /// <summary>
        /// Base station transmitter
        /// </summary>
        Bts,
        /// <summary>
        /// Mobile station transmitter
        /// </summary>
        Ms,
    }
    public enum N9020ADisplay
    {
        On,
        Off,
    }
    public enum N9020AMarkerFunc
    {
        Off,
        Bandpow,
        Banddensity,
        Noise
    }
}
