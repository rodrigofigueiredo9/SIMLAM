using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemPendencia.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemPendencia.Business
{
	public class ChecagemPendenciaValidar
	{
		#region Prorpiedades

		ChecagemPendenciaDa _da = new ChecagemPendenciaDa();
		TituloDa _TituloDa = new TituloDa();
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());

		#endregion

		internal bool VerificaTituloAssociado(int tituloId, int checagemPendenciaId = 0)
		{
			if (!_da.ExisteTitulo(tituloId))
			{
				Validacao.Add(Mensagem.ChecagemPendencia.TituloNaoEncontrado);
			} 
			else 
			{
				// verifica se este título já está associado á outra checagem de pendencia que não esta
				if (_da.TituloEstaAssociadoChecagemPendencia(tituloId, checagemPendenciaId))
				{
					Validacao.Add(Mensagem.ChecagemPendencia.TituloJaEstaAssociado);
					return false;
				}

				// verifica se modelo do título é do tipo configurado para ser usado em checagem de pendência
				List<Int32> lstCodigoModeloConfigurado = _configModelo.Obter<List<Int32>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia);
				TituloModeloLst modelo = _da.TituloObterTipoModelo(tituloId);
				
				if (!lstCodigoModeloConfigurado.Any(x => x == modelo.Codigo))
				{
					List<string> lstModelos = _da.ObterNomeModeloTitulo(lstCodigoModeloConfigurado);

					Validacao.Add(Mensagem.ChecagemPendencia.TituloDeveSerDoTipoConfigurado(lstModelos));
					return false;
				}

				Titulo titulo = _TituloDa.ObterSimplificado(tituloId);
				if (titulo.Situacao.Id != 3 && titulo.Situacao.Id != 6)//concluído/prorrogado
				{
					Validacao.Add(Mensagem.ChecagemPendencia.TituloDeveEstarConcluidoProrrogado);
				}
			}

			return Validacao.EhValido;
		}

		internal bool Salvar(ChecagemPendencia checagemPendencia)
		{
			VerificaTituloAssociado(checagemPendencia.TituloId, checagemPendencia.Id);

			VerificaChecagemPendenciaItens(checagemPendencia);

			foreach (ChecagemPendenciaItem item in checagemPendencia.Itens)
			{
				if (item.SituacaoId == 1)
				{
					Validacao.Add(Mensagem.ChecagemPendencia.SalvarChecagemItemNaoConferido);
					break;
				}
			}

			return Validacao.EhValido;
		}

		internal Resultados<ChecagemPendencia> Filtrar(Resultados<ChecagemPendencia> resultados)
		{
			if (resultados.Quantidade < 1)
			{
				Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
			}

			return resultados;
		}

		internal bool VerificaChecagemPendenciaItens(ChecagemPendencia checagemPendencia)
		{
			// verifica se há itens
			if (checagemPendencia.Itens == null || checagemPendencia.Itens.Count <= 0)
			{
				Validacao.Add(Mensagem.ChecagemPendencia.TituloSemItem);
			}

			return Validacao.EhValido;
		}

		internal bool VerificarExcluir(int id)
		{
			List<string> documentosComChecagem = _da.DocumentosComChecagem(id);

			foreach (string numeroDocumento in documentosComChecagem) 
			{
				Validacao.Add(Mensagem.ChecagemPendencia.ExcluirNaoPoisAssociadoDocumento(numeroDocumento));
			}

			return Validacao.EhValido;
		}
	}
}