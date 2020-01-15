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
    /// 	 This indicator computes the Chande Momentum Oscillator (CMO).
    /// 	 CMO calculation is mostly identical to RSI.
    /// 	 The only difference is in the last step of calculation:
    /// 	 RSI = gain / (gain+loss)
    /// 	 CMO = (gain-loss) / (gain+loss)
    /// </summary>
    public class ChandeMomentumOscillator : WindowIndicator {
        private double _prevValue;
        private double _prevGain;
        private double _prevLoss;

        /// <summary>
        /// 	 Initializes a new instance of the <see cref="ChandeMomentumOscillator"/> class using the specified period.
        /// </summary> 
        /// <param name="period">The period of the indicator</param>
        public ChandeMomentumOscillator(int period)
            : this($"CMO({period})", period) { }

        /// <summary>
        /// 	 Initializes a new instance of the <see cref="ChandeMomentumOscillator"/> class using the specified name and period.
        /// </summary> 
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period of the indicator</param>
        public ChandeMomentumOscillator(string name, int period)
            : base(name, period) { }

        /// <summary>
        /// 	 Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady => Samples > Period;

        /// <summary>
        /// 	 Required period, in data points, for the indicator to be ready and fully initialized.
        /// </summary>
        public int WarmUpPeriod => 1 + Period;

        /// <summary>
        /// 	 Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="window">The window for the input history</param>
        /// <param name="time"></param>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override Tensor<double> Forward(IReadOnlyWindow<Tensor<double>> window, long time, Tensor<double> input) {
            if (Samples == 1) {
                _prevValue = input;
                return Constants.Zero;
            }

            var difference = input.Value - _prevValue;

            _prevValue = input.Value;

            if (Samples > Period + 1) {
                _prevLoss *= (Period - 1);
                _prevGain *= (Period - 1);
            }

            if (difference < 0)
                _prevLoss -= difference;
            else
                _prevGain += difference;

            if (!IsReady)
                return Constants.Zero;

            _prevLoss /= Period;
            _prevGain /= Period;

            var sum = _prevGain + _prevLoss;
            return sum != 0 ? 10.0d * ((_prevGain - _prevLoss) / sum) : Constants.Zero;
        }

        /// <summary>
        /// 	 Resets this indicator to its initial state
        /// </summary>
        public override void Reset() {
            _prevValue = 0;
            _prevGain = 0;
            _prevLoss = 0;
            base.Reset();
        }
    }
}