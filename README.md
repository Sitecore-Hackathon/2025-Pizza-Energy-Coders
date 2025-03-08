![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

# Sitecore Hackathon 2025

-   MUST READ: **[Submission requirements](SUBMISSION_REQUIREMENTS.md)**
-   [Entry form template](ENTRYFORM.md)

## Team name

⟹ Pizza Energy Coders

## Category

⟹ Integration / AI  

## Description

In Sitecore content management, one of the most tedious and repetitive tasks for editors is manually importing data from external documents. Typically, when the team receives information in formats like Google Docs or Google Sheets, they must manually copy and paste the content, define the structure, create templates, and then generate items in Sitecore one by one.
To optimize this workflow, we automated the import process, allowing editors to simply enter a Google Docs or Google Sheets URL within Sitecore. Our system then:
Automatically reads the file using Google APIs.
Identifies the data structure (field names and values).
Uses AI to determine the appropriate field type in Sitecore (text, number, date, etc.), Also uses AI to check if the data contains sensitive data.
Generates a Sitecore template dynamically based on the extracted data.
Creates Sitecore items using the content from the file.
With this process, we achieve:
A significant reduction in manual content entry time.
Elimination of human errors in content transcription.
Standardization of how data is structured and stored.
A seamless workflow that lets editors focus on content quality rather than manual data entry.
This automation transforms the way content editors work in Sitecore, making the process faster, more accurate, and highly efficient.

⟹ Creates items/templates from an online document (Google Docs/ Google Spreadsheet)

> This module requires Sitecore 10.4, Open AI API Key and Google Cloud.

## Pre-requisites and Dependencies

Our Import module does need two key Pre-requisites
-Google Cloud (credentials.json)

#### Credentials step by step Set up

-   Login in Google Cloud
-   Click on "Get started for free"
    ![imageLogin](docs/images/image1.png?raw=true)
-   Create a new Project
    ![imageNewProject1](docs/images/image2.png?raw=true)
    ![imageNewProject2](docs/images/image3.png?raw=true)
    ![imageNewProject3](docs/images/image5.png?raw=true)
-   Set everything as the image below
    ![imageSetup](docs/images/image4.png?raw=true)
-   Select created Project
    ![imageProject](docs/images/image7.png?raw=true)
-   On the left menu bar select "APIs & Services" then "Library"
    ![imageAPI](docs/images/image6.png?raw=true)
-   Click on "View All"
    ![imageViewAll](docs/images/image9.png?raw=true)
-   Enable three APIs
    -   Google Drive API
        ![imageDrive](docs/images/image8.png?raw=true)
    -   Google Docs API
        ![imageDocs](docs/images/image10.png?raw=true)
    -   Google Sheets API
        ![imageSheets](docs/images/image12.png?raw=true)
-   On the left menu bar select "Credentials"
    ![imageCredentials](docs/images/image11.png?raw=true)
-   Create credentials "Service account"
    ![imageServiceAccount](docs/images/image13.png?raw=true)
-   Set a "Service Name"
    ![imageServiceName](docs/images/image14.png?raw=true)
-   When finished click on "Manage service accounts"
    ![imageManageService](docs/images/image15.png?raw=true)
-   On Service Accounts click on Actions button and select "Manage Keys"
    ![imageManageKey](docs/images/image16.png?raw=true)
-   Click on "Add Key" and "Create new key"
    ![imageCreateKey](docs/images/image17.png?raw=true)
-   Select "JSON" and create
    ![imageJson](docs/images/image18.png?raw=true)
-   Save as "credentials.json"
    ![saveJson](docs/images/image19.png?raw=true)

#### Open AI API Key

-   How to get an OpenAI Key
1. Open and login at https://platform.openai.com/settings/organization/api-keys
2.	In the left pane click API keys
3.	Click in the green button “Create new secret key”

![saveJson](docs/images/OpenAI1.png?raw=true)

4.	Give a name to the key and select Default project, leave permissions “All”
![saveJson](docs/images/OpenAI2.png?raw=true)

5.	Click create secret key
![saveJson](docs/images/OpenAI3.png?raw=true)

6.	Copy the key
7.	Open the following file located in the local published site e.g. C:\inetpub\wwwroot\yourlocaldomain\App_Config\Include\ImportDocumentCommand.config
8.	Locate the OPENAI_APIKEY setting
![saveJson](docs/images/OpenAI4.png?raw=true)

9.	Paste the key in the value attribute
![saveJson](docs/images/OpenAI5.png?raw=true)


#### API Key set up

## Installation instructions

⟹ Please follow the instruction below

-   Install the next [SitecorePackage](packages/ImportDocument-2.zip)
    -   This package will add "Import Document" in the Ribbon under "Developer" tab
-   Add the "credentials.json" created in the pre requisites in the local published solution under this folder "\App_Data\Creds"
-   Add the Open AI Key in "ImportDocumentCommand.config" in "OPENAI_APIKEY" setting value in the local published solution under this folder"\App_Config\Include"


## Usage instructions

Once the module is installed, its functionality must be taken into account in order to use it. This module loads the file information online, in the process creates the necessary templates and then creates the items with the file information.

Process of uploading a Google Sheets file
1.	You need to create a file in Google Sheets with the following structure
![saveJson](docs/images/OpenAI6.png?raw=true)

-	Column A
    -	Header: Template name (fixed) 
    -	Body: Template’s Names used to create/locate the template.
-	Column B
    -	Header: title (fixed) 
    -	Body: Item’s names to create new items.
-	Column C to Z
    -	Header: Field’s names used to set the fields on the template and set the content on the items.
    -	Body: Field’s content to create the items

A sample Google Sheets file can be found at the following URL
https://docs.google.com/spreadsheets/d/1i3NB4bzcSj8W58AQO5UrLembNdadCZw5JDiL-aNkxwo/edit?gid=0#gid=0

2.	Login to Sitecore Content Editor
3.	Click on the DEVELOPER tab, click on “Import Document”. A modal will open to load the file.

![saveJson](docs/images/OpenAI7.png?raw=true)

![saveJson](docs/images/OpenAI8.png?raw=true)

4.	Enter the URL of the Google Sheets or Google Docs in the textbox, then click on “Import”
5.	Once the data has been imported, the following message will appear on the screen

![saveJson](docs/images/OpenAI9.png?raw=true)

6.	We review the items created

![saveJson](docs/images/OpenAI10.png?raw=true)

The module uses AI to set the field type in the template and to check if any string in the row has sensitive data like email address.

7.	We review the templates created

![saveJson](docs/images/OpenAI11.png?raw=true)

If you want to upload a file from Google Docs you have to follow the following structure:
-	File Name: Template’s Name
-	First Column of the table: Fields Names
-	Second Column of the table: Fields Content.

A sample Google Docs file can be found at the following URL
https://docs.google.com/document/d/1u72UPKpV3XzOE7GiDoAa4NpRH_SAMh2fSeGVCBQYs4I/edit?tab=t.0
