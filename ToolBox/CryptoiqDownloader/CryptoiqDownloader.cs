﻿using System;
using System.Collections.Generic;
using System.Net;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using Newtonsoft.Json;
using System.Linq;

namespace QuantConnect.ToolBox.CryptoiqDownloader
{
    /// <summary>
    /// cryptoiq Data Downloader class 
    /// </summary>
    public class CryptoiqDownloader : ICryptoiqDataDownloader
    {


        /// <summary>
        /// Get historical data enumerable for a single symbol, type and resolution given this start and end time (in UTC).
        /// </summary>
        /// <param name="symbol">Symbol for the data we're looking for.</param>
        /// <param name="resolution">Resolution of the data request</param>
        /// <param name="startUtc">Start time of the data in UTC</param>
        /// <param name="endUtc">End time of the data in UTC</param>
        /// <returns>Enumerable of base data for this symbol</returns>
        public IEnumerable<BaseData> Get(Symbol symbol, Resolution resolution, DateTime startUtc, DateTime endUtc, string exchange = "bitfinex")
        {
            DateTime counter = startUtc;
            int hour = 1;
            var url = "http://cryptoiq.io/api/marketdata/ticker/{3}/{2}/{0}/{1}";

            while (counter < endUtc)
            {
               // Console.WriteLine(counter.ToString());
                while (hour < 24)
                {
                   // Console.WriteLine(hour.ToString());
                    string request = String.Format(url, counter.ToString("yyyy-MM-dd"), hour.ToString(), symbol.Value, exchange);

                    using (var cl = new WebClient())
                    {
                        //cl.Proxy
                        var data = cl.DownloadString(request);

                        var mbtc = JsonConvert.DeserializeObject<List<CryptoiqBitcoin>>(data);
                        mbtc = mbtc.OrderBy(m => m.time).ToList();
                        foreach (var item in mbtc)
                        {
                            yield return new Tick
                            {
                                Time = item.time,
                                Symbol = symbol.Value,
                                Value = item.last,
                                AskPrice = item.ask,
                                BidPrice = item.bid,
                                TickType = QuantConnect.TickType.Quote
                            };

                        }
                        hour++;
                    }
                }
                counter = counter.AddDays(1);
                hour = 0;

            }

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
