using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using FieldTeamEquipmentInventory.Helpers;
using FieldTeamEquipmentInventory.Interfaces;
using FieldTeamEquipmentInventory.Models;

namespace FieldTeamEquipmentInventory.Screens;

public class EnrollScreen : Page
{
    private readonly IBiometrics _biometrics;
    private readonly IDatabase _database;
    private TextBox _txt_fullName = null;
    private TextBox _txt_registry = null;
    private Button _btn_finger = null;
    private Label _lbl_status = null;
    private Button _btn_save = null;
    private Button _btn_back = null;
    private CancellationTokenSource _cts;
    private String _template = null;
    private Image _previewImage = null;


    public EnrollScreen(IDatabase database, IBiometrics biometrics)
    {
        _database = database;
        _biometrics = biometrics;
        BuildLayout();
        HandleEvent();
    }
    private void BuildLayout()
    {
        var mainStack = new StackPanel { Margin = new Thickness(20) };

        var _lbl_header = new Label
        {
            FontSize = 24,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(left: 0, top: 20, right: 0, bottom: 20),
            Content = Helpers.Resources.GetString("MAIN_SCREEN_ENROLL_BTN"),
        };

        var layout = new UniformGrid { Columns = 2 };

        var leftStack = new StackPanel { Margin = new Thickness(20), VerticalAlignment = VerticalAlignment.Center };


        _previewImage = new Image { Width = 320, Height = 240, Margin = new Thickness(0, 10, 0, 10) };

        _lbl_status = new Label
        {
            Height = 40,
            HorizontalAlignment = HorizontalAlignment.Center,
            Content = Helpers.Resources.GetString("ENROLL_SCREEN_FINGER_WAIT_LBL"),
        };

        var rightStack = new StackPanel { Margin = new Thickness(20), VerticalAlignment = VerticalAlignment.Center };

        var _lbl_name = new Label
        {
            Target = _txt_fullName,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Content = Helpers.Resources.GetString("ENROLL_SCREEN_FULLNAME_PLACEHOLDER")
        };

        _txt_fullName = new TextBox { AcceptsReturn = false, TextWrapping = TextWrapping.NoWrap, Height = 40 };

        var _lbl_registry = new Label
        {
            Target = _txt_registry,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Content = Helpers.Resources.GetString("ENROLL_SCREEN_REGISTRY_PLACEHOLDER")
        };

        _txt_registry = new TextBox { AcceptsReturn = false, TextWrapping = TextWrapping.NoWrap, Height = 40 };

        _btn_finger = new Button { Content = Helpers.Resources.GetString("ENROLL_SCREEN_FINGER_BTN"), Height = 40 };
        _btn_save = new Button { Content = Helpers.Resources.GetString("ENROLL_SCREEN_SAVE_BTN"), Height = 40 };
        _btn_back = new Button { Content = Helpers.Resources.GetString("ENROLL_SCREEN_BACK_BTN"), Height = 40 };

        // Add Preview and Status to left stack
        leftStack.Children.Add(_previewImage);
        leftStack.Children.Add(_lbl_status);
        // Add all the others controls to right stack
        rightStack.Children.Add(_lbl_name);
        rightStack.Children.Add(_txt_fullName);
        rightStack.Children.Add(_lbl_registry);
        rightStack.Children.Add(_txt_registry);
        rightStack.Children.Add(_btn_finger);
        rightStack.Children.Add(_btn_save);
        rightStack.Children.Add(_btn_back);
        // Add left and right panels to layout
        layout.Children.Add(leftStack);
        layout.Children.Add(rightStack);
        // Add header and layout to main stack
        mainStack.Children.Add(_lbl_header);
        mainStack.Children.Add(layout);

        Content = mainStack;
    }

    private async Task GetBiometric()
    {
        _cts = new CancellationTokenSource();
        try
        {
            _btn_finger.IsEnabled = false;
            _btn_save.IsEnabled = false;

            var result = await _biometrics.EnrollmentAsync(
                (string message) => {
                    Dispatcher.Invoke(() =>
                    {
                        _lbl_status.Content = message;
                    });
                }, _cts.Token);

            if (result is null)
                return;

            _template = _biometrics.ToBase64(result);
            Dispatcher.Invoke(() => _lbl_status.Content = Helpers.Resources.GetString("ENROLL_SCREEN_FINGER_SUCCESS_LBL"));
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
            _btn_finger.IsEnabled = true;
            _btn_save.IsEnabled = true;
        }
    }

    private async Task SaveEmployer()
    {
        try
        {
            var registry = int.Parse(_txt_registry.Text);
            var employer = new Employer(registry, _txt_fullName.Text, _template, DateTime.Now);
            _database.AddEmployer(employer);
            NavigationService.GoBack();
        }
        catch (Exception error)
        {
            MessageBox.Show(error.Message);
        }
    }

    private void HandleEvent()
    {
        _txt_fullName.PreviewTextInput += Validators.CharacterInputOnly;
        _txt_registry.PreviewTextInput += Validators.NumberInputOnly;
        _btn_finger.Click += async (_, _) => await GetBiometric();
        _btn_save.Click += async (_, _) => await SaveEmployer();
        _btn_back.Click += (_, _) => NavigationService.GoBack();

        _biometrics.OnPreviewFrame += (bytes) =>
        {
            Dispatcher.Invoke(() =>
            {
                var bmp = BitmapLoader.LoadBitmap(bytes);
                if (bmp != null)
                    _previewImage.Source = bmp;
            });
        };
    }
}