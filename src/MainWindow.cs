using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;

using FieldTeamEquipmentInventory.Screens;

namespace FieldTeamEquipmentInventory;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Title = Helpers.Resources.GetString("APPLICATION_TITLE");
        Width = 800;
        Height = 600;
        var frame = new Frame
        {
            NavigationUIVisibility = NavigationUIVisibility.Hidden
        };
        Content = frame;
        // TODO - change to AuthScreen when implemented
        var page = MainApplication.Host!.Services.GetRequiredService<MainScreen>();
        frame.Navigate(page);
    }
}