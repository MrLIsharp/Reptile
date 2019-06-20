using Reptile;
using System;
using System.ComponentModel;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace UpLoadService
{
    partial class Service1 : ServiceBase
    {
        //服务时间间隔
        private static System.Timers.Timer TimerTask;
        public Service1()
        {
            InitializeComponent();
            TimerTask = new System.Timers.Timer()
            {
                Interval = (1000*60*60*24),
                Enabled = true
            };
            TimerTask.Elapsed += new System.Timers.ElapsedEventHandler(TimedEvent);
        }
        string filePath = AppDomain.CurrentDomain.BaseDirectory + "/MyServiceLog.txt";
        private void TimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Append))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        string url = ServiceBll.GetUrl();
                        writer.WriteLine($"{DateTime.Now},爬取成功！=>URL"+url);
                        var fileName = ServiceBll.WriteBytesToFile("img/", url);
                        writer.WriteLine($"{DateTime.Now},下载成功！=>NAME"+fileName);
                        ServiceBll.UpdateWindowsDesk(fileName);
                        writer.WriteLine($"{DateTime.Now},桌面设置成功！");
                    }
                   
                }
                catch (Exception ex)
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Append))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        //if (!Directory.Exists(filePath))//目标地址不存在自动创建
                        //{
                        //    Directory.CreateDirectory(filePath);
                        //}
                        writer.WriteLine($"{DateTime.Now},程序错误！");
                        writer.WriteLine(ex);
                    }
                }
            });
        }     
        protected override void OnStart(string[] args)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                //if (!Directory.Exists(filePath))//目标地址不存在自动创建
                //{
                //    Directory.CreateDirectory(filePath);
                //}
                writer.WriteLine($"{DateTime.Now},服务启动！");
            }
            // TODO: 在此处添加代码以启动服务。
        }

        protected override void OnStop()
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"{DateTime.Now},服务停止!");
            }
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }
    }
}
