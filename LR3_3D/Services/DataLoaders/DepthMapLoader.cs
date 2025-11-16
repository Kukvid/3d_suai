using LR3_3D.Models;
using LR3_3D.Services.DataLoaders;

namespace LR3_3D.Models.DataLoaders
{
    public class DepthMapLoader : IDataLoader<DepthMap>
    {   
        public DepthMap Load(string path)
        {
            if (path == null)
            {
                throw new InvalidOperationException("Путь для входных данных не указан");
            }
            using FileStream fs = File.OpenRead(path);
            using BinaryReader br = new BinaryReader(fs);

            int height = (int)br.ReadDouble();
            int width = (int)br.ReadDouble();

            var depthMap = new DepthMap { Width = width, Height = height, Data = new double[height, width] };
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    depthMap.Data[i, j] = br.ReadDouble();

            return depthMap;
        }
    }
}
