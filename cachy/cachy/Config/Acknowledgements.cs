using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace cachy.Config
{

    public class Acknowledgements
    {

        #region private objects

        private static object _lock = new object();
        private static Acknowledgements _instance;

        private ObservableCollection<Acknowledgement> _acknowledgements;

        #endregion

        #region public properties

        public static Acknowledgements Instance
        {
            get
            {
                lock(_lock)
                {
                    if (_instance == null)
                    {
                        _instance = LoadDefault();
                    }
                }
                return (_instance);
            }
        }

        public ObservableCollection<Acknowledgement> Acks
        {
            get
            {
                return (_acknowledgements);
            }
        }

        #endregion

        #region constructor / destructor

        private Acknowledgements(JObject json)
        {
            _acknowledgements = new ObservableCollection<Acknowledgement>();
            ParseAcknowledgements(json);
        }

        #endregion


        #region private methods

        private static Acknowledgements LoadDefault()
        {
            JObject json;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("cachy.Assets.Acknowledgements.acknowledgements.json"))
            {
                using (JsonReader reader = new JsonTextReader(new StreamReader(stream)))
                {
                    reader.DateParseHandling = DateParseHandling.None;
                    json = JObject.Load(reader);
                }
            }

            Acknowledgements acknowledgements = new Acknowledgements(json);
            return (acknowledgements);
        }

        private void ParseAcknowledgements(JObject json)
        {
            JArray acknowledgements = json["Acknowledgements"].Value<JArray>();
            foreach (JObject curAcknowledgement in acknowledgements)
            {
                Acknowledgement acknowledgement = Acknowledgement.FromJSON(curAcknowledgement);
                _acknowledgements.Add(acknowledgement);
            }
        }

        #endregion

    }

}
