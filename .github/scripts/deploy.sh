#!/bin/bash

# Script de Deploy para PedidosBarrio - Servidor VPS Linux
# Detiene la aplicación anterior, respalda datos, e inicia la nueva versión

set -e  # Salir si hay error

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Variables de configuración
APP_PATH="${VPS_APP_PATH:-.}"
SERVICE_NAME="pedidosbarrio"
APP_NAME="PedidosBarrio.Api"
BACKUP_DIR="$APP_PATH/backup"
LOG_DIR="$APP_PATH/logs"
DEPLOY_LOG="$LOG_DIR/deploy-$(date +%Y%m%d-%H%M%S).log"

# Crear directorio de logs
mkdir -p "$LOG_DIR"

# Función para logging
log() {
    echo -e "${BLUE}[$(date '+%Y-%m-%d %H:%M:%S')]${NC} $1" | tee -a "$DEPLOY_LOG"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}" | tee -a "$DEPLOY_LOG"
}

log_error() {
    echo -e "${RED}❌ $1${NC}" | tee -a "$DEPLOY_LOG"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}" | tee -a "$DEPLOY_LOG"
}

# Función para limpiar en caso de error
cleanup() {
    log_error "Error en el deploy. Restaurando versión anterior..."
    if [ -d "$BACKUP_DIR/last-working" ]; then
        rm -rf "$APP_PATH/"*
        cp -r "$BACKUP_DIR/last-working"/* "$APP_PATH/"
        systemctl restart $SERVICE_NAME || true
        log_warning "Versión anterior restaurada"
    fi
}

trap cleanup ERR

# ============================================
# INICIO DEL DEPLOY
# ============================================

log "═══════════════════════════════════════════════════"
log "Iniciando deploy de $APP_NAME"
log "═══════════════════════════════════════════════════"

# 1. Validar que el nuevo código existe
log "1️⃣  Validando archivos..."
LATEST_PATH="$APP_PATH/latest"
if [ ! -f "$LATEST_PATH/$APP_NAME" ]; then
    log_error "No se encontró la aplicación publicada en $LATEST_PATH/$APP_NAME"
    log_error "Contenido de $LATEST_PATH:"
    ls -la "$LATEST_PATH/" 2>/dev/null || echo "Directorio no existe"
    exit 1
fi
log_success "Archivos validados"

# 2. Detener la aplicación anterior
log "2️⃣  Deteniendo aplicación anterior..."
if systemctl is-active --quiet $SERVICE_NAME; then
    systemctl stop $SERVICE_NAME
    sleep 2
    log_success "Aplicación detenida"
else
    log_warning "Servicio no estaba activo"
fi

# 3. Crear backup
log "3️⃣  Creando backup..."
BACKUP_TIMESTAMP=$(date +%Y%m%d-%H%M%S)
BACKUP_PATH="$BACKUP_DIR/backup-$BACKUP_TIMESTAMP"
mkdir -p "$BACKUP_PATH"

# Guardar la última versión que funcionaba
if [ -f "$APP_PATH/$APP_NAME" ]; then
    cp "$APP_PATH/$APP_NAME" "$BACKUP_PATH/"
    rm -rf "$BACKUP_DIR/last-working"
    cp -r "$APP_PATH"/* "$BACKUP_DIR/last-working/" 2>/dev/null || true
    log_success "Backup creado en $BACKUP_PATH"
else
    log_warning "No hay versión anterior para respaldar"
fi

# 4. Copiar nueva versión
log "4️⃣  Instalando nueva versión..."
rm -rf "$APP_PATH"/* 2>/dev/null || true
cp -r "$LATEST_PATH"/* "$APP_PATH/"
chmod +x "$APP_PATH/$APP_NAME"
log_success "Nueva versión instalada"
chmod +x "$APP_PATH"/*.sh 2>/dev/null || true
log_success "Permisos configurados"

# 5. Verificar variables de entorno
log "5️⃣  Verificando configuración..."
if [ ! -f "$APP_PATH/appsettings.Production.json" ]; then
    log_warning "appsettings.Production.json no encontrado. Copiando desde appsettings.json"
    cp "$APP_PATH/appsettings.json" "$APP_PATH/appsettings.Production.json" 2>/dev/null || true
fi
log_success "Configuración verificada"

# 6. Iniciar la aplicación
log "6️⃣  Iniciando aplicación..."
systemctl start $SERVICE_NAME

# 7. Esperar a que inicie
sleep 5

# 8. Verificar que está corriendo
log "7️⃣  Verificando estado..."
if systemctl is-active --quiet $SERVICE_NAME; then
    log_success "Aplicación iniciada correctamente"
else
    log_error "La aplicación no inició correctamente"
    journalctl -u $SERVICE_NAME -n 30 | tee -a "$DEPLOY_LOG"
    exit 1
fi

# 9. Verificar que está respondiendo
log "8️⃣  Verificando respuesta HTTP..."
sleep 2
RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/swagger || echo "000")
if [ "$RESPONSE" = "200" ] || [ "$RESPONSE" = "404" ]; then
    log_success "Aplicación respondiendo correctamente (HTTP $RESPONSE)"
else
    log_warning "Respuesta inesperada: HTTP $RESPONSE. Verificando logs..."
    journalctl -u $SERVICE_NAME -n 20 | tee -a "$DEPLOY_LOG"
fi

# 10. Verificar base de datos
log "9️⃣  Verificando conectividad a base de datos..."
if grep -q "Connection successful\|Migrating database" "$LOG_DIR"/*.log 2>/dev/null; then
    log_success "Base de datos accesible"
else
    log_warning "No se pudo verificar conexión a BD. Revisa manualmente."
fi

# ============================================
# FIN DEL DEPLOY
# ============================================

log "═══════════════════════════════════════════════════"
log_success "✨ Deploy completado exitosamente!"
log "═══════════════════════════════════════════════════"

log ""
log "📊 Información del Deploy:"
log "   - Timestamp: $(date '+%Y-%m-%d %H:%M:%S')"
log "   - Servicio: $SERVICE_NAME"
log "   - Path: $APP_PATH"
log "   - Logs: $DEPLOY_LOG"
log ""
log "🔍 Comandos útiles:"
log "   Ver estado:    systemctl status $SERVICE_NAME"
log "   Ver logs:      journalctl -u $SERVICE_NAME -f"
log "   Detener:       systemctl stop $SERVICE_NAME"
log "   Reiniciar:     systemctl restart $SERVICE_NAME"
log ""

exit 0
