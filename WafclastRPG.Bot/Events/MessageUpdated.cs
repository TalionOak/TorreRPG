﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace WafclastRPG.Bot.Events
{
    public class MessageUpdated
    {
        public static async Task Event(DiscordClient c, MessageUpdateEventArgs e)
        {
            await Task.CompletedTask;
        }
    }
}
