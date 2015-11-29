using System;
using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class PolygonButton : Control, IRect
    {
        public Point[] Points
        {
            get { return points; }
            set
            {
                points = value;
                Rect = new Rect();
                for (int i = 0; i < Points.Length; i++)
                {
                    this.Rect.Union(Points[i]);
                }
            }
        }

        private string text;
        private Point[] points;
        public Rect Rect { get; private set; }

        public string Text
        {
            get { return text; }
            private set
            {
                if (Text == value)
                {
                    return;
                }

                text = value;
                NeedRepaint = true;
            }
        }
        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        public Rect TextRect
        {
            get
            {
                var rect = new Rect(
                    new Point((Points[3].X + Points[4].X) / 2, (Points[3].Y + Points[4].Y) / 2),
                    new Point((Points[0].X + Points[1].X) / 2, (Points[0].Y + Points[1].Y) / 2)
                    );
                return rect;
            }
        }

        public bool Result { get; private set; }

        internal PolygonButton(string name, BaseForm form, Point[] points, string text)
            : base(name, form)
        {
            this.points = points;
            Text = text;

            var font = Skin.current.PolygonButton[State].Font;
            Format = Application.IocContainer.Resolve<ITextFormat>(
                new NamedParameterOverloads
                    {
                        {"fontFamilyName", font.FontFamily},
                        {"fontWeight", font.FontWeight},
                        {"fontStyle", font.FontStyle},
                        {"fontStretch", font.FontStretch},
                        {"fontSize", (float) font.Size}
                    });
            var textStyle = Skin.current.PolygonButton[State].TextStyle;
            Format.Alignment = textStyle.TextAlignment;
            Layout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                    {
                        {"text", Text},
                        {"textFormat", Format},
                        {"maxWidth", (int)TextRect.Width},
                        {"maxHeight", (int)TextRect.Height}
                    });
        }

        public static bool IsPointInPolygon(Point p, Point[] polygon)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Point q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        internal static bool DoControl(Context g, BaseForm form, Point[] points, string text, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var polygonButton = new PolygonButton(name, form, points, text);
                polygonButton.OnUpdate();
                polygonButton.OnRender(g);
            }

            var control = form.Controls[name] as PolygonButton;
            Debug.Assert(control != null);
            control.Active = true;

            return control.Result;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            var style = Skin.current.PolygonButton[State];
            Layout.MaxWidth = (int)TextRect.Width;
            Layout.MaxHeight = (int)TextRect.Height;
            Layout.Text = Text;

            var oldState = State;
            bool isHit = IsPointInPolygon(Input.Mouse.GetMousePos(Form), Points);
            bool active = Input.Mouse.LeftButtonState == InputState.Down && isHit;
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && isHit;
            if (active)
                State = "Active";
            else if (hover)
                State = "Hover";
            else
                State = "Normal";
            if (State != oldState)
            {
                NeedRepaint = true;
            }

            Result = Input.Mouse.LeftButtonClicked && isHit;
        }

        public override void OnRender(Context g)
        {
            var style = Skin.current.PolygonButton[State];
            g.StrokePolygon(Points, style.LineColor);
            g.FillPolygon(Points, style.FillColor);

            //TODO draw text at the center of this polygon
            g.DrawBoxModel(TextRect, new Content(Layout), style);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        public override void OnClear(Context g)
        {
            g.FillPolygon(Points, CairoEx.ColorWhite);//TODO effective?
        }

        #endregion
    }
}