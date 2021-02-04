using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.UidGenerator
{
    /// <summary>
    /// uid生成器接口 默认设置 1(符号位)+41(毫秒时间戳)+10(机器位)+2(时间回拨位)+10(同一毫秒内递增序号)   
    /// 可以灵活调整机器位和递增序号的位数
    /// </summary>
    public interface IUIDGenerator
    {
        /// <summary>
        /// 获取一个新的uid
        /// </summary>
        /// <returns></returns>
        long GetUID();

        /// <summary>
        /// 解析id
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        string ParseUID(long uid);
    }
}
