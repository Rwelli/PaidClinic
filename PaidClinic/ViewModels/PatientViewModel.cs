using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using DialogHostAvalonia;
using PaidClinic.Models;
using PaidClinic.Services;
using ReactiveUI;

namespace PaidClinic.ViewModels;

public class PatientViewModel : ViewModelBase
{
    private Patient _patient = new();
    private readonly PatientDbService _service = new();
    
    public OnChangedModel OnChangedModel { get; set; }

    public Patient CurrentPatient
    {
        get => _patient;
        set => this.RaiseAndSetIfChanged(ref _patient, value);
    }
    
    public ObservableCollection<Patient> Patients { get; } = [];
    
    public ReactiveCommand<Unit, Unit> LoadPatientCommand { get; }
    public ReactiveCommand<Unit, Unit> AddPatientCommand { get; }
    public ReactiveCommand<Unit, Unit> EditPatientCommand { get; }
    public ReactiveCommand<Patient?, Unit> DeletePatientCommand { get; }

    public PatientViewModel()
    {
        LoadPatientCommand = ReactiveCommand.CreateFromTask(LoadPatientAsync);
        AddPatientCommand = ReactiveCommand.CreateFromTask(AddPatientAsync);
        EditPatientCommand = ReactiveCommand.CreateFromTask(EditPatientAsync);
        DeletePatientCommand = ReactiveCommand.CreateFromTask<Patient?>(DeletePatientAsync);

        LoadPatientCommand.Execute();
    }
    
    private async Task EditPatientAsync()
    {
        if (CurrentPatient.IsEmpty()) return;
        await _service.UpdatePatientAsync(CurrentPatient);
        LoadPatientCommand.Execute();
        await DialogHost.Show(new TextBlock {Text = "Edit Doctor"});
    }

    private async Task DeletePatientAsync(Patient? patient)
    {
        if (patient == null) return;
        await _service.DeletePatientAsync(patient);
        LoadPatientCommand.Execute();
        OnChangedModel.Invoke();
    }

    private async Task AddPatientAsync()
    {
        if (CurrentPatient.IsEmpty()) return;
        await _service.AddPatientAsync(CurrentPatient);
        
        LoadPatientCommand.Execute();
    }

    private async Task LoadPatientAsync()
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { Patients.Clear(); });
        var patients = await _service.GetPatientsAsync();
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var patient in patients) Patients.Add(patient);
        });
    }
    
    
    public void RemovePatient(Patient patient)
    {
        CurrentPatient = patient;
        DeletePatientCommand.Execute();
        LoadPatientCommand.Execute();
    }
    
    public void ClearAllPatientMembers()
    {
        CurrentPatient.ClearAllMembers();
    }
    

    public void ClearFirstName() => CurrentPatient.FirstName = string.Empty;
    public void ClearLastName() => CurrentPatient.LastName = string.Empty;
    public void ClearMiddleName() => CurrentPatient.MiddleName = string.Empty;
    public void ClearBirthDate() => CurrentPatient.BirthDate = 0;
}