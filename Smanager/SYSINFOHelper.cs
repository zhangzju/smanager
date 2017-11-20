using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Management; 

namespace Smanager
{
    class SYSINFOHelper
    {
        public string CpuID; //1.cpu序列号
        public string MacAddress; //2.mac序列号
        public string DiskID; //3.硬盘id
        public string IpAddress; //4.ip地址
        public string LoginUserName; //5.登录用户名
        public string ComputerName; //6.计算机名
        public string SystemType; //7.系统类型
        public string TotalPhysicalMemory; //8.内存量 单位：M

        public SYSINFOHelper() 
        { 
            CpuID = GetCpuID(); 
            MacAddress = GetMacAddress();
            IpAddress = GetLocalIP(); 
            LoginUserName = GetUserName(); 
            SystemType = GetSystemType();  
            ComputerName = GetComputerName(); 
        }

        private string GetComputerName()
        {
            throw new NotImplementedException();
        }

        private string GetSystemType()
        {
            throw new NotImplementedException();
        }

        private string GetUserName()
        {
            throw new NotImplementedException();
        }

        private string GetMacAddress()
        {
            throw new NotImplementedException();
        }

        public static string GetCpuID()
        {
            return "";
        }
        
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名  
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址  
                    //AddressFamily.InterNetwork表示此IP为IPv4,  
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型  
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }  
    }
}
