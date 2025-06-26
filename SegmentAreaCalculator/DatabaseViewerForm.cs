using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

public class DatabaseViewerForm : Form
{
    private DataGridView dataGridView;
    private Button btnDelete, btnClearAll, btnClose, btnLoadSelected;
    private DBHelper dbHelper;
    public event Action<double, double, double, double> DataSelected;

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
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
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

        btnLoadSelected = new Button
        {
            Text = "Загрузить выбранное",
            Dock = DockStyle.Left,
            Width = 150,
            BackColor = Color.LightBlue
        };
        btnLoadSelected.Click += BtnLoadSelected_Click;

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
        buttonPanel.Controls.AddRange(new Control[] { btnDelete, btnClearAll, btnLoadSelected, btnClose });

        this.Controls.Add(dataGridView);
        this.Controls.Add(buttonPanel);
    }

    private void BtnLoadSelected_Click(object sender, EventArgs e)
    {
        if (dataGridView.SelectedRows.Count > 0)
        {
            var row = dataGridView.SelectedRows[0];
            double x0 = Convert.ToDouble(row.Cells["X0"].Value);
            double y0 = Convert.ToDouble(row.Cells["Y0"].Value);
            double radius = Convert.ToDouble(row.Cells["Radius"].Value);
            double c = Convert.ToDouble(row.Cells["C"].Value);

            DataSelected?.Invoke(x0, y0, radius, c);
            this.Close();
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите запись для загрузки.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
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