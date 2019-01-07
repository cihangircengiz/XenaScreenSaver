using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Screensaver.Computer
{
    class ComputerInfo
    {
        public static string ProgramDizini = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

        public static string Sistem = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

        public static string LocalComputerName = Dns.GetHostName();

        public static int isletimsistemi
        {
            get
            {
                Version win8version = new Version(6, 2, 9200, 0);
                if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    Environment.OSVersion.Version >= win8version)
                {
                    return 8;
                }
                else
                {
                    return 7;
                }
            }
        }
            
/// <summary>
/// 
/// </summary>
        static IPAddress dst = IPAddress.Parse("10.1.1.1");
       // int ifIndex = GetBestInterface(dst);

        public int ifIndex
        {
            get
            {
                try
                {
                  return GetBestInterface(dst);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public string _macAddress
        {
            get { return foo(ifIndex).ToString(); }
        }

        public string _ipAddress
        {
            get { return GetIp(ifIndex).First().ToString(); }
        }

        public PhysicalAddress foo(int ifIndex)
        {
            var q = from ni in NetworkInterface.GetAllNetworkInterfaces()
                    where ni.GetIPProperties().GetIPv6Properties().Index == ifIndex
                          || ni.GetIPProperties().GetIPv6Properties().Index == ifIndex
                    select ni;
            try
            {
                var rslt = q.First();
                //
                var macAddr = rslt.GetPhysicalAddress();
                return macAddr;
            }
            catch (Exception)
            {
                return PhysicalAddress.Parse("00-00-00-00-00-00");
            }
            
        }

        public List<IPAddress> GetIp(int ifIndex)
        {
            var q = from ni in NetworkInterface.GetAllNetworkInterfaces()
                    where ni.GetIPProperties().GetIPv6Properties().Index == ifIndex
                          || ni.GetIPProperties().GetIPv6Properties().Index == ifIndex
                    select ni;
            var _tmp = new List<IPAddress>();
            try
            {
                var rslt = q.First();
                //
                foreach (var ip in rslt.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.ToString().Length <= 15 && !ip.Address.ToString()[0].Equals('f'))
                    {
                        _tmp.Add(ip.Address);
                        _tmp.Add(ip.IPv4Mask);
                    }
                }
            }
            catch (Exception)
            {
                _tmp.Add(IPAddress.Parse("127.0.0.1"));
                _tmp.Add(IPAddress.Parse("255.255.255.0"));
                
            }
            return _tmp;
        }

        public IPAddress GetBestRoute(IPAddress destination)
        {
            if (destination.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException("Address must be IPv4.");
            var arr = destination.GetAddressBytes();
            var ipInt = BitConverter.ToUInt32(arr, 0);
            NativeMethods.MIB_IPFORWARDROW row;
            var ret = NativeMethods.GetBestRoute(ipInt, 0, out row);
            if (ret != 0)
                throw new Win32Exception();
            var nextHop = IPAddressFromInt(row.dwForwardNextHop);
            var dest = IPAddressFromInt(row.dwForwardDest);
            var mask = IPAddressFromInt(row.dwForwardMask);
            var proto = row.dwForwardProto;
            var ifIndex = row.dwForwardIfIndex;
            return nextHop;
        }


        public static int GetBestInterface(IPAddress destination)
        {
            var ep = new IPEndPoint(destination, 0);
            var sa = ep.Serialize();
            var arrSa = CopyArray(sa);
            int ifIndex;
            var ret = NativeMethods.GetBestInterfaceEx(arrSa, out ifIndex);
            if (ret != 0)
                throw new Win32Exception();
            return ifIndex;
        }

        public static byte[] CopyArray(SocketAddress sa)
        {
            var arr = new byte[sa.Size];
            for (var i = 0; i < arr.Length; ++i)
            {
                arr[i] = sa[i];
            }
            return arr;
        }

        public IPAddress IPAddressFromInt(int p)
        {
            var p2 = (long)p;
            return IPAddressFromInt(p2);
        }

        public IPAddress IPAddressFromInt(long p2)
        {
            return new IPAddress(p2);
        }

        public static class NativeMethods
        {
            [DllImport("Iphlpapi", SetLastError = true)]
            internal static extern int GetBestRoute(
                uint dwDestAddr,
                uint dwSourceAddr,
                IntPtr /*PMIB_IPFORWARDROW*/ pBestRoute);

            [DllImport("Iphlpapi", SetLastError = true)]
            internal static extern int GetBestRoute(
                uint dwDestAddr,
                uint dwSourceAddr,
                out MIB_IPFORWARDROW pBestRoute);

            [DllImport("Iphlpapi", SetLastError = true)]
            internal static extern int GetBestInterfaceEx(
                byte[] socketAddress,
                out int pdwBestIfIndex);

            internal struct MIB_IPFORWARDROW
            {
                internal int dwForwardDest;
                internal int dwForwardMask;
                internal int dwForwardPolicy;
                internal int dwForwardNextHop;
                internal int dwForwardIfIndex;
                internal int dwForwardType;
                internal int dwForwardProto;
                internal int dwForwardAge;
                internal int dwForwardNextHopAS;
                internal int dwForwardMetric1;
                internal int dwForwardMetric2;
                internal int dwForwardMetric3;
                internal int dwForwardMetric4;
                internal int dwForwardMetric5;
            }
        }

    }
}
