# 🚀 GitHub Actions Deploy a VPS - Resumen Rápido

## ✅ Archivos Creados

```
.github/
├── workflows/
│   └── deploy-vps.yml          ← Workflow principal
└── scripts/
    └── deploy.sh               ← Script de deploy en VPS
```

## 📋 Configuración en 5 Pasos

### 1️⃣ Configurar VPS

**En tu VPS Linux (como root o con sudo):**

```bash
# Actualizar sistema
sudo apt update && sudo apt upgrade -y

# Instalar .NET Runtime 10
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y aspnetcore-runtime-10.0

# Crear usuario y directorio
sudo useradd -m -s /bin/bash pedidosbarrio
sudo mkdir -p /opt/pedidosbarrio
sudo chown -R pedidosbarrio:pedidosbarrio /opt/pedidosbarrio
```

### 2️⃣ Crear Systemd Service

**En VPS como root:**

```bash
sudo nano /etc/systemd/system/pedidosbarrio.service
```

Pegar:

```ini
[Unit]
Description=PedidosBarrio API
After=network.target

[Service]
Type=notify
User=pedidosbarrio
WorkingDirectory=/opt/pedidosbarrio
ExecStart=/opt/pedidosbarrio/PedidosBarrio.Api
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://localhost:5000"

[Install]
WantedBy=multi-user.target
```

Luego:

```bash
sudo systemctl daemon-reload
sudo systemctl enable pedidosbarrio
```

### 3️⃣ Generar SSH Key

**En VPS como usuario pedidosbarrio:**

```bash
sudo su - pedidosbarrio
ssh-keygen -t ed25519 -f ~/.ssh/deploy_key -N ""
cat ~/.ssh/deploy_key
```

**Copiar todo el contenido** (incluyendo `-----BEGIN PRIVATE KEY-----` y `-----END PRIVATE KEY-----`)

### 4️⃣ Configurar Secretos en GitHub

1. Ve a tu repositorio: **Settings → Secrets and variables → Actions**
2. Click **New repository secret** 3 veces:

**Secret 1:**
- Name: `VPS_HOST`
- Value: `tu-vps.com` (o IP del VPS)

**Secret 2:**
- Name: `VPS_USER`
- Value: `pedidosbarrio`

**Secret 3:**
- Name: `VPS_SSH_KEY`
- Value: (todo el contenido de la clave privada que copiaste)

**Secret 4:**
- Name: `VPS_APP_PATH`
- Value: `/opt/pedidosbarrio`

### 5️⃣ Crear appsettings.Production.json

**En VPS:**

```bash
sudo nano /opt/pedidosbarrio/appsettings.Production.json
```

Contenido:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PedidosBarrio;Username=postgres;Password=TU_PASSWORD;SslMode=Require"
  },
  "Database": {
    "Provider": "PostgreSQL"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "BaseUrl": "https://tu-dominio.com",
  "AllowedHosts": "*"
}
```

```bash
sudo chown pedidosbarrio:pedidosbarrio /opt/pedidosbarrio/appsettings.Production.json
sudo chmod 600 /opt/pedidosbarrio/appsettings.Production.json
```

---

## 🎯 ¡Listo! Ahora funciona automáticamente

### Cuando hagas push a `main`:

```bash
git add .
git commit -m "Mi cambio"
git push origin main
```

### El workflow automáticamente:

1. ✅ Compila tu código
2. ✅ Ejecuta tests
3. ✅ Publica la aplicación
4. ✅ Transfiere a VPS
5. ✅ Reinicia el servicio
6. ✅ Verifica que esté corriendo

---

## 🔍 Ver estado del deploy

**En GitHub:** Tu repositorio → **Actions** → Ver workflow en vivo

**En VPS:** Ver logs de la aplicación

```bash
# Ver estado del servicio
sudo systemctl status pedidosbarrio

# Ver logs en tiempo real
sudo journalctl -u pedidosbarrio -f

# Ver últimas 50 líneas
sudo journalctl -u pedidosbarrio -n 50
```

---

## 🐛 Si algo falla

### Error: "Permission denied (publickey)"

```bash
# En VPS, verificar permisos
sudo su - pedidosbarrio
ls -la ~/.ssh/
# Debe tener 700 (directorio) y 600 (archivos)
chmod 700 ~/.ssh
chmod 600 ~/.ssh/authorized_keys
```

### Error: "Service failed to start"

```bash
# Ver error detallado
sudo journalctl -u pedidosbarrio -n 30

# Verificar que el ejecutable existe
ls -la /opt/pedidosbarrio/PedidosBarrio.Api

# Hacer ejecutable
sudo chmod +x /opt/pedidosbarrio/PedidosBarrio.Api
```

### Error: "Connection to database failed"

```bash
# Verificar credenciales en
sudo cat /opt/pedidosbarrio/appsettings.Production.json

# Probar conexión
psql -h localhost -U postgres -d PedidosBarrio
```

---

## 📊 Monitoreo útil

```bash
# Ver si está escuchando en puerto 5000
ss -tlnp | grep 5000

# Ver uso de recursos
top -p $(pgrep -f PedidosBarrio.Api)

# Ver logs en archivo
tail -f /opt/pedidosbarrio/logs/deploy-*.log

# Reiniciar manualmente
sudo systemctl restart pedidosbarrio

# Detener
sudo systemctl stop pedidosbarrio

# Iniciar
sudo systemctl start pedidosbarrio
```

---

## 🔄 Rollback (volver a versión anterior)

```bash
# Ver backups disponibles
ls -la /opt/pedidosbarrio/backup/

# Restaurar último backup que funcionaba
sudo cp -r /opt/pedidosbarrio/backup/last-working/* /opt/pedidosbarrio/

# Reiniciar
sudo systemctl restart pedidosbarrio

# Verificar
sudo systemctl status pedidosbarrio
```

---

## 📁 Estructura en VPS

```
/opt/pedidosbarrio/
├── PedidosBarrio.Api              ← Ejecutable
├── appsettings.Production.json   ← Configuración
├── logs/
│   └── deploy-20240115-103045.log
├── backup/
│   ├── last-working/
│   └── backup-20240115-103045/
└── deploy.sh                     ← Script (automático)
```

---

## ✨ Características del Workflow

✅ **Compilación automática** - Cada push a main  
✅ **Tests automáticos** - Valida todo antes de desplegar  
✅ **Backup automático** - Guarda versión anterior por si falla  
✅ **Rollback automático** - Si falla, restaura versión anterior  
✅ **Verificación automática** - Comprueba que está corriendo  
✅ **Logs detallados** - Todo registrado para debugging  

---

## 📞 Comandos rápidos útiles

```bash
# Ver qué hace cada cosa
cat ~/.github/workflows/deploy-vps.yml
cat ~/.github/scripts/deploy.sh

# Probar SSH manualmente
ssh -i ~/.ssh/deploy_key pedidosbarrio@tu-vps.com "ls -la /opt/pedidosbarrio"

# Ver logs del deploy en GitHub
# Tu repositorio → Actions → Deploy to VPS → Ver job en vivo
```

---

## 🎉 ¡Listo!

**Ahora cada vez que hagas:**

```bash
git push origin main
```

**Tu aplicación se despliega automáticamente en el VPS.**

---

**Siguientes pasos opcionales:**
- [ ] Configurar Nginx como proxy reverso
- [ ] Instalar SSL con Let's Encrypt
- [ ] Configurar monitoreo/alertas
- [ ] Agregar webhook de Slack para notificaciones
