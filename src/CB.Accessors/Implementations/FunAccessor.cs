using CB.Accessors.Contracts;
using CB.Data;
using CB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CB.Accessors.Implementations;

public class FunAccessor(CbContext context) : IFunAccessor
{
    public async Task<int> IncrementHaiBai()
    {
        var result = await context
            .FunStuff
            .FromSqlRaw("UPDATE \"FunStuff\" SET \"HaiBaiCount\" = \"HaiBaiCount\" + 1 RETURNING \"HaiBaiCount\"")
            .ToListAsync();

        return result.First().HaiBaiCount;
    }
}