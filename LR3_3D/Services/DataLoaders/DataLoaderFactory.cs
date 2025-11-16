using LR3_3D.Models.DataLoaders;
using LR3_3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR3_3D.Services.DataLoaders
{
    public static class DataLoaderFactory
    {
        public static IDataLoader<T> CreateLoader<T>(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();

            return ext switch
            {
                ".dat" when typeof(T) == typeof(DepthMap) => (IDataLoader<T>)new DepthMapLoader(),
                //".obj" when typeof(T) == typeof(Mesh) => (IDataLoader<T>)new ObjMeshLoader(),
                _ => throw new NotSupportedException($"Формат {ext} не поддерживается")
            };
        }
    }

}
