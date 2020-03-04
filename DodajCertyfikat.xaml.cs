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
    /// Logika interakcji dla klasy DodajCertyfikat.xaml
    /// </summary>
    public partial class DodajCertyfikat : Window
    {
        public int id_trenera;
        #region Przygotowanie

        public SqlConnection conn = new SqlConnection();
        #endregion
        public DodajCertyfikat(SqlConnection conn)
        {
            try
            {
               
                this.conn.ConnectionString = conn.ConnectionString;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public DodajCertyfikat(int id_trenera,SqlConnection conn)
        {
            try
            {
               
                this.conn.ConnectionString = conn.ConnectionString;
                InitializeComponent();

                this.id_trenera = id_trenera;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd pobierania danych!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Zdarzenia
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (txtNazwa.Text == null)
                {
                    MessageBox.Show("Zaznacz certyfikat!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    this.conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = this.conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DodajCertyfikatDoTrenera";

                    SqlParameter ID_Trenera = new SqlParameter();
                    ID_Trenera.ParameterName = "@ID_Trenera";
                    ID_Trenera.SqlDbType = SqlDbType.Int;
                    ID_Trenera.Direction = ParameterDirection.Input;
                    ID_Trenera.Value = this.id_trenera;
                    cmd.Parameters.Add(ID_Trenera);

                    SqlParameter Nazwa = new SqlParameter();
                    Nazwa.ParameterName = "@Nazwa";
                    Nazwa.SqlDbType = SqlDbType.NVarChar;
                    Nazwa.Direction = ParameterDirection.Input;
                    Nazwa.Value = txtNazwa.Text;
                    cmd.Parameters.Add(Nazwa);
                    SqlParameter parm = new SqlParameter("@result", SqlDbType.Int);

                    parm.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(parm);

                    cmd.ExecuteNonQuery();
                    this.conn.Close();
                    int retval = (int)parm.Value;

                    if (retval == 0)
                    {
                        MessageBox.Show("Trener ma ten certyfikat!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Pomyślnie przypisano!", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
