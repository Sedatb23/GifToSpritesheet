using Gif2Spritesheet.Commands;
using Gif2Spritesheet.Converters;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gif2Spritesheet.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public DelegateCommand<string> SelectFolderCommand { get; set; }
        public AsyncDelegateCommand ExportCommand { get; set; }
        
        private readonly IDialogService dialogService;

        private Dictionary<string, Image> gifs;

        private bool fileLoaded = false;

        public bool FileLoaded
        {
            get { return fileLoaded; }
            set
            {
                fileLoaded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReady));
            }
        }

        public bool IsReady => FileLoaded && 
            SourcePath != string.Empty && 
            DestinationPath != string.Empty;

        private string sourcePath;

        public string SourcePath
        {
            get => sourcePath;
            set {
                sourcePath = value;
                OnPropertyChanged();
                _ = Task.Run(() => LoadFiles(sourcePath));
            }
        }

        private string destinationPath;

        public string DestinationPath
        {
            get { return destinationPath; }
            set { 
                destinationPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReady));
            }
        }


        public MainViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            SelectFolderCommand = new DelegateCommand<string>(SelectInputFolder);
            ExportCommand = new AsyncDelegateCommand(Export);
        }

        private async Task Export()
        {
            string ext = ".png";

            foreach (var item in gifs)
            {
                var fn = Path.GetFileName(item.Key);
                fn = Path.ChangeExtension(fn, ext);
                fn = Path.Combine(DestinationPath, fn);

                Image result = await GifToSpritesheet.Convert(item.Value);

                await result.SaveAsPngAsync(fn);

            }
        }


        private void SelectInputFolder(string direction)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var settings = new FolderBrowserDialogSettings
            {
                Description = "Select a folder"
            };


            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            if (success == true)
            {
                if (direction == "in")
                    SourcePath = settings.SelectedPath;
                else
                    DestinationPath = settings.SelectedPath;
            }

#pragma warning restore CA1416 // Validate platform compatibility
        }

        private async Task LoadFiles(string folder)
        {
            gifs = new Dictionary<string, Image>();
            var files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);

                if (!ext.Equals(".gif")) continue;

                gifs.Add(file, await Image.LoadAsync(file));
            }

            //var result = await GifToSpritesheet.Convert(gifs.First().Value);

            //result.SaveAsPng("temp.png");
            FileLoaded = true;
        }

    }
}
