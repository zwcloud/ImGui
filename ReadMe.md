Hello ImGui,
=====

Hello ImGui is an immediate mode GUI library inspired by [IMGUI of Unity3D](https://docs.unity3d.com/Manual/GUIScriptingGuide.html) and [dear imgui](https://github.com/ocornut/imgui).

![code sample](https://raw.githubusercontent.com/wiki/zwcloud/imgui/images/code_sample.png)

Now it runs on Win10 x64, Ubuntu 16.04, and Android. See [platforms](https://github.com/zwcloud/ImGui/wiki/Platforms).

Please be infromed that ImGui is just released. There's a lack of usability and documentation. But all will be improved gradually.

## Get Started

1. Download msjh.ttf to directory `ImGui\src\ImGui\assets\fonts`. See [font note](https://github.com/zwcloud/ImGui/blob/master/src/ImGui/assets/fonts/ReadMe.md).

2. Create a .NET Core 2.0 project and referance _ImGui_.

3. Add follwing code files to your project,

    Program.cs
    ```C#
    namespace YourApp
    {
        class Program
        {
            [STAThread]
            static void Main()
            {
                Application.Init();
                Application.Run(new MainForm());
            }
        }
    }
    ```

    MainForm.cs
    ```C#
    namespace YourApp
    {
        public class MainForm : Form
        {
            public MainForm() : base(new Rect(320, 180, 1280, 720))
            {
            }

            protected override void OnGUI()
            {
                //your GUI code here
            }
        }
    }
    ```

4. Build your project

5. Run
    * run in VS2017: Press F5
    * run in console:

        ```
        cd MyApp/bin/Debug/netcoreapp2.0
        dotnet MyApp.dll
        ```
6. Exit
    Press <kbd>Esc</kbd> or click the close button of the window.

For now, please refer to [the shared project __Demo__](https://github.com/zwcloud/ImGui/tree/master/templates/Demo) for how to use Hello ImGui.

Please note that the API is unstable and will change at any time. Documentaion will be added in the future when API is stable.


## Target

A Real Universal GUI Framework.

## Dependency

* [ImageSharp](https://github.com/SixLabors/ImageSharp): A cross-platform library for the processing of image files; written in C#. It provides image loading functions for ImGui.
* [Xamarin.Android](https://github.com/xamarin/xamarin-android): Xamarin.Android provides open-source bindings of the Android SDK for use with .NET managed languages such as C#. It mainly provides C# runtime for ImGui.

## Credits

*ImGui doesn't depends on those projects, code used by ImGui are taken from them.*

* [Typography](https://github.com/LayoutFarm/Typography): C# Font Reader (TrueType / OpenType / OpenFont) , Glyphs Layout and Rendering
* [OpenTK](https://github.com/opentk/opentk): low-level C# wrapper for OpenGL
* [CSharpGL](https://github.com/bitzhuwei/CSharpGL): Object Oriented OpenGL in C#
* [LogUtility](https://github.com/Ivony/LogUtility): a light log tools

## License

Hello ImGui is licensed under the LGPL License, see LICENSE for more information.
