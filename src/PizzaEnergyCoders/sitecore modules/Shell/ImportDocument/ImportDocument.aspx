<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportDocument.aspx.cs" Inherits="PizzaEnergyCoders.sitecore_modules.Shell.ImportDocument.ImportDocument" %>
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8" />
    <title>Import Document</title>
    <style>
      /* Estilos para el modal */
      .modal {
        width: 500px;
        height: auto;
        margin: 10px auto;
        padding: 20px;
        border: 1px solid #ccc;
        background-color: #fff;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        border-radius: 5px;
      }
      /* Cabecera del modal */
      .modal-header {
        background-color: #007bff;
        color: white;
        padding: 10px;
        font-size: 18px;
        border-radius: 5px 5px 0 0;
      }
      /* Cuerpo del modal */
      .modal-body {
        padding: 20px;
      }
      /* Estilo del botón */
      .btn {
        padding: 10px;
        font-size: 14px;
        cursor: pointer;
        background-color: #28a745;
        color: white;
        border: none;
        border-radius: 5px;
        width: 100%;
      }
      .btn:hover {
        background-color: #218838;
      }
    </style>
  </head>
  <body>
    <form id="form1" runat="server">
        <!-- Modal -->
        <div class="modal">
            <!-- Cabecera del Modal -->
            <div class="modal-header">
                <h2>Document Importer</h2>
            </div>
            <!-- Cuerpo del Modal -->
            <div class="modal-body">
                <label for="txtUrl">Enter URL:</label>
                <asp:TextBox ID="txtUrl" runat="server" CssClass="form-control"></asp:TextBox>
                <br />   
                <asp:Button ID="btnImport" runat="server" Text="Import to Sitecore" OnClick="btnImport_Click" CssClass="btn" />
            </div>
        </div>
    </form>
  </body>
</html>
