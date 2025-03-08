![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

# Sitecore Hackathon 2025

-   MUST READ: **[Submission requirements](SUBMISSION_REQUIREMENTS.md)**
-   [Entry form template](ENTRYFORM.md)

### ⟹ [Insert your documentation here](ENTRYFORM.md) <<

## Team name

⟹ Pizza Energy coders

## Category

⟹ Free for all

## Description

⟹ Creates items/templates from an online document (Google Docs/ Google Spreadsheet)
Our module is created in a Sitecore 10.4 XP Single On Premises Environment with the help of Open AI and Google Cloud.

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

-Open AI API Key

#### API Key set up

## Installation instructions

⟹ Please follow the instruction below

-   Install the next [SitecorePackage](packages/ImportDocument-2.zip)
    -   This package will add "Import Document" in the Ribbon under "Developer" tab
-   Add the "credentials.json" created in the pre requisites in the local published solution under this folder "\App_Data\Creds"
-   Add the Open AI Key in "ImportDocumentCommand.config" in "OPENAI_APIKEY" setting value in the local published solution under this folder"\App_Config\Include"
