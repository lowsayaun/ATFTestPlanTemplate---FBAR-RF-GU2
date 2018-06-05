using System;
using System.Runtime.InteropServices;

namespace InstrumentDrivers
{
    public class Rsnrpz : object, System.IDisposable
    {
        private const string NrpDll = "rsnrpz_64.dll";
        private System.IntPtr _handle;

        private bool _disposed = true;

        ~Rsnrpz() { Dispose(false); }


        /// <summary>
        /// This function performs the following initialization actions and assigned specified sesnor to channel 1:
        /// 
        /// - Opens a session to the specified device using the interface and address specified in the Resource Name control.
        /// 
        /// - Performs an identification query on the sensor.
        /// 
        /// - Resets the instrument to a known state.
        /// 
        /// - Sends initialization commands to the sensor
        /// 
        /// - Returns an Instrument Handle which is used to differentiate between different sessions of this instrument driver.
        /// 
        /// Note:
        /// 
        /// 1) Do not initilize every sensor with rsnrpz_init function. if you want comunicate with more sensor use rsnrpz_AddSensor for adding a new channel. The reason is the rsnrpz_close destroys all sensor sessions assigned to process.
        /// </summary>
        /// <param name="Resource_Name">
        /// This control specifies the interface and address of the device that is to be initialized (Instrument Descriptor). The exact grammar to be used in this control is shown in the note below. 
        /// 
        /// Default Value:  "USB::0x0aad::0x000b::100000"
        /// 
        /// Notes:
        /// 
        /// (1) Based on the Instrument Descriptor, this operation establishes a communication session with a device.  The grammar for the Instrument Descriptor is shown below.  Optional parameters are shown in square brackets ([]).
        /// 
        /// Interface   Grammar
        /// ------------------------------------------------------
        /// USB         USB::vendor Id::product Id::serial number
        /// 
        /// where:
        ///   vendor Id is 0aad for all Rohde&amp;Schwarz instruments.
        /// 
        ///   product Id depends on your sensor model:
        ///              NRP-Z21 : 0x0003
        ///              NRP-FU  : 0x0004
        ///              FSH-Z1  : 0x000b
        ///              NRP-Z11 : 0x000c
        ///              NRP-Z22 : 0x0013
        ///              NRP-Z23 : 0x0014
        ///              NRP-Z24 : 0x0015
        ///              NRP-Z51 : 0x0016
        ///              NRP-Z52 : 0x0017
        ///              NRP-Z55 : 0x0018
        ///              FSH-Z18 : 0x001a
        ///              NRP-Z91 : 0x0021
        ///              NRP-Z81 : 0x0023
        ///              NRP-Z37 : 0x002d
        ///              NRP-Z27 : 0x002f
        /// 
        ///   serianl number you can find on your sensor. Serial number is number with 6 digits. For exampel 9000003
        /// 
        ///  you can use star '*' for product id or serial number in resource descriptor. Use star only for one connected sensor, because driver opens only first sensor on the bus.
        ///  
        /// The USB keyword is used for USB interface.
        /// 
        /// Examples:
        /// USB   - "USB::0x0aad::0x000b::100000"
        /// USB   - "USB::0x0aad::0x000b::*" - Opens first FSH-Z1 sensor
        /// USB   - "USB::0x0aad::*"         - Opens first R&amp;S sensor
        /// USB   - "*"                      - Opens first R&amp;S sensor
        /// </param>
        /// <param name="ID_Query">
        /// This control specifies if an ID Query is sent to the instrument during the initialization procedure.
        /// 
        /// Valid Range:
        /// VI_FALSE (0) - Skip Query
        /// VI_TRUE  (1) - Do Query (Default Value)
        /// 
        /// Notes:
        ///    
        /// (1) Under normal circumstances the ID Query ensures that the instrument initialized is the type supported by this driver. However circumstances may arise where it is undesirable to send an ID Query to the instrument.  In those cases; set this control to "Skip Query" and this function will initialize the selected interface, without doing an ID Query.
        /// 
        /// </param>
        /// <param name="Reset_Device">
        /// This control specifies if the instrument is to be reset to its power-on settings during the initialization procedure.
        /// 
        /// Valid Range:
        /// VI_FALSE (0) - Don't Reset
        /// VI_TRUE  (1) - Reset Device (Default Value)
        /// 
        /// Notes:
        /// 
        /// (1) If you do not want the instrument reset. Set this control to "Don't Reset" while initializing the instrument.
        /// 
        /// </param>
        /// <param name="Instrument_Handle">
        /// This control returns an Instrument Handle that is used in all subsequent function calls to differentiate between different sessions of this instrument driver.
        /// 
        /// Notes:
        /// 
        /// (1) Each time this function is invoked a Unique Session is opened.  It is possible to have more than one session open for the same resource.
        /// 
        /// </param>
        public Rsnrpz()
        {

        }
        public void Init(string resourceName, bool idQuery, bool resetDevice)
        {
            try
            {
                int pInvokeResult = PInvoke.init(resourceName, System.Convert.ToUInt16(idQuery), System.Convert.ToUInt16(resetDevice), out this._handle);
                PInvoke.TestForError(this._handle, pInvokeResult);
                this._disposed = false;
            }
            catch (Exception e)
            {
                // System.Windows.Forms.MessageBox.Show(string.Format("Power Sensor Initialize failed. ID:{0}", Resource_Name));
                throw e;
            }
        }

        /// <summary>
        /// This function performs the following initialization actions and adds sensor to channel list:
        /// 
        /// - Opens a session to the specified device using the interface and address specified in the Resource Name control.
        /// 
        /// - Performs an identification query on the Instrument.
        /// 
        /// - Resets the instrument to a known state.
        /// 
        /// - Sends initialization commands to the sensor
        /// 
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 2 to 31
        /// 
        /// Default Value: 2
        /// 
        /// Note:
        /// 
        /// 1) function rsnrpz_init adds assigne sensor to channel 1 automatically.
        /// </param>
        /// <param name="resourceName">
        /// This control specifies the interface and address of the device that is to be initialized (Instrument Descriptor). The exact grammar to be used in this control is shown in the note below. 
        /// 
        /// Default Value:  "USB::0x0aad::0x000b::100000"
        /// 
        /// Notes:
        /// 
        /// (1) Based on the Instrument Descriptor, this operation establishes a communication session with a device.  The grammar for the Instrument Descriptor is shown below.  Optional parameters are shown in square brackets ([]).
        /// 
        /// Interface   Grammar
        /// ------------------------------------------------------
        /// USB         USB::vendor Id::product Id::serial number
        ///             
        /// where:
        ///   vendor Id is 0aad for all Rohde&amp;Schwarz instruments.
        /// 
        ///   product Id depends on your sensor model:
        ///              NRP-Z21 : 0x0003
        ///              NRP-FU  : 0x0004
        ///              FSH-Z1  : 0x000b
        ///              NRP-Z11 : 0x000c
        ///              NRP-Z22 : 0x0013
        ///              NRP-Z23 : 0x0014
        ///              NRP-Z24 : 0x0015
        ///              NRP-Z51 : 0x0016
        ///              NRP-Z52 : 0x0017
        ///              NRP-Z55 : 0x0018
        ///              FSH-Z18 : 0x001a
        ///              NRP-Z91 : 0x0021
        ///              NRP-Z81 : 0x0023
        ///              NRP-Z37 : 0x002d
        ///              NRP-Z27 : 0x002f
        /// 
        ///   serianl number you can find on your sensor. Serial number is number with 6 digits. For exampel 9000003
        /// 
        /// The USB keyword is used for USB interface.
        /// 
        /// Examples:
        /// USB   - "USB::0x0aad::0x000b::100000"
        /// </param>
        /// <param name="idQuery">
        /// This control specifies if an ID Query is sent to the instrument during the initialization procedure.
        /// 
        /// Valid Range:
        /// VI_FALSE (0) - Skip Query
        /// VI_TRUE  (1) - Do Query (Default Value)
        /// 
        /// Notes:
        ///    
        /// (1) Under normal circumstances the ID Query ensures that the instrument initialized is the type supported by this driver. However circumstances may arise where it is undesirable to send an ID Query to the instrument.  In those cases; set this control to "Skip Query" and this function will initialize the selected interface, without doing an ID Query.
        /// 
        /// </param>
        /// <param name="resetDevice">
        /// This control specifies if the instrument is to be reset to its power-on settings during the initialization procedure.
        /// 
        /// Valid Range:
        /// VI_FALSE (0) - Don't Reset
        /// VI_TRUE  (1) - Reset Device (Default Value)
        /// 
        /// Notes:
        /// 
        /// (1) If you do not want the instrument reset. Set this control to "Don't Reset" while initializing the instrument.
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This driver defines the following status codes:
        ///           
        /// Status    Description
        /// -------------------------------------------------
        /// BFFC0002  Parameter 2 (ID Query) out of range.
        /// BFFC0003  Parameter 3 (Reset Device) out of range.
        /// BFFC0011  Instrument returned invalid response to ID Query
        ///           
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int AddSensor(int channel, string resourceName, bool idQuery, bool resetDevice)
        {
            int pInvokeResult = PInvoke.AddSensor(this._handle, channel, resourceName, System.Convert.ToUInt16(idQuery), System.Convert.ToUInt16(resetDevice));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function immediately sets all the sensors to the IDLE state. Measurements in progress are interrupted. If INIT:CONT ON is set, a new measurement is immediately started since the trigger system is not influenced.
        /// 
        /// Remote-control command(s):
        /// ABOR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chans_abort()
        {
            int pInvokeResult = PInvoke.chans_abort(this._handle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns number of available channels (1, 2 or 4 depending on installed options NRP-B2 - Two channel interface and NRP-B5 - Four channel interface).
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="count">
        /// This control returns number of available channels (1, 2 or 4 depending on installed options NRP-B2 - Two channel interface and NRP-B5 - Four channel interface).
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chans_getCount(out int count)
        {
            int pInvokeResult = PInvoke.chans_getCount(this._handle, out count);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function starts a single-shot measurement on all channels. The respective sensor goes to the INITIATED state. The command is completely executed when the sensor returns to the IDLE state. The command is ignored when the sensor is not in the IDLE state or when continuous measurements are selected (INIT:CONT ON). The command is only fully executed when the measurement is completed and the trigger system has again reached the IDLE state. INIT is the only remote control command that permits overlapping execution. Other commands can be received and processed while the command is being executed.
        /// 
        /// Remote-control command(s):
        /// STAT:OPER:MEAS?
        /// INIT:IMM
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chans_initiate()
        {
            int pInvokeResult = PInvoke.chans_initiate(this._handle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function performs zeroing using the signal at the sensor input. The sensor must be disconnected from all power sources. If the signal at the input considerably deviates from 0 W, an error message is issued and the function is aborted.
        /// 
        /// Remote-control command(s):
        /// CAL:ZERO:AUTO
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chans_zero()
        {
            int pInvokeResult = PInvoke.chans_zero(this._handle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the summary status of zeroing on all channels.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="zeroingCompleted">
        /// This control returns VI_TRUE if all channels have calibration ready.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chans_isZeroingComplete(out bool zeroingCompleted)
        {
            ushort zeroingCompletedAsUShort;
            int pInvokeResult = PInvoke.chans_isZeroingComplete(this._handle, out zeroingCompletedAsUShort);
            zeroingCompleted = System.Convert.ToBoolean(zeroingCompletedAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the summary status of measurements on all channels.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="measurementCompleted">
        /// This control returns VI_TRUE if all channels have measurement ready.
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chans_isMeasurementComplete(out bool measurementCompleted)
        {
            ushort measurementCompletedAsUShort;
            int pInvokeResult = PInvoke.chans_isMeasurementComplete(this._handle, out measurementCompletedAsUShort);
            measurementCompleted = System.Convert.ToBoolean(measurementCompletedAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the sensor to one of the measurement modes.
        /// 
        /// Remote-control command(s):
        /// SENSe[1..4]:FUNCtion
        /// SENSe[1..4]:BUFFer:STATe ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="measurementMode">
        /// This control specifies the measurement mode to which sensor should be switched.
        /// 
        /// Valid Values:
        /// RSNRPZ_SENSOR_MODE_CONTAV (0) - ContAv (Default Value)
        /// RSNRPZ_SENSOR_MODE_BUF_CONTAV (1) - Buffered ContAv
        /// RSNRPZ_SENSOR_MODE_TIMESLOT (2) - Timeslot
        /// RSNRPZ_SENSOR_MODE_BURST (3) - Burst
        /// RSNRPZ_SENSOR_MODE_SCOPE (4) - Scope
        /// RSNRPZ_SENSOR_MODE_TIMEGATE (5) - Timegate
        /// RSNRPZ_SENSOR_MODE_CCDF (6) - CCDF
        /// RSNRPZ_SENSOR_MODE_PDF (7) - PDF
        /// 
        /// Notes:
        /// (1) ContAv: After a trigger event, the power is integrated over a time interval (Averaging).
        /// 
        /// (2) Buffered ContAv: The same as ContAv except the buffered mode is switched On.
        /// 
        /// (3) Timeslot: The power is measured simultaneously in a number of timeslots (up to 26).
        /// 
        /// (4) Burst: SENS:POW:BURS:DTOL defines the time interval during which a signal drop below the trigger level is not interpreted as the end of the burst. In the BurstAv mode, the set trigger source is ignored.
        /// 
        /// (5) Scope: A sequence of measurements is performed. The individual measured values are determined as in the ContAv mode.
        /// 
        /// (6) NRP-Z51 supports only RSNRPZ_SENSOR_MODE_CONTAV and RSNRPZ_SENSOR_MODE_BUF_CONTAV.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_mode(int channel, int measurementMode)
        {
            int pInvokeResult = PInvoke.chan_mode(this._handle, channel, measurementMode);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures times that is to be excluded at the beginning and at the end of the integration.
        /// 
        /// Note:
        ///   
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TIM:EXCL:STAR
        /// SENS:TIM:EXCL:STOP
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="excludeStart">
        /// This control sets a time (in seconds) that is to be excluded at the beginning of the integration
        /// 
        /// Valid Range:
        /// NRP-Z21: 0.0 - 0.1 s
        /// FSH-Z1:  0.0 - 0.1 s
        /// 
        /// 
        /// Default Value:
        /// 0.0 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="excludeStop">
        /// This control sets a time (in seconds) that is to be excluded at the end of the integration.
        /// 
        /// Valid Range:
        /// NRP-Z21: 0.0 - 0.003 s
        /// FSH-Z1:  0.0 - 0.003 s
        /// 
        /// Default Value:
        /// 0.0 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timing_configureExclude(int channel, double excludeStart, double excludeStop)
        {
            int pInvokeResult = PInvoke.timing_configureExclude(this._handle, channel, excludeStart, excludeStop);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets a time that is to be excluded at the beginning of the integration.
        /// 
        /// Note:
        ///   
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TIM:EXCL:STAR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="excludeStart">
        /// This control sets a time (in seconds) that is to be excluded at the beginning of the integration
        /// 
        /// Valid Range:
        /// NRP-Z21: 0.0 - 0.1 s
        /// FSH-Z1:  0.0 - 0.1 s
        /// 
        /// Default Value:
        /// 0.0 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timing_setTimingExcludeStart(int channel, double excludeStart)
        {
            int pInvokeResult = PInvoke.timing_setTimingExcludeStart(this._handle, channel, excludeStart);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads a time that is to be excluded at the beginning of the integration.
        /// 
        /// Note:
        ///   
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TIM:EXCL:STAR?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="excludeStart">
        /// This control returns a time (in seconds) that is to be excluded at the beginning of the integration.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timing_getTimingExcludeStart(int channel, out double excludeStart)
        {
            int pInvokeResult = PInvoke.timing_getTimingExcludeStart(this._handle, channel, out excludeStart);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets a time that is to be excluded at the end of the integration.
        /// 
        /// Note:
        ///   
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TIM:EXCL:STOP
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="excludeStop">
        /// This control sets a time (in seconds) that is to be excluded at the end of the integration.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21: 0.0 - 0.003 s
        /// FSH-Z1:  0.0 - 0.003 s
        /// 
        /// Default Value:
        /// 0.0 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timing_setTimingExcludeStop(int channel, double excludeStop)
        {
            int pInvokeResult = PInvoke.timing_setTimingExcludeStop(this._handle, channel, excludeStop);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads a time that is to be excluded at the end of the integration.
        /// 
        /// Note:
        ///   
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TIM:EXCL:STOP?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="excludeStop">
        /// This control returns a time (in seconds) that is to be excluded at the end of the integration.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timing_getTimingExcludeStop(int channel, out double excludeStop)
        {
            int pInvokeResult = PInvoke.timing_getTimingExcludeStop(this._handle, channel, out excludeStop);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function can be used to reduce the video bandwidth for the Trace and Statistics modes. As a result, trigger sensitivity is increased and the display noise reduced. To prevent signals from being corrupted, the selected video bandwidth should not be smaller than the RF bandwidth of the measurement signal. The "FULL" setting corresponds to a video bandwidth of at least 30 MHz if there is an associated frequency setting (SENSe:FREQuency command) greater than or equal to 500 MHz. If a frequency below 500 MHz is set and the video bandwidth is set to "FULL", the video bandwidth is automatically reduced to approx. 7.5 MHz.
        /// If the video bandwidth is limited with the SENSe:BWIDth:VIDEo command, the sampling rate is also automatically reduced, i.e. the effective time resolution in the Trace mode is reduced accordingly. In the Statistics modes, the measurement time must be increased appropriately if the required sample size is to be maintained:
        /// Video bandwidth Sampling rate   Sampling interval
        /// "Full"            8e7 1/s       12.5 ns
        /// "5 MHz"           4e7 1/s         25 ns
        /// "1.5 MHz"         1e7 1/s        100 ns
        /// "300 kHz"       2.5e6 1/s        400 ns
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// sets the video bandwidth according to a list of available bandwidths. The list can be obtained using function rsnrpz_bandwidth_getBwList.
        /// 
        /// Remote-control command(s):
        /// SENSe:BWIDth:VIDeo
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="bandwidth">
        /// This control sets the video bandwidth according to a list of available bandwidths. The list can be obtained using function rsnrpz_bandwidth_getBwList.
        /// 
        /// Valid Range:
        /// Index of bandwidth in the list
        /// 
        /// Default Value:
        /// 0
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int bandwidth_setBw(int channel, int bandwidth)
        {
            int pInvokeResult = PInvoke.bandwidth_setBw(this._handle, channel, bandwidth);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries selected video bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENSe:BWIDth:VIDeo?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="bandwidth">
        /// This control returns selected video bandwidth as index in bandwidth list.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int bandwidth_getBw(int channel, out int bandwidth)
        {
            int pInvokeResult = PInvoke.bandwidth_getBw(this._handle, channel, out bandwidth);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the list of possible video bandwidths.
        /// 
        /// Remote-control command(s):
        /// SENSe:BWIDth:VIDeo:LIST?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="bufferSize">
        /// This control defines the size of buffer passed to Bandwidth List argument.
        /// 
        /// Valid Range:
        /// &gt; 0
        /// 
        /// Default Value:
        /// 200
        /// </param>
        /// <param name="bandwidthList">
        /// This control returns the comma separated list of possible video bandwidths.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int bandwidth_getBwList(int channel, int bufferSize, System.Text.StringBuilder bandwidthList)
        {
            int pInvokeResult = PInvoke.bandwidth_getBwList(this._handle, channel, bufferSize, bandwidthList);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures all parameters necessary for automatic detection of filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO ON
        /// SENS:AVER:COUN:AUTO:TYPE RES
        /// SENS:AVER:COUN:AUTO:RES &lt;value&gt;
        /// SENS:AVER:TCON REP
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="resolution">
        /// This control defines the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result.
        /// 
        /// Valid Range:
        /// 1 to 4
        /// 
        /// Default Value: 3
        /// 
        /// Notes:
        /// (1) Actual range depend on sensor used and may vary from the range stated above.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_configureAvgAuto(int channel, int resolution)
        {
            int pInvokeResult = PInvoke.avg_configureAvgAuto(this._handle, channel, resolution);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures all parameters necessary for setting the noise ratio in the measurement result and automatic detection of filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO ON
        /// SENS:AVER:COUN:AUTO:TYPE NSR
        /// SENS:AVER:COUN:AUTO:NSR &lt;value&gt;
        /// SENS:AVER:COUN:AUTO:MTIM &lt;value&gt;
        /// SENS:AVER:TCON REP
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumNoiseRatio">
        /// This control sets the maximum noise ratio in the measurement result. The value is set in dB.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21: 0.0 - 1.0
        /// FSH-Z1:  0.0 - 1.0
        /// 
        /// Default Value: 0.1
        /// 
        /// Notes:
        /// (1) This value is not range checked; the actual range depends on sensor used.
        /// </param>
        /// <param name="upperTimeLimit">
        /// This control sets the upper time limit (maximum time to fill the filter).
        /// 
        /// Valid Range:
        /// 
        /// NRP-21: 0.01 - 999.99
        /// FSH-Z1: 0.01 - 999.99
        /// 
        /// Default Value: 4.0
        /// 
        /// Notes:
        /// (1) This value is not range checked, the actual range depends on sensor used.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_configureAvgNSRatio(int channel, double maximumNoiseRatio, double upperTimeLimit)
        {
            int pInvokeResult = PInvoke.avg_configureAvgNSRatio(this._handle, channel, maximumNoiseRatio, upperTimeLimit);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures all parameters necessary for manual setting of filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN
        /// SENS:AVER:COUN:AUTO OFF
        /// SENS:AVER:TCON REP
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="count">
        /// This control sets the filter bandwidth.
        /// 
        /// Valid Range:
        /// 1 - 65536
        /// 
        /// Default Value: 4
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_configureAvgManual(int channel, int count)
        {
            int pInvokeResult = PInvoke.avg_configureAvgManual(this._handle, channel, count);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function can be used to automatically determine a value for filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO ON|OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoEnabled">
        /// This control activates or deactivates automatic determination of a value for filter bandwidth.
        /// If the automatic switchover is activated with the ON parameter, the sensor always defines a suitable filter length.
        /// 
        /// Valid Values:
        /// VI_FALSE (0) - Off
        /// VI_TRUE (1) - On (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setAutoEnabled(int channel, bool autoEnabled)
        {
            int pInvokeResult = PInvoke.avg_setAutoEnabled(this._handle, channel, System.Convert.ToUInt16(autoEnabled));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the setting of automatic switchover of filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoEnabled">
        /// This control returns the setting of automatic switchover of filter bandwidth.
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getAutoEnabled(int channel, out bool autoEnabled)
        {
            ushort autoEnabledAsUShort;
            int pInvokeResult = PInvoke.avg_getAutoEnabled(this._handle, channel, out autoEnabledAsUShort);
            autoEnabled = System.Convert.ToBoolean(autoEnabledAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        ///  This function sets an upper time limit can be set via (maximum time). It should never be exceeded. Undesired long measurement times can thus be prevented if the automatic filter length switchover is on.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:MTIM
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="upperTimeLimit">
        /// This control sets the upper time limit (maximum time to fill the filter).
        /// 
        /// Valid Range:
        /// 
        /// NRP-21: 0.01 - 999.99
        /// FSH-Z1: 0.01 - 999.99
        /// 
        /// Default Value: 4.0
        /// 
        /// Notes:
        /// (1) This value is not range checked, the actual range depends on sensor used.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setAutoMaxMeasuringTime(int channel, double upperTimeLimit)
        {
            int pInvokeResult = PInvoke.avg_setAutoMaxMeasuringTime(this._handle, channel, upperTimeLimit);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries an upper time limit (maximum time to fill the filter).
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:MTIM?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="upperTimeLimit">
        /// This control returns an upper time limit (maximum time to fill the filter).
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getAutoMaxMeasuringTime(int channel, out double upperTimeLimit)
        {
            int pInvokeResult = PInvoke.avg_getAutoMaxMeasuringTime(this._handle, channel, out upperTimeLimit);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the maximum noise ratio in the measurement result.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:NSR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumNoiseRatio">
        /// This control sets the maximum noise ratio in the measurement result. The value is set in dB.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21: 0.0 - 1.0
        /// FSH-Z1:  0.0 - 1.0
        /// 
        /// Default Value: 0.1
        /// 
        /// Notes:
        /// (1) This value is not range checked; the actual range depends on sensor used.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setAutoNoiseSignalRatio(int channel, double maximumNoiseRatio)
        {
            int pInvokeResult = PInvoke.avg_setAutoNoiseSignalRatio(this._handle, channel, maximumNoiseRatio);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the maximum noise signal ratio value.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:NSR?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumNoiseRatio">
        /// This control returns a maximum noise signal ratio in dB.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getAutoNoiseSignalRatio(int channel, out double maximumNoiseRatio)
        {
            int pInvokeResult = PInvoke.avg_getAutoNoiseSignalRatio(this._handle, channel, out maximumNoiseRatio);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result. This setting does not affect the setting of display.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:RES 1 ... 4
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="resolution">
        /// This control defines the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result.
        /// 
        /// Valid Range:
        /// 1 to 4
        /// 
        /// Default Value: 3
        /// 
        /// Notes:
        /// (1) Actual range depend on sensor used and may vary from the range stated above.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setAutoResolution(int channel, int resolution)
        {
            int pInvokeResult = PInvoke.avg_setAutoResolution(this._handle, channel, resolution);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result. 
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:RES?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="resolution">
        /// This control returns the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result.
        /// 
        /// Valid Range:
        /// 1 to 4
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getAutoResolution(int channel, out int resolution)
        {
            int pInvokeResult = PInvoke.avg_getAutoResolution(this._handle, channel, out resolution);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function selects a method by which the automatic filter length switchover can operate.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:TYPE RES | NSR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="method">
        /// This control selects a method by which the automatic filter length switchover can operate.
        /// 
        /// Valid Values:
        /// RSNRPZ_AUTO_COUNT_TYPE_RESOLUTION (0) - Resolution (Default Value)
        /// RSNRPZ_AUTO_COUNT_TYPE_NSR (1) - Noise Ratio
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setAutoType(int channel, int method)
        {
            int pInvokeResult = PInvoke.avg_setAutoType(this._handle, channel, method);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns a method by which the automatic filter length switchover can operate.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:TYPE?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="method">
        /// This control returns a method by which the automatic filter length switchover can operate.
        /// 
        /// Valid Values:
        /// Resolution (RSNRPZ_AUTO_COUNT_TYPE_RESOLUTION)
        /// Noise Ratio (RSNRPZ_AUTO_COUNT_TYPE_NSR)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getAutoType(int channel, out int method)
        {
            int pInvokeResult = PInvoke.avg_getAutoType(this._handle, channel, out method);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the filter bandwidth. The wider the filter the lower the noise and the longer it takes to obtain a measured value.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="count">
        /// This control sets the filter bandwidth.
        /// 
        /// Valid Range:
        /// 1 - 65536
        /// 
        /// Default Value: 4
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setCount(int channel, int count)
        {
            int pInvokeResult = PInvoke.avg_setCount(this._handle, channel, count);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="count">
        /// This control returns the filter bandwidth.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getCount(int channel, out int count)
        {
            int pInvokeResult = PInvoke.avg_getCount(this._handle, channel, out count);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function switches the filter function of a sensor on or off. When the filter is switched on, the number of measured values set with rsnrpz_avg_setCount() is averaged. This reduces the effect of noise so that more reliable results are obtained.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="averaging">
        /// This control switches the filter function of a sensor on or off.
        /// 
        /// Valid Values:
        /// VI_TRUE (1)  - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setEnabled(int channel, bool averaging)
        {
            int pInvokeResult = PInvoke.avg_setEnabled(this._handle, channel, System.Convert.ToUInt16(averaging));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the state of the filter function of a sensor.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="averaging">
        /// This control returns the state of the filter function of a sensor.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getEnabled(int channel, out bool averaging)
        {
            ushort averagingAsUShort;
            int pInvokeResult = PInvoke.avg_getEnabled(this._handle, channel, out averagingAsUShort);
            averaging = System.Convert.ToBoolean(averagingAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets a timeslot whose measured value is used to automatically determine the filter length.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:SLOT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeslot">
        /// This control sets a timeslot whose measured value is used to automatically determine the filter length.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21: 1 - 8
        /// FSH-Z1:  1 - 8
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setSlot(int channel, int timeslot)
        {
            int pInvokeResult = PInvoke.avg_setSlot(this._handle, channel, timeslot);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns a timeslot whose measured value is used to automatically determine the filter length.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:SLOT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeslot">
        /// This control returns a timeslot whose measured value is used to automatically determine the filter length.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getSlot(int channel, out int timeslot)
        {
            int pInvokeResult = PInvoke.avg_getSlot(this._handle, channel, out timeslot);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function determines whether a new result is calculated immediately after a new measured value is available or only after an entire range of new values is available for the filter.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:TCON MOV | REP
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="terminalControl">
        /// This control determines the type of terminal control.
        /// 
        /// Valid Values:
        /// RSNRPZ_TERMINAL_CONTROL_MOVING (0) - Moving
        /// RSNRPZ_TERMINAL_CONTROL_REPEAT (1) - Repeat (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_setTerminalControl(int channel, int terminalControl)
        {
            int pInvokeResult = PInvoke.avg_setTerminalControl(this._handle, channel, terminalControl);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the type of terminal control.
        /// 
        /// Remote-control command(s):
        /// SENSe[1..4]:AVERage:TCONtrol?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="terminalControl">
        /// This control returns the type of terminal control.
        /// 
        /// Valid Values:
        /// RSNRPZ_TERMINAL_CONTROL_MOVING (0) - Moving
        /// RSNRPZ_TERMINAL_CONTROL_REPEAT (1) - Repeat (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_getTerminalControl(int channel, out int terminalControl)
        {
            int pInvokeResult = PInvoke.avg_getTerminalControl(this._handle, channel, out terminalControl);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function initializes the digital filter by deleting the stored measured values.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:RES
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int avg_reset(int channel)
        {
            int pInvokeResult = PInvoke.avg_reset(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the automatic selection of a measurement range to ON or OFF.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:RANG:AUTO ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoRange">
        /// This control sets the automatic selection of a measurement range to ON or OFF.
        /// 
        /// Valid Values:
        /// VI_TRUE (1)  - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int range_setAutoEnabled(int channel, bool autoRange)
        {
            int pInvokeResult = PInvoke.range_setAutoEnabled(this._handle, channel, System.Convert.ToUInt16(autoRange));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the state of automatic selection of a measurement range.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:RANG:AUTO?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoRange">
        /// This control returns the state of automatic selection of a measurement range.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int range_getAutoEnabled(int channel, out bool autoRange)
        {
            ushort autoRangeAsUShort;
            int pInvokeResult = PInvoke.range_getAutoEnabled(this._handle, channel, out autoRangeAsUShort);
            autoRange = System.Convert.ToBoolean(autoRangeAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the cross-over level. Shifts the transition ranges between the measurement ranges. This may improve the measurement accuracy for special signals, i.e. signals with a high crest factor.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:RANG:AUTO:CLEV
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="crossoverLevel">
        /// This control sets the cross-over level in dB.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21: -20.0 - 0.0 dB
        /// FSH-Z1:  -20.0 - 0.0 dB
        /// 
        /// Default Value: 0.0 dB
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int range_setCrossoverLevel(int channel, double crossoverLevel)
        {
            int pInvokeResult = PInvoke.range_setCrossoverLevel(this._handle, channel, crossoverLevel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the cross-over level.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:RANG:AUTO:CLEV?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="crossoverLevel">
        /// This control returns the cross-over level in dB.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors 
        /// </returns>
        public int range_getCrossoverLevel(int channel, out double crossoverLevel)
        {
            int pInvokeResult = PInvoke.range_getCrossoverLevel(this._handle, channel, out crossoverLevel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function selects a measurement range in which the corresponding sensor is to perform a measurement.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:RANG 0 .. 2
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="range">
        /// This control selects a measurement range in which the corresponding sensor is to perform a measurement.
        /// 
        /// Valid Range:
        /// NRP-Z21:  0 to 2
        /// FSH-1:    0 to 2
        /// 
        /// Default Value: 2
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int range_setRange(int channel, int range)
        {
            int pInvokeResult = PInvoke.range_setRange(this._handle, channel, range);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns a measurement range in which the corresponding sensor is to perform a measurement.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51
        /// 
        /// Remote-control command(s):
        /// SENS:RANG?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="range">
        /// This control returns a measurement range in which the corresponding sensor is to perform a measurement.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int range_getRange(int channel, out int range)
        {
            int pInvokeResult = PInvoke.range_getRange(this._handle, channel, out range);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures all correction parameters.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:OFFS
        /// SENS:CORR:OFFS:STAT ON | OFF
        /// SENS:CORR:SPD:STAT ON | OFF
        /// 
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offsetState">
        /// This control switches the offset correction on or off.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <param name="offset">
        /// This control sets a fixed offset value can be defined for multiplying (logarithmically adding) the measured value of a sensor.
        /// 
        /// Valid Range:
        ///   -200.0 to 200.0 dB
        /// 
        /// 
        /// Default Value:
        /// 0.0 dB
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="reserved1">
        /// This prameter is reserved. Value is ignored.
        /// </param>
        /// <param name="reserved2">
        /// This prameter is reserved. Value is ignored.
        /// 
        /// Default Value:
        /// ""
        /// </param>
        /// <param name="sParameterEnable">
        /// This control enables or disables measured-value correction by means of the stored s-parameter device.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// 
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_configureCorrections(int channel, bool offsetState, double offset, bool reserved1, string reserved2, bool sParameterEnable)
        {
            int pInvokeResult = PInvoke.corr_configureCorrections(this._handle, channel, System.Convert.ToUInt16(offsetState), offset, System.Convert.ToUInt16(reserved1), reserved2, System.Convert.ToUInt16(sParameterEnable));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function informs the R&amp;S NRP about the frequency of the power to be measured since this frequency is not automatically determined. The frequency is used to determine a frequency-dependent correction factor for the measurement results.
        /// 
        /// Remote-control command(s):
        /// SENS:FREQ
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="frequency">
        /// This control sets the frequency in Hz of the power to be measured since this frequency is not automatically determined.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21: 10.0e6 - 18.0e9
        /// FSH-Z1:  10.0e6 -  8.0e9
        /// NRP-Z51: 0.0    - 18.0e9 (depends on the calibration data)
        /// 
        /// Default Value: 50.0e6 Hz
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setCorrectionFrequency(int channel, double frequency)
        {
            int pInvokeResult = PInvoke.chan_setCorrectionFrequency(this._handle, channel, frequency);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the instrument for the frequency of the power to be measured since this frequency is not automatically determined.
        /// 
        /// Remote-control command(s):
        /// SENSe[1..4]:FREQuency?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="frequency">
        /// This control returns the frequency in Hz of the power to be measured since this frequency is not automatically determined.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getCorrectionFrequency(int channel, out double frequency)
        {
            int pInvokeResult = PInvoke.chan_getCorrectionFrequency(this._handle, channel, out frequency);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// With this function a fixed offset value can be defined for multiplying (logarithmically adding) the measured value of a sensor.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:OFFS
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control sets a fixed offset value can be defined for multiplying (logarithmically adding) the measured value of a sensor.
        /// 
        /// Valid Range:
        ///   -200.0 to 200.0 dB
        /// 
        /// Default Value:
        /// 0.0 dB
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_setOffset(int channel, double offset)
        {
            int pInvokeResult = PInvoke.corr_setOffset(this._handle, channel, offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets a fixed offset value defined for multiplying (logarithmically adding) the measured value of a sensor.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:OFFS?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control returns a fixed offset value defined for multiplying (logarithmically adding) the measured value of a sensor.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_getOffset(int channel, out double offset)
        {
            int pInvokeResult = PInvoke.corr_getOffset(this._handle, channel, out offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function switches the offset correction on or off.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:OFFS:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offsetState">
        /// This control switches the offset correction on or off.
        /// 
        /// Valid Values:
        /// VI_TRUE (1)  - On 
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_setOffsetEnabled(int channel, bool offsetState)
        {
            int pInvokeResult = PInvoke.corr_setOffsetEnabled(this._handle, channel, System.Convert.ToUInt16(offsetState));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the offset correction on or off.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:OFFS:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offsetState">
        /// This control returns the offset correction on or off.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_getOffsetEnabled(int channel, out bool offsetState)
        {
            ushort offsetStateAsUShort;
            int pInvokeResult = PInvoke.corr_getOffsetEnabled(this._handle, channel, out offsetStateAsUShort);
            offsetState = System.Convert.ToBoolean(offsetStateAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function instructs the sensor to perform a measured-value correction by means of the stored s-parameter device.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:SPD:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="sParameterEnable">
        /// This control enables or disables measured-value correction by means of the stored s-parameter device.
        /// 
        /// Valid Values:
        /// VI_TRUE (1)  - On
        /// VI_FALSE (0) - Off (Default Value)
        /// 
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_setSParamDeviceEnabled(int channel, bool sParameterEnable)
        {
            int pInvokeResult = PInvoke.corr_setSParamDeviceEnabled(this._handle, channel, System.Convert.ToUInt16(sParameterEnable));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the state of a measured-value correction by means of the stored s-parameter device.
        /// 
        /// Remote-control command(s):
        /// SENSe[1..4]:CORRection:SPDevice:STATe?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="sParameterCorrection">
        /// This control returns the state of S-Parameter correction.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_getSParamDeviceEnabled(int channel, out bool sParameterCorrection)
        {
            ushort sParameterCorrectionAsUShort;
            int pInvokeResult = PInvoke.corr_getSParamDeviceEnabled(this._handle, channel, out sParameterCorrectionAsUShort);
            sParameterCorrection = System.Convert.ToBoolean(sParameterCorrectionAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function can be used to select a loaded data set for S-parameter correction. This data set is accessed by means of a consecutive number, starting with 1 for the first data set. If an invalid data set consecutive number is entered, an error message is output.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only on NRP-Z81.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:SPD:SEL
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="sParameter">
        /// This control can be used to select a loaded data set for S-parameter correction. This data set is accessed by means of a consecutive number, starting with 1 for the first data set. If an invalid data set consecutive number is entered, an error message is output.
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_setSParamDevice(int channel, int sParameter)
        {
            int pInvokeResult = PInvoke.corr_setSParamDevice(this._handle, channel, sParameter);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets selected data set for S-parameter correction. 
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only on NRP-Z81.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:SPD:SEL?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="sParameter">
        /// This control returns selected data set for S-parameter correction. 
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_getSParamDevice(int channel, out int sParameter)
        {
            int pInvokeResult = PInvoke.corr_getSParamDevice(this._handle, channel, out sParameter);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the parameters of the reflection coefficient for measured-value correction.
        /// 
        /// Remote-control command(s):
        /// SENS:SGAM
        /// SENS:SGAM:PHAS
        /// SENS:SGAM:CORR:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="sourceGammaCorrection">
        /// This control enables or disables source gamma correction of the measured value.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <param name="magnitude">
        /// This control sets the magnitude of the reflection coefficient.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21 0.0 - 1.0
        /// FSH-Z1: 0.0 - 1.0
        /// 
        /// Default Value: 1.0
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="phase">
        /// This control defines the phase angle of the reflection coefficient. Units are degrees.
        /// 
        /// Valid Range:
        /// -360.0 to 360.0 deg
        /// 
        /// Default Value:
        /// 0.0 deg
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_configureSourceGammaCorr(int channel, bool sourceGammaCorrection, double magnitude, double phase)
        {
            int pInvokeResult = PInvoke.chan_configureSourceGammaCorr(this._handle, channel, System.Convert.ToUInt16(sourceGammaCorrection), magnitude, phase);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the magnitude of the reflection coefficient for measured-value correction.
        /// 
        /// Remote-control command(s):
        /// SENS:SGAM:MAGN
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="magnitude">
        /// This control sets the magnitude of the reflection coefficient.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21 0.0 - 1.0
        /// FSH-Z1: 0.0 - 1.0
        /// 
        /// Default Value: 1.0
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setSourceGammaMagnitude(int channel, double magnitude)
        {
            int pInvokeResult = PInvoke.chan_setSourceGammaMagnitude(this._handle, channel, magnitude);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the magnitude of the reflection coefficient for measured-value correction.
        /// 
        /// Remote-control command(s):
        /// SENS:SGAM:MAGN?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="magnitude">
        /// This control returns the magnitude of the reflection coefficient for measured-value correction.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getSourceGammaMagnitude(int channel, out double magnitude)
        {
            int pInvokeResult = PInvoke.chan_getSourceGammaMagnitude(this._handle, channel, out magnitude);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the phase angle of the reflection coefficient for measured-value correction.
        /// 
        /// Remote-control command(s):
        /// SENS:SGAM:PHAS
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="phase">
        /// This control defines the phase angle of the reflection coefficient. Units are degrees.
        /// 
        /// Valid Range:
        /// -360.0 to 360.0 deg
        /// 
        /// Default Value:
        /// 0.0 deg
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setSourceGammaPhase(int channel, double phase)
        {
            int pInvokeResult = PInvoke.chan_setSourceGammaPhase(this._handle, channel, phase);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the phase angle of the reflection coefficient for measured-value correction.
        /// 
        /// Remote-control command(s):
        /// SENS:SGAM:PHAS?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="phase">
        /// This control returns the phase angle of the reflection coefficient. Units are degrees.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getSourceGammaPhase(int channel, out double phase)
        {
            int pInvokeResult = PInvoke.chan_getSourceGammaPhase(this._handle, channel, out phase);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function switches the measured-value correction of the reflection coefficient effect of the source gamma ON or OFF.
        /// 
        /// Remote-control command(s):
        /// SENS:SGAM:CORR:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="sourceGammaCorrection">
        /// This control enables or disables source gamma correction of the measured value.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setSourceGammaCorrEnabled(int channel, bool sourceGammaCorrection)
        {
            int pInvokeResult = PInvoke.chan_setSourceGammaCorrEnabled(this._handle, channel, System.Convert.ToUInt16(sourceGammaCorrection));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the state of source gamma correction.
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM:CORR:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="sourceGammaCorrection">
        /// This control returns the state of source gamma correction.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getSourceGammaCorrEnabled(int channel, out bool sourceGammaCorrection)
        {
            ushort sourceGammaCorrectionAsUShort;
            int pInvokeResult = PInvoke.chan_getSourceGammaCorrEnabled(this._handle, channel, out sourceGammaCorrectionAsUShort);
            sourceGammaCorrection = System.Convert.ToBoolean(sourceGammaCorrectionAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the parameters of the compensation of the load distortion at the signal output.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only for sensors NRP-Z27 and NRP-Z37
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM
        /// SENS:RGAM:PHAS
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="magnitude">
        /// This control sets the magnitude of the reflection coefficient of the load for distortion compensation.
        /// 
        /// Valid Range:
        /// 0.0 - 1.0
        /// 
        /// Default Value: 0.0
        /// </param>
        /// <param name="phase">
        /// This control defines the phase angle (in degrees) of the complex reflection factor of the load at the signal output.
        /// 
        /// Valid Range:
        /// -360.0 to 360.0 deg
        /// 
        /// Default Value:
        /// 0.0 deg
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_configureReflectGammaCorr(int channel, double magnitude, double phase)
        {
            int pInvokeResult = PInvoke.chan_configureReflectGammaCorr(this._handle, channel, magnitude, phase);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the magnitude of the reflection coefficient of the load for distortion compensation.
        /// To deactivate distortion compensation, set Magnitude to 0. Distortion compensation should remain deactivated in the case of questionable measurement values for the reflection coefficient of the load.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only for sensors NRP-Z27 and NRP-Z37
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM:MAGN
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="magnitude">
        /// This control sets the magnitude of the reflection coefficient of the load for distortion compensation.
        /// 
        /// Valid Range:
        /// 0.0 - 1.0
        /// 
        /// Default Value: 0.0
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setReflectionGammaMagn(int channel, double magnitude)
        {
            int pInvokeResult = PInvoke.chan_setReflectionGammaMagn(this._handle, channel, magnitude);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the magnitude of the reflection coefficient of the load for distortion compensation.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only for sensors NRP-Z27 and NRP-Z37
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM:MAGN?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="magnitude">
        /// This control returns the magnitude of the reflection coefficient of the load for distortion compensation.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getReflectionGammaMagn(int channel, out double magnitude)
        {
            int pInvokeResult = PInvoke.chan_getReflectionGammaMagn(this._handle, channel, out magnitude);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines the phase angle (in degrees) of the complex reflection factor of the load at the signal output.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only for sensors NRP-Z27 and NRP-Z37
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM:PHAS
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="phase">
        /// This control defines the phase angle (in degrees) of the complex reflection factor of the load at the signal output.
        /// 
        /// Valid Range:
        /// -360.0 to 360.0 deg
        /// 
        /// Default Value:
        /// 0.0 deg
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setReflectionGammaPhase(int channel, double phase)
        {
            int pInvokeResult = PInvoke.chan_setReflectionGammaPhase(this._handle, channel, phase);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the phase angle (in degrees) of the complex reflection factor of the load at the signal output.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only for sensors NRP-Z27 and NRP-Z37
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM:PHAS?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="phase">
        /// This control returns the phase angle (in degrees) of the complex reflection factor of the load at the signal output.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getReflectionGammaPhase(int channel, out double phase)
        {
            int pInvokeResult = PInvoke.chan_getReflectionGammaPhase(this._handle, channel, out phase);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines reflection gamma uncertainty.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only for sensors NRP-Z27 and NRP-Z37
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM:EUNC
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="uncertainty">
        /// This control defines the uncertainty.
        /// 
        /// Valid Range:
        /// 0.0 to 1.0
        /// 
        /// Default Value:
        /// 0.0
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setReflectionGammaUncertainty(int channel, double uncertainty)
        {
            int pInvokeResult = PInvoke.chan_setReflectionGammaUncertainty(this._handle, channel, uncertainty);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries reflection gamma uncertainty.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is available only for sensors NRP-Z27 and NRP-Z37
        /// 
        /// Remote-control command(s):
        /// SENS:RGAM:EUNC?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="uncertainty">
        /// This control returns the uncertainty.
        /// 
        /// Valid Range:
        /// 0.0 to 1.0
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getReflectionGammaUncertainty(int channel, out double uncertainty)
        {
            int pInvokeResult = PInvoke.chan_getReflectionGammaUncertainty(this._handle, channel, out uncertainty);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures all duty cycle parameters.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:DCYC
        /// SENS:CORR:DCYC:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dutyCycleState">
        /// This control switches measured-value correction for a specific duty cycle on or off.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <param name="dutyCycle">
        /// This control sets the duty cycle of power to be measured.
        /// 
        /// Valid Range:
        /// 0.001 - 99.999%
        /// 
        /// Default Value: 1.0 %
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_configureDutyCycle(int channel, bool dutyCycleState, double dutyCycle)
        {
            int pInvokeResult = PInvoke.corr_configureDutyCycle(this._handle, channel, System.Convert.ToUInt16(dutyCycleState), dutyCycle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function informs the R&amp;S NRP about the duty cycle of the power to be measured. Specifying a duty cycle only makes sense in the ContAv mode where measurements are performed continuously without taking the timing of the signal into account. For this reason, this setting can only be chosen in the local mode when the sensor performs measurements in the ContAv mode.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:DCYC
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dutyCycle">
        /// This control sets the duty cycle of power to be measured.
        /// 
        /// Valid Range:
        /// 0.001 - 99.999 %
        /// 
        /// Default Value: 1.0 %
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_setDutyCycle(int channel, double dutyCycle)
        {
            int pInvokeResult = PInvoke.corr_setDutyCycle(this._handle, channel, dutyCycle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets the size of duty cycle of the power to be measured.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:DCYC?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dutyCycle">
        /// This control returns the size of duty cycle of the power to be measured. Units are %.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_getDutyCycle(int channel, out double dutyCycle)
        {
            int pInvokeResult = PInvoke.corr_getDutyCycle(this._handle, channel, out dutyCycle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function switches measured-value correction for a specific duty cycle on or off.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:DCYC:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dutyCycleState">
        /// This control switches measured-value correction for a specific duty cycle on or off.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_setDutyCycleEnabled(int channel, bool dutyCycleState)
        {
            int pInvokeResult = PInvoke.corr_setDutyCycleEnabled(this._handle, channel, System.Convert.ToUInt16(dutyCycleState));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets the setting of duty cycle.
        /// 
        /// Remote-control command(s):
        /// SENS:CORR:DCYC:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dutyCycleState">
        /// This control returns the state of duty cycle.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int corr_getDutyCycleEnabled(int channel, out bool dutyCycleState)
        {
            ushort dutyCycleStateAsUShort;
            int pInvokeResult = PInvoke.corr_getDutyCycleEnabled(this._handle, channel, out dutyCycleStateAsUShort);
            dutyCycleState = System.Convert.ToBoolean(dutyCycleStateAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function determines the integration time for a single measurement in the ContAv mode. To increase the measurement accuracy, this integration is followed by a second averaging procedure in a window with a selectable number of values.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:APER
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="contAvAperture">
        /// This control defines the ContAv Aperture in seconds.
        /// 
        /// Valid Range:
        /// NRP-Z21:   0.1e-6 to 0.3 seconds
        /// NRP-Z51:   0.1e-3 to 0.3 seconds
        /// FSH-Z1:    0.1e-6 to 0.3 seconds
        /// 
        /// Default Value: 0.02 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setContAvAperture(int channel, double contAvAperture)
        {
            int pInvokeResult = PInvoke.chan_setContAvAperture(this._handle, channel, contAvAperture);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the value of ContAv mode aperture size.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:APER?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="contAvAperture">
        /// This control returns the ContAv Aperture size in seconds.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getContAvAperture(int channel, out double contAvAperture)
        {
            int pInvokeResult = PInvoke.chan_getContAvAperture(this._handle, channel, out contAvAperture);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function activates digital lowpass filtering of the sampled video signal.
        /// The problem of instable display values due to a modulation of a test signal can also be eliminated by lowpass filtering of the video signal. The lowpass filter eliminates the variations of the display even in case of unperiodic modulation and does not require any other setting.
        /// If the modulation is periodic, the setting of the sampling window is the better method since it allows for shorter measurement times.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:SMO:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="contAvSmoothing">
        /// This control sets the state of digital lowpass filtering of the sampled video signal.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setContAvSmoothingEnabled(int channel, bool contAvSmoothing)
        {
            int pInvokeResult = PInvoke.chan_setContAvSmoothingEnabled(this._handle, channel, System.Convert.ToUInt16(contAvSmoothing));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets the state of digital lowpass filtering of the sampled video signal.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:SMO:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="contAvSmoothing">
        /// This control returns the state of digital lowpass filtering of the sampled video signal.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getContAvSmoothingEnabled(int channel, out bool contAvSmoothing)
        {
            ushort contAvSmoothingAsUShort;
            int pInvokeResult = PInvoke.chan_getContAvSmoothingEnabled(this._handle, channel, out contAvSmoothingAsUShort);
            contAvSmoothing = System.Convert.ToBoolean(contAvSmoothingAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function switches on the buffered ContAv mode, after which data blocks rather than single measured values are then  returned. In this mode a higher data rate is achieved than in the non-buffered ContAv mode.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:BUFF:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="contAvBufferedMode">
        /// This control turns on or off ContAv buffered measurement mode.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setContAvBufferedEnabled(int channel, bool contAvBufferedMode)
        {
            int pInvokeResult = PInvoke.chan_setContAvBufferedEnabled(this._handle, channel, System.Convert.ToUInt16(contAvBufferedMode));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the state of ContAv Buffered Measurement Mode.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:BUFF:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="contAvBufferedMode">
        /// This control returns the state of ContAv Buffered Measurement Mode.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getContAvBufferedEnabled(int channel, out bool contAvBufferedMode)
        {
            ushort contAvBufferedModeAsUShort;
            int pInvokeResult = PInvoke.chan_getContAvBufferedEnabled(this._handle, channel, out contAvBufferedModeAsUShort);
            contAvBufferedMode = System.Convert.ToBoolean(contAvBufferedModeAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the number of desired values for the buffered ContAv mode.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:BUFF:SIZE
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="bufferSize">
        /// This control sets the number of desired values for buffered ContAv mode.
        /// 
        /// Valid Range:
        /// 1 to 1024
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setContAvBufferSize(int channel, int bufferSize)
        {
            int pInvokeResult = PInvoke.chan_setContAvBufferSize(this._handle, channel, bufferSize);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the number of desired values for the buffered ContAv mode.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:AVG:BUFF:SIZE?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="bufferSize">
        /// This control returns the number of desired values for the buffered ContAv mode.
        /// 
        /// Valid Range:
        /// 1 to 400000
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getContAvBufferSize(int channel, out int bufferSize)
        {
            int pInvokeResult = PInvoke.chan_getContAvBufferSize(this._handle, channel, out bufferSize);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// The end of a burst (power pulse) is recognized when the signal level drops below the trigger level. Especially with modulated signals, this may also happen for a short time within a burst. To prevent the supposed end of the burst is from being recognized too early or incorrectly at these positions, a time interval can be defined via using this function (drop-out tolerance parameter) in which the pulse end is only recognized if the signal level no longer exceeds the trigger level.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51
        /// 
        /// Remote-control command(s):
        /// SENS:POW:BURS:DTOL
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dropOutTolerance">
        /// This parameter defines the Drop-Out Tolerance time interval in seconds.
        /// 
        /// Valid Range:
        /// 0.0 to 3.0e-3 seconds
        /// 
        /// Default Value: 100.0e-6 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// 
        /// </returns>
        public int chan_setBurstDropoutTolerance(int channel, double dropOutTolerance)
        {
            int pInvokeResult = PInvoke.chan_setBurstDropoutTolerance(this._handle, channel, dropOutTolerance);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the drop-out tolerance parameter.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51
        /// 
        /// Remote-control command(s):
        /// SENS:POW:BURS:DTOL?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dropOutTolerance">
        /// This control returns the drop-out tolerance parameter.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getBurstDropoutTolerance(int channel, out double dropOutTolerance)
        {
            int pInvokeResult = PInvoke.chan_getBurstDropoutTolerance(this._handle, channel, out dropOutTolerance);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function enables or disables the chopper in BurstAv mode. Disabling the chopper is only good for fast but unexact and noisy measurements. If the chopper is disabled, averaging is ignored internally also disabled.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:BURSt:CHOPper:STATe
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="burstAvChopper">
        /// This control enables or disables the chopper for BurstAv mode.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setBurstChopperEnabled(int channel, bool burstAvChopper)
        {
            int pInvokeResult = PInvoke.chan_setBurstChopperEnabled(this._handle, channel, System.Convert.ToUInt16(burstAvChopper));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the state of the chopper in BurstAv mode.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:BURSt:CHOPper:STATe?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="burstAvChopper">
        /// This control returns the state of the chopper for BurstAv mode.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getBurstChopperEnabled(int channel, out bool burstAvChopper)
        {
            ushort burstAvChopperAsUShort;
            int pInvokeResult = PInvoke.chan_getBurstChopperEnabled(this._handle, channel, out burstAvChopperAsUShort);
            burstAvChopper = System.Convert.ToBoolean(burstAvChopperAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines the time gate measured by the sensor.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:OFFSet:TIME
        /// SENSe:POWer:TGATe[1...16]:FREQuency
        /// SENSe:POWer:TGATe[1...16]:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="offset">
        /// This control is used to specify the start of a timegate for recording measured values in the Timegate Average mode. The start time is referenced to the delayed trigger point (TRIGger:DELay command). Only positive values are valid. If the start of the timegate is before the physical trigger point, the trigger delay must be set to a negative value with an appropriately large magnitude (minimum 51.2 us).
        /// 
        /// Valid Range:
        /// 0.0 to 10.0
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <param name="time">
        /// This control sets the length of a timegate for recording measured values in the Timegate Average mode.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <param name="frequency">
        /// This control transfers the carrier frequency of the RF signal to be measured in the timegate indicated. When the R&amp;S NRP-Z81 power sensor is used, it is essential that the current carrier frequency is set. Otherwise non-linearities and temperature dependencies with values considerably higher than those stated in the data sheet would occur. If the frequency which has been entered is below 500 MHz, the video bandwidth of the power sensor is reduced automatically. The center frequency is set for broadband signals (spread-spectrum signals, multicarrier signals), if there is no explicit carrier.
        /// 
        /// Valid Range:
        /// 50.0e6 to 18.0e9 Hz
        /// 
        /// Default Value: 1.0e9 Hz
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_configureTimeGate(int channel, int selectGate, double offset, double time, double frequency)
        {
            int pInvokeResult = PInvoke.timegate_configureTimeGate(this._handle, channel, selectGate, offset, time, frequency);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function is used to specify the start of a timegate for recording measured values in the Timegate Average mode. The start time is referenced to the delayed trigger point (TRIGger:DELay command). Only positive values are valid. If the start of the timegate is before the physical trigger point, the trigger delay must be set to a negative value with an appropriately large magnitude (minimum 51.2 us).
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:OFFSet:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="offset">
        /// This control sets the start of the timegate after the trigger.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_setOffsetTime(int channel, int selectGate, double offset)
        {
            int pInvokeResult = PInvoke.timegate_setOffsetTime(this._handle, channel, selectGate, offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the start of the timegate after the trigger.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:OFFSet:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="offset">
        /// This control returns the start of the timegate after the trigger.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_getOffsetTime(int channel, int selectGate, out double offset)
        {
            int pInvokeResult = PInvoke.timegate_getOffsetTime(this._handle, channel, selectGate, out offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the length of a timegate for recording measured values in the Timegate Average mode.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="time">
        /// This control sets the length of a timegate for recording measured values in the Timegate Average mode.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_setTime(int channel, int selectGate, double time)
        {
            int pInvokeResult = PInvoke.timegate_setTime(this._handle, channel, selectGate, time);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the length of the timegate.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="time">
        /// This control returns the length of the timegate.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_getTime(int channel, int selectGate, out double time)
        {
            int pInvokeResult = PInvoke.timegate_getTime(this._handle, channel, selectGate, out time);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function transfers the carrier frequency of the RF signal to be measured in the timegate indicated. When the R&amp;S NRP-Z81 power sensor is used, it is essential that the current carrier frequency is set. Otherwise non-linearities and temperature dependencies with values considerably higher than those stated in the data sheet would occur. If the frequency which has been entered is below 500 MHz, the video bandwidth of the power sensor is reduced automatically. The center frequency is set for broadband signals (spread-spectrum signals, multicarrier signals), if there is no explicit carrier.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:FREQuency
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="frequency">
        /// This control transfers the carrier frequency of the RF signal to be measured in the timegate indicated. When the R&amp;S NRP-Z81 power sensor is used, it is essential that the current carrier frequency is set. Otherwise non-linearities and temperature dependencies with values considerably higher than those stated in the data sheet would occur. If the frequency which has been entered is below 500 MHz, the video bandwidth of the power sensor is reduced automatically. The center frequency is set for broadband signals (spread-spectrum signals, multicarrier signals), if there is no explicit carrier.
        /// 
        /// Valid Range:
        /// 50.0e6 to 18.0e9 Hz
        /// 
        /// Default Value: 1.0e9 Hz
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_setFrequency(int channel, int selectGate, double frequency)
        {
            int pInvokeResult = PInvoke.timegate_setFrequency(this._handle, channel, selectGate, frequency);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the carrier frequency of the RF signal to be measured in the timegate indicated.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:FREQuency?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="frequency">
        /// This control returns the signal's frequency for timegate.
        /// 
        /// Valid Range:
        /// 50.0e6 to 18.0e9 Hz
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_getFrequency(int channel, int selectGate, out double frequency)
        {
            int pInvokeResult = PInvoke.timegate_getFrequency(this._handle, channel, selectGate, out frequency);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function is used to define the start of an exclusion interval in the specified timegate. The start of the exclusion interval is referenced to the start of the timegate.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:EXCLude:MID:OFFSet
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="offset">
        /// This control is used to define the start of an exclusion interval in the specified timegate. The start of the exclusion interval is referenced to the start of the timegate.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_setMidOffset(int channel, int selectGate, double offset)
        {
            int pInvokeResult = PInvoke.timegate_setMidOffset(this._handle, channel, selectGate, offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the midamble offset after timeslot start in seconds in the Timegate mode.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:EXCLude:MID:OFFSet?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="offset">
        /// This control returns the mid offset of time slot in seconds of the timeslot in the Timegate mode.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_getMidOffset(int channel, int selectGate, out double offset)
        {
            int pInvokeResult = PInvoke.timegate_getMidOffset(this._handle, channel, selectGate, out offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function is used to specify the length of an exclusion interval.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:EXCLude:MID:TIME
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="length">
        /// This control is used to specify the length of an exclusion interval.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_setMidLength(int channel, int selectGate, double length)
        {
            int pInvokeResult = PInvoke.timegate_setMidLength(this._handle, channel, selectGate, length);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the length of an exclusion interval.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe[1...16]:EXCLude:MID:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="selectGate">
        /// This control selects the gate.
        /// 
        /// Valid Values:
        /// 1 to 16
        /// 
        /// Default Value:
        /// 1
        /// </param>
        /// <param name="length">
        /// This control returns the length of an exclusion interval.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_getMidLength(int channel, int selectGate, out double length)
        {
            int pInvokeResult = PInvoke.timegate_getMidLength(this._handle, channel, selectGate, out length);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function enables or disables the chopper stabilization for the Timegate Average mode and to deactivate averaging. Disabling means that each defined timegate is measured only once, providing the shortest possible measurement time for the signal and also making it possible to analyze one-off events. Only high power values (from approx. -30 dBm) should be measured in this mode because of the elevated noise component and the considerable zero drift.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe:CHOPper:STATe
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timegateChopper">
        /// This control enables or disables the chopper for Timegate mode.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_setChopperEnabled(int channel, bool timegateChopper)
        {
            int pInvokeResult = PInvoke.timegate_setChopperEnabled(this._handle, channel, System.Convert.ToUInt16(timegateChopper));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the state of the chopper in Timegate mode.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TGATe:CHOPper:STATe?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timegateChopper">
        /// This control returns the state of the chopper for Timegate mode.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int timegate_getChopperEnabled(int channel, out bool timegateChopper)
        {
            ushort timegateChopperAsUShort;
            int pInvokeResult = PInvoke.timegate_getChopperEnabled(this._handle, channel, out timegateChopperAsUShort);
            timegateChopper = System.Convert.ToBoolean(timegateChopperAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures the timegate (depends on trigger event) in which the sensor is doing statistic measurements.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:OFFSet:TIME
        /// SENSe:STATistics:TIME
        /// SENSe:STATistics:[EXCLude]:MID:OFFSet[:TIME]
        /// SENSe:STATistics:[EXCLude]:MID:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control sets the start after the trigger of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Valid Range:
        /// 
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <param name="time">
        /// This control sets the length of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Valid Range:
        /// 1.0E-6 to 0.3 s
        /// 
        /// Default Value: 0.01 s
        /// </param>
        /// <param name="midambleOffset">
        /// This control sets the midamble offset after timeslot start in seconds in the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <param name="midambleLength">
        /// This control sets the midamble length in seconds.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_confTimegate(int channel, double offset, double time, double midambleOffset, double midambleLength)
        {
            int pInvokeResult = PInvoke.stat_confTimegate(this._handle, channel, offset, time, midambleOffset, midambleLength);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the X-Axis of statistical measurement.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:RLEVel
        /// SENSe:STATistics:SCALE:X:RANGe
        /// SENSe:STATistics:SCALE:X:POINts
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="referenceLevel">
        /// This control sets the lower limit of the level range for the analysis result in both Statistics modes. This level can be assigned to the first pixel. The level assigned to the last pixel is equal to the level of the first pixel plus the level range.
        /// 
        /// Valid Range:
        /// -80.0 to 20.0 dB
        /// 
        /// Default Value: -30.0 dB
        /// </param>
        /// <param name="range">
        /// This control sets the width of the level range for the analysis result in both Statistics modes.
        /// 
        /// Valid Range:
        /// 0.01 to 100.0
        /// 
        /// Default Value: 50.0
        /// </param>
        /// <param name="points">
        /// This control sets the measurement-result resolution in both Statistics modes. This function specifies the number of pixels that are to be assigned to the logarithmic level range (rsnrpz_stat_setScaleRange function) for measured value output. The width of the level range divided by N-1, where N is the number of pixels, must not be less than the value which can be read out with rsnrpz_stat_getScaleWidth.
        /// 
        /// Valid Range:
        /// 3 to 8192
        /// 
        /// Default Value: 200
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_confScale(int channel, double referenceLevel, double range, int points)
        {
            int pInvokeResult = PInvoke.stat_confScale(this._handle, channel, referenceLevel, range, points);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the start after the trigger of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:OFFSet:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control sets the start after the trigger of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Valid Range:
        /// 
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_setOffsetTime(int channel, double offset)
        {
            int pInvokeResult = PInvoke.stat_setOffsetTime(this._handle, channel, offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the start after the trigger of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:OFFSet:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control returns the start after the trigger of the timegate in which the sensor is doing statistic measurements.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getOffsetTime(int channel, out double offset)
        {
            int pInvokeResult = PInvoke.stat_getOffsetTime(this._handle, channel, out offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the length of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="time">
        /// This control sets the length of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Valid Range:
        /// 1.0E-6 to 0.3 s
        /// 
        /// Default Value: 0.01 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_setTime(int channel, double time)
        {
            int pInvokeResult = PInvoke.stat_setTime(this._handle, channel, time);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the length of the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="time">
        /// This control returns the length of the timegate in which the sensor is doing statistic measurements.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getTime(int channel, out double time)
        {
            int pInvokeResult = PInvoke.stat_getTime(this._handle, channel, out time);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the midamble offset after timeslot start in seconds in the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:[EXCLude]:MID:OFFSet[:TIME]
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control sets the midamble offset after timeslot start in seconds in the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_setMidOffset(int channel, double offset)
        {
            int pInvokeResult = PInvoke.stat_setMidOffset(this._handle, channel, offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the midamble offset after timeslot start in seconds in the timegate in which the sensor is doing statistic measurements.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:[EXCLude]:MID:OFFSet[:TIME]?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control returns the midamble offset after timeslot start in seconds in the timegate in which the sensor is doing statistic measurements.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getMidOffset(int channel, out double offset)
        {
            int pInvokeResult = PInvoke.stat_getMidOffset(this._handle, channel, out offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the midamble length in seconds.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:[EXCLude]:MID:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="length">
        /// This control sets the midamble length in seconds.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_setMidLength(int channel, double length)
        {
            int pInvokeResult = PInvoke.stat_setMidLength(this._handle, channel, length);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the midamble length in seconds.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:[EXCLude]:MID:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="length">
        /// This control returns the midamble length in seconds.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getMidLength(int channel, out double length)
        {
            int pInvokeResult = PInvoke.stat_getMidLength(this._handle, channel, out length);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the lower limit of the level range for the analysis result in both Statistics modes. This level can be assigned to the first pixel. The level assigned to the last pixel is equal to the level of the first pixel plus the level range.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:RLEVel
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="referenceLevel">
        /// This control sets the lower limit of the level range for the analysis result in both Statistics modes. This level can be assigned to the first pixel. The level assigned to the last pixel is equal to the level of the first pixel plus the level range.
        /// 
        /// Valid Range:
        /// -80.0 to 20.0 dB
        /// 
        /// Default Value: -30.0 dB
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_setScaleRefLevel(int channel, double referenceLevel)
        {
            int pInvokeResult = PInvoke.stat_setScaleRefLevel(this._handle, channel, referenceLevel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the lower limit of the level range for the analysis result in both Statistics modes. This level can be assigned to the first pixel. The level assigned to the last pixel is equal to the level of the first pixel plus the level range.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:RLEVel?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="referenceLevel">
        /// This control returns the lower limit of the level range for the analysis result in both Statistics modes. This level can be assigned to the first pixel. The level assigned to the last pixel is equal to the level of the first pixel plus the level range.
        /// 
        /// Valid Range:
        /// -80.0 to 20.0 dBm
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getScaleRefLevel(int channel, out double referenceLevel)
        {
            int pInvokeResult = PInvoke.stat_getScaleRefLevel(this._handle, channel, out referenceLevel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the width of the level range for the analysis result in both Statistics modes.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:RANGe
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="range">
        /// This control sets the width of the level range for the analysis result in both Statistics modes.
        /// 
        /// Valid Range:
        /// 0.01 to 100.0
        /// 
        /// Default Value: 50.0
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_setScaleRange(int channel, double range)
        {
            int pInvokeResult = PInvoke.stat_setScaleRange(this._handle, channel, range);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the width of the level range for the analysis result in both Statistics modes.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:RANGe?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="range">
        /// This control returns the width of the level range for the analysis result in both Statistics modes.
        /// 
        /// Valid Range:
        /// 0.01 to 100
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getScaleRange(int channel, out double range)
        {
            int pInvokeResult = PInvoke.stat_getScaleRange(this._handle, channel, out range);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the measurement-result resolution in both Statistics modes. This function specifies the number of pixels that are to be assigned to the logarithmic level range (rsnrpz_stat_setScaleRange function) for measured value output. The width of the level range divided by N-1, where N is the number of pixels, must not be less than the value which can be read out with rsnrpz_stat_getScaleWidth.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:POINts
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="points">
        /// This control the measurement-result resolution in both Statistics modes.
        /// 
        /// Valid Range:
        /// 3 to 8192
        /// 
        /// Default Value: 200
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_setScalePoints(int channel, int points)
        {
            int pInvokeResult = PInvoke.stat_setScalePoints(this._handle, channel, points);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the measurement-result resolution in both Statistics modes.
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:POINts?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="points">
        /// This control returns the measurement-result resolution in both Statistics modes.
        /// 
        /// Valid Range:
        /// 3 to 8192
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getScalePoints(int channel, out int points)
        {
            int pInvokeResult = PInvoke.stat_getScalePoints(this._handle, channel, out points);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the greatest attainable level resolution. For the R&amp;S NRP-Z81 power sensor, this value is fixed at 0.006 dB per pixel. If this value is exceeded, a "Settings conflict" message is output. The reason for the conflict may be that the number of pixels that has been selected is too great or that the width chosen for the level range is too small (rsnrpz_stat_setScalePoints and rsnrpz_stat_setScaleRange functions).
        /// 
        /// Remote-control command(s):
        /// SENSe:STATistics:SCALE:X:MPWidth?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="width">
        /// This control returns the greatest attainable level resolution.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int stat_getScaleWidth(int channel, out double width)
        {
            int pInvokeResult = PInvoke.stat_getScaleWidth(this._handle, channel, out width);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures the parameters of Timeslot measurement mode. Both exclude start and stop are set to 10% of timeslot width each.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:TSL:AVG:COUN
        /// SENS:POW:TSL:AVG:WIDT
        /// SENS:TIM:EXCL:STAR
        /// SENS:TIM:EXCL:STOP
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeSlotCount">
        /// This control sets the number of simultaneously measured timeslots in the Timeslot mode.
        /// 
        /// Valid Range:
        /// 1 - 128
        /// 
        /// Default Value:
        /// 8
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="width">
        /// This control sets the length in seconds of the timeslot in the Timeslot mode.
        /// 
        /// Valid Range:
        /// 10.0e-6 - 0.1
        /// 
        /// Default Value: 1.0e-3 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_configureTimeSlot(int channel, int timeSlotCount, double width)
        {
            int pInvokeResult = PInvoke.tslot_configureTimeSlot(this._handle, channel, timeSlotCount, width);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the number of simultaneously measured timeslots in the Timeslot mode.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:TSL:AVG:COUN
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeSlotCount">
        /// This control sets the number of simultaneously measured timeslots in the Timeslot mode.
        /// 
        /// Valid Range:
        /// 1 - 128
        /// 
        /// Default Value:
        /// 8
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_setTimeSlotCount(int channel, int timeSlotCount)
        {
            int pInvokeResult = PInvoke.tslot_setTimeSlotCount(this._handle, channel, timeSlotCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the number of simultaneously measured timeslots in the Timeslot mode.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:TSL:AVG:COUN?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeSlotCount">
        /// This control returns the number of simultaneously measured timeslots in the Timeslot mode.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_getTimeSlotCount(int channel, out int timeSlotCount)
        {
            int pInvokeResult = PInvoke.tslot_getTimeSlotCount(this._handle, channel, out timeSlotCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the length of the timeslot in the Timeslot mode.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:TSL:AVG:WIDT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="width">
        /// This control sets the length in seconds of the timeslot in the Timeslot mode.
        /// 
        /// Valid Range:
        /// 10.0e-6 - 0.1
        /// 
        /// Default Value: 1.0e-3 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_setTimeSlotWidth(int channel, double width)
        {
            int pInvokeResult = PInvoke.tslot_setTimeSlotWidth(this._handle, channel, width);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the length of the timeslot in the Timeslot mode.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:POW:TSL:AVG:WIDT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="width">
        /// This control returns the length in seconds of the timeslot in the Timeslot mode.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_getTimeSlotWidth(int channel, out double width)
        {
            int pInvokeResult = PInvoke.tslot_getTimeSlotWidth(this._handle, channel, out width);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the start of an exclusion interval in a timeslot. In conjunction with the function rsnrpz_tslot_setTimeSlotMidLength, it is possible to exclude, for example, a midamble from the measurement. The start of the timeslot is used as the reference point for defining the start of the exclusion interval and this applies to each of the timeslots
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TSLot[:AVG]:MID:OFFSet
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control sets sets the start of an exclusion interval in a timeslot.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_setTimeSlotMidOffset(int channel, double offset)
        {
            int pInvokeResult = PInvoke.tslot_setTimeSlotMidOffset(this._handle, channel, offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the start of an exclusion interval in a timeslot.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TSLot[:AVG]:MID:OFFSet?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offset">
        /// This control returns sets the start of an exclusion interval in a timeslot.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_getTimeSlotMidOffset(int channel, out double offset)
        {
            int pInvokeResult = PInvoke.tslot_getTimeSlotMidOffset(this._handle, channel, out offset);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the length of an exclusion interval in a timeslot. In conjunction with the function rsnrpz_tslot_setTimeSlotMidOffset, it can be used, for example, to exclude a midamble from the measurement.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TSLot[:AVG]:MID:TIME
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="length">
        /// This control sets the length of an exclusion interval in a timeslot.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1
        /// 
        /// Default Value: 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_setTimeSlotMidLength(int channel, double length)
        {
            int pInvokeResult = PInvoke.tslot_setTimeSlotMidLength(this._handle, channel, length);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the length of an exclusion interval in a timeslot.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TSLot[:AVG]:MID:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="length">
        /// This control returns the length of an exclusion interval in a timeslot.
        /// 
        /// Valid Range:
        /// 0.0 to 0.1 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_getTimeSlotMidLength(int channel, out double length)
        {
            int pInvokeResult = PInvoke.tslot_getTimeSlotMidLength(this._handle, channel, out length);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function enables or disables the chopper in Time Slot mode. Disabling the chopper is only good for fast but unexact and noisy measurements. If the chopper is disabled, averaging is ignored internally also disabled.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TSLot[:AVG]:CHOPper:STATe
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeSlotChopper">
        /// This control enables or disables the chopper for Time Slot mode.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_setTimeSlotChopperEnabled(int channel, bool timeSlotChopper)
        {
            int pInvokeResult = PInvoke.tslot_setTimeSlotChopperEnabled(this._handle, channel, System.Convert.ToUInt16(timeSlotChopper));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the state of the chopper in Time Slot mode.
        /// 
        /// Remote-control command(s):
        /// SENSe:POWer:TSLot[:AVG]:CHOPper:STATe?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeSlotChopper">
        /// This control returns the state of the chopper for Time Slot mode.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int tslot_getTimeSlotChopperEnabled(int channel, out bool timeSlotChopper)
        {
            ushort timeSlotChopperAsUShort;
            int pInvokeResult = PInvoke.tslot_getTimeSlotChopperEnabled(this._handle, channel, out timeSlotChopperAsUShort);
            timeSlotChopper = System.Convert.ToBoolean(timeSlotChopperAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets parameters of the Scope mode.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:POIN
        /// SENS:TRAC:TIME
        /// SENS:TRAC:OFFS:TIME
        /// SENS:TRAC:REAL ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="scopePoints">
        /// This control sets the number of desired values per Scope sequence.
        /// 
        /// Valid Range:
        /// 1 to 1024
        /// 
        /// Default Value: 312
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="scopeTime">
        /// This control sets the value of scope time.
        /// 
        /// Valid Range:
        /// 0.1e-3 to 0.3 s
        /// 
        /// Default Value: 0.01 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="offsetTime">
        /// This control sets the value of offset time.
        /// 
        /// Valid Range:
        /// 
        /// -5.0e-3 to 100.0 s
        /// 
        /// Default Value: 0.0 s
        /// 
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="realtime">
        /// This control sets the state of real-time measurement.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_configureScope(int channel, int scopePoints, double scopeTime, double offsetTime, bool realtime)
        {
            int pInvokeResult = PInvoke.scope_configureScope(this._handle, channel, scopePoints, scopeTime, offsetTime, System.Convert.ToUInt16(realtime));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function performs fast zeroing, but can be called only in the sensor's Trace mode and Statistics modes. In any other measurement mode, the error message NRPERROR_CALZERO is output. Even though the execution time is shorter than that for standard zeroing by a factor of 100 or more, measurement accuracy is not affected. Fast zeroing is available for the entire frequency range.
        /// 
        /// Remote-control command(s):
        /// CAL:ZERO:FAST:AUTO
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_fastZero()
        {
            int pInvokeResult = PInvoke.scope_fastZero(this._handle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// For the Scope mode, this function switches the filter function of a sensor on or off. When the filter is switched on, the number of measured values set with SENS:TRAC:AVER:COUN (function rsnrpz_scope_setAverageCount) is averaged. This reduces the effect of noise so that more reliable results are obtained.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="scopeAveraging">
        /// This control switches the filter function of a sensor on or off.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAverageEnabled(int channel, bool scopeAveraging)
        {
            int pInvokeResult = PInvoke.scope_setAverageEnabled(this._handle, channel, System.Convert.ToUInt16(scopeAveraging));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the state of filter function of a sensor.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="scopeAveraging">
        /// This control returns the state of filter function of a sensor.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getAverageEnabled(int channel, out bool scopeAveraging)
        {
            ushort scopeAveragingAsUShort;
            int pInvokeResult = PInvoke.scope_getAverageEnabled(this._handle, channel, out scopeAveragingAsUShort);
            scopeAveraging = System.Convert.ToBoolean(scopeAveragingAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the length of the filter for the Scope mode. The wider the filter the lower the noise and the longer it takes to obtain a measured value.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="count">
        /// This control sets the length of the filter for the Scope mode.
        /// 
        /// Valid Range:
        /// 1 to 65536
        /// 
        /// Default Value: 4
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAverageCount(int channel, int count)
        {
            int pInvokeResult = PInvoke.scope_setAverageCount(this._handle, channel, count);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the length of the filter for the Scope mode.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN?
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="count">
        /// This control returns the averaging filter length in Scope mode.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getAverageCount(int channel, out int count)
        {
            int pInvokeResult = PInvoke.scope_getAverageCount(this._handle, channel, out count);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// As soon as a new single value is determined, the filter window is advanced by one value so that the new value is taken into account by the filter and the oldest value is forgotten.
        /// Terminal control then determines in the Scope mode whether a new result will be calculated immediately after a new measured value is available or only after an entire range of new values is available for the filter.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:TCON MOV | REP
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="terminalControl">
        /// This control determines the type of terminal control.
        /// 
        /// Valid Values:
        ///  RSNRPZ_TERMINAL_CONTROL_MOVING - Moving
        ///  RSNRPZ_TERMINAL_CONTROL_REPEAT - Repeat (Default Value)
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAverageTerminalControl(int channel, int terminalControl)
        {
            int pInvokeResult = PInvoke.scope_setAverageTerminalControl(this._handle, channel, terminalControl);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns selected terminal control type in the Scope mode.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:TCON?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="terminalControl">
        /// This control returns the type of terminal control.
        /// 
        /// Valid Values:
        /// Moving (RSNRPZ_TERMINAL_CONTROL_MOVING)
        /// Repeat (RSNRPZ_TERMINAL_CONTROL_REPEAT)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getAverageTerminalControl(int channel, out int terminalControl)
        {
            int pInvokeResult = PInvoke.scope_getAverageTerminalControl(this._handle, channel, out terminalControl);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function determines the relative position of the trigger event in relation to the beginning of the Scope measurement sequence.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:OFFS:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offsetTime">
        /// This control sets the value of offset time.
        /// 
        /// Valid Range:
        /// 
        /// -5.0e-3 to 100.0 s
        /// 
        /// Default Value: 0.0 s
        /// 
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setOffsetTime(int channel, double offsetTime)
        {
            int pInvokeResult = PInvoke.scope_setOffsetTime(this._handle, channel, offsetTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the relative position of the trigger event in relation to the beginning of the Scope measurement sequence.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:OFFS:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="offsetTime">
        /// This control returns the value of offset time in seconds.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getOffsetTime(int channel, out double offsetTime)
        {
            int pInvokeResult = PInvoke.scope_getOffsetTime(this._handle, channel, out offsetTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the number of desired values per Scope sequence.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:POIN
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="scopePoints">
        /// This control sets the number of desired values per Scope sequence.
        /// 
        /// Valid Range:
        /// 1 to 1024
        /// 
        /// Default Value: 312
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setPoints(int channel, int scopePoints)
        {
            int pInvokeResult = PInvoke.scope_setPoints(this._handle, channel, scopePoints);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the number of desired values per Scope sequence.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:POIN?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="scopePoints">
        /// This control returns the number of desired values per Scope sequence.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getPoints(int channel, out int scopePoints)
        {
            int pInvokeResult = PInvoke.scope_getPoints(this._handle, channel, out scopePoints);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// In the default state (OFF), each measurement sequence from the sensor is averaged over several sequences. Since the measured values of a sequence may be closer to each other in time than the measurements, several measurement sequences with a slight time offset are also superimposed on the desired sequence. With the state turned ON - this effect can be switched off, which may increase the measurement speed. This ensures that the measured values of an individual sequence are immediately delivered.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:REAL ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="realtime">
        /// This control sets the state of real-time measurement.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On 
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setRealtimeEnabled(int channel, bool realtime)
        {
            int pInvokeResult = PInvoke.scope_setRealtimeEnabled(this._handle, channel, System.Convert.ToUInt16(realtime));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the state of real-time measurement setting.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:REAL?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="realtime">
        /// This control returns the state of real-time measurement.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - On (Default Value)
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getRealtimeEnabled(int channel, out bool realtime)
        {
            ushort realtimeAsUShort;
            int pInvokeResult = PInvoke.scope_getRealtimeEnabled(this._handle, channel, out realtimeAsUShort);
            realtime = System.Convert.ToBoolean(realtimeAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the time to be covered by the Scope sequence.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:TIME
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="scopeTime">
        /// This control sets the value of scope time.
        /// 
        /// Valid Range:
        /// 0.1e-3 to 0.3 s
        /// 
        /// Default Value: 0.01 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setTime(int channel, double scopeTime)
        {
            int pInvokeResult = PInvoke.scope_setTime(this._handle, channel, scopeTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the value of the time to be covered by the Scope sequence.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:TIME?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="scopeTime">
        /// This control returns the value of scope time in seconds.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getTime(int channel, out double scopeTime)
        {
            int pInvokeResult = PInvoke.scope_getTime(this._handle, channel, out scopeTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function can be used to automatically determine a value for filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO ON|OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoEnabled">
        /// This control activates or deactivates automatic determination of a value for filter bandwidth.
        /// If the automatic switchover is activated with the ON parameter, the sensor always defines a suitable filter length.
        /// 
        /// Valid Values:
        /// VI_FALSE (0) - Off
        /// VI_TRUE  (1) - On (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAutoEnabled(int channel, bool autoEnabled)
        {
            int pInvokeResult = PInvoke.scope_setAutoEnabled(this._handle, channel, System.Convert.ToUInt16(autoEnabled));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the setting of automatic switchover of filter bandwidth.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoEnabled">
        /// This control returns the setting of automatic switchover of filter bandwidth.
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getAutoEnabled(int channel, out bool autoEnabled)
        {
            ushort autoEnabledAsUShort;
            int pInvokeResult = PInvoke.scope_getAutoEnabled(this._handle, channel, out autoEnabledAsUShort);
            autoEnabled = System.Convert.ToBoolean(autoEnabledAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        ///  This function sets an upper time limit can be set via (maximum time). It should never be exceeded. Undesired long measurement times can thus be prevented if the automatic filter length switchover is on.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO:MTIM
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="upperTimeLimit">
        /// This control sets the upper time limit (maximum time to fill the filter).
        /// 
        /// Valid Range:
        /// 
        /// NRP-21: 0.01 - 999.99
        /// FSH-Z1: 0.01 - 999.99
        /// 
        /// Default Value: 4.0
        /// 
        /// Notes:
        /// (1) This value is not range checked, the actual range depends on sensor used.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAutoMaxMeasuringTime(int channel, double upperTimeLimit)
        {
            int pInvokeResult = PInvoke.scope_setAutoMaxMeasuringTime(this._handle, channel, upperTimeLimit);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries an upper time limit (maximum time to fill the filter).
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO:MTIM
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="upperTimeLimit">
        /// This control returns an upper time limit (maximum time to fill the filter).
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getAutoMaxMeasuringTime(int channel, out double upperTimeLimit)
        {
            int pInvokeResult = PInvoke.scope_getAutoMaxMeasuringTime(this._handle, channel, out upperTimeLimit);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the maximum noise ratio in the measurement result.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO:NSR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumNoiseRatio">
        /// This control sets the maximum noise ratio in the measurement result. The value is set in dB.
        /// 
        /// Valid Range:
        /// 
        /// NRP-Z21: 0.0 - 1.0
        /// FSH-Z1:  0.0 - 1.0
        /// 
        /// Default Value: 0.1
        /// 
        /// Notes:
        /// (1) This value is not range checked; the actual range depends on sensor used.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAutoNoiseSignalRatio(int channel, double maximumNoiseRatio)
        {
            int pInvokeResult = PInvoke.scope_setAutoNoiseSignalRatio(this._handle, channel, maximumNoiseRatio);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the maximum noise signal ratio value.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO:NSR?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumNoiseRatio">
        /// This control returns a maximum noise signal ratio in dB.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getAutoNoiseSignalRatio(int channel, out double maximumNoiseRatio)
        {
            int pInvokeResult = PInvoke.scope_getAutoNoiseSignalRatio(this._handle, channel, out maximumNoiseRatio);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result. This setting does not affect the setting of display.
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:RES 1 ... 4
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="resolution">
        /// This control defines the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result.
        /// 
        /// Valid Range:
        /// 1 to 4
        /// 
        /// Default Value: 3
        /// 
        /// Notes:
        /// (1) Actual range depend on sensor used and may vary from the range stated above.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAutoResolution(int channel, int resolution)
        {
            int pInvokeResult = PInvoke.scope_setAutoResolution(this._handle, channel, resolution);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result. 
        /// 
        /// Remote-control command(s):
        /// SENS:AVER:COUN:AUTO:RES?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="resolution">
        /// This control returns the number of significant places for linear units and the number of decimal places for logarithmic units which should be free of noise in the measurement result.
        /// 
        /// Valid Range:
        /// 1 to 4
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// 
        /// </returns>
        public int scope_getAutoResolution(int channel, out int resolution)
        {
            int pInvokeResult = PInvoke.scope_getAutoResolution(this._handle, channel, out resolution);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function selects a method by which the automatic filter length switchover can operate.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO:TYPE RES | NSR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="method">
        /// This control selects a method by which the automatic filter length switchover can operate.
        /// 
        /// Valid Values:
        /// RSNRPZ_AUTO_COUNT_TYPE_RESOLUTION (0) - Resolution (Default Value)
        /// RSNRPZ_AUTO_COUNT_TYPE_NSR (1) - Noise Ratio
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_setAutoType(int channel, int method)
        {
            int pInvokeResult = PInvoke.scope_setAutoType(this._handle, channel, method);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns a method by which the automatic filter length switchover can operate.
        /// 
        /// Remote-control command(s):
        /// SENS:TRAC:AVER:COUN:AUTO:TYPE?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="method">
        /// This control returns a method by which the automatic filter length switchover can operate.
        /// 
        /// Valid Values:
        /// Resolution (RSNRPZ_AUTO_COUNT_TYPE_RESOLUTION)
        /// Noise Ratio (RSNRPZ_AUTO_COUNT_TYPE_NSR)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int scope_getAutoType(int channel, out int method)
        {
            int pInvokeResult = PInvoke.scope_getAutoType(this._handle, channel, out method);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures the parameters of internal trigger system.
        /// 
        /// Remote-control command(s):
        /// TRIG:DEL:AUTO ON
        /// TRIG:ATR OFF
        /// TRIG:COUN 1
        /// TRIG:DEL 0.0
        /// TRIG:HOLD 0.0
        /// TRIG:HYST 3DB
        /// TRIG:LEV &lt;value&gt;
        /// TRIG:SLOP POS | NEG
        /// TRIG:SOUR INT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerLevel">
        /// This control determines the power (in W) a trigger signal must exceed before a trigger event is detected.
        /// 
        /// Valid Range:
        /// 0.1e-6 to 0.2 W
        /// 
        /// Default Value:
        /// 1.0e-6 W
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <param name="triggerSlope">
        /// This control determines whether the rising (POSitive) or the falling (NEGative) edge of the signal is used for triggering.
        /// 
        /// Valid Values:
        /// RSNRPZ_SLOPE_POSITIVE (0) - Positive (Default Value)
        /// RSNRPZ_SLOPE_NEGATIVE (1) - Negative
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_configureInternal(int channel, double triggerLevel, int triggerSlope)
        {
            int pInvokeResult = PInvoke.trigger_configureInternal(this._handle, channel, triggerLevel, triggerSlope);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function configures the parameters of external trigger system.
        /// 
        /// Remote-control command(s):
        /// TRIG:DEL &lt;value&gt;
        /// TRIG:SOUR EXT
        /// TRIG:COUN 1
        /// TRIG:ATR OFF
        /// TRIG:HOLD 0.0
        /// TRIG:DEL:AUTO ON
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerDelay">
        /// This control sets a the delay (in seconds) between the trigger event and the beginning of the actual measurement (integration).
        /// 
        /// Valid Range:
        /// -5.0e-3 to 100.0 s
        /// 
        /// Default Value:
        /// 0.0 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_configureExternal(int channel, double triggerDelay)
        {
            int pInvokeResult = PInvoke.trigger_configureExternal(this._handle, channel, triggerDelay);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function performs triggering and ensures that the sensor directly changes from the WAIT_FOR_TRG state to the MEASURING state irrespective of the selected trigger source. A trigger delay set with TRIG:DEL is ignored but not the automatic delay determined when TRIG:DEL:AUTO:ON is set.
        /// When the trigger source is HOLD, a measurement can only be started with TRIG.
        /// 
        /// Remote-control command(s):
        /// TRIG:IMM
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_immediate(int channel)
        {
            int pInvokeResult = PInvoke.trigger_immediate(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function ensures (if parameter is set to On) by means of an automatically determined delay that a measurement is started only after the sensor has settled. This is important when thermal sensors are used.
        /// 
        /// Remote-control command(s):
        /// TRIG:DEL:AUTO ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoDelay">
        /// This control enables or disables automatic determination of delay.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setAutoDelayEnabled(int channel, bool autoDelay)
        {
            int pInvokeResult = PInvoke.trigger_setAutoDelayEnabled(this._handle, channel, System.Convert.ToUInt16(autoDelay));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the setting of auto delay feature.
        /// 
        /// Remote-control command(s):
        /// TRIG:DEL:AUTO?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoDelay">
        /// This control returns the state of Auto Delay feature.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getAutoDelayEnabled(int channel, out bool autoDelay)
        {
            ushort autoDelayAsUShort;
            int pInvokeResult = PInvoke.trigger_getAutoDelayEnabled(this._handle, channel, out autoDelayAsUShort);
            autoDelay = System.Convert.ToBoolean(autoDelayAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function turns On or Off the auto trigger feature. When auto trigger is set to On, the WAIT_FOR_TRG state is automatically exited when no trigger event occurs within a period that corresponds to the reciprocal of the display update rate.
        /// 
        /// Note:
        ///   
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// TRIG:ATR:STAT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoTrigger">
        /// This control enables or disables the Auto Trigger.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setAutoTriggerEnabled(int channel, bool autoTrigger)
        {
            int pInvokeResult = PInvoke.trigger_setAutoTriggerEnabled(this._handle, channel, System.Convert.ToUInt16(autoTrigger));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the state of Auto Trigger.
        /// 
        /// Note:
        ///   
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// TRIG:ATR:STAT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="autoTrigger">
        /// This control returns the state of Auto Trigger.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getAutoTriggerEnabled(int channel, out bool autoTrigger)
        {
            ushort autoTriggerAsUShort;
            int pInvokeResult = PInvoke.trigger_getAutoTriggerEnabled(this._handle, channel, out autoTriggerAsUShort);
            autoTrigger = System.Convert.ToBoolean(autoTriggerAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the number of measurement cycles to be  performed when the measurement is started with INIT.
        /// 
        /// Remote-control command(s):
        /// TRIG:COUN
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerCount">
        /// This control sets the number of measurement cycles to be  performed when the measurement is started with INIT.
        /// 
        /// Valid Range:
        /// 1 to 2147483646
        /// 
        /// Default Value:
        /// 1
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setCount(int channel, int triggerCount)
        {
            int pInvokeResult = PInvoke.trigger_setCount(this._handle, channel, triggerCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the number of measurement cycles to be  performed when the measurement is started with INIT.
        /// 
        /// Remote-control command(s):
        /// TRIG:COUN?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerCount">
        /// This control returns the number of measurement cycles to be  performed when the measurement is started with INIT.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getCount(int channel, out int triggerCount)
        {
            int pInvokeResult = PInvoke.trigger_getCount(this._handle, channel, out triggerCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines the delay between the trigger event and the beginning of the actual measurement (integration).
        /// 
        /// Remote-control command(s):
        /// TRIG:DEL
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerDelay">
        /// This control sets a the delay (in seconds) between the trigger event and the beginning of the actual measurement (integration).
        /// 
        /// Valid Range:
        /// NRP-Z21: -5.0e-3 to 100.0 s
        /// NRP-Z51:  0.0    to 100.0 s
        /// FSH-Z1:  -5.0e-3 to 100.0 s
        /// 
        /// Default Value:
        /// 0.0 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setDelay(int channel, double triggerDelay)
        {
            int pInvokeResult = PInvoke.trigger_setDelay(this._handle, channel, triggerDelay);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads value of the delay (in seconds) between the trigger event and the beginning of the actual measurement (integration).
        /// 
        /// Remote-control command(s):
        /// TRIG:DEL?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerDelay">
        /// This control returns value of the delay (in seconds) between the trigger event and the beginning of the actual measurement (integration).
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getDelay(int channel, out double triggerDelay)
        {
            int pInvokeResult = PInvoke.trigger_getDelay(this._handle, channel, out triggerDelay);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines a period after a trigger event within which all further trigger events are ignored.
        /// 
        /// Remote-control command(s):
        /// TRIG:HOLD
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerHoldoff">
        /// This control defines a period (in seconds) after a trigger event within which all further trigger events are ignored.
        /// 
        /// Valid Range:
        /// 0.0 - 10.0 s
        /// 
        /// Default Value:
        /// 0.0 s
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setHoldoff(int channel, double triggerHoldoff)
        {
            int pInvokeResult = PInvoke.trigger_setHoldoff(this._handle, channel, triggerHoldoff);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the value of a period after a trigger event within which all further trigger events are ignored.
        /// 
        /// Remote-control command(s):
        /// TRIGger[1..4]:HOLDoff?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerHoldoff">
        /// This control returns the value of a period (in seconds) after a trigger event within which all further trigger events are ignored.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getHoldoff(int channel, out double triggerHoldoff)
        {
            int pInvokeResult = PInvoke.trigger_getHoldoff(this._handle, channel, out triggerHoldoff);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function is used to specify how far the signal level has to drop below the trigger level before a new signal edge can be detected as a trigger event. Thus, this command can be used to eliminate the effects of noise in the signal on the transition filters of the trigger system.
        /// 
        /// Remote-control command(s):
        /// TRIG:HYST
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerHysteresis">
        /// This control defines the trigger hysteresis in dB.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 dB
        /// 
        /// Default Value: 0.0 dB
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setHysteresis(int channel, double triggerHysteresis)
        {
            int pInvokeResult = PInvoke.trigger_setHysteresis(this._handle, channel, triggerHysteresis);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the value of trigger hysteresis.
        /// 
        /// Remote-control command(s):
        /// TRIG:HYST?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerHysteresis">
        /// This control returns the value of trigger hysteresis in dB.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getHysteresis(int channel, out double triggerHysteresis)
        {
            int pInvokeResult = PInvoke.trigger_getHysteresis(this._handle, channel, out triggerHysteresis);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function determines the power a trigger signal must exceed before a trigger event is detected. This setting is only used for internal trigger signal source.
        /// 
        /// Remote-control command(s):
        /// TRIG:LEV
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerLevel">
        /// This control determines the power (in W) a trigger signal must exceed before a trigger event is detected.
        /// 
        /// Valid Range:
        /// NRP-Z21: 0.1e-6  to 0.2 W
        /// NRP-Z51: 0.25e-6 to 0.1 W
        /// FSH-Z1:  0.1e-6  to 0.2 W
        /// 
        /// Default Value:
        /// 1.0e-6 W
        /// 
        /// Notes:
        /// (1) Actual valid range depends on sensor used
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setLevel(int channel, double triggerLevel)
        {
            int pInvokeResult = PInvoke.trigger_setLevel(this._handle, channel, triggerLevel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads the power a trigger signal must exceed before a trigger event is detected.
        /// 
        /// Remote-control command(s):
        /// TRIG:LEV?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerLevel">
        /// This control returns the power (in W) a trigger signal must exceed before a trigger event is detected.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getLevel(int channel, out double triggerLevel)
        {
            int pInvokeResult = PInvoke.trigger_getLevel(this._handle, channel, out triggerLevel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function determines whether the rising (POSitive) or the falling (NEGative) edge of the signal is used for triggering.
        /// 
        /// Remote-control command(s):
        /// TRIG:SLOP POSitive | NEGative
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerSlope">
        /// This control determines whether the rising (POSitive) or the falling (NEGative) edge of the signal is used for triggering.
        /// 
        /// Valid Values:
        /// RSNRPZ_SLOPE_POSITIVE (0) - Positive (Default Value)
        /// RSNRPZ_SLOPE_NEGATIVE (1) - Negative
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setSlope(int channel, int triggerSlope)
        {
            int pInvokeResult = PInvoke.trigger_setSlope(this._handle, channel, triggerSlope);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads whether the rising (POSitive) or the falling (NEGative) edge of the signal is used for triggering.
        /// 
        /// Remote-control command(s):
        /// TRIG:SLOP?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerSlope">
        /// This control returns whether the rising (POSitive) or the falling (NEGative) edge of the signal is used for triggering.
        /// 
        /// Valid Values:
        /// RSNRPZ_SLOPE_POSITIVE (0) - Positive
        /// RSNRPZ_SLOPE_NEGATIVE (1) - Negative
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getSlope(int channel, out int triggerSlope)
        {
            int pInvokeResult = PInvoke.trigger_getSlope(this._handle, channel, out triggerSlope);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the trigger signal source for the WAIT_FOR_TRG state.
        /// 
        /// Remote-control command(s):
        /// TRIG:SOUR BUS | EXT | HOLD | IMM | INT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerSource">
        /// This control selects the trigger signal source for the WAIT_FOR_TRG state.
        /// 
        /// Valid Values:
        /// RSNRPZ_TRIGGER_SOURCE_BUS       (0) - Bus 
        /// RSNRPZ_TRIGGER_SOURCE_EXTERNAL  (1) - External
        /// RSNRPZ_TRIGGER_SOURCE_HOLD      (2) - Hold
        /// RSNRPZ_TRIGGER_SOURCE_IMMEDIATE (3) - Immediate (Default Value)
        /// RSNRPZ_TRIGGER_SOURCE_INTERNAL  (4) - Internal
        /// 
        /// Notes:
        /// (1) Bus: The trigger event is initiated by TRIG:IMM or *TRG. In this case, the setting for Trigger Slope is meaningless.
        /// 
        /// (2) External: Triggering is performed with an external signal applied to the trigger connector. The Trigger Slope setting determines whether the rising or the falling edge of the signal is to be used for triggering. Waiting for a trigger event can be skipped by Immediate Trigger.
        /// 
        /// (3) Hold: A measurement can only be triggered when Immediate Trigger is executed.
        /// 
        /// (4) Immediate: The sensor does not remain in the WAIT_FOR_TRG state but immediately changes to the MEASURING state.
        /// 
        /// (5) Internal: The sensor determines the trigger time by means of the signal to be measured. When this signal exceeds the power set by Trigger Level, the measurement is started after the time set by Trigger Delay. Similar to External Trigger, waiting for a trigger event can also be skipped by Immediate Trigger.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setSource(int channel, int triggerSource)
        {
            int pInvokeResult = PInvoke.trigger_setSource(this._handle, channel, triggerSource);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets the trigger signal source for the WAIT_FOR_TRG state.
        /// 
        /// Remote-control command(s):
        /// TRIG:SOUR?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="triggerSource">
        /// This control returns the trigger signal source for the WAIT_FOR_TRG state.
        /// 
        /// Valid Values:
        /// RSNRPZ_TRIGGER_SOURCE_BUS (0) - Bus
        /// RSNRPZ_TRIGGER_SOURCE_EXTERNAL (1) - External
        /// RSNRPZ_TRIGGER_SOURCE_HOLD (2) - Hold
        /// RSNRPZ_TRIGGER_SOURCE_IMMEDIATE (3) - Immediate
        /// RSNRPZ_TRIGGER_SOURCE_INTERNAL (4) - Internal
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getSource(int channel, out int triggerSource)
        {
            int pInvokeResult = PInvoke.trigger_getSource(this._handle, channel, out triggerSource);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function defines the dropout time value. With a positive (negative) trigger slope, the dropout time is the minimum time for which the signal must be below (above) the power level defined by rsnrpz_trigger_setLevel and rsnrpz_trigger_setHysteresis before triggering can occur again. As with the Holdoff parameter, unwanted trigger events can be excluded. The set dropout time only affects the internal trigger source.
        /// The dropout time parameter is useful when dealing with, for example, GSM signals with several active slots.
        /// 
        /// Remote-control command(s):
        /// TRIGger:DTIMe
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dropoutTime">
        /// This control defines the dropout time value.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value:
        /// 0.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_setDropoutTime(int channel, double dropoutTime)
        {
            int pInvokeResult = PInvoke.trigger_setDropoutTime(this._handle, channel, dropoutTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries the dropout time value.
        /// 
        /// Remote-control command(s):
        /// TRIGger:DTIMe?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="dropoutTime">
        /// This control returns the dropout time value.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int trigger_getDropoutTime(int channel, out double dropoutTime)
        {
            int pInvokeResult = PInvoke.trigger_getDropoutTime(this._handle, channel, out dropoutTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns selected information on a sensor.
        /// 
        /// Remote-control command(s):
        /// SYST:INFO? &lt;Info Type&gt;
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="infoType">
        /// This control specifies which info should be retrieved from the sensor.
        /// 
        /// Valid Values:
        ///  "Manufacturer"
        ///  "Type"
        ///  "Stock Number"
        ///  "Serial"
        ///  "HWVersion"
        ///  "HWVariant"
        ///  "SW Build"
        ///  "Technology"
        ///  "Function"
        ///  "MinPower"
        ///  "MaxPower" 
        ///  "MinFreq"
        ///  "MaxFreq"
        ///  "Resolution"
        ///  "Impedance"
        ///  "Coupling"
        ///  "Cal. Abs."
        ///  "Cal. Refl."
        ///  "Cal. S-Para."
        ///  "Cal. Misc."
        ///  "Cal. Temp."
        ///  "Cal. Lin."
        ///  "SPD Mnemonic"
        /// 
        /// Default Value:
        /// ""
        /// </param>
        /// <param name="arraySize">
        /// This control defines the size of array 'Info'.
        /// 
        /// Valid Range:
        /// -
        /// 
        /// Default Value:
        /// 100
        /// </param>
        /// <param name="info">
        /// This control returns requested informations from the sensor.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_info(int channel, string infoType, int arraySize, System.Text.StringBuilder info)
        {
            int pInvokeResult = PInvoke.chan_info(this._handle, channel, infoType, arraySize, info);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns specified parameter header which can be retrieved from selected sensor.
        /// 
        /// Remote-control command(s):
        /// SYST:INFO?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="parameterNumber">
        /// This control defines the position of parameter header to be retrieved.
        /// 
        /// Valid Range:
        /// 0 to (count of headers - 1)
        /// 
        /// Default Value:
        /// 0
        /// 
        /// Notes:
        /// (1) Only Minimum value of the parameter is checked. Maximum value depends on sensor used and can be retrieved by function rsnrpz_chan_infosCount().
        /// </param>
        /// <param name="arraySize">
        /// This control defines the size of array 'Header'.
        /// 
        /// Valid Range:
        /// -
        /// 
        /// Default Value:
        /// 100
        /// </param>
        /// <param name="header">
        /// This control returns specified parameter header.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_infoHeader(int channel, int parameterNumber, int arraySize, System.Text.StringBuilder header)
        {
            int pInvokeResult = PInvoke.chan_infoHeader(this._handle, channel, parameterNumber, arraySize, header);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the number of info headers for selected channel.
        /// 
        /// Remote-control command(s):
        /// SYST:INFO?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="count">
        /// This control returns the number of available info headers for selected sensor.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_infosCount(int channel, out int count)
        {
            int pInvokeResult = PInvoke.chan_infosCount(this._handle, channel, out count);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets status update time, which influences USB traffic during sensor's waiting for trigger.
        /// 
        /// Note:
        /// 
        /// 1) This function is available only for NRP-Z81.
        /// 
        /// Remote-control command(s):
        /// SYST:SUT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="statusUpdateTime">
        /// This control sets status update time, which influences USB traffic during sensor's waiting for trigger.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value: 100.0e-4 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int system_setStatusUpdateTime(int channel, double statusUpdateTime)
        {
            int pInvokeResult = PInvoke.system_setStatusUpdateTime(this._handle, channel, statusUpdateTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets status update time.
        /// 
        /// Note:
        /// 
        /// 1) This function is available only for NRP-Z81.
        /// 
        /// Remote-control command(s):
        /// SYST:SUT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="statusUpdateTime">
        /// This control returns status update time.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int system_getStatusUpdateTime(int channel, out double statusUpdateTime)
        {
            int pInvokeResult = PInvoke.system_getStatusUpdateTime(this._handle, channel, out statusUpdateTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets result update time, which influences USB traffic if sensor is in continuous sweep mode.
        /// 
        /// Note:
        /// 
        /// 1) This function is available only for NRP-Z81.
        /// 
        /// Remote-control command(s):
        /// SYST:RUT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="resultUpdateTime">
        /// This control sets result update time, which influences USB traffic if sensor is in continuous sweep mode.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// 
        /// Default Value: 0.1 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int system_setResultUpdateTime(int channel, double resultUpdateTime)
        {
            int pInvokeResult = PInvoke.system_setResultUpdateTime(this._handle, channel, resultUpdateTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function gets result update time.
        /// 
        /// Note:
        /// 
        /// 1) This function is available only for NRP-Z81.
        /// 
        /// Remote-control command(s):
        /// SYST:RUT
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="resultUpdateTime">
        /// This control gets result update time.
        /// 
        /// Valid Range:
        /// 0.0 to 10.0 s
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int system_getResultUpdateTime(int channel, out double resultUpdateTime)
        {
            int pInvokeResult = PInvoke.system_getResultUpdateTime(this._handle, channel, out resultUpdateTime);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function immediately sets selected sensor to the IDLE state. Measurements in progress are interrupted. If INIT:CONT ON is set, a new measurement is immediately started since the trigger system is not influenced.
        /// 
        /// Remote-control command(s):
        /// ABOR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_abort(int channel)
        {
            int pInvokeResult = PInvoke.chan_abort(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function starts a single-shot measurement on selected channel. The respective sensor goes to the INITIATED state. The command is completely executed when the sensor returns to the IDLE state. The command is ignored when the sensor is not in the IDLE state or when continuous measurements are selected (INIT:CONT ON). The command is only fully executed when the measurement is completed and the trigger system has again reached the IDLE state. INIT is the only remote control command that permits overlapping execution. Other commands can be received and processed while the command is being executed.
        /// 
        /// Remote-control command(s):
        /// STAT:OPER:MEAS?
        /// INITiate[1..4]
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_initiate(int channel)
        {
            int pInvokeResult = PInvoke.chan_initiate(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function selects either single-shot or continuous (free-running) measurement cycles.
        /// 
        /// Remote-control command(s):
        /// INIT:CONT ON | OFF
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="continuousInitiate">
        /// This control enables or disables the continuous measurement mode.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off (Default Value)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setInitContinuousEnabled(int channel, bool continuousInitiate)
        {
            int pInvokeResult = PInvoke.chan_setInitContinuousEnabled(this._handle, channel, System.Convert.ToUInt16(continuousInitiate));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns whether single-shot or continuous (free-running) measurement is selected.
        /// 
        /// Remote-control command(s):
        /// INIT:CONT?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="continuousInitiate">
        /// This control returns the state of continuous initiate.
        /// 
        /// Valid Values:
        /// VI_TRUE  (1) - On
        /// VI_FALSE (0) - Off
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getInitContinuousEnabled(int channel, out bool continuousInitiate)
        {
            ushort continuousInitiateAsUShort;
            int pInvokeResult = PInvoke.chan_getInitContinuousEnabled(this._handle, channel, out continuousInitiateAsUShort);
            continuousInitiate = System.Convert.ToBoolean(continuousInitiateAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// From the point of view of the R&amp;S NRP basic unit, the sensors are stand-alone measuring devices. They communicate with the R&amp;S NRP via a command set complying with SCPI.
        /// This function prompts the basic unit to send an *RST to the respective sensor. Measurements in progress are interrupted.
        /// 
        /// Remote-control command(s):
        /// SYSTem:SENSor[1..4]:RESet
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_reset(int channel)
        {
            int pInvokeResult = PInvoke.chan_reset(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// If the signal to be measured has modulation sections just above the video bandwidth of the sensor used, measurement errors might be caused due to aliasing effects. In this case, the sampling rate of the sensor can be set to a safe lower value (Sampling Frequency 2). However, the measurement time required to obtain noise-free results is extended compared to the normal sampling rate (Sampling Frequency 1).
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:SAMP FREQ1 | FREQ2
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="samplingFrequency">
        /// This control selects the sampling frequency.
        /// 
        /// Valid Values:
        /// RSNRPZ_SAMPLING_FREQUENCY1 (1) - Sampling Frq 1 (High) (Default Value)
        /// RSNRPZ_SAMPLING_FREQUENCY2 (2) - Sampling Frq 2 (Low)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setSamplingFrequency(int channel, int samplingFrequency)
        {
            int pInvokeResult = PInvoke.chan_setSamplingFrequency(this._handle, channel, samplingFrequency);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the selected sampling frequency.
        /// 
        /// Note:
        /// 
        /// 1) This function is not available for NRP-Z51.
        /// 
        /// Remote-control command(s):
        /// SENS:SAMP?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="samplingFrequency">
        /// This control returns the selected sampling frequency.
        /// 
        /// Valid Values:
        /// RSNRPZ_SAMPLING_FREQUENCY1 (1) - Sampling Frq 1 (High)
        /// RSNRPZ_SAMPLING_FREQUENCY2 (2) - Sampling Frq 2 (Low)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getSamplingFrequency(int channel, out int samplingFrequency)
        {
            int pInvokeResult = PInvoke.chan_getSamplingFrequency(this._handle, channel, out samplingFrequency);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function performs zeroing of selected sensor using the signal at the sensor input. The sensor must be disconnected from all power sources. If the signal at the input considerably deviates from 0 W, an error message is issued and the command is aborted.
        /// 
        /// Remote-control command(s):
        /// STAT:OPER:MEAS?
        /// CAL:ZERO:AUTO ONCE
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_zero(int channel)
        {
            int pInvokeResult = PInvoke.chan_zero(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function performs zeroing using the signal at the sensor input. The sensor must be disconnected from all power sources. If the signal at the input considerably deviates from 0 W, an error message is issued and the function is aborted.
        /// 
        /// Note(s):
        /// 
        /// (1) This function is valid only for NRP-Z81.
        /// 
        /// Remote-control command(s):
        /// CAL:ZERO:AUTO LFR | UFR
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="zeroing">
        /// This control selects type of advanced zeroing.
        /// 
        /// Valid Values:
        /// RSNRPZ_ZERO_LFR (0) - Low Frequencies
        /// RSNRPZ_ZERO_UFR (1) - High Frequencies
        /// 
        /// Default Value: RSNRPZ_ZERO_LFR (0)
        /// 
        /// Note(s):
        /// 
        /// (1) Low Frequencies - Does zeroing only for low frequencies (&lt; 500 MHZ)
        /// 
        /// (2) High Frequencies - Does zeroing for higher Frequencies (&gt;= 500 MHz).
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        /// 
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_zeroAdvanced(int channel, int zeroing)
        {
            int pInvokeResult = PInvoke.chan_zeroAdvanced(this._handle, channel, zeroing);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the state of zeroing of the sensor.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="zeroingComplete">
        /// This control returns the state of the zeroing of the sensor.
        /// 
        /// Valid Values:
        /// Complete (VI_TRUE)
        /// In Progress (VI_FALSE)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_isZeroComplete(int channel, out bool zeroingComplete)
        {
            ushort zeroingCompleteAsUShort;
            int pInvokeResult = PInvoke.chan_isZeroComplete(this._handle, channel, out zeroingCompleteAsUShort);
            zeroingComplete = System.Convert.ToBoolean(zeroingCompleteAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the state of the measurement.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="measurementComplete">
        /// This control returns the state of the measurement.
        /// 
        /// Valid Values:
        /// Complete (VI_TRUE)
        /// In Progress (VI_FALSE)
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_isMeasurementComplete(int channel, out bool measurementComplete)
        {
            ushort measurementCompleteAsUShort;
            int pInvokeResult = PInvoke.chan_isMeasurementComplete(this._handle, channel, out measurementCompleteAsUShort);
            measurementComplete = System.Convert.ToBoolean(measurementCompleteAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function performs a sensor test and returns a list of strings separated by commas. The contents of this test protocol is sensor-specific. For its meaning, please refer to the sensor documentation.
        /// 
        /// Remote-control command(s):
        /// TEST:SENS?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="result">
        /// This control returns the result string of self-test.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_selfTest(int channel, System.Text.StringBuilder result)
        {
            int pInvokeResult = PInvoke.chan_selfTest(this._handle, channel, result);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function selects which measurement results are to be made available in the Trace mode.
        /// 
        /// Note(s):
        /// 
        /// (1) This functions is available only for NRP-Z81
        /// 
        /// Remote-control command(s):
        /// SENSe:AUXiliary NONE | MINMAX | RNDMAX
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="auxiliaryValue">
        /// This control selects which measurement results are to be made available in the Trace mode.
        /// 
        /// Valid Values:
        /// RSNRPZ_AUX_NONE   (0) - None
        /// RSNRPZ_AUX_MINMAX (1) - Min Max
        /// RSNRPZ_AUX_RNDMAX (2) - Rnd Max
        /// 
        /// Note(s):
        /// 
        /// (1) None - only the average power of the associated samples
        /// 
        /// (2) Min Max - provides the maximum and minimum as well
        /// 
        /// (3) Rnd Max - provides the maximum and a random sample.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_setAuxiliary(int channel, int auxiliaryValue)
        {
            int pInvokeResult = PInvoke.chan_setAuxiliary(this._handle, channel, auxiliaryValue);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function queries which measurement results are available in the Trace mode.
        /// 
        /// Note(s):
        /// 
        /// (1) This functions is available only for NRP-Z81
        /// 
        /// Remote-control command(s):
        /// SENSe:AUXiliary?
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="auxiliaryValue">
        /// This control returns which measurement results are available in the Trace mode.
        /// 
        /// Valid Values:
        /// RSNRPZ_AUX_NONE   (0) - None (Default Value)
        /// RSNRPZ_AUX_MINMAX (1) - Min Max
        /// RSNRPZ_AUX_RNDMAX (2) - Rnd Max
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int chan_getAuxiliary(int channel, out int auxiliaryValue)
        {
            int pInvokeResult = PInvoke.chan_getAuxiliary(this._handle, channel, out auxiliaryValue);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function initiates an acquisition on the channels that you specifies in channel parameter.  It then waits for the acquisition to complete, and returns the measurement for the channel you specify.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeoutMs">
        /// Pass the maximum length of time in which to allow the read measurement operation to complete.    
        /// 
        /// If the operation does not complete within this time interval, the function returns the RSNRPZ_ERROR_MAX_TIME_EXCEEDED error code.  When this occurs, you can call rsnrpz_chan_abort to cancel the read measurement operation and return the sensor to the Idle state.
        /// 
        /// Units:  milliseconds.  
        /// 
        /// Defined Values:
        /// RSNRPZ_VAL_MAX_TIME_INFINITE
        /// RSNRPZ_VAL_MAX_TIME_IMMEDIATE
        /// 
        /// Default Value: 5000 (ms)
        /// 
        /// Notes:
        /// 
        /// (1) The Maximum Time parameter applies only to this function.  It has no effect on other timeout parameters.
        /// </param>
        /// <param name="measurement">
        /// Returns single measurement.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_readMeasurement(int channel, int timeoutMs, out double measurement)
        {
            int pInvokeResult = PInvoke.meass_readMeasurement(this._handle, channel, timeoutMs, out measurement);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the measurement the sensor acquires for the channel you specify.  The measurement is from an acquisition that you previously initiated.  
        /// 
        /// You use the rsnrpz_chan_initiate function to start an acquisition on the channels that you specify. You use the rsnrpz_chan_isMeasurementComplete function to determine when the acquisition is complete.
        /// 
        /// You can call the rsnrpz_meass_readMeasurement function instead of the rsnrpz_chan_initiate function.  The rsnrpz_meass_readMeasurement function starts an acquisition, waits for the acquisition to complete, and returns the measurement for the channel you specify.
        /// 
        /// Note:
        /// 
        /// 1) If the acquisition has not be initialized or measurement is still in progress and value is not available, function returns an error( RSNRPZ_ERROR_MEAS_NOT_AVAILABLE ).
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="measurement">
        /// Returns single measurement.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_fetchMeasurement(int channel, out double measurement)
        {
            int pInvokeResult = PInvoke.meass_fetchMeasurement(this._handle, channel, out measurement);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function initiates an acquisition on the channels that you specifies in channel parameter.  It then waits for the acquisition to complete, and returns the measurement for the channel you specify.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumTimeMs">
        /// Pass the maximum length of time in which to allow the read measurement operation to complete.    
        /// 
        /// If the operation does not complete within this time interval, the function returns the RSNRPZ_ERROR_MAX_TIME_EXCEEDED error code.  When this occurs, you can call rsnrpz_chan_abort to cancel the read measurement operation and return the sensor to the Idle state.
        /// 
        /// Units:  milliseconds.  
        /// 
        /// Defined Values:
        /// RSNRPZ_VAL_MAX_TIME_INFINITE             RSNRPZ_VAL_MAX_TIME_IMMEDIATE           
        /// 
        /// Default Value: 5000 (ms)
        /// 
        /// Notes:
        /// 
        /// (1) The Maximum Time parameter applies only to this function.  It has no effect on other timeout parameters.
        /// </param>
        /// <param name="bufferSize">
        /// Pass the number of elements in the Measurement Array parameter.
        /// 
        /// Default Value: None
        /// 
        /// </param>
        /// <param name="measurementArray">
        /// Returns the measurement buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="readCount">
        /// Indicates the number of points the function places in the Measurement Array parameter.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_readBufferMeasurement(int channel, int maximumTimeMs, int bufferSize, double[] measurementArray, out int readCount)
        {
            int pInvokeResult = PInvoke.meass_readBufferMeasurement(this._handle, channel, maximumTimeMs, bufferSize, measurementArray, out readCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the buffer measurement the sensor acquires for the channel you specify.  The measurement is from an acquisition that you previously initiated.  
        /// 
        /// You use the rsnrpz_chan_initiate function to start an acquisition on the channels that you specify. You use the rsnrpz_chan_isMeasurementComplete function to determine when the acquisition is complete.
        /// 
        /// You can call the rsnrpz_meas_readBufferMeasurement function instead of the rsnrpz_chan_initiate function.  The rsnrpz_meass_readBufferMeasurement function starts an acquisition, waits for the acquisition to complete, and returns the measurement for the channel you specify.
        /// 
        /// Note:
        /// 
        /// 1) If the acquisition has not be initialized or measurement is still in progress and value is not available, function returns an error( RSNRPZ_ERROR_MEAS_NOT_AVAILABLE ).
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="arraySize">
        /// Pass the number of elements in the Measurement Array parameter.
        /// 
        /// Default Value: None
        /// 
        /// </param>
        /// <param name="measurementArray">
        /// Returns the measurement buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="readCount">
        /// Indicates the number of points the function places in the Measurement Array parameter.
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_fetchBufferMeasurement(int channel, int arraySize, double[] measurementArray, out int readCount)
        {
            int pInvokeResult = PInvoke.meass_fetchBufferMeasurement(this._handle, channel, arraySize, measurementArray, out readCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// Triggers a BUS event. If the sensor is in the WAIT_FOR_TRG state and the source for the trigger source is set to BUS, the sensor enters the MEASURING state. This function invalidates all current measuring results. A query of measurement data following this function will thus always return the measured value determined in response to this function.
        /// 
        /// Remote-control command(s):
        /// *TRG
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_sendSoftwareTrigger(int channel)
        {
            int pInvokeResult = PInvoke.meass_sendSoftwareTrigger(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function initiates an acquisition on the channels that you specifies in channel parameter. It then waits for the acquisition to complete, and returns the auxiliary measurement for the channel you specify.
        /// 
        /// Note(s):
        /// 
        /// (1) If SENSE:AUX is set to None, this function returns error.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeoutMs">
        /// Pass the maximum length of time in which to allow the read measurement operation to complete.    
        /// 
        /// If the operation does not complete within this time interval, the function returns the RSNRPZ_ERROR_MAX_TIME_EXCEEDED error code.  When this occurs, you can call rsnrpz_chan_abort to cancel the read measurement operation and return the sensor to the Idle state.
        /// 
        /// Units:  milliseconds.  
        /// 
        /// Defined Values:
        /// RSNRPZ_VAL_MAX_TIME_INFINITE
        /// RSNRPZ_VAL_MAX_TIME_IMMEDIATE
        /// 
        /// Default Value: 5000 (ms)
        /// 
        /// Notes:
        /// 
        /// (1) The Maximum Time parameter applies only to this function.  It has no effect on other timeout parameters.
        /// </param>
        /// <param name="measurement">
        /// Returns single measurement.
        /// </param>
        /// <param name="aux1">
        /// This control returns the first Auxiliary value.
        /// </param>
        /// <param name="aux2">
        /// This control returns the second Auxiliary value.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_readMeasurementAux(int channel, int timeoutMs, out double measurement, out double aux1, out double aux2)
        {
            int pInvokeResult = PInvoke.meass_readMeasurementAux(this._handle, channel, timeoutMs, out measurement, out aux1, out aux2);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the measurement the sensor acquires for the channel you specify.  The measurement is from an acquisition that you previously initiated.  
        /// 
        /// You use the rsnrpz_chan_initiate function to start an acquisition on the channels that you specify. You use the rsnrpz_chan_isMeasurementComplete function to determine when the acquisition is complete.
        /// 
        /// You can call the rsnrpz_meass_readMeasurement function instead of the rsnrpz_chan_initiate function.  The rsnrpz_meass_readMeasurement function starts an acquisition, waits for the acquisition to complete, and returns the measurement for the channel you specify.
        /// 
        /// Note(s):
        /// 
        /// 1) If the acquisition has not be initialized or measurement is still in progress and value is not available, function returns an error( RSNRPZ_ERROR_MEAS_NOT_AVAILABLE ).
        /// 
        /// 2) If SENSE:AUX is set to None, this function returns error.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="timeoutMs">
        /// Pass the maximum length of time in which to allow the read measurement operation to complete.    
        /// 
        /// If the operation does not complete within this time interval, the function returns the RSNRPZ_ERROR_MAX_TIME_EXCEEDED error code.  When this occurs, you can call rsnrpz_chan_abort to cancel the read measurement operation and return the sensor to the Idle state.
        /// 
        /// Units:  milliseconds.  
        /// 
        /// Defined Values:
        /// RSNRPZ_VAL_MAX_TIME_INFINITE
        /// RSNRPZ_VAL_MAX_TIME_IMMEDIATE
        /// 
        /// Default Value: 5000 (ms)
        /// 
        /// Notes:
        /// 
        /// (1) The Maximum Time parameter applies only to this function.  It has no effect on other timeout parameters.
        /// </param>
        /// <param name="measurement">
        /// Returns single measurement.
        /// </param>
        /// <param name="aux1">
        /// This control returns the first Auxiliary value.
        /// </param>
        /// <param name="aux2">
        /// This control returns the second Auxiliary value.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_fetchMeasurementAux(int channel, int timeoutMs, out double measurement, out double aux1, out double aux2)
        {
            int pInvokeResult = PInvoke.meass_fetchMeasurementAux(this._handle, channel, timeoutMs, out measurement, out aux1, out aux2);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function initiates an acquisition on the channels that you specifies in channel parameter.  It then waits for the acquisition to complete, and returns the measurement for the channel you specify.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumTimeMs">
        /// Pass the maximum length of time in which to allow the read measurement operation to complete.    
        /// 
        /// If the operation does not complete within this time interval, the function returns the RSNRPZ_ERROR_MAX_TIME_EXCEEDED error code.  When this occurs, you can call rsnrpz_chan_abort to cancel the read measurement operation and return the sensor to the Idle state.
        /// 
        /// Units:  milliseconds.  
        /// 
        /// Defined Values:
        /// RSNRPZ_VAL_MAX_TIME_INFINITE             RSNRPZ_VAL_MAX_TIME_IMMEDIATE           
        /// 
        /// Default Value: 5000 (ms)
        /// 
        /// Notes:
        /// 
        /// (1) The Maximum Time parameter applies only to this function.  It has no effect on other timeout parameters.
        /// </param>
        /// <param name="bufferSize">
        /// Pass the number of elements in the Measurement Array parameter.
        /// 
        /// Default Value: None
        /// 
        /// </param>
        /// <param name="measurementArray">
        /// Returns the measurement buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="aux1Array">
        /// Returns the Aux 1 buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="aux2Array">
        /// Returns the Aux 2 buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="readCount">
        /// Indicates the number of points the function places in the Measurement Array parameter.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_readBufferMeasurementAux(int channel, int maximumTimeMs, int bufferSize, double[] measurementArray, double[] aux1Array, double[] aux2Array, out int readCount)
        {
            int pInvokeResult = PInvoke.meass_readBufferMeasurementAux(this._handle, channel, maximumTimeMs, bufferSize, measurementArray, aux1Array, aux2Array, out readCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the buffer measurement the sensor acquires for the channel you specify.  The measurement is from an acquisition that you previously initiated.  
        /// 
        /// You use the rsnrpz_chan_initiate function to start an acquisition on the channels that you specify. You use the rsnrpz_chan_isMeasurementComplete function to determine when the acquisition is complete.
        /// 
        /// You can call the rsnrpz_meas_readBufferMeasurement function instead of the rsnrpz_chan_initiate function.  The rsnrpz_meass_readBufferMeasurement function starts an acquisition, waits for the acquisition to complete, and returns the measurement for the channel you specify.
        /// 
        /// Note:
        /// 
        /// 1) If the acquisition has not be initialized or measurement is still in progress and value is not available, function returns an error( RSNRPZ_ERROR_MEAS_NOT_AVAILABLE ).
        /// 
        /// 2) If SENSE:AUX is set to None, this function returns error.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to max available channels
        /// 
        /// Default Value: 1
        /// </param>
        /// <param name="maximumTimeMs">
        /// Pass the maximum length of time in which to allow the read measurement operation to complete.    
        /// 
        /// If the operation does not complete within this time interval, the function returns the RSNRPZ_ERROR_MAX_TIME_EXCEEDED error code.  When this occurs, you can call rsnrpz_chan_abort to cancel the read measurement operation and return the sensor to the Idle state.
        /// 
        /// Units:  milliseconds.  
        /// 
        /// Defined Values:
        /// RSNRPZ_VAL_MAX_TIME_INFINITE             RSNRPZ_VAL_MAX_TIME_IMMEDIATE           
        /// 
        /// Default Value: 5000 (ms)
        /// 
        /// Notes:
        /// 
        /// (1) The Maximum Time parameter applies only to this function.  It has no effect on other timeout parameters.
        /// </param>
        /// <param name="bufferSize">
        /// Pass the number of elements in the Measurement Array parameter.
        /// 
        /// Default Value: None
        /// 
        /// </param>
        /// <param name="measurementArray">
        /// Returns the measurement buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="aux1Array">
        /// Returns the Aux 1 buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="aux2Array">
        /// Returns the Aux 2 buffer that the sensor acquires.  
        /// 
        /// </param>
        /// <param name="readCount">
        /// Indicates the number of points the function places in the Measurement Array parameter.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int meass_fetchBufferMeasurementAux(int channel, int maximumTimeMs, int bufferSize, double[] measurementArray, double[] aux1Array, double[] aux2Array, out int readCount)
        {
            int pInvokeResult = PInvoke.meass_fetchBufferMeasurementAux(this._handle, channel, maximumTimeMs, bufferSize, measurementArray, aux1Array, aux2Array, out readCount);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function resets the R&amp;S NRPZ registry to default values.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int status_preset()
        {
            int pInvokeResult = PInvoke.status_preset(this._handle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function checks selected status register for bits defined in Bitmask and returns a logical OR of all defined bits.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="statusClass">
        /// This control selects the status register.
        /// 
        /// Valid Values:
        /// RSNRPZ_STATCLASS_D_CONN (1) - Sensor Connected 
        /// RSNRPZ_STATCLASS_D_ERR  (2) - Sensor Error
        /// RSNRPZ_STATCLASS_O_CAL  (3) - Operation Calibrating Status Register
        /// RSNRPZ_STATCLASS_O_MEAS (4) - Operation Measuring Status Register
        /// RSNRPZ_STATCLASS_O_TRIGGER (5) - Operation Trigger Status Register
        /// RSNRPZ_STATCLASS_O_SENSE (6) - Operation Sense Status Register
        /// RSNRPZ_STATCLASS_O_LOWER (7) - Operation Lower Limit Fail Status Register
        /// RSNRPZ_STATCLASS_O_UPPER (8) - Operation Upper Limit Fail Status Register
        /// RSNRPZ_STATCLASS_Q_POWER (9) - Power Part of Questionable Power Status Register
        /// RSNRPZ_STATCLASS_Q_WINDOW (10) - Questionable Window Status Register
        /// RSNRPZ_STATCLASS_Q_CAL (11) - Questionable Calibration Status Register
        /// RSNRPZ_STATCLASS_Q_ZER (12) - Zero Part of Questionable Power Status Register
        /// 
        /// </param>
        /// <param name="mask">
        /// This control defines the bit which should be checked in the specified Status Register.
        /// 
        /// You can use following constant for checking operation and questionable registers. To check multiple bits, bitwise OR them together. For example, if you want check sensor on channel 1 and sensor on channel 2, then pass 
        /// RSNRPZ_SENSOR_01 | RSNRPZ_SENSOR_02
        /// 
        /// Valid Values:
        /// constant               bit     value
        /// RSNRPZ_SENSOR_01        1       0x2
        /// RSNRPZ_SENSOR_02        2       0x4
        /// RSNRPZ_SENSOR_03        3       0x8
        /// RSNRPZ_SENSOR_04        4       0x10 
        /// RSNRPZ_SENSOR_05        5       0x20 
        /// RSNRPZ_SENSOR_06        6       0x40 
        /// RSNRPZ_SENSOR_07        7       0x80 
        /// RSNRPZ_SENSOR_08        8       0x100
        /// RSNRPZ_SENSOR_09        9       0x200
        /// RSNRPZ_SENSOR_10       10       0x400 
        /// RSNRPZ_SENSOR_11       11       0x800 
        /// RSNRPZ_SENSOR_12       12       0x1000
        /// RSNRPZ_SENSOR_13       13       0x2000 
        /// RSNRPZ_SENSOR_14       14       0x4000
        /// RSNRPZ_SENSOR_15       15       0x8000
        /// RSNRPZ_SENSOR_16       16       0x10000
        /// RSNRPZ_SENSOR_17       17       0x20000
        /// RSNRPZ_SENSOR_18       18       0x40000
        /// RSNRPZ_SENSOR_19       19       0x80000
        /// RSNRPZ_SENSOR_20       20       0x100000
        /// RSNRPZ_SENSOR_21       21       0x200000
        /// RSNRPZ_SENSOR_22       22       0x400000
        /// RSNRPZ_SENSOR_23       23       0x800000
        /// RSNRPZ_SENSOR_24       24       0x1000000
        /// RSNRPZ_SENSOR_25       25       0x2000000
        /// RSNRPZ_SENSOR_26       26       0x4000000
        /// RSNRPZ_SENSOR_27       27       0x8000000
        /// RSNRPZ_SENSOR_28       28       0x10000000
        /// RSNRPZ_SENSOR_29       29       0x20000000
        /// RSNRPZ_SENSOR_30       30       0x40000000
        /// RSNRPZ_SENSOR_31       31       0x80000000
        /// RSNRPZ_ALL_SENSORS    all       0xFFFFFFFE
        /// 
        /// Default Value:
        /// RSNRPZ_ALL_SENSORS
        /// </param>
        /// <param name="state">
        /// This control returns the bitwise OR of all bits defined by the bitmask.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - Logical True
        /// VI_FALSE (0) - Logical False
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int status_checkCondition(int statusClass, uint mask, out bool state)
        {
            ushort stateAsUShort;
            int pInvokeResult = PInvoke.status_checkCondition(this._handle, statusClass, mask, out stateAsUShort);
            state = System.Convert.ToBoolean(stateAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function sets the PTransition and NTransition register of selected status register according to bitmask.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="statusClass">
        /// This control selects the status register.
        /// 
        /// Valid Values:
        /// RSNRPZ_STATCLASS_D_CONN (1) - Sensor Connected 
        /// RSNRPZ_STATCLASS_D_ERR  (2) - Sensor Error
        /// RSNRPZ_STATCLASS_O_CAL  (3) - Operation Calibrating Status Register
        /// RSNRPZ_STATCLASS_O_MEAS (4) - Operation Measuring Status Register
        /// RSNRPZ_STATCLASS_O_TRIGGER (5) - Operation Trigger Status Register
        /// RSNRPZ_STATCLASS_O_SENSE (6) - Operation Sense Status Register
        /// RSNRPZ_STATCLASS_O_LOWER (7) - Operation Lower Limit Fail Status Register
        /// RSNRPZ_STATCLASS_O_UPPER (8) - Operation Upper Limit Fail Status Register
        /// RSNRPZ_STATCLASS_Q_POWER (9) - Power Part of Questionable Power Status Register
        /// RSNRPZ_STATCLASS_Q_WINDOW (10) - Questionable Window Status Register
        /// RSNRPZ_STATCLASS_Q_CAL (11) - Questionable Calibration Status Register
        /// RSNRPZ_STATCLASS_Q_ZER (12) - Zero Part of Questionable Power Status Register
        /// 
        /// Notes:
        /// (1) For meaning of each status register consult Operation Manual.
        /// </param>
        /// <param name="mask">
        /// This control defines the bit which should be checked in the specified Status Register.
        /// 
        /// You can use following constant for checking operation and questionable registers. To check multiple bits, bitwise OR them together. For example, if you want check sensor on channel 1 and sensor on channel 2, then pass 
        /// RSNRPZ_SENSOR_01 | RSNRPZ_SENSOR_02
        /// 
        /// Valid Values:
        /// constant               bit     value
        /// RSNRPZ_SENSOR_01        1       0x2
        /// RSNRPZ_SENSOR_02        2       0x4
        /// RSNRPZ_SENSOR_03        3       0x8
        /// RSNRPZ_SENSOR_04        4       0x10 
        /// RSNRPZ_SENSOR_05        5       0x20 
        /// RSNRPZ_SENSOR_06        6       0x40 
        /// RSNRPZ_SENSOR_07        7       0x80 
        /// RSNRPZ_SENSOR_08        8       0x100
        /// RSNRPZ_SENSOR_09        9       0x200
        /// RSNRPZ_SENSOR_10       10       0x400 
        /// RSNRPZ_SENSOR_11       11       0x800 
        /// RSNRPZ_SENSOR_12       12       0x1000
        /// RSNRPZ_SENSOR_13       13       0x2000 
        /// RSNRPZ_SENSOR_14       14       0x4000
        /// RSNRPZ_SENSOR_15       15       0x8000
        /// RSNRPZ_SENSOR_16       16       0x10000
        /// RSNRPZ_SENSOR_17       17       0x20000
        /// RSNRPZ_SENSOR_18       18       0x40000
        /// RSNRPZ_SENSOR_19       19       0x80000
        /// RSNRPZ_SENSOR_20       20       0x100000
        /// RSNRPZ_SENSOR_21       21       0x200000
        /// RSNRPZ_SENSOR_22       22       0x400000
        /// RSNRPZ_SENSOR_23       23       0x800000
        /// RSNRPZ_SENSOR_24       24       0x1000000
        /// RSNRPZ_SENSOR_25       25       0x2000000
        /// RSNRPZ_SENSOR_26       26       0x4000000
        /// RSNRPZ_SENSOR_27       27       0x8000000
        /// RSNRPZ_SENSOR_28       28       0x10000000
        /// RSNRPZ_SENSOR_29       29       0x20000000
        /// RSNRPZ_SENSOR_30       30       0x40000000
        /// RSNRPZ_SENSOR_31       31       0x80000000
        /// RSNRPZ_ALL_SENSORS    all       0xFFFFFFFE
        /// 
        /// Default Value:
        /// RSNRPZ_ALL_SENSORS
        /// </param>
        /// <param name="direction">
        /// This control defines the direction of transition of the event.
        /// 
        /// Valid Values:
        /// RSNRPZ_DIRECTION_NONE (0) - None Transition
        /// RSNRPZ_DIRECTION_PTR (1) - Positive Transition  (Default Value)
        /// RSNRPZ_DIRECTION_NTR (2) - Negative Transition
        /// RSNRPZ_DIRECTION_BOTH (3) - Both Transition
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int status_catchEvent(int statusClass, uint mask, int direction)
        {
            int pInvokeResult = PInvoke.status_catchEvent(this._handle, statusClass, mask, direction);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function checks the selected status register for events specified by bitmask and sets returns their states. Finally all bits of shadow status register specified by Reset Action will be set to zero.
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="statusClass">
        /// This control selects the status register.
        /// 
        /// Valid Values:
        /// RSNRPZ_STATCLASS_D_CONN (1) - Sensor Connected 
        /// RSNRPZ_STATCLASS_D_ERR  (2) - Sensor Error
        /// RSNRPZ_STATCLASS_O_CAL  (3) - Operation Calibrating Status Register
        /// RSNRPZ_STATCLASS_O_MEAS (4) - Operation Measuring Status Register
        /// RSNRPZ_STATCLASS_O_TRIGGER (5) - Operation Trigger Status Register
        /// RSNRPZ_STATCLASS_O_SENSE (6) - Operation Sense Status Register
        /// RSNRPZ_STATCLASS_O_LOWER (7) - Operation Lower Limit Fail Status Register
        /// RSNRPZ_STATCLASS_O_UPPER (8) - Operation Upper Limit Fail Status Register
        /// RSNRPZ_STATCLASS_Q_POWER (9) - Power Part of Questionable Power Status Register
        /// RSNRPZ_STATCLASS_Q_WINDOW (10) - Questionable Window Status Register
        /// RSNRPZ_STATCLASS_Q_CAL (11) - Questionable Calibration Status Register
        /// RSNRPZ_STATCLASS_Q_ZER (12) - Zero Part of Questionable Power Status Register
        /// 
        /// Notes:
        /// (1) For meaning of each status register consult Operation Manual.
        /// </param>
        /// <param name="mask">
        /// This control defines the bit which should be checked in the specified Status Register.
        /// 
        /// You can use following constant for checking operation and questionable registers. To check multiple bits, bitwise OR them together. For example, if you want check sensor on channel 1 and sensor on channel 2, then pass 
        /// RSNRPZ_SENSOR_01 | RSNRPZ_SENSOR_02
        /// 
        /// Valid Values:
        /// constant               bit     value
        /// RSNRPZ_SENSOR_01        1       0x2
        /// RSNRPZ_SENSOR_02        2       0x4
        /// RSNRPZ_SENSOR_03        3       0x8
        /// RSNRPZ_SENSOR_04        4       0x10 
        /// RSNRPZ_SENSOR_05        5       0x20 
        /// RSNRPZ_SENSOR_06        6       0x40 
        /// RSNRPZ_SENSOR_07        7       0x80 
        /// RSNRPZ_SENSOR_08        8       0x100
        /// RSNRPZ_SENSOR_09        9       0x200
        /// RSNRPZ_SENSOR_10       10       0x400 
        /// RSNRPZ_SENSOR_11       11       0x800 
        /// RSNRPZ_SENSOR_12       12       0x1000
        /// RSNRPZ_SENSOR_13       13       0x2000 
        /// RSNRPZ_SENSOR_14       14       0x4000
        /// RSNRPZ_SENSOR_15       15       0x8000
        /// RSNRPZ_SENSOR_16       16       0x10000
        /// RSNRPZ_SENSOR_17       17       0x20000
        /// RSNRPZ_SENSOR_18       18       0x40000
        /// RSNRPZ_SENSOR_19       19       0x80000
        /// RSNRPZ_SENSOR_20       20       0x100000
        /// RSNRPZ_SENSOR_21       21       0x200000
        /// RSNRPZ_SENSOR_22       22       0x400000
        /// RSNRPZ_SENSOR_23       23       0x800000
        /// RSNRPZ_SENSOR_24       24       0x1000000
        /// RSNRPZ_SENSOR_25       25       0x2000000
        /// RSNRPZ_SENSOR_26       26       0x4000000
        /// RSNRPZ_SENSOR_27       27       0x8000000
        /// RSNRPZ_SENSOR_28       28       0x10000000
        /// RSNRPZ_SENSOR_29       29       0x20000000
        /// RSNRPZ_SENSOR_30       30       0x40000000
        /// RSNRPZ_SENSOR_31       31       0x80000000
        /// RSNRPZ_ALL_SENSORS    all       0xFFFFFFFE
        /// 
        /// Default Value:
        /// RSNRPZ_ALL_SENSORS
        /// </param>
        /// <param name="resetMask">
        /// This control defines which bits of the shadow status register will reset to zero when finishing the function.
        /// 
        /// You can use following constant for reset operation. To reset multiple bits, bitwise OR them together. For example, if you want reset only bits corresponding with sensor on channel 1 and sensor on channel 2, then pass 
        /// RSNRPZ_SENSOR_01 | RSNRPZ_SENSOR_02
        /// 
        /// Valid Values:
        /// constant               bit     value
        /// RSNRPZ_SENSOR_01        1       0x2
        /// RSNRPZ_SENSOR_02        2       0x4
        /// RSNRPZ_SENSOR_03        3       0x8
        /// RSNRPZ_SENSOR_04        4       0x10 
        /// RSNRPZ_SENSOR_05        5       0x20 
        /// RSNRPZ_SENSOR_06        6       0x40 
        /// RSNRPZ_SENSOR_07        7       0x80 
        /// RSNRPZ_SENSOR_08        8       0x100
        /// RSNRPZ_SENSOR_09        9       0x200
        /// RSNRPZ_SENSOR_10       10       0x400 
        /// RSNRPZ_SENSOR_11       11       0x800 
        /// RSNRPZ_SENSOR_12       12       0x1000
        /// RSNRPZ_SENSOR_13       13       0x2000 
        /// RSNRPZ_SENSOR_14       14       0x4000
        /// RSNRPZ_SENSOR_15       15       0x8000
        /// RSNRPZ_SENSOR_16       16       0x10000
        /// RSNRPZ_SENSOR_17       17       0x20000
        /// RSNRPZ_SENSOR_18       18       0x40000
        /// RSNRPZ_SENSOR_19       19       0x80000
        /// RSNRPZ_SENSOR_20       20       0x100000
        /// RSNRPZ_SENSOR_21       21       0x200000
        /// RSNRPZ_SENSOR_22       22       0x400000
        /// RSNRPZ_SENSOR_23       23       0x800000
        /// RSNRPZ_SENSOR_24       24       0x1000000
        /// RSNRPZ_SENSOR_25       25       0x2000000
        /// RSNRPZ_SENSOR_26       26       0x4000000
        /// RSNRPZ_SENSOR_27       27       0x8000000
        /// RSNRPZ_SENSOR_28       28       0x10000000
        /// RSNRPZ_SENSOR_29       29       0x20000000
        /// RSNRPZ_SENSOR_30       30       0x40000000
        /// RSNRPZ_SENSOR_31       31       0x80000000
        /// RSNRPZ_ALL_SENSORS    all       0xFFFFFFFE
        /// 
        /// Default Value:
        /// RSNRPZ_ALL_SENSORS
        /// </param>
        /// <param name="events">
        /// This control returns the bitwise OR of all bits defined by the bitmask.
        /// 
        /// Valid Values:
        /// VI_TRUE (1) - Logical True
        /// VI_FALSE (0) - Logical False
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int status_checkEvent(int statusClass, uint mask, uint resetMask, out bool events)
        {
            ushort eventsAsUShort;
            int pInvokeResult = PInvoke.status_checkEvent(this._handle, statusClass, mask, resetMask, out eventsAsUShort);
            events = System.Convert.ToBoolean(eventsAsUShort);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function enables events defined by Bitmask in enable register respective to the selected status register.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="statusClass">
        /// This control selects the status register.
        /// 
        /// Valid Values:
        /// RSNRPZ_STATCLASS_D_CONN (1) - Sensor Connected 
        /// RSNRPZ_STATCLASS_D_ERR  (2) - Sensor Error
        /// RSNRPZ_STATCLASS_O_CAL  (3) - Operation Calibrating Status Register
        /// RSNRPZ_STATCLASS_O_MEAS (4) - Operation Measuring Status Register
        /// RSNRPZ_STATCLASS_O_TRIGGER (5) - Operation Trigger Status Register
        /// RSNRPZ_STATCLASS_O_SENSE (6) - Operation Sense Status Register
        /// RSNRPZ_STATCLASS_O_LOWER (7) - Operation Lower Limit Fail Status Register
        /// RSNRPZ_STATCLASS_O_UPPER (8) - Operation Upper Limit Fail Status Register
        /// RSNRPZ_STATCLASS_Q_POWER (9) - Power Part of Questionable Power Status Register
        /// RSNRPZ_STATCLASS_Q_WINDOW (10) - Questionable Window Status Register
        /// RSNRPZ_STATCLASS_Q_CAL (11) - Questionable Calibration Status Register
        /// RSNRPZ_STATCLASS_Q_ZER (12) - Zero Part of Questionable Power Status Register
        /// 
        /// Notes:
        /// (1) For meaning of each status register consult Operation Manual.
        /// </param>
        /// <param name="mask">
        /// This control defines the bits(channels) which should be set to one and will generate SRQ.
        /// 
        /// You can use following constant for enabling SRQ for specified channels. To disable multiple channels, bitwise OR them together. For example, if you want enable SRQ for sensor on channel 1 and sensor on channel 2, then pass 
        /// RSNRPZ_SENSOR_01 | RSNRPZ_SENSOR_02.
        /// 
        /// Valid Values:
        /// constant               bit     value
        /// RSNRPZ_SENSOR_01        1       0x2
        /// RSNRPZ_SENSOR_02        2       0x4
        /// RSNRPZ_SENSOR_03        3       0x8
        /// RSNRPZ_SENSOR_04        4       0x10 
        /// RSNRPZ_SENSOR_05        5       0x20 
        /// RSNRPZ_SENSOR_06        6       0x40 
        /// RSNRPZ_SENSOR_07        7       0x80 
        /// RSNRPZ_SENSOR_08        8       0x100
        /// RSNRPZ_SENSOR_09        9       0x200
        /// RSNRPZ_SENSOR_10       10       0x400 
        /// RSNRPZ_SENSOR_11       11       0x800 
        /// RSNRPZ_SENSOR_12       12       0x1000
        /// RSNRPZ_SENSOR_13       13       0x2000 
        /// RSNRPZ_SENSOR_14       14       0x4000
        /// RSNRPZ_SENSOR_15       15       0x8000
        /// RSNRPZ_SENSOR_16       16       0x10000
        /// RSNRPZ_SENSOR_17       17       0x20000
        /// RSNRPZ_SENSOR_18       18       0x40000
        /// RSNRPZ_SENSOR_19       19       0x80000
        /// RSNRPZ_SENSOR_20       20       0x100000
        /// RSNRPZ_SENSOR_21       21       0x200000
        /// RSNRPZ_SENSOR_22       22       0x400000
        /// RSNRPZ_SENSOR_23       23       0x800000
        /// RSNRPZ_SENSOR_24       24       0x1000000
        /// RSNRPZ_SENSOR_25       25       0x2000000
        /// RSNRPZ_SENSOR_26       26       0x4000000
        /// RSNRPZ_SENSOR_27       27       0x8000000
        /// RSNRPZ_SENSOR_28       28       0x10000000
        /// RSNRPZ_SENSOR_29       29       0x20000000
        /// RSNRPZ_SENSOR_30       30       0x40000000
        /// RSNRPZ_SENSOR_31       31       0x80000000
        /// RSNRPZ_ALL_SENSORS    all       0xFFFFFFFE
        /// 
        /// Default Value:
        /// RSNRPZ_ALL_SENSORS
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int status_enableEventNotification(int statusClass, uint mask)
        {
            int pInvokeResult = PInvoke.status_enableEventNotification(this._handle, statusClass, mask);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function disables events defined by Bitmask in enable register respective to the selected status register.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="statusClass">
        /// This control selects the status register.
        /// 
        /// Valid Values:
        /// RSNRPZ_STATCLASS_D_CONN (1) - Sensor Connected 
        /// RSNRPZ_STATCLASS_D_ERR  (2) - Sensor Error
        /// RSNRPZ_STATCLASS_O_CAL  (3) - Operation Calibrating Status Register
        /// RSNRPZ_STATCLASS_O_MEAS (4) - Operation Measuring Status Register
        /// RSNRPZ_STATCLASS_O_TRIGGER (5) - Operation Trigger Status Register
        /// RSNRPZ_STATCLASS_O_SENSE (6) - Operation Sense Status Register
        /// RSNRPZ_STATCLASS_O_LOWER (7) - Operation Lower Limit Fail Status Register
        /// RSNRPZ_STATCLASS_O_UPPER (8) - Operation Upper Limit Fail Status Register
        /// RSNRPZ_STATCLASS_Q_POWER (9) - Power Part of Questionable Power Status Register
        /// RSNRPZ_STATCLASS_Q_WINDOW (10) - Questionable Window Status Register
        /// RSNRPZ_STATCLASS_Q_CAL (11) - Questionable Calibration Status Register
        /// RSNRPZ_STATCLASS_Q_ZER (12) - Zero Part of Questionable Power Status Register
        /// 
        /// Notes:
        /// (1) For meaning of each status register consult Operation Manual.
        /// </param>
        /// <param name="mask">
        /// This control defines the bit which should be set to zero in the specified Enable Register.
        /// 
        /// You can use following constant for disabling SRQ for specified channels. To disable multiple channels, bitwise OR them together. For example, if you want disable SRQ for sensor on channel 1 and sensor on channel 2, then pass 
        /// RSNRPZ_SENSOR_01 | RSNRPZ_SENSOR_02.
        /// 
        /// Valid Values:
        /// constant               bit     value
        /// RSNRPZ_SENSOR_01        1       0x2
        /// RSNRPZ_SENSOR_02        2       0x4
        /// RSNRPZ_SENSOR_03        3       0x8
        /// RSNRPZ_SENSOR_04        4       0x10 
        /// RSNRPZ_SENSOR_05        5       0x20 
        /// RSNRPZ_SENSOR_06        6       0x40 
        /// RSNRPZ_SENSOR_07        7       0x80 
        /// RSNRPZ_SENSOR_08        8       0x100
        /// RSNRPZ_SENSOR_09        9       0x200
        /// RSNRPZ_SENSOR_10       10       0x400 
        /// RSNRPZ_SENSOR_11       11       0x800 
        /// RSNRPZ_SENSOR_12       12       0x1000
        /// RSNRPZ_SENSOR_13       13       0x2000 
        /// RSNRPZ_SENSOR_14       14       0x4000
        /// RSNRPZ_SENSOR_15       15       0x8000
        /// RSNRPZ_SENSOR_16       16       0x10000
        /// RSNRPZ_SENSOR_17       17       0x20000
        /// RSNRPZ_SENSOR_18       18       0x40000
        /// RSNRPZ_SENSOR_19       19       0x80000
        /// RSNRPZ_SENSOR_20       20       0x100000
        /// RSNRPZ_SENSOR_21       21       0x200000
        /// RSNRPZ_SENSOR_22       22       0x400000
        /// RSNRPZ_SENSOR_23       23       0x800000
        /// RSNRPZ_SENSOR_24       24       0x1000000
        /// RSNRPZ_SENSOR_25       25       0x2000000
        /// RSNRPZ_SENSOR_26       26       0x4000000
        /// RSNRPZ_SENSOR_27       27       0x8000000
        /// RSNRPZ_SENSOR_28       28       0x10000000
        /// RSNRPZ_SENSOR_29       29       0x20000000
        /// RSNRPZ_SENSOR_30       30       0x40000000
        /// RSNRPZ_SENSOR_31       31       0x80000000
        /// RSNRPZ_ALL_SENSORS    all       0xFFFFFFFE
        /// 
        /// Default Value:
        /// RSNRPZ_ALL_SENSORS
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int status_disableEventNotification(int statusClass, uint mask)
        {
            int pInvokeResult = PInvoke.status_disableEventNotification(this._handle, statusClass, mask);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function registers message, which will be send to specified window, when SRQ is occured.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="windowHandle">
        /// Handle to the window whose window procedure is to receive the message. If the parameter is set to 0 (NULL), the message is disabled.
        /// </param>
        /// <param name="messageId">
        /// Specifies the message to be posted.  If the message ID is set to 0, message will be not posted.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int status_registerWindowMessage(uint windowHandle, uint messageId)
        {
            int pInvokeResult = PInvoke.status_registerWindowMessage(this._handle, windowHandle, messageId);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function switches state checking of the instrument (reading of the Standard Event Register and checking it for error) status subsystem. Driver functions are using state checking which is by default enabled.
        /// 
        /// Note:
        /// 
        /// (1) In debug mode enable state checking.
        /// 
        /// (2) For better bus throughput and instruments performance disable state checking.
        /// 
        /// (3) When state checking is disabled driver does not check if correct instrument model or option is used with each of the functions. This might cause unexpected behaviour of the instrument.
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="stateChecking">
        /// This control switches instrument state checking On or Off.
        /// 
        /// Valid Range:
        /// VI_FALSE (0) - Off
        /// VI_TRUE  (1) - On (Default Value)
        /// 
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This driver defines the following status codes:
        ///           
        /// Status    Description
        /// -------------------------------------------------
        /// BFFC0002  Parameter 2 (State Checking) out of range.
        ///           
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int ErrorCheckState(bool stateChecking)
        {
            int pInvokeResult = PInvoke.errorCheckState(this._handle, System.Convert.ToUInt16(stateChecking));
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function resets the instrument to a known state and sends initialization commands to the instrument that set any necessary programmatic variables such as Headers Off, Short Command form, and Data Transfer Binary to the state necessary for the operation of the instrument driver.
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors 
        /// </returns>
        public int Reset()
        {
            int pInvokeResult = PInvoke.reset(this._handle);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function runs the instrument's self test routine and returns the test result(s).
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="selfTestResult">
        /// This control contains the value returned from the instrument self test.  Zero means success.  For any other code, see the device's operator's manual.
        /// 
        /// </param>
        /// <param name="selfTestMessage">
        /// This control contains the string returned from the self test. See the device's operation manual for an explanation of the string's contents.
        /// 
        /// Notes:
        /// 
        /// (1) The array must contain at least 256 elements ViChar[256].
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This driver defines the following status codes:
        ///           
        /// Status    Description
        /// -------------------------------------------------
        /// BFFC0002  Parameter 2 (Self-Test Result) NULL pointer.
        ///           
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int self_test(out short selfTestResult, System.Text.StringBuilder selfTestMessage)
        {
            int pInvokeResult = PInvoke.self_test(this._handle, out selfTestResult, selfTestMessage);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function reads an error code from the instrument's error queue.
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="errorCode">
        /// This control returns the error code read from the instrument's error queue.
        /// 
        /// </param>
        /// <param name="errorMessage">
        /// This control returns the error message string read from the instrument's error message queue.
        /// 
        /// Notes:
        /// 
        /// (1) The array must contain at least 256 elements ViChar[256].
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This driver defines the following status codes:
        ///           
        /// Status    Description
        /// -------------------------------------------------
        /// BFFC0002  Parameter 2 (Error Code) NULL pointer.
        /// BFFC0003  Parameter 3 (Error Message) NULL pointer.
        ///           
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int error_query(out int errorCode, System.Text.StringBuilder errorMessage)
        {
            int pInvokeResult = PInvoke.error_query(this._handle, out errorCode, errorMessage);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function returns the revision numbers of the instrument driver and instrument firmware, and tells the user with which  instrument firmware this revision of the driver is compatible. 
        /// 
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="instrumentDriverRevision">
        /// This control returns the Instrument Driver Software Revision.
        /// 
        /// Notes:
        /// 
        /// (1) The array must contain at least 256 elements ViChar[256].
        /// </param>
        /// <param name="firmwareRevision">
        /// This control returns the Instrument Firmware Revision.
        /// 
        /// Notes:
        /// 
        /// (1) Because instrument does not support firmware revision the array is set to empty string or ignored if used VI_NULL.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This driver defines the following status codes:
        ///           
        /// Status    Description
        /// -------------------------------------------------
        /// 3FFC0105  Revision query not supported - VI_WARN_NSUP_REV_QUERY
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int revision_query(System.Text.StringBuilder instrumentDriverRevision, System.Text.StringBuilder firmwareRevision)
        {
            int pInvokeResult = PInvoke.revision_query(this._handle, instrumentDriverRevision, firmwareRevision);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        /// <summary>
        /// This function disconnect specified channel.
        /// 
        /// Note:
        /// 
        /// 1) rsnrpz_close function disconnect all channels automatically.
        /// </summary>
        /// <param name="Instrument_Handle">
        /// This control accepts the Instrument Handle returned by the Initialize function to select the desired instrument driver session.
        /// 
        /// Default Value:  None
        /// </param>
        /// <param name="channel">
        /// This control defines the channel number.
        /// 
        /// Valid Range:
        /// 1 to 31
        /// 
        /// Default Value: 2
        /// 
        /// Note:
        /// 
        /// 1) Function rsnrpz_init adds assigne sensor to channel 1 automatically.
        /// </param>
        /// <returns>
        /// Returns the status code of this operation. The status code  either indicates success or describes an error or warning condition. You examine the status code from each call to an instrument driver function to determine if an error occurred. To obtain a text description of the status code, call the rsnrpz_error_message function.
        ///           
        /// The general meaning of the status code is as follows:
        /// 
        /// Value                  Meaning
        /// -------------------------------
        /// 0                      Success
        /// Positive Values        Warnings
        /// Negative Values        Errors
        /// 
        /// This instrument driver also returns errors and warnings defined by other sources. The following table defines the ranges of additional status codes that this driver can return. The table lists the different include files that contain the defined constants for the particular status codes:
        ///           
        /// Numeric Range (in Hex)   Status Code Types    
        /// -------------------------------------------------
        /// 3FFC0000 to 3FFCFFFF     VXIPnP   Driver Warnings
        ///           
        /// BFFC0000 to BFFCFFFF     VXIPnP   Driver Errors
        /// </returns>
        public int CloseSensor(int channel)
        {
            int pInvokeResult = PInvoke.CloseSensor(this._handle, channel);
            PInvoke.TestForError(this._handle, pInvokeResult);
            return pInvokeResult;
        }

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if ((this._disposed == false))
            {
                PInvoke.close(this._handle);
                this._handle = System.IntPtr.Zero;
            }
            this._disposed = true;
        }

        private class PInvoke
        {
            #region DLL section
            [DllImport(NrpDll, EntryPoint = "rsnrpz_init", CallingConvention = CallingConvention.StdCall)]
            public static extern int init(string resourceName, ushort idQuery, ushort resetDevice, out System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_AddSensor", CallingConvention = CallingConvention.StdCall)]
            public static extern int AddSensor(System.IntPtr instrumentHandle, int channel, string resourceName, ushort idQuery, ushort resetDevice);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chans_abort", CallingConvention = CallingConvention.StdCall)]
            public static extern int chans_abort(System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chans_getCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int chans_getCount(System.IntPtr instrumentHandle, out int count);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chans_initiate", CallingConvention = CallingConvention.StdCall)]
            public static extern int chans_initiate(System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chans_zero", CallingConvention = CallingConvention.StdCall)]
            public static extern int chans_zero(System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chans_isZeroingComplete", CallingConvention = CallingConvention.StdCall)]
            public static extern int chans_isZeroingComplete(System.IntPtr instrumentHandle, out ushort zeroingCompleted);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chans_isMeasurementComplete", CallingConvention = CallingConvention.StdCall)]
            public static extern int chans_isMeasurementComplete(System.IntPtr instrumentHandle, out ushort measurementCompleted);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_mode", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_mode(System.IntPtr instrumentHandle, int channel, int measurementMode);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timing_configureExclude", CallingConvention = CallingConvention.StdCall)]
            public static extern int timing_configureExclude(System.IntPtr instrumentHandle, int channel, double excludeStart, double excludeStop);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timing_setTimingExcludeStart", CallingConvention = CallingConvention.StdCall)]
            public static extern int timing_setTimingExcludeStart(System.IntPtr instrumentHandle, int channel, double excludeStart);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timing_getTimingExcludeStart", CallingConvention = CallingConvention.StdCall)]
            public static extern int timing_getTimingExcludeStart(System.IntPtr instrumentHandle, int channel, out double excludeStart);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timing_setTimingExcludeStop", CallingConvention = CallingConvention.StdCall)]
            public static extern int timing_setTimingExcludeStop(System.IntPtr instrumentHandle, int channel, double excludeStop);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timing_getTimingExcludeStop", CallingConvention = CallingConvention.StdCall)]
            public static extern int timing_getTimingExcludeStop(System.IntPtr instrumentHandle, int channel, out double excludeStop);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_bandwidth_setBw", CallingConvention = CallingConvention.StdCall)]
            public static extern int bandwidth_setBw(System.IntPtr instrumentHandle, int channel, int bandwidth);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_bandwidth_getBw", CallingConvention = CallingConvention.StdCall)]
            public static extern int bandwidth_getBw(System.IntPtr instrumentHandle, int channel, out int bandwidth);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_bandwidth_getBwList", CallingConvention = CallingConvention.StdCall)]
            public static extern int bandwidth_getBwList(System.IntPtr instrumentHandle, int channel, int bufferSize, System.Text.StringBuilder bandwidthList);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_configureAvgAuto", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_configureAvgAuto(System.IntPtr instrumentHandle, int channel, int resolution);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_configureAvgNSRatio", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_configureAvgNSRatio(System.IntPtr instrumentHandle, int channel, double maximumNoiseRatio, double upperTimeLimit);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_configureAvgManual", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_configureAvgManual(System.IntPtr instrumentHandle, int channel, int count);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setAutoEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setAutoEnabled(System.IntPtr instrumentHandle, int channel, ushort autoEnabled);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getAutoEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getAutoEnabled(System.IntPtr instrumentHandle, int channel, out ushort autoEnabled);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setAutoMaxMeasuringTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setAutoMaxMeasuringTime(System.IntPtr instrumentHandle, int channel, double upperTimeLimit);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getAutoMaxMeasuringTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getAutoMaxMeasuringTime(System.IntPtr instrumentHandle, int channel, out double upperTimeLimit);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setAutoNoiseSignalRatio", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setAutoNoiseSignalRatio(System.IntPtr instrumentHandle, int channel, double maximumNoiseRatio);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getAutoNoiseSignalRatio", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getAutoNoiseSignalRatio(System.IntPtr instrumentHandle, int channel, out double maximumNoiseRatio);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setAutoResolution", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setAutoResolution(System.IntPtr instrumentHandle, int channel, int resolution);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getAutoResolution", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getAutoResolution(System.IntPtr instrumentHandle, int channel, out int resolution);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setAutoType", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setAutoType(System.IntPtr instrumentHandle, int channel, int method);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getAutoType", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getAutoType(System.IntPtr instrumentHandle, int channel, out int method);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setCount(System.IntPtr instrumentHandle, int channel, int count);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getCount(System.IntPtr instrumentHandle, int channel, out int count);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setEnabled(System.IntPtr instrumentHandle, int channel, ushort averaging);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getEnabled(System.IntPtr instrumentHandle, int channel, out ushort averaging);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setSlot", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setSlot(System.IntPtr instrumentHandle, int channel, int timeslot);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getSlot", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getSlot(System.IntPtr instrumentHandle, int channel, out int timeslot);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_setTerminalControl", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_setTerminalControl(System.IntPtr instrumentHandle, int channel, int terminalControl);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_getTerminalControl", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_getTerminalControl(System.IntPtr instrumentHandle, int channel, out int terminalControl);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_avg_reset", CallingConvention = CallingConvention.StdCall)]
            public static extern int avg_reset(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_range_setAutoEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int range_setAutoEnabled(System.IntPtr instrumentHandle, int channel, ushort autoRange);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_range_getAutoEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int range_getAutoEnabled(System.IntPtr instrumentHandle, int channel, out ushort autoRange);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_range_setCrossoverLevel", CallingConvention = CallingConvention.StdCall)]
            public static extern int range_setCrossoverLevel(System.IntPtr instrumentHandle, int channel, double crossoverLevel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_range_getCrossoverLevel", CallingConvention = CallingConvention.StdCall)]
            public static extern int range_getCrossoverLevel(System.IntPtr instrumentHandle, int channel, out double crossoverLevel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_range_setRange", CallingConvention = CallingConvention.StdCall)]
            public static extern int range_setRange(System.IntPtr instrumentHandle, int channel, int range);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_range_getRange", CallingConvention = CallingConvention.StdCall)]
            public static extern int range_getRange(System.IntPtr instrumentHandle, int channel, out int range);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_configureCorrections", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_configureCorrections(System.IntPtr instrumentHandle, int channel, ushort offsetState, double offset, ushort reserved1, string reserved2, ushort sParameterEnable);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setCorrectionFrequency", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setCorrectionFrequency(System.IntPtr instrumentHandle, int channel, double frequency);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getCorrectionFrequency", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getCorrectionFrequency(System.IntPtr instrumentHandle, int channel, out double frequency);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_setOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_setOffset(System.IntPtr instrumentHandle, int channel, double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_getOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_getOffset(System.IntPtr instrumentHandle, int channel, out double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_setOffsetEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_setOffsetEnabled(System.IntPtr instrumentHandle, int channel, ushort offsetState);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_getOffsetEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_getOffsetEnabled(System.IntPtr instrumentHandle, int channel, out ushort offsetState);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_setSParamDeviceEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_setSParamDeviceEnabled(System.IntPtr instrumentHandle, int channel, ushort sParameterEnable);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_getSParamDeviceEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_getSParamDeviceEnabled(System.IntPtr instrumentHandle, int channel, out ushort sParameterCorrection);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_setSParamDevice", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_setSParamDevice(System.IntPtr instrumentHandle, int channel, int sParameter);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_getSParamDevice", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_getSParamDevice(System.IntPtr instrumentHandle, int channel, out int sParameter);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_configureSourceGammaCorr", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_configureSourceGammaCorr(System.IntPtr instrumentHandle, int channel, ushort sourceGammaCorrection, double magnitude, double phase);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setSourceGammaMagnitude", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setSourceGammaMagnitude(System.IntPtr instrumentHandle, int channel, double magnitude);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getSourceGammaMagnitude", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getSourceGammaMagnitude(System.IntPtr instrumentHandle, int channel, out double magnitude);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setSourceGammaPhase", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setSourceGammaPhase(System.IntPtr instrumentHandle, int channel, double phase);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getSourceGammaPhase", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getSourceGammaPhase(System.IntPtr instrumentHandle, int channel, out double phase);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setSourceGammaCorrEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setSourceGammaCorrEnabled(System.IntPtr instrumentHandle, int channel, ushort sourceGammaCorrection);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getSourceGammaCorrEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getSourceGammaCorrEnabled(System.IntPtr instrumentHandle, int channel, out ushort sourceGammaCorrection);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_configureReflectGammaCorr", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_configureReflectGammaCorr(System.IntPtr instrumentHandle, int channel, double magnitude, double phase);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setReflectionGammaMagn", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setReflectionGammaMagn(System.IntPtr instrumentHandle, int channel, double magnitude);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getReflectionGammaMagn", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getReflectionGammaMagn(System.IntPtr instrumentHandle, int channel, out double magnitude);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setReflectionGammaPhase", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setReflectionGammaPhase(System.IntPtr instrumentHandle, int channel, double phase);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getReflectionGammaPhase", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getReflectionGammaPhase(System.IntPtr instrumentHandle, int channel, out double phase);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setReflectionGammaUncertainty", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setReflectionGammaUncertainty(System.IntPtr instrumentHandle, int channel, double uncertainty);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getReflectionGammaUncertainty", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getReflectionGammaUncertainty(System.IntPtr instrumentHandle, int channel, out double uncertainty);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_configureDutyCycle", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_configureDutyCycle(System.IntPtr instrumentHandle, int channel, ushort dutyCycleState, double dutyCycle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_setDutyCycle", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_setDutyCycle(System.IntPtr instrumentHandle, int channel, double dutyCycle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_getDutyCycle", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_getDutyCycle(System.IntPtr instrumentHandle, int channel, out double dutyCycle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_setDutyCycleEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_setDutyCycleEnabled(System.IntPtr instrumentHandle, int channel, ushort dutyCycleState);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_corr_getDutyCycleEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int corr_getDutyCycleEnabled(System.IntPtr instrumentHandle, int channel, out ushort dutyCycleState);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setContAvAperture", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setContAvAperture(System.IntPtr instrumentHandle, int channel, double contAvAperture);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getContAvAperture", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getContAvAperture(System.IntPtr instrumentHandle, int channel, out double contAvAperture);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setContAvSmoothingEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setContAvSmoothingEnabled(System.IntPtr instrumentHandle, int channel, ushort contAvSmoothing);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getContAvSmoothingEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getContAvSmoothingEnabled(System.IntPtr instrumentHandle, int channel, out ushort contAvSmoothing);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setContAvBufferedEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setContAvBufferedEnabled(System.IntPtr instrumentHandle, int channel, ushort contAvBufferedMode);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getContAvBufferedEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getContAvBufferedEnabled(System.IntPtr instrumentHandle, int channel, out ushort contAvBufferedMode);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setContAvBufferSize", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setContAvBufferSize(System.IntPtr instrumentHandle, int channel, int bufferSize);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getContAvBufferSize", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getContAvBufferSize(System.IntPtr instrumentHandle, int channel, out int bufferSize);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setBurstDropoutTolerance", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setBurstDropoutTolerance(System.IntPtr instrumentHandle, int channel, double dropOutTolerance);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getBurstDropoutTolerance", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getBurstDropoutTolerance(System.IntPtr instrumentHandle, int channel, out double dropOutTolerance);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setBurstChopperEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setBurstChopperEnabled(System.IntPtr instrumentHandle, int channel, ushort burstAvChopper);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getBurstChopperEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getBurstChopperEnabled(System.IntPtr instrumentHandle, int channel, out ushort burstAvChopper);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_configureTimeGate", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_configureTimeGate(System.IntPtr instrumentHandle, int channel, int selectGate, double offset, double time, double frequency);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_setOffsetTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_setOffsetTime(System.IntPtr instrumentHandle, int channel, int selectGate, double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_getOffsetTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_getOffsetTime(System.IntPtr instrumentHandle, int channel, int selectGate, out double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_setTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_setTime(System.IntPtr instrumentHandle, int channel, int selectGate, double time);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_getTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_getTime(System.IntPtr instrumentHandle, int channel, int selectGate, out double time);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_setFrequency", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_setFrequency(System.IntPtr instrumentHandle, int channel, int selectGate, double frequency);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_getFrequency", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_getFrequency(System.IntPtr instrumentHandle, int channel, int selectGate, out double frequency);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_setMidOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_setMidOffset(System.IntPtr instrumentHandle, int channel, int selectGate, double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_getMidOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_getMidOffset(System.IntPtr instrumentHandle, int channel, int selectGate, out double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_setMidLength", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_setMidLength(System.IntPtr instrumentHandle, int channel, int selectGate, double length);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_getMidLength", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_getMidLength(System.IntPtr instrumentHandle, int channel, int selectGate, out double length);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_setChopperEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_setChopperEnabled(System.IntPtr instrumentHandle, int channel, ushort timegateChopper);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_timegate_getChopperEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int timegate_getChopperEnabled(System.IntPtr instrumentHandle, int channel, out ushort timegateChopper);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_confTimegate", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_confTimegate(System.IntPtr instrumentHandle, int channel, double offset, double time, double midambleOffset, double midambleLength);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_confScale", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_confScale(System.IntPtr instrumentHandle, int channel, double referenceLevel, double range, int points);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_setOffsetTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_setOffsetTime(System.IntPtr instrumentHandle, int channel, double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getOffsetTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getOffsetTime(System.IntPtr instrumentHandle, int channel, out double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_setTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_setTime(System.IntPtr instrumentHandle, int channel, double time);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getTime(System.IntPtr instrumentHandle, int channel, out double time);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_setMidOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_setMidOffset(System.IntPtr instrumentHandle, int channel, double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getMidOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getMidOffset(System.IntPtr instrumentHandle, int channel, out double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_setMidLength", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_setMidLength(System.IntPtr instrumentHandle, int channel, double length);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getMidLength", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getMidLength(System.IntPtr instrumentHandle, int channel, out double length);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_setScaleRefLevel", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_setScaleRefLevel(System.IntPtr instrumentHandle, int channel, double referenceLevel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getScaleRefLevel", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getScaleRefLevel(System.IntPtr instrumentHandle, int channel, out double referenceLevel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_setScaleRange", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_setScaleRange(System.IntPtr instrumentHandle, int channel, double range);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getScaleRange", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getScaleRange(System.IntPtr instrumentHandle, int channel, out double range);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_setScalePoints", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_setScalePoints(System.IntPtr instrumentHandle, int channel, int points);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getScalePoints", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getScalePoints(System.IntPtr instrumentHandle, int channel, out int points);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_stat_getScaleWidth", CallingConvention = CallingConvention.StdCall)]
            public static extern int stat_getScaleWidth(System.IntPtr instrumentHandle, int channel, out double width);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_configureTimeSlot", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_configureTimeSlot(System.IntPtr instrumentHandle, int channel, int timeSlotCount, double width);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_setTimeSlotCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_setTimeSlotCount(System.IntPtr instrumentHandle, int channel, int timeSlotCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_getTimeSlotCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_getTimeSlotCount(System.IntPtr instrumentHandle, int channel, out int timeSlotCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_setTimeSlotWidth", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_setTimeSlotWidth(System.IntPtr instrumentHandle, int channel, double width);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_getTimeSlotWidth", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_getTimeSlotWidth(System.IntPtr instrumentHandle, int channel, out double width);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_setTimeSlotMidOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_setTimeSlotMidOffset(System.IntPtr instrumentHandle, int channel, double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_getTimeSlotMidOffset", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_getTimeSlotMidOffset(System.IntPtr instrumentHandle, int channel, out double offset);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_setTimeSlotMidLength", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_setTimeSlotMidLength(System.IntPtr instrumentHandle, int channel, double length);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_getTimeSlotMidLength", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_getTimeSlotMidLength(System.IntPtr instrumentHandle, int channel, out double length);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_setTimeSlotChopperEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_setTimeSlotChopperEnabled(System.IntPtr instrumentHandle, int channel, ushort timeSlotChopper);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_tslot_getTimeSlotChopperEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int tslot_getTimeSlotChopperEnabled(System.IntPtr instrumentHandle, int channel, out ushort timeSlotChopper);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_configureScope", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_configureScope(System.IntPtr instrumentHandle, int channel, int scopePoints, double scopeTime, double offsetTime, ushort realtime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_fastZero", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_fastZero(System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAverageEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAverageEnabled(System.IntPtr instrumentHandle, int channel, ushort scopeAveraging);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAverageEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAverageEnabled(System.IntPtr instrumentHandle, int channel, out ushort scopeAveraging);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAverageCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAverageCount(System.IntPtr instrumentHandle, int channel, int count);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAverageCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAverageCount(System.IntPtr instrumentHandle, int channel, out int count);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAverageTerminalControl", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAverageTerminalControl(System.IntPtr instrumentHandle, int channel, int terminalControl);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAverageTerminalControl", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAverageTerminalControl(System.IntPtr instrumentHandle, int channel, out int terminalControl);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setOffsetTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setOffsetTime(System.IntPtr instrumentHandle, int channel, double offsetTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getOffsetTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getOffsetTime(System.IntPtr instrumentHandle, int channel, out double offsetTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setPoints", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setPoints(System.IntPtr instrumentHandle, int channel, int scopePoints);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getPoints", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getPoints(System.IntPtr instrumentHandle, int channel, out int scopePoints);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setRealtimeEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setRealtimeEnabled(System.IntPtr instrumentHandle, int channel, ushort realtime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getRealtimeEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getRealtimeEnabled(System.IntPtr instrumentHandle, int channel, out ushort realtime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setTime(System.IntPtr instrumentHandle, int channel, double scopeTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getTime(System.IntPtr instrumentHandle, int channel, out double scopeTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAutoEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAutoEnabled(System.IntPtr instrumentHandle, int channel, ushort autoEnabled);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAutoEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAutoEnabled(System.IntPtr instrumentHandle, int channel, out ushort autoEnabled);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAutoMaxMeasuringTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAutoMaxMeasuringTime(System.IntPtr instrumentHandle, int channel, double upperTimeLimit);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAutoMaxMeasuringTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAutoMaxMeasuringTime(System.IntPtr instrumentHandle, int channel, out double upperTimeLimit);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAutoNoiseSignalRatio", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAutoNoiseSignalRatio(System.IntPtr instrumentHandle, int channel, double maximumNoiseRatio);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAutoNoiseSignalRatio", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAutoNoiseSignalRatio(System.IntPtr instrumentHandle, int channel, out double maximumNoiseRatio);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAutoResolution", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAutoResolution(System.IntPtr instrumentHandle, int channel, int resolution);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAutoResolution", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAutoResolution(System.IntPtr instrumentHandle, int channel, out int resolution);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_setAutoType", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_setAutoType(System.IntPtr instrumentHandle, int channel, int method);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_scope_getAutoType", CallingConvention = CallingConvention.StdCall)]
            public static extern int scope_getAutoType(System.IntPtr instrumentHandle, int channel, out int method);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_configureInternal", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_configureInternal(System.IntPtr instrumentHandle, int channel, double triggerLevel, int triggerSlope);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_configureExternal", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_configureExternal(System.IntPtr instrumentHandle, int channel, double triggerDelay);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_immediate", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_immediate(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setAutoDelayEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setAutoDelayEnabled(System.IntPtr instrumentHandle, int channel, ushort autoDelay);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getAutoDelayEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getAutoDelayEnabled(System.IntPtr instrumentHandle, int channel, out ushort autoDelay);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setAutoTriggerEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setAutoTriggerEnabled(System.IntPtr instrumentHandle, int channel, ushort autoTrigger);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getAutoTriggerEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getAutoTriggerEnabled(System.IntPtr instrumentHandle, int channel, out ushort autoTrigger);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setCount(System.IntPtr instrumentHandle, int channel, int triggerCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getCount(System.IntPtr instrumentHandle, int channel, out int triggerCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setDelay", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setDelay(System.IntPtr instrumentHandle, int channel, double triggerDelay);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getDelay", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getDelay(System.IntPtr instrumentHandle, int channel, out double triggerDelay);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setHoldoff", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setHoldoff(System.IntPtr instrumentHandle, int channel, double triggerHoldoff);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getHoldoff", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getHoldoff(System.IntPtr instrumentHandle, int channel, out double triggerHoldoff);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setHysteresis", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setHysteresis(System.IntPtr instrumentHandle, int channel, double triggerHysteresis);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getHysteresis", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getHysteresis(System.IntPtr instrumentHandle, int channel, out double triggerHysteresis);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setLevel", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setLevel(System.IntPtr instrumentHandle, int channel, double triggerLevel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getLevel", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getLevel(System.IntPtr instrumentHandle, int channel, out double triggerLevel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setSlope", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setSlope(System.IntPtr instrumentHandle, int channel, int triggerSlope);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getSlope", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getSlope(System.IntPtr instrumentHandle, int channel, out int triggerSlope);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setSource", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setSource(System.IntPtr instrumentHandle, int channel, int triggerSource);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getSource", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getSource(System.IntPtr instrumentHandle, int channel, out int triggerSource);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_setDropoutTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_setDropoutTime(System.IntPtr instrumentHandle, int channel, double dropoutTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_trigger_getDropoutTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int trigger_getDropoutTime(System.IntPtr instrumentHandle, int channel, out double dropoutTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_info", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_info(System.IntPtr instrumentHandle, int channel, string infoType, int arraySize, System.Text.StringBuilder info);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_infoHeader", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_infoHeader(System.IntPtr instrumentHandle, int channel, int parameterNumber, int arraySize, System.Text.StringBuilder header);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_infosCount", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_infosCount(System.IntPtr instrumentHandle, int channel, out int count);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_system_setStatusUpdateTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int system_setStatusUpdateTime(System.IntPtr instrumentHandle, int channel, double statusUpdateTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_system_getStatusUpdateTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int system_getStatusUpdateTime(System.IntPtr instrumentHandle, int channel, out double statusUpdateTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_system_setResultUpdateTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int system_setResultUpdateTime(System.IntPtr instrumentHandle, int channel, double resultUpdateTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_system_getResultUpdateTime", CallingConvention = CallingConvention.StdCall)]
            public static extern int system_getResultUpdateTime(System.IntPtr instrumentHandle, int channel, out double resultUpdateTime);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_abort", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_abort(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_initiate", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_initiate(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setInitContinuousEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setInitContinuousEnabled(System.IntPtr instrumentHandle, int channel, ushort continuousInitiate);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getInitContinuousEnabled", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getInitContinuousEnabled(System.IntPtr instrumentHandle, int channel, out ushort continuousInitiate);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_reset", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_reset(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setSamplingFrequency", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setSamplingFrequency(System.IntPtr instrumentHandle, int channel, int samplingFrequency);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getSamplingFrequency", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getSamplingFrequency(System.IntPtr instrumentHandle, int channel, out int samplingFrequency);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_zero", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_zero(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_zeroAdvanced", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_zeroAdvanced(System.IntPtr instrumentHandle, int channel, int zeroing);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_isZeroComplete", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_isZeroComplete(System.IntPtr instrumentHandle, int channel, out ushort zeroingComplete);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_isMeasurementComplete", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_isMeasurementComplete(System.IntPtr instrumentHandle, int channel, out ushort measurementComplete);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_selfTest", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_selfTest(System.IntPtr instrumentHandle, int channel, System.Text.StringBuilder result);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_setAuxiliary", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_setAuxiliary(System.IntPtr instrumentHandle, int channel, int auxiliaryValue);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_chan_getAuxiliary", CallingConvention = CallingConvention.StdCall)]
            public static extern int chan_getAuxiliary(System.IntPtr instrumentHandle, int channel, out int auxiliaryValue);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_readMeasurement", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_readMeasurement(System.IntPtr instrumentHandle, int channel, int timeoutMs, out double measurement);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_fetchMeasurement", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_fetchMeasurement(System.IntPtr instrumentHandle, int channel, out double measurement);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_readBufferMeasurement", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_readBufferMeasurement(System.IntPtr instrumentHandle, int channel, int maximumTimeMs, int bufferSize, [In, Out] double[] measurementArray, out int readCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_fetchBufferMeasurement", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_fetchBufferMeasurement(System.IntPtr instrumentHandle, int channel, int arraySize, [In, Out] double[] measurementArray, out int readCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_sendSoftwareTrigger", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_sendSoftwareTrigger(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_readMeasurementAux", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_readMeasurementAux(System.IntPtr instrumentHandle, int channel, int timeoutMs, out double measurement, out double aux1, out double aux2);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_fetchMeasurementAux", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_fetchMeasurementAux(System.IntPtr instrumentHandle, int channel, int timeoutMs, out double measurement, out double aux1, out double aux2);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_readBufferMeasurementAux", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_readBufferMeasurementAux(System.IntPtr instrumentHandle, int channel, int maximumTimeMs, int bufferSize, [In, Out] double[] measurementArray, [In, Out] double[] aux1Array, [In, Out] double[] aux2Array, out int readCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_meass_fetchBufferMeasurementAux", CallingConvention = CallingConvention.StdCall)]
            public static extern int meass_fetchBufferMeasurementAux(System.IntPtr instrumentHandle, int channel, int maximumTimeMs, int bufferSize, [In, Out] double[] measurementArray, [In, Out] double[] aux1Array, [In, Out] double[] aux2Array, out int readCount);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_status_preset", CallingConvention = CallingConvention.StdCall)]
            public static extern int status_preset(System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_status_checkCondition", CallingConvention = CallingConvention.StdCall)]
            public static extern int status_checkCondition(System.IntPtr instrumentHandle, int statusClass, uint mask, out ushort state);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_status_catchEvent", CallingConvention = CallingConvention.StdCall)]
            public static extern int status_catchEvent(System.IntPtr instrumentHandle, int statusClass, uint mask, int direction);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_status_checkEvent", CallingConvention = CallingConvention.StdCall)]
            public static extern int status_checkEvent(System.IntPtr instrumentHandle, int statusClass, uint mask, uint resetMask, out ushort events);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_status_enableEventNotification", CallingConvention = CallingConvention.StdCall)]
            public static extern int status_enableEventNotification(System.IntPtr instrumentHandle, int statusClass, uint mask);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_status_disableEventNotification", CallingConvention = CallingConvention.StdCall)]
            public static extern int status_disableEventNotification(System.IntPtr instrumentHandle, int statusClass, uint mask);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_status_registerWindowMessage", CallingConvention = CallingConvention.StdCall)]
            public static extern int status_registerWindowMessage(System.IntPtr instrumentHandle, uint windowHandle, uint messageId);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_errorCheckState", CallingConvention = CallingConvention.StdCall)]
            public static extern int errorCheckState(System.IntPtr instrumentHandle, ushort stateChecking);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_reset", CallingConvention = CallingConvention.StdCall)]
            public static extern int reset(System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_self_test", CallingConvention = CallingConvention.StdCall)]
            public static extern int self_test(System.IntPtr instrumentHandle, out short selfTestResult, System.Text.StringBuilder selfTestMessage);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_error_query", CallingConvention = CallingConvention.StdCall)]
            public static extern int error_query(System.IntPtr instrumentHandle, out int errorCode, System.Text.StringBuilder errorMessage);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_revision_query", CallingConvention = CallingConvention.StdCall)]
            public static extern int revision_query(System.IntPtr instrumentHandle, System.Text.StringBuilder instrumentDriverRevision, System.Text.StringBuilder firmwareRevision);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_CloseSensor", CallingConvention = CallingConvention.StdCall)]
            public static extern int CloseSensor(System.IntPtr instrumentHandle, int channel);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_close", CallingConvention = CallingConvention.StdCall)]
            public static extern int close(System.IntPtr instrumentHandle);

            [DllImport(NrpDll, EntryPoint = "rsnrpz_error_message", CallingConvention = CallingConvention.StdCall)]
            public static extern int error_message(System.IntPtr instrumentHandle, int statusCode, System.Text.StringBuilder message);

            #endregion

            public static int TestForError(System.IntPtr handle, int status)
            {
                if ((status < 0))
                {
                    PInvoke.ThrowError(handle, status);
                }
                return status;
            }

            public static int ThrowError(System.IntPtr handle, int code)
            {
                System.Text.StringBuilder msg = new System.Text.StringBuilder(256);
                PInvoke.error_message(handle, code, msg);
                throw new System.Runtime.InteropServices.ExternalException(msg.ToString(), code);
            }
        }
    }


    public class RsnrpzConstants
    {

        public const int SensorModeContav = 0;

        public const int SensorModeBufContav = 1;

        public const int SensorModeTimeslot = 2;

        public const int SensorModeBurst = 3;

        public const int SensorModeScope = 4;

        public const int SensorModeTimegate = 5;

        public const int SensorModeCcdf = 6;

        public const int SensorModePdf = 7;

        public const int AutoCountTypeResolution = 0;

        public const int AutoCountTypeNsr = 1;

        public const int TerminalControlMoving = 0;

        public const int TerminalControlRepeat = 1;

        public const int SlopePositive = 0;

        public const int SlopeNegative = 1;

        public const int TriggerSourceBus = 0;

        public const int TriggerSourceExternal = 1;

        public const int TriggerSourceHold = 2;

        public const int TriggerSourceImmediate = 3;

        public const int TriggerSourceInternal = 4;

        public const int SamplingFrequency1 = 1;

        public const int SamplingFrequency2 = 2;

        public const int ZeroLfr = 3;

        public const int ZeroUfr = 4;

        public const int AuxNone = 0;

        public const int AuxMinmax = 1;

        public const int AuxRndmax = 2;

        public const int StatclassDConn = 1;

        public const int StatclassDErr = 2;

        public const int StatclassOCal = 3;

        public const int StatclassOMeas = 4;

        public const int StatclassOTrigger = 5;

        public const int StatclassOSense = 6;

        public const int StatclassOLower = 7;

        public const int StatclassOUpper = 8;

        public const int StatclassQPower = 9;

        public const int StatclassQWindow = 10;

        public const int StatclassQCal = 11;

        public const int StatclassQZer = 12;

        public const int DirectionPtr = 1;

        public const int DirectionNtr = 2;

        public const int DirectionBoth = 3;

        public const int DirectionNone = 0;
    }
}
