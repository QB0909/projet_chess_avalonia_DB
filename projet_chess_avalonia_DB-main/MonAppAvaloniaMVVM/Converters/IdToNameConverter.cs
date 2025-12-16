using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using CHESS.Models;

namespace CHESS.Converters
{
    public class IdToNameConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 2) return string.Empty;

            if (values[0] == null) return string.Empty;

            int id;
            try
            {
                id = System.Convert.ToInt32(values[0]);
            }
            catch
            {
                return string.Empty;
            }

            var joueursObj = values[1];
            if (joueursObj == null) return $"#{id}";

            var joueurs = joueursObj as IEnumerable<Joueur> ?? (joueursObj as System.Collections.IEnumerable)?.Cast<object?>().OfType<Joueur>();

            var joueur = joueurs?.FirstOrDefault(j => j.Id == id);
            if (joueur == null) return $"#{id}";

            return string.IsNullOrWhiteSpace(joueur.Prenom) ? joueur.Nom : $"{joueur.Prenom} {joueur.Nom}";
        }

        public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
