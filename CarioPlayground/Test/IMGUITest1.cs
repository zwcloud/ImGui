using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class IMGUITest1
    {
        [TestMethod]
        public void TestTextBoxUnicode()
        {
            {
                var str = "123456";
                var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                var byteCount = strBytes.Length;
                Assert.AreEqual(6, byteCount);
            }

            {
                var str = "123456你好";
                var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                var byteCount = strBytes.Length;
                Assert.AreEqual(12, byteCount);
            }

            {
                var str = "123456你";
                var strBytes = System.Text.Encoding.UTF8.GetBytes(str);
                var byteCount = strBytes.Length;
                Assert.AreEqual(9, byteCount);
            }
        }
    }
}
