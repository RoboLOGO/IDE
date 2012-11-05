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
            logoSynProvider = new CommonSyntaxProvider(LogoKeywords.GetKeywords, LogoKeywords.GetSpecialCharacters, false);
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
                InitializeCanvas();
                rtbhelper.DeleteString(commandLine,fdtext);
                rtbhelper.SetString(sqlitehelper.GetSourceCode(), commandLine);
                EnableMenus();
                SetStatusBar();
                turtle = new Turtle(canvas);

                if (turtle != null) turtle.Clean();
            }
        }
        //új
        private void NewClick(object sender, ExecutedRoutedEventArgs e)
        {
            NewProject np = new NewProject();
            np.ShowDialog();
            if (np.IsSuccess)
            {
                InitializeCanvas();
                rtbhelper.DeleteString(commandLine,fdtext);
                EnableMenus();
                SetStatusBar();
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
            LogoRun run = new LogoRun();
            string sourceCode = SQLiteHelper.GetSqlHelper.GetSourceCode();
            List<Command> com = run.Run(sourceCode);
            for (int i = 0; i < com.Count; i++)
            {
                Draw(com[i]);
                await Task.Factory.StartNew(() => Wait());
            }
            runButton.IsEnabled = true;
        }

        private void Draw(Command com)
        {
            switch (com.word)
            {
                case "előre": turtle.Forward((int)com.param_value); break;
                case "hátra": turtle.Backward((int)com.param_value); break;
                case "jobbra": turtle.Right((int)com.param_value); break;
                case "balra": turtle.Left((int)com.param_value); break;
                case "haza": turtle.Home(); break;
                case "xpoz": turtle.XPos(); break;
                case "ypoz": turtle.YPos(); break;
                case "törölkép": turtle.Clean(); break;
                case "tollatle": turtle.PenDown(); break;
                case "tollatfel": turtle.PenUp(); break;
            }
        }

        private void Wait()
        {
            Thread.Sleep(1000);
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
