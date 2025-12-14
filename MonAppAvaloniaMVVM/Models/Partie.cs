using CommunityToolkit.Mvvm.ComponentModel;

namespace MonAppAvaloniaMVVM.Models
{
    // Énumération pour les résultats possibles d'une partie
    public enum ResultatPartie
    {
        NonTermine,
        VictoireBlancs,
        VictoireNoirs,
        Nulle
    }

    public partial class Partie : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private int _idJoueurBlancs;

        [ObservableProperty]
        private int _idJoueurNoirs;

        [ObservableProperty]
        private ResultatPartie _resultat;
        
        // On pourrait ajouter plus de détails ici plus tard, comme la liste des coups (en notation PGN par exemple)
    }
}
