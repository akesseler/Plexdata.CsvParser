<p align="center">
  <a href="https://github.com/akesseler/Plexdata.CsvParser/blob/master/LICENSE.md" alt="license">
    <img src="https://img.shields.io/github/license/akesseler/Plexdata.CsvParser.svg" />
  </a>
  <a href="https://github.com/akesseler/Plexdata.CsvParser/releases/latest" alt="latest">
    <img src="https://img.shields.io/github/release/akesseler/Plexdata.CsvParser.svg" />
  </a>
  <a href="https://github.com/akesseler/Plexdata.CsvParser/archive/master.zip" alt="master">
    <img src="https://img.shields.io/github/languages/code-size/akesseler/Plexdata.CsvParser.svg" />
  </a>
  <a href="https://akesseler.github.io/Plexdata.CsvParser" alt="docs">
    <img src="https://img.shields.io/badge/docs-guide-orange.svg" />
  </a>
  <a href="https://github.com/akesseler/Plexdata.CsvParser/wiki" alt="wiki">
    <img src="https://img.shields.io/badge/wiki-API-orange.svg" />
  </a>
</p>

## Plexdata CSV Parser

The _Plexdata CSV Parser_ represents a library that allows importing and exporting of CSV files based on a user-defined class. Further, it is possible to configure the importing and exporting behavior.

Main feature of this library is that users only need to create an own class representing a single line of a CSV file. Each property that has to be included in the CSV processing should be tagged by a proper CSV attribute, which can be obtained from this library. Thereafter, this custom class can be used either together with the importer or together with the exporter to process CSV files.

Finally, it would also be possible (assuming a proper configuration is used) to write a CSV output according to the rules of RFC 4180. For more information about RFC 4180 please visit the web-site under [https://www.ietf.org/rfc/rfc4180.txt](https://www.ietf.org/rfc/rfc4180.txt).

The software has been published under the terms of _MIT License_.

For an introduction see the Docs under [https://akesseler.github.io/Plexdata.CsvParser/](https://akesseler.github.io/Plexdata.CsvParser/).


