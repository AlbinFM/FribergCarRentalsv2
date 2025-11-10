using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients.Interfaces;

public interface ICarsApiClient
{
    Task<List<CarDto>> GetAvailableCars(CancellationToken ct = default);
    Task<CarDto?> GetDetails(int id, CancellationToken ct = default);
}