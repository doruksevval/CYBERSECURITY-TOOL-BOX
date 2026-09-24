using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace tool_cantam
{
    public partial class Base64Page : UserControl
    {
        public Base64Page()
        {
            InitializeComponent();
        }

        
        private void BtnEncode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = TxtInput.Text; 
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                string base64 = Convert.ToBase64String(bytes);
                TxtOutput.Text = base64; 
            }
            catch
            {
                TxtOutput.Text = "Hata: Encode edilemedi!";
            }
        }

       
        private void BtnDecode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = TxtInput.Text; 
                byte[] bytes = Convert.FromBase64String(input);
                string text = Encoding.UTF8.GetString(bytes);
                TxtOutput.Text = text;
            }
            catch
            {
                TxtOutput.Text = "Hata: Geçersiz Base64 formatı!";
            }
        }
    }
}