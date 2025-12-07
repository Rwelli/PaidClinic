using ReactiveUI;

namespace PaidClinic.ViewModels;

public class ErrorReportWindowViewModel : ViewModelBase
{
    private string _errorText = "";

    public string ErrorText
    {
        get => _errorText;
        set => this.RaiseAndSetIfChanged(ref _errorText, value);
    }
}