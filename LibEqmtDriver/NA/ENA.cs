using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ivi.Visa.Interop;

namespace LibEqmtDriver.NA
{
    public class Ena:NetworkAnalyzerAbstract  
    {
        readonly int _intTimeOut = 10000;
        public Ena(string address)
        {
            try
            {
                base.OpenIo(address, _intTimeOut);
                base.Operation_Complete();
                SendCommand("FORM:DATA REAL");
            }
            catch { DisplayError(this.ToString(), "ENA Init", "ENA address mismatch -" + address); }
        }

        public override void LoadState(string filename)
        {
            SendCommand("FORM:DATA REAL");
            SendCommand("MMEM:LOAD:STAT \"" + filename.Trim() + ".STA" + "\"");
            base.Operation_Complete();
        }
        public override void SaveState(string filename)
        {
            SendCommand("FORM:DATA REAL");
            SendCommand("MMEM:STOR:STAT \"" + filename.Trim() + ".STA\"");
            base.Operation_Complete();
        }
        public override void DisplayOn(bool state)
        {
            switch (state)
            {
                case true:
                    SendCommand("DISP:ENAB ON");
                    break;
                case false:
                    SendCommand("DISP:ENAB OFF");
                    break;
            }
        }

        public override void Averaging(int channelNumber, naEnum.EOnOff val)
        {
            SendCommand("TRIG:AVER " + val);
        }

        public override void ActiveChannel(int channelNumber)
        {
            SendCommand("DISP:WIND" + channelNumber.ToString() + ":ACT");
        }
        public override void ChannelMax(int channelNumber, bool state)
        {
            switch (state)
            {
                case true:
                    SendCommand("DISP:MAX ON");
                    break;
                case false:
                    SendCommand("DISP:MAX OFF");
                    break;
            }
        }

        public override void SetCorrProperty(int channelNumber, bool enable)
        {
            switch (enable)
            {
                case true:
                    SendCommand("SENS" + channelNumber.ToString() + ":CORR:PROP ON");
                    break;
                case false:
                    SendCommand("SENS" + channelNumber.ToString() + ":CORR:PROP OFF");
                    break;
            }
        }
        public override void ClearCorr(int channelNumber)
        {
            SendCommand("SENS" + channelNumber + ":CORR:CLE");
        }
        public override void SelectSubClass(int channelNumber)
        {
            SendCommand(":SENS" + channelNumber.ToString() + ":CORR:COLL:SUBC " + channelNumber.ToString());
            SendCommand(":SENS" + channelNumber.ToString() + ":CORR:COLL:CKIT:ORD:SEL " + channelNumber.ToString());
        }
        public override void SetCorrMethodSOLT(int channelNUmber, List<int> numberPorts)
        {
            string portStr = numberPorts[0].ToString();
            for (int i = 1; i < numberPorts.Count; i++)
            {
                portStr = string.Format("{0}, {1}", portStr, numberPorts[i]);
            }

            SendCommand("SENS" + channelNUmber.ToString() + ":CORR:COLL:METH:SOLT" + numberPorts.Count + " " + portStr);
        }
        public override void MeasCorr1PortStd(int channelNumber, int portNumber, int stdNumber, string stdType)
        {
            SendCommand(":SENS" + channelNumber + ":CORR:COLL:CKIT:ORD:"+ stdType +" " + portNumber + "," + stdNumber);
            SendCommand(":SENS" + channelNumber + ":CORR:COLL:" + stdType + " " + portNumber);
            
        }
        public override void MeasCorr2PortStd(int channelNumber, int port1, int port2, int stdNumber, string stdType)
        {
            SendCommand(":SENS" + channelNumber + ":CORR:COLL:CKIT:ORD:" + stdType +" " + port1 + "," + port2 + "," + stdNumber);
            SendCommand(":SENS" + channelNumber + ":CORR:COLL:" + stdType + " " + port1 + "," + port2);
            waitTimer.Wait(500);
            SendCommand(":SENS" + channelNumber + ":CORR:COLL:" + stdType + " " + port2 + "," + port1);
        }
        public override void SaveCorr(int channelNumber)
        {
            SendCommand("SENS" + channelNumber + ":CORR:COLL:SAVE");
            base.Operation_Complete();
        }

        public override void TriggerSingle(int channelNumber)
        {
            SendCommand("INIT" + channelNumber.ToString() + ":IMM");
            SendCommand ("TRIG:SEQ:SING");
            base.Operation_Complete();
        }
        public override void TriggerSource(naEnum.ETriggerSource trigSource)
        {
            SendCommand("TRIG:SEQ:SOUR " + trigSource.ToString());
        }
        public override string GetTraceInfo(int channelNumber)
        {
            string ret = "";
            string tmpStr = "";
            int tmp = int.Parse(ReadCommand("CALC" + channelNumber.ToString() + ":PAR:COUNT?"));
            for (int i = 1; i < tmp+1;i++)
            {
                tmpStr = ReadCommand("CALC" + channelNumber.ToString() + ":PAR" + i.ToString() +":DEF?").Replace("\n",",");
                ret = ret + i.ToString() + "," + tmpStr;
            }

            ret = ret.Remove(ret.Length - 1, 1);
            return ret;
        }     

        public override naEnum.ESFormat GetTraceFormat(int channelNumber, int traceNumber)
        {
            string tmp = ReadCommand("CALC" + channelNumber.ToString() + ":TRAC" + traceNumber.ToString() + ":FORM?").Replace("\n", "");
            naEnum.ESFormat format = (naEnum.ESFormat)Enum.Parse(typeof(naEnum.ESFormat), tmp);
            return format;
        }

        public override double[] GetFreqList(int channelNumber)
        {       
            return (ReadIeeeBlock("SENS" + channelNumber.ToString() + ":FREQ:DATA?"));
        }

        public override double[] GrabRealImagData(int channelNumber)
        {
            string traceNumber="";
            int tmp = int.Parse(ReadCommand("CALC" + channelNumber.ToString() + ":PAR:COUNT?"));
            for (int i = 1; i < tmp+1;i++)
            {
                traceNumber = traceNumber + i.ToString() + ",";
            }
            traceNumber = traceNumber.Remove(traceNumber.Length - 1, 1);
            return (ReadIeeeBlock("CALC" + channelNumber.ToString() + ":SEL:DATA:MFD? \"" + traceNumber + "\""));
        }

        public override void GetSegmentTable(out SSegmentTable segmentTable, int channelNumber)
        {
            string DataFormat;
            string tmpStr;
            string[] tmpSegData;
            long tmpI;
            int iData = 3, iAdd = 0;

            segmentTable = new SSegmentTable();
            DataFormat = ReadCommand("FORM:DATA?");
            SendCommand("FORM:DATA ASC");
            tmpStr = ReadCommand("SENS" + channelNumber.ToString() + ":SEGM:DATA?");
            tmpSegData = tmpStr.Split(',');

            for (int s = 0; s < tmpSegData.Length; s++)
            {
                tmpI = (long)(Convert.ToDouble(tmpSegData[s]));
                tmpSegData[s] = tmpI.ToString();
            }

            segmentTable.Mode = (naEnum.EModeSetting)Enum.Parse(typeof(naEnum.EModeSetting), tmpSegData[1]);
            segmentTable.Ifbw = (naEnum.EOnOff)Enum.Parse(typeof(naEnum.EOnOff), tmpSegData[2]);
            segmentTable.Pow = (naEnum.EOnOff)Enum.Parse(typeof(naEnum.EOnOff), tmpSegData[3]);
            segmentTable.Del = (naEnum.EOnOff)Enum.Parse(typeof(naEnum.EOnOff), tmpSegData[4]);

            if (tmpSegData[0] == "5")
            {
                segmentTable.Swp = naEnum.EOnOff.Off;
                segmentTable.Time = (naEnum.EOnOff)Enum.Parse(typeof(naEnum.EOnOff), tmpSegData[5]);
                segmentTable.Segm = int.Parse(tmpSegData[6]);
            }
            else if (tmpSegData[0] == "6")
            {
                segmentTable.Swp = (naEnum.EOnOff)Enum.Parse(typeof(naEnum.EOnOff), tmpSegData[5]);
                segmentTable.Time = (naEnum.EOnOff)Enum.Parse(typeof(naEnum.EOnOff), tmpSegData[6]);
                segmentTable.Segm = int.Parse(tmpSegData[7]);
                iAdd = 1;
            }

            segmentTable.SegmentData = new SSegmentData[segmentTable.Segm];
            for (int iSeg = 0; iSeg < segmentTable.Segm; iSeg++)
            {
                segmentTable.SegmentData[iSeg].Start = double.Parse(tmpSegData[(iSeg * iData) + 7 + iAdd]);
                segmentTable.SegmentData[iSeg].Stop = double.Parse(tmpSegData[(iSeg * iData) + 8 + iAdd]);
                segmentTable.SegmentData[iSeg].Points = int.Parse(tmpSegData[(iSeg * iData) + 9 + iAdd]);
                tmpI = 10;
                if (segmentTable.Ifbw == naEnum.EOnOff.On)
                {
                    segmentTable.SegmentData[iSeg].IfbwValue = double.Parse(tmpSegData[(iSeg * iData) + tmpI]);
                    tmpI++;
                    if (iSeg == 0) iData++;
                }
                if (segmentTable.Pow == naEnum.EOnOff.On)
                {
                    segmentTable.SegmentData[iSeg].PowValue = double.Parse(tmpSegData[(iSeg * iData) + tmpI]);
                    tmpI++;
                    if (iSeg == 0) iData++;
                }
                if (segmentTable.Del == naEnum.EOnOff.On)
                {
                    segmentTable.SegmentData[iSeg].DelValue = double.Parse(tmpSegData[(iSeg * iData) + tmpI]);
                    tmpI++;
                    if (iSeg == 0) iData++;
                }
                if (segmentTable.Swp == naEnum.EOnOff.On)
                {
                    segmentTable.SegmentData[iSeg].SwpValue = (naEnum.ESweepMode)Enum.Parse(typeof(naEnum.ESweepMode),
                        tmpSegData[(iSeg * iData) + tmpI]);
                    tmpI++;
                    if (iSeg == 0) iData++;
                }
                if (segmentTable.Time == naEnum.EOnOff.On)
                {
                    segmentTable.SegmentData[iSeg].TimeValue = double.Parse(tmpSegData[(iSeg * iData) + tmpI]);
                    tmpI++;
                    if (iSeg == 0) iData++;
                }
            }
           
            SendCommand("FORM:DATA " + DataFormat);
        }

        internal override void InsertTrace(int channelNumber, SortedList<int, String> Traces)
        {
            SendCommand("CALC" + channelNumber + ":PAR:COUN " + Traces.Count);
            waitTimer.Wait(200);
            foreach (KeyValuePair<int, string> trace in Traces)
            {
                SendCommand("CALC" + channelNumber + ":PAR" + trace.Key + ":DEF " + trace.Value);
                SendCommand("CALC" + channelNumber + ":PAR" + trace.Key + ":SEL");
                SendCommand("CALC" + channelNumber + ":FORM " + naEnum.ESFormat.SCOM.ToString());
                base.Operation_Complete();
            }  
        }
        internal override void InsertCalKitStd(SCalStdTable stdTable)
        {
            int channelNo = stdTable.channelNo;
            int stdNo;
            ActiveChannel(channelNo);
            ChannelMax( channelNo, true);

            SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT " + stdTable.calkit_locnum);
            SendCommand(":SENS" +  channelNo + ":CORR:COLL:CKIT:LAB \"" + stdTable.calkit_label + "\"");
            base.Operation_Complete();

            for (int i = 0; i < stdTable.CalStdData.Length; i++)
            {
                stdNo = stdTable.CalStdData[i].StdNo;
                SendCommand(":SENS" +  channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":TYPE " + stdTable.CalStdData[i].StdType);
                SendCommand(":SENS" +  channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":LAB \"" + stdTable.CalStdData[i].StdLabel + "\"");

                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":C0 " + stdTable.CalStdData[i].C0_L0);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":C1 " + stdTable.CalStdData[i].C1_L1);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":C2 " + stdTable.CalStdData[i].C2_L2);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":C3 " + stdTable.CalStdData[i].C3_L3);

                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":L0 " + stdTable.CalStdData[i].C0_L0);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":L1 " + stdTable.CalStdData[i].C1_L1);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":L2 " + stdTable.CalStdData[i].C2_L2);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":L3 " + stdTable.CalStdData[i].C3_L3);

                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":DEL " + stdTable.CalStdData[i].OffsetDelay);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":Z0 " + stdTable.CalStdData[i].OffsetZ0);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":LOSS " + stdTable.CalStdData[i].OffsetLoss);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":ARB " + stdTable.CalStdData[i].ArbImp);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":FMIN " + stdTable.CalStdData[i].MinFreq);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":FMAX " + stdTable.CalStdData[i].MaxFreq);
                SendCommand(":SENS" + channelNo + ":CORR:COLL:CKIT:STAN" + stdNo + ":CHAR " + Enum.GetName(typeof( naEnum.ECalStdMedia),stdTable.CalStdData[i].Media));
                base.Operation_Complete();
            }
            ChannelMax(channelNo, false);
            waitTimer.Wait(300);
            
        }
        internal override void SetCalKitLabel(int channelNumber, string label)
        {
            SendCommand(":SENS" + channelNumber.ToString() + ":CORR:COLL:CKIT:LAB \"" + label + "\"");
        }
        internal override void InsertSegmentTableData(int channelNumber, SSegmentTable segmentTable)
        {
            
            SendCommand("FORM:DATA ASC");

            switch (segmentTable.Swp)
            {
                case naEnum.EOnOff.On:
                    SendCommand("SENS" + channelNumber.ToString() + ":SEGM:DATA 6," + Convert_SegmentTable2String(segmentTable));
                    break;
                case naEnum.EOnOff.Off:
                    SendCommand("SENS" + channelNumber.ToString() + ":SEGM:DATA 5," + Convert_SegmentTable2String(segmentTable ));
                    break;
            }

            SendCommand("FORM:DATA REAL");
            base.Operation_Complete();
            waitTimer.Wait(300);
        }

        internal override string GetCalKitLabel(int channelNumber)
        {
            return (ReadCommand(":SENS" + channelNumber.ToString() + ":CORR:COLL:CKIT:LAB?"));
        }

        private string Convert_SegmentTable2String(SSegmentTable SegmentTable)
        {
            string tmpStr;
            tmpStr = "";
            naEnum.EOnOff sweepmode = SegmentTable.Swp;
            switch (sweepmode)
            {
                case naEnum.EOnOff.On:
                    tmpStr = ((int)SegmentTable.Mode).ToString();
                    tmpStr += "," + ((int)SegmentTable.Ifbw).ToString();
                    tmpStr += "," + ((int)SegmentTable.Pow).ToString();
                    tmpStr += "," + ((int)SegmentTable.Del).ToString();
                    tmpStr += "," + ((int)SegmentTable.Swp).ToString();
                    tmpStr += "," + ((int)SegmentTable.Time).ToString();
                    tmpStr += "," + SegmentTable.Segm.ToString();
                    for (int Seg = 0; Seg < SegmentTable.SegmentData.Length; Seg++)
                    {
                        tmpStr += "," + SegmentTable.SegmentData[Seg].Start.ToString();
                        tmpStr += "," + SegmentTable.SegmentData[Seg].Stop.ToString();
                        tmpStr += "," + SegmentTable.SegmentData[Seg].Points.ToString();
                        if (SegmentTable.Ifbw == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].IfbwValue.ToString();
                        if (SegmentTable.Pow == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].PowValue.ToString();
                        if (SegmentTable.Del == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].DelValue.ToString();
                        if (SegmentTable.Swp == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].SwpValue.ToString();
                        if (SegmentTable.Time == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].TimeValue.ToString();
                    }
                    break;
                case naEnum.EOnOff.Off:
                    tmpStr = ((int)SegmentTable.Mode).ToString();
                    tmpStr += "," + ((int)SegmentTable.Ifbw).ToString();
                    tmpStr += "," + ((int)SegmentTable.Pow).ToString();
                    tmpStr += "," + ((int)SegmentTable.Del).ToString();
                    tmpStr += "," + ((int)SegmentTable.Time).ToString();
                    tmpStr += "," + SegmentTable.Segm.ToString();
                    for (int Seg = 0; Seg < SegmentTable.SegmentData.Length; Seg++)
                    {
                        tmpStr += "," + SegmentTable.SegmentData[Seg].Start.ToString();
                        tmpStr += "," + SegmentTable.SegmentData[Seg].Stop.ToString();
                        tmpStr += "," + SegmentTable.SegmentData[Seg].Points.ToString();
                        if (SegmentTable.Ifbw == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].IfbwValue.ToString();
                        if (SegmentTable.Pow == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].PowValue.ToString();
                        if (SegmentTable.Del == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].DelValue.ToString();
                        if (SegmentTable.Time == naEnum.EOnOff.On)
                            tmpStr += "," + SegmentTable.SegmentData[Seg].TimeValue.ToString();
                    }
                    break;
            }
            return (tmpStr);
        }
    }
}
