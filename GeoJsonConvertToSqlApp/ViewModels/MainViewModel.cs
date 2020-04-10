using GeoJsonConvertToSqlApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.ViewModels
{
    class MainViewModel : NotifyChangedBase
    {
        /// <summary>
        /// フォルダを開く
        /// </summary>
        public DelegateCommand OpenFile { get; private set; }
        public DelegateCommand OpenFolder { get; private set; }
        private int cnt = 1;

        public MainViewModel()
        {
            this.CsvText = "巡回コースマスタCSVファイルを選択してください。";
            this.GeojsonText = "GoeJSONファイルをSQLファイルに変換します。\n「開く」ボタンから変換するGeoJSONファイルのあるフォルダを選択してください。";
            this.OpenFile = new DelegateCommand(
                () =>
                {
                    this.CsvText = "" + cnt;
                    cnt++;
                },
                () => true
                );
            this.OpenFolder = new DelegateCommand(
                () =>
                {
                    this.GeojsonText = "" + cnt;
                    cnt++;
                },
                () => true
                );
        }

        public string CsvText
        {
            get { return _csv_text; }
            private set
            {
                if (value != null && value != _csv_text)
                {
                    _csv_text = value;
                    this.OnPropertyChanged("CsvText");
                }
            }
        }
        private string _csv_text;

        public string GeojsonText
        {
            get { return _geojson_text; }
            private set
            {
                if (value != null && value != _geojson_text)
                {
                    _geojson_text = value;
                    this.OnPropertyChanged("GeojsonText");
                }
            }
        }
        private string _geojson_text;

    }


}
