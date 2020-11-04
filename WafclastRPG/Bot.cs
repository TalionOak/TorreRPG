﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using WafclastRPG.Game.Comandos;
using WafclastRPG.Game.Comandos.Acao;
using WafclastRPG.Game.Comandos.Exibir;
using WafclastRPG.Game.Config;
using WafclastRPG.Game.Eventos;

namespace WafclastRPG.Game
{
    public class Bot
    {
        public static DiscordClient Cliente { get; private set; }
        public CommandsNextExtension ComandosNext { get; private set; }
        public static BotInfo BotInfo { get; private set; }

        public Bot(DiscordConfiguration discordConfiguration)
        {
            BotInfo = BotInfo.LoadFromFile("BotInfo.json");
#if DEBUG
            BotInfo.VersaoRevisao++;
            BotInfo.SaveToFile("BotInfo.json");
#endif

            Cliente = new DiscordClient(discordConfiguration);
            Cliente.Ready += Ready.Event;
            Cliente.GuildAvailable += (c, e) => GuildAvailable.Event(c, e, BotInfo);
            Cliente.ClientErrored += ClientErrored.Event;
        }

        public void ModuloComando(CommandsNextConfiguration ccfg)
        {
            ComandosNext = Cliente.UseCommandsNext(ccfg);
            ComandosNext.CommandExecuted += CommandExecuted.Event;
            ComandosNext.CommandErrored += CommandErrored.EventAsync;

            ComandosNext.SetHelpFormatter<IAjudaComando>();

            ComandosNext.RegisterCommands<ComandoAjuda>();
            ComandosNext.RegisterCommands<ComandoStatus>();
            ComandosNext.RegisterCommands<ComandoAdministrativo>();
            ComandosNext.RegisterCommands<ComandoCriarPersonagem>();
            ComandosNext.RegisterCommands<ComandoAtacar>();
            ComandosNext.RegisterCommands<ComandoDescer>();
            ComandosNext.RegisterCommands<ComandoSubir>();
            ComandosNext.RegisterCommands<ComandoExplorar>();
            ComandosNext.RegisterCommands<ComandoZona>();
            ComandosNext.RegisterCommands<ComandoUsarPocao>();
            ComandosNext.RegisterCommands<ComandoPegar>();
            ComandosNext.RegisterCommands<ComandoMochila>();
            ComandosNext.RegisterCommands<ComandoEquipamentos>();
            ComandosNext.RegisterCommands<ComandoEquipar>();
            ComandosNext.RegisterCommands<ComandoDesequipar>();
            ComandosNext.RegisterCommands<ComandoExaminar>();
            ComandosNext.RegisterCommands<ComandoChao>();
            ComandosNext.RegisterCommands<ComandoMonstros>();
            ComandosNext.RegisterCommands<ComandoVender>();
            ComandosNext.RegisterCommands<ComandoLoja>();
            ComandosNext.RegisterCommands<ComandoComprar>();
            ComandosNext.RegisterCommands<ComandoPortal>();
            ComandosNext.RegisterCommands<ComandoBot>();
        }
    }
}