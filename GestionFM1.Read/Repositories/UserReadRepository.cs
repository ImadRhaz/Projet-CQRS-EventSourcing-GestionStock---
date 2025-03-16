using GestionFM1.Core.Models;
using GestionFM1.Read.QueryDataStore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.Repositories;

public class UserReadRepository 
{
    private readonly QueryDbContext _queryDbContext;
    private readonly ILogger<UserReadRepository> _logger;

    public UserReadRepository(QueryDbContext queryDbContext)
    {
       
    }
}