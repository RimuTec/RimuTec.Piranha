#!/bin/sh

# Change ownership of all directories and files in the mounted volume, i.e.
# what has been mapped from the host:
chown -R piranha:piranha /app

# Finall invoke what has been specified as CMD in Dockerfile or command in docker-compose:
"$@"
