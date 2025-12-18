using CHESS.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace CHESS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Joueurs
    public ObservableCollection<Joueur> Joueurs { get; set; }
    [ObservableProperty]
    private Joueur? _selectedJoueur;
    partial void OnSelectedJoueurChanged(Joueur? value)
    {
        SupprimerJoueurCommand.NotifyCanExecuteChanged();
    }

    // Commandes Joueurs
    [RelayCommand]
    private void NouveauJoueur()
    {
        var nouveauJoueur = new Joueur { Id = GenererProchainIdJoueur(), ClassementElo = 1200 }; // ELO de base
        Donnees.Joueurs.Add(nouveauJoueur);
        Joueurs.Add(nouveauJoueur);
        SelectedJoueur = nouveauJoueur; // Sélectionne le nouveau joueur pour édition
    }

    [RelayCommand(CanExecute = nameof(CanSupprimerJoueur))]
    private void SupprimerJoueur()
    {
        if (SelectedJoueur != null)
        {
            Donnees.Joueurs.Remove(SelectedJoueur);
            Joueurs.Remove(SelectedJoueur);
            SelectedJoueur = null;
        }
    }
    private bool CanSupprimerJoueur() => SelectedJoueur != null && SelectedJoueur.Id != 0; // on peut supprimer que si joueur sélectionné

    // Générateur d'ID Joueur
    private int GenererProchainIdJoueur()
    {
        return Donnees.Joueurs.Any() ? Donnees.Joueurs.Max(j => j.Id) + 1 : 1;
    }
}
