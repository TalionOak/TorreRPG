﻿using TorreRPG.Entidades;
using TorreRPG.Extensoes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using TorreRPG.Atributos;
using TorreRPG.Services;

namespace TorreRPG.Comandos.Acao
{
    public class ComandoCriarPersonagem : BaseCommandModule
    {
        public Banco banco { private get; set; }

        [Command("criar-personagem")]
        [Aliases("cp")]
        [Description("Permite criar um personagem com uma das 7 classes disponíveis e com um nome personalizado.")]
        [ComoUsar("criar-personagem <CLASSE> <NOME DO PERSONAGEM>")]
        [Exemplo("criar-personagem Caçadora Arqueiro Verde")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task CriarPersonagemAsync(CommandContext ctx, string classe = null, [RemainingText] string nomePersonagem = null)
        {
            var jogadorExiste = await JogadorExisteAsync(ctx);
            if (jogadorExiste) return;

            if (string.IsNullOrWhiteSpace(classe))
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                embed.WithColor(DiscordColor.Gold);
                embed.WithTitle("Classes disponíveis");
                embed.WithDescription($"Escolha uma classe digitando `!criar-personagem <classe> <nome do personagem>`.");
                embed.AddField("Caçadora".Titulo(), "Foco em: Destreza", true);
                embed.AddField("Berserker".Titulo(), "Foco em: Força", true);
                embed.AddField("Bruxa".Titulo(), "Foco em: Inteligência", true);
                embed.AddField("Duelista".Titulo(), "Foco em: Força e Destreza", true);
                embed.AddField("Templário".Titulo(), "Foco em: Força e Inteligência", true);
                embed.AddField("Sombra".Titulo(), "Foco em: Destreza e Inteligência", true);
                embed.AddField("Herdeira".Titulo(), "Foco em : Força, Destreza e Inteligência", true);
                embed.WithFooter("Se estiver perdido digite `!ajuda`.", "https://cdn.discordapp.com/attachments/736163626934861845/742671714386968576/help_animated_x4_1.gif");
                await ctx.RespondAsync(embed: embed.Build());
                return;
            }

            if (string.IsNullOrWhiteSpace(nomePersonagem))
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, você precisa informar o nome do seu personagem!");
                return;
            }

            if (nomePersonagem.Length > 12)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, o nome do seu personagem precisa ter menos de 12 caracteres!");
                return;
            }

            string classeFormatada = classe.RemoverAcentos().ToLower();

            switch (classeFormatada)
            {
                case "cacadora":
                    var per1 = new RPPersonagem("Caçadora", nomePersonagem, new RPAtributo(14, 32, 14), new RPDano(2, 5));
                    per1.Mochila.TryAddItem(new Metadata.Itens.Armas.DuasMaoArmas.Arcos().Arco1());
                    per1.Mochila.TryAddItem(new Metadata.Itens.Frascos.FrascosVida().Frasco1());
                    await CriarJogador(ctx, per1);
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(757575940216979562));
                    break;
                case "berserker":
                    var per2 = new RPPersonagem("Berserker", nomePersonagem, new RPAtributo(14, 14, 32), new RPDano(2, 8));
                    per2.Mochila.TryAddItem(new Metadata.Itens.Armas.UmaMaoArmas.MacasUmaMao().Maca1());
                    per2.Mochila.TryAddItem(new Metadata.Itens.Frascos.FrascosVida().Frasco1());
                    await CriarJogador(ctx, per2);
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(757575413697478738));
                    break;
                case "sombra":
                    var per6 = new RPPersonagem("Sombra", nomePersonagem, new RPAtributo(23, 23, 14), new RPDano(2, 5));
                    per6.Mochila.TryAddItem(new Metadata.Itens.Armas.UmaMaoArmas.Adagas().Adaga1());
                    per6.Mochila.TryAddItem(new Metadata.Itens.Frascos.FrascosVida().Frasco1());
                    await CriarJogador(ctx, per6);
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(757575488762806392));
                    break;
                case "duelista":
                    var per4 = new RPPersonagem("Duelista", nomePersonagem, new RPAtributo(14, 23, 23), new RPDano(2, 6));
                    per4.Mochila.TryAddItem(new Metadata.Itens.Armas.UmaMaoArmas.EspadasUmaMao().Espada1());
                    per4.Mochila.TryAddItem(new Metadata.Itens.Frascos.FrascosVida().Frasco1());
                    await CriarJogador(ctx, per4);
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(757575879420411944));
                    break;
                case "bruxa":
                    var per3 = new RPPersonagem("Bruxa", nomePersonagem, new RPAtributo(32, 14, 14), new RPDano(2, 5));
                    per3.Mochila.TryAddItem(new Metadata.Itens.Armas.UmaMaoArmas.Varinhas().Varinha1());
                    per3.Mochila.TryAddItem(new Metadata.Itens.Frascos.FrascosVida().Frasco1());
                    await CriarJogador(ctx, per3);
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(757575261448306701));
                    break;
                case "templario":
                    var per5 = new RPPersonagem("Templário", nomePersonagem, new RPAtributo(23, 14, 23), new RPDano(2, 6));
                    per5.Mochila.TryAddItem(new Metadata.Itens.Armas.UmaMaoArmas.Cetros().Cetro1());
                    per5.Mochila.TryAddItem(new Metadata.Itens.Frascos.FrascosVida().Frasco1());
                    await CriarJogador(ctx, per5);
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(757575545516195900));
                    break;
                case "herdeira":
                    var per7 = new RPPersonagem("Herdeira", nomePersonagem, new RPAtributo(20, 20, 20), new RPDano(2, 6));
                    per7.Mochila.TryAddItem(new Metadata.Itens.Armas.UmaMaoArmas.EspadasUmaMao().Espada1());
                    per7.Mochila.TryAddItem(new Metadata.Itens.Frascos.FrascosVida().Frasco1());
                    await CriarJogador(ctx, per7);
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(757575351789420546));
                    break;
                default:
                    await ctx.RespondAsync($"{ctx.User.Mention}, você informou uma classe que não existe!");
                    break;
            }
        }

        private async Task<bool> JogadorExisteAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            RPJogador jogador = await banco.GetJogadorAsync(ctx);
            if (jogador == null) return false;
            await ctx.RespondAsync($"{ctx.User.Mention}, você já criou um personagem e por isso não pode usar este comando novamente!");
            return true;
        }

        private async Task CriarJogador(CommandContext ctx, RPPersonagem personagem)
        {
            using (var session = await banco.Cliente.StartSessionAsync())
            {
                BancoSession banco = new BancoSession(session);
                RPJogador jogador = new RPJogador(ctx, personagem);
                await banco.AddJogadorAsync(jogador);
                await session.CommitTransactionAsync();
            }
            await ctx.RespondAsync($"{ctx.User.Mention}, o seu personagem foi criado!");
        }
    }
}
