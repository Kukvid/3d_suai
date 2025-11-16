using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace LR3_3D.Rendering
{
    public sealed class ShaderProgram : IDisposable
    {
        private readonly int handle;

        public ShaderProgram(string vsPath, string fsPath)
        {
            var vsrc = LoadShaderText(vsPath);
            var fsrc = LoadShaderText(fsPath);
            var vs = CompileShader(vsrc, ShaderType.VertexShader);   // GL.CreateShader, GL.ShaderSource, GL.CompileShader 
            var fs = CompileShader(fsrc, ShaderType.FragmentShader);

            handle = GL.CreateProgram();                  // создать программу 
            GL.AttachShader(handle, vs);                  // прикрепить шейдеры 
            GL.AttachShader(handle, fs);             
            GL.LinkProgram(handle);                       // линковка 

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out var ok); // проверка статуса 
            if (ok == 0)
                throw new Exception($"Ошибка линковки шейдеров: {GL.GetProgramInfoLog(handle)}"); // лог программы 

            GL.DetachShader(handle, vs); GL.DetachShader(handle, fs); // отделить 
            GL.DeleteShader(vs); GL.DeleteShader(fs);                   // удалить объекты шейдеров 
        }

        public void Use() => GL.UseProgram(handle); // активация 

        public void SetMatrix4(string name, OpenTK.Mathematics.Matrix4 m)
        {
            int loc = GL.GetUniformLocation(handle, name); // получить локацию 
            GL.UniformMatrix4(loc, false, ref m);          // передать матрицу 
        }

        public void SetInt(string name, int v)
        {
            int loc = GL.GetUniformLocation(handle, name);
            GL.Uniform1(loc, v); // int uniform 
        }

        public void SetVector3(string name, OpenTK.Mathematics.Vector3 v)
        {
            int loc = GL.GetUniformLocation(handle, name);
            GL.Uniform3(loc, v); // vec3 uniform 
        }

        private static int CompileShader(string src, ShaderType type)
        {
            int sh = GL.CreateShader(type);         // создать шейдер 
            GL.ShaderSource(sh, src);               // задать исходник 
            GL.CompileShader(sh);                   // компиляция 
            GL.GetShader(sh, ShaderParameter.CompileStatus, out int ok); // статус 
            if (ok == 0) throw new Exception($"Ошибка компиляции шейдера: {type}: {GL.GetShaderInfoLog(sh)}"); // лог 
            return sh;
        }

        public void Dispose() => GL.DeleteProgram(handle); // освобождение 

        private static string LoadShaderText(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Шейдер не найден: {path}");
            var src = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(src))
                throw new InvalidOperationException($"Шейдер пустой: {path}");
            return Normalize(src);
        }

        private static string Normalize(string s)
        {
            var text = s.Replace("\r\n", "\n");
            if (!text.EndsWith("\n")) text += "\n";
            return text;
        }
    }
}
