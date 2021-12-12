## Usage

Using the converter is as simple as launching the app and providing a path to a valid definition file.

### Definition file

The definition file specifies in which directory the scan files are located as well as the mapping between a sample and the scan numbers.

1. The first line must be an existing directory. You can surround it with double quotes, but it's not necessary.
2. All following lines must follow the format `[SAMPLE NAME] [SCAN NUMBERS] [KINETIC ENERGY (optional)]`. At least one line is required and any empty lines are ignored.

   1. The scan numbers should correspond to the numeric part of the filename, i.e. `i09-[SCAN NUMBER].nxs`.  
   2. You can specify scan number ranges by separating the first and last scans using a hyphen. Either single numbers or ranges must be delimited by a comma.  
   3. The optional kinetic energy field should be given in `eV`.

Example of a valid definition file:

```
"C:\path\to\scans\directory"

Sample1 100000,100002,100004-100010 200
Sample1 100300-100305,100308,100311-100315 400
Sample1 100400,100403

Sample2 100500,100505,100511-100514 200
Sample2 100600-100610,100612,100614-100620 400
```
All fields are delimited by a space character, so they themselves cannot contain any spaces.