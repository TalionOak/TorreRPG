// This file is part of the WafclastRPG project.

using System;
using System.Configuration;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace WafclastRPG.DiscordEvents
{
    public static class ReadyEvent
    {
        public static Task Event(DiscordClient client, ReadyEventArgs e)
        {
            client.Logger.Log(LogLevel.Information, "Bot está pronto para processar comandos!", DateTime.Now);
            client.UpdateStatusAsync(new DiscordActivity(Program.Config.AppSettings.Settings["ListeningTo"].Value, ActivityType.ListeningTo), UserStatus.Online);
            return Task.CompletedTask;
        }
    }
}
