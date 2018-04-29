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

### JavaScript Binding
You can execute certain methods within Rhinoceros via JavaScript in order to manipulate the browser window. To allow JavaScript binding, add the following code to your web page:

```
<script type="text/javascript">
    (async function() {
        await CefSharp.BindObjectAsync("Rhino", "bound");

		//add your code below (all C# methods must be lowercase)
		Rhino.maximize();
    })();
</script>
```

The following methods are supported via JavaScript:

Method|Arguments|Definition
---|---
Rhino.maximize||Maximize the browser window
Rhino.normalize||Bring window out of maximize or minimize state
Rhino.minimize||Minimize the browser window
Rhino.drag||Drags the browser window (use on mouse down event only)
Rhino.usetoolbar||Show built-in window toolbar
Rhino.bordersize|size|Changes the thickness of the window border (in pixels)
Rhino.bordercolor|r,g,b|Changes the color of the window border
Rhino.toolbarcolor|r,g,b|Changes the background color of the window toolbar (if displayed)
Rhino.toolbarfontcolor|r,g,b|Changes the font & icon colors of the window toolbar (if displayed)
Rhino.defaulttheme||Changes the theme styling back to default (border & toolbar colors)
Rhino.exit||Close the browser window (and application)