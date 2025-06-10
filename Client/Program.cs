using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Client
{
    public class FeWo
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ort { get; set; } = string.Empty;
        public int PreisProWoche { get; set; } = 0;
        public bool Inaktiv { get; set; } = false;
    }

    public class Buchung
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte KalenderWoche { get; set; } = 1;
        public byte AnzahlPersonen { get; set; } = 1;
        public long FeWoId { get; set; }
        public bool Inaktiv { get; set; } = false;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var baseUrl = "http://localhost:5130/api"; // Adresse deiner API

            using HttpClient client = new HttpClient();

            
            var feWo = new FeWo { Name = "WohnungX", Ort = "Ankara", PreisProWoche = 1234 };
            var createResponse = await client.PostAsJsonAsync($"{baseUrl}/FeWo", feWo);
            if (!createResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Hinzufügen der FeWo: {await createResponse.Content.ReadAsStringAsync()}");
                return;
            }
            var id = await createResponse.Content.ReadFromJsonAsync<long>();
            Console.WriteLine($"FeWo hinzugefügt, ID: {id}");

            // 2. Alle Ferienwohnungen abrufen
            var allFeWos = await client.GetFromJsonAsync<List<FeWo>>($"{baseUrl}/FeWo");
            if (allFeWos != null)
                foreach (var fw in allFeWos)
                    Console.WriteLine(JsonConvert.SerializeObject(fw, Formatting.Indented));

            // 3. Eine Ferienwohnung aktualisieren
            if (allFeWos != null && allFeWos.Count > 0)
            {
                var fwToUpdate = allFeWos[0];
                fwToUpdate.Ort = "Izmir";
                var updateResp = await client.PutAsJsonAsync($"{baseUrl}/FeWo/{fwToUpdate.Id}", fwToUpdate);
                Console.WriteLine(updateResp.IsSuccessStatusCode
                    ? "Aktualisierung erfolgreich"
                    : $"Fehler bei der Aktualisierung: {await updateResp.Content.ReadAsStringAsync()}");
            }

            // 4. Ferienwohnung deaktivieren
            if (allFeWos != null && allFeWos.Count > 0)
            {
                var fwId = allFeWos[0].Id;
                var deleteResp = await client.DeleteAsync($"{baseUrl}/FeWo/{fwId}");
                Console.WriteLine(deleteResp.IsSuccessStatusCode
                    ? "Deaktivierung erfolgreich"
                    : $"Fehler bei der Deaktivierung: {await deleteResp.Content.ReadAsStringAsync()}");
            }

            // 5. Fehlerbehandlung: Löschen mit falscher ID
            var wrongDeleteResp = await client.DeleteAsync($"{baseUrl}/FeWo/999999");
            if (!wrongDeleteResp.IsSuccessStatusCode)
                Console.WriteLine($"Erwarteter Fehler: {await wrongDeleteResp.Content.ReadAsStringAsync()}");

            // ===== BUCHUNG işlemleri =====

            // 6. Neue Buchung hinzufügen (Für eine bestehende FeWo!)
            var alleFeWos2 = await client.GetFromJsonAsync<List<FeWo>>($"{baseUrl}/FeWo");
            if (alleFeWos2 == null || alleFeWos2.Count == 0)
            {
                Console.WriteLine("Keine Ferienwohnungen vorhanden. Bitte erst eine FeWo anlegen.");
                return;
            }
            var ersteFeWo = alleFeWos2[0];

            var buchung = new Buchung
            {
                Name = "Max Mustermann",
                KalenderWoche = 25,
                AnzahlPersonen = 2,
                FeWoId = ersteFeWo.Id
            };
            var createBuchungResponse = await client.PostAsJsonAsync($"{baseUrl}/Buchung", buchung);
            if (!createBuchungResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fehler beim Hinzufügen der Buchung: {await createBuchungResponse.Content.ReadAsStringAsync()}");
                return;
            }
            var buchungId = await createBuchungResponse.Content.ReadFromJsonAsync<long>();
            Console.WriteLine($"Buchung hinzugefügt, ID: {buchungId}");

            // 7. Alle Buchungen abrufen
            var alleBuchungen = await client.GetFromJsonAsync<List<Buchung>>($"{baseUrl}/Buchung");
            if (alleBuchungen != null)
                foreach (var b in alleBuchungen)
                    Console.WriteLine(JsonConvert.SerializeObject(b, Formatting.Indented));

            // 8. Eine Buchung aktualisieren
            if (alleBuchungen != null && alleBuchungen.Count > 0)
            {
                var buchungToUpdate = alleBuchungen[0];
                buchungToUpdate.AnzahlPersonen = 4;
                var updateResp = await client.PutAsJsonAsync($"{baseUrl}/Buchung/{buchungToUpdate.Id}", buchungToUpdate);
                Console.WriteLine(updateResp.IsSuccessStatusCode
                    ? "Aktualisierung erfolgreich"
                    : $"Fehler bei der Aktualisierung: {await updateResp.Content.ReadAsStringAsync()}");
            }

            // 9. Buchung deaktivieren
            if (alleBuchungen != null && alleBuchungen.Count > 0)
            {
                var bId = alleBuchungen[0].Id;
                var deleteResp = await client.DeleteAsync($"{baseUrl}/Buchung/{bId}");
                Console.WriteLine(deleteResp.IsSuccessStatusCode
                    ? "Deaktivierung erfolgreich"
                    : $"Fehler bei der Deaktivierung: {await deleteResp.Content.ReadAsStringAsync()}");
            }

            // 10. Fehlerbehandlung: Löschen mit falscher ID
            var wrongDeleteBuchungResp = await client.DeleteAsync($"{baseUrl}/Buchung/999999");
            if (!wrongDeleteBuchungResp.IsSuccessStatusCode)
                Console.WriteLine($"Erwarteter Fehler: {await wrongDeleteBuchungResp.Content.ReadAsStringAsync()}");
        }
    }
}