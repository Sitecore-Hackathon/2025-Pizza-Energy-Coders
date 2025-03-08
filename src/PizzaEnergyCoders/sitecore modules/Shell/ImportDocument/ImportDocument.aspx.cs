using PizzaEnergyCoders.Services;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace PizzaEnergyCoders.sitecore_modules.Shell.ImportDocument
{
    public partial class ImportDocument : System.Web.UI.Page
    {
        readonly Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
        protected void Page_Load(object sender, EventArgs e)
        {
            lblSummary.Text = "";
        }

        protected async void btnImport_Click(object sender, EventArgs e)
        {
            string scriptInit = @"
              document.querySelector('.loader').style.display = 'block';
              document.querySelector('.modal').style.display = 'none';";

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowLoader", scriptInit, true);

            string url = txtUrl.Text;
            bool createdTemplate = false;
            int createdItems = 0;
            if (masterDb == null)
                return;
            Item parentItem = masterDb.GetItem(Settings.GetSetting("Foundation.HomePath"));
            var document = await ReadFileGoogle.Import(url);

            OpenAIService openAIService = new OpenAIService();

            foreach (var doc in document)
            {
                Item templateItem = masterDb.GetItem(Settings.GetSetting("Project.TemplatesPath") + doc.TemplateName);
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
                            Item newTemplate = templatesFolder.Add(doc.TemplateName, new TemplateID(standardTemplate.ID));
                            using (new EditContext(newTemplate))
                            {
                                newTemplate.Fields[Settings.GetSetting("Foundation.BaseTemplateField")].Value = Settings.GetSetting("Foundation.StandardTemplate");
                            }
                            templatesFolder.Editing.EndEdit();
                            templateIdVariable = newTemplate?.ID;
                            Item section = AddTemplateSection(newTemplate, "General");
                            List<bool> sensitiveArray = new List<bool>();

                            foreach (var item in doc.KeyValues)
                            {
                                //get fieldtype
                                var openAIServiceResponses = await openAIService.GetChatCompletionAsync(item.Value, false);
                                var fieldType = openAIServiceResponses.Choices[0].Message.Content;

                                if (string.IsNullOrEmpty(fieldType))
                                    fieldType = "Single-Line Text";

                                AddFieldToTemplate(section, item.Key, fieldType);
                            }
                            AddFieldToTemplate(section, "hasSensitiveData", "Checkbox");

                            createdTemplate = true;
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
                    if (parentItem == null)
                        break;

                    Item newItem = parentItem.Add(CheckItem(doc.Title), templateId);
                    if (newItem == null)
                        break;
                    newItem.Editing.BeginEdit();
                    try
                    {
                        int sensitiveCounter = 0;
                        foreach (var item in doc.KeyValues)
                        {
                            //check sensitive data
                            var openAIServiceResponsesD = await openAIService.GetChatCompletionAsync(item.Value, true);
                            var hasSensitiveData = openAIServiceResponsesD.Choices[0].Message.Content;

                            if (Convert.ToBoolean(hasSensitiveData))
                            {
                                sensitiveCounter = sensitiveCounter + 1;
                            }

                            newItem[item.Key] = item.Value;
                        }
                        if (sensitiveCounter > 0)
                        {
                            newItem["hasSensitiveData"] = "1";
                        }

                        newItem.Editing.EndEdit();
                    }
                    catch
                    {
                        newItem.Editing.CancelEdit();
                        throw;
                    }
                }
                createdItems++;
            }
            lblSummary.Text = (createdTemplate ? "A template was created. " : "") + "Total of items: " + createdItems + " created";

            string script = @"
              document.querySelector('.loader').style.display = 'none';
              document.querySelector('.modal').style.display = 'block';";

            ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", script, true);
        }

        private string CheckItem(string itemTitle, int iteration = 0, string itemTitleOriginal = "")
        {
            Item newItem = masterDb.GetItem(Settings.GetSetting("Foundation.HomePath") + "/" + itemTitle);
            if (newItem == null)
                return itemTitle;

            if (itemTitleOriginal == "")
                itemTitleOriginal = itemTitle;
            return CheckItem(itemTitleOriginal + "_" + iteration++, iteration++, itemTitleOriginal);
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