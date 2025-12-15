using CHESS.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace CHESS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Compétitions (pour la gestion principale des compétitions)
    public ObservableCollection<Competition> Competitions { get; set; }
    [ObservableProperty]
    private Competition? _selectedCompetition;
    partial void OnSelectedCompetitionChanged(Competition? value)
    {
        SupprimerCompetitionCommand.NotifyCanExecuteChanged();
    }

    // Compétitions (pour le filtre dans l'onglet Parties)
    [ObservableProperty]
    private Competition? _selectedCompetitionFilter;
    partial void OnSelectedCompetitionFilterChanged(Competition? value)
    {
        FiltrerParties();
    }

    // Commandes Compétitions
    [RelayCommand]
    private void NouveauCompetition()
    {
        var nouvelleCompetition = new Competition { Id = GenererProchainIdCompetition(), Nom = "Nouvelle Compétition" };
        Donnees.Competitions.Add(nouvelleCompetition);
        Competitions.Add(nouvelleCompetition);
        SelectedCompetition = nouvelleCompetition;
    }

    [RelayCommand(CanExecute = nameof(CanSupprimerCompetition))]
    private void SupprimerCompetition()
    {
        if (SelectedCompetition != null)
        {
            // Supprimer également les parties associées à cette compétition
            var partiesASupprimer = Parties.Where(p => p.IdCompetition == SelectedCompetition.Id).ToList();
            foreach (var partie in partiesASupprimer)
            {
                Donnees.Parties.Remove(partie);
                Parties.Remove(partie); // Supprimer de la collection principale
            }
            FiltrerParties(); // Re-appliquer le filtre pour mettre à jour l'affichage

            Donnees.Competitions.Remove(SelectedCompetition);
            Competitions.Remove(SelectedCompetition);
            SelectedCompetition = null;
        }
    }
    private bool CanSupprimerCompetition() => SelectedCompetition != null && SelectedCompetition.Id != 0;

    // Générateur d'ID Compétition
    private int GenererProchainIdCompetition()
    {
        return Donnees.Competitions.Any() ? Donnees.Competitions.Max(c => c.Id) + 1 : 1;
    }
}
