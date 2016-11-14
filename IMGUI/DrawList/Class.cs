using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImGui
{
    internal class PathPool
    {
        Dictionary<Guid, Point[]> data = new Dictionary<Guid, Point[]>();

        Point[] GetPath(Guid id)
        {
            Point[] path;
            data.TryGetValue(id, out path);
            return path;
        }

        void PutPath(Guid id, Point[] path)
        {
            data[id] = path;
        }

    }
}
