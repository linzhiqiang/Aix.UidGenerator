using Aix.UidGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aix.UidGenerator
{


    /* 下图是默认设置 1(符号位)+41(毫秒时间戳)+10(机器位)+2(时间回拨位)+10(同一毫秒内递增序号)  
    * +------+-----------------------+----------------+-----------------+-------------------+
    * | sign |       milliseconds        |    worker  id   | time callback   |      sequence       |
    * +------+----------------------+----------------+------------------+-------------------+
    *   1bit          41bits                        10bits                  2bits                       10bits
    *   
    *   
    *   使用时 可以灵活变化 机器位和序号的长度，还可以把workid使用ip的后10位代替，再加几位进程号
    *   10位ip + 4位进程标识序号 + 2位回拨位 + 6位序号 根据需要灵活变换
    *   https://www.xiaocainiao.net/2019/07/31/29.html
     */

    /// <summary>
    /// 1+41+10+2+10
    /// 1: 第一位符号位  2：41位毫秒时间戳  3：10位workid  4:2位回拨位  5: 10位顺序号
    /// workid可以考虑 数据库(biz_name,ip,port)+本地文件实现  表里没有就注册一条返回自增id作为workid，存在使用老的id。 可以做个本地文件优化（可有可无）
    /// 每秒可以100万个，如果没有这么大的并发量 最后一位序号可以减少4到8位
    /// </summary>
    public class UIDGeneratorImpl : IUIDGenerator
    {
        long LastTimestamp = 0;//上一次的时间戳
        int TimestampBit = 41;//占的位数
        int TimestampShift;//时间戳左移的位数
        long MaxTimestamp;//时间戳对应的最大值

        long WorkId; //机器标识id
        int WorkIdBit = 10;//机器标识位数
        int WorkIdShift;//机器标识左移的位数
        long MaxWorkId;//机器标识的最大值

        long TimeCheck = 0;//回拨位的值
        int TimeCheckBit = 2;//回拨占的位数
        int TimeCheckShift;//回拨位左移的位数
        long MaxTimeCheck;//回拨位的最大值

        long Sequence;//序号值
        int SequenceBit = 10;//序号值占的位数
        int SequenceShift;//序号值左移多少位
        long MaxSequence; //SequenceBit这么多位所能表示的最大值

        static object UIDLock = new object();
        DateTime EpochDateTime = new DateTime(2021, 1, 1);

        internal UIDGeneratorImpl(UIDOptions options)
        {
            ValidOptions(options);

            EpochDateTime = options.EpochDateTime;

            WorkId = options.WorkId;
            WorkIdBit = options.WorkIdBit;
            TimeCheckBit = options.TimeCheckBit;
            SequenceBit = options.SequenceBit;

            MaxTimestamp = BitUtils.MaxVaue(TimestampBit);
            MaxWorkId = BitUtils.MaxVaue(WorkIdBit);
            MaxSequence = BitUtils.MaxVaue(SequenceBit);
            MaxTimeCheck = BitUtils.MaxVaue(TimeCheckBit);

            SequenceShift = 0;
            TimeCheckShift = SequenceBit;
            WorkIdShift = TimeCheckBit + SequenceBit;
            TimestampShift = WorkIdBit + TimeCheckBit + SequenceBit;

            if (WorkId > MaxWorkId)
            {
                throw new Exception("workdId超过最大值");
            }
        }

        private void ValidOptions(UIDOptions options)
        {
            if (options.WorkIdBit <= 0) throw new Exception("WorkIdBit不能小于等于0");
            if (options.SequenceBit <= 0) throw new Exception("SequenceBit不能小于等于0");
            if (options.WorkIdBit + options.TimeCheckBit + options.SequenceBit != 22) throw new Exception("WorkIdBit+TimeCheckBit+SequenceBit=24");
        }

        /// <summary>
        /// 获取uuid
        /// </summary>
        /// <returns></returns>
        public long GetUID()
        {
            lock (UIDLock)
            {
                return getUIDInner();
            }
        }

        public string ParseUID(long uid)
        {
            var sequence = uid & MaxSequence;  
            var workdId = (uid & (MaxWorkId << WorkIdShift)) >> WorkIdShift;
            var timestamp = uid >> TimestampShift; //(uid & (MaxTimestamp << TimestampShift)) >> TimestampShift;
            var datetime = TimeStampToDateTime(timestamp);
            return $"{uid}: workdId={workdId},timestamp={datetime.ToString("yyyy-MM-dd HH:mm:ss fff")},sequence={sequence}" ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epochDateTime"></param>
        //public void SetEpochDateTime(DateTime epochDateTime)
        //{
        //    EpochDateTime = epochDateTime;
        //}

        #region private

        private long getUIDInner()
        {
            var timeStamp = GetTimeStamp();
            if (timeStamp > LastTimestamp)
            {
                Sequence = 0;
            }
            else if (timeStamp == LastTimestamp)
            {
                Sequence++;
                if (Sequence > MaxSequence)
                {
                    // 并发太大 了，要自旋一会，等到下一毫秒
                    timeStamp = WaitNextMillisecond();
                    Sequence = 0;
                }
            }
            else if (TimeCheckBit > 0) // 时间被回拨了且设置了回拨位
            {
                //这里不考虑1毫秒内 时间回拨很多次的情况  会存在吗？
                TimeCheck++;
                //TimeCheck = (int)(TimeCheck % (MaxTimeCheck + 1));
                if (TimeCheck > MaxTimeCheck) TimeCheck = 0;
                Sequence = 0;
            }
            else //时间被回拨了 就只能报错了
            {
                throw new Exception($"Clock moved backwards. Refusing for { LastTimestamp - timeStamp}  milliseconds");
            }
            if (timeStamp > MaxTimestamp)
            {
                throw new Exception("时间戳的41位已耗尽");
            }
            this.LastTimestamp = timeStamp; //把当前时间戳保存为最后生成ID的时间戳
            long nextId = (timeStamp << TimestampShift) | (WorkId << WorkIdShift) | (TimeCheck << TimeCheckShift) | (Sequence << SequenceShift);
            return nextId;

        }

        /// <summary>
        /// 自旋等待到下一毫秒
        /// </summary>
        /// <returns></returns>
        private long WaitNextMillisecond()
        {
            long timeStamp = GetTimeStamp();
            while (timeStamp <= this.LastTimestamp)
            {
                Console.WriteLine("自旋");
                timeStamp = GetTimeStamp();
            }
            return timeStamp;
        }

        private long GetTimeStamp()
        {
            DateTime theDate = DateTime.Now;
            DateTime d1 = EpochDateTime;
            DateTime d2 = theDate.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long)ts.TotalMilliseconds;
        }

        private DateTime TimeStampToDateTime(long timestamp)
        {
            //return EpochDateTime.AddMilliseconds(timestamp);
            var date = TimeZoneInfo.ConvertTimeFromUtc(EpochDateTime, TimeZoneInfo.Local);
            return date.AddMilliseconds(timestamp);
        }

        #endregion

    }
}
