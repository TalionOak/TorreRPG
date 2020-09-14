﻿using DragonsDiscordRPG.Entidades;
using DragonsDiscordRPG.Extensoes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DragonsDiscordRPG.Comandos
{
    public class ComandoDescer : BaseCommandModule
    {
        [Command("descer")]
        public async Task ComandoDescerAsync(CommandContext ctx)
        {
            var jogadorNaoExisteAsync = await ctx.JogadorNaoExisteAsync();
            if (jogadorNaoExisteAsync) return;

            int inimigos = 0;
            using (var session = await ModuloBanco.Cliente.StartSessionAsync())
            {
                BancoSession banco = new BancoSession(session);
                RPJogador jogador = await banco.GetJogadorAsync(ctx);
                RPPersonagem personagem = jogador.Personagem;

                if (personagem.Zona.Monstros.Count != 0)
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, você precisa eliminar todos os montros para descer!");
                    return;
                }

                bool temMonstros = ModuloBanco.MonstrosNomes.ContainsKey(personagem.Zona.Nivel + 1);
                if (temMonstros)
                {

                    inimigos = personagem.Zona.TrocarZona(personagem.VelocidadeAtaque.Atual, personagem.Zona.Nivel + 1);

                    await banco.EditJogadorAsync(jogador);
                    await session.CommitTransactionAsync();
                    await ctx.RespondAsync($"{ctx.User.Mention}, apareceu {inimigos} monstro na sua frente!");
                }
                else
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, não existe mais zonas para avançar!");
                }

            }
        }
    }
}
