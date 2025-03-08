using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System.Collections.Specialized;

namespace PizzaEnergyCoders.Commands
{
    public class ImportDocument : Command
    {
        public override void Execute(CommandContext context)
        {
            // Start the process in the Sitecore pipeline
            Sitecore.Context.ClientPage.Start(this, "Run", new NameValueCollection
            {
            });
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                //Opens the modal 
                string url = "/sitecore modules/Shell/ImportDocument/ImportDocument.aspx";
                SheerResponse.ShowModalDialog(url, "800", "360", "Import", true);
                args.WaitForPostBack();
            }
        }
    }
}