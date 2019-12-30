using cachy.Fonts;
using cachy.ViewModels.Attributes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.Navigation.BurgerMenu
{

    public class BurgerMenuCommandItem : BurgerMenuItem
    {

        #region private objects

        private BurgerMenuViewItem _parentViewItem;
        private string _methodName = String.Empty;

        #endregion

        #region public properties

        public BurgerMenuViewItem ParentViewItem
        {
            get
            {
                return (_parentViewItem);
            }
        }

        public string MethodName
        {
            get
            {
                return (_methodName);
            }
        }

        #endregion

        #region constructor / destructor

        public BurgerMenuCommandItem(string key,
            string menuTitle,
            CachyFont.Glyph glyph,
            BurgerMenuViewItem parentViewItem,
            string methodName) :
            base(key, menuTitle, glyph, true)
        {
            _parentViewItem = parentViewItem;
            _methodName = methodName;
        }

        #endregion

        #region public methods

        public void Invoke(object instance,
            params object[] parameter)
        {
            if (instance == null) throw new ArgumentException("Argument must not be null.", "instance");
            ViewModelMappingAttribute mappingAttribute = ViewModelMappingAttribute.GetFromType(ParentViewItem.PageViewType);
            Type viewModelType = mappingAttribute.ViewModelType;
            if (instance.GetType() != viewModelType) throw new ArgumentException(String.Format("Argument must be of type '{0}'.", ParentViewItem.PageViewType.Name), "instance");

            MethodInfo method = viewModelType.GetMethod(MethodName);
            if(method != null)
            {
                method.Invoke(instance, parameter);
            }
            else
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Error, "Could not find the method named '{0}' for the type '{1}'", MethodName, viewModelType.Name);
            }
        }

        #endregion

    }

}
