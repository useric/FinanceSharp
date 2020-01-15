﻿/*
 * All Rights reserved to Ebby Technologies LTD @ Eli Belash, 2020.
 * Original code by: 
 * 
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
using FinanceSharp.Data;
using Torch;

namespace FinanceSharp.Indicators {
    /// <summary>
    /// 	 An indicator that will always return the same value.
    /// </summary>
    /// <typeparam name="T">The type of input this indicator takes</typeparam>
    public sealed class ConstantIndicator : IndicatorBase {
        private readonly Tensor<double> _value;

        /// <summary>
        /// 	 Gets true since the ConstantIndicator is always ready to return the same value
        /// </summary>
        public override bool IsReady => true;

        /// <summary>
        /// 	 Creates a new ConstantIndicator that will always return the specified value
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="value">The constant value to be returned</param>
        public ConstantIndicator(string name, double value)
            : base(name) {
            _value = (Tensor<double>) value;

            // set this immediately so it always has the .Value property correctly set,
            // the time will be updated anytime this indicators Update method gets called.
            Current = _value;
        }

        /// <summary>
        /// 	 Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="time"></param>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override Tensor Forward(long time, Tensor<double> input) {
            return _value;
        }

        /// <summary>
        /// 	 Resets this indicator to its initial state
        /// </summary>
        public override void Reset() {
            base.Reset();

            // re-initialize the current value, constant should ALWAYS return this value
            Current = new Tensor<double>(_value);
            CurrentTime = 0;
        }
    }
}