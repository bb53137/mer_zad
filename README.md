# Upravljanje korisnicima demo verzija projekt

Ovaj projekt je demo verzija modula za upravljanje velikim brojem korisnika (preko 100000 zapisa), izrađena pomoću .NET 8 Web API-ja i Angulara.

## Pokretanje projekta

Backend:

1. Otvoriti terminal i pozicionirati se u folder:
   cd backend/CustomerApi  
2. Pokrenuti aplikaciju:
   dotnet restore  
   dotnet run  

Pri prvom pokretanju baza se automatski kreira i generira se 100000 korisnika.

Frontend:

1. Otvoriti drugi terminal i pozicionirati se u folder:
   cd frontend/customer-ui  
2. Instalirati pakete:
   npm install  
3. Pokrenuti aplikaciju:
   ng serve  

Aplikacija je dostupna na http://localhost:4200

## Kako sam radila zad

Korištena je SQLite baza podataka zbog jednostavnog pokrenutanja bez dodatne konfiguracije. Za rad s bazom korišten je Entity Framework Core. Business logika je izdvojena u service layer da kontroleri ostanu što jednostavniji. Server side pagination list implementirana je zbog velikog broja zapisa da izbjegnemo učitavanje svih podataka.

## Što bih poboljšala

Dodala bih toast notifikacije umjesto alert poruka.
Implementirala bih pravi loading spinner, sad imam samo tekstualni prikaz.
Uredila bi layout i malo više vremena odvojila za estetiku fronta.
