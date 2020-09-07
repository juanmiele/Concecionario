using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace Concecionario
{
    public partial class Form1 : Form
    {
        private List<Cliente> clientes = new List<Cliente>();
        private Acceso acceso = new Acceso();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void EnlazarC()
        {
            clientes.Clear();
            string SQL = "Select ID_Cliente,NOMBRE,Apellido,DNI from Cliente";
            SqlDataReader lector = acceso.Leer(SQL);
            while (lector.Read())
            {
                //Obteniendo los campos
                Cliente cliente = new Cliente();
                cliente.Id = int.Parse(lector["ID_Cliente"].ToString());
                cliente.Nombre = lector.GetString(1);
                cliente.Apellido = lector.GetString(2);
                cliente.Dni = int.Parse((lector["dni"].ToString()));
                clientes.Add(cliente);
            }
            lector.Close();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = clientes;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (acceso.Abrir())
            {
                label1.Text = "Conectado";
                EnlazarC();
            }
            else
            {
                label1.Text = "Error de conexion";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            int resultado = 0;
            SqlParameter nombre = new SqlParameter();
            nombre.ParameterName = "@nom";
            nombre.Value = textBox1.Text;
            nombre.SqlDbType = SqlDbType.Text;
            parametros.Add(nombre);
            SqlParameter apellido = new SqlParameter();
            apellido.ParameterName = "@ape";
            apellido.Value = textBox2.Text;
            apellido.SqlDbType = SqlDbType.Text;
            parametros.Add(apellido);
            SqlParameter dni = new SqlParameter();
            dni.ParameterName = "@dni";
            dni.Value = textBox3.Text;
            dni.SqlDbType = SqlDbType.Int;
            parametros.Add(dni);
            SqlParameter parametro = new SqlParameter("@id", 0);
            parametro.SqlDbType = SqlDbType.Int;
            string SQL = "Select ISNULL ( MAX(Id_cliente) , 0) +1 From cliente";
            parametro.Value = acceso.LeerEscalar(SQL);
            parametros.Add(parametro);
            SQL = "INSERT INTO Cliente (ID_Cliente,NOMBRE,Apellido,DNI) values (@id,@nom,@ape,@dni)";
            resultado = acceso.Escribir(SQL, parametros);
            if (resultado > 0)
            {
                label1.Text = "Alta OK";
                EnlazarC();
            }
            else
            {
                label1.Text = "Error alta";
            }
        }

    }
}
