using Chen.GradiusMod.Modules;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace Chen.GradiusMod.Artifacts
{
    /// <summary>
    /// Allows for making artifact classes into singleton classes.
    /// </summary>
    /// <typeparam name="T">The artifact class name</typeparam>
    public abstract class ArtifactBase<T> : ArtifactBase where T : ArtifactBase<T>
    {
        /// <summary>
        /// The instance of the singleton class.
        /// </summary>
        public static T instance { get; private set; }

        /// <summary>
        /// Constructor that creates the instance of the singleton class.
        /// </summary>
        protected ArtifactBase()
        {
            if (instance != null) throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" instantiated twice.");
            instance = this as T;
        }
    }

    /// <summary>
    /// The base class of the artifacts of this mod.
    /// </summary>
    public abstract class ArtifactBase : ModuleBase
    {
        /// <summary>
        /// The ArtifactDef of this artifact. Available after SetupAttributes.
        /// </summary>
        public ArtifactDef artifactDef { get; private set; }

        /// <summary>
        /// The display name of the artifact.
        /// </summary>
        public abstract string displayName { get; }

        /// <summary>
        /// The icon of the artifact when it is enabled.
        /// </summary>
        protected Sprite iconResource { get; set; }

        /// <summary>
        /// The icon of the artifact when it is disabled.
        /// </summary>
        protected Sprite iconResourceDisabled { get; set; }

        /// <inheritdoc/>
        protected override string configCategoryPrefix => "Artifacts";

        /// <summary>
        /// The prefix of the language tokens of the artifact.
        /// </summary>
        protected string tokenPrefix => $"CHENSGRADIUSMOD_{name.ToUpperInvariant()}";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        protected abstract string GetNameString(string langid = null);

        protected abstract string GetDescString(string langid = null);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc/>
        public override void SetupAttributes()
        {
            artifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
            artifactDef.cachedName = $"CGM{name}";
            artifactDef.nameToken = $"{tokenPrefix}_NAME_RENDERED";
            artifactDef.descriptionToken = $"{tokenPrefix}_DESC_RENDERED";
            artifactDef.smallIconSelectedSprite = iconResource;
            artifactDef.smallIconDeselectedSprite = iconResourceDisabled;

            LanguageAPI.Add(artifactDef.nameToken, GetNameString());
            LanguageAPI.Add(artifactDef.descriptionToken, GetDescString());

            ContentAddition.AddArtifactDef(artifactDef);
        }

        /// <summary>
        /// Checks if the artifact is enabled in the current run.
        /// </summary>
        /// <returns>True if the artifact is enabled in the current run.</returns>
        public bool IsActiveAndEnabled()
        {
            return enabled && artifactDef && RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(artifactDef);
        }
    }
}
