#!/bin/bash
set -e

# --- CONFIGURACIÓN ---
# Si VPS_APP_PATH no viene, usamos la ruta absoluta por defecto
APP_ROOT="${VPS_APP_PATH:-/var/www/backend}"
SERVICE_NAME="pedidosbarrio"
APP_NAME="PedidosBarrio.Api"

# Subcarpetas para no borrarlo todo
LATEST_PATH="$APP_ROOT/latest"
ACTIVE_PATH="$APP_ROOT/active"  # Aquí vivirá la app en ejecución
BACKUP_DIR="$APP_ROOT/backup"
LOG_DIR="$APP_ROOT/logs"

mkdir -p "$LOG_DIR" "$BACKUP_DIR" "$ACTIVE_PATH"

log() { echo -e "\033[0;34m[$(date '+%Y-%m-%d %H:%M:%S')]\033[0m $1"; }

log "1️⃣ Validando archivos en $LATEST_PATH..."
# IMPORTANTE: .NET genera un ejecutable sin extensión en Linux
if [ ! -f "$LATEST_PATH/$APP_NAME" ]; then
    echo "❌ No se encontró $APP_NAME en $LATEST_PATH"
    ls -la "$LATEST_PATH"
    exit 1
fi

log "2️⃣ Deteniendo servicio..."
sudo systemctl stop $SERVICE_NAME || true

log "3️⃣ Creando Backup de la versión anterior..."
if [ -d "$ACTIVE_PATH" ]; then
    tar -czf "$BACKUP_DIR/backup-$(date +%Y%m%d).tar.gz" -C "$ACTIVE_PATH" .
fi

log "4️⃣ Instalando nueva versión..."
# Borramos SOLO el contenido de active, NO de la raíz
rm -rf "$ACTIVE_PATH"/*
cp -r "$LATEST_PATH"/* "$ACTIVE_PATH/"
chmod +x "$ACTIVE_PATH/$APP_NAME"

log "5️⃣ Iniciando aplicación..."
sudo systemctl start $SERVICE_NAME

log "6️⃣ Verificando estado..."
sleep 5
if systemctl is-active --quiet $SERVICE_NAME; then
    echo "✅ Deploy exitoso!"
else
    echo "❌ Falló el inicio. Revisa: journalctl -u $SERVICE_NAME"
    exit 1
fi
