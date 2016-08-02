namespace ImGui
{
    class LayoutEntry
    {
        public Rect rect = new Rect(0, 0, 0, 0);
        public double minWidth;
        public double maxWidth;
        public double minHeight;
        public double maxHeight;
        public Style style;

        public LayoutGroup parent;

        public LayoutEntry(double _minWidth, double _maxWidth, double _minHeight, double _maxHeight, Style _style)
		{
			this.minWidth = _minWidth;
			this.maxWidth = _maxWidth;
			this.minHeight = _minHeight;
			this.maxHeight = _maxHeight;
			if (_style == null)
			{
				_style = Style.None;
			}
			this.style = _style;
		}

        public LayoutEntry(Style _style)
        {
            if (_style == null)
            {
                _style = Style.None;
            }
            this.style = _style;
        }

        public virtual void CalcRect()
        {
        }

        public virtual void CalcWidth() { }

        public virtual void CalcHeight() { }

        public virtual void SetHorizontal(double x, double width)
        {
            this.rect.X = x;
            this.rect.Width = width;
        }

        public virtual void SetVertical(double y, double height)
        {
            this.rect.Y = y;
            this.rect.Height = height;
        }

    }
}
