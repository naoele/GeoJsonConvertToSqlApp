using System;
using System.Collections.Generic;
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
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static List<CourseCsv> ReadCsv(string path)
        {
            if (!File.Exists(path)) return null;
            List<string[]> lines = new List<string[]>();
            var readToEnd = new StringBuilder();
            using (var sr = new StreamReader(path, Encoding.GetEncoding("shift_jis")))
            {
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
            CourseJson json = Deserialize<CourseJson>(jsonstring);
            if (json.Features.Count != 1) throw new ArgumentOutOfRangeException("" + fileName + " は複数の線で構成されているので無効です。");
            Geometry geometry = null;
            foreach (Features feature in json.Features)
            {
                geometry = feature.Geometry;
            }
            return new CoursePoint(fileName, geometry);
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

        /// <summary>
        /// 同じ分類コードで同じ名前のコースがあった場合にメッセージを表示する
        /// </summary>
        /// <param name="list"></param>
        public static string CheckDuplication(List<CourseCsv> list)
        {
            string errMsg = "";
            var indexList = new List<int>();
            var hashset = new HashSet<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (hashset.Add(list[i].Junkai_course_name) == false)
                {
                    indexList.Add(i);
                }
            }
            foreach (int index in indexList)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (index == i) continue;

                    CourseCsv value = list[i];
                    CourseCsv duplication = list[index];
                    if (value.Junkai_course_name == duplication.Junkai_course_name)
                    {
                        if (value.Cd_kikan1 == duplication.Cd_kikan1 && value.Cd_kikan2 == duplication.Cd_kikan2 && value.Cd_kikan3 == duplication.Cd_kikan3)
                        {
                            errMsg += duplication.Junkai_course_name + " は重複しています。\n";
                        }
                    }
                }
            }
            return errMsg;
        }

    }
}
