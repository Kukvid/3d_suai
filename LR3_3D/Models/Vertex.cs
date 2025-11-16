using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace LR3_3D.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Color;

        public Vertex(Vector3 pos, Vector3 normal, Vector3 color)
        {
            Position = pos;
            Normal = normal;
            Color = color;
        }
    }
}