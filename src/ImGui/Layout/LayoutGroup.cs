using System;
using System.Diagnostics;
using ImGui.Input;

namespace ImGui.Rendering
{
    internal partial class Node
    {
        public bool HorizontallyOverflow { get; set; }
        public bool VerticallyOverflow { get; set; }

        public OverflowPolicy HorizontallyOverflowPolicy { get => this.RuleSet.OverflowX; }
        public OverflowPolicy VerticallyOverflowPolicy { get => this.RuleSet.OverflowY; }

        internal Node HScrollBarRoot;
        internal Node VScrollBarRoot;
        internal Vector ScrollOffset;
        
        const string HScrollBarName = "_HScrollBar";
        const string VScrollBarName = "_VScrollBar";

        public void CheckRuleSetForLayout_Group(IStyleRuleSet child)
        {
            if (this.RuleSet.IsFixedWidth)
            {
                Debug.Assert(!this.RuleSet.HorizontallyStretched);
                if (this.IsVertical && child.RuleSet.HorizontalStretchFactor > 1)
                {
                    child.RuleSet.HorizontalStretchFactor = 1;
                }
            }
            else if (this.RuleSet.HorizontallyStretched)
            {
                if (this.IsVertical && child.RuleSet.HorizontalStretchFactor > 1)
                {
                    child.RuleSet.HorizontalStretchFactor = 1;
                }
            }
            else
            {
                child.RuleSet.HorizontalStretchFactor = 0;
            }

            if (this.RuleSet.IsFixedHeight)
            {
                Debug.Assert(!this.RuleSet.VerticallyStretched);
                if (!this.IsVertical && child.RuleSet.VerticalStretchFactor > 1)
                {
                    child.RuleSet.VerticalStretchFactor = 1;
                }
            }
            else if (this.RuleSet.VerticallyStretched)
            {
                if (!this.IsVertical && child.RuleSet.VerticalStretchFactor > 1)
                {
                    child.RuleSet.VerticalStretchFactor = 1;
                }
            }
            else
            {
                child.RuleSet.VerticalStretchFactor = 0;
            }
        }

        public void CalcWidth_Group(double unitPartWidth = -1)
        {
            if (this.RuleSet.HorizontallyStretched)//stretched width
            {
                // calculate the width
                this.Width = unitPartWidth * this.RuleSet.HorizontalStretchFactor;
                if (this.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException(
                        $"The width of Group<{this}> is too small to hold any children.");
                }
                this.ContentWidth = this.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else if (this.RuleSet.IsFixedWidth)//fixed width
            {
                // calculate the width
                this.Width = this.RuleSet.MinWidth;

                if (this.Width - this.PaddingHorizontal - this.BorderHorizontal < 1)
                {
                    throw new LayoutException($"The width of Group<{this}> is too small to hold any children.");
                }
                this.ContentWidth = this.Width - this.PaddingHorizontal - this.BorderHorizontal;

                // calculate the width of children
                this.CalcChildrenWidth();
            }
            else // default width: group width is determined by width of all children
            {
                if (this.IsVertical) //vertical group
                {
                    var temp = 0d;
                    // get the max width of children
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }

                        Debug.Assert(visual is Node);//All children should be Node.

                        Node entry = (Node) visual;
                        entry.CalcWidth();
                        temp = Math.Max(temp, entry.Width);
                    }
                    this.ContentWidth = temp;
                }
                else
                {
                    var temp = 0d;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }

                        Debug.Assert(visual is Node);//All children should be Node.

                        Node entry = (Node) visual;
                        entry.CalcWidth();
                        temp += entry.Width + this.RuleSet.CellSpacingHorizontal;
                    }
                    temp -= this.RuleSet.CellSpacingHorizontal;
                    this.ContentWidth = temp < 0 ? 0 : temp;
                }
                this.Width = this.ContentWidth + this.PaddingHorizontal + this.BorderHorizontal;
            }
        }

        private void CalcChildrenWidth()
        {
            if (this.IsVertical) //vertical group
            {
                double maxChildWidth = 0;
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf)
                    {
                        continue;
                    }

                    Debug.Assert(visual is Node);//All children should be Node.

                    Node entry = (Node) visual;

                    if (entry.RuleSet.HorizontallyStretched)
                    {
                        entry.CalcWidth(this.ContentWidth);
                        //the unitPartWidth
                        //(actually every entry will have this width, because entry.HorizontalStretchFactor is always 1 in this case. See `LayoutGroup.Add`.)
                        //for stretched children is the content-box width of the group
                    }
                    else
                    {
                        entry.CalcWidth();
                    }
                    maxChildWidth = Math.Max(maxChildWidth, entry.Width);
                }

                if (this.ContentWidth < maxChildWidth)
                {
                    this.HorizontallyOverflow = true;
                }
                else
                {
                    this.HorizontallyOverflow = false;
                }
            }
            else //horizontal group
            {
                // calculate the unitPartWidth for stretched children
                // calculate the width of fixed-size children

                var childCount = this.ChildCount;

                //only count active children
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf)
                    {
                        childCount--;
                    }

                    Debug.Assert(visual is Node);//All children should be Node.
                }

                //no child, do nothing
                if (childCount <= 0)
                {
                    return;
                }

                var cellSpacing = this.RuleSet.CellSpacingHorizontal;
                if (this.ContentWidth < this.RuleSet.CellSpacingHorizontal)//the content box is too small to hold any child
                {
                    return;
                }

                //get the total size of fixed/default-sized, namely known-sized, children
                var knownSizedChildrenWidth = 0.0;
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf)
                    {
                        continue;
                    }
                    Debug.Assert(visual is Node);//All children should be Node.
                    Node entry = (Node) visual;
                    if (entry.RuleSet.IsFixedWidth)
                    {
                        knownSizedChildrenWidth += entry.RuleSet.MinWidth;
                        knownSizedChildrenWidth += cellSpacing;
                    }
                    else if (entry.RuleSet.IsDefaultWidth)
                    {
                        knownSizedChildrenWidth += entry.GetDefaultWidth();
                        knownSizedChildrenWidth += cellSpacing;
                    }
                }

                var spaceLeftForStretchedChildren = this.ContentWidth - knownSizedChildrenWidth;
                if (spaceLeftForStretchedChildren < 0)//overflow, stretched children will be reverted to default-sized
                {
                    this.HorizontallyOverflow = true;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node)visual;
                        if (entry.RuleSet.HorizontallyStretched)
                        {
                            //set to default-sized
                            entry.RuleSet.HorizontalStretchFactor = 0;
                            entry.RuleSet.MinWidth = 1;
                            entry.RuleSet.MaxWidth = 9999;
                        }
                        entry.CalcWidth();
                    }
                }
                else
                {
                    this.HorizontallyOverflow = false;
                    var factor = 0;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        if (entry.RuleSet.HorizontallyStretched)
                        {
                            factor += entry.RuleSet.HorizontalStretchFactor;
                        }
                        else
                        {
                            entry.CalcWidth();
                        }
                    }

                    if (factor > 0)
                    {
                        var unit = spaceLeftForStretchedChildren / factor;
                        // calculate the width of stretched children
                        foreach (var visual in this.Children)
                        {
                            if (!visual.ActiveSelf)
                            {
                                continue;
                            }
                            Debug.Assert(visual is Node);//All children should be Node.
                            Node entry = (Node) visual;
                            if (entry.RuleSet.HorizontallyStretched)
                            {
                                entry.CalcWidth(unit);
                            }
                        }
                    }
                }

            }
        }

        public void CalcHeight_Group(double unitPartHeight = -1)
        {
            if (this.RuleSet.VerticallyStretched)
            {
                // calculate the height
                this.Height = unitPartHeight * this.RuleSet.VerticalStretchFactor;
                if (this.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this}> is too small to hold any children.");
                }
                this.ContentHeight = this.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else if (this.RuleSet.IsFixedHeight)//fixed height
            {
                // calculate the height
                this.Height = this.RuleSet.MinHeight;

                if (this.Height - this.PaddingVertical - this.BorderVertical < 1)
                {
                    throw new LayoutException($"The height of Group<{this}> is too small to hold any children.");
                }
                this.ContentHeight = this.Height - this.PaddingVertical - this.BorderVertical;

                // calculate the height of children
                this.CalcChildrenHeight();
            }
            else // default height: group height is determined by height of all children
            {
                if (this.IsVertical) // vertical group
                {
                    var temp = 0d;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        entry.CalcHeight();
                        temp += entry.Height + this.RuleSet.CellSpacingVertical;
                    }
                    temp -= this.RuleSet.CellSpacingVertical;
                    this.ContentHeight = temp < 0 ? 0 : temp;
                }
                else // horizontal group
                {
                    var temp = 0d;
                    // get the max height of children
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        entry.CalcHeight();
                        temp = Math.Max(temp, entry.Height);
                    }
                    this.ContentHeight = temp;
                }
                this.Height = this.ContentHeight + this.PaddingVertical + this.BorderVertical;
            }
        }

        private void CalcChildrenHeight()
        {
            if (this.IsVertical) // vertical group
            {
                // calculate the unitPartHeight for stretched children
                // calculate the height of fixed-size children

                var childCount = this.ChildCount;

                //only count active children
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf)
                    {
                        childCount--;
                    }
                    Debug.Assert(visual is Node);//All children should be Node.
                }

                //no child, do nothing
                if (childCount <= 0)
                {
                    return;
                }

                var cellSpacing = this.RuleSet.CellSpacingVertical;
                if (this.ContentHeight < this.RuleSet.CellSpacingVertical)//the content box is too small to hold any child
                {
                    return;
                }

                //get the total size of fixed/default-sized, namely known-sized, children
                var knownSizedChildrenHeight = 0.0;
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf)
                    {
                        continue;
                    }
                    Debug.Assert(visual is Node);//All children should be Node.
                    Node entry = (Node) visual;
                    if (entry.RuleSet.IsFixedHeight)
                    {
                        knownSizedChildrenHeight += entry.RuleSet.MinHeight;
                        knownSizedChildrenHeight += cellSpacing;
                    }
                    else if (entry.RuleSet.IsDefaultHeight)
                    {
                        knownSizedChildrenHeight += entry.GetDefaultHeight();
                        knownSizedChildrenHeight += cellSpacing;
                    }
                }

                var spaceLeftForStretchedChildren = this.ContentHeight - knownSizedChildrenHeight;
                if (spaceLeftForStretchedChildren < 0)//overflow, stretched children will be hidden
                {
                    this.VerticallyOverflow = true;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        if (entry.RuleSet.IsFixedHeight || entry.RuleSet.IsDefaultHeight)
                        {
                            entry.CalcHeight();
                        }
                    }
                }
                else
                {
                    this.VerticallyOverflow = false;
                    var factor = 0;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        if (entry.RuleSet.VerticallyStretched)
                        {
                            factor += entry.RuleSet.VerticalStretchFactor;
                        }
                        else
                        {
                            entry.CalcHeight();
                        }
                    }

                    if (factor > 0)
                    {
                        var unit = spaceLeftForStretchedChildren / factor;
                        // calculate the height of stretched children
                        foreach (var visual in this.Children)
                        {
                            if (!visual.ActiveSelf)
                            {
                                continue;
                            }
                            Debug.Assert(visual is Node);//All children should be Node.
                            Node entry = (Node) visual;
                            if (entry.RuleSet.VerticallyStretched)
                            {
                                entry.CalcHeight(unit);
                            }
                        }
                    }
                }
            }
            else // horizontal group
            {
                double maxChildHeight = 0;
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf)
                    {
                        continue;
                    }
                    Debug.Assert(visual is Node);//All children should be Node.
                    Node entry = (Node) visual;
                    if (entry.RuleSet.VerticallyStretched)
                    {
                        entry.CalcHeight(this.ContentHeight);
                        //the unitPartHeight
                        //(actually every entry will have this height, because entry.VerticalStretchFactor is always 1 in this case. See `LayoutGroup.Add`.)
                        //for stretched children is the content-box height of the group
                    }
                    else
                    {
                        entry.CalcHeight();
                    }
                    maxChildHeight = Math.Max(maxChildHeight, entry.Height);
                }

                if(this.ContentHeight < maxChildHeight)
                {
                    this.VerticallyOverflow = true;
                }
                else
                {
                    this.VerticallyOverflow = false;
                }
            }
        }

        public void SetX_Group(double x)
        {
            SetX_Entry(x);
            if (this.HorizontallyOverflow && HorizontallyOverflowPolicy == OverflowPolicy.Scroll)
            {
                x -= ScrollOffset.X;
            }
            if (this.IsVertical)
            {
                var childX = 0d;
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf || visual.Width < 0.1)
                    {
                        continue;
                    }
                    Debug.Assert(visual is Node);//All children should be Node.
                    Node entry = (Node) visual;
                    switch (this.RuleSet.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                            childX = x + this.BorderLeft + this.PaddingLeft;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childX = x + this.BorderLeft + this.PaddingLeft + (this.ContentWidth - entry.Width) / 2;
                            break;
                        case Alignment.End:
                            childX = x + this.Width - this.BorderRight - this.PaddingRight - entry.Width;
                            break;
                    }

                    entry.SetX(childX);
                }
            }
            else
            {
                double nextX;//position x of first child
                if (this.HorizontallyOverflow)//overflow happens so there is no room for to align children
                {
                    nextX = x + this.BorderLeft + this.PaddingLeft;

                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf || visual.Width < 0.1)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        entry.SetX(nextX);
                        nextX += entry.Width + this.RuleSet.CellSpacingHorizontal;
                    }
                }
                else
                {
                    var childWidthWithCellSpcaing = 0d;
                    var childWidthWithoutCellSpcaing = 0d;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf || visual.Width < 0.1)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        childWidthWithCellSpcaing += entry.Width + this.RuleSet.CellSpacingHorizontal;
                        childWidthWithoutCellSpcaing += entry.Width;
                    }
                    childWidthWithCellSpcaing -= this.RuleSet.CellSpacingVertical;

                    switch (this.RuleSet.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                            nextX = x + this.BorderLeft + this.PaddingLeft;
                            break;
                        case Alignment.Center:
                            nextX = x + this.BorderLeft + this.PaddingLeft + (this.ContentWidth - childWidthWithCellSpcaing) / 2;
                            break;
                        case Alignment.End:
                            nextX = x + this.Width - this.BorderRight - this.PaddingRight - childWidthWithCellSpcaing;
                            break;
                        case Alignment.SpaceAround:
                            nextX = x + this.BorderLeft + this.PaddingLeft +
                                    (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.ChildCount + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextX = x + this.BorderLeft + this.PaddingLeft;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf || visual.Width < 0.1)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        entry.SetX(nextX);
                        switch (this.RuleSet.AlignmentHorizontal)
                        {
                            case Alignment.Start:
                            case Alignment.Center:
                            case Alignment.End:
                                nextX += entry.Width + this.RuleSet.CellSpacingHorizontal;
                                break;
                            case Alignment.SpaceAround:
                                nextX += entry.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.ChildCount + 1);
                                break;
                            case Alignment.SpaceBetween:
                                nextX += entry.Width + (this.ContentWidth - childWidthWithoutCellSpcaing) / (this.ChildCount - 1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        public void SetY_Group(double y)
        {
            SetY_Entry(y);
            if (this.VerticallyOverflow && VerticallyOverflowPolicy == OverflowPolicy.Scroll)
            {
                y -= ScrollOffset.Y;
            }
            if (this.IsVertical)
            {
                double nextY;//position y of first child
                if (this.VerticallyOverflow)//overflow happens so there is no room for to align children
                {
                    nextY = y + this.BorderTop + this.PaddingTop;

                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf || visual.Height < 0.1)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        entry.SetY(nextY);
                        nextY += entry.Height + this.RuleSet.CellSpacingVertical;
                    }
                }
                else
                {
                    var childHeightWithCellSpcaing = 0d;
                    var childHeightWithoutCellSpcaing = 0d;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf || visual.Height < 0.1)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        childHeightWithCellSpcaing += entry.Height + this.RuleSet.CellSpacingVertical;
                        childHeightWithoutCellSpcaing += entry.Height;
                    }
                    childHeightWithCellSpcaing -= this.RuleSet.CellSpacingVertical;

                    switch (this.RuleSet.AlignmentVertical)
                    {
                        case Alignment.Start:
                            nextY = y + this.BorderTop + this.PaddingTop;
                            break;
                        case Alignment.Center:
                            nextY = y + this.BorderTop + this.PaddingTop + (this.ContentHeight - childHeightWithCellSpcaing) / 2;
                            break;
                        case Alignment.End:
                            nextY = y + this.Height - this.BorderBottom - this.PaddingBottom - childHeightWithCellSpcaing;
                            break;
                        case Alignment.SpaceAround:
                            nextY = y + this.BorderTop + this.PaddingTop +
                                    (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.ChildCount + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextY = y + this.BorderTop + this.PaddingTop;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf || visual.Height < 0.1)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        Node entry = (Node) visual;
                        entry.SetY(nextY);
                        switch (this.RuleSet.AlignmentVertical)
                        {
                            case Alignment.Start:
                            case Alignment.Center:
                            case Alignment.End:
                                nextY += entry.Height + this.RuleSet.CellSpacingVertical;
                                break;
                            case Alignment.SpaceAround:
                                nextY += entry.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.ChildCount + 1);
                                break;
                            case Alignment.SpaceBetween:
                                nextY += entry.Height + (this.ContentHeight - childHeightWithoutCellSpcaing) / (this.ChildCount - 1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            else
            {
                var childY = 0d;
                foreach (var visual in this.Children)
                {
                    if (!visual.ActiveSelf || visual.Height < 0.1)
                    {
                        continue;
                    }
                    Debug.Assert(visual is Node);//All children should be Node.
                    Node entry = (Node) visual;
                    switch (this.RuleSet.AlignmentVertical)
                    {
                        case Alignment.Start:
                            childY = y + this.BorderTop + this.PaddingTop;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childY = y + this.BorderTop + this.PaddingTop + (this.ContentHeight - entry.Height) / 2;
                            break;
                        case Alignment.End:
                            childY += y + this.Height - this.BorderBottom - this.PaddingBottom - entry.Height;
                            break;
                    }

                    entry.SetY(childY);
                }
            }
        }

        internal void OnGUI()
        {
            if (!IsGroup)
            {
                return;
            }

            if (HorizontallyOverflow && HorizontallyOverflowPolicy == OverflowPolicy.Scroll)
            {
                if (HScrollBarRoot == null)
                {
                    HScrollBarRoot = new Node(
                        //TMP HACK: later this ID should be generated by the IDService
                        this.Id + 23 * HScrollBarName.GetHashCode(),
                        this.Name + HScrollBarName);
                }
                HScrollBarRoot.ActiveSelf = true;
                GUIContext g = Form.current.uiContext;
                g.KeepAliveID(HScrollBarRoot.Id);

                double occupiedChildrenWidth = 0;
                if (this.IsVertical)
                {
                    double maxChildWidth = 0;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        maxChildWidth = Math.Max(maxChildWidth, visual.Width);
                    }
                    occupiedChildrenWidth = maxChildWidth;
                }
                else
                {
                    var cellSpacing = this.RuleSet.CellSpacingHorizontal;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        occupiedChildrenWidth += visual.Width + cellSpacing;
                    }
                    if (occupiedChildrenWidth != 0)
                    {
                        occupiedChildrenWidth -= cellSpacing;
                    }
                }

                var scrollWidth = this.RuleSet.ScrollBarWidth;
                var padding = this.RuleSet.Padding;
                var border = this.RuleSet.Border;

                Rect bgRect = new Rect(
                    new Point(Rect.Left + padding.left + border.left, Rect.Bottom - padding.bottom - border.bottom - scrollWidth),
                    new Point(Rect.Right - padding.right - border.right - scrollWidth, Rect.Bottom - padding.bottom - border.bottom));
                double contentSize = occupiedChildrenWidth;
                double viewSize = Rect.Width - this.RuleSet.PaddingHorizontal - this.RuleSet.BorderHorizontal;
                double viewPosition = ScrollOffset.X;
                bool hovered, held;
                viewPosition = GUIBehavior.ScrollBehavior(bgRect, contentSize, viewSize, viewPosition,
                    HScrollBarRoot.Id, true, out var gripRect, out hovered, out held);
                ScrollOffset.X = viewPosition;

                var state = GUI.Normal;
                if (hovered)
                {
                    state = GUI.Hover;
                }
                if (held)
                {
                    state = GUI.Active;
                }

                using (var dc = HScrollBarRoot.RenderOpen())
                {
                    var scrollBgColor = this.RuleSet.ScrollBarBackgroundColor;
                    dc.DrawRectangle(new Brush(scrollBgColor), null, bgRect);
                    var scrollButtonColor = this.RuleSet.Get<Color>(StylePropertyName.ScrollBarButtonColor, state);
                    dc.DrawRectangle(new Brush(scrollButtonColor), null, gripRect);
#if DrawScrollbarBorders
                    dc.DrawRectangle(null, new Pen(new Color(1, 0, 0, 0.5), 2), bgRect);
                    dc.DrawRectangle(null, new Pen(new Color(0, 0, 1, 0.5), 2), gripRect);
#endif
                }
            }
            else
            {
                HScrollBarRoot = null;
            }

            if (VerticallyOverflow && VerticallyOverflowPolicy == OverflowPolicy.Scroll)
            {
                if (VScrollBarRoot == null)
                {
                    VScrollBarRoot = new Node(
                        //TMP HACK: later this ID should be generated by the IDService
                        this.Id + 23 * VScrollBarName.GetHashCode(),
                        this.Name + VScrollBarName);
                }
                VScrollBarRoot.ActiveSelf = true;
                GUIContext g = Form.current.uiContext;
                g.KeepAliveID(VScrollBarRoot.Id);

                double occupiedChildrenHeight = 0;
                if (this.IsVertical)
                {
                    var cellSpacing = this.RuleSet.CellSpacingVertical;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        occupiedChildrenHeight += visual.Height + cellSpacing;
                    }
                    if (occupiedChildrenHeight != 0)
                    {
                        occupiedChildrenHeight -= cellSpacing;
                    }
                }
                else
                {
                    double maxChildHeight = 0;
                    foreach (var visual in this.Children)
                    {
                        if (!visual.ActiveSelf)
                        {
                            continue;
                        }
                        Debug.Assert(visual is Node);//All children should be Node.
                        maxChildHeight = Math.Max(maxChildHeight, visual.Height);
                    }
                    occupiedChildrenHeight = maxChildHeight;
                }

                var scrollWidth = this.RuleSet.ScrollBarWidth;
                var padding = this.RuleSet.Padding;
                var border = this.RuleSet.Border;

                Rect bgRect = new Rect(
                    new Point(Rect.Right - padding.right - border.right - scrollWidth, Rect.Top + padding.top + border.top),
                    new Point(Rect.Right - padding.right - border.right, Rect.Bottom - padding.bottom - border.bottom - scrollWidth));
                double contentSize = occupiedChildrenHeight;
                double viewSize = Rect.Height - this.RuleSet.PaddingVertical - this.RuleSet.BorderVertical;
                double viewPosition = ScrollOffset.Y;
                bool hovered, held;
                viewPosition = GUIBehavior.ScrollBehavior(bgRect, contentSize, viewSize, viewPosition,
                    VScrollBarRoot.Id, false, out var gripRect, out hovered, out held);
                ScrollOffset.Y = viewPosition;

                var state = GUI.Normal;
                if (hovered)
                {
                    state = GUI.Hover;
                }
                if (held)
                {
                    state = GUI.Active;
                }

                using (var dc = VScrollBarRoot.RenderOpen())
                {
                    var scrollBgColor = this.RuleSet.ScrollBarBackgroundColor;
                    dc.DrawRectangle(new Brush(scrollBgColor), null, bgRect);
                    var scrollButtonColor = this.RuleSet.Get<Color>(StylePropertyName.ScrollBarButtonColor, state);
                    dc.DrawRectangle(new Brush(scrollButtonColor), null, gripRect);
#if DrawScrollbarBorders
                    dc.DrawRectangle(null, new Pen(new Color(1, 0, 0, 0.5), 2), bgRect);
                    dc.DrawRectangle(null, new Pen(new Color(0, 0, 1, 0.5), 2), gripRect);
#endif
                }
            }
            else
            {
                VScrollBarRoot = null;
            }
        }
        
    }

    internal partial class GUIBehavior
    {
        public static double ScrollBehavior(
            Rect bgRect,
            double contentSize,
            double viewSize,
            double viewPosition,
            int id, bool horizontal,
            out Rect gripRect,
            out bool hovered, out bool held)
        {
            GUIContext g = Form.current.uiContext;

            //grip size
            var trackSize = horizontal ? bgRect.Width : bgRect.Height;
            var contentRatio = viewSize / contentSize;
            var gripSize = trackSize * contentRatio;

            const double minGripSize = 20.0;
            if(gripSize < minGripSize)
            {
                gripSize = minGripSize;
            }

            if (gripSize > trackSize)
            {
                gripSize = trackSize;
            }

            //grip position
            var viewScrollAreaSize = contentSize - viewSize;
            var viewPositionRatio = viewPosition / viewScrollAreaSize;
            var trackScrollAreaSize = trackSize - gripSize;
            var gripPositionOnTrack = trackScrollAreaSize * viewPositionRatio;

            hovered = false;
            held = false;

            hovered = g.IsMouseHoveringRect(bgRect);
            g.KeepAliveID(id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed) //start track
                {
                    g.SetActiveID(id);
                }
            }
            if (g.ActiveId == id)
            {
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    var v = Mouse.Instance.MouseDelta;
                    var mousePositionDelta = horizontal ? v.X : v.Y;
                    var newGripPosition = gripPositionOnTrack + mousePositionDelta;
                    newGripPosition = Math.Clamp(newGripPosition, 0, trackScrollAreaSize);
                    var newGripPositonRatio = newGripPosition / trackScrollAreaSize;
                    viewPosition = newGripPositonRatio * viewScrollAreaSize;
                }
                else //end track
                {
                    g.SetActiveID(0);
                }
            }

            if (g.ActiveId == id)
            {
                held = true;
            }

            if (horizontal)
            {
                gripRect = new Rect(bgRect.X + gripPositionOnTrack, bgRect.Y, gripSize, bgRect.Height);
            }
            else
            {
                gripRect = new Rect(bgRect.X, bgRect.Y + gripPositionOnTrack, bgRect.Width, gripSize);
            }

            return viewPosition;
        }

    }
}
