#define DEBUG

using Chen.GradiusMod.Items.OptionSeed.Components;
using RoR2;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Items.OptionSeed
{
    public partial class OptionSeed
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
#if DEBUG
            On.EntityStates.EntityState.OnEnter += EntityState_OnEnter;
#endif
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (!self.master) return;
            if (GetCount(self) > 0) SeedTracker.SpawnSeeds(self.gameObject);
            else SeedTracker.DestroySeeds(self.gameObject);
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (!obj.master) return;
            if (GetCount(obj) > 0) SeedTracker.SpawnSeeds(obj.gameObject);
        }

#if DEBUG

        private void EntityState_OnEnter(On.EntityStates.EntityState.orig_OnEnter orig, EntityStates.EntityState self)
        {
            orig(self);
            Log.Message($"EntityState.OnEnter: {self.GetType().Name}");
        }

#endif

        internal static bool DebugHookCheck()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}