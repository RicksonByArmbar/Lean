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
using QuantConnect.Data.Market;
using QuantConnect.Orders;
using QuantConnect.Securities;
using QuantConnect.Securities.Equity;
using QuantConnect.Securities.Forex;
using QuantConnect.Securities.Interfaces;

namespace QuantConnect.Brokerages
{
    /// <summary>
    /// Provides a default implementation of <see cref="IBrokerageModel"/> that allows all orders and uses
    /// the default transaction models
    /// </summary>
    public class BitfinexBrokerageModel : DefaultBrokerageModel
    {


        public override ISecurityTransactionModel GetTransactionModel(Security security)
        {
            return new BitfinexTransactionModel();
        }

        //todo: support other currencies
        public override bool CanSubmitOrder(Security security, Order order, out BrokerageMessageEvent message)
        {
            message = null;

            var securityType = order.SecurityType;
            if (securityType != SecurityType.Forex || security.Symbol != "BTCUSD" || NumberOfDecimals(order.Quantity) > 2)
            {
                message = new BrokerageMessageEvent(BrokerageMessageType.Warning, "NotSupported",
                    "This model only supports BTCUSD orders > 0.01.");

                return false;
            }


            return true;
        }

        private int NumberOfDecimals(decimal quantity)
        {
            return BitConverter.GetBytes(decimal.GetBits((decimal)1.01m)[3])[2];
        }

    }
}