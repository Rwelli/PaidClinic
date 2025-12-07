using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
        private void RegisterExceptionHandlers()
    {
        // Обработка неперехваченных исключений в UI потоке
        Avalonia.Threading.Dispatcher.UIThread.UnhandledException += (sender, e) =>
        {
            e.Handled = true; // Помечаем как обработанное
            ShowExceptionDialog("UI Thread Exception", e.Exception);
        };

        // Обработка исключений в AppDomain
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            ShowExceptionDialog("Unhandled Exception", exception);
            
            // Для критических ошибок можно завершить приложение
            if (e.IsTerminating)
            {
                // Логирование или другие действия перед завершением
            }
        };

        // Для обработки Task исключений (если используете async/await)
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            e.SetObserved(); // Помечаем как обработанное
            ShowExceptionDialog("Task Exception", e.Exception);
        };
    }

    private void ShowExceptionDialog(string title, Exception exception)
    {
        // Используем диспетчер, если мы не в UI потоке
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
        // Можно также логировать ошибку
        LogException(exception);
    }

    private void LogException(Exception exception)
    {
        // Логирование в файл, базу данных или сервис
        var logMessage = $"[{DateTime.Now}] {exception}\n{exception.StackTrace}\n\n";
        
        // Пример записи в файл
        try
        {
            File.AppendAllText("error_log.txt", logMessage);
        }
        catch { /* Игнорируем ошибки логирования */ }
    }
}