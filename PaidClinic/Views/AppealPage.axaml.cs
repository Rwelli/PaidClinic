using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PaidClinic.Models;
using PaidClinic.ViewModels;

namespace PaidClinic.Views;

public partial class AppealPage : UserControl
{
    public AppealPage()
    {
        InitializeComponent();
    }

    private void DataGridSelectedAppeal(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is AppealViewModel vm && sender is DataGrid { SelectedItem: Appeal appeal })
        {
            vm.CurrentAppeal = appeal;
        }
    }
}