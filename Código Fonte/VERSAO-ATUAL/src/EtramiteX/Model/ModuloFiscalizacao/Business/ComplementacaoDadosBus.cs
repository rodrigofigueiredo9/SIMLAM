using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ComplementacaoDadosBus
	{
		#region Propriedades

		ComplementacaoDadosValidar _validar = null;
		ComplementacaoDadosDa _da = new ComplementacaoDadosDa();

		#endregion

		#region Comandos DML

		public bool Salvar(ComplementacaoDados entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					if (entidade.Id < 1)
					{
						entidade.Id = _da.ObterID(entidade.FiscalizacaoId);
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(entidade, bancoDeDados);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public ComplementacaoDados Obter(int fiscalizacaoId, BancoDeDados banco = null) 
		{

			ComplementacaoDados entidade = null;
			try
			{
				entidade = _da.Obter(fiscalizacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		#endregion 

		#region Auxiliares

		public string ObterVinculoPropriedade(int responsavelId, int empreendimentoId) 
		{
			string[] vinculo = _da.ObterVinculoPropriedade(responsavelId, empreendimentoId).Split('@');

			int vinculoId = Convert.ToInt32(vinculo[0]);

			switch ((eEmpreendimentoResponsavelTipo)vinculoId)
			{
				case eEmpreendimentoResponsavelTipo.Proprietario:
					vinculoId = (int)eVinculoPropriedade.Proprietario;
					break;
				case eEmpreendimentoResponsavelTipo.Socio:
					vinculoId = (int)eVinculoPropriedade.Socio;
					break;
				case eEmpreendimentoResponsavelTipo.Arrendatario:
					vinculoId = (int)eVinculoPropriedade.Arrendatario;
					break;
				case eEmpreendimentoResponsavelTipo.RepresentanteLegal:
					vinculoId = (int)eVinculoPropriedade.RepresentanteLegal;
					break;
				case eEmpreendimentoResponsavelTipo.Herdeiro:
					vinculoId = (int)eVinculoPropriedade.Herdeiro;
					break;
				case eEmpreendimentoResponsavelTipo.Posseiro:
					vinculoId = (int)eVinculoPropriedade.Posseiro;
					break;
				case eEmpreendimentoResponsavelTipo.Empregado:
					vinculoId = (int)eVinculoPropriedade.Empregado;
					break;
				case eEmpreendimentoResponsavelTipo.Meeiro:
					vinculoId = (int)eVinculoPropriedade.Meeiro;
					break;
				case eEmpreendimentoResponsavelTipo.Outro:
					vinculoId = (int)eVinculoPropriedade.Outro;
					break;
				default:
					vinculoId = 0;
					break;
			}

			return vinculoId + "@" + vinculo[1];
		}

		#endregion

		public ComplementacaoDadosBus()
		{
			_validar = new ComplementacaoDadosValidar();
		}

		public ComplementacaoDadosBus(ComplementacaoDadosValidar validar)
		{
			_validar = validar;
		}
	}
}