using CHESS.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace CHESS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Parties
    public ObservableCollection<Partie> Parties { get; set; } // Toutes les parties
    [ObservableProperty]
    private ObservableCollection<Partie> _filteredParties; // Parties affichées dans la liste

    [ObservableProperty]
    private Partie? _selectedPartie;
    partial void OnSelectedPartieChanged(Partie? value)
    {
        SupprimerPartieCommand.NotifyCanExecuteChanged();
        // Mettre à jour les ComboBoxes des joueurs lorsque la partie sélectionnée change
        SelectedJoueurBlancComboBox = Joueurs.FirstOrDefault(j => j.Id == value?.IdJoueurBlancs);
        SelectedJoueurNoirComboBox = Joueurs.FirstOrDefault(j => j.Id == value?.IdJoueurNoirs);
    }

    // Propriétés pour la sélection des joueurs dans les ComboBoxes de la partie
    [ObservableProperty]
    private Joueur? _selectedJoueurBlancComboBox;
    partial void OnSelectedJoueurBlancComboBoxChanged(Joueur? value)
    {
        if (SelectedPartie != null)
        {
            SelectedPartie.IdJoueurBlancs = value?.Id ?? 0;
        }
    }

    [ObservableProperty]
    private Joueur? _selectedJoueurNoirComboBox;
    partial void OnSelectedJoueurNoirComboBoxChanged(Joueur? value)
    {
        if (SelectedPartie != null)
        {
            SelectedPartie.IdJoueurNoirs = value?.Id ?? 0;
        }
    }

    // Méthode de filtrage des parties
    private void FiltrerParties()
    {
        FilteredParties.Clear();
        if (SelectedCompetitionFilter != null)
        {
            foreach (var partie in Parties.Where(p => p.IdCompetition == SelectedCompetitionFilter.Id))
            {
                FilteredParties.Add(partie);
            }
        }
        else 
        {
            // Note: Pour ce cas, il faudrait probablement avoir une option "Toutes les compétitions" dans le filtre.
            // Pour l'instant, on n'affiche rien si pas de filtre et pas de compétition sélectionnée.
        }
    }

    // Commandes Parties
    [RelayCommand]
    private void NouveauPartie()
    {
        var nouvellePartie = new Partie { 
            Id = GenererProchainIdPartie(), 
            Resultat = ResultatPartie.NonTermine,
            IdCompetition = SelectedCompetitionFilter?.Id ?? 0 // Associe la partie à la compétition filtrée
        };
        Donnees.Parties.Add(nouvellePartie);
        Parties.Add(nouvellePartie);
        FiltrerParties(); // Mettre à jour la liste filtrée
        SelectedPartie = nouvellePartie;
        // Initialiser les ComboBoxes des joueurs pour la nouvelle partie
        SelectedJoueurBlancComboBox = null;
        SelectedJoueurNoirComboBox = null;
    }

    [RelayCommand(CanExecute = nameof(CanSupprimerPartie))]
    private void SupprimerPartie()
    {
        if (SelectedPartie != null)
        {
            Donnees.Parties.Remove(SelectedPartie);
            Parties.Remove(SelectedPartie); // Supprimer de la collection principale
            FiltrerParties(); // Re-appliquer le filtre pour mettre à jour l'affichage
            SelectedPartie = null;
        }
    }
    private bool CanSupprimerPartie() => SelectedPartie != null && SelectedPartie.Id != 0;

    // Générateur d'ID Partie
    private int GenererProchainIdPartie()
    {
        return Donnees.Parties.Any() ? Donnees.Parties.Max(p => p.Id) + 1 : 1;
    }

    // Propriété pour l'énumération des résultats de partie
    public ObservableCollection<ResultatPartie> ResultatsPartiePossibles { get; } = new ObservableCollection<ResultatPartie>(Enum.GetValues<ResultatPartie>());
}
