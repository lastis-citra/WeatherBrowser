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
        //public static string url_warn = string.Empty;
        //public static string url_abst = string.Empty;

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
        //List<string> back_url_warn = new List<string>();
        //List<string> back_url_abst = new List<string>();

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
        bool[,] special_array = new bool[2, 9];
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
            //back_url_warn = this.stringToStringList(Properties.Settings.Default.back_url_warn);
            //back_url_abst = this.stringToStringList(Properties.Settings.Default.back_url_abst);

            if (url_smallregion != "")
            {
                back_l.Add(selected_l);
                back_m.Add(selected_m);
                back_n.Add(selected_n);
                back_l_name.Add(l_name);
                back_m_name.Add(m_name);
                back_n_name.Add(n_name);
                back_url_smallregion.Add(url_smallregion);
                //back_url_warn.Add(url_warn);
                //back_url_abst.Add(url_abst);
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
                //back_url_warn.RemoveAt(0);
                //back_url_abst.RemoveAt(0);
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

                //MessageBox.Show("selected_l:" + selected_l + ", selected_m:" + selected_m + ", selected_n:" + selected_n);

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
                parser.Close();

                label5.Text = "【現在の警報・注意報】－" + _n_name;
                label2.Text = "【今日・明日の天気】－" + _n_name;
                label1.Text = "【週間天気】－" + _l_name + "＞" + _m_name;
                label7.Text = "【天気概況】－" + _l_name;

                l_name = _l_name;
                m_name = _m_name;
                n_name = _n_name;

                // キャッシュチェック
                string st_l = selected_l.ToString();
                string st_m = selected_l + "_" + selected_m;
                string st_n = selected_l + "_" + selected_m + "_" + selected_n;

                string filePath1 = @"./cache/listview1_" + st_n + ".cache";
                string filePath2 = @"./cache/listview2_" + st_n + ".cache";
                string filePath3 = @"./cache/listview3_" + st_n + ".cache";
                string filePath4 = @"./cache/textbox1_" + st_l + ".cache";

                this.DisplayCacheListView1(filePath1);
                this.DisplayCacheListView2(filePath2);
                this.DisplayCacheListView3(filePath3);
                this.DisplayCacheTextBox1(filePath4);
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            
            button1.Enabled = false;
            button1.Text = "情報取得中...";
            button1.Refresh();

            //MessageBox.Show(url_smallregion);

            string enc = "EUC-JP";
            string[] st = url_smallregion.Split('/');
            string[] st_2 = st[7].Split('.');

            string url_l = st[5] + ".html";
            string url_l_2 = st[5] + "/";
            string url_m = st[5] + "/" + st[6] + ".html";
            string url_m_2 = st[6];
            string url_n = st[5] + "/" + st_2[0] + "/";

            //string st_l = st[5];
            //string st_m = st[5] + "_" + st[6];
            //string st_n = st[5] + "_" + st[6] + "_" + st_2[0];
            string st_l = selected_l.ToString();
            string st_m = selected_l + "_" + selected_m;
            string st_n = selected_l + "_" + selected_m + "_" + selected_n;

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
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nまた、接続先サーバが正常通り稼働しているか確認してください\nURL:" + url + "\n上記URLが存在しない場合は設定を開き、データ取得ボタンを押してください。",
                    "エラー：ピンポイント天気");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            //MessageBox.Show(url_smallregion);
            CreteLib.HtmlDocument pinpointDoc = new CreteLib.HtmlDocument();
            try
            {
                pinpointDoc.LoadHtml(source);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（指数情報）
            // 指数情報は地域ごとのページに移動になったので不要（1.5.2）
            ////string url2 = "http://weather.yahoo.co.jp/weather/jp/expo/4610/";
            //string url2 = "http://weather.yahoo.co.jp/weather/jp/expo/" + url_m_2 + "/";

            //string source2 = "";

            //try
            //{
            //    //WebClientの作成
            //    WebClient wc2 = new WebClient();
            //    //文字コードを指定
            //    wc2.Encoding = Encoding.GetEncoding(65001);
            //    //HTMLソースをダウンロードする
            //    source2 = wc2.DownloadString(url2);
            //    //後始末
            //    wc2.Dispose();
            //}
            //catch (System.Net.WebException e2)
            //{
            //    //MessageBox.Show(e2.ToString());
            //    MessageBox.Show(
            //        "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL:" + url2,
            //        "エラー");
            //    button1.Enabled = true;
            //    button1.Text = "情報取得";
            //    return;
            //}

            //CreteLib.HtmlDocument doc2 = new CreteLib.HtmlDocument();
            //try
            //{
            //    doc2.LoadHtml(source2);
            //}
            //catch (Exception e2)
            //{
            //}

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

            CreteLib.HtmlDocument regionDoc = new CreteLib.HtmlDocument();
            try
            {
                regionDoc.LoadHtml(source6);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（警報・注意報）
            //v1.5からURL変更
            //string url7 = "https://typhoon.yahoo.co.jp/weather/jp/warn/28/28209/";
            string url7 = "https://typhoon.yahoo.co.jp/weather/jp/warn/" + url_n;

            string source7 = "";

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
            }
            catch (System.Net.WebException e2)
            {
                MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nサイト構成が変更になった場合には、本ソフトの最新版が出ていないか確認してください\nURL：" + url7,
                    "エラー");
                button1.Enabled = true;
                button1.Text = "情報取得";
                return;
            }

            CreteLib.HtmlDocument warnDoc = new CreteLib.HtmlDocument();
            try
            {
                warnDoc.LoadHtml(source7);
            }
            catch (Exception e2)
            {
            }

            // ------------------------------------------------------
            //ダウンロードするURL（天気概況）
            // v1.5からyahooに変更
            //string url8 = "https://weather.yahoo.co.jp/weather/jp/13/";
            string url8 = "https://weather.yahoo.co.jp/weather/jp/" + url_l_2;

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

            CreteLib.HtmlDocument prefectureDoc = new CreteLib.HtmlDocument();
            prefectureDoc.LoadHtml(source8);
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
            special_array.Initialize();
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
                    special_array[j, k] = false;
                }
            }

            gray_num2 = 0;

            List<string> date_array = new List<string>();
            List<string> date_array2 = new List<string>();
            List<string> time_array = new List<string>();
            List<string> weather_array = new List<string>();
            List<string> ico_array = new List<string>();
            List<string> winddr_array = new List<string>();
            List<string> weather_array2 = new List<string>();
            List<string> ico_array2 = new List<string>();

            HtmlNodeList nodeList = pinpointDoc.GetNodesByTagName("span");

            int date_count = 0;

            foreach (HtmlNode node in nodeList)
            {
                if (node.Attributes.ContainsValue("yjSt") && node.InnerText.Contains("月"))
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
            // 毎時天気アイコン取得
            getPinPointIconListView1(pinpointDoc, ref ico_array);
            // 週間1～2日目天気アイコン取得
            getRegionIconListView2(regionDoc, ref ico_array2);
            // 週間3～8日目天気アイコン取得
            getPinPointIconListView2(pinpointDoc, ref ico_array2);
            // 週間1～2日目の天気取得
            getRegionWeatherListView2(regionDoc, ref weather_array2);
            // 指数情報取得
            getPinPointShisu(pinpointDoc, ref wash_array, ref ul_vio_array, ref umb_array);

            //----------------------------------------------------------
            // 週間予報
            // 今日明日分
            i = 0;
            nodeList = regionDoc.GetNodesByTagName("em");
            // 今日明日の最高気温、最低気温
            foreach (HtmlNode node in nodeList)
            {
                //MessageBox.Show(node.InnerText);
                switch (i)
                {
                    case 0:
                    case 2:
                        //MessageBox.Show(node.InnerText);
                        max_temp_array2.Add(node.InnerText);
                        break;
                    case 1:
                    case 3:
                        min_temp_array2.Add(node.InnerText);
                        break;
                    default:
                        break;
                }
                i++;
            }
                       
            i = 0;
            int i2 = 0;
            bool allday = false;

            nodeList = regionDoc.GetNodesByTagName("table");
            foreach (HtmlNode node in nodeList)
            {
                try
                {
                    if (node.Attributes.Count > 0 && node["class"].Contains("allDay"))
                    {
                        allday = true;
                    }
                }
                catch (Exception e2)
                {
                }
            }

            nodeList = regionDoc.GetNodesByTagName("td");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("-24"))
                {
                    rain_flag4 = true;
                }
                else if (rain_flag4)
                {
                    rain_string += node.InnerText.Replace("％", "");

                    i++;
                    // 0時以降、2日目の降水確率が24時間まとめてのものしか出なくなったため（v1.4.6）
                    if (i == 4 || (allday && i2 == 1 && i == 1))
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
            // 週間予報
            // 残り5日分
            i = 0;
            nodeList = pinpointDoc.GetNodesByTagName("font");
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

            // gray_numが0の場合、次のforeachがすべてスキップされて気温が取れないため、気温フラグを事前に立てておく
            if (gray_num == 0)
            {
                max_temp_flag3 = true;
                i = 0;
            }

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
                        //MessageBox.Show(node.InnerText);
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
                    //MessageBox.Show(node.InnerText);
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
                    }
                }
            }
        

            //----------------------------------------------------------
            weather_flag = false;
            rain_flag = false;
            i = 0;
            nodeList = pinpointDoc.GetNodesByTagName("small");

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
                            //MessageBox.Show(node.InnerText);
                        }
                        if (node.ChildNodes[0].ChildNodes.Count > 0 && node.ChildNodes[0].ChildNodes[0].InnerText == "---")
                        {
                        }
                        else
                        {
                            i++;
                        }
                        //MessageBox.Show(i.ToString());
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
                        //MessageBox.Show(node.InnerText);
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
                    if (node.ChildNodes.Count > 3)
                    {
                        date_array2.Add(node.ChildNodes[0].InnerText + node.ChildNodes[2].InnerText + node.ChildNodes[3].InnerText + node.ChildNodes[4].InnerText);
                    }
                    else
                    {
                        date_array2.Add(node.ChildNodes[0].InnerText + node.ChildNodes[2].InnerText);
                    }
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
                if (node.InnerText.Contains("確率（％）"))
                {
                    rain_flag3 = true;
                }
                else if (rain_flag3)
                {
                    //週間予報の3日め以降の降水確率
                    //MessageBox.Show(node.InnerText);
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

            ////----------------------------------------------------------
            // 警報・注意報
            // v1.5からyahooに変更
            nodeList = warnDoc.GetNodesByTagName("ul");

            foreach (HtmlNode node in nodeList) 
            {
                // 現在の警報・注意報はwarnDetail_head_labelsの中にある
                if (node.Attributes.ContainsKey("class") && node.Attributes["class"] == "warnDetail_head_labels")
                {
                    //MessageBox.Show(node.ChildNodes.Count.ToString());
                    foreach (HtmlNode child in node.ChildNodes)
                    {
                        if (child.ChildNodes.Count == 0)
                        {
                            continue;
                        }
                        //MessageBox.Show(child.ChildNodes[0].Attributes["class"]);
                        if (child.ChildNodes[0].Attributes["class"] == "is-normal")
                        {
                            // 発表なしなら何もしない
                            break;
                        }
                        if (child.ChildNodes[0].Attributes["class"] == "icoEmgWarning")
                        {
                            string tmp_sp = child.ChildNodes[0].ChildNodes[0].InnerText;
                            special_array = ChangeStatusWarnNotice(tmp_sp, special_array);
                        }
                        if (child.ChildNodes[0].Attributes["class"] == "icoWarning" ||
                            child.ChildNodes[0].Attributes["class"] == "icoWarnToEmg")
                        {
                            string tmp_warn = child.ChildNodes[0].ChildNodes[0].InnerText;
                            warning_array = ChangeStatusWarnNotice(tmp_warn, warning_array);
                        }
                        if (child.ChildNodes[0].Attributes["class"] == "icoAdvisory" ||
                            child.ChildNodes[0].Attributes["class"] == "icoAdvToWarn")
                        {
                            //MessageBox.Show(child.ChildNodes[0].ChildNodes[0].InnerText);
                            string tmp_alert = child.ChildNodes[0].ChildNodes[0].InnerText;
                            notice_array = ChangeStatusWarnNotice(tmp_alert, notice_array);
                        }
                    }
                    break;
                }
            }

            nodeList = warnDoc.GetNodesByTagName("h2");

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
            
            ////----------------------------------------------------------
            string year_month = string.Empty;

            i = 0;
            nodeList = pinpointDoc.GetNodesByTagName("p");
            foreach (HtmlNode node in nodeList)
            {
                if (node.InnerText.Contains("発表"))
                {
                    if (i == 0)
                    {
                        string[] st1 = node.InnerText.Split('\n');
                        // 今日明日の天気の日時
                        label3.Text = st1[1];

                        //Regex reg = new Regex("(?<year_month>.*?)月");
                        //Match m = reg.Match(node.InnerText);
                        //year_month = m.Groups["year_month"].Value;
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

            // v1.5からyahooに変更
            nodeList = warnDoc.GetNodesByAttribute("yjSt yjw_note_h2");
            //nodeList = doc7.GetNodesByAttribute("announce-datetime");
            label6.Text = nodeList[0].InnerText;

            // 地域名はn_nameと一致していないことがあるため，ここで修正する
            label5.Text = "【現在の警報・注意報】－" + warning_region;
            label2.Text = "【今日・明日の天気】－" + n_name;
            label1.Text = "【週間天気】－" + l_name + "＞" + m_name;
            label7.Text = "【天気概況】－" + l_name;

            ////----------------------------------------------------------
            // 天気概況
            // v1.5からyahooに変更
            nodeList = prefectureDoc.GetNodesByAttribute("text jsoff");
            //nodeList = doc8.GetNodesByAttribute("forecast-comment");
            textBox1.Text = nodeList[0].InnerText.Replace("<br />", "\r\n").Replace("<br>", "\r\n");

            // 天気概況更新日時
            // v1.5からyahooに変更
            nodeList = prefectureDoc.GetNodesByAttribute("time");
            //nodeList = doc8.GetNodesByAttribute("forecast-map-announce-datetime");
            label8.Text = nodeList[0].InnerText;

            // キャッシュの作成
            //----------------------------------------------------------
            ////ImageListの設定
            LoadImageList();

            listView1.Items.Clear();
            listView2.Items.Clear();
            ResetListView3();

            string filePath2 = @"./cache/listview1_" + st_n + ".cache";
            StreamWriter sw2 = new StreamWriter(filePath2, false, Encoding.GetEncoding(65001));

            sw2.Write(
                label2.Text + "," +
                label3.Text + "," +
                gray_num + "\n"
                );
            
            //MessageBox.Show(wet_array.Count.ToString());
            for (i = 0; i < 16; i++)
            {
                windst_array[i] = windst_array[i].Replace("\n", "");

                DispListView1(date_array[i], time_array[i], weather_array[i], temp_array[i],
                    wet_array[i], rain_array[i], winddr_array[i], windst_array[i], ico_array[i]);

                // キャッシュデータの書き出し
                sw2.Write(
                    date_array[i] + "," +
                    time_array[i] + "," +
                    weather_array[i] + "," +
                    temp_array[i] + "," +
                    wet_array[i] + "," +
                    rain_array[i] + "," +
                    winddr_array[i] + "," +
                    windst_array[i] + "," +
                    ico_array[i] + "\n"
                    );

                // UseItemStyleForSubItemsをfalseにする
                // subitemごとに色を付け替えるため
                listView1.Items[i].UseItemStyleForSubItems = false;

                // 条件に応じてスタイル変更
                ChangeColorView1(i);

                // listView1の天気アイコンを変更
                //this.ChangeWeatherIcon(weather_array[i], i);
                ChangeWeatherIconListView1(ico_array[i], i);
            }
            sw2.Close();
            
            //----------------------------------------------------------
            string filePath3 = @"./cache/listview2_" + st_n + ".cache";
            StreamWriter sw3 = new StreamWriter(filePath3, false, Encoding.GetEncoding(65001));

            sw3.Write(
                label1.Text + "," +
                label4.Text + "," +
                gray_num2 + "\n"
                );

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

                DispListView2(date_array2[i], weather_array2[i], max_temp_array2[i], min_temp_array2[i],
                    rain_array2[i], wash_array[i], ul_vio_array[i], umb_array[i], star_array[i], ico_array2[i]);

                // キャッシュデータの書き出し
                sw3.Write(
                    date_array2[i] + "," +
                    weather_array2[i] + "," +
                    max_temp_array2[i] + "," +
                    min_temp_array2[i] + "," +
                    rain_array2[i] + "," +
                    wash_array[i] + "," +
                    ul_vio_array[i] + "," +
                    umb_array[i] + "," + 
                    star_array[i] + "," +
                    ico_array2[i] + "\n"
                    );

                // UseItemStyleForSubItemsをfalseにする
                // subitemごとに色を付け替えるため
                listView2.Items[i].UseItemStyleForSubItems = false;

                ChangeColorView2(i);

                // listView2の天気アイコンを変更
                //this.ChangeWeatherIcon2(weather_array2[i], i);
                ChangeWeatherIconListView2(ico_array2[i], i);
            }
            sw3.Close();

            //-------------------------------------------------------
            // 警報・注意報
            //FontStyle boldFontStyle = listView3.Font.Style | FontStyle.Bold;
            //Font boldFont = new Font(listView3.Font.Name, listView3.Font.Size, boldFontStyle);

            // UseItemStyleForSubItemsをfalseにする
            // subitemごとに色を付け替えるため
            listView3.Items[0].UseItemStyleForSubItems = false;
            listView3.Items[1].UseItemStyleForSubItems = false;

            Color specialColor = Color.FromArgb(148, 0, 211);
            Color warningColor = Color.FromArgb(204, 0, 0);
            Color noticeColor = Color.FromArgb(255, 217, 102);

            string filePath4 = @"./cache/listview3_" + st_n + ".cache";
            StreamWriter sw4 = new StreamWriter(filePath4, false, Encoding.GetEncoding(65001));

            sw4.Write(
                label5.Text + "," +
                label6.Text + "\n"
                );

            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 9; k++)
                {
                    sw4.Write(notice_array[j, k] + "," + warning_array[j, k] + "," + special_array[j, k] + "\n");

                    if (notice_array[j,k])
                    {
                        listView3.Items[j].SubItems[k + 1].BackColor = noticeColor;
                    }
                    else if (warning_array[j, k])
                    {
                        listView3.Items[j].SubItems[k + 1].ForeColor = Color.White;
                        listView3.Items[j].SubItems[k + 1].BackColor = warningColor;
                    }
                    else if (special_array[j, k])
                    {
                        listView3.Items[j].SubItems[k + 1].ForeColor = Color.White;
                        listView3.Items[j].SubItems[k + 1].BackColor = specialColor;
                    }
                    else
                    {
                        listView3.Items[j].SubItems[k + 1].ForeColor = Color.Gray;
                    }
                }
            }
            sw4.Close();

            string filePath5 = @"./cache/textbox1_" + st_l + ".cache";
            StreamWriter sw5 = new StreamWriter(filePath5, false, Encoding.GetEncoding(65001));

            sw5.Write(
                label7.Text + "," + label8.Text + "\n" +
                textBox1.Text + "\n"
                );
            sw5.Close();

            button1.Enabled = true;
            button1.Text = "情報取得";
        }

        /*
         * ピンポイント天気の天気アイコンの情報を今日明日分取得
         */
        private void getPinPointIconListView1(CreteLib.HtmlDocument pinpointDoc, ref List<string> ico_array)
        {
            HtmlNodeList imgNodeList = pinpointDoc.GetNodesByTagName("img");

            // width=40のsrc属性のみを取得する
            foreach (HtmlNode node in imgNodeList)
            {
                if (node.Attributes.ContainsKey("width"))
                {
                    if (node["width"].Contains("40"))
                    {
                        string img = node["src"];
                        string[] tmp = img.Split('/');
                        ico_array.Add(tmp[9]);
                        //MessageBox.Show(tmp[9]);
                    }
                }
            }
        }

        /*
         * 週間天気用の今日明日の天気アイコンの情報を取得
         */
        private void getRegionIconListView2(CreteLib.HtmlDocument regionDoc, ref List<string> ico_array2)
        {
            HtmlNodeList pNodeList = regionDoc.GetNodesByTagName("p");

            // class属性にpictを含むもののみ取得する
            foreach (HtmlNode node in pNodeList)
            {
                if (node.Attributes.ContainsKey("class"))
                {
                    if (node["class"].Contains("pict"))
                    {
                        HtmlNode childNode = node.ChildNodes[0];
                        string img = childNode["src"];
                        string[] tmp = img.Split('/');
                        ico_array2.Add(tmp[8]);
                        //MessageBox.Show(tmp[8]);
                    }
                }
            }

        }

        /*
         * 週間天気用の今日明日の天気情報を取得
         */
        private void getRegionWeatherListView2(CreteLib.HtmlDocument regionDoc, ref List<string> weather_array2)
        {
            HtmlNodeList pNodeList = regionDoc.GetNodesByTagName("p");

            // class属性にpictを含むもののみ取得する
            foreach (HtmlNode node in pNodeList)
            {
                if (node.Attributes.ContainsKey("class"))
                {
                    if (node["class"].Contains("pict"))
                    {
                        HtmlNode childNode = node.ChildNodes[0];
                        weather_array2.Add(childNode["alt"]);
                        //MessageBox.Show(childNode["alt"]);
                    }
                }
            }

        }

        /*
         * ピンポイント天気の天気アイコンの情報を今日明日以降の6日分取得
         */
        private void getPinPointIconListView2(CreteLib.HtmlDocument pinpointDoc, ref List<string> ico_array2)
        {
            HtmlNodeList imgNodeList = pinpointDoc.GetNodesByTagName("img");

            // src属性にsize90を含むもののみを取得する
            foreach (HtmlNode node in imgNodeList)
            {
                if (node.Attributes.ContainsKey("src"))
                {
                    if (node["src"].Contains("size90"))
                    {
                        string img = node["src"];
                        string[] tmp = img.Split('/');
                        ico_array2.Add(tmp[8]);
                        //MessageBox.Show(tmp[8]);
                    }
                }
            }
        }

        /*
         * ピンポイント天気の指数情報を今日明日分取得
         */
        private void getPinPointShisu(CreteLib.HtmlDocument pinpointDoc, ref List<string> wash_array,
            ref List<string> ul_vio_array, ref List<string> umb_array)
        {
            HtmlNodeList pNodeList = pinpointDoc.GetNodesByTagName("p");

            foreach (HtmlNode node in pNodeList)
            {
                if (node.Attributes.ContainsKey("class"))
                {
                    if (node["class"].Contains("index_value"))
                    {
                        string tmp = node.ChildNodes[0].InnerText;
                        if (tmp.Contains("洗濯指数"))
                        {
                            wash_array.Add(tmp.Replace("洗濯指数", ""));
                        }
                        else if (tmp.Contains("紫外線指数"))
                        {
                            ul_vio_array.Add(tmp.Replace("紫外線指数", ""));
                        }
                        else if (tmp.Contains("傘指数"))
                        {
                            umb_array.Add(tmp.Replace("傘指数", ""));
                        }
                        //MessageBox.Show(tmp);
                    }
                }
            }
        }

        /*
         * イメージアイコンの読み込み
         */
        private void LoadImageList()
        {
            if (listView1.SmallImageList != null && listView2.SmallImageList != null)
            {
                return;
            }

            int i = 0;

            // ピンポイント天気用のアイコンファイル読み込み
            if (listView1.SmallImageList == null) {
                ////ImageListの設定
                ImageList imageListPinPoint = new ImageList();

                List<string> pinpoint_path = new List<string>() {
                   "10", "20", "21", "30", "31", "34", "41", "43",
                   "51", "52", "53", "54", "56", "61", "63", "64",
                   "66", "81", "83", "84", "86", "99"
                };

                for (i = 0; i < 22; i++)
                {
                    string path = null;
                    path = "weatherIcon/pinpoint/" + pinpoint_path[i] + "_day.png";
                    try
                    {
                        imageListPinPoint.Images.Add(Bitmap.FromFile(path));
                    }
                    catch (Exception e2)
                    {
                        MessageBox.Show(
                            "必要なアイコンファイルが読み込めませんでした。iconフォルダ内に必要なアイコンファイルがあるかどうか確認して下さい。",
                            "エラー");
                        this.Close();
                    }
                }
                for (i = 0; i < 22; i++)
                {
                    string path = null;
                    path = "weatherIcon/pinpoint/" + pinpoint_path[i] + "_night.png";
                    try
                    {
                        imageListPinPoint.Images.Add(Bitmap.FromFile(path));
                    }
                    catch (Exception e2)
                    {
                        MessageBox.Show(
                            "必要なアイコンファイルが読み込めませんでした。iconフォルダ内に必要なアイコンファイルがあるかどうか確認して下さい。",
                            "エラー");
                        this.Close();
                    }
                }
                imageListPinPoint.ImageSize = new Size(16, 16);
                imageListPinPoint.ColorDepth = ColorDepth.Depth32Bit;
                listView1.SmallImageList = imageListPinPoint;
            }

            // 週間予報用のアイコンファイル読み込み
            if (listView2.SmallImageList == null)
            {
                ////ImageListの設定
                ImageList imageListNext = new ImageList();

                List<string> next_path = new List<string>() {
                   "100", "101", "102", "103", "104", "105", "111", "114",
                   "117", "119", "140", "149", "150", "156", "158", "166",
                   "168", "200", "201", "202", "203", "204", "205", "211",
                   "214", "217", "219", "240", "249", "250", "255", "256",
                   "257", "258", "266", "268", "300", "301", "302", "311",
                   "313", "315", "330", "341", "343", "345", "350", "400",
                   "401", "402", "411", "413", "414", "441", "443", "444",
                   "450", "500", "550", "600", "650", "700", "750", "800",
                   "850", "999"
                };

                for (i = 0; i < 66; i++)
                {
                    string path = null;
                    path = "weatherIcon/next/" + next_path[i] + "_day.png";
                    try
                    {
                        imageListNext.Images.Add(Bitmap.FromFile(path));
                    }
                    catch (Exception e2)
                    {
                        MessageBox.Show(
                            "必要なアイコンファイルが読み込めませんでした。iconフォルダ内に必要なアイコンファイルがあるかどうか確認して下さい。",
                            "エラー");
                        this.Close();
                    }
                }
                imageListNext.ImageSize = new Size(32, 16);
                imageListNext.ColorDepth = ColorDepth.Depth32Bit;
                listView2.SmallImageList = imageListNext;
            }
        }

        /*
         * ListView1の天気アイコンを変更する
         * ico: アイコンファイル名
         * i: 変更したいListView1の列数
         */
        private void ChangeWeatherIconListView1(string ico, int i)
        {
            // 拡張子を外し，_gは除去
            string icoFormatted = ico.Replace(".png", "").Replace("_g", "");

            switch (icoFormatted)
            {
                case "10_day":
                    listView1.Items[i].ImageIndex = 0; break;
                case "20_day":
                    listView1.Items[i].ImageIndex = 1; break;
                case "21_day":
                    listView1.Items[i].ImageIndex = 2; break;
                case "30_day":
                    listView1.Items[i].ImageIndex = 3; break;
                case "31_day":
                    listView1.Items[i].ImageIndex = 4; break;
                case "34_day":
                    listView1.Items[i].ImageIndex = 5; break;
                case "41_day":
                    listView1.Items[i].ImageIndex = 6; break;
                case "43_day":
                    listView1.Items[i].ImageIndex = 7; break;
                case "51_day":
                    listView1.Items[i].ImageIndex = 8; break;
                case "52_day":
                    listView1.Items[i].ImageIndex = 9; break;
                case "53_day":
                    listView1.Items[i].ImageIndex = 10; break;
                case "54_day":
                    listView1.Items[i].ImageIndex = 11; break;
                case "56_day":
                    listView1.Items[i].ImageIndex = 12; break;
                case "61_day":
                    listView1.Items[i].ImageIndex = 13; break;
                case "63_day":
                    listView1.Items[i].ImageIndex = 14; break;
                case "64_day":
                    listView1.Items[i].ImageIndex = 15; break;
                case "66_day":
                    listView1.Items[i].ImageIndex = 16; break;
                case "81_day":
                    listView1.Items[i].ImageIndex = 17; break;
                case "83_day":
                    listView1.Items[i].ImageIndex = 18; break;
                case "84_day":
                    listView1.Items[i].ImageIndex = 19; break;
                case "86_day":
                    listView1.Items[i].ImageIndex = 20; break;
                case "99_day":
                    listView1.Items[i].ImageIndex = 21; break;
                case "10_night":
                    listView1.Items[i].ImageIndex = 22; break;
                case "20_night":
                    listView1.Items[i].ImageIndex = 23; break;
                case "21_night":
                    listView1.Items[i].ImageIndex = 24; break;
                case "30_night":
                    listView1.Items[i].ImageIndex = 25; break;
                case "31_night":
                    listView1.Items[i].ImageIndex = 26; break;
                case "34_night":
                    listView1.Items[i].ImageIndex = 27; break;
                case "41_night":
                    listView1.Items[i].ImageIndex = 28; break;
                case "43_night":
                    listView1.Items[i].ImageIndex = 29; break;
                case "51_night":
                    listView1.Items[i].ImageIndex = 30; break;
                case "52_night":
                    listView1.Items[i].ImageIndex = 31; break;
                case "53_night":
                    listView1.Items[i].ImageIndex = 32; break;
                case "54_night":
                    listView1.Items[i].ImageIndex = 33; break;
                case "56_night":
                    listView1.Items[i].ImageIndex = 34; break;
                case "61_night":
                    listView1.Items[i].ImageIndex = 35; break;
                case "63_night":
                    listView1.Items[i].ImageIndex = 36; break;
                case "64_night":
                    listView1.Items[i].ImageIndex = 37; break;
                case "66_night":
                    listView1.Items[i].ImageIndex = 38; break;
                case "81_night":
                    listView1.Items[i].ImageIndex = 39; break;
                case "83_night":
                    listView1.Items[i].ImageIndex = 40; break;
                case "84_night":
                    listView1.Items[i].ImageIndex = 41; break;
                case "86_night":
                    listView1.Items[i].ImageIndex = 42; break;
                case "99_night":
                    listView1.Items[i].ImageIndex = 43; break;
                default:
                    listView1.Items[i].ImageIndex = 43; break;
            }
        }

        /*
         * ListView2の天気アイコンを変更する
         * ico: アイコンファイル名
         * i: 変更したいListView2の列数
         */
        private void ChangeWeatherIconListView2(string ico, int i)
        {
            // 拡張子を外し，nightはdayに
            string icoFormatted = ico.Replace(".png", "").Replace("night", "day");

            switch (icoFormatted)
            {
                case "100_day":
                    listView2.Items[i].ImageIndex = 0; break;
                case "101_day":
                    listView2.Items[i].ImageIndex = 1; break;
                case "102_day":
                    listView2.Items[i].ImageIndex = 2; break;
                case "103_day":
                    listView2.Items[i].ImageIndex = 3; break;
                case "104_day":
                    listView2.Items[i].ImageIndex = 4; break;
                case "105_day":
                    listView2.Items[i].ImageIndex = 5; break;
                case "111_day":
                    listView2.Items[i].ImageIndex = 6; break;
                case "114_day":
                    listView2.Items[i].ImageIndex = 7; break;
                case "117_day":
                    listView2.Items[i].ImageIndex = 8; break;
                case "119_day":
                    listView2.Items[i].ImageIndex = 9; break;
                case "140_day":
                    listView2.Items[i].ImageIndex = 10; break;
                case "149_day":
                    listView2.Items[i].ImageIndex = 11; break;
                case "150_day":
                    listView2.Items[i].ImageIndex = 12; break;
                case "156_day":
                    listView2.Items[i].ImageIndex = 13; break;
                case "158_day":
                    listView2.Items[i].ImageIndex = 14; break;
                case "166_day":
                    listView2.Items[i].ImageIndex = 15; break;
                case "168_day":
                    listView2.Items[i].ImageIndex = 16; break;
                case "200_day":
                    listView2.Items[i].ImageIndex = 17; break;
                case "201_day":
                    listView2.Items[i].ImageIndex = 18; break;
                case "202_day":
                    listView2.Items[i].ImageIndex = 19; break;
                case "203_day":
                    listView2.Items[i].ImageIndex = 20; break;
                case "204_day":
                    listView2.Items[i].ImageIndex = 21; break;
                case "205_day":
                    listView2.Items[i].ImageIndex = 22; break;
                case "211_day":
                    listView2.Items[i].ImageIndex = 23; break;
                case "214_day":
                    listView2.Items[i].ImageIndex = 24; break;
                case "217_day":
                    listView2.Items[i].ImageIndex = 25; break;
                case "219_day":
                    listView2.Items[i].ImageIndex = 26; break;
                case "240_day":
                    listView2.Items[i].ImageIndex = 27; break;
                case "249_day":
                    listView2.Items[i].ImageIndex = 28; break;
                case "250_day":
                    listView2.Items[i].ImageIndex = 29; break;
                case "255_day":
                    listView2.Items[i].ImageIndex = 30; break;
                case "256_day":
                    listView2.Items[i].ImageIndex = 31; break;
                case "257_day":
                    listView2.Items[i].ImageIndex = 32; break;
                case "258_day":
                    listView2.Items[i].ImageIndex = 33; break;
                case "266_day":
                    listView2.Items[i].ImageIndex = 34; break;
                case "268_day":
                    listView2.Items[i].ImageIndex = 35; break;
                case "300_day":
                    listView2.Items[i].ImageIndex = 36; break;
                case "301_day":
                    listView2.Items[i].ImageIndex = 37; break;
                case "302_day":
                    listView2.Items[i].ImageIndex = 38; break;
                case "311_day":
                    listView2.Items[i].ImageIndex = 39; break;
                case "313_day":
                    listView2.Items[i].ImageIndex = 40; break;
                case "315_day":
                    listView2.Items[i].ImageIndex = 41; break;
                case "330_day":
                    listView2.Items[i].ImageIndex = 42; break;
                case "341_day":
                    listView2.Items[i].ImageIndex = 43; break;
                case "343_day":
                    listView2.Items[i].ImageIndex = 44; break;
                case "345_day":
                    listView2.Items[i].ImageIndex = 45; break;
                case "350_day":
                    listView2.Items[i].ImageIndex = 46; break;
                case "400_day":
                    listView2.Items[i].ImageIndex = 47; break;
                case "401_day":
                    listView2.Items[i].ImageIndex = 48; break;
                case "402_day":
                    listView2.Items[i].ImageIndex = 49; break;
                case "411_day":
                    listView2.Items[i].ImageIndex = 50; break;
                case "413_day":
                    listView2.Items[i].ImageIndex = 51; break;
                case "414_day":
                    listView2.Items[i].ImageIndex = 52; break;
                case "441_day":
                    listView2.Items[i].ImageIndex = 53; break;
                case "443_day":
                    listView2.Items[i].ImageIndex = 54; break;
                case "444_day":
                    listView2.Items[i].ImageIndex = 55; break;
                case "450_day":
                    listView2.Items[i].ImageIndex = 56; break;
                case "500_day":
                    listView2.Items[i].ImageIndex = 57; break;
                case "550_day":
                    listView2.Items[i].ImageIndex = 58; break;
                case "600_day":
                    listView2.Items[i].ImageIndex = 59; break;
                case "650_day":
                    listView2.Items[i].ImageIndex = 60; break;
                case "700_day":
                    listView2.Items[i].ImageIndex = 61; break;
                case "750_day":
                    listView2.Items[i].ImageIndex = 62; break;
                case "800_day":
                    listView2.Items[i].ImageIndex = 63; break;
                case "850_day":
                    listView2.Items[i].ImageIndex = 64; break;
                case "999_day":
                    listView2.Items[i].ImageIndex = 65; break;
                default:
                    listView2.Items[i].ImageIndex = 65; break;
            }
        }

        private void バージョン情報AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 Form2 = new Form2();
            Form2.ShowDialog();

            Form2.Dispose();
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
            //url_warn = back_url_warn[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            //url_abst = back_url_abst[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            n_name = back_n_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];  // これがないと警報・注意報がうまく取れない
            m_name = back_m_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];
            l_name = back_l_name[parentToolStripMenuItem.DropDownItems.Count - i - 1];

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
            //string tmp_back_url_warn = back_url_warn[i];
            //string tmp_back_url_abst = back_url_abst[i];

            back_name.RemoveAt(i);
            back_l.RemoveAt(i);
            back_m.RemoveAt(i);
            back_n.RemoveAt(i);
            back_l_name.RemoveAt(i);
            back_m_name.RemoveAt(i);
            back_n_name.RemoveAt(i);
            back_url_smallregion.RemoveAt(i);
            //back_url_warn.RemoveAt(i);
            //back_url_abst.RemoveAt(i);

            back_name.Add(tmp_back_name);
            back_l.Add(tmp_back_l);
            back_m.Add(tmp_back_m);
            back_n.Add(tmp_back_n);
            back_l_name.Add(tmp_back_l_name);
            back_m_name.Add(tmp_back_m_name);
            back_n_name.Add(tmp_back_n_name);
            back_url_smallregion.Add(tmp_back_url_smallregion);
            //back_url_warn.Add(tmp_back_url_warn);
            //back_url_abst.Add(tmp_back_url_abst);

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
            //Properties.Settings.Default.back_url_warn = this.stringListToString(back_url_warn);
            //Properties.Settings.Default.back_url_abst = this.stringListToString(back_url_abst);

            Properties.Settings.Default.l = selected_l;
            Properties.Settings.Default.m = selected_m;
            Properties.Settings.Default.n = selected_n;

            Properties.Settings.Default.Save();

            if (old_l != selected_l || old_m != selected_m || old_n != selected_n)
            {
                // キャッシュチェック
                string st_l = selected_l.ToString();
                string st_m = selected_l + "_" + selected_m;
                string st_n = selected_l + "_" + selected_m + "_" + selected_n;

                string filePath1 = @"./cache/listview1_" + st_n + ".cache";
                string filePath2 = @"./cache/listview2_" + st_n + ".cache";
                string filePath3 = @"./cache/listview3_" + st_n + ".cache";
                string filePath4 = @"./cache/textbox1_" + st_l + ".cache";

                this.DisplayCacheListView1(filePath1);
                this.DisplayCacheListView2(filePath2);
                this.DisplayCacheListView3(filePath3);
                this.DisplayCacheTextBox1(filePath4);
            }
        }
        
        /**
         * listView1に情報を表示する
         */
        private void DispListView1(string date, string time, string weather, string temp, string wet,
            string rain, string winddr, string windst, string ico)
        {
            string parcent = "";
            if (Regex.IsMatch(wet, @"^[0-9]+$"))
            {
                parcent = " ％";
            }

            string[] item = {
                                    "",
                                    date,
                                    time,
                                    weather,
                                    temp + " ℃",
                                    wet + parcent,
                                    rain + " mm",
                                    winddr,
                                    windst + " m"
                                };
            listView1.Items.Add(new ListViewItem(item));
        }

        /**
         * listView2に情報を表示する
         */
        private void DispListView2(string date, string weather, string max_temp, string min_temp, string rain,
            string wash, string uv, string umb, string star, string ico)
        {
            string c = "";
            if (Regex.IsMatch(max_temp, @"^-*[0-9]+$"))
            {
                c = " ℃";
            }
            string parcent = "";
            if (Regex.IsMatch(rain, @"^[0-9]+$"))
            {
                parcent = " ％";
            }

            string[] item = {
                                    "",
                                    date,
                                    weather,
                                    max_temp + c,
                                    min_temp + c,
                                    rain + parcent,
                                    wash,
                                    uv,
                                    umb,
                                    star
                                };
            listView2.Items.Add(new ListViewItem(item));
        }

        private void DisplayCacheListView1(string filePath1)
        {
            if (System.IO.File.Exists(filePath1))
            {
                int i;

                this.LoadImageList();

                TextFieldParser parser = new TextFieldParser(filePath1,
                    System.Text.Encoding.GetEncoding(65001));

                temp_array.Clear();
                wet_array.Clear();
                rain_array.Clear();
                windst_array.Clear();

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    i = -1;

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (i == -1)
                        {
                            label2.Text = row[0];
                            label3.Text = row[1];
                            gray_num = int.Parse(row[2]);
                        }
                        else
                        {
                            DispListView1(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8]);

                            temp_array.Add(row[3]);
                            wet_array.Add(row[4]);
                            rain_array.Add(row[5]);
                            windst_array.Add(row[7]);

                            // UseItemStyleForSubItemsをfalseにする
                            // subitemごとに色を付け替えるため
                            listView1.Items[i].UseItemStyleForSubItems = false;

                            // 条件に応じてスタイル変更
                            ChangeColorView1(i);

                            //this.ChangeWeatherIcon(row[2], i);
                            ChangeWeatherIconListView1(row[8], i);
                        }
                        i++;
                    }
                }
            }
        }

        private void DisplayCacheListView2(string filePath2)
        {
            if (System.IO.File.Exists(filePath2))
            {
                int i;

                LoadImageList();

                TextFieldParser parser = new TextFieldParser(filePath2,
                    System.Text.Encoding.GetEncoding(65001));

                max_temp_array2.Clear();
                min_temp_array2.Clear();
                wash_array.Clear();
                ul_vio_array.Clear();
                umb_array.Clear();
                star_array.Clear();

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    i = -1;

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (i == -1)
                        {
                            label1.Text = row[0];
                            label4.Text = row[1];
                            gray_num2 = int.Parse(row[2]);
                        }
                        else
                        {
                            DispListView2(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9]);

                            max_temp_array2.Add(row[2]);
                            min_temp_array2.Add(row[3]);
                            wash_array.Add(row[5]);
                            ul_vio_array.Add(row[6]);
                            umb_array.Add(row[7]);
                            star_array.Add(row[8]);

                            // UseItemStyleForSubItemsをfalseにする
                            // subitemごとに色を付け替えるため
                            listView2.Items[i].UseItemStyleForSubItems = false;

                            ChangeColorView2(i);

                            //this.ChangeWeatherIcon2(row[1], i);
                            ChangeWeatherIconListView2(row[9], i);
                        }
                        i++;
                    }
                }
            }
        }

        private void DisplayCacheListView3(string filePath3)
        {
            if (System.IO.File.Exists(filePath3))
            {
                int i;

                TextFieldParser parser = new TextFieldParser(filePath3,
                    System.Text.Encoding.GetEncoding(65001));

                // UseItemStyleForSubItemsをfalseにする
                // subitemごとに色を付け替えるため
                listView3.Items[0].UseItemStyleForSubItems = false;
                listView3.Items[1].UseItemStyleForSubItems = false;

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    i = 0;
                    int j = 0;
                    int k = 0;
                    Color specialColor = Color.FromArgb(148, 0, 211);
                    Color warningColor = Color.FromArgb(204, 0, 0);
                    Color noticeColor = Color.FromArgb(255, 217, 102);

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        //MessageBox.Show("i:" + i.ToString() + " j:" + j.ToString() + " k:" + k.ToString() + " r:" + row[0]);

                        if (i == 0)
                        {
                            label5.Text = row[0];
                            label6.Text = row[1];
                        }
                        else
                        {
                            if (row.Length > 2 && row[2].Equals("True"))
                            {
                                    listView3.Items[j].SubItems[k + 1].ForeColor = Color.White;
                                    listView3.Items[j].SubItems[k + 1].BackColor = specialColor;
                            }
                            else if (row[1].Equals("True"))
                            {
                                listView3.Items[j].SubItems[k + 1].ForeColor = Color.White;
                                listView3.Items[j].SubItems[k + 1].BackColor = warningColor;
                            }
                            else if (row[0].Equals("True"))
                            {
                                listView3.Items[j].SubItems[k + 1].BackColor = noticeColor;
                            }
                            else
                            {
                                listView3.Items[j].SubItems[k + 1].ForeColor = Color.Gray;
                            }

                            if (k < 8)
                            {
                                k++;
                            }
                            else
                            {
                                j++;
                                k = 0;
                            }
                        }
                        i++;
                    }
                }
            }
        }

        private void DisplayCacheTextBox1(string filePath4)
        {
            if (System.IO.File.Exists(filePath4))
            {
                int i;

                TextFieldParser parser = new TextFieldParser(filePath4,
                    System.Text.Encoding.GetEncoding(65001));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    i = 0;

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (i == 0)
                        {
                            label7.Text = row[0];
                            label8.Text = row[1];
                        }
                        else
                        {
                            textBox1.Text += row[0] + "\r\n";
                        }
                        i++;
                    }
                }
            }
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
            if (Properties.Settings.Default.bool_max_max_temp && temp_array[i] != "---" && 
                int.Parse(temp_array[i]) >= int.Parse(Properties.Settings.Default.max_max_temp))
            {
                listView1.Items[i].SubItems[4].BackColor = Properties.Settings.Default.color_max_max_temp;
            }
            // 最低気温の下値
            if (Properties.Settings.Default.bool_min_min_temp && temp_array[i] != "---" && 
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
            if (Properties.Settings.Default.bool_max_rain && rain_array[i] != "---" && 
                int.Parse(rain_array[i]) >= int.Parse(Properties.Settings.Default.max_rain))
            {
                listView1.Items[i].SubItems[6].BackColor = Properties.Settings.Default.color_max_rain;
            }
            // 降水量の下値
            if (Properties.Settings.Default.bool_min_rain && rain_array[i] != "---" && 
                int.Parse(rain_array[i]) <= int.Parse(Properties.Settings.Default.min_rain))
            {
                listView1.Items[i].SubItems[6].BackColor = Properties.Settings.Default.color_min_rain;
            }
            // 風速の上値
            if (Properties.Settings.Default.bool_max_wind && windst_array[i] != "---" && 
                int.Parse(windst_array[i]) >= int.Parse(Properties.Settings.Default.max_wind))
            {
                listView1.Items[i].SubItems[8].BackColor = Properties.Settings.Default.color_max_wind;
            }
            // 風速の下値
            if (Properties.Settings.Default.bool_min_wind && windst_array[i] != "---" && 
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

        private void オプションOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int old_l = selected_l;
            int old_m = selected_m;
            int old_n = selected_n;

            Form3 Form3 = new Form3(this);
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
                        //back_url_warn.RemoveAt(i);
                        //back_url_abst.RemoveAt(i);
                    }

                    back_name.Add(b_name);
                    back_l.Add(selected_l);
                    back_m.Add(selected_m);
                    back_n.Add(selected_n);
                    back_l_name.Add(l_name);
                    back_m_name.Add(m_name);
                    back_n_name.Add(n_name);
                    back_url_smallregion.Add(url_smallregion);
                    //back_url_warn.Add(url_warn);
                    //back_url_abst.Add(url_abst);

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
                        //back_url_warn.RemoveAt(0);
                        //back_url_abst.RemoveAt(0);
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
                    //Properties.Settings.Default.back_url_warn = this.stringListToString(back_url_warn);
                    //Properties.Settings.Default.back_url_abst = this.stringListToString(back_url_abst);

                    Properties.Settings.Default.l = selected_l;
                    Properties.Settings.Default.m = selected_m;
                    Properties.Settings.Default.n = selected_n;

                    Properties.Settings.Default.Save();

                    // キャッシュチェック
                    string st_l = selected_l.ToString();
                    string st_m = selected_l + "_" + selected_m;
                    string st_n = selected_l + "_" + selected_m + "_" + selected_n;

                    string filePath1 = @"./cache/listview1_" + st_n + ".cache";
                    string filePath2 = @"./cache/listview2_" + st_n + ".cache";
                    string filePath3 = @"./cache/listview3_" + st_n + ".cache";
                    string filePath4 = @"./cache/textbox1_" + st_l + ".cache";

                    this.DisplayCacheListView1(filePath1);
                    this.DisplayCacheListView2(filePath2);
                    this.DisplayCacheListView3(filePath3);
                    this.DisplayCacheTextBox1(filePath4);
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

        private void 履歴を消去EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("履歴を削除してもいいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteHistories();
            }
        }

        public void DeleteHistories()
        {
            back_name.Clear();
            back_l.Clear();
            back_m.Clear();
            back_n.Clear();
            back_l_name.Clear();
            back_m_name.Clear();
            back_n_name.Clear();
            back_url_smallregion.Clear();
            //back_url_warn.Clear();
            //back_url_abst.Clear();

            履歴BToolStripMenuItem.DropDownItems.Clear();
            this.履歴BToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.testToolStripMenuItem });
            this.testToolStripMenuItem.Text = "（履歴はありません）";

            Properties.Settings.Default.back_l_name = string.Empty;
            Properties.Settings.Default.back_m_name = string.Empty;
            Properties.Settings.Default.back_n_name = string.Empty;
            Properties.Settings.Default.back_l = string.Empty;
            Properties.Settings.Default.back_m = string.Empty;
            Properties.Settings.Default.back_n = string.Empty;
            Properties.Settings.Default.back_name = string.Empty;
            Properties.Settings.Default.back_url_smallregion = string.Empty;
            //Properties.Settings.Default.back_url_warn = string.Empty;
            //Properties.Settings.Default.back_url_abst = string.Empty;

            Properties.Settings.Default.l = 0;
            Properties.Settings.Default.m = 0;
            Properties.Settings.Default.n = 0;

            Properties.Settings.Default.Save();
        }
    }
}
