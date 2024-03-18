using System.Reflection.Metadata.Ecma335;
using GameStore.Api.Dtos;
using Microsoft.VisualBasic;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGame";

    private static readonly List<GameDto> games = [
    new(
        1,
        "Avatar1",
        "Roleplaying",
        19.9M,
        new DateOnly(1992, 3, 3)),
    new(
        2,
        "FIFA4",
        "Sports",
        25.9M,
        new DateOnly(1994, 5, 5)),
    new(
        3,
        "Street fighter 2",
        "Fighting",
        5.5M,
        new DateOnly(1990, 10, 5)),
    ];

    public static RouteGroupBuilder MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                        .WithParameterValidation();

        // GET /games
        group.MapGet("/", () => games);

        // GET /games/1
        group.MapGet("/{Id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        })
            .WithName(GetGameEndPointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate);

            games.Add(game);
            return Results.CreatedAtRoute(GetGameEndPointName, new {id = game.Id}, game);
        });

        // PUT /games
        group.MapPut("/{Id}", (int id, UpdateGameDto updateGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);
            if(index == -1)
            {
                return Results.NotFound();
            }
            games[index] = new GameDto(
                id,
                updateGame.Name,
                updateGame.Genre,
                updateGame.Price,
                updateGame.ReleaseDate
            );
            return Results.NoContent();
        });

        // DELETE /games
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);
            return Results.NoContent();

        });

        return group;
    }

}
