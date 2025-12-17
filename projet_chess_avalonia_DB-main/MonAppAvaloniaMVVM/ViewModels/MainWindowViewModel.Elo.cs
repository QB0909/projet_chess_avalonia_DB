using CHESS.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;

namespace CHESS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Commande pour calculer les classements ELO
    [RelayCommand]
    private void CalculerElo()
    {
        // K-factor standard (peut être ajusté ou devenir dynamique)
        const int K_facteur = 32;

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

            eloActuel.TryGetValue(partie.IdJoueurBlancs, out int Ra);
            eloActuel.TryGetValue(partie.IdJoueurNoirs, out int Rb);
            
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
            eloActuel[partie.IdJoueurBlancs] = (int)Math.Round(Ra + K_facteur * (Sa - Ea));
            eloActuel[partie.IdJoueurNoirs] = (int)Math.Round(Rb + K_facteur * (Sb - Eb));
        }

        // Appliquer les nouveaux ELO calculés aux objets Joueur originaux
        foreach (var joueur in Donnees.Joueurs)
        {
            if (eloActuel.ContainsKey(joueur.Id))
            {
                joueur.ClassementElo = eloActuel[joueur.Id];
                joueur.ListeElo.Add(joueur.ClassementElo);
            }
        }
        
       

        Console.WriteLine("Classements ELO recalculés !");
    }
}
