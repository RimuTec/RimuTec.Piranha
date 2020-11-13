FROM mcr.microsoft.com/dotnet/sdk:3.1

# Create non-root user
RUN useradd -m -s $(which bash) piranha
# Option '-m' to create home directory (see https://askubuntu.com/a/393470)
# Option '-s' to set  shell for this user (see comment in https://askubuntu.com/a/393470)

# Create working directory and change ownership.
# Ownership will be preserved when mounting volume to /app
# See https://stackoverflow.com/a/56990338/411428
# and https://github.com/docker/compose/issues/3270#issuecomment-543603959
# Tested with Docker Desktop for Windows 2.4.0.0 and 2.5.0.1
RUN mkdir /app && chown -R piranha:piranha /app
USER piranha

# Install dotnet-outdated (see https://github.com/dotnet-outdated/dotnet-outdated)
RUN dotnet tool install --global dotnet-outdated-tool
ENV PATH "$PATH:/home/piranha/.dotnet/tools"
# If the previous doesn't work, consider https://stackoverflow.com/a/59293118/411428 [Manfred, 13nov2020]
