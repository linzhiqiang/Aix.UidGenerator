using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.UidGenerator
{
    /// <summary>
    /// 
    /// </summary>
  public class UIDOptions
    {
        /// <summary>
        /// workid的值
        /// </summary>
        public int WorkId { get; set; }

        /// <summary>
        /// workid的位数 默认10位
        /// </summary>
        public int WorkIdBit { get; set; } = 10;

        /// <summary>
        /// 时间回拨位数 默认2位 不需要时设置为0
        /// </summary>
        public int TimeCheckBit { get; set; } = 2;

        /// <summary>
        /// 序号的位数 默认10位
        /// </summary>
        public int SequenceBit { get; set; } = 10;

        /// <summary>
        /// 时间戳的开始时间
        /// </summary>
        public DateTime EpochDateTime { get; set; } = new DateTime(2021, 1, 27);
    }
}
