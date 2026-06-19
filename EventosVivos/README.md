📘 Eventos Vivos Core API — Backend
Ubicación: EventosVivos.API/README.md

Este proyecto implementa el núcleo de servicios RESTful para el sistema de reservas Eventos Vivos.
Centraliza la lógica de negocio, validaciones y reglas operativas que garantizan la integridad del sistema.

⚙️ 1. Requisitos para la ejecución local
✔️ Dependencias necesarias
.NET SDK 8.0+

Git (opcional)

Navegador moderno (para Swagger)

✔️ Dirección y puerto base
La API está configurada para ejecutarse por defecto en:

Código
https://localhost:7258
Esto permite integración directa con el frontend Angular sin necesidad de configurar CORS adicionales.

🚀 2. Cómo ejecutar el backend
1️⃣ Entrar al directorio del backend
bash
cd EventosVivos.API
2️⃣ Restaurar dependencias
bash
dotnet restore
3️⃣ Ejecutar la API
bash
dotnet run
Si todo está correcto, verás algo como:

Código
Now listening on: https://localhost:7258
📚 3. Documentación Swagger
Puedes explorar y probar todos los endpoints desde:

Código
https://localhost:7258/swagger
Incluye:

Crear eventos

Listar y filtrar

Crear reservas

Confirmar pagos

Cancelar reservas

Reportes de ocupación

🧠 4. Reglas de negocio implementadas
La API implementa automáticamente todas las reglas del documento de requerimientos:

🟦 RN-01 & RN-02 — Validación de Venue
No permite crear eventos que excedan la capacidad del venue.

Impide superposición de horarios en el mismo venue.

🟦 RN-03 — Restricción nocturna
Eventos en sábado o domingo no pueden iniciar después de las 22:00.

🟦 RN-04 — Reserva tardía
No se permiten reservas si falta menos de 1 hora para el evento.

🟦 RN-05 — Límites dinámicos de entradas
Máximo 5 entradas si faltan menos de 24 horas.

Máximo 10 entradas si el precio supera $100.

🟦 RN-06 — Estado automático
Un evento pasa a completado cuando su hora de fin ya pasó.

🟦 RN-07 — Cancelación con penalización
Si una reserva confirmada se cancela con menos de 48 horas, se marca como perdida (no libera cupos).

🧪 5. Pruebas automatizadas
El proyecto incluye pruebas unitarias para validar reglas de negocio y flujos principales.

Para ejecutarlas:

bash
dotnet test
🔌 6. Endpoints principales
Recurso	Método	Ruta
Listar eventos	GET	/api/Eventos
Filtrar eventos	GET	/api/Eventos/filtrar
Crear evento	POST	/api/Eventos
Reporte	GET	/api/Eventos/{id}/reporte-ocupacion
Crear reserva	POST	/api/Reservas
Confirmar pago	PUT	/api/Reservas/confirmar/{id}
Cancelar reserva	PUT	/api/Reservas/cancelar/{id}
Listar venues	GET	/api/Venues


🟢 7. Integración con el frontend
El frontend Angular ya está configurado para consumir directamente:

Código
https://localhost:7258/api
Por lo tanto:

Primero ejecuta el backend

Luego ejecuta el frontend con:

bash
ng serve -o
No requiere proxy ni configuraciones adicionales.