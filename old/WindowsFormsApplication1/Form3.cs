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

namespace WeatherBrowser
{
    public partial class Form3 : Form
    {
        List<string> name_array1 = new List<string>();
        List<List<string>> name_array2 = new List<List<string>>();
        List<List<string>> url_array2 = new List<List<string>>();
        List<List<List<string>>> name_array3 = new List<List<List<string>>>();
        List<List<List<string>>> url_array3 = new List<List<List<string>>>();
        Dictionary<string, string> name_url_2_2 = new Dictionary<string, string>();
        Dictionary<string, string> name_url_2_3 = new Dictionary<string, string>();
        Dictionary<string, string> name_url_2_4 = new Dictionary<string, string>();

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button3.Visible = false;

            // セルの選択を解除
            dataGridView1.ClearSelection();

            // 選択色（選択した際に青くなる）を削除
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dataGridView1.DefaultCellStyle.SelectionForeColor = SystemColors.ControlText;


            DataGridViewComboBoxCell temp_cell1 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell temp_cell2 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell temp_cell3 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell temp_cell4 = new DataGridViewComboBoxCell();
            for (int i = -20; i <= 35; i += 1)
            {
                temp_cell1.Items.Add(i.ToString());
                temp_cell2.Items.Add(i.ToString());
                temp_cell3.Items.Add(i.ToString());
                temp_cell4.Items.Add(i.ToString());
            }
            DataGridViewComboBoxCell val_cell1 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell2 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell7 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell8 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell11 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell12 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell13 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell14 = new DataGridViewComboBoxCell();
            for (int i = 0; i <= 100; i += 10)
            {
                val_cell1.Items.Add(i.ToString());
                val_cell2.Items.Add(i.ToString());
                val_cell7.Items.Add(i.ToString());
                val_cell8.Items.Add(i.ToString());
                val_cell11.Items.Add(i.ToString());
                val_cell12.Items.Add(i.ToString());
                val_cell13.Items.Add(i.ToString());
                val_cell14.Items.Add(i.ToString());
            }
            DataGridViewComboBoxCell val_cell3 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell4 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell5 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell6 = new DataGridViewComboBoxCell();
            for (int i = 0; i <= 15; i += 1)
            {
                val_cell3.Items.Add(i.ToString());
                val_cell4.Items.Add(i.ToString());
                val_cell5.Items.Add(i.ToString());
                val_cell6.Items.Add(i.ToString());
            }
            DataGridViewComboBoxCell val_cell9 = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell val_cell10 = new DataGridViewComboBoxCell();
            for (int i = 0; i <= 5; i += 1)
            {
                val_cell9.Items.Add(i.ToString());
                val_cell10.Items.Add(i.ToString());
            }
            

            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_max_temp, "最高気温の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_min_temp, "最高気温の低値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_max_temp, "最低気温の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_min_temp, "最低気温の低値" });
            dataGridView1.Rows[0].Cells[2] = temp_cell1;
            dataGridView1.Rows[1].Cells[2] = temp_cell2;
            dataGridView1.Rows[2].Cells[2] = temp_cell3;
            dataGridView1.Rows[3].Cells[2] = temp_cell4;
            dataGridView1.Rows[0].Cells[2].Value = Properties.Settings.Default.max_max_temp;
            dataGridView1.Rows[1].Cells[2].Value = Properties.Settings.Default.max_min_temp;
            dataGridView1.Rows[2].Cells[2].Value = Properties.Settings.Default.min_max_temp;
            dataGridView1.Rows[3].Cells[2].Value = Properties.Settings.Default.min_min_temp;

            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_wet, "湿度の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_wet, "湿度の低値" });
            dataGridView1.Rows[4].Cells[2] = val_cell1;
            dataGridView1.Rows[5].Cells[2] = val_cell2;
            dataGridView1.Rows[4].Cells[2].Value = Properties.Settings.Default.max_wet;
            dataGridView1.Rows[5].Cells[2].Value = Properties.Settings.Default.min_wet;

            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_rain, "降水量の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_rain, "降水量の低値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_wind, "風速の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_wind, "風速の低値" });
            dataGridView1.Rows[6].Cells[2] = val_cell3;
            dataGridView1.Rows[7].Cells[2] = val_cell4;
            dataGridView1.Rows[8].Cells[2] = val_cell5;
            dataGridView1.Rows[9].Cells[2] = val_cell6;
            dataGridView1.Rows[6].Cells[2].Value = Properties.Settings.Default.max_rain;
            dataGridView1.Rows[7].Cells[2].Value = Properties.Settings.Default.min_rain;
            dataGridView1.Rows[8].Cells[2].Value = Properties.Settings.Default.max_wind;
            dataGridView1.Rows[9].Cells[2].Value = Properties.Settings.Default.min_wind;

            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_wash, "洗濯指数の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_wash, "洗濯指数の低値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_ult_vio, "紫外線指数の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_ult_vio, "紫外線指数の低値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_umb, "傘指数の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_umb, "傘指数の低値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_max_star, "星空指数の高値" });
            dataGridView1.Rows.Add(new Object[] { Properties.Settings.Default.bool_min_star, "星空指数の低値" });
            dataGridView1.Rows[10].Cells[2] = val_cell7;
            dataGridView1.Rows[11].Cells[2] = val_cell8;
            dataGridView1.Rows[12].Cells[2] = val_cell9;
            dataGridView1.Rows[13].Cells[2] = val_cell10;
            dataGridView1.Rows[14].Cells[2] = val_cell11;
            dataGridView1.Rows[15].Cells[2] = val_cell12;
            dataGridView1.Rows[16].Cells[2] = val_cell13;
            dataGridView1.Rows[17].Cells[2] = val_cell14;
            dataGridView1.Rows[10].Cells[2].Value = Properties.Settings.Default.max_wash;
            dataGridView1.Rows[11].Cells[2].Value = Properties.Settings.Default.min_wash;
            dataGridView1.Rows[12].Cells[2].Value = Properties.Settings.Default.max_ul_vio;
            dataGridView1.Rows[13].Cells[2].Value = Properties.Settings.Default.min_ul_vio;
            dataGridView1.Rows[14].Cells[2].Value = Properties.Settings.Default.max_umb;
            dataGridView1.Rows[15].Cells[2].Value = Properties.Settings.Default.min_umb;
            dataGridView1.Rows[16].Cells[2].Value = Properties.Settings.Default.max_star;
            dataGridView1.Rows[17].Cells[2].Value = Properties.Settings.Default.min_star;

            // ボタンのスタイル
            dataGridView1.Rows[0].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_max_temp;
            dataGridView1.Rows[1].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_min_temp;
            dataGridView1.Rows[2].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_max_temp;
            dataGridView1.Rows[3].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_min_temp;
            dataGridView1.Rows[4].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_wet;
            dataGridView1.Rows[5].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_wet;
            dataGridView1.Rows[6].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_rain;
            dataGridView1.Rows[7].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_rain;
            dataGridView1.Rows[8].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_wind;
            dataGridView1.Rows[9].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_wind;
            dataGridView1.Rows[10].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_wash;
            dataGridView1.Rows[11].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_wash;
            dataGridView1.Rows[12].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_ul_vio;
            dataGridView1.Rows[13].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_ul_vio;
            dataGridView1.Rows[14].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_umb;
            dataGridView1.Rows[15].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_umb;
            dataGridView1.Rows[16].Cells[3].Style.BackColor = Properties.Settings.Default.color_max_star;
            dataGridView1.Rows[17].Cells[3].Style.BackColor = Properties.Settings.Default.color_min_star;

            // DataGridView1にユーザーが新しい行を追加できないようにする
            dataGridView1.AllowUserToAddRows = false;
            // DataGridView1の行をユーザーが削除できないようにする
            dataGridView1.AllowUserToDeleteRows = false;
            // DataGridView1のセルを読み取り専用にする
            //dataGridView1.Columns[1].ReadOnly = true;
            //並び替えができないようにする
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            // DataGridView1の列の幅をユーザーが変更できないようにする
            dataGridView1.AllowUserToResizeColumns = false;
            // DataGridView1の行の高さをユーザーが変更できないようにする
            dataGridView1.AllowUserToResizeRows = false;
            // DataGridView1でセル、行、列が複数選択されないようにする
            dataGridView1.MultiSelect = false;
            //列ヘッダーの高さを変更できないようにする
            dataGridView1.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //行ヘッダーの幅を変更できるようにする
            dataGridView1.RowHeadersWidthSizeMode =
                DataGridViewRowHeadersWidthSizeMode.DisableResizing;


            try
            {
                // ----------------------------------------------------
                TextFieldParser parser = new TextFieldParser("regionlist1.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    // parser.HasFieldsEnclosedInQuotes = false;
                    // parser.TrimWhiteSpace = false;

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        foreach (string field in row)
                        {
                            name_array1.Add(field);
                        }
                    }
                }
                comboBox1.Items.AddRange(name_array1.ToArray());

                // ----------------------------------------------------
                parser = new TextFieldParser("regionlist2.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    // parser.HasFieldsEnclosedInQuotes = false;
                    // parser.TrimWhiteSpace = false;
                    int last_l = 0;

                    List<string> tmp_name_array = new List<string>();
                    List<string> tmp_url_array = new List<string>();

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (int.Parse(row[0]) > last_l)
                        {
                            name_array2.Add(tmp_name_array);
                            //url_array2.Add(tmp_url_array);
                            tmp_name_array = new List<string>();
                        }

                        tmp_name_array.Add(row[2]);
                        //tmp_url_array.Add(row[3]);
                        last_l = int.Parse(row[0]);
                    }
                    name_array2.Add(tmp_name_array);
                    //url_array2.Add(tmp_name_array);
                }

                // ----------------------------------------------------
                parser = new TextFieldParser("regionlist2_2.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み
                        //MessageBox.Show(row[0] + ", " + row[1] + ", " + row[2] + ", " + row[3]);
                        if (!name_url_2_2.ContainsKey(row[0]))
                        {
                            name_url_2_2.Add(row[0], row[1]);   // 県、概略URL
                        }
                        name_url_2_3.Add(row[2], row[3]);   // 地域、地域URL
                        name_url_2_4.Add(row[2], row[0]);   // 地域、県
                    }
                }

                // ----------------------------------------------------
                parser = new TextFieldParser("regionlist3.csv",
                    System.Text.Encoding.GetEncoding("Shift_JIS"));

                using (parser)
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(","); // 区切り文字はコンマ

                    // parser.HasFieldsEnclosedInQuotes = false;
                    // parser.TrimWhiteSpace = false;
                    int last_l = 0;
                    int last_m = 0;
                    
                    List<string> tmp_name_array = new List<string>();
                    List<string> tmp_url_array = new List<string>();
                    List<List<string>> tmp_name_array2 = new List<List<string>>();
                    List<List<string>> tmp_url_array2 = new List<List<string>>();

                    while (!parser.EndOfData)
                    {
                        string[] row = parser.ReadFields(); // 1行読み込み

                        if (int.Parse(row[1]) > last_m)
                        {
                            tmp_name_array2.Add(tmp_name_array);
                            tmp_name_array = new List<string>();
                            tmp_url_array2.Add(tmp_url_array);
                            tmp_url_array = new List<string>();
                        }
                        else if (int.Parse(row[0]) > last_l)
                        {
                            tmp_name_array2.Add(tmp_name_array);
                            tmp_name_array = new List<string>();
                            name_array3.Add(tmp_name_array2);
                            tmp_name_array2 = new List<List<string>>();
                            tmp_url_array2.Add(tmp_url_array);
                            tmp_url_array = new List<string>();
                            url_array3.Add(tmp_url_array2);
                            tmp_url_array2 = new List<List<string>>();
                        }

                        tmp_name_array.Add(row[3]);
                        tmp_url_array.Add(row[4]);
                        last_l = int.Parse(row[0]);
                        last_m = int.Parse(row[1]);
                    }
                    tmp_name_array2.Add(tmp_name_array);
                    tmp_name_array = new List<string>();
                    name_array3.Add(tmp_name_array2);
                    tmp_name_array2 = new List<List<string>>();
                    tmp_url_array2.Add(tmp_url_array);
                    tmp_url_array = new List<string>();
                    url_array3.Add(tmp_url_array2);
                    tmp_url_array2 = new List<List<string>>();
                }
                comboBox1.SelectedIndex = Form1.selected_l;
                comboBox2.SelectedIndex = Form1.selected_m;
                comboBox3.SelectedIndex = Form1.selected_n;
            }
            catch (Exception e2)
            {
                // 地域ファイルが存在しない場合
                MessageBox.Show("地域設定ファイルが見つかりません。データ取得ボタンを押してください。", "設定ファイルチェック");
                button3.Enabled = true;
                button3.Visible = true;
            }

            button3.Enabled = true;
            button3.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button3.Text = "データ取得中...";
            button3.Refresh();

            //int l_old = comboBox1.SelectedIndex;
            //int m_old = comboBox2.SelectedIndex;
            //int n_old = comboBox3.SelectedIndex;

            ////string enc = "EUC-JP";

            //// ------------------------------------------------------
            ////ダウンロードするURL（週間天気一覧）
            //string url = "http://weather.yahoo.co.jp/weather/week/";

            //string source = "";

            //try
            //{
            //    //WebClientの作成
            //    WebClient wc = new WebClient();
            //    //文字コードを指定
            //    wc.Encoding = Encoding.GetEncoding(51932);
            //    //HTMLソースをダウンロードする
            //    source = wc.DownloadString(url);
            //    //後始末
            //    wc.Dispose();
            //}
            //catch (System.Net.WebException e2)
            //{
            //    //MessageBox.Show(e2.ToString());
            //    MessageBox.Show(
            //        "ネットワークに正しく接続されているか確認してください\nまた、接続先サーバが正常通り稼働しているか確認してください",
            //        "エラー");
            //    button3.Enabled = true;
            //    button1.Text = "データ取得";
            //    return;
            //}

            //CreteLib.HtmlDocument doc = new CreteLib.HtmlDocument();
            //doc.LoadHtml(source);

            ////----------------------------------------------------------

            //List<string> url_array1 = new List<string>();
            //List<int> count_array = new List<int>();
            //name_array1.Clear();
            //name_array2.Clear();
            //url_array2.Clear();
            //name_array3.Clear();
            //url_array3.Clear();

            ////----------------------------------------------------------

            //HtmlNodeList nodeList = doc.GetNodesByTagName("a");

            //foreach (HtmlNode node in nodeList)
            //{
            //    // href属性のないaがあるので、try-catchにしておく
            //    try
            //    {
            //        if (Regex.IsMatch(node["href"], @"^/weather/week/"))
            //        {
            //            if (node["href"].Contains("#"))
            //            {
            //                string[] st1 = node["href"].Split('#');
            //                if (!url_array1.Contains(st1[0]))
            //                {
            //                    url_array1.Add(st1[0]);
            //                }
            //            }
            //            else
            //            {
            //                url_array1.Add(node["href"]);
            //            }
            //        }
            //    }
            //    catch (Exception e2)
            //    {
            //    }
            //}

            ////----------------------------------------------------------

            //int j = 0;

            //// 各地方の下のURL（県一覧）を掘る
            //foreach (string url1 in url_array1)
            //{
            //    //ダウンロードするURL（週間天気一覧）
            //    string url2 = "http://weather.yahoo.co.jp" + url1;

            //    //WebClientの作成
            //    WebClient wc2 = new WebClient();
            //    //文字コードを指定
            //    wc2.Encoding = Encoding.GetEncoding(51932);
            //    //HTMLソースをダウンロードする
            //    string source2 = wc2.DownloadString(url2);
            //    //後始末
            //    wc2.Dispose();

            //    CreteLib.HtmlDocument doc2 = new CreteLib.HtmlDocument();
            //    doc2.LoadHtml(source2);


            //    nodeList = doc2.GetNodesByTagName("h3");

            //    foreach (HtmlNode node in nodeList)
            //    {
            //        name_array1.Add(node.InnerText);
            //    }

            //    // 県ごとに区切りを入れるため（二次元配列に入れるため）
            //    // 先に区切りとなる個数を覚えておく
            //    nodeList = doc2.GetNodesByTagName("span");
            //    foreach (HtmlNode node in nodeList)
            //    {
            //        if (node["class"].Equals("yjSt"))
            //        {
            //            // 文字の出現回数をカウント
            //            int c = node.InnerText.Length - node.InnerText.Replace("|", "").Length;

            //            // 大阪は地域分けがないらしい・・・
            //            if (node.InnerText.Contains("["))
            //            {
            //                count_array.Add(c + 2);
            //            }
            //            else
            //            {
            //                count_array.Add(c + 1);
            //            }
            //        }
            //    }

            //    nodeList = doc2.GetNodesByTagName("a");
            //    List<string> tmp_name_array = new List<string>();
            //    List<string> tmp_url_array = new List<string>();

            //    foreach (HtmlNode node in nodeList)
            //    {
            //        // href属性のないaがあるので、try-catchにしておく
            //        try
            //        {
            //            if (Regex.IsMatch(node["href"], @"^/weather/jp/[0-9]"))
            //            {
            //                string[] st1 = node["href"].Split('#');
            //                tmp_url_array.Add(st1[0]);

            //                tmp_name_array.Add(node.InnerText);

            //                if (tmp_name_array.Count == count_array[j])
            //                {
            //                    url_array2.Add(tmp_url_array);
            //                    name_array2.Add(tmp_name_array);
            //                    tmp_url_array = new List<string>();
            //                    tmp_name_array = new List<string>();
            //                    j++;
            //                }

            //            }
            //        }
            //        catch (Exception e2)
            //        {
            //        }
            //    }
            //}
            ////----------------------------------------------------------

            //// 県一覧の下のURL（市区町村）を掘る
            //foreach (List<string> u_array in url_array2)
            //{
            //    List<List<string>> tmp_name_array2 = new List<List<string>>();
            //    List<List<string>> tmp_url_array2 = new List<List<string>>();

            //    foreach (string url1 in u_array)
            //    {
            //        //ダウンロードするURL（ピンポイント予報）
            //        string url2 = "http://weather.yahoo.co.jp" + url1;

            //        //WebClientの作成
            //        WebClient wc2 = new WebClient();
            //        //文字コードを指定
            //        wc2.Encoding = Encoding.GetEncoding(51932);
            //        //HTMLソースをダウンロードする
            //        string source2 = wc2.DownloadString(url2);
            //        //後始末
            //        wc2.Dispose();

            //        CreteLib.HtmlDocument doc2 = new CreteLib.HtmlDocument();
            //        doc2.LoadHtml(source2);

            //        nodeList = doc2.GetNodesByTagName("a");
            //        List<string> tmp_name_array = new List<string>();
            //        List<string> tmp_url_array = new List<string>();

            //        foreach (HtmlNode node in nodeList)
            //        {
            //            // href属性のないaがあるので、try-catchにしておく
            //            try
            //            {
            //                if (Regex.IsMatch(node["href"], @"^http://rd.yahoo.co.jp/media/weather/fuken/pinpo/"))
            //                {
            //                    string[] st1 = node["href"].Split('?');
            //                    tmp_url_array.Add(st1[1]);
            //                    tmp_name_array.Add(node.InnerText);
            //                }
            //            }
            //            catch (Exception e2)
            //            {
            //            }
            //        }
            //        tmp_name_array2.Add(tmp_name_array);
            //        tmp_url_array2.Add(tmp_url_array);
            //    }
            //    name_array3.Add(tmp_name_array2);
            //    url_array3.Add(tmp_url_array2);
            //}

            ////----------------------------------------------------------
            ////Shift JISで書き込む
            ////書き込むファイルが既に存在している場合は、上書きする
            //System.IO.StreamWriter sw = new System.IO.StreamWriter(
            //    @"./regionlist1.csv",
            //    false,
            //    System.Text.Encoding.GetEncoding("shift_jis"));
            ////TextBox1.Textの内容を書き込む

            //foreach (string s in name_array1)
            //{
            //    //textBox1.Text += s + "\r\n";
            //    sw.Write(s + "\r\n");
            //}
            ////textBox1.Text += "----------------------------" + "\r\n";
            //sw.Close();

            ////----------------------------------------------------------
            //int l = 0;
            //int m = 0;
            //sw = new System.IO.StreamWriter(
            //    @"./regionlist2.csv",
            //    false,
            //    System.Text.Encoding.GetEncoding("shift_jis"));
            //foreach (List<string> s_array in name_array2)
            //{
            //    m = 0;
            //    foreach (string s in s_array)
            //    {
            //        //textBox1.Text += s + "\r\n";
            //        sw.Write(l.ToString() + "," + m.ToString() + "," + s + "\r\n");
            //        m++;
            //    }
            //    l++;
            //    //textBox1.Text += "- - - - - - - - - - - - - - - " + "\r\n";
            //}
            ////textBox1.Text += "----------------------------" + "\r\n";
            //sw.Close();

            ////----------------------------------------------------------
            //l = 0;
            //m = 0;
            //int n = 0;
            //sw = new System.IO.StreamWriter(
            //    @"./regionlist3.csv",
            //    false,
            //    System.Text.Encoding.GetEncoding("shift_jis"));
            //foreach (List<List<string>> s_array2 in name_array3)
            //{
            //    m = 0;
            //    foreach (List<string> s_array in s_array2)
            //    {
            //        n = 0;
            //        foreach (string s in s_array)
            //        {
            //            //textBox1.Text += s + "\r\n";
            //            sw.Write(l.ToString() + "," + m.ToString() + "," + n.ToString() + "," + s + "," + url_array3[l][m][n] + "\r\n");
            //            n++;
            //        }
            //        //textBox1.Text += "-   -   -   -   -   -   -   - " + "\r\n";
            //        m++;
            //    }
            //    //textBox1.Text += "- - - - - - - - - - - - - - - " + "\r\n";
            //    l++;
            //}
            ////textBox1.Text += "----------------------------" + "\r\n";
            //sw.Close();

            //// comboBoxにまとめて追加する
            //comboBox1.Items.Clear();
            //comboBox1.Items.AddRange(name_array1.ToArray());

            //comboBox1.SelectedIndex = l_old;
            //comboBox2.SelectedIndex = m_old;
            //comboBox3.SelectedIndex = n_old;

            // ------------------------------------------------------
            //ダウンロードするURL（警報・注意報）
            string url2 = "http://bousai.tenki.jp/bousai/warn/";

            string source2 = "";

            try
            {
                //WebClientの作成
                WebClient wc = new WebClient();
                //文字コードを指定
                wc.Encoding = Encoding.UTF8;
                //HTMLソースをダウンロードする
                source2 = wc.DownloadString(url2);
                //後始末
                wc.Dispose();
            }
            catch (System.Net.WebException e2)
            {
                //MessageBox.Show(e2.ToString());
                MessageBox.Show(
                    "ネットワークに正しく接続されているか確認してください\nまた、接続先サーバが正常通り稼働しているか確認してください",
                    "エラー");
                button3.Enabled = true;
                button1.Text = "データ取得";
                return;
            }

            CreteLib.HtmlDocument doc = new CreteLib.HtmlDocument();
            doc.LoadHtml(source2);

            HtmlNodeList nodeList = doc.GetNodesByTagName("a");

            Dictionary<string, string> l_list_warn = new Dictionary<string, string>();


            foreach (HtmlNode node in nodeList)
            {
                // href属性のないaがあるので、try-catchにしておく
                try
                {
                    if (Regex.IsMatch(node["href"], @"^http://bousai.tenki.jp/bousai/warn/pref-[0-9]+.html") &&
                        !Regex.IsMatch(node.InnerText, @"^<img>"))
                    {
                        String tmp_pref = node.InnerText.Replace("<img>", "");
                        //MessageBox.Show(node["href"]);
                        l_list_warn.Add(tmp_pref, node["href"]);
                    }
                }
                catch (Exception e2)
                {
                }
            }

            Dictionary<string, string> l_list_warn2 = new Dictionary<string, string>();
            Dictionary<string, string> l_list_warn3 = new Dictionary<string, string>();
            Dictionary<string, string> l_list_warn4 = new Dictionary<string, string>();
            
            foreach (KeyValuePair<string, string> keyValuePair in l_list_warn)
            {
                //MessageBox.Show(keyValuePair.Key + ", " + keyValuePair.Value);
                String url_detail = keyValuePair.Value;

                string source_detail = "";

                try
                {
                    //WebClientの作成
                    WebClient wc2 = new WebClient();
                    //文字コードを指定
                    wc2.Encoding = Encoding.UTF8;
                    //HTMLソースをダウンロードする
                    source_detail = wc2.DownloadString(url_detail);
                    //後始末
                    wc2.Dispose();
                }
                catch (System.Net.WebException e2)
                {
                    //MessageBox.Show(e2.ToString());
                    MessageBox.Show(
                        "ネットワークに正しく接続されているか確認してください\nまた、接続先サーバが正常通り稼働しているか確認してください",
                        "エラー");
                    button3.Enabled = true;
                    button1.Text = "データ取得";
                    return;
                }

                CreteLib.HtmlDocument doc2 = new CreteLib.HtmlDocument();
                doc2.LoadHtml(source_detail);

                HtmlNodeList nodeList2 = doc2.GetNodesByTagName("a");

                foreach (HtmlNode node in nodeList2)
                {
                    // href属性のないaがあるので、try-catchにしておく
                    try
                    {
                        if (Regex.IsMatch(node["href"], @"^/bousai/warn/point-[0-9]+.html") &&
                            !Regex.IsMatch(node.InnerText, @"^<img>"))
                        {
                            String tmp_pref = node.InnerText.Replace("<img>", "");
                            //MessageBox.Show(node["href"]);
                            l_list_warn2.Add(tmp_pref, node["href"]);
                            l_list_warn3.Add(tmp_pref, keyValuePair.Key);
                            l_list_warn4.Add(tmp_pref, keyValuePair.Value);
                        }
                    }
                    catch (Exception e2)
                    {
                    }
                }
            }

            StreamWriter sw = new System.IO.StreamWriter(
                @"./regionlist2_2.csv",
                false,
                System.Text.Encoding.GetEncoding("shift_jis"));

            foreach (KeyValuePair<string, string> keyValuePair in l_list_warn2)
            {
                sw.Write(l_list_warn3[keyValuePair.Key] + "," + l_list_warn4[keyValuePair.Key] + "," + keyValuePair.Key + "," + keyValuePair.Value + "\r\n");
                //MessageBox.Show(keyValuePair.Key + ", " + keyValuePair.Value);
            }
            sw.Close();

            button3.Enabled = true;
            button3.Text = "データ取得";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(name_array2[comboBox1.SelectedIndex].ToArray());
            comboBox2.SelectedIndex = 0;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(name_array3[comboBox1.SelectedIndex][comboBox2.SelectedIndex].ToArray());
            comboBox3.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int l = comboBox1.SelectedIndex;
            int m = comboBox2.SelectedIndex;
            int n = comboBox3.SelectedIndex;
            Form1.selected_l = l;
            Form1.selected_m = m;
            Form1.selected_n = n;
            Form1.l_name = name_array1[l];
            Form1.m_name = name_array2[l][m];
            Form1.n_name = name_array3[l][m][n];
            Form1.url_warn = String.Empty;
            
            //name_url_2_2 県、概略URL
            //name_url_2_3 地域、地域URL
            //name_url_2_4 地域、県
            foreach(string key in name_url_2_2.Keys){
                // key 県
                if (name_array1[l].Contains(key))
                {
                    Form1.url_abst = name_url_2_2[key];

                    // key2 地域
                    foreach (string key2 in name_url_2_4.Keys)
                    {
                        if (name_url_2_4[key2].Equals(key) && name_array3[l][m][n].Contains(key2))
                        {
                            Form1.url_warn = name_url_2_3[key2];
                            break;
                        }
                    }

                    // 西海市（江島・平島を除く）など注釈がついている地域用
                    if (Form1.url_warn == String.Empty)
                    {
                        foreach (string key2 in name_url_2_4.Keys)
                        {
                            // TODO:ほんとは複数マッチするはずなので、選択ボックスを出すべき
                            if (name_url_2_4[key2].Equals(key) && key2.Contains(name_array3[l][m][n]))
                            {
                                Form1.url_warn = name_url_2_3[key2];
                                break;
                            }
                        }
                    }
                    //break;
                }
            }

            Form1.conf_ok = true;

            // 設定ファイルへ保存
            Properties.Settings.Default.l = l;
            Properties.Settings.Default.m = m;
            Properties.Settings.Default.n = n;

            Properties.Settings.Default.bool_max_max_temp   = (bool)dataGridView1.Rows[0].Cells[0].Value;
            Properties.Settings.Default.bool_max_min_temp   = (bool)dataGridView1.Rows[1].Cells[0].Value;
            Properties.Settings.Default.bool_min_max_temp   = (bool)dataGridView1.Rows[2].Cells[0].Value;
            Properties.Settings.Default.bool_min_min_temp   = (bool)dataGridView1.Rows[3].Cells[0].Value;
            Properties.Settings.Default.bool_max_wet        = (bool)dataGridView1.Rows[4].Cells[0].Value;
            Properties.Settings.Default.bool_min_wet        = (bool)dataGridView1.Rows[5].Cells[0].Value;
            Properties.Settings.Default.bool_max_rain       = (bool)dataGridView1.Rows[6].Cells[0].Value;
            Properties.Settings.Default.bool_min_rain       = (bool)dataGridView1.Rows[7].Cells[0].Value;
            Properties.Settings.Default.bool_max_wind       = (bool)dataGridView1.Rows[8].Cells[0].Value;
            Properties.Settings.Default.bool_min_wind       = (bool)dataGridView1.Rows[9].Cells[0].Value;
            Properties.Settings.Default.bool_max_wash       = (bool)dataGridView1.Rows[10].Cells[0].Value;
            Properties.Settings.Default.bool_min_wash       = (bool)dataGridView1.Rows[11].Cells[0].Value;
            Properties.Settings.Default.bool_max_ult_vio    = (bool)dataGridView1.Rows[12].Cells[0].Value;
            Properties.Settings.Default.bool_min_ult_vio    = (bool)dataGridView1.Rows[13].Cells[0].Value;
            Properties.Settings.Default.bool_max_umb        = (bool)dataGridView1.Rows[14].Cells[0].Value;
            Properties.Settings.Default.bool_min_umb        = (bool)dataGridView1.Rows[15].Cells[0].Value;
            Properties.Settings.Default.bool_max_star       = (bool)dataGridView1.Rows[16].Cells[0].Value;
            Properties.Settings.Default.bool_min_star       = (bool)dataGridView1.Rows[17].Cells[0].Value;

            Properties.Settings.Default.max_max_temp    = (string)dataGridView1.Rows[0].Cells[2].Value;
            Properties.Settings.Default.max_min_temp    = (string)dataGridView1.Rows[1].Cells[2].Value;
            Properties.Settings.Default.min_max_temp    = (string)dataGridView1.Rows[2].Cells[2].Value;
            Properties.Settings.Default.min_min_temp    = (string)dataGridView1.Rows[3].Cells[2].Value;
            Properties.Settings.Default.max_wet         = (string)dataGridView1.Rows[4].Cells[2].Value;
            Properties.Settings.Default.min_wet         = (string)dataGridView1.Rows[5].Cells[2].Value;
            Properties.Settings.Default.max_rain        = (string)dataGridView1.Rows[6].Cells[2].Value;
            Properties.Settings.Default.min_rain        = (string)dataGridView1.Rows[7].Cells[2].Value;
            Properties.Settings.Default.max_wind        = (string)dataGridView1.Rows[8].Cells[2].Value;
            Properties.Settings.Default.min_wind        = (string)dataGridView1.Rows[9].Cells[2].Value;
            Properties.Settings.Default.max_wash        = (string)dataGridView1.Rows[10].Cells[2].Value;
            Properties.Settings.Default.min_wash        = (string)dataGridView1.Rows[11].Cells[2].Value;
            Properties.Settings.Default.max_ul_vio     = (string)dataGridView1.Rows[12].Cells[2].Value;
            Properties.Settings.Default.min_ul_vio     = (string)dataGridView1.Rows[13].Cells[2].Value;
            Properties.Settings.Default.max_umb         = (string)dataGridView1.Rows[14].Cells[2].Value;
            Properties.Settings.Default.min_umb         = (string)dataGridView1.Rows[15].Cells[2].Value;
            Properties.Settings.Default.max_star        = (string)dataGridView1.Rows[16].Cells[2].Value;
            Properties.Settings.Default.min_star        = (string)dataGridView1.Rows[17].Cells[2].Value;
            
            Properties.Settings.Default.color_max_max_temp  = (System.Drawing.Color)dataGridView1.Rows[0].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_min_temp  = (System.Drawing.Color)dataGridView1.Rows[1].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_max_temp  = (System.Drawing.Color)dataGridView1.Rows[2].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_min_temp  = (System.Drawing.Color)dataGridView1.Rows[3].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_wet       = (System.Drawing.Color)dataGridView1.Rows[4].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_wet       = (System.Drawing.Color)dataGridView1.Rows[5].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_rain      = (System.Drawing.Color)dataGridView1.Rows[6].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_rain      = (System.Drawing.Color)dataGridView1.Rows[7].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_wind      = (System.Drawing.Color)dataGridView1.Rows[8].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_wind      = (System.Drawing.Color)dataGridView1.Rows[9].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_wash      = (System.Drawing.Color)dataGridView1.Rows[10].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_wash      = (System.Drawing.Color)dataGridView1.Rows[11].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_ul_vio   = (System.Drawing.Color)dataGridView1.Rows[12].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_ul_vio   = (System.Drawing.Color)dataGridView1.Rows[13].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_umb       = (System.Drawing.Color)dataGridView1.Rows[14].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_umb       = (System.Drawing.Color)dataGridView1.Rows[15].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_max_star      = (System.Drawing.Color)dataGridView1.Rows[16].Cells[3].Style.BackColor;
            Properties.Settings.Default.color_min_star      = (System.Drawing.Color)dataGridView1.Rows[17].Cells[3].Style.BackColor;
            Properties.Settings.Default.Save();

            Form1.url_smallregion = url_array3[l][m][n];

            this.Close();
        }

        // DataGridViewのコンボボックスのドロップダウンリストが一回のクリックで表示されるようにする
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.Columns[e.ColumnIndex].Name == "Column3" &&
               dgv.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                SendKeys.Send("{F4}");
            }
        }

        // ボタンがクリックされたことを知る
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            //"Button"列ならば、ボタンがクリックされた
            if (dgv.Columns[e.ColumnIndex].Name == "Column4")
            {
                //MessageBox.Show(e.RowIndex.ToString() +
                //    "行のボタンがクリックされました。");

                // ColorDialog の新しいインスタンスを生成する (デザイナから追加している場合は必要ない)
                ColorDialog colorDialog1 = new ColorDialog();

                // セルの選択を解除
                // ボタンの色を戻すために必要
                dataGridView1.ClearSelection();

                // 初期選択する色を設定する
                //colorDialog1.Color = textBox1.BackColor;
                colorDialog1.Color = dataGridView1.Rows[e.RowIndex].Cells[3].Style.BackColor;

                // カスタム カラーを定義可能にする (初期値 true)
                //colorDialog1.AllowFullOpen = true;

                // カスタム カラーを表示した状態にする (初期値 false)
                colorDialog1.FullOpen = true;

                // 使用可能なすべての色を基本セットに表示する (初期値 false)
                colorDialog1.AnyColor = true;

                // 純色のみ表示する (初期値 false)
                colorDialog1.SolidColorOnly = true;

                // カスタム カラーを任意の色で設定する
                //colorDialog1.CustomColors = new int[] { 0xFFE4E1, 0xE0FFFF };
                colorDialog1.CustomColors = new int[] { ColorTranslator.ToWin32(Color.MistyRose), ColorTranslator.ToWin32(Color.LightCyan) };

                // [ヘルプ] ボタンを表示する
                colorDialog1.ShowHelp = true;

                // ダイアログを表示し、戻り値が [OK] の場合は選択した色を textBox1 に適用する
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    //textBox1.BackColor = colorDialog1.Color;
                    dataGridView1.Rows[e.RowIndex].Cells[3].Style.BackColor = colorDialog1.Color;
                }

                // OKボタンにフォーカスを移す
                button1.Focus();

                // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
                colorDialog1.Dispose();
            }
        }
    }
}
