﻿/*
 * All Rights reserved to Ebby Technologies LTD @ Eli Belash, 2020.
 * Original code by QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
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

namespace FinanceSharp.Indicators {
    /// <summary>
    /// 	 Represents an indicator capable of tracking the sum for the given period
    /// </summary>
    public class Sum : WindowIndicator {
        /// <summary>
        /// 	 The sum for the given period
        /// </summary>
        private double _sum;

        /// <summary>
        /// 	 Required period, in data points, for the indicator to be ready and fully initialized.
        /// </summary>
        public override int WarmUpPeriod => Period;

        /// <summary>
        /// 	 Resets this indicator to its initial state
        /// </summary>
        public override void Reset() {
            _sum = 0.0d;
            base.Reset();
        }

        /// <summary>
        /// 	 Initializes a new instance of the Sum class with the specified name and period
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period of the SMA</param>
        public Sum(string name, int period)
            : base(name, period) { }

        /// <summary>
        /// 	 Initializes a new instance of the Sum class with the default name and period
        /// </summary>
        /// <param name="period">The period of the SMA</param>
        public Sum(int period)
            : this($"SUM({period})", period) { }

        /// <summary>
        /// 	 Computes the next value for this indicator from the given state.
        /// </summary>
        /// <param name="timeWindow"></param>
        /// <param name="window">The window of data held in this indicator</param>
        /// <param name="time"></param>
        /// <param name="input">The input value to this indicator on this time step</param>
        /// <returns>A new value for this indicator</returns>
        protected override DoubleArray Forward(IReadOnlyWindow<long> timeWindow, IReadOnlyWindow<DoubleArray> window, long time, DoubleArray input) {
            _sum += input.Value;
            if (window.Samples > window.Size) {
                _sum -= window.MostRecentlyRemoved.Value;
            }

            return _sum;
        }
    }
}