using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

using FieldTeamEquipmentInventory.Helpers;
using FieldTeamEquipmentInventory.Interfaces;

namespace FieldTeamEquipmentInventory.Screens;

public class AuthScreen : Page
{
    private readonly IBiometrics _biometrics;
    private readonly IDatabase _database;
    private Image _img_preview = null;
    private Label _lbl_status = null;
    private Button _btn_login = null;
    private CancellationTokenSource _cts = null;
    public AuthScreen
    (
        IDatabase database,
        IBiometrics biometrics
    )
    {
        _biometrics = biometrics;
        _database = database;

        KeepAlive = false;
        Focusable = true;
        Loaded += (_, _) => Focus();
        
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
            Content = Helpers.Resources.GetString("AUTH_SCREEN_TITLE_LBL"),
        };

        _img_preview = new Image { Width = 320, Height = 240, Margin = new Thickness(0, 10, 0, 10) };

        _lbl_status = new Label { Content = Helpers.Resources.GetString("AUTH_SCREEN_FINGER_WAIT_LBL"), Height = 40 };

        _btn_login = new Button { Content = Helpers.Resources.GetString("AUTH_SCREEN_FINGER_BTN"), Height = 40 };

        stack.Children.Add(_lbl_header);
        stack.Children.Add(_img_preview);
        stack.Children.Add(_lbl_status);
        stack.Children.Add(_btn_login);
        Content = stack;
    }

    private async Task Authentication()
    {
        if (_cts != null) return;
        _cts = new CancellationTokenSource();
        try
        {
            _btn_login.IsEnabled = false;
            var result = await _biometrics.CaptureAsync(
                (string message) => {
                    Dispatcher.Invoke(() =>
                    {
                        _lbl_status.Content = message;
                    });
                }, _cts.Token);

            if (result is null)
                return;

            foreach (var employer in _database.GetEmployers())
            {
                var stored = _biometrics.FromBase64(employer.Template);
                if (_biometrics.Verify(stored, result))
                {
                    var isAuthenticated = Hodor.From != 0;

                    Dispatcher.Invoke(() => _lbl_status.Content = Helpers.Resources.GetString("AUTH_SCREEN_FINGER_SUCCESS_LBL"));
                    
                    if (isAuthenticated)
                        Hodor.To = employer.Registry;
                    else
                        Hodor.From = employer.Registry;

                    await Task.Delay(3_000, _cts.Token);

                    Page page = !isAuthenticated ?
                        MainApplication.Host!.Services.GetRequiredService<MainScreen>() :
                        MainApplication.Host!.Services.GetRequiredService<EntryScreen>();
                    
                    NavigationService.Navigate(page);
                    return;
                }
            }
            throw new InvalidOperationException(Helpers.Resources.GetString("AUTH_SCREEN_FINGER_FAILURE_LBL"));
        }
        catch (OperationCanceledException)
        {
            _lbl_status.Content = Helpers.Resources.GetString("AUTH_SCREEN_FINGER_CANCELED_LBL");
        }
        catch (Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                _lbl_status.Content = ex.Message;
            });
        }
        finally
        {
            _btn_login.IsEnabled = true;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void HandleEvent()
    {
        PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                _cts?.Cancel();
                e.Handled = true;
            }
        };
        _btn_login.Click += async (_, _) => await Authentication();
        _biometrics.OnPreviewFrame += (bytes) =>
        {
            Dispatcher.Invoke(() =>
            {
                var bmp = BitmapLoader.LoadBitmap(bytes);
                if (bmp != null)
                    _img_preview.Source = bmp;
            });
        };
    }
}
