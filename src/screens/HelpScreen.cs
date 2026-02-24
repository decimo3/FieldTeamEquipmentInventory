using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FieldTeamEquipmentInventory.Screens;

public class HelpScreen : Page
{
    private Button _btn_back = null;

    public HelpScreen()
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
            Margin = new Thickness(left: 0, top: 20, right: 0, bottom: 20),
            Content = Helpers.Resources.GetString("HELP_SCREEN_HEAD_TXT"),
        };

        var _lbl_help = new TextBox
        {
            FontSize = 12,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(left: 0, top: 20, right: 0, bottom: 20),
            Text = Helpers.Resources.GetString("HELP_SCREEN_HELP_TXT"),
            TextWrapping = TextWrapping.Wrap,
        };

        _btn_back = new Button
        {
            Height = 40,
            Content = Helpers.Resources.GetString("HELP_SCREEN_BACK_BTN"),
        };

        stack.Children.Add(_lbl_header);
        stack.Children.Add(_lbl_help);
        stack.Children.Add(_btn_back);

        Content = stack;
    }
    private void HandleEvent()
    {
        _btn_back.Click += (_, _) => NavigationService.GoBack();
    }
}