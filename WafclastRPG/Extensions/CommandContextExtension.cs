﻿using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace WafclastRPG.Extensions
{
    public static class CommandContextExtension
    {
        public static Task ResponderAsync(this CommandContext ctx, string mensagem)
            => ctx.RespondAsync($"{ctx.User.Mention}, {mensagem}");

        public static Task ResponderAsync(this CommandContext ctx, string mensagem, DiscordEmbed embed)
            => ctx.RespondAsync($"{ctx.User.Mention}, {mensagem}", embed);

        public static Task ResponderAsync(this CommandContext ctx, DiscordEmbed embed)
            => ctx.RespondAsync(ctx.User.Mention, embed: embed);
    }
}