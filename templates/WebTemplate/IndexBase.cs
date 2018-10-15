using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;

namespace WebTemplate
{
    public class IndexBase : BlazorComponent
    {
        public string GetTextFromMethodInClass()
        {
            //var canvas = document.createElement('canvas');
            //var span = (JSRuntime.Current as IJSInProcessRuntime).Invoke<object>("document.createElement", "span");
            return "The source for this text was external C# code in a .CS file";
        }
    }
}