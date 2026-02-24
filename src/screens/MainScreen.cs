using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;

using FieldTeamEquipmentInventory.Helpers;

namespace FieldTeamEquipmentInventory.Screens;

public class MainScreen : Page
{
    private Button _btn_help = null;
    private Button _btn_enroll = null;
    private Button _btn_equip = null;
    private Button _btn_entry = null;
    private Button _btn_report = null;
    private Button _btn_logout = null;

    public MainScreen()
    {
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
            Margin = new Thickness(left: 0, top:20, right:0, bottom:20),
            Content = Helpers.Resources.GetString("MAIN_SCREEN_HEAD_TXT"),
        };
        _btn_help = new Button { Content = Helpers.Resources.GetString("MAIN_SCREEN_HELP_BTN"), Height = 40 };
        _btn_enroll = new Button { Content = Helpers.Resources.GetString("MAIN_SCREEN_ENROLL_BTN"), Height = 40 };
        _btn_equip = new Button { Content = Helpers.Resources.GetString("MAIN_SCREEN_EQUIP_BTN"), Height = 40 };
        _btn_entry = new Button { Content = Helpers.Resources.GetString("MAIN_SCREEN_ENTRY_BTN"), Height = 40 };
        _btn_report = new Button { Content = Helpers.Resources.GetString("MAIN_SCREEN_REPORT_BTN"), Height = 40 };
        _btn_logout = new Button { Content = Helpers.Resources.GetString("MAIN_SCREEN_LOGOUT_BTN"), Height = 40 };

        stack.Children.Add(_lbl_header);
        stack.Children.Add(_btn_enroll);
        stack.Children.Add(_btn_equip);
        stack.Children.Add(_btn_entry);
        stack.Children.Add(_btn_report);
        stack.Children.Add(_btn_help);
        stack.Children.Add(_btn_logout);

        Content = stack;
    }
    private void HandleEvent()
    {
        _btn_help.Click += (_, _) => NavigationService.Navigate(new HelpScreen());
        _btn_enroll.Click += (_, _) => NavigationService.Navigate(new EnrollScreen());
        _btn_equip.Click += (_, _) => NavigationService.Navigate(new EquipScreen());
        _btn_entry.Click += (_, _) => NavigationService.Navigate(new AuthScreen());
        _btn_report.Click += (_, _) => NavigationService.Navigate(new ReportScreen());
        _btn_logout.Click += (_, _) => Application.Current.Shutdown();
    }
}