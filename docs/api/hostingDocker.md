# Hosting (in Docker)
This is a quick setup guide for an instance of the PiSearch API within a Docker container.  

The API is hosted on Debian Linux (version 9/Stretch) using apache2 as a reverse proxy to pass requests on to Kestrel running within a Docker container.  
Continuous Deployment is operated via a Gitlab CI/CD pipeline, which is declared (with all associated scripts) in this repo, so could easily be replicated.

## Pre-reqs
Server must have installed:
 - apache2
 - docker
 - make
 - ssh server

For other dependencies to set up, see [Running the API](running.md).

## User
```bash
adduser pisearch
usermod -aG docker pisearch
```

## Application config
The bulk of application configuration is contained within an `appsettings.{Environment Name}.json` file that must be volume mounted into the Docker container.  
These should be created for all intended environments within a directory, e.g.
```bash
mkdir ~/config
nano -w ~/config/appsettings.prod.json
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
<VirtualHost *:80>
    DocumentRoot /var/www
</VirtualHost>
```

Change `pisearch` to be the user that owns the config file, in order to allow it to update the contents later during deployment.
```bash
chown pisearch:pisearch /etc.apache2/sites-available/v2.api.pisearch.joshkeegan.co.uk.conf
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

At this point, visiting your website should return the contenbts of `/var/www/index.html` on your server.
This will change on the first deployment of PiSearch.

## Sudoers
In order for the `pisearch` user to be able to reload apache2, applying a new configuration during deployment, we must allow it to do so.  
Edit `/etc/sudoers`, adding:
```
pisearch ALL=NOPASSWD: /etc/init.d/apache2
```

## Passwordless SSH Access
In order to deploy to the server, the deployment stage of the pipeline must be able to run commands on the server.
This is achieved via SSH. In order for access to be granted without prompting for a password, generate an SSH key that can be used
by the build server, and put its public key on the list of authorised keys for the `pisearch` user.
This process is [widely documented online](http://www.linuxproblem.org/art_9.html).

## Publish
PiSearch is intended to be deployed via a CI/CD pipeline, so exact steps are not documented here.  
If setting up a new pipeline, or wanting to do a manual deployment, follow `.gitlab-ci.yml` in the root of this repo.