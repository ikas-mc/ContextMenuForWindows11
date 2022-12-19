using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ContextMenuCustomApp.View.Controls
{
    [TemplatePart(Name = PartIconPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PartDescriptionPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = RightContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = ActionContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = BottomContentPresenter, Type = typeof(ContentPresenter))]
    public class SettingItem : Control
    {
        private const string PartIconPresenter = "IconPresenter";
        private const string PartDescriptionPresenter = "DescriptionPresenter";
        private const string RightContentPresenter = "RightContentPresenter";
        private const string ActionContentPresenter = "ActionContentPresenter";
        private const string BottomContentPresenter = "BottomContentPresenter";

        private ContentPresenter _iconPresenter;
        private ContentPresenter _descriptionPresenter;
        private ContentPresenter _rightContentPresenter;
        private ContentPresenter _actionContentPresenter;
        private ContentPresenter _bottomContentPresenter;
        private SettingItem _setting;

        public SettingItem()
        {
            this.DefaultStyleKey = typeof(SettingItem);
        }


        protected override void OnApplyTemplate()
        {
            _setting = (SettingItem)this;
            _iconPresenter = (ContentPresenter)_setting.GetTemplateChild(PartIconPresenter);
            _descriptionPresenter = (ContentPresenter)_setting.GetTemplateChild(PartDescriptionPresenter);
            _rightContentPresenter = (ContentPresenter)_setting.GetTemplateChild(RightContentPresenter);
            _actionContentPresenter = (ContentPresenter)_setting.GetTemplateChild(ActionContentPresenter);
            _bottomContentPresenter = (ContentPresenter)_setting.GetTemplateChild(BottomContentPresenter);
            Update();
            base.OnApplyTemplate();
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
           "Header",
           typeof(string),
           typeof(SettingItem),
           new PropertyMetadata(default(string), OnHeaderChanged));

        public object Description
        {
            get => (object)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(object),
            typeof(SettingItem),
            new PropertyMetadata(null, OnDescriptionChanged));

        public object Icon
        {
            get => (object)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(object),
            typeof(SettingItem),
            new PropertyMetadata(default(string), OnIconChanged));

        public object RightContent
        {
            get => (object)GetValue(RightContentProperty);
            set => SetValue(RightContentProperty, value);
        }

        public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register(
            "RightContent",
            typeof(object),
            typeof(SettingItem),
            new PropertyMetadata(null, OnRightContentChanged));
        public object ActionContent
        {
            get => (object)GetValue(ActionContentProperty);
            set => SetValue(ActionContentProperty, value);
        }
        public static readonly DependencyProperty ActionContentProperty = DependencyProperty.Register(
            "ActionContent",
            typeof(object),
            typeof(SettingItem),
            new PropertyMetadata(null, OnActionContentChanged));


        public object BottomContent
        {
            get => (object)GetValue(BottomContentProperty);
            set => SetValue(BottomContentProperty, value);
        }

        public static readonly DependencyProperty BottomContentProperty = DependencyProperty.Register(
            "BottomContent",
            typeof(object),
            typeof(SettingItem),
            new PropertyMetadata(null, OnBottomContentChanged));

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingItem)d).Update();
        }

        private static void OnRightContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingItem)d).Update();
        }

        private static void OnActionContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingItem)d).Update();
        }


        private static void OnBottomContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingItem)d).Update();
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingItem)d).Update();
        }

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingItem)d).Update();
        }

        private void Update()
        {
            if (_setting == null)
            {
                return;
            }

            if (_setting.Icon == null)
            {
                _setting._iconPresenter.Visibility = Visibility.Collapsed;
            }
            else
            {
                _setting._iconPresenter.Visibility = Visibility.Visible;
            }

            if (_setting.Description == null)
            {
                _setting._descriptionPresenter.Visibility = Visibility.Collapsed;
            }
            else
            {
                _setting._descriptionPresenter.Visibility = Visibility.Visible;
            }


            if (_setting.RightContent == null)
            {
                _setting._rightContentPresenter.Visibility = Visibility.Collapsed;
            }
            else
            {
                _setting._rightContentPresenter.Visibility = Visibility.Visible;
            }


            if (_setting.ActionContent == null)
            {
                _setting._actionContentPresenter.Visibility = Visibility.Collapsed;
            }
            else
            {
                _setting._actionContentPresenter.Visibility = Visibility.Visible;
            }


            if (_setting.BottomContent == null)
            {
                _setting._bottomContentPresenter.Visibility = Visibility.Collapsed;
            }
            else
            {
                _setting._bottomContentPresenter.Visibility = Visibility.Visible;
            }
        }
    }
}
