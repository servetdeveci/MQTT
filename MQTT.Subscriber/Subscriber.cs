using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT.Subscriber
{

    public class Subscriber
    {
        private IMqttClient _client;
        private IMqttClientOptions _options;

        public Subscriber()
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient(); // mqttFactory kullanarak bir factory oluşturuyorum. 

            //configure options
            _options = new MqttClientOptionsBuilder()
                .WithClientId("Sizin_Subscriber_yada_IoT_Numaraniz") // bağlanan IoT'nin benzersiz numarası.
                .WithTcpServer("127.0.0.1", 1884) // bağlanacağı port ve ip adresi (sunucucu yada bilgisayar) biz local çalıştığımız için bu şekilde local ip girdik
                .WithCredentials("sizin_kullanici_adiniz", "sizin_sifreniz")
                .WithCleanSession()
                .Build();

            //handlers
            _client.UseConnectedHandler(e => // broker'a baglandığında çalışacak kod
            {
                Console.WriteLine("MQTT Broker'a başarılı bir şekilde bağlanıldı...");
                //Subscribe to topic
                _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("Konu_1").Build()).Wait();
                _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("Konu_2").Build()).Wait();
            });
            _client.UseDisconnectedHandler(e => // broker'dan bağlantısı koptuğunda çalışacak kod
            {
                Console.WriteLine("MQTT Broker'dan başarılı bir şekilde ayrıldı...");
            });
            _client.UseApplicationMessageReceivedHandler(e => // aşağıdaki konfigurasyon ile Topic ve Veri alanı olan Payload doldurulup client ile gönderilir
            {
                Console.WriteLine("### GELEN MESAJ ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

        }
        public void Baglan()
        {
            _client.ConnectAsync(_options).Wait(); // Broker'a bağlanmak için asenkron olarak bağlantı başlatıyor.
            Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait();
        }
        public void BaglantiyiKes()
        {
            _client.DisconnectAsync().Wait(); // broker ile bağlantının kesilmesi
            Console.WriteLine("Broker ile bağlantı kesildi...");

        }
    }
}

