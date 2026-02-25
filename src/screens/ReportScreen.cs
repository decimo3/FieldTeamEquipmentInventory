using System.IO;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Navigation;
using CsvHelper;

using FieldTeamEquipmentInventory.Interfaces;
using FieldTeamEquipmentInventory.Models;

namespace FieldTeamEquipmentInventory.Screens;

public class ReportScreen : Page
{
    private IDatabase _database;
    private DataGrid _tbl_report = null;
    private Button _btn_export = null;
    private Button _btn_back = null;
    public ReportScreen(IDatabase database)
    {
        _database = database;
        KeepAlive = false;
        BuildLayout();
        HandleEvent();
    }
    private void BuildLayout()
    {
        var stack = new StackPanel { Margin = new Thickness(20) };

        var _lbl_header = new Label
        {
            FontSize = 24,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(left: 0, top: 20, right: 0, bottom: 20),
            Content = Helpers.Resources.GetString("REPORT_SCREEN_HEADER_TXT"),
        };

        _tbl_report = new DataGrid { AutoGenerateColumns = true, ItemsSource = GetReport(), IsReadOnly = true };

        _btn_export = new Button { Content = Helpers.Resources.GetString("GLOBAL_SCREEN_EXPORT_BTN"), Height = 40 };

        _btn_back = new Button { Content = Helpers.Resources.GetString("GLOBAL_SCREEN_BACK_BTN"), Height = 40 };

        stack.Children.Add(_lbl_header);
        stack.Children.Add(_tbl_report);
        stack.Children.Add(_btn_export);
        stack.Children.Add(_btn_back);

        Content = stack;
    }

    private List<Report> GetReport()
    {
        try
        {
            return _database.GetTransactions().Select(t =>
            {
                return new Report
                {
                    CreateAt = t.Timestamp.ToString("G"),
                    Kit = t.Equipment.Kit,
                    TransactionKind = t.Kind,
                    IdEquipment = t.IdEquipment,
                    EquipmentKind = t.Equipment.Kind,
                    EquipmentStatus = t.Equipment.Status,
                    IdEmployerFrom = t.IdEmployerFrom,
                    IdEmployerTo = t.IdEmployerTo,
                    Note = t.Note,
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return new List<Report>();
        }
    }

    private void ExportTable()
    {
        try
        {
            var filepath = System.IO.Path.Combine(
                System.AppContext.BaseDirectory,
                "export.csv");
            using var writer = new StreamWriter(filepath);
            using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
            csv.WriteRecords(GetReport());
            MessageBox.Show(Helpers.Resources.GetString("GLOBAL_SCREEN_EXPORTED_TXT", filepath));
            // TODO - Open report file automatically on default viewer
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void HandleEvent()
    {
        _btn_export.Click += (_, _) => ExportTable();
        _btn_back.Click += (_, _) => NavigationService.GoBack();
    }
}