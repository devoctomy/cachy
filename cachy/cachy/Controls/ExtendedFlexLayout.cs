using System;
using System.Collections;
using Xamarin.Forms;

namespace cachy.Controls
{

    public class ExtendedFlexLayout : FlexLayout
    {

        #region bindable properties

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ExtendedFlexLayout),
            propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(ExtendedFlexLayout),
            propertyChanged: OnItemTemplateChanged);

        #endregion

        #region public properties

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        #endregion

        #region bindable property change callbacks

        static void OnItemsSourceChanged(BindableObject bindable, object oldVal, object newVal)
        {
            ExtendedFlexLayout layout = (ExtendedFlexLayout)bindable;
            layout.RecreateItems();
        }

        private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ExtendedFlexLayout layout = (ExtendedFlexLayout)bindable;
            layout.RecreateItems();
        }

        #endregion

        #region private methods

        private void RecreateItems()
        {
            Children.Clear();
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    View child = CreateChildView(item);
                    if (child != null) Children.Add(child);
                }
            }
        }

        View CreateChildView(Object item)
        {
            if (ItemTemplate != null)
            {
                ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);
                View content = (View)ItemTemplate.CreateContent();
                return (content);
            }
            else
            {
                return (null);
            }
        }

        #endregion

    }

}
