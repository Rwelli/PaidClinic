using System;
using ReactiveUI;

namespace PaidClinic.Models;

public class Doctor(
    int id = 0, string firstName = "", string lastName = "", string middleName = "", 
    string speciality = "", string category =  "") : ReactiveObject, ICloneable
{
    private int _id = id;
    private string _firstName = firstName;
    private string _lastName = lastName;
    private string _middleName = middleName;
    private string _speciality = speciality;
    private string _category = category;
    
    public int Id 
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }

    public string MiddleName
    {
        get => _middleName;
        set => this.RaiseAndSetIfChanged(ref _middleName, value);
    }

    public string Speciality
    {
        get => _speciality;
        set => this.RaiseAndSetIfChanged(ref _speciality, value);
    }

    public string Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }
    
    public string FullName => $"{FirstName} {LastName} {MiddleName}";
    public string LastNameWithInitials => $"{LastName} {FirstName[0]}. {MiddleName[0]}.";

    public void ClearAllMembers()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        MiddleName = string.Empty;
        Speciality = string.Empty;
        Category = string.Empty;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(FirstName)
               || string.IsNullOrEmpty(LastName)
               || string.IsNullOrEmpty(MiddleName)
               || string.IsNullOrEmpty(Speciality)
               || string.IsNullOrEmpty(Category);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}