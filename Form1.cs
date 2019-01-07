using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using iTuner;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Screensaver.Computer;
using Sifreleme;
using TaskbarHide;

namespace Screensaver
{
    public partial class Form1 : Form
    {
        private const int WH_KEYBOARD_LL = 13; // Номер глобального LowLevel-хука на клавиатуру
        private const int WM_KEYDOWN = 0x100; // Сообщения нажатия клавиши
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        public static bool trueWork = false;
        public static bool shutdownEvt;
        public static string StarDateTime;
        public static string lisanslamakodu = "123chngr1";
        public static string ExpTime;
        public static string lisans;
        public static string serverip;
        private static MySqlConnection baglantim;
        private static readonly string programDizini = Path.GetDirectoryName(Application.ExecutablePath);

        private static IntPtr hhook = IntPtr.Zero;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        private readonly LowLevelKeyboardProc _proc = hookProc;
        private static bool cikis = false;
        public string ekranlink;
        private bool exitThread;
        private bool exitThread1 = true;
        private bool goster;
        private string host;
        private static string mac_adress;
        private static string localComputerName;
        private string tur;
        private string sonuc = "";
        private int isletimsistemi;
        public string local_ip, devicess;
        private readonly UsbManager manager;
        public bool online = true, dokunmatik_kontrol = false;
        // private Ping ping = new Ping();
        private string saat1;
        private string saat2;
        private string cihaz1;
        private string cihaz2;
        private string inkversiyon;
        public static string serial2;
        private string lines;
        private readonly string sistem = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
        public string sonuc3;
        private readonly int sure = 1000;
        private Thread t;
        private Thread ts;
        private Thread tt1;
        private Thread oto;
        public ManagementObjectSearcher theSearcher = new ManagementObjectSearcher("SELECT VolumeSerialNumber,Size FROM Win32_LogicalDisk WHERE DriveType = 2");
        public string usbadi, disk, disk2;
        private readonly Version version = Assembly.GetEntryAssembly().GetName().Version;
        private readonly Yoklama yoklama = new Yoklama();
        public string yoklamacalistir;
        private bool yonetim;
        private bool frm_drm;
        private bool hata;

        public Form1()
        {
            InitializeComponent();
            if (Process.GetProcessesByName(Assembly.GetEntryAssembly().GetName().Name).Count() > 0)
            {
                exe_Kapat("screensaver.exe");
            }
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            manager = new UsbManager();
            var disks = manager.GetAvailableDisks();
            manager.StateChanged += DoStateChanged;
            mac_adress = GetMACAddress();
            var win8version = new Version(6, 2, 9200, 0);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8version)
            {
                isletimsistemi = 8;
            }
            else
            {
                isletimsistemi = 7;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80; // Turn on WS_EX_TOOLWINDOW
                return cp;
            }
        }

        [DllImport("user32")]
        public static extern void LockWorkStation();

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        // ... { GLOBAL HOOK }
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance,
            uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        public void SetHook()
        {
            var hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
            uint uFlags);

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                var vkCode = Marshal.ReadInt32(lParam);
                //////ОБРАБОТКА НАЖАТИЯ
                if (vkCode.ToString() == "162")
                {
                    //  MessageBox.Show("You pressed a CTR");
                }
                return (IntPtr)1;
            }
            return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }


        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // ... { GLOBAL HOOK }// ... { GLOBAL HOOK }
        public void uygulamakapama(string x)
        {
            var processInfo = new ProcessStartInfo
            {
                Arguments = "/f /im " + x + ".exe",
                FileName = "taskkill.exe",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(processInfo);
        }

        public string GetMACAddress()
        {
            var _tmp = new ComputerInfo();
            return _tmp._macAddress;
        }

        public void exe_Calistir()
        {
            frm_drm = false;
            shutdownEvt = false;
#if DEBUG
#else
            SetHook();
            uygulamakapama("explorer");
#endif
            sayfa_ciz();
            Show();
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            lines = null;
            exitThread = false;
            yonetim = false;
            if (isletimsistemi < 8)
            {
#if DEBUG
#else
                Taskbar.Hide(); 
#endif
            }
            if (inkversiyon == "3.1.1764.0")
            {
                uygulamakapama("smartink");
                uygulamakapama("SMARTInk-SBSDKProxy");
            }
        }

        private void masaustugel()
        {
            Hide();
            shutdownEvt = true;
            if (Process.GetProcessesByName("explorer").Length == 0)
            {
                Process.Start(Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"));
            }
            if (isletimsistemi < 8)
            {
                Taskbar.Show();
            }
            UnHook();
            exitThread = true;
            frm_drm = true;
            if (inkversiyon == "3.1.1764.0")
            {
                var p = new Process();
                p.StartInfo.FileName = sistem + @"\SMART Technologies\SMART Product Drivers\Smartink.exe";
                p.StartInfo.Arguments = "-a";
                p.Start();
            }
        }

        public void update_myself()
        {
            try
            {
                Process.Start(programDizini + "\\update.exe");
            }
            catch (Exception)
            {
                var webClients = new WebClient();
                webClients.DownloadFile("https://developer.venov.com.tr/update/update.exe", programDizini + "\\update.exe");
                Process.Start(programDizini + "\\update.exe");
            }
        }

        public void exe_Kapat(string processName)
        {
            try
            {
                // Process[] process = System.Diagnostics.Process.GetProcessesByName(processName);
                foreach (var procces in Process.GetProcessesByName(processName))
                {
                    procces.Kill();
                }
            }
            catch
            {
                Thread.Sleep(1000);
                try
                {
                    foreach (var procces in Process.GetProcessesByName(processName))
                    {
                        procces.Kill();
                    }
                }
                catch
                {
                }
            }
        }

        public string SqlSorgu(string sorgu)
        {
            try
            {
                if (baglantim == null || baglantim.State.ToString() != "Open")
                {
                    var bag = new MySqlConnectionStringBuilder();
                    bag.Server = serverip;
                    bag.Port = 3306;
                    bag.UserID = "mobile";
                    bag.Password = "1234";
                    bag.Database = "screensaver";
                    baglantim = new MySqlConnection(bag.ToString());
                }
                if (baglantim.State.ToString() != "Open")
                {
                    try
                    {
                        baglantim.Open();
                        hata = false;
                    }
                    catch (Exception m)
                    {
                        throw m;
                        hata = true;
                    }
                }
                var komut = new MySqlCommand();
                komut.Connection = baglantim;
                komut.CommandText = sorgu;
                tur = sorgu.Substring(0, sorgu.IndexOf(' '));

                if (tur.ToLower() == "SELECT".ToLower())
                {
                    try
                    {
                        var tmp = komut.ExecuteScalar();

                        sonuc = tmp != null ? tmp != DBNull.Value ? tmp.ToString() : "" : "";
                    }
                    catch (Exception ex)
                    {

                        //Console.WriteLine(ex.ToString());
                        sonuc = "";
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


            catch (Exception)
            {
                // MessageBox.Show(e.ToString());
                return "";
            }
        }

        public void Serverkontrol()
        {
            var registryKey = Registry.CurrentUser.CreateSubKey(@"Kilit");
            if (registryKey?.GetValue("Date") == null || registryKey?.GetValue("Date") == "")
            {
                var tmp =
                    DateTime.Today.Date;
                //registryKey?.SetValue("Date", Crypto.SifreleAES(tmp.ToString(), lisanslamakodu));
                //try cache yazılıcak
            }
            try
            {
                var registry = Registry.CurrentUser.CreateSubKey("Kilit");
                registry.SetValue("Licance", lisans);
                registry.SetValue("Server", serverip);
                registry.SetValue("Timer1", saat1);
                registry.SetValue("Timer2", saat2);
                //registry.SetValue("Data", ExpTime);
                //  registry.SetValue("Date",StarDateTime);
            }
            catch (Exception)
            {
                var registrKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\Kilit");
                registrKey.SetValue("Licance", lisans);
                registrKey.SetValue("Server", serverip);
                registrKey.SetValue("Timer1", saat1);
                registrKey.SetValue("Timer2", saat2);
               // registrKey.SetValue("Data", ExpTime);
            }
        }

        public static string ipadresi()
        {
            var _tmp = new ComputerInfo();
            return _tmp._ipAddress;
        }

        public void status_check()
        {
            var sonuc2 = SqlSorgu("select count(*) from client_list1 where mac_adress='" + GetMACAddress() + "'");
            if (sonuc2 == "1")
            {
                SqlSorgu("UPDATE client_list1 SET durum='1', plugged_flash='',pc_name='" + localComputerName +
                         "',plugged_date='',ip_adress='" + local_ip + "',versiyon='" + version + "' WHERE mac_adress='" +
                         mac_adress + "'");
            }
            if (sonuc2 == "0")
                SqlSorgu(
                    "INSERT INTO client_list1(ip_adress,durum,pc_name,mac_adress,time1,time2,settings1,url,plugged_flash,plugged_date,mesaj,lisans,versiyon) VALUES ('" +
                    local_ip + "','1','" + localComputerName + "','" + mac_adress +
                    "','17:00','18:30','0','https://developer.venov.com.tr/kilitekran','','','','" + lisans + "','" + version + "')");
            yoklamacalistir = SqlSorgu("select settings1 from client_list1 where mac_adress='" + GetMACAddress() + "'");
        }

        public void update_status(string ogretmen, string date, string durum)
        {
            SqlSorgu("UPDATE client_list1 SET plugged_flash='" + ogretmen + "',plugged_date='" + date + "', durum='" +
                     durum + "'  WHERE  mac_adress='" + mac_adress + "'");
        }

        public void serialcek()
        {
            while (true)
            {
                while (!exitThread)
                {
                    foreach (ManagementObject currentObject in theSearcher.Get())
                    {
                        try
                        {
                            serial2 = "ZEYNA" + " " + currentObject["Size"].ToString() + " " + currentObject["VolumeSerialNumber"].ToString();
#if DEBUG
                            Console.WriteLine(serial2);
#endif
                            currentObject.Dispose();
                            Thread.Sleep(1000);
                        }
                        catch (Exception x)
                        {
                           // Console.WriteLine(x.ToString());
                        }
                    }
                }
            }
        } 
                       
        public void screenupdate()
        {
            ekranlink = SqlSorgu("Select url from client_list1 where mac_adress='" + GetMACAddress() + "'");
            try
            {
                webBrowser1.Navigate(new Uri(ekranlink),"_self");
            }
            catch (System.Exception e )
            {
                SqlSorgu("UPDATE client_list1 SET url='https:/developer.venov.com.tr/kilitekran/'  WHERE  mac_adress='" + mac_adress + "'");
                //Console.Write(e.ToString());
            }
        }
        public void onlinehizmet()
        {
            while (online)
            {
                try
                {
                    sonuc3 = SqlSorgu("select durum from client_list1 where  mac_adress='" + mac_adress + "'");
                    switch (sonuc3)
                    {
                        case "200":
                            masaustugel();
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='0'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            exe_Kapat("screensaver");
                            break;
                        case "0":
                            update_status("", "", "1");
                            break;
                        case "10":
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='0'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            shutdownEvt = true;
                            Process.Start("shutdown", "/s /t 0 /f");
                            break;
                        case "13":
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='0'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            shutdownEvt = true;
                            Process.Start("shutdown", "/r /t 0 /f");
                            break;
                        case "14":
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='0'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            ExitWindowsEx(0, 0);
                            break;
                        case "80":
                            update_myself();
                            break;
                        case "35":
                            masaustugel();
                            yonetim = true;
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='2'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            break;
                        case "40":
                            exe_Calistir();
                            yonetim = false;
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='1'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            break;
                        case "27":
                            screenupdate();
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='40'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            break;
                        case "50":
                            var d = new Mesaj();
                            sonuc = "Tenefüse Çıkabilirsiniz";
                            d.label1.Text = sonuc;
                            d.ShowDialog();
                            SqlSorgu(
                                "UPDATE client_list1 SET plugged_flash='',plugged_date='', durum='1'  WHERE  mac_adress='" +
                                mac_adress + "'");
                            break;
                    }
                    Thread.Sleep(sure);
                }
                catch (Exception m)
                {
                    Console.WriteLine("Server Bağlantısı Sağlanamadı."+m.ToString());
                    hata = true;
                    //  File.WriteAllText(programDizini + "\\Errors\\Onlinelogs.txt", DateTime.Now.ToString() + "  " + m.ToString() + "\n
                }
            }
        }

        private void mesaj_al()
        {
            while (online)
            {
                if (cikis == false)
                {
                    try
                    {
                        var sonuc200 =
                            SqlSorgu("select mesaj from client_list1 where mac_adress='" + GetMACAddress() + "'");
                        if (sonuc200.Length <= 5)
                        {
                            goster = false;
                        }
                        else
                        {
                            goster = true;
                        }
                        if (goster)
                        {
                            var d = new Mesaj();
                            d.label1.Text = sonuc200;
                            d.ShowDialog();
                            SqlSorgu("UPDATE client_list1 SET mesaj='" + null + "'  WHERE mac_adress='" +
                                     GetMACAddress() + "'");
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                Thread.Sleep(sure);
            }
        }

        private void ipcek()
        {
            localComputerName = Dns.GetHostName();
            host = Dns.GetHostName();
            var ip = Dns.GetHostEntry(host);
            local_ip = ipadresi();
        }

        private void otomatik_kapatma()
        {
            while (true)
            {
                if (DateTime.Now.ToShortTimeString() == saat1)
                {
                    shutdownEvt = true;
                    Process.Start("shutdown", "/s /t 0");
                }
                if (DateTime.Now.ToShortTimeString() == saat2)
                {
                    shutdownEvt = true;
                    Process.Start("shutdown", "/s /t 0");
                }
                if (yonetim == false)
                {
                    if (Process.GetProcessesByName("Taskmgr").Length > 0)
                    {
                        exe_Kapat("Taskmgr");
                    }
                    if (Process.GetProcessesByName("Mmc").Length > 0)
                    {
                        exe_Kapat("Mmc");
                    }
                    if (Process.GetProcessesByName("Regedit").Length > 0)
                    {
                        exe_Kapat("Regedit");
                    }
                    if (Process.GetProcessesByName("cmd").Length > 0)
                    {
                        exe_Kapat("cmd");
                    }
                }
                if (yonetim && hata)
                {
                    exe_Calistir();
                }
                if (frm_drm == false)
                {
                    if (Process.GetProcessesByName("osk").Length > 0)
                    {
                        exe_Kapat("osk");
                    }
                    if (Process.GetProcessesByName("chrome").Length > 0)
                    {
                        exe_Kapat("chrome");
                    }
                    if (Process.GetProcessesByName("iexplore").Length > 0)
                    {
                        exe_Kapat("iexplore");
                    }
                    if (Process.GetProcessesByName("firefox").Length > 0)
                    {
                        exe_Kapat("firefox");
                    }
                    if (Process.GetProcessesByName("Orient").Length > 0 && inkversiyon != null)
                    {
                        exe_Kapat("Orient");
                    }
                }

                Thread.Sleep(sure);
            }
        }

        public void sayfa_ciz()
        {
            ekranlink = SqlSorgu("select url from client_list1 where mac_adress='" + GetMACAddress() + "'");
            var bb = NetworkInterface.GetIsNetworkAvailable();
            if (bb && ekranlink != "")
            {
                if (ekranlink.Length > 6)
                {
                    webBrowser1.Navigate(new Uri(ekranlink));
                }
            }
            else
            {
                var text1 = "<!DOCTYPE html><html><head><script>";
                var text2 =
                    "function startTime() {var today=new Date();var h=today.getHours();var m=today.getMinutes();var s=today.getSeconds();m = checkTime(m);s = checkTime(s);document.getElementById('txt').innerHTML = h+\":\"+m+\":\"+s;var t = setTimeout(function(){startTime()},500);}";
                var text3 = "function checkTime(i) {if (i<10) {i = \"0\" + i};return i;}";
                var text4 =
                    "</script><style>#txt{width: 100px;height: 100px;position: absolute;left: 50%;top: 50%;margin-left: -160px;margin-top: -95px;color:#FFF;	font-size:120px;}</style></head><body onload=\"startTime()\" bgcolor=\"#000000\"><div id=\"txt\"></div></body></html>";
                webBrowser1.DocumentText = text1 + text2 + text3 + text4;
            }
            ((Control) webBrowser1).Enabled = false;
            ShowInTaskbar = false;
            ResumeLayout(dokunmatik_kontrol);
        }

        public void StartupFunc()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                var keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                var valueName = "Zeyna";
                if (Registry.GetValue(keyName, valueName, null) == null)
                {
                    //code if key Not Exist
                }
                else
                {
                    key.DeleteValue("Zeyna");
                }
                var newkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon",
                    true);
                newkey.SetValue("Shell", "explorer.exe C:\\screensaver\\Screensaver.exe");
                var newkey2 =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\system", true);
                newkey2.SetValue("DisableTaskmgr", 00000001, RegistryValueKind.DWord);
            }
            catch (Exception)
            {
            }
        }

        public void GetRegisterInfo()
        {
            var currentUser = Registry.CurrentUser.OpenSubKey("Kilit");
            var localUser = Registry.LocalMachine.OpenSubKey(@"SYSTEM/Kilit");
            serverip = localUser?.GetValue("Server")?.ToString() ?? currentUser?.GetValue("Server")?.ToString() ?? "developer.venov.com.tr";
            saat1 = localUser?.GetValue("Timer1")?.ToString() ?? currentUser?.GetValue("Timer1")?.ToString() ?? "17:30";
            saat2 = localUser?.GetValue("Timer2")?.ToString() ?? currentUser?.GetValue("Timer2")?.ToString() ?? "18:30";
            lisans = localUser?.GetValue("Licance")?.ToString() ?? currentUser?.GetValue("Licance").ToString() ?? "demo";
            inkversiyon =Registry.LocalMachine.OpenSubKey(
                                @"SOFTWARE/Wow6432Node/SMART Technologies/SMART Ink/Install Information/Version")?
                                .ToString() ?? "";
            }

        public void GetClientInfoFromServer()   
        {
          var currentUser = Registry.CurrentUser.OpenSubKey("Kilit");
          var localUser = Registry.LocalMachine.OpenSubKey(@"SYSTEM/Kilit");
            try
            {
                ExpTime = currentUser?.GetValue("Data")?.ToString() ??
                                  localUser?.GetValue("Data")?.ToString();
                StarDateTime = Crypto.SifreyiCozAES(currentUser?.GetValue("Date")?.ToString() ?? localUser?.GetValue("Date")?.ToString(), lisanslamakodu);
            }
            catch (Exception)
            {
                ExpTime = SqlSorgu("Select device from settings where lisans='"+lisans+"'");
                StarDateTime = SqlSorgu("SELECT CURRENT_TIMESTAMP()");
            }
            yoklamacalistir = "0";
          
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ipcek();
            StartupFunc();
            GetRegisterInfo();
            exe_Calistir();
            //GetClientInfoFromServer();
            status_check();
            FormClosing += Form1_FormClosing;
            t = new Thread(serialcek);
            ts = new Thread(onlinehizmet);
            tt1 = new Thread(mesaj_al);
            CheckForIllegalCrossThreadCalls = false;
            t.Start();
            ts.Start();
            tt1.Start();
            Serverkontrol();
#if DEBUG
            this.WindowState = FormWindowState.Normal;

#else
            oto = new Thread(new ThreadStart(otomatik_kapatma));
            oto.Start();
#endif
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!shutdownEvt)
                e.Cancel = true;
            else
            {
                SqlSorgu("UPDATE client_list1 SET durum='0', plugged_flash='',pc_name='" + localComputerName +
                         "',plugged_date='',ip_adress='" + local_ip + "',versiyon='" + version + "' WHERE mac_adress='" +
                         mac_adress + "'");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if ((msg.Msg == 0x104) && ((int) msg.LParam == 0x203e0001))
                    return true;

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            if (e.State == UsbStateChange.Added)
            {
                exitThread1 = true;
                var durum = File.Exists(e.Disk.Name + "Screensaver/keys");
                if (durum)
                {
                    lines = Crypto.SifreyiCozAES(File.ReadAllText(e.Disk.Name + "Screensaver/keys"), lisans);
                    var lines2 = File.ReadAllText(e.Disk.Name + "Screensaver/keys");
                    while (exitThread1)
                    {
                        if (lines == serial2)
                        {
                            masaustugel();
                            disk2 = e.Disk.Name;
                            frm_drm = true;
                            yonetim = false;
                            update_status(lines2, DateTime.Now.ToString(), "2");
                            if (online)
                            {
                                try
                                {
                                    if (yoklamacalistir == "1")
                                    {
                                        yoklama.textBox1.Text = lines2;
                                        var asd = new DialogResult();
                                        asd = MessageBox.Show("Yoklama Alınsın Mı?", "Yoklama", MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question);
                                        if (asd == DialogResult.Yes)
                                        {
                                            try
                                            {
                                                yoklama.Show();
                                            }
                                            catch (Exception)
                                            {
                                                yoklama.WindowState = FormWindowState.Normal;
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            serial2 = null;
                            exitThread1 = false;
                        }
                        else
                        {
                            //  MessageBox.Show("Tanımsız Flash Takıldı");
                            serial2 = null;
                            exitThread = false;
                            exitThread1 = false;
                        }
                    }
                }
            }
            if (e.State == UsbStateChange.Removed && disk2 == e.Disk.Name)
            {
                exe_Calistir();
                serial2 = null;
                exitThread1 = true;
                yonetim = false;
                lines = null;
                if (online)
                {
                    update_status("", "", "1");
                }
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}