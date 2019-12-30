namespace devoctomy.cachy.Framework.Data
{

    public class GenericResult<RT, VT>
    {

        #region private objects

        private RT _result;
        private VT _value;

        #endregion

        #region public properties

        public RT Result
        {
            get
            {
                return (_result);
            }
        }

        public VT Value
        {
            get
            {
                return (_value);
            }
        }

        #endregion

        #region constructor / destructor

        public GenericResult(
            RT result, 
            VT value)
        {
            _result = result;
            _value = value;
        }

        #endregion

    }

}
