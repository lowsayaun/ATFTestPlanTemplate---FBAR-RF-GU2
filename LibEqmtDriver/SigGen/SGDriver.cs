using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEqmtDriver.SG
{
    public interface IISiggen
    {
        void Initialize();
        void Reset();
        void EnableModulation(InstrOutput onoff);
        void EnableRf(InstrOutput onoff);
        void SetFreq(double mHz);
        void SetAmplitude(float dBm);
        void SetPowerMode(N5182PowerMode mode);
        void SetFreqMode(N5182FrequencyMode mode);
        void MOD_FORMAT_WITH_LOADING_CHECK(string strWaveform, string strWaveformName, bool waveformInitalLoad);
        void SELECT_WAVEFORM(N5182AWaveformMode mode);
        void SET_LIST_TYPE(N5182ListType mode);
        void SET_LIST_MODE(InstrMode mode);
        void SET_LIST_TRIG_SOURCE(N5182TrigType mode);
        void SET_CONT_SWEEP(InstrOutput onoff);
        void SET_START_FREQUENCY(double mHz);
        void SET_STOP_FREQUENCY(float mHz);
        void SET_TRIG_TIMERPERIOD(double ms);
        void SET_SWEEP_POINT(int points);
        void SINGLE_SWEEP();
        void SET_SWEEP_PARAM(int points, double ms, double startFreqMHz, double stopFreqMHz);
        bool OPERATION_COMPLETE();
    }
}
