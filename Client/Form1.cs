using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }
        public void EnableInput()
        {
            
            btnLogin.Enabled = true;
        }

        public void DisableInput()
        {
            
            btnLogin.Enabled = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            
            // Send
            if (!string.IsNullOrEmpty(txtLogin.Text) & !string.IsNullOrEmpty(txtPassword.Text))
            {
                string Login_Pass = "LOGIN:"+txtLogin.Text + ":" + txtPassword.Text;

                Program.Send(Login_Pass);

            }
                
           // textBox1.Text = "";

        }
    }
}
