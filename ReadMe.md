Hello ImGui,
============

Hello ImGui is an immediate mode GUI library inspired by [IMGUI of Unity3D](https://docs.unity3d.com/Manual/GUIScriptingGuide.html) and [dear imgui](https://github.com/ocornut/imgui).

![code sample](https://raw.githubusercontent.com/wiki/zwcloud/imgui/images/code_sample.png)

(outdated and to be updated) Now it runs on Win10 x64, Ubuntu 16.04, and Android. See [platforms](https://github.com/zwcloud/ImGui/wiki/Platforms). MAC and iphone are not supported because I don't have them.

At present, ImGui lacks usability but will be improved gradually.

## Get Started

### For Windows and Linux

1. Preparation
    * Download msjh.ttf to directory `ImGui\src\ImGui\assets\fonts`. See [font note](https://github.com/zwcloud/ImGui/blob/master/src/ImGui/assets/fonts/ReadMe.md).

2. Create a .NET Standard 2.1 compatible project and reference _ImGui_.

3. Add follwing code files to your project,

    *Program.cs*
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

    *MainForm.cs*
    ```C#
    namespace YourApp
    {
        public class MainForm : Form
        {
            public MainForm() : base(new Rect(320, 180, 1280, 720)) { }

            protected override void OnGUI()
            {
                //your GUI code here
                GUILayout.Label("Hello, ImGui!");
            }
        }
    }
    ```

4. Build your project

5. Run
    * run in VS2017: Press F5
    * run in Windows console:

        ```
        cd MyImGuiApp
        dotnet MyApp.dll
       ```
    * run in Linux terminal:

        ```
        cd MyApp/bin/Debug/netcoreapp3.0
        dotnet MyApp.dll
       ```

6. Exit

    Press <kbd>Esc</kbd> or click the close button of the window.

### For Android

(outdated and to be updated)

1. Copy [Android Templates project](https://github.com/zwcloud/ImGui/tree/master/templates/AndroidTemplate). The referenced Demo can be removed if you don't need that.
2. Add your GUI code in `MainForm.OnGUI`.
3. Build and deploy it to your Android device.

For now, please refer to [the shared project __Demo__](https://github.com/zwcloud/ImGui/tree/master/templates/Demo) for how to use Hello ImGui.

## Target

A Real Universal GUI Framework.

## Dependency

* [ImageSharp](https://github.com/SixLabors/ImageSharp): A cross-platform library for the processing of image files; written in C#. It provides image loading functions for ImGui.
* [Xamarin.Android](https://github.com/xamarin/xamarin-android): Xamarin.Android provides open-source bindings of the Android SDK for use with .NET managed languages such as C#. It mainly provides C# runtime for ImGui.

## Credits

DroidSans.ttf, Droid Sans is a humanist sans serif typeface designed by Steve Matteson [licensed under Apache 2](https://github.com/google/fonts/blob/master/apache/droidsans/LICENSE.txt).

*ImGui doesn't depend on following projects, some code used by ImGui are taken from them.*

* [Typography](https://github.com/LayoutFarm/Typography): C# Font Reader (TrueType / OpenType / OpenFont) , Glyphs Layout and Rendering
* [OpenTK](https://github.com/opentk/opentk): low-level C# wrapper for OpenGL
* [CSharpGL](https://github.com/bitzhuwei/CSharpGL): Object Oriented OpenGL in C#

## License

Hello ImGui is licensed under the LGPL License, see LICENSE for more information.
