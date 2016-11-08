using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF.Business
{
    public class SilviculturaPPFFValidar
    {
        #region Propriedades

        SilviculturaPPFFDa _da = new SilviculturaPPFFDa();
        CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
        CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
        CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

        #endregion

        internal bool Salvar(SilviculturaPPFF caracterizacao)
        {
            if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
            {
                return false;
            }

            if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new SilviculturaPPFF()).Id > 0)
            {
                Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
                return false;
            }

            if (caracterizacao.Atividade <= 0)
            {
                Validacao.Add(Mensagem.SilviculturaPPFF.AtividadeObrigatoria);
            }

            if (caracterizacao.FomentoTipo == eFomentoTipo.Vazio)
            {
                Validacao.Add(Mensagem.SilviculturaPPFF.FomentoTipoObrigatorio);
            }
            
            if (!String.IsNullOrWhiteSpace(caracterizacao.AreaTotal))
            {
                decimal aux = 0;

                if (Decimal.TryParse(caracterizacao.AreaTotal, out aux))
                {
                    if (aux <= 0)
                    {
                        Validacao.Add(Mensagem.SilviculturaPPFF.AreaTotalMaiorZero);
                    }
                }
                else
                {
                    Validacao.Add(Mensagem.SilviculturaPPFF.AreaTotalInvalida);
                }
            }
            else
            {
                Validacao.Add(Mensagem.SilviculturaPPFF.AreaTotalObrigatoria);
            }

            #region Municípios

            if (caracterizacao.Itens.Count <= 0)
            {
                Validacao.Add(Mensagem.SilviculturaPPFF.MunicipioObrigatorio);
            }

            #endregion

            return Validacao.EhValido;

        }
    }
}
