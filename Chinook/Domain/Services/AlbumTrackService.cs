using Chinook.ClientModels;
using Chinook.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using static Chinook.Domain.Constants;

namespace Chinook.Domain.Services
{
    public class AlbumTrackService : IAlbumTrackService
    {
        private readonly IDbContextFactory<ChinookContext> dbFactory;
        private readonly IUserService userService;

        public AlbumTrackService(
            IDbContextFactory<ChinookContext> dbFactory, 
            IUserService userService)
        {
            this.dbFactory = dbFactory;
            this.userService = userService;
        }

        public async Task<List<PlaylistTrack>> GetTracksByArtistIdAsync(long artistId)
        {
            string currentUserId = await userService.GetAuthenticatedUserIdAsync();
            using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                var tracks = dbContext.Tracks.Where(a => a.Album != null && a.Album.ArtistId == artistId)
                    .Include(a => a.Album)
                    .Select(t => new PlaylistTrack()
                    {
                        AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                        TrackId = t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == DefaultPlaylistNames.Favorites)).Any()
                    })
                    .ToList();

                return tracks;
            }
        }
    }
}
