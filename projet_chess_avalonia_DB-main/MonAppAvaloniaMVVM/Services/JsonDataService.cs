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
            //  le fichier de sauvegarde est dans data
          
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
                
                return new Donnees();
            }

            string json = File.ReadAllText(_filePath);
            
          
            return JsonSerializer.Deserialize<Donnees>(json) ?? new Donnees();
        }

        public void SauvegarderDonnees(Donnees donnees)
        {
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(donnees, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
