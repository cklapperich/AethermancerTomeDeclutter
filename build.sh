#!/bin/bash
set -e

GAME_DIR="$HOME/.steam/debian-installation/steamapps/common/Aethermancer"
PLUGIN_DIR="$GAME_DIR/BepInEx/plugins"
DLL_NAME="AethermancerTomeDeclutter.dll"
ZIP_NAME="AethermancerTomeDeclutter.zip"

echo "Building..."
dotnet build -c Release

echo "Copying to $PLUGIN_DIR"
mkdir -p "$PLUGIN_DIR"
cp "bin/Release/net472/$DLL_NAME" "$PLUGIN_DIR/"

echo "Creating release zip..."
TEMP_DIR=$(mktemp -d)
mkdir -p "$TEMP_DIR/BepInEx/plugins"
cp "bin/Release/net472/$DLL_NAME" "$TEMP_DIR/BepInEx/plugins/"
(cd "$TEMP_DIR" && zip -r "$OLDPWD/$ZIP_NAME" BepInEx)
rm -rf "$TEMP_DIR"

echo "Done!"
echo "  Installed to: $PLUGIN_DIR/$DLL_NAME"
echo "  Release zip:  $ZIP_NAME"
