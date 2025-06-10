using System.Xml;
using Newtonsoft.Json;
using System.Reflection;
using Formatting = Newtonsoft.Json.Formatting;

namespace API.Models
{
    public abstract class Model
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty; // Defaultwert
        public bool Inaktiv { get; set; } = false; // Defaultwert

        // Alle Attribute in der Konsole ausgeben
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
