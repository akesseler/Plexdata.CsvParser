## Project Build

Best way to build the whole project is to use _Visual Studio 2017 Community_. Thereafter, download the complete sources, open the solution file ``Plexdata.CsvParser.NET.sln``, switch to release and rebuild all.

## Help Generation

The help file of type CHM is generated during release build process only. For this purpose the ``MSBuild.exe`` is used. The help configuration file ``Plexdata.CsvParser.NET.shfbproj`` has been made using [Sandcastle Help File Builder](https://ewsoftware.github.io/SHFB/html/bd1ddb51-1c4f-434f-bb1a-ce2135d3a909.htm). The final help file with name ``Plexdata.CsvParser.NET.chm`` is automatically put into the release sub-folder after a successful build.

You can disable the help file generation, if you like, by opening the _Project Settings_ and moving to tab _Build Events_. There you simply clear out the content of box _Post-build event command line_.

## Trouble Shooting

If you get an error during release build, you may need to install the _Sandcastle Help File Builder_ manually and edit the help configuration file ``Plexdata.CsvParser.NET.shfbproj``.

On the other hand, if you get an error that states something like ``MSBuild.exe not found``, then you may need to correct the path to file ``MSBuild.exe`` inside file ``post-build.cmd``.
