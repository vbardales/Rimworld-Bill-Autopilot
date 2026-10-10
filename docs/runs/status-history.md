# STATUS.md history

One line per dated section folded out of `STATUS.md` on 2026-10-10 (AUDIT.md: `STATUS.md` keeps the current state
only; the full text of each section is in git at `84b1787`, `git show 84b1787:STATUS.md`). Pickle numbers per run are in
`pickle-runs.md`.

| Date | Section | What it established |
| --- | --- | --- |
| 2026-10-02 | Audit against AUDIT.md of 2026-09-25 | Stage kept at `done`; XML 476 and unit 74 checks green; DLL `ba624725` equals the sources; evidence pruned 306 MB to 6.6 MB; no `@wip`; Workshop item 3806709456 private, `1.0.0` still unreleased. Replaced by the 2026-10-10 audit |
| 2026-10-02 | Preview source migration | Copy, layout and palette moved to `Art/Preview.config.json`; shared renderer; superseded JSON removed |
| 2026-09-29 | 24b2 red | Gallery image 3 failed on a test-isolation leak in `ProfileSteps.ResetToDefaults` (seeded/known/pending not cleared), not a mod defect; fixed, replay `24d` filed, no report on disk yet |
| 2026-09-28 | 5da4 red | Pickle ran a step off the main thread (images 1, 3, 4); not this mod; image 5 `6513` green |
| 2026-09-28 | Gallery redone in the zen studio | Owner rejected the `464d`/`0c9e` captures (wrong colony, dark); features 23 and 24 now use `nelim-zen-meadow-studio`, a roofed lit display pavilion, maps `wsl-deps.galerie.map` and `galerie-voisins.map` |
| 2026-09-27 | 464d green | Gallery images 1 to 4 all captured (4 of 4) after the region-updater fix; image 5 already green (`0c9e`) |
| 2026-09-27 | 8240 green | No Max Bills alone, 2 of 2: the 15-bill interface limit is lifted even though its Better Workbench Management bridge logs a Harmony error; worth a report to its author |
| 2026-09-27 | 88b3 red | Real defect in the suite's own `GallerySteps.MakeRoom` (region updater re-entrancy); fixed by disabling the updater for the build |
| 2026-09-27 | b2c7 green | Features 04, 05, 10, 25: 13 of 13 on build `BA624725`; first in-game proof of the Choose Your Recipe pending-trap fix |
| 2026-09-27 | 5d66, 9ae9, 92fe | Image 2 failed on a setter that was an assertion; two "infrastructure errors" were filter-syntax mistakes of the session (use bare feature file names, comma separated) |
| 2026-09-26 | French pass done, gallery captures redone | French passes `fr1` to `fr3` (features 01 to 22) green on build `4B89FBA7` |
| 2026-09-25 | RIMMSQOL green; removal chain and BWM details | Features 20, 21, 22 automated and green; manual tests closed by automation |
| 2026-09-25 | Red scenarios all replayed green | `c5dd`, `5a32`, `115b`: every scenario that was red was replayed green on `4B89FBA7` |
| 2026-09-25 | Thanks and CI | Thanks register brought to the current rules, CI path asked; description now one Markdown source in `PUBLICATION.md` |
| 2026-09-25 | First play of the optional-mod scenarios | Features 13 to 19 on the optional mods: 17 of 21 passed, failures were scenario errors plus a Better Workbench Management bridge fix |
| 2026-09-24 | Plural fix, fix check, three defects fixed | `1 recipes` plural keys (`.One`/`.Many`/`.Zero`), countability and Better Workbench Management defects fixed; DLL changed |
| 2026-09-23 | First verdict, run 4 | 65 scenarios, first verdict, found the three shipped defects |
| 2026-09-23 | Workshop item created | `0.1.0` prepublished by the owner: item 3806709456, private, `PublishedFileId.txt` committed (`13ac5ed`); three conditions to reach `tested` set (no `@wip`, every `@requires` run, manual tests green) |
| 2026-09-21 | Prepublication, Pickle runs 1 to 3 | Pickle suite written; first runs had no verdict (two defects in the suite) |
| 2026-09-21 | Workflow audit | Stage `preTest`: Pickle tests were not written. Written the same day |
| 2026-09-13 | Technical settings gate, settings fixes, description link, attribution, translation audit, ordered workflow audit | Settings: 74 checks incl. persistence round-trips; hidden MainButton shortcut; English `ATTRIBUTION.md` (copy in `Mod/`); 54 keys EN = FR |
| 2026-09-12 | Preview recomposition, repository audit, previous status | Preview 896 x 504; standalone repository; `tested_on` 2026-09-01 (base loop confirmed by hand) |
