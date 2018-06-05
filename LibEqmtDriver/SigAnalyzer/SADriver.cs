using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibEqmtDriver.SA
{
    public interface IISigAnalyzer
    {
        void Initialize(int equipId);
        void Preset();
        void Select_Instrument(N9020AInstrumentMode mode);
        void Select_Triggering(N9020ATriggeringType type);
        void Measure_Setup(N9020AMeasType type);
        void Enable_Display(N9020ADisplay onoff);
        void VBW_RATIO(double ratio);
        void Span(double freqMHz);
        void MARKER_TURN_ON_NORMAL_POINT(int markerNum, float markerFreqMHz);
        void TURN_ON_INTERNAL_PREAMP();
        void TURN_OFF_INTERNAL_PREAMP();
        void TURN_OFF_MARKER();
        double READ_MARKER(int markerNum);
        void SWEEP_TIMES(int sweeptimeMs);
        void SWEEP_POINTS(int sweepPoints);
        void CONTINUOUS_MEASUREMENT_ON();
        void CONTINUOUS_MEASUREMENT_OFF();
        void RESOLUTION_BW(double bw);
        double MEASURE_PEAK_POINT(int delayMs);
        double MEASURE_PEAK_FREQ(int delayMs);
        void VIDEO_BW(double vbwHz);
        void TRIGGER_CONTINUOUS();
        void TRIGGER_SINGLE();
        void TRIGGER_IMM();
        void TRACE_AVERAGE(int avg);
        void AVERAGE_OFF();
        void AVERAGE_ON();
        void SET_TRACE_DETECTOR(string mode);
        void CLEAR_WRITE();
        void AMPLITUDE_REF_LEVEL_OFFSET(double refLvlOffset);
        void AMPLITUDE_REF_LEVEL(double refLvl);
        void AMPLITUDE_INPUT_ATTENUATION(double inputAttenuation);
        void AUTO_ATTENUATION(bool state);
        void ELEC_ATTENUATION(float inputAttenuation);
        void ELEC_ATTEN_ENABLE(bool inputStat);
        void ALIGN_PARTIAL();
        void ALIGN_ONCE();
        void AUTOALIGN_ENABLE(bool inputStat);
        void Cal();
        bool OPERATION_COMPLETE();
        void START_FREQ(string strFreq, string strUnit);
        void STOP_FREQ(string strFreq, string strUnit);
        void FREQ_CENT(string strSaFreq, string strUnit);
        string READ_MXATrace(int traceNum);
        double READ_STARTFREQ();
        double READ_STOPFREQ();
        float READ_SWEEP_POINTS();
        float[] IEEEBlock_READ_MXATrace(int traceNum);
        void EXT_AMP_GAIN(double gain);
        void Select_MarkerFunc(N9020AMarkerFunc type);
    }
}
