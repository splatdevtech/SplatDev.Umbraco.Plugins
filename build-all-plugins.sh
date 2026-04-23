#!/bin/bash
# Build all Umbraco plugin Lit 3 frontends for Umbraco 17
# Run this from the Umbraco Projects directory

set -e

PLUGINS=(
  "Umbraco.Plugins.CacheManager"
  "Umbraco.Plugins.RedirectManager"
  "Umbraco.Plugins.Mailer"
  "SplatDev.Umbraco.Plugins.ExceptionManager"
  "SplatDev.Umbraco.Plugins.ShortUrls"
  "SplatDev.Umbraco.Plugins.PdfCurator"
  "SplatDev.Umbraco.Plugins.BackupManager"
  "SplatDev.Umbraco.Plugins.OAuth"
  "SplatDev.Umbraco.Plugins.SEO"
)

BASE="/mnt/e/Source/Repos/Umbraco Projects"
FIRST_PLUGIN_NM="$BASE/Umbraco.Plugins.CacheManager/client/node_modules"

for plugin in "${PLUGINS[@]}"; do
  CLIENT_DIR="$BASE/$plugin/client"
  echo ""
  echo "=== Building $plugin ==="

  if [ ! -d "$CLIENT_DIR" ]; then
    echo "  SKIP: client/ directory not found"
    continue
  fi

  cd "$CLIENT_DIR"

  # Install or reuse node_modules
  if [ ! -d "node_modules/vite" ]; then
    if [ -d "$FIRST_PLUGIN_NM/vite" ]; then
      echo "  Linking node_modules from CacheManager..."
      ln -sfn "$FIRST_PLUGIN_NM" "node_modules" 2>/dev/null || npm install --include=dev --silent
    else
      echo "  Installing dependencies..."
      npm install --include=dev --silent
    fi
  fi

  echo "  Building..."
  node node_modules/vite/bin/vite.js build

  echo "  Done: $(ls ../App_Plugins/*/dist/*.js 2>/dev/null | head -1)"
done

echo ""
echo "All builds complete."
