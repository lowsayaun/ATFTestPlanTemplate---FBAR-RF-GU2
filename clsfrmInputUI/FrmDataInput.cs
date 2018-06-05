using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
//using ClothoLibStandard;

namespace clsfrmInputUI
{
    public partial class FrmDataInput : Form
    {
        private string _lotId = "";
        private string _subLotId = "";
        private string _operatorId = "";
        private string _handlerId = "";
        private string _contactorId = "";
        private string _deviceId = "";
        private string _loadBoardId = "";
        private string _mfgLotId = "";
        public string  Date;
        public string Shift;
        public int PassCharCount = 0;
        public bool EnablePassword = true;
        public bool AdminLevel = false;

        public string SetTitle
        {
            set
            {
                this.Text = value;
            }
        }
        public string LotId
        {
            get
            {
                return _lotId;
            }
            set
            {
                this.Text = value;
            }
        }
        public string ContactorId
        {
            get
            {
                return _contactorId;
            }
            set
            {
                this.Text = value;
            }
        }
        public string HandlerId
        {
            get
            {
                return _handlerId;
            }
            set
            {
                this.Text = value;
            }
        }
        public string OperatorId
        {
            get
            {
                return _operatorId;

            }
            set
            {
                this.Text = value;
            }
        }
        public string SublotId
        {
            get
            {
                return _subLotId;

            }
            set
            {
                this.Text = value;
            }
        }
        public string LoadBoardId
        {
            get
            {
                return _loadBoardId;
            }
            set
            {
                this.Text = value;
            }
        }
        public string MfgLotId
        {
            get
            {
                return _mfgLotId;
            }
            set
            {
                this.Text = value;
            }
        }
        public string DeviceId
        {
            get
            {
                return _deviceId;
            }
            set
            {
                this.Text = value;
            }
        }

        public FrmDataInput()
        {
            InitializeComponent();
        }

        private void FrmDataInput_Load(object sender, EventArgs e)
        {
            DateTime timenow = DateTime.Now;
            Date = timenow.ToString("ddMMyy");
            string sttime = timenow.ToString("HHmmss");
            CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek;
            DayOfWeek firstWeekDay = DayOfWeek.Monday;
            Calendar calendar = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar;
            int currentWeek = calendar.GetWeekOfYear(timenow, weekRule, firstWeekDay);
            double lgtime = Convert.ToDouble(sttime);
            if (lgtime > 170000 || lgtime < 70000) Shift = "N";
            else if (lgtime > 70000 || lgtime < 170000) Shift = "M";

            this.txtOperatorID.Select();
            AdminLevel = false;
        }
        
        #region KeyPressEvent - set focus
        
        //1 - Operator ID
        private void txtOperatorID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                txtLotID.Focus();
            }

            else if (!Char.IsDigit(e.KeyChar) && !Char.IsLetter(e.KeyChar) && (Convert.ToByte(e.KeyChar) != 0x08))
            {
                e.KeyChar = Convert.ToChar(0);
            }
        }
        
        //2 - Lot ID
        private void txtLotID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                txtSubLotID.Focus();
            }
            else if (!Char.IsLetter(e.KeyChar) && !Char.IsDigit(e.KeyChar) && (e.KeyChar.ToString() != "-"))
            {
               e.KeyChar = Convert.ToChar(0);
            }
        }
        
        //3 - Mfg ID
        private void txtMfgLotID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                txtDeviceID.Focus();
            }
            else if (!Char.IsDigit(e.KeyChar) && !Char.IsLetter(e.KeyChar) && (e.KeyChar.ToString() != "-"))
            {
                e.KeyChar = Convert.ToChar(0);
            }
        }

        //4 - Device ID
        private void txtDeviceID_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                txtSubLotID.Focus();
            }
            else if (!Char.IsDigit(e.KeyChar) && !Char.IsLetter(e.KeyChar) && (e.KeyChar.ToString() != "-") && (e.KeyChar.ToString() != "_"))
            {
                e.KeyChar = Convert.ToChar(0);
            }
        }

        //5 - Sub Lot ID
        private void txtSubLotID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                txtHandlerID.Focus();
            }
            else if (!Char.IsDigit(e.KeyChar) && !Char.IsLetter(e.KeyChar) && (Convert.ToByte(e.KeyChar) != 0x08))
            {
                e.KeyChar = Convert.ToChar(0);
            }
        }

        //6 - Handler ID
        private void txtHandlerID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                txtContactorID.Focus();
            }
            else if (!Char.IsDigit(e.KeyChar) && !Char.IsLetter(e.KeyChar))
            {
                e.KeyChar = Convert.ToChar(0);
            }
        }

        //7 - Contactor ID
        private void txtContactorID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                txtLbID.Focus();
            }
            else if (!Char.IsLetter(e.KeyChar) && !Char.IsDigit(e.KeyChar) && (e.KeyChar.ToString() != "-"))
            {
                e.KeyChar = Convert.ToChar(0);
            }
        }
        
        //8 - Load board ID
        private void txtLbID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button1.Focus();
            }
            else if (!Char.IsDigit(e.KeyChar) && !Char.IsLetter(e.KeyChar) && (e.KeyChar.ToString() != "-"))
            {
                e.KeyChar = Convert.ToChar(0);
            }
        }

        #endregion KeyPressEvent

        #region EnterEvent

        private void txtOperatorID_Enter(object sender, EventArgs e)
        {
            txtOperatorID.SelectAll();
        }
        private void txtLotID_Enter(object sender, EventArgs e)
        {
            txtLotID.SelectAll();
        }
        private void txtMfgLotID_Enter(object sender, EventArgs e)
        {
            txtMfgLotID.SelectAll();
        }
        private void txtDeviceID_Enter(object sender, EventArgs e)
        {
            txtDeviceID.SelectAll();
        }
        private void txtSubLotID_Enter(object sender, EventArgs e)
        {
            txtSubLotID.SelectAll();
        }              
        private void txtHandlerID_Enter(object sender, EventArgs e)
        {
            txtHandlerID.SelectAll();
        }
        private void txtContactorID_Enter(object sender, EventArgs e)
        {
            txtContactorID.SelectAll();
        }        
        private void txtLbID_Enter(object sender, EventArgs e)
        {
            txtLbID.SelectAll();
        }

        #endregion EnterEvent

        #region MouseDownEvent

        private void txtOperatorID_MouseDown(object sender, MouseEventArgs e)
        {
            txtOperatorID.SelectAll();
        }

        private void txtLotID_MouseDown(object sender, MouseEventArgs e)
        {
            txtLotID.SelectAll();
        }

        private void txtMfgLotID_MouseDown(object sender, MouseEventArgs e)
        {
            txtMfgLotID.SelectAll();
        }

        private void txtDeviceID_MouseDown_1(object sender, MouseEventArgs e)
        {
            txtDeviceID.SelectAll();
        }

        private void txtSubLotID_MouseDown(object sender, MouseEventArgs e)
        {
            txtSubLotID.SelectAll();
        }
        
        private void txtHandlerID_MouseDown(object sender, MouseEventArgs e)
        {
            txtHandlerID.SelectAll();
        }

        private void txtContactorID_MouseDown(object sender, MouseEventArgs e)
        {
            txtContactorID.SelectAll();
        }

        private void txtLbID_MouseDown(object sender, MouseEventArgs e)
        {
            txtLbID.SelectAll();
        }

        #endregion MouseDownEvent

        #region KeydownEvent

        private void txtOperatorID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                txtLotID.Focus();
        }  
        private void txtLotID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                e.Handled = true;

            if (e.KeyCode == Keys.Tab)
                txtMfgLotID.Focus();
        }
        private void txtMfgLotID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                txtDeviceID.Focus();
        }
        private void txtDeviceID_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                e.Handled = true;

            if (e.KeyCode == Keys.Tab)
                txtSubLotID.Focus();
        } 
        private void txtSubLotID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                txtHandlerID.Focus();
        }
        private void txtHandlerID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                txtContactorID.Focus();
        }                           
        private void txtContactorID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                e.Handled = true;

            if (e.KeyCode == Keys.Tab)
                txtLbID.Focus();
        }
        private void txtLbID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                e.Handled = true;

            if (e.KeyCode == Keys.Tab)
                button1.Focus();
        }
        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                txtOperatorID.Focus();
        }

        #endregion KeydownEvent
                       
        #region Other Event
        
        //Picture click enable password char  
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PassCharCount++;

            if (PassCharCount % 2 != 0) //odd   
            {
                EnablePassword = true;
                txtOperatorID.PasswordChar = '*';
            }
            else //even
            {
                EnablePassword = false;
                txtOperatorID.PasswordChar = '\0';
            }
        }

        #endregion Other Event
        
        //Ok button - Entry checking
        private void button1_Click(object sender, EventArgs e)
        {
            //Set to true if entry field is N/A
            bool passflagContId = false;
            bool passflagOpid = false;
            bool passflagLbid = false;
            bool passflagHandlerId = false;
            bool passflagLotId = false;
            bool passflagSubLotId = false;
            bool passflagMfgId = true;
            bool passflagDeviceId = true; //Jupiter only (can be used as Assembly ID for other products)

            //Admin mode
            if (EnablePassword == true && txtOperatorID.Text.ToUpper() == "AVGO155")
            {
                txtOperatorID.Text = "A0001";
                txtLotID.Text = "PT0000000001";
                txtSubLotID.Text = "1A";
                txtHandlerID.Text = "EIS001";
                txtContactorID.Text = "YP-1234-001";
                txtMfgLotID.Text = "000001";
                txtLbID.Text = "JP0001";
                txtDeviceID.Text = "AFEM-8030-A";
                AdminLevel = true;
            }

            #region OperatorID checking

            List<bool> rxOpId = new List<bool>();

            //Inari:
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[I]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[P]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[N]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[W]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[D]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[L]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[R]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[A]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[C]\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^INT\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^FWI\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^FWN\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^FWM\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^FWP\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^ISK\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^FWR\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^T\d{4}"));
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^AM\d{1,8}")); //Amkor
            rxOpId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtOperatorID.Text, @"^[0-9]{1,6}")); //ASEKr

            foreach (bool rxOp in rxOpId)
            {
                if (rxOp)
                {
                    _operatorId = txtOperatorID.Text;
                    passflagOpid = true;
                    break;
                }
            }

            if (!passflagOpid)
                MessageBox.Show("No matching for Operator ID " + "(" + txtOperatorID.Text + ")" + ", please re-enter!");

            #endregion

            #region LotID checking

            List<bool> rxLotId = new List<bool>();

            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^STDUNIT\d{2}-\d{6}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^STDUNIT\d{3}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^BINCHECK-\d{1,10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^ENGR-\d{1,10}"));

            //PA
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^PT\d{10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^PT\d{10}-\w{1}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^PT\d{10}-\w{2}"));

            //ASEKr
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^W\d{8}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^W\w{8}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^W\d{8}-\w{1,5}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^W\d{3}\w{2}\d{3}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^W\d{1,4}\w{1,3}\d{1,4}"));

            //Amkor
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^M\d{8}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^M\d{8}-\w{1,5}"));
            
            //Fbar
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^FT\d{10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^FT\d{10}-\w{1}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^FT\d{10}-\w{2}"));

            //MM
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^MT\d{10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^MU\d{10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^MI\d{10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^MC\d{10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^MA\d{10}"));
            rxLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^VT\d{10}"));

            foreach (bool rxLot in rxLotId)
            {
                if (rxLot)
                {
                    if (txtLotID.Text.Contains("STDUNIT"))
                    {
                        _lotId = txtLotID.Text + "-" + Date + "-" + Shift;
                    }
                    else if (txtLotID.Text.Contains("ENG"))
                    {
                        if (txtLotID.Text.Length > 13) _lotId = txtLotID.Text.Remove(13);
                        else _lotId = txtLotID.Text;
                    }
                    else
                    {
                        if (txtLotID.Text.Length > 14) _lotId = txtLotID.Text.Remove(14);
                        else _lotId = txtLotID.Text;
                    }

                    passflagLotId = true;
                    break;
                }
            }

            if (!passflagLotId)
                MessageBox.Show("No matching for Lot ID " + "(" + txtLotID.Text + ")" + ", please re-enter!");

            #endregion

            #region MfgID checking

            int iMfgId = 0;
            List<bool> rxMfgId = new List<bool>();

            rxMfgId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtMfgLotID.Text.ToUpper(), @"^[0-9]{6}"));
            rxMfgId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtMfgLotID.Text.ToUpper(), @"^\w[NA]"));

            foreach (bool rxMfg in rxMfgId)
            {
                if (rxMfg)
                {
                    _mfgLotId = txtMfgLotID.Text;
                    passflagMfgId = true;
                    break;
                }
            }

            try
            {
                iMfgId = Convert.ToInt32(txtMfgLotID.Text);
            }
            catch
            {
                //passflag_MfgID = false;
            }

            if (iMfgId > 131071)
            {
                passflagMfgId = false;
                MessageBox.Show("Mfg Lot Number cannot bigger than 131071 <= " + "(" + txtMfgLotID.Text + ")" + ", please re-enter!");
            }

            if (!passflagMfgId)
                MessageBox.Show("No matching for MfgLot ID " + "(" + txtMfgLotID.Text + ")" + ", please re-enter!");

            #endregion MfgID Check

            #region Device ID checking

            List<bool> rxDeviceId = new List<bool>();
            rxDeviceId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[A]$")); //Only allow AFEM-8030-A
            rxDeviceId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[M]\w{0}[B]$")); //Only allow AFEM-8030-MB
            rxDeviceId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[M]\w{0}[T]$")); //Only allow AFEM-8030-MT
            rxDeviceId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[A]\w{0}[C]$")); //Only allow AFEM-8030-AC
            rxDeviceId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[M]\w{0}[B]\w{0}[C]$")); //Only allow AFEM-8030-MBC
            rxDeviceId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[M]\w{0}[T]\w{0}[C]$")); //Only allow AFEM-8030-MTC
            rxDeviceId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^NA$")); //Only allow AFEM-8030-MTC

            foreach (bool rxDevId in rxDeviceId)
            {
                if (rxDevId)
                {
                    _deviceId = txtDeviceID.Text;
                    passflagDeviceId = true;
                    break;
                }
            }

            //ASEkr checking:
            bool ka, ke = false;
            bool aSl, acSl = false;
            ka = System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^W\d{3}[K]\w{0}[A]\w{0}\d{3}$"); //from lot ID
            ke = System.Text.RegularExpressions.Regex.IsMatch(txtLotID.Text, @"^W\d{3}[K]\w{0}[E]\w{0}\d{3}$"); //from lot ID
            aSl = System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[A]_SL$");
            acSl = System.Text.RegularExpressions.Regex.IsMatch(txtDeviceID.Text, @"^AFEM-\d{4}-\w{0}[A]\w{0}[C]_SL$");
            _deviceId = txtDeviceID.Text;

            if (ka)
            {
                passflagDeviceId = (aSl == true ? true : false);
            }
            else if (ke)
            {
                passflagDeviceId = (acSl == true ? true : false);
            }

            if (passflagDeviceId)
                _deviceId = txtDeviceID.Text;
            else
                MessageBox.Show("No matching for Device ID " + "(" + txtDeviceID.Text + ")" + ", please re-enter!");

            #endregion

            #region Sublot ID checking

            List<bool> rxSubLotId = new List<bool>();

            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^1\w{0}[ABCD]$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^2\w{0}[ABCD]$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^3\w{0}[ABCD]$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^RE$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^LYT$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^PE$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^QA$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^QA1$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^QAR$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^EO$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^EO1$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^QE$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^COQ$"));
            rxSubLotId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtSubLotID.Text, @"^CCOQ$"));



            foreach (bool rxSlotId in rxSubLotId)
            {
                if (rxSlotId)
                {
                    _subLotId = txtSubLotID.Text;
                    passflagSubLotId = true;
                    break;
                }
            }

            if (!passflagSubLotId)
                MessageBox.Show("No matching for Sub lot ID " + "(" + txtSubLotID.Text + ")" + ", please re-enter!");

            #endregion Sublot ID checking

            #region HandlerID checking

            List<bool> rxHandlerId = new List<bool>();
            rxHandlerId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtHandlerID.Text, @"^EIS\d{2}", RegexOptions.IgnoreCase));
            rxHandlerId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtHandlerID.Text, @"^EIS\d{3}", RegexOptions.IgnoreCase));          
            rxHandlerId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtHandlerID.Text, @"^SRM\d{3}", RegexOptions.IgnoreCase));
            rxHandlerId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtHandlerID.Text, @"^S9\d{4}", RegexOptions.IgnoreCase));

            foreach (bool rxHandler in rxHandlerId)
            {
                if (rxHandler)
                {
                    _handlerId = txtHandlerID.Text;
                    passflagHandlerId = true;
                    break;
                }
            }

            if (!passflagHandlerId)
                MessageBox.Show("No matching for Handler SN " + "(" + txtHandlerID.Text + ")" + ", please re-enter!");

            #endregion

            #region ContactorID checking

            List<bool> rxContactorId = new List<bool>();
            rxContactorId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtContactorID.Text, @"^YPJP\d{4}", RegexOptions.IgnoreCase)); //Jupiter
            rxContactorId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtContactorID.Text, @"^SP\d{2}", RegexOptions.IgnoreCase)); //Spyro
            rxContactorId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtContactorID.Text, @"^YP-\d{4}-\d{3}", RegexOptions.IgnoreCase));
            rxContactorId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtContactorID.Text, @"^LN-\d{4}-\d{3}", RegexOptions.IgnoreCase));

            foreach (bool rxContactor in rxContactorId)
            {
                if (rxContactor)
                {
                    _contactorId = txtContactorID.Text;
                    passflagContId = true;
                    break;
                }
            }

            if (!passflagContId)
                MessageBox.Show("No matching for Contactor ID " + "(" + txtContactorID.Text + ")" + ", please re-enter!");

            #endregion

            #region LoadboardID checking

            List<bool> rxLboardId = new List<bool>();
            rxLboardId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLbID.Text, @"^LB-\d{4}-\d{3}", RegexOptions.IgnoreCase));
            rxLboardId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLbID.Text, @"^JP\d{4}", RegexOptions.IgnoreCase));
            rxLboardId.Add(System.Text.RegularExpressions.Regex.IsMatch(txtLbID.Text, @"^LBSP", RegexOptions.IgnoreCase));

            foreach (bool rxLbId in rxLboardId)
            {
                if (rxLbId)
                {
                    _loadBoardId = txtContactorID.Text;
                    passflagLbid = true;
                    break;
                }
            }

            if (!passflagLbid)
                MessageBox.Show("No matching for Contactor ID " + "(" + txtLbID.Text + ")" + ", please re-enter!");

            #endregion LoadboardID checking

            if (passflagContId && passflagLbid && passflagOpid && passflagHandlerId && passflagLotId && passflagSubLotId && passflagDeviceId && passflagMfgId)
                this.DialogResult = DialogResult.OK;
        }     
    }
}
