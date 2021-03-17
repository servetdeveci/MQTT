using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT.Publisher
{

    public class Publisher
    {
        private IMqttClient _client;
        private IMqttClientOptions _options;

        public Publisher()
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient(); // mqttFactory kullanarak bir factory oluşturuyorum. 

            //configure options
            _options = new MqttClientOptionsBuilder()
                .WithClientId("Sizin_Publisher_yada_IoT_Numaraniz") // bağlanan IoT'nin benzersiz numarası.
                .WithTcpServer("127.0.0.1", 1884) // bağlanacağı port ve ip adresi (sunucucu yada bilgisayar) biz local çalıştığımız için bu şekilde local ip girdik
                .WithCredentials("sizin_kullanici_adiniz", "sizin_sifreniz")
                .WithCleanSession()
                .Build();

            //handlers
            _client.UseConnectedHandler(e => // broker'a baglandığında çalışacak kod
            {
                Console.WriteLine("MQTT Broker'a başarılı bir şekilde bağlanıldı...");
            });
            _client.UseDisconnectedHandler(e => // broker'dan bağlantısı koptuğunda çalışacak kod
            {
                Console.WriteLine("MQTT Broker'dan başarılı bir şekilde ayrıldı...");
            });
            _client.UseApplicationMessageReceivedHandler(e => // aşağıdaki konfigurasyon ile Topic ve Veri alanı olan Payload doldurulup client ile gönderilir
            {
                // publisher aynı zaman subscriber olabildiği için onunda veri alma fonksiyonunu aşağıdaki şekilde oluşturuyoruz. 
                // ancak biz bu projeyi sadece Publisher olarak kullandığımız için aşağıdaki kod blogunu kapattım


                ////string topic = e.ApplicationMessage.Topic; // baglanılan konu (topic)
                ////if (string.IsNullOrWhiteSpace(topic) == false)
                ////{
                ////    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload); // Gelen veri veri
                ////    Console.WriteLine($"Topic: {topic}. Gelen Mesaj: {payload}");
                ////}

            });


            //connect
            _client.ConnectAsync(_options).Wait(); // Broker'a bağlanmak için asenkron olarak bağlantı başlatıyor.
            YayinYapmayaBasla(); // yayın yapmayı simule etmek için aşağıdaki şekilde bir while döngüsü ile yayın kuyruğu oluşturuyorum. 
            Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait(); 
        }
        public void YayinYapmayaBasla()
        {

            while (true)
            {
                Console.Write("YAYMAK İSTEDİĞİNİZ MESAJI YAZIN: "); // gönderilecek olan veriyi kullanıcıdan almak için mesaj 
                var counter = Console.ReadLine(); // kullanıcıdan mesaj alınıyor..
                var testMessage = new MqttApplicationMessageBuilder() // veri göndermek için bir yapı kuruyoruz. 
                    .WithTopic("Konu_1") // belirtilen bir konu ile ilgili bir mesaj yayıyoruz. İlerde Subscriber lar bu konu ile veri alacaklar
                    .WithPayload($"Payload: {counter}") // konunun dışında asıl alınacak olan veri. bu bir json yada xml verisi olabilir
                    .WithExactlyOnceQoS() // daha önce belirttiğimiz gibi verinin iletilmesinin doğruluğunun kontrolü (Quality of Service)
                    .WithRetainFlag(false) // gönderilen mesajın kalıcı olup olmama durumu. flag false olur ise kalıcı olmaz ve o anda bu konuya bağlı bir subscriber yok ise veri ona iletilmez silinir gider
                    .Build();


                if (_client.IsConnected) // Publisher halen broker'a bağlımı onu kontrol ediyoruz. 
                {
                    Console.WriteLine($"Yayın tarihi: {DateTime.UtcNow}, Topic: {testMessage.Topic}, Payload: {System.Text.Encoding.ASCII.GetString(testMessage.Payload)}"); // gönderilen verinin ekrana yazılması
                    _client.PublishAsync(testMessage); // mesajı broker'a gönderilmesi
                }
            }
        }

        void BaglantiyiKes()
        {
            _client.DisconnectAsync().Wait(); // broker ile bağlantının kesilmesi
            Console.WriteLine("Broker ile bağlantı kesildi...");

        }
    }
}
