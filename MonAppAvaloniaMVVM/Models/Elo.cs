using CommunityToolkit.Mvvm.ComponentModel;

namespace CHESS.Models
{
    // Historique ELO: stocke l'ancien et le nouvel ELO pour un joueur après une partie
    public class EloRecord
    {
        public int PartieId { get; set; }
        public int Elo { get; set; }
    }

    public partial class Elo : ObservableObject
    {
        // Ici Id sera l'Id du joueur (JoueurId)
        [ObservableProperty]
        private int _id;

        // Liste de tous les ELOs du joueur, chacun lié à une partie
        [ObservableProperty]
        private System.Collections.Generic.List<EloRecord> _records = new System.Collections.Generic.List<EloRecord>();
    }
}
