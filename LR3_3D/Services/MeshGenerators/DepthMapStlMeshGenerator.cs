using System;
using System.Collections.Generic;
using LR3_3D.Models;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace LR3_3D.Services.MeshGenerators
{
    public class DepthMapStlMeshGenerator : IMeshGenerator
    {
        public Mesh Generate(object input, Dictionary<string, object> parameters)
        {
            if (input is not DepthMap depthMap)
                throw new ArgumentException("Input должен быть DepthMap");

            float depthScale = parameters.TryGetValue("depthScale", out var ds) ? (float)ds : 1.0f;

            return GenerateMeshFromDepthMap(depthMap, depthScale);
        }

        public Mesh GenerateMeshFromDepthMap(DepthMap depthMap, float depthScale = 1.0f)
        {
            var mesh = new Mesh();
            var vertexMap = new Dictionary<(int i, int j), int>(); // (row, col) -> vertex index

            int height = depthMap.Height;
            int width = depthMap.Width;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double depth = depthMap.Data[i, j];
                    if (depth == 0.0) continue; // пропускаем фон

                    float x = j - width / 2.0f;
                    float y = height / 2.0f - i;
                    float z = -(float)depth * depthScale;


                    var vertex = new Vertex
                    {
                        Position = new Vector3(x, y, z),
                        Normal = Vector3.UnitZ, // временно, пересчитаем позже
                        Color = new Vector3(0.7f, 0.7f, 0.8f)
                    };

                    vertexMap[(i, j)] = mesh.Vertices.Count;
                    mesh.Vertices.Add(vertex);
                }
            }

            // Генерируем треугольники только там, где все 4 угла квада существуют
            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width - 1; j++)
                {
                    // Проверяем наличие всех 4 углов квада
                    if (!vertexMap.ContainsKey((i, j)) ||
                        !vertexMap.ContainsKey((i, j + 1)) ||
                        !vertexMap.ContainsKey((i + 1, j)) ||
                        !vertexMap.ContainsKey((i + 1, j + 1)))
                        continue;

                    uint v1 = (uint)vertexMap[(i, j)];
                    uint v2 = (uint)vertexMap[(i, j + 1)];
                    uint v3 = (uint)vertexMap[(i + 1, j + 1)];
                    uint v4 = (uint)vertexMap[(i + 1, j)];

                    // Quad как 2 треугольника (как в Python с f v1 v2 v3 v4)
                    mesh.Indices.Add(v1);
                    mesh.Indices.Add(v2);
                    mesh.Indices.Add(v4);

                    mesh.Indices.Add(v2);
                    mesh.Indices.Add(v3);
                    mesh.Indices.Add(v4);
                }
            }

            // Пересчитываем нормали
            CalculateVertexNormals(mesh);
            
            return mesh;
        }

        private void CalculateVertexNormals(Mesh mesh)
        {
            // Инициализируем массив нулями
            var normals = new Vector3[mesh.Vertices.Count];

            for (int i = 0; i < mesh.Indices.Count; i += 3)
            {
                uint i0 = mesh.Indices[i];
                uint i1 = mesh.Indices[i + 1];
                uint i2 = mesh.Indices[i + 2];

                var v0 = mesh.Vertices[(int)i0].Position;
                var v1 = mesh.Vertices[(int)i1].Position;
                var v2 = mesh.Vertices[(int)i2].Position;

                // Нормаль треугольника
                var edge1 = v1 - v0;
                var edge2 = v2 - v0;
                var normal = Vector3.Cross(edge1, edge2);

                // Взвешиваем по площади (длина кросс-произведения = 2*площадь)
                normals[i0] += normal;
                normals[i1] += normal;
                normals[i2] += normal;
            }

            // Нормализуем и применяем к вершинам
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var vertex = mesh.Vertices[i];
                vertex.Normal = normals[i].LengthSquared > 0
                    ? Vector3.Normalize(normals[i])
                    : Vector3.UnitZ;
                mesh.Vertices[i] = vertex;
            }
        }
    }
}
