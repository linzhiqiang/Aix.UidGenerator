﻿using Aix.UidGenerator;
using Aix.UidGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aix.UidGeneratorSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var bits = "0000000000000000000000000000000000000000001111111111110000000000";
            var bit2 = "0000000000000010101101000010111100001100010000000000100000000001";
            // System.Convert.ToString(d, 2)
            //System.Convert.ToInt64(bits, 2);
            //var str1=   System.Convert.ToString(BitUtils.MaxVaue(41), 2);
            var mask = BitUtils.MaxVaue(12) << 10;
            var workdId = (761064115406849 & mask)   >> 10; //111111111111
            Console.WriteLine("UID 测试：");

           // DefaultUIDGeneratorTest();
            IPUIDGeneratorTest();

            Console.Read();
        }

        static void DefaultUIDGeneratorTest()
        {
            var options = new DefaultUIDOptions
            {
                WorkId = 2,
                WorkIdBit = 12,
                SequenceBit = 8,
                TimeCheckBit = 2,
                EpochDateTime = new DateTime(2021, 1, 27)
            };
            IUIDGenerator uIDGenerator = DefaultUIDGenerator.Create(options);
            var uid1 = uIDGenerator.GetUID();
            var s = uIDGenerator.ParseUID(uid1);
            HashSet<long> set = new HashSet<long>();
            List<string> uidStr = new List<string>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                var uid = uIDGenerator.GetUID();
                if (set.Contains(uid))
                {
                    Console.WriteLine(uid);
                }
                else
                {
                    set.Add(uid);
                    uidStr.Add(uIDGenerator.ParseUID(uid));
                }
            }

            stopwatch.Stop();
            Console.WriteLine("耗时：" + stopwatch.ElapsedMilliseconds / 1000.0);
        }

        static void IPUIDGeneratorTest()
        {
            var options = new IPUIDOptions
            {
                EpochDateTime = new DateTime(2021, 1, 27)
            };
            IUIDGenerator uIDGenerator = IPUIDGenerator.Create(options);
           
            HashSet<long> set = new HashSet<long>();
            List<string> uidStr = new List<string>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 2000; i++)
            {
                var uid = uIDGenerator.GetUID();
               
                if (set.Contains(uid))
                {
                    Console.WriteLine(uid);
                }
                else
                {
                    set.Add(uid);
                    uidStr.Add(uIDGenerator.ParseUID(uid));
                }
            }

            stopwatch.Stop();
            Console.WriteLine("耗时：" + stopwatch.ElapsedMilliseconds / 1000.0);
        }
    }
}