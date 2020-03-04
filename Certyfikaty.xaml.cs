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
    /// Logika interakcji dla klasy Certyfikaty.xaml
    /// </summary>
    public partial class Certyfikaty : Window
    {
        public bool czyjestjuz;
        #region Przygotowanie
      
        public int id_karnetu;
        public SqlConnection conn = new SqlConnection();
        public SqlDataAdapter adapterCertyfikaty;
        public DataTable dtCertyfikaty;
        public int editedRowId;



        #endregion

        #region Walidacja danych
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        public Certyfikaty(SqlConnection conn)
        {
            try {
               
                this.conn.ConnectionString = conn.ConnectionString;
               
                InitializeComponent();
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

                InicjalizujadapterCeertyfikaty(conn);
                dtCertyfikaty = new DataTable();
                adapterCertyfikaty.Fill(dtCertyfikaty);

                lstCertyfikaty.Items.Clear();
                lstCertyfikaty.ItemsSource = dtCertyfikaty.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void InicjalizujadapterCeertyfikaty(SqlConnection conn)
        {


            adapterCertyfikaty = new SqlDataAdapter("JakieCertyfikaty", conn);
            adapterCertyfikaty.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapterCertyfikaty.SelectCommand.Parameters.Add("@ID_Pracownika", SqlDbType.Int).Value = txtWyszukaj.Text;


            adapterCertyfikaty.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            SqlCommandBuilder builder = new SqlCommandBuilder(adapterCertyfikaty);
            adapterCertyfikaty.InsertCommand = builder.GetInsertCommand();
            adapterCertyfikaty.UpdateCommand = builder.GetUpdateCommand();
            adapterCertyfikaty.DeleteCommand = builder.GetDeleteCommand();
        }
        #endregion

        #region Zdarzenia
        private void btnZatwierdz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtWyszukaj.Text == "")
                {
                    MessageBox.Show("NAPISZ NUMER TRENERA W POLU", "WUAGA!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {

                    this.conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = this.conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "CzyPracownikIstnieje";

                    SqlParameter ID_Pracownika = new SqlParameter();
                    ID_Pracownika.ParameterName = "@ID_Pracownika";
                    ID_Pracownika.SqlDbType = SqlDbType.Int;
                    ID_Pracownika.Direction = ParameterDirection.Input;
                    ID_Pracownika.Value = txtWyszukaj.Text;
                    cmd.Parameters.Add(ID_Pracownika);
                    SqlParameter parm = new SqlParameter("@result", SqlDbType.Int);

                    parm.Direction = ParameterDirection.Output;

                    SqlParameter parm1 = new SqlParameter("@dane",SqlDbType.NVarChar);
                    parm1.Size = 100;
               
                    parm1.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parm);
                    cmd.Parameters.Add(parm1);
                    cmd.ExecuteNonQuery();
                    this.conn.Close();
                    int retval = (int)parm.Value;
                   

                    if (retval == -1)
                    {
                        MessageBox.Show("TRENERA NIE MA W BAZIE DANYCH", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if(retval == 0)
                    {
                        MessageBox.Show("DANY PRACOWNIK NIE JEST TRENEREM", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else 
                    {
                        String dane = parm1.Value.ToString();
                        lblDaneTrenera.Content = dane;
                        if (czyjestjuz)
                        {
                            lstCertyfikaty.ItemsSource = null;
                            ZaladujDane(this.conn);
                        }
                        else
                        {
                            czyjestjuz = true;
                            lstCertyfikaty.IsEnabled = true;
                            ZaladujDane(this.conn);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}

