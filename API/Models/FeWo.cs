using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace API.Models
{
    [Table("FeWo")] // Veritabanı tablo adını belirtir
    public class FeWo : Model
    {
        public string Ort { get; set; } = string.Empty; // Pflichtfeld, Defaultwert
        public int PreisProWoche { get; set; } = 0; // Pflichtfeld, Defaultwert

        // İlişkili rezervasyonlar (navigation property)
        public ICollection<Buchung>? Buchungen { get; set; }
    }
}
