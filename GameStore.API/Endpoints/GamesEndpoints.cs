using GameStore.API.Dtos;

namespace GameStore.API.Endpoints;

public static class GamesEndpoints
{
    const string GetGameName = "GetGame";

    private static readonly List<GameDto> games = [
        new GameDto(1, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new GameDto(2, "Cyberpunk 2077", "RPG", 59.99m, new DateOnly(2020, 12, 10)),
        new GameDto(3, "Minecraft", "Sandbox", 26.95m, new DateOnly(2011, 11, 18)),
        new GameDto(4, "Among Us", "Party", 4.99m, new DateOnly(2018, 6, 15)),
        new GameDto(5, "Hades", "Roguelike", 24.99m, new DateOnly(2020, 9, 17))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games").WithParameterValidation();
        // get games
        group.MapGet("/", () => games);  

        // get games/1
        group.MapGet("/{id}", (int id) => 
        {
            GameDto? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        })
        .WithName(GetGameName);

        // post /games
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new (
                Id: games.Count + 1,
                Name: newGame.Name,
                Genre: newGame.Genre,
                Price: newGame.Price,
                ReleaseDate: newGame.ReleaseDate
            );

            games.Add(game);

            return Results.CreatedAtRoute(GetGameName, new {id = game.Id}, game);
        });

        // put /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            games[index] = new GameDto(
                Id: id,
                Name: updatedGame.Name,
                Genre: updatedGame.Genre,
                Price: updatedGame.Price,
                ReleaseDate: updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        // delete /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();
        });

        return group;
    }
}
