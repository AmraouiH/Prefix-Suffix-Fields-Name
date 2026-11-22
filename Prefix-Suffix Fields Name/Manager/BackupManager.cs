using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Prefix_Suffix_Fields_Name.Manager
{
    /// <summary>
    /// Saves everything the tool can change, before changing it:
    /// field labels and properties, forms, views, charts, plug-in steps and images of the table.
    /// </summary>
    public static class BackupManager
    {
        /// <summary>Creates a timestamped folder and returns its path. Throws if the backup fails.</summary>
        public static string Backup(IOrganizationService service, string entity, IList<Field> fields, string rootFolder, BackgroundWorker worker)
        {
            worker?.ReportProgress(0, "Backing up current customizations...");
            var folder = Path.Combine(rootFolder, $"{entity}_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(folder);

            // Field labels and properties
            var csv = new StringBuilder("Schema Name;Language;Display Name;Description;Required;Searchable;Audit\r\n");
            foreach (var f in fields)
            {
                var languages = (f.DisplayNameLabel?.LocalizedLabels.Select(l => l.LanguageCode) ?? Enumerable.Empty<int>())
                    .Concat(f.DescriptionLabel?.LocalizedLabels.Select(l => l.LanguageCode) ?? Enumerable.Empty<int>())
                    .Distinct().DefaultIfEmpty(0);
                foreach (var lcid in languages)
                {
                    var name = f.DisplayNameLabel?.LocalizedLabels.FirstOrDefault(l => l.LanguageCode == lcid)?.Label;
                    var description = f.DescriptionLabel?.LocalizedLabels.FirstOrDefault(l => l.LanguageCode == lcid)?.Label;
                    csv.Append(string.Join(";", Csv(f.SchemaName), lcid, Csv(name), Csv(description), f.RequiredLevel, f.IsSearchable, f.IsAuditEnabled)).Append("\r\n");
                }
            }
            File.WriteAllText(Path.Combine(folder, "fields.csv"), csv.ToString(), new UTF8Encoding(true));

            // Forms
            SaveRecords(service, folder, "forms", new QueryExpression("systemform")
            {
                ColumnSet = new ColumnSet("name", "type", "formxml"),
                Criteria = { Conditions = { new ConditionExpression("objecttypecode", ConditionOperator.Equal, entity) } }
            }, "formxml");

            // Views
            SaveRecords(service, folder, "views", new QueryExpression("savedquery")
            {
                ColumnSet = new ColumnSet("name", "querytype", "fetchxml", "layoutxml"),
                Criteria = { Conditions = { new ConditionExpression("returnedtypecode", ConditionOperator.Equal, entity) } }
            }, "fetchxml", "layoutxml");

            // Charts
            SaveRecords(service, folder, "charts", new QueryExpression("savedqueryvisualization")
            {
                ColumnSet = new ColumnSet("name", "datadescription"),
                Criteria = { Conditions = { new ConditionExpression("primaryentitytypecode", ConditionOperator.Equal, entity) } }
            }, "datadescription");

            // Plug-in steps and images
            var steps = new QueryExpression("sdkmessageprocessingstep") { ColumnSet = new ColumnSet("name", "filteringattributes") };
            steps.AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid").LinkCriteria.AddCondition("primaryobjecttypecode", ConditionOperator.Equal, entity);
            var stepCsv = new StringBuilder("Id;Name;Filtering attributes\r\n");
            foreach (var s in service.RetrieveMultiple(steps).Entities)
                stepCsv.Append(string.Join(";", s.Id, Csv(s.GetAttributeValue<string>("name")), Csv(s.GetAttributeValue<string>("filteringattributes")))).Append("\r\n");
            File.WriteAllText(Path.Combine(folder, "plugin-steps.csv"), stepCsv.ToString(), new UTF8Encoding(true));

            var images = new QueryExpression("sdkmessageprocessingstepimage") { ColumnSet = new ColumnSet("name", "attributes") };
            images.AddLink("sdkmessageprocessingstep", "sdkmessageprocessingstepid", "sdkmessageprocessingstepid")
                .AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid").LinkCriteria.AddCondition("primaryobjecttypecode", ConditionOperator.Equal, entity);
            var imageCsv = new StringBuilder("Id;Name;Attributes\r\n");
            foreach (var i in service.RetrieveMultiple(images).Entities)
                imageCsv.Append(string.Join(";", i.Id, Csv(i.GetAttributeValue<string>("name")), Csv(i.GetAttributeValue<string>("attributes")))).Append("\r\n");
            File.WriteAllText(Path.Combine(folder, "plugin-step-images.csv"), imageCsv.ToString(), new UTF8Encoding(true));

            return folder;
        }

        /// <summary>One XML file per record, holding the record id, name and the given XML columns.</summary>
        private static void SaveRecords(IOrganizationService service, string folder, string subFolder, QueryExpression query, params string[] xmlColumns)
        {
            var dir = Path.Combine(folder, subFolder);
            Directory.CreateDirectory(dir);
            foreach (var record in service.RetrieveMultiple(query).Entities)
            {
                var root = new XElement("record",
                    new XAttribute("entity", record.LogicalName),
                    new XAttribute("id", record.Id),
                    new XAttribute("name", record.GetAttributeValue<string>("name") ?? string.Empty));
                foreach (var column in xmlColumns)
                    root.Add(new XElement(column, new XCData(record.GetAttributeValue<string>(column) ?? string.Empty)));
                root.Save(Path.Combine(dir, $"{SafeName(record.GetAttributeValue<string>("name"))}_{record.Id}.xml"));
            }
        }

        private static string SafeName(string name)
        {
            name = string.IsNullOrWhiteSpace(name) ? "unnamed" : name;
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Length > 60 ? name.Substring(0, 60) : name;
        }

        private static string Csv(string value)
        {
            value = value ?? string.Empty;
            return value.IndexOfAny(new[] { ';', '"', '\n', '\r' }) >= 0 ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
        }
    }
}
