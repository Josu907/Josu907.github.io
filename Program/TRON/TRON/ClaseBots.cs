using System;
using System.Collections.Generic;

namespace TRON
{
    public class Bot : Moto
    {
        private Random random;
        private int ticksDesdeUltimoMovimiento;  // Contador de ticks para controlar el tiempo entre movimientos
        private int ticksParaMoverse;            // Tiempo en ticks para moverse de nuevo
        private Direccion ultimaDireccion;       // Almacena la última dirección en la que el bot se movió

        // Constructor
        public Bot(Grid grid) : base(ColorHelper.GenerarColorAleatorio())
        {
            random = new Random();
            ticksDesdeUltimoMovimiento = 0;
            ticksParaMoverse = 100;  // Ajustable para definir cuántos ticks entre movimientos
            ultimaDireccion = Direccion.Arriba;  // Dirección inicial por defecto

            // Genera una posición aleatoria libre en el grid para el bot
            GridNode posicionInicial = grid.GenerarPosicionAleatoriaLibre();

            // Coloca la estela inicial en la posición aleatoria
            Estela.AddLast(new EstelaNodo(posicionInicial.X, posicionInicial.Y));

            // Marca la celda como ocupada con la estela de la moto
            grid.ColocarEstela(posicionInicial.X, posicionInicial.Y, ColorMoto);
        }

        // Método para actualizar el estado del bot
        public void Actualizar(Grid grid, List<Moto> otrasMotos)
        {
            if (EstaDestruida) return;  // No hacer nada si el bot está destruido

            ticksDesdeUltimoMovimiento++;

            if (ticksDesdeUltimoMovimiento >= ticksParaMoverse)
            {
                MoverBot(grid, otrasMotos);
                ticksDesdeUltimoMovimiento = 0;
            }
        }

        // Método para mover el bot
        public void MoverBot(Grid grid, List<Moto> otrasMotos)
        {
            // Escoge una dirección aleatoria, evitando que sea la opuesta a la última dirección
            Direccion nuevaDireccion = ElegirDireccionAleatoria();

            // Mueve el bot en la nueva dirección
            Mover(nuevaDireccion, grid, otrasMotos);

            // Actualiza la última dirección
            ultimaDireccion = nuevaDireccion;
        }

        // Escoger una dirección aleatoria evitando la dirección opuesta
        private Direccion ElegirDireccionAleatoria()
        {
            Direccion nuevaDireccion;
            do
            {
                int direccion = random.Next(4);  // 0 a 3 para las 4 direcciones posibles
                nuevaDireccion = direccion switch
                {
                    0 => Direccion.Arriba,
                    1 => Direccion.Abajo,
                    2 => Direccion.Izquierda,
                    3 => Direccion.Derecha,
                    _ => Direccion.Derecha,  // Por defecto, derecha
                };
            }
            // Repite si la nueva dirección es la opuesta a la última dirección
            while (EsDireccionOpuesta(nuevaDireccion));

            return nuevaDireccion;
        }

        // Verifica si la nueva dirección es la opuesta a la última
        private bool EsDireccionOpuesta(Direccion nuevaDireccion)
        {
            return (ultimaDireccion == Direccion.Arriba && nuevaDireccion == Direccion.Abajo) ||
                   (ultimaDireccion == Direccion.Abajo && nuevaDireccion == Direccion.Arriba) ||
                   (ultimaDireccion == Direccion.Izquierda && nuevaDireccion == Direccion.Derecha) ||
                   (ultimaDireccion == Direccion.Derecha && nuevaDireccion == Direccion.Izquierda);
        }
    }
}
