using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public enum SnType
    {
        CarInsure = 1,
        Withdraw = 2,
        Transactions = 3,
        CarClaim = 4,
        DepositRent = 5
    }

    public class Sn
    {

        public static string Build(SnType type, int id)
        {
            string prefix = "UP";
            switch (type)
            {
                case SnType.CarInsure:
                    prefix = "CI";
                    break;
                case SnType.CarClaim:
                    prefix = "CC";
                    break;
                case SnType.Withdraw:
                    prefix = "WD";
                    break;
                case SnType.Transactions:
                    prefix = "TN";
                    break;
                case SnType.DepositRent:
                    prefix = "DR";
                    break;
            }

            string dateTime = DateTime.Now.ToString("yyMMddHHmmss");

            string sId = id.ToString().PadLeft(4, '0');

            string sn = prefix + dateTime + sId;

            return sn;
        }
    }
}
