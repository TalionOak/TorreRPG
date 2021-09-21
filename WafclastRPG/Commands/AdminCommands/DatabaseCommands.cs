﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WafclastRPG.DataBases;
using WafclastRPG.Entities.Itens;
using WafclastRPG.Extensions;
using WafclastRPG.Entities.Wafclast;
using System;
using WafclastRPG.Properties;
using System.Text;
using WafclastRPG.Enums;

namespace WafclastRPG.Commands.AdminCommands {
  [ModuleLifespan(ModuleLifespan.Transient)]
  public class DatabaseCommands : BaseCommandModule {
    public DataBase Data { private get; set; }
    public Response Res { private get; set; }
    public TimeSpan timeout = TimeSpan.FromMinutes(2);

    [Command("atualizar-jogadores")]
    [RequireOwner]
    public async Task AtualizarAsync(CommandContext ctx) {
      FilterDefinition<Player> filter = FilterDefinition<Player>.Empty;
      FindOptions<Player> options = new FindOptions<Player> { BatchSize = 8, NoCursorTimeout = false };
      using (IAsyncCursor<Player> cursor = await Data.CollectionPlayers.FindAsync(filter, options))
        while (await cursor.MoveNextAsync()) {
          IEnumerable<Player> list = cursor.Current;
          foreach (Player item in list) {


            await Data.CollectionPlayers.ReplaceOneAsync(x => x.Id == item.Id, item);
          }
        }

      await ctx.RespondAsync("Banco foi atualizado!");
    }

    [Command("atualizar-itens")]
    [RequireOwner]
    public async Task AtualizarItensAsync(CommandContext ctx) {
      FilterDefinition<WafclastBaseItem> filter = FilterDefinition<WafclastBaseItem>.Empty;
      FindOptions<WafclastBaseItem> options = new FindOptions<WafclastBaseItem> { BatchSize = 8, NoCursorTimeout = false };
      using (IAsyncCursor<WafclastBaseItem> cursor = await Data.CollectionItems.FindAsync(filter, options))
        while (await cursor.MoveNextAsync()) {
          IEnumerable<WafclastBaseItem> list = cursor.Current;
          foreach (WafclastBaseItem item in list) {


            await Data.CollectionItems.ReplaceOneAsync(x => x.Id == item.Id, item);
          }
        }
      await ctx.RespondAsync("Banco atualizado!");
    }

    [Command("sudo")]
    [RequireOwner]
    public async Task Sudo(CommandContext ctx, DiscordUser member, [RemainingText] string command) {
      await ctx.TriggerTypingAsync();
      var cmd = ctx.CommandsNext.FindCommand(command, out var args);
      if (cmd == null) {
        await ctx.RespondAsync("Comando não encontrado");
        return;
      }

      var cfx = ctx.CommandsNext.CreateFakeContext(member, ctx.Channel, "", "w.", cmd, args);
      await ctx.CommandsNext.ExecuteCommandAsync(cfx);
    }

    [Command("distance")]
    [RequireOwner]
    public async Task dist(CommandContext ctx, string sx1, string sy1, string sx2, string sy2) {
      await ctx.TriggerTypingAsync();

      int.TryParse(sx1, out int x1);
      int.TryParse(sy1, out int y1);
      int.TryParse(sx2, out int x2);
      int.TryParse(sy2, out int y2);

      Vector v = new Vector(x1, y1);
      await ctx.RespondAsync(v.Distance(new Vector(x2, y2)).ToString("N2"));
    }

    [Command("setmapa")]
    [RequireOwner]
    public async Task setMap(CommandContext ctx) {
      await ctx.TriggerTypingAsync();
      var config = ctx.GetService<Config>();
      var map = ctx.Message.Attachments[0].Url;
      config.MapUrl = map.ToString();
      config.SaveToJsonFile();
      await ctx.RespondAsync($"Mapa salvo: {config.MapUrl}");
    }

    [Command("aviajar")]
    [Aliases("av", "atravel")]
    [Hidden]
    [RequireOwner]
    public async Task AdminTravelCommandAsync(CommandContext ctx, [RemainingText] string roomName) {
      using (var session = await Data.StartDatabaseSessionAsync())
        Res = await session.WithTransactionAsync(async (s, ct) => {
          var player = await session.FindPlayerAsync(ctx);

          var character = player.Character;
          Room room = null;

          if (string.IsNullOrWhiteSpace(roomName)) {
            room = await session.FindRoomAsync(ctx.Channel.Id);
            if (room == null)
              return new Response("você foi para algum lugar, talvez alguns passos a frente.");
          } else {
            room = await session.FindRoomAsync(roomName);
            if (room == null)
              return new Response("você tenta procurar no mapa o lugar, mas não encontra! Como você chegaria em um lugar em que você não conhece?!");
          }

          room.Monster = null;
          character.Room = room;
          await player.SaveAsync();

          return new Response($"você chegou em: **[{room.Name}]!**");
        });
      await ctx.ResponderAsync(Res);
    }
  }
}
