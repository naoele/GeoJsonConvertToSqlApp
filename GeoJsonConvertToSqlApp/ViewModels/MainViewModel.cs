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
        public DelegateCommand OpenFolder { get; private set; }
        private int cnt = 1;

        public MainViewModel()
        {
            this.Text = "GoeJSONファイルをSQLファイルに変換します。\n「開く」ボタンから変換するGeoJSONファイルのあるフォルダを選択してください。";
            this.OpenFolder = new DelegateCommand(
                () =>
                {
                    this.Text = "" + cnt;
                    cnt++;
                },
                () => true
                );
        }

        public string Text
        {
            get { return _text; }
            private set
            {
                if (value != null && value != _text)
                {
                    _text = value;
                    this.OnPropertyChanged("Text");
                }
            }
        }
        private string _text;

    }


}
