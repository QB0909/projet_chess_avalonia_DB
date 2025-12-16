using System.IO;
using System.Text.Json;
using CHESS.Models;

namespace CHESS.Services
{
    public class JsonDataService
    {
        private readonly string _filePath;

        public JsonDataService(string fileName)
        {
            // Le chemin vers le fichier de sauvegarde est dans le dossier de l'application
            // pour éviter de polluer le bureau ou les documents de l'utilisateur.
            string appDataPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            _filePath = Path.Combine(appDataPath, fileName);
        }

        public Donnees ChargerDonnees()
        {
            if (!File.Exists(_filePath))
            {
                // Si le fichier n'existe pas, retourne un nouvel objet de données vide.
                return new Donnees();
            }

            string json = File.ReadAllText(_filePath);
            
            // Si le fichier est vide ou corrompu, la désérialisation peut retourner null.
            return JsonSerializer.Deserialize<Donnees>(json) ?? new Donnees();
        }

        public void SauvegarderDonnees(Donnees donnees)
        {
            // Configure les options de sérialisation pour un JSON bien indenté et lisible.
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(donnees, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
