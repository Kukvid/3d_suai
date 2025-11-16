using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace LR3_3D.Rendering
{
    public class EBO : IDisposable
    {
        private readonly int handle;
        public int IndexCount { get; private set; }

        public EBO()
        {
            handle = GL.GenBuffer();
        }

        public void Bind(BufferTarget target = BufferTarget.ElementArrayBuffer)
        {
            GL.BindBuffer(target, handle);
        }

        public void Unbind(BufferTarget target = BufferTarget.ElementArrayBuffer)
        {
            GL.BindBuffer(target, 0);
        }

        public void BufferData<uintT>(uintT[] indices, BufferUsageHint usage) where uintT : struct
        {
            IndexCount = indices.Length;
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexCount * Marshal.SizeOf<uintT>(), indices, usage);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(handle);
        }
    }
}
