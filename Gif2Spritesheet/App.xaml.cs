using CommunityToolkit.Mvvm.DependencyInjection;
using Gif2Spritesheet.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq; 
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
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton<IDialogService, DialogService>()
                    .AddTransient<MainViewModel>()
                    .BuildServiceProvider());                    
        }
    }
}
