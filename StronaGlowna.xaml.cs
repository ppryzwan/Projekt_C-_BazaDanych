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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Media;

namespace AplikacjaBest
{
    /// <summary>
    /// Logika interakcji dla klasy StronaGlowna.xaml
    /// </summary>
    public partial class StronaGlowna : Window
    {
        public bool rozwiniete;
        public bool czybyljuzzalogowany;
        public StronaGlowna()
        {
            InitializeComponent();
        }
        public SqlConnection conn = new SqlConnection();

        public StronaGlowna(int uprawnienie,SqlConnection conn)
        {
            this.conn.ConnectionString = conn.ConnectionString;
            InitializeComponent();
            if(uprawnienie == 2)
            {
                btnZarzadzanieKarnetami.IsEnabled = true;
                btnZarzadzanieKlientami.IsEnabled = true;
                btnZarzadzanieAtrakcjami.IsEnabled = true;
                Kontrolki.IsEnabled = true;

            }
            else if(uprawnienie == 3)
            {
                btnZarzadzanieKarnetami.IsEnabled = true;
                btnZarzadzaniePracownikami.IsEnabled = true;
                btnZarzadzanieKlientami.IsEnabled = true;
                btnZarzadzanieAtrakcjami.IsEnabled = true;
                Kontrolki.IsEnabled = true;
            }
        }
        #region Zdarzenia
  

        private void ZarzadzajKlientami(object sender, RoutedEventArgs e)
        {
            ZarzadzajKlientami dodaj1 = new ZarzadzajKlientami(this.conn);
            dodaj1.Show();
        }


        private void ZarzadzajKarnetami(object sender, RoutedEventArgs e)
        {
            ZarzadzajKarnetami dodaj1 = new ZarzadzajKarnetami(this.conn);
            dodaj1.Show();
        }
        private void ZarzadzajAtrakcjami(object sender, RoutedEventArgs e)
        {
            ZarzadzajAtrakcjami dodaj1 = new ZarzadzajAtrakcjami(this.conn);
            dodaj1.Show();
        }

        private void ZarzadzajPracownikami(object sender, RoutedEventArgs e)
        {
            ZarzadzajPracownikami dodaj1 = new ZarzadzajPracownikami(this.conn);
            dodaj1.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SprawdzanieKarnetu wnd = new SprawdzanieKarnetu(this.conn);
            wnd.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AtrakcjeKlient wnd = new AtrakcjeKlient(this.conn);
            wnd.Show();
        }
        private void Historie(object sender, RoutedEventArgs e)
        {
            HistoriaKarnetowKlient wnd = new HistoriaKarnetowKlient(this.conn);
            wnd.Show();
        }

        private void btnRozwin_Click(object sender, RoutedEventArgs e)
        {
            if (rozwiniete == false)
            {
                Kontrolki.Visibility = System.Windows.Visibility.Visible;
                rozwiniete = true;
                BitmapImage image = new BitmapImage(new Uri("/Resources/img3.png", UriKind.Relative));
                button.Source = image;
                this.MaxHeight = 450;
                this.Height = 450;
            }
            else
            {
   
                Kontrolki.Visibility = System.Windows.Visibility.Hidden;
                rozwiniete = false;
                BitmapImage image = new BitmapImage(new Uri("/Resources/img2.png", UriKind.Relative));
                button.Source = image;
                this.MaxHeight = 250;
                this.Height = 250;
            }
        }

        private void Certyfikaty(object sender, RoutedEventArgs e)
        {
            Certyfikaty wnd = new Certyfikaty(this.conn);
            wnd.Show();
        }
        #endregion

        private void btnwyloguj_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Czy Chcesz się wylogować?";
            string caption = "UWAGA!";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        try
                        {
                            MainWindow wnd = new MainWindow();
                            wnd.Show();
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    {
                        //do nothing
                    }
                    break;
            }
        }
    }
}

