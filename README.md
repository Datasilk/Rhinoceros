# Rhinoceros
Create desktop applications with ease by combining any website with a borderless Chrome browser window. 


#### Requirements
* Visual Studio 2017

### Features
* Customize the application with your own app icon & publisher details
* Use config file to set up initial URL along with UI options
* Includes an installer used to redistribute your custom application
* Allow user to create desktop shortcuts to their favorite websites, transforming them into desktop applications
* Option to display a built-in toolbar at the top of the window (if the website doesn't have it's own custom designed toolbar)

### Javascript Binding
You can execute certain methods within Rhinoceros via JavaScript in order to manipulate the browser window. To allow JavaScript binding, add the following code to your web page:

```
<script type="text/javascript">
    (async function() {
        await CefSharp.BindObjectAsync("Rhino", "bound");

		//add your code below
		Rhino.Maximize();
    })();
</script>
```

The following methods are supported via JavaScript:

Method|Arguments|Definition
---|---
Rhino.Maximize||Maximize the browser window
Rhino.Minimize||Minimize the browser window
Rhino.Exit||Close the browser window (and application)
Rhino.Toolbar|bool show|Show or hide built-in window toolbar