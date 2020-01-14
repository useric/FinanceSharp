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
 *
*/

using System;
using QuantConnect.Data.Market;

namespace QuantConnect.Indicators.CandlestickPatterns {
    /// <summary>
    /// 	 Evening Doji Star candlestick pattern
    /// </summary>
    /// <remarks>
    /// 	 Must have:
    /// - first candle: long white real body
    /// - second candle: doji gapping up
    /// - third candle: black real body that moves well within the first candle's real body
    /// 	 The meaning of "doji" and "long" is specified with SetCandleSettings
    /// 	 The meaning of "moves well within" is specified with penetration and "moves" should mean the real body should
    /// 	 not be short ("short" is specified with SetCandleSettings) - Greg Morris wants it to be long, someone else want
    /// 	 it to be relatively long
    /// 	 The returned value is negative(-1): evening star is always bearish;
    /// 	 The user should consider that an evening star is significant when it appears in an uptrend,
    /// 	 while this function does not consider the trend
    /// </remarks>
    public class EveningDojiStar : CandlestickPattern {
        private readonly double _penetration;

        private readonly int _bodyDojiAveragePeriod;
        private readonly int _bodyLongAveragePeriod;
        private readonly int _bodyShortAveragePeriod;

        private double _bodyDojiPeriodTotal;
        private double _bodyLongPeriodTotal;
        private double _bodyShortPeriodTotal;

        /// <summary>
        /// 	 Initializes a new instance of the <see cref="EveningDojiStar"/> class using the specified name.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        public EveningDojiStar(string name, double penetration = 0.3d)
            : base(name, Math.Max(Math.Max(CandleSettings.Get(CandleSettingType.BodyDoji).AveragePeriod, CandleSettings.Get(CandleSettingType.BodyLong).AveragePeriod),
                             CandleSettings.Get(CandleSettingType.BodyShort).AveragePeriod) + 2 + 1) {
            _penetration = penetration;

            _bodyDojiAveragePeriod = CandleSettings.Get(CandleSettingType.BodyDoji).AveragePeriod;
            _bodyLongAveragePeriod = CandleSettings.Get(CandleSettingType.BodyLong).AveragePeriod;
            _bodyShortAveragePeriod = CandleSettings.Get(CandleSettingType.BodyShort).AveragePeriod;
        }

        /// <summary>
        /// 	 Initializes a new instance of the <see cref="EveningDojiStar"/> class.
        /// </summary>
        /// <param name="penetration">Percentage of penetration of a candle within another candle</param>
        public EveningDojiStar(double penetration)
            : this("EVENINGDOJISTAR", penetration) { }

        /// <summary>
        /// 	 Initializes a new instance of the <see cref="EveningDojiStar"/> class.
        /// </summary>
        public EveningDojiStar()
            : this("EVENINGDOJISTAR") { }

        /// <summary>
        /// 	 Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady {
            get { return Samples >= Period; }
        }

        /// <summary>
        /// 	 Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="window">The window of data held in this indicator</param>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override double ComputeNextValue(IReadOnlyWindow<IBaseDataBar> window, IBaseDataBar input) {
            if (!IsReady) {
                if (Samples >= Period - _bodyLongAveragePeriod - 2 && Samples < Period - 2) {
                    _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, input);
                }

                if (Samples >= Period - _bodyDojiAveragePeriod - 1 && Samples < Period - 1) {
                    _bodyDojiPeriodTotal += GetCandleRange(CandleSettingType.BodyDoji, input);
                }

                if (Samples >= Period - _bodyShortAveragePeriod) {
                    _bodyShortPeriodTotal += GetCandleRange(CandleSettingType.BodyShort, input);
                }

                return 0d;
            }

            double value;
            if (
                // 1st: long
                GetRealBody(window[2]) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal, window[2]) &&
                //      white
                GetCandleColor(window[2]) == CandleColor.White &&
                // 2nd: doji
                GetRealBody(window[1]) <= GetCandleAverage(CandleSettingType.BodyDoji, _bodyDojiPeriodTotal, window[1]) &&
                //      gapping up
                GetRealBodyGapUp(window[1], window[2]) &&
                // 3rd: longer than short
                GetRealBody(input) > GetCandleAverage(CandleSettingType.BodyShort, _bodyShortPeriodTotal, input) &&
                //      black real body
                GetCandleColor(input) == CandleColor.Black &&
                //      closing well within 1st rb
                input.Close < window[2].Close - GetRealBody(window[2]) * _penetration
            )
                value = -1d;
            else
                value = 0d;

            // add the current range and subtract the first range: this is done after the pattern recognition 
            // when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)

            _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, window[2]) -
                                    GetCandleRange(CandleSettingType.BodyLong, window[_bodyLongAveragePeriod + 2]);

            _bodyDojiPeriodTotal += GetCandleRange(CandleSettingType.BodyDoji, window[1]) -
                                    GetCandleRange(CandleSettingType.BodyDoji, window[_bodyDojiAveragePeriod + 1]);

            _bodyShortPeriodTotal += GetCandleRange(CandleSettingType.BodyShort, input) -
                                     GetCandleRange(CandleSettingType.BodyShort, window[_bodyShortAveragePeriod]);

            return value;
        }

        /// <summary>
        /// 	 Resets this indicator to its initial state
        /// </summary>
        public override void Reset() {
            _bodyLongPeriodTotal = 0;
            _bodyDojiPeriodTotal = 0;
            _bodyShortPeriodTotal = 0;
            base.Reset();
        }
    }
}