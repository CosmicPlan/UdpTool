// 定义UDP端口
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            // 从JSON文件中读取配置
            string jsonConfig = System.IO.File.ReadAllText("config.json");
            UdpConfig config = JsonSerializer.Deserialize<UdpConfig>(jsonConfig);

            if (config != null)
            {
                // 创建UdpClient并绑定端口
                IPAddress iPAddress = IPAddress.Parse(config.Ip);
                UdpClient udpClient = new UdpClient(new IPEndPoint(iPAddress, config.Port));

                Console.WriteLine($"等待接收数据，端口: {config.Port}");

                DateTime lastReceiveTime = DateTime.MinValue;

                while (true)
                {
                    // 异步接收UDP数据
                    UdpReceiveResult result = await udpClient.ReceiveAsync();

                    // 获取接收到的字节数组
                    byte[] receivedData = result.Buffer;

                    // 将接收到的字节数组转换为字符串
                    string receivedMessage = Encoding.UTF8.GetString(receivedData);
                    // 获取当前时间
                    DateTime currentTime = DateTime.Now;

                    // 计算时间差
                    TimeSpan timeDifference = currentTime - lastReceiveTime;

                    // 显示接收到的数据和发送者的信息
                    Console.WriteLine($"接收到数据: {receivedMessage}");
                    Console.WriteLine($"来自: {result.RemoteEndPoint.Address}:{result.RemoteEndPoint.Port}");
                    Console.WriteLine($"时间差: {timeDifference.TotalSeconds} 秒");

                    // 更新上一个接收到数据的时间
                    lastReceiveTime = currentTime;
                }
            }
            else
            {
                Console.WriteLine("无法解析配置文件。");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"发生错误: {e.Message}");
        }
    }

    private class UdpConfig
    {
        public int Port { get; set; }

        public string Ip { get; set; }
    }
}