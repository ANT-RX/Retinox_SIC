using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Retinox.Services
{
    public class OnnxModelEvaluator : IDisposable
    {
        private InferenceSession _session;

        // Arreglo de clases por defecto (8 del estándar ODIR)
        private string[] _clases = new string[]
        {
            "Normal (N)",
            "Diabetes (D)",
            "Glaucoma (G)",
            "Catarata (C)",
            "Degeneración Macular Asociada a la Edad (A)",
            "Hipertensión (H)",
            "Miopía Patológica (M)",
            "Otras anomalías (O)"
        };

        public OnnxModelEvaluator(string modelPath)
        {
            if (File.Exists(modelPath))
            {
                _session = new InferenceSession(modelPath);
                CargarClasesDesdeArchivo(modelPath);
            }
        }

        private void CargarClasesDesdeArchivo(string modelPath)
        {
            try
            {
                // Buscar un archivo "clases.txt" en la misma ruta donde está el modelo ONNX.
                string directorio = Path.GetDirectoryName(modelPath) ?? "";
                string archivoClases = Path.Combine(directorio, "clases.txt");

                if (File.Exists(archivoClases))
                {
                    // Leer las líneas ignorando las que estén vacías
                    var lineas = File.ReadAllLines(archivoClases)
                                     .Where(l => !string.IsNullOrWhiteSpace(l))
                                     .Select(l => l.Trim())
                                     .ToArray();

                    if (lineas.Length > 0)
                    {
                        _clases = lineas;
                    }
                }
            }
            catch
            {
                // Si hay algún problema leyendo el archivo, se mantendrán las clases por defecto.
            }
        }

        public string Predict(string imagePath)
        {
            if (_session == null)
            {
                // MODO DE PRUEBA (DUMMY): Si el modelo no existe, simulamos el procesamiento
                // para que el médico pueda probar la interfaz y la generación de PDF.
                System.Threading.Thread.Sleep(2500); // Simulamos 2.5 segundos de procesamiento
                return "Glaucoma (G): 87.5% (MODO DE PRUEBA - Modelo ONNX no detectado)";
            }

            try
            {
                // Leemos las dimensiones reales que espera el modelo subido
                var inputMeta = _session.InputMetadata;
                var inputName = inputMeta.Keys.First();
                var inputDimensions = inputMeta[inputName].Dimensions;
                
                int expectedHeight = 320; // El modelo subido requiere 320x320
                int expectedWidth = 320;

                bool isNCHW = (inputDimensions.Length == 4 && inputDimensions[1] == 3);
                bool isNHWC = !isNCHW; // Por defecto NHWC como en Python

                if (isNHWC)
                {
                    expectedHeight = (inputDimensions.Length == 4 && inputDimensions[1] > 0) ? inputDimensions[1] : 320;
                    expectedWidth = (inputDimensions.Length == 4 && inputDimensions[2] > 0) ? inputDimensions[2] : 320;
                }
                else
                {
                    expectedHeight = (inputDimensions.Length == 4 && inputDimensions[2] > 0) ? inputDimensions[2] : 320;
                    expectedWidth = (inputDimensions.Length == 4 && inputDimensions[3] > 0) ? inputDimensions[3] : 320;
                }

                // 1. Cargar y redimensionar la imagen al tamaño dinámico que pide la IA
                using (Image<Rgb24> image = Image.Load<Rgb24>(imagePath))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(expectedWidth, expectedHeight),
                        Mode = ResizeMode.Stretch
                    }));

                    // 2. Convertir imagen a Tensor (Formato NHWC o NCHW)
                    Tensor<float> inputTensor;

                    if (isNHWC)
                    {
                        inputTensor = new DenseTensor<float>(new[] { 1, expectedHeight, expectedWidth, 3 });
                        for (int y = 0; y < image.Height; y++)
                        {
                            for (int x = 0; x < image.Width; x++)
                            {
                                var pixel = image[x, y];
                                // Pasar píxeles brutos (0-255) ya que el modelo asume su propio procesamiento
                                inputTensor[0, y, x, 0] = pixel.R;
                                inputTensor[0, y, x, 1] = pixel.G;
                                inputTensor[0, y, x, 2] = pixel.B;
                            }
                        }
                    }
                    else
                    {
                        // NCHW
                        inputTensor = new DenseTensor<float>(new[] { 1, 3, expectedHeight, expectedWidth });
                        for (int y = 0; y < image.Height; y++)
                        {
                            for (int x = 0; x < image.Width; x++)
                            {
                                var pixel = image[x, y];
                                // Pasar píxeles brutos (0-255) ya que el modelo asume su propio procesamiento
                                inputTensor[0, 0, y, x] = pixel.R;
                                inputTensor[0, 1, y, x] = pixel.G;
                                inputTensor[0, 2, y, x] = pixel.B;
                            }
                        }
                    }

                    // 3. Crear input para el modelo
                    var inputs = new List<NamedOnnxValue>
                    {
                        NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
                    };

                    // 4. Ejecutar Inferencia
                    using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _session.Run(inputs))
                    {
                        // Extraer el resultado
                        var output = results.First().AsTensor<float>();
                        var probabilidades = output.ToArray();

                        // Formatear todas las probabilidades, ordenadas de mayor a menor
                        var pares = new System.Collections.Generic.List<(string Nombre, float Prob)>();
                        for (int i = 0; i < probabilidades.Length; i++)
                        {
                            string nombreClase = (i < _clases.Length) ? _clases[i] : $"Clase Desconocida {i}";
                            pares.Add((nombreClase, probabilidades[i]));
                        }

                        var paresOrdenados = System.Linq.Enumerable.OrderByDescending(pares, p => p.Prob).ToList();
                        var resultadosString = System.Linq.Enumerable.Select(paresOrdenados, p => $"{p.Nombre}: {Math.Round(p.Prob * 100, 2)}%");
                        
                        return string.Join(Environment.NewLine, resultadosString);
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error de predicción: {ex.Message}";
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
