# connect-zorro-plugin
Plugin prototype for integration with Zorro Project (http://zorro-project.com)

The plugin is not finished yet. Working features:

* oAuth Authentication
* Account list loading
* Symbols list loading

TODO:

* Subscribing to symbol prices
* Buy/Sell functionality

## How to use the plugin:

1. Create a new app on the https://sandbox-connect.spotware.com
1. Import the project to Visual Studio
1. Set Client Public ID->ClientId, Client Public ID->ClientPublic and Client Secret->ClientSecret properties at the SpotwareConnectConfiguration class
1. Build the OpenApiLib project
1. Put the OpenApiLib.dll and the Newtonsoft.Json.dll to the Zorro/Plugin folder
1. Run the Zorro application and chooose the "cTrader" account
1. You can use the TestOpenApiLib project to test functionality without Zorro platform
