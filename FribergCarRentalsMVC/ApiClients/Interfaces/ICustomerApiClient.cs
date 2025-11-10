using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients.Interfaces
{
    public interface ICustomerApiClient
    {
        Task<CustomersAllDto.CustomerDto?> Register(CustomersAllDto.CustomerRegisterDto dto, CancellationToken ct = default
        );
    }
}