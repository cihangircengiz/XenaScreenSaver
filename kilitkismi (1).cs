using Kitleyici;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace Screensaver
{
    public partial class kilit : Form
    {
        bool a = false;
        TcpClient tcpClient = new TcpClient();
        static MySqlConnection baglantim = null;   
        string tur, sonuc = "",link = "";
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        string serverip = Registry.CurrentUser.OpenSubKey("Kilit").GetValue("Server").ToString(); 
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        static string programDizini = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        private Kitleyiciler kontrol;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public kilit()
        {
            kontrol = new Kitleyiciler();
            InitializeComponent();
        }
        private Point MouseXY;
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            } return sMacAddress;
        }
        private void OnMouseEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!MouseXY.IsEmpty)
            {
                if (MouseXY != new Point(e.X, e.Y))
                    Close();
                if (e.Clicks > 0)
                    Close();
            }
            MouseXY = new Point(e.X, e.Y);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt) || (e.KeyCode == Keys.Alt))
                e.Handled = true;

            base.OnKeyDown(e);
        }
        public string SqlSorgu(string sorgu)
        {
            try
            {
                if (baglantim == null)
                {
                    MySqlConnectionStringBuilder bag = new MySqlConnectionStringBuilder();
                    bag.Server = serverip;
                    bag.Port = 3306;
                    bag.UserID = "mobile";
                    bag.Password = "1234";
                    bag.Database = "screensaver";
                    baglantim = new MySqlConnection(bag.ToString());
                    //baglantim.Open();                    
                }
                if (baglantim.State.ToString() != "Open")
                {
                    try
                    {
                        baglantim.Open();

                    }
                    catch (Exception)
                    {

                    }
                }
                MySqlCommand komut = new MySqlCommand();
                komut.Connection = baglantim;
                komut.CommandText = sorgu;
                tur = sorgu.Substring(0, sorgu.IndexOf(' '));

                if (tur.ToLower() == "SELECT".ToLower())
                {
                    try
                    {
                        sonuc = komut.ExecuteScalar().ToString();
                    }
                    catch (Exception)
                    {
                        
                    }

                }
                else
                    try
                    {
                        komut.ExecuteNonQuery();
                        komut.Connection.Close();
                    }
                    catch (Exception)
                    {
                        // File.WriteAllText(programDizini + "\\Errors\\Mysqllogs2.txt", DateTime.Now.ToString() + "  " + m.ToString() + "\n");

                    }
                return sonuc;
            }
            //   baglantim.Close();


            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return "";
            }


        }

        private void kilit_Load(object sender, EventArgs e)
        {
              try
            {
                tcpClient.Connect(serverip, 3306);
                a = true;
            }
            catch (Exception)
            {
                a = false;
            }
            bool bb = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();  
            if (bb == true & a == true)
            {
                link = SqlSorgu("select url from client_list1 where mac_adress='"+GetMACAddress()+"'");
                if (link.Length > 6)
                {
                    webBrowser1.Navigate(new Uri(link));  
                }
                else
                {
                    link = null;
                    string text1 = "<!DOCTYPE html><html><head><script>";
                    string text2 = "function startTime() {var today=new Date();var h=today.getHours();var m=today.getMinutes();var s=today.getSeconds();m = checkTime(m);s = checkTime(s);document.getElementById('txt').innerHTML = h+\":\"+m+\":\"+s;var t = setTimeout(function(){startTime()},500);}";
                    string text3 = "function checkTime(i) {if (i<10) {i = \"0\" + i};return i;}";
                    string text4 = "</script><style>#txt{width: 100px;height: 100px;position: absolute;left: 50%;top: 50%;margin-left: -160px;margin-top: -95px;color:#FFF;	font-size:120px;}</style></head><body onload=\"startTime()\" bgcolor=\"#000000\"><div id=\"txt\"></div></body></html>";
                    webBrowser1.DocumentText = text1 + text2 + text3 + text4;
                }
                
            }
            else if (bb == false & a == true)
            {
                webBrowser1.Navigate("http://" + serverip + "/screensaver/");
            }
            else if (bb == true & a == false)
            {
                webBrowser1.Navigate("http://www.beyhanyazilim.com/kilitekran/");
            }
            else if (bb == false & a == false)
            {
                link = null;
                string text1 = "<!DOCTYPE html><html><head><script>";
                string text2 = "function startTime() {var today=new Date();var h=today.getHours();var m=today.getMinutes();var s=today.getSeconds();m = checkTime(m);s = checkTime(s);document.getElementById('txt').innerHTML = h+\":\"+m+\":\"+s;var t = setTimeout(function(){startTime()},500);}";
                string text3 = "function checkTime(i) {if (i<10) {i = \"0\" + i};return i;}";
                string text4 = "</script><style>#txt{width: 100px;height: 100px;position: absolute;left: 50%;top: 50%;margin-left: -160px;margin-top: -95px;color:#FFF;	font-size:120px;}</style></head><body onload=\"startTime()\" bgcolor=\"#000000\"><div id=\"txt\"></div></body></html>";
                webBrowser1.DocumentText = text1+text2+text3+text4;
              
            }
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            ((Control)webBrowser1).Enabled = false;
            this.Name = "Ekran-Koruyucu";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            
            
        }  
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
