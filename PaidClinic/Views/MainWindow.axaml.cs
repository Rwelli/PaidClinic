using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Material.Colors;
using Material.Styles.Themes;
using PaidClinic.Services;
using PaidClinic.ViewModels;

namespace PaidClinic.Views;

public partial class MainWindow : Window
{
    private readonly DoctorsViewModel _doctorsViewModel = new();
    private readonly PatientViewModel _patientViewModel = new();
    private readonly AppealViewModel _appealViewModel;
    
    private DoctorsPage? _doctorsPage;
    private PatientPage? _patientPage;
    private AppealPage? _appealPage;
    private ReportPage? _reportPage;
    
    public MainWindow()
    {
        InitializeComponent();
        _appealViewModel = new(_doctorsViewModel, _patientViewModel);
#if DEBUG
        if (true)
        {
            this.AttachDevTools();
        }
#endif
    }

    private void MenuItemDoctorsClick(object? sender, RoutedEventArgs e)
    {
        _doctorsPage ??= new DoctorsPage();
        _doctorsPage.DataContext = _doctorsViewModel; 
        MainControl.Content = _doctorsPage;
    }

    private void MenuItemPatientClick(object? sender, RoutedEventArgs e)
    {
        _patientPage ??= new PatientPage();
        _patientPage.DataContext = _patientViewModel; 
        MainControl.Content = _patientPage;
    }
    
    private void MenuItemAppealClick(object? sender, RoutedEventArgs e)
    {
        _appealPage ??= new AppealPage();
        _appealPage.DataContext = _appealViewModel;
        MainControl.Content = _appealPage;
    }

    private void MenuItemReportClick(object? sender, RoutedEventArgs e)
    {
        _reportPage = new ReportPage();
        _reportPage.DataContext = new ReportViewModel();
        MainControl.Content =  _reportPage;
    }

    private void MenuItemExitClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }

    private void ChangeThemeOnLight(object? sender, RoutedEventArgs e)
    {
        if (Application.Current == null) return;
        var materialTheme = Application.Current.LocateMaterialTheme<MaterialTheme>();
        var primaryColor = SwatchHelper.Lookup[MaterialColor.Blue];
        var secondaryColor = SwatchHelper.Lookup[MaterialColor.Green];
        materialTheme.CurrentTheme = Material.Styles.Themes.Theme.Create(Material.Styles.Themes.Theme.Light, primaryColor, secondaryColor); 
    }

    private void ChangeThemeOnNight(object? sender, RoutedEventArgs e)
    {
        if (Application.Current == null) return;
        var materialTheme = Application.Current.LocateMaterialTheme<MaterialTheme>();

        var primaryColor = SwatchHelper.Lookup[MaterialColor.Blue];
        var secondaryColor = SwatchHelper.Lookup[MaterialColor.Green];
        materialTheme.CurrentTheme = Material.Styles.Themes.Theme.Create(Material.Styles.Themes.Theme.Dark, primaryColor, secondaryColor);
    }
}