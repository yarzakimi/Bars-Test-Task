Задание
=====================
Разработать десктоп(консольное) приложение, которое определяет размер баз данных PostgreSQL на диске (размер можно получить через SQL запрос). К приложению необходимо создать файл конфигурации, в котором определяется строка подключения к БД  и определяется строка подключения к выбранному аккаунту Google. Результатом работы приложения должно быть создание/обновление документа в Google Tables. 
***
На каждый сервер свой лист. К каждому серверу в файле конфигурации должна быть своя строка подключения. Кроме того в конфигурации должен задаваться размер дисков в ГБ и последней строкой выводить  на листе в Google Tables.
***
После обновления приложение должно ожидать заданный промежуток времени, после чего повторять действия.

Использование файла конфигураций App.config
=====================
----
### Строка подключения к серверу
##### Оформляется внутри секции ***connectionStrings*** в следующем формате:
    <add name = "Server Name" connectionString = "Connection string for PostgreSQL" providerName = "Npgsql" />
#### Пример:
    <connectionStrings>
      <add name = "LocalServer" connectionString = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=1234;Database=postgres;" providerName = "Npgsql" />
    </connectionStrings>
##### Также необходимо добавить размер диска уже в секцию ***appSettings*** следующим образом:
    <add key="Server Name" value="DiskSize"/>
#### Пример:
    <appSettings>
      <add key="LocalServer" value="85"/> 
    </appSettings>
----   
### Строка подключения Google аккаунта
##### Данные берутся из json файла с [Google Sheets API](https://developers.google.com/sheets/api/quickstart/dotnet). В файле конфигураций данные оформляются внутри секции ***appSettings*** следующим образом:
    <add key="credentials" value='{"Client Secret"}'/>
#### Пример:
    <appSettings>
      <add key="credentials" value='{"installed":{"client_id":"524349200653-0ac3dsbom60rea39humqp053j9kfivdt.apps.googleusercontent.com","project_id":"testtaskbars-1539956070553","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://www.googleapis.com/oauth2/v3/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_secret":"VJK_5yn-cGbOD3HWvAWnhqit","redirect_uris":["urn:ietf:wg:oauth:2.0:oob","http://localhost"]}}'/>
    </appSettings>
----
### Строка подключения Google Spreadsheet
##### Необходимо указать два параметра: ***ReaderRange*** и ***SheetId***. Причём ***SheetId*** необязательный, так как при отсутствии этого параметра будет создана новая таблица со своим идентификатором, а ***SheetId*** автоматически занесётся в файл конфигурации. Параметры оформляются внутри секции ***appSettings*** следующим образом:
    <add key="SheetId" value="Spreadsheet ID" />
    <add key="ReaderRange" value="Range Value" />
#### Пример:
    <appSettings>        
        <add key="SheetId" value="1lG8Jad9hhhPZCrSe13abW-g3clKbRirP2Jvn0Cx_sFs" />
        <add key="ReaderRange" value="A1:J" />        
      </appSettings>
