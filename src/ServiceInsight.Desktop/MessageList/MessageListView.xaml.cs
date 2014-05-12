﻿namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using DevExpress.Data;
    using DevExpress.Xpf.Core;
    using DevExpress.Xpf.Core.Native;
    using DevExpress.Xpf.Grid;
    using ExtensionMethods;

    public interface IMessageListView
    {
    }

    public partial class MessageListView : IMessageListView
    {
        static class UnboundColumns
        {
            public const string IsFaulted = "IsFaulted";
        }

        PropertyInfo sortUpProperty;
        PropertyInfo sortDownProperty;

        public MessageListView()
        {
            InitializeComponent();
            sortUpProperty = typeof(BaseGridColumnHeader).GetProperty("SortUpIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
            sortDownProperty = typeof(BaseGridColumnHeader).GetProperty("SortDownIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        IMessageListViewModel Model
        {
            get { return (IMessageListViewModel)DataContext; }
        }

        void OnRequestAdvancedMessageData(object sender, GridColumnDataEventArgs e)
        {
            var msg = Model.Rows[e.ListSourceRowIndex];

            if (e.IsGetData && e.Column.FieldName == UnboundColumns.IsFaulted)
            {
                e.Value = Model.GetMessageErrorInfo(msg);
            }
        }

        void OnBeforeLayoutRefresh(object sender, CancelRoutedEventArgs e)
        {
            e.Cancel = grid.ShowLoadingPanel;
        }

        void SortData(ColumnBase column, ColumnSortOrder order)
        {
            Model.RefreshMessages(column.Tag as string, order == ColumnSortOrder.Ascending);
        }

        void OnGridControlClicked(object sender, MouseButtonEventArgs e)
        {
            var columnHeader = LayoutHelper.FindLayoutOrVisualParentObject((DependencyObject)e.OriginalSource, typeof(GridColumnHeader)) as GridColumnHeader;
            if (columnHeader == null || Model == null || Model.WorkInProgress) return;

            var clickedColumn = (GridColumn)columnHeader.DataContext;
            if (clickedColumn.Tag == null) return;

            ClearSortExcept(columnHeader);

            var sortUpControl = (ColumnHeaderSortIndicatorControl)sortUpProperty.GetValue(columnHeader, null);
            var sortDownControl = (ColumnHeaderSortIndicatorControl)sortDownProperty.GetValue(columnHeader, null);
            ColumnSortOrder sort;

            if (sortUpControl.Visibility != Visibility.Visible)
            {
                sortUpControl.Visibility = Visibility.Visible;
                sortDownControl.Visibility = Visibility.Hidden;
                sort = ColumnSortOrder.Ascending;
            }
            else
            {
                sortUpControl.Visibility = Visibility.Hidden;
                sortDownControl.Visibility = Visibility.Visible;
                sort = ColumnSortOrder.Descending;
            }

            SortData(clickedColumn, sort);
        }

        void HideIndicator(BaseGridColumnHeader header)
        {
            var sortUpControl = (ColumnHeaderSortIndicatorControl)sortUpProperty.GetValue(header, null);
            var sortDownControl = (ColumnHeaderSortIndicatorControl)sortDownProperty.GetValue(header, null);

            sortUpControl.Visibility = Visibility.Hidden;
            sortDownControl.Visibility = Visibility.Hidden;
        }

        void ClearSortExcept(GridColumnHeader clickedHeader)
        {
            var headers = grid.FindVisualChildren<GridColumnHeader>();
            foreach (var header in headers)
            {
                if (header == clickedHeader)
                    continue;

                HideIndicator(header);
            }
        }

        DataController Controller
        {
            get { return grid.DataController; }
        }

        public void BeginSelection()
        {
            Controller.Selection.BeginSelection();
        }

        public void EndSelection()
        {
            Controller.Selection.EndSelection();
        }
    }
}
