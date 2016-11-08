using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloModeloValidacao
	{
		TituloModeloDa _da = new TituloModeloDa();

		public bool Salvar(TituloModelo tituloModelo)
		{
			TituloModeloResposta resposta = null;

			if (tituloModelo.Tipo < 1)
			{
				Validacao.Add(Mensagem.TituloModelo.TipoObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(tituloModelo.Nome))
			{
				Validacao.Add(Mensagem.TituloModelo.NomeObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(tituloModelo.Sigla))
			{
				Validacao.Add(Mensagem.TituloModelo.SiglaObrigatorio);
			}

			if (tituloModelo.TipoProtocolo < 1)
			{
				Validacao.Add(Mensagem.TituloModelo.TipoProtocoloObrigatorio);
			}
			else
			{
				if (tituloModelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.TituloDeclaratorio && tituloModelo.TipoProtocolo != 4)//Nao possui
				{
					Validacao.Add(Mensagem.TituloModelo.DeclaratorioTipoProtocolo);
				}
			}

			if (tituloModelo.Setores.Count < 1)
			{
				Validacao.Add(Mensagem.TituloModelo.SetoresObrigatorio);
			}

			if (tituloModelo.Regra(eRegra.PdfGeradoSistema))
			{
				if (tituloModelo.Arquivo == null || tituloModelo.Arquivo.Id == null)
				{
					Validacao.Add(Mensagem.TituloModelo.ArquivoObrigatorio);
				}
			}

			if (tituloModelo.Regra(eRegra.Prazo))
			{
				resposta = tituloModelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo);
				if (resposta == null || resposta.Valor == null || Convert.ToInt32(resposta.Valor) < 1)
				{
					Validacao.Add(Mensagem.TituloModelo.InicioPrazoObrigatorio);
				}

				resposta = tituloModelo.Resposta(eRegra.Prazo, eResposta.TipoPrazo);
				if (resposta == null || resposta.Valor == null || Convert.ToInt32(resposta.Valor) < 1)
				{
					Validacao.Add(Mensagem.TituloModelo.TipoPrazoObrigatorio);
				}
			}

			if (tituloModelo.Regra(eRegra.EnviarEmail))
			{
				resposta = tituloModelo.Resposta(eRegra.EnviarEmail, eResposta.TextoEmail);

				if (resposta == null || resposta.Valor == null || string.IsNullOrWhiteSpace(resposta.Valor.ToString()))
				{
					Validacao.Add(Mensagem.TituloModelo.TextoEmailObrigatorio);
				}
				else
				{
					if (resposta.Valor.ToString().Length > 500)
					{
						Validacao.Add(Mensagem.TituloModelo.TextoEmailTamanhoMaximo);
					}
				}
			}

			if (tituloModelo.Regra(eRegra.FaseAnterior))
			{
				List<TituloModeloResposta> respostas = tituloModelo.Respostas(eRegra.FaseAnterior, eResposta.Modelo);

				if (respostas.Count < 1)
				{
					Validacao.Add(Mensagem.TituloModelo.ModeloAnteriorObrigatorio);
				}
				else
				{
					respostas.ForEach(x =>
					{
						int count = 0;

						respostas.ForEach(y => { if (x.Valor.ToString() == y.Valor.ToString()) count++; });

						if (count > 1) { Validacao.Add(Mensagem.TituloModelo.ModeloAnteriorDuplicado); }
					});
				}
			}

			return Validacao.EhValido;
		}

		internal void VerificarPublicoExternoAtividade(int id)
		{
			string nomesGrupo = _da.VerificarPublicoExternoAtividade(id);

			if (!string.IsNullOrWhiteSpace(nomesGrupo))
			{
				Validacao.Add(Mensagem.TituloModelo.ModeloAssociadoAtividade(nomesGrupo));
			}
		}

		internal bool PossuiConfiguracaoAtividade(TituloModelo modelo)
		{
			if (_da.PossuiConfiguracaoAtividade(modelo))
			{
				Validacao.Add(Mensagem.TituloModelo.TituloConfiguradoAtividade);
			}
			return Validacao.EhValido;
		}
	}
}