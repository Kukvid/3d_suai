using LR3_3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR3_3D.Services.DataLoaders
{
    public interface IDataLoader<out T>
    {
        T Load(string path);
    }

}
