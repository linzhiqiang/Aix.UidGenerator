using Aix.UidGenerator.Utils;
using System;
using System.Threading;

namespace Aix.UidGenerator
{
    /// <summary>
    /// 1+29+15+2+16
    /// 1: 第一位符号位  2：29位秒(十几年吧)  3：15位序号  4:2位回拨位  5: 16位ip（后2位数字） 可以变为 10位ip+6位
    /// https://www.xiaocainiao.net/2019/07/31/29.html
    /// </summary>
    public class IPUIDGenerator : IUIDGenerator
    {
        long LastTimestamp = 0;
        int TimestampBit = 29;//占的位数
        int TimestampShift;
        long MaxTimestamp;

        long Sequence;
        int SequenceBit = 15;//占的位数
        int SequenceShift;//左移多少位
        long MaxSequence; //SequenceBit这么多位所能表示的最大值

        long TimeCheck = 0;
        int TimeCheckBit = 2;
        int TimeCheckShift;
        long MaxTimeCheck;

        long IP = 0;
        int IpBit = 16;
        int IpShift = 0;
        long MaxIp;

        static object UIDLock = new object();
        DateTime EpochDateTime = new DateTime(2021, 1, 1);

        private static IUIDGenerator Isntance;
        public static IUIDGenerator Create(IPUIDOptions options)
        {
            lock (UIDLock)
            {
                if (Isntance == null)
                {
                    Isntance = new IPUIDGenerator(options);
                }
            }
            return Isntance;
        }

        private IPUIDGenerator(IPUIDOptions options)
        {
            EpochDateTime = options.EpochDateTime;

            MaxTimestamp = BitUtils.MaxVaue(TimestampBit);
            MaxSequence = BitUtils.MaxVaue(SequenceBit);
            MaxTimeCheck = BitUtils.MaxVaue(TimeCheckBit);
            MaxIp = BitUtils.MaxVaue(IpBit);
            IP = IPUtils.IPToInt() & 0x0000ffff; //取后2位
            IpShift = 0;
            TimeCheckShift = IpBit;
            SequenceShift = TimeCheckBit + IpBit;
            TimestampShift = SequenceBit + TimeCheckBit + IpBit;

        }

        //public void SetStartTime(DateTime epochDateTime)
        //{
        //    EpochDateTime = epochDateTime;
        //}

        public long GetUID()
        {
            lock (UIDLock)
            {
                return getUIDInner();
            }
        }

        public string ParseUID(long uid)
        {
            var ip = uid & MaxIp;
            var sequence = (uid & (MaxSequence << SequenceShift)) >> SequenceShift;
            var timestamp = uid >> TimestampShift;
            var datetime = TimeStampToDateTime(timestamp);
            return $"{uid}: timestamp={datetime.ToString("yyyy-MM-dd HH:mm:ss fff")},sequence={sequence},ip={ip >> 8}.{ip & 0x00ff}";
        }

        #region  private 
        private long getUIDInner()
        {
            var timeStamp = GetTimeStampForSecond();
            if (timeStamp > LastTimestamp)
            {
                Sequence = 0;
            }
            else if (timeStamp == LastTimestamp)
            {
                Sequence++;
                if (Sequence > MaxSequence)
                {
                    // 并发太大 了，要自旋一会，等到下一秒
                    timeStamp = WaitNextSecond();
                    Sequence = 0;
                }
            }
            else //不可能吧 时间被回拨了
            {
                TimeCheck++;
                //TimeCheck = (int)(TimeCheck % (MaxTimeCheck + 1));
                if (TimeCheck > MaxTimeCheck) TimeCheck = 0;
                Sequence = 0;
            }
            if (timeStamp > MaxTimestamp)
            {
                throw new Exception("时间戳的29位已耗尽");
            }
            this.LastTimestamp = timeStamp; //把当前时间戳保存为最后生成ID的时间戳
            long nextId = (timeStamp << TimestampShift) | (Sequence << SequenceShift) | (TimeCheck << TimeCheckShift) | (IP << IpShift);
            return nextId;

        }
        protected long WaitNextSecond()
        {
            long timeStamp = GetTimeStampForSecond();
            while (timeStamp <= this.LastTimestamp)
            {
                Thread.Sleep(10);
                timeStamp = GetTimeStampForSecond();
            }
            return timeStamp;
        }

        private long GetTimeStampForSecond()
        {
            DateTime theDate = DateTime.Now;
            DateTime d1 = EpochDateTime;
            DateTime d2 = theDate.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long)ts.TotalSeconds;
        }

        private DateTime TimeStampToDateTime(long timestamp)
        {
            //return EpochDateTime.AddMilliseconds(timestamp);
            var date = TimeZoneInfo.ConvertTimeFromUtc(EpochDateTime, TimeZoneInfo.Local);
            return date.AddSeconds(timestamp);
        }
        #endregion
    }
}
