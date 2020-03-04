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
    /// Logika interakcji dla klasy DodajKarnet.xaml
    /// </summary>
    public partial class DodajKarnet : Window
    {
     
        bool isEdit;
        ZarzadzajKarnetami Zarzadzaj;
        int editedRowId;
        public DodajKarnet()
        {
            InitializeComponent();
        }
        #region Walidacja Danych
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^\d+\.?\d{0,2}$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
        #endregion
        public DodajKarnet(ZarzadzajKarnetami Zarzadzaj , bool IsEdit)
        {
            InitializeComponent();

            this.Zarzadzaj = Zarzadzaj;
            this.isEdit = IsEdit;

            if (isEdit)
            {
                this.Title = "Edytuj";
                btnOK.Content = "Aktualizuj";

                DataRowView row = Zarzadzaj.lstKarnety.SelectedItem as DataRowView;
                Karnet.DataContext = row;

                editedRowId = (int)row["ID_Karnetu"];
            }
            else
            {
                this.Title = "Dodaj";
                btnOK.Content = "Dodaj";
                
            }
        }

        #region Zdarzenia
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (isEdit)
                {
                    // edycja
                    DataRow[] rows = Zarzadzaj.dtKarnety.Select("ID_Karnetu = " + editedRowId.ToString());

                    rows[0]["Nazwa"] = txtNazwa.Text;
                    rows[0]["Cena"] = Decimal.Parse(txtCena.Text.Replace('.',','));
                    rows[0]["Uprawnienia"] = txtUprawnienia.Text;
                    rows[0]["Ilosc_Miesiecy"] = txtIlosc_Miesiecy.Text;
                    rows[0]["Opis"] = txtOpis.Text;

                }
                else
                {
                    DataRow row = Zarzadzaj.dtKarnety.NewRow();
                    row["Nazwa"] = txtNazwa.Text;
                    row["Cena"] = Decimal.Parse(txtCena.Text.Replace('.', ','));
                    row["Uprawnienia"] = txtUprawnienia.Text;
                    row["Ilosc_Miesiecy"] = txtIlosc_Miesiecy.Text;
                    row["Opis"] = txtOpis.Text;
                    Zarzadzaj.dtKarnety.Rows.Add(row);
                }

                Zarzadzaj.adapterKarnet.Update(Zarzadzaj.dtKarnety);
                Zarzadzaj.lstKarnety.ItemsSource = null;
                Zarzadzaj.dtKarnety.Clear();
                Zarzadzaj.adapterKarnet.Fill(Zarzadzaj.dtKarnety);
                Zarzadzaj.lstKarnety.ItemsSource = Zarzadzaj.dtKarnety.DefaultView;


                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
