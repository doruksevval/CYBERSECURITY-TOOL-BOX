using System;
using System.Text;      // StringBuilder için (metin birleştirme)
using System.Linq;      // Karakterleri karıştırmak için OrderBy
using System.Windows;   // Clipboard (pano) için
using System.Windows.Controls;  // UserControl, TextBox, Button vb. için
using System.Windows.Media;     // Renk işlemleri için

namespace tool_cantam
{
    // Bu sınıf, PasswordPage adlı ekranın arka plan kodudur
    public partial class PasswordPage : UserControl
    {
        // Rastgele sayı üretici (şifre oluştururken kullanılacak)
        private Random rnd = new Random();

        // Constructor - sayfa açıldığında çalışır
        public PasswordPage()
        {
            InitializeComponent();  // XAML'deki butonları, textleri buraya bağlar
        }

        // "GENERATE & COPY" butonuna tıklandığında çalışır
        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Yeni bir şifre oluştur
            string sifre = SifreOlustur();

            // Oluşan şifreyi ekrandaki TextBox'a yaz
            TxtGenerated.Text = sifre;

            // Şifreyi panoya kopyala (Ctrl+V ile yapıştırılabilir)
            Clipboard.SetText(sifre);

            // Durum yazısını güncelle
            StatusText.Text = "STATUS: GENERATED & COPIED!";
            // Yazı rengini yeşil yap
            StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF9C"));
        }

        // *** ŞİFRE OLUŞTURMA FONKSİYONU ***
        // Bu fonksiyon rastgele bir şifre üretir
        string SifreOlustur()
        {
            // Kullanılacak karakter havuzları
            string kucuk = "abcdefghijklmnopqrstuvwxyz";  // küçük harfler
            string buyuk = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";  // büyük harfler
            string sayi = "0123456789";                   // rakamlar
            string ozel = "!@#$%^&*()";                   // özel karakterler

            // StringBuilder = metinleri hızlıca birleştirmek için
            StringBuilder sifre = new StringBuilder();

            // HER TÜRDEN EN AZ 1 KARAKTER EKLE (garanti olsun diye)
            sifre.Append(kucuk[rnd.Next(kucuk.Length)]);  // rastgele küçük harf
            sifre.Append(buyuk[rnd.Next(buyuk.Length)]);  // rastgele büyük harf
            sifre.Append(sayi[rnd.Next(sayi.Length)]);    // rastgele rakam
            sifre.Append(ozel[rnd.Next(ozel.Length)]);    // rastgele özel karakter

            // Tüm karakterleri birleştir (kalan rastgele karakterler için)
            string tum = kucuk + buyuk + sayi + ozel;

            // 8 tane daha rastgele karakter ekle (toplam 12 karakter olacak)
            for (int i = 0; i < 8; i++)
            {
                sifre.Append(tum[rnd.Next(tum.Length)]);
            }

            // Şifreyi karıştır (sırayı rastgele yap)
            // OrderBy(x => rnd.Next()) = her karaktere rastgele bir sayı ver, ona göre sırala
            return new string(sifre.ToString().OrderBy(x => rnd.Next()).ToArray());
        }

        // *** ŞİFRE PUANLAMA FONKSİYONU ***
        // Bu fonksiyon girilen şifrenin gücünü hesaplar (0-100 arası)
        int SifrePuanla(string sifre)
        {
            int puan = 0;  // Başlangıç puanı 0

            // Kontrol değişkenleri (başlangıçta hepsi false)
            bool kucuk = false, buyuk = false, sayi = false, ozelVar = false;
            string ozelKarakterler = "!@#$%^&*()";

            // Şifredeki HER KARAKTER için kontrol et
            foreach (char c in sifre)
            {
                if (char.IsLower(c)) kucuk = true;      // küçük harf varsa true yap
                if (char.IsUpper(c)) buyuk = true;      // büyük harf varsa true yap
                if (char.IsDigit(c)) sayi = true;       // rakam varsa true yap
                if (ozelKarakterler.Contains(c)) ozelVar = true;  // özel karakter varsa true yap
            }

            // PUANLARI EKLE (her sağlanan özellik için puan ver)
            if (kucuk) puan += 15;   // küçük harf var mı? +15 puan
            if (buyuk) puan += 20;   // büyük harf var mı? +20 puan
            if (sayi) puan += 20;    // rakam var mı? +20 puan
            if (ozelVar) puan += 25; // özel karakter var mı? +25 puan

            // Uzunluk kontrolü
            if (sifre.Length >= 8) puan += 10;   // 8 karakter veya daha uzunsa +10
            if (sifre.Length >= 12) puan += 10;  // 12 karakter veya daha uzunsa +10

            return puan;  // Toplam puanı geri gönder (en fazla 100 olabilir)
        }

        // *** ŞİFRE ANALİZİ - Kullanıcı şifre yazarken her tuşta çalışır ***
        private void TxtTest_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Kullanıcının yazdığı şifreyi al
            string sifre = TxtTest.Text;

            // EĞER ŞİFRE BOŞSA (hiçbir şey yazılmamışsa)
            if (string.IsNullOrWhiteSpace(sifre))
            {
                StrengthBar.Value = 0;           // Çubuğu sıfırla
                StrengthBar.Maximum = 100;       // Çubuğun max değeri 100
                ScoreText.Text = "0";            // Skoru 0 yaz
                StatusText.Text = "STATUS: WAITING...";  // Bekleme mesajı
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")); // Turuncu
                StrengthBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")); // Koyu gri
                return;  // Burada dur, aşağıdaki kodları çalıştırma
            }

            // Şifre doluysa, puanını hesapla
            int puan = SifrePuanla(sifre);

            // ProgressBar'ı (yeşil çubuk) puana göre ayarla
            StrengthBar.Value = puan;
            StrengthBar.Maximum = 100;

            // Skor yazısını güncelle
            ScoreText.Text = puan.ToString();

            // PUANA GÖRE DURUM MESAJI VE RENKLERİ AYARLA
            if (puan >= 90)
            {
                StatusText.Text = "Çok güçlü şifre ";
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF9C")); // Yeşil
                StrengthBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF9C")); // Yeşil
            }
            else if (puan >= 70)
            {
                StatusText.Text = "Güçlü şifre ";
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88FF00")); // Açık yeşil
                StrengthBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88FF00")); // Açık yeşil
            }
            else if (puan >= 50)
            {
                StatusText.Text = "Orta seviye ";
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")); // Turuncu
                StrengthBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")); // Turuncu
            }
            else
            {
                StatusText.Text = "Zayıf ";
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4444")); // Kırmızı
                StrengthBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4444")); // Kırmızı
            }
        }

        private void TxtGenerated_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}