# Electricity analysis and metering system
---------------------------------------------------------------------------------------------
1. ���������
---------------------------------------------------------------------------------------------
������������ �����������:
>git clone https://github.com/Vinniko/SAUEP.git

�� ������� ������� ���� ������ PostgreSQL, ��������:
>sudo -i -u postgres
>psql
>CREATE DATABASE MyDatabase
>Ctrl+Z

������ ���������� ������� ����� ������:
>pgrestore -U postgres -d MyDatabase -1 ../SAUEPDump.sql

���������� ��������� ����������� � ���� ������ � ���e�� ApiServer.
��� ����� ���������� ������� � ./SAUEP.ApiServer/Connections/Connection.json
������ ����������: 
>{
>  "Host": "localhost",
>  "Username": "postgres",
>  "Password": "root",
>  "Database":  "MyDatabase"
>}

��� ��������� ����� � ����� ��� ������� ����������� ApiServer, �������� � ����� ./SAUEP.ApiServer/Properties/launchSettings.json
� ������� ����: applicationUrl � sslPort.

������ ApiServer. 
�������� � ����� ./SAUEP.ApiServer/ � �������� ������� 
>dotnet publish --configuration Release

����� ���������� �������� � ����� ./bin/Release/netcoreapp3.1 � �������� �������:
>dotnet SAUEP.ApiServer.dll &

����� �������� TcpServer.
�������� ����������� � ���� �� �������� � ApiServer.
�������� � ./SAUEP.TCPServer/Connections/Connection.json
������ ����������: 
>{
>  "Host": "localhost",
>  "Username": "postgres",
>  "Password": "root",
>  "Database":  "MyDatabase"
>}

����� �������� ����� � ���� ��� �������� ����������� TcpServer, ���������� ������� ����./SAUEP.TCPServer/Models/SocketModel.cs 
������ ���������:
>private const int _listenPort = 8005;
>private const int _sayPort = 8003;
>private string _ipAddress = "127.0.0.1";

�������� ����� ������ � ����� ./SAUEP.TCPServer:
>dotnet publish --configuration Release

� �������� ������ �� ����� ./SAUEP.TCPServer/bin/Release/ ��������:
>dotnet SAUEP.TCPServer.dll &

�������� �������� ������ DeviceClient:
�������� ����������� � ����.
�������� � ./SAUEP.DeviceClient/Connections/Connection.json
������ ����������: 
>{
>  "Host": "localhost",
>  "Username": "postgres",
>  "Password": "root",
>  "Database":  "MyDatabase"
>}

����� �������� ����� � ���� ��� �������� ����������� DeviceServer, ���������� ������� ���� ./SAUEP.DeviceClient/Models/SocketModel.cs.
_sayPort ������ ��������� � _listentPort TcpServer.
������ ���������:
>private const int _sayPort = 8005;
>private string _ipAddress = "127.0.0.1";

�������� ����� ������ � ����� ./SAUEP.DeviceClient:
>dotnet publish --configuration Release

� �������� ������ ��������� �� ����� ./SAUEP.DeviceClient/bin/Release/ ��������:
>dotnet SAUEP.DeviceClient.dll &

��������� Desktop-������ ����� ������ ��� Windows. 
�������� ����������� � ApiServer. 
������� ���� ./Core/Connections/Connection.json � ������� ConnectionUrl �� ���� ��� ������� ������� ApiServer. 
��������:
>{
>  "ConnectionUrl": "http://localhost:53773/"
>}

� ����� ./Core/Models/SocketModel.cs
�������� _listenPort � _ipAddress �������:
>private const int _listenPort = 8003;
>private string _ipAddress = "127.0.0.1";

����� ��������� SAUEP.exe

---------------------------------------------------------------------------------------------
2. �����
---------------------------------------------------------------------------------------------

<br> Email: vinnik_21@bk.ru / vinniko333@gmail.com
<br> Telegram: https://t.me/vinnik0
<br> LinkedIn: https://www.linkedin.com/in/�������-������-7450a5208/
<br> HeadHunter: https://spb.hh.ru/resume/f658e91bff090474030039ed1f5a4141446844



