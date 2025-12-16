using System.Collections.Generic;

namespace CHESS.Models
{
    public class Donnees
    {
        public List<Joueur> Joueurs { get; set; } = new List<Joueur>();
        public List<Competition> Competitions { get; set; } = new List<Competition>();
        public List<Partie> Parties { get; set; } = new List<Partie>();
        public List<Elo> EloHistorique { get; set; } = new List<Elo>();
    }
}
