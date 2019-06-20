using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reptile
{
    public partial class Form1 : Form
    {

        public static readonly string serviceFilePath = AppDomain.CurrentDomain.BaseDirectory + "UpLoadService.exe";
        public static readonly string serviceName = "UploadService1";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.InstallService(serviceFilePath);
            MessageBox.Show("安装服务成功", "提示");
        }

        //判断服务是否存在
        private bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController sc in services)
            {
                if (sc.ServiceName.ToLower() == serviceName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        //安装服务
        private void InstallService(string serviceFilePath)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                IDictionary savedState = new Hashtable();
                installer.Install(savedState);
                installer.Commit(savedState);
            }
        }

        //卸载服务
        private void UninstallService(string serviceFilePath)
        {
            using (AssemblyInstaller installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                installer.Uninstall(null);
            }
        }
        //启动服务
        private void ServiceStart(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Stopped)
                {
                    control.Start();
                }
            }
        }

        //停止服务
        private void ServiceStop(string serviceName)
        {
            using (ServiceController control = new ServiceController(serviceName))
            {
                if (control.Status == ServiceControllerStatus.Running)
                {
                    control.Stop();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.ServiceStart(serviceName);
            MessageBox.Show("服务启动成功","提示");          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.ServiceStop(serviceName);
            MessageBox.Show("服务停止成功","提示" );
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.UninstallService(serviceFilePath);
            MessageBox.Show("服务卸载成功","提示");
        }
    }
}
