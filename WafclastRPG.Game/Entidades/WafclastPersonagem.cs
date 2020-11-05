﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using WafclastRPG.Game.Entidades.Itens;
using WafclastRPG.Game.Enums;
using WafclastRPG.Game.Services;

namespace WafclastRPG.Game.Entidades
{
    [BsonIgnoreExtraElements]
    public class WafclastPersonagem
    {
        public WafclastPontoRegenerativo Vida { get; private set; } = new WafclastPontoRegenerativo();
        public WafclastPontoRegenerativo Mana { get; private set; } = new WafclastPontoRegenerativo();

        #region Atributos
        public int Forca { get; private set; }
        public int Destreza { get; private set; }
        public int Inteligencia { get; private set; }
        #endregion

        #region Pontos
        public WafclastPonto Precisao { get; private set; } = new WafclastPonto();
        public WafclastPonto Evasao { get; private set; } = new WafclastPonto();
        public WafclastPonto Armadura { get; private set; } = new WafclastPonto();
        #endregion

        #region Dano físico
        [BsonIgnore]
        public WafclastDano DanoFisicoCalculado
        {
            get
            {
                Random rng = new Random();
                WafclastDano dano = new WafclastDano(DanoFisicoBase);
                dano.Minimo *= DanoFisicoExtraPorcentagem;
                dano.Maximo *= DanoFisicoExtraPorcentagem;
                dano.Somar(DanoFisicoExtra);
                return dano;
            }
        }
        public double DanoFisicoExtraPorcentagem { get; private set; } = 1;
        public WafclastDano DanoFisicoExtra { get; private set; } = new WafclastDano();
        public WafclastDano DanoFisicoBase { get; private set; }
        public double DanoFisicoDesviarChance { get; private set; }
        public double DanoFisicoCriticoChance { get; private set; }
        public double DanoFisicoCriticoMultiplicador { get; private set; } = 1.5;
        #endregion

        #region Equipamentos equipados
        public WafclastItem MaoPrincipal { get; private set; }
        public WafclastItem MaoSecundaria { get; private set; }
        #endregion

        #region Outros
        public WafclastZona Zona { get; private set; } = new WafclastZona();
        public WafclastClasse Classe { get; private set; }
        public WafclastNivel Nivel { get; private set; } = new WafclastNivel();
        public WafclastMochila Mochila { get; private set; } = new WafclastMochila();
        #endregion

        public WafclastPersonagem(WafclastClasse classe, WafclastDano dano,
            int forca, int destreza, int inteligencia)
        {
            this.Classe = classe;
            this.Forca = forca;
            this.Destreza = destreza;
            this.Inteligencia = inteligencia;

            DanoFisicoBase = dano;

            CalcVida();
            CalcMana();
            CalcEvasao();
            CalcPrecisao();

            Vida.Incrementar(double.MaxValue);
            Mana.Incrementar(double.MaxValue);
        }

        public double CausarDanoFisico(double danoFisico)
        {
            double porcentagemReducao = Math.Clamp(Armadura.Calculado / (Armadura.Calculado + 10 * danoFisico), 0, 0.9) * danoFisico;
            double danoReduzido = danoFisico - porcentagemReducao;
            Vida.Diminuir(danoReduzido);
            return danoReduzido;
        }

        public void Resetar()
        {
            Vida.Incrementar(double.MaxValue);
            Mana.Incrementar(double.MaxValue);
            Nivel.Penalizar();
        }

        public void Equipar(WafclastItem item)
        {
            switch (item)
            {
                case WafclastItemArma arma:
                    DanoFisicoExtra.Somar(arma.DanoFisicoCalculado);
                    break;
            }
        }

        public void CalcVida()
        {
            Vida.WithBase(38 + (Nivel.Atual * 12) + (Forca / 2));
        }

        public void CalcMana()
        {
            Mana.WithBase(40 + (Nivel.Atual * 6) + (Inteligencia / 2));
            Mana.WithRegen(0.018 * Mana.Maximo);
        }

        public void CalcEvasao()
        {
            Evasao.WithBase((53 + (Nivel.Atual * 3)) * ((Destreza / 5 * 0.01) + 1));
        }

        public void CalcPrecisao()
        {
            Precisao.WithBase((Destreza * 2) + ((Nivel.Atual - 1) * 2));
        }

        public bool AddExp(double exp)
        {
            int quantEvoluiu = Nivel.AddExp(exp);
            if (quantEvoluiu != 0)
            {
                CalcVida();
                CalcMana();
                CalcEvasao();
                CalcPrecisao();
                Vida.Incrementar(double.MaxValue);
                Mana.Incrementar(double.MaxValue);
                return true;
            }
            return false;
        }

        public StringBuilder AtacarMonstro(out WafclastBatalha resultado, int ataque = 0)
        {
            resultado = WafclastBatalha.InimigoAbatido;

            if (Zona.Monstro == null)
                Zona.SortearMonstro(Nivel.Atual);

            if (Zona.MonstroAtacar(this, out var batalha))
            {
                Resetar();
                batalha.AppendLine($"**{Emoji.CrossBone} Você morreu!!! {Emoji.CrossBone}**");
                resultado = WafclastBatalha.PersonagemAbatido;
                return batalha;
            }
            Zona.Turno++;
            // Chance acerto.
            if (Calculo.DanoFisicoChanceAcerto(Precisao.Calculado, Zona.Monstro.Evasao))
            {
                var dp = DanoFisicoCalculado;
                var dano = Calculo.SortearValor(dp.Minimo, dp.Maximo);
                batalha.AppendLine($"\n{Emoji.Adaga} Você causou {dano:N2} de dano no {Zona.Monstro.Nome}!");
                // Monstro morto.
                if (Zona.Monstro.CausarDano(dano))
                {
                    batalha.AppendLine($"{Emoji.CrossBone} **{Zona.Monstro.Nome}** ️{Emoji.CrossBone}");
                    batalha.AppendLine($"<:xp:758439721016885308>+{Zona.Monstro.Exp:N2}.");
                    if (AddExp(Zona.Monstro.Exp))
                        resultado = WafclastBatalha.Evoluiu;

                    Zona.Monstro = null;
                }
            }
            else
                batalha.AppendLine($"\n{Emoji.CarinhaDesapontado} Você errou o ataque!");
            return batalha;
        }

        public static string VidaEmoji(double porcentagem)
        {
            switch (porcentagem)
            {
                case double n when (n > 0.75):
                    return Emoji.CoracaoVerde;
                case double n when (n > 0.50):
                    return Emoji.CoracaoAmarelo;
                case double n when (n > 0.25):
                    return Emoji.CoracaoLaranja;
            }
            return Emoji.CoracaoVermelho;
        }

        public static string ManaEmoji(double porcentagem)
        {
            switch (porcentagem)
            {
                case double n when (n > 0.75):
                    return Emoji.CirculoVerde;
                case double n when (n > 0.50):
                    return Emoji.CirculoAmarelo;
                case double n when (n > 0.25):
                    return Emoji.CirculoLaranja;
            }
            return Emoji.CirculoVermelho;
        }
    }

    public enum WafclastBatalha
    {
        InimigoAbatido,
        PersonagemAbatido,
        Evoluiu,
    }
}
