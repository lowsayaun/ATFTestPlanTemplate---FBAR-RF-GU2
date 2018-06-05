using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEqmtDriver.SG
{
    public enum N5182ListType
    {
        //set sweep type either LIST or STEP mode
        Step,
        List
    }
    public enum N5182TrigType
    {
        //immediate triggering or Free Run
        Imm,
        //This choice enables triggering through front panel interaction by pressing the Trigger hardkey
        Key,
        //This choice enables GPIB triggering using the *TRG or GET command
        Bus,
        //This choice enables the triggering of a sweep event by an externally applied signal at the TRIG IN connector
        Ext,
        //This choice enables the sweep trigger timer
        Tim
    }
    public enum N5182PowerMode
    {
        /// <summary>
        /// This choice stops a power sweep, allowing the signal generator to operate at a
        /// fixed power level.
        /// </summary>
        Fix,
        /// <summary>
        /// This choice selects the swept power mode. If sweep triggering is set to immediate
        /// along with continuous sweep mode, executing the command starts the LIST or
        /// STEP power sweep.
        /// </summary>
        List,
        /// <summary>
        /// setting the frequency in the SWEEP mode
        /// </summary>
        Sweep,
    }
    public enum N5182FrequencyMode
    {
        /// <summary>
        /// setting the frequency in the FIXed mode
        /// </summary>
        Fix,
        /// <summary>
        /// This choice selects the swept frequency mode. If sweep triggering is set to
        /// immediate along with continuous sweep mode, executing the command starts the
        /// LIST or STEP frequency sweep.
        /// </summary>
        List,
        /// <summary>
        /// setting the frequency in the CW mode
        /// </summary>
        Cw,
        /// <summary>
        /// setting the frequency in the SWEEP mode
        /// </summary>
        Sweep,
    }
    public enum InstrOutput
    {
        /// <summary>
        /// On Instrument Output
        /// </summary>
        On,
        /// <summary>
        /// Off Intrument Output
        /// </summary>
        Off
    }

    public enum InstrMode
    {
        Auto,   // Sets mode for the list sweep - either MANUAL or AUTO
        Man
    }
    public enum N5182AWaveformMode
    {
        None,
        Cw,
        Pulse,
        Drep,
        TdscdmaTs1,
        Cdma2K,
        Cdma2KRc1,
        Gsm850,
        Gsm900,
        Gsm1800,
        Gsm1900,
        Gsm850A,
        Gsm900A,
        Gsm1800A,
        Gsm1900A,
        Hsdpa,
        HsupaTc3,
        HsupaSt2,
        HsupaSt3,
        HsupaSt4,
        Is95A,
        Is95AReWfm1,
        Is98,
        Wcdma,
        WcdmaUl,
        WcdmaGtc1,
        WcdmaGtc3,
        WcdmaGtc4,
        WcdmaGtc1New,
        Edge900,
        Edge1800,
        Edge1900,
        Edge850A,
        Edge900A,
        Edge1800A,
        Edge1900A,
        Ltetd5M8Rb,
        Ltetd5M8Rb17S,
        Ltetd10M12Rb,
        Ltetd10M1Rb49S,
        Ltetd10M12Rb38S,
        Ltetd10M50Rb,
        Lte10M1Rb,
        Lte10M1Rb49S,
        Lte10M12Rb19S,
        Lte10M12Rb,
        Lte10M12Rb38S,
        Lte10M20Rb,
        Lte10M50Rb,
        Lte20M18Rb82SMcs6,
        Lte10M48Rb,
        Lte15M18Rb57S,
        Lte15M75Rb,
        Lte5M25Rb,
        Lte5M8Rb,
        Lte5M8Rb17S,
        Lte5M25Rb38S,
        Lte20M100Rb,
        Lte20M18Rb,
        Lte20M48Rb,
        Lte10M12RbMcs6,
        Lte10M12Rb38SMcs6,
        Lte5M25RbMcs5,
        Lte5M25RbMcs6,
        Lte5M8Rb17SMcs6,
        Lte16Qam5M8Rb17S,
        Lte5M1Rb,
        Lte1P4M5RbMcs5,
        Lte1P4M5Rb1SMcs5,
        Lte3M4RbMcs5,
        Lte3M4Rb11SMcs5,
        Lte5M8RbMcs5,
        Lte5M8RbMcs6,
        Lte5M8Rb17SMcs5,
        Lte10M50RbMcs6,
        Lte15M16RbMcs5,
        Lte15M16Rb59SMcs5,
        Lte15M75RbMcs5,
        Lte16Qam10M50Rb,
        Lte16Qam15M75Rb,
        Lte16Qam5M8Rb,
        Lte20M18RbMcs6,
        Lte20M100RbMcs2,
        Lte16Qam5M25Rb,
        HspaplusMpr0,
        HspaplusMpr2,
        Gmsk900,
        Gmsk800,
        Gmsk850,
        Edge800,
        Edge850,
        Gmsk1700,
        Gmsk1900,
        GmskTs01,
        EdgeTs01,
        Evdo4096,
        EvdoB
    }
}
