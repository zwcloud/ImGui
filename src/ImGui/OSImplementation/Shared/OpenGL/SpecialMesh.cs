using System;
using ImGui;

namespace ImGui.Development
{
    public static class SpecialMesh
    {
        internal static Mesh UnitQuadrantMesh {get; set;}
        internal static Mesh OriginPointMesh {get; set;}

        /// <summary>
        /// Init special meshes, called after initialize the graphics renderer
        /// </summary>
        internal static void Init()
        {
            {
                int size = 100;
                var mesh = new Mesh();
                mesh.PrimReserve(6, 4);
                Point a = new Point(-1, -1)*size;
                Point b = new Point(-1, 1)*size;
                Point c = new Point(1, 1)*size;
                Point d = new Point(1, -1)*size;
                Point uvA = new Point(0, 0);
                Point uvB = new Point(0, 1);
                Point uvC = new Point(1, 1);
                Point uvD = new Point(1, 0);
                mesh.AppendVertex(new DrawVertex { pos = a, uv = uvA, color = Color.Red });
                mesh.AppendVertex(new DrawVertex { pos = b, uv = uvB, color = Color.Green });
                mesh.AppendVertex(new DrawVertex { pos = c, uv = uvC, color = Color.Blue });
                mesh.AppendVertex(new DrawVertex { pos = d, uv = uvD, color = Color.Yellow });
                mesh.AppendIndex(0);
                mesh.AppendIndex(1);
                mesh.AppendIndex(2);
                mesh.AppendIndex(0);
                mesh.AppendIndex(2);
                mesh.AppendIndex(3);
                mesh.currentIdx += 4;

                UnitQuadrantMesh = mesh;
            }
            {
                int size = 1;
                var mesh = new Mesh();
                mesh.PrimReserve(6, 4);
                Point a = new Point(-1, -1)*size;
                Point b = new Point(-1, 1)*size;
                Point c = new Point(1, 1)*size;
                Point d = new Point(1, -1)*size;
                Point uvA = new Point(0, 0);
                Point uvB = new Point(0, 1);
                Point uvC = new Point(1, 1);
                Point uvD = new Point(1, 0);
                mesh.AppendVertex(new DrawVertex { pos = a, uv = uvA, color = Color.Red });
                mesh.AppendVertex(new DrawVertex { pos = b, uv = uvB, color = Color.Green });
                mesh.AppendVertex(new DrawVertex { pos = c, uv = uvC, color = Color.Blue });
                mesh.AppendVertex(new DrawVertex { pos = d, uv = uvD, color = Color.Yellow });
                mesh.AppendIndex(0);
                mesh.AppendIndex(1);
                mesh.AppendIndex(2);
                mesh.AppendIndex(0);
                mesh.AppendIndex(2);
                mesh.AppendIndex(3);
                mesh.currentIdx += 4;

                OriginPointMesh = mesh;
            }
        }
    }
}
