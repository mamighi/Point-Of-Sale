using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Linq;
using System.IO;

namespace Point_Of_Sale
{
    class DataBaseManager
    {
        //Global variables
        SQLiteConnection conn = new SQLiteConnection("Data Source=DataSource.sqlite;Version=3;");
        public DataBaseManager()
        {
            //userTable
           

        }
        public void creatTable()
        {
             createUserDatabase();
            addFirstAdmin();

            createProductTable();

            createSaleTable();
            createSaleDetailTable();
        }
        public void createUserDatabase()
        {
            conn.Open();
            string sql = "create table if not exists user (userName varchar(50),password varchar(50),firstName varchar(50),lastName varchar(50),type varchar(20))";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public void createProductTable()
        {
            conn.Open();
            string sql = "create table if not exists product (id int,name varchar(50),des varchar(200),price float,image blob)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public void createSaleTable()
        {
            conn.Open();
            string sql = "create table if not exists sale (id int,date varchar(100),payment varchar(10),total float,discount int,gtotal float)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public void createSaleDetailTable()
        {
            conn.Open();
            string sql = "create table if not exists saleDet (saleid int,itemid int,qun int)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
      
        public int getProdId()
        {
            int retVal = 0;
            conn.Open();
            string sql = "SELECT id FROM product";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            while (sqReader.Read())
                retVal = sqReader.GetInt32(0);
            sqReader.Close();
            conn.Close();
            return retVal+1;
        }
        public int getSaleId()
        {
            int retVal = 0;
            conn.Open();
            string sql = "SELECT id FROM sale";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            while (sqReader.Read())
                retVal = sqReader.GetInt32(0);
            sqReader.Close();
            conn.Close();
            return retVal + 1;
        }
        public List<int> allProductsId()
        {
            List<int> retVal = new List<int>();
            conn.Open();
            string sql = "SELECT id FROM product";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            while (sqReader.Read())
                retVal.Add(sqReader.GetInt32(0));
            sqReader.Close();
            conn.Close();
            return retVal;

        }
        public DataManager.product getProductById(int id)
        {
            DataManager.product retVal = new DataManager.product();
            conn.Open();
            string sql = "SELECT * FROM product WHERE id='"+id+"'";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            if (sqReader.Read())
            {
                retVal.id = id;
                retVal.name = sqReader.GetString(1);
                retVal.des = sqReader.GetString(2);
                retVal.price = sqReader.GetFloat(3);
                retVal.image = (byte[])sqReader.GetValue(4); 
            }
            sqReader.Close();
            conn.Close();

            return retVal;
        }
        public List<DataManager.product> getAllProducts()
        {
            List<DataManager.product> retVal = new List<DataManager.product>();
            conn.Open();
            string sql = "SELECT * FROM product";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            while (sqReader.Read())
            {
                DataManager.product temp = new DataManager.product();
                temp.id = sqReader.GetInt32(0);
                temp.name = sqReader.GetString(1);
                temp.des = sqReader.GetString(2);
                temp.price = sqReader.GetFloat(3);
                temp.image = (byte[])sqReader.GetValue(4);
                retVal.Add(temp);
            }
            sqReader.Close();
            conn.Close();

            return retVal;
        }
        public void addNewProduct(int id, string name, string des, float price, Byte[] image)
        {
            SQLiteCommand mycommand = new SQLiteCommand(conn);
            mycommand.CommandText = "insert into product (id,name,des,price,image) values ('" + id + "', '" + name + "','" + des + "', '" + price + "',@image)";
            SQLiteParameter parameter = new SQLiteParameter("@image", System.Data.DbType.Binary);
            parameter.Value = image;
            mycommand.Parameters.Add(parameter);
            conn.Open();
            //string sql = "insert into product (id,name,des,price,image) values ('"+id+"', '"+name+"','"+des+"', '"+price+"','"+image+"')";
            mycommand.ExecuteNonQuery();
            conn.Close();
        }
        public void editProduct(int id, string name, string des, float price, Byte[] image)
        {
            SQLiteCommand mycommand = new SQLiteCommand(conn);
            mycommand.CommandText = "update product Set name='" + name + "', des='" + des + "',price='" + price + "',image=@image WHERE id='"+id+"'";
            SQLiteParameter parameter = new SQLiteParameter("@image", System.Data.DbType.Binary);
            parameter.Value = image;
            mycommand.Parameters.Add(parameter);
            conn.Open();
            //string sql = "insert into product (id,name,des,price,image) values ('"+id+"', '"+name+"','"+des+"', '"+price+"','"+image+"')";
            mycommand.ExecuteNonQuery();
            conn.Close();
        }

        public void deleteProduct(int id)
        {
            conn.Open();
            string sql = "DELETE FROM product WHERE id='" + id + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public bool isAdminExist()
        {
            bool retVal = false;
            conn.Open();
            string sql = "SELECT * FROM user";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            if (sqReader.Read())
                retVal = true;
            sqReader.Close();
            conn.Close();
            return retVal;
        }
        public void addFirstAdmin()
        {
            if (isAdminExist())
                return;
            conn.Open();
            string sql = "insert into user (userName,password,firstName,lastName,type) values ('admin', 'admin','admin', 'admin','admin')";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public string login(string userName, string password)
        {
            string retVal = string.Empty;
            conn.Open();
            string sql = "SELECT * FROM user where userName='"+userName+"' AND password='"+password+"'";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            if (sqReader.Read())
                retVal = sqReader.GetString(4);
            sqReader.Close();
            conn.Close();
            return retVal;
        }
        public DataManager.user getUserDetails(string userName)
        {
            DataManager.user retVal = new DataManager.user();
            conn.Open();
            string sql = "SELECT * FROM user where userName='" + userName + "'";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            if (sqReader.Read())
            {
                retVal.userName = userName;
                retVal.password = sqReader.GetString(1);
                retVal.firstName = sqReader.GetString(2);
                retVal.lastName = sqReader.GetString(3);
                
            }
            sqReader.Close();
            conn.Close();
            return retVal;
        }
        public void updateAccount(DataManager.user details)
        {
            conn.Open();
            string sql="UPDATE user SET password='"+details.password+"', firstName='"+details.firstName+"', lastName='"+details.lastName+"' WHERE userName='"+details.userName+"'";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            sqCommand.ExecuteNonQuery();
            conn.Close();
        }
        public bool isUserNameExist(string userName)
        {
            bool retVal = false;
            conn.Open();
            string sql = "SELECT * FROM user WHERE userName='"+userName+"'";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            if (sqReader.Read())
                retVal = true;
            sqReader.Close();
            conn.Close();
            return retVal;
        }
        public void addNewAccount(string userName, string password, string fn, string ln, string type)
        {
            conn.Open();
            string sql = "insert into user (userName,password,firstName,lastName,type) values ('"+userName+"', '"+password+"','"+fn+"', '"+ln+"','"+type+"')";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public void addNewSale(DataManager.sale sale)
        {
            conn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(conn);
            mycommand.CommandText = "insert into sale (id,date,payment,total,discount,gtotal) values ('" + sale.id + "','" + sale.date.ToString() + "','" + sale.payment + "','" + sale.total + "','" + sale.discount + "','" + sale.gtotal + "')";
            //string sql = "insert into product (id,name,des,price,image) values ('"+id+"', '"+name+"','"+des+"', '"+price+"','"+image+"')";
            mycommand.ExecuteNonQuery();
            string sql = "";
            SQLiteCommand command;


            foreach (DataManager.saleDetails temp in sale.det)
            {
                sql = "insert into saleDet (saleid,itemid,qun) values ('" + sale.id + "','" + temp.itemId + "','" + temp.qun + "')";
                command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
            conn.Close();
        }
        public List<DataManager.sale> getAllSales()
        {
            List<DataManager.sale> retVal = new List<DataManager.sale>();
            conn.Open();
            string sql = "SELECT * FROM sale";
            SQLiteCommand sqCommand = new SQLiteCommand(sql, conn);
            SQLiteDataReader sqReader = sqCommand.ExecuteReader();
            while (sqReader.Read())
            {
                DataManager.sale temp = new DataManager.sale();
                temp.id = sqReader.GetInt32(0);
                temp.date = DateTime.Parse(sqReader.GetString(1));
                temp.payment = sqReader.GetString(2);
                temp.total = sqReader.GetFloat(3);
                temp.discount = sqReader.GetInt32(4);
                temp.gtotal=sqReader.GetFloat(5);
                retVal.Add(temp);
            }
           

            for (int i = 0; i < retVal.Count; i++)
            {
                sql = "SELECT * FROM saleDet WHERE saleid='" + retVal[i].id + "'";
                sqCommand = new SQLiteCommand(sql, conn);
                sqReader = sqCommand.ExecuteReader();
                List<DataManager.saleDetails> addToRetVal = new List<DataManager.saleDetails>();
                while (sqReader.Read())
                {
                    DataManager.saleDetails tmp = new DataManager.saleDetails();
                    tmp.itemId = sqReader.GetInt32(1);
                    tmp.qun = sqReader.GetInt32(2);
                    addToRetVal.Add(tmp);
                }
                retVal[i].det = addToRetVal;
            }

            sqReader.Close();
            conn.Close();
            return retVal;

        }

    }
}
