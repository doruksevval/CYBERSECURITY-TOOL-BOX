using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

// IMAGE
using System.Drawing;
using System.Drawing.Imaging;

// METADATA
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

// PDF
using iText.Kernel.Pdf;

namespace tool_cantam
{
    public partial class MetadataPage : UserControl
    {
        private string? _selectedFilePath;

        public MetadataPage()
        {
            InitializeComponent();
        }

        private void DropArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                DosyaHazirla(files[0]);
            }
        }

        private void DropArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                DosyaHazirla(dialog.FileName);
            }
        }

        private void DosyaHazirla(string path)
        {
            _selectedFilePath = path;
            TxtStatus.Text = "DOSYA SEÇİLDİ";
            TxtFileName.Text = Path.GetFileName(path);

            MetadataGoster(path);
        }

        private void BtnClean_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                MessageBox.Show("Önce dosya seç!");
                return;
            }

            try
            {
                ProgressClean.Visibility = Visibility.Visible;

                string eskiMetadata = TxtMetadata.Text;

                string ext = Path.GetExtension(_selectedFilePath).ToLower();
                string directory = Path.GetDirectoryName(_selectedFilePath)!;
                string name = Path.GetFileNameWithoutExtension(_selectedFilePath);
                string newPath = Path.Combine(directory, name + "_CLEANED" + ext);

                FileInfo before = new FileInfo(_selectedFilePath);

                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    TemizleResim(_selectedFilePath, newPath);
                }
                else if (ext == ".pdf")
                {
                    TemizlePdf(_selectedFilePath, newPath);
                }
                else
                {
                    File.Copy(_selectedFilePath, newPath, true);
                }

                FileInfo after = new FileInfo(newPath);

                ProgressClean.Visibility = Visibility.Collapsed;

                // Yeni metadata göster
                MetadataGoster(newPath);

                MessageBox.Show(
                    $"✔ Temizleme tamamlandı!\n\n" +
                    $"📉 Boyut:\nÖnce: {before.Length / 1024} KB\nSonra: {after.Length / 1024} KB\n\n" +
                    $"🧹 Silinen Metadata:\n{eskiMetadata}",
                    "Başarılı"
                );
            }
            catch (Exception ex)
            {
                ProgressClean.Visibility = Visibility.Collapsed;
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void TemizleResim(string input, string output)
        {
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(input))
            {
                foreach (var prop in img.PropertyItems)
                {
                    try
                    {
                        img.RemovePropertyItem(prop.Id);
                    }
                    catch { }
                }

                img.Save(output, ImageFormat.Jpeg);
            }
        }

        private void TemizlePdf(string input, string output)
        {
            using (PdfReader reader = new PdfReader(input))
            using (PdfWriter writer = new PdfWriter(output))
            using (PdfDocument pdf = new PdfDocument(reader, writer))
            {
                var info = pdf.GetDocumentInfo();

                info.SetAuthor("");
                info.SetTitle("");
                info.SetSubject("");
                info.SetKeywords("");
                info.SetCreator("");
                info.SetProducer("");
            }
        }

        private void MetadataGoster(string path)
        {
            try
            {
                string ext = Path.GetExtension(path).ToLower();
                StringBuilder sb = new StringBuilder();

                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    var directories = ImageMetadataReader.ReadMetadata(path);

                    var subIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    var ifd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                    var gps = directories.OfType<GpsDirectory>().FirstOrDefault();

                    if (subIfd != null)
                    {
                        sb.AppendLine(" Çekim Tarihi: " +
                            subIfd.GetDescription(ExifDirectoryBase.TagDateTimeOriginal));
                    }

                    if (ifd0 != null)
                    {
                        sb.AppendLine(" Kamera: " +
                            ifd0.GetDescription(ExifDirectoryBase.TagModel));
                    }

                    if (gps != null)
                    {
                        sb.AppendLine(" GPS: Var");
                    }

                    FileInfo fi = new FileInfo(path);
                    sb.AppendLine(" Boyut: " + (fi.Length / 1024) + " KB");
                }
                else if (ext == ".pdf")
                {
                    using (PdfReader reader = new PdfReader(path))
                    using (PdfDocument pdf = new PdfDocument(reader))
                    {
                        var info = pdf.GetDocumentInfo();

                        sb.AppendLine(" Author: " + info.GetAuthor());
                        sb.AppendLine(" Title: " + info.GetTitle());
                        sb.AppendLine(" Creator: " + info.GetCreator());
                    }
                }
                else
                {
                    sb.AppendLine("Bu dosya türü desteklenmiyor.");
                }

                TxtMetadata.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                TxtMetadata.Text = "Hata: " + ex.Message;
            }
        }
    }
}