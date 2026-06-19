---

### 3. 📁 README del Frontend (Ubicación: `EventosVivos.Web/README.md`)

```markdown
# Eventos Vivos Web - Frontend

Esta aplicación es el cliente web para **Eventos Vivos**, estructurada como una SPA (Single Page Application) reactiva mediante **Angular**[cite: 68]. Su objetivo es proveer interfaces de usuario fluidas y validadas para el registro, control de reservas y visualización de reportes analíticos.

## 💻 Características Implementadas

* **Dashboard de Eventos (RF-02):** Panel visual con filtros combinados por tipo de evento, rangos de fechas, recintos (*venues*) y búsqueda de texto predictiva (case-insensitive)[cite: 23, 24, 25, 26, 27, 28, 29].
* **Formularios Reactivos Validados (RF-01):** Interfaz estricta para la creación de eventos, validando en tiempo real las longitudes de los textos (Título: 5-100, Descripción: 10-500) y consistencia en el orden cronológico de las fechas[cite: 12, 13, 14, 16, 19, 20].
* **Gestión de Reservas y Pagos (RF-03, RF-04, RF-05):** Flujos interactivos para que el usuario reserve entradas (con validaciones de formato de correo y cantidades) [cite: 30, 31, 32, 34, 35] y paneles de administración rápidos para confirmar pagos o tramitar cancelaciones[cite: 38, 39, 44, 45].
* **Panel de Reportes (RF-06):** Pantalla dedicada que consume los cálculos analíticos de ocupación, entradas remanentes e ingresos totales capturados[cite: 50, 51, 52, 53, 54, 55].

---

## 🔧 Configuración de la API Externa

La comunicación con los servicios del servidor está centralizada en los archivos de entorno de la SPA (`src/environments/`). Por defecto, la aplicación apunta a la URL segura local del backend:

* **Endpoint Destino:** `https://localhost:7258/api`

*Nota: Si necesitas alterar el puerto de ejecución del Backend, recuerda reflejar dicho cambio en el archivo `environment.ts`.*

---

## 🚀 Instrucciones de Desarrollo Local

### Instalación
Descarga todos los módulos e integraciones del ecosistema de Node instalados en el proyecto ejecutando:
```bash
npm install
Servidor de DesarrolloLevanta la aplicación localmente mediante el CLI de Angular:Bashng serve
Una vez compilado correctamente, abre tu navegador web e ingresa a: http://localhost:4200. El entorno cuenta con Hot Reload, lo que significa que cualquier cambio guardado se refrescará automáticamente en pantalla.Generar Artefactos de Producción (Build)Para compilar y optimizar la aplicación web reduciendo el peso de los bundles de cara a un despliegue en la nube, ejecuta:  Bashng build
Los archivos optimizados resultantes se depositarán ordenadamente en la carpeta interna dist/.🧪 Pruebas UnitariasLa verificación de componentes, estados de carga y directivas de renderizado se gestiona mediante el test runner de Vitest para garantizar una ejecución veloz. Lánzalos usando:  Bashng test
