# Electricity analysis and metering system
---------------------------------------------------------------------------------------------
1. Установка
---------------------------------------------------------------------------------------------
Склонировать репозиторий:
>git clone https://github.com/Vinniko/SAUEP.git

На сервере создать базу данных PostgreSQL, например:
>sudo -i -u postgres
>psql
>CREATE DATABASE MyDatabase

Теперь необходимо поднять схему данных:
>pgrestore -U postgres -d MyDatabase -1 ../SAUEPDump.sql

Необходимо настроить подключение к базе данных в модeле ApiServer.
Для этого необходимо перейти в ./SAUEP.ApiServer/Connections
И выполнить команду:
>cp Connection.json.example Connection.json

Пример заполнения: 
>{
>  "Host": "localhost",
>  "Username": "postgres",
>  "Password": "root",
>  "Database":  "MyDatabase"
>}

Для изменения хоста и порта под которым запускается ApiServer, перейдем в папку ./SAUEP.ApiServer/Properties/launchSettings.json
и изменим поля: applicationUrl и sslPort.

Запуск ApiServer. 
Перейдем в папку ./SAUEP.ApiServer/ и выполним команду 
>dotnet publish --configuration Release

После выполнения перейдем в папку ./bin/Release/netcoreapp3.1 и выполним команду:
>dotnet SAUEP.ApiServer.dll &

Далее настроим TcpServer.
Настроим подключение к базе по аналогии с ApiServer.
Перейдем в ./SAUEP.TCPServer/Connections/Connection.json
Пример заполнения: 
>{
>  "Host": "localhost",
>  "Username": "postgres",
>  "Password": "root",
>  "Database":  "MyDatabase"
>}

Чтобы поменять порты и хост под которыми запускается TcpServer, необходимо открыть файл./SAUEP.TCPServer/Models/SocketModel.cs 
Пример настройки:
>private const int _listenPort = 8005;
>private const int _sayPort = 8003;
>private string _ipAddress = "127.0.0.1";

Создадим релиз версию в папке ./SAUEP.TCPServer:
>dotnet publish --configuration Release

И запустим сервер из папки ./SAUEP.TCPServer/bin/Release/ командой:
>dotnet SAUEP.TCPServer.dll &

Настроим тестовую версию DeviceClient:
Настроим подключение к базе.
Перейдем в ./SAUEP.DeviceClient/Connections
И выполним команду:
>cp Connection.json.example Connection.json

Пример заполнения: 
>{
>  "Host": "localhost",
>  "Username": "postgres",
>  "Password": "root",
>  "Database":  "MyDatabase"
>}

Чтобы поменять порт и хост под которыми запускается DeviceServer, необходимо открыть ./SAUEP.DeviceClient/Configs
И выполнить команду:
>cp Config.json.example Config.json

После необходимо настроить конфиг. Например:
>{ 
>   "Ip": "127.0.0.1", 
>   "Port": 8005
>}

Создадим релиз версию в папке ./SAUEP.DeviceClient:
>dotnet publish --configuration Release

И запустим клиент устройств из папки ./SAUEP.DeviceClient/bin/Release/ командой:
>dotnet SAUEP.DeviceClient.dll &

Запустить Desktop-клиент можно только под Windows. 
Настроим подключение к ApiServer. 
Откроем файл ./Core/Connections/Connection.json и изменим ConnectionUrl на хост под которым запущен ApiServer. 
Например:
>{
>  "ConnectionUrl": "http://localhost:53773/"
>}

В файле ./Core/Models/SocketModel.cs
Настроим _listenPort и _ipAddress сервера:
>private const int _listenPort = 8003;
>private string _ipAddress = "127.0.0.1";

Можно запускать SAUEP.exe

---------------------------------------------------------------------------------------------
2. Связь
---------------------------------------------------------------------------------------------

<br> Email: vinnik_21@bk.ru / vinniko333@gmail.com
<br> Telegram: https://t.me/vinnik0
<br> LinkedIn: https://www.linkedin.com/in/алексей-винник-7450a5208/
<br> HeadHunter: https://spb.hh.ru/resume/f658e91bff090474030039ed1f5a4141446844



