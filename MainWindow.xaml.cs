using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RemoteScreenshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool mainTimerState = false;
        DispatcherTimer mainTimer = null;
        public MainWindow()
        {
            InitializeComponent();
            mainTimer = new DispatcherTimer();
            mainTimer.Tick += MainTimer_Tick;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            ScreenShot();
        }

        public static System.Drawing.Image ByteArrayToImage(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }
        private static string CreateImageName()
        {
            //Random rnd = new Random();
            return $"{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";
        }
        private void screenshotBtn_Click(object sender, RoutedEventArgs e)
        {
            ScreenShot();
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ScreenShot() {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipTxtBox.Text), int.Parse(portTxtBox.Text));

            //Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            UdpClient client = new UdpClient();

            try
            {
                client.Connect(endPoint);

                byte[] data = Encoding.Unicode.GetBytes("Screen");
                client.Send(data, data.Length);


                byte[] bytes = null;

                do
                {
                    bytes = client.Receive(ref endPoint);


                    //fs.Write(data, 0, bytes);

                } while (client.Available > 0);
                string fileName = CreateImageName()/*+".bmp"*/;
                System.Drawing.Image img = ByteArrayToImage(bytes);
                img.Save(fileName, ImageFormat.Jpeg);
                //var uri_source = new Uri(/*Directory.GetCurrentDirectory()+*/fileName+".jpg",UriKind.Relative);
                //MessageBox.Show(uri_source.LocalPath);
                var source = new BitmapImage();
                source.BeginInit();
                source.StreamSource = new MemoryStream(bytes);
                source.EndInit();
                screenImage.Source = source;
                //screenImage.Source = new BitmapImage(uri_source);
                //Bitmap bitmap = BitmapFactory.decodeByteArray(BitmapData, 0, bitmapdata.length);
                //using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                //{
                //    // зчитуємо файл в масив байтів
                //    byte[] fileData = new byte[fs.Length];
                //    fs.Read(fileData, 0, fileData.Length);

                //    client.Send(fileData, fileData.Length);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ////client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mainTimer != null)
            {
                mainTimer.Interval = TimeSpan.FromSeconds(mainSlider.Value);
            }
        }

        private void timerStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mainTimerState)
            {
                mainTimer.Stop();
                timerStartBtn.Content = "Stop Timer";

            }
            else
            {
                mainTimer.Start();
                timerStartBtn.Content = "Start Timer";
            }
            mainTimerState = !mainTimerState;
        }
    }
}
