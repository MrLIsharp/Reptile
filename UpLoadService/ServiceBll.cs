using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Reptile
{
    public class ServiceBll
    {
        
        //引用Userdll
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
          int uAction,
          int uParam,
          string lpvParam,
          int fuWinIni
          );
        /// <summary>
        /// 設置圖片桌面
        /// </summary>
        /// <param name="randomFileName"></param>
        public static void UpdateWindowsDesk(string randomFileName)
        {
            //Image image = Image.FromFile("E:\\新建文件夹\\test.jpg");
            //var PicName = "E:\\新建文件夹\\" + DateTime.Now.ToShortTimeString() + ".bmp";
            var PicName = AppDomain.CurrentDomain.BaseDirectory+"img/" + randomFileName;
            //image.Save(PicName, System.Drawing.Imaging.ImageFormat.Bmp);
            SystemParametersInfo(20, 0, PicName, 0x2);
            // MessageBox.Show(PicName);
        }

        /// <summary>
        /// 下载图片到指定文件夹
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        public static string WriteBytesToFile(string fileName, string path)
        {
            string newPath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            //string newPath = string.Format(@"D:\UPLOAD\");//目标地址

            string ImsFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".bmp"; //目标文件名

            System.Net.WebRequest imgRequest = System.Net.WebRequest.Create(path);
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)imgRequest.GetResponse();
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;
            }
            if (res.StatusCode.ToString() == "OK")
            {
                Image dwonImage = Image.FromStream(imgRequest.GetResponse().GetResponseStream());
                if (!Directory.Exists(newPath))//目标地址不存在自动创建
                {
                    Directory.CreateDirectory(newPath);
                }
                dwonImage.Save(newPath + ImsFileName);
                dwonImage.Dispose();
            }
            return ImsFileName;
        }
        /// <summary>
        /// 抓取地址
        /// </summary>
        /// <returns></returns>
        public static string GetUrl(string Url= "https://cn.bing.com/?FORM=BEHPTB")
        {
           // var Url = "https://cn.bing.com/?ensearch=1&FORM=BEHPTB";
            var cityList = new List<fileImage>();
            var Crawler = new SimpleCrawler();
            Crawler.OnStart += (s, e) =>
            {
                //Console.WriteLine("爬虫开始抓取地址：" + e.Uri.ToString());
            };
            Crawler.OnError += (s, e) =>
            {
                Console.WriteLine("爬虫抓取出现错误：" + e.Message);
            };
            Crawler.OnCompleted += (s, e) =>
            {
                var links = Regex.Matches(e.PageSource, @"<link (.*?) href=(.*?)*(?<href>)>", RegexOptions.IgnoreCase);
                foreach (Match match in links)
                {
                    var href = Regex.Matches(match.Value, "(?<=href=\")(.*?)\"", RegexOptions.IgnoreCase);
                    foreach (Match item in href)
                    {
                        var city = new fileImage
                        {
                            Name = DateTime.Now.ToShortDateString(),
                            Uri = new Uri("https://cn.bing.com/" + item.Value)
                        };
                        if (!cityList.Contains(city))
                        {
                            cityList.Add(city);//将数据加入到泛型列表
                        }
                        //Console.WriteLine(city.Name + "|" + city.Uri);
                    }
                }
                ////Console.WriteLine(e.PageSource);
                //Console.WriteLine("================================");
                //Console.WriteLine("爬虫抓取任务完成！合计" + links.Count + "个");
                //Console.WriteLine("耗时：" + e.Milliseconds + "毫秒");
                //Console.WriteLine("线程：" + e.ThreadId);
                //Console.WriteLine("地址：" + e.Uri.ToString());

            };

            Crawler.Start(new Uri(Url)).Wait();//没被分所就别使用代理
            return cityList.ToList().FirstOrDefault().Uri.ToString();
        }
    }

    public class fileImage
    {
        public string Name { get; set; }

        public Uri Uri { get; set; }
    }
}
