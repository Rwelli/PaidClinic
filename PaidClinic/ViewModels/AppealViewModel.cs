using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using PaidClinic.Models;
using PaidClinic.Services;
using ReactiveUI;

namespace PaidClinic.ViewModels;

public class AppealViewModel : ViewModelBase
{
    private readonly DoctorsDbService _doctorDbService = new();
    private readonly PatientDbService _patientDbService = new();
    private readonly AppealDbService _appealDbService = new();
    private readonly DoctorsViewModel _doctorsViewModel;
    private readonly PatientViewModel _patientViewModel;
    private Appeal _currentAppeal = new();

    public Appeal CurrentAppeal
    {
        get => _currentAppeal;
        set => this.RaiseAndSetIfChanged(ref _currentAppeal, value);
    }
    
    public ObservableCollection<Appeal> Appeals { get; } = [];

    public ObservableCollection<Doctor> Doctors => _doctorsViewModel.Doctors;

    public ObservableCollection<Patient> Patients => _patientViewModel.Patients;
    
    public ReactiveCommand<Unit, Unit> LoadDoctorsCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadPatientCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadAppealsCommand { get; }
    
    
    public ReactiveCommand<Unit, Unit> AddAppealCommand { get; }
    public ReactiveCommand<Appeal?, Unit> DeleteAppealCommand { get; }
    public ReactiveCommand<Unit, Unit> EditAppealCommand { get; }

    public AppealViewModel(DoctorsViewModel doctorsViewModel, PatientViewModel patientViewModel)
    {
        _doctorsViewModel = doctorsViewModel;
        _patientViewModel = patientViewModel;
        
        LoadDoctorsCommand = ReactiveCommand.CreateFromTask(LoadDoctorsAsync);
        LoadPatientCommand = ReactiveCommand.CreateFromTask(LoadPatientAsync);
        LoadAppealsCommand = ReactiveCommand.CreateFromTask(LoadAppealsAsync);
        AddAppealCommand = ReactiveCommand.CreateFromTask(AddAppealAsync);
        DeleteAppealCommand = ReactiveCommand.CreateFromTask<Appeal?>(DeleteAppealAsync);
        EditAppealCommand = ReactiveCommand.CreateFromTask(RemoveAppealAsync);
        LoadAppealsCommand.Execute();
        
        _doctorsViewModel.OnChangedModel += () => LoadAppealsCommand.Execute();
        _patientViewModel.OnChangedModel += () => LoadAppealsCommand.Execute();
    }
    
    private async Task LoadDoctorsAsync()
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { Doctors.Clear(); });
        var doctors = await _doctorDbService.GetDoctors();
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var doctor in doctors) Doctors.Add(doctor);
        });
    }
    
    private async Task LoadPatientAsync()
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { Patients.Clear(); });
        var patients = await _patientDbService.GetPatientsAsync();
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var patient in patients) Patients.Add(patient);
        });
    }
    
    private async Task LoadAppealsAsync()
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { Appeals.Clear(); });
        var appeals = await _appealDbService.GetAppeals();
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var appeal in appeals) Appeals.Add(appeal);
        });
    }

    private async Task AddAppealAsync()
    {
        if (CurrentAppeal.IsEmpty()) return;
        await _appealDbService.AddAppealAsync(CurrentAppeal);
        
        LoadAppealsCommand.Execute();
        _doctorsViewModel.LoadDoctorsCommand.Execute();
        _patientViewModel.LoadPatientCommand.Execute();
    }

    private async Task DeleteAppealAsync(Appeal? appeal)
    {
        if (appeal == null) return;
        await _appealDbService.DeleteAppeal(appeal);
        LoadAppealsCommand.Execute();
    }
    
    private async Task RemoveAppealAsync()
    {
        if (CurrentAppeal.IsEmpty()) return;
        await _appealDbService.UpdateAppealAsync(CurrentAppeal);
        LoadAppealsCommand.Execute();
    }
    
    public void ClearAllAppealMembers() => CurrentAppeal.ClearAllMembers();

    public void ClearDoctor() => CurrentAppeal.Doctor = null;
    public void ClearPatient() => CurrentAppeal.Patient = null;
    public void ClearDate() => CurrentAppeal.Date = null;
    public void ClearDiagnosis() => CurrentAppeal.Diagnosis = string.Empty;
    public void ClearCostOfTreatment() => CurrentAppeal.CostOfTreatment = 0;
}