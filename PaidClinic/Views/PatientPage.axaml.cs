using Avalonia.Controls;
using PaidClinic.Models;
using PaidClinic.ViewModels;

namespace PaidClinic.Views;

public partial class PatientPage : UserControl
{
    public PatientPage()
    {
        InitializeComponent();
    }

    private void DataGridSelectedPatient(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is PatientViewModel vm && sender is DataGrid { SelectedItem: Patient patient })
        {
            vm.CurrentPatient = patient;
        }
    }
}