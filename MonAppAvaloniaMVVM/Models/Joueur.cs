using CommunityToolkit.Mvvm.ComponentModel;

namespace MonAppAvaloniaMVVM.Models
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
    }
}
