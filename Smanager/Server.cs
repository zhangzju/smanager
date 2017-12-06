using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Smanager
{
    public partial class Server : Form
    {

        private string serverPath;
        public Server(string serverpath)
        {
            InitializeComponent();
            this.serverPath = serverpath;
            this.label13.Text = serverPath;
            this.label9.Text = SYSINFOHelper.GetLocalIP();
            this.label11.Text = SYSINFOHelper.GetMacAddress();
            this.label10.Text = getDHCPrange();
            this.label12.Text = getTFTPaddress();
        }

        private string getDHCPrange()
        {
            string dhcpFile = Application.StartupPath + @"\DHCP\OpenDHCPServer.ini";
            string range = INIOperationClass.INIGetStringValue(dhcpFile, "RANGE_SET", "DHCPRange", "Get result failed!");

            return range;
        }

        private string getTFTPaddress()
        {
            string dhcpFile = Application.StartupPath + @"\DHCP\OpenDHCPServer.ini";
            string option = INIOperationClass.INIGetStringValue(dhcpFile, "GLOBAL_OPTIONS", "66", "Get result failed!");

            return option;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string startIp = this.textBox1.Text;
            checkIPValid(startIp);
            string endIp = this.textBox2.Text;
            checkIPValid(endIp);
            decimal leaseTime = this.numericUpDown1.Value;
            bool crypto = this.checkBox1.Checked;
            bool zipped = this.checkBox2.Checked;

            string DHCPRange = startIp + "-" + endIp;
            writeConf("dhcp", "RANGE_SET", "DHCPRange", DHCPRange);

        }

        private int checkIPValid(string value)
        {
            Regex rx = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (rx.IsMatch(value))
            {
                return 0;
            }
            else
            {
                MessageBox.Show("IP Address is not valid.");
                return 1;
            }  
        }

        private void writeConf(string filename, string section, string node, string value)
        {
            string tftpFile = Application.StartupPath + "\\TFTP\\OpenTFTPServerMT.ini";
            string dhcpFile = Application.StartupPath + @"\DHCP\OpenDHCPServer.ini";
            if(filename == "dhcp")
            {
                INIOperationClass.INIWriteValue(dhcpFile, section, node, value);
            }
            else if(filename == "tftp")
            {
                INIOperationClass.INIWriteValue(tftpFile, section, node, value);
            }
        }


    }
}
