cd /harvester
python generate_shader_nodes.py datatypes_all.json shadernodes_cs
cp shadernodes_cs/ccycles.cpp /cycles/src/ccycles/ccycles.cpp
cd /cycles
make
