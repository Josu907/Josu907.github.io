using System;
using System.Drawing;

namespace TRON
{
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
        public static Poder GenerarPoderAleatorio()
        {
            Random random = new Random();
            string[] nombresPoderes = { "Escudo", "HiperVelocidad" };
            Color[] coloresPoderes = { Color.Green, Color.Red };  // Colores para los poderes
            int index = random.Next(nombresPoderes.Length);

            string nombreAleatorio = nombresPoderes[index];
            Color colorAleatorio = coloresPoderes[index];
            int duracionAleatoria = random.Next(5, 15);  // Duración entre 5 y 15 ticks

            return new Poder(nombreAleatorio, duracionAleatoria, colorAleatorio);
        }

    }
}
