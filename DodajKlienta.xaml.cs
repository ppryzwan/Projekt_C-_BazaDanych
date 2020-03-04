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
namespace AplikacjaBest
{
    /// <summary>
    /// Logika interakcji dla klasy DodajKlienta.xaml
    /// </summary>
    public partial class DodajKlienta : Window
    {

        ZarzadzajKlientami Zarzadzaj;
        bool isEdit;
        int editedRowId;
        public DodajKlienta()
        {
            InitializeComponent();
        }


        public DodajKlienta(ZarzadzajKlientami Zarzadzaj  , bool IsEdit)
        {
            InitializeComponent();

            this.Zarzadzaj = Zarzadzaj;
            this.isEdit = IsEdit;

            if (isEdit)
            {
                this.Title = "Edytuj";
                btnOK.Content = "Aktualizuj";

                DataRowView row = Zarzadzaj.lstKlienci.SelectedItem as DataRowView;
                Klient.DataContext = row;

                editedRowId = (int)row["ID_Klienta"];
            }
            else
            {
                this.Title = "Dodaj";
                btnOK.Content = "Dodaj";
            }
        }
        #region Walidacja Danych
        private void NumberValidationTextBox1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^\d{1,3}[a-z]?$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^\d{1,3}$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));

        }
        private void NumberValidationTextBox2(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^\d{1,12}$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
        #endregion
        #region Zdarzenia
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
     
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (isEdit)
                {
                    // edycja
                    DataRow[] rows = Zarzadzaj.dtKlienci.Select("ID_Klienta = " + editedRowId.ToString());

                    rows[0]["Imie"] = txtImie.Text;
                    rows[0]["Nazwisko"] = txtNazwisko.Text;
                    rows[0]["Plec"] = txtPlec.Text;
                    rows[0]["Miasto"] = txtMiasto.Text;
                    rows[0]["Ulica"] = txtUlica.Text;
                    rows[0]["Numer_Lokalu"] = txtNumerLokalu.Text;
                    rows[0]["Numer_Mieszkania"] = txtNumerMieszkania.Text;
                    rows[0]["Email"] = txtEmail.Text;
                    rows[0]["Numer_Telefonu"] = txtNumerTelefonu.Text;
                }
                else
                {
                    DataRow row = Zarzadzaj.dtKlienci.NewRow();
                    row["Imie"] = txtImie.Text;
                    row["Nazwisko"] = txtNazwisko.Text;
                    row["Plec"] = txtPlec.Text;
                    row["Miasto"] = txtMiasto.Text;
                    row["Ulica"] = txtUlica.Text;
                    row["Numer_Lokalu"] = txtNumerLokalu.Text;
                    row["Numer_Mieszkania"] = txtNumerMieszkania.Text;
                    row["Email"] = txtEmail.Text;
                    row["Numer_Telefonu"] = txtNumerTelefonu.Text;
                    Zarzadzaj.dtKlienci.Rows.Add(row);
                }

         
                Zarzadzaj.adapterKlienci.Update(Zarzadzaj.dtKlienci);
                Zarzadzaj.lstKlienci.ItemsSource = null;
                Zarzadzaj.dtKlienci.Clear();
                Zarzadzaj.adapterKlienci.Fill(Zarzadzaj.dtKlienci);
                Zarzadzaj.lstKlienci.ItemsSource = Zarzadzaj.dtKlienci.DefaultView;
                
        
                this.Close();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        #endregion
    }
}
