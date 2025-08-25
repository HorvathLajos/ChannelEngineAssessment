# ChannelEngineAssessment
 A .NET application with two entry points, a console app and an ASP.NET web app, which are connected to the ChannelEngine REST-API.
 
Please make sure to create your appsettings.json file like so:

{

"ChannelEngine": {

"BaseUrl": "https://api-dev.channelengine.net/api/v2/",

"ApiKey": "YOUR_API_KEY"

},

"Logging": {

"LogLevel": {

"Default": "Information",

"Microsoft.AspNetCore": "Warning"

}

},

"AllowedHosts": "*"

}
