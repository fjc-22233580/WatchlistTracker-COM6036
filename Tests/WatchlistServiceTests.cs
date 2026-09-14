using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Models;
using src.Models.Enums;
using src.Services;

namespace Tests
{
    public class WatchlistServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetUserWatchlistAsync_ReturnsOnlyItemsForSpecifiedUser()
        {
            // Arrange
            await using var context = CreateContext();

            context.WatchlistItems.AddRange(
                new WatchlistItem
                {
                    Title = "Alien",
                    UserId = "user-a"
                },
                new WatchlistItem
                {
                    Title = "Heat",
                    UserId = "user-b"
                });

            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            // Act
            var result = await service.GetUserWatchlistAsync("user-a");

            // Assert
            Assert.Single(result);
            Assert.Equal("Alien", result[0].Title);
            Assert.Equal("user-a", result[0].UserId);
        }

        [Fact]
        public async Task AddAsync_AddsItemToDatabase()
        {
            // Arrange
            await using var context = CreateContext();

            var service = new WatchlistService(context);

            var item = new WatchlistItem
            {
                Title = "The Thing",
                UserId = "user-a",
                Status = WatchStatus.Planned,
                Rating = 9
            };

            // Act
            await service.AddAsync(item);

            // Assert
            var savedItem = await context.WatchlistItems.SingleAsync();

            Assert.Equal("The Thing", savedItem.Title);
            Assert.Equal("user-a", savedItem.UserId);
            Assert.Equal(WatchStatus.Planned, savedItem.Status);
            Assert.Equal(9, savedItem.Rating);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesItemInDatabase()
        {
            // Arrange
            await using var context = CreateContext();

            var item = new WatchlistItem
            {
                Title = "Alien",
                UserId = "user-a",
                Status = WatchStatus.Planned,
                Rating = 7
            };

            context.WatchlistItems.Add(item);
            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            // Act
            item.Status = WatchStatus.Watched;
            item.Rating = 9;

            await service.UpdateAsync(item);

            // Assert
            var updatedItem = await context.WatchlistItems.SingleAsync();

            Assert.Equal("Alien", updatedItem.Title);
            Assert.Equal(WatchStatus.Watched, updatedItem.Status);
            Assert.Equal(9, updatedItem.Rating);
        }

        [Fact]
        public async Task DeleteAsync_RemovesItemFromDatabase()
        {
            // Arrange
            await using var context = CreateContext();

            var item = new WatchlistItem
            {
                Title = "Alien",
                UserId = "user-a",
                Status = WatchStatus.Planned
            };

            context.WatchlistItems.Add(item);
            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            // Act
            await service.DeleteAsync(item);

            // Assert
            Assert.Empty(context.WatchlistItems);
        }

        [Fact]
        public async Task GetUserWatchlistAsync_FiltersBySearchTermAndStatus()
        {
            // Arrange
            await using var context = CreateContext();

            context.WatchlistItems.AddRange(
                new WatchlistItem
                {
                    Title = "Alien",
                    UserId = "user-a",
                    Status = WatchStatus.Watched
                },
                new WatchlistItem
                {
                    Title = "Aliens",
                    UserId = "user-a",
                    Status = WatchStatus.Planned
                },
                new WatchlistItem
                {
                    Title = "Heat",
                    UserId = "user-a",
                    Status = WatchStatus.Watched
                });

            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            // Act
            var result = await service.GetUserWatchlistAsync(
                "user-a",
                "Alien",
                WatchStatus.Watched);

            // Assert
            Assert.Single(result);
            Assert.Equal("Alien", result[0].Title);
            Assert.Equal(WatchStatus.Watched, result[0].Status);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenItemBelongsToDifferentUser()
        {
            // Arrange
            await using var context = CreateContext();

            var item = new WatchlistItem
            {
                Title = "Alien",
                UserId = "user-a",
                Status = WatchStatus.Watched
            };

            context.WatchlistItems.Add(item);
            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            // Act
            var result = await service.GetByIdAsync(item.Id, "user-b");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetDashboardCountsAsync_ReturnsCorrectCounts()
        {
            // Arrange
            await using var context = CreateContext();

            context.WatchlistItems.AddRange(
                new WatchlistItem
                {
                    Title = "Alien",
                    UserId = "user-a",
                    Status = WatchStatus.Planned
                },
                new WatchlistItem
                {
                    Title = "Heat",
                    UserId = "user-a",
                    Status = WatchStatus.Watching
                },
                new WatchlistItem
                {
                    Title = "The Thing",
                    UserId = "user-a",
                    Status = WatchStatus.Watched
                },
                new WatchlistItem
                {
                    Title = "Aliens",
                    UserId = "user-a",
                    Status = WatchStatus.Watched
                },
                new WatchlistItem
                {
                    Title = "Dune",
                    UserId = "user-b",
                    Status = WatchStatus.Watched
                });

            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            // Act
            var result = await service.GetDashboardCountsAsync("user-a");

            // Assert
            Assert.Equal(4, result.Total);
            Assert.Equal(1, result.Planned);
            Assert.Equal(1, result.Watching);
            Assert.Equal(2, result.Watched);
        }

        [Fact]
        public async Task GetCurrentlyWatchingAsync_ReturnsOnlyWatchingItemsForSpecifiedUser()
        {
            await using var context = CreateContext();

            context.WatchlistItems.AddRange(
                new WatchlistItem
                {
                    Title = "Movie A",
                    Status = WatchStatus.Watching,
                    UserId = "user-a"
                },
                new WatchlistItem
                {
                    Title = "Movie B",
                    Status = WatchStatus.Planned,
                    UserId = "user-a"
                },
                new WatchlistItem
                {
                    Title = "Movie C",
                    Status = WatchStatus.Watching,
                    UserId = "user-b"
                });

            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            var result = await service.GetCurrentlyWatchingAsync("user-a");

            Assert.Single(result);
            Assert.Equal("Movie A", result[0].Title);
            Assert.Equal(WatchStatus.Watching, result[0].Status);
            Assert.Equal("user-a", result[0].UserId);
        }

        [Fact]
        public async Task GetCurrentlyWatchingAsync_RespectsCountLimit()
        {
            await using var context = CreateContext();

            context.WatchlistItems.AddRange(
                new WatchlistItem
                {
                    Title = "Movie A",
                    Status = WatchStatus.Watching,
                    UserId = "user-a"
                },
                new WatchlistItem
                {
                    Title = "Movie B",
                    Status = WatchStatus.Watching,
                    UserId = "user-a"
                },
                new WatchlistItem
                {
                    Title = "Movie C",
                    Status = WatchStatus.Watching,
                    UserId = "user-a"
                });

            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            var result = await service.GetCurrentlyWatchingAsync(
                "user-a",
                count: 2);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenMovieExistsForUser()
        {
            await using var context = CreateContext();

            context.WatchlistItems.Add(
                new WatchlistItem
                {
                    Title = "The Matrix",
                    Status = WatchStatus.Watched,
                    UserId = "user-a",
                    TmdbId = 603
                });

            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            var result = await service.ExistsAsync(
                "user-a",
                603);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenMovieExistsForDifferentUser()
        {
            await using var context = CreateContext();

            context.WatchlistItems.Add(
                new WatchlistItem
                {
                    Title = "The Matrix",
                    Status = WatchStatus.Watched,
                    UserId = "user-a",
                    TmdbId = 603
                });

            await context.SaveChangesAsync();

            var service = new WatchlistService(context);

            var result = await service.ExistsAsync(
                "user-b",
                603);

            Assert.False(result);
        }
    }
}
