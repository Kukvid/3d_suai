using LR3_3D.Models;
using LR3_3D.Rendering;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using LR3_3D.Services.Export;
using LR3_3D.Services.MeshGenerators;
using LR3_3D.Services.DataLoaders;

namespace LR3_3D
{
    public class MainWindow : GameWindow
    {
        private MeshRenderer? renderer;
        private Mesh? mesh;
        private ShaderProgram? shaderProgram;

        private float yaw = 90.0f; // угол (влево-вправо), начальный повёрнут к оси Z
        private float pitch = 0.0f; // угол (вверх-вниз)
        private float zoom = 600.0f; // расстояние камеры от центра
        private string path = "Data\\DepthMap_11.dat";

        private Matrix4 model;
        private Matrix4 view;
        private Matrix4 projection;

        private readonly List<IExporter> exporters = new(){
            new StlExporter(),
        };

        public MainWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Настройка OpenGL
            GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Multisample); // Включаем мультисемплинг

            try
            {
                // Инициализация всех компонентов
                InitializeShaders();
                InitializeData(path);
                InitializeOpenGLObjects();
                InitializeMatrices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации: {ex.Message}");
                Close();
            }
        }

        private void InitializeShaders()
        {
            var baseDir = AppContext.BaseDirectory;
            var vertex = Path.Combine(baseDir, "Rendering", "vertex.glsl");
            var fragment = Path.Combine(baseDir, "Rendering", "fragment.glsl");
            shaderProgram = new ShaderProgram(vertex, fragment);
        }

        private void InitializeData(string path)
        {
            var loader = DataLoaderFactory.CreateLoader<DepthMap>(path);
            var depthMap = loader.Load(path);

            Console.WriteLine($"Карта глубины загружена: {depthMap.Width}×{depthMap.Height}");

            IMeshGenerator generator = new DepthMapStlMeshGenerator();
            var parameters = new Dictionary<string, object> { ["depthScale"] = 1.0f };
            mesh = generator.Generate(depthMap, parameters);
            Console.WriteLine($"Mesh: {mesh.Vertices.Count} вершин, {mesh.Indices.Count / 3} треугольников");
        }

        private void InitializeOpenGLObjects()
        {
            if (mesh == null)
                throw new InvalidOperationException("Меш не был инициализирован");

            renderer = new MeshRenderer();
            renderer.Initialize(mesh);
        }

        private void InitializeMatrices()
        {
            model = Matrix4.Identity;

            // Фиксированная камера на расстоянии zoom, с углами yaw/pitch
            float radYaw = MathHelper.DegreesToRadians(yaw);
            float radPitch = MathHelper.DegreesToRadians(pitch);
            float x = (float)(Math.Cos(radPitch) * Math.Cos(radYaw));
            float y = (float)(Math.Sin(radPitch));
            float z = (float)(Math.Cos(radPitch) * Math.Sin(radYaw));
            Vector3 direction = new Vector3(x, y, z);
            Vector3 cameraPos = -direction * zoom;

            // Матрица вида (камера смотрит на начало координат)
            view = Matrix4.LookAt(cameraPos, Vector3.Zero, Vector3.UnitY);

            // Матрица проекции (перспективная)
            projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                (float)Size.X / (float)Size.Y,
                0.1f,
                3000f);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                (float)e.Width / (float)e.Height,
                0.1f,
                3000f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            // Рендер
            if (mesh != null && renderer != null && shaderProgram != null)
            {
                shaderProgram.Use();

                // Установка матриц трансформации
                shaderProgram.SetMatrix4("model", model);
                shaderProgram.SetMatrix4("view", view);
                shaderProgram.SetMatrix4("projection", projection);

                // Настройка освещения
                shaderProgram.SetVector3("lightPos", new Vector3(10.0f, 10.0f, 10.0f));
                shaderProgram.SetVector3("viewPos", view.ExtractTranslation());

                renderer.Render();
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            var input = KeyboardState;

            // Выход с Escape
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                Close();
            }

            // Экспорт
            if (input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.E))
            {
                ExportMesh();
            }
        }

        private void ExportMesh()
        {
            if (mesh == null)
                return;

            // Комбинированный фильтр для всех форматов
            string filter = string.Join("|", exporters.Select(e => e.FilterString));

            using var saveFileDialog = new SaveFileDialog{ Filter = filter };

            using var dialog = new SaveFileDialog { Filter = filter };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Определяем экспортер по расширению
                string ext = Path.GetExtension(dialog.FileName).TrimStart('.').ToLower();
                var exporter = exporters.First(e => e.FileExtension == ext);

                if (exporter != null)
                    exporter.ExportToFile(mesh, dialog.FileName);
                else
                    MessageBox.Show("Неизвестный формат!");
            }
        }

        protected override void OnUnload()
        {
            shaderProgram?.Dispose();
            renderer?.Dispose();
            base.OnUnload();
        }
    }

}
