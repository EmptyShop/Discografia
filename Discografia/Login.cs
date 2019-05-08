using System;
using System.Data.Entity.Validation;
using System.Deployment.Application;
using System.Linq;
using System.Windows.Forms;

namespace Discografia
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Program.AppVersion = " (ver. " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() + ")";
            }
            else
            {
                Program.AppVersion = "";
            }
            this.Text = "Discografía" + Program.AppVersion + " Login";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != String.Empty && txtPassword.Text.Trim() != String.Empty)
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        string passwordEncriptado = Encriptacion.MD5Hash(txtPassword.Text);

                        //obtención del usuario ingresado
                        var usuarioValido = contexto.Usuarios.FirstOrDefault(s => s.User == txtUsuario.Text.Trim()
                            && s.Password == passwordEncriptado);

                        //el registro se encontró
                        if (usuarioValido != null)
                        {
                            //estatus 1: usuario activo
                            if (usuarioValido.Estatus == 1)
                            {
                                Program.AbrirAplicacion = true;

                                Cursor.Current = Cursors.Default;
                                this.Close();
                            }
                            //estatus 0: usuario suspendido
                            else
                            {
                                Cursor.Current = Cursors.Default;
                                MessageBox.Show("El usuario está suspendido.", "Validación");
                            }
                        }
                        else
                        {
                            Cursor.Current = Cursors.Default;
                            MessageBox.Show("Usuario o contraseña incorrectos.", "Validación");
                        }
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    string errorsBuffer = "";
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        errorsBuffer += error.Entry.Entity.GetType().Name + " in state " + error.Entry.State + " has validation errors:\n";
                        foreach (var ve in error.ValidationErrors)
                        {
                            errorsBuffer += "\tProperty: " + ve.PropertyName + ", Error: " + ve.ErrorMessage + "\n";
                        }
                    }
                    MessageBox.Show("Errores de EF:\n" + errorsBuffer, "Error");
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No es posible acceder a la aplicación. " + exception.Message, "Error");
                }
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Show("Proporciona usuario y contraseña.", "Validación");
            }
        }
    }
}
