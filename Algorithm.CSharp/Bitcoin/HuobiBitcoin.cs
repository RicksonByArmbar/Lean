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

    public class HuobiBitcoin : BaseData
    {

        #region Properties
        public int time;

        public HuobiBitcoin ticker { get; set; }

        public decimal open;
        public decimal vol;
        public decimal last;
        public decimal buy;
        public decimal sell;
        public decimal high;
        public decimal low;
        #endregion

        /// <summary>
        /// 1. DEFAULT CONSTRUCTOR: Custom data types need a default constructor.
        /// We search for a default constructor so please provide one here. It won't be used for data, just to generate the "Factory".
        /// </summary>
        public HuobiBitcoin()
        {
            Symbol = "BTCCNY";
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
            if (isLiveMode)
            {
                string url = "http://api.huobi.com/staticmarket/ticker_btc_json.js";
                return new SubscriptionDataSource(url, SubscriptionTransportMedium.Rest);
            }
            else
            {
                var dataType = TickType.Quote;
                var dateFormat = "yyyyMMdd";
                if (config.SecurityType == SecurityType.Forex)
                {
                    dataType = TickType.Quote;
                }

                var symbol = string.IsNullOrEmpty(config.MappedSymbol) ? config.Symbol.Value : config.MappedSymbol;
                var securityType = SecurityType.Forex.ToString();
                var market = config.Market.ToLower();
                var resolution = config.Resolution.ToString().ToLower();
                var file = date.ToString(dateFormat) + "_" + dataType.ToString().ToLower() + ".zip";

                //Add in the market for equities/cfd/forex for internationalization support.
                var source = System.IO.Path.Combine(Constants.DataFolder, securityType, Market.Bitcoin.ToString(), resolution, symbol.ToLower(), file);

                return new SubscriptionDataSource(source, SubscriptionTransportMedium.LocalFile, FileFormat.Csv);
            }
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

            if (isLiveMode)
            {
                HuobiBitcoin coin = JsonConvert.DeserializeObject<HuobiBitcoin>(line);
                MapBaseData(coin);
                return coin;
            }
            else
            {
                var split = line.Split(',');

                HuobiBitcoin coin = new HuobiBitcoin
                {
                    Time = date.AddMilliseconds(long.Parse(split[0])),
                    ticker = new HuobiBitcoin
                    {
                        last = decimal.Parse(split[1])
                    },
                    Value = decimal.Parse(split[1])
                };
                return coin;
            }

        }

        private void MapBaseData(HuobiBitcoin coin)
        {
            coin.DataType = MarketDataType.Base;
            coin.Symbol = Symbol;
            coin.Value = coin.ticker.last;
            coin.Time = QuantConnect.Time.UnixTimeStampToDateTime(coin.time);
        }


    }
}
