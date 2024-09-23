using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRON
{
    public class ColorHelper
    {
        private static Random random = new Random();

        // Método para generar colores aleatorios
        public static Color GenerarColorAleatorio()
        {
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
    }
}
