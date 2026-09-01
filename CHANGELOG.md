# Changelog

## 0.1.0 — non publié

Première version.

- Pilote automatique par type d'établi : le mod pose un travail quand il y a de quoi faire et le
  retire une fois la cible atteinte.
- Modes *Maintenir un stock* et *Toujours*, avec cible et seuil de relance.
- Réglage séparé pour les recettes au produit non comptable (découpe, fonte, crémation, chirurgie),
  que le jeu ne sait pas compter.
- Surcharge par recette, écrite aussi quand on ajuste un travail automatique dans l'onglet.
- Supprimer un travail automatique exclut sa recette du pilote.
- Recette débloquée après coup : travail suspendu + lettre récapitulative.
- Plafond de travaux automatiques par établi, pour laisser de la place sous la limite de 15 du jeu.
- Interface en anglais et en français, entièrement au pointeur (Steam Deck).
- Rien n'est écrit dans la sauvegarde sous forme de classe du mod : le retirer d'une partie en cours
  ne produit aucune erreur au chargement.
- Better Workbench Management : ce qu'il pose sur un travail survit au cycle retirer/reposer — nom,
  comptage hors-carte, produits additionnels, appartenance à un groupe de travaux liés. Sa
  restriction d'établi s'applique aux travaux du pilote, ses règles de comptage servent aux seuils,
  et son plafond de travaux remplace le 15 du jeu.
- Nice Bill Tab - Expansion : une recette masquée sur un établi est tenue pour exclue.
- Choose Your Recipe : déjà respecté sans rien faire, il retire les recettes désactivées de l'établi.
