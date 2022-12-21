# AirStack

## Klient
Aplikace slouží jako uživatelský vstup do systému AirStack. Data příchozí na seriový port jsou odesílána na API a výsledek požadavku je následně zobrazen.
<br>//TODO: obrázky jak vypadá result requestů + error hlášky

### Režimy aplikace
Stavy Testy a Expedice se v systému přiřazují na základě triggeru v databázi, aplikace má tuhle funkčnost jen pro krajní případy, kdy trigger nelze využít

#### Vstup do produkce
Uživatel skenuje SN airbagu
#### Testy
Uživatel skenuje SN dílu na kterém se airbag nachází
#### Expedice
Uživatel skenuje SN dílu na kterém se airbag nachází
#### Reklamace od zákazníka
Uživatel skenuje SN dílu na kterém se airbag nachází
#### Reklamace dodavateli
Uživatel skenuje SN airbagu

### Nastavení
V adresáři aplikace se nachází settings.json, který obsahuje veškeré nastavení. Hodnoty je možno editovat v klientu nebo jako soubor, ovšem režim aplikace
je nutno nastavit v souboru, parametr **AppMode**.
<br>Hodnoty:
- 1 (Vstup do produkce)
- 2 (Testy)
- 3 (Expedice/Prodáno)
- 4 (reklamace od zákazníka)
- 5 (reklamace dodavateli)

## Regulární výrazy pro airbag
Nastavují se v databázi v tabulce dbo.Settings, jakýkoliv záznam s **Name** hodnotou začínající **CodeRegex_**, tak **Value** hodnota je považována jako regex pro airbag.
<br>Je možno definovat více regexů.
<br><br>![Settings_Regex](https://user-images.githubusercontent.com/59573257/208864854-40206980-cf48-49a0-8d5c-ca811badbfa0.png)

Není-li uveden žádný regex, validace se neprovádí.<br>
Po editaci hodnot regexů je potřeba provést znovunačtení na API běžící na serveru, toho se docílí buď restartem API nebo dotazem na <br>"adresa api"/reload:
<br><br>![ReloadEndpoint](https://user-images.githubusercontent.com/59573257/208865786-14fbc2dc-cd81-4b9b-b453-e9caae1794ad.png)
