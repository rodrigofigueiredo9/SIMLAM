using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class EnquadramentoBus
	{
		#region Propriedades

		EnquadramentoValidar _validar = null;
		EnquadramentoDa _da = new EnquadramentoDa();

		#endregion

		#region Comandos DML

		public bool Salvar(Enquadramento entidade)
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

		public Enquadramento Obter(int fiscalizacaoId, BancoDeDados banco = null) 
		{

			Enquadramento entidade = null;
			try
			{
				entidade = _da.Obter(fiscalizacaoId);

				if (entidade.Id <= 0) 
				{
					entidade.Artigos.Add(new Artigo() { Identificador = Guid.NewGuid().ToString() });
					entidade.Artigos.Add(new Artigo() { Identificador = Guid.NewGuid().ToString() });
					entidade.Artigos.Add(new Artigo() { Identificador = Guid.NewGuid().ToString() });
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		#endregion 

		public EnquadramentoBus()
		{
			_validar = new EnquadramentoValidar();
		}

		public EnquadramentoBus(EnquadramentoValidar validar)
		{
			_validar = validar;
		}
	}
}