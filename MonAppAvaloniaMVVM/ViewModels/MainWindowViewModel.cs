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

    // Collection observable pour la liste des joueurs, qui sera liée à l'interface utilisateur
    public ObservableCollection<Joueur> Joueurs { get; set; }

    // Propriété pour le joueur actuellement sélectionné dans la liste
    [ObservableProperty]
    private Joueur? _selectedJoueur;

    // Cette méthode est appelée automatiquement par le MVVM Toolkit lorsque la propriété SelectedJoueur change.
    partial void OnSelectedJoueurChanged(Joueur? value)
    {
        // On notifie à la commande que son état d'exécution a peut-être changé.
        SupprimerJoueurCommand.NotifyCanExecuteChanged();
    }

    public MainWindowViewModel()
    {
        _dataService = new JsonDataService("federation.json");
        Donnees = _dataService.ChargerDonnees();

        // Si aucune donnée n'est chargée (premier lancement), on ajoute des exemples.
        if (!Donnees.Joueurs.Any())
        {
            Donnees.Joueurs.Add(new Joueur { Id = GenererProchainIdJoueur(), Nom = "Kasparov", Prenom = "Garry", ClassementElo = 2800 });
            Donnees.Joueurs.Add(new Joueur { Id = GenererProchainIdJoueur(), Nom = "Carlsen", Prenom = "Magnus", ClassementElo = 2830 });
            
            // Sauvegarde immédiate des données d'exemple
            _dataService.SauvegarderDonnees(Donnees);
        }

        // Initialise la collection observable avec les données chargées/créées
        Joueurs = new ObservableCollection<Joueur>(Donnees.Joueurs);
    }

    // Commande pour ajouter un nouveau joueur
    [RelayCommand]
    private void NouveauJoueur()
    {
        var nouveauJoueur = new Joueur { Id = GenererProchainIdJoueur(), ClassementElo = 1200 }; // ELO de base
        Donnees.Joueurs.Add(nouveauJoueur);
        Joueurs.Add(nouveauJoueur);
        SelectedJoueur = nouveauJoueur; // Sélectionne le nouveau joueur pour édition
    }

    // Commande pour supprimer le joueur sélectionné
    [RelayCommand(CanExecute = nameof(CanSupprimerJoueur))]
    private void SupprimerJoueur()
    {
        if (SelectedJoueur != null)
        {
            Donnees.Joueurs.Remove(SelectedJoueur);
            Joueurs.Remove(SelectedJoueur);
            SelectedJoueur = null; // Désélectionne le joueur
        }
    }

    // Détermine si la commande SupprimerJoueur peut être exécutée
    private bool CanSupprimerJoueur() => SelectedJoueur != null && SelectedJoueur.Id != 0;

    // Commande pour sauvegarder toutes les données
    [RelayCommand]
    private void Sauvegarder()
    {
        _dataService.SauvegarderDonnees(Donnees);
        // Vous pouvez ajouter une notification à l'utilisateur ici (ex: "Données sauvegardées !")
        Console.WriteLine("Données sauvegardées !"); // Pour le débogage
    }

    // Génère un ID unique pour un nouveau joueur
    private int GenererProchainIdJoueur()
    {
        return Donnees.Joueurs.Any() ? Donnees.Joueurs.Max(j => j.Id) + 1 : 1;
    }
}
