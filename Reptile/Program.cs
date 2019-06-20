using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Reptile
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //var url = ServiceBll.GetUrl();//"https://cn.bing.com/?ensearch=1&FORM=BEHPTB"
            //var fileName = ServiceBll.WriteBytesToFile("img/", url);
            //ServiceBll.UpdateWindowsDesk(fileName);
            //Console.WriteLine("Success");
            //Console.ReadKey();


        }

    }
}
