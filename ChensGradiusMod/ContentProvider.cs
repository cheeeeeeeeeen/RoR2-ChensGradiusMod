using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chen.GradiusMod
{
    internal class ContentProvider : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();

        internal List<GameObject> bodyObjects = new List<GameObject>();

        internal List<GameObject> masterObjects = new List<GameObject>();

        public string identifier => GradiusModPlugin.ModGuid;

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.identifier = identifier;
            contentPack.bodyPrefabs.Add(bodyObjects.ToArray());
            contentPack.masterPrefabs.Add(masterObjects.ToArray());

            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}