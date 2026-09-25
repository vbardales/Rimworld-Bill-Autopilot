# Protocols read, and in which version

What the Bill Autopilot session read of the workspace protocols and of this repository's own documents, on
**2026-09-25**, after a context compaction and at the owner's request. A version is the last commit that touched the
file in the repository that holds it, plus the first 12 characters of the SHA256 of the file **as read**; when a hash
no longer matches, the document changed and must be read again before it is relied on. Line counts are those of the
file read.

**Where the protocols live.** `AGENTS`, `AUDIT`, `PUBLISHING`, `TRANSLATIONS`, `STYLE_RIMWORLD` and `scripts/SEARCHING`
left the monorepo: the repository `vbardales/Rimworld-protocols` owns them, with the collection folder as its work tree
(git dir `Documents\rimworld-protocols.git`, HEAD `7546e86` when read). Commits below are that repository's. The other
documents are read from the repository that holds each (`PickleTools` `587488f`, `Rimworld-Release-Admin` `2ce34a3`,
`Rimworld-Ticket-Dispatcher` `4130d70`, this repository `0320020`).

## Documents of the collection

| Document | Read | Version | Useful here? |
|---|---|---|---|
| `AGENTS.md` | whole | 3a1d2cb, 2026-09-24, 36631e730433, 46 lines | Yes: the evidence rule (latest report per scenario, history as one line per run, never a folder), publishing by CI |
| `AUDIT.md` | whole | 7546e86, 2026-09-25, 1c4ace2867de, 232 lines | Yes: the stage chain, the Pickle rules, requests without a SHA, two passes named, fail fast (the wording rewritten today is committed as 359a460) |
| `PUBLISHING.md` | whole | 04aa365, 2026-09-25, 3d83491eb5bf, 683 lines | Yes: thanks and links, the gallery folder rule, the CI section, the fail-fast line, the pitfalls of a shared index |
| `TRANSLATIONS.md` | whole | b83933b, 2026-09-23, 3368579d01dc, 100 lines; **changed by this session afterwards**: f5c2d9d, 2026-09-25, 298f74d226da, 112 lines (the "Counts and plurals" bullet) | Yes, and the rule that a change to texts resets the language fields until revalidated |
| `STYLE_RIMWORLD.md` | whole | 7311308, 2026-09-25, de13cbe5e1f9, 484 lines | **No**: the graphic style of Preview and ModIcon. Only the file constraints matter (Preview 896x504 under 1 MB, ModIcon 128 px checked at 32 px, checked by the owner, never generated here) |
| `scripts/SEARCHING.md` | whole | 372c447, 2026-09-23, 9dbd52b2bcd4, 168 lines | **No**: searching the mod corpus. One line worth keeping: the session's own Grep and Glob tools time out after 20 seconds on the collection folder and return nothing, so search a named directory |
| `PickleTools/README.md` | whole | d20db95, 2026-09-25, 86a5939ae1f3, 83 lines | Yes, for one lead (see below): `RimmsqolSteps` |
| `PickleTools/Headless/README.md` | whole | b2712fc, 2026-09-25, 988dbf0dcee7, 487 lines | Yes: filter terms, `-DepMap`, passes, the traps (report names longer than MAX_PATH, the game logs in UTC, waits) |
| `Rimworld-Release-Admin/docs/OPERATIONS.md` | whole | 2ce34a3, 2026-09-25, 41428924093a, 207 lines | Yes: the manual publish workflow, description sources, dry-run, gallery is manual |
| `Rimworld-Ticket-Dispatcher/docs/WELCOME.md` | whole | 4130d70, 2026-09-25, **modified, uncommitted**, b9f93a680f17, 97 lines | Yes: which test for which task, no watcher, no SHA in a request, note the version read |
| `Rimworld-Ticket-Dispatcher/docs/SUBMIT.md` | whole | **untracked** (new file), b91f10de193f, 131 lines | Yes: every option of a request, the exit codes |
| Pickle `Docs/steps.md` | whole | **no local copy**: fetched from GitHub, `RimWorks/Rimworld-Pickle` tag `v4.9.1` (`e22b90a`), 0b82a2617e10, 462 lines, kept in the session scratch folder | Yes, as a catalogue: no step this suite needs is missing from it |

## Documents of this repository

| Document | Read | Version | Useful here? |
|---|---|---|---|
| `STATUS.md` | lines 1 to 400 of 940 | 0320020, 2026-09-25, **modified, uncommitted**, b9ccc7aad121 | Yes. Lines 400 onward are older history (2026-09-01 to 21), not reread |
| `README.md` | whole | e551bbb, 2026-09-23, **modified, uncommitted**, ec1143ada1f0, 203 lines | Yes |
| `CHANGELOG.md` | whole | e551bbb, 2026-09-23, bb7391009718, 71 lines | Yes |
| `ATTRIBUTION.md` (root and `Mod/`) | whole | 3942ebc, 2026-09-19, 61e3ed032283, 69 lines; the two copies are identical | Yes, see the findings |
| `LICENSE` | whole | 3942ebc, 2026-09-19, ae6ae5fa894c, 21 lines; identical to `Mod/LICENSE` | Checked, nothing to do |
| `PUBLICATION.md` | whole | 96b28eb, 2026-09-24, **modified, uncommitted**, bcaacdffc2c3, 225 lines | Yes |
| `TESTING.md` | whole | e551bbb, 2026-09-23, f5a56e40a7b4, 562 lines | Yes, and partly out of date (see below) |
| `Tests/Pickle/README.md` | whole | e551bbb, 2026-09-23, 12085e3a89e3, 137 lines | Yes, and partly out of date |
| `docs/runs/pickle-runs.md` | first 40 lines | 0320020, 2026-09-25, **modified, uncommitted**, ddfe5ac8f4f4 | Written by this session, not reread |
| `Mod/About/About.xml` | whole | 96b28eb, 2026-09-24, **modified, uncommitted**, 5e802880b1bc, 99 lines | Yes |
| `BACKLOG.md`, `NOTES.md`, `BUGS.md` | | do not exist in this repository | |

## What reading them changed

- **A request carries no SHA, and the tree must not move until `RUN_DONE`** (`AUDIT.md`, `WELCOME.md` point 4). This
  session did not do it: the labels named a DLL hash, not a commit, and files under `Mod/` (`About.xml`) and the
  features were edited while requests were queued (`5a32` was filed at 13:54 and `About.xml` changed at 14:05). Nothing
  observed was wrong for that reason, but from now on: file a request only when the tree is frozen, write the commit SHA
  in `-Label`, and change nothing under `Mod/` or `Tests/Pickle/` until the `RUN_DONE`.
- **Fail fast** needs, before a `publish`: no scenario left red without a green replay, the gallery, the owner's manual
  validations, the exact-commit dry-run, the full SHA, the owner's approval. Only the regression pass follows.
- **Thanks** must cover every integration named, claimed or exercised, the test-only ones included: done in
  `About.xml`, `PUBLICATION.md` and the register on 2026-09-25 (nothing posted, the item is private).
- **Evidence**: `report.html` and `messages.ndjson` of a superseded build prove nothing and are not kept; captures
  reduced to JPEG; delete with `robocopy /MIR` when `Remove-Item` stalls on long names.

## Findings, and what was not done about them

1. **`TRANSLATIONS.md` asks to translate complete sentences and not to assemble them from translated fragments.** The
   plural fix of 2026-09-24/25 passes a translated noun phrase (`Recipes.One/Many`, `Overridden.*`) as an argument into
   sentences. It fixes "1 recipes" and "1 surchargées", but it is a fragment argument in the strict reading. The
   compliant alternative is a `.One` and a `.Many` variant of each whole sentence. **Decided by the owner on
   2026-09-25: keep the mechanism** and write it into `TRANSLATIONS.md` as a bounded exception ("Counts and
   plurals", commit f5c2d9d of the protocols repository). Checked beforehand: the engine's `LanguageWorker_French.
   Pluralize` ignores its `count` argument, so it cannot serve a count in French.
2. **Out of date in this repository**, not corrected: `TESTING.md` and `Tests/Pickle/README.md` still say the suite is
   "not yet run" and that the pass with the optional mods mounts six neighbours plus No Max Bills (it now mounts TD
   Find Lib and TDS Bug Fixes too); the gate to `tested`, condition 2, says features 13 to 17 "have never run" (they
   ran on 2026-09-25); several `remaining` lines of `STATUS.md` are superseded; its `session:` field is an older id.
3. **`CHANGELOG.md` names the Everybody Gets One modes as "one per person, X per person, with surplus".** This note
   first called that wrong, after reading one `Defs` folder of the mod; the replay `5a32` showed the game loads
   `TD_PersonCount`, `TD_XPerPerson` and `TD_WithSurplusIng`, so the CHANGELOG was right and nothing is to be done.
   Lesson kept: which `Defs` folder a mod loads is decided by its `LoadFolders.xml`, not by the first folder read.
4. **`ATTRIBUTION.md` (both copies) lists six neighbours and no test tool**: No Max Bills, TD Find Lib, TDS Bug Fixes,
   Pickle and RimLogging are missing, and the copies must stay identical.
5. **A lead on the manual test that blocks `tested`.** `PickleTools/RimmsqolSteps` reveals, hides and forgets a
   main-bar button through RIMMSQOL's own settings and asserts what the bar draws. Condition 3 of the gate asks for
   manual tests to be automated and green or listed as not applicable; the RIMMSQOL entry (the shortcut revealed in its
   own interface, and its visibility surviving a restart) may be reachable that way. Not verified; not written.
6. **Two shared documents were still moving when read**: `WELCOME.md` (modified, not committed) and `SUBMIT.md`
   (untracked). Reread them, and this note's hashes, before relying on a rule that depends on them.
