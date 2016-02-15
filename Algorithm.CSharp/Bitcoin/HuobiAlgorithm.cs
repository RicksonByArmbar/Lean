using NodaTime;
using QuantConnect.Algorithm;
using QuantConnect.Algorithm.CSharp;
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Data.Custom;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Orders.Fees;
using QuantConnect.Orders.Fills;
using QuantConnect.Orders.Slippage;
using QuantConnect.Securities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantConnect
{

    public partial class HuobiAlgorithm : QCAlgorithm
    {

        string symbol = "BTCUSD";
        string cnySymbol = "BTCCNY";
        static int period = 5;
        public MomentumPercent momcny;
        public MomentumPercent mumusd;
        private static object locker = new Object();
        private static object locker2 = new Object();
        decimal lastUSD = 0;
        decimal lastCNY = 0;
        DateTime lastUsdDate;
        DateTime lastCnyDate;

        public override void Initialize()
        {
            SetStartDate(2015, 11, 01);
            SetEndDate(2016, 1, 31);
            var resolution = Resolution.Tick;
            SetBrokerageModel(BrokerageName.Bitfinex, AccountType.Margin);
            SetTimeZone(DateTimeZone.Utc);
            SetCash(1000);
            AddSecurity(SecurityType.Forex, symbol, resolution, Market.Bitcoin.ToString(), false, 2.0m, false);
            AddData<HuobiBitcoin>(cnySymbol, Resolution.Tick, DateTimeZone.Utc, false, 1.0m);

            mumusd = MOMP(symbol, period, Resolution.Tick);
            momcny = MOMP(cnySymbol, period, Resolution.Tick);
        }

        public void OnData(Tick data)
        {
            //mumusd.Update(new IndicatorDataPoint
            //{
            //    DataType = MarketDataType.Tick,
            //    Time = data.Time,
            //    Symbol = symbol,
            //    Value = data.Value
            //});
            //Log(MOMPUSD.ToDetailedString());
            lock (locker)
            {
                lastUSD = mumusd;
                lastUsdDate = data.Time;
            }
            Compare();
        }

        public void OnData(Ticks data)
        {
            foreach (var item in data)
            {
                foreach (var tick in item.Value.OrderBy(v => v.Time))
                {
                    OnData(tick);
                }
            }
        }

        public void OnData(HuobiBitcoin data)
        {
            if (momcny.Current.Time < data.Time)
            {
                momcny.Update(new IndicatorDataPoint
                {
                    DataType = MarketDataType.Tick,
                    Time = data.Time,
                    Symbol = cnySymbol,
                    Value = data.Value
                });
                lock (locker2)
                {
                    lastCNY = momcny;
                    lastCnyDate = data.Time;
                }
            }
            if (mumusd.IsReady && momcny.IsReady)
            {
                Compare();
            }
        }

        private void Compare()
        {
            decimal buySwing = 2m;
            decimal sellSwing = 3m;
            if ((lastCNY - lastUSD > buySwing || lastCNY - lastUSD < -sellSwing) && lastCnyDate - lastUsdDate < new TimeSpan(0, 0, 3))
            {
                if (lastCNY - lastUSD > buySwing)
                {
                    int volume = CalculateOrderQuantity(symbol, 0.9);
                    if (volume != 0)
                    {
                        SetHoldings(symbol, 0.9);
                        Output("buy " + " diff:" + (lastCNY - lastUSD).ToString());
                    }
                }
                else if (lastCNY - lastUSD < -sellSwing)
                {

                    int volume = CalculateOrderQuantity(symbol, -0.9);
                    if (volume != 0)
                    {
                        SetHoldings(symbol, -0.9);
                        Output("sell " + " diff:" + (lastCNY - lastUSD).ToString());
                    }
                }
            }
        }

        private void Output(string title)
        {
            Log(title + ": " + Portfolio.Securities[symbol].Price.ToString() + " Trade:" + Portfolio[symbol].LastTradeProfit
                + " Total:" + Portfolio.TotalPortfolioValue);
        }


    }
}