using Prefix_Suffix_Fields_Name.Manager;

namespace Prefix_Suffix_Fields_Name
{
    /// <summary>
    /// Plugin settings, persisted by XrmToolBox between sessions.
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    public class Settings
    {
        public string LastUsedOrganizationWebappUrl { get; set; }

        public string LastText { get; set; }
        public bool LastIsSuffix { get; set; }
        public bool LastIsRemove { get; set; }

        /// <summary>State of the "Options" toggle of the Rename card.</summary>
        public bool CleanupEnabled { get; set; }

        /// <summary>Everything configured in the Options dialog.</summary>
        public CleanupOptions Options { get; set; } = new CleanupOptions();
    }
}
