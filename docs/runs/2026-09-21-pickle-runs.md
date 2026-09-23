# Pickle runs of 2026-09-21

Text summary of the first three WSL passes of `Tests/Pickle/`. The raw evidence (reports, launcher
logs, captures) is **on disk only**, in `docs/runs/evidence/`, and ignored by git: it weighs about
78 MB after pruning. This file is what survives a clone. STATUS.md holds the reasoning; this holds the numbers.

All three: pass `sans-facultatifs`, English, `-pickle-run="Bill Autopilot - Pickle tests"`.
**None produced a verdict** (`exitReason` was never `passed` nor `failed`).

| Run | exitReason | Scenarios | Passed | Failed | Evidence |
| --- | --- | --- | --- | --- | --- |
| 1 | `infrastructure-error` | 0 of 65 selected (19 features discovered) | 0 | 0 | **lost**: the shared script archives five reports and this one rotated out. Cause was read from its `Player.log` before that; see below |
| 2 | `in-progress`, killed by the 5-minute stall watchdog | 43 of 65 | 20 | 23 | `evidence/2026-09-21-run2/`: summary, junit, log and only the captures of features 07 to 12, the rest superseded by run 3 |
| 3 | `watchdog-timeout` (10-minute stall setting) | 26 of 65 | 15 | 11 | `evidence/2026-09-21-run3/` |

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

`evidence/2026-09-21-run3/screenshots/manual--bills-tab--one-automatic-bill-and-one-placed-by-hand--step0.png`
is the `@review` capture of feature 06. Whether the marker reads in it is a judgement for a person; the
green scenario only says the path ran.

## Two defects in shared tooling seen here

Neither is fixed by this repository (`scripts/` belongs to the monorepo):

1. `PICKLE_DEPMAP=none` arrives empty through `WSLENV=...:PICKLE_DEPMAP/p`, so a mod owning
   `wsl-deps.map` gets it staged even in its minimal pass.
2. `Run-PickleWsl.ps1:367` writes `"pickle-reports-archive\bloque-..."` where `\b` is a literal
   backspace byte, so the stall archive fails and the `Player.log` it exists to keep is lost.
