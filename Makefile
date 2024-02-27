.DEFAULT_GOAL := help

# General Variables
date=$(shell date +'%y.%m.%d.%H.%M')
project := Init stack
container := dev
docker-file-check := /.dockerenv
docker-warning := ""
RED=\033[0;31m
GREEN=\033[0;32m
NC=\033[0m # No Color
versionPrefix := 0.1
version := $(versionPrefix).$(shell git rev-list HEAD --count)
git-short-hash := $(shell git rev-parse --short=8 HEAD)
version-suffix := ''
dockerhub := dockerhub.com/init-stack

release := release
ifeq ($(env), dev)
	release := debug
	version-suffix:= ""
endif

ifdef GITHUB_BASE_REF
	current-branch :=  $(patsubst refs/heads/%,%,${GITHUB_HEAD_REF})
else ifdef GITHUB_REF
	current-branch :=  $(patsubst refs/heads/%,%,${GITHUB_REF})
else
	current-branch :=  $(shell git rev-parse --abbrev-ref HEAD)
endif

ifeq ($(current-branch), main)
	docker-tags := -t $(dockerhub):alpha -t $(dockerhub):latest -t $(dockerhub):v$(version) -t $(dockerhub):$(git-short-hash)
	version-full := $(version)
else
	version := $(versionPrefix).$(shell git rev-list origin/master --count).$(shell git rev-list origin/master..HEAD --count)
	version-suffix := alpha
	version-full := $(version)-$(version-suffix)
	docker-tags := -t $(dockerhub):$(version-suffix) -t $(dockerhub):$(git-short-hash) -t $(dockerhub):v$(version-full)
endif

# Docker Warning
ifeq ("$(wildcard $(docker-file-check))","")
	docker-warning = "⚠️  WARNING: Can't find /.dockerenv - it's strongly recommended that you run this from within the docker container."
endif

# Targets
help:
	@echo "The following commands can be used for building & running & deploying the the $(project) container"
	@echo "---------------------------------------------------------------------------------------------"
	@echo "Targets:"
	@echo "  Docker Targets (run from local machine)"
	@echo "   - up          : brings up the container & attach to the default container ($(container))"
	@echo "   - down        : stops the container"
	@echo "   - build       : (re)builds the container"
	@echo ""
	@echo "  Service Targets (should only be run inside the docker container)"
	@echo "   - version               : Set current version number $(project)"
	@echo "   - start                 : Run the $(project)"
	@echo "   - test                  : Test the $(project)"
	@echo "   - publish               : Publish the $(project)"
	@echo "   - docker-login          : Login to docker registry"
	@echo "   - docker-build          : Build the docker image"
	@echo "   - docker-push           : Push the docker image"
	@echo "   - docker-pull-short-tag : Pull the docker image based in git short hash"
	@echo "   - docker-tag-env        : Tag the docker image based in the environment"
	@echo "   - docker-publish        : Publish the docker image"
	@echo "   - deploy                : Deploy the $(project)"
	@echo "   - update-packages       : Update the packages"

	
	@echo ""
	@echo "Options:"
	@echo " - env    : sets the environment - supported environments are: dev | prod"
	@echo ""
	@echo "Examples:"
	@echo " - Start Docker Container              : make up"
	@echo " - Rebuild Docker Container            : make build"
	@echo " - Rebuild & Start Docker Container    : make build up"
	@echo " - publish and deploy                  : make publish deploy env=dev"

up:
	@echo "Starting containers..."
	@docker compose up -d
	@echo "Attaching shell..."
	@docker compose exec $(container) zsh

down:
	@echo "Stopping containers..."
	@docker compose down

build: down
	@echo "Building containers..."
	@docker compose build

version:
	@echo -e "Setting version number ${GREEN}v${version}${NC} "
	@echo '{ "version": "${version}" }' > src/version.json


start: 
	@echo -e "Starting the $(release) release of $(project)"
	@cd src/InitStack.Cmd; dotnet run -- --help

test: 
	@echo -e "Testing ${GREEN}v${version}${NC}"
	@dotnet test InitStack.sln --logger:trx --results-directory ../TestResults \
		 /p:CollectCoverage=true \
		 /p:CoverletOutput=../TestResults/ \
		 /p:MergeWith=../TestResults/coverlet.info \
		 /p:Exclude="[*.Tests]*" \
		 /p:CoverletOutputFormat="lcov"

publish: 
	@echo -e "Building the ${GREEN}v${version}${NC}-$(release) release of $(project)"
		
	@dotnet publish src/InitStack.Cmd/InitStack.Cmd.csproj -r linux-x64 -c $(release) \
		-p:DebugType=None \
		-p:DebugSymbols=false \
		-p:PublishSingleFile=true \
		-p:PublishTrimmed=false  \
		-p:SelfContained=false \
		-p:VersionSuffix=$(version-suffix) \
		-p:FileVersion=$(version) \
		-p:VersionPrefix=$(version) \
	--output ./dist/$(release)/linux-x64 
	@dotnet publish src/InitStack.Cmd/InitStack.Cmd.csproj -r win-x64 -c $(release) \
		-p:DebugType=None \
		-p:DebugSymbols=false \
		-p:PublishSingleFile=true \
		-p:PublishTrimmed=false  \
		-p:SelfContained=false \
		-p:VersionSuffix=$(version-suffix) \
		-p:FileVersion=$(version) \
		-p:VersionPrefix=$(version) \
		--output ./dist/$(release)/win-x64 


docker-login: 
	@echo -e "Login to docker $(registry)"
	@read -p "Username: " docker_username; \
	read -s -p "Password: " docker_password; \
	echo ""; \
	echo $$docker_password | docker login --username $$docker_username --password-stdin

docker-build:
	@echo -e "Building branch ${RED}$(current-branch)${NC} to ${GREEN}$(docker-tags)${NC} with ${GREEN}$(version-full)${NC}"
	@cd src && docker build -f InitStack.Cmd/Dockerfile --build-arg VERSION=$(version) --build-arg VERSION_SUFFIX=$(version-suffix) ${docker-tags} .

docker-push:
	@echo -e "Pushing to ${GREEN}$(docker-tags)${NC}"
	@docker push --all-tags $(registry)
	@docker images | grep "$(registry)" | awk '{system("docker rmi " "'"$(registry):"'" $$2)}'

docker-pull-short-tag:
	@echo -e "Pulling ${GREEN}$(registry):$(git-short-hash)${NC}"
	@docker pull "$(registry):$(git-short-hash)" 

docker-tag-env: env-check
	@echo -e "Tagging release ${GREEN}$(env)${NC}"
	@docker tag "$(registry):$(git-short-hash)" "$(registry):$(env)"	
	@docker images | grep "$(registry)"

docker-publish: docker-build docker-login docker-push
	@echo -e "Done"

deploy: publish env-check
	@echo -e "Deploying ${GREEN}v${version}${NC} of $(release)"
	
update-packages:
	@echo "Updating package references to the latest versions..."
	@for proj in $$(find . -name '*.csproj'); do \
		echo "Updating $$proj..."; \
		for package in $$(dotnet list $$proj package | awk '/>/ {print $$2}'); do \
			dotnet add $$proj package $$package; \
		done; \
	done
	@echo "Update complete."

docker-check:
	$(call assert-file-exists,$(docker-file-check), This step should only be run from Docker. Please run `make up` first.)

env-check:
	$(call check_defined, env, No environment set. Supported environments are: [ dev | prod ]. Please set the env variable. e.g. `make env=dev publish`)

# Check that given variables are set and all have non-empty values,
# die with an error otherwise.
#
# Params:
#   1. Variable name(s) to test.
#   2. (optional) Error message to print.
check_defined = \
    $(strip $(foreach 1,$1, \
    	$(call __check_defined,$1,$(strip $(value 2)))))
__check_defined = \
    $(if $(value $1),, \
    	$(error Undefined $1$(if $2, ($2))))

define assert
  $(if $1,,$(error Assertion failed: $2))
endef

define assert_warn
  $(if $1,,$(warn Assertion failed: $2))
endef

define assert-file-exists
  $(call assert,$(wildcard $1),$1 does not exist. $2)
endef
