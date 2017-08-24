using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImGui.Development
{
    class Graphics
    {
        public static void SaveToObjFile(string path, IList<DrawVertex> vertexes, IList<DrawIndex> indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# ImGui Test");

            for (int i = 0; i < vertexes.Count; i++)
            {
                var position = vertexes[i].pos;
                sb.AppendFormat("v {0} {1} {2}", -position.X, position.Y, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Count; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 2] + 1,
                    indexes[i + 1] + 1);
                sb.AppendLine();
            }
            File.WriteAllText(path, sb.ToString());
        }

        public static void SaveToObjFile(IList<DrawVertex> vertexes, IList<DrawIndex> indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# ImGui Test");

            for (int i = 0; i < vertexes.Count; i++)
            {
                var position = vertexes[i].pos;
                sb.AppendFormat("v {0} {1} {2}", -position.X, position.Y, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Count; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 2] + 1,
                    indexes[i + 1] + 1);
                sb.AppendLine();
            }
            File.WriteAllText("D:\\imgui_test.obj", sb.ToString());
        }

        public static void SaveToObjFile2(string filePath, float[] positions, int[] indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# Triangle");
            sb.AppendLine();

            for (int i = 0; i < positions.Length; i += 2)
            {
                sb.AppendFormat("v {0} {1} 0", positions[i], positions[i + 1]);
                sb.AppendLine();
            }

            for (int i = 0; i < positions.Length; i++)
            {
                sb.AppendFormat("vt {0} {1}", 0, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Length; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 1] + 1,
                    indexes[i + 2] + 1);
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
