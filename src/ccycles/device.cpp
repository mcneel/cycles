/**
Copyright 2014-2017 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

#include "internal_types.h"

unsigned int cycles_number_devices() {
	return (unsigned int)devices.size();
}

unsigned int cycles_number_multidevices() {
	return (unsigned int)multi_devices.size();
}

CCL_CAPI unsigned int CDECL cycles_number_devices_by_type(unsigned int device_type)
{
	int i{ 0 };
	for (auto di : devices) {
		if (di.type == (ccl::DeviceType)device_type) i++;
	}

	return i;
}

unsigned int cycles_number_multi_subdevices(int i) {
	if (MULTIDEVICEIDX(i) >= 0 && MULTIDEVICEIDX(i) < multi_devices.size())
		return multi_devices[MULTIDEVICEIDX(i)].multi_devices.size();
	else
		return 0;
}

unsigned int cycles_get_multidevice_subdevice_id(int i, int j) {
	if (MULTIDEVICEIDX(i) >= 0 && MULTIDEVICEIDX(i) < multi_devices.size())
	{
		auto d = multi_devices[MULTIDEVICEIDX(i)];
		if (j >= 0 && j < d.multi_devices.size()) {
			auto sd = d.multi_devices[j];
			int k = 0;
			for (auto di : devices) {
				if (sd == di) return k;
				k++;
			}
		}
	}
	return -1;

}


const char *cycles_device_description(int i) {
	if (i>= 0 && i < devices.size())
		return devices[i].description.c_str();
	else if(MULTIDEVICEIDX(i) >= 0 && MULTIDEVICEIDX(i) < multi_devices.size())
		return multi_devices[MULTIDEVICEIDX(i)].description.c_str();
	else
		return "-";
}

const char *cycles_device_id(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].id.c_str();
	else if(MULTIDEVICEIDX(i) >= 0 && MULTIDEVICEIDX(i) < multi_devices.size())
		return multi_devices[MULTIDEVICEIDX(i)].id.c_str();
	else
		return "-";
}

int cycles_device_num(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].num;
	else if(MULTIDEVICEIDX(i) >= 0 && MULTIDEVICEIDX(i) < multi_devices.size())
		return multi_devices[MULTIDEVICEIDX(i)].num;
	else
		return -1;
}

bool cycles_device_display_device(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].display_device;
	else if(MULTIDEVICEIDX(i) >= 0 && MULTIDEVICEIDX(i) < multi_devices.size())
		return multi_devices[MULTIDEVICEIDX(i)].display_device;
	else
		return false;
}

unsigned int cycles_device_type(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].type;
	else if(MULTIDEVICEIDX(i) >= 0 && MULTIDEVICEIDX(i) < multi_devices.size())
		return multi_devices[MULTIDEVICEIDX(i)].type;
	else
		return 0;
}


const char* cycles_device_capabilities() {
	static std::string capabilities = ccl::Device::device_capabilities();
	return capabilities.c_str();
}

int cycles_create_multidevice(int count, int* idx) {
	int foundidx = -1;

	ccl::vector<ccl::DeviceInfo> subdevices;
	for (int i = 0; i < count; i++)
	{
		ccl::DeviceInfo dev = devices[idx[i]];
		subdevices.push_back(dev);
	}
	ccl::DeviceInfo themulti = ccl::Device::get_multi_device(subdevices, 0, true);

	bool found = false;
	for (auto multi : multi_devices)
	{
		found = multi == themulti;
		if (found) {
			foundidx = multi.num;
			break;
		}
	}

	if (!found) {
		themulti.num = multi_devices.size() + MULTIDEVICEOFFSET;
		foundidx = themulti.num;
		multi_devices.push_back(themulti);
	}

	return foundidx;
}
