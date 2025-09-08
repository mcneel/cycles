# Following assumptions are made and needed
# With CCSycles checked out at SOMEPATH/cycles
#
# Bind volumes:
# SOMEPATH/cycles/cycles:/cycles
# SOMEPATH/cycles/csycles:/csycles
# .:/harvester
#
# With the container running attach VSCode to
# it and open the harvester workspace

FROM debian:trixie-20250203

RUN ls
RUN apt update && apt upgrade -y

RUN apt install -y python3-full python3-pip python3-clang-19
RUN apt install -y clang-19
RUN apt install -y libc++-19-dev
RUN apt install -y tmux htop vim git
RUN apt install -y rsync cmake dos2unix
RUN ln -sf /usr/bin/python3 /usr/bin/python

#RUN apt install -y dotnet-sdk-8.0 dotnet-runtime-8.0

RUN ln -sf /usr/bin/clang-19 /etc/alternatives/cc
RUN ln -sf /usr/bin/clang-cpp-19 /etc/alternatives/cpp
RUN ln -sf /usr/bin/clang++-19 /etc/alternatives/c++


WORKDIR /cycles
WORKDIR /csycles
WORKDIR /harvester

ENTRYPOINT ["bash", "-c", "tail -f /dev/null"]
