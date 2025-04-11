using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult<Client>> GetClientByIdAsync(string id);
    Task<ClientResult<IEnumerable<Client>>> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult<IEnumerable<Client>>> GetClientsAsync()
    {
        var repositoryResult = await _clientRepository.GetAllAsync();

        var entities = repositoryResult;
        var clients = entities?.OrderBy(x => x.ClientName).Select(entity => entity.MapTo<Client>()) ?? Enumerable.Empty<Client>();

        return new ClientResult<IEnumerable<Client>> { Succeeded = true, StatusCode = 200, Result = clients };
    }

    public async Task<ClientResult<Client>> GetClientByIdAsync(string id)
    {
        var repositoryResult = await _clientRepository.GetAsync(x => x.Id == id);

        var entity = repositoryResult; // Removed `.Result` as `repositoryResult` is already the entity.
        if (entity == null)
            return new ClientResult<Client> { Succeeded = false, StatusCode = 404, Error = $"Client with id '{id}' was not found." };

        var client = entity.MapTo<Client>();
        return new ClientResult<Client> { Succeeded = true, StatusCode = 200, Result = client };
    }
}
