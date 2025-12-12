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

    public class Partie
    {
        public int Id { get; set; }
        public int IdJoueurBlancs { get; set; }
        public int IdJoueurNoirs { get; set; }
        public ResultatPartie Resultat { get; set; }
        // On pourrait ajouter plus de détails ici plus tard, comme la liste des coups (en notation PGN par exemple)
    }
}
