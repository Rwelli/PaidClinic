using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PaidClinic.Controls;
using PaidClinic.ViewModels;
using PaidClinic.Views;

namespace PaidClinic;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        RegisterExceptionHandlers();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
    private void RegisterExceptionHandlers()
    {
        Avalonia.Threading.Dispatcher.UIThread.UnhandledException += (sender, e) =>
        {
            e.Handled = true;
            ShowExceptionDialog("UI Thread Exception", e.Exception);
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            ShowExceptionDialog("Unhandled Exception", exception);
            
            if (e.IsTerminating)
            {
            }
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            e.SetObserved();
            ShowExceptionDialog("Task Exception", e.Exception);
        };
    }

    private void ShowExceptionDialog(string title, Exception exception)
    {
        if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
        {
            ShowDialog(title, exception);
        }
        else
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => 
                ShowDialog(title, exception));
        }
    }

    private async void ShowDialog(string title, Exception exception)
    {
        LogException(exception);

        var errorWindow = new ErrorReportWindow
        {
            DataContext = new ErrorReportWindowViewModel { ErrorText = exception.Message }
        };
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: not null } desktopLifetime)
        {
            await errorWindow.ShowDialog(desktopLifetime.MainWindow);
        }
    }

    private void LogException(Exception exception)
    {
        var logMessage = $"[{DateTime.Now}] {exception}\n{exception.StackTrace}\n\n";
        
        try
        {
            File.AppendAllText("error_log.txt", logMessage);
        }
        catch
        {
            // ignored
        }
    }
}