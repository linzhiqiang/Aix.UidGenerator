# Aix.UidGenerator

var options = new DefaultUIDOptions
{
    WorkId = 2,
    WorkIdBit = 12,
    SequenceBit = 8,
    TimeCheckBit = 2,
    EpochDateTime = new DateTime(2021, 1, 27)
};
IUIDGenerator uIDGenerator = DefaultUIDGenerator.Create(options);
var uid = uIDGenerator.GetUID();
var uidStr = uIDGenerator.ParseUID(uid);
