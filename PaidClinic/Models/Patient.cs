using System;
using ReactiveUI;

namespace PaidClinic.Models;

public class Patient(
    int id = 0, string lastName = "", string firstName = "", string middleName = "", int birthDate = 0
    ) : ReactiveObject, ICloneable
{
    private int _id = id;
    private string _lastName = lastName;
    private string _firstName = firstName;
    private string _middleName = middleName;
    private int _birthDate = birthDate;

    public int Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    public string MiddleName
    {
        get => _middleName;
        set => this.RaiseAndSetIfChanged(ref _middleName, value);
    }

    public int BirthDate
    {
        get => _birthDate;
        set => this.RaiseAndSetIfChanged(ref _birthDate, value);
    }
    
    public string FullName => $"{FirstName} {LastName} {MiddleName}";
    
    public void ClearAllMembers()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        MiddleName = string.Empty;
        BirthDate = 0;
        Id = -1;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(FirstName) 
               || string.IsNullOrEmpty(LastName)
               || string.IsNullOrEmpty(MiddleName)
               || BirthDate == 0;
    }
    
    public object Clone()
    {
        return MemberwiseClone();
    }
}