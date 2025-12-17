using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace CHESS.Models
{
    public partial class Joueur : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string? _nom;

        [ObservableProperty]
        private string? _prenom;

        [ObservableProperty]
        private int _classementElo;

        [ObservableProperty]
        private List<int> listeElo = new List<int>();
    }
}
