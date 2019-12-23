## Overview

The help file project named `Plexdata.CsvParser.NET.help.shfbproj` has been created using [Sandcastle Help File Builder](https://ewsoftware.github.io/SHFB/html/bd1ddb51-1c4f-434f-bb1a-ce2135d3a909.htm) version v2018.7.8.0.

## Building the Help

Usually, there should be no need to change this file because of the CHM help file is automatically created during release build of the project sources via `MSBuild.exe`. But if you like, you can download _Sandcastle Help File Builder_ from [https://github.com/EWSoftware/SHFB/releases](https://github.com/EWSoftware/SHFB/releases) and modify the project help fitting your own needs.

For example you may like to create an HTML version of the project API documentation. In such a case just download and install the _Sandcastle Help File Builder_ as mentioned above. Then follow the steps below:

- As first, you should create a copy of the help file project `Plexdata.CsvParser.NET.help.shfbproj` and rename it, for instance, into `Plexdata.CsvParser.NET.html.shfbproj`. 
- As next, open this new file with the _Sandcastle Help File Builder_.
- Thereafter, show tab _Project Properties_ and move to section _Build_. 
- Now un-tick the _HTML help 1 (chm)_ setting and tick the setting _Website (HTML/ASP.NET)_ instead.
- Then you should change the output path accordingly. For this purpose move to section _Paths_ and modify the value of box _Help content output path_. For example you could change this path into `html\`. 
- Finally, rebuild the whole help.

After a successful build you will find the result inside the project file’s sub-folder you named above.
