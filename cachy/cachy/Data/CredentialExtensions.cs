using cachy.Config;
using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cachy.Data
{

    public static class CredentialExtensions
    {

        #region private objects

        static double _weakThreshold = -1;

        #endregion

        #region public methods

        public static bool IsWeak(this Credential credential)
        {
            if(_weakThreshold == -1)
            {
                int recommendedCharSelection = SimpleRandomGenerator.GetTotalCharCountForSelection(SimpleRandomGenerator.CharSelection.Lowercase |
                    SimpleRandomGenerator.CharSelection.Uppercase |
                    SimpleRandomGenerator.CharSelection.Digits |
                    SimpleRandomGenerator.CharSelection.Minus |
                    SimpleRandomGenerator.CharSelection.Underline);
                _weakThreshold = Math.Pow(12, recommendedCharSelection);
            }
            double strength = SimpleRandomGenerator.GetStrength(credential.Password);
            return (strength < _weakThreshold);
        }

        public static bool IsKnown(this Credential credential)
        {
            return (Dictionaries.Instance.IsKnownWeak(credential.Password));
        }

        public static bool IsReused(this Credential credential)
        {
            return (credential.Vault.Credentials.Any(cred => cred != credential && cred.Password == credential.Password));
        }

        public static bool IsOlderThan(
            this Credential credential,
            TimeSpan age)
        {
            return (DateTime.UtcNow - credential.PasswordLastModifiedAt > age);
        }

        #endregion

    }

}
