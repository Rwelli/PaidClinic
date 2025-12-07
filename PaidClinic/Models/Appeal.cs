using System;
using ReactiveUI;

namespace PaidClinic.Models;

public class Appeal(
    int id = 0,
    Doctor? doctor = null,
    Patient? patient = null,
    DateTimeOffset? date = null,
    string diagnosis = "",
    uint costOfTreatment = 0
    ) : ReactiveObject, ICloneable
{
    private int _id = id;
    private Doctor? _doctor = doctor;
    private Patient? _patient = patient;
    private DateTimeOffset? _date = date;
    private string _diagnosis = diagnosis;
    private uint _costOfTreatment = costOfTreatment;

    public int Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public Doctor? Doctor
    {
        get => _doctor;
        set => this.RaiseAndSetIfChanged(ref _doctor, value);
    }

    public Patient? Patient
    {
        get => _patient;
        set => this.RaiseAndSetIfChanged(ref _patient, value);
    }

    public DateTimeOffset? Date
    {
        get => _date;
        set => this.RaiseAndSetIfChanged(ref _date, value);
    }

    public string Diagnosis
    {
        get => _diagnosis;
        set => this.RaiseAndSetIfChanged(ref _diagnosis, value);
    }

    public uint CostOfTreatment
    {
        get => _costOfTreatment;
        set => this.RaiseAndSetIfChanged(ref _costOfTreatment, value);
    }

    public void ClearAllMembers()
    {
        Id = 0;
        Doctor = null;
        Patient = null;
        Date = null;
        Diagnosis = "";
        CostOfTreatment = 0;
    }

    public bool IsEmpty()
    {
        return Doctor == null 
               || Patient == null
               || Date == null
               || Diagnosis == "";
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}