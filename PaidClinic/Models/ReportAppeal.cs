using System;
using ReactiveUI;

namespace PaidClinic.Models;

public class ReportAppeal( Doctor? doctor = null, int costOfTreatment = 0) : ReactiveObject, ICloneable
{
    private Doctor? _doctor = doctor;
    private int _costOfTreatment = costOfTreatment;

    public Doctor? Doctor
    {
        get => _doctor;
        set => this.RaiseAndSetIfChanged(ref _doctor, value);
    }

    public int CostOfTreatment
    {
        get => _costOfTreatment;
        set => this.RaiseAndSetIfChanged(ref _costOfTreatment, value);
    }
    
    public bool isEmpty() => Doctor == null;
    
    public object Clone()
    {
        return MemberwiseClone();
    }
}