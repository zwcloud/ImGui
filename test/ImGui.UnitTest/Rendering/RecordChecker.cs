using System;
using System.Collections.Generic;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class FillExpectedRecordStrategy : ContentCheckerBase.IStrategy
    {
        public void Reset()
        {
        }

        public bool ReadRecord(List<object> list, object record)
        {
            list.Add(record);
            return true;
        }
    }

    public class CompareActualRecordStrategy : ContentCheckerBase.IStrategy
    {
        private int currentIndex = 0;

        public void Reset()
        {
            this.currentIndex = 0;
        }

        public bool ReadRecord(List<object> list, object record)
        {
            var expected = list[this.currentIndex];
            var actual = record;
            Assert.Equal(expected, actual);
            this.currentIndex++;
            return true;
        }
    }

    internal class ContentChecker : ContentCheckerBase
    {
        public ContentChecker()
        {
            this.SetStrategy(fillStrategy);
        }

        public void StartCheck()
        {
            this.SetStrategy(compareStrategy);
            compareStrategy.Reset();
        }

        private readonly FillExpectedRecordStrategy fillStrategy = new FillExpectedRecordStrategy();
        private readonly CompareActualRecordStrategy compareStrategy = new CompareActualRecordStrategy();
    }
}