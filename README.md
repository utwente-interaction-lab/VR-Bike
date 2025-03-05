# VR-Bike

VR-Bike System Documentation

---

**Version/Date: 24.04.2024**

### Introduction

The VR bike system is a project combining virtual reality with cycling. Streaming the data from a spinning bike to a Unity project rendered on a VR headset allows users to immerse themselves in virtual environments while cycling, potentially providing more engaging and interactive experiences. This document explains how the system works and how to run it with the associated Unity Project.

### Requirements

- **Hardware**:
    - VR-Bike
    - Sufficiently powerful PC (supporting Unity and Visual Studio)
    - VR headset (e.g. Meta Quest 3)
- **Software**:
    - Unity version 2020.3.18f1
    - Visual Studio 2022
        - .NET Framework 4.8

### Creation

The VR-Bike system was conceptualized and built by Tom Onderwater as a tool for his research project, titled: "Investigating the Effect of Visual Information and Optic Flow on Interception Adequacy using a VR-Bike". Tom visualized the VR bike as "a new research tool which interfaces with a virtual environment, enabling research into the effect of optic flow."

After Tom had finished his research, he kindly offered the VR bike and the source code to the Interaction Lab.

### System Description

The main physical component of the system is the VR-Bike [fig ref], which was developed to interface with the VR environment and consists of the following two sub-systems, which are both connected to the host PC via USB [fig ref]:

- *Praxtour spinning bike* is a commercially available spinning bike with a USB interface. The bike has a flywheel the resistance of which can be programmatically adjusted to simulate braking, riding on an inclined surface, or with increased drag, etc. The spinning bike generates the following data: cycling speed; left brake and right brake intensity; gear; and cadence.
- *Handlebar* that can rotate and is used to turn inside the virtual environment. The rotation of the handlebar is measured by a sensor connected to an Arduino board [fig ref].

The project 

The readings from the Praxtour bike and the Arduino are aggregated together before being streamed to the virtual environment over a web socket connection at 50 Hz. There's a fixed 0.1-second delay between action and perception by the system.

This functionality is implemented through a controller program called *MinimalController,* written in C# using .NET 4.8. The primary classes are:

- *InputControl -* which connects to the Praxtour bike and the Arduino Uno and reads the data from both devices.
- *Program -* which **uses InputControl and a class called SocketServer to read and send the data through a web socket.

The virtual environment consists of a scene within the *BikeMan* Unity project. It contains the bike, a plain ground surface, trees, and a target ball, which moves and is to be intercepted by the player. The headset’s position is fixed relative to the cyclist model but head movement is allowed.

The behavior of the bike in the Unity Project is enabled by multiple scripts, mainly *Bike,* *BikePhysics,* and *BikeSocketClient* which use a web socket client to connect to the controller’s socket server, read the aggregated data, and based on this data implement the bike’s dynamics within the Unity scene.

The flywheel's resistance, which makes the pedals feel heavier, is calculated by adding a predefined *Normal Resistance* and the braking intensity from the brake levers, streamed back to the Praxtour bike through the controller via the web socket connection.

**Note:** Since the readings from the sensors on the bike need to be streamed to the Unity scene via a PC, the Unity project has to run in PCVR mode. Therefore, you can connect any VR headset supporting PCVR to render the scene.

Note: The script to read the handlebar rotation has already been loaded on the Arduino Uno board. In case you need to modify or reload it, the script can be found in the BikeSteer directory in the repository.

TIP: the readings from the Arduino board can be checked directly in Arduino IDE.

TIP: if you use a Meta Quest headset and have a laptop with hybrid graphics (e.g. Lenovo Legion 5), make sure to set the discrete graphics mode from the bios, otherwise, the Quest Link might get stuck in the loading screen.


![vr-bike-scheme](https://github.com/user-attachments/assets/614efb17-4932-4f68-a3f7-12b9b10bfa9d)

### Instructions on running the project

To run the VR bike Unity project,

Step 0: prerequisites

- Clone the VR bike project from Interaction Lab’s GitHub repository (https://github.com/utwente-interaction-lab/VR-Bike)
- Install Visual Studio 2022 (https://visualstudio.microsoft.com/downloads/?cid=learn-onpage-download-install-visual-studio-page-cta)
    - Install the .NET Framework 4.8 within Visual Studio
- Install 2020.3.18f1 through Unity Hub [video ref]

Step 1: Run the MinimalController program to automatically connect and start streaming the readings from the VR Bike through a web socket.

1. Browse to “MinimalController\USB2550HidTest\USB2550HidTest\bin\Release” within the root project directory.
2. Run “USB2550HidTest.exe”.

Step 2: Launch the BikeMan Unity project, which reads the data from the MinimalController via a client web socket and generates the virtual environment.

1. Open “Unity Hub”, then go to “Projects”.
2. Click “Add” near the top right corner.
3. Select the “BikeMan” directory from the project’s root directory. The project will appear in the list of projects.
4. Click on the project list entry to launch it.

Step 2.1: Select the “BikeMan” scene:

1. In the “Project” panel on the bottom, open the “Scenes” directory within the “Assets” directory in the left navigation sub-panel.
    1. Alternatively, search for “BikeMan” in the search box at the top of the “Project” panel.
2. Open the “BikeMan” scene.

Step 2.2: Lock Input to Game View - a necessary fix for running the project in PCVR mode:

1. In the top menu bar, click “Edit”, and then select “Project Settings…” from the drop-down list.
2. Go to “XR Plug-in Management” in the left navigation panel.
3. Make sure the “OpenXR” checkbox is checked. Then, click on the warning triangle icon on the right.
4. In the pop-up window click on the “Fix All” button in the top right corner.

Step 3: Connect the VR headset and play the BikeMan scene.

1. Connect your VR headset of choice which supports running Unity in PCVR mode. (e.g. Meta Quest 3)
2. Click the play icon on the top in Unity. The scene should be rendered to the headset
3. Mount the VR bike and start cycling.

Warning: Use caution when mounting the VR Bike while wearing the headset as you risk falling or hurting yourself. Ideally, you should ask someone to assist you by launching the Unity scene after you have mounted the bike.

To debug, modify or improve the MinimalController program, open it using Visual Studio 2022.
