using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Robopreter;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SQLiteHelper sqlitehelper;
        CanvasSize canvasSize;
        Menu menu;
        RTextboxHelper rtbhelper;
        Turtle turtle;
        CommonSyntaxProvider logoSynProvider;
        SyntaxHighlight synHighligt;

        public MainWindow()
        {
            InitializeComponent();
            canvasSize = CanvasSize.GetCanvasSize;
            InitializeCanvas();
            sqlitehelper = SQLiteHelper.GetSqlHelper;
            menu = Menu.GetMenu;
            rtbhelper = new RTextboxHelper();
            logoSynProvider = new CommonSyntaxProvider(LogoKeywords.GetLogoKeywords().GetKeywords, LogoKeywords.GetLogoKeywords().GetSpecialCharacters, false);
            synHighligt = new SyntaxHighlight(commandLine, logoSynProvider);
        }

        private void InitializeCanvas()
        {
            canvas.Height = canvasSize.Height;
            canvas.Width = canvasSize.Width;
            this.Height = canvas.Height + 75;
            this.Width = canvas.Width + 50;
            canvas.Background = new SolidColorBrush(Colors.White); 
        }
      
        //bezárás
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //mentés
        private void SaveClick(object sender, ExecutedRoutedEventArgs e)
        {
            menu.Save(rtbhelper.GetString(commandLine));
        }
        //mentés másként
        private void SaveAsClick(object sender, ExecutedRoutedEventArgs e)
        {
            menu.SaveAs(rtbhelper.GetString(commandLine), sqlitehelper.FileSource);
        }
        //megnyitás
        private void OpenClick(object sender, ExecutedRoutedEventArgs e)
        {
            if (menu.Open() == true)
            {
                sqlitehelper.SetCanvasSize();
                SetMain();
                rtbhelper.SetString(sqlitehelper.GetSourceCode(), commandLine);
                turtle = new Turtle(canvas);
                turtle.Clean();
            }
        }

        private void SetMain()
        {
            InitializeCanvas();
            rtbhelper.DeleteString(commandLine, fdtext);
            EnableMenus();
            SetStatusBar();
            SetLogoLang();
        }
        //új
        private void NewClick(object sender, ExecutedRoutedEventArgs e)
        {
            NewProject np = new NewProject();
            np.ShowDialog();
            if (np.IsSuccess)
            {
                SetMain();
                turtle = new Turtle(canvas);
                turtle.Clean();
            }
        }

        private void EnableMenus()
        {
            commandLine.IsEnabled = true;
            runButton.IsEnabled = true;
            runMenuItem.IsEnabled = true;
            clearButton.IsEnabled = true;
            clearMenuItem.IsEnabled = true;
            saveButton.IsEnabled = true;
            saveMenuItem.IsEnabled = true;
            saveAsMenuItem.IsEnabled = true;
            methodMenu.IsEnabled = true;
            projectinfoBar.Visibility = System.Windows.Visibility.Visible;
        }

        private void SetStatusBar()
        {
            creatorText.Text = App.Current.TryFindResource("username").ToString() + ": " + sqlitehelper.GetName();
            projectText.Text = App.Current.TryFindResource("projectname").ToString() + ": " + sqlitehelper.GetProjectName();
            languageText.Text = App.Current.TryFindResource("lang").ToString() + ": " + App.Current.TryFindResource(sqlitehelper.GetLanguage().ToString()).ToString();
        }
        
        //futtatás
        private async void RunClick(object sender, RoutedEventArgs e)
        {
            runButton.IsEnabled = false;
            menu.Save(rtbhelper.GetString(commandLine));
            turtle.Clean();
            RoboPreter rp = new RoboPreter();
            List<Robopreter.Parancs> com = rp.Run(rtbhelper.GetString(commandLine));
            for (int i = 0; i < com.Count; i++)
            {
                Draw(com[i]);
                int time = 800;
                if (com[i].parancs == Parancsok.balra || com[i].parancs == Parancsok.jobbra || com[i].parancs == Parancsok.elore || com[i].parancs == Parancsok.hatra)
                    time = (int)com[i].parancs * 10;
                await Task.Factory.StartNew(() => Wait(time));
            }
            runButton.IsEnabled = true;
        }

        private void Draw(Parancs com)
        {
            switch (com.parancs)
            {
                case Parancsok.elore: turtle.Forward((int)com.ertek); break;
                case Parancsok.hatra: turtle.Backward((int)com.ertek); break;
                case Parancsok.jobbra: turtle.Right((int)com.ertek); break;
                case Parancsok.balra: turtle.Left((int)com.ertek); break;
                //case Parancsok.haza: turtle.Home(); break;
                //case Parancsok.torol: turtle.Clean(); break;
                case Parancsok.tollatle: turtle.PenDown(); break;
                case Parancsok.tollatfel: turtle.PenUp(); break;
            }
        }

        private void Wait(int time)
        {
            Thread.Sleep(time);
        }
        //kép mentés
        private void SaveImageClick(object sender, RoutedEventArgs e)
        {
            menu.Image_Save(canvas, this);
        }
        //képernyő törlés
        private void ClearClick(object sender, RoutedEventArgs e)
        {
            menu.Clear(turtle);
        }

        private void Method_Click(object sender, RoutedEventArgs e)
        {
            Method method = new Method();
            method.ShowDialog();
        }

        private void SetLogoLang()
        {
            string logolang = sqlitehelper.GetLanguage();

            switch (logolang)
            {
                case "langhu":
                    App.Current.Resources.MergedDictionaries[1].Source = new Uri("SyntaxKeywords/Hungarian.xaml", UriKind.Relative);
                    break;
                case "langen":
                    App.Current.Resources.MergedDictionaries[1].Source = new Uri("SyntaxKeywords/English.xaml", UriKind.Relative);
                    break;
            }
            LogoKeywords.GetLogoKeywords().UpdateKeywords();

        }
       
        private void LangHu_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Resources.MergedDictionaries[0].Source = new Uri("Languages/Hungarian.xaml", UriKind.Relative);

        }

        private void LangEn_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Resources.MergedDictionaries[0].Source = new Uri("Languages/English.xaml", UriKind.Relative);
        }
       
    }
}
