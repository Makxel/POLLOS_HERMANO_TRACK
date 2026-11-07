# PollosTrack 🐥

Proyecto de escritorio para gestionar mesas, comandas, cocina, facturación e inventario de un restaurante. Desarrollado en **C# con Windows Forms, .NET 10 y SQL Server**.

## Requisitos

- Windows.
- Visual Studio compatible con .NET 10 y la carga de trabajo **Desarrollo de escritorio de .NET**.
- SDK de .NET 10.
- SQL Server local y SQL Server Management Studio (SSMS) para preparar la base de datos.

## Cómo ejecutarlo

1. Clonar o descargar el repositorio y abrir `PollosTrack.slnx` en Visual Studio.
2. Crear la base `POLLOS_HERMANOS_TRACK` mediante el script `PollosTrack/Utils/ESTRUCTURA_TABLAS.sql`. Revisarlo antes de ejecutarlo: está destinado a una base nueva.
3. Cargar los datos necesarios: estados por entidad, cliente **Consumidor Generico**, mesas, categorías, productos e inventario. El script de estructura no sustituye esta carga inicial.
4. Ajustar el servidor y la autenticación de SQL Server en las conexiones locales:
   - `PollosTrack/Utils/Conexion.cs` para Catálogos e inicio de sesión.
   - `PollosTrack/Utils/conexion_sql_server.cs` para POS.
   - La cadena de conexión de `PollosTrack/Inventario/Forms/UC_Inventario.cs` para Inventario.
5. Restaurar los paquetes NuGet, compilar y ejecutar con **F5**.

Todas las conexiones deben apuntar a la misma base. Si falta `conexion_sql_server.cs`, se necesita esa clase compartida para compilar los formularios POS.

## Paquetes NuGet

Ya están declarados en el proyecto; basta con **restaurarlos**, sin instalarlos uno por uno.

| Paquete | Versión | Uso |
| --- | --- | --- |
| Microsoft.Data.SqlClient | 7.1.1 | Conexión con SQL Server |
| QuestPDF | 2026.9.1 | Generación de recibos PDF |
| PdfiumViewer | 2.13.0 | Renderizado del PDF para su vista previa |
| PdfiumViewer.Native.x86_64.v8-xfa | 2018.4.8.256 | Biblioteca nativa de PDF para x64 |
| PdfiumViewer.Native.x86.v8-xfa | 2018.4.8.256 | Biblioteca nativa de PDF para x86 |

También se pueden restaurar desde la raíz del repositorio:

```powershell
dotnet restore PollosTrack/PollosTrack.csproj
```

El proyecto copia las DLL nativas de PDFium a las carpetas `x64` y `x86` de la salida. Deben conservarse al distribuir el programa. PdfiumViewer puede mostrar una advertencia de compatibilidad por ser un paquete antiguo.

## Primer acceso

Si la tabla de usuarios está vacía, el inicio de sesión intenta crear el administrador de prueba: usuario `admin`, contraseña `Admin123*`. Cambiarla después de las pruebas. Si ya existe cualquier usuario, este administrador no se crea automáticamente.

## Módulos

- **POS:** mapa de mesas, comandas, monitor de pendientes, facturación, recibos e historial.
- **Inventario:** existencias, ingreso de mercancía y kardex.
- **Catálogos:** categorías y productos.

El stock se reserva al agregar productos y se descuenta al confirmar su entrega en cocina. El cobro no vuelve a descontarlo. El control actual es por producto vendido; no incluye recetas ni cálculo de ingredientes.

## Organización

```text
PollosTrack/
├── POS/               # Mesas, pedidos y facturación
├── Inventario/        # Existencias y movimientos
├── Catalogos/         # Catálogos e inicio de sesión
├── Utils/             # Conexiones, sesión, helpers y script SQL
└── FormPrincipal.cs   # Menú y contenedor de las vistas
```
