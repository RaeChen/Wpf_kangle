using System;
using System.Collections.Generic;
using System.Data;
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
using MySql.Data.MySqlClient;


namespace Wpf_kangle
{
    /// <summary>
    /// PageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PageWindow : Window
    {
        public string userid;

        public PageWindow(string file)
        {
            InitializeComponent();
            userid = file;
        }

        //数据库连接的变量
        private string conn;
        private MySqlConnection connect;

        public PageWindow()
        {
            InitializeComponent();

            /*
            conn = "server=localhost;user=root;database=kangle;port=3306;password=rebirth610;";
            connect = new MySqlConnection(conn);

            MySqlCommand cmd = new MySqlCommand("select * from mingxi", connect);
            connect.Open();
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            connect.Close();

            datagrid_qiantaimingxi_piaomian.DataContext = dt;
            */
        }


        //数据库连接
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

        private void tiqupiaomian_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string richText = new TextRange(richtextbox_mingxi_tiqupiaomian.Document.ContentStart, richtextbox_mingxi_tiqupiaomian.Document.ContentEnd).Text;

                //取得票面信息
                string piaomian = richText;

                /*
                            DETR: TN7312462959963                                                          
                ISSUED BY: XIAMEN AIRLINES           ORG / DST: XMN / NGB                 ARL - D
                E / R: ZX0120003 / 不得签转
                TOUR CODE:                                                    RECEIPT PRINTED
                PASSENGER: 庄睦贤
                EXCH:                               CONJ TKT:                                   
                O FM:1XMN MF    8075  Y 01NOV 0725 OK Y                / 31OCT8 20K LIFT/ BOARDED
                     T3-- RL: NDP9R8 / BN:109
                  TO: NGB
                FC: 01NOV17XMN MF NGB880.00CNY880.00END
                FARE:           CNY  880.00 | FOP:CASH(CNY)
                TAX: CNY 50.00CN | OI:                                                 
                TAX: EXEMPTYQ |
                TOTAL:          CNY  930.00 | TKTN: 731 - 2462959963
                */

                //票面测试

                //工号=登录时的id
                string piaomian_gonghao = userid;
                //出票日期为选择的日期
                DateTime piaomian_dingpiaoriqi = datepicker_mingxi.SelectedDate.Value.Date; //.ToShortDateString();
                                                                                            //piaomian_dingpiaoriqi=String.Format("{0:MM/dd/yyyy}", piaomian_dingpiaoriqi);

                //行程 城市对
                string piaomian_xingcheng = piaomian.Substring(piaomian.IndexOf("ORG/DST:") + 9, 7).Trim();
                //航班号
                string piaomian_hangbanhao;
                //舱位
                string piaomian_cangwei;
                //航班日期
                string piaomian_hangbanriqi;

                //含税价 900.00
                string 价格 = piaomian.Substring(piaomian.IndexOf("TOTAL:") + 20, piaomian.IndexOf("|TKTN:") - piaomian.IndexOf("TOTAL:") - 20).Trim();
                价格 = 价格.Remove(价格.Length - 3, 3);
                int piaomian_hanshuijia = Convert.ToInt32(价格);

                //票号
                string piaomian_piaohao = piaomian.Substring(piaomian.IndexOf("DETR:") + 10, piaomian.IndexOf("ISSUED BY") - piaomian.IndexOf("DETR:") - 10 - 1 - 7).Trim();
                //公司
                string piaomian_gongsi;
                //备注
                string piaomian_beizhu;

                //协议号(此处有不得签转字样需要修改)？
                string piaomian_xieyihao = piaomian.Substring(piaomian.IndexOf("E/R:") + 5, piaomian.IndexOf("TOUR CODE:") - piaomian.IndexOf("E/R:") - 7 - 1).Trim().Substring(0, 9);

                //退票
                string piaomian_tuipiao;

                //往返程
                string piaomian_wangfancheng;


                //乘机人
                string piaomian_chengjiren = piaomian.Substring(piaomian.IndexOf("PASSENGER:") + 10, piaomian.IndexOf("EXCH") - piaomian.IndexOf("PASSENGER:") - 10 - 1).Trim();


                //OPEN票
                //string piaomian_openpiao = piaomian.Substring(piaomian.IndexOf("TOTAL:") + 20, piaomian.IndexOf("|TKTN:"));

                Console.Write("工号" + piaomian_gonghao);
                Console.Write("订票日期" + piaomian_dingpiaoriqi.ToString());
                Console.Write("城市对" + piaomian_xingcheng);
                Console.Write("含税价" + piaomian_hanshuijia);
                Console.Write("协议号" + piaomian_xieyihao);
                Console.Write("乘机人" + piaomian_chengjiren);

                insert_mingxi(piaomian_gonghao, piaomian_dingpiaoriqi, piaomian_hanshuijia, "要华额度测试", "白爽测试", piaomian_xieyihao, piaomian_xingcheng, "Y测试", piaomian_piaohao);



                load_datagrid_qiantaimingxi_piaomian();
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常" + ex.ToString());
            }
            
        }

        private void insert_mingxi(string gh, DateTime dprq, int pj, string gs, string bz, string xyh, string hc, string cw, string ph)
        {
            try
            {
                //This is my connection string i have assigned the database file address path  
                string string_insert_mingxi = "server=localhost;user=root;database=kangle;port=3306;password=rebirth610;";

                //This is my insert sql_insert_mingxi in which i am taking input from the user through windows forms  
                string sql_insert_mingxi = "insert into mingxi(mingxi_gonghao,mingxi_dingpiaoriqi,mingxi_xingcheng,mingxi_hanshuijia,mingxi_piaohao,mingxi_xieyihao) values('" + gh + "','" + dprq + "','" + hc + "','" + pj + "','" + ph + "','" + xyh + "');";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection connect_insert_mingxi = new MySqlConnection(string_insert_mingxi);
                //This is command class which will handle the sql_insert_mingxi and connection object.  
                MySqlCommand command_insert_mingxi = new MySqlCommand(sql_insert_mingxi, connect_insert_mingxi);
                MySqlDataReader reader_insert_mingxi;
                connect_insert_mingxi.Open();
                reader_insert_mingxi = command_insert_mingxi.ExecuteReader();     // Here our sql_insert_mingxi will be executed and data saved into the database.  
               // MessageBox.Show("Save Data");

                while (reader_insert_mingxi.Read())
                {
                }
                connect_insert_mingxi.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void load_datagrid_qiantaimingxi_piaomian()
        {
            try
            {
                conn = "server=localhost;user=root;database=kangle;port=3306;password=rebirth610;";
                connect = new MySqlConnection(conn);

                MySqlCommand cmd = new MySqlCommand("select mingxi_gonghao,mingxi_dingpiaoriqi,mingxi_xingcheng,mingxi_hanshuijia,mingxi_piaohao,mingxi_xieyihao from mingxi", connect);
                connect.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                connect.Close();

                datagrid_qiantaimingxi_piaomian.DataContext = dt;

                //修改日期格式
                //dataGridView1.Columns[0].DefaultCellStyle.Format = "dd/MM/yyyy";

                //var col = new DataGridTextColumn();
                //col.Header = "工号";
                //col.Binding = new Binding("mingxi_gonghao");
                //datagrid_qiantaimingxi_piaomian.Columns.Add(col);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /*
       //改
       private void update()
       {
           try
           {
               //This is my connection string i have assigned the database file address path  
               string MyConnection2 = "datasource=localhost;port=3307;username=root;password=root";
               //This is my update query in which i am taking input from the user through windows forms and update the record.  
               string Query = "update student.studentinfo set idStudentInfo='" + this.IdTextBox.Text + "',Name='" + this.NameTextBox.Text + "',Father_Name='" + this.FnameTextBox.Text + "',Age='" + this.AgeTextBox.Text + "',Semester='" + this.SemesterTextBox.Text + "' where idStudentInfo='" + this.IdTextBox.Text + "';";
               //This is  MySqlConnection here i have created the object and pass my connection string.  
               MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
               MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
               MySqlDataReader MyReader2;
               MyConn2.Open();
               MyReader2 = MyCommand2.ExecuteReader();
               MessageBox.Show("Data Updated");
               while (MyReader2.Read())
               {
               }
               MyConn2.Close();//Connection closed here  
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }
       }

       //删
       private void delete()
       {
           try
           {
               string MyConnection2 = "datasource=localhost;port=3307;username=root;password=root";
               string Query = "delete from student.studentinfo where idStudentInfo='" + this.IdTextBox.Text + "';";
               MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
               MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
               MySqlDataReader MyReader2;
               MyConn2.Open();
               MyReader2 = MyCommand2.ExecuteReader();
               MessageBox.Show("Data Deleted");
               while (MyReader2.Read())
               {
               }
               MyConn2.Close();
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }
       }

       //查
       private void display()
       {
           try
           {
               string MyConnection2 = "datasource=localhost;port=3307;username=root;password=root";
               //Display query  
               string Query = "select * from student.studentinfo;";
               MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
               MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
               //  MyConn2.Open();  
               //For offline connection we weill use  MySqlDataAdapter class.  
               MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
               MyAdapter.SelectCommand = MyCommand2;
               DataTable dTable = new DataTable();
               MyAdapter.Fill(dTable);
               //此处为datagrid需要修改？
               dataGridView1.DataSource = dTable; // here i have assign dTable object to the dataGridView1 object to display data.               
                                                  // MyConn2.Close();  
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }
       }
       */
    }
}
