using System.Collections.Generic;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering;
using ImGui.Rendering.Composition;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    internal class ContentChecker : RecordReader
    {
        private interface IStrategy
        {
            void Reset();
            void ReadRecord(List<object> list, object record);
        }

        private class FillExpectedRecordStrategy : IStrategy
        {
            public void Reset()
            {
            }

            public void ReadRecord(List<object> list, object record)
            {
                list.Add(record);
            }
        }

        private class CompareActualRecordStrategy : IStrategy
        {
            private int currentIndex = 0;

            public void Reset()
            {
                this.currentIndex = 0;
            }

            public void ReadRecord(List<object> list, object record)
            {
                var expected = list[this.currentIndex];
                var actual = record;
                Assert.Equal(expected, actual);
                this.currentIndex++;
            }
        }

        public ContentChecker()
        {
            this.strategy = s_fillExpectedRecordStrategy;
        }

        #region Overrides of DrawingContext
        public override void DrawLine(Pen pen, Point point0, Point point1)
        {
            this.strategy.ReadRecord(this.records, (pen, point0, point1));
        }

        public override void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            this.strategy.ReadRecord(this.records, (brush, pen, rectangle));
        }

        public override void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
        {
            this.strategy.ReadRecord(this.records, (brush, pen, rectangle, radiusX, radiusY));
        }

        public override void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            this.strategy.ReadRecord(this.records, (brush, pen, center, radiusX, radiusY));
        }

        public override void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            this.strategy.ReadRecord(this.records, (brush, pen, geometry));
        }

        public override void DrawImage(ITexture image, Rect rectangle)
        {
            this.strategy.ReadRecord(this.records, (image, rectangle));
        }

        public override void DrawImage(ITexture image, Rect rectangle, (double top, double right, double bottom, double left) slice)
        {
            this.strategy.ReadRecord(this.records, (image, rectangle, slice));
        }

        public override void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun)
        {
            this.strategy.ReadRecord(this.records, (foregroundBrush, glyphRun));
        }

        public override void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun, Point origin, double maxTextWidth, double maxTextHeight)
        {
            this.strategy.ReadRecord(this.records, (foregroundBrush, glyphRun, origin, maxTextWidth, maxTextHeight));
        }

        public override void DrawDrawing(Drawing drawing)
        {
            this.strategy.ReadRecord(this.records, drawing);
        }
        #endregion

        #region Overrides of RecordReader
        public override void OnBeforeRead()
        {
        }

        public override void OnAfterRead(MeshList meshList)
        {
        }
        #endregion

        public void StartCheck()
        {
            this.strategy = s_compareActualRecordStrategy;
            this.strategy.Reset();
        }

        private List<object> records = new List<object>();
        private IStrategy strategy;

        private static readonly IStrategy s_fillExpectedRecordStrategy = new FillExpectedRecordStrategy();
        private static readonly IStrategy s_compareActualRecordStrategy = new CompareActualRecordStrategy();
    }
}