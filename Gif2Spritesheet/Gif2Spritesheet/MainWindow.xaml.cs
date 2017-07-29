using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Gif2Spritesheet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        private Dictionary<string, System.Drawing.Image> _gifs;
        private BackgroundWorker _worker;

        public Dictionary<string, System.Drawing.Image> Gifs
        {
            get { return _gifs; }
            set { _gifs = value; }
        } 

        public MainWindow()
        {
            InitializeComponent();

            // Slider settings
            slider.IsSnapToTickEnabled = true;
            slider.TickFrequency = 1;
            slider.Minimum = 1;
            slider.Maximum = 12;
        }
         

        /// <summary>
        /// Opens the folder selection window
        /// </summary>
        /// <returns>Selected folder path</returns>
        private string HandleFolderSelection()
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog
            {
                Title = "Select folder",
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName; 
            }

            return string.Empty;
        }

        /// <summary>
        /// Handle logic for import button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_import_Click(object sender, RoutedEventArgs e)
        {
            string folder = this.HandleFolderSelection();
            Console.WriteLine($"Loading images, {folder}% complete...");
            if (string.IsNullOrEmpty(folder) == false && Directory.Exists(folder))
            {
                ImportWorker.Run(this, folder);
            }
        }


        /// <summary>
        /// Handle logic for export button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            string folder = this.HandleFolderSelection();
            if (string.IsNullOrEmpty(folder) == false && Directory.Exists(folder))
            { 
                txt_export.Text = folder;
                CheckRunStatus();
            }
        }

        public void CheckRunStatus()
        {
            btn_run.IsEnabled = false;

            if (Directory.Exists(txt_import.Text) && Directory.Exists(txt_export.Text))
            {
                btn_run.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handle logic for export button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_run_Click(object sender, RoutedEventArgs e)
        {
            ExportWorker.Run(this);
        }


        /// <summary>
        /// Convert an Image to a spritesheet
        /// </summary>
        /// <param name="gif">The original gif file</param>
        /// <param name="columns">How many columns to use</param>
        /// <returns></returns>
        public Bitmap GifToSpritesheet(System.Drawing.Image gif, int columns = 1)
        { 
            FrameDimension frameSize = new FrameDimension(gif.FrameDimensionsList[0]);
            System.Drawing.Size imageSize = new System.Drawing.Size(gif.Size.Width, gif.Size.Height);
            int frames = gif.GetFrameCount(frameSize);
            int rows = (int)Math.Ceiling((double)frames / columns);
            Bitmap bitmap = new Bitmap(columns * imageSize.Width, rows * imageSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            // In case of adding a background color
//            Brush brush = new SolidBrush(somecolor);
//            g.FillRectangle(brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            for (int i = 0; i < frames; i++)
            {
                gif.SelectActiveFrame(frameSize, i);
                g.DrawImage(gif, i % columns * imageSize.Width, i / columns * imageSize.Height);
            }
            g.Dispose();

            return bitmap;
        }


        /// <summary>
        /// Convert an Bitmap to BitmapSource in order to use it as Image Source
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public BitmapSource ToWpfBitmap(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        /// <summary>
        /// Handle slider value change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        { 
            lbl_columns_value.Content = e.NewValue.ToString();
            if (_gifs?.Count > 0)
            {
                Bitmap bitmap = GifToSpritesheet(_gifs.Values.First(), (int)slider.Value);
                sprite.Source = ToWpfBitmap(bitmap);
            }
        }
    }
}
