---
name: ContextMenuForWindows11
description: Generate ContextMenuForWindows11 context-menu config JSON files. Use whenever the user asks to create, generate, or fix a custom right-click menu item (menu config, right-click menu, custom_commands) for this project, or wants help with menu.schema.json fields.
---

# ContextMenuForWindows11 Menu Config Generator

You are generating context-menu configuration JSON files for [ContextMenuForWindows11](https://github.com/ikas-mc/ContextMenuForWindows11). Each menu item is a single `.json` file stored in the app's `custom_commands` directory (`%LocalAppData%\Packages\<package>\LocalState\custom_commands\`). The host `ContextMenuCustomHost` parses each file and registers it into the Windows 11 right-click menu.

**Before generating any config, you MUST read:**
1. `references/schema.md` — complete field reference and validation rules
2. `references/variables.md` — variable substitution, quoting semantics, and PowerShell integration patterns

Both documents are cross-verified against the actual source code: `menu.schema.json`, `ContextMenuCustomApp`/`ContextMenuCustomShare` (C# models and serialization), and `ContextMenuCustomHost` (C++ host execution semantics).

## Standard Workflow

1. **Clarify requirements** (ask if unclear):
   - Target program and invocation style (direct exe? cmd/powershell wrapper?)
   - Where should the menu appear: files / folders / background / desktop / drives? (determines `acceptDirectoryFlag`)
   - File type filtering (determines `acceptFileFlag` + `acceptExts`/`acceptFileRegex`)
   - Multi-selection behavior (determines `acceptMultipleFilesFlag` + Join-related fields)
   - Admin elevation / hidden window needed?

2. **Select field values** using the decision tables in `references/schema.md`. Prefer modern Flag fields; never write deprecated boolean fields (`acceptDirectory`/`acceptFile`).

3. **Generate** a single `.json` file (or output for user to paste). Filename = title (Chinese characters OK). **Validate against `menu.schema.json`** after generation.

4. **Tell the user how to install**: copy JSON to `custom_commands` folder → click "Build Cache" in the app (or enable cache in settings) → restart Explorer.

## Hard Rules

- **Top-level must be a single JSON object** (not an array). Multiple menus = multiple files, ordered by `index`.
- **Required fields**: `title`, `exe`. Omit empty fields (serializer uses `WhenWritingNull`).
- **All keys must be camelCase**, matching `MenuItem` property serialization names: `title`, `exe`, `param`, `icon`, `iconDark`, `acceptDirectoryFlag`, `acceptFileFlag`, `acceptExts`, `acceptFileRegex`, `acceptMultipleFilesFlag`, `acceptMultipleFilesRuleFlag`, `pathDelimiter`, `paramForMultipleFiles`, `index`, `showWindowFlag`, `runAsFlag`, `workingDirectory`.
- **Never output** deprecated boolean fields `acceptDirectory`/`acceptFile` (`ShouldSerialize` returns false; new configs must not rely on compatibility logic).
- **Never invent fields**; schema has `additionalProperties: false`.
- Enum fields are always integers, never strings.

## Critical Pitfalls (from host source code behavior)

1. **Paths with spaces in `exe`/`param`**: quotes are part of the JSON string value. Example: `"exe": "\"C:\\Program Files\\...\\7zG.exe\""`. Directly resolvable paths (e.g., `cmd`, `explorer`) don't need quotes.

2. **`{path}` does NOT auto-quote**. For paths with spaces, wrap manually: `"\"{path}\""` or `"'{path}'"`.

3. **Join mode `{path}` is pre-quoted**: The host generates `"path1"<delimiter>"path2"`. Each path already has double quotes. Adding your own quotes creates double-quoting and breaks parsing.

4. **PowerShell Join mode pattern** (verified working):
   - `pathDelimiter: "','"` — comma between pre-quoted paths
   - `paramForMultipleFiles: "-NoExit -Command Get-FileHash -LiteralPath '{path}' -Algorithm SHA256 | Format-List"`
   - The single quotes around `{path}` are critical — they prevent PowerShell from expanding `$` or backticks in paths, and handle spaces/Asian characters/parentheses correctly.
   - **Do NOT use** `\\\"{path}\\\"` in `paramForMultipleFiles` — the double-quote escaping interacts incorrectly with the host's pre-quoted paths when paths contain spaces or parentheses.

5. **Ext mode (=1) wildcard behavior**: `acceptExts` containing `*` or empty → matches ALL files. For exact extension lists, use ExtList (=3) with `|`-separated lowercase extensions.

6. **`acceptFileFlag=0`** means "not for files" (directory/background matching still works). For all file types use 4 (All). Don't write `"acceptFileFlag":0` with `acceptExts`.

7. **`acceptDirectoryFlag` is a bitmask**: Directory=1, Background=2, Desktop=4, Drive=8. Both folder and background = `3`; all = `15`. **Defaults to 0** (host `GetNamedNumber` default) — no directory matching unless explicitly set.

8. **`showWindowFlag`**: `0` = normal window, `-1` = hidden. GUI apps stay `0`; console scripts use `-1` (note: this only minimizes console windows, for true background use a powershell/cmd wrapper with window-hidden flags).

9. **Admin**: `"runAsFlag": 1` always admin; `2` = admin only while Shift is held (host detects Shift key).

10. **`index` ordering**: app sorts ascending by `index`; on save it rewrites to `i*10`. When hand-editing, use multiples of 10 to leave insertion gaps.

## Validation Steps

1. After generation, validate against root `menu.schema.json` using any JSON Schema draft-07 validator.
2. Manually check hard rules (single object, camelCase, no deprecated fields, no invented fields).
3. For icons: `icon` supports `.ico` or `exe,dll` + `,index` (e.g., `SHELL32.dll,196`); use `iconDark` for dark mode.

## Output Format

- Single menu item: provide complete JSON (indented, matching app save format).
- Multiple menu items: separate JSON code blocks, each saveable as an independent file, with suggested filename.
- When user asks for "a config", also mention the `custom_commands` folder and cache-build step.

## Minimal Examples

Open-with style (right-click file → open with VSCode):

```json
{
  "title": "Open With VSCode",
  "index": 10,
  "exe": "\"%LocalAppData%\\Programs\\Microsoft VS Code\\Code.exe\"",
  "param": "\"{path}\"",
  "icon": "\"%LocalAppData%\\Programs\\Microsoft VS Code\\Code.exe\",0",
  "acceptDirectoryFlag": 3,
  "acceptFileFlag": 4,
  "acceptMultipleFilesFlag": 1,
  "showWindowFlag": 0,
  "runAsFlag": 0
}
```

Archive extraction (right-click zip/rar/7z → extract with 7z, files only, exact extensions):

```json
{
  "title": "Extract by 7z",
  "index": 11,
  "exe": "\"C:\\Program Files\\7-Zip\\7zG.exe\"",
  "param": "x \"{path}\" -o\"{parent}\\{nameNoExt}\"",
  "icon": "\"C:\\Program Files\\7-Zip\\7zFM.exe\",0",
  "acceptDirectoryFlag": 0,
  "acceptFileFlag": 3,
  "acceptExts": ".zip|.rar|.7z",
  "acceptMultipleFilesFlag": 1,
  "showWindowFlag": 0,
  "runAsFlag": 0
}
```

PowerShell multi-file hash (Join mode, single window, handles spaces/Asian characters):

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