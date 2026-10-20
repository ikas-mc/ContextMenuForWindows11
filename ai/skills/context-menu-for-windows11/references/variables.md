# Variable Substitution and Quoting Semantics (Host PathHelper / CustomSubExplorerCommand)

The host (C++) performs **pure string replacement** (`PathHelper::simpleFormat`, no escaping, no automatic quoting) during `Invoke`, then passes the result to `ShellExecute`.

## Single-File Mode (Each or single selection)

Available variables in `param` template:

| Variable | Meaning | Version |
|----------|---------|---------|
| `{path}` | File/folder absolute path | - |
| `{parent}` | Parent directory path | - |
| `{name}` | Filename (with extension) | - |
| `{nameNoExt}` | Filename without extension | v5.0+ |
| `{extension}` | Extension (with `.`, lowercase) | v5.0+ |

Available variables in `workingDirectory`: `{parent}`, `{path}`, `{path0}` (equivalent to `{path}` for single selection).

## Multi-File Join Mode

Available variables in `paramForMultipleFiles` (falls back to `param` when empty):

| Variable | Meaning |
|----------|---------|
| `{path}` | **All paths pre-quoted with double quotes and joined by `pathDelimiter`** (e.g. `"C:\a.txt","C:\b.txt"`) |
| `{parent}` | First file's parent directory |
| `{path0}` | First file path (v5.6+) |
| `{name0}` | First filename (v5.6+) |
| `{nameNoExt0}` | First filename without extension (v5.6+) |
| `{extension0}` | First file extension (v5.6+) |

Available variables in `workingDirectory` (Join mode): `{parent}`, `{path0}`.

## Quoting Rules (Most Error-Prone)

1. **`{path}` does NOT auto-quote**. Paths with spaces must be wrapped manually:
   - Double quotes: `"param": "\"{path}\""`
   - Single quotes (for PowerShell): `"param": "'{path}'"`

2. **Join mode `{path}` is PRE-QUOTED by the host**. Adding your own quotes creates double-quoting and breaks parsing. Control the separator via `pathDelimiter` instead of adding outer quotes:
   - Want `"a","b"` → `pathDelimiter: ","`
   - Want `"a" "b"` → `pathDelimiter: " "` (default)
   - Want PowerShell single-quoted array `'a','b'` → **DO NOT** use `pathDelimiter: "','"` + template `'{path}'` — this produces `'"a"','"b"'` which includes literal double quotes in the path string.

3. **PowerShell Join mode (verified working pattern)**:
   - `pathDelimiter: "','"` — comma between host-pre-quoted paths
   - `paramForMultipleFiles: "-NoExit -Command Get-FileHash -LiteralPath '{path}' -Algorithm SHA256 | Format-List"`
   - The **single quotes** around `{path}` are critical. They tell PowerShell to treat the entire merged string as a literal (no `$`/backtick expansion) and handle spaces, Asian characters, and parentheses correctly.
   - **Do NOT use** `\\\"{path}\\\"` in `paramForMultipleFiles` — the backslash-double-quote escaping interacts incorrectly with the host's pre-quoted paths when paths contain spaces or parentheses.

4. **`exe` field follows the same rule**: paths with spaces need embedded quotes (`"\"C:\\Program Files\\7-Zip\\7zG.exe\""`); `%LocalAppData%` and other env vars are expanded by the host and don't need quotes if they contain no spaces.

5. **JSON escaping**: backslashes in templates must be written as `\\`, quotes as `\"`. PowerShell inner escaping becomes `\\\"` (e.g., `-Command Get-FileHash -LiteralPath \\\"{path}\\\"`).

## Environment Variable Expansion

The host calls `ExpandEnvironmentStringsW` on `exe`, `icon`, and `workingDirectory` when they contain `%`. Supports `%LocalAppData%`, `%ProgramFiles%`, etc. **`param` does NOT expand environment variables** (only menu variable substitution).

## Empty Values and Defaults

- Empty `param` → launches without arguments.
- Empty `workingDirectory` → single/Each mode uses the file's parent directory; Join mode uses the first file's parent directory.
- Empty `icon`/`iconDark` → no icon (default).
- Empty `pathDelimiter` → paths concatenated directly (usually not desired).

## Debugging

Use `customeContextMenuDebug.exe` (repo sample) to echo parameters and verify substitution results:

```json
{
  "title": "Debug Echo",
  "exe": "customeContextMenuDebug.exe",
  "param": "\"{path}\"",
  "acceptDirectoryFlag": 15,
  "acceptFileFlag": 4,
  "acceptMultipleFilesFlag": 2,
  "pathDelimiter": "','",
  "paramForMultipleFiles": "\"{path}\""
}
```

## PowerShell Multi-File Hash Example (Verified)

```json
{
  "title": "Compute SHA256",
  "index": 20,
  "exe": "pwsh",
  "param": "-NoExit -Command Get-FileHash -LiteralPath \\\"{path}\\\" -Algorithm SHA256 | Format-List",
  "icon": "\"C:\\Program Files\\PowerShell\\7\\pwsh.exe\",0",
  "acceptDirectoryFlag": 0,
  "acceptFileFlag": 4,
  "acceptMultipleFilesFlag": 2,
  "pathDelimiter": "','",
  "paramForMultipleFiles": "-NoExit -Command Get-FileHash -LiteralPath '{path}' -Algorithm SHA256 | Format-List",
  "showWindowFlag": 0,
  "runAsFlag": 0
}
```

**Why this works**:
- Single selection: `\\\"{path}\\\"` → `\"{path}\"` after JSON decode → `"C:\path with space.txt"` after CRT parsing → PowerShell sees a double-quoted string.
- Multi-selection Join: `'{path}'` + `pathDelimiter: "','"` → host generates `"path1","path2"` → template becomes `'"path1","path2"'` → PowerShell sees a single-quoted string containing `"path1","path2"` → PowerShell parses this as a string array `['path1', 'path2']` because the outer single quotes make the inner content literal, and the comma is interpreted as an array operator in the `-LiteralPath` argument context.