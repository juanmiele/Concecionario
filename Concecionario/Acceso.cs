using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.InteropServices;


namespace Concecionario
{
    class Acceso
    {


        private SqlConnection conexion;
        private SqlTransaction transaccion;

        public void IniciarTransaccion()
        {
            if (conexion != null && conexion.State == System.Data.ConnectionState.Open && transaccion == null)
            {
                transaccion = conexion.BeginTransaction();

            }

        }
        public void ConfirmarTransaccion()
        {
            transaccion.Commit();
            transaccion.Dispose();
            transaccion = null;
        }

        public void CancelarTransaccion()
        {
            transaccion.Rollback();
            transaccion.Dispose();
            transaccion = null;
        }

        public bool Abrir()
        {
            bool ok;
            if (conexion != null && conexion.State == System.Data.ConnectionState.Open)
            {
                ok = false;
            }
            else
            {
                conexion = new SqlConnection();
                try
                {
                    conexion.ConnectionString = @"Data Source=.\; Initial Catalog=LUG; Integrated Security=SSPI";
                    conexion.Open();
                    ok = true;
                }
                catch (SqlException ex)
                {
                    ok = false;
                }
            }
            return ok;
        }
        public void Cerrar()
        {

            if (conexion != null)
            {
                conexion.Close();
                conexion.Dispose();
                conexion = null;
                GC.Collect();
            }

        }
        public SqlDataReader Leer(string SQL)
        {
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandText = SQL;
            comando.CommandType = System.Data.CommandType.Text;
            if (transaccion != null)
            {
                comando.Transaction = transaccion;
            }

            SqlDataReader lector = comando.ExecuteReader();

            return lector;

        }

        //cuando quiero ejecutar procedimientos agregar el paramentro System.Data.CommandType tipo = System.Data.CommandType.Text
        public int Escribir(string SQL, List<SqlParameter> parametros = null, System.Data.CommandType tipo = System.Data.CommandType.Text)
        {
            int filasAfectadas;
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandText = SQL;
            comando.CommandType = tipo;
            //esta declarado que si no le pongo otro pone por defecto el de abajo
            //comando.CommandType = System.Data.CommandType.Text;


            if (parametros != null && parametros.Count > 0)
            {

                #region "variantes de agregado de parámetros"
                /*  foreach (SqlParameter p in parametros)
                  {
                      comando.Parameters.Add(p);
                  }

                foreach (SqlParameter p in parametros)
               {
                   comando.Parameters.AddWithValue (p.ParameterName,p.Value);
               }
               */
                #endregion

                comando.Parameters.AddRange(parametros.ToArray());
            }
            if (transaccion != null)
            {
                comando.Transaction = transaccion;
            }
            try
            {
                filasAfectadas = comando.ExecuteNonQuery();
            }

            catch (SqlException ex)
            {
                filasAfectadas = -1;
            }
            catch (Exception ex)
            {
                filasAfectadas = -2;
            }
            return filasAfectadas;


        }
        public bool AltaBaja(string SQL)
        {
            bool ok = false; ;
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandText = SQL;
            comando.CommandType = System.Data.CommandType.Text;
            if (transaccion != null)
            {
                comando.Transaction = transaccion;
            }
            try
            {
                comando.ExecuteNonQuery();
                ok = true;
            }
            catch (SqlException ex)
            {

            }
            catch (Exception ex)
            {

            }

            return ok;
        }
        public int LeerEscalar(string SQL)
        {
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandText = SQL;
            comando.CommandType = System.Data.CommandType.Text;

            if (transaccion != null)
            {
                comando.Transaction = transaccion;
            }

            int resultado = int.Parse(comando.ExecuteScalar().ToString());
            return resultado;
        }

    }
}
