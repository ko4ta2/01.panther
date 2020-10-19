using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using NLog;

namespace Common
{
    public static class XmlFileLoader
    {
        /// <summary>
        /// log
        /// </summary>
        private static Logger _Log = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// XMLからオブジェクトを生成する
        /// </summary>
        /// <typeparam name="T">デシリアライズするオブジェクトの型</typeparam>
        /// <param name="fileName">XMLファイル名</param>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public static T CreateObject<T>(string fileName){
            T obj = default(T);

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var reader = new StreamReader(fileName, new UTF8Encoding(false)))
                {
                    obj = (T)xmlSerializer.Deserialize(reader);
                }
            }
            catch(Exception ex)
            {
                _Log.Error(ex);
            }

            return obj;
        }
    }
}
