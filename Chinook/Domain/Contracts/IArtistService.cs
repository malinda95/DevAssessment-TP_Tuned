using Chinook.ClientModels;

namespace Chinook.Domain.Contracts
{
    public interface IArtistService
    {
        Task<ArtistViewModel> GetArtistByIdAsync(long artistId);
        Task<List<ArtistViewModel>> GetArtistsAsync();
    }
}