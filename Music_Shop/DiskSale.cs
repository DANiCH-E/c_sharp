using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Music_Shop
{
    
    public partial class DiskSale : Form
    {
        int selectedRow;
        public DiskSale()
        {
            InitializeComponent();
            LoadData();
        }

        public OleDbConnection myConnection = new OleDbConnection(connectString);
        public static string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Mus_shop.mdb";
        private void LoadData()
        {


            myConnection.Open();

            string query = "SELECT * FROM DiskSale ORDER BY ДискID";

            OleDbCommand command = new OleDbCommand(query, myConnection);

            OleDbDataReader reader = command.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[6]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();
                data[data.Count - 1][3] = reader[3].ToString();
                data[data.Count - 1][4] = reader[4].ToString();
                data[data.Count - 1][5] = reader[5].ToString();
                
            }
            reader.Close();
            myConnection.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[8].Value = RowState.Deleted;
                return;
            }
        }

        private void ClearFields()
        {
            ID_Text.Text = "";
            Name_Text.Text = "";
            AlbText.Text = "";
            Genre.Text = "";
            Count.Text = "";
            Year.Text = "";
            
        }

        private void Update()
        {
            DB db = new DB();
            db.openConnection();
            for (int index = 0; index <= dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[6].Value;

                if (rowState == RowState.Existed)
                    continue;
                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = "DELETE * FROM DiskSale WHERE ДискID = @id";
                    var command = new OleDbCommand(deleteQuery, db.GetConnection());
                    command.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    command.ExecuteNonQuery();
                }
                if (rowState == RowState.Modified)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var alb = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var genre = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var count = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var year = dataGridView1.Rows[index].Cells[5].Value.ToString();
                    

                    var changeQuery = "UPDATE DiskSale SET ID_Диска = @id, Название_диска = @name, Продавец = @alb, Покупатель = @genre, Цена_диска = @count, Дата_покупки = @year WHERE ДискID = @id";
                    var command = new OleDbCommand(changeQuery, db.GetConnection());
                    command.Parameters.Add("@id", OleDbType.VarChar).Value = id;
                    command.Parameters.Add("@name", OleDbType.VarChar).Value = name;
                    command.Parameters.Add("@alb", OleDbType.VarChar).Value = alb;
                    command.Parameters.Add("@genre", OleDbType.VarChar).Value = genre;
                    command.Parameters.Add("@count", OleDbType.VarChar).Value = count;
                    command.Parameters.Add("@year", OleDbType.VarChar).Value = year;
                    
                    command.ExecuteNonQuery();
                }
            }
            db.closeConnection();
        }

        private void Change()
        {
            var SelectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var name = Name_Text.Text;
            var alb = AlbText.Text;
            var genre = Genre.Text;
            var count = Count.Text;
            var year = Year.Text;
            

            if (dataGridView1.Rows[SelectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[SelectedRowIndex].SetValues(name, alb, genre, count, year);
                dataGridView1.Rows[SelectedRowIndex].Cells[5].Value = RowState.Modified;
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            DB db = new DB();
            this.Hide();
            AdminMenu adminmenu = new AdminMenu();
            adminmenu.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myConnection.Open();
            String GroupName = textBox1.Text;
            string query = "SELECT ДискID, Название_диска, Продавец, Покупатель, Цена_диска  FROM DiskSale WHERE Название_диска = @gN";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.Parameters.Add("@gN", OleDbType.VarChar).Value = GroupName;
            OleDbDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();

            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                data.Add(new string[5]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();
                data[data.Count - 1][3] = reader[3].ToString();
                data[data.Count - 1][4] = reader[4].ToString();
            }
            reader.Close();
            myConnection.Close();
            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
            textBox1.Clear();
        }

        private void ShowAllDisks_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DB db = new DB();
            
            AddForm_DiskSale adminmenu = new AddForm_DiskSale();
            adminmenu.Show();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                ID_Text.Text = row.Cells[0].Value.ToString();
                Name_Text.Text = row.Cells[1].Value.ToString();
                AlbText.Text = row.Cells[2].Value.ToString();
                Genre.Text = row.Cells[3].Value.ToString();
                Count.Text = row.Cells[4].Value.ToString();
                Year.Text = row.Cells[5].Value.ToString();
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void DiskSale_Load(object sender, EventArgs e)
        {

        }
    }
}
