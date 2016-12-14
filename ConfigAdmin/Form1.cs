using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConfigAdmin.ViewModels;
using Core.Model;
using Core.Repositories;
using Core.Services;
using Core.Tools;

namespace ConfigAdmin
{
    public partial class Form1 : Form
    {
        private readonly Timer delayedLoadTimer;
        private CleanFolderTargetRepository cleanFolderTargetRepository;
        private ConfigService configService;
        private List<CleanFolderTargetViewModel> targetViewModels;
        private BindingSource dataGridView1BindingSource;

        private const string FolderNameColumn = "FolderPath";
        private const string DeleteColumn = "ActionDelete";

        public Form1()
        {
            InitializeComponent();

            delayedLoadTimer = new Timer
            {
                Interval = 100,
                Enabled = true
            };
            delayedLoadTimer.Tick += DelayedLoad;
            delayedLoadTimer.Start();
        }

        private void DelayedLoad(object sender, EventArgs eventArgs)
        {
            //configService = new ConfigService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\Service\\config.xml"));
            //configService = new ConfigService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..", "FileSysService\\config.xml"));
            configService = new ConfigService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Lubi Datakonsult\\FileSysWatcher\\config.xml"));
            
            cleanFolderTargetRepository = new CleanFolderTargetRepository(configService);

            LoadData();
            delayedLoadTimer.Enabled = false;
        }

        private void LoadData()
        {
            var targets = cleanFolderTargetRepository.GetAll();
            targetViewModels = targets.Select(_ => _.CopyToNew<CleanFolderTargetViewModel>()).ToList();
            dataGridView1BindingSource = new BindingSource
            {
                DataSource = targetViewModels
            };
            dataGridView1.DataSource = dataGridView1BindingSource;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var columnName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;

            if (e.RowIndex >= dataGridView1.RowCount) return;

            if (columnName == FolderNameColumn)
            {
                folderBrowserDialog1.SelectedPath = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;

                var result = folderBrowserDialog1.ShowDialog();
                if (result != DialogResult.OK) return;

                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = folderBrowserDialog1.SelectedPath;
            }
            else if (columnName == DeleteColumn)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                dataGridView1BindingSource.ResetBindings(false);
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var targets = targetViewModels.Select(_ => (CleanFolderTarget)_).ToList();
            cleanFolderTargetRepository.SaveAll(targets);
        }
    }
}
