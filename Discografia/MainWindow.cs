using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Discografia
{
    public partial class MainWindow : Form
    {
        //fuentes de datos comunes para llenar múltiples controles
        private ICollection<Artista> _listaArtistas;
        private ICollection<Cancion> _listaCanciones;
        private ICollection<Formato> _listaFormatos;

        private TextInfo TICaseFormat;

        private Rectangle dragBox;  //usado para drag & drop en el tracklist
        private int rowIndexTrackSeleccionado;  //usado para drag & drop en el tracklist

        //actualiza la fuente de datos de la lista de artistas
        public void ActualizaListBoxArtista()
        {
            lstArtista.DataSource = _listaArtistas;
            lstArtista.SelectedIndex = -1;
        }

        //actualiza la fuente de datos de la lista de artistas seleccionables en la sección de canciones
        public void ActualizaListBoxCancionArtistas()
        {
            int cancionId;  //id de la canción seleccionada

            //se ha seleccionado una canción
            if (dgrCancion.CurrentRow != null && dgrCancion.SelectedRows.Count > 0)
            {
                cancionId = Convert.ToInt32(dgrCancion.CurrentRow.Cells["CancionID"].Value);
            }
            else
            {
                cancionId = 0;
            }

            lstArtistas.Items.Clear();

            try
            {
                using (var contexto = new DiscografiaDB())
                {
                    //si se seleccionó una canción, la lista de artistas consiste en todos
                    //los artistas menos los artistas asignados a la canción seleccionada.
                    if (cancionId > 0)
                    {
                        Cancion cancion = contexto.Canciones.Find(cancionId);

                        var losDemasArtistas = contexto.Artistas.AsEnumerable().Except(cancion.Artistas)
                            .OrderBy(s => s.Nombre);
                        foreach (Artista artista in losDemasArtistas)
                        {
                            lstArtistas.Items.Add(artista);
                        }
                    }
                    else
                    {
                        //aquí la lista consiste en todos los artistas existentes
                        foreach (Artista artista in _listaArtistas)
                        {
                            lstArtistas.Items.Add(artista);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("No se pudo localizar la información de los artistas para la sección de canciones. " + exception.Message, "Error");
            }
        }

        //actualiza la fuente de datos de la lista de artistas seleccionables en la sección de álbumes
        public void ActualizaListBoxAlbumArtistas()
        {
            int albumId;

            //se ha seleccionado un álbum
            if (dgrAlbum.CurrentRow != null && dgrAlbum.SelectedRows.Count > 0)
            {
                albumId = Convert.ToInt32(dgrAlbum.CurrentRow.Cells["AlbumID"].Value);
            }
            else
            {
                albumId = 0;
            }

            lstArtistas2.Items.Clear();

            try
            {
                using (var contexto = new DiscografiaDB())
                {
                    //si se seleccionó un álbum la lista consiste de todos los artistas menos
                    //los asignados al álbum
                    if (albumId > 0)
                    {
                        Album album = contexto.Albums.Find(albumId);

                        var losDemasArtistas = contexto.Artistas.AsEnumerable().Except(album.Artistas)
                            .OrderBy(s => s.Nombre);
                        foreach (Artista artista in losDemasArtistas)
                        {
                            lstArtistas2.Items.Add(artista);
                        }
                    }
                    else
                    {
                        //aquí la lista consiste de todos los artistas existentes
                        foreach (Artista artista in _listaArtistas)
                        {
                            lstArtistas2.Items.Add(artista);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("No se pudo obtener la información de los artistas para la sección de álbumes. " + exception.Message, "Error");
            }
        }

        //actualiza la fuente de datos del grid de canciones seleccionables en la sección de álbumes
        public void ActualizaGridAlbumCanciones()
        {
            int albumId;

            //se seleccionó un álbum
            if (dgrAlbum.CurrentRow != null && dgrAlbum.SelectedRows.Count > 0)
            {
                albumId = Convert.ToInt32(dgrAlbum.CurrentRow.Cells["AlbumID"].Value);
            }
            else
            {
                albumId = 0;
            }

            dgvCanciones.Rows.Clear();

            try
            {
                using (var contexto = new DiscografiaDB())
                {
                    //si un álbum se seleccionó, la lista consiste de todas las canciones menos
                    //las asignadas al álbum
                    if (albumId > 0)
                    {
                        Album album = contexto.Albums.Find(albumId);
                        ICollection<Cancion> tracklist = new HashSet<Cancion>();

                        //llenamos el objeto tracklist con los registros de la propiedad de
                        //navegación Tracklist del album seleccionado
                        album.Tracklist.ToList().ForEach(s => tracklist.Add(contexto.Canciones.Find(s.Cancion_CancionID)));

                        //todas las canciones menos las del tracklist, ordenadas por: artista, canción
                        var lasDemasCanciones = contexto.Canciones.AsEnumerable().Except(tracklist)
                            .OrderBy(s => s.Artistas.FirstOrDefault().Nombre).ThenBy(s => s.Nombre);
                        
                        //llenamos el grid
                        //la colummna con índice 3 es una columna oculta auxiliar utilizada cuando
                        //se desasigna una canción y sirve para reordenar por: (artista, canción)
                        foreach (Cancion cancion in lasDemasCanciones)
                        {
                            String nombreArtista = cancion.Artistas.First().Nombre;
                            dgvCanciones.Rows.Add(cancion.CancionID, cancion.Artistas.First().Nombre, cancion.Nombre,
                                new StringBuilder(nombreArtista).Append("-").Append(cancion.Nombre));
                        }

                    }
                    else
                    {
                        //llenamos el grid con todas las canciones existentes
                        //la colummna con índice 3 es una columna oculta auxiliar utilizada cuando
                        //se desasigna una canción y sirve para reordenar por: (artista, canción)
                        foreach (Cancion cancion in _listaCanciones)
                        {
                            String nombreArtista = cancion.Artistas.First().Nombre;
                            dgvCanciones.Rows.Add(cancion.CancionID, cancion.Artistas.First().Nombre, cancion.Nombre,
                                new StringBuilder(nombreArtista).Append("-").Append(cancion.Nombre));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("No se pudo obtener la información de las canciones para la sección de álbumes. " + exception.Message, "Error");
            }
        }

        //actualiza la fuente de datos de la lista de formatos
        public void ActualizaListBoxFormato()
        {
            lstFormatos.DataSource = _listaFormatos.ToArray<Formato>();
            lstFormatos.SelectedIndex = -1;
        }

        //actualiza la lista de opciones de formatos en la sección de álbumes
        public void ActualizaComboAlbumFormatos()
        {
            cboFormato.DataSource = _listaFormatos;
            cboFormato.SelectedIndex = -1;
        }

        //vacía los campos de edición de la sección de canciones
        public void VaciaCamposCancion(bool? fijarAño = false, bool? fijarArtistas = false)
        {
            dgrCancion.ClearSelection();
            txtCancion.Text = String.Empty;
            txtDuracion.Text = String.Empty;
            txtAño.Text = fijarAño.HasValue && fijarAño == true ? txtAño.Text : String.Empty;
            if (!(fijarArtistas.HasValue && fijarArtistas == true))
            {
                lstArtistasCancion.Items.Clear();
                ActualizaListBoxCancionArtistas();
            }
        }

        //vacía los campos de edición de la sección de álbumes
        public void VaciaCamposAlbum()
        {
            dgrAlbum.ClearSelection();
            txtAlbum.Text = String.Empty;
            cboFormato.SelectedValue = -1;
            dtpGrabacion.Value = DateTime.Today;
            dtpAdquisicion.Value = DateTime.Today;
            lstArtistasAlbum.Items.Clear();
            ActualizaListBoxAlbumArtistas();
            dgvCancionesAlbum.Rows.Clear();
            ActualizaGridAlbumCanciones();
        }

        //actualiza la columna de posición en el grid de tracklist para tener siempre 
        //una secuencia numérica ordenada
        private void OrdenaTracklist()
        {
            foreach (DataGridViewRow row in dgvCancionesAlbum.Rows)
            {
                row.Cells["colPosicion"].Value = row.Index + 1;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Text = "Discografía" + Program.AppVersion;
        }

        //obtención de información inicial de la aplicación: catálogos
        private void MainWindow_Load(object sender, EventArgs e)
        {
            try
            {
                TICaseFormat = new CultureInfo("es-MX", false).TextInfo;

                using (var contexto = new DiscografiaDB())
                {
                    //llenado de catálogo de artistas
                    _listaArtistas = contexto.Artistas.OrderBy(s => s.Nombre).ToList<Artista>();
                    ActualizaListBoxArtista();

                    //llenado de catálogo de canciones
                    _listaCanciones = contexto.Canciones.OrderBy(s => s.Artistas.FirstOrDefault().Nombre).ThenBy(s => s.Nombre).ToList<Cancion>();
                    dgrCancion.DataSource = Cancion.CatalogoCanciones();

                    //llenado de listas de artistas para las secciones de canciones y álbumes
                    var losDemasArtistas = contexto.Artistas.OrderBy(s => s.Nombre);

                    foreach (Artista artista in losDemasArtistas)
                    {
                        lstArtistas.Items.Add(artista);
                        lstArtistas2.Items.Add(artista);
                    }

                    //llenado de catálogo de álbumes
                    dgrAlbum.DataSource = Album.CatalogoAlbumes();

                    foreach (Cancion cancion in _listaCanciones)
                    {
                        dgvCanciones.Rows.Add(cancion.CancionID, cancion.Artistas.First().Nombre, cancion.Nombre);
                    }
                    dgvCanciones.ClearSelection();

                    //llenado de catálogo de formatos
                    _listaFormatos = contexto.Formatos.OrderBy(s => s.Descripcion).ToList<Formato>();
                    ActualizaListBoxFormato();
                    ActualizaComboAlbumFormatos();

                    //llenado del catálogo de usuarios
                    lstUsuarios.DataSource = contexto.Usuarios.OrderBy(s => s.User).ToList<Usuario>();
                    lstUsuarios.SelectedIndex = -1;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("No se pudieron cargar todos los datos de la discografía. " + exception.Message, "Error");
            }

            tabMenu.SelectTab("tabArtista");
        }

        //formato seleccionado en la sección de formatos
        private void lstFormatos_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (lstFormatos.SelectedIndex >= 0)
            {
                txtFormato.Text = lstFormatos.Text;
            }
            else
            {
                txtFormato.Text = String.Empty;
            }
        }

        //agregar nuevo formato a la BD
        private void btnAgregarFormato_Click(object sender, EventArgs e)
        {
            if (txtFormato.Text.Trim() != "" && txtFormato.Text.Trim().Length <= 16)
            {
                //verificamos que el formato no sea repetido
                if (!_listaFormatos.Any(s => s.Descripcion.ToLower() == txtFormato.Text.Trim().ToLower()))
                {
                    Cursor.Current = Cursors.WaitCursor;

                    try
                    {
                        using (var contexto = new DiscografiaDB())
                        {
                            var nuevoFormato = new Formato();

                            nuevoFormato.Descripcion = TICaseFormat.ToTitleCase(txtFormato.Text.Trim());
                            contexto.Formatos.Add(nuevoFormato);
                            contexto.SaveChanges();

                            //actualización de la fuente de datos de las listas de formatos
                            _listaFormatos = contexto.Formatos.OrderBy(s => s.Descripcion).ToList<Formato>();
                            ActualizaListBoxFormato();
                            ActualizaComboAlbumFormatos();
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("No se registró el formato. " + exception.Message, "Error");
                    }
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    MessageBox.Show("El formato ya existe en la base de datos.", "Validación");
                }
            }
            else
            {
                MessageBox.Show("Proporciona un formato válido.", "Nuevo Formato");
            }
        }

        //modificar un formato en la BD
        private void btnModificarFormato_Click(object sender, EventArgs e)
        {
            if (lstFormatos.SelectedIndex >= 0)
            {
                if (txtFormato.Text.Trim() != "" && txtFormato.Text.Trim().Length <= 16)
                {
                    //verificamos que el formato no sea repetido
                    if (!_listaFormatos.Any(s => s.Descripcion.ToLower() == txtFormato.Text.Trim().ToLower()
                        && s.FormatoID != ((Formato)lstFormatos.SelectedItem).FormatoID))
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        try
                        {
                            using (var contexto = new DiscografiaDB())
                            {
                                Formato formato = (Formato)lstFormatos.SelectedItem;
                                formato.Descripcion = txtFormato.Text.Trim();
                                contexto.Entry(formato).State = System.Data.Entity.EntityState.Modified;
                                contexto.SaveChanges();

                                //actualización de la fuente de datos de las listas de formatos
                                _listaFormatos = contexto.Formatos.OrderBy(s => s.Descripcion).ToList<Formato>();
                                ActualizaListBoxFormato();
                                ActualizaComboAlbumFormatos();
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("No se actualizó el formato. " + exception.Message, " Error");
                        }
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        MessageBox.Show("Ya existe otro formato con la misma descripción.", "Validación");
                    }
                }
                else
                {
                    MessageBox.Show("Proporciona un nombre de formato válido.", "Modificación de Formato");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un formato para modificar.", "Modificación de Formato");
            }
        }

        //eliminar un formato de la BD
        private void btnEliminarFormato_Click(object sender, EventArgs e)
        {
            if (lstFormatos.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Confirma para eliminar este formato.", "Eliminar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                        using (var contexto = new DiscografiaDB())
                        {
                            //verificamos que el formato no esté asociado a ningún álbum
                            int formatoId = Convert.ToInt32(lstFormatos.SelectedValue);
                            var album = contexto.Albums.Where(s => s.Formato.FormatoID == formatoId)
                                .FirstOrDefault<Album>();

                            if (album != null)
                            {
                                MessageBox.Show("El formato tiene albums asociados. No se eliminará.", "Eliminación de Formato");
                            }
                            else
                            {
                                Formato formato = (Formato)lstFormatos.SelectedItem;
                                contexto.Entry(formato).State = System.Data.Entity.EntityState.Deleted;
                                contexto.SaveChanges();

                                //actualización de la fuente de datos de las listas de formatos 
                                _listaFormatos = contexto.Formatos.OrderBy(s => s.Descripcion).ToList<Formato>();
                                ActualizaListBoxFormato();
                                ActualizaComboAlbumFormatos();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("No se pudo eliminar el formato. " + exception.Message, "Error");
                    }
                    Cursor.Current = Cursors.Default;
                }
            }
            else
            {
                MessageBox.Show("Selecciona un formato para eliminar.", "Eliminación de Formato");
            }
        }

        //artista seleccionado en la sección de aertistas
        private void lstArtista_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstArtista.SelectedIndex >= 0)
            {
                txtArtista.Text = lstArtista.Text;
                txtPais.Text = ((Artista)lstArtista.SelectedItem).Pais;
            }
            else
            {
                txtArtista.Text = String.Empty;
                txtPais.Text = String.Empty;
            }
        }

        //agregar nuevo artista a la BD
        private void btnAgregarArtista_Click(object sender, EventArgs e)
        {
            if (txtArtista.Text.Trim() != String.Empty)
            {
                //verificamos que el artista no sea repetido
                if (!_listaArtistas.Any(s => s.Nombre.ToLower() == txtArtista.Text.Trim().ToLower()))
                {
                    Cursor.Current = Cursors.WaitCursor;

                    try
                    {
                        using (var contexto = new DiscografiaDB())
                        {
                            var nuevoArtista = new Artista();

                            nuevoArtista.Nombre = TICaseFormat.ToTitleCase(txtArtista.Text.Trim());
                            nuevoArtista.Pais = TICaseFormat.ToTitleCase(txtPais.Text.Trim());
                            contexto.Artistas.Add(nuevoArtista);
                            contexto.SaveChanges();

                            //actualización de la fuente de datos de las listas de artistas
                            _listaArtistas = contexto.Artistas.OrderBy(s => s.Nombre).ToList<Artista>();
                            ActualizaListBoxArtista();
                            ActualizaListBoxCancionArtistas();
                            ActualizaListBoxAlbumArtistas();
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("No se registró el artista. " + exception.Message);
                    }
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    MessageBox.Show("Ya existe otro artista con el mismo nombre en la base de datos.", "Validación");
                }
            }
            else
            {
                MessageBox.Show("Proporciona un nombre de artista válido.", "Nuevo Artista");
            }
        }

        //modificar un artista en la BD
        private void btnModificarArtista_Click(object sender, EventArgs e)
        {
            if (lstArtista.SelectedIndex >= 0)
            {
                if (txtArtista.Text.Trim() != String.Empty)
                {
                    //verificamos que el artista no sea repetido
                    if (!_listaArtistas.Any(s => s.Nombre.ToLower() == txtArtista.Text.Trim().ToLower()
                        && s.ArtistaID != ((Artista)lstArtista.SelectedItem).ArtistaID))
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        try
                        {
                            using (var contexto = new DiscografiaDB())
                            {
                                Artista artista = (Artista)lstArtista.SelectedItem;
                                artista.Nombre = txtArtista.Text.Trim();
                                artista.Pais = TICaseFormat.ToTitleCase(txtPais.Text.Trim());
                                contexto.Entry(artista).State = System.Data.Entity.EntityState.Modified;
                                contexto.SaveChanges();

                                //actualización de la fuente de datos de las listas de artistas
                                _listaArtistas = contexto.Artistas.OrderBy(s => s.Nombre).ToList<Artista>();
                                ActualizaListBoxArtista();
                                ActualizaListBoxCancionArtistas();
                                ActualizaListBoxAlbumArtistas();
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("No se pudo modificar el artista. " + exception.Message, "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ya existe otro artista con el mismo nombre en la base de datos.", "Validación");
                    }
                }
                else
                {
                    MessageBox.Show("Proporciona un nombre de artista válido.", "Modificación de Artista");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un artista para modificar.", "Modificación de Artista");
            }
        }

        //eliminar un artista de la BD
        private void btnEliminarArtista_Click(object sender, EventArgs e)
        {
            if (lstArtista.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Confirma para eliminar este artista.", "Eliminación de Artista",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    try
                    {
                        //obtenemos el id del artista para buscarlo en canciones y álbumes
                        int id = Convert.ToInt32(lstArtista.SelectedValue);

                        using (var contexto = new DiscografiaDB())
                        {
                            //buscamos si el artista está asociado a algún álbum
                            bool enAlbums = contexto.Albums.Any(s => s.Artistas.Any(a => a.ArtistaID == id));
                            if (enAlbums)
                            {
                                MessageBox.Show("El artista seleccionado tiene albums asociados. No se eliminará.", "Eliminación de Artista");
                            }
                            else
                            {
                                bool enCanciones = contexto.Canciones.Any(s => s.Artistas.Any(a => a.ArtistaID == id));
                                if (enCanciones)
                                {
                                    MessageBox.Show("El artista seleccionado tiene canciones asociadas. No se eliminará.", "Eliminación de Artista");
                                }
                                else
                                {
                                    Artista artista = (Artista)lstArtista.SelectedItem;
                                    contexto.Entry(artista).State = System.Data.Entity.EntityState.Deleted;
                                    contexto.SaveChanges();

                                    //actualización de la fuente de datos de las listas de artistas
                                    _listaArtistas = contexto.Artistas.OrderBy(s => s.Nombre).ToList<Artista>();
                                    ActualizaListBoxArtista();
                                    ActualizaListBoxCancionArtistas();
                                    ActualizaListBoxAlbumArtistas();
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("No se pudo eliminar el artista. " + exception.Message, "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona un artista para eliminar.", "Eliminación de Artista");
            }
        }

        //canción seleccionada en la sección de canciones
        private void dgrCancion_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //se seleccionó un renglón del grid de canciones
            if (e.RowIndex >= 0)
            {
                int cancionId = Convert.ToInt32(dgrCancion.CurrentRow.Cells["CancionID"].Value);
                txtCancion.Text = dgrCancion.CurrentRow.Cells["colCancion"].Value.ToString();
                lstArtistasCancion.Items.Clear();

                //obtenemos los datos complementarios de la canción (duración, año y lista de artistas)
                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        Cancion cancion = contexto.Canciones.Find(cancionId);

                        txtDuracion.Text = cancion.Duracion != null ? 
                            cancion.Duracion.Value.Hours != 0 ? cancion.Duracion.Value.ToString(@"hh\:mm\:ss") : cancion.Duracion.Value.ToString(@"mm\:ss") 
                            : String.Empty;

                        txtAño.Text = cancion.Año != null ? cancion.Año.ToString() : String.Empty;

                        var artistasCancion = cancion.Artistas.OrderBy(s => s.Nombre);
                        foreach (Artista artista in artistasCancion)
                        {
                            lstArtistasCancion.Items.Add(artista);
                        }
                        ActualizaListBoxCancionArtistas();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No se pudo obtener la información completa de la canción. " + exception.Message, "Error");
                }
            }
        }

        //si el usuario hizo click en una zona del grid donde no hay renglones, vaciamos los campos de edición
        private void dgrCancion_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo h = dgrCancion.HitTest(e.X, e.Y);
            if (h == DataGridView.HitTestInfo.Nowhere || h.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                VaciaCamposCancion();
            }
        }

        /*al hacer el binding el grid va seleccionando renglón por renglón. Para evitar que quede
          seleccionado el último renglón desseleccionamos cuando termina el binding*/
        private void dgrCancion_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgrCancion.ClearSelection();
        }

        //asignar un artista a una canción
        private void btnAsignarArtistaCancion_Click(object sender, EventArgs e)
        {
            if (lstArtistas.SelectedIndex >= 0)
            {
                lstArtistasCancion.Items.Add(lstArtistas.SelectedItem);
                lstArtistas.Items.Remove(lstArtistas.SelectedItem);
            }
        }

        //desasignar un artista a una canción
        private void btnDesasignarArtistaCancion_Click(object sender, EventArgs e)
        {
            if (lstArtistasCancion.SelectedIndex >= 0)
            {
                lstArtistas.Items.Add(lstArtistasCancion.SelectedItem);
                lstArtistasCancion.Items.Remove(lstArtistasCancion.SelectedItem);
            }
        }

        //agregar una canción a la BD
        private void btnAgregarCancion_Click(object sender, EventArgs e)
        {
            if (txtCancion.Text.Trim() != String.Empty && lstArtistasCancion.Items.Count > 0)
            {
                if (txtCancion.Text.Trim().Length <= txtCancion.MaxLength)
                {
                    //verificamos que la canción no sea repetida
                    if (!_listaCanciones.Any(s => s.Nombre.ToLower() == txtCancion.Text.Trim().ToLower()
                        && s.Artistas.Count == lstArtistasCancion.Items.Count
                        && !s.Artistas.Except(lstArtistasCancion.Items.Cast<Artista>().ToList(), new Model.ComparadorArtista())
                        .Any()))
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        try
                        {
                            using (var contexto = new DiscografiaDB())
                            {
                                Cancion nuevaCancion = new Cancion();
                                nuevaCancion.Nombre = TICaseFormat.ToTitleCase(txtCancion.Text.Trim());
                                if (txtDuracion.Text != String.Empty)
                                {
                                    if (txtDuracion.Text.Length <= 5)
                                    {
                                        nuevaCancion.Duracion = TimeSpan.Parse(new StringBuilder("00:") + txtDuracion.Text);
                                    }
                                    else
                                    {
                                        nuevaCancion.Duracion = TimeSpan.Parse(txtDuracion.Text);
                                    }
                                }
                                if (txtAño.Text != String.Empty)
                                {
                                    nuevaCancion.Año = Convert.ToInt16(txtAño.Text);
                                }
                                foreach (Artista elemento in lstArtistasCancion.Items)
                                {
                                    nuevaCancion.Artistas.Add(elemento);
                                    contexto.Entry(elemento).State = System.Data.Entity.EntityState.Unchanged;
                                }
                                contexto.Canciones.Add(nuevaCancion);
                                contexto.SaveChanges();

                                //actualización de la fuente de datos de las listas de canciones
                                _listaCanciones = contexto.Canciones.OrderBy(s => s.Artistas.FirstOrDefault().Nombre).ThenBy(s => s.Nombre).ToList<Cancion>();
                                dgrCancion.DataSource = Cancion.CatalogoCanciones();
                                dgrCancion.ClearSelection();
                                VaciaCamposCancion(chxFijarAñoCancion.Checked, chxFijarArtistasCancion.Checked);
                                ActualizaGridAlbumCanciones();
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("No se pudo agregar la canción. " + exception.Message, "Error");
                        }

                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        MessageBox.Show("La canción ya está registrada en la base de datos.", "Validación");
                    }
                }
            }
            else
            {
                MessageBox.Show("Proporciona el nombre de la canción y su artista.", "Validación");
            }
        }

        //validación de formato de año
        private void txtAño_Leave(object sender, EventArgs e)
        {
            txtAño.Text.Trim();
            string s = txtAño.Text;
            short r;

            if (s != String.Empty && !Int16.TryParse(s, out r))
            {
                MessageBox.Show("El año de la canción no es válido.", "Validación");
                txtAño.Text = String.Empty;
            }
        }

        //validación de formato de duración
        private void txtDuracion_Leave(object sender, EventArgs e)
        {
            txtDuracion.Text.Trim();
            string s = txtDuracion.Text;
            TimeSpan r;
            Regex regexp = new Regex(@"^(\d{1,2}:)?[0-5]?\d:[0-5]\d$");

            if (s != String.Empty && !(TimeSpan.TryParse(s, out r) && regexp.IsMatch(s)))
            {
                MessageBox.Show("el formato de la duración es incorrecto. Utiliza hh:MM:SS", "Validación");
                txtDuracion.Text = String.Empty;
            }
        }

        //modificar una canción en la BD
        private void btnModificarCancion_Click(object sender, EventArgs e)
        {
            if (dgrCancion.SelectedRows.Count > 0)
            {
                int cancionId = Convert.ToInt32(dgrCancion.CurrentRow.Cells["CancionID"].Value);
                if (txtCancion.Text.Trim() != String.Empty && lstArtistasCancion.Items.Count > 0)
                {
                    if (txtCancion.Text.Trim().Length <= txtCancion.MaxLength)
                    {
                        //verificamos que la canción no sea repetida
                        if (!_listaCanciones.Any(s => s.Nombre.ToLower() == txtCancion.Text.Trim().ToLower()
                            && s.Artistas.Count == lstArtistasCancion.Items.Count
                            && !s.Artistas.Except(lstArtistasCancion.Items.Cast<Artista>().ToList(), new Model.ComparadorArtista())
                            .Any()
                            && s.CancionID != cancionId))
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            try
                            {
                                using (var contexto = new DiscografiaDB())
                                {
                                    Cancion cancion = contexto.Canciones.Find(cancionId);
                                    cancion.Nombre = txtCancion.Text.Trim();
                                    if (txtDuracion.Text != String.Empty)
                                    {
                                        if (txtDuracion.Text.Length <= 5)
                                        {
                                            cancion.Duracion = TimeSpan.Parse(new StringBuilder("00:") + txtDuracion.Text);
                                        }
                                        else
                                        {
                                            cancion.Duracion = TimeSpan.Parse(txtDuracion.Text);
                                        }
                                    }
                                    else
                                    {
                                        cancion.Duracion = null;
                                    }
                                    if (txtAño.Text != String.Empty)
                                    {
                                        cancion.Año = Convert.ToInt16(txtAño.Text);
                                    }
                                    else
                                    {
                                        cancion.Año = null;
                                    }

                                    //obtenemos la lista de artistas a eliminar (los artistas desasignados por el usuario)
                                    var artistasAEliminar = cancion.Artistas.AsEnumerable()
                                        .Except(lstArtistasCancion.Items.Cast<Artista>().ToList(), new Model.ComparadorArtista());

                                    artistasAEliminar.ToList().ForEach(s => cancion.Artistas.Remove(s));

                                    //obtenemos la lista de artistas a agregar (los artistas asignados por el usuario)
                                    var artistasAgregados = lstArtistasCancion.Items.Cast<Artista>().ToList()
                                        .Except(cancion.Artistas.AsEnumerable(), new Model.ComparadorArtista());

                                    artistasAgregados.ToList().ForEach(s =>
                                    {
                                        Artista artista = contexto.Artistas.Find(s.ArtistaID);
                                        cancion.Artistas.Add(artista);
                                        contexto.Entry(artista).State = System.Data.Entity.EntityState.Unchanged;
                                    });

                                    contexto.SaveChanges();

                                    //actualización de la fuente de datos de las listas de canciones
                                    _listaCanciones = contexto.Canciones.OrderBy(s => s.Artistas.FirstOrDefault().Nombre).ThenBy(s => s.Nombre).ToList<Cancion>();
                                    dgrCancion.DataSource = Cancion.CatalogoCanciones();
                                    dgrCancion.ClearSelection();
                                    VaciaCamposCancion();
                                    ActualizaGridAlbumCanciones();
                                }
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show("No se pudo modificar la canción. " + exception.Message, "Error");
                            }
                            Cursor.Current = Cursors.Default;
                        }
                        else
                        {
                            MessageBox.Show("La canción ya está registrada en la base de datos.", "Validación");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona una canción.", "Validación");
            }
        }

        //eliminar una canción de la BD
        private void btnEliminarCancion_Click(object sender, EventArgs e)
        {
            if (dgrCancion.SelectedRows.Count > 0)
            {
                int cancionID = Convert.ToInt32(dgrCancion.CurrentRow.Cells["CancionID"].Value);
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        Cancion cancion = contexto.Canciones.Find(cancionID);

                        contexto.Entry(cancion).State = System.Data.Entity.EntityState.Deleted;
                        contexto.SaveChanges();

                        //actualización de la fuente de datos de las listas de canciones
                        _listaCanciones = contexto.Canciones.OrderBy(s => s.Nombre).ToList<Cancion>();
                        dgrCancion.DataSource = Cancion.CatalogoCanciones();
                        dgrCancion.ClearSelection();
                        VaciaCamposCancion();
                        ActualizaGridAlbumCanciones();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No se pudo eliminar la canción. " + exception.Message, "Error");
                }

                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Show("Selecciona una canción para eliminar.", "Validación");
            }
        }

        //álbum seleccionado
        private void dgrAlbum_SelectionChanged(object sender, EventArgs e)
        {
            /*se verifica la propiedad CanFocus debido a que al llenar el grid de álbumes,
              en el método de inicialización de la aplicación, el binding dispara el evento SelectionChanged.
              CanFocus es verdadero sólo cuando el usuario realiza el cambio de selección.*/
            if (dgrAlbum.CurrentRow != null && dgrAlbum.SelectedRows.Count == 1
                && dgrAlbum.CanFocus && Convert.ToInt32(dgrAlbum.CurrentRow.Cells["AlbumID"].Value) >= 0)
            {
                int albumId = Convert.ToInt32(dgrAlbum.CurrentRow.Cells["AlbumID"].Value);
                txtAlbum.Text = dgrAlbum.CurrentRow.Cells["colAlbum"].Value.ToString();
                lstArtistasAlbum.Items.Clear();
                dgvCancionesAlbum.Rows.Clear();

                //obtenemos los datos complementarios del álbum
                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        Album album = contexto.Albums.Find(albumId);
                        cboFormato.SelectedItem = album.Formato;
                        dtpGrabacion.Value = album.FechaGrabacion.Value;
                        dtpAdquisicion.Value = album.FechaAdquisicion.Value;

                        //llenado de la lista de artistas del album
                        var artistasAlbum = album.Artistas.OrderBy(s => s.Nombre);
                        foreach (Artista artista in artistasAlbum)
                        {
                            lstArtistasAlbum.Items.Add(artista);
                        }
                        ActualizaListBoxAlbumArtistas();

                        //llenado de la lista de canciones del album
                        var tracklist = album.Tracklist.OrderBy(s => s.Posicion);
                        foreach (CancionAlbums track in tracklist)
                        {
                            dgvCancionesAlbum.Rows.Add(track.Cancion_CancionID, track.Posicion, track.Cancion.Artistas.First().Nombre, track.Cancion.Nombre);
                        }
                        ActualizaGridAlbumCanciones();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No se pudo obtener la información completa del album. " + exception.Message, "Error");
                }
            }
        }

        //si el usuario hizo click en una zona del grid donde no hay renglones, vaciamos los campos de edición
        private void dgrAlbum_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo h = dgrAlbum.HitTest(e.X, e.Y);
            if (h == DataGridView.HitTestInfo.Nowhere || h.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                VaciaCamposAlbum();
            }
        }

        /*al hacer el binding el grid va seleccionando renglón por renglón. Para evitar que quede
          seleccionado el último renglón desseleccionamos cuando termina el binding*/
        private void dgrAlbum_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgrAlbum.ClearSelection();
        }

        //si el usuario hizo click en una zona del grid donde no hay renglones, cancelamos la selección actual
        private void dgvCanciones_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo h = dgvCanciones.HitTest(e.X, e.Y);
            if (h == DataGridView.HitTestInfo.Nowhere)
            {
                dgvCanciones.ClearSelection();
            }
        }

        //si el usuario hizo click en una zona del grid donde no hay renglones, cancelamos la selección actual
        private void dgvCancionesAlbum_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo h = dgvCancionesAlbum.HitTest(e.X, e.Y);
            if (h == DataGridView.HitTestInfo.Nowhere)
            {
                dgvCancionesAlbum.ClearSelection();
            }
            else
            {
                if (dgvCancionesAlbum.SelectedRows.Count > 0)
                {
                    rowIndexTrackSeleccionado = dgvCancionesAlbum.SelectedRows[0].Index;
                }
            }
        }

        //capturamos los datos de posición para drag & drop
        private void dgvCancionesAlbum_MouseDown(object sender, MouseEventArgs e)
        {
            if (dgvCancionesAlbum.SelectedRows.Count > 0)
            {
                Size dragSize = SystemInformation.DragSize;
                dragBox = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                
                //conservamos el index del track que se va a arrastrar si se determina que habrá drag & drop 
                rowIndexTrackSeleccionado = dgvCancionesAlbum.SelectedRows[0].Index;
            }
            else
            {
                dragBox = Rectangle.Empty;
            }
        }

        //determinamos si se trata de un drag
        private void dgvCancionesAlbum_MouseMove(object sender, MouseEventArgs e)
        {
            //la validación de la tecla CTRL se introduce para permitir el resize de las columnas
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && Form.ModifierKeys != Keys.Control)
            {
                //valida si se mueve el puntero lo suficiente para iniciar el drag
                if (dragBox != Rectangle.Empty && !dragBox.Contains(e.X, e.Y))
                {
                    dgvCancionesAlbum.DoDragDrop(dgvCancionesAlbum.Rows[rowIndexTrackSeleccionado], DragDropEffects.Move);
                }
            }
        }

        //asigna el efecto move durante el drag
        private void dgvCancionesAlbum_DragEnter(object sender, DragEventArgs e)
        {
            if (dgvCancionesAlbum.SelectedRows.Count > 0)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        //asigna el efecto move durante el drag
        private void dgvCancionesAlbum_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        //inserta el renglón arrastrado a la posición del drop
        private void dgvCancionesAlbum_DragDrop(object sender, DragEventArgs e)
        {
            //el punto ayuda a determinar posteriormente cuál es el renglón donde se hace el drop
            Point clientPoint = dgvCancionesAlbum.PointToClient(new Point(e.X, e.Y));

            rowIndexTrackSeleccionado = dgvCancionesAlbum.SelectedRows[0].Index;

            //obtenemos el índice del renglón donde se hace el drop
            int rowIndexNuevaPosicion = dgvCancionesAlbum.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            //se arrastró pasado el último renglón
            if (rowIndexNuevaPosicion == -1)
            {
                rowIndexNuevaPosicion = dgvCancionesAlbum.Rows.Count - 1;
            }

            //movemos el renglón arrastrado a la nueva posición
            if (e.Effect == DragDropEffects.Move && rowIndexTrackSeleccionado != rowIndexNuevaPosicion)
            {
                try
                {
                    //DataGridViewRow track = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
                    DataGridViewRow track = dgvCancionesAlbum.Rows[rowIndexTrackSeleccionado];
                    //dgvCancionesAlbum.Rows.RemoveAt(rowIndexTrackSeleccionado);
                    dgvCancionesAlbum.Rows.Remove(track);
                    dgvCancionesAlbum.Rows.Insert(rowIndexNuevaPosicion, track);

                    //seleccionamos el renglón arrastrado
                    dgvCancionesAlbum.CurrentCell = dgvCancionesAlbum.Rows[rowIndexNuevaPosicion].Cells[1];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al reordenar: " + ex.Message);
                }
                OrdenaTracklist();
            }
        }

        //asignar un artista al álbum
        private void btnAsignarArtistaAlbum_Click(object sender, EventArgs e)
        {
            if (lstArtistas2.SelectedIndex >= 0)
            {
                lstArtistasAlbum.Items.Add(lstArtistas2.SelectedItem);
                lstArtistas2.Items.Remove(lstArtistas2.SelectedItem);
            }
        }

        //desasignar un artista del álbum
        private void btnDesasignarArtistaAlbum_Click(object sender, EventArgs e)
        {
            if (lstArtistasAlbum.SelectedIndex >= 0)
            {
                lstArtistas2.Items.Add(lstArtistasAlbum.SelectedItem);
                lstArtistasAlbum.Items.Remove(lstArtistasAlbum.SelectedItem);
            }
        }

        //asignar una canción al álbum (tracklist)
        private void btnAsignarCancion_Click(object sender, EventArgs e)
        {
            if (dgvCanciones.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvCanciones.CurrentRow;
                dgvCanciones.Rows.Remove(dgvCanciones.CurrentRow);
                dgvCancionesAlbum.Rows.Add(row.Cells[0].Value, 0, row.Cells[1].Value, row.Cells[2].Value);
                OrdenaTracklist();
            }
        }

        //desasignar una canción del álbum (tracklist)
        private void btnDesasignarCancion_Click(object sender, EventArgs e)
        {
            if (dgvCancionesAlbum.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvCancionesAlbum.CurrentRow;
                dgvCancionesAlbum.Rows.Remove(dgvCancionesAlbum.CurrentRow);

                //la colummna con índice 3 de dgvCanciones (colOrdenCanciones) es una columna oculta auxiliar 
                //para reordenar por: (artista, canción)
                dgvCanciones.Rows.Add(row.Cells[0].Value, row.Cells[2].Value, row.Cells[3].Value,
                    new StringBuilder(row.Cells[2].Value.ToString()).Append("-").Append(row.Cells[3].Value));

                dgvCanciones.Sort(colOrdenCanciones, System.ComponentModel.ListSortDirection.Ascending);
                OrdenaTracklist();
            }
        }

        //agregar un álbum a la BD
        private void btnAgregarAlbum_Click(object sender, EventArgs e)
        {
            if (txtAlbum.Text.Trim() != String.Empty && cboFormato.SelectedIndex >= 0
                && lstArtistasAlbum.Items.Count > 0)
            {
                if (dtpGrabacion.Value == DateTime.Today || dtpAdquisicion.Value == DateTime.Today
                    || dtpGrabacion.Value.CompareTo(dtpAdquisicion.Value) <= 0)
                {
                    try
                    {
                        using (var contexto = new DiscografiaDB())
                        {
                            int idFormato = Convert.ToInt16(cboFormato.SelectedValue);

                            //verificamos que el álbum no sea repetido
                            if (!contexto.Albums.AsEnumerable().Any(s =>
                                s.Nombre.ToLower() == txtAlbum.Text.Trim().ToLower()
                                    && s.Artistas.Count == lstArtistasAlbum.Items.Count
                                    && !s.Artistas.Except(lstArtistasAlbum.Items.Cast<Artista>().ToList(),
                                        new Model.ComparadorArtista())
                                        .Any()
                                    && s.Formato.FormatoID == idFormato))
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                Album nuevoAlbum = new Album();
                                nuevoAlbum.Nombre = TICaseFormat.ToTitleCase(txtAlbum.Text.Trim());
                                nuevoAlbum.Formato = contexto.Formatos.Find(cboFormato.SelectedValue);

                                if (dtpGrabacion.Value != DateTime.Today)
                                {
                                    nuevoAlbum.FechaGrabacion = dtpGrabacion.Value;
                                }
                                if (dtpAdquisicion.Value != DateTime.Today)
                                {
                                    nuevoAlbum.FechaAdquisicion = dtpAdquisicion.Value;
                                }

                                //Asignación de la lista de artistas
                                foreach (Artista elemento in lstArtistasAlbum.Items)
                                {
                                    nuevoAlbum.Artistas.Add(elemento);
                                    contexto.Entry(elemento).State = System.Data.Entity.EntityState.Unchanged;
                                }

                                contexto.Albums.Add(nuevoAlbum);
                                contexto.SaveChanges();

                                //Asignación de tracklist (se necesita el id del álbum creado, por eso este
                                //proceso se realiza después de crear el álbum)
                                foreach (DataGridViewRow row in dgvCancionesAlbum.Rows)
                                {
                                    CancionAlbums track = new CancionAlbums();
                                    track.Cancion = contexto.Canciones.Find(row.Cells["CancionID_AlbumCancion"].Value);
                                    track.Cancion_CancionID = track.Cancion.CancionID;
                                    track.Album = nuevoAlbum;
                                    track.Album_AlbumID = track.Album.AlbumID;
                                    track.Posicion = Convert.ToInt16(row.Cells["colPosicion"].Value);
                                    nuevoAlbum.Tracklist.Add(track);
                                }
                                contexto.SaveChanges();

                                //actualización de la fuente de datos de la lista de álbumes
                                dgrAlbum.DataSource = Album.CatalogoAlbumes();
                                dgrAlbum.ClearSelection();
                                VaciaCamposAlbum();

                                Cursor.Current = Cursors.Default;
                            }
                            else
                            {
                                MessageBox.Show("El álbum ya está registrado en la base de datos.", "Validación");
                            }
                        }
                    }
                    catch (DbEntityValidationException exception)
                    {
                        string mensaje = String.Empty;
                        var EVErrors = exception.EntityValidationErrors.ToList();
                        EVErrors.ForEach(err =>
                        {
                            var VErrors = err.ValidationErrors.ToList();
                            VErrors.ForEach(err2 => mensaje += err2.PropertyName + new StringBuilder(" -> ") + err2.ErrorMessage + "\n");
                        });

                        MessageBox.Show("No se modificó el album. No se cumplieron las validaciones:\n"
                            + mensaje, "Error");
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("No se pudo agregar el album. " + exception.Message, "Error");
                    }
                }
                else
                {
                    MessageBox.Show("La fecha de grabación no puede ser posterior a la de adquisición.", "Validación");
                }
            }
            else
            {
                MessageBox.Show("Proporciona el nombre del album, formato y artista.", "Validación");
            }
        }

        //modificar un álbum en la BD
        private void btnModificarAlbum_Click(object sender, EventArgs e)
        {
            if (dgrAlbum.SelectedRows.Count > 0)
            {
                int albumId = Convert.ToInt32(dgrAlbum.CurrentRow.Cells["AlbumID"].Value);
                if (txtAlbum.Text.Trim() != String.Empty && cboFormato.SelectedIndex >= 0
                    && lstArtistasAlbum.Items.Count > 0)
                {
                    if (dtpGrabacion.Value == DateTime.Today || dtpAdquisicion.Value == DateTime.Today
                        || dtpGrabacion.Value.CompareTo(dtpAdquisicion.Value) <= 0)
                    {
                        try
                        {
                            using (var contexto = new DiscografiaDB())
                            {
                                int idFormato = Convert.ToInt16(cboFormato.SelectedValue);

                                //verificamos que el álbum no sea repetido
                                if (!contexto.Albums.AsEnumerable().Any(s =>
                                    s.Nombre.ToLower() == txtAlbum.Text.Trim().ToLower()
                                    && s.Artistas.Count == lstArtistasAlbum.Items.Count
                                    && !s.Artistas.Except(lstArtistasAlbum.Items.Cast<Artista>().ToList(),
                                        new Model.ComparadorArtista())
                                        .Any()
                                    && s.Formato.FormatoID == idFormato
                                    && s.AlbumID != albumId))
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    Album album = contexto.Albums.Find(albumId);
                                    contexto.Entry(album).State = System.Data.Entity.EntityState.Modified;
                                    album.Nombre = txtAlbum.Text.Trim();
                                    album.Formato = contexto.Formatos.Find(cboFormato.SelectedValue);
                                    album.FechaGrabacion = dtpGrabacion.Value != DateTime.Today ? dtpGrabacion.Value : (DateTime?)null;
                                    album.FechaAdquisicion = dtpAdquisicion.Value != DateTime.Today ? dtpAdquisicion.Value : (DateTime?)null;

                                    //obtenemos la lista de artistas a eliminar (los artistas desasignados por el uuario)
                                    var artistasAEliminar = album.Artistas.AsEnumerable()
                                        .Except(lstArtistasAlbum.Items.Cast<Artista>().ToList(), new Model.ComparadorArtista());

                                    artistasAEliminar.ToList().ForEach(s => album.Artistas.Remove(s));

                                    //obtenemos la lista de artistas a agregar (los artistas asignados por el usuario)
                                    var artistasAgregados = lstArtistasAlbum.Items.Cast<Artista>().ToList()
                                        .Except(album.Artistas.AsEnumerable(), new Model.ComparadorArtista());

                                    artistasAgregados.ToList().ForEach(s =>
                                    {
                                        Artista artista = contexto.Artistas.Find(s.ArtistaID);
                                        album.Artistas.Add(artista);
                                        contexto.Entry(artista).State = System.Data.Entity.EntityState.Unchanged;
                                    });

                                    //creamos una estructura de datos con el tracklist final
                                    ICollection<CancionAlbums> tracklistActual = new HashSet<CancionAlbums>();
                                    dgvCancionesAlbum.Rows.Cast<DataGridViewRow>().ToList()
                                        .ForEach(r =>
                                        {
                                            CancionAlbums track = new CancionAlbums();
                                            track.Cancion = contexto.Canciones.Find(r.Cells["CancionID_AlbumCancion"].Value);
                                            track.Cancion_CancionID = track.Cancion.CancionID;
                                            track.Album = album;
                                            track.Album_AlbumID = track.Album.AlbumID;
                                            track.Posicion = Convert.ToInt16(r.Cells["colPosicion"].Value);
                                            tracklistActual.Add(track);
                                        });

                                    //obtenemos la lista de canciones a eliminar (las canciones desasignadas por el usuario)
                                    var cancionesAEliminar = album.Tracklist.AsEnumerable()
                                        .Except(tracklistActual, new Model.ComparadorCancionAlbums());

                                    cancionesAEliminar.ToList().ForEach(s => album.Tracklist.Remove(s));

                                    //Actualización del orden de los tracks existentes (las canciones que permanecen)
                                    var cancionesExistentes = tracklistActual.Intersect(album.Tracklist.AsEnumerable(),
                                        new Model.ComparadorCancionAlbums());
                                    cancionesExistentes.ToList().ForEach(s => contexto.CancionAlbums.Find(s.Cancion_CancionID, s.Album_AlbumID)
                                        .Posicion = s.Posicion);

                                    //obtenemos la lista de canciones a agregar (las canciones asignadas por el usuario)
                                    var cancionesAgregadas = tracklistActual.Except(album.Tracklist.AsEnumerable(),
                                        new Model.ComparadorCancionAlbums());
                                    cancionesAgregadas.ToList().ForEach(s => album.Tracklist.Add(s));

                                    contexto.SaveChanges();

                                    //actualización de la fuente de datos de la lista de álbumes
                                    dgrAlbum.DataSource = Album.CatalogoAlbumes();
                                    dgrAlbum.ClearSelection();
                                    VaciaCamposAlbum();

                                    Cursor.Current = Cursors.Default;
                                }
                                else
                                {
                                    MessageBox.Show("El álbum ya está registrado en la base de datos.", "Validación");
                                }
                            }
                        }

                        catch (DbEntityValidationException exception)
                        {
                            string mensaje = String.Empty;
                            var EVErrors = exception.EntityValidationErrors.ToList();
                            EVErrors.ForEach(err =>
                            {
                                var VErrors = err.ValidationErrors.ToList();
                                VErrors.ForEach(err2 => mensaje += err2.PropertyName + new StringBuilder(" -> ") + err2.ErrorMessage + "\n");
                            });
                            MessageBox.Show("No se modificó el album. No se cumplieron las validaciones:\n"
                                + mensaje, "Error");
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("No se pudo actualizar el album. " + exception.Message, "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("La fecha de grabación no puede ser posterior a la de adquisición.", "Validación");
                    }
                }
                else
                {
                    MessageBox.Show("Proporciona el nombre del album, formato y artista.", "Validación");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un album para modificar.", "Validación");
            }
        }

        //eliminar un álbum de la BD
        private void btnEliminarAlbum_Click(object sender, EventArgs e)
        {
            if (dgrAlbum.SelectedRows.Count > 0)
            {
                int albumId = Convert.ToInt32(dgrAlbum.CurrentRow.Cells["AlbumID"].Value);
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        Album album = contexto.Albums.Find(albumId);

                        //eliminamos tanto el álbum como el tracklist asociado (no las canciones)
                        contexto.Entry(album).State = System.Data.Entity.EntityState.Deleted;
                        album.Tracklist.ToList().ForEach(s => contexto.Entry(s).State = System.Data.Entity.EntityState.Deleted);
                        contexto.SaveChanges();

                        //actualización de la fuente de datos de la lista de álbumes
                        dgrAlbum.DataSource = Album.CatalogoAlbumes();
                        dgrAlbum.ClearSelection();
                        VaciaCamposAlbum();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No se pudo eliminar el album. " + exception.Message, "Error");
                }
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Show("Selecciona un album para eliminar.", "Validación");
            }
        }

        //usuario seleccionado en la sección de usuarios
        private void lstUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsuarios.SelectedIndex >= 0)
            {
                txtUsuario.Text = lstUsuarios.Text;
                btnSuspenderUsuario.Text = ((Usuario)lstUsuarios.SelectedItem).Estatus == 1 ? "Suspender" : "Restaurar";
            }
            else
            {
                txtUsuario.Text = String.Empty;
                btnSuspenderUsuario.Text = "Suspender";
            }
            txtPassword.Text = String.Empty;
        }

        //agregar un nuevo usuario a la BD
        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != String.Empty && txtPassword.Text.Trim() != String.Empty)
            {
                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        //verificamos que el usuario no sea repetido
                        if (!contexto.Usuarios.Any(s => s.User.ToLower() == txtUsuario.Text.Trim().ToLower()))
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            Usuario nuevoUsuario = new Usuario();
                            nuevoUsuario.User = txtUsuario.Text.Trim();
                            nuevoUsuario.Password = Encriptacion.MD5Hash(txtPassword.Text.Trim());
                            nuevoUsuario.Estatus = 1;

                            contexto.Usuarios.Add(nuevoUsuario);
                            contexto.SaveChanges();

                            //actualización de la fuente de datos de la lista de usuarios
                            lstUsuarios.DataSource = contexto.Usuarios.OrderBy(s => s.User).ToList<Usuario>();
                            lstUsuarios.SelectedIndex = -1;

                            Cursor.Current = Cursors.Default;
                        }
                        else
                        {
                            MessageBox.Show("El usuario ya está registrado en la base de datos.", "Validación");
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No se pudo agregar el usuario. " + exception.Message, "Error");
                }
            }
            else
            {
                MessageBox.Show("Proporciona usuario y password.", "Validación");
            }
        }

        //modificar un usuario en la BD
        private void btnModificarUsuario_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != String.Empty && txtPassword.Text.Trim() != String.Empty)
            {
                if (lstUsuarios.SelectedIndex >= 0)
                {
                    try
                    {
                        using (var contexto = new DiscografiaDB())
                        {
                            //verificamos que el usuario no sea repetido
                            if (!contexto.Usuarios.Any(s => s.User.ToLower() == txtUsuario.Text.Trim().ToLower()
                                && s.UsuarioID != ((Usuario)lstUsuarios.SelectedItem).UsuarioID))
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                Usuario usuario = contexto.Usuarios.Find(lstUsuarios.SelectedValue);
                                usuario.User = txtUsuario.Text.Trim();
                                usuario.Password = Encriptacion.MD5Hash(txtPassword.Text.Trim());

                                contexto.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                                contexto.SaveChanges();

                                //actualización de la fuente de datos de la lista de usuarios
                                lstUsuarios.DataSource = contexto.Usuarios.OrderBy(s => s.User).ToList<Usuario>();
                                lstUsuarios.SelectedIndex = -1;

                                Cursor.Current = Cursors.Default;
                            }
                            else
                            {
                                MessageBox.Show("El usuario ya existe en la base de datos.", "Validación");
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("No se pudo modificar el usuario. " + exception.Message, "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Selecciona un usuario para modificar.", "Validación");
                }
            }
            else
            {
                MessageBox.Show("Proporciona usuario y password.", "Validación");
            }
        }

        //eliminar un usuario de la BD
        private void btnEliminarUsuario_Click(object sender, EventArgs e)
        {
            if (lstUsuarios.SelectedIndex >= 0)
            {
                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        Usuario usuario = contexto.Usuarios.Find(lstUsuarios.SelectedValue);
                        contexto.Entry(usuario).State = System.Data.Entity.EntityState.Deleted;
                        
                        contexto.SaveChanges();

                        //actualización de la fuente de datos de la lista de usuarios
                        lstUsuarios.DataSource = contexto.Usuarios.OrderBy(s => s.User).ToList<Usuario>();
                        lstUsuarios.SelectedIndex = -1;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No se pudo eiminar el usuario. " + exception.Message, "Error");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un usuario para eliminar.", "Validación");
            }
        }

        //suspender/reactivar un usuario en la BD
        private void btnSuspenderUsuario_Click(object sender, EventArgs e)
        {
            if (lstUsuarios.SelectedIndex >= 0)
            {
                try
                {
                    using (var contexto = new DiscografiaDB())
                    {
                        Usuario usuario = contexto.Usuarios.Find(lstUsuarios.SelectedValue);
                        usuario.Estatus = (short)(btnSuspenderUsuario.Text == "Suspender" ? 0 : 1);
                        contexto.Entry(usuario).State = System.Data.Entity.EntityState.Modified;

                        contexto.SaveChanges();

                        //actualización de la fuente de datos de la lista de usuarios
                        lstUsuarios.DataSource = contexto.Usuarios.OrderBy(s => s.User).ToList<Usuario>();
                        lstUsuarios.SelectedIndex = -1;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("No se pudo cambiar el estatus del usuario. " + exception.Message, "Error");
                }
            }
        }
    }
}
