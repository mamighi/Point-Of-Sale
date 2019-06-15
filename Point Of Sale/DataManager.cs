using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point_Of_Sale
{
    class DataManager
    {
        public class product
        {
            public int id;
            public string name, des;
            public float price;
            public Byte[] image;
        }
        public class sale
        {
            public int id;
            public DateTime date;
            public string payment;
            public float total, gtotal;
            public int discount;
            public List<saleDetails> det = new List<saleDetails>();
        }
        public class saleDetails
        {
            public int itemId;
            public int qun;
        }
        public class user
        {
            public string userName, password, firstName, lastName;

        }
    }
}
