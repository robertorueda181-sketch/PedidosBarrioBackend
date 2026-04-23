# Push Notifications Endpoint - Documentación Completa

## 📱 Descripción General

Sistema completo de notificaciones push mediante **Firebase Cloud Messaging (FCM)** que permite:
- ✅ Registrar tokens de dispositivos
- ✅ Enviar notificaciones masivas a todos los dispositivos
- ✅ Enviar notificaciones a empresas específicas
- ✅ Enviar notificaciones a clientes específicos
- ✅ Enviar notificaciones a tópicos
- ✅ Gestionar suscripciones a tópicos

## 🔧 Componentes Implementados

### 1. **Entidad: DeviceToken**
Almacena información de los tokens FCM de los dispositivos.

```csharp
public class DeviceToken
{
    public int Id { get; set; }
    public string Token { get; set; }              // Token FCM único
    public int? ClienteId { get; set; }            // Cliente asociado (opcional)
    public Guid? EmpresaId { get; set; }           // Empresa asociada (opcional)
    public string? Platform { get; set; }          // iOS, Android, Web
    public string? DeviceId { get; set; }          // ID único del dispositivo
    public bool IsActive { get; set; }             // Token activo
    public DateTime RegisteredDate { get; set; }   // Fecha de registro
    public DateTime? LastUsedDate { get; set; }    // Último uso
}
```

### 2. **Repositorio: IDeviceTokenRepository**
Interfaz para acceder a los datos de tokens de dispositivos.

### 3. **Servicio: IFirebaseMessagingService**
Servicio para interactuar con Firebase Cloud Messaging.

### 4. **Endpoints: PushNotificationEndpoint**
Endpoints REST para gestionar notificaciones.

---

## 🔗 Endpoints Disponibles

### 1. Registrar Token de Dispositivo
**POST** `/api/Notificaciones/Push/registrar`

```json
{
  "token": "c_x1MjZRJhJw:APA91bFST...",
  "clienteId": 123,
  "empresaId": "550e8400-e29b-41d4-a716-446655440000",
  "platform": "Android",
  "deviceId": "device-uuid-123456"
}
```

**Respuesta (200 OK):**
```json
{
  "success": true,
  "message": "Token registrado exitosamente (ID: 1)",
  "successCount": 0,
  "failureCount": 0
}
```

---

### 2. Enviar Notificación Push Masiva
**POST** `/api/Notificaciones/Push/enviar`

#### Opción A: Enviar a TODOS los dispositivos
```json
{
  "title": "Nuevo producto disponible",
  "body": "Tenemos nuevos productos en nuestro catálogo",
  "targetType": "all",
  "data": {
    "category": "products",
    "action": "open_catalog"
  }
}
```

#### Opción B: Enviar a una EMPRESA específica
```json
{
  "title": "Notificación para tu negocio",
  "body": "Tu pedido ha sido confirmado",
  "targetType": "empresa",
  "empresaId": "550e8400-e29b-41d4-a716-446655440000"
}
```

#### Opción C: Enviar a un CLIENTE específico
```json
{
  "title": "Estado de tu pedido",
  "body": "Tu pedido está listo para retirar",
  "targetType": "cliente",
  "clienteId": 123
}
```

#### Opción D: Enviar a un TOKEN específico
```json
{
  "title": "Prueba de notificación",
  "body": "Esta es una notificación de prueba",
  "targetType": "token",
  "token": "c_x1MjZRJhJw:APA91bFST..."
}
```

#### Opción E: Enviar a un TÓPICO
```json
{
  "title": "Ofertas especiales",
  "body": "Tenemos descuentos especiales para ti",
  "targetType": "topic",
  "topic": "premium_users"
}
```

**Respuesta (200 OK):**
```json
{
  "success": true,
  "message": "Notificaciones enviadas",
  "successCount": 150,
  "failureCount": 5
}
```

---

### 3. Desuscribir Token
**POST** `/api/Notificaciones/Push/desuscribir`

```json
{
  "token": "c_x1MjZRJhJw:APA91bFST..."
}
```

**Respuesta (200 OK):**
```json
{
  "success": true,
  "message": "Token desuscrito exitosamente"
}
```

---

### 4. Verificar Estado del Token
**GET** `/api/Notificaciones/Push/estado/{token}`

**Respuesta (200 OK):**
```json
{
  "isActive": true,
  "platform": "Android",
  "registeredDate": "2024-01-15T10:30:00Z",
  "lastUsedDate": "2024-01-15T12:45:00Z",
  "clienteId": 123,
  "empresaId": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

## 📚 Ejemplos de Uso

### JavaScript/React
```javascript
// 1. Registrar token
async function registerDeviceToken(token, platform) {
  const response = await fetch('/api/Notificaciones/Push/registrar', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      token: token,
      platform: platform || 'Web',
      deviceId: navigator.hardwareConcurrency
    })
  });
  return await response.json();
}

// 2. Enviar notificación masiva
async function sendNotificationToAll(title, body) {
  const response = await fetch('/api/Notificaciones/Push/enviar', {
    method: 'POST',
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${your_auth_token}`
    },
    body: JSON.stringify({
      title: title,
      body: body,
      targetType: 'all'
    })
  });
  return await response.json();
}

// 3. Enviar a empresa específica
async function sendToEnterprise(empresaId, title, body) {
  const response = await fetch('/api/Notificaciones/Push/enviar', {
    method: 'POST',
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${your_auth_token}`
    },
    body: JSON.stringify({
      title: title,
      body: body,
      targetType: 'empresa',
      empresaId: empresaId
    })
  });
  return await response.json();
}
```

### cURL
```bash
# 1. Registrar token
curl -X POST http://localhost:5000/api/Notificaciones/Push/registrar \
  -H "Content-Type: application/json" \
  -d '{
    "token": "c_x1MjZRJhJw:APA91bFST...",
    "platform": "Android"
  }'

# 2. Enviar notificación a todos
curl -X POST http://localhost:5000/api/Notificaciones/Push/enviar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "title": "¡Hola!",
    "body": "Mensaje de prueba",
    "targetType": "all"
  }'

# 3. Desuscribir
curl -X POST http://localhost:5000/api/Notificaciones/Push/desuscribir \
  -H "Content-Type: application/json" \
  -d '{"token": "c_x1MjZRJhJw:APA91bFST..."}'
```

### PowerShell
```powershell
# Registrar token
$token = @{
    token = "c_x1MjZRJhJw:APA91bFST..."
    platform = "Android"
}

Invoke-RestMethod -Uri "http://localhost:5000/api/Notificaciones/Push/registrar" `
    -Method Post `
    -Headers @{'Content-Type' = 'application/json'} `
    -Body ($token | ConvertTo-Json)

# Enviar notificación
$notification = @{
    title = "Notificación de prueba"
    body = "Este es un mensaje de prueba"
    targetType = "all"
}

Invoke-RestMethod -Uri "http://localhost:5000/api/Notificaciones/Push/enviar" `
    -Method Post `
    -Headers @{
        'Content-Type' = 'application/json'
        'Authorization' = 'Bearer YOUR_TOKEN'
    } `
    -Body ($notification | ConvertTo-Json)
```

---

## 🔐 Seguridad y Autenticación

- **Registrar token**: Sin autenticación (Anonymous)
- **Enviar notificación**: Requiere autorización JWT
- **Desuscribir**: Sin autenticación
- **Verificar estado**: Sin autenticación

---

## 📊 Modelo de Base de Datos

```sql
CREATE TABLE DeviceTokens (
    DeviceTokenID SERIAL PRIMARY KEY,
    Token VARCHAR(500) NOT NULL UNIQUE,
    ClienteID INT NULLABLE,
    EmpresaID UUID NULLABLE,
    Platform VARCHAR(50),
    DeviceId VARCHAR(255),
    IsActive BOOLEAN DEFAULT true,
    RegisteredDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastUsedDate TIMESTAMP NULLABLE
);

CREATE INDEX idx_device_tokens_active ON DeviceTokens(IsActive);
CREATE INDEX idx_device_tokens_token ON DeviceTokens(Token);
CREATE INDEX idx_device_tokens_empresa ON DeviceTokens(EmpresaID);
CREATE INDEX idx_device_tokens_cliente ON DeviceTokens(ClienteID);
```

---

## 🚀 Características Avanzadas

### 1. **Envío en Lotes**
El sistema divide automáticamente notificaciones masivas en lotes de 500 tokens (límite de Firebase).

### 2. **Manejo de Errores**
- Tokens inválidos se rastrean
- Fallos se reportan en la respuesta
- Logs detallados en la base de datos

### 3. **Tópicos**
Permite crear canales de comunicación:
```javascript
// Suscribir a tópico "premium_users"
await firebaseService.SubscribeToTopicAsync(
    tokens, 
    "premium_users"
);

// Enviar a tópico
await firebaseService.SendNotificationToTopicAsync(
    "premium_users",
    "Oferta especial",
    "20% de descuento"
);
```

---

## 📋 Configuración Necesaria

### 1. **Firebase JSON**
Ubicación: `PedidosBarrio.Infrastructure/messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json`

### 2. **appsettings.json**
```json
{
  "Firebase": {
    "ServiceAccountPath": "messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json"
  }
}
```

### 3. **Program.cs**
```csharp
app.MapPushNotificationEndpoints();
```

### 4. **DependencyInjection.cs**
```csharp
services.AddScoped<IFirebaseMessagingService, FirebaseMessagingService>();
services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
```

---

## ✅ Casos de Uso

### 1. **Confirmación de Pedido**
```javascript
sendToEnterprise(
    empresaId,
    "Nuevo pedido recibido",
    `Pedido #${orderId} ha sido confirmado`
);
```

### 2. **Cambio de Estado de Entrega**
```javascript
sendToCustomer(
    customerId,
    "Tu pedido está en camino",
    "Tu pedido salió para la entrega"
);
```

### 3. **Ofertas y Promociones**
```javascript
sendNotificationToAll(
    "¡Oferta especial!",
    "50% de descuento en productos seleccionados"
);
```

### 4. **Mantenimiento o Avisos Importantes**
```javascript
sendNotificationToAll(
    "Mantenimiento programado",
    "Nuestro servicio estará en mantenimiento el 15/01"
);
```

---

## 🔧 Troubleshooting

| Problema | Solución |
|----------|----------|
| `Firebase service account JSON no encontrado` | Verificar la ruta del archivo JSON en appsettings.json |
| `Token inválido` | Obtener nuevo token desde la app móvil |
| `Notificación no llega` | Verificar que el dispositivo esté conectado a Internet |
| `Tasa de errores alta` | Revisar logs y eliminar tokens no válidos |

---

## 📈 Monitoreo

Los logs se guardan en la base de datos en la tabla `Logs` con información:
- Notificaciones enviadas
- Tasa de éxito/fallo
- Errores detallados
- Timestamps

```csharp
// Consultar logs de notificaciones
SELECT * FROM Logs 
WHERE Source = 'FirebaseMessagingService' 
ORDER BY CreatedAt DESC 
LIMIT 100;
```

---

## 🔄 Próximas Mejoras

- [ ] Dashboard de estadísticas de notificaciones
- [ ] Programación de notificaciones (envíos diferidos)
- [ ] Segmentación avanzada de audiencia
- [ ] A/B testing de mensajes
- [ ] Integración con Analytics
- [ ] Rich Media notifications (imágenes, videos)
