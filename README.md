# DEVLCS 
# Version 1.0.13 
First bug, sorry for this, wasn't testing enough, if you try to change the default image for a MS hosted environment it will crash badly, fixed in this release

# Version 1.0.12
Added region support, only able to test United States and Europe but I assume it will work for other regions to. Removed functions not supported today by LCS (this was functions before the self service LCS and didn't work anymore). Cleaned upp the UI a little and fixed some bugs i found. 

*Note! un-install the old version, clear the settings from the C:\Users\<yourname>\AppData\Local\DevMethod folder*

# Version 1.0.8
This version has a fix for the missing values in the filter dialog, it will also make it possible to have individual filters on each carousell. The rotation increment can be set to a fixed value.

# Version 1.0.6
This version will handle both Self Service LCS and old style LCS (if there is any more around). 

** Important **
Uninstall the previous version and remove the folder DevMethod folder from your users appdata folder "Users\<your user>\AppData\Local\DevMethod", then install the new version

# About DEVLCS
This app is intended for developers to help them to manage vm's in a simple way. This application is based on the work of **Tomek Melissa** with the 2LCS app. https://github.com/microsoft/2LCS. It is also using the carousell control created by **Leif Simon Goodwin** https://www.codeproject.com/Articles/4051491/A-Custom-WPF-Carousel-Control 

Feel free to download and contribute. The UI is based on MahApps.Metro (great library!) and ControlzEx components.

You will find some info on how to use it on my blog https://memyselfandax.wordpress.com/the-purpose-of-devlcs/

# Visual Studio 2019
This is built with visual studio 2019, use nuget to get the referenced librarys used by this app.
The solution also consist of a setup program that let you easily distribute the app within your project. To be able to load the setup program in Visual Studio the Extension "Microsoft Visual Studio Installer Project" must be installed first.

# No support
Use this with care, I have written this for my own use in my spare time, it is provided AS IS and it can break anytime because of changes introduced in LCS or by me or anyone else, it is your responsibility to test the functions before you use it on a environment that is important for you. As I think this a useful tool, I want to help and share it with anybody who needs it.

# More cool tools
Visit DevMethod's site and check out the DevMethod tools for Azure TFVC projects: https://devmethod.azurewebsites.net

/Hasse
