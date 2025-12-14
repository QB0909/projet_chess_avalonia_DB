using MonAppAvaloniaMVVM.Models;
using MonAppAvaloniaMVVM.Services;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic; 

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
        // Cela doit être fait après l'initialisation de Joueurs.
        if (SelectedPartie != null) 
        {
            SelectedJoueurBlancComboBox = Joueurs.FirstOrDefault(j => j.Id == SelectedPartie.IdJoueurBlancs);
            SelectedJoueurNoirComboBox = Joueurs.FirstOrDefault(j => j.Id == SelectedPartie.IdJoueurNoirs);
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
        else // Si aucun filtre de compétition n'est sélectionné, afficher toutes les parties
        {
            // Note: Pour ce cas, il faudrait probablement avoir une option "Toutes les compétitions" dans le filtre.
            // Pour l'instant, on n'affiche rien si pas de filtre et pas de compétition sélectionnée.
            // Une amélioration serait de permettre une sélection "Aucune / Toutes" dans SelectedCompetitionFilter.
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

    // Commande pour calculer les classements ELO
    [RelayCommand]
    private void CalculerElo()
    {
        // K-factor standard (peut être ajusté ou devenir dynamique)
        const int K_FACTOR = 32;

        // Réinitialiser les ELO des joueurs aux valeurs de base (ou à l'état sauvegardé) avant de recalculer toutes les parties.
        // Cela est important pour que le calcul soit reproductible et non cumulatif si on le lance plusieurs fois.
        foreach (var joueur in Donnees.Joueurs)
        {
            // Pour un exemple, on peut réinitialiser l'ELO à 1200 ou la valeur d'origine si on la stockait.
            // Pour l'instant, nous prendrons la valeur de base si elle n'a pas encore été établie.
            if (joueur.ClassementElo == 0) joueur.ClassementElo = 1200; // Assurer un ELO de base si 0
        }

        // Créer une copie des ELO actuels pour le calcul itératif
        var eloActuel = Donnees.Joueurs.ToDictionary(j => j.Id, j => j.ClassementElo);
        
        // Pour chaque partie, calculer les nouveaux ELO
        foreach (var partie in Donnees.Parties.OrderBy(p => p.Id)) // Ordre des parties important pour l'ELO
        {
            if (partie.Resultat == ResultatPartie.NonTermine)
                continue; // Ne pas calculer pour les parties non terminées

            int Ra = eloActuel.GetValueOrDefault(partie.IdJoueurBlancs);
            int Rb = eloActuel.GetValueOrDefault(partie.IdJoueurNoirs);
            
            // Si un joueur n'existe plus ou n'a pas d'ELO (devrait être initialisé), on passe
            if (Ra == 0 || Rb == 0)
            {
                Console.WriteLine($"Skipping ELO for game {partie.Id}: one or both players not found or have 0 ELO.");
                continue;
            }

            // Calcul des probabilités de gain
            double Ea = 1.0 / (1.0 + Math.Pow(10.0, (Rb - Ra) / 400.0));
            double Eb = 1.0 / (1.0 + Math.Pow(10.0, (Ra - Rb) / 400.0));

            // Scores réels
            double Sa = 0.0;
            double Sb = 0.0;
            if (partie.Resultat == ResultatPartie.VictoireBlancs)
            {
                Sa = 1.0;
                Sb = 0.0;
            }
            else if (partie.Resultat == ResultatPartie.VictoireNoirs)
            {
                Sa = 0.0;
                Sb = 1.0;
            }
            else // Nulle
            {
                Sa = 0.5;
                Sb = 0.5;
            }

            // Mise à jour des ELO dans la map temporaire
            eloActuel[partie.IdJoueurBlancs] = (int)Math.Round(Ra + K_FACTOR * (Sa - Ea));
            eloActuel[partie.IdJoueurNoirs] = (int)Math.Round(Rb + K_FACTOR * (Sb - Eb));
        }

        // Appliquer les nouveaux ELO calculés aux objets Joueur originaux
        foreach (var joueur in Donnees.Joueurs)
        {
            if (eloActuel.ContainsKey(joueur.Id))
            {
                joueur.ClassementElo = eloActuel[joueur.Id];
            }
        }
        
        // Assurer que la UI des joueurs se met à jour
        // En réassignant la collection ou en notifiant chaque joueur
        // Puisque Joueur est ObservableObject, la modification directe de ClassementElo devrait suffire.

        Console.WriteLine("Classements ELO recalculés !");
    }


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