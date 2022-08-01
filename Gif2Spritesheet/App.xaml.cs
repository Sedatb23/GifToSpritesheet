using CommunityToolkit.Mvvm.DependencyInjection;
using Gif2Spritesheet.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using System.Windows;

namespace Gif2Spritesheet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Src : https://github.com/FantasticFiasco/mvvm-dialogs
            // Inversion of control
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton<IDialogService, DialogService>()
                    .AddTransient<MainViewModel>()
                    .BuildServiceProvider());                    
        }
    }
}
