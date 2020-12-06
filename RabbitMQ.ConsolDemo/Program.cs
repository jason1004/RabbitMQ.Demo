using System;
using RabbitMQ.Client;

namespace RabbitMQ.ConsolDemo
{
    class Program
    {
        private static readonly ConnectionFactory RabbitMqFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin",
            VirtualHost = "my_vhost", //默认值： /
            Port = 5672   //默认值：5672
        };
        const string ExchangeName = "hello.exchange";
        const string QueueName = "hello.queue";
        const string RoutingKey = "hello";
        static void Main(string[] args)
        {
            Console.Title = "产生者";
            //1.连接到RabbitMQ
            IConnection conn = RabbitMqFactory.CreateConnection();
            //2.获取信道
            IModel channel = conn.CreateModel();
            //3.声明交换器
            channel.ExchangeDeclare(exchange: ExchangeName, type: "direct", durable: true, autoDelete: false, arguments: null);

            // 4.申明队列
            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            // 5.通过键“hello”将队列和交换器绑定起来
            channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);
            var str = string.Empty;
            while (string.IsNullOrEmpty(str))
            {
                Console.WriteLine("请输入需要发送的消息：");
                str = Console.ReadLine();

                //4.创建消息内容
                byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " : " + str);
                //5.发布消息到指定交换器
                channel.BasicPublish(ExchangeName,
                    RoutingKey,
                    null,    // new BasicProperties { Expiration = "3600000"} //可以指定失效时间
                    messageBodyBytes);

                str = string.Empty;
            }

            //关闭信道
            channel.Close(); //channel.Dispose(); 也可以
            //关闭连接
            conn.Close();  //conn.Dispose(); 也可以
        }

        

    }
}
