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
    /// Logika interakcji dla klasy DodajPracownika.xaml
    /// </summary>
    public partial class DodajPracownika : Window
    {
        ZarzadzajPracownikami Zarzadzaj;
        bool isEdit;
        int editedRowId;
        public DodajPracownika()
        {
            InitializeComponent();
        }


        public DodajPracownika(ZarzadzajPracownikami Zarzadzaj, bool IsEdit)
        {
            InitializeComponent();

            this.Zarzadzaj = Zarzadzaj;
            this.isEdit = IsEdit;

            if (isEdit)
            {
                this.Title = "Edytuj Dane Pracownika";
                btnOK.Content = "Aktualizuj";
                txtRola.Visibility = System.Windows.Visibility.Hidden;
                lblRola.Visibility = System.Windows.Visibility.Hidden;
                DataRowView row = Zarzadzaj.lstPracownicy.SelectedItem as DataRowView;
                Pracownik.DataContext = row;

                editedRowId = (int)row["ID_Pracownika"];
            }
            else
            {
                this.Title = "Dodaj Pracownika";
                btnOK.Content = "Dodaj";
            }
        }
        #region Walidacja danych
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
                    DataRow[] rows = Zarzadzaj.dtPracownicy.Select("ID_Pracownika = " + editedRowId.ToString());

                    rows[0]["Imie"] = txtImie.Text;
                    rows[0]["Nazwisko"] = txtNazwisko.Text;
                    rows[0]["Plec"] = txtPlec.Text;
                    rows[0]["Email"] = txtEmail.Text;
                    rows[0]["Numer_Telefonu"] = txtNumer_Telefonu.Text;
                    rows[0]["Login"] = txtLogin.Text;
                    rows[0]["Haslo"] = txtLogin.Text;
                    rows[0]["Uprawnienia_Systemowe"] = txtUprawnienia.Text;
                }
                else
                {
                    DataRow row = Zarzadzaj.dtPracownicy.NewRow();
                    row["Imie"] = txtImie.Text;
                    row["Nazwisko"] = txtNazwisko.Text;
                    row["Plec"] = txtPlec.Text;
                    row["Email"] = txtEmail.Text;
                    row["Numer_Telefonu"] = txtNumer_Telefonu.Text;
                    row["Login"] = txtLogin.Text;
                    row["Haslo"] = txtLogin.Text;
                    row["Uprawnienia_Systemowe"] = txtUprawnienia.Text;
                    row["Rola"] = txtRola.Text;
                    Zarzadzaj.dtPracownicy.Rows.Add(row);
                }


                Zarzadzaj.adapterPracownicy.Update(Zarzadzaj.dtPracownicy);
                Zarzadzaj.lstPracownicy.ItemsSource = null;
                Zarzadzaj.dtPracownicy.Clear();
                if (Zarzadzaj.trenerzy)
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Pracownicy WHERE Rola like 'Trener'", Zarzadzaj.conn);
                    Zarzadzaj.adapterPracownicy.SelectCommand = cmd;
                    Zarzadzaj.adapterPracownicy.Fill(Zarzadzaj.dtPracownicy);
                    Zarzadzaj.lstPracownicy.ItemsSource = Zarzadzaj.dtPracownicy.DefaultView;

                    this.Close();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Pracownicy", Zarzadzaj.conn);
                    Zarzadzaj.adapterPracownicy.SelectCommand = cmd;
                    Zarzadzaj.adapterPracownicy.Fill(Zarzadzaj.dtPracownicy);
                    Zarzadzaj.lstPracownicy.ItemsSource = Zarzadzaj.dtPracownicy.DefaultView;

                    this.Close();
                 
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        #endregion
    }
}
