﻿using WafclastRPG.Game.Entidades;
using WafclastRPG.Game.Extensoes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text;
using System.Threading.Tasks;
using WafclastRPG.Game.Entidades.Itens;
using WafclastRPG.Game.Services;
using System;
using WafclastRPG.Game.Enuns;

namespace WafclastRPG.Game.Comandos.Exibir
{
    public class ComandoMochila : BaseCommandModule
    {
        public Banco banco;

        [Command("mochila")]
        [Description("Permite ver os itens que estão na mochila. Você consegue itens de monstros, vendendo e trocando com outros jogadores.")]
        public async Task ComandoMochilaAsync(CommandContext ctx)
        {
            // Verifica se existe o jogador,
            var (naoCriouPersonagem, personagemNaoModificar) = await banco.VerificarJogador(ctx);
            if (naoCriouPersonagem) return;

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < personagemNaoModificar.Mochila.Itens.Count; i++)
            {
                var item = personagemNaoModificar.Mochila.Itens[i];
                str.Append($"`#{i}` ");
                str.Append(GerarEmojiRaridade(item.Raridade));

                str.Append($" {item.TipoBaseModificado.Titulo().Bold()} ");
                switch (item)
                {
                    case RPMoedaEmpilhavel rcp:
                        str.Append($"*x{rcp.PilhaAtual}*");
                        break;
                }
                str.AppendLine();
            }

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithAuthor($"{ctx.User.Username} - Nível {personagemNaoModificar.Nivel.Atual} - {personagemNaoModificar.Classe}", iconUrl: ctx.User.AvatarUrl);
            embed.WithDescription($"**Espaço da mochila: {personagemNaoModificar.Mochila.Espaco}/64**\n\n{str}");

            await ctx.RespondAsync(embed: embed.Build());
        }

        public static DiscordEmoji GerarEmojiRaridade(RPRaridade raridade)
        {
            switch (raridade)
            {
                case RPRaridade.Normal:
                    return Emoji.ItemNormal;
                case RPRaridade.Magico:
                    return Emoji.ItemMagico;
                case RPRaridade.Raro:
                    return Emoji.ItemRaro;
                case RPRaridade.Unico:
                    return Emoji.ItemUnico;
                default:
                    return null;
            }
        }
    }
}