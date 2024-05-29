namespace Chinook.ClientModels
{
    public partial class AlbumViewModel
    {
        public long AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public long ArtistId { get; set; }
        public virtual ArtistViewModel Artist { get; set; } = null!;
    }
}
