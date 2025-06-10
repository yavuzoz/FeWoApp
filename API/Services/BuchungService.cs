using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace API.Services
{
    public class BuchungService : IService<Buchung>
    {
        private readonly FeWoDbContext _context;

        public BuchungService(FeWoDbContext context)
        {
            _context = context;
        }

        public Buchung? LesenEinzeln(long id)
        {
            return _context.Buchungen
                           .Include(b => b.FeWo) // İlişkili FeWo nesnesini de yükle
                           .FirstOrDefault(b => b.Id == id);
        }

        public List<Buchung> LesenAlle()
        {
            return _context.Buchungen
                           .Include(b => b.FeWo) // İlişkili FeWo nesnesini de yükle
                           .ToList();
        }

        public List<Buchung> LesenAlleAktive()
        {
            return _context.Buchungen
                           .Include(b => b.FeWo)
                           .Where(b => !b.Inaktiv)
                           .ToList();
        }

        public long Erstellen(Buchung buchung)
        {
            // Kontrol: Aynı FeWo ve KalenderWoche için başka aktif Buchung var mı?
            if (_context.Buchungen.Any(b => b.FeWoId == buchung.FeWoId && b.KalenderWoche == buchung.KalenderWoche && !b.Inaktiv))
            {
                Console.WriteLine($"Fehler: Wohnung (ID: {buchung.FeWoId}) ist in Kalenderwoche {buchung.KalenderWoche} bereits belegt.");
                return 0;
            }

            try
            {
                _context.Buchungen.Add(buchung);
                _context.SaveChanges();
                Console.WriteLine($"Buchung '{buchung.Name}' für FeWo ID {buchung.FeWoId} in KW {buchung.KalenderWoche} erfolgreich erstellt mit ID: {buchung.Id}");
                return buchung.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Erstellen der Buchung: {ex.Message}");
                return 0;
            }
        }

        public bool Aktualisieren(Buchung buchung)
        {
            var existingBuchung = _context.Buchungen.Find(buchung.Id);
            if (existingBuchung == null)
            {
                Console.WriteLine($"Buchung mit ID {buchung.Id} nicht gefunden.");
                return false;
            }

            // Eğer KalenderWoche veya FeWoId değiştiyse çakışma kontrolü yap
            if (existingBuchung.FeWoId != buchung.FeWoId || existingBuchung.KalenderWoche != buchung.KalenderWoche)
            {
                if (_context.Buchungen.Any(b => b.FeWoId == buchung.FeWoId && b.KalenderWoche == buchung.KalenderWoche && b.Id != buchung.Id && !b.Inaktiv))
                {
                    Console.WriteLine($"Fehler: Wohnung (ID: {buchung.FeWoId}) ist in Kalenderwoche {buchung.KalenderWoche} bereits belegt.");
                    return false;
                }
            }

            try
            {
                _context.Entry(existingBuchung).CurrentValues.SetValues(buchung);
                _context.SaveChanges();
                Console.WriteLine($"Buchung '{buchung.Name}' (ID: {buchung.Id}) erfolgreich aktualisiert.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Aktualisieren der Buchung: {ex.Message}");
                return false;
            }
        }

        public bool Deaktivieren(long id)
        {
            try
            {
                var buchung = _context.Buchungen.Find(id);
                if (buchung == null)
                {
                    Console.WriteLine($"Buchung mit ID {id} nicht gefunden.");
                    return false;
                }
                buchung.Inaktiv = true;
                _context.SaveChanges();
                Console.WriteLine($"Buchung '{buchung.Name}' (ID: {id}) erfolgreich deaktiviert.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Deaktivieren der Buchung: {ex.Message}");
                return false;
            }
        }
    }
}
