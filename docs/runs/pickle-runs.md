# Pickle runs

Text summary of the WSL passes of `Tests/Pickle/`. The raw evidence (reports, launcher
logs, captures) is **on disk only**, in `docs/runs/evidence/`, and ignored by git. It weighs under 3 MB after
pruning and converting the captures to reduced JPEG (full size, quality 88, for the `@review` one).
This file is what survives a clone. STATUS.md holds the reasoning; this holds the numbers.

All runs: pass `sans-facultatifs`, English, `-pickle-run="Bill Autopilot - Pickle tests"`.
Runs 1 to 3 produced no verdict; **run 4 is the first: `exitReason: failed`, all 65 scenarios selected and run** (its evidence is deleted, see the table).
Runs 5a and 5b are on the build that fixes its three shipped defects; from 5b on, runs are small requests
through the TicketDispatcher.

| Run | exitReason | Scenarios | Passed | Failed | Evidence |
| --- | --- | --- | --- | --- | --- |
| 14 | `failed` (queued 2026-09-26, run 2026-09-27, request `5d66`, feature 23, pass `sans-facultatifs`; the tree is staged at run time, so the exact build it ran against was not recorded) | 4 of 4 played | 3 | 1 | deleted 2026-10-02 (`evidence/2026-09-26-gallery-1-4/`; old test colony, replaced by the zen studio runs): images 1, 3, 4 were good captures; image 2 failed on a feature-file mistake (a step text matched an assertion instead of a setter, see STATUS.md) before its window ever opened. Fixed, queued for retry as `ba6d` |
| 1 | `infrastructure-error` | 0 of 65 selected (19 features discovered) | 0 | 0 | **lost**: the shared script archives five reports and this one rotated out. Cause was read from its `Player.log` before that; see below |
| 2 | `in-progress`, killed by the 5-minute stall watchdog | 43 of 65 | 20 | 23 | deleted 2026-09-23: superseded by run 4, which replayed every scenario |
| 3 | `watchdog-timeout` (10-minute stall setting) | 26 of 65 | 15 | 11 | deleted 2026-09-23: superseded by run 4 |
| 4 | `failed` (2026-09-23, `-pickle-no-http`) | 65 of 65: 40 passed, 11 failed, 14 skipped | 40 | 11 | deleted 2026-09-26: report of the previous build, replaced by the French passes on build 4B89FBA7 |
| 5a | `watchdog-timeout` (2026-09-24, new build) | 4 of 65: the load of the save took 78 s on a loaded machine, past the 120 s scenario limit | 4 | 0 | deleted 2026-09-24: no verdict, superseded by 5b |
| 6a | `passed` (2026-09-24, build 8E3C5CC7, request `6e5f`, features 01 to 06, pass `avec-facultatifs`) | 27 of 27, none skipped | 27 | 0 | deleted 2026-09-26: build superseded, replayed by run 11 |
| 13 | `passed` (2026-09-26, build 4B89FBA7, request `7018`, features 13-19 and 22, pass `avec-facultatifs`, French run) | 23 of 23 | 23 | 0 | `evidence/2026-09-25-fr3-13-22/`: five `@review` captures (not yet opened) |
| 12 | `passed` (2026-09-26, build 4B89FBA7, request `dd78`, features 07-12, pass `avec-facultatifs`, French run) | 20 of 20 | 20 | 0 | `evidence/2026-09-25-fr2-07-12/`: no screenshot, no `@review` |
| 11 | `passed` (2026-09-26, build 4B89FBA7, request `baba`, features 01-06, pass `avec-facultatifs`, French run) | 27 of 27 | 27 | 0 | `evidence/2026-09-25-fr1-01-06/`: one `@review` capture (bills tab, not yet opened) |
| 10 | `passed` (2026-09-25, build 4B89FBA7, request `932e`, feature 22, pass `avec-facultatifs`) | 2 of 2 | 2 | 0 | `evidence/2026-09-25-bwm-22/`: restriction on a tick-created bill, agreement of the widened count |
| 9 | `passed` (2026-09-25, build 4B89FBA7, request `cf3a`, pass `removal`, `-Then` chain of two launches) | 1 of 1 in each launch | 2 | 0 | `evidence/2026-09-25-removal-chain/seq1`, `seq2`: a game saved with the mod loads and runs without it |
| 8 | `passed` (2026-09-25, build 4B89FBA7, request `dafa`, feature 20, pass `avec-rimmsqol`) | 3 of 3 | 3 | 0 | `evidence/2026-09-25-rimmsqol-20/`: three `@review` captures opened and read |
| 7c | `passed` (2026-09-25, build 4B89FBA7, request `115b`, feature 14, pass `avec-facultatifs` + 2 libraries) | 3 of 3 | 3 | 0 | deleted 2026-09-26: replayed by run 13 on the same build |
| 7b | `failed` (2026-09-25, build 4B89FBA7, request `5a32`, features 14 and 15, pass `avec-facultatifs` + 2 libraries) | 7 of 7 played | 5 | 2 | deleted 2026-09-26 (replayed by run 13): feature 15 green; feature 14 red on mode names I had wrongly renamed and on a bill already standing; replay `115b` queued |
| 7a | `failed` (2026-09-25, build 4B89FBA7, request `c5dd`, features 13 to 15, pass `avec-facultatifs` + 2 libraries) | 10 of 10 played | 7 | 3 | deleted 2026-09-26 (replayed by run 13): feature 13 all green; the 3 failures were scenario errors (mode names, bill cap, starting state) |
| 6d | `passed` (2026-09-25, build 8E3C5CC7, request `c6a4`, features 13 to 17, pass `sans-facultatifs`) | 15 of 15: 14 skipped by requirement, 1 passed | 1 | 0 | `evidence/2026-09-24-skipcheck-13-17/` |
| 6c | `failed` (2026-09-25, build 8E3C5CC7, request `479c`, features 13 to 19, pass `avec-facultatifs`) | 21 of 21 played | 17 | 4 | deleted 2026-09-26 (replayed by run 13): first play of the optional scenarios; 2 mod defects and 1 environment gap, see STATUS.md |
| 6b | `passed` (2026-09-25, build 8E3C5CC7, request `1f5e`, features 07 to 12, pass `avec-facultatifs`) | 20 of 20, none skipped | 20 | 0 | deleted 2026-09-26: replayed by run 12 |
| 5b | `passed` (2026-09-24, build 436E7179, before the plural fix; request `41c5`, features 09, 18, 19) | 12 of 12 | 12 | 0 | deleted 2026-09-26: replaced by the passes on build 4B89FBA7 |

## Pruned 2026-10-02

Deleted from `docs/runs/evidence/`, each replaced by a later report of the same scenarios (list made before deleting):
`2026-09-25-bwm-22` (feature 22, replayed in `fr3-13-22`); `2026-09-26-gallery-1-4`, `-gallery-5`, `2026-09-27-gallery-1-4-retry`,
`-retry2`, `-retry3` (gallery images 1-5 in the test colony the owner judged unusable); `2026-09-27-nomaxbills-26` and
`2026-09-27-replay-fixes` (0 scenarios played); `2026-09-28-galerie-studio-1-4`, `-1-4b` (red or partial, replaced by `1-4c`) and
`-5` (red, replaced by `5b`). Kept: `2026-09-24-skipcheck-13-17`, `2026-09-25-fr1-01-06`, `fr2-07-12`, `fr3-13-22`,
`removal-chain`, `rimmsqol-20`, `2026-09-27-nomaxbills-26-retry` (2 of 2), `2026-09-27-replay-fixes-v2` (13 of 13),
`2026-09-28-galerie-studio-1-4c` (3 of 4; image 3 red on a test-isolation leak, replay `24d` queued) and `2026-09-28-galerie-studio-5b` (1 of 1).

## Run 1 — no scenario played

Pickle builds its whole step table before running anything. One invalid pattern out of 87 stopped it:

```
Invalid step pattern: Bill Autopilot syncs the {string} at ({int}, {int})
An optional may not contain a parameter type.
```

Parentheses are optional text in a Cucumber Expression; 36 patterns carried them. The same log showed
Better Workbench Management, Nice Bill Tab, its Expansion and Dubs Mint Menus detected, with the bill
ceiling at 2147483647: the pass meant to be minimal had picked up `wsl-deps.map` by default (see
STATUS.md, "And the minimal pass was not minimal").

## Run 2 — 43 scenarios, stalled on the save round trip

| Passed / played | Feature |
| --- | --- |
| 4/4 | 01 loading |
| 3/4 | 02 activation |
| 2/5 | 03 base loop |
| 0/4 | 04 new recipe |
| 2/5 | 05 delete refuses |
| 4/4 | 06 the marker |
| 1/4 | 07 drift capture |
| 0/1 | 08 hand-placed wins |
| 1/4 | 09 uncountable |
| 1/3 | 10 bill cap |
| 0/2 | 11 per-bench memory |
| 2/3 | 12 save and reload |

Failure causes, all in the suite, none proven against the mod:

- 7 x "no free cell for a stockpile at (134, 150)": the fixture colony occupies that square.
- 4 x `Make_Apparel_Pants is already known`: `test-colony` has ComplexClothing researched.
- 3 x `ButcherCorpseFlesh reports a countable product`: **the one open question about the mod.**
  TESTING.md scenario 9 says it cannot be counted.
- 3 x `Object reference not set to an instance of an object`, with no stack: unexplained.
- 2 x `the letter stack is not empty: Fallen monolith`: the assertion demanded an empty stack.
- 2 x `8 bills up, not 2` / `not 3`: the autopilot had filled its allowance before the cap was lowered.

Stopped on scenario 44, `the save round trips`, after a reload that had taken 62 s.

## Run 3 — 26 scenarios, stalled after feature 07

| Passed / played | Feature |
| --- | --- |
| 4/4 | 01 loading |
| 4/4 | 02 activation |
| 1/5 | 03 base loop |
| 0/4 | 04 new recipe |
| 2/5 | 05 delete refuses |
| 4/4 | 06 the marker |
| 1/1 | 07 drift capture (first scenario only) |

Three features fully green. The 11 failures were two causes, both corrected afterwards and **not yet
replayed**:

- `GenPlace.TryPlaceThing` refuses a cell and returns false; the answer was ignored, so no stock was
  spawned and "the autopilot counts 0 of Make_Patchleather, not 30" read as a broken threshold.
- The stove filled its eight-bill allowance with cooking recipes before reaching `Make_Pemmican`.

The last line of the log is `dashboard request failed`: Pickle's HTTP server went down and the runner
with it. Untested hypothesis: `-Extra '-pickle-no-http'` avoids it.

## Capture of note

The `@review` capture of feature 06 (run 4, since deleted with its report). Opened on 2026-09-23: the automatic bill reads
"Make patchleather (auto)" and the hand-placed one, "Make pants", carries no mark. So the marker does read
in the vanilla tab. A green scenario alone would only say that the path ran.

## Two defects in shared tooling seen here

Neither is fixed by this repository (`scripts/` belongs to the monorepo):

1. `PICKLE_DEPMAP=none` arrives empty through `WSLENV=...:PICKLE_DEPMAP/p`, so a mod owning
   `wsl-deps.map` gets it staged even in its minimal pass.
2. `Run-PickleWsl.ps1:367` writes `"pickle-reports-archive\bloque-..."` where `\b` is a literal
   backspace byte, so the stall archive fails and the `Player.log` it exists to keep is lost.
