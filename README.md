# Eventos Vivos - Sistema de Reservas y Gestión Cultural

[cite_start]**Eventos Vivos** es una solución web fullstack modular diseñada para automatizar la gestión de eventos, controlar el aforo en tiempo real y mitigar los problemas de sobreventa o conflictos de horarios en recintos culturales[cite: 5, 6, 7, 8].

La solución se compone de dos aplicaciones totalmente desacopladas:
1. [cite_start]**EventosVivos.API (Backend):** Construido en .NET Core[cite: 3]. [cite_start]Actúa como el núcleo transaccional, exponiendo servicios RESTful y asegurando el cumplimiento estricto de las reglas de negocio[cite: 10, 57, 67].
2. [cite_start]**EventosVivos.Web (Frontend):** Una Single Page Application (SPA) desarrollada en Angular que consume la API para ofrecer una interfaz administrativa y de usuario intuitiva[cite: 68].

---

## 📐 Estructura del Proyecto

```text
📁 eventos-vivos-root/
 ├── 📁 EventosVivos.API/    # Proyecto Backend (.NET Core)
 └── 📁 EventosVivos.Web/    # Proyecto Frontend (Angular)
⚙️ Requisitos PreviosAntes de comenzar, asegúrate de tener instalado en tu entorno local:.NET SDK 8.0+ (o la última versión estable)Node.js (v18.x o superior recomendado) y npmAngular CLI instalado globalmente (npm install -g @angular/cli)🚀 Ejecución Rápida del Sistema CompletoPara levantar toda la plataforma localmente, sigue estos pasos en orden:Paso 1: Levantar el BackendAbre una terminal y navega a la carpeta del backend:Bashcd EventosVivos.API
Ejecuta el proyecto:Bashdotnet run
El servicio web se iniciará de forma segura en: https://localhost:7258.  Paso 2: Levantar el FrontendAbre una nueva terminal y navega a la carpeta del frontend:Bashcd EventosVivos.Web
Instala las dependencias necesarias de Node:Bashnpm install
Inicia el servidor de desarrollo:Bashng serve
Accede a la aplicación desde tu navegador en: http://localhost:4200.🛠️ Arquitectura y Justificación TécnicaClean Architecture / N-Capas: Aplicada en el Backend para aislar las reglas de negocio críticas (como la prevención de solapamiento de horarios o penalizaciones por cancelación) de la persistencia de datos y de la capa de presentación.  Persistencia Desacoplada: Implementación del patrón Repository que facilita el uso de bases de datos en memoria para testing y desarrollo ágil, con la flexibilidad de migrar a SQL Server o PostgreSQL sin alterar el dominio.  Diseño Orientado a Componentes y Servicios: En el frontend se modularizan las vistas de creación, listado y reportes utilizando servicios inyectables de Angular para centralizar la comunicación HTTP con tipado fuerte en TypeScript.  