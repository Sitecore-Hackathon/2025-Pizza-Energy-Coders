<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8" />
    <title>Import Document</title>
    <style>
      /* General styles */
      body {
        font-family: Arial, sans-serif;
        background-color: #f8f9fa;
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100vh;
        margin: 0;
      }
      /* Modal container */
      .modal {
        width: 400px;
        background-color: #fff;
        border-radius: 10px;
        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
        overflow: hidden;
        text-align: center;
        padding: 20px;
      }
      /* Modal header */
      .modal-header {
        border: solid 1px ;
        border-color: #007bff;
        color: #007bff;
        padding: 15px;
        font-size: 20px;
        font-weight: bold;
      }
      /* Modal body */
      .modal-body {
        padding: 20px;
      }
      /* Input field */
      .form-control {
        width: calc(100% - 20px);
        padding: 10px;
        font-size: 16px;
        border: 1px solid #ccc;
        border-radius: 5px;
        margin-top: 10px;
      }
      /* Button */
      .btn {
        padding: 12px;
        font-size: 16px;
        cursor: pointer;
        background-color: #28a745;
        color: white;
        border: none;
        border-radius: 5px;
        width: 100%;
        margin-top: 15px;
        transition: background-color 0.3s;
      }
      .btn:hover {
        background-color: #218838;
      }
    </style>
  </head>
  <body>
    <div class="modal">
      <div class="modal-header">Document Importer</div>
 
      <div class="modal-body">
        <label for="txtUrl">Enter URL:</label>
        <input
          type="text"
          id="txtUrl"
          class="form-control"
          placeholder="Enter document URL"
        />
        <button id="btnImport" class="btn" onclick="importToSitecore()">
          Import to Sitecore
        </button>
      </div>
    </div>
 
    <script>
      function importToSitecore() {
        const url = document.getElementById("txtUrl").value;
        if (!url) {
          alert("Please enter a valid URL");
          return;
        }
        alert("Importing: " + url);
      }
    </script>
  </body>
</html>