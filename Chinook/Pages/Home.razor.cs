using Chinook.ClientModels;
using Chinook.Domain.Contracts;
using Microsoft.AspNetCore.Components;

namespace Chinook.Pages
{
    public partial class Home
    {
        private string SearchText { get; set; } = "";
        [Inject] IArtistService ArtistsService { get; set; }
        [Inject] ILogger<Home> Logger { get; set; }

        private List<ArtistViewModel> allArtists;
        private List<ArtistViewModel> artists;
        private string message = "";
        private bool isError = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                allArtists = await ArtistsService.GetArtistsAsync();
                artists = new List<ArtistViewModel>(allArtists);
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while fetching the artists.", ex);
            }
        }

        private void SearchArtists(ChangeEventArgs e)
        {
            try
            {
                SearchText = e.Value.ToString();
                SearchArtists();
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while searching for artists.", ex);
            }
        }

        private void SearchArtists()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                artists = new List<ArtistViewModel>(artists);
            }
            else
            {
                artists = allArtists.Where(a => a.Name.ToLower().Contains(SearchText.ToLower())).ToList();
            }
        }

        private void HandleError(string errorMessage, Exception ex)
        {
            isError = true;
            message = errorMessage;
            Logger.LogError(ex, errorMessage);
        }
    }
}