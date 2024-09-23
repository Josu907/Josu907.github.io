using System;
using System.Collections.Generic;
using System.Drawing;  // Para manejar los colores

namespace TRON
{
    // Clase que representa el nodo de la grid
    public class GridNode
    {
        public int X { get; set; }  // Coordenada X en la malla
        public int Y { get; set; }  // Coordenada Y en la malla

        // Referencias a los nodos adyacentes
        public GridNode? Arriba { get; set; }
        public GridNode? Abajo { get; set; }
        public GridNode? Izquierda { get; set; }
        public GridNode? Derecha { get; set; }

        // Estado de la celda
        public bool TieneEstela { get; set; }  // Indica si hay una estela destructiva en la celda
        public bool TieneItem { get; set; }    // Indica si hay un ítem en la celda (combustible)
        public bool TienePoder { get; set; }   // Indica si hay un poder en la celda

        // Cantidad de combustible en la celda
        public int Combustible { get; set; }

        // Poder disponible en la celda
        public Poder Poder? { get; set; }

        // Color de la celda (usado por estelas, ítems y poderes)
        public Color Color { get; set; }

        public GridNode(int x, int y)
        {
            X = x;
            Y = y;
            TieneEstela = false;
            TieneItem = false;
            TienePoder = false;
            Combustible = 0;  // No hay combustible por defecto
            Poder = null;  // No hay poder por defecto
            Color = Color.Black;  // Color predeterminado (negro para celdas vacías)
        }

        // Actualiza el color de la celda basado en su contenido (combustible o poder)
        public void ActualizarColor()
        {
            if (TieneItem)
            {
                Color = Color.White;  // Color blanco para las celdas de combustible
            }
            else if (TienePoder && Poder != null)
            {
                Color = Poder.ObtenerColor();  // Usa el color específico del poder
            }
            else
            {
                Color = Color.Black;  // Color negro para celdas vacías
            }
        }
    }

    // Clase que representa la malla completa del juego
    public class Grid
    {
        public GridNode[,] GridNodes { get; private set; }  // Matriz de nodos del grid
        public int Filas { get; private set; }  // Número de filas
        public int Columnas { get; private set; }  // Número de columnas

        public Grid(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;
            GridNodes = new GridNode[filas, columnas];  // Inicializa la matriz de nodos

            // Inicializa cada nodo del grid
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    GridNodes[i, j] = new GridNode(i, j);
                }
            }

            // Establece las referencias entre los nodos adyacentes
            EstablecerReferenciasAdyacentes();
        }

        // Método para establecer las referencias entre nodos adyacentes
        private void EstablecerReferenciasAdyacentes()
        {
            for (int i = 0; i < Filas; i++)
            {
                for (int j = 0; j < Columnas; j++)
                {
                    GridNode nodo = GridNodes[i, j];

                    // Establece las referencias a los nodos adyacentes
                    if (i > 0) nodo.Arriba = GridNodes[i - 1, j];  // Nodo superior
                    if (i < Filas - 1) nodo.Abajo = GridNodes[i + 1, j];  // Nodo inferior
                    if (j > 0) nodo.Izquierda = GridNodes[i, j - 1];  // Nodo izquierdo
                    if (j < Columnas - 1) nodo.Derecha = GridNodes[i, j + 1];  // Nodo derecho
                }
            }
        }

        // Método para colocar una estela con color brillante de "neón"
        public void ColocarEstela(int x, int y, Color color)
        {
            if (x >= 0 && x < Filas && y >= 0 && y < Columnas)
            {
                // Verifica si la celda ya tiene una estela de otro bot o del jugador
                if (!GridNodes[x, y].TieneEstela)
                {
                    GridNodes[x, y].TieneEstela = true;
                    GridNodes[x, y].Color = color;  // Asignar color de la estela/moto
                }
                else
                {
                    // Si la celda ya tiene estela, ocurre una colisión
                    ManejarColision(x, y, this, null, null);  // Temporalmente null, debes pasar las instancias correctas
                }
            }
        }

        // Método para restaurar el color original de una celda
        public void RestaurarColorCelda(int x, int y)
        {
            if (x >= 0 && x < Filas && y >= 0 && y < Columnas)
            {
                GridNodes[x, y].TieneEstela = false;
                GridNodes[x, y].ActualizarColor();  // Restaurar el color basado en el contenido (ítems, poderes, etc.)
            }
        }

        // Método para generar una posición aleatoria libre (sin estela, ítem o poder)
        public GridNode GenerarPosicionAleatoriaLibre()
        {
            Random random = new Random();
            GridNode nodoAleatorio;

            do
            {
                // Genera coordenadas X e Y aleatorias dentro de los límites de la malla
                int x = random.Next(Filas);  // Entre 0 y el número de filas
                int y = random.Next(Columnas);  // Entre 0 y el número de columnas

                nodoAleatorio = GridNodes[x, y];

                // Verifica que la celda no tenga estela, ítems ni poderes
            } while (nodoAleatorio.TieneEstela || nodoAleatorio.TieneItem || nodoAleatorio.TienePoder);

            return nodoAleatorio;
        }

        // Método para generar celdas de combustible en la malla
        public void GenerarCeldasDeCombustible(int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                GridNode nodo = GenerarPosicionAleatoriaLibre();
                nodo.TieneItem = true;
                nodo.Combustible = new Random().Next(10, 50);  // Cantidad de combustible entre 10 y 50
                nodo.ActualizarColor();  // Actualizar el color a blanco
            }
        }

        // Método para generar poderes en la malla
        public void GenerarPoderesAleatorios(int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                GridNode nodo = GenerarPosicionAleatoriaLibre();
                nodo.TienePoder = true;
                nodo.Poder = Poder.GenerarPoderAleatorio();  // Asignar un poder aleatorio
                nodo.ActualizarColor();  // Actualizar el color basado en el poder
            }
        }

        // Método para manejar colisiones
        private void ManejarColision(int x, int y, Grid miGrid, Moto? miMoto, List<Bot>? bots)
        {
            // Si bots no es null, verificar colisión con los bots
            if (bots != null)
            {
                foreach (var bot in bots)
                {
                    if (bot?.Estela?.First != null && bot.Estela.First.Value.X == x && bot.Estela.First.Value.Y == y)
                    {
                        bot.DestruirMoto(miGrid);  // Destruir el bot si hay colisión
                    }
                }
            }

            // Si miMoto no es null, verificar colisión con el jugador
            if (miMoto?.Estela?.First != null && miMoto.Estela.First.Value.X == x && miMoto.Estela.First.Value.Y == y)
            {
                miMoto.DestruirMoto(miGrid);  // Destruir al jugador si colisiona
            }
        }
    }

    // Clase de Poder para representar poderes específicos
    public class Poder
    {
        public string Nombre { get; set; }
        public int Duracion { get; set; }
        public Color ColorPoder { get; set; }  // Color específico del poder

        public Poder(string nombre, int duracion, Color color)
        {
            Nombre = nombre;
            Duracion = duracion;
            ColorPoder = color;
        }

        // Método para obtener el color del poder
        public Color ObtenerColor()
        {
            return ColorPoder;
        }

        // Generar un poder aleatorio con colores predefinidos
        
    }
}
