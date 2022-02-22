namespace Gif2Spritesheet.Services
{
    public interface IFileDialogService : IDialogService
    {
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }
        public string Filename { get; set; }
    }
}
