using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace API.Models
{
    [Table("Buchung")] // Veritabanı tablo adını belirtir
    public class Buchung : Model
    {
        public byte KalenderWoche { get; set; } = 1; // Pflichtfeld, Defaultwert (1-52)
        public byte AnzahlPersonen { get; set; } = 1; // Pflichtfeld, Defaultwert

        // Yabancı anahtar (FK) için FeWoId
        public long FeWoId { get; set; } // Pflichtfeld
        // İlişkili FeWo nesnesi (navigation property)
        public FeWo? FeWo { get; set; }
    }
}
