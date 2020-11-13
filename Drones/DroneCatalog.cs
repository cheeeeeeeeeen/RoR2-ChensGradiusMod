using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chen.GradiusMod
{
    public static class DroneCatalog
    {
        internal static List<DroneInfo> allInstances = new List<DroneInfo>();
        internal static List<DroneInfo> enabledInstances = new List<DroneInfo>();

        public static List<DroneInfo> Initialize(string modGuid, ConfigFile configFile)
        {
            List<DroneInfo> droneInfos = new List<DroneInfo>();
            bool filter(Type t) => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Drone));
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes().Where(filter))
            {
                Drone droneInstance = (Drone)Activator.CreateInstance(type);
                DroneInfo newDroneInfo = new DroneInfo(modGuid, droneInstance, configFile);
                droneInfos.Add(newDroneInfo);
                if (!allInstances.Exists(droneInfo => newDroneInfo.mod == droneInfo.mod && newDroneInfo.instance.name == droneInfo.instance.name))
                {
                    allInstances.Add(newDroneInfo);
                }
            }
            return droneInfos;
        }

        public static void SetupAll(List<DroneInfo> droneInfos)
        {
            foreach (DroneInfo droneInfo in droneInfos)
            {
                var droneInstance = droneInfo.instance;
                if (droneInstance.alreadySetup)
                {
                    Log.Warning($"{droneInfo.mod}:{droneInstance.name} was already set up. Skipping.");
                    continue;
                }
                if (droneInstance.SetupFirstPhase())
                {
                    droneInstance.SetupSecondPhase();
                    enabledInstances.Add(droneInfo);
                }
            }
        }
    }

    public class DroneInfo
    {
        public string mod;
        public Drone instance;

        public DroneInfo(string mod, Drone instance, ConfigFile config)
        {
            this.mod = mod;
            this.instance = instance;
            this.instance.BindConfig(config);
        }
    }
}