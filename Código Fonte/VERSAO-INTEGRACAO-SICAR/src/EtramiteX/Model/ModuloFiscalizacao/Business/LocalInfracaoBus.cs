using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class LocalInfracaoBus
	{
		#region Propriedades

		LocalInfracaoValidar _validar = null;
		LocalInfracaoDa _da = new LocalInfracaoDa();
		FiscalizacaoDa _fiscalizacaoDa = new FiscalizacaoDa();

		#endregion

		#region Comandos DML

		#endregion

		#region Obter

		public LocalInfracao Obter(int id, BancoDeDados banco = null) 
		{
			LocalInfracao entidade = null;
			try
			{
				entidade = _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		public LocalInfracao ObterHistorico(int fiscalizacaoId, BancoDeDados banco = null)
		{
			LocalInfracao entidade = null;
			try
			{
				entidade = _da.ObterHistorico(fiscalizacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}


		public List<PessoaLst> ObterResponsaveis(int empreendimentoId)
		{
			return _da.ObterResponsaveis(empreendimentoId);
		}

		public List<PessoaLst> ObterResponsaveisHistorico(int empreendimentoId, string empreendimentoTid)
		{
			return _da.ObterResponsaveisHistorico(empreendimentoId, empreendimentoTid);
		}

		public Pessoa ObterPessoaSimplificadaPorHistorico(int pessoa_id, string tid, BancoDeDados banco = null)
		{
			Pessoa entidade = null;
			try
			{
				entidade = _da.ObterPessoaSimplificadaPorHistorico(pessoa_id, tid);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		#endregion 

		public LocalInfracaoBus()
		{
			_validar = new LocalInfracaoValidar();
		}

		public LocalInfracaoBus(LocalInfracaoValidar validar)
		{
			_validar = validar;
		}
	}
}