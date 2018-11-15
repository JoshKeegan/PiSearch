# Hosting
This is a quick setup guide for an instance of the PiSearch API.  
It is hosted on Debian Linux (version 9/Stretch) using apache2 as a reverse proxy to pass requests on to Kestrel running as a service listening on port 5000.

## Pre-reqs
apache2 and dotnet must be installed.

## User
```bash
adduser pisearch
```

## Directories
```bash
mkdir /var/www/v2.api.pisearch.joshkeegan.co.uk
chown pisearch /var/www/v2.api.pisearch.joshkeegan.co.uk
```

## Apache
### Mods
```bash
a2enmod headers
a2enmod proxy
a2enmod proxy_http
```

### Config
Create a config file at `/etc.apache2/sites-available/v2.api.pisearch.joshkeegan.co.uk.conf` containing:
```
<VirtualHost *:*>
    RequestHeader set "X-Forwarded-Proto" expr=%{REQUEST_SCHEME}
</VirtualHost>

<VirtualHost *:80>
    ProxyPreserveHost On
    ProxyPass / http://127.0.0.1:5000/
    ProxyPassReverse / http://127.0.0.1:5000/
    ServerName v2.api.pisearch.joshkeegan.co.uk
    ErrorLog ${APACHE_LOG_DIR}/pisearch-error.log
    CustomLog ${APACHE_LOG_DIR}/pisearch-access.log common
</VirtualHost>
```

Symlink to it from `sites-enabled`:
```bash
ln -s /etc/apache2/sites-available/v2.api.pisearch.joshkeegan.co.uk.conf /etc/apache2/sites-enabled/v2.api.pisearch.joshkeegan.co.uk.conf
```

### Restart
Finally, in order to use the new modules and config, restart apache:
```bash
service apache2 restart
```

## Publish
Locally, run `make publish-api` to publish the api.  
Upload the contents of `StringSearch.Api/out` to `/var/www/v2.api.pisearch.joshkeegan.co.uk` on the server.
Set any production config in `appsettings.Production.json`.

## Service
To check you have set everything up correctly, you can run Kestrel from the terminal with `dotnet StringSearch.Api.dll`, but for production it is better to use a service.  
Configure the new service by creating `/etc/systemd/system/pisearch-api.service` containing:
```
[Unit]
Description=PiSearch API

[Service]
WorkingDirectory=/var/www/v2.api.pisearch.joshkeegan.co.uk
ExecStart=/usr/bin/dotnet /var/www/v2.api.pisearch.joshkeegan.co.uk/StringSearch.Api.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=pisearch-api
User=pisearch
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Then start the service & check its status:
```bash
service pisearch-api start
systemctl status pisearch-api.service
```
