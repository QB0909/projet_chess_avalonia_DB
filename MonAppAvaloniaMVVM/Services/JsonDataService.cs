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
            // Cherche un fichier "data/{fileName}" en remontant l'arborescence
            // Cela permet d'utiliser le fichier de données du projet (projet_chess_avalonia_DB/data/federation.json)
            // si présent, sinon on crée/utilise "data" dans le répertoire courant.
            string? found = null;
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "data", fileName);
                if (File.Exists(candidate))
                {
                    found = candidate;
                    break;
                }
                dir = dir.Parent;
            }

            if (found != null)
            {
                _filePath = found;
                return;
            }

            // Sinon utiliser/créer data/ dans le répertoire courant
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
