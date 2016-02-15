using QuantConnect.Indicators;
using QuantConnect.Algorithm;
using QuantConnect;
using QuantConnect.Brokerages;
using NodaTime;
using QuantConnect.Data.Market;
using System;
using System.Collections.Generic;

namespace QuantConnect.Algorithm.CSharp
{

    public class Macd : QCAlgorithm
    {
        private MovingAverageConvergenceDivergence _macd;
        public int Period = 8;
        public int LongCycle = 34;
        public int ShortCycle = 13;
        public decimal stopLoss = 0.04m;
        public decimal takeProfit = 0.4m;
        string symbol = "BTCUSD";
        decimal leverage = 3.3m;
        object locker = new object();

        public override void Initialize()
        {
            SetStartDate(2015, 10, 1);
            SetEndDate(2016, 2, 1);
            SetBrokerageModel(BrokerageName.Bitfinex, AccountType.Margin);
            SetTimeZone(DateTimeZone.Utc);
            AddSecurity(SecurityType.Forex, symbol, Resolution.Tick, "bitcoin", false, leverage + 1, false);
            SetCash("USD", 1000, 1m);
            _macd = MACD(symbol, ShortCycle, LongCycle, Period, MovingAverageType.LinearWeightedMovingAverage, Resolution.Hour);
            SetDefaultMarkets(new Dictionary<SecurityType, string> { { SecurityType.Forex, Market.Bitcoin.ToString() } });

            IEnumerable<List<Tick>> history = History<List<Tick>>(symbol, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow, Resolution.Tick);

            foreach (var list in history)
            {
                foreach (var item in list)
                {

                    _macd.Update(new IndicatorDataPoint
                    {
                        DataType = MarketDataType.Tick,
                        EndTime = item.EndTime,
                        Symbol = symbol,
                        Value = item.Price
                    });
                }
            }
        }

        protected void OnData(Tick data)
        {

            if (_macd.IsReady)
            {
                if (!Portfolio[symbol].IsLong && _macd.Current.Value > 0.0m && _macd.Signal.Current.Value > 0)
                {
                    System.Threading.Thread.Sleep(1);
                    Buy(data);
                }
                if (!Portfolio[symbol].IsShort && _macd.Current.Value < 0.0m && _macd.Signal.Current.Value < 0)
                {
                    System.Threading.Thread.Sleep(1);
                    Sell(data);
                }
            }
        }

        public void OnData(Ticks data)
        {
            foreach (var item in data)
            {
                foreach (var tick in item.Value)
                {
                    OnData(tick);
                }
            }
        }

        private void Buy(Tick tick)
        {
            int volume = CalculateOrderQuantity(symbol, leverage - 0.5m);
            if (volume > 0)
            {
                StopMarketOrder(symbol, volume, tick.Price - tick.Price * stopLoss);
                //SetHoldings(symbol, 0.9);
                Output("buy");
            }
        }

        private void Sell(Tick tick)
        {
            int volume = CalculateOrderQuantity(symbol, -leverage + 0.5m);
            if (volume < 0)
            {
                StopMarketOrder(symbol, volume, tick.Price + tick.Price * stopLoss);
                //SetHoldings(symbol, -0.9);
                Output("sell");
                //System.Threading.Thread.Sleep(1000);
            }
        }

        private void Output(string title)
        {
            Log(title + ": " + Portfolio.Securities[symbol].Price.ToString() + " Trade:" + Portfolio[symbol].LastTradeProfit
                + " Total:" + Portfolio.TotalPortfolioValue);
        }


    }
}