using devoctomy.cachy.Framework.Serialisers.Interfaces;
using devoctomy.cachy.Framework.Serialisers.JSON.Exceptions;
using devoctomy.cachy.Framework.Serialisers.JSON.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace devoctomy.cachy.Framework.Serialisers.JSON
{

    public sealed class FormatVersions
    {

        #region private objects

        private static object _lock = new object();
        private static FormatVersions _instance;

        private Dictionary<FormatVersionAttribute, Type> _versionSerialisers;

        #endregion

        #region public properties

        public static FormatVersions Instance
        {
            get
            {
                lock(_lock)
                {
                    _instance = new FormatVersions();
                }
                return (_instance);
            }
        }

        public IReadOnlyDictionary<FormatVersionAttribute, Type> VersionSerialisers
        {
            get
            {
                return (_versionSerialisers);
            }
        }

        #endregion

        #region constructor / destructor

        private FormatVersions()
        {
            Assembly thisAssembly = typeof(FormatVersions).Assembly;
            _versionSerialisers = new Dictionary<FormatVersionAttribute, Type>();
            Type[] allTypes = thisAssembly.GetTypes();
            foreach (Type type in allTypes)
            {            
                try
                {
                    if (FormatVersionAttribute.TypeHasAttribute(type))
                    {
                        FormatVersionAttribute attribute = FormatVersionAttribute.GetAttributeFromType(type);
                        _versionSerialisers.Add(attribute, type);
                    }
                }
                catch(Exception)
                {
                    //Do nothing
                }
            }
        }

        #endregion

        #region public methods

        public ISerialiser GetSerialiser(
            string version,
            Type objectType)
        {
            IEnumerable<FormatVersionAttribute> matches = _versionSerialisers.Keys.Where(fva => fva.Version == version && fva.ObjectType == objectType);
            if(matches.Any())
            {
                FormatVersionAttribute[] allMatches = matches.ToArray();
                if(allMatches.Length == 1)
                {
                    FormatVersionAttribute match = allMatches[0];
                    Type type = _versionSerialisers[match];
                    object instance = Activator.CreateInstance(type);
                    return ((ISerialiser)instance);
                }
                else
                {
                    throw new MultipleVersionSerialiserMatchesException(
                        version,
                        objectType);
                }
            }
            else
            {
                throw new UnmatchedVersionSerialiserException(
                    version,
                    objectType);
            }
        }

        public static ISerialiser GetLatestSerialiser(Type objectType)
        {
            ISerialiser serialiser = FormatVersions.Instance.GetSerialiser(
                Common.LATEST_VAULT_VERSION,
                objectType);
            return (serialiser);
        }

        #endregion

    }

}
