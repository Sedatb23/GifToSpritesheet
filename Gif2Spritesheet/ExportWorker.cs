using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Gif2Spritesheet
{
    class ExportWorker
    {

        private readonly BackgroundWorker _worker;
        private readonly MainWindow _window;

        public ExportWorker(MainWindow window)
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
            _worker.RunWorkerAsync();
        }
        public static void Run(MainWindow window)
        {
            new ExportWorker(window);
        }


        /// <summary>
        /// Do worker logic.
        /// Save all gifs as png spritesheets...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int columns = 0;
            _window.btn_run.Dispatcher.Invoke(new Action(() =>
            {
                columns = (int) _window.slider.Value;
                _window.btn_run.IsEnabled = false;
                _window.btn_import.IsEnabled = false;
                _window.btn_export.IsEnabled = false; 
            }));

            int total = _window.Gifs.Count;
            int completed = 0;
            //lbl_run_progress
            foreach (KeyValuePair<string, System.Drawing.Image> pair in _window.Gifs)
            {
                Bitmap bitmap = _window.GifToSpritesheet(pair.Value, columns);
                string file = Path.GetFileNameWithoutExtension(pair.Key) + ".png";
                _window.Dispatcher.Invoke(new Action(() =>
                {
                    bitmap.Save(Path.Combine(_window.txt_export.Text, file), ImageFormat.Png);
                }));


                int progress = (int)Math.Ceiling((double)100 / total * completed++);
                _worker.ReportProgress(progress); 
            } 
        }


        /// <summary>
        /// Show convert progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _window.txt_import.Dispatcher.Invoke(new Action(() => { _window.lbl_run_progress.Content = $"{e.ProgressPercentage}%"; }));
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
                _window.btn_run.IsEnabled = true; 
                _window.btn_import.IsEnabled = true;
                _window.btn_export.IsEnabled = true; 
                _window.lbl_run_progress.Content = "Converted"; 
            }));
        }
    }
}
