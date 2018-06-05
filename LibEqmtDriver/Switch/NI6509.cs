using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.DAQmx;

namespace LibEqmtDriver.SCU
{
    public class Ni6509 : IISwitch 
    {
        private static ArrayList _taskList;

        private static Task _digitalWriteTaskP00;
        private static Task _digitalWriteTaskP01;
        private static Task _digitalWriteTaskP02;
        private static Task _digitalWriteTaskP03;
        private static Task _digitalWriteTaskP04;
        private static Task _digitalWriteTaskP05;

        private static Task _digitalWriteTaskP09;

        private static DigitalSingleChannelWriter _writerP00;
        private static DigitalSingleChannelWriter _writerP01;
        private static DigitalSingleChannelWriter _writerP02;
        private static DigitalSingleChannelWriter _writerP03;
        private static DigitalSingleChannelWriter _writerP04;
        private static DigitalSingleChannelWriter _writerP05;

        private static DigitalSingleChannelWriter _writerP09;

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
        //Constructor
        public Ni6509(string ioAddress)
        {
            Address = ioAddress;
            Initialize();
        }
        Ni6509() { }


        #region iSwitch Interface

        public void Initialize()
        {
            try
            {
                _digitalWriteTaskP00 = new Task();
                _digitalWriteTaskP01 = new Task();
                _digitalWriteTaskP02 = new Task();
                _digitalWriteTaskP03 = new Task();
                _digitalWriteTaskP04 = new Task();
                _digitalWriteTaskP05 = new Task();

                _digitalWriteTaskP09 = new Task();

                _digitalWriteTaskP00.DOChannels.CreateChannel(IoAddress + "/port0", "port0",
                                ChannelLineGrouping.OneChannelForAllLines);
                _digitalWriteTaskP01.DOChannels.CreateChannel(IoAddress + "/port1", "port1",
                                ChannelLineGrouping.OneChannelForAllLines);
                _digitalWriteTaskP02.DOChannels.CreateChannel(IoAddress + "/port2", "port2",
                                ChannelLineGrouping.OneChannelForAllLines);
                _digitalWriteTaskP03.DOChannels.CreateChannel(IoAddress + "/port3", "port3",
                                ChannelLineGrouping.OneChannelForAllLines);
                _digitalWriteTaskP04.DOChannels.CreateChannel(IoAddress + "/port4", "port4",
                                ChannelLineGrouping.OneChannelForAllLines);
                _digitalWriteTaskP05.DOChannels.CreateChannel(IoAddress + "/port5", "port5",
                                ChannelLineGrouping.OneChannelForAllLines);

                _digitalWriteTaskP09.DOChannels.CreateChannel(IoAddress + "/port9", "port9",
                                ChannelLineGrouping.OneChannelForAllLines);

                _writerP00 = new DigitalSingleChannelWriter(_digitalWriteTaskP00.Stream);
                _writerP01 = new DigitalSingleChannelWriter(_digitalWriteTaskP01.Stream);
                _writerP02 = new DigitalSingleChannelWriter(_digitalWriteTaskP02.Stream);
                _writerP03 = new DigitalSingleChannelWriter(_digitalWriteTaskP03.Stream);
                _writerP04 = new DigitalSingleChannelWriter(_digitalWriteTaskP04.Stream);
                _writerP05 = new DigitalSingleChannelWriter(_digitalWriteTaskP05.Stream);

                _writerP09 = new DigitalSingleChannelWriter(_digitalWriteTaskP09.Stream);

                _writerP00.WriteSingleSamplePort(true, 0);
                _writerP01.WriteSingleSamplePort(true, 0);
                _writerP02.WriteSingleSamplePort(true, 0);
                _writerP03.WriteSingleSamplePort(true, 0);
                _writerP04.WriteSingleSamplePort(true, 0);
                _writerP05.WriteSingleSamplePort(true, 0);
                _writerP09.WriteSingleSamplePort(true, 0);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Initialize");
            }
        }

        public void SetPath(string val)
        {
            string[] tempdata;
            tempdata = val.Split(';');
            string[] tempdata2;
            
            try
            {
                for (int i = 0; i < tempdata.Length; i++)
                {
                    tempdata2 = tempdata[i].Split('_');
                    
                    switch(tempdata2[0].ToUpper())
                    {
                        case "P0":
                            _writerP00.WriteSingleSamplePort(true, Convert.ToUInt32(tempdata2[1]));
                            break;
                        case "P1":
                            _writerP01.WriteSingleSamplePort(true, Convert.ToUInt32(tempdata2[1]));
                            break;
                        case "P2":
                            _writerP02.WriteSingleSamplePort(true, Convert.ToUInt32(tempdata2[1]));
                            break;
                        case "P3":
                            _writerP03.WriteSingleSamplePort(true, Convert.ToUInt32(tempdata2[1]));
                            break;
                        case "P4":
                            _writerP04.WriteSingleSamplePort(true, Convert.ToUInt32(tempdata2[1]));
                            break;
                        case "P5":
                            _writerP05.WriteSingleSamplePort(true, Convert.ToUInt32(tempdata2[1]));
                            break;
                        case "P9":
                            _writerP09.WriteSingleSamplePort(true, Convert.ToUInt32(tempdata2[1]));
                            break;
                        default :
                            MessageBox.Show("Port No : " + tempdata2[1].ToUpper(), "Only P0,P1,P2,P3,P4,P5 AND P9 ALLOWED !!!!\n" + "Pls check your switching configuration in Input Folder");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Agilent3499: SetPath -> " + ex.Message);
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
