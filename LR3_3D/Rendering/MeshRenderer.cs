using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using LR3_3D.Models;

namespace LR3_3D.Rendering
{
    public class MeshRenderer : IDisposable
    {
        private VAO? vao = null;
        private VBO vbo = null!;
        private EBO ebo = null!;

        public void Initialize(Mesh mesh)
        {
            vao = new VAO();
            vao.Bind();

            vbo = new VBO();
            vbo.Bind(BufferTarget.ArrayBuffer);
            vbo.BufferData(mesh.Vertices.ToArray(), BufferUsageHint.StaticDraw);

            ebo = new EBO();
            ebo.Bind(BufferTarget.ElementArrayBuffer);
            ebo.BufferData(mesh.Indices.ToArray(), BufferUsageHint.StaticDraw);

            int stride = Marshal.SizeOf<Vertex>();
            // location = 0 -> Position
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            // location = 1 -> Normal
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));
            // location = 2 -> Color
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, Marshal.OffsetOf<Vertex>(nameof(Vertex.Color)));

            vao.Unbind();
        }

        public void Render()
        {
            if (vao != null) vao.Bind();
            else return;

            GL.DrawElements(PrimitiveType.Triangles, ebo.IndexCount, DrawElementsType.UnsignedInt, 0);
            vao.Unbind();
        }

        public void Dispose()
        {
            if (vao != null) vao.Dispose();
            else return;

            ebo?.Dispose();
            vbo?.Dispose();
        }
    }
}