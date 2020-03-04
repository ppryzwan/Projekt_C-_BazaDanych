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
    /// Logika interakcji dla klasy ZarzadzajAtrakcjami.xaml
    /// </summary>
    public partial class ZarzadzajAtrakcjami : Window
    {
        int editedRowId;
        int id_trenera;

        #region Przygotowanie

        public SqlConnection conn = new SqlConnection();
        public SqlDataAdapter adapterAtrakcje;
        public DataTable dtAtrakcje;
        #endregion

        public ZarzadzajAtrakcjami(SqlConnection conn)
        {
            try
            {
                
                this.conn.ConnectionString = conn.ConnectionString;
                InitializeComponent();

                this.conn.Open();
                ZaladujDane(conn);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public ZarzadzajAtrakcjami(int ID_Trenera,SqlConnection conn)
        {
            try
            {
             
                this.conn.ConnectionString = conn.ConnectionString;
                this.id_trenera = ID_Trenera;
                InitializeComponent();
                this.conn.Open();
                ZaladujDane(conn);
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
                InicjalizujadapterAtrakcje(conn);
                dtAtrakcje = new DataTable();
                adapterAtrakcje.Fill(dtAtrakcje);

                lstAtrakcje.Items.Clear();
                lstAtrakcje.ItemsSource = dtAtrakcje.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void InicjalizujadapterAtrakcje(SqlConnection conn)
        {


            adapterAtrakcje = new SqlDataAdapter();
            adapterAtrakcje.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            SqlCommand cmd = new SqlCommand("SELECT * FROM Atrakcje", conn);
            adapterAtrakcje.SelectCommand = cmd;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapterAtrakcje);
            adapterAtrakcje.InsertCommand = builder.GetInsertCommand();
            adapterAtrakcje.UpdateCommand = builder.GetUpdateCommand();
            adapterAtrakcje.DeleteCommand = builder.GetDeleteCommand();
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
            if (lstAtrakcje.SelectedItems.Count != 0)
            {
                DataRowView row = this.lstAtrakcje.SelectedItem as DataRowView;
                this.editedRowId = (int)row["ID_Atrakcji"];

                if (this.id_trenera == 0)
                {
                    ZarzadzajPracownikami wnd = new ZarzadzajPracownikami(this.editedRowId,this.conn);
                    this.Close();
                    wnd.Show();

                }
                else
                {

                    //Przypisanie Atrakcji Trenerowi
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "PrzypiszTrenerAtrakcja";

                    SqlParameter ID_Trenera = new SqlParameter();
                    ID_Trenera.ParameterName = "@ID_Trenera";
                    ID_Trenera.SqlDbType = SqlDbType.Int;
                    ID_Trenera.Direction = ParameterDirection.Input;
                    ID_Trenera.Value = this.id_trenera;
                    cmd.Parameters.Add(ID_Trenera);

                    SqlParameter ID_Atrakcji = new SqlParameter();
                    ID_Atrakcji.ParameterName = "@ID_Atrakcji";
                    ID_Atrakcji.SqlDbType = SqlDbType.Int;
                    ID_Atrakcji.Direction = ParameterDirection.Input;
                    ID_Atrakcji.Value = this.editedRowId;
                    cmd.Parameters.Add(ID_Atrakcji);
                    SqlParameter parm = new SqlParameter("@result", SqlDbType.Int);

                    parm.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(parm);

                    cmd.ExecuteNonQuery();
                    int retval = (int)parm.Value;

                    if (retval == 0)
                    {
                        MessageBox.Show("Trener jest już przypisany do tej atrakcji!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Pomyślnie przypisano!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }

            else
            {
                MessageBox.Show("Aby przypisać atrakcje do trenera należy wybrać atrakcje", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                DodajAtrakcje wnd = new DodajAtrakcje(this, false);
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
                if (lstAtrakcje.SelectedItems.Count != 0)
                {
                    DodajAtrakcje wnd = new DodajAtrakcje(this, true);
                    wnd.Owner = this;
                    wnd.Show();
                }
                else
                {
                    MessageBox.Show("Musisz zaznaczyć jaką atrakcję chcesz edytować.", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Wystąpił Błąd Połączenia. Zadzwoń do Administratora", "Brak połączenia!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnUsun_Click(object sender, RoutedEventArgs e)
        {
            if (lstAtrakcje.SelectedItems.Count != 0)
            {
                string messageBoxText = "Chcesz usunąć tą atrakcję?";
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
                                DataRowView row = lstAtrakcje.SelectedItem as DataRowView;
                                int rowId = (int)row["ID_Atrakcji"];

                                DataRow[] rows = dtAtrakcje.Select("ID_Atrakcji = " + rowId.ToString());
                                rows[0].Delete();

                                adapterAtrakcje.Update(dtAtrakcje);

                                ICollectionView view = CollectionViewSource.GetDefaultView(lstAtrakcje.ItemsSource);
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
                            MessageBox.Show("Postanowiłeś nie usuwać tej atrakcji", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        break;
   
                }
            }
            else
            {
                MessageBox.Show("Musisz zaznaczyć jaką atrakcję aby usunąć", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        #endregion
    }
}
