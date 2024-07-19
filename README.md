# AppLicenseCalculator

# .NET 6 Console Application

This repository contains a .NET 6 console application.

## Prerequisites

- .NET 6 SDK

## Getting Started

Follow these steps to get the project up and running:

1. **Clone the Repository**: Clone this repository to your local machine. You can use the following command:
    ```
    git clone <repository-url>
    ```
    Replace `<repository-url>` with the URL of this repository.

2. **Navigate to the Project Directory**: Use the command line to navigate into the root directory of the project.

3. **Build the Project**: Run the following command in the terminal to build the project:
    ```
    dotnet build
    ```
    This command compiles the code and checks for any syntax errors.

4. **Run the Application**: After a successful build, run the application with the following command:
    ```
    dotnet run
    ```
    This command runs the application in the console. Follow the prompts in the console to interact with the application.

5. **Execution**: Run this file by supplying the path of the input file as the first argument.
    ```
    AppLicenseCalculator.exe "input file path"
    ```
6. **No Argument**: If no argument is given, the console will prompt you to input the file path.
7. **Null Filename**: If the filename is null, an error will occur.
8. **File Format**: The utility only supports CSV format. If the file is not in CSV format, please restart the application with a valid input.
9. **Post Execution**: After execution, press any key to close the screen.

Particular considerations taken into account in this project include:
-------------------------------------------------------------------------
1. Input file is a valid csv file with header as ComputerID,UserID,ApplicationID,ComputerType,Comment.
2. There should be no spaces before or after the values in the CSV file.
3. The ComputerType will either have ‘Laptop’ or ‘Desktop’ as values (case-insensitive). Any other type will not be considered in the License count.
4. If a user has installed applications only on laptops, then each laptop will contribute to the total number of required copies. 
5. Utility has hardcoded value for application ID in Configuration file as 374.

Implemented Logic:
--------------------------
1. If a user has installed apps only on a desktop, the count is incremented based on number of desktops.
2. If a user has installed apps only on a laptop, the count is incremented based on number of laptops.
3. If a user has installed apps on multiple desktops and laptops, the count is incremented based on the fact that a user can use one license on one desktop and one laptop.
