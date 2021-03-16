using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT.Broker
{
    public class Broker
    {
     

        public void Start()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionValidator(c =>
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Endpoint: {c.Endpoint}  ==> ClientId {c.ClientId} username: {c.Username}  ==> password {c.Password}");
                    c.ReasonCode = MqttConnectReasonCode.Success;

                    //if (c.Username == "atlas" && c.Password == "atlas")
                    //    c.ReasonCode = MqttConnectReasonCode.Success;
                    //else
                    //    c.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                })
                .WithApplicationMessageInterceptor(async context =>
                {
                    // await SendToApi(context); // apiye gönderme 
                })
                .WithConnectionBacklog(100)
                .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Parse("192.168.1.101"))
                .WithDefaultEndpointPort(1884);

            //start server
            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build()).Wait();

            Console.WriteLine($"Mqtt Broker çalışıyor: Host: {mqttServer.Options.DefaultEndpointOptions.BoundInterNetworkAddress} Port: {mqttServer.Options.DefaultEndpointOptions.Port}");

            Console.ReadLine();
            Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait();
            mqttServer.StopAsync().Wait();
        }
    }
}
