#!/bin/bash

sfctl application delete --application-id nodesfapp
sfctl application unprovision --application-type-name nodesfappType --application-type-version 1.0.0
sfctl store delete --content-path nodesfapp
