using AutoMapper;
using Chinook.ClientModels;
using Chinook.Domain.Contracts;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Domain.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> dbFactory;
        private readonly IMapper mapper;
        public ArtistService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper)
        {
            this.dbFactory = dbFactory;
            this.mapper = mapper;
        }

        public async Task<ArtistViewModel> GetArtistByIdAsync(long artistId)
        {
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                var artist = dbContext.Artists.SingleOrDefault(a => a.ArtistId == artistId);
                return artist != null ? mapper.Map<Artist, ArtistViewModel>(artist) : null;
            }
        }

        public async Task<List<ArtistViewModel>> GetArtistsAsync()
        {
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                var artists = dbContext.Artists.Include(a => a.Albums).ToList();
                return mapper.Map<List<Artist>, List<ArtistViewModel>>(artists);
            }
        }
    }
}
