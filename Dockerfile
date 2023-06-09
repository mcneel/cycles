# This Dockerfile will build AMD HIP binaries (.fatbin files)
# All you need to do is the following:
# On Windows:
#  * Install Docker Desktop
#  * Follow instructions on how to install WSL2
#  * Make sure Docker Desktop is running
#  * Use a terminal to build a Docker image: 'docker build -t ubuntu-cycles-hip-build .'
#  * After that, run the Docker image in a container while mounting your Rhino repository root:
#    'docker run -it -v D:/dev/mcneel/rhino-8.x-cyclesx:/rhino/rhino-8.x-cyclesx ubuntu-cycles-hip-build'
#  * The .fatbin files should en up in the cycles/cycles/install/lib directory

FROM ubuntu:latest

RUN apt-get update
RUN apt-get install -y g++
RUN apt-get install -y cmake
RUN apt-get install -y subversion
RUN apt-get install -y git
RUN apt-get install -y python3
RUN apt-get install -y wget

WORKDIR /rhino
RUN wget https://repo.radeon.com/amdgpu-install/5.5.1/ubuntu/jammy/amdgpu-install_5.5.50501-1_all.deb
RUN apt-get install -y ./amdgpu-install_5.5.50501-1_all.deb
RUN amdgpu-install -y --no-dkms --usecase=rocm

CMD ["bash", "-c", "cd /rhino/rhino-8.x-cyclesx/src4/rhino4/Plug-ins/RDK/cycles/cycles && ./make_hip.sh Debug"]