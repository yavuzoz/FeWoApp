using API.Data;
using API.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Services
{
    public class FeWoService : IService<FeWo>
    {
        private readonly FeWoDbContext _context;

        public FeWoService(FeWoDbContext context)
        {
            _context = context;
        }

        public FeWo? LesenEinzeln(long id)
        {
            return _context.FeWos.Find(id);
        }

        public List<FeWo> LesenAlle()
        {
            return _context.FeWos.ToList();
        }

        public List<FeWo> LesenAlleAktive()
        {
            return _context.FeWos.Where(f => !f.Inaktiv).ToList();
        }

        public long Erstellen(FeWo feWo)
        {
            try
            {
                _context.FeWos.Add(feWo);
                _context.SaveChanges();
                Console.WriteLine($"FeWo '{feWo.Name}' ({feWo.Ort}) erfolgreich erstellt mit ID: {feWo.Id}");
                return feWo.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Erstellen der FeWo: {ex.Message}");
                return 0;
            }
        }

        public bool Aktualisieren(FeWo feWo)
        {
            try
            {
                var existingFeWo = _context.FeWos.Find(feWo.Id);
                if (existingFeWo == null)
                {
                    Console.WriteLine($"FeWo mit ID {feWo.Id} nicht gefunden.");
                    return false;
                }

                _context.Entry(existingFeWo).CurrentValues.SetValues(feWo);
                _context.SaveChanges();
                Console.WriteLine($"FeWo '{feWo.Name}' (ID: {feWo.Id}) erfolgreich aktualisiert.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Aktualisieren der FeWo: {ex.Message}");
                return false;
            }
        }

        public bool Deaktivieren(long id)
        {
            try
            {
                var feWo = _context.FeWos.Find(id);
                if (feWo == null)
                {
                    Console.WriteLine($"FeWo mit ID {id} nicht gefunden.");
                    return false;
                }
                feWo.Inaktiv = true;
                _context.SaveChanges();
                Console.WriteLine($"FeWo '{feWo.Name}' (ID: {id}) erfolgreich deaktiviert.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Deaktivieren der FeWo: {ex.Message}");
                return false;
            }
        }
    }
}
