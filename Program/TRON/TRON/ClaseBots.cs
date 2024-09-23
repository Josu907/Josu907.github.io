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
            // Escoge una dirección segura evitando colisiones con su propia estela
            Direccion nuevaDireccion = ElegirDireccionSegura(grid);

            // Calcular la nueva posición a la que intentará moverse
            EstelaNodo nuevaPosicion = CalcularNuevaPosicion(nuevaDireccion, grid);

            // Verificar si la nueva posición está ocupada por otra moto o estela
            bool colision = false;
            foreach (var otraMoto in otrasMotos)
            {
                if (otraMoto.Estela.First.Value.X == nuevaPosicion.X && otraMoto.Estela.First.Value.Y == nuevaPosicion.Y)
                {
                    DestruirMoto(Grid grid);
                    colision = true;  // Se detecta una colisión
                    break;
                }
            }

            if (!colision)
            {
                // Si no hay colisión, mover el bot a la nueva posición
                Mover(nuevaDireccion, grid, otrasMotos);
                ultimaDireccion = nuevaDireccion;  // Actualiza la última dirección solo si se mueve
            }
            else
            {
                DestruirMoto(Grid grid);
            }
        }


        // Escoger una dirección segura evitando la estela propia y la dirección opuesta
        private Direccion ElegirDireccionSegura(Grid grid)
        {
            Direccion nuevaDireccion;
            int intentos = 0;
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

                // Verificar si la nueva dirección no lleva a una colisión con la estela propia
                EstelaNodo proximaPosicion = CalcularNuevaPosicion(nuevaDireccion, grid);

                // Verifica si el bot no chocará con su propia estela
                if (!grid.GridNodes[proximaPosicion.X, proximaPosicion.Y].TieneEstela ||
    (proximaPosicion.X == Estela.Last.Value.X && proximaPosicion.Y == Estela.Last.Value.Y))
                {
                    // Si la celda no tiene estela o es la última parte de su estela, es una dirección segura
                    break;
                }
               

                intentos++;
            }
            // Repite si la nueva dirección es la opuesta a la última dirección o lleva a una colisión
            while (EsDireccionOpuesta(nuevaDireccion) || intentos < 10);  // Limitar intentos a 10 para evitar loops infinitos

            return nuevaDireccion;
        }

        // Método para calcular la nueva posición basada en la dirección
        private EstelaNodo CalcularNuevaPosicion(Direccion direccion, Grid grid)
        {
            // Obtiene la posición actual de la cabeza del bot
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
