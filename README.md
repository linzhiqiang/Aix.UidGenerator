# Aix.UidGenerator

```
 下图是默认设置 1(符号位)+41(毫秒时间戳)+10(机器位)+2(时间回拨位)+10(同一毫秒内递增序号)  
 +------+-----------------------+----------------+-----------------+-------------------+
 | sign |       milliseconds    |    worker  id  | time callback   |      sequence     |
 +------+-----------------------+----------------+------------------+------------------+
   1bit          41bits               10bits            2bits                10bits
     
 使用时 可以灵活变化 机器位和序号的长度，还可以把workid使用ip的后10位代替，再加几位进程号
 比如：10位ip + 2位进程标识序号 + 2位回拨位 + 8位序号 根据需要灵活变换
```

```
//示例：
var ipKey = IPUtils2.IPToInt() & 0x03ff;// ip后10位
var processKey = 0; //进程标识
var workId = (ipKey << 2) + processKey;  // 采用ip后10位 + 2位的序号(区分进程)
var options = new UIDOptions
{
	WorkId = (int)workId,
	WorkIdBit = 12,
	SequenceBit = 8,
	TimeCheckBit = 2,
	EpochDateTime = new DateTime(2021, 1, 27)
};
IUIDGenerator uIDGenerator = UIDGeneratorFactory.Instance.CreateUIDGenerator(options);
var uid = uIDGenerator.GetUID();
var uidStr = uIDGenerator.ParseUID(uid);

```


