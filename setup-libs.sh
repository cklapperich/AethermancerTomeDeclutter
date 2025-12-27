#!/bin/bash
# Setup script to copy required DLLs for building

GAME_DIR="$HOME/.steam/debian-installation/steamapps/common/Aethermancer"
BEPINEX_DIR="$GAME_DIR/BepInEx"
MANAGED_DIR="$GAME_DIR/Aethermancer_Data/Managed"

LIB_DIR="$(dirname "$0")/lib"
mkdir -p "$LIB_DIR"

echo "Copying BepInEx libraries..."
cp "$BEPINEX_DIR/core/BepInEx.dll" "$LIB_DIR/" 2>/dev/null || echo "  Warning: BepInEx.dll not found"
cp "$BEPINEX_DIR/core/0Harmony.dll" "$LIB_DIR/" 2>/dev/null || echo "  Warning: 0Harmony.dll not found"

echo "Copying game libraries..."
cp "$MANAGED_DIR/Assembly-CSharp.dll" "$LIB_DIR/" 2>/dev/null || echo "  Warning: Assembly-CSharp.dll not found"
cp "$MANAGED_DIR/UnityEngine.dll" "$LIB_DIR/" 2>/dev/null || echo "  Warning: UnityEngine.dll not found"
cp "$MANAGED_DIR/UnityEngine.CoreModule.dll" "$LIB_DIR/" 2>/dev/null || echo "  Warning: UnityEngine.CoreModule.dll not found"

echo ""
echo "Done! Libraries copied to $LIB_DIR:"
ls -la "$LIB_DIR"
