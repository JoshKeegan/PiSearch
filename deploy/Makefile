#
# Deployment Makefile
#

UNIQUEIFIER_PATH = ../uniqueifier#
INTERNAL_CONTAINER_PORT = 80#
EXTERNAL_CONTAINER_PORT = 80#

include ../../sharedScripts/make/dockerImages.mk

clean:
	rm -r nginx.conf || true

# Args:
#	- result variable
#	- environment
define get-container-name
	$(1) = pisearch-$(2)-$(shell cat $(UNIQUEIFIER_PATH))
endef

# 
# Docker
#

# Args:
#	- rootPath
#	- environment
#	- appsettingsDir
run-container:
	$(eval $(call get-container-name,containerName,$(environment)))

	export MSYS_NO_PATHCONV=1; \
		docker run \
			--detach \
			--name "$(containerName)" \
			--restart always \
			-v "$(rootPath)":/var/www/pi_digits:ro \
			-e ASPNETCORE_ENVIRONMENT="$(environment)" \
			-e ASPNETCORE_URLS="http://*:$(INTERNAL_CONTAINER_PORT)" \
			-v "$(appsettingsDir)/appsettings.$(environment).json":/app/appsettings.$(environment).json \
			--network=reverseProxy \
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

# Args:
#	- environment
run-deployment-tests-against-container:
	$(eval $(call get-container-name,containerName,$(environment)))

# Run within docker so that we can connect to the same docker network
	export MSYS_NO_PATHCONV=1; \
		docker run \
			-v ${PWD}/runDeploymentTests.sh:/app/runDeploymentTests.sh:ro \
			--network=reverseProxy \
			--entrypoint /bin/bash \
			joshkeegan/deploy-ssh \
			/app/runDeploymentTests.sh $(containerName) $(EXTERNAL_CONTAINER_PORT)

#
# Reverse Proxy
#

# Args:
#	- environment
#	- hostname
generate-nginx-conf:
	$(eval $(call get-container-name,containerName,$(environment)))

	cp nginx.conf.template nginx.conf
	sed s/{{PORT}}/$(EXTERNAL_CONTAINER_PORT)/g -i nginx.conf
	sed s/{{ENVIRONMENT}}/$(environment)/g -i nginx.conf
	sed s/{{HOSTNAME}}/$(hostname)/g -i nginx.conf
	sed s/{{CONTAINER_HOSTNAME}}/$(containerName)/g -i nginx.conf
	
# Args:
#	- environment
#	- hostname
reverse-proxy-target-new-container: generate-nginx-conf
	cp nginx.conf /home/josh/data/webServer/conf/$(hostname).conf
	cd /home/josh/ServerAutomation/webServer && \
		/bin/bash reload.sh

# Main deployment

# Args:
#	- rootPath
#	- environment
#	- appsettingsDir
#	- hostname
deploy: run-container wait-for-container-ready run-deployment-tests-against-container reverse-proxy-target-new-container teardown-old-containers
	$(info Deployment Complete. Now running version $(shell cat $(UNIQUEIFIER_PATH)) in environment $(environment))