using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Prefix_Suffix_Fields_Name.Manager
{
    /// <summary>
    /// Finds every place fields are used and cleans them up (forms, views, charts, plug-in steps,
    /// relationship mappings, duplicate detection rules, field security).
    /// </summary>
    public static class UsageManager
    {
        private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

        // Solution component types
        public const int ComponentAttribute = 2;
        public const int ComponentView = 26;
        public const int ComponentChart = 59;
        public const int ComponentForm = 60;
        public const int ComponentStep = 92;
        public const int ComponentStepImage = 93;

        public const string DataUsageCategory = "Data usage";

        #region Priority

        /// <summary>
        /// Sort rank of a usage, most important first:
        /// 1 blocks the deletion of the field, 2 code / automation that can break,
        /// 3 user interface, 4 configuration, 5 personal items, 6 field properties, 7 information.
        /// </summary>
        public static int PriorityRank(FieldUsage usage)
        {
            if (usage.Action == UsageAction.Info || usage.SchemaName == "-")
                return 7;

            var c = usage.Category ?? string.Empty;
            if (c.StartsWith("Field (") || c == "Alternate key" || c == "Relationship" || c == "Relationship mapping")
                return 1;
            if (c.StartsWith("Plug-in step") || c == "Business rule" || c == "Classic workflow" || c == "Action" || c == "Business process flow"
                || c == "Cloud flow" || c == "Dialog" || c == "Desktop flow" || c == "Process" || c.StartsWith("Form script") || c == "Web resource" || c.StartsWith("Component"))
                return 2;
            if (c.Contains("form") || c.Contains("view") || c == "Chart")
                return c.StartsWith("Personal") ? 5 : 3;
            if (c == "Duplicate detection rule" || c == "Field security profile" || c == "E-mail template")
                return 4;
            if (c.StartsWith("Personal"))
                return 5;
            return 6; // Display name, Requirement, Advanced Find, Auditing, Description
        }

        public static string PriorityLabel(int rank) => rank <= 2 ? "High" : rank <= 4 ? "Medium" : "Low";

        /// <summary>Most important first; within a rank, items to review manually before automatic changes.</summary>
        public static IEnumerable<FieldUsage> SortByPriority(IEnumerable<FieldUsage> usages) =>
            usages.OrderBy(PriorityRank)
                .ThenBy(u => u.Action == UsageAction.Kept ? 0 : u.Action == UsageAction.WillChange ? 1 : 2)
                .ThenBy(u => u.Category)
                .ThenBy(u => u.SchemaName)
                .ThenBy(u => u.Name);

        #endregion

        #region Labels

        public static string FormTypeLabel(int type)
        {
            switch (type)
            {
                case 2: return "Main form";
                case 5: return "Mobile form";
                case 6: return "Quick View form";
                case 7: return "Quick Create form";
                case 11: return "Card form";
                case 12: return "Main form (interactive)";
                default: return "Form";
            }
        }

        public static string ViewTypeLabel(int queryType)
        {
            switch (queryType)
            {
                case 0: return "Public view";
                case 1: return "Advanced Find view";
                case 2: return "Associated view";
                case 4: return "Quick Find view";
                case 64: return "Lookup view";
                default: return "View";
            }
        }

        private static string WorkflowCategoryLabel(int category)
        {
            switch (category)
            {
                case 0: return "Classic workflow";
                case 1: return "Dialog";
                case 2: return "Business rule";
                case 3: return "Action";
                case 4: return "Business process flow";
                case 5: return "Cloud flow";
                case 6: return "Desktop flow";
                default: return "Process";
            }
        }

        #endregion

        #region Helpers

        /// <summary>The field name as a quoted token ("fax" or 'fax'), to avoid matching other words.</summary>
        private static Regex QuotedToken(string field) =>
            new Regex("[\"']" + Regex.Escape(field) + "[\"']", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static XDocument Parse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return null;
            try { return XDocument.Parse(xml); }
            catch { return null; }
        }

        private static bool Same(string a, string b) => string.Equals(a, b, IgnoreCase);

        private static bool CsvContains(string csv, string field) =>
            !string.IsNullOrWhiteSpace(csv) && csv.Split(',').Any(a => Same(a.Trim(), field));

        private static string CsvRemove(string csv, string field) =>
            string.Join(",", csv.Split(',').Select(a => a.Trim()).Where(a => a.Length > 0 && !Same(a, field)));

        private static int CsvCount(string csv) =>
            string.IsNullOrWhiteSpace(csv) ? 0 : csv.Split(',').Count(a => a.Trim().Length > 0);

        /// <summary>Runs one analysis step; a failure becomes an "Info" line instead of stopping everything.</summary>
        private static void Safe(List<FieldUsage> usages, string category, Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                usages.Add(new FieldUsage { SchemaName = "-", Category = category, Name = "Could not be analysed", Action = UsageAction.Info, Detail = ex.Message });
            }
        }

        /// <summary>Decides what happens to a component and explains why.</summary>
        private static UsageAction Decide(bool optionOn, string optionName, bool removable, bool managed, CleanupOptions options, List<string> details)
        {
            if (!optionOn)
            {
                details.Add($"Option “{optionName}” is off");
                return UsageAction.Kept;
            }
            if (managed && options.SkipManaged)
            {
                details.Add("managed component: skipped (option “Skip managed components”)");
                return UsageAction.Kept;
            }
            return removable ? UsageAction.WillChange : UsageAction.Kept;
        }

        private static string Join(List<string> details) => string.Join(" · ", details.Where(d => !string.IsNullOrEmpty(d)));

        #endregion

        #region Retrieve components

        private class FormInfo { public Guid Id; public string Name; public int Type; public string Xml; public bool Managed; }
        private class ViewInfo { public Guid Id; public string Name; public int QueryType; public string FetchXml; public string LayoutXml; public bool Managed; }
        private class ChartInfo { public Guid Id; public string Name; public string Data; public bool Managed; }
        private class StepInfo { public Guid Id; public string Name; public string Filtering; public bool Managed; }
        private class ImageInfo { public Guid Id; public string Name; public string Step; public string Attributes; public bool Managed; }
        private class MappingInfo { public Guid Id; public string Source; public string Target; public string SourceAttribute; public string TargetAttribute; public bool System; }
        private class DuplicateConditionInfo { public Guid Id; public Guid RuleId; public string Rule; public int RuleState; public string BaseEntity; public string MatchingEntity; public string BaseAttribute; public string MatchingAttribute; }
        private class PermissionInfo { public Guid Id; public string Profile; public string Attribute; }

        private static List<FormInfo> GetForms(IOrganizationService service, string entity)
        {
            var query = new QueryExpression("systemform")
            {
                ColumnSet = new ColumnSet("name", "type", "formxml", "ismanaged"),
                Criteria = { Conditions = { new ConditionExpression("objecttypecode", ConditionOperator.Equal, entity) } }
            };
            return service.RetrieveMultiple(query).Entities
                .Where(e => !string.IsNullOrEmpty(e.GetAttributeValue<string>("formxml")))
                .Select(e => new FormInfo
                {
                    Id = e.Id,
                    Name = e.GetAttributeValue<string>("name"),
                    Type = e.GetAttributeValue<OptionSetValue>("type")?.Value ?? -1,
                    Xml = e.GetAttributeValue<string>("formxml"),
                    Managed = e.GetAttributeValue<bool>("ismanaged")
                })
                .ToList();
        }

        private static List<ViewInfo> GetViews(IOrganizationService service, string entity, bool personal = false)
        {
            var columns = new ColumnSet("name", "querytype", "fetchxml", "layoutxml");
            if (!personal) columns.AddColumn("ismanaged");
            var query = new QueryExpression(personal ? "userquery" : "savedquery")
            {
                ColumnSet = columns,
                Criteria = { Conditions = { new ConditionExpression("returnedtypecode", ConditionOperator.Equal, entity) } }
            };
            return service.RetrieveMultiple(query).Entities
                .Select(e => new ViewInfo
                {
                    Id = e.Id,
                    Name = e.GetAttributeValue<string>("name"),
                    QueryType = e.GetAttributeValue<int>("querytype"),
                    FetchXml = e.GetAttributeValue<string>("fetchxml"),
                    LayoutXml = e.GetAttributeValue<string>("layoutxml"),
                    Managed = e.GetAttributeValue<bool>("ismanaged")
                })
                .ToList();
        }

        private static List<ChartInfo> GetCharts(IOrganizationService service, string entity, bool personal = false)
        {
            var columns = new ColumnSet("name", "datadescription");
            if (!personal) columns.AddColumn("ismanaged");
            return service.RetrieveMultiple(new QueryExpression(personal ? "userqueryvisualization" : "savedqueryvisualization")
            {
                ColumnSet = columns,
                Criteria = { Conditions = { new ConditionExpression("primaryentitytypecode", ConditionOperator.Equal, entity) } }
            }).Entities.Select(e => new ChartInfo
            {
                Id = e.Id,
                Name = e.GetAttributeValue<string>("name"),
                Data = e.GetAttributeValue<string>("datadescription"),
                Managed = e.GetAttributeValue<bool>("ismanaged")
            }).ToList();
        }

        private static List<StepInfo> GetSteps(IOrganizationService service, string entity)
        {
            var query = new QueryExpression("sdkmessageprocessingstep")
            {
                ColumnSet = new ColumnSet("name", "filteringattributes", "ismanaged"),
                Criteria = { Conditions = { new ConditionExpression("filteringattributes", ConditionOperator.NotNull) } }
            };
            var filter = query.AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid");
            filter.LinkCriteria.AddCondition("primaryobjecttypecode", ConditionOperator.Equal, entity);
            return service.RetrieveMultiple(query).Entities
                .Select(e => new StepInfo { Id = e.Id, Name = e.GetAttributeValue<string>("name"), Filtering = e.GetAttributeValue<string>("filteringattributes"), Managed = e.GetAttributeValue<bool>("ismanaged") })
                .ToList();
        }

        private static List<ImageInfo> GetStepImages(IOrganizationService service, string entity)
        {
            var query = new QueryExpression("sdkmessageprocessingstepimage")
            {
                ColumnSet = new ColumnSet("name", "attributes", "ismanaged"),
                Criteria = { Conditions = { new ConditionExpression("attributes", ConditionOperator.NotNull) } }
            };
            var step = query.AddLink("sdkmessageprocessingstep", "sdkmessageprocessingstepid", "sdkmessageprocessingstepid");
            step.EntityAlias = "step";
            step.Columns = new ColumnSet("name");
            var filter = step.AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid");
            filter.LinkCriteria.AddCondition("primaryobjecttypecode", ConditionOperator.Equal, entity);
            return service.RetrieveMultiple(query).Entities
                .Select(e => new ImageInfo
                {
                    Id = e.Id,
                    Name = e.GetAttributeValue<string>("name"),
                    Step = e.GetAttributeValue<AliasedValue>("step.name")?.Value as string,
                    Attributes = e.GetAttributeValue<string>("attributes"),
                    Managed = e.GetAttributeValue<bool>("ismanaged")
                })
                .ToList();
        }

        private static List<MappingInfo> GetMappings(IOrganizationService service, string entity, IList<Field> fields)
        {
            var names = fields.Select(f => (object)f.SchemaName).ToArray();
            var query = new QueryExpression("attributemap")
            {
                ColumnSet = new ColumnSet("sourceattributename", "targetattributename", "issystem"),
                Criteria =
                {
                    FilterOperator = LogicalOperator.Or,
                    Conditions =
                    {
                        new ConditionExpression("sourceattributename", ConditionOperator.In, names),
                        new ConditionExpression("targetattributename", ConditionOperator.In, names)
                    }
                }
            };
            var map = query.AddLink("entitymap", "entitymapid", "entitymapid");
            map.EntityAlias = "map";
            map.Columns = new ColumnSet("sourceentityname", "targetentityname");

            return service.RetrieveMultiple(query).Entities.Select(m => new MappingInfo
            {
                Id = m.Id,
                Source = m.GetAttributeValue<AliasedValue>("map.sourceentityname")?.Value as string,
                Target = m.GetAttributeValue<AliasedValue>("map.targetentityname")?.Value as string,
                SourceAttribute = m.GetAttributeValue<string>("sourceattributename"),
                TargetAttribute = m.GetAttributeValue<string>("targetattributename"),
                System = m.GetAttributeValue<bool>("issystem")
            })
            .Where(m => Same(m.Source, entity) || Same(m.Target, entity))
            .ToList();
        }

        private static bool MappingUses(MappingInfo m, string entity, string field) =>
            (Same(m.Source, entity) && Same(m.SourceAttribute, field)) || (Same(m.Target, entity) && Same(m.TargetAttribute, field));

        private static List<DuplicateConditionInfo> GetDuplicateConditions(IOrganizationService service)
        {
            var query = new QueryExpression("duplicaterulecondition") { ColumnSet = new ColumnSet("baseattributename", "matchingattributename", "regardingobjectid") };
            var rule = query.AddLink("duplicaterule", "regardingobjectid", "duplicateruleid");
            rule.EntityAlias = "rule";
            rule.Columns = new ColumnSet("name", "baseentityname", "matchingentityname", "statecode");
            return service.RetrieveMultiple(query).Entities.Select(c => new DuplicateConditionInfo
            {
                Id = c.Id,
                RuleId = c.GetAttributeValue<EntityReference>("regardingobjectid")?.Id ?? Guid.Empty,
                Rule = c.GetAttributeValue<AliasedValue>("rule.name")?.Value as string,
                RuleState = (c.GetAttributeValue<AliasedValue>("rule.statecode")?.Value as OptionSetValue)?.Value ?? 0,
                BaseEntity = c.GetAttributeValue<AliasedValue>("rule.baseentityname")?.Value as string,
                MatchingEntity = c.GetAttributeValue<AliasedValue>("rule.matchingentityname")?.Value as string,
                BaseAttribute = c.GetAttributeValue<string>("baseattributename"),
                MatchingAttribute = c.GetAttributeValue<string>("matchingattributename")
            }).ToList();
        }

        private static bool ConditionUses(DuplicateConditionInfo c, string entity, string field) =>
            (Same(c.BaseEntity, entity) && Same(c.BaseAttribute, field)) || (Same(c.MatchingEntity, entity) && Same(c.MatchingAttribute, field));

        private static List<PermissionInfo> GetFieldPermissions(IOrganizationService service, string entity)
        {
            var query = new QueryExpression("fieldpermission")
            {
                ColumnSet = new ColumnSet("attributelogicalname"),
                Criteria = { Conditions = { new ConditionExpression("entityname", ConditionOperator.Equal, entity) } }
            };
            var profile = query.AddLink("fieldsecurityprofile", "fieldsecurityprofileid", "fieldsecurityprofileid");
            profile.EntityAlias = "profile";
            profile.Columns = new ColumnSet("name");
            return service.RetrieveMultiple(query).Entities.Select(p => new PermissionInfo
            {
                Id = p.Id,
                Attribute = p.GetAttributeValue<string>("attributelogicalname"),
                Profile = p.GetAttributeValue<AliasedValue>("profile.name")?.Value as string
            }).ToList();
        }

        #endregion

        #region Form XML

        private static IEnumerable<XElement> FormControls(XDocument form, string field) =>
            form.Descendants("control").Where(c => Same((string)c.Attribute("datafieldname"), field));

        private static IEnumerable<XElement> FormCells(XDocument form, string field) =>
            FormControls(form, field).Select(c => c.Ancestors("cell").FirstOrDefault() ?? c).Distinct();

        private static bool FormHasEvents(XDocument form, string field) =>
            form.Descendants("event").Any(e => Same((string)e.Attribute("attribute"), field));

        /// <summary>Removes the cells holding the field controls. Returns true if the form changed.</summary>
        private static bool RemoveFromForm(XDocument form, string field)
        {
            var cells = FormCells(form, field).ToList();
            foreach (var cell in cells)
                cell.Remove();
            return cells.Count > 0;
        }

        /// <summary>Hides the cells holding the field controls (scripts keep working). Returns true if the form changed.</summary>
        private static bool HideOnForm(XDocument form, string field)
        {
            var changed = false;
            foreach (var cell in FormCells(form, field).Where(c => c.Name.LocalName == "cell" && (string)c.Attribute("visible") != "false"))
            {
                cell.SetAttributeValue("visible", "false");
                changed = true;
            }
            return changed;
        }

        private static IEnumerable<string> FormLibraries(XDocument form) =>
            form.Descendants("Library").Select(l => (string)l.Attribute("name")).Where(n => !string.IsNullOrEmpty(n));

        #endregion

        #region View / chart XML

        private class ViewMatch
        {
            public bool InColumns, InSort, InQuickFind, InFilter;
            public bool Any => InColumns || InSort || InQuickFind || InFilter;
            public bool Removable => InColumns || InSort || InQuickFind;

            public string Describe()
            {
                var where = new List<string>();
                if (InColumns) where.Add("column");
                if (InSort) where.Add("sort");
                if (InQuickFind) where.Add("Quick Find search");
                if (InFilter) where.Add("filter (kept)");
                return string.Join(", ", where);
            }
        }

        private static XElement RootEntity(XDocument fetch) => fetch.Root?.Element("entity");

        private static IEnumerable<XElement> RootConditions(XElement entity, string field) =>
            entity.Descendants("condition")
                .Where(c => Same((string)c.Attribute("attribute"), field) && c.Attribute("entityname") == null && !c.Ancestors("link-entity").Any());

        private static bool IsQuickFindCondition(XElement condition) =>
            condition.Ancestors("filter").Any(f => (string)f.Attribute("isquickfindfields") == "1");

        private static ViewMatch MatchView(ViewInfo view, string field)
        {
            var match = new ViewMatch();
            var fetch = Parse(view.FetchXml);
            var entity = fetch == null ? null : RootEntity(fetch);
            if (entity != null)
            {
                match.InColumns = entity.Elements("attribute").Any(a => Same((string)a.Attribute("name"), field));
                match.InSort = entity.Elements("order").Any(o => Same((string)o.Attribute("attribute"), field));
                foreach (var c in RootConditions(entity, field))
                {
                    if (IsQuickFindCondition(c)) match.InQuickFind = true;
                    else match.InFilter = true;
                }
            }

            var layout = Parse(view.LayoutXml);
            if (layout != null && layout.Descendants("cell").Any(c => Same((string)c.Attribute("name"), field)))
                match.InColumns = true;

            return match;
        }

        private static bool RemoveFromView(ref string fetchXml, ref string layoutXml, string field)
        {
            var changed = false;

            var fetch = Parse(fetchXml);
            var entity = fetch == null ? null : RootEntity(fetch);
            if (entity != null)
            {
                var nodes = entity.Elements("attribute").Where(a => Same((string)a.Attribute("name"), field))
                    .Concat(entity.Elements("order").Where(o => Same((string)o.Attribute("attribute"), field)))
                    .Concat(RootConditions(entity, field).Where(IsQuickFindCondition))
                    .ToList();
                foreach (var n in nodes)
                    n.Remove();
                if (nodes.Count > 0)
                {
                    fetchXml = fetch.ToString(SaveOptions.DisableFormatting);
                    changed = true;
                }
            }

            var layout = Parse(layoutXml);
            if (layout != null)
            {
                var cells = layout.Descendants("cell").Where(c => Same((string)c.Attribute("name"), field)).ToList();
                foreach (var c in cells)
                    c.Remove();

                var grid = layout.Root;
                if (grid != null && Same((string)grid.Attribute("jump"), field))
                {
                    var first = layout.Descendants("cell").Select(c => (string)c.Attribute("name")).FirstOrDefault(n => n != null && !n.Contains("."));
                    if (first != null) grid.SetAttributeValue("jump", first);
                }

                if (cells.Count > 0)
                {
                    layoutXml = layout.ToString(SaveOptions.DisableFormatting);
                    changed = true;
                }
            }

            return changed;
        }

        /// <summary>
        /// Chart usage: "axis" when the field is grouped or aggregated (the chart would break if it was removed).
        /// </summary>
        private static void MatchChart(string dataDescription, string field, out bool used, out bool axis)
        {
            used = axis = false;
            var doc = Parse(dataDescription);
            if (doc == null) return;
            var attributes = doc.Descendants("attribute").Where(a => Same((string)a.Attribute("name"), field)).ToList();
            var orders = doc.Descendants("order").Where(o => Same((string)o.Attribute("attribute"), field)).ToList();
            var conditions = doc.Descendants("condition").Where(c => Same((string)c.Attribute("attribute"), field)).ToList();
            used = attributes.Count > 0 || orders.Count > 0 || conditions.Count > 0;
            axis = attributes.Any(a => (string)a.Attribute("groupby") == "true" || a.Attribute("aggregate") != null) || conditions.Count > 0;
        }

        private static bool RemoveFromChart(ref string dataDescription, string field)
        {
            MatchChart(dataDescription, field, out var used, out var axis);
            if (!used || axis) return false;
            var doc = Parse(dataDescription);
            var nodes = doc.Descendants("attribute").Where(a => Same((string)a.Attribute("name"), field))
                .Concat(doc.Descendants("order").Where(o => Same((string)o.Attribute("attribute"), field)))
                .ToList();
            foreach (var n in nodes) n.Remove();
            dataDescription = doc.ToString(SaveOptions.DisableFormatting);
            return nodes.Count > 0;
        }

        #endregion

        #region Analysis

        /// <summary>Lists every change the tool will make and every place the fields are used. Nothing is modified.</summary>
        public static List<FieldUsage> Analyze(IOrganizationService service, string entity, IList<Field> fields,
            IDictionary<string, string> newDisplayNames, CleanupOptions options, BackgroundWorker worker)
        {
            options = options ?? CleanupOptions.None;
            newDisplayNames = newDisplayNames ?? new Dictionary<string, string>();
            var usages = new List<FieldUsage>();
            void Progress(string message) => worker?.ReportProgress(0, message);

            AnalyzeProperties(usages, fields, newDisplayNames, options);

            var forms = new List<FormInfo>();
            Progress("Analysing forms...");
            Safe(usages, "Forms", () =>
            {
                forms = GetForms(service, entity);
                AnalyzeForms(usages, forms, fields, options);
            });

            Progress("Analysing views...");
            Safe(usages, "Views", () => AnalyzeViews(usages, GetViews(service, entity), fields, options, personal: false));
            Safe(usages, "Personal views", () => AnalyzeViews(usages, GetViews(service, entity, personal: true), fields, options, personal: true));

            Progress("Analysing charts...");
            Safe(usages, "Charts", () => AnalyzeCharts(usages, GetCharts(service, entity), fields, options, personal: false));
            Safe(usages, "Personal charts", () => AnalyzeCharts(usages, GetCharts(service, entity, personal: true), fields, options, personal: true));

            Progress("Analysing plug-in steps...");
            Safe(usages, "Plug-in steps", () => AnalyzeSteps(service, usages, entity, fields, options));

            Progress("Analysing processes and business rules...");
            Safe(usages, "Processes", () => AnalyzeProcesses(service, usages, entity, fields));

            EntityMetadata metadata = null;
            Safe(usages, "Table metadata", () =>
            {
                metadata = ((RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest { LogicalName = entity, EntityFilters = EntityFilters.Entity })).EntityMetadata;
            });

            Progress("Analysing cloud flows...");
            if (metadata?.EntitySetName != null)
                Safe(usages, "Cloud flows", () => AnalyzeCloudFlows(service, usages, metadata.EntitySetName, fields));

            Progress("Analysing form scripts...");
            Safe(usages, "Form scripts", () => AnalyzeScripts(service, usages, forms, fields));

            Progress("Analysing e-mail templates...");
            Safe(usages, "E-mail templates", () => AnalyzeTemplates(service, usages, entity, fields));

            Progress("Analysing duplicate detection rules...");
            Safe(usages, "Duplicate detection rules", () => AnalyzeDuplicateRules(service, usages, entity, fields, options));

            Progress("Analysing field security...");
            Safe(usages, "Field security", () => AnalyzeFieldSecurity(service, usages, entity, fields, options));

            Progress("Analysing relationship mappings...");
            Safe(usages, "Relationship mappings", () => AnalyzeMappings(service, usages, entity, fields, options));

            if (metadata?.Keys != null)
                AnalyzeKeys(usages, metadata, fields);

            if (options.ShowDataUsage)
            {
                Progress("Counting records...");
                Safe(usages, "Data usage", () => AnalyzeData(service, usages, entity, fields));
            }

            foreach (var f in fields)
            {
                Progress($"Analysing dependencies of {f.SchemaName}...");
                Safe(usages, "Dependencies", () => usages.AddRange(GetDependencies(service, f)));
            }

            if (options.AutoBackup)
                usages.Add(new FieldUsage { SchemaName = "-", Category = "Backup", Name = options.EffectiveBackupFolder(), Action = UsageAction.Info, Detail = "forms, views, charts, steps and labels are saved before any change" });
            if (!string.IsNullOrWhiteSpace(options.SolutionUniqueName))
                usages.Add(new FieldUsage { SchemaName = "-", Category = "Solution", Name = options.SolutionUniqueName, Action = UsageAction.Info, Detail = "changed components will be added to this solution" });
            if (!options.Publish)
                usages.Add(new FieldUsage { SchemaName = "-", Category = "Publish", Name = "Not published", Action = UsageAction.Info, Detail = "option “Publish automatically” is off: publish the table yourself" });

            // The same component can be found by a scan and by the dependency API: keep one line.
            return usages
                .GroupBy(u => new { u.SchemaName, u.Category, u.Name })
                .Select(g => g.OrderBy(u => u.Action).First())
                .ToList();
        }

        private static void AnalyzeProperties(List<FieldUsage> usages, IList<Field> fields, IDictionary<string, string> newNames, CleanupOptions options)
        {
            foreach (var f in fields)
            {
                if (newNames.TryGetValue(f.SchemaName, out var newName) && newName != null)
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Display name", Name = $"{f.DisplayName} → {newName}", Action = UsageAction.WillChange, Detail = "All translations" });

                if (f.RequiredLevel != AttributeRequiredLevel.None)
                {
                    var canChange = f.RequiredLevel != AttributeRequiredLevel.SystemRequired;
                    usages.Add(new FieldUsage
                    {
                        SchemaName = f.SchemaName,
                        Category = "Requirement",
                        Name = options.MakeNotRequired && canChange ? $"{f.RequiredLevel} → Optional" : f.RequiredLevel.ToString(),
                        Action = options.MakeNotRequired && canChange ? UsageAction.WillChange : UsageAction.Kept,
                        Detail = canChange ? (options.MakeNotRequired ? null : "Option “Make the field optional” is off") : "System required fields cannot be changed"
                    });
                }

                if (f.IsSearchable)
                    usages.Add(new FieldUsage
                    {
                        SchemaName = f.SchemaName,
                        Category = "Advanced Find",
                        Name = options.RemoveFromAdvancedFind ? "Searchable → Not searchable" : "Searchable",
                        Action = options.RemoveFromAdvancedFind ? UsageAction.WillChange : UsageAction.Kept,
                        Detail = options.RemoveFromAdvancedFind ? null : "Option “Remove from Advanced Find” is off"
                    });

                if (f.IsAuditEnabled)
                    usages.Add(new FieldUsage
                    {
                        SchemaName = f.SchemaName,
                        Category = "Auditing",
                        Name = options.DisableAuditing ? "Enabled → Disabled" : "Enabled",
                        Action = options.DisableAuditing ? UsageAction.WillChange : UsageAction.Kept,
                        Detail = options.DisableAuditing ? "existing audit history is kept" : "Option “Disable auditing” is off"
                    });

                if (options.AddDeprecationNote)
                {
                    var current = PrefixSuffixFieldsNameManager.GetLabel(f.DescriptionLabel) ?? string.Empty;
                    var already = current.StartsWith(options.DeprecationPrefix(), IgnoreCase);
                    usages.Add(new FieldUsage
                    {
                        SchemaName = f.SchemaName,
                        Category = "Description",
                        Name = already ? current : $"{options.DeprecationText()} {current}".Trim(),
                        Action = already ? UsageAction.Kept : UsageAction.WillChange,
                        Detail = already ? "already contains the deprecation note" : "note added before the current description, all translations"
                    });
                }
            }
        }

        private static void AnalyzeForms(List<FieldUsage> usages, List<FormInfo> forms, IList<Field> fields, CleanupOptions options)
        {
            foreach (var form in forms)
            {
                var doc = Parse(form.Xml);
                if (doc == null) continue;
                foreach (var f in fields)
                {
                    var count = FormControls(doc, f.SchemaName).Count();
                    if (count == 0) continue;

                    var details = new List<string>();
                    if (count > 1) details.Add($"{count} controls");
                    if (FormHasEvents(doc, f.SchemaName)) details.Add("has an OnChange script (kept)");

                    UsageAction action;
                    if (f.IsPrimaryName)
                    {
                        details.Add("primary name field is never removed");
                        action = UsageAction.Kept;
                    }
                    else if (options.RemoveFromForms)
                    {
                        action = Decide(true, "Remove from forms", true, form.Managed, options, details);
                        if (action == UsageAction.WillChange) details.Insert(0, "will be removed");
                    }
                    else if (options.HideOnForms)
                    {
                        var hidden = FormCells(doc, f.SchemaName).All(c => (string)c.Attribute("visible") == "false");
                        action = Decide(true, "Hide on forms", !hidden, form.Managed, options, details);
                        details.Insert(0, hidden ? "already hidden" : "will be hidden");
                    }
                    else
                    {
                        action = Decide(false, "Remove from forms", true, form.Managed, options, details);
                    }

                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = FormTypeLabel(form.Type), Name = form.Name, Action = action, Detail = Join(details) });
                }
            }
        }

        private static void AnalyzeViews(List<FieldUsage> usages, List<ViewInfo> views, IList<Field> fields, CleanupOptions options, bool personal)
        {
            foreach (var view in views)
            {
                foreach (var f in fields)
                {
                    var match = MatchView(view, f.SchemaName);
                    if (!match.Any) continue;

                    var details = new List<string> { match.Describe() };
                    UsageAction action;
                    if (personal)
                    {
                        details.Add("personal view, the owner must update it");
                        action = UsageAction.Kept;
                    }
                    else if (f.IsPrimaryName)
                    {
                        details.Add("primary name field is never removed");
                        action = UsageAction.Kept;
                    }
                    else
                    {
                        action = Decide(options.RemoveFromViews, "Remove from views", match.Removable, view.Managed, options, details);
                    }

                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = personal ? "Personal view" : ViewTypeLabel(view.QueryType), Name = view.Name, Action = action, Detail = Join(details) });
                }
            }
        }

        private static void AnalyzeCharts(List<FieldUsage> usages, List<ChartInfo> charts, IList<Field> fields, CleanupOptions options, bool personal)
        {
            foreach (var chart in charts)
            {
                foreach (var f in fields)
                {
                    MatchChart(chart.Data, f.SchemaName, out var used, out var axis);
                    if (!used) continue;

                    var details = new List<string>();
                    if (axis) details.Add("used as axis, series or filter: the chart would break, review manually");
                    UsageAction action;
                    if (personal)
                    {
                        details.Add("personal chart, the owner must update it");
                        action = UsageAction.Kept;
                    }
                    else
                    {
                        action = Decide(options.RemoveFromCharts, "Remove from charts", !axis, chart.Managed, options, details);
                    }

                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = personal ? "Personal chart" : "Chart", Name = chart.Name, Action = action, Detail = Join(details) });
                }
            }
        }

        private static void AnalyzeSteps(IOrganizationService service, List<FieldUsage> usages, string entity, IList<Field> fields, CleanupOptions options)
        {
            foreach (var step in GetSteps(service, entity))
            {
                foreach (var f in fields.Where(f => CsvContains(step.Filtering, f.SchemaName)))
                {
                    var last = CsvCount(step.Filtering) == 1;
                    var details = new List<string>();
                    if (last) details.Add("only filtering attribute: kept (an empty list would trigger the step on every update)");
                    var action = Decide(options.RemoveFromSteps, "Remove from plug-in steps", !last, step.Managed, options, details);
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Plug-in step (filtering attributes)", Name = step.Name, Action = action, Detail = Join(details) });
                }
            }

            foreach (var image in GetStepImages(service, entity))
            {
                foreach (var f in fields.Where(f => CsvContains(image.Attributes, f.SchemaName)))
                {
                    var last = CsvCount(image.Attributes) == 1;
                    var details = new List<string>();
                    if (last) details.Add("only attribute of the image: kept (an empty list means all attributes)");
                    var action = Decide(options.RemoveFromSteps, "Remove from plug-in steps", !last, image.Managed, options, details);
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Plug-in step image", Name = $"{image.Name} ({image.Step})", Action = action, Detail = Join(details) });
                }
            }
        }

        private static void AnalyzeProcesses(IOrganizationService service, List<FieldUsage> usages, string entity, IList<Field> fields)
        {
            var query = new QueryExpression("workflow")
            {
                ColumnSet = new ColumnSet("name", "category", "xaml", "statecode"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("primaryentity", ConditionOperator.Equal, entity),
                        new ConditionExpression("type", ConditionOperator.Equal, 1) // definitions only
                    }
                }
            };
            foreach (var wf in service.RetrieveMultiple(query).Entities)
            {
                var xaml = wf.GetAttributeValue<string>("xaml");
                if (string.IsNullOrEmpty(xaml)) continue;
                var active = wf.GetAttributeValue<OptionSetValue>("statecode")?.Value == 1;
                foreach (var f in fields.Where(f => QuotedToken(f.SchemaName).IsMatch(xaml)))
                    usages.Add(new FieldUsage
                    {
                        SchemaName = f.SchemaName,
                        Category = WorkflowCategoryLabel(wf.GetAttributeValue<OptionSetValue>("category")?.Value ?? -1),
                        Name = wf.GetAttributeValue<string>("name"),
                        Action = UsageAction.Kept,
                        Detail = (active ? "active" : "draft") + " · review manually"
                    });
            }
        }

        private static void AnalyzeCloudFlows(IOrganizationService service, List<FieldUsage> usages, string entitySetName, IList<Field> fields)
        {
            var query = new QueryExpression("workflow")
            {
                ColumnSet = new ColumnSet("name", "clientdata"),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("category", ConditionOperator.Equal, 5),
                        new ConditionExpression("clientdata", ConditionOperator.Like, $"%{entitySetName}%")
                    }
                }
            };
            foreach (var flow in service.RetrieveMultiple(query).Entities)
            {
                var data = flow.GetAttributeValue<string>("clientdata") ?? string.Empty;
                foreach (var f in fields.Where(f => QuotedToken(f.SchemaName).IsMatch(data) || data.IndexOf($"/{f.SchemaName}", IgnoreCase) >= 0))
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Cloud flow", Name = flow.GetAttributeValue<string>("name"), Action = UsageAction.Kept, Detail = "text match on the flow definition · review manually" });
            }
        }

        private static void AnalyzeScripts(IOrganizationService service, List<FieldUsage> usages, List<FormInfo> forms, IList<Field> fields)
        {
            var libraries = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var form in forms)
            {
                var doc = Parse(form.Xml);
                if (doc == null) continue;
                foreach (var lib in FormLibraries(doc))
                {
                    if (!libraries.TryGetValue(lib, out var list))
                        libraries[lib] = list = new List<string>();
                    if (!list.Contains(form.Name)) list.Add(form.Name);
                }
            }

            foreach (var lib in libraries)
            {
                var resource = service.RetrieveMultiple(new QueryExpression("webresource")
                {
                    ColumnSet = new ColumnSet("name", "content"),
                    Criteria = { Conditions = { new ConditionExpression("name", ConditionOperator.Equal, lib.Key) } }
                }).Entities.FirstOrDefault();
                var content = resource?.GetAttributeValue<string>("content");
                if (string.IsNullOrEmpty(content)) continue;

                string script;
                try { script = Encoding.UTF8.GetString(Convert.FromBase64String(content)); }
                catch { continue; }

                foreach (var f in fields.Where(f => QuotedToken(f.SchemaName).IsMatch(script)))
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Form script (web resource)", Name = lib.Key, Action = UsageAction.Kept, Detail = "used by: " + string.Join(", ", lib.Value) + " · review manually" });
            }
        }

        private static void AnalyzeTemplates(IOrganizationService service, List<FieldUsage> usages, string entity, IList<Field> fields)
        {
            var query = new QueryExpression("template")
            {
                ColumnSet = new ColumnSet("title", "body", "subject", "presentationxml"),
                Criteria = { Conditions = { new ConditionExpression("templatetypecode", ConditionOperator.Equal, entity) } }
            };
            foreach (var t in service.RetrieveMultiple(query).Entities)
            {
                var text = string.Concat(t.GetAttributeValue<string>("body"), t.GetAttributeValue<string>("subject"), t.GetAttributeValue<string>("presentationxml"));
                foreach (var f in fields)
                {
                    var placeholder = new Regex($@"(:{Regex.Escape(f.SchemaName)}[;}}\]/])|([""']{Regex.Escape(f.SchemaName)}[""'])", RegexOptions.IgnoreCase);
                    if (placeholder.IsMatch(text))
                        usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "E-mail template", Name = t.GetAttributeValue<string>("title"), Action = UsageAction.Kept, Detail = "Review manually" });
                }
            }
        }

        private static void AnalyzeDuplicateRules(IOrganizationService service, List<FieldUsage> usages, string entity, IList<Field> fields, CleanupOptions options)
        {
            var conditions = GetDuplicateConditions(service);
            foreach (var c in conditions)
            {
                foreach (var f in fields.Where(f => ConditionUses(c, entity, f.SchemaName)))
                {
                    var ruleConditions = conditions.Count(x => x.RuleId == c.RuleId);
                    var only = ruleConditions == 1;
                    var details = new List<string>();
                    if (only) details.Add("only condition of the rule: the rule will be unpublished, delete it manually");
                    else if (c.RuleState == 1) details.Add("the rule will be unpublished, then published again");
                    var action = Decide(options.RemoveFromDuplicateRules, "Remove from duplicate detection rules", true, false, options, details);
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Duplicate detection rule", Name = c.Rule, Action = action, Detail = Join(details) });
                }
            }
        }

        private static void AnalyzeFieldSecurity(IOrganizationService service, List<FieldUsage> usages, string entity, IList<Field> fields, CleanupOptions options)
        {
            foreach (var p in GetFieldPermissions(service, entity))
            {
                foreach (var f in fields.Where(f => Same(f.SchemaName, p.Attribute)))
                {
                    var details = new List<string> { "field permission" };
                    var action = Decide(options.RemoveFromFieldSecurity, "Remove from field security profiles", true, false, options, details);
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Field security profile", Name = p.Profile, Action = action, Detail = Join(details) });
                }
            }
        }

        private static void AnalyzeMappings(IOrganizationService service, List<FieldUsage> usages, string entity, IList<Field> fields, CleanupOptions options)
        {
            foreach (var m in GetMappings(service, entity, fields))
            {
                foreach (var f in fields.Where(f => MappingUses(m, entity, f.SchemaName)))
                {
                    var details = new List<string>();
                    if (m.System) details.Add("system mapping: cannot be deleted");
                    var action = Decide(options.RemoveFromMappings, "Remove from relationship mappings", !m.System, false, options, details);
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Relationship mapping", Name = $"{m.Source}.{m.SourceAttribute} → {m.Target}.{m.TargetAttribute}", Action = action, Detail = Join(details) });
                }
            }
        }

        private static void AnalyzeKeys(List<FieldUsage> usages, EntityMetadata metadata, IList<Field> fields)
        {
            foreach (var key in metadata.Keys)
                foreach (var f in fields.Where(f => key.KeyAttributes != null && key.KeyAttributes.Contains(f.SchemaName, StringComparer.OrdinalIgnoreCase)))
                    usages.Add(new FieldUsage { SchemaName = f.SchemaName, Category = "Alternate key", Name = key.LogicalName, Action = UsageAction.Kept, Detail = "Review manually" });
        }

        /// <summary>How many records hold a value, and when the most recent one was modified.</summary>
        private static void AnalyzeData(IOrganizationService service, List<FieldUsage> usages, string entity, IList<Field> fields)
        {
            long? total = null;
            string totalError = null;
            try
            {
                var primary = ((RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest { LogicalName = entity, EntityFilters = EntityFilters.Entity })).EntityMetadata.PrimaryIdAttribute;
                total = Count(service, $"<fetch aggregate='true'><entity name='{SecurityElement.Escape(entity)}'><attribute name='{primary}' alias='n' aggregate='count' /></entity></fetch>");
            }
            catch (Exception ex) { totalError = CountError(ex); }

            foreach (var f in fields)
            {
                var usage = new FieldUsage { SchemaName = f.SchemaName, Category = DataUsageCategory, Action = UsageAction.Info, Total = total };
                try
                {
                    usage.Filled = Count(service, $"<fetch aggregate='true'><entity name='{SecurityElement.Escape(entity)}'><attribute name='{f.SchemaName}' alias='n' aggregate='countcolumn' /></entity></fetch>");
                    usage.Name = total.HasValue && total.Value > 0
                        ? $"{usage.Filled:N0} of {total.Value:N0} records ({100.0 * usage.Filled / total.Value:0.#} %)"
                        : $"{usage.Filled:N0} records with a value" + (totalError != null ? $" (total: {totalError})" : string.Empty);
                }
                catch (Exception ex)
                {
                    usage.Name = "Count not available";
                    usage.Detail = CountError(ex);
                    usages.Add(usage);
                    continue;
                }

                try
                {
                    var last = service.RetrieveMultiple(new QueryExpression(entity)
                    {
                        TopCount = 1,
                        ColumnSet = new ColumnSet("modifiedon"),
                        Criteria = { Conditions = { new ConditionExpression(f.SchemaName, ConditionOperator.NotNull) } },
                        Orders = { new OrderExpression("modifiedon", OrderType.Descending) }
                    }).Entities.FirstOrDefault();
                    usage.LastModified = last?.GetAttributeValue<DateTime>("modifiedon").ToLocalTime();
                    usage.Detail = last == null
                        ? "no record has a value: good candidate for deletion"
                        : $"most recent record with a value modified on {usage.LastModified:yyyy-MM-dd}";
                }
                catch (Exception ex) { usage.Detail = ex.Message; }

                usages.Add(usage);
            }
        }

        private static long Count(IOrganizationService service, string fetch)
        {
            var row = service.RetrieveMultiple(new FetchExpression(fetch)).Entities.FirstOrDefault();
            var value = row?.GetAttributeValue<AliasedValue>("n")?.Value;
            return value == null ? 0 : Convert.ToInt64(value);
        }

        private static string CountError(Exception ex) =>
            ex.Message.IndexOf("AggregateQueryRecordLimit", IgnoreCase) >= 0 || ex.Message.IndexOf("50000", IgnoreCase) >= 0
                ? "more than 50,000 records"
                : ex.Message;

        private static IEnumerable<FieldUsage> GetDependencies(IOrganizationService service, Field field)
        {
            var response = (RetrieveDependentComponentsResponse)service.Execute(new RetrieveDependentComponentsRequest
            {
                ComponentType = ComponentAttribute,
                ObjectId = field.MetadataId
            });

            foreach (var dependency in response.EntityCollection.Entities)
            {
                var type = dependency.GetAttributeValue<OptionSetValue>("dependentcomponenttype")?.Value ?? -1;
                var id = dependency.GetAttributeValue<Guid>("dependentcomponentobjectid");
                // Analysed above with more details.
                if (type == ComponentForm || type == ComponentView || type == ComponentChart || type == ComponentStep || type == ComponentStepImage)
                    continue;
                yield return DescribeComponent(service, field.SchemaName, type, id);
            }
        }

        private static FieldUsage DescribeComponent(IOrganizationService service, string schema, int type, Guid id)
        {
            var usage = new FieldUsage { SchemaName = schema, Action = UsageAction.Kept, Detail = "Dataverse dependency · review manually" };
            try
            {
                switch (type)
                {
                    case 29:
                        var wf = service.Retrieve("workflow", id, new ColumnSet("name", "category"));
                        usage.Category = WorkflowCategoryLabel(wf.GetAttributeValue<OptionSetValue>("category")?.Value ?? -1);
                        usage.Name = wf.GetAttributeValue<string>("name");
                        break;
                    case 61:
                        usage.Category = "Web resource";
                        usage.Name = service.Retrieve("webresource", id, new ColumnSet("name")).GetAttributeValue<string>("name");
                        break;
                    case 2:
                        usage.Category = "Field (calculated / rollup / key)";
                        usage.Name = ((RetrieveAttributeResponse)service.Execute(new RetrieveAttributeRequest { MetadataId = id })).AttributeMetadata.LogicalName;
                        break;
                    case 10:
                        usage.Category = "Relationship";
                        usage.Name = ((RetrieveRelationshipResponse)service.Execute(new RetrieveRelationshipRequest { MetadataId = id })).RelationshipMetadata.SchemaName;
                        break;
                    case 14:
                        usage.Category = "Alternate key";
                        usage.Name = id.ToString();
                        break;
                    case 70:
                        usage.Category = "Field security profile";
                        usage.Name = service.Retrieve("fieldsecurityprofile", id, new ColumnSet("name")).GetAttributeValue<string>("name");
                        break;
                    default:
                        usage.Category = $"Component (type {type})";
                        usage.Name = id.ToString();
                        break;
                }
            }
            catch
            {
                usage.Category = usage.Category ?? $"Component (type {type})";
                usage.Name = usage.Name ?? id.ToString();
            }
            return usage;
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleans the components of the table (re-read fresh, one update per component).
        /// The primary name field is never removed from forms and views; managed components are skipped when asked.
        /// </summary>
        public static List<UpdateResult> CleanupComponents(IOrganizationService service, string entity, IList<Field> fields,
            CleanupOptions options, BackgroundWorker worker)
        {
            var results = new List<UpdateResult>();
            var targets = fields.Where(f => !f.IsPrimaryName).ToList();

            void Report(IEnumerable<Field> list, string operation, ChangeStatus status, string message, int type = 0, Guid id = default(Guid)) =>
                results.AddRange(list.Select(f => new UpdateResult { SchemaName = f.SchemaName, Operation = operation, Status = status, Message = message, ComponentType = type, ComponentId = id }));

            bool SkipManaged(bool managed, IEnumerable<Field> list, string operation)
            {
                if (!managed || !options.SkipManaged) return false;
                Report(list, operation, ChangeStatus.Skipped, "Managed component: skipped");
                return true;
            }

            void Try(IList<Field> list, string operation, string message, int type, Guid id, Action action)
            {
                try
                {
                    action();
                    Report(list, operation, ChangeStatus.Updated, message, type, id);
                }
                catch (Exception ex) { Report(list, operation, ChangeStatus.Failed, ex.Message); }
            }

            // Forms: remove or hide
            if ((options.RemoveFromForms || options.HideOnForms) && targets.Count > 0)
            {
                worker.ReportProgress(0, options.RemoveFromForms ? "Removing fields from forms..." : "Hiding fields on forms...");
                foreach (var form in GetForms(service, entity))
                {
                    var doc = Parse(form.Xml);
                    if (doc == null) continue;
                    var changed = targets.Where(f => options.RemoveFromForms ? RemoveFromForm(doc, f.SchemaName) : HideOnForm(doc, f.SchemaName)).ToList();
                    if (changed.Count == 0) continue;
                    var operation = $"{FormTypeLabel(form.Type)} · {form.Name}";
                    if (SkipManaged(form.Managed, changed, operation)) continue;
                    Try(changed, operation, options.RemoveFromForms ? "Removed from the form" : "Hidden on the form", ComponentForm, form.Id,
                        () => service.Update(new Entity("systemform", form.Id) { ["formxml"] = doc.ToString(SaveOptions.DisableFormatting) }));
                }
            }

            // Views
            if (options.RemoveFromViews && targets.Count > 0)
            {
                worker.ReportProgress(0, "Removing fields from views...");
                foreach (var view in GetViews(service, entity))
                {
                    var fetchXml = view.FetchXml;
                    var layoutXml = view.LayoutXml;
                    var changed = targets.Where(f => RemoveFromView(ref fetchXml, ref layoutXml, f.SchemaName)).ToList();
                    if (changed.Count == 0) continue;
                    var operation = $"{ViewTypeLabel(view.QueryType)} · {view.Name}";
                    if (SkipManaged(view.Managed, changed, operation)) continue;
                    Try(changed, operation, "Removed from the view", ComponentView, view.Id, () =>
                    {
                        var update = new Entity("savedquery", view.Id);
                        if (fetchXml != view.FetchXml) update["fetchxml"] = fetchXml;
                        if (layoutXml != view.LayoutXml) update["layoutxml"] = layoutXml;
                        service.Update(update);
                    });
                }
            }

            // Charts (only when the field is not an axis / series / filter)
            if (options.RemoveFromCharts)
            {
                worker.ReportProgress(0, "Removing fields from charts...");
                foreach (var chart in GetCharts(service, entity))
                {
                    var data = chart.Data;
                    var changed = fields.Where(f => RemoveFromChart(ref data, f.SchemaName)).ToList();
                    if (changed.Count == 0) continue;
                    var operation = $"Chart · {chart.Name}";
                    if (SkipManaged(chart.Managed, changed, operation)) continue;
                    Try(changed, operation, "Removed from the chart", ComponentChart, chart.Id,
                        () => service.Update(new Entity("savedqueryvisualization", chart.Id) { ["datadescription"] = data }));
                }
            }

            // Plug-in steps and images
            if (options.RemoveFromSteps)
            {
                worker.ReportProgress(0, "Removing fields from plug-in steps...");
                foreach (var step in GetSteps(service, entity))
                {
                    var operation = $"Plug-in step · {step.Name}";
                    var csv = step.Filtering;
                    var changed = new List<Field>();
                    foreach (var f in fields.Where(f => CsvContains(csv, f.SchemaName)))
                    {
                        if (CsvCount(csv) <= 1) { Report(new[] { f }, operation, ChangeStatus.Skipped, "Only filtering attribute: kept"); continue; }
                        csv = CsvRemove(csv, f.SchemaName);
                        changed.Add(f);
                    }
                    if (changed.Count == 0 || SkipManaged(step.Managed, changed, operation)) continue;
                    Try(changed, operation, "Removed from the filtering attributes", ComponentStep, step.Id,
                        () => service.Update(new Entity("sdkmessageprocessingstep", step.Id) { ["filteringattributes"] = csv }));
                }

                foreach (var image in GetStepImages(service, entity))
                {
                    var operation = $"Step image · {image.Name} ({image.Step})";
                    var csv = image.Attributes;
                    var changed = new List<Field>();
                    foreach (var f in fields.Where(f => CsvContains(csv, f.SchemaName)))
                    {
                        if (CsvCount(csv) <= 1) { Report(new[] { f }, operation, ChangeStatus.Skipped, "Only attribute of the image: kept"); continue; }
                        csv = CsvRemove(csv, f.SchemaName);
                        changed.Add(f);
                    }
                    if (changed.Count == 0 || SkipManaged(image.Managed, changed, operation)) continue;
                    Try(changed, operation, "Removed from the image attributes", ComponentStepImage, image.Id,
                        () => service.Update(new Entity("sdkmessageprocessingstepimage", image.Id) { ["attributes"] = csv }));
                }
            }

            // Relationship mappings (deleted)
            if (options.RemoveFromMappings)
            {
                worker.ReportProgress(0, "Removing relationship mappings...");
                foreach (var m in GetMappings(service, entity, fields))
                {
                    var changed = fields.Where(f => MappingUses(m, entity, f.SchemaName)).ToList();
                    if (changed.Count == 0) continue;
                    var operation = $"Mapping · {m.Source}.{m.SourceAttribute} → {m.Target}.{m.TargetAttribute}";
                    if (m.System) { Report(changed, operation, ChangeStatus.Skipped, "System mapping: cannot be deleted"); continue; }
                    Try(changed, operation, "Mapping deleted", 0, Guid.Empty, () => service.Delete("attributemap", m.Id));
                }
            }

            // Duplicate detection rules
            if (options.RemoveFromDuplicateRules)
            {
                worker.ReportProgress(0, "Updating duplicate detection rules...");
                var conditions = GetDuplicateConditions(service);
                foreach (var rule in conditions.Where(c => fields.Any(f => ConditionUses(c, entity, f.SchemaName))).GroupBy(c => c.RuleId))
                {
                    var all = conditions.Where(c => c.RuleId == rule.Key).ToList();
                    var toDelete = rule.ToList();
                    var changed = fields.Where(f => toDelete.Any(c => ConditionUses(c, entity, f.SchemaName))).ToList();
                    var first = toDelete[0];
                    var operation = $"Duplicate rule · {first.Rule}";
                    var published = first.RuleState == 1;

                    if (toDelete.Count >= all.Count)
                    {
                        // Removing every condition would leave an invalid rule: unpublish it instead.
                        Try(changed, operation, published ? "Rule unpublished (only condition): delete it manually" : "Rule already unpublished (only condition): delete it manually", 0, Guid.Empty, () =>
                        {
                            if (published) service.Execute(new UnpublishDuplicateRuleRequest { DuplicateRuleId = rule.Key });
                        });
                        continue;
                    }

                    Try(changed, operation, published ? "Condition removed, rule published again" : "Condition removed", 0, Guid.Empty, () =>
                    {
                        if (published) service.Execute(new UnpublishDuplicateRuleRequest { DuplicateRuleId = rule.Key });
                        foreach (var c in toDelete) service.Delete("duplicaterulecondition", c.Id);
                        if (published) service.Execute(new PublishDuplicateRuleRequest { DuplicateRuleId = rule.Key });
                    });
                }
            }

            // Field security profiles (permissions deleted)
            if (options.RemoveFromFieldSecurity)
            {
                worker.ReportProgress(0, "Removing field permissions...");
                foreach (var p in GetFieldPermissions(service, entity))
                {
                    var changed = fields.Where(f => Same(f.SchemaName, p.Attribute)).ToList();
                    if (changed.Count == 0) continue;
                    Try(changed, $"Field security · {p.Profile}", "Field permission deleted", 0, Guid.Empty, () => service.Delete("fieldpermission", p.Id));
                }
            }

            return results;
        }

        #endregion
    }
}
