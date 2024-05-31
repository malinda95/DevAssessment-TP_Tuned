using Chinook.ClientModels;

namespace Chinook.Domain.Contracts
{
    public interface IAlbumTrackService
    {
        Task<List<PlaylistTrack>> GetTracksByArtistIdAsync(long artistId);
    }
}