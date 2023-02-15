# MyHotels

Sample project showing how to use WebAPI in ASP.NET Core.

To start the project:

- recreate the database (`dotnet ef database update`),
- define environment variable storing random key for the JWT token signing (`$Env:Jwt__Key='SOME_GUID'`) to generate a random key you might use [online GUID generator](https://guidgenerator.com/).
