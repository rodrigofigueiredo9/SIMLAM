using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class TermoCompromissoAmbientalValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		TermoCompromissoAmbientalDa _daEspecificidade = new TermoCompromissoAmbientalDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			TermoCompromissoAmbiental esp = especificidade as TermoCompromissoAmbiental;

			RequerimentoAtividade(esp, solicitado: false, atividadeAndamento: false);

			ValidarTituloGenericoAtividadeCaracterizacao(esp, eEspecificidade.TermoCompromissoAmbiental);

			#region Licenca

			if (esp.Licenca <= 0)
			{
				Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.TituloModeloObrigatorio);
			}
			else
			{
				Titulo(esp.Licenca);

				Destinatario(_daEspecificidade.ObterProtocolo(esp.Licenca), esp.Destinatario, "Termo_Destinatario");

				#region Representantes

				Pessoa destinatario = _daEspecificidade.ObterPessoa(esp.Destinatario);

				if (destinatario.IsJuridica)
				{
					if (esp.Representante <= 0)
					{
						Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.RepresentanteObrigatorio);
					}
					else
					{
						List<PessoaLst> representantes = _daEspecificidade.ObterRepresentantes(destinatario.Id);
						if (!representantes.Exists(x => x.Id == esp.Representante))
						{
							Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.RepresentanteDesassociado);
						}
					}
				}

				#endregion
			}

			#endregion

			if (String.IsNullOrWhiteSpace(esp.Descricao))
			{
				Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.DescricaoCompromissoAmbientalObrigatorio);
			}
			else
			{
				if (esp.Descricao.Length > 5000)
				{
					Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.DescricaoCompromissoAmbientalMaxLength);
				}
			}

			int empreendimento = _daEspecificidade.ObterEmpreendimentoPorProtocolo(esp.ProtocoloReq.Id);

			if (empreendimento <= 0)
			{
				Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.EmpreendimentoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool Titulo(int tituloId)
		{
			Titulo titulo = _daEspecificidade.ObterTitulo(tituloId);

			if (titulo.Modelo.Codigo != (int)eEspecificidade.LicencaAmbientalRegularizacao)
			{
				Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.TituloModeloInvalido);

				return Validacao.EhValido;
			}

			if (titulo.Situacao.Id != (int)eTituloSituacao.Concluido && titulo.Situacao.Id != (int)eTituloSituacao.Prorrogado)
			{
				Validacao.Add(Mensagem.TermoCompromissoAmbientalMsg.TituloSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}