using ScooterRental.Interfaces;
using System.Collections.Generic;

namespace ScooterRental
{
    public class Database : IDatabase
    {
        private Dictionary<int, List<decimal>> _incomeData = new Dictionary<int, List<decimal>>();

        public Database(Dictionary<int, List<decimal>> incomeData)
        {
            _incomeData = incomeData;
        }

        public void AddToDatabase(int key, decimal value)
        {
            {
                if (_incomeData.ContainsKey(key))
                {
                    List<decimal> rentTotalPriceList = _incomeData[key];
                    rentTotalPriceList.Add(value);
                }
                else
                {
                    List<decimal> rentTotalPriceList = new List<decimal>();
                    rentTotalPriceList.Add(value);
                    _incomeData.Add(key, rentTotalPriceList);
                }
            }
        }
    }
}
