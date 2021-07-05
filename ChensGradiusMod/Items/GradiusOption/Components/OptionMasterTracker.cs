using Chen.Helpers.UnityHelpers;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Items.GradiusOption.Components
{
    internal class OptionMasterTracker : MonoBehaviour
    {
        public int optionItemCount = 0;

        public static void SpawnOption(GameObject owner, int itemCount)
        {
            OptionTracker ownerOptionTracker = owner.GetOrAddComponent<OptionTracker>();
            GameObject option = Instantiate(GradiusOption.gradiusOptionPrefab, owner.transform.position, owner.transform.rotation);
            OptionBehavior behavior = option.GetComponent<OptionBehavior>();
            behavior.owner = owner;
            behavior.numbering = itemCount;
            ownerOptionTracker.existingOptions.Add(option);
        }

        public static void DestroyOption(OptionTracker optionTracker, int optionNumber)
        {
            int index = optionNumber - 1;
            GameObject option = optionTracker.existingOptions[index];
            optionTracker.existingOptions.RemoveAt(index);
            Destroy(option);
        }

        public static void SpawnOptions(GameObject minion, int oldCount, int newCount)
        {
            AkSoundEngine.PostEvent(GradiusOption.GetOptionEventId, minion);
            for (int t = oldCount + 1; t <= newCount; t++) SpawnOption(minion, t);
        }

        public static void DestroyOptions(GameObject minion, int oldCount, int newCount)
        {
            AkSoundEngine.PostEvent(GradiusOption.LoseOptionEventId, minion);
            OptionTracker minionOptionTracker = minion.GetComponent<OptionTracker>();
            if (minionOptionTracker)
            {
                for (int t = oldCount; t > newCount; t--) DestroyOption(minionOptionTracker, t);
            }
            else Log.Warning($"OptionMasterTracker.DestroyOptions: OptionTracker not found for {minion.name}.");
        }
    }
}