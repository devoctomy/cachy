using System;

namespace cachy.ViewModels.Attributes
{

    public class ViewModelMappingAttribute : Attribute
    {

        #region public enums

        public enum InstancingType
        {
            None = 0,
            Single = 1,
            Multiple = 2
        }

        #endregion

        #region private objects

        private Type _viewModelType;
        private InstancingType _instancing;

        #endregion

        #region public properties

        public Type ViewModelType
        {
            get
            {
                return (_viewModelType);
            }
        }

        public InstancingType Instancing
        {
            get
            {
                return (_instancing);
            }
        }

        #endregion

        #region constructor / destructor

        public ViewModelMappingAttribute(Type viewModelType,
            InstancingType instancing)
        {
            _viewModelType = viewModelType;
            _instancing = instancing;
        }

        #endregion

        #region public methods

        public static ViewModelMappingAttribute GetFromType(Type type)
        {
            Object[] mappingAttributes = type.GetCustomAttributes(typeof(ViewModelMappingAttribute), true);
            if (mappingAttributes != null && mappingAttributes.Length == 1)
            {
                ViewModelMappingAttribute mappingAttribute = (ViewModelMappingAttribute)mappingAttributes[0];
                return (mappingAttribute);
            }
            else
            {
                return (null);
            }
        }

        #endregion

    }

}
