# PanelPedidos

API en .NET 10 (controladores + servicios + repositorios con Dapper) para
inventario, productos, pedidos y ganancias. PostgreSQL en Supabase.

## Estructura
- `PanelPedidos.Api/` API (Controladores, Servicios, Repositorios, Datos, Entidades, Dtos, Seguridad, Middleware, Configuracion)
- `PanelPedidos.Pruebas/` pruebas unitarias (xUnit + NSubstitute)
- `BaseDeDatos/` scripts SQL (ejecutar en orden en el SQL Editor de Supabase)

## Primer arranque
```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project PanelPedidos.Api
# probar: GET http://localhost:<puerto>/api/salud
```

## Secretos
Nada sensible en `appsettings.json`. Usar variables de entorno
(doble guion bajo para anidar), por ejemplo:
`ConnectionStrings__Principal`, `Jwt__Clave`.
En desarrollo: `dotnet user-secrets`.
