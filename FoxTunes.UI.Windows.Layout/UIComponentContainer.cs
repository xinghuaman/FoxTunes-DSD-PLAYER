﻿using FoxTunes.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FoxTunes
{
    public class UIComponentContainer : DockPanel, IInvocableComponent, IUIComponent, IValueConverter
    {
        public const string CLEAR = "YYYY";

        public const string EXIT = "ZZZZ";

        public static readonly LayoutDesignerBehaviour Behaviour = ComponentRegistry.Instance.GetComponent<LayoutDesignerBehaviour>();

        public static readonly UIComponentFactory Factory = ComponentRegistry.Instance.GetComponent<UIComponentFactory>();

        protected static ILogger Logger
        {
            get
            {
                return LogManager.Logger;
            }
        }

        public static readonly DependencyProperty ComponentProperty = DependencyProperty.Register(
            "Component",
            typeof(UIComponentConfiguration),
            typeof(UIComponentContainer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnComponentChanged))
       );

        public static UIComponentConfiguration GetComponent(UIComponentContainer source)
        {
            return (UIComponentConfiguration)source.GetValue(ComponentProperty);
        }

        public static void SetComponent(UIComponentContainer source, UIComponentConfiguration value)
        {
            source.SetValue(ComponentProperty, value);
        }

        public static void OnComponentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var componentContainer = sender as UIComponentContainer;
            if (componentContainer == null)
            {
                return;
            }
            if (e.OldValue is UIComponentConfiguration oldComponent && e.NewValue is UIComponentConfiguration newComponent)
            {
                //Preserve alignment and such.
                foreach (var pair in oldComponent.MetaData)
                {
                    newComponent.MetaData.TryAdd(pair.Key, pair.Value);
                }
            }
            componentContainer.OnComponentChanged();
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(UIComponentBase),
            typeof(UIComponentContainer),
            new PropertyMetadata(new PropertyChangedCallback(OnContentChanged))
       );

        public static UIComponentBase GetContent(UIComponentContainer source)
        {
            return (UIComponentBase)source.GetValue(ContentProperty);
        }

        public static void SetContent(UIComponentContainer source, UIComponentBase value)
        {
            source.SetValue(ContentProperty, value);
        }

        public static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var container = sender as UIComponentContainer;
            if (container == null)
            {
                return;
            }
            container.OnContentChanged();
        }

        public UIComponentContainer()
        {
            this.InitializeComponent();
        }

        public ContentControl ContentControl { get; private set; }

        public Rectangle Rectangle { get; private set; }

        public UIComponentConfiguration Component
        {
            get
            {
                return GetComponent(this);
            }
            set
            {
                SetComponent(this, value);
            }
        }

        protected virtual void OnComponentChanged()
        {
            if (this.ComponentChanged != null)
            {
                this.ComponentChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler ComponentChanged;

        public UIComponentBase Content
        {
            get
            {
                return this.GetValue(ContentProperty) as UIComponentBase;
            }
            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        protected virtual void OnContentChanged()
        {
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("Content");
        }

        public event EventHandler ContentChanged;

        protected virtual void InitializeComponent()
        {
            this.Rectangle = new Rectangle();
            this.Rectangle.Fill = Brushes.Transparent;
            this.ContentControl = new ContentControl();
            this.Children.Add(this.Rectangle);
            this.Children.Add(this.ContentControl);
            this.Loaded += this.OnLoaded;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ContentControl.SetBinding(
                ContentControl.ContentProperty,
                new Binding()
                {
                    Path = new PropertyPath("Component"),
                    Source = this,
                    Converter = this
                }
            );
            this.Loaded -= this.OnLoaded;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.ContentControl.Content is FrameworkElement element)
            {
                UIDisposer.Dispose(element);
            }
            var configuration = value as UIComponentConfiguration;
            if (configuration == null || string.IsNullOrEmpty(configuration.Component))
            {
                return this.CreateSelector();
            }
            var component = default(UIComponentBase);
            var control = Factory.CreateControl(configuration, out component);
            if (control == null)
            {
                //If a plugin was uninstalled the control will be null.
                return this.CreateSelector();
            }
            this.Content = component;
            return control;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected virtual FrameworkElement CreateSelector()
        {
            var textBlock = new TextBlock();
            textBlock.Text = "Select Component";
            textBlock.IsHitTestVisible = false;
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(8, 0, 40, 0);
            textBlock.SetResourceReference(
                TextBlock.ForegroundProperty,
                "TextBrush"
            );

            var selector = new UIComponentSelector();
            selector.HorizontalAlignment = HorizontalAlignment.Stretch;
            selector.VerticalAlignment = VerticalAlignment.Center;
            selector.SetBinding(
                UIComponentSelector.ComponentProperty,
                new Binding()
                {
                    Source = this,
                    Path = new PropertyPath("Component")
                }
            );

            var grid = new Grid();
            grid.Children.Add(selector);
            grid.Children.Add(textBlock);

            return grid;
        }

        public void InitializeComponent(ICore core)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<string> InvocationCategories
        {
            get
            {
                yield return InvocationComponent.CATEGORY_GLOBAL;
            }
        }

        public virtual IEnumerable<IInvocationComponent> Invocations
        {
            get
            {
                yield return new InvocationComponent(InvocationComponent.CATEGORY_GLOBAL, CLEAR, Strings.UIComponentContainer_Clear, attributes: InvocationComponent.ATTRIBUTE_SEPARATOR);
                if (!Windows.Registrations.IsVisible(ToolWindowManagerWindow.ID))
                {
                    yield return new InvocationComponent(InvocationComponent.CATEGORY_GLOBAL, EXIT, Strings.UIComponentContainer_Exit);
                }
            }
        }

        public virtual Task InvokeAsync(IInvocationComponent component)
        {
            switch (component.Id)
            {
                case CLEAR:
                    return this.Clear();
                case EXIT:
                    return this.Exit();
            }
#if NET40
            return TaskEx.FromResult(false);
#else
            return Task.CompletedTask;
#endif
        }

        public Task Clear()
        {
            return Windows.Invoke(() => this.Component = new UIComponentConfiguration());
        }

        public Task Exit()
        {
            return Windows.Invoke(() => Behaviour.IsDesigning = false);
        }

        protected virtual void Dispatch(Func<Task> function)
        {
#if NET40
            var task = TaskEx.Run(function);
#else
            var task = Task.Run(function);
#endif
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging == null)
            {
                return;
            }
            this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
