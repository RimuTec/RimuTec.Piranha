#! /usr/bin/env pwsh

Write-Host "Removing database rimutec from server rimutec-integration"

az sql db delete -g buildpipeline -s rimutec-integration -n rimutec --yes

Write-Host "Database rimutec removed successfully"
