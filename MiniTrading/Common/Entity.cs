using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Entity
    {
        /// <summary>
        /// 发布行情订阅的频道
        /// </summary>
        public static readonly string CHANNEL = "RealMarketData";

        /// <summary>
        /// 发布交易订阅频道
        /// </summary>
        public static readonly string CHANNELTRADE = "Trade";

    }
}
