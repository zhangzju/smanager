﻿using System;
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

namespace Smanager
{
    public partial class Form1 : Form
    {
        static string path = Application.StartupPath;
        static Int32 DHCP_PID = 0;
        static Int32 TFTP_PID = 0;
        static string serverPath = Application.StartupPath;
        static bool serverStatus = false;

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
                proc.Exited += new EventHandler(dhcp_Exited);
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

        private void dhcp_Exited(object sender, EventArgs e)
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
            this.textBox1.Text = mainFolder.SelectedPath;

            DirectoryInfo TheFolder = new DirectoryInfo(mainFolder.SelectedPath);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                this.comboBox1.Items.Add(NextFolder.Name);
            }
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

        private void button5_Click(object sender, EventArgs e)
        {
            if(serverStatus)
            {
                int shutDownDHCP = shutDownServer(DHCP_PID);
                int shutDownTFTP = shutDownServer(TFTP_PID);

                if(shutDownDHCP == 0 && shutDownTFTP == 0)
                {
                    this.button3.BackColor = System.Drawing.Color.Red;
                    this.button4.BackColor = System.Drawing.Color.Red;
                    serverStatus = false;
                }
            }
            else
            {
                if (isPortUsed(67))
                {
                    startDHCP();
                    this.button3.BackColor = System.Drawing.Color.Lime;
                }
                else
                {
                    //MessageBox.Show("Warning: the port of DHCP is occupied!");
                    this.richTextBox1.AppendText("Warning: the port of DHCP is occupied!\n");
                }

                if(isPortUsed(69))
                {
                    startTFTP();
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

        private bool isPortUsed(int port)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("netstat", "-a");
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

        private int startTFTP()
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

        private int startDHCP()
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = path + "\\DHCP\\OpenDHCPServer.exe";
                proc.StartInfo.Arguments = "-v";//this is argument
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(dhcp_Exited);
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

        private int  shutDownServer(int pid)
        {
            Process localById = Process.GetProcessById(pid);
            try
            {
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(serverPath + "\\" + this.comboBox1.Text);
            DirectoryInfo TheFolder = new DirectoryInfo(serverPath + "\\" + this.comboBox1.Text);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                this.comboBox2.Items.Add(NextFolder.Name);
            }
        }
    }
}
