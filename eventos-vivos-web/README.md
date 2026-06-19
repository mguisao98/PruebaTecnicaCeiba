📘 Eventos Vivos Web — Frontend
Ubicación: EventosVivos.Web/README.md

Este proyecto es la interfaz web del sistema Eventos Vivos, desarrollado con Angular 18 + Vite.
Permite gestionar eventos, reservas, reportes y administración general del sistema.

🚀 1. Requisitos previos
Antes de ejecutar el frontend, asegúrate de tener instalado:

Node.js 18+

npm 9+

Angular CLI 17+

Verifica con:

bash
node -v
npm -v
ng version
📦 2. Instalación de dependencias
En la carpeta del frontend:

bash
npm install
Esto descargará todos los módulos necesarios para ejecutar la aplicación.

▶️ 3. Ejecutar la aplicación
Una vez instaladas las dependencias:

bash
ng serve -o
Esto abrirá automáticamente:

Código
http://localhost:4200
La aplicación usa Hot Reload, por lo que cualquier cambio se refleja al instante.

🔌 4. Conexión con el Backend
El frontend está configurado para consumir directamente la API en:

Código
https://localhost:7258/api
Esto significa:

✔ No necesitas configurar proxy
✔ No necesitas editar archivos
✔ No necesitas parámetros adicionales en ng serve

Solo asegúrate de que el backend esté ejecutándose antes de abrir el frontend:

bash
dotnet run
Y que muestre:

Código
Now listening on: https://localhost:7258
🗂 5. Estructura del proyecto
Código
src/
 ├── app/
 │    ├── core/
 │    │     ├── services/   → Servicios HTTP (Eventos, Reservas, Venues)
 │    │     └── models/     → Modelos y DTOs
 │    ├── features/          → Componentes funcionales
 │    ├── pages/             → Páginas principales
 │    └── shared/            → Componentes reutilizables
 ├── assets/
 └── index.html
🔧 6. Servicios HTTP (ya configurados)
Los servicios consumen directamente la API:

ts
private readonly baseUrl = 'https://localhost:7258/api/Eventos';
Esto garantiza que el evaluador pueda ejecutar el proyecto sin configurar nada adicional.

🧪 7. Pruebas unitarias
El proyecto usa Vitest.

Para ejecutarlas:

bash
ng test
🟢 8. Flujo recomendado para ejecutar todo el sistema
Backend

bash
dotnet run
Confirmar que está en https://localhost:7258.

Frontend

bash
npm install
ng serve -o
Abrir:

Código
http://localhost:4200
Y listo: el sistema queda completamente funcional.