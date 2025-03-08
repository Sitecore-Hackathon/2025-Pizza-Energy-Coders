using PizzaEnergyCoders.Models;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;

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
                TemplateName = "Card",
                Title = "item1",
                KeyValues = new Dictionary<string, string>
                { {"headline", "headline 1" } ,
                {"shortDescription", "Lorem impsum" }}
            };

            if (masterDb == null)
                return;
            Item parentItem = masterDb.GetItem(Settings.GetSetting("Foundation.HomePath"));
            Item templateItem = masterDb.GetItem(Settings.GetSetting("Project.TemplatesPath") + obj.TemplateName);
            ID templateIdVariable = new ID();
            if (templateItem != null)
            {
                templateIdVariable = templateItem.ID; // Returns the GUID of the template
            }
            else
            {
                Item templatesFolder = masterDb.GetItem(Settings.GetSetting("Project.TemplatesPath") + "/");
                using (new SecurityDisabler())
                {
                    templatesFolder.Editing.BeginEdit();
                    try
                    {
                        TemplateItem standardTemplate = masterDb.GetTemplate(Sitecore.TemplateIDs.Template);
                        Item newTemplate = templatesFolder.Add(obj.TemplateName, new TemplateID(standardTemplate.ID));
                        templatesFolder.Editing.EndEdit();
                        templateIdVariable = newTemplate?.ID;
                        Item section = AddTemplateSection(newTemplate, "General");
                        foreach (var item in obj.KeyValues)
                        {
                            AddFieldToTemplate(section, item.Key, "Single-Line Text");
                        }

                    }
                    catch
                    {
                        templatesFolder.Editing.CancelEdit();
                        throw;
                    }
                }
            }
            TemplateID templateId = new TemplateID(templateIdVariable);
            using (new SecurityDisabler())
            {
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
        private Item AddTemplateSection(Item templateItem, string sectionName)
        {
            using (new SecurityDisabler())
            {
                Item section = templateItem.Children[sectionName];

                if (section == null)
                {
                    templateItem.Editing.BeginEdit();
                    try
                    {
                        section = templateItem.Add(sectionName, new TemplateID(Sitecore.TemplateIDs.TemplateSection));
                        templateItem.Editing.EndEdit();
                    }
                    catch
                    {
                        templateItem.Editing.CancelEdit();
                        throw;
                    }
                }

                return section;
            }
        }

        // Helper method to add fields to a template
        private void AddFieldToTemplate(Item section, string fieldName, string fieldType)
        {
            using (new SecurityDisabler())
            {
                section.Editing.BeginEdit();
                try
                {
                    Item newField = section.Add(fieldName, new TemplateID(Sitecore.TemplateIDs.TemplateField));
                    if (newField != null)
                    {
                        newField.Editing.BeginEdit();
                        newField["Type"] = fieldType; // Assign correct field type (e.g., "Single-Line Text")
                        newField["Title"] = fieldName; // Set the field's title
                        newField.Editing.EndEdit();
                    }

                    section.Editing.EndEdit();
                }
                catch
                {
                    section.Editing.CancelEdit();
                    throw;
                }
            }
        }

        // Helper method to create Standard Values for the template
        private void CreateStandardValues(Item templateItem)
        {
            using (new SecurityDisabler())
            {
                Item standardValues = templateItem.Children["__Standard Values"];
                if (standardValues == null)
                {
                    templateItem.Editing.BeginEdit();
                    try
                    {
                        standardValues = templateItem.Add("__Standard Values", new TemplateID(new ID(Settings.GetSetting("Foundation.StandardValuesTemplate"))));
                        templateItem.Editing.EndEdit();
                    }
                    catch
                    {
                        templateItem.Editing.CancelEdit();
                        throw;
                    }
                }
            }
        }
    }
}