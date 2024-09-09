using System;
using System.Collections.Generic;

namespace TRON
{
    public class Bot : Moto
    {
        private Random random;
            
        // Constructor
        public Bot()
        {
            random = new Random();
            // Puedes ajustar el tamaño de la estela o los parámetros si es necesario
        }

        // Método para mover el bot
        public void MoverBot(Grid grid, List<Moto> otrasMotos)
        {
            if (EstaDestruida) return;  // Si el bot está destruido, no hacer nada

            // Escoge una dirección aleatoria
            Direccion direccionAleatoria = ElegirDireccionAleatoria();

            // Mueve el bot en la dirección seleccionada
            Mover(direccionAleatoria, grid, otrasMotos);
        }

        // Escoger una dirección aleatoria
        private Direccion ElegirDireccionAleatoria()
        {
            int direccion = random.Next(4);  // Hay 4 direcciones posibles (0 a 3)
            switch (direccion)
            {
                case 0: return Direccion.Arriba;
                case 1: return Direccion.Abajo;
                case 2: return Direccion.Izquierda;
                case 3: return Direccion.Derecha;
                default: return Direccion.Derecha;  // Por defecto derecha
            }
        }
    }
}
