using GeoJsonConvertToSqlApp.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace GeoJsonConvertToSqlApp.ViewModels
{
    public class MainViewModel : NotifyChangedBase
    {
        public MainViewModel()
        {
            // テキスト表示
            this.CsvText = "巡回コースマスタCSVファイルを選択してください。";
            this.GeojsonText = "GoeJSONファイルを取得します。\nGeoJSONファイルのあるフォルダを選択してください。";
            this.Kanri1Text = "管理機関大分類コード：";
            this.Kanri2Text = "中分類コード：";
            this.Kanri3Text = "小分類コード：";
            _select_csv = FileExists(Util.GetCurrentAppDir() + @"\m_junkai_course.csv");
            _select_folder = DirExists(Util.GetCurrentAppDir() + @"\geojson");
            this.SelectCsvText = StoreCsvData(_select_csv);
            this.SelectFolderText = StoreJsonData(_select_folder);

            // ボタンアクション
            this.OpenFile = new DelegateCommand(
                () =>
                    {
                        // CSVを読み込み
                        OpenFileDialog ofDialog = new OpenFileDialog();
                        ofDialog.InitialDirectory = @"C:";
                        ofDialog.Title = "CSVファイル選択";
                        if (ofDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.SelectCsvText = ofDialog.FileName;
                            StoreCsvData(ofDialog.FileName);
                        }
                        else
                        {
                            Console.WriteLine("キャンセルされました");
                            this.LogText = "キャンセルされました";
                        }
                        ofDialog.Dispose();
                    },
                () => true
            );
            this.OpenFolder = new DelegateCommand(
                () =>
                {
                    try
                    {
                        // geojsonを読み込み
                        FolderBrowserDialog fbDialog = new FolderBrowserDialog();
                        fbDialog.SelectedPath = @"C:";
                        fbDialog.Description = "GeoJSONフォルダ選択";
                        fbDialog.ShowNewFolderButton = true;
                        if (fbDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.SelectFolderText = fbDialog.SelectedPath;
                            StoreJsonData(fbDialog.SelectedPath);
                        }
                        else
                        {
                            Console.WriteLine("キャンセルされました");
                            this.LogText = "キャンセルされました";
                        }
                        fbDialog.Dispose();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        this.LogText = e.StackTrace;
                    }
                },
                () => true
            );
            this.CreateSql = new DelegateCommand(
                () =>
                {
                    try
                    {
                        // SQLを作成
                        var sql = new StringBuilder();
                        sql.AppendLine("INSERT INTO m_course_line_point(cd_junkai_course, disp_order, latitude, longitude) ");
                        sql.AppendLine("VALUES ");
                        foreach (Course c in _course_list)
                        {
                            int cnt = 1;
                            foreach (GeoCoordinate Coordinate in c.Coordinates)
                            {
                                sql.Append(" (");
                                sql.Append($" {c.Id},");
                                sql.Append($" {cnt},");
                                sql.Append($" {Coordinate.Latitude},");
                                sql.Append($" {Coordinate.Longitude}");
                                sql.AppendLine(" ),");
                                cnt++;
                            }
                        }
                        sql.Length -= 3;
                        sql.AppendLine(";");

                        string path = Util.GetCurrentAppDir() + @"\m_course_line_point.sql";
                        File.WriteAllText(path, sql.ToString(), Encoding.GetEncoding("UTF-8"));
                        this.LogText = path + "にSQLを作成しました。";

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        this.LogText = e.StackTrace;
                    }
                },
                () => _course_list != null && _course_list.Count > 0 && _course_list[0].Coordinates != null
            );
        }

        private string FileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                string err = "選択している " + filePath + " は存在しません";
                Console.WriteLine(err);
                this.LogText = err;
                return "";
            }
            return filePath;
        }

        private string DirExists(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                string err = "選択している " + filePath + " は存在しません";
                Console.WriteLine(err);
                this.LogText = err;
                return "";
            }
            return filePath;
        }

        /// <summary>
        /// 同じ分類コードで同じ名前のコースがあった場合にメッセージを表示し
        /// そのコースは飛ばして処理する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<CourseCsv> DuplicationCheck(List<CourseCsv> courselist)
        {
            if (courselist == null) return null;

            var courseCsvList = new List<CourseCsv>();
            try
            {
                List<CourseCsv> list = ExtractOnlyCdKikan(courselist);
                string errMsg = "";
                var indexList = new List<int>();
                var hashset = new HashSet<string>();
                // 同じ事務所同じコース名がないか重複チェック
                for (int i = 0; i < list.Count; i++)
                {
                    if (hashset.Add(list[i].Junkai_course_name) == false)
                    {
                        // 重複したコース名のインデックスを追加
                        indexList.Add(i);
                    }
                    courseCsvList.Add(list[i]);
                }
                // 重複していたコースの管理機関番号が重複していないかチェック
                var removeList = new List<CourseCsv>();
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
                                removeList.Add(value);
                                removeList.Add(duplication);
                                errMsg += duplication.Junkai_course_name + " は事務所とコース名が重複しています。\n";
                            }
                        }
                    }
                }
                if (errMsg != "")
                {
                    this.LogText = errMsg;
                    Console.WriteLine(errMsg);

                    foreach (CourseCsv c in removeList)
                    {
                        courseCsvList.Remove(c);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.LogText = e.Message;
                courseCsvList.Clear();
            }

            return courseCsvList;
        }

        /// <summary>
        /// CSVとgeojsonのコース名で引き当ててSQLのもとになるモデルを作成する
        /// </summary>
        /// <param name="course_csv_list"></param>
        /// <param name="course_point_list"></param>
        /// <returns></returns>
        private List<Course> MergeCourse(List<CourseCsv> course_csv_list, List<CoursePoint> course_point_list)
        {
            if (course_csv_list == null && course_point_list == null) return null;

            bool isMatch = false;
            List<Course> list = new List<Course>();
            if (course_point_list == null)
            {
                foreach (CourseCsv csv in course_csv_list)
                {
                    isMatch = true;
                    list.Add(new Course(csv));
                }
                if (!isMatch) this.LogText = "CSVにコースがありませんでした。";
            }
            else if (course_csv_list == null)
            {
                foreach (CoursePoint point in course_point_list)
                {
                    isMatch = true;
                    list.Add(new Course(point));
                }
                if (!isMatch) this.LogText = "geojsonにコースがありませんでした。";
            }
            else
            {
                string err = "";
                foreach (CoursePoint point in course_point_list)
                {
                    foreach (CourseCsv csv in course_csv_list)
                    {
                        if (csv.Junkai_course_name == point.Name)
                        {
                            isMatch = true;
                            list.Add(new Course(csv, point));
                        }
                    }
                    if (!isMatch) err += point.Name + ".geojsonとマッチするコースがありませんでした。\n";
                    isMatch = false;
                }
                this.LogText = err;
            }

            return list;
        }

        /// <summary>
        /// CSVからデータを取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string StoreCsvData(string filePath)
        {
            if (filePath == "") return "";

            try
            {
                _course_csv_list = Util.ReadCsv(filePath);
                _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                this.CourseText = _course_list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.LogText = e.StackTrace;
            }
            return filePath;
        }

        /// <summary>
        /// geojsonからデータを取得する
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public string StoreJsonData(string dirPath)
        {
            if (dirPath == "") return "";

            try
            {
                _course_point_list = new List<CoursePoint>();
                IEnumerable<string> files = Directory.EnumerateFiles(dirPath, "*.geojson");
                foreach (string str in files)
                {
                    Console.WriteLine(str);
                    CoursePoint model = Util.ReadJson(str);
                    _course_point_list.Add(model);
                }

                _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                this.CourseText = _course_list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.LogText = e.StackTrace;
            }
            return dirPath;
        }

        public List<CourseCsv> ExtractOnlyCdKikan(List<CourseCsv> courseList)
        {
            if (courseList == null && _dai_bunrui_number == null && _chu_bunrui_number == null && _sho_bunrui_number == null) return null;
            else if (_dai_bunrui_number == null && _chu_bunrui_number == null && _sho_bunrui_number == null) return courseList;

            List<CourseCsv> list = new List<CourseCsv>();
            foreach (CourseCsv model in _course_csv_list)
            {
                if (_dai_bunrui_number != null && _chu_bunrui_number != null && _sho_bunrui_number != null)
                {
                    if (model.Cd_kikan1 == _dai_bunrui_number && model.Cd_kikan2 == _chu_bunrui_number && model.Cd_kikan3 == _sho_bunrui_number)
                    {
                        list.Add(model);
                    }
                }
                else if (_dai_bunrui_number != null && _chu_bunrui_number != null && _sho_bunrui_number == null)
                {
                    if (model.Cd_kikan1 == _dai_bunrui_number && model.Cd_kikan2 == _chu_bunrui_number)
                    {
                        list.Add(model);
                    }
                }
                else if (_dai_bunrui_number != null && _chu_bunrui_number == null && _sho_bunrui_number != null)
                {
                    if (model.Cd_kikan1 == _dai_bunrui_number && model.Cd_kikan3 == _sho_bunrui_number)
                    {
                        list.Add(model);
                    }
                }
                else if (_dai_bunrui_number == null && _chu_bunrui_number != null && _sho_bunrui_number != null)
                {
                    if (model.Cd_kikan2 == _chu_bunrui_number && model.Cd_kikan3 == _sho_bunrui_number)
                    {
                        list.Add(model);
                    }
                }
                else if (_dai_bunrui_number != null && _chu_bunrui_number == null && _sho_bunrui_number == null)
                {
                    if (model.Cd_kikan1 == _dai_bunrui_number)
                    {
                        list.Add(model);
                    }
                }
                else if (_dai_bunrui_number == null && _chu_bunrui_number != null && _sho_bunrui_number == null)
                {
                    if (model.Cd_kikan2 == _chu_bunrui_number)
                    {
                        list.Add(model);
                    }
                }
                else if (_dai_bunrui_number == null && _chu_bunrui_number == null && _sho_bunrui_number != null)
                {
                    if (model.Cd_kikan3 == _sho_bunrui_number)
                    {
                        list.Add(model);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// コースリスト
        /// </summary>
        private List<Course> _course_list;

        /// <summary>
        /// CSVファイルコースリスト
        /// </summary>
        private List<CourseCsv> _course_csv_list;

        /// <summary>
        /// geojsonコースリスト
        /// </summary>
        private List<CoursePoint> _course_point_list;

        /// <summary>
        /// コースCSVファイルを開く
        /// </summary>
        public DelegateCommand OpenFile { get; private set; }

        /// <summary>
        /// geojsonフォルダを開く
        /// </summary>
        public DelegateCommand OpenFolder { get; private set; }

        /// <summary>
        /// SQLを作成する
        /// </summary>
        public DelegateCommand CreateSql { get; private set; }

        /// <summary>
        /// CSVテキスト
        /// </summary>
        public string CsvText { get; private set; }

        /// <summary>
        /// GeoJSONテキスト
        /// </summary>
        public string GeojsonText { get; private set; }

        /// <summary>
        /// 管理機関大分類コードテキスト
        /// </summary>
        public string Kanri1Text { get; private set; }

        /// <summary>
        /// 管理機関中分類コードテキスト
        /// </summary>
        public string Kanri2Text { get; private set; }

        /// <summary>
        /// 管理機関小分類コードテキスト
        /// </summary>
        public string Kanri3Text { get; private set; }

        /// <summary>
        /// 選択中CSVファイル名テキスト
        /// </summary>
        public string SelectCsvText
        {
            get { return _select_csv_text; }
            private set
            {
                if (value != null && value != _select_csv_text)
                {
                    _select_csv_text = "選択中ファイル：" + value;
                    this.OnPropertyChanged("SelectCsvText");
                }
            }
        }
        private string _select_csv_text;
        private string _select_csv;

        /// <summary>
        /// 選択中フォルダ名テキスト
        /// </summary>
        public string SelectFolderText
        {
            get { return _select_folder_text; }
            private set
            {
                if (value != null && value != _select_folder_text)
                {
                    _select_folder_text = "選択中フォルダ：" + value;
                    this.OnPropertyChanged("SelectFolderText");
                }
            }
        }
        private string _select_folder_text;
        private string _select_folder;

        /// <summary>
        /// 管理機関大分類コードテキストボックス
        /// </summary>
        public string DaiBunruiCodeTextbox
        {
            get { return _dai_bunrui_textbox; }
            set
            {
                _dai_bunrui_number = null;
                if (value != null && value == "")
                {
                    _dai_bunrui_textbox = value;
                    this.OnPropertyChanged("DaiBunruiCodeTextbox");

                    _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                    this.CourseText = _course_list;
                }
                else if (value != null && value != _dai_bunrui_textbox)
                {
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        _dai_bunrui_textbox = value;
                        this.OnPropertyChanged("DaiBunruiCodeTextbox");

                        _dai_bunrui_number = number;
                        _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                        this.CourseText = _course_list;
                    }
                }
            }
        }
        private string _dai_bunrui_textbox;
        private int? _dai_bunrui_number;

        /// <summary>
        /// 管理機関中分類コードテキストボックス
        /// </summary>
        public string ChuBunruiCodeTextbox
        {
            get { return _chu_bunrui_textbox; }
            set
            {
                _chu_bunrui_number = null;
                if (value != null && value == "")
                {
                    _chu_bunrui_textbox = value;
                    this.OnPropertyChanged("ChuBunruiCodeTextbox");

                    _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                    this.CourseText = _course_list;
                }
                else if (value != null && value != _chu_bunrui_textbox)
                {
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        _chu_bunrui_textbox = value;
                        this.OnPropertyChanged("ChuBunruiCodeTextbox");

                        _chu_bunrui_number = number;
                        _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                        this.CourseText = _course_list;
                    }
                }
            }
        }
        private string _chu_bunrui_textbox;
        private int? _chu_bunrui_number;

        /// <summary>
        /// 管理機関小分類コードテキストボックス
        /// </summary>
        public string ShoBunruiCodeTextbox
        {
            get { return _sho_bunrui_textbox; }
            set
            {
                _sho_bunrui_number = null;
                if (value != null && value == "")
                {
                    _sho_bunrui_textbox = value;
                    this.OnPropertyChanged("ShoBunruiCodeTextbox");

                    _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                    this.CourseText = _course_list;
                }
                else if (value != null && value != _sho_bunrui_textbox)
                {
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        _sho_bunrui_textbox = value;
                        this.OnPropertyChanged("ShoBunruiCodeTextbox");

                        _sho_bunrui_number = number;
                        _course_list = MergeCourse(DuplicationCheck(_course_csv_list), _course_point_list);
                        this.CourseText = _course_list;
                    }
                }
            }
        }
        private string _sho_bunrui_textbox;
        private int? _sho_bunrui_number;

        /// <summary>
        /// コースバインディング
        /// </summary>
        public List<Course> CourseText
        {
            get { return _course_text; }
            private set
            {
                if (value != null)
                {
                    _course_text = value;
                    this.OnPropertyChanged("CourseText");
                }
            }
        }
        private List<Course> _course_text;

        /// <summary>
        /// Logテキスト
        /// </summary>
        public string LogText
        {
            get { return _log_text; }
            private set
            {
                if (value != null && value != _log_text)
                {
                    _log_text = value + "\n";
                    this.OnPropertyChanged("LogText");
                }
            }
        }
        private string _log_text;

    }


}
