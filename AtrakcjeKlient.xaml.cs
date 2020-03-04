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
    /// Logika interakcji dla klasy AtrakcjeKlient.xaml
    /// </summary>
    public partial class AtrakcjeKlient : Window
    {
        public bool czyjestjuz;
        #region Przygotowanie
       
        public int id_karnetu;
        public SqlConnection conn = new SqlConnection();
        public SqlDataAdapter adapterAtrakcje;
        public DataTable dtAtrakcje;
        public int editedRowId;



        #endregion

        #region Walidacja Danych
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion
        public AtrakcjeKlient(SqlConnection con)
        {
          
            this.conn.ConnectionString = con.ConnectionString;
            InitializeComponent();
        }

        #region ładowanie danych
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


            adapterAtrakcje = new SqlDataAdapter("JakieAtrakcje", conn);
            adapterAtrakcje.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapterAtrakcje.SelectCommand.Parameters.Add("@ID_Klienta", SqlDbType.Int).Value = txtWyszukaj.Text;


            adapterAtrakcje.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapterAtrakcje);
            adapterAtrakcje.InsertCommand = builder.GetInsertCommand();
            adapterAtrakcje.UpdateCommand = builder.GetUpdateCommand();
            adapterAtrakcje.DeleteCommand = builder.GetDeleteCommand();
        }
        #endregion
        #region Zdarzenia 
        private void btnZatwierdz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtWyszukaj.Text == "")
                {
                    MessageBox.Show("NAPISZ NUMER KLIENTA W POLU", "WUAGA!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    this.conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = this.conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "CzyKlientIstnieje";

                    SqlParameter ID_Klienta = new SqlParameter();
                    ID_Klienta.ParameterName = "@ID_Klienta";
                    ID_Klienta.SqlDbType = SqlDbType.Int;
                    ID_Klienta.Direction = ParameterDirection.Input;
                    ID_Klienta.Value = txtWyszukaj.Text;
                    cmd.Parameters.Add(ID_Klienta);
                    SqlParameter parm = new SqlParameter("@result", SqlDbType.Int);

                    parm.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(parm);
                    cmd.ExecuteNonQuery();
                    this.conn.Close();
                    int retval = (int)parm.Value;

                    if (retval == -1)
                    {
                        string messageBoxText = "Klienta nie ma w bazie danych! \n Czy chcesz dodać go do bazy danych?";
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
                                        ZarzadzajKlientami wnd = new ZarzadzajKlientami(this.conn);
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
                                    MessageBox.Show("Wpisz inny numer, jesli klient zarzeka sie ze jest w bazie danych", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (czyjestjuz)
                        {
                            lstAtrakcje.ItemsSource = null;
                            ZaladujDane(this.conn);
                        }
                        else
                        {
                            czyjestjuz = true;
                            lstAtrakcje.IsEnabled = true;
                            ZaladujDane(this.conn);
                        }
                    }
                }
            
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
