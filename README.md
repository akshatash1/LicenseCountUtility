# AppLicenseCalculator

The project has been built using .NET 6.0.

To run the application, navigate to the Executable folder and locate the executable file named AppLicenseCalculator.exe.

1. **Execution:** Run this file by supplying the path of the input file as the first argument.
AppLicenseCalculator.exe "input file path"
2. **No Argument:** If no argument is given, the console will prompt you to input the file path.
3. **Null Filename:** If the filename is null, an error will occur.
4. **File Format:** The utility only supports CSV format. If the file is not in CSV format, please restart the application with a valid input.
5. **Post Execution:** After execution, press any key to close the screen.


Special Considerations made in this project are:
--------------------------------------
1. Input file is a valid csv file with header as ComputerID,UserID,ApplicationID,ComputerType,Comment.
2. There should be no spaces before or after the values in the CSV file.
3. The ComputerType will either have ‘Laptop’ or ‘Desktop’ as values (case-insensitive). Any other type will not be considered in the License count.
4. If a user has installed applications only on laptops, then each laptop will contribute to the total number of required copies. 
5. Utility has hardcoded value for application ID in Configuration file as 374.

Logic Used:
--------------------------
1. If a user has installed apps only on a desktop, the count is incremented based on number of desktops.
2. If a user has installed apps only on a laptop, the count is incremented based on number of laptops.
3. If a user has installed apps on multiple desktops and laptops, the count is incremented based on the fact that a user can use one license on one desktop and one laptop.
