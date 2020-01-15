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

using FinanceSharp.Data;
using FinanceSharp.Data.Rolling;
using Torch;

namespace FinanceSharp.Indicators {
    /// <summary>
    /// 	 Represents an indicator that is a ready after ingesting enough samples (# samples > period) 
    /// 	 and always returns the same value as it is given.
    /// </summary>
    public class WindowIdentity : WindowIndicator {
        /// <summary>
        /// 	 Initializes a new instance of the WindowIdentity class with the specified name and period
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period of the WindowIdentity</param>
        public WindowIdentity(string name, int period)
            : base(name, period) { }

        /// <summary>
        /// 	 Initializes a new instance of the WindowIdentity class with the default name and period
        /// </summary>
        /// <param name="period">The period of the WindowIdentity</param>
        public WindowIdentity(int period)
            : this("WIN-ID" + period, period) { }

        /// <summary>
        /// 	 Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady {
            get { return Samples >= Period; }
        }

        /// <summary>
        /// 	 Computes the next value for this indicator from the given state.
        /// </summary>
        /// <param name="window">The window of data held in this indicator</param>
        /// <param name="time"></param>
        /// <param name="input">The input value to this indicator on this time step</param>
        /// <returns>A new value for this indicator</returns>
        protected override Tensor<double> Forward(IReadOnlyWindow<Tensor<double>> window, long time, Tensor<double> input) {
            return input.Value;
        }
    }
}