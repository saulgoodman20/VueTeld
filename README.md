# Uruchomienie wersji zbudowanej (WINDOWS/LINUX)
W obu paczkach znajduje się dużo plików, aby ułatwić uruchomienie. Instalacja dotnet nie jest dzięki temu wymagana.

### WINDOWS
1. Pobierz najnowszy release dla windowsa i rozpakuj
2. Przejdź do rozpakowanego folderu dla windowsa i uruchom plik TodoApi.exe
```
C:\Users\PC\Desktop\ToDoList-windows\windows>todoapi.exe
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
      Content root path: C:\Users\PC\Desktop\ToDoList-windows\windows
```
(ewentualnie, gdy aplikacja nadal się nie uruchamia, warto sprawdzić, czy port 5127 jest wolny)

4. We wierszu poleceń (CMD) wykonaj komendę ipconfig i znajdź swój lokalny adres IPv4 w sieci
```
C:\Users\PC>ipconfig
[...]
Wireless LAN adapter Wi-Fi 2:

   Connection-specific DNS Suffix  . :
   Link-local IPv6 Address . . . . . :
   >>> IPv4 Address. . . . . . . . . . . : 192.168.0.167
   Subnet Mask . . . . . . . . . . . :
   Default Gateway . . . . . . . . . :
[...]
```
4. Udaj się do lokalnego adresu URL z portem wskazanym w konsoli TodoApi.exe: `http://192.168.0.167:5127` lub `localhost:5127` - port powinien być taki sam za każdym razem, ale warto się upewnić

### LINUX
1. Pobierz najnowszy release dla linuxa i rozpakuj
2. Zainstaluj net-tools i wykonaj komendę ifconfig oraz zanotuj lokalny adres IPv4 (przykład w WSL: 172.26.50.185)
```
linux@DESKTOP-L9JDK7L:/mnt/c/users/pc/desktop/ToDoList/linux$ sudo apt install net-tools
linux@DESKTOP-L9JDK7L:/mnt/c/users/pc/desktop/ToDoList/linux$ ifconfig
eth0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        >>> inet 172.26.50.185 <<<  netmask 255.255.240.0  broadcast 172.26.63.255
[...]
```
4. Przejdź do rozpakowanego dla linuxa (np. `/mnt/c/users/pc/desktop/ToDoList-linux/linux`)
5. Uruchom TodoApi
```
linux@DESKTOP-L9JDK7L:/mnt/c/users/pc/desktop/ToDoList-linux/linux$ ls -la | grep -i "todoapi"
-rwxrwxrwx 1 root root    75208 Dec 30 09:55 TodoApi
-rwxrwxrwx 1 root root    80814 Dec 30 09:55 TodoApi.deps.json
-rwxrwxrwx 1 root root    28672 Dec 30 09:55 TodoApi.dll
-rwxrwxrwx 1 root root    26400 Dec 30 09:55 TodoApi.pdb
-rwxrwxrwx 1 root root      565 Dec 30 09:55 TodoApi.runtimeconfig.json
-rwxrwxrwx 1 root root   285031 Dec 30 09:33 TodoApi.staticwebassets.endpoints.json
linux@DESKTOP-L9JDK7L:/mnt/c/users/pc/desktop/ToDoList-linux/linux$ ./TodoApi
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (14ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*) FROM "sqlite_master" WHERE "type" = 'table' AND "rootpage" IS NOT NULL;
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5127
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /mnt/c/users/pc/desktop/ToDoList-linux/linux
```
6. Udaj się do zapisanego wcześniej adresu z portem wskazanym w konsoli: `http://172.26.50.185:5127` lub `localhost:5127` - port powinien być taki sam za każdym razem, ale warto się upewnić

# Instalacja oraz uruchomienie wersji deweloperskiej (WINDOWS)
### Wymagania wstępne
- dotnet w wersji minimum 9.0.11
- .NET sdk w wersji 9.0.302 (https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-9.0.308-windows-x64-installer)
- aspnetcore w wersji minimum 9.0.0
- node.js w wersji minimum 20.12
- npm

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
3. Udaj się do URL: `localhost:5127` lub `http://lokalny_adres_ipv4:5127` - port powinien być taki sam za każdym razem, ale warto się upewnić
