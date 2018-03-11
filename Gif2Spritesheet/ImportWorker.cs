using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Gif2Spritesheet
{
    class ImportWorker
    {
        private readonly BackgroundWorker _worker;
        private readonly MainWindow _window;

        public ImportWorker(MainWindow window, string folder)
        {
            _window = window;
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            _worker.RunWorkerAsync(folder);
        }
        public static void Run(MainWindow window, string folder)
        {
            new ImportWorker(window, folder);
        }  

        /// <summary>
        /// Do worker logic.
        /// Import all gifs...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string folder = e.Argument.ToString();
            _window.btn_import.Dispatcher.Invoke(new Action(() => { _window.btn_import.IsEnabled = false; }));

            Dictionary<string, System.Drawing.Image> gifs = new Dictionary<string, System.Drawing.Image>();
            string[] files = Directory.GetFiles(folder);
            string firstGif = string.Empty;
            for (int i = 0; i < files.Length; i++)
            {
                string extension = System.IO.Path.GetExtension(files[i]);
                if (extension.Equals(".gif") == false)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(firstGif))
                {
                    firstGif = files[i];
                }
                gifs.Add(files[i], System.Drawing.Image.FromFile(files[i]));

                int progress = (int)Math.Ceiling((double)100 / files.Length * i);
                _worker.ReportProgress(progress);
            }
            if (gifs.Count > 0)
            {
                _window.sprite.Dispatcher.Invoke(new Action(() =>
                {
                    _window.Gifs = gifs.ToDictionary(entry => entry.Key, entry => entry.Value);
                    Bitmap bitmap = _window.GifToSpritesheet(_window.Gifs.Values.First(), (int)_window.slider.Value);
                    _window.sprite.Source = _window.ToWpfBitmap(bitmap);
                }));
            }
            e.Result = folder;
        }


        /// <summary>
        /// Show file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _window.txt_import.Dispatcher.Invoke(new Action(() => { _window.txt_import.Text = $"Loading images, {e.ProgressPercentage}% complete..."; }));
        } 

        /// <summary>
        /// Handle logic on background worker complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            _window.txt_import.Dispatcher.Invoke(new Action(() =>
            {
                _window.btn_import.IsEnabled = true;
                _window.txt_import.Text = e.Result.ToString();
                _window.CheckRunStatus();
            }));
        }
    }
}
