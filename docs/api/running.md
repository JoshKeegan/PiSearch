# Running the PiSearch API
Wherever you're running the PiSearch API (from a dev PC through to production), you'll need to get the following set up beforehand . . .  

## PiSearch DB
The PiSearch DB is in MySQL.  
The schema in in `schema.sql` under the API project.  
The connection string mu be made available as configuration to the API, see `appsettings.json` for an example.

## Digits, Suffix Array & Precomputed Results
The API needs data to serve . . .  

You are welcome to use your own data (for any digits, not just Pi). All of the required data files can be generated by `StringSearchConsole.exe`.  
TODO: document this process.

If you just want to serve the same data as the live API though, it can be downloaded from [TODO](TODO)

## (Optional): Logs DB
The API uses Serilog, and has the MySQL sink installed.  
If you want to make use of this, make an empty database and enter the connection string into your `appsettings.{Environment Name}.json`.
See `appsettings.json` for an example.