using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Microsoft.VisualBasic.FileIO;
using CreteLib;
// http://csfun.blog49.fc2.com/blog-entry-19.html

namespace WeatherBrowser
{
    public partial class Form1 : Form
    {
        public static string url_smallregion = "";
        public static int selected_l = 0;
        public static int selected_m = 0;
        public static int selected_n = 0;

        public static string l_name = string.Empty;
        public static string m_name = string.Empty;
        public static string n_name = string.Empty;
        public static string url_warn = string.Empty;
        public static string url_abst = string.Empty;

        public static Boolean conf_ok = false;

        // 履歴保持用
        public static int back_max = 10;
        List<string> back_url_smallregion = new List<string>();
        //List<string> back_url_m = new List<string>();
        List<string> back_name = new List<string>();
        List<int> back_l = new List<int>();
        List<int> back_m = new List<int>();
        List<int> back_n = new List<int>();
        List<string> back_l_name = new List<string>();
        List<string> back_m_name = new List<string>();
        List<string> back_n_name = new List<string>();
        List<string> back_url_warn = new List<string>();
        List<string> back_url_abst = new List<string>();

        // 色変更設定の都合上，こいつらはグローバル？でないといけない
        List<string> temp_array = new List<string>();
        List<string> wet_array = new List<string>();
        List<string> rain_array = new List<string>();
        List<string> windst_array = new List<string>();

        List<string> max_temp_array2 = new List<string>();
        List<string> min_temp_array2 = new List<string>();
        List<string> rain_array2 = new List<string>();
        List<string> wash_array = new List<string>();
        List<string> ul_vio_array = new List<string>();
        List<string> umb_array = new List<string>();
        List<string> star_array = new List<string>();
        bool[,] notice_array = new bool[2, 9];
        bool[,] warning_array = new bool[2, 9];
        string wash_start_day = string.Empty;
        string ul_vio_start_day = string.Empty;
        string umb_start_day = string.Empty;
        string star_start_day = string.Empty;
        string warning_region = string.Empty;

        // すでに過ぎて灰色になっている個数
        // 天気，降水量等灰色になっている数が常に等しいという前提
        int gray_num = 0;
        int gray_num2 = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ロード時に初期パスフォルダをオープン
            // 設定ファイルへのパスを取得
            System.Configuration.Configuration config =
                System.Configuration.ConfigurationManager.OpenExeConfiguration(
                System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);

            // 設定ファイルがなければ，前のバージョンから引き継ぐ
            if (!System.IO.File.Exists(config.FilePath))
            {
                Properties.Settings.Default.Upgrade();
            }

            // CreteLib.dllがない場合のエラー処理
            if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "/CreteLib.dll"))
            {
                MessageBox.Show(
                    "CreteLib.dllが見つかりません。本体と同じフォルダにCreteLib.dllを置いてください。",
                    "エラー");
                this.Close();
            }

            // cacheフォルダがない場合は作成する
            if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "/cache/"))
            {
                System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "/cache/");
            }

            // 履歴を設定から読み出す処理
            back_l_name = this.stringToStringList(Properties.Settings.Default.back_l_name);
            back_m_name = this.stringToStringList(Properties.Settings.Default.back_m_name);
            back_n_name = this.stringToStringList(Properties.Settings.Default.back_n_name);
            back_l = this.stringToIntList(Properties.Settings.Default.back_l);
            back_m = this.stringToIntList(Properties.Settings.Default.back_m);
            back_n = this.stringToIntList(Properties.Settings.Default.back_n);
            back_name = this.stringToStringList(Properties.Settings.Default.back_name);
            back_url_smallregion = this.stringToStringList(Properties.Settings.Default.back_url_smallregion);
            back_url_warn = this.stringToStringList(Properties.Settings.Default.back_url_warn);
            back_url_abst = this.stringToStringList(Properties.Settings.Default.back_url_abst);

            if (url_smallregion != "")
            {
                back_l.Add(selected_l);
                back_m.Add(selected_m);
                back_n.Add(selected_n);
                back_l_name.Add(l_name);
                back_m_name.Add(m_name);
                back_n_name.Add(n_name);
                back_url_smallregion.Add(url_smallregion);
                back_url_warn.Add(url_warn);
                back_url_abst.Add(url_abst);
            }

            // 履歴最大数を超えたら、最初のデータを消す
            if (back_name.Count > back_max)
            {
                back_name.RemoveAt(0);
                back_l.RemoveAt(0);
                back_m.RemoveAt(0);
                back_n.RemoveAt(0);
                back_l_name.RemoveAt(0);
                back_m_name.RemoveAt(0);
                back_n_name.RemoveAt(0);
                back_url_smallregion.RemoveAt(0);
                back_url_warn.RemoveAt(0);
                back_url_abst.RemoveAt(0);
            }

            if (back_url_smallregion.Count > 1)
            {
                履歴BToolStripMenuItem.DropDownItems.Clear();

                // 履歴と逆順にメニューに表示する
                // つまり最新の履歴が一番上に来るように表示
                for (int i = back_name.Count - 1; i >= 0; i--)
                {
                    履歴BToolStripMenuItem.DropDownItems.Add(back_name[i] + "(&"
                        + ((back_name.Count - i) % 10) + ")");
                }

                // 新たに履歴に登録したもののチェックをtrueにしておく
                ToolStripMenuItem toolStripMenuItem =
                    (ToolStripMenuItem)履歴BToolStripMenuItem.DropDownItems[0];
                toolStripMenuItem.CheckOnClick = true;
                toolStripMenuItem.Checked = true;

                // Clickイベントハンドラを追加する
                toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);

                // それ以外はfalseにしておく
                for (int i = 1; i < 履歴BToolStripMenuItem.DropDownItems.Count; i++)
                {
                    toolStripMenuItem = (ToolStripMenuItem)履歴BToolStripMenuItem.DropDownItems[i];
                    toolStripMenuItem.CheckOnClick = true;
                    toolStripMenuItem.Checked = false;

                    // Clickイベントハンドラを追加する
                    toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);
                }
            }

            selected_l = Properties.Settings.Default.l;
            selected_m = Properties.Settings.Default.m;
            selected_n = Properties.Settings.Default.n;


            // 設定がある場合は設定からURLを読み出す
            if (selected_l < 0 && selected_m < 0 && selected_n < 0)
            {
                // ない場合は設定ダイアログを開いたときにエラーにならないように0にしておく
                selected_l = 0;
                selected_m = 0;
                selected_n = 0;
            }

            // デフォルトで警報・注意報の文字列を入れておく
            ResetListView3();

            try
            {
                TextFieldParser parser = new TextFieldParser("regionlist3.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                string _l_name = "";
                string _m_name = "";
                string _n_name = "";

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (int.Parse(row[0]) == selected_l && int.Parse(row[1]) == selected_m && int.Parse(row[2]) == selected_n)
                        {
                            url_smallregion = row[4];
                            _n_name = row[3];
                            break;
                        }
                    }
                }

                parser = new TextFieldParser("regionlist2.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (int.Parse(row[0]) == selected_l && int.Parse(row[1]) == selected_m)
                        {
                            _m_name = row[2];
                            break;
                        }
                    }
                }

                parser = new TextFieldParser("regionlist1.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    int k = 0;

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (k == selected_l)
                        {
                            _l_name = row[0];
                            break;
                        }
                        k++;
                    }
                }

                parser = new TextFieldParser("regionlist2_2.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    //MessageBox.Show("_l_name:" + _l_name + ", _n_name:" + _n_name);
                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (_l_name.Contains(row[0]) && _n_name.Contains(row[2]))
                        {
                            url_warn = row[3];
                            url_abst = row[1];
                            //MessageBox.Show(url_warn);
                            break;
                        }
                    }
                }
                label5.Text = "【現在の警報・注意報】－" + _n_name;
                label2.Text = "【今日・明日の天気】－" + _n_name;
                label1.Text = "【週間天気】－" + _l_name + "＞" + _m_name;
                label7.Text = "【天気概況】－" + _l_name;

                l_name = _l_name;
                m_name = _m_name;
                n_name = _n_name;
            }
            catch (Exception e2)
            {
                // 地域ファイルが存在しない場合
                MessageBox.Show(
                    "地域設定ファイルが見つかりません。設定を開き、データ取得ボタンを押してください。",
                    "設定ファイルチェック");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "情報取得中...";
            button1.Refresh();

            string enc = "EUC-JP";
            string[] st = url_smallregion.Split('/');
            string[] st_2 = st[7].Split('.');

            string url_l = st[5] + ".html";
            string url_m = st[5] + "/" + st[6] + ".html";

            string st_l = st[5];
            string st_m = st[5] + "_" + st[6];
            string st_n = st[5] + "_" + st[6] + "_" + st_2[0];

            // ------------------------------------------------------
            //ダウンロードするURL（ピンポイント天気）
            //string url = "http://weather.yahoo.co.jp/weather/jp/14/4610/14134.html";
            //string url = "http://weather.yahoo.co.jp/weather/jp/12/4520/12423.html";
            string url = url_smallregion;

            string source = "";

            try
            {
                //WebClientの作成
                WebClient wc = new WebClient();
                //文字コードを指定
                wc.Encoding = Encoding.GetEncoding(65001);
                //HTMLソースをダウンロードする
                source = wc.DownloadString(url);
                //後始末
                wc.Dispose();

                string filePath = @"./cache/weathern_" + st_n + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nまた、接続先サーバが正常通り稼働しているか確認してください\nURL:" + url,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            //MessageBox.Show(url_smallregion);
            CreteLib.HtmlDocument doc = new CreteLib.HtmlDocument();
            //try
            //{
                doc.LoadHtml(source);
            //}
            //catch (Exception e2)
            //{
            //}

            // ------------------------------------------------------
            //ダウンロードするURL（洗濯）
            //string url2 = "http://weather.yahoo.co.jp/weather/jp/expo/clothdried/14/4610.html";
            string url2 = "http://weather.yahoo.co.jp/weather/jp/expo/clothdried/" + url_m;

            string source2 = "";

            try
            {
                //WebClientの作成
                WebClient wc2 = new WebClient();
                //文字コードを指定
                wc2.Encoding = Encoding.GetEncoding(65001);
                //HTMLソースをダウンロードする
                source2 = wc2.DownloadString(url2);
                //後始末
                wc2.Dispose();

                string filePath = @"./cache/clothdried_" + st_m + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url2,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument doc2 = new CreteLib.HtmlDocument();
            try
            {
                doc2.LoadHtml(source2);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（紫外線）
            //string url3 = "http://weather.yahoo.co.jp/weather/jp/expo/uv/14/4610.html";
            string url3 = "http://weather.yahoo.co.jp/weather/jp/expo/uv/" + url_m;

            string source3 = "";

            try
            {
                //WebClientの作成
                WebClient wc3 = new WebClient();
                //文字コードを指定
                wc3.Encoding = Encoding.GetEncoding(65001);
                //HTMLソースをダウンロードする
                source3 = wc3.DownloadString(url3);
                //後始末
                wc3.Dispose();

                string filePath = @"./cache/uv_" + st_m + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url3,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument doc3 = new CreteLib.HtmlDocument();
            try
            {
                doc3.LoadHtml(source3);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（傘）
            //string url4 = "http://weather.yahoo.co.jp/weather/jp/expo/umbrella/14/4610.html";
            string url4 = "http://weather.yahoo.co.jp/weather/jp/expo/umbrella/" + url_m;

            string source4 = "";

            try
            {
                //WebClientの作成
                WebClient wc4 = new WebClient();
                //文字コードを指定
                wc4.Encoding = Encoding.GetEncoding(65001);
                //HTMLソースをダウンロードする
                source4 = wc4.DownloadString(url4);
                //後始末
                wc4.Dispose();

                string filePath = @"./cache/umbrella_" + st_m + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url4,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument doc4 = new CreteLib.HtmlDocument();
            try
            {
                doc4.LoadHtml(source4);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（星空）
            //string url5 = "http://weather.yahoo.co.jp/weather/jp/expo/starry/14/4610.html";
            string url5 = "http://weather.yahoo.co.jp/weather/jp/expo/starry/" + url_m;

            string source5 = "";

            try
            {
                //WebClientの作成
                WebClient wc5 = new WebClient();
                //文字コードを指定
                wc5.Encoding = Encoding.GetEncoding(65001);
                //HTMLソースをダウンロードする
                source5 = wc5.DownloadString(url5);
                //後始末
                wc5.Dispose();

                string filePath = @"./cache/starry_" + st_m + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url5,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument doc5 = new CreteLib.HtmlDocument();
            try
            {
                doc5.LoadHtml(source5);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（地域）
            //string url6 = "http://weather.yahoo.co.jp/weather/jp/14/4610.html";
            string url6 = "http://weather.yahoo.co.jp/weather/jp/" + url_m;

            string source6 = "";

            try
            {
                //WebClientの作成
                WebClient wc6 = new WebClient();
                //文字コードを指定
                wc6.Encoding = Encoding.GetEncoding(65001);
                //HTMLソースをダウンロードする
                source6 = wc6.DownloadString(url6);
                //後始末
                wc6.Dispose();

                string filePath = @"./cache/weatherm_" + st_m + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url6,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument doc6 = new CreteLib.HtmlDocument();
            try
            {
                doc6.LoadHtml(source6);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（警報・注意報）
            //string url7 = "http://typhoon.yahoo.co.jp/weather/jp/keihou/14.html";
            //string url7 = "http://typhoon.yahoo.co.jp/weather/jp/keihou/" + url_l;
            //1.3.5からURL変更
            //string url7 = "http://bousai.tenki.jp/bousai/warn/point-708.html";
            //string url7 = "http://bousai.tenki.jp/bousai/warn/point-207.html";
            //string url7 = "http://bousai.tenki.jp/bousai/warn/point-781.html";
            //string url7 = "http://bousai.tenki.jp/bousai/warn/point-708.html";
            string url7 = "http://bousai.tenki.jp" + url_warn;

            string source7 = "";
            String[] st_warn = url_warn.Split('/');
            String[] st_warn2 = st_warn[2].Split('.');

            try
            {
                //WebClientの作成
                WebClient wc7 = new WebClient();
                //文字コードを指定
                wc7.Encoding = Encoding.UTF8;
                //HTMLソースをダウンロードする
                source7 = wc7.DownloadString(url7);
                //後始末
                wc7.Dispose();

                string filePath = @"./cache/warn_" + st_warn2[0] + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url7,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument doc7 = new CreteLib.HtmlDocument();
            try
            {
                doc7.LoadHtml(source7);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（天気概況）
            string tmp8 = url_abst.Replace("bousai.", "");
            tmp8 = tmp8.Replace("bousai/warn", "forecast");
            string url8 = tmp8;

            string source8 = "";

            try
            {
                //WebClientの作成
                WebClient wc8 = new WebClient();
                //文字コードを指定
                wc8.Encoding = Encoding.UTF8;
                //HTMLソースをダウンロードする
                source8 = wc8.DownloadString(url8);
                //後始末
                wc8.Dispose();

                string filePath = @"./cache/forecast_" + st_warn2[0] + ".cache";

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding(65001));
                sw.Write(source);
                sw.Close();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url8,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument doc8 = new CreteLib.HtmlDocument();
            doc8.LoadHtml(source8);
            // ------------------------------------------------------


            temp_array.Clear();
            wet_array.Clear();
            rain_array.Clear();
            windst_array.Clear();
            max_temp_array2.Clear();
            min_temp_array2.Clear();
            rain_array2.Clear();
            wash_array.Clear();
            ul_vio_array.Clear();
            umb_array.Clear();
            star_array.Clear();
            warning_array.Initialize();
            notice_array.Initialize();
            wash_start_day = string.Empty;
            ul_vio_start_day = string.Empty;
            umb_start_day = string.Empty;
            star_start_day = string.Empty;
            warning_region = string.Empty;

            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 9; k++)
                {
                    warning_array[j,k] = false;
                    notice_array[j,k] = false;
                }
            }

            gray_num2 = 0;

            List<string> date_array = new List<string>();
            List<string> date_array2 = new List<string>();
            List<string> time_array = new List<string>();
            List<string> weather_array = new List<string>();
            List<string> winddr_array = new List<string>();
            List<string> weather_array2 = new List<string>();

            HtmlNodeList nodeList = doc.GetNodesByTagName("span");

            int date_count = 0;

            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("月"))
                {
                    string tmp_date = node.InnerText.Replace('-', ' ');
                    for (int k = 0; k < 8; k++)
                    {
                        date_array.Add(tmp_date.Trim());
                    }
                    // date_array2にも1回分ずつ入れておく
                    date_array2.Add(tmp_date.Trim());
                    date_count++;

                    if (date_count >= 2)
                    {
                        break;
                    }
                }
            }

            //----------------------------------------------------------
            int i = 0;
            Boolean weather_flag = false;
            Boolean time_flag = false;
            Boolean temp_flag2 = false;
            Boolean wet_flag = false;
            Boolean wet_flag2 = false;
            Boolean rain_flag = false;
            Boolean winddr_flag = false;
            Boolean windst_flag = false;
            Boolean date_flag3 = false;
            Boolean weather_flag3 = false;
            Boolean max_temp_flag3 = false;
            Boolean min_temp_flag3 = false;
            Boolean rain_flag3 = false;
            Boolean rain_flag4 = false;
            string rain_string = "";
            
            //----------------------------------------------------------
            i = 0;
            nodeList = doc6.GetNodesByTagName("img");
            foreach (HtmlNode node in nodeList)
            {
                // alt属性のないimgがあるので、try-catchにしておく
                try
                {
                    if (node["src"].Contains("forecast"))
                    {
                        weather_array2.Add(node["alt"]);

                        i++;
                        if (i == 2)
                        {
                            i = 0;
                            break;
                        }
                    }
                }
                catch (Exception e2)
                {
                }
            }

            i = 0;
            nodeList = doc6.GetNodesByTagName("font");
            foreach (HtmlNode node in nodeList){
                if (node.InnerText.Contains("最高気温"))
                {
                    string[] st2 = node.InnerText.Split('>');
                    string[] st1 = st2[1].Split('[');
                    max_temp_array2.Add(st1[0].Trim());
                }
                else if (node.InnerText.Contains("最低気温"))
                {
                    string[] st2 = node.InnerText.Split('>');
                    string[] st1 = st2[1].Split('[');
                    min_temp_array2.Add(st1[0].Trim());

                    i++;
                    if (i == 2)
                    {
                        i = 0;
                        break;
                    }
                }
            }
            
            i = 0;
            int i2 = 0;
            nodeList = doc6.GetNodesByTagName("small");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("降水確率"))
                {
                    rain_flag4 = true;
                }
                else if (rain_flag4)
                {
                    rain_string += node.InnerText;

                    i++;
                    if (i == 4)
                    {
                        rain_array2.Add(rain_string);
                        rain_string = "";
                        rain_flag4 = false;
                        i2++;
                        i = 0;

                        if (i2 == 2)
                        {
                            i2 = 0;
                            break;
                        }
                    }
                    else
                    {
                        rain_string += "/";
                    }
                }
            }
                    
            //------------------------------------------------------------
            i = 0;
            nodeList = doc.GetNodesByTagName("font");
            gray_num = 0;
            int max_num = 0;

            // 先にgray_numとmax_numの最大値を求めてしまう（v1.4.3）
            foreach (HtmlNode node in nodeList)
            {
                // ～時の直後に天気が複数個続く
                if (Regex.IsMatch(node.InnerText, @"^[0-9]+時"))
                {
                    // gray_numの算出
                    if (node.Attributes["color"] == "#999999")
                    {
                        gray_num++;
                    }
                }
            }

            if (gray_num > 8)
            {
                max_num = 8;
            }
            else
            {
                max_num = gray_num;
            }
            //MessageBox.Show("gray_num:" + gray_num + ", max_num:" + max_num);

            bool twice = false;
            bool twice_check = false;

            foreach (HtmlNode node in nodeList)
            {
                //MessageBox.Show(node.InnerText);
                //MessageBox.Show(node.Attributes["color"]);

                // ～時の直後に天気が複数個続く
                if (Regex.IsMatch(node.InnerText, @"^[0-9]+時"))
                {
                    time_array.Add(node.InnerText);
                    weather_flag = true;

                    // 2週目用（夜中3時以降）
                    if (twice_check)
                    {
                        twice = true;
                    }
                }
                else if (weather_flag)
                {
                    twice_check = true;

                    // ここから天気
                    // 2バイト文字が続くなら天気
                    // その直後に気温が8個続く
                    if (node.InnerText.Length * 2 == Encoding.GetEncoding(enc).GetByteCount(node.InnerText))
                    {
                        weather_array.Add(node.InnerText);
                        time_flag = true;
                    }
                    else if (time_flag)
                    {
                        temp_array.Add(node.InnerText);
                        i++;

                        if (twice && i == gray_num - 8 || i == max_num)
                        {
                            weather_flag = false;
                            time_flag = false;
                            wet_flag = true;
                            i = 0;
                        }
                    }
                }
                else if (wet_flag)
                {
                    // ここから湿度
                    // この方法でのgray_num算出は廃止（v1.3.1）
                    //if (node.InnerText.Equals("---"))
                    //{
                    //    gray_num++;
                    //    //textBox1.Text += gray_num.ToString() + "\r\n";
                    //}
                    //MessageBox.Show(node.InnerText);
                    wet_array.Add(node.InnerText);
                    i++;

                    if (twice && i == gray_num - 8 || i == max_num)
                    {
                        wet_flag = false;
                        rain_flag = true;
                        i = 0;
                    }
                }
                else if (rain_flag)
                {
                    // ここから降水量
                    rain_array.Add(node.InnerText);
                    i++;

                    if (twice && i == gray_num - 8 || i == max_num)
                    {
                        rain_flag = false;
                        winddr_flag = true;
                        i = 0;
                    }
                }
                else if (winddr_flag)
                {
                    // 風向と風速は交互
                    winddr_array.Add(node.InnerText);
                    winddr_flag = false;
                    windst_flag = true;
                }
                else if (windst_flag)
                {
                    // 風速
                    windst_array.Add(node.InnerText);
                    winddr_flag = true;
                    windst_flag = false;
                    i++;
                    if (twice && i == gray_num - 8 || i == max_num)
                    {
                        windst_flag = false;
                        winddr_flag = false;
                        // v1.3.3から
                        //temp_flag2 = true;
                        max_temp_flag3 = true;
                        i = 0;
                    }
                }
                // v1.3.3から
                //else if (temp_flag2)
                //{
                //    // ここから気温2
                //    // 数字のみ許可
                //    if (Regex.IsMatch(node.InnerText, @"^-*[0-9]+"))
                //    {
                //        i++;
                //        temp_array.Add(node.InnerText);
                //    }
                //    if (i == gray_num || i == 8)
                //    //if (i == 8)
                //    {
                //        temp_flag2 = false;
                //        wet_flag2 = true;
                //        i = 0;
                //    }
                //}
                //else if (wet_flag2)
                //{
                //    // ここから湿度2
                //    // 数字のみ許可
                //    if (Regex.IsMatch(node.InnerText, @"^[0-9]+"))
                //    {
                //        i++;
                //        wet_array.Add(node.InnerText);
                //    }
                //    if (i == gray_num || i == 8)
                //    //if (i == 8)
                //    {
                //        wet_flag2 = false;
                //        max_temp_flag3 = true;

                //        i = 0;
                //    }
                //}
                else if (max_temp_flag3)
                {
                    // ここから気温3
                    // 最高気温と最低気温は交互
                    if (Regex.IsMatch(node.InnerText, @"^-*[0-9]+"))
                    {
                        i++;
                        max_temp_array2.Add(node.InnerText);
                        // v1.3.3から
                        max_temp_flag3 = false;
                        min_temp_flag3 = true;
                    }
                }
                else if (min_temp_flag3)
                {
                    if (Regex.IsMatch(node.InnerText, @"^-*[0-9]+"))
                    {
                        i++;
                        min_temp_array2.Add(node.InnerText);
                        // v1.3.3から
                        max_temp_flag3 = true;
                        min_temp_flag3 = false;
                    }

                    if (i == 12)
                    {
                        max_temp_flag3 = false;
                        min_temp_flag3 = false;
                        //i = 0;
                        //break;
                    }
                }
            }

            //----------------------------------------------------------
            weather_flag = false;
            rain_flag = false;
            i = 0;
            nodeList = doc.GetNodesByTagName("small");

            foreach (HtmlNode node in nodeList)
            {
                //if (node.InnerText.Contains("時"))
                // ～時の直後に天気が8個続く
                if (Regex.IsMatch(node.InnerText, @"^[0-9]+時"))
                {
                    time_array.Add(node.InnerText);
                    weather_flag = true;
                }
                else if (weather_flag)
                {
                    // ここから天気
                    if (!node.InnerText.Contains("天気"))
                    {
                        if (node.InnerText.Length != 0)
                        {
                            weather_array.Add(node.InnerText);
                        }
                        i++;
                        if (i == 8)
                        {
                            weather_flag = false;
                            i = 0;
                        }
                    }
                }
                else if (node.InnerText.Contains("気温"))
                {
                    temp_flag2 = true;
                }
                else if (temp_flag2)
                {
                    // ここから気温
                    if (node.InnerText.Length != 0)
                    {
                        temp_array.Add(node.InnerText);
                    }
                    i++;
                    if (i == 8)
                    {
                        temp_flag2 = false;
                        i = 0;
                    }
                }
                else if (node.InnerText.Contains("湿度"))
                {
                    wet_flag2 = true;
                }
                else if (wet_flag2)
                {
                    // ここから湿度
                    if (node.InnerText.Length != 0)
                    {
                        wet_array.Add(node.InnerText);
                    }
                    i++;
                    if (i == 8)
                    {
                        wet_flag2 = false;
                        i = 0;
                    }
                }
                else if (node.InnerText.Contains("降水量"))
                {
                    rain_flag = true;
                }
                else if (rain_flag)
                {
                    // ここから降水量
                    if (node.InnerText.Length != 0)
                    {
                        rain_array.Add(node.InnerText);
                    }
                    i++;
                    if (i == 8)
                    {
                        rain_flag = false;
                        i = 0;
                    }
                }
                else if (node.InnerText.Contains("風速"))
                {
                    winddr_flag = true;
                }
                else if (winddr_flag)
                {
                    // 風向と風速は同時
                    if (node.InnerText.Length > 5)
                    {
                        string[] st1 = node.InnerText.Split('<');
                        string[] st2 = node.InnerText.Split('>');

                        winddr_array.Add(st1[0]);
                        windst_array.Add(st2[1]);
                    }

                    i++;
                    if (i == 8)
                    {
                        winddr_flag = false;
                        i = 0;
                    }
                }
                else if (node.InnerText.Equals("日付"))
                {
                    date_flag3 = true;
                }
                else if (date_flag3)
                {
                    string[] st1 = node.InnerText.Split('<');
                    string[] st2 = node.InnerText.Split('>');

                    date_array2.Add(st1[0] + st2[1]);

                    i++;
                    if (i == 6)
                    {
                        date_flag3 = false;
                        i = 0;
                    }
                }
                // <td bgcolor="#eeeeee" width=10%><small>天気</small></td>となっている天気のみ反応するように（v1.3.1）
                else if (node.InnerText.Equals("天気") && node.ParentNode.Attributes.ContainsKey("width"))
                {
                    weather_flag3 = true;
                }
                else if (weather_flag3)
                {
                    weather_array2.Add(node.InnerText);

                    i++;
                    if (i == 6)
                    {
                        weather_flag3 = false;
                        i = 0;
                    }
                }
                // 修正中
                if (node.InnerText.Contains("確率（％）"))
                //if (node.InnerText.Contains("降水"))
                //else if (node.InnerText.Contains("降水"))
                {
                    rain_flag3 = true;
                }
                else if (rain_flag3)
                {
                    rain_array2.Add(node.InnerText);

                    i++;
                    if (i == 6)
                    {
                        rain_flag3 = false;
                        i = 0;
                        break;
                    }
                }
            }

            //----------------------------------------------------------
            i = 0;
            nodeList = doc2.GetNodesByTagName("b");
            // 洗濯指数
            foreach (HtmlNode node in nodeList)
            {
                if (wash_start_day == "")
                {
                    if (node.InnerText.Contains("月"))
                    {
                        wash_start_day = node.InnerText;
                    }
                }
                if (node.InnerText.Contains("指数："))
                {
                    string[] st1 = node.InnerText.Split('：');
                    wash_array.Add(st1[1]);

                    i++;
                    if (i == 2)
                    {
                        i = 0;
                        break;
                    }
                }
            }

            i = 0;
            nodeList = doc2.GetNodesByTagName("small");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("指数："))
                {
                    string[] st1 = node.InnerText.Split('：');
                    wash_array.Add(st1[1]);

                    i++;
                    if (i == 6)
                    {
                        i = 0;
                        break;
                    }
                }
            }

            //----------------------------------------------------------
            i = 0;
            nodeList = doc3.GetNodesByTagName("img");
            // 紫外線指数
            foreach (HtmlNode node in nodeList)
            {
                // alt属性のないimgがあるので、try-catchにしておく
                try
                {
                    // 紫外線指数の取得をimg srcのgif画像名から取得するよう変更（v1.3.1）
                    if (node["alt"].Contains("指数") && node["alt"].EndsWith("指数"))
                    {
                        //string[] st1 = node["alt"].Split('数');
                        //ul_vio_array.Add(st1[1]);
                        string[] st2 = node["src"].Split('0');
                        string[] st1 = st2[1].Split('.');
                        ul_vio_array.Add(st1[0]);

                        i++;
                    }
                }
                catch (Exception e2)
                {
                }
            }
            // 以下は不要（v1.3.1）
            // 紫外線指数のみ、凡例の5つが引っかかるので、
            // あとで後ろから5つを抜いておく
            //for (i = 0; i < 5; i++)
            //{
            //    ul_vio_array.RemoveAt(ul_vio_array.Count - 1);
            //}
            
            // 日付取得用
            nodeList = doc3.GetNodesByTagName("b");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("月"))
                {
                    ul_vio_start_day = node.InnerText;
                    break;
                }
            }

            //----------------------------------------------------------
            i = 0;
            nodeList = doc4.GetNodesByTagName("b");
            // 傘指数
            foreach (HtmlNode node in nodeList)
            {
                if (umb_start_day == "")
                {
                    if (node.InnerText.Contains("月"))
                    {
                        umb_start_day = node.InnerText;
                    }
                }
                if (node.InnerText.Contains("指数："))
                {
                    string[] st1 = node.InnerText.Split('：');
                    umb_array.Add(st1[1]);

                    i++;
                    if (i == 2)
                    {
                        i = 0;
                        break;
                    }
                }
            }

            i = 0;
            nodeList = doc4.GetNodesByTagName("small");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("指数："))
                {
                    string[] st1 = node.InnerText.Split('：');
                    umb_array.Add(st1[1]);

                    i++;
                    if (i == 6)
                    {
                        i = 0;
                        break;
                    }
                }
            }

            ////----------------------------------------------------------
            i = 0;
            nodeList = doc5.GetNodesByTagName("b");
            // 星空指数
            foreach (HtmlNode node in nodeList)
            {
                if (star_start_day == "")
                {
                    if (node.InnerText.Contains("月"))
                    {
                        star_start_day = node.InnerText;
                    }
                }
                if (node.InnerText.Contains("指数："))
                {
                    string[] st1 = node.InnerText.Split('：');
                    star_array.Add(st1[1]);

                    i++;
                    if (i == 2)
                    {
                        i = 0;
                        break;
                    }
                }
            }

            i = 0;
            nodeList = doc5.GetNodesByTagName("small");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("指数："))
                {
                    string[] st1 = node.InnerText.Split('：');
                    star_array.Add(st1[1]);

                    i++;
                    if (i == 6)
                    {
                        i = 0;
                        break;
                    }
                }
            }
            ////----------------------------------------------------------
            // 警報・注意報
            nodeList = doc7.GetNodesByTagName("span");

            bool check_warn = false;
            bool check_notice = false;
            bool check_warn_in = false;
            bool check_notice_in = false;

            foreach (HtmlNode node in nodeList)
            {
                // <span>警報が最初に出てきたところで
                if (!check_warn && node.InnerText.Equals("警報"))
                {
                    check_warn = true;
                }
                else if (check_warn)
                {
                    //MessageBox.Show(node.InnerText);
                    if (node.InnerText.Contains("発表なし"))
                    {
                        // 発表なしなら何もしない
                        check_notice = true;
                        check_warn_in = false;
                        check_warn = false;
                    }
                    else if (node.Attributes.Keys.Count > 0 && node.Attributes["class"] == "is_warn")
                    {
                        warning_array = ChangeStatusWarnNotice(node.InnerText, warning_array);
                        check_warn_in = true;
                    }
                    else if (check_warn_in && node.Attributes.Keys.Count == 0)
                    {
                        if (node.InnerText.Equals("注意報"))
                        {
                            check_notice = true;
                            check_warn_in = false;
                            check_warn = false;
                        }
                    }
                }
                else if (check_notice)
                {
                    //MessageBox.Show("注意報：" + node.InnerText);
                    if (node.InnerText.Contains("発表なし"))
                    {
                        // 発表なしなら何もしない
                        break;
                    }
                    else if (node.Attributes.Keys.Count > 0 && node.Attributes["class"] == "is_alert")
                    {
                        notice_array = ChangeStatusWarnNotice(node.InnerText, notice_array);
                        check_notice_in = true;
                    }
                    else if (check_notice_in)
                    {
                        if (node.Attributes.Keys.Count == 0 || node.Attributes["class"] != "is_alert")
                        break;
                    }
                }
            }

            nodeList = doc7.GetNodesByTagName("h2");

            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("の警報・注意報"))
                {
                    //MessageBox.Show(node.InnerText);
                    Regex reg = new Regex("(?<warning_region>.*?)の警報・注意報");
                    Match m = reg.Match(node.InnerText);
                    warning_region = m.Groups["warning_region"].Value;
                    break;
                }
            }

            // 1.3.5でWebページレイアウト変更のため大幅修正
            //bool check = false;

            //// tdFirstTopの場合は数がずれる・・・
            //foreach (HtmlNode node in nodeList)
            //{
            //    if (node.Attributes.ContainsKey("class") && node.Attributes["class"] == "tdFirst")
            //    {
            //        //MessageBox.Show(n_name + ":" + node.InnerText);
            //        if (n_name.Contains(node.InnerText) || node.InnerText.Contains(n_name))
            //        {
            //            // node.ParentNode.ChildNodes[3].InnerTextで日時
            //            // node.ParentNode.ChildNodes[5].ChildNodes[1].InnerTextで注意報
            //            // node.InnerTextが地域
            //            warning_region = node.InnerText;
            //            if (node.ParentNode.ChildNodes[5].ChildNodes[0].InnerText.Contains("発表なし"))
            //            {
            //                //notice_array.Add("なし");
            //            }
            //            else
            //            {
            //                for (int j = 1; j < node.ParentNode.ChildNodes[5].ChildNodes.Count - 1; j++)
            //                {
            //                    //notice_array.Add(node.ParentNode.ChildNodes[5].ChildNodes[j].InnerText);
            //                    if (node.ParentNode.ChildNodes[5].ChildNodes[j].Attributes["class"] == "textBgYellow")
            //                    {
            //                        notice_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[5].ChildNodes[j].InnerText, notice_array);
            //                    }
            //                    else
            //                    {
            //                        warning_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[5].ChildNodes[j].InnerText, warning_array);
            //                    }
            //                }
            //            }
            //            check = true;
            //            //MessageBox.Show(node.InnerText + "," + tmp_notice_array[0] + "," + tmp_notice_array[1]);
            //            break;
            //        }
            //    }
            //    else if (node.Attributes.ContainsKey("class") && node.Attributes["class"] == "tdFirstTop")
            //    {
            //        if (n_name.Contains(node.InnerText) || node.InnerText.Contains(n_name))
            //        {
            //            // node.ParentNode.ChildNodes[5].InnerTextで日時
            //            // node.ParentNode.ChildNodes[7].ChildNodes[1].InnerTextで注意報
            //            // node.InnerTextが地域
            //            warning_region = node.InnerText;
            //            if (node.ParentNode.ChildNodes[7].ChildNodes[0].InnerText.Contains("発表なし"))
            //            {
            //                //notice_array.Add("なし");
            //            }
            //            else
            //            {
            //                for (int j = 1; j < node.ParentNode.ChildNodes[7].ChildNodes.Count - 1; j++)
            //                {
            //                    //notice_array.Add(node.ParentNode.ChildNodes[7].ChildNodes[j].InnerText);
            //                    if (node.ParentNode.ChildNodes[7].ChildNodes[j].Attributes["class"] == "textBgYellow")
            //                    {
            //                        notice_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[7].ChildNodes[j].InnerText, notice_array);
            //                    }
            //                    else
            //                    {
            //                        warning_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[7].ChildNodes[j].InnerText, warning_array);
            //                    }
            //                }
            //            }
            //            check = true;
            //            //MessageBox.Show(node.InnerText + "," + tmp_notice_array[0] + "," + tmp_notice_array[1]);
            //            break;
            //        }
            //    }
            //}

            //// もし上記の処理で取得できなかった場合のみ、特殊な処理を行う
            //if (!check)
            //{
            //    string[] tmp = n_name.Split('市');
            //    foreach (HtmlNode node in nodeList)
            //    {
            //        if (node.Attributes.ContainsKey("class") && node.Attributes["class"] == "tdFirst")
            //        {
            //            if (tmp[0].Contains(node.InnerText) || node.InnerText.Contains(tmp[0]))
            //            {
            //                warning_region = node.InnerText;
            //                if (node.ParentNode.ChildNodes[5].ChildNodes[0].InnerText.Contains("発表なし"))
            //                {
            //                }
            //                else
            //                {
            //                    for (int j = 1; j < node.ParentNode.ChildNodes[5].ChildNodes.Count - 1; j++)
            //                    {
            //                        if (node.ParentNode.ChildNodes[5].ChildNodes[j].Attributes["class"] == "textBgYellow")
            //                        {
            //                            notice_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[5].ChildNodes[j].InnerText, notice_array);
            //                        }
            //                        else
            //                        {
            //                            warning_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[5].ChildNodes[j].InnerText, warning_array);
            //                        }
            //                    }
            //                }
            //                check = true;
            //                break;
            //            }
            //        }
            //        else if (node.Attributes.ContainsKey("class") && node.Attributes["class"] == "tdFirstTop")
            //        {
            //            if (tmp[0].Contains(node.InnerText) || node.InnerText.Contains(tmp[0]))
            //            {
            //                warning_region = node.InnerText;
            //                if (node.ParentNode.ChildNodes[7].ChildNodes[0].InnerText.Contains("発表なし"))
            //                {
            //                }
            //                else
            //                {
            //                    for (int j = 1; j < node.ParentNode.ChildNodes[7].ChildNodes.Count - 1; j++)
            //                    {
            //                        if (node.ParentNode.ChildNodes[7].ChildNodes[j].Attributes["class"] == "textBgYellow")
            //                        {
            //                            notice_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[7].ChildNodes[j].InnerText, notice_array);
            //                        }
            //                        else
            //                        {
            //                            warning_array = ChangeStatusWarnNotice(node.ParentNode.ChildNodes[7].ChildNodes[j].InnerText, warning_array);
            //                        }
            //                    }
            //                }
            //                check = true;
            //                break;
            //            }
            //        }
            //    }
            //}


            ////----------------------------------------------------------
            String year_month = String.Empty;

            i = 0;
            nodeList = doc.GetNodesByTagName("p");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("発表"))
                {
                    if (i == 0)
                    {
                        string[] st1 = node.InnerText.Split('\n');
                        // 今日明日の天気の日時
                        label3.Text = st1[1];

                        Regex reg = new Regex("(?<year_month>.*?)月");
                        Match m = reg.Match(node.InnerText);
                        year_month = m.Groups["year_month"].Value;
                        //MessageBox.Show(year_month);
                        i++;
                    }
                    else if (i == 1)
                    {
                        label4.Text = node.InnerText;
                        break;
                    }
                }
            }

            nodeList = doc7.GetNodesByTagName("div");

            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("分現在"))
                {
                    string tmp_date = node.InnerText.Replace("日", "日　");
                    // 現在の警報・注意報の日時
                    label6.Text = year_month + "月" + tmp_date;
                }
            }
            // 地域名はn_nameと一致していないことがあるため，ここで修正する
            label5.Text = "【現在の警報・注意報】－" + warning_region;

            ////----------------------------------------------------------
            // 天気概況
            nodeList = doc8.GetNodesByTagName("div");

            foreach (HtmlNode node in nodeList)
            {
                if (node.Attributes.ContainsKey("id") && node.Attributes["id"] == "weatherCountryDescriptionBody")
                {
                    string tmp_text = node.ChildNodes[0].InnerText.Replace("<br>", "\r\n");
                    textBox1.Text = tmp_text;
                    break;
                }
            }

            foreach (HtmlNode node in nodeList)
            {
                if (node.Attributes.ContainsKey("id") && node.Attributes["id"] == "announce_datetime")
                {
                    string tmp_text = node.ChildNodes[0].InnerText.Replace(" ", "　");
                    label8.Text = tmp_text;
                    break;
                }
            }
            
            //----------------------------------------------------------

            //foreach (string s in time_array)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in weather_array)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in temp_array)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in wet_array)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in rain_array)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in winddr_array)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in windst_array)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";
            ////----------------------------------------------------------

            //foreach (string s in date_array2)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in weather_array2)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";

            //foreach (string s in rain_array2)
            //{
            //    textBox1.Text += s + "\r\n";
            //}
            //textBox1.Text += "----------------------------" + "\r\n";
            ////----------------------------------------------------------

            ////ImageListの設定
            ImageList imageListSmall = new ImageList();

            // http://www.bond-d.com/download-blog/2009/04/weather-icon01.html
            for (i = 1; i <= 34; i++)
            {
                string path = null;

                if (i < 10)
                {
                    path = "weatherIcon/0" + i + ".png";
                }
                else
                {
                    path = "weatherIcon/" + i + ".png";
                }
                try
                {
                    imageListSmall.Images.Add(Bitmap.FromFile(path));
                }
                catch (Exception e2)
                {
                    MessageBox.Show(
                        "必要なアイコンファイルが読み込めませんでした。iconフォルダ内に必要なアイコンファイルがあるかどうか確認して下さい。",
                        "エラー");
                    this.Close();
                }
            }

            listView1.Items.Clear();
            listView2.Items.Clear();
            ResetListView3();

            listView1.SmallImageList = imageListSmall;

            //MessageBox.Show(wet_array.Count.ToString());
            for (i = 0; i < 16; i++)
            {
                string parcent = "";
                if (Regex.IsMatch(wet_array[i], @"^[0-9]+$"))
                {
                    parcent = " ％";
                }

                string[] item = {
                                    "",
                                    date_array[i],
                                    time_array[i],
                                    weather_array[i],
                                    temp_array[i] + " ℃",
                                    wet_array[i] + parcent,
                                    rain_array[i] + " mm",
                                    winddr_array[i],
                                    windst_array[i] + " m"
                                };
                listView1.Items.Add(new ListViewItem(item));

                // UseItemStyleForSubItemsをfalseにする
                // subitemごとに色を付け替えるため
                listView1.Items[i].UseItemStyleForSubItems = false;

                // 条件に応じてスタイル変更
                this.ChangeColorView1(i);

                if (weather_array[i].Contains("晴"))
                {
                    listView1.Items[i].ImageIndex = 0;
                }
                else if (weather_array[i].Contains("曇"))
                {
                    listView1.Items[i].ImageIndex = 7;
                }
                else if (weather_array[i].Contains("強雨"))
                {
                    listView1.Items[i].ImageIndex = 29;
                }
                else if (weather_array[i].Contains("雨"))
                {
                    listView1.Items[i].ImageIndex = 14;
                }
                else if (weather_array[i].Contains("湿雪"))
                {
                    listView1.Items[i].ImageIndex = 32;
                }
                else if (weather_array[i].Contains("雪"))
                {
                    listView1.Items[i].ImageIndex = 21;
                }
                else if (weather_array[i].Contains("みぞれ"))
                {
                    listView1.Items[i].ImageIndex = 31;
                }
                else
                {
                    listView1.Items[i].ImageIndex = 28;
                }
            }

            //----------------------------------------------------------

            listView2.SmallImageList = imageListSmall;

            // 洗濯予報やらのみがまだ発表されていない時間帯があるので埋める処理
            if (!date_array2[0].Equals(wash_start_day))
            {
                wash_array.Insert(0, "-");
            }
            if (!date_array2[0].Equals(ul_vio_start_day))
            {
                ul_vio_array.Insert(0, "-");
            }
            if (!date_array2[0].Equals(umb_start_day))
            {
                umb_array.Insert(0, "-");
            }
            if (!date_array2[0].Equals(star_start_day))
            {
                star_array.Insert(0, "-");
                gray_num2++;
            }

            // 7日後の予報がまだ発表されていない時間帯があるので，iの最大値を固定にできない
            for (i = 0; i < date_array2.Count; i++)
            {
                // 洗濯予報やらのみがまだ発表されていない時間帯があるので埋める処理
                if (date_array2.Count != weather_array2.Count && i >= weather_array2.Count)
                {
                    weather_array2.Add("-");
                }
                if (date_array2.Count != max_temp_array2.Count && i >= max_temp_array2.Count)
                {
                    max_temp_array2.Add("-");
                }
                if (date_array2.Count != min_temp_array2.Count && i >= min_temp_array2.Count)
                {
                    min_temp_array2.Add("-");
                }
                if (date_array2.Count != rain_array2.Count && i >= rain_array2.Count)
                {
                    rain_array2.Add("-");
                }
                if (date_array2.Count != wash_array.Count && i >= wash_array.Count)
                {
                    wash_array.Add("-");
                }
                if (date_array2.Count != ul_vio_array.Count && i >= ul_vio_array.Count)
                {
                    ul_vio_array.Add("-");
                }
                if (date_array2.Count != umb_array.Count && i >= umb_array.Count)
                {
                    umb_array.Add("-");
                }
                if (date_array2.Count != star_array.Count && i >= star_array.Count)
                {
                    star_array.Add("-");
                }

                string c = "";
                if (Regex.IsMatch(max_temp_array2[i], @"^-*[0-9]+$"))
                {
                    c = " ℃";
                }
                string parcent = "";
                if (Regex.IsMatch(rain_array2[i], @"^[0-9]+$"))
                {
                    parcent = " ％";
                }

                string[] item = {
                                    "",
                                    date_array2[i],
                                    weather_array2[i],
                                    max_temp_array2[i] + c,
                                    min_temp_array2[i] + c,
                                    rain_array2[i] + parcent,
                                    wash_array[i],
                                    ul_vio_array[i],
                                    umb_array[i],
                                    star_array[i]
                                };
                listView2.Items.Add(new ListViewItem(item));

                // UseItemStyleForSubItemsをfalseにする
                // subitemごとに色を付け替えるため
                listView2.Items[i].UseItemStyleForSubItems = false;

                this.ChangeColorView2(i);

                //// とりあえず
                //if (i >= 2)
                //{
                    switch (weather_array2[i])
                    {
                        case "晴れ":
                            listView2.Items[i].ImageIndex = 0;
                            break;
                        case "晴時々曇":
                            listView2.Items[i].ImageIndex = 1;
                            break;
                        case "晴時々雨":
                            listView2.Items[i].ImageIndex = 2;
                            break;
                        case "晴時々雪":
                            listView2.Items[i].ImageIndex = 3;
                            break;
                        case "晴後曇":
                            listView2.Items[i].ImageIndex = 4;
                            break;
                        case "晴後雨":
                            listView2.Items[i].ImageIndex = 5;
                            break;
                        case "晴後雪":
                            listView2.Items[i].ImageIndex = 6;
                            break;
                        case "曇り":
                            listView2.Items[i].ImageIndex = 7;
                            break;
                        case "曇時々晴":
                            listView2.Items[i].ImageIndex = 8;
                            break;
                        case "曇時々雨":
                            listView2.Items[i].ImageIndex = 9;
                            break;
                        case "曇時々雪":
                            listView2.Items[i].ImageIndex = 10;
                            break;
                        case "曇後晴":
                            listView2.Items[i].ImageIndex = 11;
                            break;
                        case "曇後雨":
                            listView2.Items[i].ImageIndex = 12;
                            break;
                        case "曇後雪":
                            listView2.Items[i].ImageIndex = 13;
                            break;
                        case "雨":
                            listView2.Items[i].ImageIndex = 14;
                            break;
                        case "雨時々晴":
                            listView2.Items[i].ImageIndex = 15;
                            break;
                        case "雨時々曇":
                            listView2.Items[i].ImageIndex = 16;
                            break;
                        case "雨時々雪":
                            listView2.Items[i].ImageIndex = 17;
                            break;
                        case "雨後晴":
                            listView2.Items[i].ImageIndex = 18;
                            break;
                        case "雨後曇":
                            listView2.Items[i].ImageIndex = 19;
                            break;
                        case "雨後雪":
                            listView2.Items[i].ImageIndex = 20;
                            break;
                        case "雪":
                            listView2.Items[i].ImageIndex = 21;
                            break;
                        case "雪時々晴":
                            listView2.Items[i].ImageIndex = 22;
                            break;
                        case "雪時々曇":
                            listView2.Items[i].ImageIndex = 23;
                            break;
                        case "雪時々雨":
                            listView2.Items[i].ImageIndex = 24;
                            break;
                        case "雪後晴":
                            listView2.Items[i].ImageIndex = 25;
                            break;
                        case "雪後曇":
                            listView2.Items[i].ImageIndex = 26;
                            break;
                        case "雪後雨":
                            listView2.Items[i].ImageIndex = 27;
                            break;
                        case "暴風雨":
                            listView2.Items[i].ImageIndex = 30;
                            break;
                        case "暴風雪":
                            listView2.Items[i].ImageIndex = 33;
                            break;
                        default:
                            listView2.Items[i].ImageIndex = 28;
                            break;
                    }
                }
            //}

            //-------------------------------------------------------
            // 警報・注意報
            //FontStyle boldFontStyle = listView3.Font.Style | FontStyle.Bold;
            //Font boldFont = new Font(listView3.Font.Name, listView3.Font.Size, boldFontStyle);

            // UseItemStyleForSubItemsをfalseにする
            // subitemごとに色を付け替えるため
            listView3.Items[0].UseItemStyleForSubItems = false;
            listView3.Items[1].UseItemStyleForSubItems = false;

            Color warningColor = Color.FromArgb(204, 0, 0);
            Color noticeColor = Color.FromArgb(255, 217, 102);

            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 9; k++)
                {
                    if (notice_array[j,k])
                    {
                        listView3.Items[j].SubItems[k + 1].BackColor = noticeColor;
                    }
                    else if (warning_array[j, k])
                    {
                        listView3.Items[j].SubItems[k + 1].ForeColor = Color.White;
                        listView3.Items[j].SubItems[k + 1].BackColor = warningColor;
                    }
                    else
                    {
                        listView3.Items[j].SubItems[k + 1].ForeColor = Color.Gray;
                    }
                }
            }

            button1.Enabled = true;
            button1.Text = "情報取得";
        }

        private void バージョン情報AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 Form2 = new Form2();
            Form2.ShowDialog();

            Form2.Dispose();
        }

        private void 設定OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int old_l = selected_l;
            int old_m = selected_m;
            int old_n = selected_n;

            Form3 Form3 = new Form3();
            Form3.ShowDialog();

            if (conf_ok)
            {
                if (old_l != selected_l || old_m != selected_m || old_n != selected_n)
                {
                    label5.Text = "【現在の警報・注意報】－" + n_name;
                    label2.Text = "【今日・明日の天気】－" + n_name;
                    label1.Text = "【週間天気】－" + l_name + "＞" + m_name;
                    label7.Text = "【天気概況】－" + l_name;

                    string b_name = n_name + "（" + l_name + "＞" + m_name + "）";

                    // 履歴の処理
                    if (back_name.Contains(b_name))
                    {
                        // b_nameは重複しないはずなので，b_nameを使ってindexを取得
                        int i = back_name.IndexOf(b_name);
                        back_name.Remove(b_name);

                        // indexに合致するデータを削除
                        back_l.RemoveAt(i);
                        back_m.RemoveAt(i);
                        back_n.RemoveAt(i);
                        back_l_name.RemoveAt(i);
                        back_m_name.RemoveAt(i);
                        back_n_name.RemoveAt(i);
                        back_url_smallregion.RemoveAt(i);
                        back_url_warn.RemoveAt(i);
                    }

                    back_name.Add(b_name);
                    back_l.Add(selected_l);
                    back_m.Add(selected_m);
                    back_n.Add(selected_n);
                    back_l_name.Add(l_name);
                    back_m_name.Add(m_name);
                    back_n_name.Add(n_name);
                    back_url_smallregion.Add(url_smallregion);
                    back_url_warn.Add(url_warn);

                    // 履歴最大数を超えたら、最初のデータを消す
                    if (back_name.Count > back_max)
                    {
                        back_name.RemoveAt(0);
                        back_l.RemoveAt(0);
                        back_m.RemoveAt(0);
                        back_n.RemoveAt(0);
                        back_l_name.RemoveAt(0);
                        back_m_name.RemoveAt(0);
                        back_n_name.RemoveAt(0);
                        back_url_smallregion.RemoveAt(0);
                        back_url_warn.RemoveAt(0);
                    }

                    履歴BToolStripMenuItem.DropDownItems.Clear();

                    // 履歴と逆順にメニューに表示する
                    // つまり最新の履歴が一番上に来るように表示
                    for (int i = back_name.Count - 1; i >= 0; i--)
                    {
                        履歴BToolStripMenuItem.DropDownItems.Add(back_name[i] + "(&"
                            + ((back_name.Count - i) % 10) + ")");
                    }

                    // 新たに履歴に登録したもののチェックをtrueにしておく
                    ToolStripMenuItem toolStripMenuItem =
                        (ToolStripMenuItem)履歴BToolStripMenuItem.DropDownItems[0];
                    toolStripMenuItem.CheckOnClick = true;
                    toolStripMenuItem.Checked = true;

                    // Clickイベントハンドラを追加する
                    toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);

                    // それ以外はfalseにしておく
                    for (int i = 1; i < 履歴BToolStripMenuItem.DropDownItems.Count; i++)
                    {
                        toolStripMenuItem = (ToolStripMenuItem)履歴BToolStripMenuItem.DropDownItems[i];
                        toolStripMenuItem.CheckOnClick = true;
                        toolStripMenuItem.Checked = false;

                        // Clickイベントハンドラを追加する
                        toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);
                    }

                    listView1.Items.Clear();
                    listView2.Items.Clear();
                    ResetListView3();
                    label3.Text = string.Empty;
                    label4.Text = string.Empty;
                    label6.Text = string.Empty;
                    label8.Text = string.Empty;
                    textBox1.Text = string.Empty;

                    Properties.Settings.Default.back_l_name = this.stringListToString(back_l_name);
                    Properties.Settings.Default.back_m_name = this.stringListToString(back_m_name);
                    Properties.Settings.Default.back_n_name = this.stringListToString(back_n_name);
                    Properties.Settings.Default.back_l = this.intListToString(back_l);
                    Properties.Settings.Default.back_m = this.intListToString(back_m);
                    Properties.Settings.Default.back_n = this.intListToString(back_n);
                    Properties.Settings.Default.back_name = this.stringListToString(back_name);
                    Properties.Settings.Default.back_url_smallregion = this.stringListToString(back_url_smallregion);
                    Properties.Settings.Default.back_url_warn = this.stringListToString(back_url_warn);

                    Properties.Settings.Default.l = selected_l;
                    Properties.Settings.Default.m = selected_m;
                    Properties.Settings.Default.n = selected_n;

                    Properties.Settings.Default.Save();
                }
                else if (temp_array.Count >= 16)
                {
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        this.ChangeColorView1(i);
                    }

                    for (int i = 0; i < listView2.Items.Count; i++)
                    {
                        this.ChangeColorView2(i);
                    }
                }
                conf_ok = false;
            }

            button1.Focus();

            Form3.Dispose();
        }

        // 動的に追加されるコントロールなので，イベントを自作する
        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
            ToolStripMenuItem parentToolStripMenuItem = (ToolStripMenuItem)toolStripMenuItem.OwnerItem;

            int i = 0;

            for (i = 0; i < parentToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (parentToolStripMenuItem.DropDownItems[i].Text == toolStripMenuItem.Text)
                {
                    //MessageBox.Show(i.ToString());
                    break;
                }
            }

            int old_l = selected_l;
            int old_m = selected_m;
            int old_n = selected_n;

            selected_l = back_l[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            selected_m = back_m[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            selected_n = back_n[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            url_smallregion = back_url_smallregion[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            url_warn = back_url_warn[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            n_name = back_n_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];  // これがないと警報・注意報がうまく取れない

            if (old_l != selected_l || old_m != selected_m || old_n != selected_n)
            {
                label5.Text = "【現在の警報・注意報】－" + back_n_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];
                label2.Text = "【今日・明日の天気】－" + back_n_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];
                label1.Text = "【週間天気】－" + back_l_name[parentToolStripMenuItem.DropDownItems.Count - i - 1]
                                + "＞" + back_m_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];
                label7.Text = "【天気概況】－" + back_l_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];

                listView1.Items.Clear();
                listView2.Items.Clear();
                ResetListView3();
                textBox1.Text = string.Empty;
                label3.Text = string.Empty;
                label4.Text = string.Empty;
                label6.Text = string.Empty;
                label8.Text = string.Empty;
            }

            // 上下が入れ替わっているので
            i = parentToolStripMenuItem.DropDownItems.Count - i - 1;

            // 選択されたものを最後にもってくる処理
            // 選択されたもののindexはi
            string tmp_back_name = back_name[i];
            int tmp_back_l = back_l[i];
            int tmp_back_m = back_m[i];
            int tmp_back_n = back_n[i];
            string tmp_back_l_name = back_l_name[i];
            string tmp_back_m_name = back_m_name[i];
            string tmp_back_n_name = back_n_name[i];
            string tmp_back_url_smallregion = back_url_smallregion[i];
            string tmp_back_url_warn = back_url_warn[i];

            back_name.RemoveAt(i);
            back_l.RemoveAt(i);
            back_m.RemoveAt(i);
            back_n.RemoveAt(i);
            back_l_name.RemoveAt(i);
            back_m_name.RemoveAt(i);
            back_n_name.RemoveAt(i);
            back_url_smallregion.RemoveAt(i);
            back_url_warn.RemoveAt(i);

            back_name.Add(tmp_back_name);
            back_l.Add(tmp_back_l);
            back_m.Add(tmp_back_m);
            back_n.Add(tmp_back_n);
            back_l_name.Add(tmp_back_l_name);
            back_m_name.Add(tmp_back_m_name);
            back_n_name.Add(tmp_back_n_name);
            back_url_smallregion.Add(tmp_back_url_smallregion);
            back_url_warn.Add(tmp_back_url_warn);

            履歴BToolStripMenuItem.DropDownItems.Clear();

            // 履歴と逆順にメニューに表示する
            // つまり最新の履歴が一番上に来るように表示
            for (i = back_name.Count - 1; i >= 0; i--)
            {
                履歴BToolStripMenuItem.DropDownItems.Add(back_name[i] + "(&"
                    + ((back_name.Count - i) % 10) + ")");
            }

            // 新たに履歴に登録したもののチェックをtrueにしておく
            toolStripMenuItem =
                (ToolStripMenuItem)履歴BToolStripMenuItem.DropDownItems[0];
            toolStripMenuItem.CheckOnClick = true;
            toolStripMenuItem.Checked = true;

            // Clickイベントハンドラを追加する
            toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);

            // それ以外はfalseにしておく
            for (i = 1; i < 履歴BToolStripMenuItem.DropDownItems.Count; i++)
            {
                toolStripMenuItem = (ToolStripMenuItem)履歴BToolStripMenuItem.DropDownItems[i];
                toolStripMenuItem.CheckOnClick = true;
                toolStripMenuItem.Checked = false;

                // Clickイベントハンドラを追加する
                toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);
            }

            Properties.Settings.Default.back_l_name = this.stringListToString(back_l_name);
            Properties.Settings.Default.back_m_name = this.stringListToString(back_m_name);
            Properties.Settings.Default.back_n_name = this.stringListToString(back_n_name);
            Properties.Settings.Default.back_l = this.intListToString(back_l);
            Properties.Settings.Default.back_m = this.intListToString(back_m);
            Properties.Settings.Default.back_n = this.intListToString(back_n);
            Properties.Settings.Default.back_name = this.stringListToString(back_name);
            Properties.Settings.Default.back_url_smallregion = this.stringListToString(back_url_smallregion);
            Properties.Settings.Default.back_url_warn = this.stringListToString(back_url_warn);

            Properties.Settings.Default.l = selected_l;
            Properties.Settings.Default.m = selected_m;
            Properties.Settings.Default.n = selected_n;

            Properties.Settings.Default.Save();
        }

        /**
         * string型のlistを1つなぎの文字列にして返す
         */
        private string stringListToString(List<string> list)
        {
            string ret = string.Empty;

            foreach (string l in list)
            {
                ret += l + ",";
            }
            return ret;
        }

        /**
         * int型のlistを1つなぎの文字列にして返す
         */
        private string intListToString(List<int> list)
        {
            string ret = string.Empty;

            foreach (int l in list)
            {
                ret += l.ToString() + ",";
            }
            return ret;
        }

        /**
         * コンマ区切りのstringをstring型のlistにして返す
         */
        private List<string> stringToStringList(string st)
        {
            List<string> ret = new List<string>();
            string[] sts = st.Split(',');

            for (int i = 0; i < sts.Length; i++)
            {
                if (sts[i].Length > 0)
                {
                    ret.Add(sts[i]);
                }
            }

            return ret;
        }

        /**
         * コンマ区切りのstringをint型のlistにして返す
         */
        private List<int> stringToIntList(string st)
        {
            List<int> ret = new List<int>();
            string[] sts = st.Split(',');

            for (int i = 0; i < sts.Length; i++)
            {
                if (sts[i].Length > 0)
                {
                    ret.Add(int.Parse(sts[i]));
                }
            }

            return ret;
        }

        private void 終了EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangeColorView1(int i)
        {
            // スタイル定義
            if (i % 2 == 1)
            {
                for (int j = 0; j <= 8; j++)
                {
                    listView1.Items[i].SubItems[j].BackColor = Color.WhiteSmoke;
                }
            }
            else
            {
                for (int j = 0; j <= 8; j++)
                {
                    listView1.Items[i].SubItems[j].BackColor = Color.White;
                }
            }

            if (i < gray_num)
            {
                for (int j = 0; j <= 8; j++)
                {
                    listView1.Items[i].SubItems[j].ForeColor = Color.Gray;
                }
            }

            // 最高気温の上値
            if (Properties.Settings.Default.bool_max_max_temp &&
                int.Parse(temp_array[i]) >= int.Parse(Properties.Settings.Default.max_max_temp))
            {
                listView1.Items[i].SubItems[4].BackColor = Properties.Settings.Default.color_max_max_temp;
            }
            // 最低気温の下値
            if (Properties.Settings.Default.bool_min_min_temp &&
                int.Parse(temp_array[i]) <= int.Parse(Properties.Settings.Default.min_min_temp))
            {
                listView1.Items[i].SubItems[4].BackColor = Properties.Settings.Default.color_min_min_temp;
            }
            // 湿度の上値
            if ((Regex.IsMatch(wet_array[i], @"^[0-9]+$")) &&
                Properties.Settings.Default.bool_max_wet &&
                int.Parse(wet_array[i]) >= int.Parse(Properties.Settings.Default.max_wet))
            {
                listView1.Items[i].SubItems[5].BackColor = Properties.Settings.Default.color_max_wet;
            }
            // 湿度の下値
            if ((Regex.IsMatch(wet_array[i], @"^[0-9]+$")) &&
                Properties.Settings.Default.bool_min_wet &&
                int.Parse(wet_array[i]) <= int.Parse(Properties.Settings.Default.min_wet))
            {
                listView1.Items[i].SubItems[5].BackColor = Properties.Settings.Default.color_min_wet;
            }
            // 降水量の上値
            if (Properties.Settings.Default.bool_max_rain &&
                int.Parse(rain_array[i]) >= int.Parse(Properties.Settings.Default.max_rain))
            {
                listView1.Items[i].SubItems[6].BackColor = Properties.Settings.Default.color_max_rain;
            }
            // 降水量の下値
            if (Properties.Settings.Default.bool_min_rain &&
                int.Parse(rain_array[i]) <= int.Parse(Properties.Settings.Default.min_rain))
            {
                listView1.Items[i].SubItems[6].BackColor = Properties.Settings.Default.color_min_rain;
            }
            // 風速の上値
            if (Properties.Settings.Default.bool_max_wind &&
                int.Parse(windst_array[i]) >= int.Parse(Properties.Settings.Default.max_wind))
            {
                listView1.Items[i].SubItems[8].BackColor = Properties.Settings.Default.color_max_wind;
            }
            // 風速の下値
            if (Properties.Settings.Default.bool_min_wind &&
                int.Parse(windst_array[i]) <= int.Parse(Properties.Settings.Default.min_wind))
            {
                listView1.Items[i].SubItems[8].BackColor = Properties.Settings.Default.color_min_wind;
            }
        }

        private void ChangeColorView2(int i)
        {
            // スタイル定義
            if (i % 2 == 1)
            {
                //MessageBox.Show(listView2.Items.Count.ToString());

                //for (int j = 0; j <= 9; j++)
                for (int j = 0; j <= 9; j++)
                {
                    listView2.Items[i].SubItems[j].BackColor = Color.WhiteSmoke;
                }
            }
            else
            {
                for (int j = 0; j <= 9; j++)
                {
                    listView2.Items[i].SubItems[j].BackColor = Color.White;
                }
            }
            if (i < gray_num2)
            {
                for (int j = 0; j <= 8; j++)
                {
                    listView2.Items[i].SubItems[j].ForeColor = Color.Gray;
                }
            }

            // 最高気温の上値
            try
            {
                if (Properties.Settings.Default.bool_max_max_temp &&
                    int.Parse(max_temp_array2[i]) >= int.Parse(Properties.Settings.Default.max_max_temp))
                {
                    listView2.Items[i].SubItems[3].BackColor = Properties.Settings.Default.color_max_max_temp;
                }
            }
            catch (FormatException e)
            {
            }
            // 最高気温の下値
            try
            {
                if (Properties.Settings.Default.bool_max_min_temp &&
                    int.Parse(max_temp_array2[i]) <= int.Parse(Properties.Settings.Default.max_min_temp))
                {
                    listView2.Items[i].SubItems[3].BackColor = Properties.Settings.Default.color_max_min_temp;
                }
            }
            catch (FormatException e)
            {
            }
            // 最低気温の上値
            try
            {
                if (Properties.Settings.Default.bool_min_max_temp &&
                    int.Parse(min_temp_array2[i]) >= int.Parse(Properties.Settings.Default.min_max_temp))
                {
                    listView2.Items[i].SubItems[4].BackColor = Properties.Settings.Default.color_min_max_temp;
                }
            }
            catch (FormatException e)
            {
            }
            // 最低気温の下値
            try
            {
                if (Properties.Settings.Default.bool_min_min_temp &&
                    int.Parse(min_temp_array2[i]) <= int.Parse(Properties.Settings.Default.min_min_temp))
                {
                    listView2.Items[i].SubItems[4].BackColor = Properties.Settings.Default.color_min_min_temp;
                }
            }
            catch (FormatException e)
            {
            }
            // 洗濯指数の上値
            try
            {
                if (Properties.Settings.Default.bool_max_wash &&
                    int.Parse(wash_array[i]) >= int.Parse(Properties.Settings.Default.max_wash))
                {
                    listView2.Items[i].SubItems[6].BackColor = Properties.Settings.Default.color_max_wash;
                }
            }
            catch (FormatException e)
            {
            }
            // 洗濯指数の下値
            // 以下、未発表で"-"が入っている場合にIntにParseできないため、try-catchで受け流す
            try
            {
                if (Properties.Settings.Default.bool_min_wash &&
                    int.Parse(wash_array[i]) <= int.Parse(Properties.Settings.Default.min_wash))
                {
                    listView2.Items[i].SubItems[6].BackColor = Properties.Settings.Default.color_min_wash;
                }
            }
            catch (FormatException e)
            {
            }
            // 紫外線指数の上値
            try
            {
                if (Properties.Settings.Default.bool_max_ult_vio &&
                    int.Parse(ul_vio_array[i]) >= int.Parse(Properties.Settings.Default.max_ul_vio))
                {
                    listView2.Items[i].SubItems[7].BackColor = Properties.Settings.Default.color_max_ul_vio;
                }
            }
            catch (FormatException e)
            {
            }
            // 紫外線指数の下値
            try
            {
                if (Properties.Settings.Default.bool_min_ult_vio &&
                    int.Parse(ul_vio_array[i]) <= int.Parse(Properties.Settings.Default.min_ul_vio))
                {
                    listView2.Items[i].SubItems[7].BackColor = Properties.Settings.Default.color_min_ul_vio;
                }
            }
            catch (FormatException e)
            {
            }
            // 傘指数の上値
            try
            {
                if (Properties.Settings.Default.bool_max_umb &&
                    int.Parse(umb_array[i]) >= int.Parse(Properties.Settings.Default.max_umb))
                {
                    listView2.Items[i].SubItems[8].BackColor = Properties.Settings.Default.color_max_umb;
                }
            }
            catch (FormatException e)
            {
            }
            // 傘指数の下値
            try
            {
                if (Properties.Settings.Default.bool_min_umb &&
                    int.Parse(umb_array[i]) <= int.Parse(Properties.Settings.Default.min_umb))
                {
                    listView2.Items[i].SubItems[8].BackColor = Properties.Settings.Default.color_min_umb;
                }
            }
            catch (FormatException e)
            {
            }
            // 星空指数の上値
            try
            {
                if (Properties.Settings.Default.bool_max_star &&
                    int.Parse(star_array[i]) >= int.Parse(Properties.Settings.Default.max_star))
                {
                    listView2.Items[i].SubItems[9].BackColor = Properties.Settings.Default.color_max_star;
                }
            }
            catch (FormatException e)
            {
            }
            // 星空指数の下値
            try
            {
                if (Properties.Settings.Default.bool_min_star &&
                    int.Parse(star_array[i]) <= int.Parse(Properties.Settings.Default.min_star))
                {
                    listView2.Items[i].SubItems[9].BackColor = Properties.Settings.Default.color_min_star;
                }
            }
            catch (FormatException e)
            {
            }
        }

        private void ResetListView3()
        {
            listView3.Items.Clear();

            // デフォルトで警報・注意報の文字列を入れておく
            // 1つ目のアイテムはalign=centerにできないため、空欄にしてごまかす
            List<string> list_item3 = new List<string>();
            list_item3.Add("");
            list_item3.Add("大雨");
            list_item3.Add("洪水");
            list_item3.Add("暴風");
            list_item3.Add("強風");
            list_item3.Add("暴風雪");
            list_item3.Add("風雪");
            list_item3.Add("大雪");
            list_item3.Add("波浪");
            list_item3.Add("高潮");

            listView3.Items.Add(new ListViewItem(list_item3.ToArray()));
            list_item3.Clear();

            list_item3.Add("");
            list_item3.Add("雷");
            list_item3.Add("融雪");
            list_item3.Add("濃霧");
            list_item3.Add("乾燥");
            list_item3.Add("なだれ");
            list_item3.Add("低温");
            list_item3.Add("霜");
            list_item3.Add("着氷");
            list_item3.Add("着雪");

            listView3.Items.Add(new ListViewItem(list_item3.ToArray()));
            list_item3.Clear();
        }

        private bool[,] ChangeStatusWarnNotice(string text, bool[,] array){
            switch(text){
                case "大雨":
                    array[0,0] = true;
                    break;
                case "洪水":
                    array[0,1] = true;
                    break;
                case "暴風":
                    array[0,2] = true;
                    break;
                case "強風":
                    array[0,3] = true;
                    break;
                case "暴風雪":
                    array[0,4] = true;
                    break;
                case "風雪":
                    array[0,5] = true;
                    break;
                case "大雪":
                    array[0,6] = true;
                    break;
                case "波浪":
                    array[0,7] = true;
                    break;
                case "高潮":
                    array[0,8] = true;
                    break;
                case "雷":
                    array[1,0] = true;
                    break;
                case "融雪":
                    array[1,1] = true;
                    break;
                case "濃霧":
                    array[1,2] = true;
                    break;
                case "乾燥":
                    array[1,3] = true;
                    break;
                case "なだれ":
                    array[1,4] = true;
                    break;
                case "低温":
                    array[1,5] = true;
                    break;
                case "霜":
                    array[1,6] = true;
                    break;
                case "着氷":
                    array[1,7] = true;
                    break;
                case "着雪":
                    array[1,8] = true;
                    break;
                default:
                    break;
            }
            return array;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 終了時にフォーム位置を保存するため
            // 最小化時、最大化時に保存されると変な値が保存されるので、ウィンドウ状態を見てから保存
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.Save();
            }
        }

        private void コピーCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView2.Items.Count == 0)
            {
                return;
            }

            int selected_i = -1;
            if (this.listView2.SelectedIndices.Count > 0)
            {
                selected_i = this.listView2.SelectedIndices[0];
            }

            StringBuilder sb = new StringBuilder();

            if (selected_i >= 0)
            {
                sb = this.PrintListView2(selected_i, sb);
            }
            else
            {
                for (int i = 0; i < this.listView2.Items.Count; i++)
                {
                    sb = this.PrintListView2(i, sb);
                }
            }

            Clipboard.SetText(sb.ToString());
        }

        StringBuilder PrintListView2(int selected_i, StringBuilder sb)
        {
            sb.Append(this.listView2.Items[selected_i].SubItems[1].Text);
            sb.Append(" ");
            sb.Append(this.listView2.Items[selected_i].SubItems[2].Text);
            sb.Append("/最高気温");
            sb.Append(this.listView2.Items[selected_i].SubItems[3].Text.Replace(" ", ""));
            sb.Append("/最低気温");
            sb.Append(this.listView2.Items[selected_i].SubItems[4].Text.Replace(" ", ""));
            sb.Append("/降水確率");
            sb.Append(this.listView2.Items[selected_i].SubItems[5].Text.Replace(" ", ""));
            sb.Append("/洗濯指数");
            sb.Append(this.listView2.Items[selected_i].SubItems[6].Text);
            sb.Append("/紫外線指数");
            sb.Append(this.listView2.Items[selected_i].SubItems[7].Text);
            sb.Append("/傘指数");
            sb.Append(this.listView2.Items[selected_i].SubItems[8].Text);
            sb.Append("/星空指数");
            sb.Append(this.listView2.Items[selected_i].SubItems[9].Text);
            sb.Append("\r\n");

            return sb;
        }

        private void コピーCToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count == 0)
            {
                return;
            }

            int selected_i = -1;
            if (this.listView1.SelectedIndices.Count > 0)
            {
                selected_i = this.listView1.SelectedIndices[0];
            }

            StringBuilder sb = new StringBuilder();

            if (selected_i >= 0)
            {
                sb = this.PrintListView1(selected_i, sb);
            }
            else
            {
                for (int i = 0; i < this.listView1.Items.Count; i++)
                {
                    sb = this.PrintListView1(i, sb);
                }
            }

            Clipboard.SetText(sb.ToString());
        }

        StringBuilder PrintListView1(int selected_i, StringBuilder sb)
        {
            sb.Append(this.listView1.Items[selected_i].SubItems[1].Text);
            sb.Append(" ");
            sb.Append(this.listView1.Items[selected_i].SubItems[2].Text);
            sb.Append(" ");
            sb.Append(this.listView1.Items[selected_i].SubItems[3].Text);
            sb.Append("/気温");
            sb.Append(this.listView1.Items[selected_i].SubItems[4].Text.Replace(" ", ""));
            sb.Append("/湿度");
            sb.Append(this.listView1.Items[selected_i].SubItems[5].Text.Replace(" ", ""));
            sb.Append("/降水量");
            sb.Append(this.listView1.Items[selected_i].SubItems[6].Text.Replace(" ", ""));
            sb.Append("/風向 ");
            sb.Append(this.listView1.Items[selected_i].SubItems[7].Text);
            sb.Append("/風速");
            sb.Append(this.listView1.Items[selected_i].SubItems[8].Text.Replace(" ", ""));
            sb.Append("\r\n");

            return sb;
        }
    }
}
