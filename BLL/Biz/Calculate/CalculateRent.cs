using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{

    public class RentList
    {
        public RentList(string text, int months, decimal price)
        {
            this.Text = text;
            this.Months = months;
            this.Price = price;

        }
        public string Text { get; set; }

        public int Months { get; set; }

        public decimal Price { get; set; }
    }

    public class CalculateRent
    {
        private string _version = "2017.02.20";
        private List<RentList> _rentList = new List<RentList>();
        private string _remark = "";
        private decimal _monthlyRent = 200;


        public string Version
        {
            get
            {
                return _version;
            }

        }

        public string Remark
        {
            get
            {
                return _remark;
            }

        }

        public decimal MonthlyRent
        {
            get
            {
                return _monthlyRent;
            }
        }

        public List<RentList> RentList
        {
            get
            {
                return _rentList;
            }
        }

        public CalculateRent(decimal monthlyRent)
        {
            _monthlyRent = monthlyRent;

            this.RentList.Add(new BLL.RentList("3个月", 3, 3 * monthlyRent));
            this.RentList.Add(new BLL.RentList("6个月", 6, 6 * monthlyRent));
            this.RentList.Add(new BLL.RentList("12个月", 12, 12 * monthlyRent));
        }

        public decimal GetRent(int months)
        {
            var rent = this.RentList.Where(m => m.Months == months).FirstOrDefault();
            return rent.Price;
        }

    }
}
