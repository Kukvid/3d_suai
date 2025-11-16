using LR3_3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR3_3D.Services.Export
{
    public interface IExporter
    {
        string FileExtension { get; }
        string FilterString { get; }
        void ExportToFile(Mesh mesh, string filePath);
        void ExportToStream(Mesh mesh, Stream stream);
    }
}
