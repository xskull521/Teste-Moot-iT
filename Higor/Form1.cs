using Microsoft.Data.Sqlite;
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

namespace Higor
{
    public partial class Form1 : Form
    {
        private bool incluindo = false;
        private int idVooEditado = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VerificarBanco();

            CarregarDados();

        }

        private void VerificarBanco()
        {
            var connection = AbrirConexao();
            var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS TB_VOO (ID_VOO integer PRIMARY KEY AUTOINCREMENT, DATA_VOO datetime, CUSTO numeric(10,2), DISTANCIA integer, CAPTURA char(1), NIVEL_DOR integer)";
            command.ExecuteNonQuery();
            connection.Close();
        }

        private void CarregarDados()
        {
            var connection = AbrirConexao();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT ID_VOO, DATA_VOO, CAPTURA, NIVEL_DOR FROM TB_VOO";

            var dataTable = new DataTable();

            dataTable.Load(command.ExecuteReader());

            dataGridView1.DataSource = dataTable;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].HeaderText = "Data";
            dataGridView1.Columns[2].HeaderText = "Captura";
            dataGridView1.Columns[3].HeaderText = "Nível de dor";
        }

        private void LimparCampos()
        {
            txCusto.Text = "";
            txData.Text = "";
            txDistancia.Text = "";
            txNivelDor.Text = "";
            rbNao.Checked = false;
            rbSim.Checked = false;
            btSalvar.Enabled = false;
            btCancelar.Enabled = false;
        }

        private SqliteConnection AbrirConexao()
        {
            var connection = new SqliteConnection("Data Source=acme.sqlite.db");

            connection.Open();

            return connection;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var connection = AbrirConexao();
                var command = connection.CreateCommand();

                var id = dataGridView1.SelectedRows[0].Cells[0].Value;
                idVooEditado = Convert.ToInt32(id);

                command.CommandText = "SELECT * FROM TB_VOO WHERE ID_VOO = " + id;

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    txData.Text = reader["DATA_VOO"].ToString();
                    txCusto.Text = reader["CUSTO"].ToString();
                    txDistancia.Text = reader["DISTANCIA"].ToString();

                    if (reader["CAPTURA"].ToString() == "N")
                        rbNao.Checked = true;
                    else
                        rbSim.Checked = true;

                    txNivelDor.Text = reader["NIVEL_DOR"].ToString();


                    btSalvar.Enabled = true;
                    btCancelar.Enabled = true;
                }
                connection.Close();
            }
        }

        private void btExcluir_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var id = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                var connection = AbrirConexao();

                var command = connection.CreateCommand();

                command.CommandText = "DELETE FROM TB_VOO WHERE ID_VOO = " + id;

                command.ExecuteNonQuery();

                connection.Close();
                CarregarDados();
            }
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
            incluindo = false;
        }

        private void btIncluir_Click(object sender, EventArgs e)
        {
            LimparCampos();
            incluindo = true;
            btSalvar.Enabled = true;
            btCancelar.Enabled = true;
        }

        private void btSalvar_Click(object sender, EventArgs e)
        {
            var connection = AbrirConexao();
            var command = connection.CreateCommand();

            if(incluindo == true)
            {
                var captura = "S";

                if (rbNao.Checked == true)
                    captura = "N";

                command.CommandText = "INSERT INTO TB_VOO (DATA_VOO, CUSTO, DISTANCIA, CAPTURA, NIVEL_DOR) VALUES ('" + txData.Text +
                    "','" + txCusto.Text +
                    "','" + txDistancia.Text +
                    "','" + captura +
                    "','" + txNivelDor.Text +
                    "')";
                command.ExecuteNonQuery();
            }
            else
            {
                var captura = "S";

                if (rbNao.Checked == true)
                    captura = "N";

                command.CommandText = "UPDATE TB_VOO SET DATA_VOO = '" + txData.Text +
                    "', CUSTO = '" + txCusto.Text +
                    "', DISTANCIA = '" + txDistancia.Text +
                    "', CAPTURA = '" + captura +
                    "', NIVEL_DOR = '" + txNivelDor.Text +
                    "' WHERE ID_VOO = " + idVooEditado;

                command.ExecuteNonQuery();

            }

            connection.Close();

            LimparCampos();
            CarregarDados();
        }
    }
}
