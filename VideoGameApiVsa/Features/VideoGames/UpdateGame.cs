using Carter;
using MediatR;
using VideoGameApiVsa.Data;

namespace VideoGameApiVsa.Features.VideoGames
{
    public static class UpdateGame
    {
        public record Command(int Id, string Title, string Genre, int ReleaseYear) : IRequest<Response?>;
        public record Response(int Id, string Title, string Genre, int ReleaseYear);
        public class Handler(VideoGameDbContext context) : IRequestHandler<Command, Response?>
        {
            public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
            {
                var videoGame = await context.VideoGames.FindAsync(request.Id);
                if (videoGame == null)
                {
                    return null;
                }

                videoGame.Title = request.Title;
                videoGame.Genre = request.Genre;
                videoGame.ReleaseYear = request.ReleaseYear;

                await context.SaveChangesAsync(cancellationToken);

                return new Response(videoGame.Id,
                    videoGame.Title, videoGame.Genre, videoGame.ReleaseYear);
            }
        }
        public class Endpoint : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapPut("api/games/{id}", async (ISender sender, int id, Command command) =>
                {
                    var updatedGame = await sender.Send(command with { Id = id });
                    return updatedGame is not null ? Results.Ok(updatedGame)
                        : Results.NotFound($"Video game with id {id} not found.");
                });
            }
        }
    }
}
