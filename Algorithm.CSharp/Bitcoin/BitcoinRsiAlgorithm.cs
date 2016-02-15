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


    /// <summary>
    /// Sample Bitcoin Trading Algo. Not tuned for live trading
    /// </summary>
    public partial class BitcoinRsiAlgorithm : QCAlgorithm
    {
        string symbol = "BTCUSD";
        RelativeStrengthIndex rsi;

        public override void Initialize()
        {

            SetStartDate(2016, 1, 1);
            SetEndDate(2016, 2, 1);
            SetBrokerageModel(BrokerageName.Bitfinex, AccountType.Margin);
            SetTimeZone(DateTimeZone.Utc);

            AddSecurity(SecurityType.Forex, symbol, Resolution.Tick, Market.Bitcoin.ToString(), false, 3.3m, false);
            rsi = RSI(symbol, 12, MovingAverageType.Exponential, Resolution.Hour);

            SetCash("USD", 1000, 1m);

        }

        private void Analyse(DateTime time, decimal price)
        {
            if (rsi.IsReady && !this.IsWarmingUp)
            {

                if (this.LiveMode)
                {
                    Log("rsi:" + (Math.Round(rsi.Current.Value, 2)).ToString());
                }
                Long();
                Short();
            }
        }

        public void OnData(TradeBars data)
        {
            Analyse(data.Values.First().Time, data.Values.First().Price);
        }

        public void OnData(Tick data)
        {
            Analyse(data.Time, data.Price);
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

        ///<summary>
        /// Scan for an entry signal to invest.
        ///</summary>
        private void Long()
        {
            System.Threading.Thread.Sleep(1);
            if (!Portfolio[symbol].IsLong && rsi.Current.Value > 5 && rsi.Current.Value < 30)
            {
                int quantity = CalculateOrderQuantity(symbol, 3m);
                if (quantity > 0)
                {
                    SetHoldings(symbol, 3.0m);
                    //maker fee
                    //LimitOrder(symbol, quantity, Portfolio[symbol].Price - 0.1m);
                }
                Output("Long");
            }

        }

        private void Short()
        {
            System.Threading.Thread.Sleep(1);
            if (!Portfolio[symbol].IsShort && rsi.Current.Value > 70)
            {
                SetHoldings(symbol, -3.0m);
                Output("Short");
            }
        }

        private void Output(string title)
        {
            Log(title + ":" + Portfolio.Securities[symbol].Price.ToString() + " rsi:" + Math.Round(rsi.Current.Value, 0) + " Total:" + Portfolio.TotalPortfolioValue);
        }

    }
}