using System.Collections.Generic;

namespace PizzaEnergyCoders.Models
{
    /// <summary>
    /// Model for the item creation
    /// </summary>
    public class DocumentModel
    {
        public string TemplateName { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> KeyValues { set; get; }
    }
}


