#!/usr/bin/env sh
set -eu

BIN="init-stack"
RELEASES="https://github.com/rolfwessels/init-stack/releases"
INSTALL_DIR="${INSTALL_DIR:-$HOME/.local/bin}"

fail() {
  echo "init-stack installer: $*" >&2
  exit 1
}

case "$(uname -s)" in
  Linux)  OS=linux ;;
  Darwin) OS=osx ;;
  *) fail "Unsupported OS: $(uname -s). See $RELEASES/latest." ;;
esac

case "$(uname -m)" in
  x86_64|amd64)  ARCH=x64 ;;
  aarch64|arm64) ARCH=arm64 ;;
  *) fail "Unsupported CPU: $(uname -m). See $RELEASES/latest." ;;
esac

for command in curl unzip; do
  command -v "$command" >/dev/null 2>&1 || fail "$command is required."
done
if [ "$OS" = osx ]; then
  command -v shasum >/dev/null 2>&1 || fail "shasum is required."
else
  command -v sha256sum >/dev/null 2>&1 || fail "sha256sum is required."
fi

LATEST="$(curl -fsSL -o /dev/null -w '%{url_effective}' "$RELEASES/latest")"
TAG="${LATEST#"$RELEASES/tag/"}"
case "$TAG" in
  v[0-9]*) ;;
  *) fail "Could not resolve the latest release. See $RELEASES/latest." ;;
esac
case "${TAG#v}" in
  *[!0-9.]*) fail "Unexpected release tag: $TAG." ;;
esac

ARCHIVE="$BIN-$OS-$ARCH.zip"
DOWNLOAD="$RELEASES/download/$TAG"
mkdir -p "$INSTALL_DIR"
TMP="$(mktemp -d "$INSTALL_DIR/.init-stack-install.XXXXXX")"
trap 'rm -rf "$TMP"' EXIT

echo "Downloading $ARCHIVE from $TAG..."
curl -fsSL "$DOWNLOAD/$ARCHIVE" -o "$TMP/$ARCHIVE" ||
  fail "$TAG has no downloadable $ARCHIVE. See $RELEASES/latest."
curl -fsSL "$DOWNLOAD/checksums.txt" -o "$TMP/checksums.txt"

EXPECTED="$(awk -v archive="$ARCHIVE" '$2 == archive { print $1 }' "$TMP/checksums.txt")"
if [ "$OS" = osx ]; then
  ACTUAL="$(shasum -a 256 "$TMP/$ARCHIVE")"
else
  ACTUAL="$(sha256sum "$TMP/$ARCHIVE")"
fi
[ "$EXPECTED" = "${ACTUAL%% *}" ] || fail "SHA-256 verification failed for $ARCHIVE."

unzip -p "$TMP/$ARCHIVE" "$BIN" > "$TMP/$BIN"
chmod +x "$TMP/$BIN"
VERSION="$("$TMP/$BIN" --version)" || fail "The downloaded executable failed to start."

[ ! -d "$INSTALL_DIR/$BIN" ] || fail "$INSTALL_DIR/$BIN is a directory."
mv -f "$TMP/$BIN" "$INSTALL_DIR/$BIN"
echo "Installed init-stack $VERSION at $INSTALL_DIR/$BIN"

case ":$PATH:" in
  *":$INSTALL_DIR:"*) ;;
  *)
    echo ""
    echo "Note: $INSTALL_DIR is not on PATH. Add this to your shell profile:"
    echo "  export PATH=\"$INSTALL_DIR:\$PATH\""
    ;;
esac
