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
using System.Xml;

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
        static List<string> titleList = new List<string>();
        private string ispName;
        private string modelName;

        public macbin(string mainPath, string ispname, string modelname)
        {
            InitializeComponent();
            this.radioButton1.Checked = true;
            this.textBox2.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            this.inputFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Agile-Mac.xlsx";
            this.textBox1.Text = this.inputFile;
            this.mainPath = mainPath;
            this.ispppoe = true;
            this.ispName = ispname;
            this.modelName = modelname;
            this.outputPath = this.mainPath + @"\" + this.ispName + @"\" + this.modelName;
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
            inputDialog.Filter = "xlsx files(*.xls*)|*.xlsx|All files (*.*)|*.*";
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
            pppoeusernamelength = Convert.ToInt32(this.numericUpDown1.Value);
            pppoepasswordlength = Convert.ToInt32(this.numericUpDown2.Value);

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

            for (Int32 index = 0; index < count; index++)
            {
                row = sheet.CreateRow(Convert.ToInt32(index) + 1);
                if (ispppoe)
                {
                    if (ispppoeusername)
                    {
                        cell = row.CreateCell(1);
                        cell.SetCellValue(UTILITYHelper.getRandomizer(pppoeusernamelength, true, true, true, false));
                    }

                    if(ispppoepassword)
                    {
                        cell = row.CreateCell(2);
                        cell.SetCellValue(UTILITYHelper.getRandomizer(pppoepasswordlength, true, true, true, false));
                    }
                }
            }

                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Agile-Mac.xlsx"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Agile-Mac.xlsx");
                }
            try
            {
                FileStream sw = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Agile-Mac.xlsx");
                workbook.Write(sw);
                MessageBox.Show("File generated successfully in" + Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Agile-Mac.xlsx");
            }
            catch(Exception error)
            {
                MessageBox.Show(error.ToString()+"\n"+"Please Choose another output path!");
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.checkBox1.Checked = false;
            this.checkBox2.Checked = false;
            this.checkBox3.Checked = false;
            this.checkBox4.Checked = false;
            this.radioButton1.Checked = true;
            this.numericUpDown1.Value = 0;
            this.numericUpDown2.Value = 0;
            this.numericUpDown3.Value = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IWorkbook workbook = null;
            using (FileStream fs = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new XSSFWorkbook(fs);
                fs.Close();

                ISheet sheet = workbook.GetSheetAt(0);
                int index=0,count = 0;
                string macAddress;
                string pppoeusername;
                string pppoepassword;
                string ipaddress;
                string mask;
                string gateway;
                string dns;
                string ssid2;
                string ssid5;

                IRow titlerow = sheet.GetRow(index);
                ICell titlecell = titlerow.GetCell(1);

                if (!Directory.Exists(outputPath + @"\macbin"))
                {
                    Directory.CreateDirectory(outputPath + @"\macbin");
                }

                if (titlecell.ToString() == "PPPoE Username")
                {
                    while (sheet.GetRow(index) != null && sheet.GetRow(index).GetCell(0) != null && !string.IsNullOrEmpty(sheet.GetRow(index).GetCell(0).ToString()))
                    {
                        count++;
                        index++;
                    }

                    for (index = 1; index < Convert.ToInt32(count); index++)
                    {
                        IRow row = sheet.GetRow(index);
                        macAddress = row.GetCell(0).ToString();
                        pppoeusername = row.GetCell(1).ToString();
                        pppoepassword = row.GetCell(2).ToString();
                        ssid2 = row.GetCell(3).ToString();
                        ssid5 = row.GetCell(4).ToString();

                        MessageBox.Show(macAddress + pppoeusername + pppoepassword + ssid2 + ssid5);
                        XmlDocument xmldoc = new XmlDocument();

                        try
                        {
                            xmldoc.Load(mainPath + @"\PPPoE.xml");
                            XmlNode xn = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/LANDevice/WLANConfiguration[@instance=1]/SSID");
                            xn.Attributes["val"].Value = ssid2;
                            XmlNode xn_ssid5 = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/LANDevice/WLANConfiguration[@instance=2]/SSID");
                            xn_ssid5.Attributes["val"].Value = ssid5;
                            XmlNode xn_pppuser = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/WANDevice[@instance=1]/WANConnectionDevice[@instance=1]/WANPPPConnection[@instance=1]/Username");
                            xn_pppuser.Attributes["val"].Value = pppoeusername;
                            XmlNode xn_ppppwd = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/WANDevice[@instance=1]/WANConnectionDevice[@instance=1]/WANPPPConnection[@instance=1]/Password");
                            xn_ppppwd.Attributes["val"].Value = pppoepassword;
                            xmldoc.Save(outputPath + "/macbin" + @"/"+macAddress+".xml");

                            MessageBox.Show(outputPath + @"\macbin" + @"\" + macAddress + ".xml");
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show(error.Message);
                        }
                        
                    }
                }
                else
                {
                    while (sheet.GetRow(index) != null && sheet.GetRow(index).GetCell(0) != null && !string.IsNullOrEmpty(sheet.GetRow(index).GetCell(0).ToString()))
                    {
                        count++;
                        index++;
                    }

                    for (index = 1; index < Convert.ToInt32(count); index++)
                    {
                        try
                        {
                            IRow row = sheet.GetRow(index);
                            macAddress = row.GetCell(0).ToString();
                            ipaddress = row.GetCell(1).ToString();
                            mask = row.GetCell(2).ToString();
                            gateway = row.GetCell(3).ToString();
                            dns = row.GetCell(4).ToString();
                            ssid2 = row.GetCell(5).ToString();
                            ssid5 = row.GetCell(6).ToString();
                        }
                        catch (Exception error)
                        {
                            UTILITYHelper.ShowError("Wrong format Macbin file!");
                            break;
                        }
                        

                        MessageBox.Show(macAddress + ipaddress + mask + ssid2 + ssid5);
                        XmlDocument xmldoc = new XmlDocument();

                        try
                        {
                            xmldoc.Load(mainPath + @"\Static.xml");
                            XmlNode xn = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/LANDevice/WLANConfiguration[@instance=1]/SSID");
                            xn.Attributes["val"].Value = ssid2;
                            XmlNode xn_ssid5 = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/LANDevice/WLANConfiguration[@instance=2]/SSID");
                            xn_ssid5.Attributes["val"].Value = ssid5;
                            XmlNode xn_ipaddr = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/WANDevice[@instance=1]/WANConnectionDevice[@instance=1]/WANIPConnection[@instance=1]/ExternalIPAddress");
                            xn_ipaddr.Attributes["val"].Value = ipaddress;
                            XmlNode xn_mask = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/WANDevice[@instance=1]/WANConnectionDevice[@instance=1]/WANIPConnection[@instance=1]/SubnetMask");
                            xn_mask.Attributes["val"].Value = mask;
                            XmlNode xn_gateway = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/WANDevice[@instance=1]/WANConnectionDevice[@instance=1]/WANIPConnection[@instance=1]/DefaultGateway");
                            xn_gateway.Attributes["val"].Value = gateway;
                            XmlNode xn_dns = xmldoc.SelectSingleNode("/DslCpeConfig/InternetGatewayDevice/WANDevice[@instance=1]/WANConnectionDevice[@instance=1]/WANIPConnection[@instance=1]/DNSServers");
                            xn_dns.Attributes["val"].Value = dns;
                            xmldoc.Save(outputPath + "/macbin" + @"/" + macAddress + ".xml");

                            MessageBox.Show(outputPath + @"\macbin" + @"\" + macAddress + ".xml");
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show(error.Message);
                        }

                    }
                }


                
            }
            
            
        }



        public string mainPath { get; set; }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
