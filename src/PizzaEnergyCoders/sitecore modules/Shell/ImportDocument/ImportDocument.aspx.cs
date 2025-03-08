using PizzaEnergyCoders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace PizzaEnergyCoders.sitecore_modules.Shell.ImportDocument
{
    public partial class ImportDocument : System.Web.UI.Page
    {
        readonly Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text;
            DocumentModel obj = new DocumentModel()
            {
                TemplateId = "4E37348D-A319-4577-B207-0C5F6627FDA7",
                Title = "item1",
                KeyValues = new Dictionary<string, string>
                { {"headline", "headline 1" } ,
                {"shortDescription", "Lorem impsum" }}
            };

            if (masterDb == null)
                return;
            Item parentItem = masterDb.GetItem("/sitecore/content/Home"); // Change path as needed
            TemplateID templateId = new TemplateID(new ID(obj.TemplateId)); // Replace with your template ID
            if (parentItem != null)
            {
                Item newItem = parentItem.Add(obj.Title, templateId);
                if (newItem != null)
                {
                    newItem.Editing.BeginEdit();
                    try
                    {
                        foreach (var item in obj.KeyValues)
                        {
                            newItem[item.Key] = item.Value;
                        }
                        newItem.Editing.EndEdit();
                    }
                    catch
                    {
                        newItem.Editing.CancelEdit();
                        throw;
                    }
                }
            }
        }
    }
}