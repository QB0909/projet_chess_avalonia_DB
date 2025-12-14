using MonAppAvaloniaMVVM.Models;
using MonAppAvaloniaMVVM.Services;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace MonAppAvaloniaMVVM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly JsonDataService _dataService;
    
    // Propriété pour contenir toutes les données de l'application
    public Donnees Donnees { get; set; }

    // Joueurs
    public ObservableCollection<Joueur> Joueurs { get; set; }
    [ObservableProperty]
    private Joueur? _selectedJoueur;
    partial void OnSelectedJoueurChanged(Joueur? value)
    {
        SupprimerJoueurCommand.NotifyCanExecuteChanged();
    }

    // Compétitions
    public ObservableCollection<Competition> Competitions { get; set; }
    [ObservableProperty]
    private Competition? _selectedCompetition;
    partial void OnSelectedCompetitionChanged(Competition? value)
    {
        SupprimerCompetitionCommand.NotifyCanExecuteChanged();
    }

    // Parties
    public ObservableCollection<Partie> Parties { get; set; }
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

    public MainWindowViewModel()
    {
        _dataService = new JsonDataService("federation.json");
        Donnees = _dataService.ChargerDonnees();

        // Initialisation des joueurs
        if (!Donnees.Joueurs.Any())
        {
            Donnees.Joueurs.Add(new Joueur { Id = GenererProchainIdJoueur(), Nom = "Kasparov", Prenom = "Garry", ClassementElo = 2800 });
            Donnees.Joueurs.Add(new Joueur { Id = GenererProchainIdJoueur(), Nom = "Carlsen", Prenom = "Magnus", ClassementElo = 2830 });
        }
        Joueurs = new ObservableCollection<Joueur>(Donnees.Joueurs);

        // Initialisation des compétitions
        if (!Donnees.Competitions.Any())
        {
            Donnees.Competitions.Add(new Competition { Id = GenererProchainIdCompetition(), Nom = "Championnat de Paris 2025" });
            Donnees.Competitions.Add(new Competition { Id = GenererProchainIdCompetition(), Nom = "Open de France" });
        }
        Competitions = new ObservableCollection<Competition>(Donnees.Competitions);

        // Initialisation des parties
        if (!Donnees.Parties.Any() && Donnees.Joueurs.Count >= 2) // Seulement si au moins deux joueurs existent
        {
            Donnees.Parties.Add(new Partie { 
                Id = GenererProchainIdPartie(), 
                IdJoueurBlancs = Donnees.Joueurs[0].Id, 
                IdJoueurNoirs = Donnees.Joueurs[1].Id, 
                Resultat = ResultatPartie.VictoireBlancs 
            });
        }
        Parties = new ObservableCollection<Partie>(Donnees.Parties);
        
        // Sauvegarde immédiate des données d'exemple si elles viennent d'être créées
        if (!Donnees.Joueurs.Any() || !Donnees.Competitions.Any() || !Donnees.Parties.Any())
        {
            _dataService.SauvegarderDonnees(Donnees);
        }

        // Initialiser les sélections des ComboBoxes des joueurs de partie après le chargement des parties et des joueurs
        if (SelectedPartie != null)
        {
            SelectedJoueurBlancComboBox = Joueurs.FirstOrDefault(j => j.Id == SelectedPartie.IdJoueurBlancs);
            SelectedJoueurNoirComboBox = Joueurs.FirstOrDefault(j => j.Id == SelectedPartie.IdJoueurNoirs);
        }
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
    private bool CanSupprimerJoueur() => SelectedJoueur != null && SelectedJoueur.Id != 0;


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
            Donnees.Competitions.Remove(SelectedCompetition);
            Competitions.Remove(SelectedCompetition);
            SelectedCompetition = null;
        }
    }
    private bool CanSupprimerCompetition() => SelectedCompetition != null && SelectedCompetition.Id != 0;

    // Commandes Parties
    [RelayCommand]
    private void NouveauPartie()
    {
        var nouvellePartie = new Partie { Id = GenererProchainIdPartie(), Resultat = ResultatPartie.NonTermine };
        Donnees.Parties.Add(nouvellePartie);
        Parties.Add(nouvellePartie);
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
            Parties.Remove(SelectedPartie);
            SelectedPartie = null;
        }
    }
    private bool CanSupprimerPartie() => SelectedPartie != null && SelectedPartie.Id != 0;


    // Commande globale de sauvegarde
    [RelayCommand]
    private void Sauvegarder()
    {
        _dataService.SauvegarderDonnees(Donnees);
        Console.WriteLine("Données sauvegardées !");
    }

    // Générateurs d'ID
    private int GenererProchainIdJoueur()
    {
        return Donnees.Joueurs.Any() ? Donnees.Joueurs.Max(j => j.Id) + 1 : 1;
    }

    private int GenererProchainIdCompetition()
    {
        return Donnees.Competitions.Any() ? Donnees.Competitions.Max(c => c.Id) + 1 : 1;
    }

    private int GenererProchainIdPartie()
    {
        return Donnees.Parties.Any() ? Donnees.Parties.Max(p => p.Id) + 1 : 1;
    }

    // Propriété pour l'énumération des résultats de partie
    public ObservableCollection<ResultatPartie> ResultatsPartiePossibles { get; } = new ObservableCollection<ResultatPartie>(Enum.GetValues<ResultatPartie>());
}