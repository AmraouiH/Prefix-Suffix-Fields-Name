using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prefix_Suffix_Fields_Name.Manager
{
    public enum AffixPosition
    {
        Prefix,
        Suffix
    }

    public enum AffixAction
    {
        Add,
        Remove
    }

    public class Field
    {
        public Guid MetadataId { get; set; }
        public string DisplayName { get; set; }
        public string SchemaName { get; set; }
        public string Type { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsPrimaryName { get; set; }
        public bool IsAuditEnabled { get; set; }
        public AttributeRequiredLevel RequiredLevel { get; set; }

        /// <summary>Full display name label (all languages), used to keep translations when updating.</summary>
        public Label DisplayNameLabel { get; set; }

        /// <summary>Full description label (all languages).</summary>
        public Label DescriptionLabel { get; set; }
    }

    /// <summary>
    /// Everything configured in the Options dialog (XML serialized in the plugin settings).
    /// </summary>
    public class CleanupOptions
    {
        #region Clean up actions (applied when the "Options" toggle is on)
        public bool RemoveFromForms { get; set; }
        public bool HideOnForms { get; set; }
        public bool RemoveFromViews { get; set; }
        public bool RemoveFromCharts { get; set; }
        public bool RemoveFromSteps { get; set; }
        public bool RemoveFromMappings { get; set; }
        public bool RemoveFromDuplicateRules { get; set; }
        public bool RemoveFromFieldSecurity { get; set; }
        public bool MakeNotRequired { get; set; }
        public bool RemoveFromAdvancedFind { get; set; } = true;
        public bool DisableAuditing { get; set; }
        public bool AddDeprecationNote { get; set; }
        public string DeprecationNote { get; set; } = "Deprecated on {date} - do not use.";
        #endregion

        #region Safety (always applied)
        public bool AutoBackup { get; set; } = true;
        public string BackupFolder { get; set; }
        public bool SkipManaged { get; set; } = true;
        public string SolutionUniqueName { get; set; }
        public bool Publish { get; set; } = true;
        public bool DryRun { get; set; }
        #endregion

        #region Analysis
        public bool ShowDataUsage { get; set; } = true;
        #endregion

        /// <summary>True when at least one clean up action is on.</summary>
        public bool Any =>
            RemoveFromForms || HideOnForms || RemoveFromViews || RemoveFromCharts || RemoveFromSteps ||
            RemoveFromMappings || RemoveFromDuplicateRules || RemoveFromFieldSecurity ||
            MakeNotRequired || RemoveFromAdvancedFind || DisableAuditing || AddDeprecationNote;

        /// <summary>No clean up action, safety defaults.</summary>
        public static CleanupOptions None => new CleanupOptions { RemoveFromAdvancedFind = false, AutoBackup = false, ShowDataUsage = false };

        /// <summary>Copy used for one run: clean up actions are cleared when the toggle is off, safety options are kept.</summary>
        public CleanupOptions ForRun(bool actionsEnabled)
        {
            var copy = (CleanupOptions)MemberwiseClone();
            if (!actionsEnabled)
            {
                copy.RemoveFromForms = copy.HideOnForms = copy.RemoveFromViews = copy.RemoveFromCharts = copy.RemoveFromSteps = false;
                copy.RemoveFromMappings = copy.RemoveFromDuplicateRules = copy.RemoveFromFieldSecurity = false;
                copy.MakeNotRequired = copy.RemoveFromAdvancedFind = copy.DisableAuditing = copy.AddDeprecationNote = false;
            }
            return copy;
        }

        public string Summary()
        {
            var parts = new List<string>();
            if (RemoveFromForms) parts.Add("forms");
            else if (HideOnForms) parts.Add("hide on forms");
            if (RemoveFromViews) parts.Add("views");
            if (RemoveFromCharts) parts.Add("charts");
            if (RemoveFromSteps) parts.Add("plug-in steps");
            if (RemoveFromMappings) parts.Add("mappings");
            if (RemoveFromDuplicateRules) parts.Add("duplicate rules");
            if (RemoveFromFieldSecurity) parts.Add("field security");
            if (MakeNotRequired) parts.Add("optional");
            if (RemoveFromAdvancedFind) parts.Add("Advanced Find");
            if (DisableAuditing) parts.Add("no audit");
            if (AddDeprecationNote) parts.Add("deprecation note");
            return string.Join(", ", parts);
        }

        /// <summary>The deprecation note with today's date.</summary>
        public string DeprecationText() =>
            (string.IsNullOrWhiteSpace(DeprecationNote) ? "Deprecated on {date}" : DeprecationNote).Replace("{date}", DateTime.Today.ToString("yyyy-MM-dd"));

        /// <summary>Start of the note, before the date: used to detect a note already added.</summary>
        public string DeprecationPrefix()
        {
            var note = string.IsNullOrWhiteSpace(DeprecationNote) ? "Deprecated on {date}" : DeprecationNote;
            var i = note.IndexOf("{date}", StringComparison.Ordinal);
            return (i >= 0 ? note.Substring(0, i) : note).Trim();
        }

        public string EffectiveBackupFolder() =>
            string.IsNullOrWhiteSpace(BackupFolder)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Bulk Prefix-Suffix backups")
                : BackupFolder;
    }

    public enum UsageAction
    {
        /// <summary>The tool will change it.</summary>
        WillChange,
        /// <summary>Found, but left as is (option off, managed, or must be reviewed manually).</summary>
        Kept,
        /// <summary>Information only.</summary>
        Info
    }

    /// <summary>One place where a field is used (form, view, chart, process...) or one property change.</summary>
    public class FieldUsage
    {
        public string SchemaName { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public UsageAction Action { get; set; }
        public string Detail { get; set; }

        // Data usage (Category "Data usage" only): shown in the dialog header, not in the grid.
        public long? Filled { get; set; }
        public long? Total { get; set; }
        public DateTime? LastModified { get; set; }
    }

    public enum ChangeStatus
    {
        Updated,
        Skipped,
        Failed
    }

    public class UpdateResult
    {
        public string SchemaName { get; set; }
        public string Operation { get; set; }
        public ChangeStatus Status { get; set; }
        public string Message { get; set; }

        public string OldDisplayName { get; set; }
        public string NewDisplayName { get; set; }

        /// <summary>Solution component type and id of the changed component (0 / empty when none).</summary>
        public int ComponentType { get; set; }
        public Guid ComponentId { get; set; }
    }
}
