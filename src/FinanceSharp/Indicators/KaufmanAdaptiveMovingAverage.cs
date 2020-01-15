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
using FinanceSharp.Data.Rolling;
using Torch;

namespace FinanceSharp.Indicators {
    /// <summary>
    /// 	 This indicator computes the Kaufman Adaptive Moving Average (KAMA).
    /// 	 The Kaufman Adaptive Moving Average is calculated as explained here:
    /// 	 http://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:kaufman_s_adaptive_moving_average
    /// </summary>
    public class KaufmanAdaptiveMovingAverage : WindowIndicator {
        private const double _constMax = 2d / (3.0d + 1d);
        private const double _constDiff = 2d / (2d + 1d) - _constMax;

        private double _sumRoc1;
        private double _periodRoc;
        private double _prevKama;
        private double _trailingValue;

        /// <summary>
        /// 	 Initializes a new instance of the <see cref="KaufmanAdaptiveMovingAverage"/> class using the specified name and period.
        /// </summary> 
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The period of the KAMA</param>
        public KaufmanAdaptiveMovingAverage(string name, int period)
            : base(name, period + 1) { }

        /// <summary>
        /// 	 Initializes a new instance of the <see cref="KaufmanAdaptiveMovingAverage"/> class using the specified period.
        /// </summary> 
        /// <param name="period">The period of the KAMA</param>
        public KaufmanAdaptiveMovingAverage(int period)
            : this($"KAMA({period})", period) { }

        /// <summary>
        /// 	 Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady => Samples >= Period;

        /// <summary>
        /// 	 Required period, in data points, for the indicator to be ready and fully initialized.
        /// </summary>
        public int WarmUpPeriod => Period;

        /// <summary>
        /// 	 Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="window">The window for the input history</param>
        /// <param name="time"></param>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override Tensor<double> Forward(IReadOnlyWindow<Tensor<double>> window, long time, Tensor<double> input) {
            if (Samples < Period) {
                if (Samples > 1) {
                    _sumRoc1 += Math.Abs(input.Value - window[1].Value);
                }

                return input.Value;
            }

            if (Samples == Period) {
                _sumRoc1 += Math.Abs(input.Value - window[1].Value);

                // At this point sumROC1 represents the 
                // summation of the 1-day price difference
                // over the (optInTimePeriod-1)

                // Calculate the first KAMA
                // The yesterday price is used here as the previous KAMA.
                _prevKama = window[1].Value;
            }

            var newTrailingValue = window[Period - 1];
            _periodRoc = input.Value - newTrailingValue;

            if (Samples > Period) {
                // Adjust sumROC1:
                // - Remove trailing ROC1 
                // - Add new ROC1
                _sumRoc1 -= Math.Abs(_trailingValue - newTrailingValue);
                _sumRoc1 += Math.Abs(input.Value - window[1].Value);
            }

            // Save the trailing value.
            _trailingValue = newTrailingValue;

            // Calculate the efficiency ratio
            var efficiencyRatio = (_sumRoc1 <= _periodRoc) || _sumRoc1 == 0 ? 1d : Math.Abs(_periodRoc / _sumRoc1);

            // Calculate the smoothing constant
            var smoothingConstant = efficiencyRatio * _constDiff + _constMax;
            smoothingConstant *= smoothingConstant;

            // Calculate the KAMA like an EMA, using the
            // smoothing constant as the adaptive factor.
            _prevKama = (input.Value - _prevKama) * smoothingConstant + _prevKama;

            return _prevKama;
        }

        /// <summary>
        /// 	 Resets this indicator to its initial state
        /// </summary>
        public override void Reset() {
            _sumRoc1 = 0;
            _periodRoc = 0;
            _prevKama = 0;
            _trailingValue = 0;
            base.Reset();
        }
    }
}