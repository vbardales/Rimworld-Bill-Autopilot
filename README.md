# Bill Autopilot

Mod RimWorld 1.6. On dit une fois à quoi sert un établi, et on arrête de réécrire sa liste de
travaux à chaque déblocage.

## Ce que ça fait

Un type d'établi mis sous pilote automatique prend **toutes** les recettes qu'il sait faire. Le mod
pose un travail quand il y a de quoi faire, et le retire une fois le stock rempli : l'onglet montre
donc ce qu'il reste à produire, pas quarante lignes de configuration à faire défiler.

Quand une recherche débloque une nouvelle recette, elle rejoint son établi toute seule, **en travail
suspendu**, avec une lettre qui la nomme. La réactiver l'accepte, la supprimer la refuse — et le
pilote ne la reproposera plus. Rien n'est jamais dépensé à votre insu.

## Les réglages

Tout se règle **par type d'établi** : un établi construit plus tard est déjà configuré. Accessible
depuis les réglages du mod, ou depuis le gizmo « Profil du pilote » sur l'établi sélectionné.

- **Mode par défaut** : *Maintenir un stock de N*, ou *Toujours*.
- **Cible** et **seuil de relance** : le travail apparaît quand on descend au seuil, disparaît quand
  la cible est atteinte. L'écart entre les deux évite qu'il clignote à chaque unité produite.
- **Recettes au produit non comptable** (découpe, fonte, crémation, chirurgie) : le jeu ne sait pas
  les compter, « maintenir un stock » leur est donc impossible. Elles ont leur propre réglage,
  *Jamais* par défaut.
- **Surcharge par recette** : hériter, maintenir un stock différent, toujours, ou jamais.
- **Plafond de travaux automatiques par établi** (8 par défaut) : le jeu n'accepte que 15 travaux
  par établi et masque le bouton « Ajouter » au-delà. Le plafond garde de la place pour les vôtres.

**Régler un travail dans l'onglet, c'est régler le profil.** Changez la cible d'un travail
automatique et le mod l'inscrit comme surcharge de la recette, au lieu de la perdre au prochain
retrait du travail.

## Ce qu'il ne fait pas

- Pas de mode « x1 » comme consigne permanente : un ordre qui dit « en faire un » serait reposé dès
  qu'il se termine, sans fin. *Maintenir un stock de 1* donne l'effet recherché et s'arrête tout seul.
- Rien n'est redessiné dans l'onglet des travaux : les mods qui le remplacent continuent de
  fonctionner. Conçu à côté de Nice Bill Tab, Better Workbench Management, Categorized Bill Dropdown
  et Choose Your Recipe.
- Un travail posé à la main l'emporte toujours : le pilote se retire de cette recette tant qu'il
  existe.

À la **première activation** d'un type d'établi, toutes ses recettes déjà débloquées sont acceptées
en silence — c'est bien ce que « je veux toutes les recettes » veut dire, mais sur un atelier
d'usinage cela lance beaucoup de production d'un coup. Réglez la cible avant d'activer.

## Construire

```bash
dotnet build BillAutopilot/Source/BillAutopilot.csproj -c Release
```

La DLL sort dans `Mod/Assemblies/`. Une jonction NTFS relie
`RimWorld\Mods\BillAutopilot` à `BillAutopilot/Mod` : la compilation suffit, aucune copie.

## Comment ça marche

- `AutoBillSync` — le moteur. Un passage par établi décide, recette par recette, s'il faut poser ou
  retirer un travail.
- `RecipeProbe` — compte le stock d'un produit sans laisser de trace : `RecipeWorkerCounter` exige
  une `Bill_Production` et remonte à la carte par `billStack.billGiver`, on garde donc une bill
  témoin par recette dans une pile détachée.
- `BillAutopilotGameComponent` — état de la partie : quelles bills nous appartiennent (par `loadID`)
  et quelles recettes ont déjà été vues. La configuration, elle, vit dans les réglages du mod.
- `BillAutopilotSettings` — lu dans le constructeur du `Mod`, donc **avant** le chargement des defs :
  aucun `Scribe_Defs` n'y est possible, d'où l'énumération `AutoMode` plutôt qu'un
  `BillRepeatModeDef`.

## Licence

MIT. Voir `LICENSE`.
