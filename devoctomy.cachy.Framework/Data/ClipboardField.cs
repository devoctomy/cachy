using System.Windows.Input;

namespace devoctomy.cachy.Framework.Data
{

    public class ClipboardField
    {

        #region private objects

        private string _name;
        private string _value;
        private ICommand _copyCommand;
        private ICommand _revealCommand;

        #endregion

        #region public properties

        public string Name
        {
            get
            {
                return (_name);
            }
        }

        public string Value
        {
            get
            {
                return (_value);
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                return (_copyCommand);
            }
        }

        public ICommand RevealCommand
        {
            get
            {
                return (_revealCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public ClipboardField(string name,
            string value,
            ICommand copyCommand,
            ICommand revealCommand)
        {
            _name = name;
            _value = value;
            _copyCommand = copyCommand;
            _revealCommand = revealCommand;
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return (Name);
        }

        #endregion

    }

}
