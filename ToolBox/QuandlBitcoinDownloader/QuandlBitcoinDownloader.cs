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
using System.Collections.Generic;
using System.Net;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.IO.Compression;

namespace QuantConnect.ToolBox.QuandlBitcoinDownloader
{
    /// <summary>
    /// Yahoo Data Downloader class 
    /// </summary>
    public class QuandlBitcoinDownloader : IDataDownloader
    {


        /// <summary>
        /// Get historical data enumerable for a single symbol, type and resolution given this start and end time (in UTC).
        /// </summary>
        /// <param name="symbol">Symbol for the data we're looking for.</param>
        /// <param name="resolution">Resolution of the data request</param>
        /// <param name="startUtc">Start time of the data in UTC</param>
        /// <param name="endUtc">End time of the data in UTC</param>
        /// <returns>Enumerable of base data for this symbol</returns>
        public IEnumerable<BaseData> Get(Symbol symbol, Resolution resolution, DateTime startUtc, DateTime endUtc)
        {
            string apiKey = "vAhfNvBxVe3Ev66cjbTY";
            string collapse = "daily";

            var url = "https://www.quandl.com/api/v3/datasets/BCHARTS/BITFINEXUSD.csv?order=asc&collapse=" + collapse + "&api_key=" + apiKey + "&start_date="
                + startUtc.ToString("yyyy-MM-dd");
            using (var cl = new WebClient())
            {

                var data = cl.DownloadString(url);

                bool header = true;
                foreach (string item in data.Split('\n'))
                {

                    if (header)
                    {
                        header = false;
                        continue;
                    }

                    string[] line = item.Split(',');
                    if (line.Count() == 8)
                    {
                        var date = DateTime.Parse(line[0]);
                        System.Diagnostics.Debug.WriteLine(line[0]);
                        yield return new TradeBar
                        {
                            Time = date,
                            Open = decimal.Parse(line[1]),
                            High = decimal.Parse(line[2]),
                            Low = decimal.Parse(line[3]),
                            Close = decimal.Parse(line[4]),
                            Value = decimal.Parse(line[7]),
                            Volume = (long)Math.Round(decimal.Parse(line[5]), 0),
                            Symbol = "BTCUSD",
                            DataType = MarketDataType.TradeBar
                        };
                    }


                }
            }

        }

        private static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public class CryptoiqBitcoin
        {

            public DateTime time;
            public decimal ask;
            public decimal bid;
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal volume;
        }

    }
}
