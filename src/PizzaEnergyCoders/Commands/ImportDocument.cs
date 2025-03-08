using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace PizzaEnergyCoders.Commands
{
    public class ImportDocument : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.Start(this, "Run", new NameValueCollection{});
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                string url = "/sitecore modules/Shell/ImportDocument/ImportDocument.aspx";
                SheerResponse.ShowModalDialog(url, "800", "360", "Import", true);
                args.WaitForPostBack();
            }
        }
    }
}