using CB.Data.Entities;
using CB.Shared.Dtos;

namespace CB.Accessors.Contracts;

public interface IDropdownPayloadAccessor
{
    Task<List<DropdownPayloadDto>> GetAllAsync();

    Task<DropdownPayloadDto> GetByIdAsync(int id);

    Task<DropdownPayloadDto> CreateAsync(DropdownPayload entity);

    //Task<DropdownPayloadDto?> UpdateAsync(string id, 
    //    DropdownPayload entity);

    Task<bool> DeleteAsync(string id);
}