FROM ubuntu

# Set timezone
ENV TZ=America/New_York
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Add source files to docker image
ADD .	/root

# Update and install dependencies
RUN apt-get -y update \
    && apt-get -y upgrade \
    && apt-get -y install wget \
    && wget https://packages.microsoft.com/config/ubuntu/20.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y apt-transport-https \
    && apt-get update \
    && apt-get install -y dotnet-sdk-5.0
    
## compile websocket
RUN cd /root \
    && dotnet build

    
EXPOSE 8080

WORKDIR /root
CMD ["dotnet", "run", "--project", "cs_fleck_websocket_benchmark_server", "--framework", "netcoreapp5.0"]
