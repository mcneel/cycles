# Harvesting and generating API

## Requirements

* [CCSycles](https://github.com/mcneel/CCSycles) checked out at SOMEPATH/cycles
* Docker

## Set up Docker image

$> docker build -f Dockerfile -t cppharvester .

## Run Docker image

To use the Docker image it needs to be started with a couple of folders mounted
as volumes:

- SOMEPATH/cycles/cycles:/cycles
- SOMEPATH/cycles/csycles:/csycles
- .:/harvester

$> docker run -it -v SOMEPATH/cycles/cycles:/cycles -v SOMEPATH/cycles/csycles:/csycles -v .:/harvester -name cppharvester cppharvester

## Harvesting data from Cycles

With the container running attach VSCode to it and open the harvester workspace.

Then run the debug target `Debug CllHarvester (container)`.

This will generate the file `datatypes.json`.

## Generating API

After harvesting datatypes from Cycles run the debug target `Debug Shadernodes
Generation`. This will generate files in `shadernodes_cs`. There will be a
`ccycles.cpp` containing the wrapper C API. `CSycles.cs` contains the P/Invoke
layer bringing the C API into .NET. Files matching the pattern `.+Node\.cs` are
the shader nodes.

