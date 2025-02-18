# Web Scraper to Markdown Converter

This project is a C# console application that scrapes content from a list of URLs specified in an Excel file, converts the HTML content to Markdown, and saves the Markdown files to a specified directory structure. The application uses the `ClosedXML` library to read Excel files, `HtmlAgilityPack` for web scraping, and `ReverseMarkdown` for converting HTML to Markdown.

## Features

- **Excel Integration**: Reads URLs and corresponding folder paths from an Excel file.
- **Web Scraping**: Scrapes the main content of web pages.
- **HTML to Markdown Conversion**: Converts the scraped HTML content to Markdown format.
- **Parallel Processing**: Uses `Parallel.ForEachAsync` to process multiple URLs concurrently.
- **File Organization**: Saves Markdown files in a structured directory based on the folder paths specified in the Excel file.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [ClosedXML](https://github.com/ClosedXML/ClosedXML) (for Excel file handling)
- [HtmlAgilityPack](https://html-agility-pack.net/) (for web scraping)
- [ReverseMarkdown](https://github.com/mysticmind/reversemarkdown-net) (for HTML to Markdown conversion)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/web-scraper-markdown-converter.git
   cd web-scraper-markdown-converter
   ```

2. Install the required NuGet packages:
   ```bash
   dotnet add package ClosedXML
   dotnet add package HtmlAgilityPack
   dotnet add package ReverseMarkdown
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

## Usage

1. **Prepare the Excel File**:
   - Create an Excel file named `links.xlsx` in the root directory.
   - The first column should contain the URLs, and the second column should contain the folder paths where the Markdown files will be saved.
   - Example:

     | URL                                      | Folder Path          |
     |------------------------------------------|----------------------|
     | https://example.com/page1                | folder1/subfolder1   |
     | https://example.com/page2                | folder2/subfolder2   |

2. **Run the Application**:
   ```bash
   dotnet run
   ```

3. **Output**:
   - The application will create a directory named `output` in the root directory.
   - Markdown files will be saved in the specified folder structure within the `output` directory.

## Code Overview

- **Program.cs**: The main entry point of the application.
  - Reads URLs and folder paths from the Excel file.
  - Scrapes the content from the URLs.
  - Converts the HTML content to Markdown.
  - Saves the Markdown files to the specified directory structure.

- **Excel File Handling**: Uses `ClosedXML` to read the Excel file.
- **Web Scraping**: Uses `HtmlAgilityPack` to scrape the main content of web pages.
- **HTML to Markdown Conversion**: Uses `ReverseMarkdown` to convert HTML to Markdown.

## Example

Given the following Excel file:

| URL                                      | Folder Path          |
|------------------------------------------|----------------------|
| https://example.com/page1                | folder1/subfolder1   |
| https://example.com/page2                | folder2/subfolder2   |

The application will create the following directory structure:

```
output/
├── folder1/
│   └── subfolder1/
│       └── page1.md
└── folder2/
    └── subfolder2/
        └── page2.md
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [ClosedXML](https://github.com/ClosedXML/ClosedXML) for Excel file handling.
- [HtmlAgilityPack](https://html-agility-pack.net/) for web scraping.
- [ReverseMarkdown](https://github.com/mysticmind/reversemarkdown-net) for HTML to Markdown conversion.

---

Feel free to customize this README to better fit your project's needs!
