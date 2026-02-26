using FieldTeamEquipmentInventory.Helpers;
using FieldTeamEquipmentInventory.Interfaces;
using FieldTeamEquipmentInventory.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FieldTeamEquipmentInventory.Screens;

public class EntryScreen : Page
{
    private readonly IDatabase _database;
    private ComboBox _sel_kind = null;
    private TextBox _txt_equip = null;
    private ComboBox _sel_status = null;
    private TextBox _txt_note = null;
    private Button _btn_next = null;
    private Button _btn_save = null;
    private Button _btn_back = null;
    private List<Transaction> transactions = new();
    private string _mem_kit = null;
    private bool forceInsertDiffEmployer = false;
    private bool forceInsertDiffKit = false;
    private bool forceInsertTransaction = false;
    public EntryScreen(IDatabase database)
    {
        if (Hodor.From == 0 || Hodor.To == 0)
            throw new InvalidOperationException(
                Helpers.Resources.GetString("ENTRY_SCREEN_AUTH_FAIL"));
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
            Content = Helpers.Resources.GetString("ENTRY_SCREEN_HEADER_TXT"),
        };

        _sel_kind = new ComboBox { ItemsSource = Enum.GetValues<Transaction.KindEnum>(), Height = 40 };

        _txt_equip = new TextBox { Text = Helpers.Resources.GetString("ENTRY_SCREEN_EQUIP_PLACEHOLDER"), AcceptsReturn = false, TextWrapping = TextWrapping.NoWrap, Height = 40 };

        _sel_status = new ComboBox { ItemsSource = Enum.GetValues<Equipment.StatusEnum>(), SelectedItem = Equipment.StatusEnum.Ok, Height = 40 };

        _txt_note = new TextBox { Text = Helpers.Resources.GetString("ENTRY_SCREEN_NOTE_TXT"), AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, Height = 80 };
    
        _btn_next = new Button { Content = Helpers.Resources.GetString("ENTRY_SCREEN_NEXT_BTN"), Height = 40 };
    
        _btn_save = new Button { Content = Helpers.Resources.GetString("GLOBAL_SCREEN_SAVE_BTN"), Height = 40, IsEnabled = false };

        _btn_back = new Button { Content = Helpers.Resources.GetString("GLOBAL_SCREEN_BACK_BTN"), Height = 40 };
        
        stack.Children.Add(_lbl_header);
        stack.Children.Add(_sel_kind);
        stack.Children.Add(_txt_equip);
        stack.Children.Add(_sel_status);
        stack.Children.Add(_txt_note);
        stack.Children.Add(_btn_next);
        stack.Children.Add(_btn_save);
        stack.Children.Add(_btn_back);

        Content = stack;
    }
    private void ClearInputs()
    {
        _txt_equip.Text = Helpers.Resources.GetString("ENTRY_SCREEN_EQUIP_PLACEHOLDER");
        _sel_status.SelectedValue = null;
        _txt_note.Text = Helpers.Resources.GetString("ENTRY_SCREEN_NOTE_TXT");
    }
    private void AddTransaction()
    {
        try
        {
            if (_sel_kind.SelectedItem is null)
                throw new InvalidOperationException(
                    Helpers.Resources.GetString("ENTRY_SCREEN_KIND_400"));
            if (_sel_status.SelectedItem is null)
                throw new InvalidOperationException(
                    Helpers.Resources.GetString("ENTRY_SCREEN_STATUS_400"));
            if (!long.TryParse(_txt_equip.Text, out long idEquipment))
                throw new InvalidOperationException(
                    Helpers.Resources.GetString("ENTRY_SCREEN_EQUIP_400", _txt_equip.Text));
            var equipment = _database.GetEquipment(idEquipment) ??
                throw new InvalidOperationException(
                    Helpers.Resources.GetString("ENTRY_SCREEN_EQUIP_404"));
            equipment.Status = (Equipment.StatusEnum)_sel_status.SelectedItem;

            _mem_kit ??= equipment.Kit;
            if (equipment.Kit != _mem_kit)
            {
                if (!forceInsertDiffKit)
                {
                    var result = MessageBox.Show(
                        Helpers.Resources.GetString("ENTRY_SCREEN_EQUIP_KIT", _mem_kit, equipment.Kit),
                            null, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        forceInsertDiffKit = true;
                        AddTransaction();
                        return;
                    }
                    throw new InvalidOperationException(Helpers.Resources.GetString("ENTRY_SCREEN_NO_FORCE"));
                }
            }

            var last_tx = _database.GetTransaction(idEquipment);
            if (last_tx is not null && last_tx.IdEmployerTo != Hodor.To &&
                (Transaction.KindEnum)_sel_kind.SelectedValue == Transaction.KindEnum.Checkout)
            {
                if (!forceInsertDiffEmployer)
                {
                    var currentEmployer = _database.GetEmployer(Hodor.To) ??
                        throw new InvalidOperationException();
                    var previousEmployer = _database.GetEmployer(last_tx.IdEmployerTo) ??
                        throw new InvalidOperationException();

                    var result = MessageBox.Show(Helpers.Resources.GetString("ENTRY_SCREEN_DIFF_EMPLOYER",
                        $"{currentEmployer.Registry} - {currentEmployer.FullName}",
                        $"{previousEmployer.Registry} - {previousEmployer.FullName}"),
                            null, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        forceInsertDiffEmployer = true;
                        AddTransaction();
                        return;
                    }
                    throw new InvalidOperationException(Helpers.Resources.GetString("ENTRY_SCREEN_NO_FORCE"));
                }
            }

            var transaction = new Transaction
            (
                timestamp: DateTime.Now,
                idEquipment: equipment.Id,
                kind: (Transaction.KindEnum)_sel_kind.SelectedItem,
                idEmployerFrom: Hodor.From,
                idEmployerTo: Hodor.To,
                equipment: equipment,
                note: (_txt_note.Text != Helpers.Resources.GetString("ENTRY_SCREEN_NOTE_TXT")) ? _txt_note.Text : null
            );

            try
            {
                Transaction.CanBeTransacted(last_tx, transaction);
            }
            catch (InvalidOperationException error)
            {
                if (!forceInsertTransaction)
                {
                    var result = MessageBox.Show(Helpers.Resources.GetString("ENTRY_SCREEN_DIFF_TRANSACTION",
                        error.Message), null, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        forceInsertTransaction = true;
                        AddTransaction();
                        return;
                    }
                    throw new InvalidOperationException(Helpers.Resources.GetString("ENTRY_SCREEN_NO_FORCE"));
                }
            }

            transactions.Add(transaction);
            _sel_kind.IsEnabled = false;
            _btn_save.IsEnabled = true;
            ClearInputs();
        }
        catch ( Exception ex )
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void SaveTransactions()
    {
        try
        {
            if (transactions.Count == 0)
                throw new InvalidOperationException(Helpers.Resources.GetString("ENTRY_SCREEN_SAVE_EMPTY"));
            if (transactions.Count < 1)
                throw new InvalidOperationException(Helpers.Resources.GetString("MODEL_TRANSACTION_EMPTY_KIT"));
            if (transactions.Count > 5)
                throw new InvalidOperationException(Helpers.Resources.GetString("MODEL_TRANSACTION_KIT_OVERFLOW"));
            if (transactions.Select(t => t.Equipment.Kind).Distinct().Count() != transactions.Count)
                throw new InvalidOperationException(Helpers.Resources.GetString("MODEL_TRANSACTION_MULTI_KINDS"));

            var expected_kit = _database.GetTransactions().Where(t => t.Equipment.Kit == _mem_kit).ToList();

            var current_ids = transactions.Select(t => t.Equipment.Id).OrderBy(x => x).ToList();
            var expected_ids = expected_kit.Select(t => t.Equipment.Id).OrderBy(x => x).ToList();

            var less = expected_ids.Except(current_ids).ToList();
            var more = current_ids.Except(expected_ids).ToList();

            if (less.Any() || more.Any())
            {
                if (!forceInsertDiffKit)
                {
                    var message = new System.Text.StringBuilder();
                    message.AppendLine(
                        Helpers.Resources.GetString("ENTRY_SCREEN_STATUS_HEAD"));

                    // For each expected equipment, print line with Missing or OK
                    foreach (var expected in expected_kit)
                    {
                        message.Append(expected.Equipment.Id.ToString("D12"));
                        message.Append(", ");
                        message.Append(expected.Equipment.Kind.ToString().PadLeft(12));
                        message.Append(", ");
                        message.AppendLine(less.Contains(expected.Equipment.Id) ?
                            Helpers.Resources.GetString("ENTRY_SCREEN_STATUS_MISS") :
                            Helpers.Resources.GetString("ENTRY_SCREEN_STATUS_FINE"));
                    }

                    // Now print any extra equipments that are in current but not expected
                    foreach (var extraId in more)
            {
                        var tx = transactions.Single(t => t.Equipment.Id == extraId);
                        message.Append(tx.Equipment.Id.ToString("D12"));
                        message.Append(", ");
                        message.Append(tx.Equipment.Kind.ToString().PadLeft(12));
                        message.Append(", ");
                        message.AppendLine(
                            Helpers.Resources.GetString("ENTRY_SCREEN_STATUS_MORE"));
                    }

                    var result = MessageBox.Show(Helpers.Resources.GetString("ENTRY_SCREEN_DIFF_KITS",
                        message.ToString()), null, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                {
                        forceInsertDiffKit = true;
                    SaveTransactions();
                        return;
                }
                throw new InvalidOperationException(Helpers.Resources.GetString("ENTRY_SCREEN_NO_FORCE"));
            }
            }
            foreach (var transaction in transactions)
            {
                _database.AddTransaction(transaction);
            }
            // Reset all values to default
            Hodor.To = 0;
            ClearInputs();
            forceInsertTransaction = false;
            forceInsertDiffKit = false;
            forceInsertDiffEmployer = false;
            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    private void HandleEvent()
    {
        _btn_next.Click += (_, _) => AddTransaction();
        _btn_save.Click += (_, _) => SaveTransactions();
        _btn_back.Click += (_, _) =>
        {
            Hodor.To = 0;
            ClearInputs();
            transactions.Clear();
            _sel_kind.IsEnabled = true;
            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        };
    }
}