using PizzaEnergyCoders.Services;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;
using System.Web.UI;

namespace PizzaEnergyCoders.sitecore_modules.Shell.ImportDocument
{
    public partial class ImportDocument : System.Web.UI.Page
    {
        //Gets master DB
        readonly Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// Main method to trigger the import module. It is called from the aspx button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void btnImport_Click(object sender, EventArgs e)
        {
            //Sets Javascripts to show loader gift
            string scriptInit = @"
              document.querySelector('.loader').style.display = 'block';
              document.querySelector('.modal').style.display = 'none';";

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowLoader", scriptInit, true);
            //Gets the url from the Textbox
            string url = txtUrl.Text;
            //Set variables to show the final message
            bool createdTemplate = false;
            int createdItems = 0;

            if (masterDb == null)
                return;
            //Sets the main path to create the new items
            Item parentItem = masterDb.GetItem(Settings.GetSetting("Foundation.HomePath"));
            //Gets rows from the URL
            var document = await ReadFileGoogle.Import(url);
            //Init Open AI
            OpenAIService openAIService = new OpenAIService();

            foreach (var doc in document)
            {
                //Gets the template item / id
                Item templateItem = masterDb.GetItem(Settings.GetSetting("Project.TemplatesPath") + doc.TemplateName);

                ID templateIdVariable = new ID();
                //checks if the template exists in the CMS
                if (templateItem != null)
                {
                    templateIdVariable = templateItem.ID; // Returns the GUID of the template
                }
                else
                {
                    //Sets the path for new templates
                    Item templatesFolder = masterDb.GetItem(Settings.GetSetting("Project.TemplatesPath") + "/");

                    using (new SecurityDisabler())
                    {
                        templatesFolder.Editing.BeginEdit();
                        try
                        {
                            TemplateItem standardTemplate = masterDb.GetTemplate(Sitecore.TemplateIDs.Template);
                            //creates the new template
                            Item newTemplate = templatesFolder.Add(doc.TemplateName, new TemplateID(standardTemplate.ID));
                            //Sets the Standard template 
                            using (new EditContext(newTemplate))
                            {
                                newTemplate.Fields[Settings.GetSetting("Foundation.BaseTemplateField")].Value = Settings.GetSetting("Foundation.StandardTemplate");
                            }
                            templatesFolder.Editing.EndEdit();
                            templateIdVariable = newTemplate?.ID;
                            //Creates section
                            Item section = AddTemplateSection(newTemplate, "General");
                            foreach (var item in doc.KeyValues)
                            {
                                //get fieldtype
                                var openAIServiceResponse = await openAIService.GetChatCompletionAsync(item.Value);
                                var fieldType = openAIServiceResponse.Choices[0].Message.Content;

                                if (string.IsNullOrEmpty(fieldType))
                                    fieldType = "Single-Line Text";
                                //Creates new field
                                AddFieldToTemplate(section, item.Key, fieldType);
                            }
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
                //Creates new Items
                using (new SecurityDisabler())
                {
                    if (parentItem == null)
                        break;
                    //Creates new Item
                    Item newItem = parentItem.Add(CheckItem(doc.Title), templateId);
                    if (newItem == null)
                        break;
                    newItem.Editing.BeginEdit();
                    try
                    {
                        foreach (var item in doc.KeyValues)
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
                createdItems++;
            }

            //Shows the summary label
            lblSummary.Text = (createdTemplate ? "A template has been created. " : "") + "Total of created items: " + createdItems + ".";

            //Sets Javascripts to hide loader gift
            string script = @"
              document.querySelector('.loader').style.display = 'none';
              document.querySelector('.modal').style.display = 'block';";

            ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", script, true);
        }
        /// <summary>
        /// Helper Method th check if the item exits in the CMS. If it is true changes the name until it does not.
        /// </summary>
        /// <param name="itemTitle">Item title</param>
        /// <param name="iteration">Iteration number</param>
        /// <param name="itemTitleOriginal">Item Original title</param>
        /// <returns>New Item title</returns>
        private string CheckItem(string itemTitle, int iteration = 0, string itemTitleOriginal = "")
        {
            Item newItem = masterDb.GetItem(Settings.GetSetting("Foundation.HomePath") + "/" + itemTitle);
            if (newItem == null)
                return itemTitle;

            if (itemTitleOriginal == "")
                itemTitleOriginal = itemTitle;
            return CheckItem(itemTitleOriginal + "_" + iteration++, iteration++, itemTitleOriginal);
        }
        /// <summary>
        /// Helper Method to add section to the template
        /// </summary>
        /// <param name="templateItem">template Id</param>
        /// <param name="sectionName">section Name to create</param>
        /// <returns>Returns item</returns>
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

        /// <summary>
        /// Helper method to add fields to a template
        /// </summary>
        /// <param name="section">Section Name</param>
        /// <param name="fieldName">New field Name</param>
        /// <param name="fieldType">New field Type</param>
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

        /// <summary>
        ///  Helper method to create Standard Values for the template
        /// </summary>
        /// <param name="templateItem"></param>
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