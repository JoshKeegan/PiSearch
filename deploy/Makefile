#
# Deployment Makefile
#

UNIQUEIFIER_PATH = ../uniqueifier#
PORT_PATH = ../port#
INTERNAL_CONTAINER_PORT = 80#

include ../../sharedScripts/make/dockerImages.mk

clean:
	rm -r $(PORT_PATH) || true

find-available-port: clean
	/bin/bash findAvailablePort.sh > $(PORT_PATH)

# 
# Docker
#

# Args:
#	- rootPath
#	- environment
#	- appsettingsDir
run-container: find-available-port
	export MSYS_NO_PATHCONV=1; \
		docker run \
			--detach \
			--name "PiSearch-$(environment)-$(shell cat $(UNIQUEIFIER_PATH))" \
			--restart always \
			-v "$(rootPath)":/var/www/pi_digits:ro \
			-e ASPNETCORE_ENVIRONMENT="$(environment)" \
			-e ASPNETCORE_URLS="http://*:$(INTERNAL_CONTAINER_PORT)" \
			-p $(shell cat $(PORT_PATH)):$(INTERNAL_CONTAINER_PORT) \
			-v "$(appsettingsDir)/appsettings.$(environment).json":/app/appsettings.$(environment).json \
			$(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH))

wait-for-container-ready:
# TODO: Improve to actually wait for the web server to start.
#	Check every n seconds for port to respond, also checking the container is running
	sleep 10

# Args:
#	- environment
teardown-old-containers:
	/bin/bash teardownOldContainers.sh $(environment) $(shell cat $(UNIQUEIFIER_PATH))

#
# Deployment Tests
#

run-deployment-tests-against-container:
	/bin/bash runDeploymentTests.sh localhost $(shell cat $(PORT_PATH))

#
# Reverse Proxy
#

# Args:
#	- environment
#	- hostname
generate-apache-conf: clean
	cp apache.conf.template apache.conf
	sed s/{{PORT}}/$(shell cat $(PORT_PATH))/g -i apache.conf
	sed s/{{ENVIRONMENT}}/$(environment)/g -i apache.conf
	sed s/{{HOSTNAME}}/$(hostname)/g -i apache.conf
	
# Args:
#	- environment
#	- hostname
reverse-proxy-target-new-container: generate-apache-conf
	cp apache.conf /etc/apache2/sites-available/$(hostname).conf
	sudo /etc/init.d/apache2 reload

# Main deployment

# Args:
#	- rootPath
#	- environment
#	- appsettingsDir
#	- hostname
deploy: run-container wait-for-container-ready run-deployment-tests-against-container reverse-proxy-target-new-container teardown-old-containers
	$(info Deployment Complete. Now running version $(shell cat $(UNIQUEIFIER_PATH)) in environment $(environment))