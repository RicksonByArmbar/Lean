﻿/*
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
using QuantConnect.Configuration;

namespace QuantConnect.ToolBox.QuandlBitfinexDownloader
{
    /// <summary>
    /// Quandl Bitfinex Data Downloader class 
    /// </summary>
    public class QuandlBitfinexDownloader : IDataDownloader
    {

        string _apiKey;

        public QuandlBitfinexDownloader(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// Get historical data enumerable for Bitfinex from Quandl
        /// </summary>
        /// <param name="symbol">Symbol for the data we're looking for.</param>
        /// <param name="resolution">Only Daily is supported</param>
        /// <param name="startUtc">Start time of the data in UTC</param>
        /// <param name="endUtc">End time of the data in UTC</param>
        /// <returns>Enumerable of base data for this symbol</returns>
        public IEnumerable<BaseData> Get(Symbol symbol, Resolution resolution, DateTime startUtc, DateTime endUtc)
        {

            if (resolution != Resolution.Daily)
            {
                throw new ArgumentException("Only daily data is currently supported.");
            }

            string collapse = "daily";

            var url = "https://www.quandl.com/api/v3/datasets/BCHARTS/BITFINEXUSD.csv?order=asc&collapse=" + collapse + "&api_key=" + _apiKey + "&start_date="
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
                        var bar = new TradeBar
                        {
                            Time = DateTime.Parse(line[0]),
                            Open = decimal.Parse(line[1]),
                            High = decimal.Parse(line[2]),
                            Low = decimal.Parse(line[3]),
                            Close = decimal.Parse(line[4]),
                            Value = decimal.Parse(line[7]),
                            Volume = (long)Math.Round(decimal.Parse(line[5]), 0),
                            Symbol = symbol,
                            DataType = MarketDataType.TradeBar,
                            Period = new TimeSpan(24,0,0),
                        };
                        System.Diagnostics.Debug.WriteLine(line[0]);
                        yield return bar;
                    }


                }
            }

        }

    }
}
