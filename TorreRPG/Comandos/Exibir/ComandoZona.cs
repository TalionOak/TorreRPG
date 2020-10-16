﻿using TorreRPG.Entidades;
using TorreRPG.Extensoes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using TorreRPG.Services;
using System;

namespace TorreRPG.Comandos.Exibir
{
    public class ComandoZona : BaseCommandModule
    {
        public Banco banco { private get; set; }

        [Command("zona")]
        [Description("Permite examinar a zona.")]
        public async Task ComandoZonaAsync(CommandContext ctx)
        {
            // Verifica se existe o jogador,
            var (naoCriouPersonagem, personagemNaoModificar) = await banco.VerificarJogador(ctx);
            if (naoCriouPersonagem) return;

            RPJogador jogador = await banco.GetJogadorAsync(ctx);
            RPPersonagem personagem = jogador.Personagem;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithAuthor($"{ctx.User.Username} - {personagem.Nome}", iconUrl: ctx.User.AvatarUrl);
            embed.WithColor(DiscordColor.Aquamarine);
            embed.WithDescription($"Batalhando contra {personagem.Zona.Monstros.Count} monstros.\n" +
                $"Onda {personagem.Zona.OndaAtual.Bold()}/{personagem.Zona.OndaTotal.Bold()}.\n" +
                $"Nivel {personagem.Zona.Nivel}\n" +
                $"Tem {(personagem.Zona.ItensNoChao == null ? 0 : personagem.Zona.ItensNoChao.Count)} itens no chão\n\n" +
                $"*Digite `!monstros` para ver os inimigos*\n" +
                $"*Digite `!chao` para ver os itens no chão*");

            await ctx.RespondAsync(embed: embed.Build());
        }
    }
}