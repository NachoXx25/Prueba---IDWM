using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chairs_dotnet7_api
{
    public class Chair
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; } 
        public string Material { get; set; } 
        public string Color { get; set; } 
        public int? Altura { get; set; }  
        public int? Anchura { get; set; } 
        public int? Profundidad { get; set; }  
        public int? Precio { get; set; } 
        public int? Stock { get; set; } 
    }
}