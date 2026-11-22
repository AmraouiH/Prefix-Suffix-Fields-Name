using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Prefix_Suffix_Fields_Name.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using Label = Microsoft.Xrm.Sdk.Label;

namespace Prefix_Suffix_Fields_Name
{
    public static class PrefixSuffixFieldsNameManager
    {
        /// <summary>
        /// ExecuteMultiple accepts at most 1000 requests; keep a safety margin.
        /// </summary>
        private const int BatchSize = 500;

        public static EntityMetadata[] GetEntitiesMetadata(IOrganizationService service)
        {
            var request = new RetrieveMetadataChangesRequest
            {
                Query = new EntityQueryExpression
                {
                    Criteria = new MetadataFilterExpression(LogicalOperator.And),
                    Properties = new MetadataPropertiesExpression("LogicalName", "DisplayName")
                },
                ClientVersionStamp = null
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(request);
            return response.EntityMetadata.ToArray();
        }

        public static string GetLabel(Label label)
        {
            if (label == null)
                return null;
            if (label.UserLocalizedLabel != null)
                return label.UserLocalizedLabel.Label;
            return label.LocalizedLabels.FirstOrDefault()?.Label;
        }

        public static List<Field> GetEntityFields(IOrganizationService service, string entityLogicalName, BackgroundWorker worker = null)
        {
            worker?.ReportProgress(0, "Retrieving table metadata...");

            var response = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = entityLogicalName,
                RetrieveAsIfPublished = true
            });

            return FilterAttributes(response.EntityMetadata.Attributes)
                .Select(a => new Field
                {
                    DisplayName = GetLabel(a.DisplayName) ?? "N/A",
                    SchemaName = a.LogicalName,
                    Type = a.AttributeType?.ToString() ?? string.Empty,
                    IsSearchable = a.IsValidForAdvancedFind?.Value ?? false,
                    IsPrimaryName = a.IsPrimaryName ?? false,
                    RequiredLevel = a.RequiredLevel?.Value ?? AttributeRequiredLevel.None,
                    MetadataId = a.MetadataId ?? Guid.Empty,
                    IsAuditEnabled = a.IsAuditEnabled?.Value ?? false,
                    DescriptionLabel = a.Description,
                    DisplayNameLabel = a.DisplayName
                })
                .ToList();
        }

        public static IEnumerable<AttributeMetadata> FilterAttributes(AttributeMetadata[] attributes)
        {
            return attributes.Where(a =>
                    a.AttributeOf == null
                    && a.AttributeType != AttributeTypeCode.Virtual
                    && a.AttributeType != AttributeTypeCode.PartyList
                    && (a.IsValidForRead ?? false)
                    && a.LogicalName.IndexOf("composite", StringComparison.OrdinalIgnoreCase) < 0
                    && (a.IsRenameable?.Value ?? false))
                .OrderBy(a => a.LogicalName);
        }

        /// <summary>
        /// Returns the new label, or null when there is nothing to change
        /// (prefix already present on Add, prefix absent on Remove, empty result).
        /// Comparison is case-insensitive.
        /// </summary>
        public static string ComputeNewLabel(string label, string text, AffixPosition position, AffixAction action)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            label = label ?? string.Empty;
            const StringComparison cmp = StringComparison.OrdinalIgnoreCase;
            string result;

            if (action == AffixAction.Add)
            {
                if (position == AffixPosition.Prefix)
                    result = label.StartsWith(text, cmp) ? null : text + label;
                else
                    result = label.EndsWith(text, cmp) ? null : label + text;
            }
            else
            {
                if (position == AffixPosition.Prefix)
                    result = label.StartsWith(text, cmp) ? label.Substring(text.Length) : null;
                else
                    result = label.EndsWith(text, cmp) ? label.Substring(0, label.Length - text.Length) : null;
            }

            return string.IsNullOrWhiteSpace(result) ? null : result;
        }

        /// <summary>
        /// Returns the new display name (user language) of each field, or null when it does not change.
        /// </summary>
        public static Dictionary<string, string> PreviewDisplayNames(IList<Field> fields, string text, AffixPosition position, AffixAction action, int baseLanguageCode)
        {
            var result = new Dictionary<string, string>();
            foreach (var f in fields)
            {
                var label = string.IsNullOrEmpty(text) ? null : BuildNewLabel(f, text, position, action, baseLanguageCode, out _);
                result[f.SchemaName] = label == null ? null : GetNewName(label);
            }
            return result;
        }

        private static string GetNewName(Label label) => label.UserLocalizedLabel?.Label ?? label.LocalizedLabels.FirstOrDefault()?.Label;

        /// <summary>
        /// 1. Removes the fields from forms / views (cleanup options),
        /// 2. updates the field properties: display name (all translations), requirement, Advanced Find,
        /// 3. publishes the table.
        /// </summary>
        public static List<UpdateResult> UpdateAttributes(
            IOrganizationService service,
            string entityLogicalName,
            IList<Field> fields,
            string text,
            AffixPosition position,
            AffixAction action,
            CleanupOptions cleanup,
            int baseLanguageCode,
            BackgroundWorker worker)
        {
            cleanup = cleanup ?? CleanupOptions.None;
            var results = new List<UpdateResult>();

            // 0. Backup (a failed backup stops everything: nothing is changed without it)
            if (cleanup.AutoBackup)
            {
                var folder = BackupManager.Backup(service, entityLogicalName, fields, cleanup.EffectiveBackupFolder(), worker);
                results.Add(new UpdateResult { SchemaName = "-", Operation = "Backup", Status = ChangeStatus.Updated, Message = folder });
            }

            // 1. Components: forms, views, charts, plug-in steps, mappings, duplicate rules, field security
            results.AddRange(UsageManager.CleanupComponents(service, entityLogicalName, fields, cleanup, worker));

            // 2. Field properties
            worker.ReportProgress(0, "Preparing field updates...");
            var pending = new List<KeyValuePair<UpdateResult, OrganizationRequest>>();

            foreach (var field in fields)
            {
                var result = new UpdateResult
                {
                    SchemaName = field.SchemaName,
                    Operation = "Field properties",
                    OldDisplayName = field.DisplayName,
                    NewDisplayName = field.DisplayName,
                    ComponentType = UsageManager.ComponentAttribute,
                    ComponentId = field.MetadataId
                };

                var newLabel = string.IsNullOrEmpty(text) ? null : BuildNewLabel(field, text, position, action, baseLanguageCode, out _);
                var changeSearch = cleanup.RemoveFromAdvancedFind && field.IsSearchable;
                var changeRequired = cleanup.MakeNotRequired
                    && field.RequiredLevel != AttributeRequiredLevel.None
                    && field.RequiredLevel != AttributeRequiredLevel.SystemRequired;

                var changes = new List<string>();
                var attribute = new AttributeMetadata { LogicalName = field.SchemaName };
                if (newLabel != null)
                {
                    attribute.DisplayName = newLabel;
                    result.NewDisplayName = GetNewName(newLabel);
                    changes.Add($"Display name: {field.DisplayName} → {result.NewDisplayName}");
                }
                if (changeRequired)
                {
                    attribute.RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);
                    changes.Add("Requirement: Optional");
                }
                if (changeSearch)
                {
                    attribute.IsValidForAdvancedFind = new BooleanManagedProperty(false);
                    changes.Add("Advanced Find: off");
                }
                if (cleanup.DisableAuditing && field.IsAuditEnabled)
                {
                    attribute.IsAuditEnabled = new BooleanManagedProperty(false);
                    changes.Add("Auditing: off");
                }
                if (cleanup.AddDeprecationNote)
                {
                    var description = BuildDeprecationDescription(field, cleanup, baseLanguageCode);
                    if (description != null)
                    {
                        attribute.Description = description;
                        changes.Add("Description: deprecation note added");
                    }
                }

                if (changes.Count == 0)
                {
                    // Nothing to report when only a cleanup of forms/views was asked.
                    if (!string.IsNullOrEmpty(text))
                    {
                        result.Status = ChangeStatus.Skipped;
                        result.Message = action == AffixAction.Add
                            ? "Display name already contains the text, nothing to change"
                            : "Display name does not contain the text, nothing to change";
                        results.Add(result);
                    }
                    continue;
                }

                result.Message = string.Join(" · ", changes);
                results.Add(result);
                pending.Add(new KeyValuePair<UpdateResult, OrganizationRequest>(result, new UpdateAttributeRequest
                {
                    Attribute = attribute,
                    EntityName = entityLogicalName,
                    // Keep labels of languages we do not send.
                    MergeLabels = true
                }));
            }

            for (int start = 0; start < pending.Count; start += BatchSize)
            {
                var batch = pending.Skip(start).Take(BatchSize).ToList();
                worker.ReportProgress(0, $"Updating fields {start + 1}-{start + batch.Count} of {pending.Count}...");

                var multiple = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings { ContinueOnError = true, ReturnResponses = true },
                    Requests = new OrganizationRequestCollection()
                };
                multiple.Requests.AddRange(batch.Select(p => p.Value));

                var response = (ExecuteMultipleResponse)service.Execute(multiple);

                for (int i = 0; i < batch.Count; i++)
                {
                    var item = response.Responses.FirstOrDefault(r => r.RequestIndex == i);
                    var result = batch[i].Key;
                    if (item?.Fault != null)
                    {
                        result.Status = ChangeStatus.Failed;
                        result.NewDisplayName = result.OldDisplayName;
                        result.Message = item.Fault.Message;
                    }
                    else
                    {
                        result.Status = ChangeStatus.Updated;
                    }
                }
            }

            var changed = results.Where(r => r.Status == ChangeStatus.Updated && r.ComponentType != 0 && r.ComponentId != Guid.Empty)
                .GroupBy(r => new { r.ComponentType, r.ComponentId })
                .ToList();

            // 3. Solution
            if (!string.IsNullOrWhiteSpace(cleanup.SolutionUniqueName) && changed.Count > 0)
            {
                worker.ReportProgress(0, $"Adding {changed.Count} component(s) to the solution {cleanup.SolutionUniqueName}...");
                int added = 0;
                var errors = new List<string>();
                foreach (var c in changed)
                {
                    try
                    {
                        service.Execute(new AddSolutionComponentRequest
                        {
                            ComponentType = c.Key.ComponentType,
                            ComponentId = c.Key.ComponentId,
                            SolutionUniqueName = cleanup.SolutionUniqueName,
                            AddRequiredComponents = false
                        });
                        added++;
                    }
                    catch (Exception ex) { errors.Add($"{c.First().Operation}: {ex.Message}"); }
                }
                results.Add(new UpdateResult
                {
                    SchemaName = "-",
                    Operation = $"Solution · {cleanup.SolutionUniqueName}",
                    Status = errors.Count == 0 ? ChangeStatus.Updated : ChangeStatus.Failed,
                    Message = $"{added} component(s) added" + (errors.Count > 0 ? " · " + string.Join(" | ", errors) : string.Empty)
                });
            }

            // 4. Publish
            if (results.Any(r => r.Status == ChangeStatus.Updated && r.SchemaName != "-"))
            {
                if (cleanup.Publish)
                {
                    worker.ReportProgress(0, "Publishing table customizations...");
                    service.Execute(new PublishXmlRequest
                    {
                        ParameterXml = $"<importexportxml><entities><entity>{SecurityElement.Escape(entityLogicalName)}</entity></entities></importexportxml>"
                    });
                    results.Add(new UpdateResult { SchemaName = "-", Operation = "Publish", Status = ChangeStatus.Updated, Message = $"Table {entityLogicalName} published" });
                }
                else
                {
                    results.Add(new UpdateResult { SchemaName = "-", Operation = "Publish", Status = ChangeStatus.Skipped, Message = "Not published (option): publish the table yourself" });
                }
            }

            return results;
        }

        /// <summary>
        /// Adds the deprecation note before the description, in every language the field is translated in.
        /// Returns null when every translation already starts with the note.
        /// </summary>
        private static Label BuildDeprecationDescription(Field field, CleanupOptions options, int baseLanguageCode)
        {
            var note = options.DeprecationText();
            var prefix = options.DeprecationPrefix();
            var languages = (field.DisplayNameLabel?.LocalizedLabels.Select(l => l.LanguageCode) ?? Enumerable.Empty<int>())
                .Concat(field.DescriptionLabel?.LocalizedLabels.Select(l => l.LanguageCode) ?? Enumerable.Empty<int>())
                .DefaultIfEmpty(baseLanguageCode)
                .Distinct();

            var labels = new List<LocalizedLabel>();
            foreach (var lcid in languages)
            {
                var current = field.DescriptionLabel?.LocalizedLabels.FirstOrDefault(l => l.LanguageCode == lcid)?.Label ?? string.Empty;
                if (current.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;
                labels.Add(new LocalizedLabel($"{note} {current}".Trim(), lcid));
            }
            if (labels.Count == 0)
                return null;

            var user = labels.FirstOrDefault(l => l.LanguageCode == (field.DescriptionLabel?.UserLocalizedLabel?.LanguageCode ?? baseLanguageCode)) ?? labels[0];
            return new Label(user, labels.ToArray());
        }

        /// <summary>
        /// Applies the prefix/suffix to every translation of the display name.
        /// Returns null when no translation changes.
        /// </summary>
        private static Label BuildNewLabel(Field field, string text, AffixPosition position, AffixAction action, int baseLanguageCode, out string newDisplayName)
        {
            newDisplayName = field.DisplayName;
            var localized = field.DisplayNameLabel?.LocalizedLabels?.ToList() ?? new List<LocalizedLabel>();
            if (localized.Count == 0)
            {
                localized.Add(new LocalizedLabel(string.Empty, baseLanguageCode));
            }

            var newLabels = new List<LocalizedLabel>();
            foreach (var l in localized)
            {
                var computed = ComputeNewLabel(l.Label, text, position, action);
                if (computed != null)
                    newLabels.Add(new LocalizedLabel(computed, l.LanguageCode));
            }

            if (newLabels.Count == 0)
                return null;

            var userLcid = field.DisplayNameLabel?.UserLocalizedLabel?.LanguageCode ?? baseLanguageCode;
            var userLabel = newLabels.FirstOrDefault(l => l.LanguageCode == userLcid) ?? newLabels[0];
            newDisplayName = userLabel.Label;

            return new Label(userLabel, newLabels.ToArray());
        }
    }
}
