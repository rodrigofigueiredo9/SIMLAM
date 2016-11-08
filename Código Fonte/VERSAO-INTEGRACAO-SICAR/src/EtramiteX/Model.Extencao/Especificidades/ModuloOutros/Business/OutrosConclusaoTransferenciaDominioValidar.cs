using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosConclusaoTransferenciaDominioValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		EspecificidadeDa _daEspecificidade = new EspecificidadeDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			OutrosConclusaoTransferenciaDominio esp = especificidade as OutrosConclusaoTransferenciaDominio;

			if (esp.Destinatarios.Count < 1)
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioObrigatorio("ddlDestinatarios"));
			}

			if (esp.Responsaveis.Count < 1)
			{
				Validacao.Add(Mensagem.Especificidade.ResponsavelObrigatorio("ddlResponsaveis"));
			}

			if (esp.Interessados.Count < 1)
			{
				Validacao.Add(Mensagem.Especificidade.InteressadoObrigatorio("ddlInteressados"));
			}

			List<PessoaEspecificidade> lstValida = null;
			esp.Destinatarios.ForEach(x =>
			{
				lstValida = new List<PessoaEspecificidade>();
				lstValida.AddRange(esp.Destinatarios.Where(y => x.Id == y.Id).ToList());
				if (lstValida.Count > 1)
				{
					Validacao.Add(Mensagem.Especificidade.DestinatarioJaAdicionado);
				}
			});

			esp.Interessados.ForEach(x =>
			{
				lstValida = new List<PessoaEspecificidade>();
				lstValida.AddRange(esp.Interessados.Where(y => x.Id == y.Id).ToList());
				if (lstValida.Count > 1)
				{
					Validacao.Add(Mensagem.Especificidade.InteressadoJaAdicionado);
				}
			});

			esp.Responsaveis.ForEach(x =>
			{
				lstValida = new List<PessoaEspecificidade>();
				lstValida.AddRange(esp.Responsaveis.Where(y => x.Id == y.Id).ToList());
				if (lstValida.Count > 1)
				{
					Validacao.Add(Mensagem.Especificidade.ResponsavelJaAdicionado);
				}
			});

			foreach (PessoaEspecificidade pessoa in esp.Responsaveis)
			{
				for (int i = 0; i < esp.Interessados.Count; i++)
				{
					if (esp.Interessados[i].Id == pessoa.Id)
					{
						Validacao.Add(new EspecificidadeMsg().InteressadoIgualResponsavel);
						return Validacao.EhValido;
					}
				}
			}

			#region Verificar lista de pessoas desassociadas

			List<PessoaEspecificidade> destinatarios = PessoasDesassociadas(esp.Destinatarios, _daEspecificidade.ObterInteressados(esp.ProtocoloReq.Id));
			List<PessoaEspecificidade> responsaveis = PessoasDesassociadas(esp.Responsaveis, _daEspecificidade.ObterEmpreendimentoResponsaveis(esp.Titulo.EmpreendimentoId.GetValueOrDefault()));
			List<PessoaEspecificidade> interessados = PessoasDesassociadas(esp.Interessados, _daEspecificidade.ObterInteressados(esp.ProtocoloReq.Id));

			if (destinatarios.Count > 0)
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioNaoAssociadoMais(Mensagem.Concatenar(destinatarios.Select(x => x.Nome).ToList())));
			}

			if (responsaveis.Count > 0)
			{
				Validacao.Add(Mensagem.Especificidade.ResponsavelNaoAssociadoMais(Mensagem.Concatenar(responsaveis.Select(x => x.Nome).ToList())));
			}

			if (interessados.Count > 0)
			{
				Validacao.Add(Mensagem.Especificidade.InteressadoNaoAssociadoMais(Mensagem.Concatenar(interessados.Select(x => x.Nome).ToList())));
			}

			#endregion

			RequerimentoAtividade(esp);

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}

		public List<PessoaEspecificidade> PessoasDesassociadas(List<PessoaEspecificidade> entidadesTela, List<PessoaLst> entidadesAtuais)
		{
			List<PessoaEspecificidade> pessoas = new List<PessoaEspecificidade>();
			entidadesTela.ForEach(entidadeTela =>
			{
				if (!entidadesAtuais.Exists(x => x.Id == entidadeTela.Id))
				{
					pessoas.Add(entidadeTela);
				}
			});

			return pessoas;
		}
	}
}