#!/bin/bash

sfctl application upload --path nodesfapp --show-progress
sfctl application provision --application-type-build-path nodesfapp
sfctl application create --app-name fabric:/nodesfapp --app-type nodesfappType --app-version 1.0.0
