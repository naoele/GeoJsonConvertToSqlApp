using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GeoJsonConvertToSqlApp.Models
{
    public class Util
    {
        /// <summary>
        /// exe実行パス取得
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAppDir()
        {
            return Directory.GetCurrentDirectory();
        }

        public static List<CourseCsv> ReadCsv(string path)
        {
            if (!File.Exists(path)) return null;
            List<string[]> lines = new List<string[]>();
            var readToEnd = new StringBuilder();
            using (var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8")))
            {
                string[] header = sr.ReadLine().Split(',');
                if ("id" != Util.TrimDoubleQuotationMarks(header[0]))
                {
                    // csvの先頭がヘッダー行ではないので追加
                    lines.Add(header);
                }
                while (sr.Peek() > -1)
                {
                    string[] values = sr.ReadLine().Split(',');
                    lines.Add(values);
                }
            }
            List<CourseCsv> list = new List<CourseCsv>();
            foreach (string[] ary in lines)
            {
                CourseCsv model = new CourseCsv(int.Parse(ary[0]), int.Parse(ary[1]), int.Parse(ary[2]), int.Parse(ary[3]), ary[4], int.Parse(ary[5]));
                list.Add(model);
            }
            return list;
        }

        public static CoursePoint ReadJson(string fileName)
        {
            if (!File.Exists(fileName)) return null;

            string jsonstring = File.ReadAllText(fileName, Encoding.GetEncoding("UTF-8"));
            JsonCourse json = Deserialize<JsonCourse>(jsonstring);
            if (json.Features.Count != 1) throw new ArgumentOutOfRangeException("" + fileName + " は複数の線で構成されているので無効です。");
            JsonGeometry geometry = null;
            foreach (JsonFeatures feature in json.Features)
            {
                geometry = feature.Geometry;
            }
            return new CoursePoint(Path.GetFileNameWithoutExtension(fileName), geometry);
        }

        /// <summary>
        /// JSONをオブジェクトへデシリアライズする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string message)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static string TrimDoubleQuotationMarks(string target)
        {
            return target.Trim(new char[] { '"' });
        }

        /// <summary>
        /// 型指定に応じて値を返却
        /// </summary>
        /// <typeparam name="T">変換型指定</typeparam>
        /// <param name="key">キー値</param>
        /// <returns>返還後値</returns>
        public static T GetAppValue<T>(string key)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)bool.Parse(GetAppSetting(key));
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)int.Parse(GetAppSetting(key));
            }

            if (typeof(T) == typeof(double))
            {
                return (T)(object)double.Parse(GetAppSetting(key));
            }

            return (T)(object)GetAppSetting(key);
        }

        /// <summary>
        /// appSettingsに存在するaddタグの中から引数に該当するKey値をもつ値を取得
        /// </summary>
        /// <param name="key">キー値</param>
        /// <returns>キーに紐づく値</returns>
        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
