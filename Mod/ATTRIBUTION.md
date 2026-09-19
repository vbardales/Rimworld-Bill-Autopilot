# Attribution

## Bill Autopilot

Bill Autopilot is an original mod maintained by Nelim. The project's own code and
documentation are distributed under the MIT notice in `LICENSE`, copyright
(c) 2026 nelim. A copy of that notice accompanies the distributed mod.

The recorded provenance is original development, not a continuation or a port of
any of the mods below. The compatibility code implements adapters to their
interfaces; studying those interfaces does not make their implementations part
of this project's MIT grant. The public repository and the `original` status
describe this project's work, not ownership of RimWorld or its companion mods.

## Interfaces studied for compatibility

The following credits identify the projects by their names and package IDs from
the repository's README and About.xml. The listed mechanisms are the basis of
the compatibility work documented in the README and `Source/Compat/`.

| Project / package ID | Studied behavior and local integration |
| --- | --- |
| Better Workbench Management (`falconne.BWM`) | Extended bill data, linked bills, workbench restrictions, product counting and bill ceilings. `BetterWorkbenchesCompat` accesses its `ImprovedWorkbenches` interfaces by reflection. |
| Dubs Mint Menus (`dubwise.dubsmintmenus`) | Bench-template capture and application. `DubsMintMenusCompat` removes transient autopilot bills from captured templates. |
| Nice Bill Tab (`Andromeda.NiceBillTab`) | Cached bill-list refresh and drag behavior. `NiceBillTabCompat` signals that the list must be refreshed after automatic changes. |
| Nice Bill Tab - Expansion (`HICON.NiceBillTabExpansion`) | Hidden-recipe lookup. `HiddenRecipesCompat` consults the optional hidden-recipe store. |
| Everybody Gets One (`Memegoddess.EverybodyGetsOne`) | Additional repeat-mode definitions and their interpretation of bill counts. Bill Autopilot preserves the selected mode and delegates work eligibility to the owning mod. |
| Choose Your Recipe (`zal.chooseyourrecipe`) | Filtering of a workbench's available recipes. Bill Autopilot uses the resulting recipe list without bundling this mod. |

Credit for each companion mod belongs to its respective authors and contributors.
These are optional integrations, not required dependencies. Their names, package
IDs and reflected member names identify the interfaces with which this mod works.

**Study versus reuse:** the reviewed repository records interface study and
original adapter code. No copied third-party implementation, artwork or bundled
companion-mod assembly was identified in the sources or distributed files.
This statement records the available evidence; it is not a claim to have audited
the complete development history. No permission to redistribute companion-mod
code or assets is asserted, and their licences are not replaced by this project's
MIT notice. Any future copied material must be separately identified with its
actual licence or permission and required notices before distribution.

## Runtime and build tools

- RimWorld and its game interfaces are provided by Ludeon Studios. Game assemblies
  are used as references and for local tests; they are not included in `Mod/`.
- Harmony, by Andreas Pardeike and contributors, provides runtime patching. It is
  declared as the required external mod (`brrainz.harmony`); its assembly is not
  bundled here. The build references `Lib.Harmony` with runtime assets excluded.
- `Krafs.Rimworld.Ref` and `Krafs.Publicizer` provide build-time reference and
  accessibility tooling. Their packages and reference assemblies are not shipped.
- The changelog format acknowledges Keep a Changelog in `CHANGELOG.md`.

## AI assistance and artwork

The About description records code written with Claude Code (Anthropic), under
human direction, review and testing. The project records AI-generated showcase
artwork and retains source images and composition files in `Art/`. The preview
overlay uses Segoe UI through the rendering system; no font file is distributed.

These tool credits do not imply endorsement or transfer any third-party rights.
The repository records no imported companion-mod artwork. The artwork provenance
is recorded here separately from the MIT software notice; this document does not
invent a licence for third-party material.

## Distribution

This document is also included as `Mod/ATTRIBUTION.md`. Keep both copies identical
when updating credits. Keep `LICENSE` and `Mod/LICENSE` identical as well.
