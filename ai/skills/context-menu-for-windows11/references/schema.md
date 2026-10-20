# Schema Reference (menu.schema.json)

Single file = single menu item JSON object (`additionalProperties: false`). **Required: `title`, `exe`**.

All keys are camelCase, matching `ContextMenuCustomShare\Service\Menu\MenuItem` serialization names. Empty fields can be omitted (serializer uses `DefaultIgnoreCondition.WhenWritingNull`).

## Basic Fields

| Key | Type | Description |
|-----|------|-------------|
| `title` | string | Menu display text. Required; app `CheckMenu` rejects empty. |
| `exe` | string | Executable path. Supports env vars (`%LocalAppData%`), Store App (`shell:AppsFolder\...`), URI protocols (e.g. `quick-look://`), and bare command names (`cmd`, `explorer`, `powershell`). Host launches via `ShellExecute`. |
| `param` | string | Parameter template; see `variables.md` for variable substitution. |
| `icon` | string | Menu icon path. `.ico` file, or `exe/dll` + `,index` (e.g. `C:\WINDOWS\System32\SHELL32.dll,196`). Supports env vars. |
| `iconDark` | string | Dark-mode icon (optional; falls back to `icon`). |
| `index` | int | Sort weight; list sorts ascending by `index`. App rewrites to `i*10` on save; use multiples of 10 for hand-editing. |

## Matching Scope

### `acceptDirectoryFlag` (int, bitmask, schema range 0-15)

| Bit | Value | Scenario |
|-----|-------|----------|
| Directory | 1 | When a folder is selected |
| Background | 2 | Folder background (empty area) |
| Desktop | 4 | Desktop |
| Drive | 8 | Drive root |

- All = 15; none = 0 (host default 0).
- **Must explicitly include 1 to appear on folder right-click**; background-only (2) won't show on folders.

### `acceptFileFlag` (int, enum)

| Value | Mode | Semantics |
|-------|------|-----------|
| 0 | None | Not for files (directory/background matching still active) |
| 1 | Ext | Extension fuzzy match (legacy); `acceptExts` space- or comma-separated (e.g. `.apk .mapk` / `.apk,.mapk`); **contains `*` or empty → matches ALL files** |
| 2 | Regex | Filename regex match; requires `acceptFileRegex` (host `std::regex_match` full match) |
| 3 | ExtList | Exact extension list (recommended, v5.0+); `acceptExts` pipe-separated, each starting with `.` (e.g. `.apk\|.apkx`) |
| 4 | All | All files (default) |

**Conditional requirements** (schema `allOf`): `acceptExts` required for =1 or =3; `acceptFileRegex` required for =2. For =3, `acceptExts` must match `^\.[a-zA-Z0-9]+(\|\.[a-zA-Z0-9]+)*$`.

**Never write** `acceptFile` (bool, deprecated). Host compatibility: `acceptFileFlag==0 && acceptFile==true` auto-promotes to Ext (=1).

### `acceptMultipleFilesFlag` (int, multi-selection behavior)

| Value | Mode | Semantics |
|-------|------|-----------|
| 0 | None | Menu hidden on multi-select |
| 1 | Each | Execute once per file (loop `DoInvoke`) |
| 2 | Join | Single execution with merged paths (uses `pathDelimiter`, template `paramForMultipleFiles`, falls back to `param`) |

### `acceptMultipleFilesRuleFlag` (int, multi-selection filter rule, v5.0+)

Only active when multi-selecting AND value != Any(0); host calls `Accept` per item:

| Value | Mode | Semantics |
|-------|------|-----------|
| 0 | Any | Show if any item matches (default, no filtering) |
| 1 | One | Show if at least one item matches (stops on first match) |
| 2 | All | Show only if ALL items match (stops on first mismatch) |

### `pathDelimiter` / `paramForMultipleFiles` (string)

Join mode only:
- `pathDelimiter`: separator between pre-quoted paths; default space. Host format: `"path1"<delim>"path2"` (each path already has double quotes).
- `paramForMultipleFiles`: Join-mode parameter template; supports `{path}` (merged string), `{parent}`, `{path0}`, `{name0}`, `{nameNoExt0}`, `{extension0}`. Falls back to `param` when empty.

## Execution Behavior

### `showWindowFlag` (int)

| Value | Mode |
|-------|------|
| -1 | Hide |
| 0 | ShowNormal (default) |
| 1 | ShowMinimized |
| 2 | ShowMaximized |

Host calls `ShellExecute(..., _show_window_flag + 1)`, mapping to `SW_HIDE/SW_SHOWNORMAL/SW_SHOWMINIMIZED/SW_SHOWMAXIMIZED`. Use -1 for silent console scripts; GUI apps keep 0.

### `runAsFlag` (int)

| Value | Mode |
|-------|------|
| 0 | Default (`open` verb) |
| 1 | RunAsAdmin (always `runas`) |
| 2 | RunAsAdminWhileShift (`runas` only while Shift held) |

### `workingDirectory` (string)

Working directory template; supports `{parent}`, `{path0}` (v5.6+), env vars. Empty defaults to file's parent directory (Join mode: first file's parent).

## Deprecated Fields (read-only compatibility, never write)

- `acceptDirectory` (bool): legacy. Host: `acceptDirectoryFlag==0 && acceptDirectory==true` auto-sets `Directory\|Background\|Desktop` (=7).
- `acceptFile` (bool): legacy. Host: `acceptFileFlag==0 && acceptFile==true` auto-sets `acceptFileFlag=1`.
- App `ShouldSerializeAcceptDirectory/AcceptFile` both return false; values are dropped on save.

## Top-Level Constraints

- Single JSON object; **not an array**. Multiple menus = multiple `.json` files.
- Filename suggestion = `title` + `.json`; app renames invalid chars to `_`.
- Enable/disable = file extension: `.json` enabled, `.json.disabled` disabled (app renames).
