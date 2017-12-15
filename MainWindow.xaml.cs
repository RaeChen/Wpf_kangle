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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;


namespace Wpf_kangle
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //数据库连接的变量
        private string conn;
        private MySqlConnection connect;

        public string form1user;

        private void db_connection()
        {
            try
            {
                conn = "server=localhost;user=root;database=kangle;port=3306;password=rebirth610;";
                connect = new MySqlConnection(conn);
                connect.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("数据库连接异常" + e.ToString());
            }
        }

        //验证登录-连数据库
        private bool validate_login(string user, string pass)
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select * from yonghu where yonghu_gonghao=@user and mima=@pass";
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pass", pass);
            cmd.Connection = connect;
            MySqlDataReader login = cmd.ExecuteReader();
            if (login.Read())
            {
                connect.Close();
                return true;
            }
            else
            {
                connect.Close();
                return false;
            }
        }


        private void denglu_Click(object sender, RoutedEventArgs e)
        {
            string user = name.Text;
            string pass = password.Text;
            if (!String.IsNullOrEmpty(name.Text) && !String.IsNullOrEmpty(password.Text))
            {
                bool r = validate_login(user, pass);
                if (r)
                {
                    form1user = name.Text;
                    this.Hide();
                    PageWindow page = new PageWindow(this.form1user);
                    page.ShowDialog();

                }
                else
                {
                    MessageBox.Show("用户名或密码有误");
                    name.Text = "";
                    password.Text = "";
                }
            }
            else
            {
                MessageBox.Show("用户名或密码不能为空");
            }
        }
    }
}
