using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace YSedit
{
    static class Program
    {

        static public Form1 form;
        static public Option option = new Option();

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            loadResourceFiles();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            Application.Run(form);
        }

        public static void loadResourceFiles()
        {
            try
            {
                ObjectInfo.init();
                MapName.init();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "The exception raised in loading resource files:\n" + e.ToString(),
                    "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void showInfo(string s)
        {
            form.setInfoStatusText(s);
        }

        public static void showError(string s)
        {
            form.setInfoStatusText("Error: " + s);
            System.Media.SystemSounds.Exclamation.Play();
        }

        public static string getResourceDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
