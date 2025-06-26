using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace MonteCarloApp.Forms
{
    public class DatabaseViewerForm : Form
    {
        private DataGridView dataGridView;
        private Button btnDelete, btnClearAll, btnClose, btnLoadSelected, btnShowGraph;
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
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);

            // DataGridView
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                TabIndex = 0
            };

            // Кнопки
            btnDelete = new Button
            {
                Text = "Удалить выбранное",
                Size = new Size(160, 40),
                Location = new Point(20, 10),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = SystemColors.ControlText,
                TabIndex = 1
            };
            btnDelete.Click += BtnDelete_Click;

            btnClearAll = new Button
            {
                Text = "Очистить все",
                Size = new Size(160, 40),
                Location = new Point(190, 10),
                BackColor = Color.Orange,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TabIndex = 2
            };
            btnClearAll.Click += BtnClearAll_Click;

            btnLoadSelected = new Button
            {
                Text = "Загрузить выбранное",
                Size = new Size(160, 40),
                Location = new Point(360, 10),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TabIndex = 3
            };
            btnLoadSelected.Click += BtnLoadSelected_Click;

            btnShowGraph = new Button
            {
                Text = "Показать график",
                Size = new Size(160, 40),
                Location = new Point(530, 10),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TabIndex = 4
            };
            btnShowGraph.Click += BtnShowGraph_Click;

            btnClose = new Button
            {
                Text = "Закрыть",
                Size = new Size(160, 40),
                Location = new Point(700, 10),
                BackColor = SystemColors.ControlLight,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TabIndex = 5
            };
            btnClose.Click += (s, e) => this.Close();

            // Панель с кнопками
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = SystemColors.Control,
                Padding = new Padding(20, 10, 20, 10)
            };
            buttonPanel.Controls.AddRange(new Control[] { btnDelete, btnClearAll, btnLoadSelected, btnShowGraph, btnClose });

            // Добавление элементов на форму
            this.Controls.Add(dataGridView);
            this.Controls.Add(buttonPanel);
        }

        private void LoadData()
        {
            try
            {
                DataTable data = dbHelper.GetAllResults();
                dataGridView.DataSource = data;
                dataGridView.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных из базы:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления",
                              "Информация",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            try
            {
                int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
                dbHelper.DeleteResult(id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении записи:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить ВСЕ записи?\nЭто действие нельзя отменить!",
                                       "Подтверждение удаления",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    dbHelper.ClearAllResults();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при очистке базы данных:\n{ex.Message}",
                                  "Ошибка",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void BtnLoadSelected_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите запись для загрузки",
                              "Информация",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            try
            {
                DataRowView row = (DataRowView)dataGridView.SelectedRows[0].DataBoundItem;
                double x0 = Convert.ToDouble(row["X0"]);
                double y0 = Convert.ToDouble(row["Y0"]);
                double radius = Convert.ToDouble(row["Radius"]);
                double c = Convert.ToDouble(row["C"]);

                DataSelected?.Invoke(x0, y0, radius, c);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void BtnShowGraph_Click(object sender, EventArgs e)
        {
            try
            {
                using (var graphForm = new StatisticsForm(dbHelper))
                {
                    graphForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении графика:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
    }
}