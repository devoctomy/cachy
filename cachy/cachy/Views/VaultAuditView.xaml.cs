using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(VaultAuditViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VaultAuditView : PageNavigationAwareView
    {

        #region constructor / destructor

        public VaultAuditView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}