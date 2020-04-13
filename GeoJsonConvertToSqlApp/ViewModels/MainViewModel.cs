using GeoJsonConvertToSqlApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GeoJsonConvertToSqlApp.ViewModels
{
    class MainViewModel : NotifyChangedBase
    {
        public MainViewModel()
        {
            // テキスト表示
            this.CsvText = "巡回コースマスタCSVファイルを選択してください。";
            this.GeojsonText = "GoeJSONファイルを取得します。\nGeoJSONファイルのあるフォルダを選択してください。";
            this.Kanri1Text = "管理機関大分類コード：";
            this.Kanri2Text = "中分類コード：";
            this.Kanri3Text = "小分類コード：";
            _select_csv = exists(Util.GetCurrentAppDir() + @"\M_JUNKAI_COURSE.csv");
            _select_folder = exists(Util.GetCurrentAppDir() + @"\geojson");
            this.SelectCsvText = _select_csv;
            this.SelectFolderText = _select_folder;

            // ボタンアクション
            this.OpenFile = new DelegateCommand(
                () =>
                    {
                        System.Windows.Forms.OpenFileDialog ofDialog = new System.Windows.Forms.OpenFileDialog();
                        ofDialog.InitialDirectory = @"C:";
                        ofDialog.Title = "CSVファイル選択";
                        if (ofDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.SelectCsvText = ofDialog.FileName;
                            _csv_list = Util.ReadCsv(ofDialog.FileName);
                            foreach (CourseCsv model in _csv_list)
                            {
                                string txt = "" + model.id + "   " + model.junkai_course_name;
                                this.CourseText = txt;
                            }
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
                            List<CourseJson> list = new List<CourseJson>();
                            this.SelectFolderText = fbDialog.SelectedPath;
                            IEnumerable<string> files = Directory.EnumerateFiles(fbDialog.SelectedPath, "*.geojson");
                            foreach (string str in files)
                            {
                                Console.WriteLine(str);
                                Geometry model = Util.ReadJson(str);
                                //list.Add(model);
                            }

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

        private string exists(string filePath)
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



        private List<CourseCsv> _csv_list;

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
        /// Logテキスト
        /// </summary>
        public string CourseText
        {
            get { return _course_text; }
            private set
            {
                if (value != null && value != _course_text)
                {
                    _course_text += value + "\n";
                    this.OnPropertyChanged("CourseText");
                }
            }
        }
        private string _course_text;

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
