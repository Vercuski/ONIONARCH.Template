#!/usr/bin/env bash
# Generates a solution from the ONIONARCH template for one option combination and verifies it:
#   1. builds with zero warnings (TreatWarningsAsErrors comes from Directory.Build.props)
#   2. all generated tests pass (when the test project is included)
#   3. no template directives survived generation
#   4. no identity leaks (ONIONARCH / Vercuski) remain
#   5. the generated .sln lists exactly the .csproj files on disk
#   7. generated JSON parses strictly, and host database settings match the generated providers
#   6. with every option on, the generated projects match the repo's real ONIONARCH.sln (drift guard)
#
# Usage: verify-template.sh <case-name> [dotnet new options...]
#   e.g. verify-template.sh api-postgres --Database PostgreSql
# Requires the template to be installed: dotnet new install <repo>/source
set -euo pipefail

CASE="$1"; shift
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
WORK="${VERIFY_WORK_DIR:-${TMPDIR:-/tmp}/onionarch-verify}/$CASE"
NAME="Acme.Verify"

fail() { echo "FAIL [$CASE]: $*" >&2; exit 1; }

rm -rf "$WORK" && mkdir -p "$WORK"
dotnet new onionarch -n "$NAME" -o "$WORK" "$@" >/dev/null

cd "$WORK"

# 3. leftover template directives
if grep -rnE '^[[:space:]]*(//)?#(if|else|elseif|elif|endif)\b|<!--#(if|else|elseif|endif)' \
     --include='*.cs' --include='*.csproj' --include='*.json' --include='*.sln' --include='Dockerfile' --include='*.props' . ; then
  fail "template directives left in generated output"
fi

# 4. identity leaks
if grep -rniE 'onionarch|vercuski' . ; then
  fail "template identity leaked into generated output"
fi

# 5. solution <-> disk consistency
sln_projects=$(dotnet sln "$NAME.sln" list | grep -E '\.csproj$' | tr '\\' '/' | sort)
disk_projects=$(find . -name '*.csproj' -not -path '*/bin/*' -not -path '*/obj/*' | sed 's|^\./||' | sort)
[ "$sln_projects" = "$disk_projects" ] || fail "solution and disk disagree:
  sln:  $(echo $sln_projects)
  disk: $(echo $disk_projects)"

# 6. drift guard: with everything on, the template must reproduce the reference solution's projects
if [ "${VERIFY_EXPECT_REFERENCE_PROJECTS:-false}" = "true" ]; then
  ref_projects=$(dotnet sln "$REPO_ROOT/source/ONIONARCH.sln" list | grep -E '\.csproj$' | tr '\\' '/' | sed "s/ONIONARCH/$NAME/g" | sort)
  [ "$sln_projects" = "$ref_projects" ] || fail "template solution has drifted from source/ONIONARCH.sln:
  template:  $(echo $sln_projects)
  reference: $(echo $ref_projects)"
fi

# 7. generated JSON is valid, and each host's database configuration matches the generated providers
python3 - "$NAME" <<'PY' || fail "configuration check failed"
import json, pathlib, re, sys
name = sys.argv[1]

def strip_jsonc(text):
    # Remove // and /* */ comments outside of strings; anything left must be strict JSON
    # (so a dangling comma left behind by a removed block fails here).
    out, i, in_str = [], 0, False
    while i < len(text):
        c = text[i]
        if in_str:
            out.append(c)
            if c == "\\": out.append(text[i + 1]); i += 1
            elif c == '"': in_str = False
        elif c == '"': in_str = True; out.append(c)
        elif text.startswith("//", i): i = text.find("\n", i) - 1 if "\n" in text[i:] else len(text)
        elif text.startswith("/*", i): i = text.index("*/", i) + 1
        else: out.append(c)
        i += 1
    return "".join(out)

ok = True
for path in pathlib.Path(".").rglob("*.json"):
    if any(part in ("bin", "obj") for part in path.parts): continue
    try: json.loads(strip_jsonc(path.read_text(encoding="utf-8-sig")))
    except Exception as e: print(f"  invalid JSON: {path}: {e}"); ok = False

provider_project = {"MSSQL": "SqlServer", "POSTGRESQL": "PostgreSql", "MYSQL": "MySql"}
has_mysql = pathlib.Path(f"{name}.Persistence.MySql").is_dir()
for settings in pathlib.Path(".").glob(f"{name}.Presentation.*/appsettings.json"):
    platform = json.loads(strip_jsonc(settings.read_text(encoding="utf-8-sig")))["DatabasePlatform"]
    for side in ("QueryDbPlatform", "CommandDbPlatform"):
        project = provider_project.get(platform[side].upper())
        if project is None or not pathlib.Path(f"{name}.Persistence.{project}").is_dir():
            print(f"  {settings}: {side}={platform[side]} has no generated provider project"); ok = False
    if ("MySqlServerVersion" in platform) != has_mysql:
        print(f"  {settings}: MySqlServerVersion present={'MySqlServerVersion' in platform}, MySQL provider present={has_mysql}"); ok = False
sys.exit(0 if ok else 1)
PY

# 1. build, zero warnings
build_log=$(dotnet build "$NAME.sln" -c Release 2>&1) || { echo "$build_log" | grep -E 'error|warning' | sort -u | head -20; fail "build failed"; }
echo "$build_log" | grep -qE '^\s*0 Warning\(s\)' || { echo "$build_log" | grep -E 'warning' | sort -u | head -20; fail "build produced warnings"; }

# 2. tests
if [ -d "$NAME.Tests" ]; then
  test_log=$(dotnet test "$NAME.sln" -c Release --no-build 2>&1) || { echo "$test_log" | grep -E 'Failed|error' | head -20; fail "tests failed"; }
  summary=$(echo "$test_log" | grep -E 'Passed!|Failed!' | sed -E 's/, Duration.*//; s/ +/ /g')
else
  summary="(no test project)"
fi

echo "PASS [$CASE] $(echo "$sln_projects" | wc -l | tr -d ' ') projects; $summary"
