using System;
using System.Globalization;
using QuantConnect.Configuration;
using QuantConnect.Logging;

namespace QuantConnect.ToolBox.BitcoinChartsDownloader
{
    class Program
    {
        /// <summary>
        /// BitcoinCharts Downloader Toolbox Project For LEAN Algorithmic Trading Engine.
        /// </summary>
        static void Main(string[] args)
        {

            try
            {
                // Load settings from config.json
                var dataDirectory = Config.Get("data-directory", "../../../Data");

                // Create an instance of the downloader
                const string market = Market.Bitcoin;
                var downloader = new BitcoinChartsDownloader();

                // Download the data
                var symbolObject = Symbol.Create("BTCUSD", SecurityType.Forex, market);
                var data = downloader.Get(null, Resolution.Second, DateTime.UtcNow, DateTime.UtcNow);

                // Save the data
                var writer = new LeanDataWriter(SecurityType.Forex, Resolution.Second, "BTCUSD", dataDirectory, market);
                writer.Write(data);

            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }
    }
}
