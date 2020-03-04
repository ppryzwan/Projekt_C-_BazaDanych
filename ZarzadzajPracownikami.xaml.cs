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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace AplikacjaBest
{
    /// <summary>
    /// Logika interakcji dla klasy ZarzadzajPracownikami.xaml
    /// </summary>
    public partial class ZarzadzajPracownikami : Window
    {
        int editedRowId;
        int id_atrakcji;
        public bool trenerzy;

        #region Przygotowanie

        public SqlConnection conn = new SqlConnection();
        public SqlDataAdapter adapterPracownicy;
        public DataTable dtPracownicy;
        #endregion

        public ZarzadzajPracownikami(SqlConnection conn)
        {
            try {
               
                this.conn.ConnectionString = conn.ConnectionString;
                InitializeComponent();
                this.conn.Open();
                ZaladujDane(conn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ZarzadzajPracownikami(int id_atrakcji, SqlConnection conn)
        {
            try {
               
                this.conn.ConnectionString = conn.ConnectionString;
                InitializeComponent();
                this.conn.Open();
                ZaladujDaneTrenerow(conn);
                this.id_atrakcji = id_atrakcji;

                btnDodaj.Visibility = System.Windows.Visibility.Hidden;
                btnCertyfikat.Visibility = System.Windows.Visibility.Hidden;
                btnEdytuj.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region Ładowanie Danych
        public void ZaladujDane(SqlConnection conn)
        {
            try
            {
                InicjalizujadapterPracownicy(conn);
                dtPracownicy = new DataTable();
                adapterPracownicy.Fill(dtPracownicy);

                lstPracownicy.Items.Clear();
                lstPracownicy.ItemsSource = dtPracownicy.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void InicjalizujadapterPracownicy(SqlConnection conn)
        {


            adapterPracownicy = new SqlDataAdapter();
            adapterPracownicy.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            SqlCommand cmd = new SqlCommand("SELECT * FROM Pracownicy", conn);
            adapterPracownicy.SelectCommand = cmd;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapterPracownicy);
            adapterPracownicy.InsertCommand = builder.GetInsertCommand();
            adapterPracownicy.UpdateCommand = builder.GetUpdateCommand();
            adapterPracownicy.DeleteCommand = builder.GetDeleteCommand();
        }

        public void ZaladujDaneTrenerow(SqlConnection conn)
        {
            try
            {
                InicjalizujadapterTrenerzy(conn);
                dtPracownicy = new DataTable();
                adapterPracownicy.Fill(dtPracownicy);

                lstPracownicy.Items.Clear();
                lstPracownicy.ItemsSource = dtPracownicy.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void InicjalizujadapterTrenerzy(SqlConnection conn)
        {


            adapterPracownicy = new SqlDataAdapter();
            adapterPracownicy.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            SqlCommand cmd = new SqlCommand("SELECT * FROM Pracownicy WHERE Rola like 'Trener'", conn);
            adapterPracownicy.SelectCommand = cmd;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapterPracownicy);
            adapterPracownicy.InsertCommand = builder.GetInsertCommand();
            adapterPracownicy.UpdateCommand = builder.GetUpdateCommand();
            adapterPracownicy.DeleteCommand = builder.GetDeleteCommand();
        }

        #endregion

       
        #region zamykanie okna
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {
                this.conn.Close();
            }

            this.conn.Dispose();
        }

        private void btnAnuluj_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
            {
                this.conn.Close();
            }

            this.conn.Dispose();
        }
        #endregion

        #region Zdarzenia
        private void btnPrzypisz_Click(object sender, RoutedEventArgs e)
        {


            if (lstPracownicy.SelectedItems.Count != 0)
            {
                DataRowView row = this.lstPracownicy.SelectedItem as DataRowView;
                this.editedRowId = (int)row["ID_Pracownika"];

                if ((string)row["Rola"] == "Trener")
                {
                    if (this.id_atrakcji == 0)
                    {
                        ZarzadzajAtrakcjami wnd = new ZarzadzajAtrakcjami(this.editedRowId, this.conn );

                        wnd.Show();
                        this.Close();
                    }
                    else
                    {
                        //Przypisanie Atrakcji
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PrzypiszTrenerAtrakcja";

                        SqlParameter ID_Trenera = new SqlParameter();
                        ID_Trenera.ParameterName = "@ID_Trenera";
                        ID_Trenera.SqlDbType = SqlDbType.Int;
                        ID_Trenera.Direction = ParameterDirection.Input;
                        ID_Trenera.Value = this.editedRowId;
                        cmd.Parameters.Add(ID_Trenera);

                        SqlParameter ID_Atrakcji = new SqlParameter();
                        ID_Atrakcji.ParameterName = "@ID_Atrakcji";
                        ID_Atrakcji.SqlDbType = SqlDbType.Int;
                        ID_Atrakcji.Direction = ParameterDirection.Input;
                        ID_Atrakcji.Value = this.id_atrakcji;
                        cmd.Parameters.Add(ID_Atrakcji);
                        SqlParameter parm = new SqlParameter("@result", SqlDbType.Int);

                        parm.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(parm);

                        cmd.ExecuteNonQuery();
                        int retval = (int)parm.Value;

                        if (retval == 0)
                        {
                            MessageBox.Show("Trener jest juz przypisany do tej atrakcji!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Pomyślnie przypisano!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("ABY PRZYPISAĆ ATRAKCJE MUSISZ MIEC ZAZNACZONEGO TRENERA", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("ABY PRZYPISAĆ ATRAKCJE MUSISZ MIEC ZAZNACZONEGO TRENERA", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
     
        }

        private void btnAnuluj_Click(object sender, RoutedEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
            {
                this.conn.Close();
            }

            this.conn.Dispose();
            this.Close();
        }

        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
            {
                DodajPracownika wnd = new DodajPracownika(this, false);
                wnd.Owner = this;
                wnd.Show();
            }
            else
            {
                MessageBox.Show("Aby dodać nowego pracownika musisz mieć połączenie z bazą danych.", "Brak połączenia!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnEdytuj_Click(object sender, RoutedEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
            {
                if (lstPracownicy.SelectedItems.Count != 0)
                {
                    DodajPracownika wnd = new DodajPracownika(this,true);
                    wnd.Owner = this;
                    wnd.Show();
                }
                else
                {
                    MessageBox.Show("WYBIERZ PRACOWNIKA", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Aby edytować pracownika musisz mieć połączenie z bazą danych.", "Brak połączenia!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

      
      

        private void btnCertyfikat_Click(object sender, RoutedEventArgs e)
        {
            if (lstPracownicy.SelectedItems.Count != 0)
            {
                DataRowView row = this.lstPracownicy.SelectedItem as DataRowView;
                this.editedRowId = (int)row["ID_Pracownika"];
                if ((string)row["Rola"] == "Trener")
                {
                    DodajCertyfikat wnd = new DodajCertyfikat(this.editedRowId,this.conn);
                    wnd.Show();
                }
            }
            else
            {
                MessageBox.Show("ABY PRZYPISAĆ CERTIFKAT  MUSISZ MIEC ZAZNACZONEGO TRENERA", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void btnWyswietlTrenerow_Click(object sender, RoutedEventArgs e)
        {
            if (trenerzy)
            {
                lstPracownicy.ItemsSource = null;
                SqlCommand cmd = new SqlCommand("SELECT * FROM Pracownicy", conn);
                adapterPracownicy.SelectCommand = cmd;

                SqlCommandBuilder builder = new SqlCommandBuilder(adapterPracownicy);
                adapterPracownicy.InsertCommand = builder.GetInsertCommand();
                adapterPracownicy.UpdateCommand = builder.GetUpdateCommand();
                adapterPracownicy.DeleteCommand = builder.GetDeleteCommand();
                dtPracownicy = new DataTable();
                adapterPracownicy.Fill(dtPracownicy);
                lstPracownicy.ItemsSource = dtPracownicy.DefaultView;
                this.trenerzy = false;
                this.btnWyswietlTrenerow.Content = "Wyswietl Trenerow";
            }
            else
            {


                lstPracownicy.ItemsSource = null;
                SqlCommand cmd = new SqlCommand("SELECT * FROM Pracownicy WHERE Rola like 'Trener'", conn);
                adapterPracownicy.SelectCommand = cmd;

                SqlCommandBuilder builder = new SqlCommandBuilder(adapterPracownicy);
                adapterPracownicy.InsertCommand = builder.GetInsertCommand();
                adapterPracownicy.UpdateCommand = builder.GetUpdateCommand();
                adapterPracownicy.DeleteCommand = builder.GetDeleteCommand();
                dtPracownicy = new DataTable();
                adapterPracownicy.Fill(dtPracownicy);
                lstPracownicy.ItemsSource = dtPracownicy.DefaultView;
                this.trenerzy = true;
                this.btnWyswietlTrenerow.Content = "Wyswietl Pracowników";
            }
        }
        #endregion
    }
}
