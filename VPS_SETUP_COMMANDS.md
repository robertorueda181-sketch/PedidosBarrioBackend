# 🚀 Comandos de Setup en VPS

Ejecuta estos comandos directamente en tu VPS Linux (vía SSH):

## 1️⃣ Actualizar Sistema

```bash
sudo apt update -y
sudo apt upgrade -y
```

## 2️⃣ Instalar .NET Runtime 10

```bash
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb
sudo dpkg -i /tmp/packages-microsoft-prod.deb
sudo apt update -y
sudo apt install -y aspnetcore-runtime-10.0
dotnet --version
```

## 3️⃣ Crear Usuario y Directorios

```bash
sudo useradd -m -s /bin/bash pedidosbarrio 2>/dev/null || echo "Usuario ya existe"
sudo mkdir -p /opt/pedidosbarrio
sudo mkdir -p /opt/pedidosbarrio/backup
sudo mkdir -p /opt/pedidosbarrio/logs
sudo mkdir -p /opt/pedidosbarrio/latest
sudo chown -R pedidosbarrio:pedidosbarrio /opt/pedidosbarrio
sudo chmod 755 /opt/pedidosbarrio
```

## 4️⃣ Crear Systemd Service

Ejecuta este comando para crear el archivo:

```bash
sudo nano /etc/systemd/system/pedidosbarrio.service
```

Y pega este contenido:

```ini
[Unit]
Description=PedidosBarrio API
After=network.target

[Service]
Type=simple
User=pedidosbarrio
WorkingDirectory=/opt/pedidosbarrio
ExecStart=/opt/pedidosbarrio/PedidosBarrio.Api
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://0.0.0.0:5000"

[Install]
WantedBy=multi-user.target
```

Luego presiona `Ctrl+X`, `Y`, `Enter` para guardar.

Después ejecuta:

```bash
sudo systemctl daemon-reload
sudo systemctl enable pedidosbarrio
sudo systemctl status pedidosbarrio
```

## 5️⃣ Crear appsettings.Production.json

Ejecuta:

```bash
sudo nano /opt/pedidosbarrio/appsettings.Production.json
```

Y pega (⚠️ CAMBIA TU PASSWORD DE POSTGRESQL):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PedidosBarrio;Username=postgres;Password=TU_PASSWORD_AQUI;Pooling=true;Maximum Pool Size=20;"
  },
  "Firebase": {
    "ServiceAccountPath": "/opt/pedidosbarrio/messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json"
  }
}
```

Guardar: `Ctrl+X`, `Y`, `Enter`

Permisos:

```bash
sudo chown pedidosbarrio:pedidosbarrio /opt/pedidosbarrio/appsettings.Production.json
sudo chmod 644 /opt/pedidosbarrio/appsettings.Production.json
```

## 6️⃣ Copiar Archivo JSON de Firebase

Si tienes el archivo Firebase en local, cópialo al VPS:

```bash
# DESDE TU MÁQUINA LOCAL:
scp -i ~/.ssh/deploy_key messagesespacioonline-firebase-adminsdk-fbsvc-3497fea4a0.json pedidosbarrio@TU_VPS_IP:/opt/pedidosbarrio/
```

## 7️⃣ Verificar Instalación

```bash
# Ver versión de .NET
dotnet --version

# Ver usuario creado
id pedidosbarrio

# Ver directorios
ls -la /opt/pedidosbarrio

# Ver estado del servicio
systemctl status pedidosbarrio

# Ver logs en tiempo real
sudo journalctl -u pedidosbarrio -f
```

## ✅ Checklist Antes del Deploy

- [ ] .NET Runtime 10 instalado: `dotnet --version` → debe mostrar 10.x.x
- [ ] Usuario `pedidosbarrio` creado: `id pedidosbarrio` → debe existir
- [ ] Directorios existen: `ls -la /opt/pedidosbarrio` → debe mostrar carpetas
- [ ] Systemd service creado: `systemctl status pedidosbarrio` → debe estar enabled
- [ ] `appsettings.Production.json` existe con credenciales correctas
- [ ] Firebase JSON copiado (si aplica)

## 🔑 Permisos SSH para GitHub Actions

También necesitas crear una clave SSH para que GitHub Actions pueda conectarse:

```bash
# En VPS como usuario pedidosbarrio
sudo -u pedidosbarrio ssh-keygen -t ed25519 -C "github-actions" -N "" -f /home/pedidosbarrio/.ssh/github_deploy

# Ver la clave pública
cat /home/pedidosbarrio/.ssh/github_deploy.pub

# Autorizar la clave
echo "$(cat /home/pedidosbarrio/.ssh/github_deploy.pub)" | sudo -u pedidosbarrio tee -a /home/pedidosbarrio/.ssh/authorized_keys
sudo -u pedidosbarrio chmod 600 /home/pedidosbarrio/.ssh/authorized_keys

# Ver la clave privada (para GitHub Secrets)
sudo cat /home/pedidosbarrio/.ssh/github_deploy
```

Copia la **clave privada completa** (desde `-----BEGIN` hasta `-----END`) para agregar al GitHub Secret `SSH_PRIVATE_KEY`.

## 🐍 Prueba de Deploy Manual

Una vez todo listo, simula el deploy:

```bash
# Crear archivo de prueba
ssh -i ~/.ssh/deploy_key pedidosbarrio@TU_VPS_IP mkdir -p /opt/pedidosbarrio/latest

# Verificar conexión
ssh -i ~/.ssh/deploy_key pedidosbarrio@TU_VPS_IP "ls -la /opt/pedidosbarrio"
```

Si todo funciona, GitHub Actions podrá desplegar automáticamente. 🎉
