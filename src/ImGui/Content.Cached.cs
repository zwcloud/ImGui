using System.Collections.Generic;

namespace ImGui
{
    internal sealed partial class Content
    {
        internal static Content Cached(string t, string id)
        {
            Content content;
            if (!chachedContentMap.TryGetValue(id, out content))
            {
                content = new Content(t);
                chachedContentMap.Add(id, content);
                return content;
            }
            else
            {
                // existing text
                if(t != chachedContentMap[id].Text)
                {
                    chachedContentMap[id].Text = t;
                    chachedContentMap[id].Dirty = true;
                }
            }
            return content;
        }

        internal static Content Cached(ITexture texture, string id)
        {
            Content content;
            if (!chachedContentMap.TryGetValue(id, out content))
            {
                content = new Content(texture);
                chachedContentMap.Add(id, content);
            }
            chachedContentMap[id].Image = texture;
            return content;
        }

        internal static Content CachedTexture(string filePath, string id)
        {
            Content content;
            if (!chachedContentMap.TryGetValue(id, out content))
            {
                var texture = GUI.CreateTexture(filePath);
                content = new Content(texture);
                chachedContentMap.Add(id, content);
            }
            return content;
        }

        private static readonly Dictionary<string, Content> chachedContentMap = new Dictionary<string, Content>(256);
    }
}