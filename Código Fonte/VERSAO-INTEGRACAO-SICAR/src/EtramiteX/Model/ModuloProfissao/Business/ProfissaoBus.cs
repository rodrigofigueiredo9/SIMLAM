using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProfissao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProfissao.Business
{
	public class ProfissaoBus
	{
		ProfissaoDa _da = new ProfissaoDa();
		ProfissaoValidar _validar = new ProfissaoValidar();

		#region Comandos DML's

		public bool Salvar(Profissao profissao)
		{
			try
			{
				profissao.Texto = DeixarApenasUmEspaco(profissao.Texto);

				if (_validar.Salvar(profissao))
				{
					GerenciadorTransacao.ObterIDAtual();
					
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(profissao, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.Profissao.Salvar);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion Comandos DML's

		#region Obter/Filtrar

		public Profissao Obter(int id)
		{
			try
			{
				return _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<Profissao> Filtrar(ProfissaoListarFiltros filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ProfissaoListarFiltros> filtro = new Filtro<ProfissaoListarFiltros>(filtrosListar, paginacao);
				Resultados<Profissao> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion Obter/Filtrar

		public string DeixarApenasUmEspaco(string texto)
		{
			bool isEspaco = false;

			for (int i = 0; i < texto.Length; i++)
			{
				isEspaco = (isEspaco && texto[i] == ' ');

				if (isEspaco)
				{
					texto = texto.Remove(i, 1);
					i--;
				}

				isEspaco = (texto[i] == ' ');
			}

			return texto.Trim();
		}
	}
}