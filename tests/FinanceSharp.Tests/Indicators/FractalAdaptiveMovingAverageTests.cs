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

using System;
using NUnit.Framework;
using FinanceSharp.Indicators;
using FinanceSharp.Data;


namespace FinanceSharp.Tests.Indicators {
    [TestFixture]
    public class FractalAdaptiveMovingAverageTests : CommonIndicatorTests<TradeBarValue> {
        protected override IndicatorBase CreateIndicator() {
            return new FractalAdaptiveMovingAverage(16);
        }

        protected override string TestFileName => "frama.txt";

        protected override string TestColumnName => "Filt";

        protected override Action<IndicatorBase, double> Assertion =>
            (indicator, expected) =>
                Assert.AreEqual(expected, (double) indicator.Current.Value, 0.006);

        [Test]
        public void ResetsProperly() {
            var frama = new FractalAdaptiveMovingAverage(6);

            foreach (var data in TestHelper.GetDataStream(7)) {
                frama.Update(DateTime.UtcNow, new TradeBarValue {High = data.Value.Value, Low = data.Value.Value});
            }

            Assert.IsTrue(frama.IsReady);
            Assert.AreNotEqual(0d, frama.Current.Value);
            Assert.AreNotEqual(0, frama.Samples);

            frama.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(frama);
        }
    }
}