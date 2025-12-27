#!/bin/bash
set -e

GAME_DIR="$HOME/.steam/debian-installation/steamapps/common/Aethermancer"
PLUGIN_DIR="$GAME_DIR/BepInEx/plugins"
DLL_NAME="AethermancerTomeDeclutter.dll"

echo "Building..."
dotnet build -c Release

echo "Copying to $PLUGIN_DIR"
mkdir -p "$PLUGIN_DIR"
cp "bin/Release/net472/$DLL_NAME" "$PLUGIN_DIR/"

echo "Done! Installed to $PLUGIN_DIR/$DLL_NAME"
