In this technical task, I used **ASP.NET Core Minimal API** project and an inmemory database to keep things simple.

The project defines two endpoints: 
1. **POST** - receives CVS file with user information defined as follows: id(GUID),username,age,city,phone,email. The data is parsed and persisted to the database. If the record is already on the database, it is updated.
1. **GET** - recieves Order and Count parameters. Sorts users in Order direction (asc, desc) and returns Count number of them.
