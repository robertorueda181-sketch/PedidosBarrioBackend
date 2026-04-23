# 🚀 GitHub Actions Deploy a VPS Linux - Guía Completa

## 📋 Tabla de Contenidos

1. [Requisitos Previos](#requisitos-previos)
2. [Configuración en VPS](#configuración-en-vps)
3. [Configuración en GitHub](#configuración-en-github)
4. [Testing](#testing)
5. [Troubleshooting](#troubleshooting)

---

## ✅ Requisitos Previos

### En tu VPS Linux:
- [ ] Ubuntu 20.04+ o similar
- [ ] SSH accesible
- [ ] Usuario con permisos sudo
- [ ] .NET Runtime 10.0 instalado
- [ ] PostgreSQL (si no está en otro servidor)
- [ ] Nginx (para proxy reverso, opcional)

### En tu máquina local:
- [ ] Git configurado
- [ ] GitHub account
- [ ] Clave SSH para el VPS

---

## 🔧 Configuración en VPS Linux

### Paso 1: Actualizar sistema

```bash
sudo apt update && sudo apt upgrade -y
sudo apt install -y wget curl git build-essential
```

### Paso 2: Instalar .NET Runtime 10

```bash
# Agregar repositorio de Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Instalar .NET Runtime
sudo apt update
sudo apt install -y aspnetcore-runtime-10.0

# Verificar instalación
dotnet --version
```

### Paso 3: Crear usuario para la aplicación

```bash
# Crear usuario no-privilegiado
sudo useradd -m -s /bin/bash pedidosbarrio
sudo usermod -aG sudo pedidosbarrio  # Opcional, solo si necesita sudo

# Crear directorio de la aplicación
sudo mkdir -p /opt/pedidosbarrio
sudo chown -R pedidosbarrio:pedidosbarrio /opt/pedidosbarrio
sudo chmod 755 /opt/pedidosbarrio
```

### Paso 4: Crear systemd service

```bash
sudo nano /etc/systemd/system/pedidosbarrio.service
```

Copiar y pegar:

```ini
[Unit]
Description=PedidosBarrio API
After=network.target postgresql.service

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
sudo systemctl status pedidosbarrio
```

### Paso 5: Crear archivo de configuración

```bash
# Crear appsettings.Production.json
sudo nano /opt/pedidosbarrio/appsettings.Production.json
```

Contenido (adaptar según tus valores):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PedidosBarrio;Username=postgres;Password=TU_PASSWORD_AQUI;SslMode=Require"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
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

### Paso 6: Configurar Nginx como proxy reverso (Opcional pero recomendado)

```bash
sudo apt install -y nginx
sudo nano /etc/nginx/sites-available/pedidosbarrio
```

Contenido:

```nginx
server {
    listen 80;
    server_name tu-dominio.com www.tu-dominio.com;

    # Redirigir HTTP a HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name tu-dominio.com www.tu-dominio.com;

    # Certificados SSL (usar Let's Encrypt)
    ssl_certificate /etc/letsencrypt/live/tu-dominio.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/tu-dominio.com/privkey.pem;

    # Configuración SSL
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    # Proxy reverso
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

Activar sitio:

```bash
sudo ln -s /etc/nginx/sites-available/pedidosbarrio /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### Paso 7: Instalar SSL con Let's Encrypt (Opcional pero recomendado)

```bash
sudo apt install -y certbot python3-certbot-nginx
sudo certbot certonly --nginx -d tu-dominio.com -d www.tu-dominio.com
```

---

## 🔐 Configuración en GitHub

### Paso 1: Generar clave SSH

En tu VPS como usuario deploy:

```bash
# Como el usuario deploy (o root si lo prefieres)
ssh-keygen -t ed25519 -f /home/pedidosbarrio/.ssh/deploy_key -N ""

# Ver la clave privada
cat /home/pedidosbarrio/.ssh/deploy_key
```

### Paso 2: Agregar clave autorizada

```bash
# Crear directorio .ssh si no existe
mkdir -p /home/pedidosbarrio/.ssh
chmod 700 /home/pedidosbarrio/.ssh

# Crear archivo authorized_keys
touch /home/pedidosbarrio/.ssh/authorized_keys
chmod 600 /home/pedidosbarrio/.ssh/authorized_keys

# Agregar clave pública
cat /home/pedidosbarrio/.ssh/deploy_key.pub >> /home/pedidosbarrio/.ssh/authorized_keys
```

### Paso 3: Configurar secretos en GitHub

1. Ve a tu repositorio en GitHub
2. Settings → Secrets and variables → Actions
3. Agregar los siguientes secretos:

```
VPS_HOST          : tu-vps.com (o IP)
VPS_USER          : pedidosbarrio
VPS_SSH_KEY       : (Contenido de /home/pedidosbarrio/.ssh/deploy_key)
VPS_APP_PATH      : /opt/pedidosbarrio
```

**Ejemplo de cómo copiar la SSH Key:**

En tu máquina local:
```bash
# Copiar el contenido de la clave privada
cat /ruta/a/deploy_key | xclip -selection clipboard

# O si no tienes xclip
cat /ruta/a/deploy_key
# Copiar manualmente todo el contenido
```

En GitHub:
1. New repository secret
2. Name: `VPS_SSH_KEY`
3. Paste the entire private key content (incluyendo `-----BEGIN` y `-----END`)

### Paso 4: Agregar secretos opcionales

Para notificaciones en Slack (opcional):

```
SLACK_WEBHOOK : https://hooks.slack.com/services/YOUR/WEBHOOK/URL
```

Para obtener webhook de Slack:
1. Ve a https://api.slack.com/apps
2. Create New App
3. From scratch → Incoming Webhooks
4. Copiar webhook URL

---

## 🧪 Testing

### Prueba 1: Verificar SSH

```bash
# En GitHub Actions (ver en el workflow)
# O localmente:
ssh -i ~/.ssh/deploy_key pedidosbarrio@tu-vps.com "ls -la /opt/pedidosbarrio"
```

### Prueba 2: Hacer un push a main

```bash
git add .
git commit -m "Agregar GitHub Actions deploy"
git push origin main
```

Ve a tu repositorio → Actions → Deploy to VPS para ver el progreso.

### Prueba 3: Verificar logs en VPS

```bash
# En VPS
sudo journalctl -u pedidosbarrio -f
```

O:

```bash
# Ver último deploy
cat /opt/pedidosbarrio/logs/deploy-*.log | tail -50
```

---

## 🐛 Troubleshooting

### ❌ Error: "Permission denied (publickey)"

**Solución:**
```bash
# Verificar permisos en VPS
ls -la ~/.ssh/
# Deben tener permisos 700 para el directorio y 600 para los archivos
chmod 700 ~/.ssh
chmod 600 ~/.ssh/authorized_keys
```

### ❌ Error: "Command not found: dotnet"

**Solución:**
```bash
# En VPS
dotnet --version
# Si no funciona, instalar:
sudo apt install -y aspnetcore-runtime-10.0
```

### ❌ Error: "Service failed to start"

**Solución:**
```bash
# Ver logs detallados
sudo journalctl -u pedidosbarrio -n 50
# Verificar que el ejecutable existe
ls -la /opt/pedidosbarrio/PedidosBarrio.Api
# Verificar permisos
sudo chmod +x /opt/pedidosbarrio/PedidosBarrio.Api
```

### ❌ Error: "Connection to database failed"

**Solución:**
```bash
# Verificar conexión a PostgreSQL
psql -h localhost -U postgres -d PedidosBarrio
# Verificar credenciales en appsettings.Production.json
sudo cat /opt/pedidosbarrio/appsettings.Production.json
```

### ❌ Error: "Timeout transferring files"

**Solución:**
1. Aumentar timeout en el workflow (editar `.github/workflows/deploy-vps.yml`)
2. Verificar velocidad de conexión al VPS
3. Usar FTP alternativa (SCP tiene timeouts)

---

## 📊 Monitoreo

### Ver estado de la aplicación

```bash
# Estado del servicio
sudo systemctl status pedidosbarrio

# Ver logs en tiempo real
sudo journalctl -u pedidosbarrio -f

# Ver últimas líneas
sudo journalctl -u pedidosbarrio -n 100
```

### Reiniciar aplicación manualmente

```bash
# Reiniciar
sudo systemctl restart pedidosbarrio

# Detener
sudo systemctl stop pedidosbarrio

# Iniciar
sudo systemctl start pedidosbarrio
```

### Verificar que está escuchando

```bash
# Ver si el puerto 5000 está activo
netstat -tlnp | grep 5000

# O con ss
ss -tlnp | grep 5000
```

---

## 🔄 Rollback Manual

Si necesitas volver a una versión anterior:

```bash
# Listar backups disponibles
ls -la /opt/pedidosbarrio/backup/

# Restaurar backup específico
sudo cp -r /opt/pedidosbarrio/backup/backup-20240115-103045/* /opt/pedidosbarrio/
sudo systemctl restart pedidosbarrio
```

---

## 📈 Mejorar Rendimiento

### 1. Aumentar límite de conexiones

```bash
sudo nano /etc/security/limits.conf
# Agregar al final:
# pedidosbarrio soft nofile 65536
# pedidosbarrio hard nofile 65536
```

### 2. Configurar cache en Nginx

```nginx
# En la configuración de Nginx:
proxy_cache_valid 200 1h;
proxy_cache_key $scheme$request_method$host$request_uri;
```

### 3. Usar systemd socket activation

En `pedidosbarrio.service`:
```ini
ListenStream=5000
```

---

## 📝 Logs

Los logs se guardan en:
```
/opt/pedidosbarrio/logs/deploy-YYYYMMDD-HHMMSS.log
```

Y también en systemd journal:
```bash
journalctl -u pedidosbarrio
```

---

## ✅ Checklist Final

- [ ] VPS configurado y accesible por SSH
- [ ] .NET Runtime 10 instalado
- [ ] Usuario `pedidosbarrio` creado
- [ ] Directorio `/opt/pedidosbarrio` creado
- [ ] Systemd service `pedidosbarrio.service` creado
- [ ] `appsettings.Production.json` configurado
- [ ] SSH key configurada
- [ ] Secretos de GitHub configurados
- [ ] Workflow de GitHub Actions pusheado
- [ ] Primer deploy realizado exitosamente
- [ ] Aplicación respondiendo en VPS
- [ ] Nginx configurado (opcional)
- [ ] SSL configurado con Let's Encrypt (opcional)
- [ ] Logs monitoreados

---

## 🎉 ¡Listo!

Tu aplicación PedidosBarrio está configurada para deployar automáticamente cada vez que hagas push a la rama `main`.

**Workflow:**
1. Haces push a GitHub
2. GitHub Actions compila y testa
3. Si todo está OK, publica y transfiere a VPS
4. Deploy script reinicia el servicio
5. ¡Tu aplicación está en producción!

---

**Necesitas ayuda?** Revisa los logs en:
- GitHub Actions: Tu repositorio → Actions
- VPS: `sudo journalctl -u pedidosbarrio -f`
