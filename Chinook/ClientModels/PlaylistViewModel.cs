namespace Chinook.ClientModels;

public class PlaylistViewModel
{
    public long PlaylistId { get; set; }
    public string Name { get; set; }
    public List<PlaylistTrack> Tracks { get; set; }
}