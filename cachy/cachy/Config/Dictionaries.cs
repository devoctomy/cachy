using devoctomy.cachy.Framework.Cryptography.Random;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cachy.Config
{

    public class Dictionaries
    {

        #region private objects

        private static object _lock = new object();
        private static Dictionaries _instance;

        private List<string> _verbs;
        private List<string> _adjectives;
        private List<string> _nouns;
        private Dictionary<string, object> _weak;

        #endregion

        #region public properties

        public static Dictionaries Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = LoadDefault();
                    }
                }
                return (_instance);
            }
        }

        public IReadOnlyList<string> Verbs
        {
            get
            {
                return (_verbs);
            }
        }

        public IReadOnlyList<string> Adjectives
        {
            get
            {
                return (_adjectives);
            }
        }

        public IReadOnlyList<string> Nouns
        {
            get
            {
                return (_nouns);
            }
        }

        #endregion

        #region constructor / destructor

        private Dictionaries(
            string[] verbs,
            string[] adjectives,
            string[] nouns,
            Dictionary<string, object> weak)
        {
            _verbs = new List<string>(verbs);
            _adjectives = new List<string>(adjectives);
            _nouns = new List<string>(nouns);
            _weak = weak;
        }

        #endregion

        #region private methods

        private static string[] ReadAllLinesFromTextResourceToArray(string resource)
        {
            Dictionary<string, string> all = new Dictionary<string, string>();
            List<string> lines = new List<string>();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                StreamReader reader = new StreamReader(stream);
                string curLine = reader.ReadLine();
                while (stream.Position < stream.Length)
                {
                    if (!String.IsNullOrEmpty(curLine) && !all.ContainsKey(curLine))
                    {
                        lines.Add(curLine);
                        all.Add(curLine, curLine);
                    }
                    curLine = reader.ReadLine();
                }
            }
            return (lines.ToArray());
        }

        private static Dictionary<string,object> ReadAllLinesFromTextResourceToDictionary(string resource)
        {
            Dictionary<string, object> all = new Dictionary<string, object>();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                StreamReader reader = new StreamReader(stream);
                string curLine = reader.ReadLine();
                while (stream.Position < stream.Length)
                {
                    if (!String.IsNullOrEmpty(curLine) && !all.ContainsKey(curLine))
                    {
                        all.Add(curLine, null);
                    }
                    curLine = reader.ReadLine();
                }
            }
            return (all);
        }

        private static Dictionaries LoadDefault()
        {
            string[] verbs = ReadAllLinesFromTextResourceToArray("cachy.Assets.Dictionaries.verbs.txt");
            string[] adjectives = ReadAllLinesFromTextResourceToArray("cachy.Assets.Dictionaries.adjectives.txt");
            string[] nouns = ReadAllLinesFromTextResourceToArray("cachy.Assets.Dictionaries.nouns.txt");
            Dictionary<string, object> weak = ReadAllLinesFromTextResourceToDictionary("cachy.Assets.Dictionaries.weakpasswords.txt");

            Dictionaries dictionaries = new Dictionaries(
                verbs,
                adjectives,
                nouns,
                weak);
            return (dictionaries);
        }

        #endregion

        #region public methods

        public static void Initialise()
        {
            Dictionaries instance = Instance;
        }

        public bool IsKnownWeak(string password)
        {
            return (_weak.ContainsKey(password));
        }

        public string GeneratePhrase(string format)
        {
            string phrase = format;

            //we do this in a loop so we can replace individual tokens as we go
            Regex tokenRegex = new Regex("\\{(.*?)\\}");
            Dictionary<string, string> invalidTokens = new Dictionary<string, string>();
            MatchCollection tokens = tokenRegex.Matches(phrase);
            while(tokens.Count > 0)
            {
                Match firstMatch = tokens[0];

                string token = firstMatch.Value.Trim(new char[] { '{', '}' });
                string[] tokenParts = token.Split(':');
                if(tokenParts.Length == 2)
                {
                    string randomWord = String.Empty;
                    switch(tokenParts[0])
                    {
                        case "verb":
                            {
                                randomWord = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(Verbs.ToArray());
                                break;
                            }
                        case "adjective":
                            {
                                randomWord = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(Adjectives.ToArray());
                                break;
                            }
                        case "noun":
                            {
                                randomWord = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(Nouns.ToArray());
                                break;
                            }
                        case "int":
                            {
                                randomWord = "{int}";
                                break;
                            }
                        case "special1":
                            {
                                randomWord = "{special1}";
                                break;
                            }
                        default:
                            {
                                randomWord = "{invalid}";
                                break;
                            }
                    }
                    bool replace = true;
                    switch(randomWord)
                    {
                        case "{int}":
                            {
                                string range = tokenParts[1];
                                string[] rangeParts = range.Split('-');
                                int min = int.Parse(rangeParts[0]);
                                int max = int.Parse(rangeParts[1]);
                                if (max > min)
                                {
                                    int randomInt = SimpleRandomGenerator.QuickGetRandomInt(min, max);
                                    randomWord = randomInt.ToString();
                                }
                                else
                                {
                                    randomWord = String.Empty;
                                }
                                break;
                            }
                        case "{special1}":
                            {
                                int count = int.Parse(tokenParts[1]);
                                randomWord = SimpleRandomGenerator.QuickGetRandomString(
                                    SimpleRandomGenerator.CharSelection.Minus |
                                    SimpleRandomGenerator.CharSelection.Underline,
                                    count,
                                    false);
                                break;
                            }
                        case "{invalid}":
                            {
                                string tempReplacement = String.Format("[invalidtoken:{0}]", invalidTokens.Count);
                                invalidTokens.Add(tempReplacement, firstMatch.Value);
                                phrase = phrase.Remove(firstMatch.Index, firstMatch.Length);
                                phrase = phrase.Insert(firstMatch.Index, tempReplacement);
                                replace = false;
                                break;
                            }
                        default:
                            {
                                string casing = tokenParts[1];
                                if (casing == "rc")
                                {
                                    casing = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(new string[] { "lc", "uc", "ic" });
                                }
                                switch (casing)
                                {
                                    case "lc":
                                        {
                                            randomWord = randomWord.ToLower();
                                            break;
                                        }
                                    case "uc":
                                        {
                                            randomWord = randomWord.ToUpper();
                                            break;
                                        }
                                    case "ic":
                                        {
                                            randomWord = randomWord[0].ToString().ToUpper() + randomWord.Substring(1);
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                    if(replace)
                    {
                        phrase = phrase.Remove(firstMatch.Index, firstMatch.Length);
                        phrase = phrase.Insert(firstMatch.Index, randomWord);
                    }
                }
                else
                {
                    //invalid token, remove it so that we don't get stuck in the loop
                    string tempReplacement = String.Format("[invalidtoken:{0}]", invalidTokens.Count);
                    invalidTokens.Add(tempReplacement, firstMatch.Value);
                    phrase = phrase.Remove(firstMatch.Index, firstMatch.Length);
                    phrase = phrase.Insert(firstMatch.Index, tempReplacement);
                }

                tokens = tokenRegex.Matches(phrase);
            }

            foreach(string token in invalidTokens.Keys)
            {
                if(phrase.Contains(token))
                {
                    phrase = phrase.Replace(token, invalidTokens[token]);
                }
            }

            return (phrase);
        }

        #endregion

    }

}
