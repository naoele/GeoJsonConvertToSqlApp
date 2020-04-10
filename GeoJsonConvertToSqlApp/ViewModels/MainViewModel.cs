using GeoJsonConvertToSqlApp.Models;
using Microsoft.Win32;
using System;
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
            this.SelectCsvText = Util.GetCurrentAppDir() + @"\M_JUNKAI_COURSE.csv";
            this.SelectFolderText = Util.GetCurrentAppDir() + @"\geojson";

            // ボタンアクション
            this.OpenFile = new DelegateCommand(
                () =>
                    {
                        System.Windows.Forms.OpenFileDialog ofDialog = new System.Windows.Forms.OpenFileDialog();
                        ofDialog.InitialDirectory = @"C:";
                        ofDialog.Title = "CSVファイル選択";
                        if (ofDialog.ShowDialog() == DialogResult.OK)
                        {
                            Console.WriteLine(ofDialog.FileName);
                            this.SelectCsvText = ofDialog.FileName;
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
                    System.Windows.Forms.FolderBrowserDialog fbDialog = new System.Windows.Forms.FolderBrowserDialog();
                    fbDialog.SelectedPath = @"C:";
                    fbDialog.Description = "GeoJSONフォルダ選択";
                    fbDialog.ShowNewFolderButton = true;
                    if (fbDialog.ShowDialog() == DialogResult.OK)
                    {
                        Console.WriteLine(fbDialog.SelectedPath);
                        this.SelectFolderText = fbDialog.SelectedPath;
                    }
                    else
                    {
                        Console.WriteLine("キャンセルされました");
                        this.LogText = "キャンセルされました";
                    }
                    fbDialog.Dispose();
                },
                () => true
            );
        }

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
