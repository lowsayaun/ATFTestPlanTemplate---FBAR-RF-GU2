using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Visa.Interop;
using System.Windows.Forms;

namespace LibEqmtDriver
{
    public class LibEqmtCommon
    {
        internal FormattedIO488 MyIo;
        internal Utility.HiPerfTimer waitTimer = new Utility.HiPerfTimer();

        public void OpenIo(string ioAddress, int intTimeOut = 5000)
        {
            try
            {
                ResourceManager mgr = new ResourceManager();
                MyIo = new FormattedIO488 {IO = (IMessage) mgr.Open(ioAddress, AccessMode.NO_LOCK, intTimeOut, "")};
                
            }
            catch (SystemException)
            {
                //common.DisplayError(ClassName, "Initialize IO Error", ex.Message);
                MyIo.IO = null;
                return;
            }
        }
        public void CloseIo()
        {
            MyIo.IO.Close();
        }
        public void Reset()
        {
            SendCommand("*RST");
            waitTimer.Wait(2000);
        }
        public void SendCommand(string cmd)
        {
            MyIo.WriteString(cmd, true);
            //Operation_Complete();
        }
        public void SendCommand(string cmd, bool flushAndEnd)
        {
            MyIo.WriteString(cmd, flushAndEnd);
            //Operation_Complete();
        }
        public string ReadCommand(string cmd)
        {
            MyIo.WriteString(cmd, true);
            return (MyIo.ReadString());
        }
        public string ReadCommand(string cmd, bool flushAndEnd)
        {
            MyIo.WriteString(cmd, flushAndEnd);
            return (MyIo.ReadString());
        }
        public string ReadCommand()
        {
            return (MyIo.ReadString());
        }
        public double[] ReadIeeeBlock(string cmd)
        {
            MyIo.WriteString(cmd, true);
            return ((double[])MyIo.ReadIEEEBlock(IEEEBinaryType.BinaryType_R8, true, true));
        }
        public double[] ReadIeeeBlock(string cmd, IEEEBinaryType binaryType)
        {
            MyIo.WriteString(cmd, true);
            return ((double[])MyIo.ReadIEEEBlock(binaryType, true, true));
        }
        public int Operation_Complete()
        {
            return (Convert.ToInt32(ReadCommand("*OPC?")));
        }

        internal void DisplayError(string ClassName, string ErrParam, string ErrDesc)
        {
            MessageBox.Show("Class Name: " + ClassName + "\nParameters: " + ErrParam + "\n\nErrorDesciption: \n"
                + ErrDesc, "Error found in Class " + ClassName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
        public void DisplayMessage(string ClassName, string DescParam, string DescDetail)
        {
            MessageBox.Show("Class Name: " + ClassName + "\nParameters: " + DescParam + "\n\nDesciption: \n"
                + DescDetail, ClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
