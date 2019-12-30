using System;

namespace devoctomy.cachy.Framework.Serialisers.JSON.Versions
{

    public class FormatVersionAttribute : Attribute
    {

        #region private objects

        private string _version = String.Empty;
        private Type _objectType;

        #endregion

        #region public properties

        public string Version
        {
            get
            {
                return (_version);
            }
        }

        public Type ObjectType
        {
            get
            {
                return (_objectType);
            }
        }

        #endregion

        #region constructor / destructor

        public FormatVersionAttribute(
            string version,
            Type objectType)
        {
            _version = version;
            _objectType = objectType;
        }

        #endregion

        #region public methods

        public static bool TypeHasAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(FormatVersionAttribute), false);
            return (attributes.Length > 0);
        }

        public static  FormatVersionAttribute GetAttributeFromType(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(FormatVersionAttribute), false);
            if(attributes.Length == 1)
            {
                return ((FormatVersionAttribute)attributes[0]);
            }
            else
            {
                throw new Exception(String.Format("FormatVersionAttribute not found on type '{0}'.", type.Name));
            }
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return (String.Format("[{0}/{1}]", ObjectType.Name, Version));
        }

        #endregion

    }

}
