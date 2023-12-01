namespace ItauAPI.Models;

public class Pais
{
    public string Nome { get; set; }
    public string CodigoISO { get; set; }
    public string Continente { get; set; }
    public double Populacao { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}