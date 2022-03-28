using Gif2Spritesheet.Commands;
using Gif2Spritesheet.Converters;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Advanced;

namespace Gif2Spritesheet.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public DelegateCommand<string> SelectFolderCommand { get; set; }
        public AsyncDelegateCommand ExportCommand { get; set; }
        
        private readonly IDialogService dialogService;

        private Dictionary<string, Image> gifs;

        private string currentFilename;

        public string CurrentFilename
        {
            get { return currentFilename; }
            set {
                currentFilename = value;
                OnPropertyChanged();

            }
        }


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

        private string numberGIFs;

        public string NumberGIFs
        {
            get => numberGIFs;
            set
            {
                numberGIFs = value;
                OnPropertyChanged();
            }
        }


        private byte[] displayImageInput;

        public byte[] DisplayImageInput
        {
            get { 
                return displayImageInput;
            }
            set { 
                displayImageInput = value;
                OnPropertyChanged();
            }
        }

        private byte[] displayImageOutput;

        public byte[] DisplayImageOutput
        {
            get => displayImageOutput;
            set
            {
                displayImageOutput = value;
                OnPropertyChanged();
            }
        }


        public bool IsReady => FileLoaded && 
            !string.IsNullOrEmpty(SourcePath) &&
            !string.IsNullOrEmpty(DestinationPath);

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

            SelectFolderCommand = new DelegateCommand<string>(SelectFolder);
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

                result.Dispose();

            }
        }


        private void SelectFolder(string direction)
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

            var count = 0;

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);

                if (!ext.Equals(".gif")) continue;

                gifs.Add(file, await Image.LoadAsync(file));
                count += 1;
            }

            if (gifs.Count > 0)
            {
                NumberGIFs = count.ToString();
                FileLoaded = true;
                await LoadFirstImage(gifs.First());
            }

        }

        private async Task LoadFirstImage(KeyValuePair<string, Image> img)
        {
            MemoryStream stream = new MemoryStream();
            await img.Value.SaveAsBmpAsync(stream);
            DisplayImageInput = stream.ToArray();
            CurrentFilename = img.Key;

            var streamOutput = new MemoryStream();
            Image result = await GifToSpritesheet.Convert(img.Value);
            await result.SaveAsBmpAsync(streamOutput);
            DisplayImageOutput = streamOutput.ToArray();
        }

    }
}
