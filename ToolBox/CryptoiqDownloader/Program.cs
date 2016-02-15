using System;
using System.Globalization;
using QuantConnect.Configuration;
using QuantConnect.Logging;

namespace QuantConnect.ToolBox.CryptoiqDownloader
{
    class Program
    {
        /// <summary>
        /// cryptoiq Downloader Toolbox Project For LEAN Algorithmic Trading Engine.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                args = new string[] { args[0], DateTime.UtcNow.ToString("yyyyMMdd"), args[1], args[2] };
            }
            else if (args.Length < 4)
            {
                Console.WriteLine("Usage: CryptoiqDownloader FROMDATE TODATE EXCHANGE SYMBOL");
                Console.WriteLine("FROMDATE = yyyymmdd");
                Console.WriteLine("TODATE = yyyymmdd");
                //todo: remove default params

                args = new string[] { "20160201", DateTime.UtcNow.ToString("yyyyMMdd"), "bitfinex", "BTCUSD" };
            }

            try
            {
                // Load settings from command line
                var startDate = DateTime.ParseExact(args[0], "yyyyMMdd", CultureInfo.InvariantCulture);
                var endDate = DateTime.ParseExact(args[1], "yyyyMMdd", CultureInfo.InvariantCulture);

                // Load settings from config.json
                var dataDirectory = Config.Get("data-directory", "../../../Data");

                // Create an instance of the downloader
                const string market = Market.Bitcoin;
                var downloader = new CryptoiqDownloader();

                // Download the data
                var symbolObject = Symbol.Create(args[3], SecurityType.Forex, market);
                var data = downloader.Get(symbolObject, Resolution.Tick, startDate, endDate, args[2]);

                // Save the data
                var writer = new LeanDataWriter(SecurityType.Forex, Resolution.Tick, symbolObject, dataDirectory, market);
                writer.Write(data);

            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }
    }
}
