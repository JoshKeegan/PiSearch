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

	# Make the container
	export MSYS_NO_PATHCONV=1; \
		docker container create \
			--name "$(containerName)" \
			--restart always \
			--log-driver fluentd \
			--log-opt fluentd-address=localhost:24224 \
			--log-opt fluentd-sub-second-precision=true \
			--log-opt fluentd-async=true \
			--log-opt tag=pisearch.api.$(environment) \
			-v "$(rootPath)":/var/www/pi_digits:ro \
			-v "$(appsettingsDir)/appsettings.$(environment).json":/app/appsettings.$(environment).json \
			-e ASPNETCORE_ENVIRONMENT="$(environment)" \
			-e ASPNETCORE_URLS="http://*:$(INTERNAL_CONTAINER_PORT)" \
			-e UNIQUEIFIER="$(shell cat $(UNIQUEIFIER_PATH))" \
			$(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH))

	# Join networks
	docker network connect reverseProxy $(containerName)
	docker network connect mysql $(containerName)

	# Start the container
	docker start $(containerName)
	
wait-for-container-ready:
# TODO: Improve to actually wait for the web server to start.
#	Check every n seconds for port to respond, also checking the container is running
	sleep 10

# Args:
#	- environment
teardown-old-containers:
	/bin/bash teardownOldContainers.sh $(environment) $(shell cat $(UNIQUEIFIER_PATH))

delete-unused-images:
	/bin/bash deleteUnusedImages.sh $(IMAGE_API)

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
			--rm \
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
	sed s/{{UNIQUEIFIER}}/$(shell cat $(UNIQUEIFIER_PATH))/g -i nginx.conf
	
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
deploy: run-container wait-for-container-ready run-deployment-tests-against-container reverse-proxy-target-new-container teardown-old-containers delete-unused-images
	$(info Deployment Complete. Now running version $(shell cat $(UNIQUEIFIER_PATH)) in environment $(environment))