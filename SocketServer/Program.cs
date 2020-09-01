using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    class Program
    {
        // 创建一个和客户端通信的套接字
        static Socket socketWatch;

        static void Main(string[] args)
        {
            //定义一个套接字用于监听客户端发来的消息，包含三个参数（IP4寻址协议，流式连接，Tcp协议）
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //服务端发送信息需要一个IP地址和端口号  
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");

            //将IP地址和端口号绑定到网络节点point上  此端口专门用来监听的
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 5005);

            //监听绑定的网络节点 
            socketWatch.Bind(iPEndPoint);

            //将套接字的监听队列长度限制为20  
            socketWatch.Listen(20);

            //负责监听客户端的线程:创建一个监听线程
            Thread threadWatch = new Thread(watchconnecting);

            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            threadWatch.IsBackground = true;

            //启动线程
            threadWatch.Start();

            Console.WriteLine("开启监听。。。");
            Console.WriteLine("点击输入任意数据回车退出程序。。。");
            Console.ReadKey();
            Console.WriteLine("退出监听，并关闭程序。");
        }

        /// <summary>
        /// 监听客户端发来的请求  
        /// </summary>
        static void watchconnecting()
        {
            Socket connection = null;

            //持续不断监听客户端发来的请求
            while (true)
            {
                try
                {
                    connection = socketWatch.Accept();
                }
                catch(Exception ex)
                {
                    //提示套接字监听异常     
                    Console.WriteLine(ex.Message);
                    break;
                }

                //获取客户端的IP和端口号
                IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;

                //让客户显示"连接成功的"的信息  
                string sendmsg = "连接服务端成功！\r\n" + "本地IP:" + clientIP + "，本地端口" + clientPort.ToString();
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
                connection.Send(arrSendMsg);

                //新建线程，去接收客户端发来的信息
                Thread td = new Thread(AcceptMgs);
                td.IsBackground = true;
                td.Start(socketWatch);

            }

            private void AcceptMgs(object o)
            {

            }
        }
    }
}
