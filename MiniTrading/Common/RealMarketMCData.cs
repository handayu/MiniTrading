using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable()]

    public class RealMarketMCData
    {
        /// <summary>
        /// 前一根的：开，高，低，收，AVg,K,D
        /// </summary>
        public double[] ThreeBarAgoInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 昨一根的：开，高，低，收，AVg,K,D
        /// </summary>
        public double[] TwoBarAgoInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 当前的：开，高，低，收，AVg,K,D
        /// </summary>
        public double[] NowBarInfo
        {
            get;
            set;
        }

    }
}
