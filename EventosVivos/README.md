---

### 2. 📁 README del Backend (Ubicación: `EventosVivos.API/README.md`)

```markdown
# Eventos Vivos Core API - Backend

Este proyecto representa el núcleo de servicios web (API RESTful) para el sistema de reservas de **Eventos Vivos**[cite: 10, 67]. Centraliza la lógica transaccional y valida el cumplimiento estricto de todas las directrices operativas del negocio[cite: 57].

## ⚙️ Requisitos para la Ejecución Local

### Dirección y Puerto Base
Por motivos de configuración de seguridad, políticas de CORS e integración directa con el cliente de Angular, la API está configurada por defecto para correr bajo **HTTPS** en la siguiente URL fija:
* **URL Base:** `https://localhost:7258`

### Instrucciones de Arranque
1. Asegúrate de estar dentro del directorio del backend:
   ```bash
   cd EventosVivos.API
Restaura los paquetes NuGet del proyecto:Bashdotnet restore
Compila y ejecuta la aplicación en modo desarrollo:Bashdotnet run
Una vez en ejecución, puedes explorar e interactuar directamente con los endpoints utilizando la interfaz de Swagger en:🔗 https://localhost:7258/swagger💡 Reglas de Negocio Implementadas (Core del Negocio)La API garantiza de manera automatizada las siguientes directrices extraídas de los requerimientos:RN-01 & RN-02 (Validación de Venue): Impide la creación de un evento que supere el aforo del recinto asignado. Bloquea de forma automática la superposición de horarios para eventos activos en un mismo lugar.  RN-03 (Horario Nocturno): Restringe el inicio de eventos en fines de semana (sábados y domingos) para que no comiencen después de las 22:00.  RN-04 (Reserva Tardía): No se admiten reservas si falta menos de 1 hora para el inicio del evento.  RN-05 & RF-03 (Límites Dinámicos de Transacción): Aplica un máximo de 5 entradas por transacción si restan menos de 24 horas para iniciar el evento , y un límite de 10 entradas si el precio supera los $100.  RN-06 (Estado Automático): Transiciona los eventos a estado completado de forma automática cuando el reloj del sistema supera la fecha de fin establecida.  RN-07 (Penalización de Cancelación): Si una reserva confirmada se cancela a menos de 48 horas del evento, se registra como "perdida" (no libera inventario para la venta y se aísla únicamente para reportes de auditoría).  🧪 Pruebas AutomatizadasSe dispone de una suite de pruebas unitarias enfocada en blindar la lógica de las reglas de negocio ante cualquier refactorización. Para ejecutarlas:  Bashdotnet test