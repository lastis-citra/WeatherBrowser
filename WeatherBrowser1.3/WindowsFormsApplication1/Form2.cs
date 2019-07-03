using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace WeatherBrowser
{
    public partial class Form2 : Form
    {
        // SHGetFileInfo関数
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        // SHGetFileInfo関数で使用するフラグ
        private const uint SHGFI_ICON = 0x100; // アイコン・リソースの取得
        private const uint SHGFI_LARGEICON = 0x0; // 大きいアイコン
        private const uint SHGFI_SMALLICON = 0x1; // 小さいアイコン

        // SHGetFileInfo関数で使用する構造体
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public Form2()
        {
            InitializeComponent();

            // バージョン名（AssemblyInformationalVersion属性）を取得
            string appVersion = Application.ProductVersion;
            // 製品名（AssemblyProduct属性）を取得
            string appProductName = Application.ProductName;
            // 会社名（AssemblyCompany属性）を取得
            string appCompanyName = Application.CompanyName;
            // ***** アセンブリから直接取得 *****
            Assembly mainAssembly = Assembly.GetEntryAssembly();

            label1.Text = appProductName + "\nVersion " + appVersion + "\nBy " + appCompanyName;

            Icon appIcon;
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hSuccess = SHGetFileInfo(
              mainAssembly.Location, 0,
              ref shinfo, (uint)Marshal.SizeOf(shinfo),
              SHGFI_ICON | SHGFI_LARGEICON);
            if (hSuccess != IntPtr.Zero)
            {
                appIcon = Icon.FromHandle(shinfo.hIcon);
            }
            else
            {
                appIcon = SystemIcons.Application;
            }
            pictureBox1.Image = appIcon.ToBitmap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
