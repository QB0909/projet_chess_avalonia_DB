using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CHESS.Models
{
    public partial class Competition : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string? _nom;

        // Liste des identifiants des parties jouées dans cette compétition
        [ObservableProperty]
        private ObservableCollection<int> _idsParties = new ObservableCollection<int>();
    }
}
