using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Prefix_Suffix_Components_Name.Manager;
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
            entityGridView.RowHeadersVisible = false;
            entityGridView.Sort(entityGridView.Columns[0], ListSortDirection.Ascending);
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
            fieldDataGridView.RowHeadersVisible = false;
            fieldDataGridView.Sort(fieldDataGridView.Columns[0], ListSortDirection.Ascending);
            fieldDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Regular);
            fieldDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[0].HeaderText = "Display Name";
            fieldDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[1].HeaderText = "Schema Name";
            fieldDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[2].HeaderText = "Type";
            fieldDataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[3].HeaderText = "IsSearchable";
            
        }
        public static void SetFieldsGridViewHeaders(DataTable dt, DataGridView fieldDataGridView, bool isForResult=false)
        {
            fieldDataGridView.DataSource = dt;
            fieldDataGridView.RowHeadersVisible = false;
            fieldDataGridView.Sort(fieldDataGridView.Columns[0], ListSortDirection.Ascending);
            fieldDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Regular);
            fieldDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[0].HeaderText = "DisplayName";
            fieldDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[1].HeaderText = "SchemaName";
            fieldDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[2].HeaderText = "Type";
            fieldDataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fieldDataGridView.Columns[3].HeaderText = "isSearchable";
            if (isForResult) {
                fieldDataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                fieldDataGridView.Columns[4].HeaderText = "isUpdated";
            }
        }

        public static List<Field> getEntityFields(IOrganizationService service, String entityTechnicalName, BackgroundWorker worker)
        {
            //Dictionary<String, String> _fields = new Dictionary<string, string>();
            List<Field> _fields = new List<Field>();
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

        public static void formatList(RetrieveEntityResponse _metadata, List<Field> _fields)
        {
            var attributes = FilterAttributes(_metadata.EntityMetadata.Attributes);
            foreach (var field in attributes)
            {
                //_fields.Add(field.LogicalName, field.DisplayName.UserLocalizedLabel != null ? field.DisplayName.UserLocalizedLabel.Label : @"N/A");
                Field attr = new Field();
                attr.displayName = field.DisplayName.UserLocalizedLabel != null ? field.DisplayName.UserLocalizedLabel.Label : @"N/A";
                attr.schemaName = field.LogicalName;
                attr.type = field.AttributeType.Value.ToString();
                attr.IsSearchable = field.IsValidForAdvancedFind.Value.ToString();
                _fields.Add(attr);
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
                            && a.IsRenameable.Value
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
            //TechninaclName,DisplayName
            Dictionary<String, String> selectedFields = new Dictionary<String, String>();
            foreach (DataGridViewRow row in rows)
            {
                selectedFields.Add(row.Cells[1].Value.ToString(), row.Cells[0].Value.ToString());
            }
            return selectedFields;
        }


        public static void UpdateAttributes(IOrganizationService service, String text, String preffixSuffix, String addRemove, int searchable, Dictionary<String, String> fields, String entityName, int languageCode, BackgroundWorker worker) {
            Dictionary<String, String> UpdatedFields = FormatedFieldsName(text, preffixSuffix, addRemove, fields);
            UpdateAttribute(service, entityName, searchable, languageCode, UpdatedFields, worker);
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

        private static void UpdateAttribute(IOrganizationService service , String entityName, int searchable, int languageCode,  Dictionary<String, String> UpdatedFields, BackgroundWorker worker) {
            // Create an ExecuteMultipleRequest object.
            worker.ReportProgress(0, "Preparing Update Request ...");
            var multipleRequest = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = false
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (var field in UpdatedFields.Keys)
            {
                try
                {
                    AttributeMetadata retrievedAttributeMetadata = new AttributeMetadata();
                    retrievedAttributeMetadata.DisplayName = new Label(UpdatedFields[field], languageCode);
                    retrievedAttributeMetadata.LogicalName = field;
                    if (searchable == 0)//Nothing to update when user select Non
                    {
                        retrievedAttributeMetadata.IsValidForAdvancedFind = new BooleanManagedProperty(false);
                    }

                    UpdateAttributeRequest updateRequest = new UpdateAttributeRequest
                    {
                        Attribute = retrievedAttributeMetadata,
                        EntityName = entityName,
                        MergeLabels = false
                    };
                    multipleRequest.Requests.Add(updateRequest);
                }
                catch
                {
                }
            }
            worker.ReportProgress(0, "Updating Attributes Display Name ...");
            service.Execute(multipleRequest);
        }
    }
}
