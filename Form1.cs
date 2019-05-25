
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ItemSlotListDatEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        String[] items = new string[18] {
            "Green Shell",
            "Red Shell",
            "Banana",
            "Fake Itembox",
            "Mushroom",
            "Triple Mushroom",
            "Bob-omb",
            "Spiny Shell",
            "Thunderbolt",
            "Triple Green Shells",
            "Triple Banana",
            "Triple Red Shell",
            "Star",
            "Golden Mushroom",
            "Bullet Bill",
            "Blooper",
            "Boo",
            "Unused item"
        };

        String[] tableNames = new string[11]
        {
            "Grand Prix Player",
            "Grand Prix CPU",
            "VS Player",
            "VS CPU",
            "Online Slot 1",
            "Online Slot 2",
            "Special Itemboxes",
            "Balloon Battle Player",
            "Balloon Battle CPU",
            "Shine Runners",
            "Shine Runners CPU"
        };

        private int currentButton = 0;

        private void selectTable(object sender, EventArgs e)
        {
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            //dataGridView1.DataSource = fileClass.itemTableShine[0];
            //dataGridView1.AutoGenerateColumns = true;
            var button = (ToolStripButton)sender;
            foreach (var pb in toolStrip1.Controls.OfType<ToolStripButton>())
            {
                pb.Checked = false;
            }
            currentButton = Int32.Parse(button.Tag.ToString());
            /*button.Checked = true;
             * 
            int curpos = startingPointers[Int32.Parse(button.Tag.ToString())];
            */
            int columns;
            int rows;
            if (currentButton < 6)
            {
                columns = fileClass.itemTableRace[currentButton].columnsCount;
                rows = fileClass.itemTableRace[currentButton].rowsCount;
                for (int i = 0; i < columns; i++)
                {
                    dataGridView1.Columns.Add("column" + i, "Place #" + (i + 1));
                }

                for (int i = 0; i < rows; i++)
                {
                    var row = dataGridView1.Rows.Add();
                    DataGridViewRow R = dataGridView1.Rows[i];
                    R.HeaderCell.Value = items[i];
                    for (int j = 0; j < columns; j++)
                    {
                        R.Cells[j].Value = fileClass.itemTableRace[currentButton].items[i].positions[j];
                    }
                }
            }
            else if (currentButton < 7)
            {
                columns = fileClass.itemTableSpecialItemBox[currentButton-6].columnsCount;
                rows = fileClass.itemTableSpecialItemBox[currentButton-6].rowsCount;
                for (int i = 0; i < columns; i++)
                {
                    dataGridView1.Columns.Add("column" + i, "Itembox index " + i);
                }

                for (int i = 0; i < rows; i++)
                {
                    var row = dataGridView1.Rows.Add();
                    DataGridViewRow R = dataGridView1.Rows[i];
                    R.HeaderCell.Value = items[i];
                    for (int j = 0; j < columns; j++)
                    {
                        R.Cells[j].Value = fileClass.itemTableSpecialItemBox[currentButton-6].items[i].itemboxes[j];
                    }
                }
            }
            else if (currentButton < 9)
            {
                columns = fileClass.itemTableBalloon[currentButton-7].columnsCount;
                rows = fileClass.itemTableBalloon[currentButton-7].rowsCount;
                for (int i = 0; i < columns; i++)
                {
                    dataGridView1.Columns.Add("column" + i, (3-i) + " balloons left");
                }

                for (int i = 0; i < rows; i++)
                {
                    var row = dataGridView1.Rows.Add();
                    DataGridViewRow R = dataGridView1.Rows[i];
                    R.HeaderCell.Value = items[i];
                    for (int j = 0; j < columns; j++)
                    {
                        R.Cells[j].Value = fileClass.itemTableBalloon[currentButton-7].items[i].balloons[j];
                    }
                }
            }
            else
            {
                columns = fileClass.itemTableShine[currentButton-9].columnsCount;
                rows = fileClass.itemTableShine[currentButton-9].rowsCount;
                for (int i = 0; i < columns; i++)
                {
                    dataGridView1.Columns.Add("column" + i, "Shine coefficient " + i);
                }

                for (int i = 0; i < rows; i++)
                {
                    var row = dataGridView1.Rows.Add();
                    DataGridViewRow R = dataGridView1.Rows[i];
                    R.HeaderCell.Value = items[i];
                    for (int j = 0; j < columns; j++)
                    {
                        R.Cells[j].Value = fileClass.itemTableShine[currentButton-9].items[i].shines[j];
                    }
                }
            }

            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;


            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }/*

            button.Text = "0x" + curpos.ToString("X");*/
            //}*/
        }

        ItemSlotListDat fileClass;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open ItemSlotList.dat";

            openFileDialog1.DefaultExt = "dat";
            openFileDialog1.Filter = "ItemSlotList (*.dat)|*.dat";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                byte[] file = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                fileClass = new ItemSlotListDat(file);
                for (int i=0; i<fileClass.amountsOfTables; i++)
                {
                    ToolStripButton tsb = new ToolStripButton();
                    tsb.Text = tableNames[i];
                    tsb.Tag = i;
                    tsb.Click += new EventHandler(selectTable);
                    toolStrip1.Items.Add(tsb);
                }
                button3.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] file = fileClass.Write();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save ItemSlotList.dat";
            saveFileDialog1.DefaultExt = "dat";
            saveFileDialog1.Filter = "ItemSlotList Files (*.dat)|*.dat";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "") System.IO.File.WriteAllBytes(saveFileDialog1.FileName, file);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int value;
            try
            {
                value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            }
            catch {
                MessageBox.Show("Not a proper integer value!", "Not an integer!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
                return;
            }

            if (value > 200)
            {
                MessageBox.Show("The value is too big!", "Too big!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "200";
            }
            if (value < 0)
            {
                MessageBox.Show("The value is too small!", "Too small!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            } else {
                if (currentButton < 6)
                {
                    fileClass.itemTableRace[currentButton].items[e.RowIndex].positions[e.ColumnIndex] = (byte) value;
                } else if (currentButton < 7)
                {
                    fileClass.itemTableSpecialItemBox[currentButton-6].items[e.RowIndex].itemboxes[e.ColumnIndex] = (byte) value;
                } else if (currentButton < 9)
                {
                    fileClass.itemTableBalloon[currentButton - 7].items[e.RowIndex].balloons[e.ColumnIndex] = (byte) value;
                } else
                {
                    fileClass.itemTableShine[currentButton - 9].items[e.RowIndex].shines[e.ColumnIndex] = (byte) value;
                }
            }
        }
    }
}
