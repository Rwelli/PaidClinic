using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using PaidClinic.Models;
using PaidClinic.Services;
using ReactiveUI;

namespace PaidClinic.ViewModels;

public class PieData(string name, double value)
{
    public string Name { get; set; } = name;
    public double[] Values { get; set; } = [value];
}

public class ReportViewModel : ViewModelBase
{
    private DateTimeOffset? _beginDate = DateTimeOffset.Now;
    private DateTimeOffset? _endDate = DateTimeOffset.Now;
    private readonly ReportDbService _reportDbService = new();
    
    public ObservableCollection<PieData> ReportAppealData { get; set; } = [];

    public DateTimeOffset? BeginDate
    {
        get => _beginDate;
        set => this.RaiseAndSetIfChanged(ref _beginDate, value);
    }
    
    public DateTimeOffset? EndDate
    {
        get => _endDate;
        set => this.RaiseAndSetIfChanged(ref _endDate, value);
    }
    
    public ObservableCollection<ReportAppeal> ReportAppeals { get; set; } = [];
    
    public ReactiveCommand<Unit, Unit> LoadReportAppealsCommand { get; }

    public ReportViewModel()
    {
        LoadReportAppealsCommand = ReactiveCommand.CreateFromTask(LoadReportAppealsAsync);
    }

    public async Task LoadReportAppealsAsync()
    {
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => { ReportAppeals.Clear(); ReportAppealData.Clear(); });
        var reportAppeals = await _reportDbService.GetReportAppeals(beginDate: BeginDate.Value.Date.Date, endDate: EndDate.Value.Date.Date);
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var reportAppeal in reportAppeals)
            {
                ReportAppeals.Add(reportAppeal);
                ReportAppealData.Add(new PieData(reportAppeal.Doctor.LastNameWithInitials, reportAppeal.CostOfTreatment));
            };
        });
    }
}