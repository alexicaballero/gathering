# Guía de Contribución

Agradecemos tu interés en contribuir a este proyecto. Por favor, sigue estas directrices:

## Antes de Empezar

1. Haz un fork del repositorio
2. Clona tu fork: `git clone https://github.com/tu-usuario/gathering.git`
3. Crea una rama para tu feature: `git checkout -b feature/descripcion`

## Configuración del Entorno de Desarrollo

### Backend
```bash
cd backend
dotnet restore
dotnet build
```

### Frontend
```bash
cd frontend
pnpm install
```

## Estándares de Código

### C# (.NET)
- Seguir las convenciones estándar de C#
- Usar PascalCase para nombres de clases y métodos
- Usar camelCase para variables locales
- Ver `.editorconfig` para indentación y formato

### TypeScript/JavaScript
- Follower Prettier para formato
- Usar camelCase para variables y funciones
- Usar PascalCase para componentes React
- El `.editorconfig` será aplicado automáticamente

## Proceso de Commit

1. Asegúrate de que tu código cumple con los estándares
2. Escribe mensajes de commit claros y descriptivos:
   - ✅ `feat: agregar validación de emails`
   - ✅ `fix: corregir error en consultas de comunidades`
   - ✅ `docs: actualizar README`
3. No commitear archivos de configuración local (`.env`, `appsettings.Development.json`, etc.)

## Pull Requests

1. Actualiza tu rama desde `main`: `git pull origin main`
2. Haz push de tu rama: `git push origin feature/descripcion`
3. Abre un Pull Request en GitHub
4. Describe claramente:
   - Qué cambios realizaste
   - Por qué son necesarios
   - Cómo se pueden probar

## Reglas Importantes

- ❌ **NO** guardar secretos, contraseñas o tokens
- ❌ **NO** commitear archivos de build (bin/, obj/, node_modules/)
- ✅ **SÍ** probar los cambios localmente antes de hacer PR
- ✅ **SÍ** documentar cambios significativos

## Preguntas o Problemas

Si tienes dudas, abre un issue o contacta a los mantenedores.

¡Gracias por contribuir! 🚀
