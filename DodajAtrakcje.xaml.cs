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
    /// Logika interakcji dla klasy DodajAtrakcje.xaml
    /// </summary>
    public partial class DodajAtrakcje : Window
    {
        ZarzadzajAtrakcjami Zarzadzaj;
        bool isEdit;
        int editedRowId;
        public DodajAtrakcje()
        {
            InitializeComponent();
        }


        public DodajAtrakcje(ZarzadzajAtrakcjami Zarzadzaj, bool IsEdit)
        {
            InitializeComponent();

            this.Zarzadzaj = Zarzadzaj;
            this.isEdit = IsEdit;

            if (isEdit)
            {
                this.Title = "Edytuj Atrakcje";
                btnOK.Content = "Aktualizuj";

                DataRowView row = Zarzadzaj.lstAtrakcje.SelectedItem as DataRowView;
                Atrakcja.DataContext = row;

                editedRowId = (int)row["ID_Atrakcji"];
            }
            else
            {
                this.Title = "Dodaj Atrakcje";
                btnOK.Content = "Dodaj";
            }
        }

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
                    DataRow[] rows = Zarzadzaj.dtAtrakcje.Select("ID_Atrakcji = " + editedRowId.ToString());

                    rows[0]["Nazwa"] = txtNazwa.Text;
                    rows[0]["Uprawnienia"] = txtUprawnienie.Text;

                }
                else
                {
                    DataRow row = Zarzadzaj.dtAtrakcje.NewRow();
                    row["Nazwa"] = txtNazwa.Text;
                    row["Uprawnienia"] = txtUprawnienie.Text;
                    Zarzadzaj.dtAtrakcje.Rows.Add(row);
                }

                Zarzadzaj.adapterAtrakcje.Update(Zarzadzaj.dtAtrakcje);
                Zarzadzaj.lstAtrakcje.ItemsSource = null;
                Zarzadzaj.dtAtrakcje.Clear();
                Zarzadzaj.adapterAtrakcje.Fill(Zarzadzaj.dtAtrakcje);
                Zarzadzaj.lstAtrakcje.ItemsSource = Zarzadzaj.dtAtrakcje.DefaultView;


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
