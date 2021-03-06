﻿namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Threading;

    [ContentProperty("Items")]
    public class DiagramControl : ListBox, IDiagram
    {
        bool isLoaded;

        public static string ItemHostPart = "ItemsHost";
        public static string DiagramSurfacePart = "DiagramSurface";

        public static readonly DependencyProperty LayoutManagerProperty = DependencyProperty.Register("LayoutManager", typeof(ILayoutManager), typeof(DiagramControl), new PropertyMetadata());

        static DiagramControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramControl), new FrameworkPropertyMetadata(typeof(DiagramControl)));
        }

        public DiagramControl()
        {
            LayoutManager = new SequenceDiagramLayoutManager();
            Loaded += (sender, args) => OnControlLoaded();
            IsVisibleChanged += (sender, args) => PerformLayout();
            SizeChanged += (s, a) => PerformLayout();
        }

        public ILayoutManager LayoutManager
        {
            get { return (ILayoutManager)GetValue(LayoutManagerProperty); }
            set { SetValue(LayoutManagerProperty, value); }
        }

        public DiagramItemCollection DiagramItems => ItemsSource as DiagramItemCollection;

        public DiagramVisualItem GetItemFromContainer(DiagramItem item)
        {
            if (item == null)
            {
                return null;
            }

            return (DiagramVisualItem)ItemContainerGenerator.ContainerFromItem(item);
        }

        public void BringIntoView(DiagramItem item)
        {
            var visual = GetItemFromContainer(item);
            visual?.BringIntoView();
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is DiagramVisualItem;

        protected override DependencyObject GetContainerForItemOverride() => new DiagramVisualItem();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(PerformLayout));
        }

        void OnControlLoaded()
        {
            isLoaded = true;
        }

        void PerformLayout()
        {
            if (isLoaded && IsVisible)
            {
                LayoutManager.PerformLayout(this);
            }
        }
    }
}