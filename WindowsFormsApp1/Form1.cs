using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;




namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // stopボタンを無効化
            button4.Enabled = false;

            // レートに9600を追加
            comboBox2.Items.Add(9600);

            // デフォルトで選択されるレートを決定
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }

            // 接続されているSerialぽーとを選択
            string[] ports = SerialPort.GetPortNames();

            // 接続されているポートを全てコンボボックスに追加
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                MessageBox.Show("シリアルポートは既に接続されています", "シリアルポート");
            }
            else
            {
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.SelectedItem);
                serialPort1.Parity = Parity.None;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Handshake = Handshake.None;
                serialPort1.PortName = Convert.ToString(comboBox1.SelectedItem);
                serialPort1.Open();
                button1.Enabled = false;
            }
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            
            if (serialPort1.IsOpen)
            {
                button2.Enabled = false;
                button4.Enabled = true;

                // 測定開始時刻を取得
                DateTime startTime = DateTime.Now;


                serialPort1.Write("*RST" + "\n"); // Reset device setting (device command)
                serialPort1.Write(":SENS:FUNC 'TEMP'" + "\n"); // set to temperature mode (device command)
                serialPort1.Write(":SENS:CHAN 1" + "\n");
                serialPort1.Write(":SENS:TEMP:TRAN TC" + "\n");
                serialPort1.Write(":SENS:TEMP:RJUN:RSEL INTERNAL" + "\n");
                serialPort1.Write(":SENS:TEMP:RJUN:SIM 0" + "\n");
                serialPort1.Write(":SENS:TEMP:TC K" + "\n");

                
                for (; ; ) // 無限ループ
                {
                    

                    // 各データ点取得時刻を所得
                    DateTime nowTime = DateTime.Now;
                    

                    // データ取得時刻 - 測定開始時刻
                    TimeSpan dt = nowTime - startTime;
                    double Dt = Convert.ToDouble(dt.TotalMinutes); // double に変換
                    double DT = Math.Round(Dt, 3); //四捨五入
                    serialPort1.Write(":READ?" + "\n");
                    string str = serialPort1.ReadLine();
                    label4.Text = str;
                    label5.Text = Convert.ToString(DT);
                    double data = Convert.ToDouble(str);

                    

                    
                    // chartへの書き込み
                    chart1.Series["temp"].Points.AddXY(DT, data);

                    // csv ファイルへの書き込み
                    
                    string filename = startTime.ToString("yyyy-MM-dd-hh-mm-ss")+ "data.csv";
                    StreamWriter file = new StreamWriter(filename, true, Encoding.UTF8);
                    file.WriteLine(nowTime + "," + data + ",");
                    file.Close();

                    

                    await Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                    );

                    if (button4.Enabled == false)
                    {
                        button2.Enabled = true;
                        break;
                    }

                }
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
        
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            button4.Enabled = false;

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
