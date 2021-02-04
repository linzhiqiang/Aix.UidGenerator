# Aix.UidGenerator

```
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


