using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace LR3_3D.Rendering
{
    public class VBO : IDisposable
    {
        private readonly int handle;

        public VBO()
        {
            handle = GL.GenBuffer();
        }

        public void Bind(BufferTarget target = BufferTarget.ArrayBuffer)
        {
            GL.BindBuffer(target, handle);
        }

        public void Unbind(BufferTarget target = BufferTarget.ArrayBuffer)
        {
            GL.BindBuffer(target, 0);
        }

        public void BufferData<T>(T[] data, BufferUsageHint usage) where T : struct
        {
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Marshal.SizeOf<T>(), data, usage);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(handle);
        }
    }
}
