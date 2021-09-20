using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Diagnostics;
using Microsoft.Crm.Sdk.Messages;
using Prefix_Suffix_Components_Name.Manager;

namespace Prefix_Suffix_Fields_Name
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;
        DataTable dtEntities = null;
        DataTable dtFields = null;
        DataTable dtFieldsUpdate = null;
        DataTable dtFieldsUpdateResult = null;
        //Dictionary<String, String> entityFields = null;
        List<Field> entityFields = null;
        String entitySelectedName = String.Empty;
        String entitySelectedSchemaName = String.Empty;
        private int languageCode;
        private int isSearchable;

        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        private void loadEntitiesButto_Click(object sender, EventArgs e)
        {
            ExecuteMethod(loadEntities);
        }

        private void loadEntities()
        {
            fieldsTextSearch.Enabled = false;
            PSTextBox.Enabled = false;
            ProceedButton.Enabled = false;
            //searchEntity.Enabled = false;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Entities...",
                Work = (worker, args) =>
                {
                    #region Variables
                    dtEntities = new DataTable();
                    languageCode = Service.RetrieveMultiple(new QueryExpression("organization")
                    {
                        ColumnSet = new ColumnSet("languagecode"),
                    }).Entities.First().GetAttributeValue<int>("languagecode");

                    #endregion
                    #region getEntitiesMetadata
                    RetrieveMetadataChangesResponse _allEntitiesResp = PreffixSuffixFieldsNameManager.GetEntitiesMetadat(Service);
                    #endregion

                    worker.ReportProgress(0, string.Format("Metadata has been retrieved!"));

                    #region Entities Data Table Set
                    dtEntities.Columns.Add("Analyse", typeof(bool));
                    dtEntities.Columns.Add("DisplayName", typeof(string));
                    dtEntities.Columns.Add("SchemaName", typeof(string));

                    foreach (var item in _allEntitiesResp.EntityMetadata)
                    {
                        DataRow row = dtEntities.NewRow();
                        row["Analyse"] = false;
                        row["DisplayName"] = item.DisplayName.LocalizedLabels.Count > 0 ? item.DisplayName.UserLocalizedLabel.Label.ToString() : null;
                        row["SchemaName"] = item.LogicalName;

                        dtEntities.Rows.Add(row);
                    }
                    #endregion
                    args.Result = dtEntities;
                },
                ProgressChanged = e =>
                {
                    // If progress has to be notified to user, use the following method:
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = (DataTable)args.Result;
                    if (result != null)
                    {
                        #region Set Retrieved Data in the Data Grid View
                        PreffixSuffixFieldsNameManager.SetEntitiesGridViewHeaders(result, entityDataGridView);
                        #endregion
                        #region ManageComponenets
                        entityTextSearch.Enabled = true;
                        loadFieldsButton.Enabled = true;
                        fieldsUpdateDataGridView.DataSource = null;
                        updateFieldsResultDataGridView.DataSource = null;
                        fieldsDataGridView.DataSource = null;
                        PSTextBox.Text = String.Empty;
                        fieldsTextSearch.Text = "Search";
                        entityTextSearch.Text = "Search";
                        #endregion
                    }
                }
            });
        }

        private void loadFieldsButton_Click(object sender, EventArgs e)
        {
            String[] entitySelected = PreffixSuffixFieldsNameManager.SelectedEntity(entityDataGridView.Rows);
            if (entitySelected.Length > 0)
            {
                entitySelectedName = entitySelected[0];
                entitySelectedSchemaName = entitySelected[1];
                ExecuteMethod(GetEntityFields);
            }
            else
            {
                MessageBox.Show("No Entity Was Selected! Please Select an Entity", "Warning");
                return;
            }
        }

        private void GetEntityFields()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Analysing ...",
                Work = (worker, args) =>
                {
                    try
                    {
                        dtFields = new DataTable();
                        entityFields = PreffixSuffixFieldsNameManager.getEntityFields(Service, entitySelectedSchemaName, worker);

                        #region Entity Fiels Metadata  Set
                        dtFields.Columns.Add("Display Name", typeof(string));
                        dtFields.Columns.Add("Schema Name", typeof(string));
                        dtFields.Columns.Add("Type", typeof(string));
                        dtFields.Columns.Add("IsSearchable", typeof(string));

                        #endregion

                        foreach (var item in entityFields)
                        {
                            DataRow row = dtFields.NewRow();
                            row["Display Name"] = item.displayName;
                            row["Schema Name"] = item.schemaName;
                            row["Type"] = item.type;
                            row["IsSearchable"] = item.IsSearchable;

                            dtFields.Rows.Add(row);
                        }

                        args.Result = dtFields;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Warning");
                    }
                },
                ProgressChanged = e =>
                {
                    // If progress has to be notified to user, use the following method:
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = (DataTable)args.Result;
                    if (result != null)
                    {
                        PreffixSuffixFieldsNameManager.SetFieldsGridViewHeaders(result, fieldsDataGridView);
                    }
                    fieldsTextSearch.Enabled = true;
                    PSTextBox.Enabled = true;
                    ProceedButton.Enabled = true;
                    searchable.Enabled = true;
                    searchable.SelectedIndex = 1;
                }
            });
        }

        private void entityDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                foreach (DataGridViewRow row in this.entityDataGridView.Rows)
                {
                    if ((bool)entityDataGridView.Rows[e.RowIndex].Cells["Analyse"].Value)
                        continue;
                    row.Cells["Analyse"].Value = false;
                }
                this.entityDataGridView.Rows[e.RowIndex].Cells["Analyse"].Value = !(bool)entityDataGridView.Rows[e.RowIndex].Cells["Analyse"].Value;
            }
        }

        private void ProceedButton_Click(object sender, EventArgs e)
        {

            if (PSTextBox.Text == String.Empty)
                MessageBox.Show("Please Enter a value to Preffix/Suffix With", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                ExecuteMethod(ProceedButtonFunction);
        }

        private void ProceedButtonFunction() {
            isSearchable = searchable.SelectedIndex;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Processing ...",
                Work = (worker, args) =>
                {
                    try
                    {
                        var text = PSTextBox.Text;
                        var preffixSuffix = preffixButton.Checked;
                        var addDelete = AddButton.Checked;

                        Dictionary<String, String> selectedFields = PreffixSuffixFieldsNameManager.SelectedFields(fieldsUpdateDataGridView.Rows);
                        PreffixSuffixFieldsNameManager.UpdateAttributes(Service, text, preffixSuffix ? "Preffix" : "Suffix", addDelete ? "Add" : "Remove", isSearchable, selectedFields, entitySelectedSchemaName, languageCode, worker);
                        List<Field> entityUpdatedFields = UpdatedFieldsValues(Service, selectedFields, entitySelectedSchemaName);
                        worker.ReportProgress(0, "Check The Update Result ...");

                        dtFieldsUpdateResult = new DataTable();
                        dtFieldsUpdateResult.Columns.Add("DisplayName", typeof(string));
                        dtFieldsUpdateResult.Columns.Add("SchemaName", typeof(string));
                        dtFieldsUpdateResult.Columns.Add("Type", typeof(string));
                        dtFieldsUpdateResult.Columns.Add("isSearchable", typeof(string));
                        dtFieldsUpdateResult.Columns.Add("isUpdated", typeof(string));


                        entityUpdatedFields = entityUpdatedFields.Where(x => selectedFields.ContainsKey(x.schemaName)).ToList<Field>();


                        foreach (var item in entityUpdatedFields)
                        {
                            DataRow row = dtFieldsUpdateResult.NewRow();
                            row["DisplayName"] = item.displayName;
                            row["SchemaName"] = item.schemaName;
                            row["Type"] = item.type;
                            row["isSearchable"] = item.IsSearchable;
                            row["isUpdated"] = item.displayName == selectedFields[item.schemaName] ? "NO" : "YES";

                            dtFieldsUpdateResult.Rows.Add(row);
                        }
                        worker.ReportProgress(0, "Processing ...");

                        args.Result = dtFieldsUpdateResult;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Warning");
                    }
                },
                ProgressChanged = e =>
                {
                    // If progress has to be notified to user, use the following method:
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = (args) =>
                {
                    PreffixSuffixFieldsNameManager.SetFieldsGridViewHeaders((DataTable)args.Result, updateFieldsResultDataGridView, true);
                    loadFieldsButton.PerformClick();
                    dtFieldsUpdate.Clear();
                }
            });

        }

        private void entityTextSearch_TextChanged(object sender, EventArgs e)
        {
            FilterEntities(entityDataGridView, entityTextSearch, dtEntities);
        }

        private List<Field> UpdatedFieldsValues(IOrganizationService service, Dictionary<String,String> fields, String entityName) {
            //Dictionary<String, String> fieldsUpdate = new Dictionary<string, string>();
            List<Field> fieldsUpdate = new List<Field>();
            string fieldsRequest = String.Empty;
            foreach (String field in fields.Keys)
            {
                fieldsRequest += field;
            }

            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = entityName,
                RetrieveAsIfPublished = true
            };

            RetrieveEntityResponse retrieveEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveEntityRequest);
            PreffixSuffixFieldsNameManager.formatList(retrieveEntityResponse, fieldsUpdate);
            return fieldsUpdate;
        }

        private void FilterEntities(DataGridView dataGrid, TextBox textBoxName, DataTable dataTable)
        {

            if (textBoxName.Text == "" && dataTable.Rows.Count > 0 && dataGrid.Rows.Count != dataTable.Rows.Count)
            {
                PreffixSuffixFieldsNameManager.SetEntitiesGridViewHeaders(dataTable, dataGrid);
            }
            else if (textBoxName.Text != "Search" && textBoxName.Text != "" && dataTable.Rows.Count > 0)
            {
                string searchValue = textBoxName.Text.ToLower();
                try
                {
                    DataRow[] filtered = dataTable.Select("DisplayName LIKE '%" + searchValue + "%' OR SchemaName LIKE '%" + searchValue + "%'");
                    if (filtered.Count() > 0)
                    {
                        PreffixSuffixFieldsNameManager.SetEntitiesGridViewHeaders(filtered.CopyToDataTable(), dataGrid);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid Search Character. Please do not use ' [ ] within searches.");
                }
            }
        }


        private void FilterFields(DataGridView dataGrid, TextBox textBoxName, DataTable dataTable)
        {

            if (textBoxName.Text == "" && dataTable.Rows.Count > 0 && dataGrid.Rows.Count != dataTable.Rows.Count)
            {
                PreffixSuffixFieldsNameManager.SetFieldsGridViewHeaders(dataTable, dataGrid);
            }
            else if (textBoxName.Text != "Search" && textBoxName.Text != "" && dataTable.Rows.Count > 0)
            {
                string searchValue = textBoxName.Text.ToLower();
                try
                {
                    DataRow[] filtered = dataTable.Select("[Display Name] LIKE '%" + searchValue + "%' OR [Schema Name] LIKE '%" + searchValue + "%'");
                    if (filtered.Count() > 0)
                    {
                        PreffixSuffixFieldsNameManager.SetFieldsGridViewHeaders(filtered.CopyToDataTable(), dataGrid);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid Search Character. Please do not use ' [ ] within searches.");
                }
            }
        }

        private void entityTextSearch_Click(object sender, EventArgs e)
        {
            if (entityTextSearch.Text == "Search")
            {
                entityTextSearch.Clear();
            }
        }

        private void fieldsTextSearch_TextChanged(object sender, EventArgs e)
        {
            FilterFields(fieldsDataGridView, fieldsTextSearch, dtFields);
        }

        private void fieldsTextSearch_Click(object sender, EventArgs e)
        {
            if (fieldsTextSearch.Text == "Search")
            {
                fieldsTextSearch.Clear();
            }
        }

        private void AddFieldsToGridToBeUpdated()
        {
            if (dtFieldsUpdate == null)
            {
                dtFieldsUpdate = new DataTable();
                #region Entity Fiels Metadata  Set
                dtFieldsUpdate.Columns.Add("Display Name", typeof(string));
                dtFieldsUpdate.Columns.Add("Schema Name", typeof(string));
                dtFieldsUpdate.Columns.Add("Type", typeof(string));
                dtFieldsUpdate.Columns.Add("isSearchable", typeof(string));

                #endregion
            }
            int selectedrowindex = fieldsDataGridView.SelectedCells[0].RowIndex;
            DataGridViewRow selectedRow = fieldsDataGridView.Rows[selectedrowindex];
            Boolean alreadyExiste = false;
            foreach (DataGridViewRow row in fieldsUpdateDataGridView.Rows)
            {
                if (row.Cells["Schema Name"].Value == selectedRow.Cells["Schema Name"].Value) {
                    alreadyExiste = true;
                }
            }
            if (!alreadyExiste) {
                DataRow newRow = dtFieldsUpdate.NewRow();
                newRow["Display Name"] = selectedRow.Cells["Display Name"].Value;
                newRow["Schema Name"] = selectedRow.Cells["Schema Name"].Value;
                newRow["Type"] = selectedRow.Cells["Type"].Value;
                newRow["isSearchable"] = selectedRow.Cells["isSearchable"].Value;


                dtFieldsUpdate.Rows.Add(newRow);
            }

            PreffixSuffixFieldsNameManager.SetFieldsGridViewHeaders(dtFieldsUpdate, fieldsUpdateDataGridView, false);
        }

        private void fieldsDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddFieldsToGridToBeUpdated();
        }

        private void fieldsUpdateDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.fieldsUpdateDataGridView.SelectedRows.Count > 0)
            {
                fieldsUpdateDataGridView.Rows.RemoveAt(this.fieldsUpdateDataGridView.SelectedRows[0].Index);
            }
        }

        private void updateFieldsResultDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv.Columns[e.ColumnIndex].Name.Equals("isUpdated"))
            {
                if (e.Value != null && e.Value.ToString().Trim() == "YES")
                {
                    dgv.Rows[e.RowIndex].Cells["isUpdated"].Style.BackColor = Color.Green;
                }
                else
                {
                    dgv.Rows[e.RowIndex].Cells["isUpdated"].Style.BackColor = Color.Red;
                }
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string message = "";

            message += "1. Click on Load Entities to load all entities in your organization";
            message += Environment.NewLine;
            message += "2. After selection an entity, click on Load Fields";
            message += Environment.NewLine;
            message += "3. Double click on the fields you want to update, each field you select will be added to the grid on right top";
            message += Environment.NewLine;
            message += "4. Select the type of update you want Preffix/Suffix and Add/Delete";
            message += Environment.NewLine;
            message += "5. Set the text you want to add to the selected fields display name, and click on Proceed";
            message += Environment.NewLine;
            message += "6. After updating the display name of the selected fields, the grid right bottom display the new fields with display name, and a status if the field is updated or not";
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += "If you have any issues please log them via GitHub and/or contact me at hamzamraoui11@gmail.com";

            MessageBox.Show(message);
        }

        private void byButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/hamza-amraoui/");
        }

    }
}