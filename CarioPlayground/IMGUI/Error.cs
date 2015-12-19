using System;
using System.Collections.Generic;

namespace ImGui
{
    public class Error
    {
        public static string Get(ErrorId errorId)
        {
            return s_errorText[errorId];
        }

        static private readonly Dictionary<ErrorId, string> s_errorText;

        static Error()
        {
            s_errorText = new Dictionary<ErrorId, string>
            {
                {ErrorId.Unknown, "Unknown Error"},
                {ErrorId.Size_WidthAndHeightCannotBeNegative, "Size width and height can not be negative"},
                {ErrorId.Size_CannotModifyEmptySize, "Size can not modify empty size"},
                {ErrorId.Size_WidthCannotBeNegative, "Size width can not be negative"},
                {ErrorId.Size_HeightCannotBeNegative, "Size height can not be negative"},
            };
        }
    }

    public enum ErrorId
    {
        Unknown,
        Size_WidthAndHeightCannotBeNegative,
        Size_CannotModifyEmptySize,
        Size_WidthCannotBeNegative,
        Size_HeightCannotBeNegative,
        Rect_CannotModifyEmptyRect,
        Rect_CannotCallMethod
    }

}