using System;
using System.Reflection;

namespace devoctomy.cachy.Framework.Data
{

    public class StandardFieldAttribute : Attribute
    {

        #region private objects

        private string _name = String.Empty;
        private string _displayName = string.Empty;
        private bool _requiredForImport;

        #endregion

        #region public properties

        public string Name
        {
            get
            {
                return (_name);
            }
        }

        public string DisplayName
        {
            get
            {
                return (_displayName);
            }
        }

        public bool RequiredForImport
        {
            get
            {
                return (_requiredForImport);
            }
        }

        #endregion

        #region constructor / destructor

        public StandardFieldAttribute(
            string name,
            string displayName,
            bool requiredForImport)
        {
            _name = name;
            _displayName = displayName;
            _requiredForImport = requiredForImport;
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return (DisplayName);
        }

        #endregion

        #region public methods

        public static StandardFieldAttribute Get(
            Type type, 
            string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName);
            if(property != null)
            {
                StandardFieldAttribute attribute = property.GetCustomAttribute<StandardFieldAttribute>();
                return (attribute);
            }
            else
            {
                return (null);
            }
        }

        #endregion

    }

}
