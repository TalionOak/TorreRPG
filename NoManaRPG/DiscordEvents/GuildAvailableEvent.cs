﻿// This file is part of NoManaRPG project.

using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using NoManaRPG.Extensions;

namespace NoManaRPG.DiscordEvents;

public static class GuildAvailableEvent
{
    public static Task Event(DiscordClient client, GuildCreateEventArgs e)
    {
        client.Logger.LogInformation(new EventId(603, "Nova guilda"), $"Guilda {e.Guild.Name.RemoverAcentos()} : {e.Guild.MemberCount} Membros.", DateTime.Now);
        return Task.CompletedTask;
    }
}