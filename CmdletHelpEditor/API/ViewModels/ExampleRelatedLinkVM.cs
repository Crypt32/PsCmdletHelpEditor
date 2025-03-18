#nullable enable
using System;
using System.Collections.Generic;
using System.Windows.Input;
using PsCmdletHelpEditor.Core.Utils;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;

public abstract class ExampleRelatedLinkVM<T> : ViewModelBase {
    Boolean nameTextBoxEnabled;
    T? selectedItem;
    IList<T>? innerList;

    protected ExampleRelatedLinkVM() {
        NewItemCommand = new RelayCommand(newItem, canNewItem);
        RemoveItemCommand = new RelayCommand(removeItem, canRemoveItem);
        UpItemCommand = new RelayCommand(upItem, canUpItem);
        DownItemCommand = new RelayCommand(downItem, canDownItem);
    }

    public ICommand NewItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand UpItemCommand { get; }
    public ICommand DownItemCommand { get; }

    protected Int32 Count => innerList?.Count ?? -1;

    public Boolean NameTextBoxEnabled {
        get => nameTextBoxEnabled;
        set {
            nameTextBoxEnabled = value;
            OnPropertyChanged();
        }
    }
    public T? SelectedItem {
        get => selectedItem;
        set {
            selectedItem = value;
            NameTextBoxEnabled = selectedItem != null;
            OnPropertyChanged();
        }
    }

    protected abstract T CreateNewItem();

    void newItem(Object? obj) {
        var newItem = CreateNewItem();
        innerList!.Add(newItem);
        SelectedItem = newItem;
    }
    Boolean canNewItem(Object? obj) {
        return innerList is not null;
    }
    void removeItem(Object? obj) {
        Int32 index = innerList!.IndexOf(SelectedItem);
        innerList.Remove(SelectedItem);
        if (index > 0) {
            SelectedItem = innerList[index - 1];
        }
    }
    Boolean canRemoveItem(Object? obj) {
        return SelectedItem != null;
    }
    void upItem(Object? obj) {
        SelectedItem = innerList!.MoveUp(SelectedItem);
    }
    Boolean canUpItem(Object? obj) {
        return canRemoveItem(null) && innerList!.IndexOf(SelectedItem) > 0;
    }
    void downItem(Object? obj) {
        SelectedItem = innerList!.MoveDown(SelectedItem);
    }
    Boolean canDownItem(Object? obj) {
        if (!canNewItem(null)) {
            return false;
        }

        Int32 count = innerList!.Count - 1;
        return canRemoveItem(null) && innerList.IndexOf(SelectedItem) < count;
    }

    protected void OnCmdletSet(IList<T>? list) {
        innerList = list;
        SelectedItem = default;
    }
}