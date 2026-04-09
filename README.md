# Retinox

Aplicación de escritorio desarrollada en **WPF (.NET Framework 4.7.2)** para apoyar el análisis de imágenes de retina mediante un modelo de inferencia **ONNX**. El sistema permite registrar pacientes, seleccionar una imagen funduscópica, ejecutar una predicción automática, guardar el resultado en un historial local y generar un reporte en PDF.

## Descripción general del proyecto

Retinox es una herramienta de apoyo clínico orientada a la detección de retinopatía. Su objetivo principal es facilitar el flujo de trabajo de análisis de pacientes a partir de una imagen de retina y mostrar un resultado automático con base en un modelo entrenado previamente.

### Funcionalidades principales

- Registro y búsqueda de pacientes.
- Carga de imágenes de retina desde disco.
- Inferencia local con un modelo ONNX.
- Persistencia de pacientes e historial mediante SQLite.
- Exportación del resultado a un reporte PDF.
- Interfaz de escritorio organizada en vistas separadas para navegación por módulos.

### Flujo de trabajo

1. El usuario registra o selecciona un paciente.
2. Carga una imagen de retina en formato compatible.
3. La aplicación ejecuta la predicción local con el modelo `modelo_odir.onnx`.
4. El resultado se guarda en la base de datos y en el historial.
5. El reporte puede exportarse a PDF desde la pantalla de resultados.

## Dataset utilizado

### Fuente

El proyecto se basa en el **Diabetic Retinopathy Resized Dataset**, una versión redimensionada del dataset de retinografías usado en la competencia de Kaggle para clasificación de retinopatía diabética.

### Características relevantes

- **Tipo de datos:** imágenes médicas de fondo de ojo (fundus retinal images).
- **Tamaño de referencia:** en la versión citada en la literatura, el conjunto redimensionado contiene **35,100 imágenes por carpeta** (`resized_train` y `resized_train_cropped`).
- **Clases:** **5 niveles de severidad**:
  - `level_0`: normal
  - `level_1`: retinopatía ligera
  - `level_2`: retinopatía moderada
  - `level_3`: retinopatía severa
  - `level_4`: retinopatía proliferativa
- **Formato de imágenes:** el proyecto acepta archivos `.jpg`, `.jpeg` y `.png`.
- **Preprocesamiento:** las imágenes se convierten a RGB y se redimensionan automáticamente al tamaño esperado por el modelo durante la inferencia.
- **Tamaño de entrada del modelo:** `320 x 320` píxeles en la configuración actual.

### Consideraciones de uso

- El repositorio incluye el modelo ya exportado a ONNX, por lo que no es necesario reentrenar para ejecutar inferencias.
- El archivo `clases.txt` define las etiquetas utilizadas por el modelo al mostrar los resultados.
- Si se reemplaza el modelo, debe mantenerse la compatibilidad con el preprocesamiento y el orden de clases.

## Estructura del proyecto

```text
Retinox/
├── App.xaml
├── App.xaml.cs
├── App.config
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Retinox.csproj
├── Retinox.slnx
├── modelo_odir.onnx
├── clases.txt
├── Model/
│   ├── Paciente.cs
│   ├── AnalisisHistorial.cs
│   └── AppDatabase.cs
├── Services/
│   ├── OnnxModelEvaluator.cs
│   └── PdfGeneratorService.cs
├── View/
│   ├── InicioView.xaml
│   ├── AnalisisView.xaml
│   ├── ResultadoView.xaml
│   ├── PacientesView.xaml
│   ├── HistorialView.xaml
│   ├── NuevoPacienteView.xaml
│   └── InfoView.xaml
├── ViewModel/
│   ├── ViewModelBase.cs
│   ├── RelayCommand.cs
│   ├── NuevoPacienteViewModel.cs
│   ├── PacientesViewModel.cs
│   ├── AnalisisViewModel.cs
│   ├── ResultadoViewModel.cs
│   └── HistorialViewModel.cs
└── Properties/
    ├── AssemblyInfo.cs
    ├── Resources.resx
    ├── Resources.Designer.cs
    ├── Settings.settings
    └── Settings.Designer.cs
```

### Componentes principales

- **Model/**: entidades y acceso a datos local.
- **Services/**: inferencia ONNX y generación de PDF.
- **View/**: interfaz gráfica WPF.
- **ViewModel/**: lógica de presentación bajo patrón MVVM.
- **modelo_odir.onnx**: modelo entrenado para clasificación.
- **clases.txt**: etiquetas legibles para mostrar los resultados en la interfaz.

## Dependencias y librerías

El proyecto utiliza las siguientes dependencias principales:

- **Microsoft.ML.OnnxRuntime**: ejecución de inferencia con el modelo ONNX.
- **SixLabors.ImageSharp**: carga y redimensionamiento de imágenes.
- **sqlite-net-pcl**: persistencia local en SQLite.
- **SQLitePCLRaw.bundle_e_sqlite3**: proveedor nativo requerido por SQLite.
- **PdfSharp**: creación de reportes en PDF.

### Requisitos del entorno

- Windows.
- Visual Studio con soporte para **WPF** y **.NET Framework 4.7.2**.
- Plataforma objetivo: **x64**.

## Instrucciones de uso

### 1. Requisitos previos

Instala lo siguiente antes de compilar:

- **Visual Studio 2022** o una versión compatible con WPF en .NET Framework.
- **.NET Framework 4.7.2 Developer Pack**.
- Workload de **Desktop development with .NET**.

### 2. Abrir el proyecto

1. Descomprime el repositorio.
2. Abre `Retinox.slnx` en Visual Studio.
3. Espera a que se restauren los paquetes NuGet.
4. Compila el proyecto en configuración **Debug** o **Release** para **x64**.

### 3. Ejecutar la aplicación

Puedes ejecutar la aplicación desde Visual Studio con **F5** o desde el binario generado en:

```text
bin/Debug/net472/Retinox.exe
```

### 4. Uso básico

#### Registrar un paciente
- Abre la sección **Nuevo paciente**.
- Captura nombre, NSS y edad.
- Guarda la información en la base de datos local.

#### Realizar un análisis
- Ve a **Análisis**.
- Selecciona un paciente registrado.
- Carga una imagen de retina.
- Ejecuta la predicción para obtener el resultado.

#### Exportar reporte
- En la pantalla de resultados, usa **Exportar a PDF**.
- El archivo se guarda automáticamente en el **Escritorio**.

#### Revisar historial
- En **Historial** puedes consultar los análisis previos.
- También es posible limpiar el historial desde la interfaz.

## Persistencia local

La aplicación crea automáticamente una base de datos SQLite en la carpeta local del usuario:

```text
%LOCALAPPDATA%\Retinox.db3
```

Ahí se almacenan:

- Pacientes.
- Historial de análisis.

## Notas importantes

- Si el archivo `modelo_odir.onnx` no está disponible en la carpeta de salida, la aplicación entra en un modo de prueba para conservar la usabilidad de la interfaz.
- El archivo `clases.txt` debe permanecer junto al ejecutable para que las etiquetas se muestren correctamente.
- Los reportes PDF incluyen datos del paciente, el resultado de la IA y la imagen analizada.

## Resultado esperado

Al finalizar la ejecución normal, el usuario debe poder:

- Administrar pacientes.
- Analizar imágenes de retina.
- Consultar diagnósticos previos.
- Generar reportes PDF de los análisis realizados.
