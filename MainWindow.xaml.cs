using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPF_TabImageControl_Karvatyuk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TabItem> TabItems { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            TabItems = new List<TabItem>();

            DirectoryInfo dirInfo = new DirectoryInfo("Images");
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                CreateTabItem(file.Name, file.FullName);
            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {
            if (tcMain.SelectedItem != null)
            {
                bool last = true;
                string header = (string)((TabItem)tcMain.SelectedItem).Header;
                string path = $"Images\\{header}";
                TabItems.Remove((TabItem)tcMain.SelectedItem);
                tcMain.Items.Remove(tcMain.SelectedItem);
                foreach (TabItem item in TabItems)
                {
                    if ((string)item.Header == header)
                    {
                        last = false;
                    }
                }
                if (last == true)
                    File.Delete(path);
            }
        }

        private void CreateTabItem(string fileName, string filePath)
        {
            try
            {
                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32) });

                if (!File.Exists($"Images\\{fileName}"))
                    File.Copy(filePath, $"Images\\{fileName}", true);

                System.Drawing.Imaging.ImageFormat imageFormat;
                string ext = Path.GetExtension(filePath);
                switch (ext.ToLower())
                {
                    case "png":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    case "jpg":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                    case "jpeg":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                    case "bmp":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                    case "ico":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Icon;
                        break;
                    default:
                        imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                }

                System.Drawing.Image image = System.Drawing.Image.FromFile(filePath);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                MemoryStream ms = new MemoryStream();
                image.Save(ms, imageFormat);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();


                Image img = new Image() { Source = bi };
                image.Dispose();

                TabItem tmp = new TabItem() { Header = $"{fileName}" };
                Slider slider = new Slider() { Minimum = 0.1, Maximum = 2, Value = 1, IsSnapToTickEnabled = true, TickFrequency = 0.1, SmallChange = 0.1 };
                img.RenderTransformOrigin = new Point(0.5, 0.5);
                slider.ValueChanged += (s, e) =>
                {
                    img.Width = 300 * slider.Value;
                    img.Height = 300 * slider.Value;
                };
                Grid.SetRow(img, 0);
                Grid.SetRow(slider, 1);
                grid.Children.Add(img);
                grid.Children.Add(slider);
                tmp.Content = grid;
                tcMain.Items.Add(tmp);
                tcMain.SelectedIndex = tcMain.Items.Count - 1;
                TabItems.Add(tmp);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Select a picture";
            ofd.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp;*.ico|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png|" +
                "Bitmap Image (*.bmp)|*.bmp|" +
                "Icon (*.ico)|*.ico";
            if (ofd.ShowDialog() == true)
            {
                CreateTabItem(ofd.SafeFileName, ofd.FileName);
            }
        }
    }
}
