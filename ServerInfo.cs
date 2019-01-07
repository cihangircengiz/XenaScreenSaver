using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Sifreleme;

namespace Screensaver.Computer
{
    class ServerInfo
    {
        
        public string lisans { set; get; }
        public string kilitekran { set; get; }
        public string time1 { set; get; }
        public string time2{ set; get; }
        public string yoklama { set; get; }
        public string Isfirststart { set; get; }
        public string exprite_date { set; get; }
        public string server { set; get; }
        public string todaydate { set; get; }
        public List<string> result; 
        public ServerInfo()
        {
            Connection _Connection = new Connection();
            string sql =
                "Select client_list1.lisans,url,time1,time2,settings1,count(*),settings.device,curdate() from client_list1 inner join settings on client_list1.lisans = settings.lisans where mac_adress = '" +
                new ComputerInfo()._macAddress + "'";
            result = _Connection.multiValueQuery(sql);
            if(result.Count != 0) { 
                lisans = result[0] == "" ? "demo" : result[0];
                kilitekran = result[1] == "" ? "https://developer.venov.com.tr/kilitekran" : result[1];
                time1 = result[2] == "" ? "18:00" : result[2];
                time2 = result[3] == "" ? "19:30" : result[3];
                yoklama = result[4] == "" ? "0" : result[4];
                Isfirststart = result[5];
                var currentUser = Registry.CurrentUser.OpenSubKey("Kilit");
                var localUser = Registry.LocalMachine.OpenSubKey(@"SYSTEM/Kilit");
                server = currentUser?.GetValue("Server")?.ToString() ??
                   localUser?.GetValue("Server")?.ToString() ?? "developer.venov.com.tr";
                todaydate = result[7];
            if (result[6] == "") {
                sql =
                   "Select device from settings where lisans ='"+lisans+"'";
            result = _Connection.multiValueQuery(sql);
                    try
                    {
                        exprite_date = result[0];
                    }
                    catch (Exception )
                    {
                        exprite_date = "";
                    }
                    
            }
            else
                exprite_date = result[6];
        }
        }
    }
}
