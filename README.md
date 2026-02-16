# Gathering - Monorepo

Proyecto fullstack con backend en **ASP.NET Core API** y frontend en **Next.js**.

## 📁 Estructura del Proyecto

```
Gathering/
├── backend/                 # ASP.NET Core API
│   └── src/
│       ├── Gathering.Api/              # API endpoints
│       ├── Gathering.Application/      # Lógica de aplicación
│       ├── Gathering.Domain/           # Modelos de dominio
│       ├── Gathering.Infrastructure/   # Acceso a datos
│       └── Gathering.SharedKernel/     # Entidades compartidas
├── frontend/                # Next.js frontend
│   └── src/
│       ├── app/             # App directory (routing)
│       ├── components/      # Componentes React
│       └── lib/             # Utilidades compartidas
└── README.md
```

## 🚀 Requisitos Previos

- **Backend**: .NET 10.0 o superior
- **Frontend**: Node.js 18+ y pnpm

## ⚙️ Instalación y Configuración

### Backend

```bash
cd backend
dotnet restore
dotnet build
```

**Configuración de base de datos:**
- Editar `src/Gathering.Api/appsettings.Development.json` con la cadena de conexión

### Frontend

```bash
cd frontend
pnpm install
```

## 🏃 Ejecutar Localmente

### Backend
```bash
cd backend/src/Gathering.Api
dotnet run
```

La API estará disponible en `https://localhost:5001`

### Frontend
```bash
cd frontend
pnpm dev
```

El frontend estará disponible en `http://localhost:3000`

## 📝 Variables de Entorno

### Backend
- Crear `src/Gathering.Api/appsettings.Development.json` basado en `appsettings.json`
- Configurar conexión a base de datos y otros secretos

### Frontend
- Crear `.env.local` con las variables necesarias (ejemplo: `NEXT_PUBLIC_API_URL`)

## 🔐 Seguridad

- **No commitar** archivos `.env`, `appsettings.Development.json` ni archivos `.user`
- Los secretos deben almacenarse en variables de entorno o en servicios seguros
- Ver `.gitignore` para archivos automáticamente ignorados

## 📦 Tecnologías

### Backend
- ASP.NET Core 10
- Mediator pattern
- Entity Framework Core
- Arquitectura Clean

### Frontend
- Next.js 15
- TypeScript
- Tailwind CSS
- pnpm workspaces

## 🤝 Contribuciones

1. Crear una rama desde `main`: `git checkout -b feature/nombre-feature`
2. Realizar los cambios
3. Commit con mensajes descriptivos
4. Push a la rama
5. Crear Pull Request

## 📄 Licencia

[Especificar licencia]
