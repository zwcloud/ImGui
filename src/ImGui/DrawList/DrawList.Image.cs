namespace ImGui
{
    partial class DrawList
    {
        // textured triangle part, mainly used for rendering images
        void AddImageRect(Point a, Point c, Point uv_a, Point uv_c, Color col)
        {
            Point b = new Point(c.X, a.Y);
            Point d = new Point(a.X, c.Y);
            Point uv_b = new Point(uv_c.X, uv_a.Y);
            Point uv_d = new Point(uv_a.X, uv_c.Y);

            ImageBuffer.AppendVertex(new DrawVertex { pos = (PointF)a, uv = (PointF)uv_a, color = (ColorF)col });
            ImageBuffer.AppendVertex(new DrawVertex { pos = (PointF)b, uv = (PointF)uv_b, color = (ColorF)col });
            ImageBuffer.AppendVertex(new DrawVertex { pos = (PointF)c, uv = (PointF)uv_c, color = (ColorF)col });
            ImageBuffer.AppendVertex(new DrawVertex { pos = (PointF)d, uv = (PointF)uv_d, color = (ColorF)col });
            ImageBuffer.AppendIndex(0);
            ImageBuffer.AppendIndex(1);
            ImageBuffer.AppendIndex(2);
            ImageBuffer.AppendIndex(0);
            ImageBuffer.AppendIndex(2);
            ImageBuffer.AppendIndex(3);
            ImageBuffer._currentIdx += 4;
        }

    }
}