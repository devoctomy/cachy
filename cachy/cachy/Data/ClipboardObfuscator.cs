using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.DFramework.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace cachy.Data
{

    public class ClipboardObfuscator
    {

        #region private objects

        private static object _instanceLock = new object();
        private static ClipboardObfuscator _instance;
        private IDLogger _logger;
        private bool _fakeUsernames;
        private bool _fakePassword;
        private Func<ClipboardObfuscatorGeneratedFakeEventArgs, Task> _action;
        private List<string> _domains;
        private bool _running;
        private bool _stop;

        #endregion

        #region public properties

        public static ClipboardObfuscator Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        throw new InvalidOperationException("Clipboard obfuscator has not been initialised.");
                    }
                }
                return (_instance);
            }
        }

        public bool FakeUsernames
        {
            get
            {
                return (_fakeUsernames);
            }
            set
            {
                _fakeUsernames = value;
            }
        }

        public bool FakePasswords
        {
            get
            {
                return (_fakePassword);
            }
            set
            {
                _fakePassword = value;
            }
        }

        #endregion

        #region constructor / destructor

        private ClipboardObfuscator(
            IDLogger logger,
            bool fakeUsernames,
            bool fakePasswords,
            Func<ClipboardObfuscatorGeneratedFakeEventArgs, Task> action)
        {
            _logger = logger;
            _fakeUsernames = fakeUsernames;
            _fakePassword = fakePasswords;
            _action = action;
        }

        #endregion

        #region private methods

        private static ClipboardObfuscator CreateDefault(
            IDLogger logger,
            Func<ClipboardObfuscatorGeneratedFakeEventArgs, Task> action)
        {
            ClipboardObfuscator instance = new ClipboardObfuscator(
                logger,
                true,
                true,
                action);
            return (instance);
        }

        private async Task PerformObfuscation()
        {
            await Task.Yield();

            //generate a fake username or password here
            List<string> _fakes = new List<string>();
            if (FakeUsernames)
            {
                _logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Information, "Generating fake username.");
                string user = SimpleRandomGenerator.QuickGetRandomString(
                    SimpleRandomGenerator.CharSelection.Lowercase | SimpleRandomGenerator.CharSelection.Uppercase | SimpleRandomGenerator.CharSelection.Digits,
                    SimpleRandomGenerator.QuickGetRandomInt(4, 12),
                    false);
                string userName = String.Empty;
                if (_domains == null)
                {
                    _logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Information, "Loading list of domains for first time.");
                    _domains = new List<string>();
                    using (StringReader stringReader = new StringReader(Properties.Resources.domains))
                    {
                        string curLine = stringReader.ReadLine();
                        while (!String.IsNullOrEmpty(curLine))
                        {
                            if (!_domains.Contains(curLine)) _domains.Add(curLine);
                            curLine = stringReader.ReadLine();
                        }
                    }
                }
                string domain = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(_domains.ToArray());
                userName = String.Format("{0}@{1}", user, domain);
                _fakes.Add(userName);
            }

            if (FakePasswords)
            {
                _logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Information, "Generating fake password.");
                _fakes.Add(SimpleRandomGenerator.QuickGetRandomString(SimpleRandomGenerator.CharSelection.All, 16, false));
            }

            _logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Information, "Randomly picking fake data.");
            string fakeEntry = SimpleRandomGenerator.QuickGetRandomStringArrayEntry(_fakes.ToArray());

            _logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Information, "Fake data picked, invoking obfuscate action.");
            await _action.Invoke(new ClipboardObfuscatorGeneratedFakeEventArgs(fakeEntry));
        }

        #endregion

        #region public methods

        public static void Initialise(
            IDLogger logger,
            Func<ClipboardObfuscatorGeneratedFakeEventArgs, Task> action)
        {
            _instance = CreateDefault(
                logger,
                action);
        }

        public void Start()
        {
            if(!_running)
            {
                Device.StartTimer(TimeSpan.FromMilliseconds(SimpleRandomGenerator.QuickGetRandomInt(2000, 30000)), TimerCallback);
                _stop = false;
                _running = true;
            }
        }

        public void Stop()
        {
            if(_running)
            {
                _stop = true;
            }
        }

        public async Task PerformObfuscation(int count)
        {
            for(int i = 0; i < count; i++)
            {
                await PerformObfuscation();
            }
        }

        #endregion

        #region object events

        private bool TimerCallback()
        {
            try
            {
                _running = false;
                if (!_stop)
                {
                    Task performTask = PerformObfuscation();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Exception, "Clipboard obfuscation failed. {0}", ex.Message);
            }
            return (false);
        }

        #endregion

    }

}
