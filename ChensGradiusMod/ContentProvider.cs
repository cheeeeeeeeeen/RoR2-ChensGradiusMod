using RoR2.ContentManagement;
using System.Collections.Generic;
using UnityEngine;

namespace Chen.GradiusMod
{
    internal class ContentProvider : GenericContentPackProvider
    {
        internal List<GameObject> bodyObjects = new List<GameObject>();
        internal List<GameObject> masterObjects = new List<GameObject>();

        protected override string ContentIdentifier() => GradiusModPlugin.ModGuid;

        protected override void LoadStaticContentAsyncActions(LoadStaticContentAsyncArgs args)
        {
            contentPack.bodyPrefabs.Add(bodyObjects.ToArray());
            contentPack.masterPrefabs.Add(masterObjects.ToArray());
        }
    }
}