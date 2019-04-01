using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EF.Model;
using EF.Pages;
using EF.ViewModel;
using System.Collections.ObjectModel;
using System.Net.Mail;

//using System.Data.SqlClient;
namespace EF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginPage logPage;
        private MainPage mainPage;
        private TopSerials topPage;
        private MySerials mySerialsPage;
        private RegPage regPage;
        private SerialInfoPage infoPage;
        private MyShedule shedulePage;
        AppManager app; 
        public MainWindow()
        {
            InitializeComponent();
            app = new AppManager();
            logPage = new LoginPage();
            mainPage = new MainPage();
            topPage = new TopSerials();
            mySerialsPage = new MySerials();
            regPage = new RegPage();
            infoPage = new SerialInfoPage();
            shedulePage = new MyShedule();


            AddDoubleClickEventStyle(mainPage.dGSerials, new MouseButtonEventHandler(dGSerials_MouseDoubleClick));

            topPage.dGSerials.MouseDoubleClick += dGSerials_MouseDoubleClick;
            mySerialsPage.ViewedSerials.MouseDoubleClick += dGSerials_MouseDoubleClick;
            mySerialsPage.WatchingSerials.MouseDoubleClick += dGSerials_MouseDoubleClick;


            MainFrame.Navigate(logPage);
            app.LoggedHandler += app_LoggedHandler;
            MainFrame.Navigating += MainFrame_Navigating;

            logPage.btnConnect.Click += btnConnect_Click;
            logPage.btnReg.Click += btnReg_Click;

            regPage.btnRegister.Click += btnRegister_Click;
            regPage.btnCancel.Click += btnCancel_Click;

            mainPage.cbStatus.SelectionChanged += SerialFilterChanges;
            mainPage.cbGenres.SelectionChanged += SerialFilterChanges;
  
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(logPage);
        }

        void SerialFilterChanges(object sender, SelectionChangedEventArgs e)
        {
            var serials = AppManager.GetSerials();
            var genre = mainPage.cbGenres.SelectedItem as Genre;
            if (genre!=null && genre.Name != "Any")
            {
                serials = serials.FindAll(s => s.Genres.Contains(genre)).Select(s => s).ToList();
            }

            var status = mainPage.cbStatus.SelectedItem as Status;
            if(status!=null &&status.Name !="Any")
            {
                serials = serials.FindAll(s => s.Status == status.Name).Select(s => s).ToList();
            }
            mainPage.dGSerials.ItemsSource = serials.OrderBy(s=>s.Name);
        }
        void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Content is RegPage)
            {
                (e.Content as RegPage).ResetContent();

            }
            if (e.Content is LoginPage)
            {
                (e.Content as LoginPage).ResetContent();

            }
            if (e.Content is MainPage)
            {   
                (e.Content as MainPage).cbGenres.ItemsSource = AppManager.GetGenres();
                (e.Content as MainPage).cbGenres.SelectedIndex = 0;

                (e.Content as MainPage).cbStatus.ItemsSource = AppManager.GetStatus();
                (e.Content as MainPage).cbStatus.SelectedIndex = 0; 
            }
            if (e.Content is MyShedule)
            {
                var Series = AppManager.GetUserShedule();
                ListCollectionView Customers = new ListCollectionView(Series);
                Customers.GroupDescriptions.Add(new PropertyGroupDescription("Serial_Name"));
                (e.Content as MyShedule).Shedule.ItemsSource = Customers;

            }
            if (e.Content is TopSerials)
            {

                (e.Content as TopSerials).dGSerials.ItemsSource = AppManager.GetTop(50);
            }
            if (e.Content is MySerials)
            {
                (e.Content as MySerials).WatchingSerials.ItemsSource = AppManager.GetUserSerials();
                (e.Content as MySerials).ViewedSerials.ItemsSource = AppManager.GetUserViewedSerials();
            }
            if (e.Content is SerialInfoPage)
            {
                try
                {
                    (e.Content as SerialInfoPage).StackSerial.DataContext = AppManager.SelectedSerial;
                    (e.Content as SerialInfoPage).Img.DataContext = AppManager.SelectedSerial;
                    SetRadioGroupBindings(AppManager.SelectedSerial, e);
                    var Series = AppManager.GetViewModelSeries();
                    (e.Content as SerialInfoPage).lbGenres.ItemsSource = AppManager.SelectedSerial.Genres;
                    ListCollectionView Customers = new ListCollectionView(Series);
                    Customers.GroupDescriptions.Add(new PropertyGroupDescription("Season"));
                    (e.Content as SerialInfoPage).Series.ItemsSource = Customers;
                }
               catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

  

        void SetRadioGroupBindings(SerialView Serial, NavigatingCancelEventArgs e)
        {
            BindingOperations.ClearAllBindings((e.Content as SerialInfoPage).rbStopWatching);
            BindingOperations.ClearAllBindings((e.Content as SerialInfoPage).rbWatched);
            BindingOperations.ClearAllBindings((e.Content as SerialInfoPage).rbWatching);

            Binding bind = new Binding();
            bind.Source = Serial;
            bind.Path = new PropertyPath("Watching");
            bind.Mode = BindingMode.TwoWay;
            (e.Content as SerialInfoPage).rbWatching.SetBinding(RadioButton.IsCheckedProperty, bind);

            Binding bind2 = new Binding();
            bind2.Source = Serial;
            bind2.Path = new PropertyPath("Watched");
            bind2.Mode = BindingMode.TwoWay;
            (e.Content as SerialInfoPage).rbWatched.SetBinding(RadioButton.IsCheckedProperty, bind2);

            Binding bind3 = new Binding();
            bind3.Source = Serial;
            bind3.Path = new PropertyPath("DontWatch");
            bind3.Mode = BindingMode.TwoWay;
            (e.Content as SerialInfoPage).rbStopWatching.SetBinding(RadioButton.IsCheckedProperty, bind3);
        }

        void app_LoggedHandler(object sender, EventArgs e)
        {
            if (app.Logged)
            {
                MainFrame.Navigate(mainPage);
                Menu.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                MainFrame.Navigate(logPage);
                Menu.Visibility = System.Windows.Visibility.Collapsed;
            }
               
        }

        void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                app.TestConnection(logPage.Login.Text, logPage.Password.Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message,"Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        void btnReg_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(regPage);
        }
        void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MailAddress mail = new MailAddress(regPage.Email.Text);
                if (String.IsNullOrEmpty(regPage.Login.Text) || String.IsNullOrEmpty(regPage.Password.Password) || String.IsNullOrEmpty(regPage.Email.Text))
                {
                    throw new Exception("You need to fill all fields");
                }
                AppManager.Register(regPage.Login.Text, regPage.Password.Password, regPage.Email.Text);
                MainFrame.Navigate(logPage);
            }
            catch(FormatException)
            {
                MessageBox.Show(this, "The email format is not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message ,"Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private void ButtonMainPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(mainPage);
        }

        private void ButtonMySerials_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(mySerialsPage);
        }

        private void ButtonShedule_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(shedulePage);
        }

        private void ButtonTop_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(topPage);
        }

        private void ButtonLogOut_Click(object sender, RoutedEventArgs e)
        {
            app.Logged = false;
            MainFrame.Navigate(logPage);
        }

        private void AddDoubleClickEventStyle(ListBox listBox, MouseButtonEventHandler mouseButtonEventHandler)
        {
            if (listBox.ItemContainerStyle == null)
                listBox.ItemContainerStyle = new Style(typeof(ListBoxItem));
            listBox.ItemContainerStyle.Setters.Add(new EventSetter()
            {
                Event = MouseDoubleClickEvent,
                Handler = mouseButtonEventHandler
            });
        }

        public void dGSerials_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if(sender is ListBoxItem)
            {
                  
                  AppManager.SetViewModelSerial(((sender as ListBoxItem).Content as SerialInfo).Id);
               
            }

            if (sender is DataGrid)
            {
                if ((sender as DataGrid).SelectedItem == null) return;
                var SelectedSerial = (sender as DataGrid).SelectedItem as SerialView;
                AppManager.SetViewModelSerial(SelectedSerial.Id);
            
            }
            MainFrame.Navigate(infoPage);
           
        }


    }
}
