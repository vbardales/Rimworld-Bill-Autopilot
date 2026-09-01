using System.Collections.Generic;
using Verse;

namespace BillAutopilot
{
    /// <summary>
    /// Ce qu'un autre mod avait pose sur une bill automatique, garde entre le moment ou le pilote la
    /// retire et celui ou il la repose. Range par "DefEtabli/DefRecette", puisque c'est la maille du
    /// profil : deux etablis du meme type se ressemblent, c'est tout l'interet.
    ///
    /// Pas d'attribut Class a l'ecriture : les champs sont primitifs et le ThingFilter est un type du
    /// jeu, deep-save dans un champ de son propre type. Retirer le mod laisse donc des noeuds que
    /// personne ne lit, sans erreur - voir BillAutopilotState.
    /// </summary>
    public class BillMemory : IExposable
    {
        /// <summary>Better Workbench Management : compter aussi hors de la carte d'origine.</summary>
        public bool countAway;

        /// <summary>Nom personnalise, qu'il vienne de BWM ou du renommage vanilla.</summary>
        public string name;

        /// <summary>Better Workbench Management : produits additionnels a compter dans la cible.</summary>
        public ThingFilter productFilter;

        /// <summary>loadID des bills avec lesquelles celle-ci etait liee.</summary>
        public List<int> linkedTo = new List<int>();

        public void ExposeData()
        {
            Scribe_Values.Look(ref countAway, "countAway", defaultValue: false);
            Scribe_Values.Look(ref name, "name");
            Scribe_Deep.Look(ref productFilter, "productFilter");
            Scribe_Collections.Look(ref linkedTo, "linkedTo", LookMode.Value);

            if (linkedTo == null) linkedTo = new List<int>();
        }
    }
}
