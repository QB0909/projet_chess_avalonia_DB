using CHESS.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CHESS.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Commande pour calculer les classements ELO (une seule application par partie)
    [RelayCommand]
    private void CalculerElo()
    {
        const int K_facteur = 32;

        // Assurer la collection d'historique existe
        if (Donnees.EloHistorique == null)
            Donnees.EloHistorique = new System.Collections.Generic.List<Elo>();

        // Pour chaque partie terminée, appliquer le calcul une seule fois
        foreach (var partie in Donnees.Parties.OrderBy(p => p.Id))
        {
            if (partie.Resultat == ResultatPartie.NonTermine)
                continue;

                // Si l'historique contient déjà une entrée pour cette partie (pour l'un des joueurs), on skip
                if (Donnees.EloHistorique.Any(e => e.Records.Any(r => r.PartieId == partie.Id)))
                    continue;

            var joueurBlancs = Donnees.Joueurs.FirstOrDefault(j => j.Id == partie.IdJoueurBlancs);
            var joueurNoirs = Donnees.Joueurs.FirstOrDefault(j => j.Id == partie.IdJoueurNoirs);
            if (joueurBlancs == null || joueurNoirs == null)
            {
                Console.WriteLine($"Skipping game {partie.Id}: players not found.");
                continue;
            }

            int Ra = joueurBlancs.ClassementElo == 0 ? 1200 : joueurBlancs.ClassementElo;
            int Rb = joueurNoirs.ClassementElo == 0 ? 1200 : joueurNoirs.ClassementElo;

            double Ea = 1.0 / (1.0 + Math.Pow(10.0, (Rb - Ra) / 400.0));
            double Eb = 1.0 / (1.0 + Math.Pow(10.0, (Ra - Rb) / 400.0));

            double Sa = 0.0, Sb = 0.0;
            if (partie.Resultat == ResultatPartie.VictoireBlancs) { Sa = 1.0; Sb = 0.0; }
            else if (partie.Resultat == ResultatPartie.VictoireNoirs) { Sa = 0.0; Sb = 1.0; }
            else { Sa = 0.5; Sb = 0.5; }

            int nouvelEloB = (int)Math.Round(Ra + K_facteur * (Sa - Ea));
            int nouvelEloN = (int)Math.Round(Rb + K_facteur * (Sb - Eb));

            // Enregistrer l'historique par joueur (création si nécessaire)
            var eloPlayerB = Donnees.EloHistorique.FirstOrDefault(e => e.Id == joueurBlancs.Id);
            if (eloPlayerB == null)
            {
                eloPlayerB = new Elo { Id = joueurBlancs.Id, Records = new List<EloRecord>() };
                Donnees.EloHistorique.Add(eloPlayerB);
            }
            eloPlayerB.Records.Add(new EloRecord { PartieId = partie.Id, Elo = nouvelEloB });

            var eloPlayerN = Donnees.EloHistorique.FirstOrDefault(e => e.Id == joueurNoirs.Id);
            if (eloPlayerN == null)
            {
                eloPlayerN = new Elo { Id = joueurNoirs.Id, Records = new List<EloRecord>() };
                Donnees.EloHistorique.Add(eloPlayerN);
            }
            eloPlayerN.Records.Add(new EloRecord { PartieId = partie.Id, Elo = nouvelEloN });

            // Mettre à jour les joueurs
            joueurBlancs.ClassementElo = nouvelEloB;
            joueurNoirs.ClassementElo = nouvelEloN;
        }

        // Sauvegarder les données après traitement
        try
        {
            _dataService?.SauvegarderDonnees(Donnees);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur sauvegarde ELO: {ex.Message}");
        }

        Console.WriteLine("Classements ELO recalculés et historisés !");
    }

    // plus de GenerateEloId : les enregistrements sont indexés par joueurId
}
