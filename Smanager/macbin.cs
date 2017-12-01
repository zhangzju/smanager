using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Smanager
{
    public partial class macbin : Form
    {
        private bool is2gssid;
        private bool is5gssid;
        private bool ispppoe;
        private bool ispppoeusername;
        private Int32 pppoeusernamelength;
        private bool ispppoepassword;
        private Int32 pppoepasswordlength;
        private Int32 count;
        private string outputPath;
        private string inputFile;

        public macbin()
        {
            InitializeComponent();

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                foreach (Control c in this.groupBox3.Controls)
                {
                    c.Enabled = true;
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked)
            {
                foreach (Control c in this.groupBox3.Controls)
                {
                    c.Enabled = false;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputDialog = new OpenFileDialog();
            inputDialog.Filter = "(*.xls)|*.xls";
            inputDialog.ShowDialog();
            inputFile = inputDialog.InitialDirectory + inputDialog.FileName;
            this.textBox1.Text = inputFile;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog outputFolder = new FolderBrowserDialog();
            outputFolder.ShowDialog();
            outputPath = outputFolder.SelectedPath;
            this.textBox2.Text = outputPath;
        }

      
    }
}
