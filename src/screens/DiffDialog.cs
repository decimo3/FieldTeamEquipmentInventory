using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FieldTeamEquipmentInventory.Models;

namespace FieldTeamEquipmentInventory.Screens;

public class DiffReport
{
    public char _ { get; set; }
    public long Id { get; set; }
    public Equipment.KindEnum Kind { get; set; }
    public DiffEnum Status { get; set; }
    public enum DiffEnum { Miss = -1, Fine = 0, Extra = 1 }
}

public class DiffDialog : Window
{
    private DataGrid _tbl_main = null;
    private Button confirmBtn = null;
    private Button cancelBtn = null;
    public DiffDialog
    (
        List<Transaction> current_kit,
        List<Transaction> expected_kit
    )
    {
        var contents = HeavyLogic(current_kit, expected_kit);
        BuildLayout(contents);
        HandleEvent();
    }
    private List<DiffReport> HeavyLogic
    (
        List<Transaction> current_kit,
        List<Transaction> expected_kit
    )
    {
        var contents = new List<DiffReport>();

        var current_ids = current_kit.Select(t => t.Equipment.Id).OrderBy(x => x).ToList();
        var expected_ids = expected_kit.Select(t => t.Equipment.Id).OrderBy(x => x).ToList();

        var less = expected_ids.Except(current_ids).ToList();
        var more = current_ids.Except(expected_ids).ToList();

        // For each expected equipment, print line with Missing or OK
        foreach (var expected in expected_kit)
        {
            contents.Add(new DiffReport
            {
                _ = less.Contains(expected.Equipment.Id) ? '\u2796' : '\u2714',
                Id = expected.Equipment.Id,
                Kind = expected.Equipment.Kind,
                Status = less.Contains(expected.Equipment.Id) ?
                    DiffReport.DiffEnum.Miss : DiffReport.DiffEnum.Fine,
            });
        }

        // For each any extra equipments that are in current but not expected
        foreach (var extraId in more)
        {
            var tx = current_kit.Single(t => t.Equipment.Id == extraId);
            contents.Add(new DiffReport
            {
                _ = '\u2795',
                Id = tx.Equipment.Id,
                Kind = tx.Equipment.Kind,
                Status = DiffReport.DiffEnum.Extra,
            });
        }

        return contents;
    }

    private void BuildLayout(List<DiffReport> contents)
    {
        Title = Helpers.Resources.GetString("DIFF_DIALOG_TITLE_TXT");
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ResizeMode = ResizeMode.NoResize;
        Height = 430;
        Width = 400;

        var mainStack = new StackPanel
        {
            Margin = new Thickness(20),
        };

        var _lbl_header = new Label
        {
            FontSize = 24,
            
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(left: 0, top: 10, right: 0, bottom: 20),
            Content = Helpers.Resources.GetString("DIFF_DIALOG_HEADER_TXT"),
        };

        _tbl_main = new DataGrid
        {
            IsReadOnly = true,
            ItemsSource = contents,
            AutoGenerateColumns = true,
            ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star),
        };

        var _lbl_alert = new Label
        {
            Foreground = Brushes.Red,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(left: 0, top: 10, right: 0, bottom: 0),
            Content = Helpers.Resources.GetString("DIFF_DIALOG_ALERT_TXT"),
        };

        var _lbl_footer = new Label
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(left: 0, top: 10, right: 0, bottom: 0),
            Content = Helpers.Resources.GetString("DIFF_DIALOG_FOOTER_TXT"),
        };

        var footerStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0)
        };

        confirmBtn = new Button
        {
            Width = 80,
            Height = 40,
            Margin = new Thickness(10, 0, 10, 0),
            Content = Helpers.Resources.GetString("DIFF_DIALOG_CONFIRM_BTN"),
        };

        cancelBtn = new Button
        {
            Width = 80,
            Height = 40,
            Margin = new Thickness(10, 0, 10, 0),
            Content = Helpers.Resources.GetString("DIFF_DIALOG_CANCEL_BTN"),
        };

        footerStack.Children.Add(confirmBtn);
        footerStack.Children.Add(cancelBtn);

        mainStack.Children.Add(_lbl_header);
        mainStack.Children.Add(_tbl_main);
        if (contents.Any(c => c.Status != DiffReport.DiffEnum.Fine))
            mainStack.Children.Add(_lbl_alert);
        mainStack.Children.Add(_lbl_footer);
        mainStack.Children.Add(footerStack);

        Content = mainStack;
    }

    private void HandleEvent()
    {
        _tbl_main.LoadingRow += (s, e) =>
        {
            var item = (DiffReport)e.Row.Item;

            switch (item.Status)
            {
                case DiffReport.DiffEnum.Extra:
                    e.Row.Background = Brushes.Yellow;
                    break;

                case DiffReport.DiffEnum.Miss:
                    e.Row.Background = Brushes.LightCoral;
                    break;

                default:
                    e.Row.Background = Brushes.White;
                    break;
            }
        };
        confirmBtn.Click += (_, _) =>
        {
            DialogResult = true;
        };
        cancelBtn.Click += (_, _) =>
        {
            DialogResult = false;
        };
    }
}