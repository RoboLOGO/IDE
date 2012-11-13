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
using Editor.Exeptions;
using InTheHand.Net.Sockets;
using InTheHand.Net;

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
        BluetoothHelper bluehelp;

        public MainWindow()
        {
            InitializeComponent();
            canvasSize = CanvasSize.GetCanvasSize;
            InitializeCanvas();
            sqlitehelper = SQLiteHelper.GetSqlHelper;
            InitBT();
            menu = Menu.GetMenu;
            rtbhelper = new RTextboxHelper();
            logoSynProvider = new CommonSyntaxProvider(LogoKeywords.GetLogoKeywords().GetKeywords, LogoKeywords.GetLogoKeywords().GetSpecialCharacters, false);
            synHighligt = new SyntaxHighlight(commandLine, logoSynProvider);
        }

        private void InitBT()
        {
            try 
            { 
                bluehelp = BluetoothHelper.GetBluetoothHelper();
            }
            catch
            {

            }
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
            if (bluehelp != null && bluehelp.IsConnected())
                runbtButton.IsEnabled = true;
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
            turtle.PenDown();
            RoboPreter rp = new RoboPreter();
            try
            {

                List<Robopreter.Command> com = rp.Run(menu.GetFullSource());
                for (int i = 0; i < com.Count; i++)
                {
                    Draw(com[i]);
                    int time = GetTime(com[i]);
                    await Task.Factory.StartNew(() => Wait(time));
                }
            }
            catch (RPExeption rpe)
            {
                MessageBox.Show(rpe.NewMessage);
            }
            catch
            {
                MessageBox.Show(App.Current.TryFindResource("error").ToString());
            }
            runButton.IsEnabled = true;

        }

        private static int GetTime(Robopreter.Command com)
        {
            int time = 800;
            if (com.comm == Commands.balra || com.comm == Commands.jobbra || com.comm == Commands.elore || com.comm == Commands.hatra)
                time = (int)com.comm * 10;
            return time;
        }

        private void Draw(Command com)
        {
            switch (com.comm)
            {
                case Commands.elore: turtle.Forward((int)com.value); break;
                case Commands.hatra: turtle.Backward((int)com.value); break;
                case Commands.jobbra: turtle.Right((int)com.value); break;
                case Commands.balra: turtle.Left((int)com.value); break;
                case Commands.haza: turtle.Home(); break;
                case Commands.torol: turtle.Clean(); break;
                case Commands.tollatle: turtle.PenDown(); break;
                case Commands.tollatfel: turtle.PenUp(); break;
            }
        }

        private void Wait(int time)
        {
            Thread.Sleep(time);
        }

        //kép mentés
        private void SaveImageClick(object sender, RoutedEventArgs e)
        {
            menu.Image_Save(canvas);
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

        private async void BluetoothUpdate_Click(object sender, RoutedEventArgs e)
        {
            BluetoothDeviceInfo[] devices = null;
            try
            {
                bluetoothList.IsEnabled = false;
                bluetoothupdate.IsEnabled = false;
                bluetoothconnect.IsEnabled = false;
                runbtButton.IsEnabled = false;
                if (bluehelp == null)
                    bluehelp = BluetoothHelper.GetBluetoothHelper();
                bluetoothList.IsEnabled = false;
                bluetoothupdate.IsEnabled = false;
                bluetoothconnect.IsEnabled = false;
                runbtButton.IsEnabled = false;
                await Task.Factory.StartNew(() => (devices = bluehelp.Search()));
                bluetoothList.ItemsSource = devices;
                bluetoothList.DisplayMemberPath = "DeviceName";
                bluetoothList.SelectedValuePath = "DeviceAddress";
                bluetoothList.SelectedValue = "";
                if (bluetoothList.Items.Count != 0)
                    bluetoothList.SelectedIndex = 0;
                bluetoothList.IsEnabled = true;
                bluetoothconnect.IsEnabled = true;
                if (sqlitehelper.FileSource != null && bluehelp.IsConnected())
                    runbtButton.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            bluetoothupdate.IsEnabled = true;
        }

        private void BluetoothConnect_Click(object sender, RoutedEventArgs e)
        {
            if (bluetoothList.SelectedItem != null)
            {
                try
                {
                    bluehelp.Connect((BluetoothDeviceInfo)bluetoothList.SelectedItem);
                    if (sqlitehelper.FileSource != null)
                        runbtButton.IsEnabled = true;
                    MessageBox.Show("Csatlakozva");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void runbtButton_Click(object sender, RoutedEventArgs e)
        {
            runbtButton.IsEnabled = false;
            menu.Save(rtbhelper.GetString(commandLine));
            turtle.Clean();
            RoboPreter rp = new RoboPreter();
            try
            {

                List<Robopreter.Command> com = rp.Run(menu.GetFullSource());
                await Task.Factory.StartNew(() => BTRun(com));
            }
            catch (RPExeption rpe)
            {
                MessageBox.Show(rpe.NewMessage);
            }
            catch
            {
                MessageBox.Show(App.Current.TryFindResource("error").ToString());
                //MessageBox.Show(ex.Message);
            }
            runbtButton.IsEnabled = true;
        }

        private void BTRun(List<Robopreter.Command> coms)
        {
            try
            {
                foreach (var x in coms)
                {
                    BTDo(x);
                    //if (x.value != null && (int)x.value < 30)
                    Thread.Sleep(250);
                    bluehelp.Read();
                }
            }
            catch
            {
                //if (!bluehelp.IsConnected())
                //    bluehelp.ReInit();
                MessageBox.Show(App.Current.TryFindResource("error").ToString());
            }
        }

        private void BTDo(Command com)
        {
            switch (com.comm)
            {
                case Commands.elore:
                    bluehelp.Send("1" + (int)com.value + "\n");
                    break;
                case Commands.hatra: 
                    bluehelp.Send("2" + (int)com.value + "\n");
                    break;
                case Commands.balra:
                    bluehelp.Send("3" + (int)com.value + "\n");
                    break;
                case Commands.jobbra:
                    bluehelp.Send("4" + (int)com.value + "\n");
                    break;                
                case Commands.tollatle:
                    bluehelp.Send("5");
                    break;
                case Commands.tollatfel:
                    bluehelp.Send("6");
                    break;
            }
        }
    }
}
