using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.ConsolDemo.Consume
{
    class Program
    {
        private static readonly ConnectionFactory RabbitMqFactory = new ConnectionFactory
        {
            HostName = "www.c-code.xin",
            UserName = "test",
            Password = "pwd@rabbit.com",
            VirtualHost = "/"
        };
        const string ExchangeName = "hello.exchange";
        const string QueueName = "hello.queue";
        private static string _routingKey = "hello";
        static void Main(string[] args)
        {
            Console.Title = "消费者";
            //Console.WriteLine("请输入RoutingKey：");
            //var key = Console.ReadLine();
            //_routingKey = key;

            // 1.连接到RabbitmQ
            IConnection conn = RabbitMqFactory.CreateConnection();
            // 2.获取信道
            IModel channel = conn.CreateModel();
            // 3.声明交换器
            channel.ExchangeDeclare(exchange: ExchangeName, type: "direct", durable: true, autoDelete: false, arguments: null);
            // 4.申明队列
            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            // 5.通过键“hello”将队列和交换器绑定起来
            channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: _routingKey);

            Console.WriteLine("请选择消息获取方式；推:1,拉:2");
            var type = Console.ReadLine();

            //6.消费
            switch (type)
            {
                case "1":
                    
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (ch, ea) =>
                    {
                        var body = ea.Body;
                        Console.WriteLine();
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 收到RabbitMQ的消息:");
                        Console.WriteLine(System.Text.Encoding.UTF8.GetString(body));

                        //通过AMQP的basic.ack命令显示地向RabbitMQ发送一个确认，服务器会删除这条消息；
                        //或者在订阅队列的时候就将auto_ack参数设置为true，消费者一旦接收到消息后，RabbitMQ视为其确认了消息；
                        channel.BasicAck(ea.DeliveryTag, false); 
                    };
                    //命令订阅，信道将会被设置为接收模式；直到取消对队列的订阅为止
                    var consumerTag = channel.BasicConsume(QueueName, false, consumer); 


                    break;
                case "2":
                    while (true)
                    {
                        //向队列请求单条消息，
                        //这里要注意：不应该将BasicGet放在一个循环里面代替BasicConsume，这样会影响性能。
                        //BasicGet 会订阅->获取单条消息->取消订阅；
                        var basicGet = channel.BasicGet(QueueName, false);
                        if (basicGet==null)
                        {
                            Console.WriteLine("没有获取到消息！！！");
                            Console.ReadKey();
                            continue;
                        }
                        //通过AMQP的basic.ack命令显示地向RabbitMQ发送一个确认，服务器会删除这条消息；
                        //或者在订阅队列的时候就将auto_ack参数设置为true，消费者一旦接收到消息后，RabbitMQ视为其确认了消息；
                        channel.BasicAck(basicGet.DeliveryTag,false);  
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 收到RabbitMQ的消息:");
                        Console.WriteLine(System.Text.Encoding.UTF8.GetString(basicGet.Body));
                        Console.ReadKey();
                    }

            }
            Console.ReadKey();
        }

    }
}
