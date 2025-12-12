using System.Collections.Generic;

namespace MonAppAvaloniaMVVM.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        // Liste des identifiants des parties jouées dans cette compétition
        public List<int> IdsParties { get; set; } = new List<int>();
    }
}
