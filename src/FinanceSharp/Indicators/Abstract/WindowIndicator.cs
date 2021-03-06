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

using System.Collections.Generic;

namespace FinanceSharp.Indicators {
    /// <summary>
    /// 	 Represents an indicator that acts on a rolling window of data in a <see cref="List{T}"/>
    /// </summary>
    public abstract class WindowIndicator : IndicatorBase {
        // a window of data over a certain look back period
        private readonly RollingWindow<DoubleArray> _window;
        private readonly RollingWindow<long> _windowTimes; 

        /// <summary>
        /// 	 Gets the period of this window indicator
        /// </summary>
        public int Period => _window.Size;

        /// <summary>
        /// 	 Initializes a new instance of the WindowIndicator class
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The number of data points to hold in the window</param>
        protected WindowIndicator(string name, int period)
            : base(name) {
            _window = new RollingWindow<DoubleArray>(period);
            _windowTimes = new RollingWindow<long>(period);
        }

        /// <summary>
        /// 	 Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady => _window.IsReady;

        /// <summary>
        /// 	 Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="time"></param>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override DoubleArray Forward(long time, DoubleArray input) {
            _window.Add(input);
            _windowTimes.Add(time);
            return Forward(_windowTimes, _window, time, input);
        }

        /// <summary>
        ///     Resets this indicator to its initial state
        /// </summary>
        public override void Reset() {
            _window.Reset();
            _windowTimes.Reset();
            base.Reset();
        }

        /// <summary>
        /// 	 Computes the next value for this indicator from the given state.
        /// </summary>
        /// <param name="timeWindow"></param>
        /// <param name="window">The window of data held in this indicator</param>
        /// <param name="time"></param>
        /// <param name="input"></param>
        /// <returns>A new value for this indicator</returns>
        protected abstract DoubleArray Forward(IReadOnlyWindow<long> timeWindow, IReadOnlyWindow<DoubleArray> window, long time, DoubleArray input);
    }
}