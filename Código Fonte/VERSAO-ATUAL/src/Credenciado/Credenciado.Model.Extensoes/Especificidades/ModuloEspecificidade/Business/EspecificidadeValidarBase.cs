using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public class EspecificidadeValidarBase
	{
		EspecificidadeDa _da = new EspecificidadeDa();

		public bool DeclaratorioRequerimentoAtividade(IEspecificidade especificidade, bool solicitado = true, bool jaAssociado = true, bool apenasObrigatoriedade = false)
		{
			if (especificidade.RequerimentoId <= 0)
			{
				// Msg: O requerimento padrão é obrigatório.
				Validacao.Add(Mensagem.Especificidade.RequerimentoPradroObrigatoria);
			}

			if (especificidade.Atividades == null || especificidade.Atividades.Count == 0 || especificidade.Atividades[0].Id == 0)
			{
				// Msg: A atividade é obrigatória.
				Validacao.Add(Mensagem.Especificidade.AtividadeObrigatoria);
			}

			//Retorna se já encontrou erro de obrigatóriedade
			if (Validacao.Erros.Exists(x => x.Texto == Mensagem.Especificidade.RequerimentoPradroObrigatoria.Texto) || Validacao.Erros.Exists(x => x.Texto == Mensagem.Especificidade.AtividadeObrigatoria.Texto))
			{
				return false;
			}

			_da.ObterAtividadesNome(especificidade.Atividades);

			if (!_da.RequerimentoPossuiEmpreendimento(especificidade.RequerimentoId))
			{
				Validacao.Add(Mensagem.Especificidade.RequerimentoEmpreendimentoObrigatorio(especificidade.Atividades.FirstOrDefault().NomeAtividade));
			}
			else
			{
				if (especificidade.Atividades.Any(x => x.Id == 327) && !_da.EmpreendimentoPossuiCaracterizacaoBarragemDis(especificidade.RequerimentoId))
				{
					Validacao.Add(Mensagem.Especificidade.CaracterizacaoBarragemDisNaoCadastrada);
				}
			}

			if (especificidade.Atividades != null)
			{
				foreach (var item in especificidade.Atividades)
				{
					if (!item.Ativada)
					{
						Validacao.Add(Mensagem.AtividadeEspecificidade.AtividadeDesativada(item.NomeAtividade));
					}
				}
			}

			if (apenasObrigatoriedade)
			{
				return Validacao.EhValido;
			}

			bool ehValido = true;

			if (especificidade.Atividades != null)
			{
				object retorno = false;

				foreach (var item in especificidade.Atividades)
				{
					if (jaAssociado)
					{
						//Validação da existencia de um outro título do mesmo modelo e atividade para o mesmo empreendimento
						if (!DeclaratorioValidarAtividadeJaAssociada(especificidade.RequerimentoId, item.Id, especificidade.Titulo.Id, Convert.ToInt32(especificidade.Titulo.Modelo)))
						{
							ehValido = false;
							continue;
						}
					}

					if (solicitado)
					{
						//Validação da solicitação do modelo/atividade no requerimento
						retorno = DeclaratorioValidarModeloNaoRequisitado(especificidade.RequerimentoId, item.Id, Convert.ToInt32(especificidade.Titulo.Modelo));

						if (!Convert.ToBoolean(retorno))
						{
							ehValido = false;
						}
					}
				}
			}

			return ehValido;
		}

		public bool DeclaratorioValidarAtividadeJaAssociada(int requerimento, int atividade, int titulo, int modelo)
		{
			List<TituloEsp> lista = _da.ObterTitulosAtividadeEmpreendimento(requerimento, atividade, titulo, modelo);

			if (lista != null && lista.Count > 0)
			{
				foreach (TituloEsp tit in lista)
				{
					Validacao.Add(Mensagem.Especificidade.AtividadeEstaAssociadaAOutroTitulo(tit.Numero.Texto, tit.Modelo, _da.ObterAtividadeNome(atividade)));
				}

				return false;
			}

			return true;
		}

		public bool DeclaratorioValidarModeloNaoRequisitado(int requerimento, int atividade, int modelo)
		{
			if (!_da.ValidarRequerimentoAtividadePossuiModelo(requerimento, atividade, modelo))
			{
				Validacao.Add(Mensagem.Especificidade.AtividadeComOutroModeloTitulo(_da.ObterAtividadeNome(atividade)));
				return false;
			}

			return true;
		}
	}
}