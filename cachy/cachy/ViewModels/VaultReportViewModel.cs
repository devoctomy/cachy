using cachy.Config;
using cachy.Controls;
using cachy.Data;
using cachy.Fonts;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class VaultReportViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private VaultReportView _view;

        private Vault _vault;
        private string _reportSummaryText = string.Empty;
        private IEnumerable<Credential> _reusedPasswords;
        private IEnumerable<Credential> _knownPasswords;
        private IEnumerable<Credential> _weakPasswords;
        private IEnumerable<Credential> _oldPasswords;

        private ICommand _backCommand;
        private ICommand _fixCommand;
        private ICommand _showInVault;

        #endregion

        #region public properties

        public VaultReportView View
        {
            get
            {
                return (_view);
            }
        }

        public Vault Vault
        {
            get
            {
                return (_vault);
            }
        }

        public Color ReportResultGlyphColour
        {
            get
            {
                if (HasReusedPasswords || HasKnownPasswords || HasWeakPasswords || HasOldPasswords)
                {
                    return (Color.Red);
                }
                else
                {
                    return (Color.Green);
                }
            }
        }

        public string ReportResultGlyphText
        {
            get
            {
                if (HasReusedPasswords || HasKnownPasswords || HasWeakPasswords || HasOldPasswords)
                {
                    return(CachyFont.GetString(CachyFont.Glyph.Warning_Message));
                }
                else
                {
                    return(CachyFont.GetString(CachyFont.Glyph.Check));
                }
            }
        }

        public string ReportSummaryText
        {
            get
            {
                return (_reportSummaryText);
            }
            set
            {
                if(_reportSummaryText != value)
                {
                    _reportSummaryText = value;
                    NotifyPropertyChanged("ReportSummaryText");
                }
            }
        }

        public double ReusedPasswordsHeight
        {
            get
            {
                return (ReusedPasswords.ToArray().Length * 60);
            }
        }

        public IEnumerable<Credential> ReusedPasswords
        {
            get
            {
                return (_reusedPasswords);
            }
        }

        public bool HasReusedPasswords
        {
            get
            {
                return (ReusedPasswords.Any());
            }
        }

        public double KnownPasswordsHeight
        {
            get
            {
                return (KnownPasswords.ToArray().Length * 60);
            }
        }

        public IEnumerable<Credential> KnownPasswords
        {
            get
            {
                return (_knownPasswords);
            }
        }

        public bool HasKnownPasswords
        {
            get
            {
                return (KnownPasswords.Any());
            }
        }

        public double WeakPasswordsHeight
        {
            get
            {
                return (WeakPasswords.ToArray().Length * 60);
            }
        }

        public IEnumerable<Credential> WeakPasswords
        {
            get
            {
                return (_weakPasswords);
            }
        }

        public bool HasWeakPasswords
        {
            get
            {
                return (WeakPasswords.Any());
            }
        }

        public double OldPasswordsHeight
        {
            get
            {
                return (OldPasswords.ToArray().Length * 60);
            }
        }

        public IEnumerable<Credential> OldPasswords
        {
            get
            {
                return (_oldPasswords);
            }
        }

        public bool HasOldPasswords
        {
            get
            {
                return (OldPasswords.Any());
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        public ICommand FixCommand
        {
            get
            {
                return (_fixCommand);
            }
        }

        public ICommand ShowInVault
        {
            get
            {
                return (_showInVault);
            }
        }

        #endregion

        #region constructor / destructor

        public VaultReportViewModel(VaultReportView view)
        {
            _view = view;
            _reusedPasswords = new ObservableCollection<Credential>();
            _knownPasswords = new ObservableCollection<Credential>();
            _weakPasswords = new ObservableCollection<Credential>();
            _oldPasswords = new ObservableCollection<Credential>();
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _fixCommand= new Command(new Action<Object>(FixCommandAction));
            _showInVault= new Command(new Action<Object>(ShowInVaultAction));
        }

        #endregion

        #region private methods

        private void CreateReport()
        {
            _reusedPasswords = Vault.Credentials.Where(cred => cred.IsReused());
            _knownPasswords = Vault.Credentials.Where(cred => cred.IsKnown());
            _weakPasswords = Vault.Credentials.Where(cred => cred.IsWeak());
            _oldPasswords = Vault.Credentials.Where(cred => cred.IsOlderThan(new TimeSpan(AppConfig.Instance.DaysForOld, 0, 0, 0)));

            ExpandableView reusedView = View.FindByName<ExpandableView>("ReusedView");
            if (reusedView != null) reusedView.IsExpanded = false;
            ExpandableView knownView = View.FindByName<ExpandableView>("KnownView");
            if (knownView != null) knownView.IsExpanded = false;
            ExpandableView weakView = View.FindByName<ExpandableView>("WeakView");
            if (weakView != null) weakView.IsExpanded = false;
            ExpandableView oldView = View.FindByName<ExpandableView>("OldView");
            if (oldView != null) oldView.IsExpanded = false;

            NotifyPropertyChanged("HasReusedPasswords");
            NotifyPropertyChanged("HasKnownPasswords");
            NotifyPropertyChanged("HasWeakPasswords");
            NotifyPropertyChanged("ReusedPasswordsHeight");
            NotifyPropertyChanged("KnownPasswordsHeight");
            NotifyPropertyChanged("WeakPasswordsHeight");
            NotifyPropertyChanged("ReportResultGlyphColour");
            NotifyPropertyChanged("ReportResultGlyphText");
            NotifyPropertyChanged("ReusedPasswords");
            NotifyPropertyChanged("KnownPasswords");
            NotifyPropertyChanged("WeakPasswords");
            NotifyPropertyChanged("OldPasswords");

            if (HasReusedPasswords || HasKnownPasswords || HasWeakPasswords || HasOldPasswords)
            {
                ReportSummaryText = "Your vault contains credentials that cachy considers to be at risk.  Please review the points of concern below.";
            }
            else
            {
                ReportSummaryText = "Your vault appears to be in good standing, with no immediate conerns.";
            }

            View.ForceLayout();
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch (key)
            {
                case "Vault":
                    {
                        _vault = (Vault)parameter;
                        CreateReport();
                        break;
                    }
            }
        }

        public void SetParameters(params KeyValuePair<String, Object>[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                foreach (KeyValuePair<String, Object> curParameter in parameters)
                {
                    SetParameter(curParameter.Key,
                        curParameter.Value);
                }
            }
        }

        public void OnClosePopup(View item, object parameter)
        {

        }

        public void OnNavigateTo(View view, object parameter)
        {

        }

        public void OnNavigateFrom(View view, object parameter)
        {

        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region commands

        public void BackCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(null);
        }

        public void FixCommandAction(object parameter)
        {
            Credential credential = (Credential)parameter;
            Credential credentialCopy = credential.Clone(true);
            credentialCopy.BlockVaultDirty = true;
            App.Controller.NavigateTo("crededit",
                new KeyValuePair<String, Object>("Credential", credentialCopy),
                new KeyValuePair<String, Object>("Mode", CreateCredentialViewModel.EditorMode.Edit));
        }

        public void ShowInVaultAction(Object parameter)
        {
            App.Controller.ClosePopup(parameter);
        }

        #endregion

    }

}
