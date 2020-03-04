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
    /// Logika interakcji dla klasy ZarzadzajKarnetami.xaml
    /// </summary>
    public partial class ZarzadzajKarnetami : Window
    {
        int editedRowId;
        int id_klienta;
        #region Przygotowanie


        public SqlConnection conn = new SqlConnection();
        #endregion
        public SqlDataAdapter adapterKarnet;
        public DataTable dtKarnety;

        public ZarzadzajKarnetami(SqlConnection conn)
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

        public ZarzadzajKarnetami(int id_klienta, SqlConnection conn)
        {
            try {
                
                this.conn.ConnectionString = conn.ConnectionString;
                InitializeComponent();
                this.conn.Open();
                ZaladujDane(conn);
                this.id_klienta = id_klienta;
                this.Title = "Przypisz Karnet";
                btnDodaj.Visibility = System.Windows.Visibility.Hidden;
                btnUsun.Visibility = System.Windows.Visibility.Hidden;
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
                InicjalizujadapterKarnet(conn);
                dtKarnety = new DataTable();
                adapterKarnet.Fill(dtKarnety);

                lstKarnety.Items.Clear();
                lstKarnety.ItemsSource = dtKarnety.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void InicjalizujadapterKarnet(SqlConnection conn)
        {


            adapterKarnet = new SqlDataAdapter();
            adapterKarnet.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            SqlCommand cmd = new SqlCommand("SELECT * FROM Karnety", conn);
            adapterKarnet.SelectCommand = cmd;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapterKarnet);
            adapterKarnet.InsertCommand = builder.GetInsertCommand();
            adapterKarnet.UpdateCommand = builder.GetUpdateCommand();
            adapterKarnet.DeleteCommand = builder.GetDeleteCommand();
        }
        #endregion

        #region zamykanie okna
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
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


        #region zdarzenia
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (lstKarnety.SelectedItems.Count != 0)
            {
                DataRowView row = this.lstKarnety.SelectedItem as DataRowView;
                this.editedRowId = (int)row["ID_Karnetu"];

                if (this.id_klienta == 0)
                {
            
                    ZarzadzajKlientami wnd = new ZarzadzajKlientami(this.editedRowId, this.conn);
                    wnd.Show();
                    this.Close();
                }
                else
                {

                    //Przypisanie Karnetu
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "PrzypiszKarnetKlient";

                    SqlParameter ID_Karnetu = new SqlParameter();
                    ID_Karnetu.ParameterName = "@ID_Karnetu";
                    ID_Karnetu.SqlDbType = SqlDbType.Int;
                    ID_Karnetu.Direction = ParameterDirection.Input;
                    ID_Karnetu.Value = this.editedRowId;
                    cmd.Parameters.Add(ID_Karnetu);

                    SqlParameter ID_Klienta = new SqlParameter();
                    ID_Klienta.ParameterName = "@ID_Klienta";
                    ID_Klienta.SqlDbType = SqlDbType.Int;
                    ID_Klienta.Direction = ParameterDirection.Input;
                    ID_Klienta.Value = this.id_klienta;
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
                MessageBox.Show("Aby przypisać karnet do klienta nalezy wybrać karnet ", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnAnuluj_Click(object sender, RoutedEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Dispose();
            this.Close();
        }

        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (this.conn.State == ConnectionState.Open)
            {
                DodajKarnet wnd = new DodajKarnet(this,false);
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
                if (lstKarnety.SelectedItems.Count != 0)
                {
                    DodajKarnet wnd = new DodajKarnet(this, true);
                    wnd.Owner = this;
                    wnd.Show();
                }
                else
                {
                    MessageBox.Show("WYBIERZ KARNET.", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Wystąpił Błąd Połączenia. Zadzwoń do Administratora", "Brak połączenia!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnUsun_Click(object sender, RoutedEventArgs e)
        {
            if (lstKarnety.SelectedItems.Count != 0)
            {
                string messageBoxText = "Chcesz usunąć ten karnet?";
                string caption = "UWAGA! UWAGA!";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            try
                            {
                                DataRowView row = lstKarnety.SelectedItem as DataRowView;
                                int rowId = (int)row["ID_Karnetu"];

                                DataRow[] rows = dtKarnety.Select("ID_Karnetu = " + rowId.ToString());
                                rows[0].Delete();

                                adapterKarnet.Update(dtKarnety);

                                ICollectionView view = CollectionViewSource.GetDefaultView(lstKarnety.ItemsSource);
                                view.Refresh();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                        break;
                    case MessageBoxResult.No:
                        {
                            MessageBox.Show("Postanowiłeś nie usuwać tego karnetu ", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        break;

                }
            }
            else
            {
                MessageBox.Show("Musisz zaznaczyć jakiś karnet aby usunąć", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        #endregion

    }
}
