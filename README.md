# API de Gestión de Envíos

Microservicio RESTful de acceso público para la gestión logística de envíos: generación de etiquetas PDF con código de barras Code128, registro de entrega a la transportadora y rastreo por número de guía.

**Producción:** https://api-gestion-envios-f5eue7c8fudxc2b3.mexicocentral-01.azurewebsites.net/swagger

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/v1/envios/generar-etiqueta` | Crea el envío, asigna guía y genera la etiqueta PDF |
| `POST` | `/api/v1/envios/{id}/entregar-transportadora` | Registra la entrega y cambia el estado a `EN_TRANSITO` |
| `GET`  | `/api/v1/envios/rastrear/{numeroGuia}` | Consulta estado e historial de eventos |
| `GET`  | `/api/v1/envios/{id}/descargar-etiqueta` | Descarga el PDF de la etiqueta |
| `GET`  | `/api/v1/envios?pagina=1&porPagina=20` | Lista paginada de envíos |

Flujo de estados: `ETIQUETA_GENERADA` → `EN_TRANSITO` → `ENTREGADO` (o `CANCELADO`).

## Tecnologías

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core (Code-First) + SQL Server / Azure SQL
- QuestPDF (etiquetas) y ZXing.Net + SkiaSharp (código de barras Code128)
- Swagger / OpenAPI
- Despliegue en Azure App Service

La transportadora es simulada (`ServicioTransportadoraSimulada` detrás de `IServicioTransportadora`), por lo que no requiere contratos con paqueterías reales.

## Ejecución local

Requisitos: .NET 8 SDK y SQL Server LocalDB (incluido con Visual Studio).

```bash
git clone <url-del-repositorio>
cd GestionEnvios.Api/GestionEnvios.Api
dotnet run
```

Las migraciones se aplican automáticamente al iniciar. Swagger queda disponible en `https://localhost:{puerto}/swagger`.

La cadena de conexión local está en `appsettings.json` (`ConexionPrincipal`). Para Azure SQL, define `ConexionPrincipal` como variable de entorno o *connection string* del App Service; no la subas al repositorio.

## Estructura

```text
GestionEnvios.Api/
├── Controladores/          EnviosControlador
├── Dominio/                Modelos (Envio, HistorialEnvio) y constantes de estado
├── ModelosTransferencia/   DTOs de solicitud y respuesta
├── Infraestructura/
│   ├── Datos/              ContextoBaseDatos (EF Core)
│   └── Servicios/          Transportadora simulada y generador de etiquetas PDF
├── Migrations/
└── Program.cs
```
