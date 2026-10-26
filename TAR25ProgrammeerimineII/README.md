# Raamatukogu infosüsteem (EF Core)

ASP.NET Core MVC + Entity Framework Core (SQL Server LocalDB).

## Projekti kihid
- `ShopTARpe25.Core` – domeeniklassid (Book, Reader, Loan), DTO-d, teenuse liides `IBookServices`
- `ShopTARpe25.Data` – `ShopTARpe25Context` (DbContext)
- `ShopTARpe25.ApplicationServices` – `LibaryService` (kõik LINQ päringud ja andmete muutmine)
- `ShopTARpe25` – MVC kontrollerid ja vaated

## Käivitamine
1. Package Manager Console (Default project: `ShopTARpe25.Data`, Startup project: `ShopTARpe25`):
   `Add-Migration Init` ja `Update-Database`
2. Käivita rakendus, kõik tegevused on tehtavad menüüst.

## Ülesanne 6. Vastused

**Milline on DbContext-i ülesanne?**
DbContext on ühendus C# objektide ja andmebaasi vahel. Ta hoiab andmebaasiühendust, teab tabeleid (DbSet), jälgib objektides tehtud muudatusi (change tracking) ja kirjutab need `SaveChanges()` abil andmebaasi. Samuti tõlgib ta LINQ päringud SQL-iks.

**Miks ei ole hea paigutada kõiki andmebaasipäringuid otse kasutajaliidese koodi?**
- Kood läheb segaseks: kasutajaliides ja andmeloogika on ühes kohas ning üht ja sama päringut korratakse mitmes kohas.
- Muutmine on raske: kui andmebaas või päring muutub, tuleb parandada kogu liidest.
- Testida on keeruline, sest kasutajaliidest ei saa testida ilma päris andmebaasita.
- Valideerimine ja õiguste kontroll jäävad hajusaks, mis suurendab vigade ja turvaaukude ohtu.
- See rikub vastutuste lahususe põhimõtet (separation of concerns).

**Millisesse kihti paigutaksid andmebaasiga suhtlemise MVC- või VIPER-arhitektuuris?**
MVC-s paigutaksin selle **Model** kihti (teenuste klassidesse, siin `LibaryService` ja `ShopTARpe25Context`), mitte Controllerisse ega View'sse. Controller küsib andmed teenuse liidese (`IBookServices`) kaudu ja annab need View'le. VIPER-is läheks see **Interactor**'i alla (äriloogika ja andmete hankimine), mitte View'sse ega Presenter'isse. Põhjus: andmetega töötamine on äriloogika osa, nii saab kasutajaliidest muuta ilma andmeloogikat puutumata ja vastupidi.
