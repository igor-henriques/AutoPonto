using System;

namespace AutoPonto.Models
{
    public class Ponto
    {
        public DateTime InitialInterval { get; set; }
        public DateTime FinalInterval { get; set; }
        public string PontoDescription { get; set; }
    }
}
