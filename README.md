# SmartHome.Net
SmartHome.Net is an intelligent home IoT project that leverages the power of Azure cloud services, open source hardware and built on top of.Net 5. proteus
An IoT project that leverages the power of Azure Cloud Services and built on top of .NET and unity

# Contents
This repository contains the following:
- Unity3d project files used to build the client application.
- RaspberryPi device-side project files.
- Arduino sketches used to control and automate arduino devices.
- detailed UML sequence diagram and use case diagram to illustrate programs' execution sequence.
- [optional] Proteus Files with arduino circuit setup, simulation ready.
- The unity project can be found inside the UNITY3D_PROJECT_ClientSide folder 

# Features
- Monitor and Control relay connected lights and servo controlled door.
- Identify intruder and alert client using sensors and conditionals based on if client is home, away or asleep.
- Facebook SSO login authentication and device authorization as an extra layer of security.
- Alert client of unwanted conditions such as high temerature or humidity.
- Alerts are sent to client by mail and SMS .
- Provide graphical visualization of operating conditions in the home.
- Allow client set the home into "home", "away", "sleep" and "awake" modes preset modes.

# Prerequisites

## Hardware Requirements
The project code can be edited to add or remove certain hardware needs but the list of hardware devices used in the project are included in the *Requirements* docx within the **FlowCHarts** folder.

## Software Requirements
The software reqirements can also be seen in the *Requirements* docx. It is important to take note of the software components that are optional and replaceable and those that are not.

## **Important**
For the RaspberryPi to communicate with the right Arduino at all times, the Rpi port should be assigned permanent/fixed labels and the Arduino USB connection ports should not be changed afterwards.

Detailed steps on how to achieve this can be found in this freva site [here](https://www.freva.com/assign-fixed-usb-port-names-to-your-raspberry-pi/)

# Testing
Certain important edits will need to be made to the project to achieve successful build and test scenarios.
The edits include:
- Every occurrence of a connection string. This can be gotten from Azure whenever you create a storage type or a reprsentation of a device in the cloud.
- Every occurrence of an API access token. This can be gotten from API details of the associated Azure resource or Facebook resource(eg. Azure Function API key).

# Building
To successfully build this project, ensure to follow the instructions of the prerequisites and testing section.

- To see how to build a unity project for Android, visit [here](https://learn.unity.com/tutorial/publishing-for-android#5ecf930aedbc2a002088a2cc).
- To see how to build and deploy a .NET project for RaspberryPi, visit [here](https://docs.microsoft.com/en-us/dotnet/iot/deployment).
- To see how to to build and deploy a sketch to an Arduino board, visit [here](https://create.arduino.cc/projecthub/yeshvanth_muniraj/getting-started-with-arduino-bcb879).
- For Some tutorials on how to transfer data between a RaspberryPi and an Arduino over serial, check out these these links [here](https://create.arduino.cc/projecthub/sxwei123/serial-communication-with-net-core-3-0-on-rpi-linux-0f2ed4) and [here](https://roboticsbackend.com/raspberry-pi-arduino-serial-communication/).