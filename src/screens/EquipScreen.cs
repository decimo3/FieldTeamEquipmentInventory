using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using FieldTeamEquipmentInventory.Helpers;
using FieldTeamEquipmentInventory.Interfaces;
using FieldTeamEquipmentInventory.Models;

namespace FieldTeamEquipmentInventory.Screens;

public class EquipScreen : Page
{
    private readonly IDatabase _database;
    private TextBox _txt_equip = null;
    private ComboBox _sel_kind = null;
    private ComboBox _sel_status = null;
    private TextBox _txt_note = null;
    private Button _btn_save = null;
    private Button _btn_back = null;
    private TextBox _txt_kits = null;
    private Transaction.KindEnum _mem_kind = Transaction.KindEnum.Idle;
    public EquipScreen(IDatabase database)
    {
        _database = database;
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
            Content = Helpers.Resources.GetString("EQUIP_SCREEN_TITLE_LBL"),
        };

        _txt_equip = new TextBox { Text = Helpers.Resources.GetString("EQUIP_SCREEN_EQUIP_PLACEHOLDER"), AcceptsReturn = false, TextWrapping = TextWrapping.NoWrap, Height = 40 };

        _sel_kind = new ComboBox { ItemsSource = Enum.GetValues<Equipment.KindEnum>(), Height = 40 };

        _sel_status = new ComboBox { ItemsSource = Enum.GetValues<Equipment.StatusEnum>(), Height = 40 };

        _txt_kits = new TextBox { Text = Helpers.Resources.GetString("EQUIP_SCREEN_KIT_PLACEHOLDER"), AcceptsReturn = false, TextWrapping = TextWrapping.NoWrap, Height = 40 };

        _txt_note = new TextBox { Text = Helpers.Resources.GetString("EQUIP_SCREEN_NOTE_PLACEHOLDER"), AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, Height = 80 };

        _btn_save = new Button { Content = Helpers.Resources.GetString("GLOBAL_SCREEN_SAVE_BTN"), Height = 40 };

        _btn_back = new Button { Content = Helpers.Resources.GetString("GLOBAL_SCREEN_BACK_BTN"), Height = 40 };

        stack.Children.Add(_lbl_header);
        stack.Children.Add(_txt_equip);
        stack.Children.Add(_sel_kind);
        stack.Children.Add(_sel_status);
        stack.Children.Add(_txt_kits);
        stack.Children.Add(_txt_note);
        stack.Children.Add(_btn_save);
        stack.Children.Add(_btn_back);

        Content = stack;
    }
    private void HandleEvent()
    {
        _txt_equip.PreviewTextInput += (s, e) =>
        {
            Validators.NumberInputOnly(s, e);
            if (_txt_equip.Text.Length > 5)
            {
                var id = long.Parse(_txt_equip.Text + e.Text);
                var equip = _database.GetEquipment(id);
                if (equip is not null)
                {
                    _sel_kind.Text = equip.Kind.ToString();
                    _sel_kind.IsEnabled = false;
                    _sel_status.Text = equip.Status.ToString();
                    _sel_status.IsEnabled = false;
                    _txt_kits.Text = equip.Kit.ToString();
                    _mem_kind = _database.GetTransaction(id)!.Kind;
                }
                else
                {
                    _sel_kind.IsEnabled = true;
                    _sel_status.IsEnabled = true;
                }
            }
        };
        _btn_back.Click += (_, _) => NavigationService.GoBack();
        _btn_save.Click += (_, _) =>
        {
            try
            {
                var id_equipment = long.Parse(_txt_equip.Text);
                var equip = new Equipment
                (
                    id: id_equipment,
                    kind: (Equipment.KindEnum)_sel_kind.SelectedItem,
                    status: (Equipment.StatusEnum)_sel_status.SelectedItem,
                    kit: _txt_kits.Text
                );
                var transa = new Transaction
                (
                    timestamp: DateTime.Now,
                    idEquipment: id_equipment,
                    kind: _mem_kind,
                    idEmployerFrom: Hodor.From,
                    idEmployerTo: Hodor.From,
                    equipment: equip,
                    note: (_txt_note.Text != Helpers.Resources.GetString("EQUIP_SCREEN_NOTE_PLACEHOLDER")) ? _txt_note.Text : null
                );
                _database.AddEquipment(equip);
                _database.AddTransaction(transa);
                NavigationService.GoBack();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        };
    }
}