# Review refusals

ChatGPT suggestions refused for this mod, one line each: key or text, proposal, reason, date. `/review-translation` sends this file with every page and tells the reviewer not to propose these again.

- `BillAutopilot_Settings.label` (French, MainButtonDef): proposed "Bill Autopilot (Pilote automatique des commandes)"; refused, the main-bar button is hidden by default (`buttonVisible` false) so the label is never drawn, and the brand name stays as it is. The long title is in `Settings.Title` and in the button description. 2026-11-13.
- `BillAutopilot.Mode.Inherit` (French): proposed "Déf. {0}"; refused, an abbreviation reads worse than the full word, "Défaut ({0})" fits the button (checked in game, ticket 3609). 2026-11-09.
- `BillAutopilot.Settings.Detected` and `.NotDetected` (French): proposed "Détection de {0}" and "Absence de {0}"; not taken as written, "Détection de X" does not say whether the mod was found; applied "Détecté : {0}" and "Absent : {0}" instead (no agreement with the mod name). 2026-11-14.
