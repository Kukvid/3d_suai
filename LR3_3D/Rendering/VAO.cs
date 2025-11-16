using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LR3_3D.Rendering
{
    public class VAO : IDisposable
    {
        private readonly int handle;

        public VAO()
        {
            handle = GL.GenVertexArray();
        }

        public void Bind()
        {
            GL.BindVertexArray(handle);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(handle);
        }
    }

}
