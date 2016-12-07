using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class frmServer : Form
    {
        public frmServer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (button1.Text == "Start")
            {
                Program.StartServer();
                button1.Text = "Shut down";
            }
            else
            {
                Program.Shutdown();
                button1.Text = "Start";
            }

        }
    }
}
