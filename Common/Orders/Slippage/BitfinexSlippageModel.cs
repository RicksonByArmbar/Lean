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
using QuantConnect.Data.Market;
using QuantConnect.Securities;

namespace QuantConnect.Orders.Slippage
{
    /// <summary>
    /// A slippage model that uses half of the bid/ask spread if available,
    /// if not available, zero slippage is assumed.
    /// </summary>
    public class BitfinexSlippageModel : SpreadSlippageModel
    {
        /// <summary>
        /// Slippage Model. Return a decimal cash slippage approximation on the order.
        /// </summary>
        public override decimal GetSlippageApproximation(Security asset, Order order)
        {
            if (order.Price != 0)
            {
                return base.GetSlippageApproximation(asset, order);
            }
            
            var lastData = asset.GetLastData();
            var lastTick = lastData as Tick;

            // if we have tick data use the spread
            if (lastTick != null)
            {
                if (asset.Price != 0)
                {
                    if (order.Direction == OrderDirection.Buy)
                    {
                        //We're buying, assume slip to Asking Price.
                        return Math.Abs(asset.Price - lastTick.AskPrice);
                    }
                    if (order.Direction == OrderDirection.Sell)
                    {
                        //We're selling, assume slip to the bid price.
                        return Math.Abs(asset.Price - lastTick.BidPrice);
                    }
                }
                else
                {
                    return (lastTick.AskPrice - lastTick.BidPrice) / 2;                
                }
            }

            return 0;                
            
        }
    }
}