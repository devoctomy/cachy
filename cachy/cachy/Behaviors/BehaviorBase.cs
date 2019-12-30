using System;
using Xamarin.Forms;

namespace cachy.Behaviors
{

	public class BehaviorBase<T> : Behavior<T> where T : BindableObject
	{

        #region public properties

        public T AssociatedObject { get; private set; }

        #endregion

        #region base class overrides

        protected override void OnAttachedTo (T bindable)
		{
			base.OnAttachedTo(bindable);
			AssociatedObject = bindable;
			if (bindable.BindingContext != null)
            {
				BindingContext = bindable.BindingContext;
			}
			bindable.BindingContextChanged += OnBindingContextChanged;
		}

		protected override void OnDetachingFrom (T bindable)
		{
			base.OnDetachingFrom(bindable);
			bindable.BindingContextChanged -= OnBindingContextChanged;
			AssociatedObject = null;
		}

		protected override void OnBindingContextChanged ()
		{
			base.OnBindingContextChanged ();
			BindingContext = AssociatedObject.BindingContext;
		}

        #endregion

        #region private methods

        private void OnBindingContextChanged(Object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        #endregion

    }

}

