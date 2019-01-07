using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql;
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;
namespace Screensaver
{
    public partial class Yoklama : Form
    {
        static MySqlConnection baglantim = null;
        static public string key;
        string gelmeyenler;
        static string programDizini = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        string sonuc, tur = "";
        public Yoklama()
        {
            InitializeComponent();
        }
        public int lisansToid()
        {
            //textBox1.Text;
            return Convert.ToInt32(SqlSorgu("Select okul_id from ogretmen where ogretmen_adi='"+textBox1.Text+"'"));
        }
        public string SqlSorgu(string sorgu)
        {
            try
            {
                if (baglantim == null)
                {
                    MySqlConnectionStringBuilder bag = new MySqlConnectionStringBuilder();
                    bag.Server = Form1.serverip;
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
                    catch (Exception m)
                    {
                        File.WriteAllText(programDizini + "\\Errors\\Mysqllogs2.txt", DateTime.Now.ToString() + "  " + m.ToString() + "\n");

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
        public void ogretmen_adi(string kodu)
        {
            try
            { 
                string ogretmen_adi = SqlSorgu("select ogretmen_adi from keyler where usb_anahtar='" + kodu + "'");        
                textBox1.Text = ogretmen_adi;
            }
            catch (Exception)
            {
                MessageBox.Show("Öğretmen Adı seçilemedi");
            }
          
        }
        public void sinif()
        {
            try
            {
                MySqlConnectionStringBuilder bag = new MySqlConnectionStringBuilder();
                bag.Server = Form1.serverip;
                bag.UserID = "mobile";
                bag.Password = "1234";
                bag.Database = "screensaver";
                MySqlConnection baglantim = new MySqlConnection(bag.ToString());
                baglantim.Open();
                MySqlCommand komut = new MySqlCommand();
                komut.Connection = baglantim;
                komut.CommandText = "select * from class where okul_id="+lisansToid()+"  order by sinif";

                MySqlDataReader myReader = komut.ExecuteReader();
                var items = comboBox1.Items;

                while (myReader.Read())
                {
                    items.Add(myReader.GetString(0));

                }

                myReader.Close();
                baglantim.Close();
                // comboBox1.SelectedIndex = 0;
            }
            catch (Exception )
            {
            }
        }
        public void ogrenci_getir(string degisken)
        {
            MySqlConnectionStringBuilder bag = new MySqlConnectionStringBuilder();
            bag.Server = Form1.serverip;
            bag.UserID = "mobile";
            bag.Password = "1234";
            bag.Database = "screensaver";
            MySqlConnection baglantim = new MySqlConnection(bag.ToString());
            baglantim.Open();
            MySqlCommand komut = new MySqlCommand();
            komut.Connection = baglantim;
            komut.CommandText = "select * from students where okul_id="+lisansToid()+" and class='" + degisken + "'";
            MySqlDataReader myReader = komut.ExecuteReader();

            var items = checkedListBox1.Items;
            try
            {
                while (myReader.Read())
                {
                    items.Add(myReader.GetString(2) + " " + myReader.GetString(1));
                }
            }
            finally
            {
                myReader.Close();
                baglantim.Close();
            }
        }
        private void Yoklama_Load(object sender, EventArgs e)
        {
            //this.ControlBox = false;         
            sinif();
        }
       

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = SqlSorgu("Select ogretmen_adi from keyler where usb_anahtar='" + textBox1.Text + "'"); ;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            ogrenci_getir(comboBox1.Text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1 )
            {
                MessageBox.Show("Lütfen ders seçimini yapınız.");
            }
            else { 
            foreach (object itemChecked in checkedListBox1.CheckedItems)
            {
                string s = itemChecked.ToString();
                string[] ogr_no = Regex.Split(s, " ");
                gelmeyenler = ogr_no[0] + "," + gelmeyenler;
            }
            MySqlConnectionStringBuilder bag = new MySqlConnectionStringBuilder();
            bag.Server = Form1.serverip;
            bag.UserID = "mobile";
            bag.Password = "1234";
            bag.Database = "screensaver";
            MySqlConnection baglantim = new MySqlConnection(bag.ToString());
            baglantim.Open();
            MySqlCommand komut = new MySqlCommand();
            komut.Connection = baglantim;
            komut.CommandText = "SELECT count(*) FROM yoklama WHERE tarih='" + DateTime.Now.ToShortDateString() + "' and ders='" + comboBox2.SelectedItem.ToString()+"' and sinif='"+comboBox1.SelectedItem.ToString()+"'";
            string sonuc = komut.ExecuteScalar().ToString();
            if (sonuc == "1")
            {
             //   MessageBox.Show(komut.CommandText);
                MessageBox.Show("Bu Dersin Yoklaması alınmıştır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                gelmeyenler = null;
            }
            else
            {
                if (comboBox2.SelectedItem == null || comboBox1.SelectedItem == null)
                {
                     MessageBox.Show("Gerekli alanları doldurunuz ! ", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     gelmeyenler = null;
                }
                else
                {
                    komut.CommandText = "INSERT INTO yoklama(okul_id,tarih, ogr_adi, ogr_no, sinif, ders) VALUES ("+lisansToid()+",'" + DateTime.Now.ToShortDateString() + "','" + textBox1.Text + "','" + gelmeyenler + "','" + comboBox1.Text + "','" + comboBox2.Text + "')";
                    komut.ExecuteNonQuery();
                    baglantim.Close();
                    gelmeyenler = null;
                    if ((MessageBox.Show("Yoklama Başarı ile alındı", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information)) == DialogResult.OK)
                    {
                        this.Hide();
                    } 
                }
               
            }
        }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }
    }
}