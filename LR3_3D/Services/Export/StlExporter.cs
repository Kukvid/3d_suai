using LR3_3D.Models;
using System.Globalization;
using System.Text;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace LR3_3D.Services.Export
{
    public class StlExporter : IExporter
    {
        public string FileExtension => "stl";
        public string FilterString => "STL Files (*.stl)|*.stl";

        public void ExportToFile(Mesh mesh, string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create);
            ExportToStream(mesh, fileStream);
        }

        public void ExportToStream(Mesh mesh, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.ASCII);

            // Заголовок STL
            writer.WriteLine("solid DepthMapMesh");

            // Экспорт всех треугольников
            for (int i = 0; i < mesh.IndexCount; i += 3)
            {
                var v1 = mesh.Vertices[(int)mesh.Indices[i]];
                var v2 = mesh.Vertices[(int)mesh.Indices[i + 1]];
                var v3 = mesh.Vertices[(int)mesh.Indices[i + 2]];

                var normal = CalculateFacetNormal(v1.Position, v2.Position, v3.Position);

                // Формат STL ASCII
                writer.WriteLine($"  facet normal {Format(normal.X)} {Format(normal.Y)} {Format(normal.Z)}");
                writer.WriteLine("    outer loop");
                writer.WriteLine($"      vertex {Format(v1.Position.X)} {Format(v1.Position.Y)} {Format(v1.Position.Z)}");
                writer.WriteLine($"      vertex {Format(v2.Position.X)} {Format(v2.Position.Y)} {Format(v2.Position.Z)}");
                writer.WriteLine($"      vertex {Format(v3.Position.X)} {Format(v3.Position.Y)} {Format(v3.Position.Z)}");
                writer.WriteLine("    endloop");
                writer.WriteLine("  endfacet");
            }

            // Закрывающий тег
            writer.WriteLine("endsolid DepthMapMesh");
        }

        private Vector3 CalculateFacetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var edge1 = v2 - v1;
            var edge2 = v3 - v1;
            return Vector3.Normalize(Vector3.Cross(edge1, edge2));
        }

        private string Format(float value)
        {
            return value.ToString("g6", CultureInfo.InvariantCulture);
        }
    }
}
