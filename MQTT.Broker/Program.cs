using System;

namespace MQTT.Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            Broker broker = new Broker();
            broker.Start();
        }
    }
}
