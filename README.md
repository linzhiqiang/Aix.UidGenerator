# Aix.UidGenerator

```
var ipKey = IPUtils2.IPToInt() & 0x03ff;// ip��10λ
var processKey = 0; //���̱�ʶ
var workId = (ipKey << 2) + processKey;  // ����ip��10λ + 2λ�����(���ֽ���)
var options = new UIDOptions
{
	WorkId = (int)workId,
	WorkIdBit = 12,
	SequenceBit = 8,
	TimeCheckBit = 2,
	EpochDateTime = new DateTime(2021, 1, 27)
};
IUIDGenerator uIDGenerator = DefaultUIDGenerator.Create(options);
var uid = uIDGenerator.GetUID();
var uidStr = uIDGenerator.ParseUID(uid);

```


