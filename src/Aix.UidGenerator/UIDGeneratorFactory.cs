using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.UidGenerator
{
    public class UIDGeneratorFactory
    {
        public static UIDGeneratorFactory Instance = new UIDGeneratorFactory();

        static object UIDLock = new object();
        private UIDGeneratorFactory() { }

        private static IUIDGenerator UIDGenerator;

        /// <summary>
        /// 创建UID生成器
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IUIDGenerator CreateUIDGenerator(UIDOptions options)
        {
            return new UIDGeneratorImpl(options);
        }

        /// <summary>
        /// 创建UID生成器 1+29+15+2+16  1: 第一位符号位  2：29位秒(十几年吧)  3：15位序号  4:2位回拨位  5: 16位ip（后2位数字） 可以变为 10位ip+6位
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IUIDGenerator CreateUIDGenerator(IPUIDOptions options)
        {
            return new UIDGeneratorIPImpl(options);
        }
    }
}
