using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class InfracaoValidar
	{
		InfracaoDa _da = new InfracaoDa();

		public bool Salvar(Infracao infracao)
		{
            if (infracao.ComInfracao == null)
            {
                Validacao.Add(Mensagem.InfracaoMsg.TipoInfracaoFiscalizacaoObrigatorio);
            }
            else
            {
                #region Caracterização da infração

                if (infracao.ClassificacaoId == 0)
                {
                    Validacao.Add(Mensagem.InfracaoMsg.ClassificacaoObrigatorio);
                }

                if (infracao.TipoId == 0)
                {
                    Validacao.Add(Mensagem.InfracaoMsg.TipoInfracaoObrigatorio);
                }

                if (infracao.ItemId == 0)
                {
                    Validacao.Add(Mensagem.InfracaoMsg.ItemObrigatorio);
                }

                if (infracao.Campos.Count > 0 && infracao.Campos.Count(x => string.IsNullOrWhiteSpace(x.Texto)) > 0)
                {
                    Validacao.Add(Mensagem.InfracaoMsg.CamposObrigatorioo);
                }

                if (infracao.Perguntas.Count > 0 && infracao.Perguntas.Count(x => x.RespostaId == 0 || (x.IsEspecificar && string.IsNullOrWhiteSpace(x.Especificacao))) > 0)
                {
                    Validacao.Add(Mensagem.InfracaoMsg.QuestionariosObrigatorio);
                }

                #endregion

                if (_da.ConfigAlterada(infracao.ConfiguracaoId, infracao.ConfiguracaoTid))
                {
                    Validacao.Add(Mensagem.InfracaoMsg.ConfigAlteradaSemAtualizar);
                }

                if (_da.PerguntaRespostaAlterada(infracao))
                {
                    Validacao.Add(Mensagem.InfracaoMsg.ConfigAlteradaSemAtualizar);
                }

                if (infracao.ComInfracao == true)
                {
                    if (infracao.EnquadramentoInfracao.Artigos.Count == 0)
                    {
                        Validacao.Add(Mensagem.InfracaoMsg.EnquadramentoInfracaoObrigatorio);
                    }

                    if (string.IsNullOrWhiteSpace(infracao.DescricaoInfracao))
                    {
                        Validacao.Add(Mensagem.InfracaoMsg.DescricaoInfracaoObrigatorio);
                    }
                }

                ValidacoesGenericasBus.DataMensagem(infracao.DataConstatacao, "Infracao_DataConstatacao", "constatação/vistoria");

                DateTime hora = new DateTime();
                if (!DateTime.TryParse(infracao.HoraConstatacao, out hora))
                {
                    Validacao.Add(Mensagem.InfracaoMsg.HoraConstatacaoObrigatorio);
                }

                //OBS: foi colocado esse if duas vezes para poder exibir as mensagens de acordo com a ordem dos campos na tela
                if (infracao.ComInfracao == true)
                {
                    if (infracao.ClassificacaoInfracao == null)
                    {
                        Validacao.Add(Mensagem.InfracaoMsg.ClassificacaoInfracaoObrigatorio);
                    }

                    if (!PenalidadeSelecionada(infracao))
                    {
                        Validacao.Add(Mensagem.InfracaoMsg.PenalidadeObrigatorio);

                    }
                }
            }

			return Validacao.EhValido;
		}

        private bool PenalidadeSelecionada(Infracao infracao)
        {
            bool penalidadeSelecionada = true;

            if (infracao.PossuiAdvertencia != true
                && infracao.PossuiApreensao != true
                && infracao.PossuiInterdicaoEmbargo != true
                && infracao.PossuiMulta != true
                && infracao.IdsOutrasPenalidades.Count == 0)
            {
                penalidadeSelecionada = false;
            }

            return penalidadeSelecionada;
        }
	}
}
