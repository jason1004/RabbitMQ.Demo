﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;

namespace RabbitMQ.ConsolDemo.EasyNetQ
{
    class Program
    {
        static void Main(string[] args)
        {
            //c
            //http://mikehadlow.blogspot.sg/2014/02/easynetq-layered-api.html

            var bus = RabbitHutch.CreateBus("host=localhost:5672;" +
                                            "virtualHost=my_vhost;" +
                                            "username=admin;" +
                                            "password=admin");

            var msg = "";
            while (string.IsNullOrEmpty(msg))
            {
                //  msg = Console.ReadLine();
                msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                bus.Advanced.Publish(new Exchange("hello.exchange"), "hello",
                    false, new Message<string>(msg));
                msg =String.Empty;
            }
         
            Console.ReadKey();
        }



    }

    internal class MyMessage
    {
        public string Text { get; set; }
    }
}
