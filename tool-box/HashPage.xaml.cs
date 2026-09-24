using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace tool_cantam
{
    public partial class HashPage : UserControl
    {
        public HashPage()
        {
            InitializeComponent();
        }

        private async void BtnCompute_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtHashInput.Text))
            {
                MessageBox.Show("Önce veri gir! ");
                return;
            }

            HashProgress.Value = 0;
            StatusText.Text = "PROCESSING...";
            StatusText.Foreground = System.Windows.Media.Brushes.Orange;

            await Task.Delay(400); 
            HashProgress.Value = 60;

            string algo = (CmbAlgorithm.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "SHA-256";

            try
            {
                string result = "";
                if (algo.Contains("SHA-256")) result = GetHash(TxtHashInput.Text, SHA256.Create());
                else if (algo.Contains("MD5")) result = GetHash(TxtHashInput.Text, MD5.Create());
                else if (algo.Contains("SHA-1")) result = GetHash(TxtHashInput.Text, SHA1.Create());

                await Task.Delay(200);

                HashProgress.Value = 100;
                TxtHashResult.Text = result;
                Clipboard.SetText(result);

                StatusText.Text = "DONE & COPIED";
                StatusText.Foreground = System.Windows.Media.Brushes.SpringGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                HashProgress.Value = 0;
            }
        }

        private string GetHash(string input, HashAlgorithm hashAlgo)
        {
            byte[] data = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}