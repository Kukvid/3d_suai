using LR3_3D;
using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System.Text;
using System.IO;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var gws = GameWindowSettings.Default; // таймеры и частоты по умолчанию
        var nws = new NativeWindowSettings
        {
            Title = "LR3_3D",
            ClientSize = new Vector2i(1280, 720),
            API = ContextAPI.OpenGL,             // OpenGL контекст
            Profile = ContextProfile.Core,       // Core профиль
            Flags = ContextFlags.ForwardCompatible
        };
        using var window = new MainWindow(gws, nws);
        window.Run();
    }
}