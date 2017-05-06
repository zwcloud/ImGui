using System.Collections.Generic;

namespace ImGui
{
    internal sealed partial class Content
    {
        internal static Content Cached(string t, string d) { return null; }//dummy
        internal static Content Cached(string t)
        {
            GUIContext g = Form.current.uiContext;
            Window window = g.CurrentWindow;

            var id = window.GetID(t);
            Content content;
            if (!cachedContentMap.TryGetValue(id, out content))
            {
                content = new Content(t);
                cachedContentMap.Add(id, content);
                return content;
            }
            else
            {
                // existing text
                if(t != cachedContentMap[id].Text)
                {
                    cachedContentMap[id].Text = t;
                    cachedContentMap[id].Dirty = true;
                }
            }
            return content;
        }

        internal static Content Cached(ITexture texture, string id) { return null; }//dummy
        internal static Content Cached(ITexture texture)
        {
            GUIContext g = Form.current.uiContext;
            Window window = g.CurrentWindow;

            var id = window.GetID(texture);
            Content content;
            if (!cachedContentMap.TryGetValue(id, out content))
            {
                content = new Content(texture);
                cachedContentMap.Add(id, content);
            }
            cachedContentMap[id].Image = texture;
            return content;
        }

        internal static Content CachedTexture(string filePath, string id) { return null; }//dummy
        internal static Content CachedTexture(string filePath)
        {
            GUIContext g = Form.current.uiContext;
            Window window = g.CurrentWindow;

            var id = window.GetID(filePath);
            Content content;
            if (!cachedContentMap.TryGetValue(id, out content))
            {
                var texture = GUI.CreateTexture(filePath);
                content = new Content(texture);
                cachedContentMap.Add(id, content);
            }
            return content;
        }

        private static Content tempText = new Content();

        private static readonly Dictionary<int, Content> cachedContentMap = new Dictionary<int, Content>(256);
    }
}