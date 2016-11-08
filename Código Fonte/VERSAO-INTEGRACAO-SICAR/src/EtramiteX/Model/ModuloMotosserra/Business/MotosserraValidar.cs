using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloMotosserra.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloMotosserra.Business
{
	public class MotosserraValidar
	{
		MotosserraDa _da = new MotosserraDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		internal bool Salvar(Motosserra motosserra)
		{
			if (motosserra.PossuiRegistro)
			{
				if (motosserra.RegistroNumero <= 0)
				{
					Validacao.Add(Mensagem.Motosserra.RegistroNumeroObrigatorio);
				}

				if (_da.MotosserraCadastrada(motosserra.RegistroNumero, motosserra.Id) > 0)
				{
					Validacao.Add(Mensagem.Motosserra.RegistroNumeroCadastrado);
				}

				string ultimoNumeroCliente = _configSys.Obter<String>(ConfiguracaoSistema.KeyMotosserraNumeroRegistro);

				if (motosserra.RegistroNumero > Convert.ToInt32(ultimoNumeroCliente))
				{
					Validacao.Add(Mensagem.Motosserra.RegistroNumeroSuperiorSistema(ultimoNumeroCliente));
				}
			}

			if (string.IsNullOrWhiteSpace(motosserra.SerieNumero))
			{
				Validacao.Add(Mensagem.Motosserra.SerieNumeroObrigatorio);
			}
			else
			{

				motosserra.SerieNumero = motosserra.SerieNumero.Trim();

				if (_da.ExisteNumero(motosserra.SerieNumero, motosserra.Id))
				{
					Validacao.Add(Mensagem.Motosserra.SerieNumeroIndisponivel);
				}
			}

			if (string.IsNullOrWhiteSpace(motosserra.NotaFiscalNumero))
			{
				Validacao.Add(Mensagem.Motosserra.NotaFiscalNumeroObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(motosserra.Modelo))
			{
				Validacao.Add(Mensagem.Motosserra.ModeloObrigatorio);
			}

			if (motosserra.Proprietario.Id <= 0)
			{
				Validacao.Add(Mensagem.Motosserra.ProprietarioObrigatorio);
			}

			if (motosserra.Id > 0 && motosserra.SituacaoId <= 0)
			{
				Validacao.Add(Mensagem.Motosserra.SituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		internal bool AlterarSituacao(Motosserra motosserra)
		{
			Motosserra motosserraAtual = _da.Obter(motosserra.Id);

			if (motosserra.SituacaoId == (int)eMotosserraSituacao.Desativo)
			{
				if (motosserra.SituacaoId == motosserraAtual.SituacaoId)
				{
					Validacao.Add(Mensagem.Motosserra.SituacaoJaDesativo);

					return Validacao.EhValido;
				}

				List<String> numeroSituacoesTituloAssociado = _da.ValidarDesativarAssociadoTitulo(motosserra.Id);
				if (numeroSituacoesTituloAssociado.Count > 0)
				{
					List<String> numeros = new List<String>();
					List<String> situacoes = new List<String>();

					numeroSituacoesTituloAssociado.ForEach(x => {
						numeros.Add(x.Split('|')[0]);
						situacoes.Add(x.Split('|')[1]);
					});

					Validacao.Add(Mensagem.Motosserra.MotosserraNaoPodeDesativarAssociado(Mensagem.Concatenar(numeros), Mensagem.Concatenar(situacoes)));

				}
			}

			return Validacao.EhValido;

		}

		internal bool Verificar(String numero)
		{
			if (String.IsNullOrWhiteSpace(numero)) {
				Validacao.Add(Mensagem.Motosserra.SerieNumeroObrigatorio);
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			List<String> lstTitulos = _da.ValidarAssociadoTitulo(id);
			if (lstTitulos.Count > 0)
			{
				Validacao.Add(Mensagem.Motosserra.TitulosAssociados(lstTitulos));
			}

			return Validacao.EhValido;
		}
	}
}