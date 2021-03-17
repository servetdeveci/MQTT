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
        IMqttServer _mqttServer;
        MqttServerOptionsBuilder _mqttServerOptionsBuilder;

        public Broker()
        {
            _mqttServerOptionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionValidator(c =>
                {
                    // Bağlanan Client ip adresi ClientId si konsol ekranından görmek için yazıyoruz. 
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Endpoint: {c.Endpoint}  ==> ClientId {c.ClientId}");
                    if (c.Username == "sizin_kullanici_adiniz" && c.Password == "sizin_sifreniz")
                        c.ReasonCode = MqttConnectReasonCode.Success; // Bağlanmaya çalışan kişiye username ve password dogru ise bağlanma izni verme
                    else
                        c.ReasonCode = MqttConnectReasonCode.NotAuthorized; // Bağlanmaya çalışan kişiye username ve password yanlış ise bağlanma izni verilmez
                })
                .WithApplicationMessageInterceptor(async context => // asenkron kod blogu içerecek ise async kullanılabilir yoksa kaldırılmalı
                {
                    Console.WriteLine($"Id: {context.ClientId} ==>  \ntopic: {context.ApplicationMessage.Topic} \nPayload==> {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}");
                    // await SendToApi(context); // Gelen-Giden Veri trafiğinin kontrolü  ve Gerekirse bir servise veri gönderme işlemi burdan yapılabilir.
                })
                .WithConnectionBacklog(1000) // aynı anda kaç bağlantının kuyrukta tutulacağı
                .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Parse("127.0.0.1")) // server ip adres yada bilgisayarın localhost u kullanılabilir
                .WithDefaultEndpointPort(1884); // bilgisayar yada server'dan bağlanan port bilgsi



        }

        public void Start()
        {
            _mqttServer = new MqttFactory().CreateMqttServer();
            _mqttServer.StartAsync(_mqttServerOptionsBuilder.Build()).Wait();

            // MQTT 
            Console.WriteLine($"Mqtt Broker Oluşturuldu: Host: {_mqttServer.Options.DefaultEndpointOptions.BoundInterNetworkAddress} Port: {_mqttServer.Options.DefaultEndpointOptions.Port}"); ;

            //start server
            Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait();

        }
        public void Stop()
        {
            _mqttServer.StopAsync().Wait();
        }
    }
}
