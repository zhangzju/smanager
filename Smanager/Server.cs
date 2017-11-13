using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smanager
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string startIp = this.textBox1.Text;
            string endIp = this.textBox2.Text;
            decimal leaseTime = this.numericUpDown1.Value;
            bool crypto = this.checkBox1.Checked;
            bool zipped = this.checkBox2.Checked;

            string DHCPRange = startIp + "-" + endIp;
            writeConf("dhcp", "RANGE_SET", "DHCPRange", DHCPRange);

        }

        private void checkValid(string type, string value)
        {

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
