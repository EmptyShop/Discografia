using System;
using System.Windows.Forms;

namespace Discografia
{
    static class Program
    {
        /// <summary>
        /// Control de Discografía.
        /// Catálogos de artistas y registro de canciones y albumes.
        /// </summary>
        /// 

        //bandera que determina si se abre la aplicación principal
        public static bool AbrirAplicacion { get; set; }
        public static string AppVersion { get; set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AbrirAplicacion = false;

            Application.Run(new Login());

            if (AbrirAplicacion)
            {
                Application.Run(new MainWindow());
            }
        }
    }
}
