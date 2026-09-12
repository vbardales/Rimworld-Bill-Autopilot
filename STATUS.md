---
mod:          Bill Autopilot
packageId:    nelim.billautopilot
repo:         Rimworld-Bill-Autopilot
visibility:   public
detached:     yes
stage:        done
licence:      original
licence_at:   original work
licence_name: MIT
licence_file: LICENSE (identical copy in Mod/LICENSE)
dependencies: declared
showcase:     complete
tested_on:    2026-09-01
workshop:
remaining:
  - unverified: the mark in the bill label, in the tabs other mods redraw
  - unverified: a change made in the bills tab being kept as a profile override
  - unverified: the memory held per workbench, two benches of a kind set differently
  - unverified: a repeat mode from another mod, kept and asked rather than guessed at
  - unverified: all five compatibility bridges, Better Workbench Management the largest
  - unverified: Nice Bill Tab's cached list, where a drag could bring a deleted bill back
  - unverified: loading a save after removing the mod, the reason its state avoids a GameComponent
session:      local_3527e6d8-def4-4e54-97ff-a6430f1dc569
updated:      2026-09-12, mod session
---

# Bill Autopilot — status

## Preview recomposition — 2026-09-12

- Revised overlay brief checked and composition rendered again: `Bill` and `Autopilot`
  are both identity-bearing words, so both remain at 100% (46 px, weight 600).
  The exact name has no prefix, suffix or linking word requiring a 65% span.
  Orange accent from the lit subject contrasts by hue and saturation with the dominant
  cool blue-slate family and its light-blue secondary ink; it is not a brighter version
  of that secondary ink. Full-size and 268-pixel previews were visually rechecked.
  Existing illustration, title, summary and palette are retained for this revised brief;
  no further source replacement was needed. The font and contrast checks below passed again.

- Final asset: `Mod/About/Preview.png`, 896 x 504, 557,964 bytes (below 900 KB).
- New text-free illustration: `Art/Preview.png`. The available `Art/Preview-source.png`
  already contained engraved words and a bill interface, so it could not serve as a clean
  background. It remains untouched and was archived before generation as
  `Art/Preview-previous-engraved.png`. The replacement was visually inspected before composition.
- Composition: `Art/preview.html`; parameters and preserved title/summary:
  `Art/preview-layout.json`; sole colour reference: `Art/preview-palette.json`.
  Reproduce using `Art/render-preview.cjs` with Node.js, Playwright, Sharp and Chrome
  (`NODE_PATH` may point to the bundled Node modules). The script serves the files locally,
  waits for `document.fonts.ready` and image decoding, then captures at native size.
- Palette rationale: the broad blue-slate floor supplies the veil and the blue family of
  the lightened secondary ink. The characteristic orange task-lamp light and worker's clothing
  supply the saturated accent for the rule and badge. Title and summary share the same ivory ink.
  Secondary ink is saved for consistency but unused: this original public mod has no status tag.
- Fonts verified through Chrome's actual platform-font report: Segoe UI Semibold for the
  title, Segoe UI regular for the summary, Segoe UI Bold for the version. No fallback font.
- Version `1.6` checked against the highest stable `supportedVersions` entry in delivered
  `Mod/About/About.xml`; triangle and rotated digits use the guide's coordinates.
- QA: `Art/preview-qa.json` records actual fonts, geometry and contrast. The text-free rendered
  background is `Art/preview-background.png`; thumbnail is `Art/preview-268.png`.
  Minimum contrast over every pixel in each text rectangle (including all four corners):
  title 11.70:1, summary 8.74:1; badge digits against the opaque accent 8.82:1.
  Tag contrast is not applicable because no tag is displayed. Visual inspection at both
  896 x 504 and 268 pixels wide found no overlap or clipping; title/version identifiable,
  rule visible. The summary is intended for the full-size view, as specified by the guide.
- No publication or push performed for this preview revision.

Marked `done` at the owner's request on 2026-09-12 after the repository audit.
The unverified in-game scenarios remain recorded above; this status change does not
claim a new manual test pass or a Workshop release. `licence` remains `original`.

## Repository audit — 2026-09-12

- **Title:** keep `Bill Autopilot`. The repository records an original, unpublished mod;
  no continuation or port suffix is justified by the available provenance. Version 1.6 is
  already declared in `supportedVersions`.
- **Licence:** MIT, copyright (c) 2026 nelim. `LICENSE` and `Mod/LICENSE` are byte-identical.
  The `original` field above describes provenance; the actual licence is MIT.
- **Manual tests:** 19 functional scenarios in `TESTING.md`, including the added hidden-recipe
  integration scenario. Their presence is verified; no in-game test was run in this audit.
  The historical `tested_on` date remains unchanged. The owner subsequently requested `done`.
- **Automated tests:** both Release builds succeeded with zero warnings and errors;
  all 44 decision-layer checks passed against the shipped DLL and installed game assemblies.
  This does not validate runtime patches, save round-trips, or optional integrations.
- **XML:** `Tests/ValidateXml.ps1` passes 376 checks across the three shipped XML files:
  parsing, metadata/dependency contract, GitHub description link, duplicate/empty language keys,
  English/French key and placeholder parity, and literal translation keys referenced in C#.
  There are no shipped Defs or XML patches. Save serialization remains a manual scenario (12).
- **Description:** added the repository link directly in `About.xml`'s description; it was
  previously present only in the separate `url` field.

## Previous status and in-game evidence

Status sheet, read by a pass over every mod rather than by asking each session in turn.
It lives at the root, never in `Mod/`, so Steam never receives it.

The fields derived from disk on 2026-09-12 were checked one by one and hold. The four the sweep
cannot fill are settled here.

- **Previous `stage`** — `preTest` before the owner's completion request. Nothing has ever been published: no Workshop item, and
  `CHANGELOG.md` still marks 1.0.0 unreleased. The code and the showcase are finished, which is
  what separates this from `port` or `showcase`; what it waits on is a test pass, which is what
  separates it from `tested`.
- **`dependencies`** — `declared`, but only after checking, because the About lists **six**
  non-vanilla `loadAfter` entries that are not in `modDependencies`: Better Workbench Management,
  Nice Bill Tab, its Expansion, Choose Your Recipe, Dubs Mint Menus and Everybody Gets One. None
  of them is a dependency. Every one is reached by reflection, the only assemblies referenced at
  build time are the game's and Harmony's, and each accessor returns a neutral value when its mod
  is absent. `loadAfter` is there because we hook onto them when they happen to be present, and
  scenario 17 of `TESTING.md` exists to prove the mod runs with all six gone. An undeclared
  dependency is not cosmetic — on 2026-09-11 Reequilibrage animaux took 47 vanilla animals down
  with it, Muffalo included, because the class it injects belongs to a mod that was not declared
  and not loaded — which is why this one is written out rather than waved through.
- **`tested_on`** — 2026-09-01, when the base loop was confirmed by hand in a real save: bills put
  up and taken down on their own, a recipe unlocked by research arriving suspended, and deleting an
  automatic bill excluding its recipe. The line the sweep puts there by default, "never seen
  running", was wrong. But read the date against the seven lines above it: the build that ran that
  day is not this one, and everything written since has only ever been compiled.
- **`remaining`** — seven scenarios of `TESTING.md` that have never run. No known defect left
  unfixed and no feature missing from the first pass: what remains is unverified, not broken. The
  compatibility bridges come high because they fail silently by construction — a lost feature, never
  a crash — so nothing but a run will tell.

`TESTING.md` stays the source: it says for each scenario what it proves and what its failure looks
like. This sheet keeps only the balance.

One layer no longer waits on a run at all. `Tests/` holds a program that exercises the shipped
assembly against the real game assemblies with no game running, 44 checks over the decision layer,
non-zero exit on failure. It does not shorten the list above, because what it proves is arithmetic
and what the list wants is wiring.

`licence` vocabulary: `open` an explicit licence, `silent` no licence and a dead source,
`alive` no licence but a living source, `forbidden` a written refusal, `original` owing nothing
to anyone — not a name, not an idea traceable to one mod, not a value derived from its assets.
