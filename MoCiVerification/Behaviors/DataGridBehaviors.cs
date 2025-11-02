using Avalonia;
using Avalonia.Controls;
using System.Collections;
using Avalonia.Data;

namespace MoCiVerification.Behaviors;

public class DataGridBehaviors
{
    public static readonly AttachedProperty<IList?> SelectedItemsProperty =
        AvaloniaProperty.RegisterAttached<DataGridBehaviors, DataGrid, IList?>(
            "SelectedItems", defaultBindingMode: BindingMode.TwoWay);

    static DataGridBehaviors()
    {
        SelectedItemsProperty.Changed.AddClassHandler<DataGrid>(OnSelectedItemsChanged);
    }

    public static IList? GetSelectedItems(AvaloniaObject obj) =>
        obj.GetValue(SelectedItemsProperty);

    public static void SetSelectedItems(AvaloniaObject obj, IList? value) =>
        obj.SetValue(SelectedItemsProperty, value);

    private static void OnSelectedItemsChanged(DataGrid dataGrid, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not IList viewModelCollection)
            return;
        dataGrid.SelectionChanged += (s, args) =>
        {
            viewModelCollection.Clear();
            foreach (var item in dataGrid.SelectedItems)
            {
                viewModelCollection.Add(item);
            }
        };
    }
}