using AutoMapper;
using AutoMapper.QueryableExtensions;
using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using CB.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class DropdownPayloadAccessor(CbContext context, 
    IMapper mapper) 
    : IDropdownPayloadAccessor
{
    public Task<List<DropdownPayloadDto>> GetAllAsync() =>  context
            .DropdownPayloads
            .AsNoTracking()
            .ProjectTo<DropdownPayloadDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public Task<DropdownPayloadDto> GetByIdAsync(int id) => context.DropdownPayloads
            .AsNoTracking()
            .Where(g => g.Id == id)
            .ProjectTo<DropdownPayloadDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

    public async Task<DropdownPayloadDto> CreateAsync(DropdownPayload entity)
    {
context.DropdownPayloads.Add(entity);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return mapper.Map<DropdownPayloadDto>(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var dropdownPayload = await context
            .DropdownPayloads
            .FindAsync(id)
            .ConfigureAwait(false);

        if (dropdownPayload == null)
        {
            return false;
        }

        context.DropdownPayloads.Remove(dropdownPayload);
        await context
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return true;
    }
}