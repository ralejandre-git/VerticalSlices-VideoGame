using Carter;
using MediatR;
using VideoGameApiVsa.Data;

namespace VideoGameApiVsa.Features.VideoGames
{
    public static class DeleteGame
    {
        public record Command(int Id) : IRequest<bool>;
        public class Handler(VideoGameDbContext context) : IRequestHandler<Command, bool>
        {
            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var videoGame = await context.VideoGames.FindAsync(request.Id);
                if (videoGame == null)
                {
                    return false;
                }

                context.VideoGames.Remove(videoGame);
                await context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }

        public class Endpoint : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapDelete("api/games/{id}", async (ISender sender, int id) =>
                await sender.Send(new Command(id)) ? Results.NoContent()
                    : Results.NotFound($"Video game with id {id} not found."));
            }
        }
    }
}
