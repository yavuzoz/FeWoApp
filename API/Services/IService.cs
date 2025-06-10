using API.Models;

namespace API.Services
{
    // <<interface>> Service.IService
    public interface IService<T> where T : Model
    {
        T? LesenEinzeln(long id);
        List<T> LesenAlle();
        List<T> LesenAlleAktive();
        long Erstellen(T model); // Primärschlüssel zurück oder 0
        bool Aktualisieren(T model); // Erfolgreich oder nicht
        bool Deaktivieren(long id); // Erfolgreich oder nicht
    }
}
