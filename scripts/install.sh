#!/usr/bin/env sh
set -eu

REPO="rolfwessels/init-stack"
BIN="init-stack"
INSTALL_DIR="${HOME}/.local/bin"

case "$(uname -s)" in
  Linux*) ;;
  *) echo "Unsupported OS: $(uname -s). Download manually from https://github.com/${REPO}/releases/latest" >&2; exit 1 ;;
esac

ARCHIVE="${BIN}-linux-x64.zip"
URL="https://github.com/${REPO}/releases/latest/download/${ARCHIVE}"

TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

echo "Downloading ${ARCHIVE}..."
curl -fsSL "$URL" -o "$TMP/$ARCHIVE"

echo "Extracting to $INSTALL_DIR..."
mkdir -p "$INSTALL_DIR"
unzip -o "$TMP/$ARCHIVE" -d "$INSTALL_DIR"
chmod +x "$INSTALL_DIR/$BIN"

echo "Installed: $INSTALL_DIR/$BIN"
"$INSTALL_DIR/$BIN" --version

case ":$PATH:" in
  *":$INSTALL_DIR:"*) ;;
  *)
    echo ""
    echo "Note: $INSTALL_DIR is not on PATH. Add this to your shell profile:"
    echo "  export PATH=\"\$HOME/.local/bin:\$PATH\""
    ;;
esac
