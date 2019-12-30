using devoctomy.cachy.Framework.Data;
using System;

namespace cachy.Data
{

    public class ImportFieldMapping
    {

        #region private objects

        StandardFieldAttribute _attribute;
        string _importFieldName = String.Empty;

        #endregion

        #region public properties

        public ImportFieldMapping Me
        {
            get
            {
                return (this);
            }
        }

        public StandardFieldAttribute Attribute
        {
            get
            {
                return (_attribute);
            }
        }

        public string AttributeName
        {
            get
            {
                return (Attribute.Name);
            }
        }

        public string ImportFieldName
        {
            get
            {
                return (_importFieldName);
            }
        }

        public bool RequiredForImportButNotSet
        {
            get
            {
                return (Attribute.RequiredForImport && String.IsNullOrEmpty(ImportFieldName));
            }
        }

        #endregion

        #region constructor / destructor

        public ImportFieldMapping(
            StandardFieldAttribute attribute,
            string importFieldName)
        {
            _attribute = attribute;
            _importFieldName = importFieldName;
        }

        #endregion

    }

}
