using System;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver.SCU
{
    public class Agilent3499:IISwitch 
    {
        public static string ClassName = "3449A Switch Control Unit Class";
        private FormattedIO488 _myVisaEq = new FormattedIO488();
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
                return _myVisaEq;
            }
            set
            {
                _myVisaEq = ParseIo;
            }
        }
        public void OpenIo()
        {
            if (IoAddress.Length > 3)
            {
                try
                {
                    ResourceManager mgr = new ResourceManager();
                    _myVisaEq.IO = (IMessage)mgr.Open(IoAddress, AccessMode.NO_LOCK, 2000, "");
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("Class Name: " + ClassName + "\nParameters: OpenIO" + "\n\nErrorDesciption: \n"
                        + ex, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _myVisaEq.IO = null;
                    return;
                }
            }
        }

        //Constructor
        public Agilent3499(string ioAddress) 
        {
            Address = ioAddress;
            OpenIo();
        }
        Agilent3499() { }

        #region iSwitch Members

        void IISwitch.Initialize()
        {
            try
            {
                _myVisaEq.WriteString("*CLS; *RST", true);
            }
            catch (Exception ex)
            {
                throw new Exception("Agilent3499: Initialize -> " + ex.Message);
            }
        }

        void IISwitch.SetPath(string val)
        {
            string[] tempdata;
            tempdata = val.Split(';');

            try
            {
                for (int i = 0; i < tempdata.Length; i++)
                {
                    _myVisaEq.WriteString(tempdata[i], true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Agilent3499: SetPath -> " + ex.Message);
            }
        }

        void IISwitch.Reset()
        {
             try
             {
                 _myVisaEq.WriteString("*CLS; *RST", true);
             }
             catch (Exception ex)
             {
                 throw new Exception("Agilent3499: Reset -> " + ex.Message);
             }
        }

        #endregion

        private void Write(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
        }
        private void SW_control(string statusSw,string switchSlot)
        {
            _myVisaEq.WriteString(statusSw +" (@"+switchSlot+")", true);
        }
        private double WRITE_READ_DOUBLE(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
            return Convert.ToDouble(_myVisaEq.ReadString());
        }
        private string WRITE_READ_STRING(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
            return _myVisaEq.ReadString();
        }
        private float WRITE_READ_SINGLE(string cmd)
        {
            _myVisaEq.WriteString(cmd, true);
            return Convert.ToSingle(_myVisaEq.ReadString());
        }



      
    }
}
