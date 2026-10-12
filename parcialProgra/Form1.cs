using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace parcialProgra
{
    public partial class Form1 : Form
    {
        private string cadenaConexion = @"Server=(localdb)\MSSQLLocalDB;Database=ParcialProgramacion1;Trusted_Connection=True;";
        private int idProductoSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConsultarProductos();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {

            ConsultarProductos();

        }

        private void ConsultarProductos()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    string query = "SELECT IdProducto, Nombre, Categoria, Precio, Existencias FROM dbo.Productos";
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(query, conexion))
                    {
                        DataTable dt = new DataTable();
                        adaptador.Fill(dt);
                        dgvProductos.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar los productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del producto es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MessageBox.Show("La categoría es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoria.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un valor numérico mayor que cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return false;
            }

            if (!int.TryParse(txtExistencias.Text, out int existencias) || existencias < 0)
            {
                MessageBox.Show("Las existencias deben ser un número entero igual o mayor que cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtExistencias.Focus();
                return false;
            }

            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    string query = "INSERT INTO dbo.Productos (Nombre, Categoria, Precio, Existencias) VALUES (@Nombre, @Categoria, @Precio, @Existencias)";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                        comando.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                        comando.Parameters.AddWithValue("@Precio", Convert.ToDecimal(txtPrecio.Text));
                        comando.Parameters.AddWithValue("@Existencias", Convert.ToInt32(txtExistencias.Text));

                        conexion.Open();
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                ConsultarProductos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto de la tabla para actualizar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarDatos()) return;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    string query = "UPDATE dbo.Productos SET Nombre = @Nombre, Categoria = @Categoria, Precio = @Precio, Existencias = @Existencias WHERE IdProducto = @IdProducto";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                        comando.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                        comando.Parameters.AddWithValue("@Precio", Convert.ToDecimal(txtPrecio.Text));
                        comando.Parameters.AddWithValue("@Existencias", Convert.ToInt32(txtExistencias.Text));
                        comando.Parameters.AddWithValue("@IdProducto", idProductoSeleccionado);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                        MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                ConsultarProductos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtExistencias.Clear();
            idProductoSeleccionado = 0;
            dgvProductos.ClearSelection();
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto de la tabla para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                    {
                        string query = "DELETE FROM dbo.Productos WHERE IdProducto = @IdProducto";
                        using (SqlCommand comando = new SqlCommand(query, conexion))
                        {
                            comando.Parameters.AddWithValue("@IdProducto", idProductoSeleccionado);

                            conexion.Open();
                            comando.ExecuteNonQuery();
                            MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    ConsultarProductos();
                    LimpiarCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvProductos_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
                idProductoSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value);
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                txtCategoria.Text = fila.Cells["Categoria"].Value.ToString();
                txtPrecio.Text = fila.Cells["Precio"].Value.ToString();
                txtExistencias.Text = fila.Cells["Existencias"].Value.ToString();
            }
        }
    }
}
