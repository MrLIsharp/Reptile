using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Reptile
{
    public class SimpleCrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;

        public event EventHandler<OnCompleteedEventArgs> OnCompleted;

        public event EventHandler<Exception> OnError;//爬虫出错事件

        public CookieContainer CookieContainer { get; set; } //定义 cookie 容器
        public async Task<string> Start(Uri uri, WebProxy Proxy = null)
        {
            return await Task.Run(() =>
            {
                var pageSource = string.Empty;
                try
                {
                    if (this.OnStart != null) this.OnStart(this, new OnStartEventArgs(uri));

                    var watch = new Stopwatch();
                    watch.Start();
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Accept = "*/*";
                    request.ContentType = "application/x-www-form-urlencoded";//定义文档类型及编码
                    request.AllowAutoRedirect = false;//禁止自动跳转
                    //设置User-Agent，伪装成 Google Chrome浏览器
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";
                    request.Timeout = 5000;//请求超时时间
                    request.KeepAlive = true;//启用长连接
                    request.Method = "GET";
                    if (Proxy != null) request.Proxy = Proxy;//设置代理ip 伪装求取地址
                    request.CookieContainer = this.CookieContainer;//附加Cook容器
                    request.ServicePoint.ConnectionLimit = int.MaxValue;//定义最大连接数
                    var response = (HttpWebResponse)request.GetResponse();//获取请求响应
                    foreach (Cookie cookie in response.Cookies) this.CookieContainer.Add(cookie);//将cookie加入容器，保存登录状态
                    var stream = response.GetResponseStream();//获取响应流
                    var reader = new StreamReader(stream, Encoding.UTF8);//以UTF8的方法读取流
                    pageSource = reader.ReadToEnd();//获取网页源代码
                    watch.Stop();
                    var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;//获取当前任务线程ID
                    var milliseconds = watch.ElapsedMilliseconds;//获取请求执行时间
                    reader.Close();
                    stream.Close();
                    request.Abort();
                    response.Close();
                    //if (this.OnCompleted != null) this.OnCompleted(this, new OnCompleteedEventArgs(uri, threadId, milliseconds, pageSource));
                    this.OnCompleted?.Invoke(this, new OnCompleteedEventArgs(uri, threadId, milliseconds, pageSource));

                }
                catch (Exception ex)
                {
                    this.OnError?.Invoke(this, ex);
                }
                return pageSource;
            });
        }

        public SimpleCrawler() { }

    }
    //爬虫完成事件
    public class OnCompleteedEventArgs
    {
        public Uri Uri { get; private set; }

        public int ThreadId { get; private set; }//任务线程ID

        public string PageSource { get; private set; }//页面源代码

        public long Milliseconds { get; private set; }//爬虫请求执行事件


        public OnCompleteedEventArgs(Uri uri,int threadId,long milliseconds,string pageSource)
        {
            this.Uri = uri;
            this.ThreadId = threadId;
            this.Milliseconds = milliseconds;
            this.PageSource = pageSource;
        }
    }
    //爬虫启动事件
    public class OnStartEventArgs
    {
        public Uri Uri { get; set; }

        public OnStartEventArgs(Uri uri)
        {
            this.Uri = uri;
        }
    }
}
