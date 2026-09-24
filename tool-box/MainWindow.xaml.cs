// ============================================
// TOOL BOX - ANA PENCERE ARKA PLAN KODU
// ============================================
// Bu dosya MainWindow.xaml'in arkasındaki C# kodudur.
// Sol menüdeki butonlara tıklandığında sağ tarafta hangi sayfanın gösterileceğini belirler.

using System.Windows;  // Window, MessageBox gibi WPF temel sınıfları için gerekli

namespace tool_cantam  // Projenin namespace'i (ad alanı)
{
    // MainWindow sınıfı: Ana pencereyi temsil eder
    // "partial" = Bu sınıfın bir kısmı başka dosyada (MainWindow.g.cs) otomatik oluşturulur
    public partial class MainWindow : Window  // Window sınıfından türetilmiş
    {
        // Constructor (Yapıcı Metot) - Pencere ilk açıldığında çalışır
        public MainWindow()
        {
            // InitializeComponent(): XAML dosyasındaki tüm kontrolleri (butonlar, textboxlar vb.)
            // C# tarafına bağlar. Bu satır OLMADAN butonlara erişemezsin!
            InitializeComponent();

            // ============================================
            // BUTON TIKLAMA OLAYLARI (Event Handler)
            // ============================================
            // Her butona tıklandığında, sağ taraftaki MainContentFrame alanına
            // ilgili sayfayı (UserControl) yükler.
            // 
            // "+=" operatörü = Olaya yeni bir davranış ekle
            // "(s, e) =>" ifadesi = Lambda ile kısa fonksiyon yazma
            // MainContentFrame.Content = ... = Sağ alanın içeriğini değiştir
            // ============================================

            // Password Master Butonu
            // Tıklandığında PasswordPage adlı kullanıcı kontrolünü göster
            BtnPassword.Click += (s, e) => MainContentFrame.Content = new PasswordPage();

            // Base64 Tool Butonu
            // Tıklandığında Base64Page adlı kullanıcı kontrolünü göster
            BtnBase64.Click += (s, e) => MainContentFrame.Content = new Base64Page();

            // Metadata Cleaner Butonu
            // Tıklandığında MetadataPage adlı kullanıcı kontrolünü göster
            BtnMetadata.Click += (s, e) => MainContentFrame.Content = new MetadataPage();

            // Hash Generator Butonu
            // Tıklandığında HashPage adlı kullanıcı kontrolünü göster
            BtnHash.Click += (s, e) => MainContentFrame.Content = new HashPage();

            // Port Scanner Butonu
            // Tıklandığında PortScannerPage adlı kullanıcı kontrolünü göster
            BtnPort.Click += (s, e) => MainContentFrame.Content = new PortScannerPage();
        }
    }
}