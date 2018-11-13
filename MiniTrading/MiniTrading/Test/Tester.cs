using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniTrading
{
    public static class Tester
    {
        public static RedisHelper m_redisConn = new RedisHelper();

        public static void TestRedis()
        {
            try
            {
                m_redisConn.StringSet("test1:test2", new Student() { ID = 1, Name = "Hanyu" });
                Sub();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void Sub()
        {
             m_redisConn.Subscribe("1", (channel, message) =>
            {
                Debug.WriteLine("接受到发布的内容为：" + RedisHelper.Deserialize<string>(message));
            });
            Debug.WriteLine("您订阅的通道为：<< " + "1" + " >> ! 请耐心等待消息的到来！！");
        }

    }

    [Serializable()]
    public class Student
    {
        public int ID
        { get; set; }

        public string Name
        { get; set; }
    }

}
