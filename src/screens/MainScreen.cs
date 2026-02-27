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

        _btn_help = new Button
        {
            Height = 40,
        _btn_report = new Button { Content = Helpers.Resources.GetString("MAIN_SCREEN_REPORT_BTN"), Height = 40 };
            Content = Helpers.Resources.GetString("MAIN_SCREEN_HELP_BTN"),
        };

        _btn_enroll = new Button
        {
            Height = 40,
            Content = Helpers.Resources.GetString("MAIN_SCREEN_ENROLL_BTN"),
        };

        _btn_equip = new Button
        {
            Height = 40,
            Content = Helpers.Resources.GetString("MAIN_SCREEN_EQUIP_BTN"),
        };

        _btn_entry = new Button
        {
            Height = 40,
            Content = Helpers.Resources.GetString("MAIN_SCREEN_ENTRY_BTN"),
        };

        _btn_report = new Button
        {
            Height = 40,
            Content = Helpers.Resources.GetString("MAIN_SCREEN_REPORT_BTN"),
        };

        _btn_logout = new Button
        {
            Height = 40,
            Content = Helpers.Resources.GetString("MAIN_SCREEN_LOGOUT_BTN"),
        };

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
        _btn_help.Click += (_, _) =>
        {
            var page = MainApplication.Host!.Services.GetRequiredService<HelpScreen>();
            NavigationService.Navigate(page);
        };

        _btn_enroll.Click += (_, _) =>
        {
            var page = MainApplication.Host!.Services.GetRequiredService<EnrollScreen>();
            NavigationService.Navigate(page);
        };

        _btn_equip.Click += (_, _) =>
        {
            var page = MainApplication.Host!.Services.GetRequiredService<EquipScreen>();
            NavigationService.Navigate(page);
        };

        _btn_entry.Click += (_, _) =>
        {
            var page = MainApplication.Host!.Services.GetRequiredService<AuthScreen>();
            NavigationService.Navigate(page);
        };

        _btn_report.Click += (_, _) =>
        {
            var page = MainApplication.Host!.Services.GetRequiredService<ReportScreen>();
            NavigationService.Navigate(page);
        };

        _btn_logout.Click += (_, _) => Application.Current.Shutdown();
    }
}