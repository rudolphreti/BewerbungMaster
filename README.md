# BewerbungMaster

BewerbungMaster is a Blazor web application designed to efficiently manage job applications. This application allows users to add, display, and delete job applications, as well as generate related documents such as CVs and cover letters.

This project is an evolution of an older Python-based project, which can be found at: https://github.com/rudolphreti/autobewerbung

## Features

- Job application management (add, display, delete)
- Document generation (CVs, cover letters)
- Initially, job application documents are generated from JSON data

## To-Do

- Implement CRUD operations
- Add functionality for sending messages to employers

## Project Structure

```
BewerbungMasterApp/
├── wwwroot/
│   ├── bootstrap/
│   ├── css/
│   ├── JobAppDocs/
│   ├── Users/
│   ├── app.css
│   ├── data.json
│   └── favicon.png
├── Services/
│   ├── FileManagementService.cs
│   ├── FileManagementServiceStatic.cs
│   ├── GetJobApplicationServices.cs
│   ├── GetUserData.cs
│   └── PdfGenerationService.cs
└── ... (other directories)
```

### wwwroot Directory
- `bootstrap/`: Contains Bootstrap framework files for styling
- `css/`: Additional CSS files for custom styling
- `JobAppDocs/`: Stores generated job application documents
- `Users/`: Contains user-specific document templates
- `app.css`: Application-wide CSS styles
- `data.json`: Stores job application data in JSON format
- `favicon.png`: Application favicon

### Services
The `Services` directory contains the core business logic of the application:
- `FileManagementService.cs`: Handles file operations for job application documents
- `FileManagementServiceStatic.cs`: Provides static file management utilities
- `GetJobApplicationServices.cs`: Retrieves job application data
- `GetUserData.cs`: Manages user data retrieval
- `PdfGenerationService.cs`: Generates PDF documents for job applications

#### PdfGenerationService.cs
This service is responsible for generating PDF documents for job applications. It utilizes the iText7 library to create professional-looking PDFs with advanced features:

1. **iText7 Integration**: The service leverages iText7, a powerful PDF manipulation library, to create and modify PDF documents programmatically.

2. **Hyperlink Detection**: The service includes functionality to automatically detect URLs within the text content. When a URL is identified, it's converted into an active hyperlink in the generated PDF, enhancing the interactivity of the document.

3. **Syllable Hyphenation**: To improve the readability and layout of the generated PDFs, the service implements syllable hyphenation. This feature intelligently breaks words at appropriate syllable boundaries when necessary, ensuring a clean and professional appearance of the text, especially in narrow columns or justified text alignments.

4. **Dynamic Content Placement**: The service can dynamically position text, images, and other elements within the PDF, allowing for flexible and customizable document layouts.

5. **Font Management**: It handles various font styles and sizes, enabling rich text formatting in the generated PDFs.

These advanced features of the PdfGenerationService.cs contribute to creating polished, professional-looking job application documents that stand out to potential employers.

## Installation

Currently, only the source code is available. To set up the project:

1. Clone the repository
2. Open the solution in Visual Studio or your preferred IDE
3. Restore NuGet packages
4. Build the project

## Usage

1. Provide job data in the `data.json` file located in the `wwwroot` directory. Include the following information for each job application:
   - URL
   - Position
   - Company

2. You can create your own documents in the `Users` directory within `wwwroot`. Make sure to indicate this directory in the `appsetings.Development.json` file.

3. Run the application and use the interface to manage your job applications and generate documents.

## Dependencies

This project uses the following main dependencies:

- .NET 8.0
- itext7 (v8.0.5)
- itext7.bouncy-castle-adapter (v8.0.5)
- itext7.commons (v8.0.5)
- itext7.hyph (v8.0.5)

For a complete list of dependencies, please refer to the `BewerbungMasterApp.csproj` file.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

If you encounter any problems or have any questions, please open an issue in the GitHub repository.
