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
    public class QuandlBitcoin : BaseData
    {
        #region Properties
        //Set the defaults:
        /// <summary>
        /// Open Price
        /// </summary>
        public decimal Open = 0;

        /// <summary>
        /// High Price
        /// </summary>
        public decimal High = 0;

        /// <summary>
        /// Low Price
        /// </summary>
        public decimal Low = 0;

        /// <summary>
        /// Closing Price
        /// </summary>
        public decimal Close = 0;

        /// <summary>
        /// Volume in BTC
        /// </summary>
        public decimal VolumeBTC = 0;

        /// <summary>
        /// Volume in USD
        /// </summary>
        public decimal VolumeUSD = 0;

        /// <summary>
        /// Volume in USD:
        /// </summary>
        public decimal WeightedPrice = 0;

        public override DateTime EndTime { get; set; }
        #endregion

        /// <summary>
        /// 1. DEFAULT CONSTRUCTOR: Custom data types need a default constructor.
        /// We search for a default constructor so please provide one here. It won't be used for data, just to generate the "Factory".
        /// </summary>
        public QuandlBitcoin()
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
            string apiKey = ConfigurationManager.AppSettings["QuandlApiKey"];
            string url = "https://www.quandl.com/api/v3/datasets/BCHARTS/BITFINEXUSD.csv?order=asc&collapse=daily&api_key=" + apiKey + "&start_date="
                + date.ToString("yyyy-MM-dd");
            return new SubscriptionDataSource(url, SubscriptionTransportMedium.RemoteFile);
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
            var coin = new QuandlBitcoin();

            try
            {
                string[] data = line.Split(',');
                coin.Time = DateTime.Parse(data[0], CultureInfo.InvariantCulture);
                coin.Open = Convert.ToDecimal(data[1], CultureInfo.InvariantCulture) / 100;
                coin.High = Convert.ToDecimal(data[2], CultureInfo.InvariantCulture) / 100;
                coin.Low = Convert.ToDecimal(data[3], CultureInfo.InvariantCulture) / 100;
                coin.Close = Convert.ToDecimal(data[4], CultureInfo.InvariantCulture) / 100;
                coin.VolumeBTC = Convert.ToDecimal(data[5], CultureInfo.InvariantCulture);
                coin.VolumeUSD = Convert.ToDecimal(data[6], CultureInfo.InvariantCulture);
                coin.WeightedPrice = Convert.ToDecimal(data[7], CultureInfo.InvariantCulture);
                coin.Symbol = Symbol;
                coin.Value = coin.Close;
                coin.EndTime = coin.Time;
            }
            catch { /* Do nothing, skip first title row */ }

            return coin;
        }
    }
}
