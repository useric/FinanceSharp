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
using QuantConnect.Data;
using QuantConnect.Data.Market;

namespace QuantConnect {
    /// <summary>
    /// 	 Provides static properties to be used as selectors with the indicator system
    /// </summary>
    public static class Field {
        /// <summary>
        /// 	 Gets a selector that selects the Open value
        /// </summary>
        public static Func<IBaseData, double> Open {
            get { return BaseDataBarPropertyOrValue(x => x.Open); }
        }

        /// <summary>
        /// 	 Gets a selector that selects the High value
        /// </summary>
        public static Func<IBaseData, double> High {
            get { return BaseDataBarPropertyOrValue(x => x.High); }
        }

        /// <summary>
        /// 	 Gets a selector that selects the Low value
        /// </summary>
        public static Func<IBaseData, double> Low {
            get { return BaseDataBarPropertyOrValue(x => x.Low); }
        }

        /// <summary>
        /// 	 Gets a selector that selects the Close value
        /// </summary>
        public static Func<IBaseData, double> Close {
            get { return x => x.Value; }
        }

        /// <summary>
        /// 	 Defines an average price that is equal to (O + H + L + C) / 4
        /// </summary>
        public static Func<IBaseData, double> Average {
            get { return BaseDataBarPropertyOrValue(x => (x.Open + x.High + x.Low + x.Close) / 4d); }
        }

        /// <summary>
        /// 	 Defines an average price that is equal to (H + L) / 2
        /// </summary>
        public static Func<IBaseData, double> Median {
            get { return BaseDataBarPropertyOrValue(x => (x.High + x.Low) / 2d); }
        }

        /// <summary>
        /// 	 Defines an average price that is equal to (H + L + C) / 3
        /// </summary>
        public static Func<IBaseData, double> Typical {
            get { return BaseDataBarPropertyOrValue(x => (x.High + x.Low + x.Close) / 3d); }
        }

        /// <summary>
        /// 	 Defines an average price that is equal to (H + L + 2*C) / 4
        /// </summary>
        public static Func<IBaseData, double> Weighted {
            get { return BaseDataBarPropertyOrValue(x => (x.High + x.Low + 2 * x.Close) / 4d); }
        }

        /// <summary>
        /// 	 Defines an average price that is equal to (2*O + H + L + 3*C)/7
        /// </summary>
        public static Func<IBaseData, double> SevenBar {
            get { return BaseDataBarPropertyOrValue(x => (2 * x.Open + x.High + x.Low + 3 * x.Close) / 7d); }
        }

        /// <summary>
        /// 	 Gets a selector that selectors the Volume value
        /// </summary>
        public static Func<IBaseData, double> Volume {
            get { return BaseDataBarPropertyOrValue(x => x is TradeBar ? ((TradeBar) x).Volume : 0d, x => 0d); }
        }

        private static Func<IBaseData, double> BaseDataBarPropertyOrValue(Func<IBaseDataBar, double> selector, Func<IBaseData, double> defaultSelector = null) {
            return x => {
                var bar = x as IBaseDataBar;
                if (bar != null) {
                    return selector(bar);
                }

                defaultSelector = defaultSelector ?? (data => data.Value);
                return defaultSelector(x);
            };
        }
    }
}