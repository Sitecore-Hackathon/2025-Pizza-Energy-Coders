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
            // Inicia el proceso en el pipeline de Sitecore
            Sitecore.Context.ClientPage.Start(this, "Run", new NameValueCollection
            {
                //{ "itemId", itemSeleccionado.ID.ToString() }
            });
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                // Abre una ventana modal para la interacción con la IA
                string url = "/sitecore modules/Shell/ImportDocument/ImportDocument.aspx";
                SheerResponse.ShowModalDialog(url, "640", "360", "Import", true);
                args.WaitForPostBack();
            }
            else
            {
                if (!string.IsNullOrEmpty(args.Result) && args.Result != "undefined")
                {
                    /*string[] resultado = args.Result.Split('|');
                    if (resultado.Length == 2)
                    {
                        string nombreCampo = resultado[0];
                        string contenidoGenerado = resultado[1];

                        Item item = Sitecore.Context.Database.GetItem(args.Parameters["itemId"]);
                        if (item != null)
                        {
                            item.Editing.BeginEdit();
                            item.Fields[nombreCampo].Value = contenidoGenerado;
                            item.Editing.EndEdit();
                            SheerResponse.Alert("El campo ha sido actualizado con el contenido generado.");
                        }
                    }*/
                }
            }
        }
    }
}