using System;
using System.Collections.Generic;
using System.Drawing;  // Para manejar colores

namespace TRON
{
    public class Moto
    {
        // Atributos de la moto
        public int Velocidad { get; private set; }  // Velocidad de la moto
        public int Combustible { get; private set; }  // Combustible de la moto
        public Queue<Item> Items { get; private set; }  // Cola de ítems
        public Stack<Poder> Poderes { get; private set; }  // Pila de poderes
        public LinkedList<EstelaNodo> Estela { get; private set; }  // Lista enlazada simple que representa la estela
        public int MaxEstela { get; set; } = 4;  // Tamaño máximo de la estela

        public bool EstaDestruida { get; private set; } = false;  // Estado de la moto (si está destruida)

        // Nuevo atributo para el color de la moto/estela
        public Color ColorMoto { get; private set; }

        // Constructor de la moto
        public Moto(Color color)
        {
            Velocidad = new Random().Next(1, 11);  // Velocidad aleatoria entre 1 y 10
            Combustible = 100;  // Combustible inicial al máximo
            Items = new Queue<Item>();  // Inicializa la cola de ítems
            Poderes = new Stack<Poder>();  // Inicializa la pila de poderes
            Estela = new LinkedList<EstelaNodo>();  // Inicializa la lista enlazada de estela
            ColorMoto = color;  // Asigna el color proporcionado

            // Inicializa la estela con 3 posiciones (tamaño inicial de la estela)
            for (int i = 0; i < 3; i++)
            {
                Estela.AddLast(new EstelaNodo(0, i));  // Añade nodos de estela con posiciones iniciales
            }
        }

        // Método para mover la moto
        public void Mover(Direccion direccion, Grid grid, List<Moto> otrasMotos)
        {
            if (EstaDestruida) return;  // Si la moto ya está destruida, no hacer nada

            if (Combustible <= 0)
            {
                DestruirMoto(grid);
                return;
            }

            if (Estela.First == null || grid == null) return;

            // Calcular la nueva posición de la moto
            EstelaNodo nuevaPosicion = CalcularNuevaPosicion(direccion, grid);

            // Verificar si la nueva posición tiene combustible o poder
            RecogerCombustibleYPoder(nuevaPosicion, grid);

            // Verificar si la nueva posición choca con una estela (propia o de otro bot)
            if (grid.GridNodes[nuevaPosicion.X, nuevaPosicion.Y].TieneEstela)
            {
                DestruirMoto(grid);
                return;
            }

            // Verificar si la nueva posición choca con otra moto
            foreach (var otraMoto in otrasMotos)
            {
                if (otraMoto != this && !otraMoto.EstaDestruida && otraMoto.Estela.First != null)
                {
                    EstelaNodo cabezaOtraMoto = otraMoto.Estela.First.Value;
                    if (cabezaOtraMoto.X == nuevaPosicion.X && cabezaOtraMoto.Y == nuevaPosicion.Y)
                    {
                        DestruirMoto(grid);
                        otraMoto.DestruirMoto(grid);  // Destruir la otra moto también
                        return;
                    }
                }
            }

            // Restaurar el color de la celda anterior (la última de la estela) a negro
            if (Estela.Count > 0)
            {
                EstelaNodo ultimaPosicion = Estela.Last.Value;
                grid.RestaurarColorCelda(ultimaPosicion.X, ultimaPosicion.Y);
            }

            // Añadir la nueva posición a la estela y aplicar el color de la moto
            Estela.AddFirst(nuevaPosicion);
            grid.ColocarEstela(nuevaPosicion.X, nuevaPosicion.Y, ColorMoto);  // Aplica el color de la moto a la estela

            // Limitar el tamaño de la estela
            if (Estela.Count > MaxEstela)
            {
                Estela.RemoveLast();
            }

            Combustible -= Velocidad / 5;
        }

        // Método para recoger combustible y poderes
        private void RecogerCombustibleYPoder(EstelaNodo nuevaPosicion, Grid grid)
        {
            var nodoActual = grid.GridNodes[nuevaPosicion.X, nuevaPosicion.Y];

            // Recoger combustible si hay en la celda
            if (nodoActual.TieneItem)
            {
                Combustible = Math.Min(100, Combustible + nodoActual.Combustible);  // Limitar a 100
                nodoActual.TieneItem = false;  // Eliminar el combustible de la celda
                nodoActual.Combustible = 0;  // Restablecer el valor de combustible
            }

            // Recoger poder si hay en la celda
            if (nodoActual.TienePoder)
            {
                Poderes.Push(nodoActual.Poder);  // Añadir el poder a la pila
                nodoActual.TienePoder = false;  // Eliminar el poder de la celda
                nodoActual.Poder = null;  // Restablecer el poder
            }
        }

        // Método para calcular la nueva posición de la moto
        private EstelaNodo CalcularNuevaPosicion(Direccion direccion, Grid grid)
        {
            // Obtiene la posición actual de la cabeza de la moto
            EstelaNodo cabeza = Estela.First.Value;
            int nuevaX = cabeza.X;
            int nuevaY = cabeza.Y;

            // Calcula la nueva posición basada en la dirección
            switch (direccion)
            {
                case Direccion.Arriba:
                    nuevaX -= 1;
                    if (nuevaX < 0) nuevaX = grid.Filas - 1;  // Teletransportar de arriba a abajo
                    break;
                case Direccion.Abajo:
                    nuevaX += 1;
                    if (nuevaX >= grid.Filas) nuevaX = 0;  // Teletransportar de abajo a arriba
                    break;
                case Direccion.Izquierda:
                    nuevaY -= 1;
                    if (nuevaY < 0) nuevaY = grid.Columnas - 1;  // Teletransportar de izquierda a derecha
                    break;
                case Direccion.Derecha:
                    nuevaY += 1;
                    if (nuevaY >= grid.Columnas) nuevaY = 0;  // Teletransportar de derecha a izquierda
                    break;
            }

            return new EstelaNodo(nuevaX, nuevaY);
        }

        // Método para destruir la moto
        public void DestruirMoto(Grid grid)
        {
            if (!EstaDestruida)
            {
                EstaDestruida = true;

                // Limpiar la estela del grid
                foreach (var nodo in Estela)
                {
                    grid.RestaurarColorCelda(nodo.X, nodo.Y);  // Restaurar el color de las celdas ocupadas por la estela
                }
                Estela.Clear();  // Limpiar la estela de la moto
            }
        }
    }

    // Clase auxiliar para representar los nodos de la estela
    public class EstelaNodo
    {
        public int X { get; set; }  // Coordenada X del nodo
        public int Y { get; set; }  // Coordenada Y del nodo

        public EstelaNodo(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Enumeración para representar las direcciones posibles
    public enum Direccion
    {
        Arriba,
        Abajo,
        Izquierda,
        Derecha
    }

    // Clase Item (simplificada para el ejemplo)
    public class Item { }
}
