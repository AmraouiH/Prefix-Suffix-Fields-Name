using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Label = Microsoft.Xrm.Sdk.Label;

namespace Prefix_Suffix_Fields_Name
{
    public static class PreffixSuffixFieldsNameManager
    {
        public static RetrieveMetadataChangesResponse GetEntitiesMetadat(IOrganizationService service)
        {

            MetadataFilterExpression entityFilter = new MetadataFilterExpression(LogicalOperator.And);

            EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = entityFilter,
                Properties = new MetadataPropertiesExpression("LogicalName", "DisplayName")
            };
            RetrieveMetadataChangesRequest retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression,
                ClientVersionStamp = null
            };

            return (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);
        }

        public static void SetEntitiesGridViewHeaders(DataTable dt, DataGridView entityGridView)
        {
            entityGridView.DataSource = dt;
            entityGridView.Sort(entityGridView.Columns[1], ListSortDirection.Descending);
            entityGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Regular);
            entityGridView.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            entityGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            entityGridView.Columns[0].HeaderText = String.Empty;
            entityGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            entityGridView.Columns[1].HeaderText = "Display Name";
            entityGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            entityGridView.Columns[2].HeaderText = "Schema Name";
        }


        public static void SetFieldsGridViewHeaders(DataTable dt, DataGridView fieldDataGridView)
        {
            fieldDataGridView.DataSource = dt;
            fieldDataGridView.Sort(fieldDataGridView.Columns[1], ListSortDirection.Ascending);
            fieldDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Regular);
            fieldDataGridView.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            fieldDataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            fieldDataGridView.Columns[0].HeaderText = String.Empty;
            fieldDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[1].HeaderText = "DisplayName";
            fieldDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[2].HeaderText = "SchemaName";
        }

        public static Dictionary<String, String> getEntityFields(IOrganizationService service, String entityTechnicalName, BackgroundWorker worker)
        {
            Dictionary<String, String> _fields = new Dictionary<string, string>();
            RetrieveEntityRequest retrieveBankAccountEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = entityTechnicalName,
                RetrieveAsIfPublished = true
            };
            
            worker.ReportProgress(0, "Retrieving Entity MetaData...");
            RetrieveEntityResponse retrieveEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveBankAccountEntityRequest);
            formatList(retrieveEntityResponse, _fields);

            worker.ReportProgress(0, "Retrievig Fields ...");
            return _fields;
        }

        private static void formatList(RetrieveEntityResponse _metadata, Dictionary<String,String> _fields)
        {
            var attributes = FilterAttributes(_metadata.EntityMetadata.Attributes);
            foreach (var field in attributes)
            {
                    _fields.Add(field.LogicalName, field.DisplayName.UserLocalizedLabel != null ? field.DisplayName.UserLocalizedLabel.Label : @"N/A");
            }
        }

        public static IEnumerable<AttributeMetadata> FilterAttributes(AttributeMetadata[] attributes)
        {
            return attributes.Where(a =>
                            a.AttributeOf == null
                            && a.AttributeType.Value != AttributeTypeCode.Virtual
                            && a.AttributeType.Value != AttributeTypeCode.PartyList
                            && a.IsValidForRead.Value
                            && a.LogicalName.IndexOf("composite") < 0
                            //&& a.IsValidForForm.Value
                            //&& ((BooleanManagedProperty)a.IsRenameable).Value ==  true
                            ).OrderBy(a => a.LogicalName);
        }

        public static string[] SelectedEntity(DataGridViewRowCollection rows)
        {
            List<String> selectedEntity = new List<string>();
            foreach (DataGridViewRow row in rows)
            {
                var cell = row.Cells[0];
                if (cell != null)
                {
                    if ((bool)cell.Value != null && (bool)cell.Value == true)
                    {
                        selectedEntity.Add(row.Cells[1].Value.ToString());
                        selectedEntity.Add(row.Cells[2].Value.ToString());
                    }
                }
            }
            return selectedEntity.ToArray();
        }

        public static Dictionary<String, String> SelectedFields(DataGridViewRowCollection rows)
        {
            Dictionary<String, String> selectedFields = new Dictionary<String, String>();
            foreach (DataGridViewRow row in rows)
            {
                var cell = row.Cells[0];
                if (cell != null)
                {
                    if ((bool)cell.Value != null && (bool)cell.Value == true)
                    {
                        selectedFields.Add(row.Cells[2].Value.ToString(), row.Cells[1].Value.ToString());
                    }
                }
            }
            return selectedFields;
        }


        public static void UpdateNames(IOrganizationService service, String text, String preffixSuffix, String addRemove, Dictionary<String, String> fields, String entityName) {
            Dictionary<String, String> UpdatedFields = FormatedFieldsName(text, preffixSuffix, addRemove, fields);
            foreach (var field in UpdatedFields.Keys)
            {
                UpdateRecordName(service, field, UpdatedFields[field], entityName);
            }
        }


        private static Dictionary<String, String> FormatedFieldsName(String text, String preffixSuffix, String addRemove, Dictionary<String, String> fields) {
            Dictionary<String, String> fieldsUpdate = new Dictionary<string, string>();
            foreach (String field in fields.Keys)
            {
                if (preffixSuffix == "Preffix")
                {
                    if (addRemove == "Add")
                    {
                        fieldsUpdate.Add(field, text + fields[field]);
                    }
                    else if (addRemove == "Remove")
                    {
                        if (fields[field].StartsWith(text))
                            fieldsUpdate.Add(field, fields[field].StartsWith(text) ? fields[field].Substring(text.Length) : fields[field]);
                    }
                }
                else if (preffixSuffix == "Suffix")
                {
                    if (addRemove == "Add")
                    {
                        fieldsUpdate.Add(field, fields[field] + text);
                    }
                    else if (addRemove == "Remove")
                    {
                        if (fields[field].EndsWith(text))
                            fieldsUpdate.Add(field, fields[field].Substring(0, fields[field].Length - text.Length));
                    }
                }
            }

            return fieldsUpdate;
        }

        private static void UpdateRecordName(IOrganizationService service , String schemaName, String displayName, String entityName) {
            AttributeMetadata retrievedAttributeMetadata = new AttributeMetadata() {
                DisplayName = new Label(displayName, 1033),
                LogicalName = schemaName
            };
            UpdateAttributeRequest updateRequest = new UpdateAttributeRequest
            {
                Attribute = retrievedAttributeMetadata,
                EntityName = entityName,
                MergeLabels = false
            };

            var attributeResponse = (UpdateAttributeResponse)service.Execute(updateRequest);
        }
    }
}
