using System;
using System.Linq;
using QuantConnect.Data.Market;
using QuantConnect.Algorithm;
using QuantConnect.Indicators;
using System.ComponentModel;
using QuantConnect;
using QuantConnect.Brokerages;
using NodaTime;

namespace QuantConnect.Algorithm.CSharp
{

    public class BlackChevette : QCAlgorithm
    {

        public decimal stopLoss { get; set; }
        public decimal takeProfit { get; set; }
        private BollingerBands _bb;
        string symbol = "BTCUSD";
        object locker = new object();
        Tick last = null;
        decimal lastLow;
        decimal lastUp;
        decimal leverage = 3.3m;
        RelativeStrengthIndex rsi;

        public override void Initialize()
        {
            decimal standardDeviation = 2.75m;
            int period = 377;
            stopLoss = 0.1m;
            takeProfit = 0.4m;
            SetStartDate(2015, 11, 10);
            SetEndDate(2016, 2, 5);
            var resolution = Resolution.Tick;
            SetBrokerageModel(BrokerageName.Bitfinex, AccountType.Margin);
            SetTimeZone(DateTimeZone.Utc);

            AddSecurity(SecurityType.Forex, symbol, resolution, "bitcoin", false, leverage + 1, false);
            _bb = BB(symbol, period, standardDeviation, MovingAverageType.LinearWeightedMovingAverage, Resolution.Minute);
            rsi = RSI(symbol, 89, MovingAverageType.LinearWeightedMovingAverage, Resolution.Minute);
            SetCash("USD", 1000, 1m);
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

        void OnData(Tick tick)
        {
            if (last != null && _bb.IsReady)
            {
                if (Portfolio[symbol].TotalCloseProfit() / Portfolio.TotalPortfolioValue > takeProfit)
                {
                    Liquidate();
                    Output("TakeProfit");
                }

                if (Portfolio[symbol].TotalCloseProfit() / Portfolio.TotalPortfolioValue < -stopLoss)
                {
                    Liquidate();
                    Output("StopLoss");
                }

                if (!Portfolio[symbol].IsLong && tick.LastPrice > _bb.UpperBand.Current.Value && last.LastPrice <= lastUp && tick.LastPrice < 430 && rsi.Current.Value < 70)
                {
                    if (!LiveMode)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                    int volume = CalculateOrderQuantity(symbol, leverage - 0.3m);
                    if (volume > 0)
                    {
                        StopMarketOrder(symbol, volume, tick.Price - tick.Price * stopLoss);
                        Output("long");
                    }
                }
                if (!Portfolio[symbol].IsShort && tick.LastPrice < _bb.LowerBand.Current.Value && last.LastPrice <= lastLow && tick.LastPrice > 250 && rsi.Current.Value > 35)
                {
                    if (!LiveMode)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                    int volume = CalculateOrderQuantity(symbol, -leverage + 0.3m);
                    if (volume < 0)
                    {
                        StopMarketOrder(symbol, volume, tick.Price + tick.Price * stopLoss);
                        Output("short");
                    }
                }

                lastUp = _bb.UpperBand.Current.Value;
                lastLow = _bb.LowerBand.Current.Value;
            }
            last = tick;
        }

        private void Output(string title)
        {
            Log(title + ": " + Portfolio.Securities[symbol].Price.ToString() + " Trade:" + Portfolio[symbol].LastTradeProfit
                + " Total:" + Portfolio.TotalPortfolioValue + " rsi:" + rsi.Current.Value.ToString());
        }

    }
}