Hello ImGui,
============

Hello ImGui is an immediate mode GUI library inspired by [IMGUI of Unity3D](https://docs.unity3d.com/Manual/GUIScriptingGuide.html) and [dear imgui](https://github.com/ocornut/imgui).

It's still a [work in progress](https://github.com/zwcloud/ImGui/projects/10).

![code sample](https://raw.githubusercontent.com/wiki/zwcloud/imgui/images/code_sample.png)

Now it runs on Win10, Linux(Ubuntu 16.04) and Android. See [platforms](https://github.com/zwcloud/ImGui/wiki/Platforms). Mac and iPhone are not supported because I don't have them.

## Get Started

### Windows and Linux
1. Clone ImGui

```
git clone https://github.com/zwcloud/ImGui.git
```

2. Create a .NET7 console project and reference ImGui.
```
mkdir MyImGuiApp
```
Create `MyImGuiApp.csproj` with following content:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=".\ImGui\src\ImGui\ImGui.csproj" />
  </ItemGroup>
</Project>
```

3. Add `Program.cs` to your project,

    ```C#
    using ImGui;
    var demo = new Demo();
    Application.Run(new Form(new Rect(320, 180, 1280, 720)), () =>
    {
        demo.OnGUI();
        ImGui.GUILayout.Label("Hello, ImGui!");
    });
    ```

4. Build your project

5. Run
    * run with Visual Studio 2022: Press F5
    * run on Windows/Linux:
        ```
        cd MyImGuiApp
        dotnet MyApp.dll
       ```

6. Exit

    Press <kbd>Esc</kbd> or click the close button of the window.

### Android

1. Copy [Android Templates project](https://github.com/zwcloud/ImGui/tree/master/templates/AndroidTemplate). Referenced shared project Demo can be removed if not needed.
2. Add your GUI code in `MainForm.OnGUI`.
3. Build and deploy it to your Android device.

## Documentation

For now, please refer to [the shared project __Demo__](https://github.com/zwcloud/ImGui/tree/master/templates/Demo) for how to use Hello ImGui.

## Dependency

* [Xamarin.Android](https://github.com/xamarin/xamarin-android): Xamarin.Android provides open-source bindings of the Android SDK for use with .NET managed languages such as C#. It mainly provides C# runtime for ImGui.

## Credits

*ImGui doesn't depend on following projects, some code used by ImGui are taken from them.*

* [BigGustave](https://github.com/EliotJones/BigGustave): Open, read and create PNG images in fully managed C#.
* [Typography](https://github.com/LayoutFarm/Typography): C# Font Reader (TrueType / OpenType / OpenFont) , Glyphs Layout and Rendering
* [OpenTK](https://github.com/opentk/opentk): low-level C# wrapper for OpenGL
* [CSharpGL](https://github.com/bitzhuwei/CSharpGL): Object Oriented OpenGL in C#

Droid Sans and Terminus TTF fount, see [fonts/ReadMe](https://github.com/zwcloud/ImGui/blob/master/src/ImGui/assets/fonts/ReadMe.md).

## License

Hello ImGui is licensed under the AGPL License, see LICENSE for more information.
