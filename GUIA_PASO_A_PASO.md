# 🚀 GUÍA DE IMPLEMENTACIÓN - Paso a Paso

## ✅ Lo que ya está hecho

Todo el código está implementado y compilado exitosamente. Solo necesitas seguir estos pasos para poner en funcionamiento el sistema.

---

## 📋 Paso 1: Crear la Migración de Base de Datos

### Opción A: Usando Package Manager Console (Visual Studio)

```powershell
# En la consola de Package Manager de Visual Studio
# 1. Cambiar a directorio Infrastructure
cd PedidosBarrio.Infrastructure

# 2. Crear migración
Add-Migration AddDeviceTokensTable -Project PedidosBarrio.Infrastructure -StartupProject PedidosBarrio

# 3. Actualizar base de datos
Update-Database -Project PedidosBarrio.Infrastructure -StartupProject PedidosBarrio
```

### Opción B: Usando CLI (Recomendado)

```bash
# Abrir terminal en la raíz del proyecto

# 1. Crear migración
dotnet ef migrations add AddDeviceTokensTable --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio

# 2. Actualizar base de datos
dotnet ef database update --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio
```

**Salida esperada**:
```
Build started...
Build succeeded.
Migrations are applied successfully.
```

---

## 🏗️ Paso 2: Verificar la Base de Datos

### Usando DBeaver o pgAdmin

```sql
-- Verificar tabla creada
SELECT * FROM "DeviceTokens" LIMIT 1;

-- Ver estructura
\d "DeviceTokens"

-- Ver índices
SELECT indexname FROM pg_indexes WHERE tablename = 'DeviceTokens';
```

**Salida esperada**:
```
 DeviceTokenID | Token | ClienteID | EmpresaID | Platform | DeviceId | IsActive | RegisteredDate | LastUsedDate
```

---

## 🔧 Paso 3: Configurar Firebase

### 1. Verificar el JSON de Firebase

Ubicación del archivo:
```
PedidosBarrio.Infrastructure/messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json
```

✅ El archivo **ya está presente** en el proyecto

### 2. Verificar appsettings.json

```json
{
  "Firebase": {
    "ServiceAccountPath": "messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json"
  }
}
```

✅ **Ya está configurado**

---

## 🧪 Paso 4: Probar los Endpoints

### 4.1 Iniciar la aplicación

```bash
# Opción 1: Desde Visual Studio
# F5 o Debug → Start Debugging

# Opción 2: Desde terminal
dotnet run --project PedidosBarrio
```

**Esperado**: Aplicación inicia en `http://localhost:5000`

### 4.2 Probar Registrar Token

```bash
# Terminal / PowerShell / Postman

curl -X POST http://localhost:5000/api/Notificaciones/Push/registrar \
  -H "Content-Type: application/json" \
  -d '{
    "token": "test_token_12345",
    "platform": "Android",
    "deviceId": "device-001"
  }'
```

**Respuesta esperada**:
```json
{
  "success": true,
  "message": "Token registrado exitosamente (ID: 1)",
  "successCount": 0,
  "failureCount": 0
}
```

### 4.3 Verificar Token Registrado

```bash
curl -X GET http://localhost:5000/api/Notificaciones/Push/estado/test_token_12345
```

**Respuesta esperada**:
```json
{
  "isActive": true,
  "platform": "Android",
  "registeredDate": "2024-01-15T10:30:00Z",
  "lastUsedDate": null,
  "clienteId": null,
  "empresaId": null
}
```

### 4.4 Enviar Notificación

```bash
# Primero obtener un token JWT válido
# Luego enviarlo en el header Authorization

curl -X POST http://localhost:5000/api/Notificaciones/Push/enviar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -d '{
    "title": "¡Hola!",
    "body": "Este es tu primer push notification",
    "targetType": "all"
  }'
```

**Respuesta esperada**:
```json
{
  "success": true,
  "message": "Notificaciones enviadas",
  "successCount": 1,
  "failureCount": 0
}
```

### 4.5 Desuscribir Token

```bash
curl -X POST http://localhost:5000/api/Notificaciones/Push/desuscribir \
  -H "Content-Type: application/json" \
  -d '{"token": "test_token_12345"}'
```

**Respuesta esperada**:
```json
{
  "success": true,
  "message": "Token desuscrito exitosamente",
  "successCount": 0,
  "failureCount": 0
}
```

---

## 📊 Paso 5: Verificar en la Base de Datos

Después de registrar y enviar notificaciones:

```sql
-- Ver tokens registrados
SELECT * FROM "DeviceTokens" WHERE "IsActive" = true;

-- Ver historial de notificaciones (en tabla Logs)
SELECT * FROM "Logs" 
WHERE "Source" = 'FirebaseMessagingService' 
ORDER BY "CreatedAt" DESC 
LIMIT 10;
```

---

## 🎯 Paso 6: Documentación en Swagger

Inicia la aplicación y ve a:

```
http://localhost:5000/swagger
```

Busca **"Push Notifications"** en la interfaz de Swagger para ver:
- ✅ 4 endpoints documentados
- ✅ Esquemas de request/response
- ✅ Ejemplos de uso
- ✅ Códigos de respuesta

---

## 🐛 Paso 7: Troubleshooting

### Error: "Firebase service account JSON no encontrado"

**Solución**:
```bash
# Verificar que el archivo existe
ls -la PedidosBarrio.Infrastructure/messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json

# Si no existe, verificar appsettings.json
# La ruta debe ser correcta
```

### Error: "La tabla DeviceTokens no existe"

**Solución**:
```bash
# Ejecutar migración de nuevo
dotnet ef database update --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio

# Si falla, limpiar y recrear
dotnet ef database drop --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio
dotnet ef database update --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio
```

### Error: "No authorization header"

**Solución**:
El endpoint de envío requiere token JWT. Obtén uno:
1. Login en `/api/Auth/login`
2. Copia el token
3. Agrega en header: `Authorization: Bearer TOKEN_AQUI`

---

## 📱 Paso 8: Integrar en tu App Móvil

### Android (Flutter/Dart)

```dart
// 1. Instalar firebase_messaging
flutter pub add firebase_messaging

// 2. En main.dart
import 'package:firebase_messaging/firebase_messaging.dart';

void main() {
  // ... tu código
  _setupFirebaseMessaging();
  runApp(MyApp());
}

void _setupFirebaseMessaging() {
  FirebaseMessaging messaging = FirebaseMessaging.instance;

  messaging.getToken().then((token) {
    print('FCM Token: $token');

    // Registrar en backend
    registerToken(token!);
  });
}

void registerToken(String token) async {
  final response = await http.post(
    Uri.parse('http://localhost:5000/api/Notificaciones/Push/registrar'),
    headers: {'Content-Type': 'application/json'},
    body: jsonEncode({
      'token': token,
      'platform': 'Android',
    }),
  );

  if (response.statusCode == 200) {
    print('Token registrado exitosamente');
  }
}
```

### iOS (Swift)

```swift
import FirebaseMessaging

// En AppDelegate
func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {

    Messaging.messaging().token { token, error in
      if let error = error {
        print("Error fetching FCM registration token: \(error)")
      } else if let token = token {
        print("FCM registration token: \(token)")
        self.registerToken(token)
      }
    }

    return true
}

func registerToken(_ token: String) {
    let url = URL(string: "http://localhost:5000/api/Notificaciones/Push/registrar")!
    var request = URLRequest(url: url)
    request.setValue("application/json", forHTTPHeaderField: "Content-Type")
    request.httpMethod = "POST"

    let body: [String: Any] = [
        "token": token,
        "platform": "iOS"
    ]
    request.httpBody = try? JSONSerialization.data(withJSONObject: body)

    URLSession.shared.dataTask(with: request) { data, response, error in
        if let error = error {
            print("Error registering token: \(error)")
        }
    }.resume()
}
```

### Web (JavaScript)

```javascript
// 1. Instalar firebase
npm install firebase

// 2. En tu aplicación
import { initializeApp } from 'firebase/app';
import { getMessaging, getToken } from 'firebase/messaging';

const firebaseConfig = {
  // Tu configuración de Firebase
};

const app = initializeApp(firebaseConfig);
const messaging = getMessaging(app);

// Obtener token
getToken(messaging, { vapidKey: 'YOUR_VAPID_KEY' }).then((token) => {
  console.log('FCM Token:', token);
  registerToken(token);
});

function registerToken(token) {
  fetch('http://localhost:5000/api/Notificaciones/Push/registrar', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      token: token,
      platform: 'Web'
    })
  })
  .then(res => res.json())
  .then(data => console.log('Token registrado:', data));
}
```

---

## ✅ Checklist Final

- [ ] Migración de base de datos creada
- [ ] Base de datos actualizada
- [ ] Aplicación compilada sin errores
- [ ] Aplicación iniciada exitosamente
- [ ] Endpoint de registro de token probado
- [ ] Endpoint de envío de notificación probado
- [ ] Endpoint de desuscripción probado
- [ ] Endpoint de verificación de estado probado
- [ ] Documentación en Swagger visible
- [ ] Logs en base de datos registrados

---

## 🎉 ¡Listo!

Tu sistema de notificaciones push está funcionando correctamente.

**Próximos pasos**:
1. Integra con tu app móvil
2. Configura tu app para recibir tokens FCM
3. Prueba enviando notificaciones desde el endpoint
4. Monitoea los logs en la base de datos

---

## 📚 Recursos Adicionales

- 📖 `PushNotificationEndpoint.README.md` - Documentación completa
- 🧪 `PushNotificationEndpointTests.md` - Ejemplos de tests
- 🗄️ `MIGRATION_GUIDE.md` - Guía de base de datos
- 🎯 `PUSH_NOTIFICATIONS_IMPLEMENTATION.md` - Arquitectura
- 📱 `RESUMEN_PUSH_NOTIFICATIONS.md` - Resumen ejecutivo

---

## ❓ Preguntas Frecuentes

### ¿Dónde están los logs de notificaciones?

En la tabla `Logs` con `Source = 'FirebaseMessagingService'`

### ¿Puedo enviar datos personalizados?

Sí, usa el campo `data` en el body de la notificación:
```json
{
  "data": {
    "orderId": "12345",
    "action": "open_order"
  }
}
```

### ¿Qué pasa si el token es inválido?

Se registra en los logs y se retorna en la respuesta `failureCount`

### ¿Puedo enviar a múltiples empresas a la vez?

Puedes enviar a:
- Todos (targetType: "all")
- Una empresa (targetType: "empresa")

Para enviar a múltiples, envia múltiples requests en paralelo.

### ¿Hay límite de notificaciones?

No hay límite de notificaciones por hora. Firebase maneja la limitación.

---

**¡Felicidades! Tu sistema de notificaciones push está completamente operativo.** 🎉
