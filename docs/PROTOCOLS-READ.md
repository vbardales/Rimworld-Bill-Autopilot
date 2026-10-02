# Protocols read, and in which version

What the Bill Autopilot session read on **2026-10-02** (audit against `AUDIT.md`, at the owner's request). A version is
the last commit that touched the file in the repository that holds it; `M` means modified and not committed. Protocol
documents live in `vbardales/Rimworld-protocols` (git dir `../rimworld-protocols.git`, work tree = the monorepo root),
so `git log` from the monorepo would answer for the commit that removed them: read them with `--git-dir` as
`WELCOME.md` says. When a commit below has moved, the document changed: read it again before relying on it.
"Whole" means every line was read; "skimmed" means only the parts named; "not read" means not opened this time.

## Documents of the collection

| Document | Read | Version | Useful here? |
|---|---|---|---|
| `AGENTS.md` | whole (the copy loaded as project instructions) | 7fd7475, 2026-09-29 | Yes: the evidence rule (latest report per scenario, one line per run in `docs/runs/`), publishing by CI |
| `AUDIT.md` | whole | 7fd7475, 2026-09-29, 328 lines | Yes: it is the task. Stage chain, `done -> showcase/preTest` step, Pickle rules, title of the session |
| `MOD_SETTINGS.md` | whole | b83933b, 2026-09-23, 107 lines | Yes: `settings_audit` values; this mod stays `partial` for runtime checks only |
| `TRANSLATIONS.md` | whole | c105a43, 2026-10-01, 213 lines | Yes: plural keys, the French review by the owner, `FRENCH_REVIEW.md`, gender switch (none apply here) |
| `PUBLISHING.md` | whole | 02394c0, 2026-10-01 (M), 790 lines | Yes: Markdown description source, gallery `0-` copy of the Preview, thanks register, pathspec commits, one index shared by sessions |
| `WORKSHOP_COMMENTS.md` | whole | 7fd7475, 2026-09-29, 159 lines | Yes for the register rows of this mod (all `drafted`); the writing method is only needed when drafts are rewritten |
| `STYLE_RIMWORLD.md` | **not read** | c105a43, 2026-10-01 (M), 716 lines | **No**: the graphic style of Preview and ModIcon, which a session never generates here. Only the file limits matter (Preview under 1 MB, ModIcon checked at 32 px by the owner) |
| `scripts/SEARCHING.md` | **not read** | 50de695, 2026-09-28, 222 lines | **No**: searching the mod corpus, which this audit did not need (and the session's own memory forbids sweeping the tree) |

## Tools

| Document | Read | Version | Useful here? |
|---|---|---|---|
| `PickleTools/README.md` | whole | ff20d89, 2026-09-29 (M), 89 lines | Yes: the table of tools (RimmsqolSteps, ClickDiagnostics, ScreenshotStudio) |
| `PickleTools/Authoring/README.md` | evidence parts (section 6-7 and the `Evidence` lines) | a47799f, 2026-09-29, 323 lines | Yes, for what to keep after a run (`-EvidenceDir`, summary, junit, minified captures) |
| `PickleTools/Headless/README.md` | **not read** | ed4e73a, 2026-09-26, 509 lines | No for this audit (no run was filed). Needed before the next pass: filter terms, `-DepMap`, traps |
| `PickleTools/docs/steps.md` | **not read** | da7c3b0, 2026-09-28 (M), 285 lines | No for this audit. A catalogue to consult before writing a step |
| `Rimworld-Release-Admin/docs/OPERATIONS.md` | whole | 3c03f51, 2026-09-26, 112 lines | Yes: manual `publish-tag.yml`, dry-run of the exact SHA, full 40-character SHA, the gallery is manual |
| `Rimworld-Ticket-Dispatcher/docs/WELCOME.md` | whole | 77ca9d7, 2026-09-27, 150 lines | Yes: which test for which task, no watcher, a request carries no SHA, the list of documents to note |
| `Rimworld-Ticket-Dispatcher/docs/SUBMIT.md` | whole | d07b2b8, 2026-09-26, 133 lines | Yes for the next request: options, `-EvidenceDir`, exit codes |

## Documents of this repository (HEAD `f269fe9`)

| Document | Read | Version | Useful here? |
|---|---|---|---|
| `STATUS.md` | head (front matter, sections down to 2026-09-28), the "Workflow audit" decision and commands | 31cc7b6 (M) | Yes. Its front matter sat below a section; moved back to line 1 on 2026-10-02 |
| `README.md` | not reread (the audit did not touch it) | a12568a, 2026-09-25 | Not needed |
| `CHANGELOG.md` | whole | e551bbb, 2026-09-23 | Yes. `1.0.0 - unreleased` above `0.1.0` |
| `ATTRIBUTION.md` (root and `Mod/`) | not reread | 53cdb38, 2026-09-25 | Not needed now; the two copies must stay identical |
| `LICENSE` | not reread | 3942ebc, 2026-09-19 | Not needed |
| `PUBLICATION.md` | structure only | a12568a, 2026-09-25 (M) | Yes (Markdown description source, change note, thanks drafts); not needed for this audit's edits |
| `TESTING.md` | the evidence and "gate to `tested`" sections | 31cc7b6, 2026-09-25 (M) | Yes; the evidence table was added 2026-10-02 |
| `BACKLOG.md` | whole | untracked (new), the mod's own, not the monorepo's | Yes |
| `docs/runs/pickle-runs.md` | whole | 31cc7b6 (M) | Yes; pruning recorded 2026-10-02 |
| `Tests/Pickle/` | feature list, tags (`@wip` search), pass maps | 53cdb38 (README) | Yes |
| `Mod/About/About.xml` | head, dependencies, tail | a12568a (M) | Yes |
| `NOTES.md`, `BUGS.md` | do not exist | | |

## Useless this time, not to reread when they change

`STYLE_RIMWORLD.md`, `scripts/SEARCHING.md`, `PickleTools/docs/steps.md` and `PickleTools/Headless/README.md` were not
needed to apply this audit. Reread the last two before the next Pickle request, the first two never for this mod.

## What reading them changed

- Version `1.0.0` stays `unreleased` in `CHANGELOG.md` until the CI publishes it; the working-tree edit that dated it was reverted.
- `ValidateXml.ps1` accepts the generated plain-text link of the Markdown description source.
- Evidence was pruned to the latest report per scenario (306 MB to 6.6 MB) and the proofs worth keeping are listed in `TESTING.md`.
- Commits use a pathspec (`git commit -- <paths>`): the index is shared with other sessions (a preview session committed `f269fe9` during this audit).
