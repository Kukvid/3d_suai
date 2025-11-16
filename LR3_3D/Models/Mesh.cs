using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR3_3D.Models
{
    public class Mesh
    {
        public List<Vertex> Vertices { get; set; } = new();
        public List<uint> Indices { get; set; } = new();

        public int VertexCount => Vertices.Count;
        public int IndexCount => Indices.Count;
        public int TriangleCount => IndexCount / 3;
    }
}
