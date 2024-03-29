using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DMS.ABE.Common
{
    public class DataEntity
    {

        private static Dictionary<string, JObject> _ErrorResource;
        public static Dictionary<string, JObject> ErrorResource
        {
            get
            {
                if (_ErrorResource == null)
                {
                    _ErrorResource = new Dictionary<string, JObject>();
                    List<string> languages = new List<string> { "vi", "en" };
                    foreach (string language in languages)
                    {
                        string folder = Path.Combine(Directory.GetCurrentDirectory(), "Resources/" + language);
                        if (Directory.Exists(folder))
                        {
                            List<string> files = Directory.GetFiles(folder).ToList();
                            foreach (string file in files)
                            {
                                string content = File.ReadAllText(file);
                                ErrorResource.Add(language + "." + Path.GetFileNameWithoutExtension(file), JObject.Parse(content));
                            }
                        }
                    }
                }
                return _ErrorResource;
            }
        }

        private string _DataPath;
        private string DataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_DataPath))
                    return this.GetType().Name;
                else
                    return _DataPath;
            }
            set
            {
                _DataPath = value + "." + this.GetType().Name;
            }
        }


        private string _BaseLanguage;
        internal string BaseLanguage
        {
            get
            {
                return _BaseLanguage;
            }
            set
            {
                _BaseLanguage = value;
                List<PropertyInfo> PropertyInfoes = this.GetType().GetProperties().ToList();
                foreach (PropertyInfo PropertyInfo in PropertyInfoes)
                {
                    if (PropertyInfo.PropertyType.IsGenericType && PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IEnumerable<DataEntity> DataEntities = PropertyInfo.GetValue(this) as IEnumerable<DataEntity>;
                        if (DataEntities != null)
                            foreach (DataEntity DataEntity in DataEntities)
                            {
                                DataEntity.DataPath = this.DataPath;
                                DataEntity.BaseLanguage = this._BaseLanguage;
                            }
                    }
                    if (PropertyInfo.PropertyType.IsSubclassOf(typeof(DataEntity)))
                    {
                        DataEntity DataEntity = PropertyInfo.GetValue(this) as DataEntity;
                        if (DataEntity != null)
                        {
                            DataEntity.DataPath = this.DataPath;
                            DataEntity.BaseLanguage = this._BaseLanguage;
                        }
                    }
                }
            }
        }

        private bool _IsValidated = true;
        internal bool IsValidated
        {
            get
            {
                if (Errors != null && Errors.Count > 0) return _IsValidated = false;
                List<PropertyInfo> PropertiesInfo = this.GetType().GetProperties().ToList();
                foreach (PropertyInfo PropertyInfo in PropertiesInfo)
                {
                    if (PropertyInfo.PropertyType.IsGenericType && PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        if (PropertyInfo.GetValue(this) is IEnumerable<DataEntity> DataEntities)
                            foreach (DataEntity DataEntity in DataEntities)
                            {
                                this._IsValidated &= DataEntity.IsValidated;
                            }
                    }
                    if (PropertyInfo.PropertyType.IsSubclassOf(typeof(DataEntity)))
                    {
                        if (PropertyInfo.GetValue(this) is DataEntity DataEntity)
                        {
                            this._IsValidated &= DataEntity.IsValidated;
                        }
                    }
                }
                return this._IsValidated;
            }
        }

        public Dictionary<string, string> Errors { get; private set; }

        public DataEntity()
        {
        }

        public void AddError(string className, string Key, Enum Value)
        {
            if (string.IsNullOrEmpty(_BaseLanguage)) _BaseLanguage = "VN";
            if (Errors == null) Errors = new Dictionary<string, string>();

            string file = string.Format("{0}.{1}", _BaseLanguage, className);
            string path = string.Format("{0}.{1}.{2}", DataPath, Key, Value.ToString());
            JToken token = ErrorResource.GetValueOrDefault(file)?.SelectToken(path);
            string content = token == null ? Value.ToString() : token.ToString();
            Key = ToCamel(Key);
            if (Errors.ContainsKey(Key))
            {
                if (!Errors[Key].Contains(content))
                    Errors[Key] += content;
            }
            else
                Errors.Add(Key, content);
        }

        private string ToCamel(string name)
        {
            return Char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }

    public class GenericEnum : IEquatable<GenericEnum>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public bool Equals([AllowNull] GenericEnum other)
        {
            if (other == null)
                return false;
            return Id == other.Id;
        }
    }
}
