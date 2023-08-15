# CheapSharkAPI_App
I have developed an application that utilizes the CheapShark API to collect extensive information about games and their sales across various stores. The API also enables users to set up alerts for when a game reaches a specific price or falls below it, triggering an email notification. I chose this API for its comprehensive and up-to-date data.

To switch between using the API and local files, go to the MainViewModel.cs file, located in the ViewModel folder. Change the UseAPI property to true to use the API or false to use local files.

The "load more games" button at the end of the game list loads in over 100 games. Although the games are loaded in per 25 IDs, not all IDs are used, resulting in slightly more than 100 games being loaded.

You can also receive notifications when a specific game drops below a designated price. Simply provide your email, specify the desired price point, and then click the "set price alert" button.

link to the api: https://apidocs.cheapshark.com/
