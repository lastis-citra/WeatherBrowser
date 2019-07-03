namespace WeatherBrowser
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.icon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hours = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.weather = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.temperature = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.wet = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rain = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.winddr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.windst = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.コピーCToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.listView2 = new System.Windows.Forms.ListView();
            this.icon2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.date2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.weather2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.max_temp2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.min_temp2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rain_rate2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.wash_rate2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ul_vio_rate2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.umb_rate2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.star_rate2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.コピーCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ファイルFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.履歴BToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.終了EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.設定OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.履歴を消去EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.オプションOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.バージョン情報AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(432, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "情報取得";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.icon,
            this.date,
            this.hours,
            this.weather,
            this.temperature,
            this.wet,
            this.rain,
            this.winddr,
            this.windst});
            this.listView1.ContextMenuStrip = this.contextMenuStrip2;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(12, 56);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(504, 300);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // icon
            // 
            this.icon.Text = "";
            this.icon.Width = 20;
            // 
            // date
            // 
            this.date.Tag = "";
            this.date.Text = "日付";
            this.date.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.date.Width = 80;
            // 
            // hours
            // 
            this.hours.Text = "時間";
            this.hours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.hours.Width = 50;
            // 
            // weather
            // 
            this.weather.Text = "天気";
            this.weather.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.weather.Width = 50;
            // 
            // temperature
            // 
            this.temperature.Text = "気温";
            this.temperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // wet
            // 
            this.wet.Text = "湿度";
            this.wet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rain
            // 
            this.rain.Text = "降水量";
            this.rain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // winddr
            // 
            this.winddr.Text = "風向";
            this.winddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // windst
            // 
            this.windst.Text = "風速";
            this.windst.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.コピーCToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(131, 26);
            // 
            // コピーCToolStripMenuItem1
            // 
            this.コピーCToolStripMenuItem1.Name = "コピーCToolStripMenuItem1";
            this.コピーCToolStripMenuItem1.Size = new System.Drawing.Size(130, 22);
            this.コピーCToolStripMenuItem1.Text = "コピー（&C）";
            this.コピーCToolStripMenuItem1.Click += new System.EventHandler(this.コピーCToolStripMenuItem1_Click);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.icon2,
            this.date2,
            this.weather2,
            this.max_temp2,
            this.min_temp2,
            this.rain_rate2,
            this.wash_rate2,
            this.ul_vio_rate2,
            this.umb_rate2,
            this.star_rate2});
            this.listView2.ContextMenuStrip = this.contextMenuStrip1;
            this.listView2.GridLines = true;
            this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView2.Location = new System.Drawing.Point(11, 377);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(506, 164);
            this.listView2.TabIndex = 3;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // icon2
            // 
            this.icon2.Text = "";
            this.icon2.Width = 20;
            // 
            // date2
            // 
            this.date2.Tag = "";
            this.date2.Text = "日付";
            this.date2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.date2.Width = 80;
            // 
            // weather2
            // 
            this.weather2.Text = "天気";
            this.weather2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // max_temp2
            // 
            this.max_temp2.Text = "最高";
            this.max_temp2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.max_temp2.Width = 48;
            // 
            // min_temp2
            // 
            this.min_temp2.Text = "最低";
            this.min_temp2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.min_temp2.Width = 48;
            // 
            // rain_rate2
            // 
            this.rain_rate2.Text = "降水確率";
            this.rain_rate2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.rain_rate2.Width = 102;
            // 
            // wash_rate2
            // 
            this.wash_rate2.Text = "洗濯";
            this.wash_rate2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.wash_rate2.Width = 40;
            // 
            // ul_vio_rate2
            // 
            this.ul_vio_rate2.Text = "紫";
            this.ul_vio_rate2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ul_vio_rate2.Width = 24;
            // 
            // umb_rate2
            // 
            this.umb_rate2.Text = "傘";
            this.umb_rate2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.umb_rate2.Width = 40;
            // 
            // star_rate2
            // 
            this.star_rate2.Text = "星空";
            this.star_rate2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.star_rate2.Width = 40;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.コピーCToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(131, 26);
            // 
            // コピーCToolStripMenuItem
            // 
            this.コピーCToolStripMenuItem.Name = "コピーCToolStripMenuItem";
            this.コピーCToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.コピーCToolStripMenuItem.Text = "コピー（&C）";
            this.コピーCToolStripMenuItem.Click += new System.EventHandler(this.コピーCToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 362);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "【週間天気】";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "【今日・明日の天気】";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルFToolStripMenuItem,
            this.設定OToolStripMenuItem,
            this.ヘルプHToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(529, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ファイルFToolStripMenuItem
            // 
            this.ファイルFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.履歴BToolStripMenuItem,
            this.終了EToolStripMenuItem});
            this.ファイルFToolStripMenuItem.Name = "ファイルFToolStripMenuItem";
            this.ファイルFToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.ファイルFToolStripMenuItem.Text = "ファイル（&F）";
            // 
            // 履歴BToolStripMenuItem
            // 
            this.履歴BToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.履歴BToolStripMenuItem.Name = "履歴BToolStripMenuItem";
            this.履歴BToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.履歴BToolStripMenuItem.Text = "履歴（&B）";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.testToolStripMenuItem.Text = "（履歴はありません）";
            // 
            // 終了EToolStripMenuItem
            // 
            this.終了EToolStripMenuItem.Name = "終了EToolStripMenuItem";
            this.終了EToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.終了EToolStripMenuItem.Text = "閉じる（&X）";
            this.終了EToolStripMenuItem.Click += new System.EventHandler(this.終了EToolStripMenuItem_Click);
            // 
            // 設定OToolStripMenuItem
            // 
            this.設定OToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.履歴を消去EToolStripMenuItem,
            this.オプションOToolStripMenuItem});
            this.設定OToolStripMenuItem.Name = "設定OToolStripMenuItem";
            this.設定OToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.設定OToolStripMenuItem.Text = "設定（&O）";
            // 
            // 履歴を消去EToolStripMenuItem
            // 
            this.履歴を消去EToolStripMenuItem.Name = "履歴を消去EToolStripMenuItem";
            this.履歴を消去EToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.履歴を消去EToolStripMenuItem.Text = "履歴を消去（&E）...";
            this.履歴を消去EToolStripMenuItem.Click += new System.EventHandler(this.履歴を消去EToolStripMenuItem_Click);
            // 
            // オプションOToolStripMenuItem
            // 
            this.オプションOToolStripMenuItem.Name = "オプションOToolStripMenuItem";
            this.オプションOToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.オプションOToolStripMenuItem.Text = "オプション（&P）...";
            this.オプションOToolStripMenuItem.Click += new System.EventHandler(this.オプションOToolStripMenuItem_Click);
            // 
            // ヘルプHToolStripMenuItem
            // 
            this.ヘルプHToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.バージョン情報AToolStripMenuItem});
            this.ヘルプHToolStripMenuItem.Name = "ヘルプHToolStripMenuItem";
            this.ヘルプHToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.ヘルプHToolStripMenuItem.Text = "ヘルプ（&H）";
            // 
            // バージョン情報AToolStripMenuItem
            // 
            this.バージョン情報AToolStripMenuItem.Name = "バージョン情報AToolStripMenuItem";
            this.バージョン情報AToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.バージョン情報AToolStripMenuItem.Text = "バージョン情報（&A）...";
            this.バージョン情報AToolStripMenuItem.Click += new System.EventHandler(this.バージョン情報AToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(356, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 12);
            this.label3.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(356, 362);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 12);
            this.label4.TabIndex = 8;
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            this.listView3.GridLines = true;
            this.listView3.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView3.Location = new System.Drawing.Point(12, 562);
            this.listView3.Name = "listView3";
            this.listView3.Scrollable = false;
            this.listView3.Size = new System.Drawing.Size(504, 36);
            this.listView3.TabIndex = 9;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "";
            this.columnHeader2.Text = "";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 56;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 56;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 56;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 56;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 56;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 56;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 56;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 56;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader10.Width = 56;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 547);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "【現在の警報・注意報】";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(435, 547);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 12);
            this.label6.TabIndex = 11;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 621);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(504, 66);
            this.textBox1.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 606);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "【天気概況】";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(435, 606);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 12);
            this.label8.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 697);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listView3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::WeatherBrowser.Properties.Settings.Default, "MyLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::WeatherBrowser.Properties.Settings.Default.MyLocation;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "WeatherBrowser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader date;
        private System.Windows.Forms.ColumnHeader hours;
        private System.Windows.Forms.ColumnHeader weather;
        private System.Windows.Forms.ColumnHeader temperature;
        private System.Windows.Forms.ColumnHeader wet;
        private System.Windows.Forms.ColumnHeader icon;
        private System.Windows.Forms.ColumnHeader rain;
        private System.Windows.Forms.ColumnHeader winddr;
        private System.Windows.Forms.ColumnHeader windst;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader icon2;
        private System.Windows.Forms.ColumnHeader date2;
        private System.Windows.Forms.ColumnHeader weather2;
        private System.Windows.Forms.ColumnHeader max_temp2;
        private System.Windows.Forms.ColumnHeader min_temp2;
        private System.Windows.Forms.ColumnHeader rain_rate2;
        private System.Windows.Forms.ColumnHeader wash_rate2;
        private System.Windows.Forms.ColumnHeader ul_vio_rate2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader umb_rate2;
        private System.Windows.Forms.ColumnHeader star_rate2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem ファイルFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 終了EToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ヘルプHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem バージョン情報AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 設定OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 履歴BToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem コピーCToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem コピーCToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 履歴を消去EToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem オプションOToolStripMenuItem;
    }
}

