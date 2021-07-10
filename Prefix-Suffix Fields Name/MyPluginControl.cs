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

namespace Prefix_Suffix_Fields_Name
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;
        DataTable dtEntities = null;
        DataTable dtFields = null;
        Dictionary<String, String> entityFields = null;
        String entitySelectedName = String.Empty;
        String entitySelectedSchemaName = String.Empty;

        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

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

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbSample_Click(object sender, EventArgs e)
        {
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
            InitComponents();
            //searchEntity.Enabled = false;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Entities...",
                Work = (worker, args) =>
                {
                    #region Variables
                    dtEntities = new DataTable();
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
                        #region ManageComponenetVisibility
                        //searchEntity.Enabled = true;
                        //analyseButton.Enabled = true;
                        #endregion
                    }
                }
            });
        }

        private void InitComponents()
        {
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
            //fieldTypeCombobox.Enabled = false;
            //buttonExport.Enabled = false;
            //analyseButton.Enabled = false;
            //DisplayPercentageCheckbox.Enabled = false;
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
                        dtFields.Columns.Add("Select", typeof(bool));
                        dtFields.Columns.Add("DisplayName", typeof(string));
                        dtFields.Columns.Add("SchemaName", typeof(string));
                        #endregion

                        foreach (var item in entityFields)
                        {
                            DataRow row = dtFields.NewRow();
                            row["Select"] = false;
                            row["DisplayName"] = item.Value;
                            row["SchemaName"] = item.Key;

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
            var entityName = String.Empty;
            var text = PSTextBox.Text;
            Dictionary <String, String> selectedFields = PreffixSuffixFieldsNameManager.SelectedFields(fieldsDataGridView.Rows);
            PreffixSuffixFieldsNameManager.UpdateNames(Service, text, preffixButton.Checked ? "Preffix" : "Suffix", AddButton.Checked ? "Add":"Remove", selectedFields, entitySelectedSchemaName);
        }

        private void entityTextSearch_TextChanged(object sender, EventArgs e)
        {
            FilterDate(entityDataGridView, entityTextSearch, dtEntities);
        }

        private void FilterDate(DataGridView dataGrid, TextBox textBoxName, DataTable dataTable)
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

        private void entityTextSearch_Click(object sender, EventArgs e)
        {
            if (entityTextSearch.Text == "Search")
            {
                entityTextSearch.Clear();
            }
        }

        private void fieldsTextSearch_TextChanged(object sender, EventArgs e)
        {
            FilterDate(fieldsDataGridView, fieldsTextSearch, dtFields);
        }

        private void fieldsTextSearch_Click(object sender, EventArgs e)
        {
            if (fieldsTextSearch.Text == "Search")
            {
                entityTextSearch.Clear();
            }
        }
    }
}