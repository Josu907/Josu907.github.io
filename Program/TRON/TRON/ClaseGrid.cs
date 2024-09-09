using System;
using System.Collections.Generic;

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
        public bool TieneItem { get; set; }    // Indica si hay un ítem en la celda
        public bool TienePoder { get; set; }   // Indica si hay un poder en la celda

        public GridNode(int x, int y)
        {
            X = x;
            Y = y;
            TieneEstela = false;
            TieneItem = false;
            TienePoder = false;
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

        // Método para colocar una estela
        public void ColocarEstela(int x, int y)
        {
            if (x >= 0 && x < Filas && y >= 0 && y < Columnas)
            {
                GridNodes[x, y].TieneEstela = true;
            }
        }

        // Método para restaurar el color original de una celda
        public void RestaurarColorCelda(int x, int y)
        {
            if (x >= 0 && x < Filas && y >= 0 && y < Columnas)
            {
                GridNodes[x, y].TieneEstela = false;  // Restablecer la celda sin estela
            }
        }
    }
}
