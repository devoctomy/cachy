using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace devoctomy.cachy.Framework.Cryptography.Random
{

    public class SimpleRandomGenerator : IDisposable
    {

        #region public constants

        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_LOWERCASE = "abcdefghijklmnopqrstuvwxyz";
        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_UPPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_DIGITS = "0123456789";
        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_MINUS = "-";
        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_UNDERLINE = "_";
        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_SPACE = " ";
        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_BRACKETS = "[]{}()<>";
        public const string SIMPLERANDOMGENERATOR_CHARSELECTION_OTHER = "!£$%&+@#,.\\/";

        #endregion

        #region public enums

        [Flags]
        public enum CharSelection
        {
            None = 0,
            Lowercase = 1,
            Uppercase = 2,
            Digits = 4,
            Minus = 8,
            Underline = 16,
            Space = 32,
            Brackets = 64,
            Other = 128,
            All = 255
        }

        #endregion

        #region private objects

        private RNGCryptoServiceProvider _rand;

        #endregion

        #region constructor / destructor

        public SimpleRandomGenerator()
        {
            _rand = new RNGCryptoServiceProvider();
        }

        ~SimpleRandomGenerator()
        {
            Dispose(false);
        }

        #endregion

        #region private methods

        private static bool StringContainsOneOf(
            string value,
            string chars)
        {
            foreach(char curChar in chars)
            {
                if(value.Contains(curChar.ToString()))
                {
                    return (true);
                }
            }
            return (false);
        }

        #endregion

        #region public methods

        public static CharSelection GetSelections(string value)
        {
            CharSelection selections = CharSelection.None;
            foreach (char curChar in value)
            {
                if (SIMPLERANDOMGENERATOR_CHARSELECTION_LOWERCASE.Contains(curChar.ToString())) selections |= CharSelection.Lowercase;
                else if (SIMPLERANDOMGENERATOR_CHARSELECTION_UPPERCASE.Contains(curChar.ToString())) selections |= CharSelection.Uppercase;
                else if (SIMPLERANDOMGENERATOR_CHARSELECTION_DIGITS.Contains(curChar.ToString())) selections |= CharSelection.Digits;
                else if (SIMPLERANDOMGENERATOR_CHARSELECTION_MINUS.Contains(curChar.ToString())) selections |= CharSelection.Minus;
                else if (SIMPLERANDOMGENERATOR_CHARSELECTION_UNDERLINE.Contains(curChar.ToString())) selections |= CharSelection.Underline;
                else if (SIMPLERANDOMGENERATOR_CHARSELECTION_SPACE.Contains(curChar.ToString())) selections |= CharSelection.Space;
                else if (SIMPLERANDOMGENERATOR_CHARSELECTION_BRACKETS.Contains(curChar.ToString())) selections |= CharSelection.Brackets;
                else if (SIMPLERANDOMGENERATOR_CHARSELECTION_OTHER.Contains(curChar.ToString())) selections |= CharSelection.Other;
            }
            return (selections);
        }

        public static int GetTotalCharCountForSelection(CharSelection selection)
        {
            int totalCount = 0;
            if (selection.HasFlag(CharSelection.Lowercase)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_LOWERCASE.Length;
            if (selection.HasFlag(CharSelection.Uppercase)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_UPPERCASE.Length;
            if (selection.HasFlag(CharSelection.Digits)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_DIGITS.Length;
            if (selection.HasFlag(CharSelection.Minus)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_MINUS.Length;
            if (selection.HasFlag(CharSelection.Underline)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_UNDERLINE.Length;
            if (selection.HasFlag(CharSelection.Space)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_SPACE.Length;
            if (selection.HasFlag(CharSelection.Brackets)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_BRACKETS.Length;
            if (selection.HasFlag(CharSelection.Other)) totalCount += SIMPLERANDOMGENERATOR_CHARSELECTION_OTHER.Length;
            return (totalCount);
        }

        public static double GetStrength(string value)
        {
            CharSelection selection = GetSelections(value);
            int charCount = GetTotalCharCountForSelection(selection);
            double strength = Math.Pow(value.Length, charCount);
            return (strength);
        }

        public static Double QuickGetRandomDouble()
        {
            using (SimpleRandomGenerator rand = new SimpleRandomGenerator())
            {
                return (rand.GetRandomDouble());
            }
        }

        public Double GetRandomDouble()
        {
            Byte[] bytes = new Byte[8];
            _rand.GetBytes(bytes);
            UInt64 unscaled = BitConverter.ToUInt64(bytes, 0);
            unscaled &= ((1UL << 53) - 1);
            Double random = (Double)unscaled / (Double)(1UL << 53);
            return (random);
        }

        public static Int32 QuickGetRandomInt(
            Int32 min,
            Int32 max)
        {
            double fraction = QuickGetRandomDouble();
            double range = max - min;
            Int32 retVal = min + (Int32)(fraction * range);
            return ((Int32)retVal);
        }

        public static Byte[] QuickGetRandomBytes(Int32 length)
        {
            using (SimpleRandomGenerator rand = new SimpleRandomGenerator())
            {
                Byte[] randBytes = new Byte[length];
                rand.GetRandomBytes(randBytes);
                return (randBytes);
            }
        }

        public void GetRandomBytes(Byte[] bytes)
        {
            _rand.GetBytes(bytes);
        }

        public static Char QuickGetRandomChar(String chars)
        {
            using (SimpleRandomGenerator rand = new SimpleRandomGenerator())
            {
                return (rand.GetRandomChar(chars));
            }
        }

        public Char GetRandomChar(String chars)
        {
            Double percentage = GetRandomDouble();
            Int32 index = (Int32)Math.Floor(percentage * chars.Length);
            return (chars[index]);
        }

        public static String QuickGetRandomString(String chars,
            Int32 length)
        {
            using (SimpleRandomGenerator rand = new SimpleRandomGenerator())
            {
                return (rand.GetRandomString(chars, length));
            }
        }

        public static String QuickGetRandomString(
            CharSelection selection,
            Int32 length,
            bool atLeastOneOfEachGroup)
        {
            string random = String.Empty;
            StringBuilder chars = new StringBuilder();
            if (selection.HasFlag(CharSelection.Lowercase)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_LOWERCASE);
            if (selection.HasFlag(CharSelection.Uppercase)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_UPPERCASE);
            if (selection.HasFlag(CharSelection.Digits)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_DIGITS);
            if (selection.HasFlag(CharSelection.Minus)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_MINUS);
            if (selection.HasFlag(CharSelection.Underline)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_UNDERLINE);
            if (selection.HasFlag(CharSelection.Space)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_SPACE);
            if (selection.HasFlag(CharSelection.Brackets)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_BRACKETS);
            if (selection.HasFlag(CharSelection.Other)) chars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_OTHER);
            random = SimpleRandomGenerator.QuickGetRandomString(chars.ToString(), length);

            int groupCount = 0;
            groupCount += (selection.HasFlag(CharSelection.Lowercase)) ? 1 : 0;
            groupCount += (selection.HasFlag(CharSelection.Uppercase)) ? 1 : 0;
            groupCount += (selection.HasFlag(CharSelection.Digits)) ? 1 : 0;
            groupCount += (selection.HasFlag(CharSelection.Minus | CharSelection.Underline | CharSelection.Space | CharSelection.Brackets | CharSelection.Other)) ? 1 : 0;

            if (atLeastOneOfEachGroup && length >= groupCount)
            {
                StringBuilder specialChars = new StringBuilder();
                if (selection.HasFlag(CharSelection.Minus)) specialChars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_MINUS);
                if (selection.HasFlag(CharSelection.Underline)) specialChars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_UNDERLINE);
                if (selection.HasFlag(CharSelection.Space)) specialChars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_SPACE);
                if (selection.HasFlag(CharSelection.Brackets)) specialChars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_BRACKETS);
                if (selection.HasFlag(CharSelection.Other)) specialChars.Append(SIMPLERANDOMGENERATOR_CHARSELECTION_OTHER);

                bool fixing = true;
                while(fixing)
                {
                    fixing = false;
                    if (selection.HasFlag(CharSelection.Lowercase))
                    {
                        if (!StringContainsOneOf(random, SIMPLERANDOMGENERATOR_CHARSELECTION_LOWERCASE))
                        {
                            fixing = true;
                            string single = SimpleRandomGenerator.QuickGetRandomString(SIMPLERANDOMGENERATOR_CHARSELECTION_LOWERCASE, 1);
                            int index = SimpleRandomGenerator.QuickGetRandomInt(1, random.Length) - 1;
                            random = random.Remove(index, 1);
                            random = random.Insert(index, single);
                        }
                    }
                    if (selection.HasFlag(CharSelection.Uppercase))
                    {
                        if (!StringContainsOneOf(random, SIMPLERANDOMGENERATOR_CHARSELECTION_UPPERCASE))
                        {
                            fixing = true;
                            string single = SimpleRandomGenerator.QuickGetRandomString(SIMPLERANDOMGENERATOR_CHARSELECTION_UPPERCASE, 1);
                            int index = SimpleRandomGenerator.QuickGetRandomInt(1, random.Length) - 1;
                            random = random.Remove(index, 1);
                            random = random.Insert(index, single);
                        }
                    }
                    if (selection.HasFlag(CharSelection.Digits))
                    {
                        if (!StringContainsOneOf(random, SIMPLERANDOMGENERATOR_CHARSELECTION_DIGITS))
                        {
                            fixing = true;
                            string single = SimpleRandomGenerator.QuickGetRandomString(SIMPLERANDOMGENERATOR_CHARSELECTION_DIGITS, 1);
                            int index = SimpleRandomGenerator.QuickGetRandomInt(1, random.Length) - 1;
                            random = random.Remove(index, 1);
                            random = random.Insert(index, single);
                        }
                    }
                    if (specialChars.Length > 0)
                    {
                        if (!StringContainsOneOf(random, specialChars.ToString()))
                        {
                            fixing = true;
                            string single = SimpleRandomGenerator.QuickGetRandomString(specialChars.ToString(), 1);
                            int index = SimpleRandomGenerator.QuickGetRandomInt(1, random.Length) - 1;
                            random = random.Remove(index, 1);
                            random = random.Insert(index, single);
                        }
                    }
                }

            }
            return (random);
        }

        public String GetRandomString(String chars,
            Int32 length)
        {
            StringBuilder randomString = new StringBuilder();
            while(randomString.Length < length)
            {
                randomString.Append(GetRandomChar(chars));
            }
            return (randomString.ToString());
        }

        public static String QuickGetRandomStringArrayEntry(String[] array)
        {
            Double randomDouble = QuickGetRandomDouble();
            Int32 randomIndex = (Int32)(array.Length * randomDouble);
            return (array[randomIndex]);
        }

        public static T QuickGetRandomEnum<T>(Int32 count, Enum exclude)
        {
            String[] names = Enum.GetNames(typeof(T));
            List<String> random = new List<String>();
            while(random.Count < count)
            {
                String randomEnumString = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(names);
                Enum randomEnumValue = (Enum)Enum.Parse(typeof(T), randomEnumString, true);
                while(exclude.HasFlag(randomEnumValue))
                {
                    randomEnumString = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(names);
                    randomEnumValue = (Enum)Enum.Parse(typeof(T), randomEnumString, true);
                }
                random.Add(randomEnumString);
            }
            String randomEnumValues = String.Join(",", random.ToArray());
            T randomEnum = (T)Enum.Parse(typeof(T), randomEnumValues);
            return (randomEnum);
        }

        #endregion

        #region idisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(_rand != null)
                {
                    _rand.Dispose();
                    _rand = null;
                }
            }
        }

        #endregion

    }

}
