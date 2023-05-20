using Newtonsoft.Json;
using UnityEngine;

namespace Project.Meta
{
    public class Storage
    {
        private JsonSerializerSettings _serializerSettings = null;

        private Storage()
        {
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.NullValueHandling = NullValueHandling.Include;
            _serializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
        }
        
        public void Save(IStoragable storagable)
        {
            var data = storagable.StorageData;
            var serializedStorageData = JsonConvert.SerializeObject(data, data.GetType(), _serializerSettings);
            PlayerPrefs.SetString(storagable.Key, serializedStorageData);
        }
        
        public void Load(IStoragable storagable)
        {
            var serializedData = PlayerPrefs.GetString(storagable.Key);
            storagable.StorageData =
                JsonConvert.DeserializeObject(serializedData, storagable.StorageData.GetType(), _serializerSettings) as
                    StorageData;
        }
        
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}