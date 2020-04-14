using GeoJsonConvertToSqlApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            _select_csv = Exists(Util.GetCurrentAppDir() + @"\M_JUNKAI_COURSE.csv");
            _select_folder = Exists(Util.GetCurrentAppDir() + @"\geojson");
            this.SelectCsvText = StoreCsvData(_select_csv);
            this.SelectFolderText = StoreJsonData(_select_folder);

            // ボタンアクション
            this.OpenFile = new DelegateCommand(
                () =>
                    {
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
        }

        private string Exists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                string err = "選択している " + filePath + " は存在しません";
                Console.WriteLine(err);
                //this.LogText = err;
                return "";
            }
            return filePath;
        }

        private void DuplicationCheck(List<CourseCsv> list)
        {
            if (list == null) return;
            string errMsg = Util.CheckDuplication(list);
            if (errMsg != "")
            {
                Console.WriteLine(errMsg);
                this.LogText = errMsg;
            }

            this.CourseText = list;
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
                DuplicationCheck(_course_csv_list);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.LogText = e.StackTrace;
            }
            return filePath;
        }

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

                foreach (CoursePoint model in _course_point_list)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.LogText = e.StackTrace;
            }
            return dirPath;
        }

        public void ExtractOnlyCdKikan1(int number)
        {
            if (_course_csv_list == null) return;
            List<CourseCsv> list = new List<CourseCsv>();
            foreach (CourseCsv model in _course_csv_list)
            {
                if (model.Cd_kikan1 == number)
                {
                    list.Add(model);
                }
            }
            DuplicationCheck(list);
        }

        public void ExtractOnlyCdKikan2(int number)
        {
            if (_course_csv_list == null) return;
            List<CourseCsv> list = new List<CourseCsv>();
            foreach (CourseCsv model in _course_csv_list)
            {
                if (model.Cd_kikan2 == number)
                {
                    list.Add(model);
                }
            }
            DuplicationCheck(list);
        }

        public void ExtractOnlyCdKikan3(int number)
        {
            if (_course_csv_list == null) return;
            List<CourseCsv> list = new List<CourseCsv>();
            foreach (CourseCsv model in _course_csv_list)
            {
                if (model.Cd_kikan3 == number)
                {
                    list.Add(model);
                }
            }
            DuplicationCheck(list);
        }

        /// <summary>
        /// 
        /// </summary>
        private List<CoursePoint> _course_point_list;

        /// <summary>
        /// 
        /// </summary>
        private List<CourseCsv> _course_csv_list;

        /// <summary>
        /// コースCSVファイルを開く
        /// </summary>
        public DelegateCommand OpenFile { get; private set; }

        /// <summary>
        /// geojsonフォルダを開く
        /// </summary>
        public DelegateCommand OpenFolder { get; private set; }

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
                if (value != null && value == "")
                {
                    _dai_bunrui_textbox = value;
                    this.OnPropertyChanged("DaiBunruiCodeTextbox");

                    DuplicationCheck(_course_csv_list);
                }
                else if (value != null && value != _dai_bunrui_textbox)
                {
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        _dai_bunrui_textbox = value;
                        this.OnPropertyChanged("DaiBunruiCodeTextbox");

                        ExtractOnlyCdKikan1(number);
                    }
                }
            }
        }
        private string _dai_bunrui_textbox;

        /// <summary>
        /// 管理機関中分類コードテキストボックス
        /// </summary>
        public string ChuBunruiCodeTextbox
        {
            get { return _chu_bunrui_textbox; }
            set
            {
                if (value != null && value == "")
                {
                    _chu_bunrui_textbox = value;
                    this.OnPropertyChanged("ChuBunruiCodeTextbox");

                    DuplicationCheck(_course_csv_list);
                }
                else if (value != null && value != _chu_bunrui_textbox)
                {
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        _chu_bunrui_textbox = value;
                        this.OnPropertyChanged("ChuBunruiCodeTextbox");

                        ExtractOnlyCdKikan2(number);
                    }
                }
            }
        }
        private string _chu_bunrui_textbox;

        /// <summary>
        /// 管理機関小分類コードテキストボックス
        /// </summary>
        public string ShoBunruiCodeTextbox
        {
            get { return _sho_bunrui_textbox; }
            set
            {
                if (value != null && value == "")
                {
                    _sho_bunrui_textbox = value;
                    this.OnPropertyChanged("ShoBunruiCodeTextbox");

                    DuplicationCheck(_course_csv_list);
                }
                else if (value != null && value != _sho_bunrui_textbox)
                {
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        _sho_bunrui_textbox = value;
                        this.OnPropertyChanged("ShoBunruiCodeTextbox");

                        ExtractOnlyCdKikan3(number);
                    }
                }
            }
        }
        private string _sho_bunrui_textbox;

        /// <summary>
        /// コースバインディング
        /// </summary>
        public List<CourseCsv> CourseText
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
        private List<CourseCsv> _course_text;

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
                    _log_text += value + "\n";
                    this.OnPropertyChanged("LogText");
                }
            }
        }
        private string _log_text;

    }


}
