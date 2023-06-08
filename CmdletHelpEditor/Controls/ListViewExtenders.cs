using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
// source: https://michlg.wordpress.com/2010/01/17/listbox-automatically-scroll-to-bottom/

namespace CmdletHelpEditor.Controls {
    /// <summary>
    /// This class contains a few useful extenders for the ListBox
    /// </summary>
    public class ListViewExtenders : DependencyObject {
        public static readonly DependencyProperty AutoScrollToEndProperty = DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(Boolean), typeof(ListViewExtenders), new UIPropertyMetadata(default(Boolean), OnAutoScrollToEndChanged));

        /// <summary>
        /// Returns the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object which value should be returned</param>
        /// <returns>The value of the given property</returns>
        public static Boolean GetAutoScrollToEnd(DependencyObject obj) {
            return (Boolean)obj.GetValue(AutoScrollToEndProperty);
        }

        /// <summary>
        /// Sets the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object which value should be set</param>
        /// <param name="value">The value which should be assigned to the AutoScrollToEndProperty</param>
        public static void SetAutoScrollToEnd(DependencyObject obj, Boolean value) {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        /// <summary>
        /// This method will be called when the AutoScrollToEnd
        /// property was changed
        /// </summary>
        /// <param name="s">The sender (the ListBox)</param>
        /// <param name="e">Some additional information</param>
        public static void OnAutoScrollToEndChanged(DependencyObject s, DependencyPropertyChangedEventArgs e) {
            if (!(s is ListView listView)) { return; }
            ItemCollection listViewItems = listView.Items;
            INotifyCollectionChanged data = listViewItems.SourceCollection as INotifyCollectionChanged;

            var scrollToEndHandler = new NotifyCollectionChangedEventHandler((s1, e1) => {
                if (listView.Items.Count > 0) {
                    Object lastItem = listView.Items[listView.Items.Count - 1];
                    listViewItems.MoveCurrentTo(lastItem);
                    listView.ScrollIntoView(lastItem);
                }
            });
            if (data == null) { return; }
            if ((Boolean)e.NewValue) {
                data.CollectionChanged += scrollToEndHandler;
            } else {
                data.CollectionChanged -= scrollToEndHandler;
            }

        }
    }
}
