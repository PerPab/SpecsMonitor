using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Management;
using System.Globalization;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Windows.Media.Converters;
using Microsoft.VisualBasic.Devices;
using System.IO;
using Microsoft.VisualBasic.ApplicationServices;
using ProgressBar = System.Windows.Controls.ProgressBar;

namespace NuevaApp
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            GetUser();
            GetVersionOS();
            GetCPU();
            GetModel();
            GetVideo();
            GetBattery();
            GetTotalRam();
            GetRamSpeed();
            GetInfoDisk();



        }

        public void GetUser()
        {
            string userName = Environment.UserName;
            usuarioText.Text = userName.ToUpper();
        }

        public void GetVersionOS()
        {

            ComputerInfo OSInfo = new ComputerInfo();
            OStext.Text = $"{OSInfo.OSFullName} {OSInfo.OSVersion}".ToUpper();

        }

        public void GetCPU()
        {
            SelectQuery Sq = new SelectQuery("Win32_Processor");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();
            foreach (ManagementObject mo in osDetailsCollection)
            {
                sb.AppendLine(string.Format("{0}", (string)mo["Name"]));

            }
            CPUtext.Text = sb.ToString().ToUpper();
        }

        public void GetModel()
        {
            var fabricante = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS").GetValue("SystemManufacturer").ToString();
            var modelo = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS").GetValue("SystemProductName").ToString();
            modeloText.Text = $"{fabricante} {modelo}".ToUpper();
        }

        public void GetVideo()
        {
            SelectQuery Sq = new SelectQuery("Win32_VideoController");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();
            foreach (ManagementObject mo in osDetailsCollection)
            {
                sb.Append(string.Format("{0} ", (string)mo["Name"]));

            }
            videoTexto.Text = sb.ToString().ToUpper();

        }

        public void GetBattery()
        {
            PowerStatus pwr = SystemInformation.PowerStatus;

            bateriaText.Text = pwr.PowerLineStatus.ToString().ToUpper() == "ONLINE" ? "CARGANDO" : "DESCARGANDO";

            bar.Value = (int)(pwr.BatteryLifePercent * 100);
        }



        public void GetTotalRam()
        {

            ulong ram = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            const ulong GB_BYTES = 1024 * 1024 * 1024;
            ulong ramGB = (ulong)Math.Round((double)ram / GB_BYTES);
            ramTotal.Text = $"{ramGB} GB";

            ulong ramDis = new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
            ulong ramDisGb = (ulong)Math.Round((double)ramDis / GB_BYTES);
            ramDisp.Text = $"{ramDisGb} GB";

            barRam.Maximum = ramGB;
            barRam.Value = ramGB - ramDisGb;

        }

        public void GetRamSpeed()
        {
            SelectQuery Sq = new SelectQuery("Win32_PhysicalMemory");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();
            foreach (ManagementObject mo in osDetailsCollection)
            {
                sb.Append(mo["Speed"]);

            }
            int Hz = 10000;
            var Mhz = Double.Parse(sb.ToString()) / Hz;

            ramVel.Text = $"{Mhz} MHz";

        }

        public class Disco
        {
            public string unidad { get; set; }
            public string capTotal { get; set; }
            public string capDis { get; set; }
            public string formato { get; set; }
            public long progreso { get; set; }
        }

        public void setBar()
        {
            
        }

        public void GetInfoDisk()
        {

            List<Disco> discosDisponibles = new List<Disco>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                var GB = 1024 * 1024 * 1024;

                ProgressBar pb = new ProgressBar();

                pb.Width = 130;
                pb.Minimum = 0;
                pb.Maximum = drive.TotalSize / GB;
                pb.Value = (drive.TotalSize / GB) - (drive.TotalFreeSpace / GB);


                discosDisponibles.Add(new Disco()
                {

                    unidad = $"{drive.Name} {drive.VolumeLabel}",
                    capTotal = $"{drive.TotalSize / GB} GB",
                    capDis = $"{drive.TotalFreeSpace / GB} GB",
                    formato = drive.DriveFormat,
                    progreso = (drive.TotalSize / GB) - (drive.TotalFreeSpace / GB)

                }) ;

                discos.ItemsSource = discosDisponibles;
                

            }
            
        }
    }
}








