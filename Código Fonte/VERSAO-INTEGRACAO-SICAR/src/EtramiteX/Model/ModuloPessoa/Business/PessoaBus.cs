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
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business
{
	public class PessoaBus
	{
		#region Propriedades

		IPessoaValidar _validar = null;

		PessoaDa _da = new PessoaDa();
		GerenciadorConfiguracao<ConfiguracaoPessoa> _configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
		GerenciadorConfiguracao<ConfiguracaoEndereco> _configEndereco = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		#region Ações de DML

		public PessoaBus(string Esquema = null)
		{
			_da = new PessoaDa(Esquema);
		}

		public PessoaBus(IPessoaValidar pessoaValidar, string Esquema = null)
		{
			_validar = pessoaValidar;
			_da = new PessoaDa(Esquema);
		}

		public void Criar(Pessoa pessoa, BancoDeDados banco = null)
		{
			_da.Criar(pessoa, banco);
		}

		public void Salvar(Pessoa pessoa, BancoDeDados banco, Executor executor = null)
		{
			_da.Salvar(pessoa, banco, executor);
		}

		public bool Salvar(Pessoa pessoa)
		{
			try
			{
				if (_validar.Salvar(pessoa))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Conjuge
						
						if (pessoa.Id > 0 && pessoa.IsFisica)
						{
							Pessoa pessoaBanco = _da.Obter(pessoa.Id);
							
							//Remover conjuge anterior
							if ((pessoaBanco.Fisica.ConjugeId ?? 0) > 0 && (pessoaBanco.Fisica.ConjugeId ?? 0) != (pessoa.Fisica.ConjugeId ?? 0))
							{
								//Volta estado Civil anterior [Default - 1 Solteiro(a)]
								int estadoCivil = _da.ObterEstadoCivilAnterior(pessoaBanco.Fisica.ConjugeId.GetValueOrDefault()) ?? 1;
								_da.AlterarEstadoCivil(pessoaBanco.Fisica.ConjugeId.GetValueOrDefault(), estadoCivil, bancoDeDados);
							}
						}

						//Alterar o estado civil do conjuge
						if (pessoa.IsFisica && (pessoa.Fisica.ConjugeId ?? 0) > 0)
						{
							_da.AlterarEstadoCivil(pessoa.Fisica.ConjugeId.GetValueOrDefault(), pessoa.Fisica.EstadoCivil.GetValueOrDefault(), bancoDeDados);
						}

						#endregion

						_da.Salvar(pessoa, bancoDeDados);
						
						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public Boolean Excluir(int id)
		{
			if (_validar.VerificarExcluirPessoa(id))
			{
				_da.Excluir(id);
				Validacao.Add(Mensagem.Pessoa.Excluir);
			}
			return Validacao.EhValido;
		}

		public Pessoa Importar(Pessoa pessoa, BancoDeDados banco = null, bool conjuge = false)
		{
			try
			{
				int id = pessoa.Id;
				int conj = (pessoa.IsFisica)? pessoa.Fisica.ConjugeId.GetValueOrDefault():0;
				PessoaCredenciadoBus _busPessoaCredenciado = new PessoaCredenciadoBus();
				List<Pessoa> representantes = pessoa.Juridica.Representantes;
				pessoa = _busPessoaCredenciado.Obter(pessoa.Id);
				pessoa.Juridica.Representantes = representantes;
				pessoa.Id = ObterId(pessoa.CPFCNPJ);
				pessoa.Endereco.Id = 0;
				pessoa.Fisica.ConjugeId = conj;

				if (conjuge)
				{
					conj = pessoa.Fisica.ConjugeId.GetValueOrDefault();
					pessoa.Fisica.ConjugeId = 0;
				}

				if (pessoa.Id == 0)
				{
					pessoa.Fisica.ConjugeId = 0;
					_da.Criar(pessoa, banco);
				}
				else
				{
					_da.Editar(pessoa, banco);
				}

				if (conjuge)
				{
					pessoa.Fisica.ConjugeId = conj;
				}

				pessoa.InternoId = pessoa.Id;
				pessoa.Id = id;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return pessoa;
		}

		public void AlterarConjuge(int id, BancoDeDados banco = null)
		{
			_da.AlterarConjuge(id, banco);
		}

		public void AlterarConjugeEstadoCivil(int id, int conjuge, BancoDeDados banco = null)
		{
			_da.AlterarConjugeEstadoCivil(id, conjuge, banco);
		}

		#endregion

		#region Obter/Filtrar

		public List<Pessoa> ObterPessoasRelacionadas(List<int> pessoas)
		{
			try
			{
				return _da.ObterPessoasRelacionadas(pessoas);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
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

		public Resultados<Pessoa> Filtrar(ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltro> filtro = new Filtro<ListarFiltro>(filtrosListar, paginacao);
				Resultados<Pessoa> resultados = _da.Filtrar(filtro);

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
				Validacao.AddErro(exc);				
			}

			return null;
		}

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

		public String ObterProfissao(int id)
		{
			return _da.ObterProfissao(id);
		}

		public Pessoa ObterHistorico(int id, string tid, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = _da.ObterHistorico(id, tid, banco, simplificado);

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

		public bool VerificarCriarCpfCnpj(Pessoa pessoa)
		{
			if (pessoa.IsFisica)
			{
				_validar.VerificarCriarCpf(pessoa);
			}
			else
			{
				_validar.VerificarCriarCnpj(pessoa);
			}

			return Validacao.EhValido;
		}

		public bool ExisteProfissao(int pessoa, BancoDeDados banco = null)
		{
			return _da.ExisteProfissao(pessoa, banco);
		}

		public bool ExisteEndereco(int pessoa, BancoDeDados banco = null)
		{
			return _da.ExisteEndereco(pessoa, banco);
		}

		public bool ValidarAssociarRepresentante(int pessoaId)
		{
			return _validar.ValidarAssociarRepresentante(pessoaId);
		}

		internal bool ValidarAssociarResponsavelTecnico(int id)
		{
			return _validar.ValidarAssociarResponsavelTecnico(id);
		}

		#endregion
	}
}