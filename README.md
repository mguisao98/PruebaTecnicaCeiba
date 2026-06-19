# Eventos Vivos - Sistema de Gestión y Reservas de Eventos

Este proyecto es la solución a la prueba técnica para el rol de **Desarrollador Fullstack**. Consiste en el diseño e implementación del núcleo de un sistema de reservas en tiempo real para una startup que organiza eventos culturales, conferencias y talleres. 

La aplicación resuelve de forma automatizada problemas críticos de negocio como el sobrecupo de aforos, los conflictos de horarios (*overlapping*) en los establecimientos y la gestión ineficiente de reservas manuales.

El ecosistema está desarrollado utilizando las últimas tecnologías solicitadas: **.NET 10** en el backend y **Angular 18** en la capa del frontend.

---

## 🚀 Arquitectura y Diseño de la Solución

Para el desarrollo del backend se ha optado por una **Arquitectura Limpia (Clean Architecture)** acoplada al patrón **CQRS (Command Query Responsibility Segregation)** a nivel de aplicación mediante el uso de **MediatR**. 

Esta decisión de diseño se justifica bajo los siguientes pilares de ingeniería de software requeridos para evaluar las capacidades de diseño:
1. **Desacoplamiento de Reglas de Negocio:** El núcleo del negocio (Dominio y Casos de Uso) está completamente aislado de detalles de infraestructura como las bases de datos, los controladores de la API o frameworks externos.
2. **Escalabilidad y Mantenibilidad:** Separar la lógica de consultas (Queries) de la lógica de mutación de datos (Commands) permite optimizar los flujos de lectura de eventos de manera independiente a las transacciones complejas de reservas.
3. **Principio de Responsabilidad Única (SRP):** Cada requerimiento funcional está encapsulado en su propio controlador/manejador (*Handler*), facilitando el mantenimiento del código a largo plazo y la creación de pruebas automatizadas aisladas.

### Capas del Sistema (Backend)
* **Domain:** Entidades puras (`Evento`, `Venue`, `Reserva`), objetos de valor y excepciones de dominio.
* **Application:** Casos de uso (`Commands` / `Queries`), validaciones de negocio con **FluentValidation** y abstracciones de interfaces.
* **Infrastructure:** Implementación de persistencia con **Entity Framework Core**, repositorios de datos y configuraciones del ORM.
* **API:** Controladores RESTful expuestos, inyección de dependencias y middlewares globales para el manejo unificado de errores y excepciones.

---

## 🛠️ Tecnologías Utilizadas

### Backend
* **Lenguaje y Framework:** .NET 10 (C#)
* **Persistencia:** Entity Framework Core (Base de datos relacional / En Memoria para testing)
* **Librerías Clave:** MediatR (CQRS), FluentValidation (Validaciones de inputs), AutoMapper (Mapeo de Entidades a DTOs)
* **Documentación:** Swagger / OpenAPI para la exploración y testeo de endpoints de la API

### Frontend
* **Framework:** Angular 18 (Arquitectura basada en componentes independientes / *Standalone Components*)
* **Gestión de Estado y Reactividad:** RxJS junto con las últimas características de **Signals** de Angular 18 para un renderizado y control de estado eficiente en tiempo real.
* **Diseño UI:** Maquetación responsiva moderna para facilitar la usabilidad por parte de usuarios y administradores.

---

## ⚙️ Instrucciones de Ejecución Local

### Prerrequisitos
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instalado.
* [Node.js](https://nodejs.org/) (Versión 18 o superior) junto con npm.
* [Angular CLI](https://angular.dev/cli) instalado de manera global (`npm install -g @angular/cli`).

---

### Paso 1: Configurar y Desplegar el Backend (.NET 10)

1. Abre tu terminal y desplázate al directorio del proyecto API:
   ```bash
   cd backend/src/EventosVivos.Api

Restaura los paquetes NuGet necesarios para el proyecto:
dotnet restore

Ejecuta el servidor del backend en modo de desarrollo:
dotnet run

El backend iniciará correctamente y estará escuchando peticiones en los puertos locales preconfigurados (ej. https://localhost:7258).

Puedes ingresar a la interfaz interactiva de la documentación de la API en la siguiente ruta de tu navegador:
https://localhost:7258/swagger/index.html

Paso 2: Configurar y Desplegar el Frontend (Angular 18)
Desde otra terminal independiente, navega hacia la carpeta del frontend de la aplicación:
cd frontend

Instala los paquetes y dependencias de desarrollo locales con npm:

Bash
npm install

Inicializa el servidor de desarrollo local de Angular:

Bash
ng serve --open

Tu navegador predeterminado se abrirá de forma automática en la dirección: http://localhost:4200 consumiendo activamente la API RESTful de .NET.

Pruebas Automatizadas
La solución incorpora una suite completa de pruebas unitarias automatizadas para blindar el núcleo de las reglas de negocio, asegurando que los flujos críticos no se rompan ante cambios en el código.

Para ejecutar los tests automatizados del backend, corre el siguiente comando desde la raíz del proyecto:
dotnet test


Requerimientos Funcionales y Reglas de Negocio Cubiertas
El sistema implementa de extremo a extremo las siguientes funcionalidades visualizadas e integradas en la API:

Gestión de Eventos (/api/Eventos)
POST /api/Eventos: Creación de un evento validando título, descripción, fecha futura y asegurando que la capacidad máxima ingresada no exceda la capacidad estructural del local asignado (RN-01).

GET /api/Eventos / /filtrar: Listado integral de eventos soportando búsquedas parciales e insensibles a mayúsculas/minúsculas (case-insensitive) y filtros dinámicos por tipo de evento, venue, rango de fechas y estado (RF-02).

GET /api/Eventos/reporte-ocupacion/{eventoId}: Generación de reportes métricos en tiempo real detallando total de entradas vendidas, disponibles, porcentaje exacto de ocupación y total de ingresos financieros recaudados (RF-06).

Gestión de Reservas (/api/Reservas)
POST /api/Reservas: Registro de reservas en estado inicial pendiente_pago validando stock real, formato de email y la restricción transaccional de un máximo de 5 entradas si el evento inicia en menos de 24 horas (RF-03).

PUT /api/Reservas/confirmar/{id}: Confirmación administrativa de pagos que genera de forma automática un código alfanumérico único bajo el formato estricto EV-{6 dígitos} (RF-04).

PUT /api/Reservas/cancelar/{id}: Cancelación flexible de la reserva liberando de manera inmediata las entradas al inventario del evento general (RF-05), controlando la penalización de pérdida si ocurre con menos de 48 horas de anticipación (RN-07).

Gestión de Sedes (/api/Venues)
Mantenimiento base de locaciones (Auditorio Central, Sala Norte, Arena Sur) para el correcto control estructural de aforos y prevención de colisión de horarios entre eventos concurrentes (RN-02).



