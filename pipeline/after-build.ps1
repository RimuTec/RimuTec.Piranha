#! /usr/bin/env pwsh

Write-Host "Removing database rimutec from server rimutec-integration in region EastUS"

#az sql db delete -g buildpipeline -s rimutec-integration -n rimutec

Write-Host "Database rimutec removed successfull"
