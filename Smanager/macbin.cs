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
        private string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);  
        private string inputFile;
        static List<string> titleList = new List<string>();

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

        private void button2_Click(object sender, EventArgs e)
        {
            is2gssid = this.checkBox1.Checked;
            is5gssid = this.checkBox2.Checked;
            ispppoe = this.radioButton1.Checked;
            ispppoeusername = this.checkBox3.Checked;
            ispppoepassword = this.checkBox4.Checked;
            count = Convert.ToInt32(this.numericUpDown3.Value);

            titleList.Add("MAC Address");
            if (ispppoe)
            {
                titleList.Add("PPPoE Username");
                titleList.Add("PPPoE Password");
            }
            else
            {
                titleList.Add("IP Address");
                titleList.Add("Mask");
                titleList.Add("Gateway");
                titleList.Add("DNS");
            }
            titleList.Add("2.4GHZ SSID");
            titleList.Add("5GHZ SSID");
            titleList.Add("Generate Flag");

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(DateTime.Now.ToString("yyyy-MM-dd"));
            IRow row;
            ICell cell;

            row = sheet.CreateRow(0);
            int tempcount = 0;
            foreach (var title in titleList)
            {
                cell = row.CreateCell(tempcount);
                cell.SetCellValue(title);
                sheet.SetColumnWidth(tempcount, 20 * 256);
                tempcount++;
            }

            if (File.Exists(outputPath + @"\Agile-Mac.xlsx"))
            {
                File.Delete(outputPath);
            }
            try
            {
                FileStream sw = File.Create(outputPath);
                workbook.Write(sw);
            }
            catch(Exception error)
            {
                MessageBox.Show(error.ToString()+"\n"+"Please Choose another output path!");
            }
            
        }

      
    }
}
