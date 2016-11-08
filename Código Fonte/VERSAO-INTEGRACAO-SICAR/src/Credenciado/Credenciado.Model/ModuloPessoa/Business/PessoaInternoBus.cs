using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business
{
	public class PessoaInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		GerenciadorConfiguracao<ConfiguracaoPessoa> _configPessoa;
		IPessoaInternoValidar _validar;
		PessoaInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public PessoaInternoBus(string Esquema = null)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
			_da = new PessoaInternoDa(Esquema);
		}

		public PessoaInternoBus(IPessoaInternoValidar pessoaValidar, string Esquema = null)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
			_validar = pessoaValidar;
			_da = new PessoaInternoDa(UsuarioInterno);
		}

		#region Ações de DML

		public void Criar(Pessoa pessoa, BancoDeDados banco = null)
		{
			_da.Criar(pessoa, banco);
		}

		public void Salvar(Pessoa pessoa, BancoDeDados banco, Executor executor = null)
		{
			_da.Salvar(pessoa, banco, executor);
		}

		#endregion

		#region Obter/Filtrar

		public Pessoa Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = _da.Obter(id, banco, simplificado);
			
			if (pessoa.IsFisica)
			{
				if ((pessoa.Fisica.EstadoCivil ?? 0) > 0)
				{
					pessoa.Fisica.EstadoCivilTexto = _configPessoa.Obter<List<EstadoCivil>>(ConfiguracaoPessoa.KeyEstadosCivis).Single(x => x.Id == pessoa.Fisica.EstadoCivil).Texto;
				}
				if ((pessoa.Fisica.Sexo ?? 0) > 0)
				{
					pessoa.Fisica.SexoTexto = _configPessoa.Obter<List<Sexo>>(ConfiguracaoPessoa.KeySexos).Single(x => x.Id == pessoa.Fisica.Sexo).Texto;
				}
			}

			return pessoa;
		}

		public Pessoa Obter(String cpfCnpj, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = _da.Obter(cpfCnpj, banco, simplificado);

			if (pessoa != null && pessoa.Id > 0 && pessoa.IsFisica)
			{
				if ((pessoa.Fisica.EstadoCivil ?? 0) > 0)
				{
				pessoa.Fisica.EstadoCivilTexto = _configPessoa.Obter<List<EstadoCivil>>(ConfiguracaoPessoa.KeyEstadosCivis).Single(x => x.Id == pessoa.Fisica.EstadoCivil).Texto;
				}
				if ((pessoa.Fisica.Sexo ?? 0) > 0)
				{
				pessoa.Fisica.SexoTexto = _configPessoa.Obter<List<Sexo>>(ConfiguracaoPessoa.KeySexos).Single(x => x.Id == pessoa.Fisica.Sexo).Texto;
			}
			}

			return pessoa;
		}

		public Resultados<ProfissaoLst> FiltrarProfissao(ProfissaoFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ProfissaoFiltro> filtro = new Filtro<ProfissaoFiltro>(filtrosListar, paginacao);
				Resultados<ProfissaoLst> resultados = _da.FiltrarProfissao(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				throw exc;
			}
		}

		public int ObterId(String cpfCnpj, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterId(cpfCnpj, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		#endregion

		#region Validar

		public bool Existe(String cpfCnpj, BancoDeDados banco = null)
		{
			try
			{
				return _da.ExistePessoa(cpfCnpj, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		#endregion
	}
}