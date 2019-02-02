using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinPrimitiveRenderer
    {
        public void DrawRectPrimitive(Mesh shapeMesh, RectPrimitive primitive, Color lineColor, double lineWidth)
        {
            this.SetShapeMesh(shapeMesh);

            this.PathRect(primitive.Rect);
            this.PathStroke(lineColor, false, lineWidth);

            this.SetShapeMesh(null);
        }

        public void FillRectPrimitive(Mesh shapeMesh, RectPrimitive primitive, Color fillColor)
        {
            this.SetShapeMesh(shapeMesh);

            this.PathRect(primitive.Rect);
            this.PathFill(fillColor);

            this.SetShapeMesh(null);
        }

        public void DrawRoundedRectPrimitive(Mesh shapeMesh, RoundedRectPrimitive primitive, Color lineColor, double lineWidth)
        {
            this.SetShapeMesh(shapeMesh);

            var roundedRect = primitive.Rect;
            var rect = roundedRect.Rect;
            var radiusX = roundedRect.RadiusX;
            var radiusY = roundedRect.RadiusY;//TODO use radiusY
            this.PathRect(rect, (float)radiusX, 0);
            this.PathStroke(lineColor, false, lineWidth);

            this.SetShapeMesh(null);
        }

        public void FillRoundedRectPrimitive(Mesh shapeMesh, RoundedRectPrimitive primitive, Color fillColor)
        {
            this.SetShapeMesh(shapeMesh);

            var roundedRect = primitive.Rect;
            var rect = roundedRect.Rect;
            var radiusX = roundedRect.RadiusX;
            var radiusY = roundedRect.RadiusY;//TODO use radiusY
            this.PathRect(rect, (float)radiusX);
            this.PathFill(fillColor);

            this.SetShapeMesh(null);
        }

        public void DrawPathPrimitive(Mesh shapeMesh, PathPrimitive pathPrimitive, Vector offset)
        {
            this.SetShapeMesh(shapeMesh);
            this.DrawPath(pathPrimitive, offset);
            this.SetShapeMesh(null);
        }

        public void DrawImagePrimitive(Mesh imageMesh,
            ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style, Vector offset)
        {
            this.SetImageMesh(imageMesh);
            this.DrawImage(imagePrimitive, Rect.Offset(rect, offset), style);
            this.SetImageMesh(null);
        }

        public void DrawSlicedImagePrimitive(Mesh imageMesh,
            ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style, Vector offset)
        {
            this.SetImageMesh(imageMesh);
            this.DrawSlicedImage(imagePrimitive, Rect.Offset(rect, offset), style);
            this.SetImageMesh(null);
        }

        public void DrawTextPrimitive(TextMesh textMesh,
            TextPrimitive textPrimitive, Rect rect, StyleRuleSet style, Vector offset)
        {
            this.SetTextMesh(textMesh);
            this.DrawText(textPrimitive, rect, style);
            this.SetTextMesh(null);
        }

        public void DrawBoxModelPrimitive(Mesh shapeMesh, Mesh imageMesh,
            Rect borderBoxRect, Rect paddingBoxRect, Rect contentBoxRect, StyleRuleSet style)
        {
            this.SetShapeMesh(shapeMesh);
            this.SetImageMesh(imageMesh);

            this.DrawBackground(style, paddingBoxRect);

            this.DrawBorder(style, borderBoxRect, paddingBoxRect);
            this.DrawOutline(style, borderBoxRect);

            this.DrawDebug(paddingBoxRect, contentBoxRect);

            this.SetShapeMesh(null);
            this.SetImageMesh(null);
        }

        public void DrawPrimitiveList(List<Primitive> primitiveList, bool useBoxModel, Rect nodeRect,
            StyleRuleSet ruleSet, MeshList meshList)
        {
            Rect rect = nodeRect;
            if (useBoxModel)
            {
                GetBoxes(nodeRect, ruleSet, out var bRect, out var pRect, out rect);

                var shapeMesh = MeshPool.ShapeMeshPool.Get();
                shapeMesh.Clear();
                shapeMesh.CommandBuffer.Add(DrawCommand.Default);

                var imageMesh = MeshPool.ImageMeshPool.Get();
                imageMesh.Clear();

                this.DrawBoxModelPrimitive(shapeMesh, imageMesh, bRect, pRect, rect, ruleSet);

                meshList.AddOrUpdateShapeMesh(shapeMesh);
                meshList.AddOrUpdateImageMesh(imageMesh);
            }

            foreach (var primitive in primitiveList)
            {
                switch (primitive)
                {
                    case ShapePrimitive sp:
                    {
                        switch (sp)
                        {
                            case RectPrimitive rp:
                            {
                                var shapeMesh = MeshPool.ShapeMeshPool.Get();
                                shapeMesh.Clear();
                                shapeMesh.CommandBuffer.Add(DrawCommand.Default);
                                if (rp.IsFill)
                                {
                                    this.FillRectPrimitive(shapeMesh, rp, ruleSet.FillColor);
                                }
                                else
                                {
                                    this.DrawRectPrimitive(shapeMesh, rp, ruleSet.StrokeColor, ruleSet.StrokeWidth);
                                }
                                meshList.AddOrUpdateShapeMesh(shapeMesh);
                                break;
                            }
                            case RoundedRectPrimitive rrp:
                            {
                                var shapeMesh = MeshPool.ShapeMeshPool.Get();
                                shapeMesh.Clear();
                                shapeMesh.CommandBuffer.Add(DrawCommand.Default);
                                if (rrp.IsFill)
                                {
                                    this.FillRoundedRectPrimitive(shapeMesh, rrp, ruleSet.FillColor);
                                }
                                else
                                {
                                    this.DrawRoundedRectPrimitive(shapeMesh, rrp, ruleSet.StrokeColor, ruleSet.StrokeWidth);
                                }
                                meshList.AddOrUpdateShapeMesh(shapeMesh);
                            }
                            break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    }
                    case PathPrimitive p:
                    {
                        var shapeMesh = MeshPool.ShapeMeshPool.Get();
                        shapeMesh.Clear();
                        shapeMesh.CommandBuffer.Add(DrawCommand.Default);
                        this.DrawPathPrimitive(shapeMesh, p, (Vector)rect.Location);
                        meshList.AddOrUpdateShapeMesh(shapeMesh);
                    }
                    break;
                    case TextPrimitive t:
                    {
                        var textMesh = MeshPool.TextMeshPool.Get();
                        textMesh.Clear();
                        this.DrawTextPrimitive(textMesh, t, rect, ruleSet, primitive.Offset);
                        meshList.AddOrUpdateTextMesh(textMesh);
                    }
                    break;
                    case ImagePrimitive i:
                    {
                        var imageMesh = MeshPool.ImageMeshPool.Get();
                        imageMesh.Clear();
                        this.DrawImagePrimitive(imageMesh, i, rect, ruleSet, primitive.Offset);
                        meshList.AddOrUpdateImageMesh(imageMesh);
                    }
                    break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}