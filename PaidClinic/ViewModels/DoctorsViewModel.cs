using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using DialogHostAvalonia;
using PaidClinic.Models;
using PaidClinic.Services;
using ReactiveUI;

namespace PaidClinic.ViewModels;

public delegate void OnChangedModel();

public class DoctorsViewModel : ViewModelBase
{
    private readonly DoctorsDbService _service;
    private Doctor _doctor = new(0, "", "", "", "", "");
    private string _action = string.Empty;
    
    public OnChangedModel OnChangedModel { get; set; }
    
    public Doctor Doctor
    {
        get => _doctor;
        set => this.RaiseAndSetIfChanged(ref _doctor, value);
    }

    public string Action
    {
        get => _action;
        set => this.RaiseAndSetIfChanged(ref _action, value);
    }

    public ObservableCollection<Doctor> Doctors { get; } = [];
    
    public ReactiveCommand<Unit, Unit> LoadDoctorsCommand { get; }
    public ReactiveCommand<Unit, Unit> AddDoctorCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteDoctorCommand { get; }
    public ReactiveCommand<Unit, Unit> EditDoctorCommand { get; }

    public DoctorsViewModel()
    {
        _service = new DoctorsDbService();
        LoadDoctorsCommand = ReactiveCommand.CreateFromTask(LoadDoctorsAsync);
        AddDoctorCommand = ReactiveCommand.CreateFromTask(AddDoctorAsync);
        DeleteDoctorCommand = ReactiveCommand.CreateFromTask(DeleteDoctorAsync);
        EditDoctorCommand = ReactiveCommand.CreateFromTask(EditDoctorAsync);
        LoadDoctorsCommand.Execute();
    }

    private async Task EditDoctorAsync()
    {
        if (Doctor.IsEmpty()) return;
        await _service.UpdateDoctorAsync(Doctor);
        LoadDoctorsCommand.Execute();
        await DialogHost.Show(new TextBlock {Text = "Edit Doctor"});
    }

    private async Task DeleteDoctorAsync()
    {
        await _service.DeleteDoctor(Doctor);
        OnChangedModel.Invoke();
    }

    private async Task AddDoctorAsync()
    {
        if (Doctor.IsEmpty()) return;
        await _service.AddDoctorAsync(Doctor);
        
        LoadDoctorsCommand.Execute();
    }

    private async Task LoadDoctorsAsync()
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { Doctors.Clear(); });
        var doctors = await _service.GetDoctors();
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var doctor in doctors) Doctors.Add(doctor);
        });
    }

    public void RemoveDoctor(Doctor doctor)
    {
        Doctor = doctor;
        DeleteDoctorCommand.Execute();
        LoadDoctorsCommand.Execute();
    }
    
    public void ClearFirstName() => Doctor.FirstName = string.Empty;
    public void ClearLastName() => Doctor.LastName = string.Empty;
    public void ClearMiddleName() => Doctor.MiddleName = string.Empty;
    public void ClearSpecialty() => Doctor.Speciality = string.Empty;
    public void ClearCategory() => Doctor.Category = string.Empty;
}