using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TRON
{
    public partial class Menu : Form
    {
        private Moto miMoto;  // Instancia de la moto del jugador
        private Grid miGrid;  // Instancia del grid
        private const int tamañoCelda = 15;  // Tamaño de cada celda en píxeles
        private const int offsetX = 235;  // Desplazamiento en X para el dibujo del grid
        private const int offsetY = 0;  // Desplazamiento en Y para el dibujo del grid
        private System.Windows.Forms.Timer gameTimer;  // Timer para el movimiento continuo
        private Direccion direccionActual = Direccion.Derecha;  // Dirección inicial de la moto
        private Direccion direccionAnterior = Direccion.Derecha;  // Mantener la dirección anterior
        private List<Moto> otrasMotos;  // Lista de otras motos (bots)
        private List<Bot> bots;  // Lista de bots
        private ProgressBar progressBarCombustible;  // Barra de progreso para el combustible

        public Menu()
        {
            InitializeComponent();
            miGrid = new Grid(40, 40);  // Crear un grid de 40x40
            miMoto = new Moto();  // Crear una moto
            otrasMotos = new List<Moto>();  // Inicializa la lista de otras motos (bots)
            bots = new List<Bot>();  // Inicializa la lista de bots

            // Inicializa la barra de progreso del combustible
            progressBarCombustible = new ProgressBar();
            progressBarCombustible.Location = new Point(10, 10);  // Posición de la barra en la ventana
            progressBarCombustible.Size = new Size(200, 20);  // Tamaño de la barra de progreso
            progressBarCombustible.Maximum = 100;  // El valor máximo de la barra (100 de combustible)
            progressBarCombustible.Value = miMoto.Combustible;  // Valor inicial del combustible
            this.Controls.Add(progressBarCombustible);  // Agregar la barra al formulario

            // Establecer el tamaño inicial de la estela en 3 nodos
            miMoto.Estela.Clear();  // Limpiar cualquier nodo existente
            miMoto.Estela.AddFirst(new EstelaNodo(10, 10));  // Posición inicial
            miMoto.Estela.AddFirst(new EstelaNodo(10, 9));  // Nodo 2
            miMoto.Estela.AddFirst(new EstelaNodo(10, 8));  // Nodo 3

            // Crear y agregar bots
            CrearBots();

            this.DoubleBuffered = true;  // Activa el doble búfer para reducir el parpadeo al dibujar
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);  // Asegura el manejador de eventos para KeyDown

            // Configurar el Timer para el movimiento continuo
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 200;  // Intervalo de tiempo en milisegundos (ajústalo según la velocidad deseada)
            gameTimer.Tick += new EventHandler(GameTick);
            gameTimer.Start();  // Iniciar el Timer
        }

        // Crear bots y añadirlos a la lista
        private void CrearBots()
        {
            for (int i = 0; i < 4; i++)  // Crear 4 bots
            {
                Bot nuevoBot = new Bot();
                nuevoBot.Estela.AddFirst(new EstelaNodo(5 + i, 5 + i));  // Colocar los bots en diferentes posiciones
                bots.Add(nuevoBot);
            }
        }

        // Evento que se ejecuta en cada tick del Timer
        private void GameTick(object? sender, EventArgs e)
        {
            miMoto.Mover(direccionActual, miGrid, bots.Cast<Moto>().ToList());  // Mover la moto en la dirección actual

            // Mover bots
            foreach (var bot in bots)
            {
                bot.MoverBot(miGrid, bots.Cast<Moto>().ToList());  // Línea 76
            }

            // Actualizar la barra de progreso del combustible
            progressBarCombustible.Value = Math.Max(0, miMoto.Combustible);  // Asegurarse de que el valor no sea negativo

            Invalidate();  // Redibujar la pantalla
        }
        private void Form1_Load(object? sender, EventArgs e)
        {
            // Código adicional si es necesario
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Dibujar el grid
            for (int i = 0; i < miGrid.Filas; i++)
            {
                for (int j = 0; j < miGrid.Columnas; j++)
                {
                    // Determinar color de la celda
                    Color colorCelda = miGrid.GridNodes[i, j].TieneEstela ? Color.Red : Color.LightGray;

                    // Dibujar la celda con offset
                    g.FillRectangle(new SolidBrush(colorCelda), offsetX + j * tamañoCelda, offsetY + i * tamañoCelda, tamañoCelda, tamañoCelda);
                    g.DrawRectangle(Pens.Black, offsetX + j * tamañoCelda, offsetY + i * tamañoCelda, tamañoCelda, tamañoCelda);  // Bordes de las celdas
                }
            }

            // Dibujar la moto del jugador
            DibujarMoto(g, miMoto);

            // Dibujar los bots
            foreach (var bot in bots)
            {
                DibujarMoto(g, bot);
            }
        }

        // Método auxiliar para dibujar una moto (jugador o bot)
        private void DibujarMoto(Graphics g, Moto moto)
        {
            bool esCabeza = true;
            foreach (EstelaNodo nodo in moto.Estela)
            {
                // Determinar el color para la cabeza de la moto
                Brush brushMoto = esCabeza ? Brushes.Green : Brushes.Blue;  // Verde para la cabeza, azul para el resto de la estela
                esCabeza = false;

                // Dibujar cada parte de la estela de la moto con offset
                g.FillRectangle(brushMoto, offsetX + nodo.Y * tamañoCelda, offsetY + nodo.X * tamañoCelda, tamañoCelda, tamañoCelda);
                g.DrawRectangle(Pens.Black, offsetX + nodo.Y * tamañoCelda, offsetY + nodo.X * tamañoCelda, tamañoCelda, tamañoCelda);
            }
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // Evitar moverse en dirección opuesta directamente
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direccionAnterior != Direccion.Abajo)  // Evitar moverse hacia abajo si iba hacia arriba
                        direccionActual = Direccion.Arriba;
                    break;
                case Keys.Down:
                    if (direccionAnterior != Direccion.Arriba)  // Evitar moverse hacia arriba si iba hacia abajo
                        direccionActual = Direccion.Abajo;
                    break;
                case Keys.Left:
                    if (direccionAnterior != Direccion.Derecha)  // Evitar moverse hacia la derecha si iba a la izquierda
                        direccionActual = Direccion.Izquierda;
                    break;
                case Keys.Right:
                    if (direccionAnterior != Direccion.Izquierda)  // Evitar moverse hacia la izquierda si iba a la derecha
                        direccionActual = Direccion.Derecha;
                    break;
            }

            // Guardar la dirección actual para la próxima validación
            direccionAnterior = direccionActual;


        }

    }
}
