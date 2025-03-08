using System.Collections.Generic;

namespace PizzaEnergyCoders.Models
{
    public class DocumentModel
    {
        public string TemplateId { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> KeyValues { set; get; }
    }
}