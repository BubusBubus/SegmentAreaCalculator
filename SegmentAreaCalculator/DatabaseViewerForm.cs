using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

public class DatabaseViewerForm : Form
{
    private DataGridView dataGridView;
    private Button btnDelete, btnClearAll, btnClose;
    private DBHelper dbHelper;

    public DatabaseViewerForm(DBHelper dbHelper)
    {
        this.dbHelper = dbHelper;
        InitializeComponents();
        LoadData();
    }

    private void InitializeComponents()
    {
        this.Text = "Просмотр базы данных результатов";
        this.Size = new Size(800, 600);

        dataGridView = new DataGridView
        {
            Dock = DockStyle.Top,
            Height = 500,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        btnDelete = new Button
        {
            Text = "Удалить выбранное",
            Dock = DockStyle.Left,
            Width = 150
        };
        btnDelete.Click += BtnDelete_Click;

        btnClearAll = new Button
        {
            Text = "Очистить все",
            Dock = DockStyle.Left,
            Width = 150
        };
        btnClearAll.Click += BtnClearAll_Click;

        btnClose = new Button
        {
            Text = "Закрыть",
            Dock = DockStyle.Right,
            Width = 150
        };
        btnClose.Click += (s, e) => this.Close();

        Panel buttonPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50
        };
        buttonPanel.Controls.AddRange(new Control[] { btnDelete, btnClearAll, btnClose });

        this.Controls.Add(dataGridView);
        this.Controls.Add(buttonPanel);
    }

    private void LoadData()
    {
        dataGridView.DataSource = dbHelper.GetAllResults();
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (dataGridView.SelectedRows.Count > 0)
        {
            int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
            dbHelper.DeleteResult(id);
            LoadData();
        }
    }

    private void BtnClearAll_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show("Вы уверены, что хотите удалить все записи?",
                                   "Подтверждение",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            dbHelper.ClearAllResults();
            LoadData();
        }
    }
}