using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;

namespace Smanager
{
    public partial class Form1 : Form
    {
        static string path = Application.StartupPath;
        static Int32 DHCP_PID = 0;
        static Int32 TFTP_PID = 0;
        static string serverPath = Application.StartupPath;
        static bool serverStatus = false;
        static string ispName = "TM";
        static string modelName = "C20";
        static char[] protocol = {'0','0','0','0','0','0','0','0'};

        public Form1()
        {
            InitializeComponent();

            this.textBox1.Text = serverPath;
            this.comboBox1.Text = "ISP";
            this.comboBox2.Text = "Model";
            this.button3.BackColor = System.Drawing.Color.Red;
            this.button4.BackColor = System.Drawing.Color.Red;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = path+ "\\DHCP\\OpenDHCPServer.exe";
                proc.StartInfo.Arguments = "-v";//this is argument
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(Dhcp_Exited);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit(100);
                proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                DHCP_PID = proc.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }

       
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
            StringBuilder sb = new StringBuilder(this.textBox1.Text);
            this.textBox1.Text = sb.AppendLine(outLine.Data).ToString();
            this.textBox1.SelectionStart = this.textBox1.Text.Length;
            this.textBox1.ScrollToCaret();
            }
        }

        private void Dhcp_Exited(object sender, EventArgs e)
        {
            //MessageBox.Show(DHCP_PID.ToString());
            //this.richTextBox1.AppendText("DHCP exited!\n");

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = path + "\\TFTP\\OpenTFTPServerMT.exe";
                proc.StartInfo.Arguments = "-v";//this is argument
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(Tftp_Exited);
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = false;
                proc.Start();
                proc.WaitForExit(100);
                TFTP_PID = proc.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }

        private void Tftp_Exited(object sender, EventArgs e)
        {
            //MessageBox.Show(TFTP_PID.ToString());
            //this.richTextBox1.AppendText("TFTP exited!\n");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var ini = new IniFile();
            ini.Load(path + "\\DHCP\\OpenDHCPServer.ini");
            ini["test1"]["doubleB"] = "not a double";
            MessageBox.Show(ini.GetContents());
        }

        private int ConfigTftpPath(string path)
        {
            //var ini = new IniFile();
            //ini.Load(Application.StartupPath + "\\TFTP\\OpenTFTPServerMT.ini");
            //ini["HOME"]["Agile"] = path;
            //this.richTextBox1.AppendText("TFTP path is " + path + " \n");
            //MessageBox.Show(ini.GetContents());
            string iniFile = Application.StartupPath + "\\TFTP\\OpenTFTPServerMT.ini";
            INIOperationClass.INIWriteValue(iniFile, "HOME", "Agile", serverPath);
            this.richTextBox1.AppendText("TFTP path is " + path + " \n");
            return 0;
        }

        private void KillProcess(string processName)
        {
            Process[] myproc = Process.GetProcesses();
            foreach (Process item in myproc)
            {
                if (item.ProcessName == processName)
                {
                    item.Kill();
                }
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Process localById = Process.GetProcessById(DHCP_PID);
            try
            {
                localById.Kill();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog mainFolder = new FolderBrowserDialog();
            mainFolder.ShowDialog();
            serverPath = mainFolder.SelectedPath;
            //MessageBox.Show(serverPath);
            this.textBox1.Text = mainFolder.SelectedPath;

            DirectoryInfo TheFolder = new DirectoryInfo(mainFolder.SelectedPath);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                this.comboBox1.Items.Add(NextFolder.Name);
            }

            //configTftpPath(serverPath);
        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            macbin macbin = new macbin();
            macbin.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Server server = new Server();
            server.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            this.richTextBox1.AppendText(serverStatus.ToString());
            if (serverStatus)
            {
                int shutDownDHCP = ShutDownServer(DHCP_PID);
                int shutDownTFTP = ShutDownServer(TFTP_PID);

                if(shutDownDHCP == 0 && shutDownTFTP == 0)
                {
                    this.button3.BackColor = System.Drawing.Color.Red;
                    this.button4.BackColor = System.Drawing.Color.Red;
                    serverStatus = false;
                }
            }
            else
            {               
                if (PortInUse(67))
                {
                    this.richTextBox1.AppendText("Warning: the port of DHCP is occupied!\n");
                }
                else
                {                   
                    StartDHCP();
                    this.button3.BackColor = System.Drawing.Color.Lime;
                }

                if(!PortInUse(69))
                {
                    StartTFTP();
                    this.button4.BackColor = System.Drawing.Color.Lime;
                }
                else
                {
                    //MessageBox.Show("Warning: the port of TFTP is occupied!");
                    this.richTextBox1.AppendText("Warning: the port of TFTP is occupied!\n");

                }

                serverStatus = true;
            }         
        }

        private bool IsPortUsed(int port)
        {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo("netstat", "-a")
            };
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string result = p.StandardOutput.ReadToEnd();
            if (result.IndexOf(Environment.MachineName.ToLower() + ":"+ port) >= 0)
                return false;
            else
            {
                return true;
            }       
        }

        public static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;
        }

        private int StartTFTP()
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = path + "\\TFTP\\OpenTFTPServerMT.exe";
                proc.StartInfo.Arguments = "-v";//this is argument
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(Tftp_Exited);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit(100);
                proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                TFTP_PID = proc.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                return -1;
            }
            //MessageBox.Show("TFTP started");
            this.richTextBox1.AppendText("TFTP started\n");
            return 0;
        }

        private int StartDHCP()
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = path + "\\DHCP\\OpenDHCPServer.exe";
                proc.StartInfo.Arguments = "-v";//this is argument
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(Dhcp_Exited);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit(100);
                proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                DHCP_PID = proc.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                return -1;
            }
            //MessageBox.Show("DHCP started");
            this.richTextBox1.AppendText("DHCP started\n");
            return 0;
        }

        private int  ShutDownServer(int pid)
        {            
            try
            {
                Process localById = Process.GetProcessById(pid);
                this.richTextBox1.AppendText("Process " + pid + " was terminated!\n");
                localById.Kill();
                return 0;
            }
            catch (Exception ex)
            {
                this.richTextBox1.AppendText("Process "+ pid + " could not be terminated!\n");
                return -1;
                throw ex;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(serverPath + "\\" + this.comboBox1.Text);
            DirectoryInfo TheFolder = new DirectoryInfo(serverPath + "/" + this.comboBox1.Text);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                this.comboBox2.Items.Add(NextFolder.Name);
            }
            ispName = this.comboBox1.Text;
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelName = this.comboBox2.Text;
            string TftpPath;
            TftpPath = serverPath + "\\" + ispName + "\\" + modelName;
            ConfigTftpPath(TftpPath);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string initFile = serverPath + "\\" + ispName + "\\" + modelName + @"\table.json";
            CheckDirIsValid(serverPath + "\\" + ispName + "\\" + modelName);
            unionFileGen(initFile);
            string protocolString = new string(protocol);
            string ipAddress = SYSINFOHelper.GetLocalIP();
            string option = "\""+"TP-LINK " + ipAddress + " " + protocolString+"\"";
            this.richTextBox1.AppendText(option);
            writeConf("dhcp", "GLOBAL_OPTIONS", "66", option);
        }

        private void CheckDirIsValid(string path)
        {
            if (File.Exists(path + @"\table.json"))
            {
                this.richTextBox1.AppendText("Load the table.json!\n");
            }
            else
            {
                MessageBox.Show("This dir is invalid!");
            }
        }

        private int unionFileGen(string initFile)
        {
            string initInfo = File.ReadAllText(initFile);
            var table = JObject.Parse(initInfo);
            string unionPath = serverPath + "\\" + ispName + "\\" + modelName + @"\union.bin";
            MessageBox.Show(unionPath);
            BinaryWriter unionFileWriter = new BinaryWriter(new FileStream(unionPath, FileMode.OpenOrCreate, FileAccess.ReadWrite));

            unionFileWriter.Seek(0, SeekOrigin.Begin);
            //byte[] initInfoByte = System.Text.Encoding.Default.GetBytes(initInfo);

            BinaryReader initInfoStream = new BinaryReader(new FileStream(initFile, FileMode.OpenOrCreate, FileAccess.ReadWrite));
            byte[] initInfoByte = initInfoStream.ReadBytes(4 * 128);
            unionFileWriter.Seek(0, SeekOrigin.Begin);
            unionFileWriter.Write(initInfoByte, 0, initInfoByte.Length);


            if(table["config"]["flag"].ToString() == "1")
            {
                string filename = table["config"]["name"].ToString();
                string fullpath = serverPath + "\\" + ispName + "\\" + modelName + "\\" + filename;
                Int32 size = Convert.ToInt32(table["config"]["size"].ToString());
                Int32 offset = Convert.ToInt32(table["config"]["offset"].ToString());
                BinaryReader confStream = new BinaryReader(new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite));
                byte[] confByte = confStream.ReadBytes(size * 1024);
                unionFileWriter.Seek(4 * 1024, SeekOrigin.Begin);
                unionFileWriter.Write(confByte, 0, confByte.Length);
            }

            if(table["kernel"]["flag"].ToString() == "1")
            {
                string filename = table["kernel"]["name"].ToString();
                string fullpath = serverPath + "\\" + ispName + "\\" + modelName + "\\" + filename;
                Int32 size = Convert.ToInt32(table["kernel"]["size"].ToString());
                Int32 offset = Convert.ToInt32(table["kernel"]["offset"].ToString());
                BinaryReader kernelStream = new BinaryReader(new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite));
                byte[] confByte = kernelStream.ReadBytes(size * 1024);
                unionFileWriter.Seek(offset * 1024, SeekOrigin.Begin);
                unionFileWriter.Write(confByte, 0, confByte.Length);

            }

            if (table["rootfs"]["flag"].ToString() == "1")
            {
                string filename = table["rootfs"]["name"].ToString();
                string fullpath = serverPath + "\\" + ispName + "\\" + modelName + "\\" + filename;
                Int32 size = Convert.ToInt32(table["rootfs"]["size"].ToString());
                Int32 offset = Convert.ToInt32(table["rootfs"]["offset"].ToString());
                BinaryReader rootfsStream = new BinaryReader(new FileStream(fullpath, FileMode.Open, FileAccess.ReadWrite));
                byte[] rootfsByte = rootfsStream.ReadBytes(size * 1024);
                unionFileWriter.Seek(offset * 1024, SeekOrigin.Begin);
                unionFileWriter.Write(rootfsByte, 0, rootfsByte.Length);

            }


            unionFileWriter.Close();
            return 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {   
            //Set the parameter of protocol(transferred by option 66
            if(this.checkBox1.Checked == true)
            {
                protocol[4] = '1';
            }else if(this.checkBox1.Checked == false)
            {
                protocol[4] = '0';
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked == true)
            {
                protocol[5] = '1';
            }
            else if (this.checkBox3.Checked == false)
            {
                protocol[5] = '0';
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked == true)
            {
                protocol[6] = '1';
            }
            else if (this.checkBox4.Checked == false)
            {
                protocol[6] = '0';
            }
        }

        private void writeConf(string filename, string section, string node, string value)
        {
            string tftpFile = Application.StartupPath + "\\TFTP\\OpenTFTPServerMT.ini";
            string dhcpFile = Application.StartupPath + @"\DHCP\OpenDHCPServer.ini";
            if (filename == "dhcp")
            {
                INIOperationClass.INIWriteValue(dhcpFile, section, node, value);
            }
            else if (filename == "tftp")
            {
                INIOperationClass.INIWriteValue(tftpFile, section, node, value);
            }
        }
    }
}
