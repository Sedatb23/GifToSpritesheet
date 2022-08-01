using CommunityToolkit.Mvvm.DependencyInjection;
using Gif2Spritesheet.ViewModels;

namespace Gif2Spritesheet
{
    public class ViewModelLocator
    {
        public MainViewModel MainWindow => Ioc.Default.GetRequiredService<MainViewModel>();
    }
}
