using Avalonia.Controls;
using Avalonia.Interactivity;
using PaidClinic.Models;
using PaidClinic.ViewModels;

namespace PaidClinic.Views;

public partial class DoctorsPage : UserControl
{
    public DoctorsPage()
    {
        InitializeComponent();
    }

    private void DataGridSelectedDoctor(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is DoctorsViewModel vm && sender is DataGrid { SelectedItem: Doctor doctor })
        {
            vm.Doctor = doctor;
        }
    }

    private void RemoveDoctorFromDataGrid(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DoctorsViewModel vm && this.FindControl<DataGrid>("DataGridDoctors") is { SelectedItem: Doctor doctor })
        {
            vm.RemoveDoctor(doctor);
        }
    }
}