using Microsoft.Extensions.Caching.Memory;

namespace ImGui.OSAbstraction.Text
{
    internal class TextContextCache
    {
        public static TextContextCache Default { get; } = new TextContextCache();

        private MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        private int CalcKey(char character, string fontFamily)
        {
            int hash = 17;
            hash = hash * 23 + character.GetHashCode();
            hash = hash * 23 + fontFamily.GetHashCode();
            //TODO consider fontStyle and fontWeight when Typography is ready.
            return hash;
        }

        public ITextContext Add(string text, string fontFamily, double fontSize, TextAlignment alignment)
        {
            var textContext = Application.PlatformContext.CreateTextContext(text, fontFamily, fontSize, alignment);
            textContext.Build();

            var key = CalcKey(text, fontFamily, fontSize, alignment);
            cache.Set(key, textContext);

            return textContext;
        }

        private int CalcKey(string fontFamily, string text, double fontSize, TextAlignment alignment)
        {
            int hash = 17;
            hash = hash * 23 + fontFamily.GetHashCode();
            hash = hash * 23 + text.GetHashCode();
            hash = hash * 23 + fontSize.GetHashCode();
            hash = hash * 23 + alignment.GetHashCode();
            return hash;
        }

        public ITextContext Get(char character, string fontFamily)
        {
            int key = CalcKey(character, fontFamily);
            return cache.Get<ITextContext>(key);
        }

        public ITextContext GetOrAdd(string text, string fontFamily, double fontSize, TextAlignment alignment)
        {
            int key = CalcKey(text, fontFamily, fontSize, alignment);

            var textContext = cache.Get<ITextContext>(key);
            if (textContext != null)
            {
                return textContext;
            }

            textContext = Application.PlatformContext.CreateTextContext(text, fontFamily, fontSize, alignment);
            textContext.Build();
            cache.Set(key, textContext);

            return textContext;
        }
    }
}