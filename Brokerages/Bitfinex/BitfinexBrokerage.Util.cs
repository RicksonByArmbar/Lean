﻿using QuantConnect.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TradingApi.ModelObjects.Bitfinex.Json;

namespace QuantConnect.Brokerages.Bitfinex
{

    //todo: margin orders
    public partial class BitfinexBrokerage
    {

        #region Declarations
        private class OrderTypeMap
        {
            public string BitfinexOrderType { get; set; }
            public string Wallet { get; set; }
            public OrderType OrderType { get; set; }
        }

        enum WalletType
        {
            exchange,
            trading
        }

        //todo: trailing stop support
        private static List<OrderTypeMap> _orderTypeMap = new List<OrderTypeMap>
        {
            { new OrderTypeMap { BitfinexOrderType = "exchange market", Wallet = WalletType.exchange.ToString(), OrderType = OrderType.Market } },
            { new OrderTypeMap { BitfinexOrderType = "exchange limit", Wallet = WalletType.exchange.ToString(), OrderType = OrderType.Limit } },
            { new OrderTypeMap { BitfinexOrderType = "exchange stop", Wallet = WalletType.exchange.ToString(), OrderType = OrderType.StopMarket } },
            //{ new OrderTypeMap { BitfinexOrderType = "exchange trailing stop", Wallet = WalletType.exchange.ToString(), OrderType = OrderType.StopLimit } },

            { new OrderTypeMap { BitfinexOrderType = "market", Wallet = WalletType.trading.ToString(), OrderType = OrderType.Market } },
            { new OrderTypeMap { BitfinexOrderType = "limit", Wallet = WalletType.trading.ToString(), OrderType = OrderType.Limit } },
            { new OrderTypeMap { BitfinexOrderType = "stop", Wallet = WalletType.trading.ToString(), OrderType = OrderType.StopMarket } },
            //{ new OrderTypeMap { BitfinexOrderType = "trailing stop", Wallet = WalletType.trading.ToString(), OrderType = OrderType.StopLimit } },
        };
        #endregion

        public static OrderStatus MapOrderStatus(BitfinexOrderStatusResponse response)
        {
            decimal remainingAmount;
            decimal executedAmount;
            if (response.IsCancelled)
            {
                return OrderStatus.Canceled;
            }
            else if (decimal.TryParse(response.RemainingAmount, out remainingAmount) && remainingAmount > 0
                && decimal.TryParse(response.ExecutedAmount, out executedAmount) && executedAmount > 0)
            {
                return OrderStatus.PartiallyFilled;
            }
            else if (response.IsLive)
            {
                return OrderStatus.Submitted;
            }

            return OrderStatus.Invalid;
        }

        public string MapOrderType(OrderType orderType)
        {
            var result = _orderTypeMap.Where(o => o.Wallet == _wallet && o.OrderType == orderType);

            if (result != null & result.Count() == 1)
            {
                return result.Single().BitfinexOrderType;
            }

            throw new Exception("Order type not supported: " + orderType.ToString());
        }

        public OrderType MapOrderType(string orderType)
        {
            var result = _orderTypeMap.Where(o => o.Wallet == _wallet && o.BitfinexOrderType == orderType);

            if (result != null & result.Count() == 1)
            {
                return result.Single().OrderType;
            }

            throw new Exception("Order type not supported: " + orderType.ToString());
        }

        public static OrderStatus MapOrderStatus(TradeMessage msg)
        {
            if (msg.FEE > 0)
            {
                //todo: maybe still partially filled?
                return OrderStatus.Filled;
            }
            else
            {
                return OrderStatus.PartiallyFilled;
            }

            return OrderStatus.Invalid;
        }

        protected string GetHexHashSignature(string payload, string apiSecret)
        {
            HMACSHA384 hmac = new HMACSHA384(Encoding.UTF8.GetBytes(apiSecret));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
