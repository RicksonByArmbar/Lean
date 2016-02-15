using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantConnect.Brokerages.Bitfinex;
using NUnit.Framework;
using WebSocketSharp;
using Microsoft.QualityTools.Testing.Fakes;
using Newtonsoft.Json;
using System.Reflection;
namespace QuantConnect.Brokerages.Bitfinex.Tests
{

    //no DI/IOC means we get this
    [TestFixture()]
    public class BitfinexWebsocketsBrokerageTests
    {

        BitfinexWebsocketsBrokerage unit = new BitfinexWebsocketsBrokerage();

        [TestFixtureSetUp]
        public void Initialize()
        {


        }

        [Test()]
        public void BitfinexWebsocketsBrokerageTest()
        {
        }

        [Test()]
        public void SubscribeTest()
        {

        }

        [Test()]
        public void UnsubscribeTest()
        {

        }

        [Test()]
        public void IsConnectedTest()
        {
            using (ShimsContext.Create())
            {
                WebSocketSharp.Fakes.ShimWebSocket.AllInstances.IsAliveGet = (arg) => true;
                Assert.IsTrue(unit.IsConnected);

                WebSocketSharp.Fakes.ShimWebSocket.AllInstances.IsAliveGet = (arg) => false;
                Assert.IsFalse(unit.IsConnected);
            }
        }

        [Test()]
        public void ConnectTest()
        {
            using (ShimsContext.Create())
            {
                bool actual = false;
                bool actual2 = false;
                WebSocketSharp.Fakes.ShimWebSocket.AllInstances.Connect = (arg) => { actual = true; };
                WebSocketSharp.Fakes.ShimWebSocket.AllInstances.OnMessageAddEventHandlerOfMessageEventArgs = (arg, arg2) => { actual2 = true; };

                unit.Connect();
                Assert.IsTrue(actual);
                Assert.IsTrue(actual2);
            }
        }

        [Test()]
        public void DisconnectTest()
        {

        }

        [Test()]
        public void DisposeTest()
        {

        }

        [Test()]
        public void OnMessageTradeTest()
        {
            using (ShimsContext.Create())
            {
                WebSocketSharp.Fakes.ShimMessageEventArgs.AllInstances.DataGet = (instance) => "[0,\"tu\", [\"abc123\",\"1\",\"BTCUSD\",\"1453989092 \",\"2\",\"3\",\"4\",\"<ORD_TYPE>\",\"5\",\"6\",\"USD\"]]";

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                System.Globalization.CultureInfo culture = null;
                MessageEventArgs args = (MessageEventArgs)Activator.CreateInstance(typeof(MessageEventArgs), flags, null, new object[] {new Opcode(), new byte[0]}, culture);

                unit.OnMessage(null, args);

                unit.OrderStatusChanged += (s, e) =>
                {
                    Assert.AreEqual("BTCUSD", e.Symbol);
                    Assert.AreEqual(3, e.FillQuantity);
                    Assert.AreEqual(4, e.FillPrice);
                    Assert.AreEqual(6, e.OrderFee);
                };
            }

        }


        [Test()]
        public void OnMessageTickerTest()
        {
            using (ShimsContext.Create())
            {
                WebSocketSharp.Fakes.ShimMessageEventArgs.AllInstances.DataGet = (instance) => "{\"event\":\"subscribed\",\"channel\":\"ticker\",\"chanId\":\"0\"}";

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                System.Globalization.CultureInfo culture = null;
                MessageEventArgs args = (MessageEventArgs)Activator.CreateInstance(typeof(MessageEventArgs), flags, null, new object[] { new Opcode(), new byte[0] }, culture);

                unit.OnMessage(null, args);

                WebSocketSharp.Fakes.ShimMessageEventArgs.AllInstances.DataGet = (instance) => "[\"0\",\"<BID>\",\"<BID_SIZE>\",\"<ASK>\",\"<ASK_SIZE>\",\"<DAILY_CHANGE>\",\"<DAILY_CHANGE_PERC>\",\"1\",\"<VOLUME>\",\"<HIGH>\",\"<LOW>\"]";

                unit.OnMessage(null, args);

                var actual = unit.GetNextTicks().First();
                Assert.AreEqual("BTCUSD", actual.Symbol.Value);
                Assert.AreEqual(1m, actual.Price);
                Assert.AreEqual(1m, actual.Value);


            }

        }

    }
}
