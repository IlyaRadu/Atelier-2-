using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;

namespace MediaTekDocuments.dal
{
    public class Access
    {
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/";
        private static Access instance = null;
        private readonly ApiRest api = null;
        private const string GET = "GET";
        private const string POST = "POST";

        private Access()
        {
            string apiUser = ConfigurationManager.AppSettings["ApiUser"];
            string apiPwd = ConfigurationManager.AppSettings["ApiPwd"];
            string authenticationString = apiUser + ":" + apiPwd;
            api = ApiRest.GetInstance(uriApi, authenticationString);
        }

        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> liste = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(liste);
        }

        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> liste = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(liste);
        }

        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> liste = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(liste);
        }

        public List<Livre> GetAllLivres()
        {
            List<Livre> liste = TraitementRecup<Livre>(GET, "livre", null);
            return liste;
        }

        public List<Dvd> GetAllDvd()
        {
            List<Dvd> liste = TraitementRecup<Dvd>(GET, "dvd", null);
            return liste;
        }

        public List<Revue> GetAllRevues()
        {
            List<Revue> liste = TraitementRecup<Revue>(GET, "revue", null);
            return liste;
        }

        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            string json = convertToJson("id", idDocument);
            List<Exemplaire> liste = TraitementRecup<Exemplaire>(GET, "exemplaire/" + json, null);
            return liste;
        }

        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            string json = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + json);
                return (liste != null);
            }
            catch
            {
                return false;
            }
        }

        private List<T> TraitementRecup<T>(string methode, string message, string parametres)
        {
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                string code = (string)retour["code"];
                if (code.Equals("200"))
                {
                    if (methode.Equals(GET))
                    {
                        string resultString = JsonConvert.SerializeObject(retour["result"]);
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
            }
            catch
            {
            }
            return liste;
        }

        private string convertToJson(object nom, object valeur)
        {
            Dictionary<object, object> d = new Dictionary<object, object>();
            d.Add(nom, valeur);
            return JsonConvert.SerializeObject(d);
        }

        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }
            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
