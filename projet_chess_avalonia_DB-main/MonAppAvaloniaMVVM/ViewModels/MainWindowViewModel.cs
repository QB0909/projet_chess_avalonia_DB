using CHESS.Models;
using CHESS.Services;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;


namespace CHESS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly JsonDataService _dataService;
    
    // Propriété pour contenir toutes les données de l'application
    public Donnees Donnees { get; set; }

    
    // Ici afficher ses statistiques du joueurSelectionne
    [ObservableProperty]
    private Joueur? joueurSelectionne;

    
    public ObservableCollection<int> ListeEloJoueurSelectionne => JoueurSelectionne?.ListeElo != null
        ? new ObservableCollection<int>(JoueurSelectionne.ListeElo)
        : new ObservableCollection<int>();

    public string EloMaxJoueurSelectionne => JoueurSelectionne?.ListeElo?.Any() == true
        ? JoueurSelectionne.ListeElo.Max().ToString()
        : "RIEN";

    public string EloMinJoueurSelectionne => JoueurSelectionne?.ListeElo?.Any() == true
        ? JoueurSelectionne.ListeElo.Min().ToString()
        : "RIEN";

    public string EloMoyenJoueurSelectionne => JoueurSelectionne?.ListeElo?.Any() == true
        ? JoueurSelectionne.ListeElo.Average().ToString("F0") // "F0" pour aucun chiffre après la virgule
        : "RIEN";

    
    partial void OnJoueurSelectionneChanged(Joueur? value)
    {
        OnPropertyChanged(nameof(ListeEloJoueurSelectionne));
        OnPropertyChanged(nameof(EloMaxJoueurSelectionne));
        OnPropertyChanged(nameof(EloMinJoueurSelectionne));
        OnPropertyChanged(nameof(EloMoyenJoueurSelectionne));
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
        if (!Donnees.Parties.Any() && Donnees.Joueurs.Count >= 2 && Donnees.Competitions.Any()) // Seulement si au moins deux joueurs et une compétition existent
        {
            Donnees.Parties.Add(new Partie { 
                Id = GenererProchainIdPartie(), 
                IdJoueurBlancs = Donnees.Joueurs[0].Id, 
                IdJoueurNoirs = Donnees.Joueurs[1].Id, 
                Resultat = ResultatPartie.VictoireBlancs,
                IdCompetition = Donnees.Competitions.First().Id // Associer à la première compétition
            });
        }
        Parties = new ObservableCollection<Partie>(Donnees.Parties);

        // Initialisation de la collection filtrée des parties et du filtre de compétition
        _filteredParties = new ObservableCollection<Partie>(); // Initialisé vide, puis filtré
        if (Competitions.Any())
        {
            SelectedCompetitionFilter = Competitions.First(); // Sélectionne la première compétition par défaut pour le filtre
        }
        FiltrerParties(); // Applique le filtre initial

        // Sauvegarde immédiate des données d'exemple si elles viennent d'être créées
        if (!Donnees.Joueurs.Any() || !Donnees.Competitions.Any() || !Donnees.Parties.Any())
        {
            _dataService.SauvegarderDonnees(Donnees);
        }

        // Initialiser les sélections des ComboBoxes des joueurs de partie après le chargement des parties et des joueurs
        // Si SelectedPartie est null (pas de parties ou première exécution) on ne fait rien.
        // Sinon, on initialise les ComboBoxes pour la partie déjà sélectionnée.
        
        if (SelectedPartie != null) 
        {
            SelectedJoueurBlancComboBox = Joueurs.FirstOrDefault(j => j.Id == SelectedPartie.IdJoueurBlancs);
            SelectedJoueurNoirComboBox = Joueurs.FirstOrDefault(j => j.Id == SelectedPartie.IdJoueurNoirs);
        }
    }

    // Commande globale de sauvegarde
    [RelayCommand]
    private void Sauvegarder()
    {
        _dataService.SauvegarderDonnees(Donnees);
        Console.WriteLine("Données sauvegardées !");
    }
}
