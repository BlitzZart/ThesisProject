This software was utilized for the master's thesis evaluation. The representation (simulation/animation) of users' avatars in co-located settings is the core topic. It allows a comparison between two different tracking technologies while utilizing the same avatar.

A custom LIDAR based approach was compared to state of the art 6DoF trackers (Vive Trackers). The Samsung GearVR with Galaxy S6 smartphone served as HMDs.

## Abstract

The so-called second wave of virtual reality (VR) is the result of technological advances and ongoing research over the past 50 years. It entails a variety of consumer devices and a great diversity of new content. While most current VR applications focus on single-user interaction, it is highly likely that multi-user 
applications are going to gain in importance.
Hence, the representation of users in VR environments becomes a more crucial factor for such VR experiences. That especially applies to co-located settings, where multiple users share both the virtual and the real space. The way how avatars are visualized strongly influences their perception and therefore the feeling of presence.
The goal of this thesis is to develop a modular tracking and animation system for VR environments, which should reveal further insights within this domain. Expert interviews are conducted to detect possible improvements to the introduced approach. The outcome of this thesis should facilitate the examination of avatar visualization and presence in co-located VR environments.



## Information concerning the utilized tracking plugin.

This is a Unity3D Project which includes implementations of a transmission client for position tracking protocols.
The project currently is set up for Unity3D 5.3.2f1 but will very likely run flawlessly in older and newer versions.

### Which protocols are supported? ###

* open TUIO protocol (UDP / OSC.NET)
* proprietary TrackLink (formerly referred to as Pharus) protocol (TCP)
* proprietary TrackLink (formerly referred to as Pharus) protocol (UDP)

### What else is included? ###

* A simple evaluation module which records positions (and optionally also game events) to JSON format
* config.xml files for easy access to settings in builds
