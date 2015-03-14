using System;
using System.IO;
using System.Runtime.Serialization.Json;
using TinyMetroWpfLibrary.LogUtil;
namespace TinyMetroWpfLibrary.Utility
{
    public class JsonSerializeHelper<T> where T : class
    {
        private string _filePath = string.Empty;
        private static ILogService _logger = new FileLogService(typeof(JsonSerializeHelper<T>));
        public JsonSerializeHelper(string filePath)
        {
            _filePath = filePath;
        }
        private Stream _stream;
        public JsonSerializeHelper(Stream stream)
        {
            _stream = stream;
        }
        public bool Serialize(T serializeObj)
        {
            if (serializeObj == null)
            {
                return false;
            }

            try
            {
                using (FileStream fs = new FileStream(_filePath, FileMode.Create))
                {
                    DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
                    formatter.WriteObject(fs, serializeObj);
                    _logger.Info(string.Format("Object: {0} serialized at {1}", serializeObj.ToString(), _filePath));
                }
            }
            catch (System.Exception e)
            {
                _logger.Fatal("JsonSerializeHelper.Serialize", e);
                return false;
            }

            return true;
        }

        public T DeSerialize()
        {
            T deSerializObj = null;
            try
            {
                if (File.Exists(_filePath))
                {
                    using (FileStream fs = new FileStream(_filePath, FileMode.Open))
                    {
                        fs.Position = 0;
                        DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
                        deSerializObj = (T)formatter.ReadObject(fs);
                    }
                }
                else
                {
                    //Norris Log: <_filePath> doesn't exist.
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal("JsonSerializeHelper.DeSerialize", ex);
            }
            return deSerializObj;
        }
        public T DeSerializeFromStream()
        {
            T deSerializObj = null;
            try
            {
                _stream.Position = 0;
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
                deSerializObj = (T)formatter.ReadObject(_stream);
            }
            catch (Exception ex)
            {
                _logger.Fatal("JsonSerializeHelper.DeSerializeFromStream", ex);
            }
            return deSerializObj;
        }
    }
}
