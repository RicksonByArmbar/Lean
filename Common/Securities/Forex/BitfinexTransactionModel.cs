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

using QuantConnect.Orders;
using QuantConnect.Orders.Fees;
using QuantConnect.Orders.Fills;
using QuantConnect.Orders.Slippage;
using QuantConnect.Securities.Interfaces;

namespace QuantConnect.Securities.Forex
{
    /// <summary>
    /// Forex Transaction Model Class: Specific transaction fill models for FOREX orders
    /// </summary>
    /// <seealso cref="SecurityTransactionModel"/>
    /// <seealso cref="ISecurityTransactionModel"/>
    public class BitfinexTransactionModel : SecurityTransactionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForexTransactionModel"/> class
        /// </summary>
        /// <param name="monthlyTradeAmountInUSDollars">The monthly dollar volume traded</param>
        public BitfinexTransactionModel()
            : base(new ImmediateFillModel(), new BitfinexFeeModel(), new BitfinexSlippageModel())
        {
        }
  

    }

}