/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
using System;
using System.Globalization;
using QuantConnect.Configuration;
using QuantConnect.Logging;

namespace QuantConnect.ToolBox.QuandlBitcoinDownloader
{
    class Program
    {
        /// <summary>
        /// Yahoo Downloader Toolbox Project For LEAN Algorithmic Trading Engine.
        /// Original by @chrisdk2015, tidied by @jaredbroad
        /// </summary>
        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Downloader FROMDATE");
                Console.WriteLine("FROMDATE = yyyymmdd");
                args = new string[] { "20140101" };
            }

            try
            {
                // Load settings from config.json
                var dataDirectory = Config.Get("data-directory", "../../../Data");

                // Create an instance of the downloader
                const string market = Market.Bitcoin;
                var downloader = new QuandlBitcoinDownloader();

                // Download the data
                var symbolObject = Symbol.Create("BTCUSD", SecurityType.Forex, market);
                var data = downloader.Get(null, Resolution.Daily, DateTime.ParseExact(args[0], "yyyyMMdd", CultureInfo.CurrentCulture), DateTime.UtcNow);

                // Save the data
                var writer = new LeanDataWriter(SecurityType.Forex, Resolution.Daily, symbolObject, dataDirectory, market);
                writer.Write(data);

            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }
    }
}
