using Chen.Helpers.UnityHelpers;
using UnityEngine;

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
    }
}