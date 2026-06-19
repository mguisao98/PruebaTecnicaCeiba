📘 Eventos Vivos – Sistema de Gestión de Eventos y Reservas
Repositorio Principal

Este proyecto implementa una solución completa para la gestión de eventos, reservas, control de aforo y reportes analíticos para la plataforma Eventos Vivos.
Incluye:

Backend en .NET 8 (API RESTful)

Frontend en Angular 18 + Vite

Reglas de negocio avanzadas

Pruebas automatizadas

Arquitectura modular y mantenible

🏗️ Arquitectura General
El sistema está dividido en dos aplicaciones independientes:

Código
/EventosVivos.API      → Backend .NET 8 (REST API)
/EventosVivos.Web      → Frontend Angular 18 (SPA)
Ambas se comunican mediante HTTP usando rutas directas:

Código
https://localhost:7258/api
No se requiere proxy ni configuraciones adicionales.

🚀 1. Requisitos Previos
✔ Backend
.NET SDK 8.0+

Navegador moderno (para Swagger)

✔ Frontend
Node.js 18+

npm 9+

Angular CLI 17+

🔧 2. Ejecución del Backend (.NET 8)
Ubicación: /EventosVivos.API

1️⃣ Restaurar dependencias
bash
cd EventosVivos.API
dotnet restore
2️⃣ Ejecutar la API
bash
dotnet run
La API se ejecutará en:

Código
https://localhost:7258
3️⃣ Documentación Swagger
Disponible en:

Código
https://localhost:7258/swagger
🧠 3. Reglas de Negocio Implementadas
El backend implementa todas las reglas del documento técnico:

Validación de capacidad del venue

Prevención de superposición de horarios

Restricción nocturna (fines de semana)

Reservas tardías (menos de 1 hora)

Límites dinámicos de entradas

Estado automático del evento

Penalización por cancelación tardía

🧪 4. Pruebas Automatizadas (Backend)
bash
dotnet test
Incluyen validaciones de:

creación de eventos

reservas

confirmación y cancelación

reglas de negocio

reportes

🌐 5. Ejecución del Frontend (Angular 18)
Ubicación: /EventosVivos.Web

1️⃣ Instalar dependencias
bash
cd EventosVivos.Web
npm install
2️⃣ Ejecutar la aplicación
bash
ng serve -o
Se abrirá automáticamente:

Código
http://localhost:4200
🔌 6. Conexión Frontend ↔ Backend
Los servicios Angular ya están configurados con URLs directas:

ts
private readonly baseUrl = 'https://localhost:7258/api/Eventos';
Esto significa:

✔ No se necesita proxy
✔ No se edita angular.json
✔ No se usa ng serve --proxy-config

Solo debes:

Ejecutar backend

Ejecutar frontend

Listo

🗂️ 7. Estructura del Repositorio
Código
/
├── EventosVivos.API/        → Backend .NET 8
│     ├── Controllers/
│     ├── Application/
│     ├── Domain/
│     ├── Infrastructure/
│     └── Tests/
│
└── EventosVivos.Web/        → Frontend Angular 18
      ├── src/app/
      │     ├── core/        → servicios y modelos
      │     ├── features/    → componentes funcionales
      │     ├── pages/       → pantallas principales
      │     └── shared/      → componentes reutilizables
      └── assets/
🟢 8. Flujo Completo para Ejecutar Todo el Sistema
1️⃣ Backend
bash
cd EventosVivos.API
dotnet run
Debe mostrar:

Código
https://localhost:7258
2️⃣ Frontend
bash
cd EventosVivos.Web
npm install
ng serve -o
3️⃣ Abrir en navegador
Código
http://localhost:4200
📝 9. Tecnologías Utilizadas
Backend
.NET 8

Entity Framework Core InMemory

Swagger

FluentValidation

Arquitectura por capas

Frontend
Angular 18

Vite

RxJS

Angular Material (si aplica)

Vitest (pruebas)

🎯 10. Estado del Proyecto
✔ Cumple requerimientos funcionales
✔ Cumple reglas de negocio
✔ Backend + Frontend totalmente funcionales
✔ Pruebas automatizadas
✔ Documentación completa