using Microsoft.Extensions.Configuration;
using src.Services;
using System.Net;
using System.Text;

namespace Tests;


public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;

    public HttpRequestMessage? LastRequest { get; private set; }

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        return Task.FromResult(_response);
    }
}

public class ThrowingHttpMessageHandler : HttpMessageHandler
{
    private readonly Exception _exception;

    public ThrowingHttpMessageHandler(Exception exception)
    {
        _exception = exception;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromException<HttpResponseMessage>(_exception);
    }
}

public class TmdbServiceTests
{
    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["Tmdb:AccessToken"] = "test-token"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    [Fact]
    public async Task SearchMoviesAsync_ReturnsResults_WhenApiSucceeds()
    {
        var json = """
                   {
                       "page": 1,
                       "results": [
                           {
                               "id": 603,
                               "title": "The Matrix",
                               "overview": "A computer hacker learns the truth.",
                               "poster_path": "/matrix.jpg",
                               "release_date": "1999-03-31",
                               "vote_average": 8.2
                           }
                       ],
                       "total_pages": 1,
                       "total_results": 1
                   }
                   """;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json")
        };

        var handler = new FakeHttpMessageHandler(response);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };

        var service = new TmdbService(httpClient, CreateConfiguration());

        var result = await service.SearchMoviesAsync("The Matrix");

        Assert.NotNull(result);
        Assert.Single(result.Results);

        var movie = result.Results[0];

        Assert.Equal(603, movie.Id);
        Assert.Equal("The Matrix", movie.Title);
        Assert.Equal("/matrix.jpg", movie.PosterPath);
        Assert.Equal(8.2, movie.VoteAverage);
    }

    [Fact]
    public async Task SearchMoviesAsync_ReturnsNull_WhenQueryIsEmpty()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        var handler = new FakeHttpMessageHandler(response);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };

        var service = new TmdbService(httpClient, CreateConfiguration());

        var result = await service.SearchMoviesAsync("");

        Assert.Null(result);
    }

    [Fact]
    public void GetPosterUrl_ReturnsExpectedUrl_WhenPosterPathExists()
    {
        var handler = new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };

        var service = new TmdbService(
            httpClient,
            CreateConfiguration());

        var result = service.GetPosterUrl("/matrix.jpg");

        Assert.Equal(
            "https://image.tmdb.org/t/p/w185/matrix.jpg",
            result);
    }

    [Fact]
    public void GetPosterUrl_ReturnsNull_WhenPosterPathIsMissing()
    {
        var handler = new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };

        var service = new TmdbService(
            httpClient,
            CreateConfiguration());

        var result = service.GetPosterUrl(null);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetMovieAsync_ReturnsMovie_WhenApiSucceeds()
    {
        var json = """
                   {
                       "id": 603,
                       "title": "The Matrix",
                       "overview": "A computer hacker learns the truth.",
                       "poster_path": "/matrix.jpg",
                       "release_date": "1999-03-31",
                       "vote_average": 8.2
                   }
                   """;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json")
        };

        var handler = new FakeHttpMessageHandler(response);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };

        var service = new TmdbService(
            httpClient,
            CreateConfiguration());

        var result = await service.GetMovieAsync(603);

        Assert.NotNull(result);
        Assert.Equal(603, result.Id);
        Assert.Equal("The Matrix", result.Title);
        Assert.Equal("/matrix.jpg", result.PosterPath);
        Assert.Equal(8.2, result.VoteAverage);
    }
}