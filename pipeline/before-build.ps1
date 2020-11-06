#! /usr/bin/env pwsh

Write-Host "Creating database rimutec on server rimutec-integration"

az sql db create -g buildpipeline -s rimutec-integration -n rimutec --service-objective S0

Write-Host "Database rimutec created successfully"
