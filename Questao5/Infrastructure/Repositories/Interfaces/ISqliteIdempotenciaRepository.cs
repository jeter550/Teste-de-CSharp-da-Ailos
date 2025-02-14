namespace Questao5.Infrastructure.Repositories.Interfaces
{
    public interface ISqliteIdempotenciaRepository
    {
        Task<string> ConsultarAsync(string chaveIdempotencia);
        Task RegistrarAsync(string chaveIdempotencia, string requisicaoJson, string resultadoJson);
    }
}
