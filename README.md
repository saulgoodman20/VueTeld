# Wymagania wstępne
1. dotnet w wersji minimum 9.0.11
2. node.js i npm
```
C:\Users\PC>dotnet --version
9.0.302

C:\Users\PC>node -v
v22.14.0

C:\Users\PC>npm -v
10.9.2
```

# Uruchomienie wersji zbudowanej
1. Pobierz najnowszy release
2. Upewnij się, że masz zainstalowany dotnet w wersji 9.0.11
```
CMD:
C:\Users\PC>dotnet --version
9.0.302
```
2a. Jeżeli nie masz wersji przynajmniej 9.0.11, aplikacja może się nie uruchomić. W takim przypadku należy pobrać i zainstalować tą wersję:

https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-9.0.11-windows-x64-installer?cid=getdotnetcore

3. Po ukończonej pomyślnie weryfikacji, udaj się do folderu ToDoList i uruchom plik TodoApi.exe
```
C:\Users\PC\Desktop\ToDoList>todoapi.exe
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (5ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*) FROM "sqlite_master" WHERE "type" = 'table' AND "rootpage" IS NOT NULL;
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5127
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\PC\Desktop\ToDoList
```
(ewentualnie, gdy aplikacja nadal się nie uruchamia, warto sprawdzić, czy port 5127 jest wolny)

4. Udaj się do wskazanego w konsoli URL: `http://0.0.0.0:5127` - powinien być taki sam za każdym razem, ale warto się upewnić

# Instalacja oraz uruchomienie wersji deweloperskiej
1. Sklonuj repozytorium do folderu lokalnego `git clone https://github.com/saulgoodman20/VueTeld.git`
2. W folderze docelowym (np. `C:/Users/PC/VueTeld/`) uruchom 2 terminale powershell/cmd oraz wykonaj następujące komendy:

Terminal numer 1:
```
PS C:\Users\PC\VueTeld> cd .\TodoFront\
PS C:\Users\PC\VueTeld\TodoFront> npm install

added 113 packages, and audited 114 packages in 3s

22 packages are looking for funding
  run `npm fund` for details

found 0 vulnerabilities
PS C:\Users\PC\VueTeld\TodoFront> npm run dev

> TodoFront@0.0.0 dev
> vite


  VITE v7.3.0  ready in 332 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

Terminal numer 2:
```
PS C:\Users\PC\VueTeld> cd .\TodoApi\
PS C:\Users\PC\VueTeld\TodoApi> dotnet run
Używanie ustawień uruchamiania z profilu C:\Users\PC\VueTeld\TodoApi\Properties\launchSettings.json...
Trwa kompilowanie...
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (5ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*) FROM "sqlite_master" WHERE "type" = 'table' AND "rootpage" IS NOT NULL;
warn: Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware[16]
      The WebRootPath was not found: C:\Users\PC\VueTeld\TodoApi\wwwroot. Static files may be unavailable.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5127
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\PC\VueTeld\TodoApi
```
3. Udaj się do wskazanego w konsoli URL: `http://0.0.0.0:5127` - powinien być taki sam za każdym razem, ale warto się upewnić
