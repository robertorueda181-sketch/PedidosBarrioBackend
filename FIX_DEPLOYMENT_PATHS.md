# 🔧 Fix: Deployment Paths Issue

## Problema Identificado
❌ **Error:** `No se encontró la aplicación publicada en ./PedidosBarrio.Api`

El error ocurría porque:
1. El workflow publicaba a `./publish`
2. Descargaba los artefactos a `./publish`
3. Pero transfería los archivos a `$VPS_APP_PATH/` directamente
4. El script de deploy esperaba encontrar la app en `$APP_PATH/PedidosBarrio.Api`

## Soluciones Implementadas

### 1. ✅ RuntimeIdentifiers en .csproj
Agregamos `<RuntimeIdentifiers>linux-x64;win-x64</RuntimeIdentifiers>` al proyecto para que .NET pueda compilar para Linux.

**Archivo:** `PedidosBarrio/PedidosBarrio.Api.csproj`
```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <RuntimeIdentifiers>linux-x64;win-x64</RuntimeIdentifiers>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

### 2. ✅ Workflow mejorado
Agregamos una estructura clara de directorios:

**Archivo:** `.github/workflows/deploy-vps.yml`

```yaml
# Ahora transferimos a un directorio "latest"
- name: 📤 Transferir archivos
  run: |
    scp -i ~/.ssh/deploy_key \
      -o ConnectTimeout=10 \
      -o StrictHostKeyChecking=no \
      -r ./publish/ \
      ${{ secrets.SERVER_USER }}@${{ secrets.SERVER_HOST  }}:${{ secrets.VPS_APP_PATH }}/latest/

# Agregamos verificación de artefactos
- name: 📋 Verificar artefactos descargados
  run: |
    echo "📂 Contenido del directorio publish:"
    ls -la ./publish/
    echo "📊 Archivos totales:"
    find ./publish -type f | wc -l
```

### 3. ✅ Script de deploy actualizado
El deploy script ahora:
- Busca la app en `$APP_PATH/latest/`
- Copia de `latest/` al `$APP_PATH/` principal
- Crea backups correctamente
- Restaura automáticamente si hay error

**Archivo:** `.github/scripts/deploy.sh`

```bash
LATEST_PATH="$APP_PATH/latest"

# Validar que existe
if [ ! -f "$LATEST_PATH/$APP_NAME" ]; then
    log_error "No se encontró la aplicación publicada en $LATEST_PATH/$APP_NAME"
    ls -la "$LATEST_PATH/" 2>/dev/null || echo "Directorio no existe"
    exit 1
fi

# Copiar nueva versión
log "4️⃣  Instalando nueva versión..."
rm -rf "$APP_PATH"/* 2>/dev/null || true
cp -r "$LATEST_PATH"/* "$APP_PATH/"
chmod +x "$APP_PATH/$APP_NAME"
```

## Estructura de directorios en VPS

```
/opt/pedidosbarrio/
├── latest/              ← Nuevos artefactos transferidos
│   ├── PedidosBarrio.Api
│   ├── appsettings.json
│   └── ... (otros archivos)
├── backup/
│   ├── backup-20250122-143022/
│   └── last-working/    ← Para rollback automático
├── logs/
│   └── deploy-*.log
├── appsettings.Production.json
└── PedidosBarrio.Api    ← Executable actual
```

## Pasos para desplegar

1. **Commit de cambios:**
```powershell
git add -A
git commit -m "Fix: Deploy paths and RuntimeIdentifiers"
git push origin main
```

2. **GitHub Actions ejecutará automáticamente:**
   - ✅ Build y test
   - ✅ Publicar para linux-x64
   - ✅ Transferir a VPS
   - ✅ Ejecutar deploy script
   - ✅ Verificar servicio

3. **Monitorear el deploy:**
   - En GitHub: Actions tab → Ver workflow en progreso
   - En VPS: `sudo journalctl -u pedidosbarrio -f`

## Verificación

Si todo funciona correctamente, verás:
```
✅ Archivos validados
✅ Aplicación detenida
✅ Backup creado
✅ Nueva versión instalada
✅ Aplicación iniciada correctamente
✅ Aplicación respondiendo correctamente (HTTP 404)
```

## Troubleshooting

Si algo falla:

1. **Ver logs del workflow:**
   - GitHub → Actions → Ver build fallido → Expandir pasos

2. **Ver logs en VPS:**
   ```bash
   sudo journalctl -u pedidosbarrio -n 50  # Últimos 50 logs
   sudo journalctl -u pedidosbarrio -f      # En tiempo real
   cat /opt/pedidosbarrio/logs/deploy-*.log
   ```

3. **Verificar estructura:**
   ```bash
   ls -la /opt/pedidosbarrio/
   ls -la /opt/pedidosbarrio/latest/
   file /opt/pedidosbarrio/PedidosBarrio.Api
   ```

4. **Si falla, se restaura automáticamente** desde `/opt/pedidosbarrio/backup/last-working/`

---

**Cambios resumidos:**
- ✅ Agregado `RuntimeIdentifiers` a `.csproj`
- ✅ Mejorado flujo de artifacts en workflow
- ✅ Actualizado deploy script para nueva estructura
- ✅ Agregada verificación de artefactos
- ✅ Mejor logging y diagnóstico de errores
