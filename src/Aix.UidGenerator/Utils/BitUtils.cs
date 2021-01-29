using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.UidGenerator.Utils
{
    public static class BitUtils
    {
        /// <summary>
        /// n位二进制最大值
        /// </summary>
        /// <param name="bitCount"></param>
        /// <returns></returns>
        public static long MaxVaue(int bitCount)
        {
            if (bitCount <= 0) return 0;

            long count = 1L << (bitCount-1);
            return count * 2 - 1;
        }
    }
}
