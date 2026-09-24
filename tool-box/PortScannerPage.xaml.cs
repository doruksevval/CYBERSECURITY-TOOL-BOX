// ============================================
// PORT TARAYICI MODÜLÜ (PortScannerPage)
// ============================================
// Bu sayfa, belirtilen IP adresindeki cihazın açık portlarını tarar.
// Kullanıcı IP adresi ve port aralığı girdikten sonra tarama başlatır.

using System;                      // Temel sınıflar için (int, string vb.)
using System.Net.Sockets;          // TCP/IP bağlantıları için (TcpClient sınıfı)
using System.Threading.Tasks;      // Async/await ile çoklu işlem için
using System.Windows;              // MessageBox, MessageBoxButton vb. için
using System.Windows.Controls;     // UserControl, Button, ListBox vb. için

namespace tool_cantam               // Projenin ad alanı
{
    // PortScannerPage: Port tarama işlemlerini yapan kullanıcı kontrolü
    public partial class PortScannerPage : UserControl
    {
        // Constructor - Sayfa açıldığında çalışır
        public PortScannerPage()
        {
            InitializeComponent();  // XAML'deki buton/textbox/listbox vb. bağlar
        }

        // ============================================
        // "TARAMA BAŞLAT" BUTONUNA TIKLANINCA
        // ============================================
        // async = Bu metot içinde "bekleme" işlemi yapılabilir (await)
        // void = Geriye değer döndürmez
        private async void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            // 1. KULLANICIDAN GİRİLEN VERİLERİ AL
            string ip = TxtIP.Text;                    // IP adresi (örn: 192.168.1.1)
            string[] parts = TxtPortRange.Text.Split('-'); // Port aralığını ayır (örn: "20-80" → ["20","80"])

            // 2. GİRİLEN VERİLERİ KONTROL ET (Geçerli mi?)
            // parts.Length != 2 → Aralık "başlangıç-bitiş" formatında değilse
            // int.TryParse başarısızsa → Sayıya çevrilemiyorsa
            if (parts.Length != 2 || !int.TryParse(parts[0], out int baslangicPort) || !int.TryParse(parts[1], out int bitisPort))
            {
                MessageBox.Show("Port aralığını düzgün gir (Örn: 1-100)", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;  // Hata varsa burada dur, tarama başlatma
            }

            // 3. TARAMA ÖNCESİ HAZIRLIKLAR
            LstPorts.Items.Clear();                    // Listeyi temizle (önceki sonuçları sil)
            ScanProgress.Value = 0;                    // İlerleme çubuğunu sıfırla

            BtnScan.IsEnabled = false;                 // Tarama bitene kadar butonu devre dışı bırak
            BtnScan.Content = "TARANIYOR...";          // Butonun yazısını değiştir

            // 4. PORT TARAMA DÖNGÜSÜ
            // Başlangıç portundan bitiş portuna kadar TEK TEK kontrol et
            for (int port = baslangicPort; port <= bitisPort; port++)
            {
                // Zaman aşımı süresini slider'dan al (milisaniye cinsinden)
                int zamanAsimi = Convert.ToInt32(SldTimeout.Value);

                // Bağlantı denemesi için ayrı bir iş parçacığı başlat (UI donmasın diye)
                // Task.Run = Arka planda çalıştır
                bool acikMi = await Task.Run(() => BaglanmayiDene(ip, port, zamanAsimi));

                // Eğer port AÇIK ise listeye ekle
                if (acikMi)
                {
                    LstPorts.Items.Add($"[+] PORT {port} - AÇIK");
                }

                // İlerleme çubuğunu güncelle (yüzde olarak)
                // (şu anki port - başlangıç + 1) / (toplam port sayısı) * 100
                ScanProgress.Value = (double)(port - baslangicPort + 1) / (bitisPort - baslangicPort + 1) * 100;
            }

            // 5. TARAMA BİTTİKTEN SONRA TEMİZLİK
            BtnScan.IsEnabled = true;                  // Butonu tekrar aktif et
            BtnScan.Content = "TARAMA BAŞLAT";         // Buton yazısını eski haline getir
            MessageBox.Show("Tarama tamamlandı!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ============================================
        // BAĞLANTI DENEME FONKSİYONU
        // ============================================
        // Bu fonksiyon belirtilen IP ve porta bağlanmaya çalışır.
        // Bağlanabilirse true, bağlanamazsa false döner.
        private bool BaglanmayiDene(string ip, int port, int zamanAsimi)
        {
            try
            {
                // TcpClient = TCP bağlantıları için kullanılan sınıf
                using (TcpClient istemci = new TcpClient())
                {
                    // BeginConnect = Bağlantıyı ASENKRON olarak başlat (beklemeden devam et)
                    IAsyncResult sonuc = istemci.BeginConnect(ip, port, null, null);

                    // AsyncWaitHandle.WaitOne = Bağlantı için belirtilen süre kadar BEKLE
                    // Zaman aşımına uğrarsa false döner
                    bool basarili = sonuc.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(zamanAsimi));

                    // Bağlantı başarılıysa VE istemci hala bağlıysa
                    if (basarili && istemci.Connected)
                    {
                        istemci.EndConnect(sonuc);  // Bağlantıyı tamamla
                        return true;                // Port AÇIK
                    }
                    return false;                   // Port KAPALI (zaman aşımı)
                }
            }
            catch
            {
                // Herhangi bir hata olursa (örn: geçersiz IP) false döndür
                return false;
            }
        }
    }
}