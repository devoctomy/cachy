using cachy.Navigation.BurgerMenu;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace cachy.ViewModels.Interfaces
{

    public interface IPageNavigationAware
    {

        #region methods

        void SetParameter(string key, object parameter);

        void SetParameters(params KeyValuePair<string, object>[] parameters);

        void OnClosePopup(View item, object parameter);

        void OnNavigateTo(View view, object parameter);

        void OnNavigateFrom(View view, object parameter);

        void OnGoBack(View view, object parameter);

        #endregion

    }

}
