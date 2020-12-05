﻿using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chen.GradiusMod
{
    /// <summary>
    /// A static class that caters initializing and registering custom drones.
    /// </summary>
    public static class DroneCatalog
    {
        internal static List<DroneInfo> validInstances = new List<DroneInfo>();

        /// <summary>
        /// Generates a list of data containing the custom drones of the mod that called this method.
        /// </summary>
        /// <param name="modGuid">The mod GUID</param>
        /// <param name="configFile">The file where the mod's custom drones will bind their configs</param>
        /// <returns>A list of DroneInfos from the mod that called this method.</returns>
        public static List<DroneInfo> Initialize(string modGuid, ConfigFile configFile)
        {
            List<DroneInfo> droneInfos = new List<DroneInfo>();
            bool filter(Type t) => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Drone));
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes().Where(filter))
            {
                Drone droneInstance = (Drone)Activator.CreateInstance(type);
                DroneInfo newDroneInfo = new DroneInfo(modGuid, droneInstance, configFile);
                droneInstance.info = newDroneInfo;
                droneInfos.Add(newDroneInfo);
            }
            return droneInfos;
        }

        /// <summary>
        /// Sets all the custom drones contained in the list up. Mod creators may instantiate the drones their own if they have a sophisticated logic.
        /// This flavor shows the normal way of setting up the drone instances.
        /// The phases will still capture generic flags such as if the drone is enabled or not, or if the drone was already set up or not.
        /// </summary>
        /// <param name="droneInfos">List of DroneInfos generated by Initialize</param>
        public static void SetupAll(List<DroneInfo> droneInfos)
        {
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.PreSetup();
            }
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.SetupFirstPhase();
            }
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.SetupSecondPhase();
            }
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.SetupThirdPhase();
            }
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.PostSetup();
            }
        }

        /// <summary>
        /// Sets all the custom drones contained in the list up. Mod creators may instantiate the drones their own if they have a sophisticated logic.
        /// This flavor comes with efficient an setup by taking advantage of the boolean return values of each phase.
        /// </summary>
        /// <param name="droneInfos">List of DroneInfos generated by Initialize</param>
        public static void EfficientSetupAll(List<DroneInfo> droneInfos)
        {
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.PreSetup();
            }
            foreach (DroneInfo droneInfo in droneInfos)
            {
                if (droneInfo.instance.SetupFirstPhase())
                {
                    validInstances.Add(droneInfo);
                }
            }
            foreach (DroneInfo droneInfo in validInstances)
            {
                if (!droneInfo.instance.SetupSecondPhase())
                {
                    validInstances.Remove(droneInfo);
                }
            }
            foreach (DroneInfo droneInfo in validInstances)
            {
                if (!droneInfo.instance.SetupThirdPhase())
                {
                    validInstances.Remove(droneInfo);
                }
            }
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.PostSetup();
            }
        }

        /// <summary>
        /// Sets all the custom drones contained in the list up. Mod creators may instantiate the drones their own if they have a sophisticated logic.
        /// This flavor does a scoped setup, effective if the custom drones are coded in such a way they have no dependent/shared components/behaviors from one another.
        /// </summary>
        /// <param name="droneInfos">List of DroneInfos generated by Initialize</param>
        public static void ScopedSetupAll(List<DroneInfo> droneInfos)
        {
            foreach (DroneInfo droneInfo in droneInfos)
            {
                droneInfo.instance.PreSetup();
                bool isGood = droneInfo.instance.SetupFirstPhase();
                if (isGood)
                {
                    droneInfo.instance.SetupSecondPhase();
                    droneInfo.instance.SetupThirdPhase();
                }
                droneInfo.instance.PostSetup();
            }
        }
    }

    /// <summary>
    /// A structure that stores data of custom drones as well as where they originated from.
    /// </summary>
    public struct DroneInfo : IEquatable<DroneInfo>
    {
        /// <summary>
        /// Mod identifier for differentiation, preferably the GUID.
        /// </summary>
        public string mod;

        /// <summary>
        /// The instance of a Drone.
        /// </summary>
        public Drone instance;

        /// <summary>
        /// Basic constructor that stores the data of a custom drone.
        /// </summary>
        /// <param name="mod">Mod identifier</param>
        /// <param name="instance">Instance of the drone</param>
        /// <param name="config">Config file to bind onto</param>
        public DroneInfo(string mod, Drone instance, ConfigFile config)
        {
            this.mod = mod;
            this.instance = instance;
            this.instance.BindConfig(config);
        }

        /// <summary>
        /// Compares this instance and the other to see if they are "equal" as defined.
        /// For equality, always use this method instead of equality operators.
        /// </summary>
        /// <param name="other">The other instance being compared with</param>
        /// <returns>True if equal, false if not.</returns>
        public bool Equals(DroneInfo other) => mod == other.mod && instance.name == other.instance.name;
    }
}