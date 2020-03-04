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
    /// Logika interakcji dla klasy ZarzadzajKlientami.xaml
    /// </summary>
    public partial class ZarzadzajKlientami : Window
    {
        #region Przygotowanie

        public int id_karnetu;
        public SqlConnection conn = new SqlConnection();
        public SqlDataAdapter adapterKlienci;
        public DataTable dtKlienci;
        public int editedRowId;
      

        #endregion
        public ZarzadzajKlientami(int ID_Karnetu,SqlConnection conn)

        {
            try
            {
                
                this.conn.ConnectionString = conn.ConnectionString;
                this.id_karnetu = ID_Karnetu;
                InitializeComponent();
                this.conn.Open();
                ZaladujDane(conn);

                btnDodaj.Visibility = System.Windows.Visibility.Hidden;
                btnEdytuj.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ZarzadzajKlientami(SqlConnection conn)
        {
            try
            {
                
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
      
        #region ładowanie danych
        public void ZaladujDane(SqlConnection conn)
        {
            try
            {
                InicjalizujAdapterKlienci(conn);
                
                dtKlienci = new DataTable();
                adapterKlienci.Fill(dtKlienci);

                lstKlienci.Items.Clear();
                lstKlienci.ItemsSource = dtKlienci.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void InicjalizujAdapterKlienci(SqlConnection conn)
        {
          

            adapterKlienci = new SqlDataAdapter();
            adapterKlienci.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            SqlCommand cmd = new SqlCommand("SELECT * FROM KLienci", conn);
            adapterKlienci.SelectCommand = cmd;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapterKlienci);
            adapterKlienci.InsertCommand = builder.GetInsertCommand();
            adapterKlienci.UpdateCommand = builder.GetUpdateCommand();
            adapterKlienci.DeleteCommand = builder.GetDeleteCommand();
        }
        #endregion

        #region zdarzenia
        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
         
            if (this.conn.State == ConnectionState.Open)
            {
                DodajKlienta wnd = new DodajKlienta(this, false);
                wnd.Owner = this;
                wnd.Show();
            }
            else
            {
                MessageBox.Show("Wystąpił Błąd Połączenia. Zadzwoń do Administratora", "Brak połączenia!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnEdytuj_Click(object sender, RoutedEventArgs e)
        {

            if (this.conn.State == ConnectionState.Open)
            {
 

                if (lstKlienci.SelectedItems.Count != 0)
                {
                    DodajKlienta wnd = new DodajKlienta(this, true);
                    wnd.Owner = this;
                    wnd.Show();
                }
                else
                {
                    MessageBox.Show("Zaznacz klienta, którego chcesz edytować", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Wystąpił Błąd Połączenia. Zadzwoń do Administratora", "Brak połączenia!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


          
        }
        

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
            {
                this.conn.Close();
            }

            this.conn.Dispose();
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

        private void btnPrzypisz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstKlienci.SelectedItems.Count != 0)
                {
                    DataRowView row = this.lstKlienci.SelectedItem as DataRowView;
                    this.editedRowId = (int)row["ID_Klienta"];

                    if (this.id_karnetu == 0)
                    {
                        ZarzadzajKarnetami wnd = new ZarzadzajKarnetami(this.editedRowId, this.conn);
                        wnd.Show();
                        this.Close();
                    }
                    else
                    {

                        //Przypisanie Karnetu
                        this.conn.Close();
                        this.conn.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = this.conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PrzypiszKarnetKlient";

                        SqlParameter ID_Karnetu = new SqlParameter();
                        ID_Karnetu.ParameterName = "@ID_Karnetu";
                        ID_Karnetu.SqlDbType = SqlDbType.Int;
                        ID_Karnetu.Direction = ParameterDirection.Input;
                        ID_Karnetu.Value = this.id_karnetu;
                        cmd.Parameters.Add(ID_Karnetu);

                        SqlParameter ID_Klienta = new SqlParameter();
                        ID_Klienta.ParameterName = "@ID_Klienta";
                        ID_Klienta.SqlDbType = SqlDbType.Int;
                        ID_Klienta.Direction = ParameterDirection.Input;
                        ID_Klienta.Value = this.editedRowId;
                        cmd.Parameters.Add(ID_Klienta);
                        SqlParameter parm = new SqlParameter("@result", SqlDbType.Int);

                        parm.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(parm);

                        cmd.ExecuteNonQuery();
                        int retval = (int)parm.Value;

                        if (retval == 0)
                        {
                            MessageBox.Show("Osoba ma już taki karnet!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Pomyślnie przypisano!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Aby przypisać karnet do klienta nalezy wybrać klienta ", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            #endregion
        }
    }
}
