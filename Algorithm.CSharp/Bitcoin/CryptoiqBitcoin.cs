using Newtonsoft.Json;
using QuantConnect.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Algorithm.CSharp
{
    public class CryptoiqBitcoin : BaseData
    {

        #region Properties
        public List<CryptoiqBitcoin> List;
        private int counter = 0;
        public DateTime time;
        public decimal ask;
        public decimal bid;
        public decimal last;
        public decimal high;
        public decimal low;
        public decimal volume;
        #endregion

        /// <summary>
        /// 1. DEFAULT CONSTRUCTOR: Custom data types need a default constructor.
        /// We search for a default constructor so please provide one here. It won't be used for data, just to generate the "Factory".
        /// </summary>
        public CryptoiqBitcoin()
        {
            Symbol = "BTC";
        }


        /// <summary>
        /// 2. RETURN THE STRING URL SOURCE LOCATION FOR YOUR DATA:
        /// This is a powerful and dynamic select source file method. If you have a large dataset, 10+mb we recommend you break it into smaller files. E.g. One zip per year.
        /// We can accept raw text or ZIP files. We read the file extension to determine if it is a zip file.
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="date">Date of this source file</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>String URL of source file.</returns>
        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
            string url = "http://cryptoiq.io/api/marketdata/ticker/bitfinex/btcusd/" + date.ToString("yyyy-MM-dd/") + date.ToString("hh");
            return new SubscriptionDataSource(url, SubscriptionTransportMedium.Rest);
        }

        /// <summary>
        /// 3. READER METHOD: Read 1 line from data source and convert it into Object.
        /// Each line of the CSV File is presented in here. The backend downloads your file, loads it into memory and then line by line
        /// feeds it into your algorithm
        /// </summary>
        /// <param name="line">string line from the data source file submitted above</param>
        /// <param name="config">Subscription data, symbol name, data type</param>
        /// <param name="date">Current date we're requesting. This allows you to break up the data source into daily files.</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>New Bitcoin Object which extends BaseData.</returns>
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {

            if (List == null)
            {
                List = new List<CryptoiqBitcoin>();
                try
                {
                    List = JsonConvert.DeserializeObject<List<CryptoiqBitcoin>>(line);
                    List.ForEach(l => MapBaseData(l));
                    List = List.OrderBy(m => m.Time).ToList();
                }
                catch (Exception ex)
                { }
            }


            counter++;
            if (counter < List.Count)
            {
                return List.ElementAt(counter - 1);
            }
            return null;
        }

        private void MapBaseData(CryptoiqBitcoin coin)
        {
            coin.DataType = MarketDataType.Base;
            coin.Symbol = "XBTUSD";
            coin.Value = coin.last;
            coin.Time = coin.time;
        }      
        

    }
}
