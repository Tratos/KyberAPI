using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KyberAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.box = rtb1;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!JSONServer._exit) JSONServer.Stop();
            Application.Exit();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            JSONServer.basicMode = false;
            JSONServer.Start();
        }
    }
}
