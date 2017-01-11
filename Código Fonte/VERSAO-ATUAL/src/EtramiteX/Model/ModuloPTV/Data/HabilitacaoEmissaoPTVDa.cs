
using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Data;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Data
{
	public class HabilitacaoEmissaoPTVDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		#region Ações DML's

		public void Salvar(HabilitacaoEmissaoPTV habilitacao, BancoDeDados banco = null)
		{
			if (habilitacao.Id > 0)
			{
				Editar(habilitacao, banco);
			}
			else
			{
				Criar(habilitacao, banco);
			}
		}

		private void Criar(HabilitacaoEmissaoPTV habilitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				insert into tab_hab_emi_ptv (id, tid, situacao, funcionario, arquivo, numero_habilitacao, rg, numero_matricula, profissao, orgao_classe, registro_orgao_classe, uf_habilitacao, 
				numero_visto_crea, numero_crea, telefone_comercial, telefone_residencial, telefone_celular, cep, logradouro, bairro_gleba, estado, municipio, numero, distrito_localidade, complemento) 
				values (seq_tab_hab_emi_ptv.nextval, :tid, :situacao, :funcionario, :arquivo, :numero_habilitacao, :rg, :numero_matricula, :profissao, :orgao_classe, :registro_orgao_classe, :uf_habilitacao, 
				:numero_visto_crea, :numero_crea, :telefone_comercial, :telefone_residencial, :telefone_celular, :cep, :logradouro, :bairro_gleba, :estado, :municipio, :numero, :distrito_localidade, :complemento) 
				returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("situacao", habilitacao.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", habilitacao.Funcionario.Id > 0 ? habilitacao.Funcionario.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", habilitacao.Arquivo.Id > 0 ? habilitacao.Arquivo.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_habilitacao", habilitacao.NumeroHabilitacao, DbType.String);
				comando.AdicionarParametroEntrada("rg", habilitacao.RG, DbType.String);
				comando.AdicionarParametroEntrada("numero_matricula", habilitacao.NumeroMatricula, DbType.String);
				comando.AdicionarParametroEntrada("profissao", habilitacao.Profissao.Id > 0 ? habilitacao.Profissao.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("orgao_classe", habilitacao.Profissao.OrgaoClasseId > 0 ? habilitacao.Profissao.OrgaoClasseId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("registro_orgao_classe", habilitacao.Profissao.Registro, DbType.String);
				comando.AdicionarParametroEntrada("uf_habilitacao", habilitacao.EstadoRegistro > 0 ? habilitacao.EstadoRegistro : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_visto_crea", DbType.String, 12, habilitacao.NumeroVistoCREA);
				comando.AdicionarParametroEntrada("numero_crea", DbType.String, 12, habilitacao.NumeroCREA);
				comando.AdicionarParametroEntrada("telefone_comercial", habilitacao.TelefoneComercial, DbType.String);
				comando.AdicionarParametroEntrada("telefone_residencial", habilitacao.TelefoneResidencial, DbType.String);
				comando.AdicionarParametroEntrada("telefone_celular", habilitacao.TelefoneCelular, DbType.String);
				comando.AdicionarParametroEntrada("cep", habilitacao.Endereco.Cep, DbType.String);
				comando.AdicionarParametroEntrada("logradouro", habilitacao.Endereco.Logradouro, DbType.String);
				comando.AdicionarParametroEntrada("bairro_gleba", habilitacao.Endereco.Bairro, DbType.String);
				comando.AdicionarParametroEntrada("estado", habilitacao.Endereco.EstadoId > 0 ? habilitacao.Endereco.EstadoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", habilitacao.Endereco.MunicipioId > 0 ? habilitacao.Endereco.MunicipioId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", habilitacao.Endereco.Numero, DbType.String);
				comando.AdicionarParametroEntrada("distrito_localidade", habilitacao.Endereco.DistritoLocalizacao, DbType.String);
				comando.AdicionarParametroEntrada("complemento", habilitacao.Endereco.Complemento, DbType.String);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				habilitacao.Id = comando.ObterValorParametro<int>("id");

				comando = bancoDeDados.CriarComando(@"insert into tab_hab_emi_ptv_operador (id, tid, habilitacao, funcionario) 
				values (seq_tab_hab_emi_ptv_operador.nextval, :tid, :habilitacao, :funcionario)", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("habilitacao", habilitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", DbType.Int32);

				habilitacao.Operadores.ForEach(operador =>
				{
					comando.SetarValorParametro("funcionario", operador.FuncionarioId > 0 ? operador.FuncionarioId : (object)DBNull.Value);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				Historico.Gerar(habilitacao.Id, eHistoricoArtefato.habilitacaoemissaoptv, eHistoricoAcao.criar, bancoDeDados);
				bancoDeDados.Commit();
			}
		}

		private void Editar(HabilitacaoEmissaoPTV habilitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				update tab_hab_emi_ptv set tid = :tid, situacao = :situacao, funcionario = :funcionario, arquivo = :arquivo, numero_habilitacao = :numero_habilitacao, rg = :rg, numero_matricula = :numero_matricula, 
				profissao = :profissao, orgao_classe = :orgao_classe, registro_orgao_classe = :registro_orgao_classe, uf_habilitacao = :uf_habilitacao, numero_visto_crea = :numero_visto_crea, 
				numero_crea = :numero_crea, telefone_comercial = :telefone_comercial, telefone_residencial = :telefone_residencial, telefone_celular = :telefone_celular, cep = :cep, logradouro = :logradouro, 
				bairro_gleba = :bairro_gleba, estado = :estado, municipio = :municipio, numero = :numero, distrito_localidade = :distrito_localidade, complemento = :complemento 
				where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("situacao", habilitacao.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", habilitacao.Funcionario.Id > 0 ? habilitacao.Funcionario.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", habilitacao.Arquivo.Id > 0 ? habilitacao.Arquivo.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_habilitacao", habilitacao.NumeroHabilitacao, DbType.String);
				comando.AdicionarParametroEntrada("rg", habilitacao.RG, DbType.String);
				comando.AdicionarParametroEntrada("numero_matricula", habilitacao.NumeroMatricula, DbType.String);
				comando.AdicionarParametroEntrada("profissao", habilitacao.Profissao.Id > 0 ? habilitacao.Profissao.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("orgao_classe", habilitacao.Profissao.OrgaoClasseId > 0 ? habilitacao.Profissao.OrgaoClasseId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("registro_orgao_classe", habilitacao.Profissao.Registro, DbType.String);
				comando.AdicionarParametroEntrada("uf_habilitacao", habilitacao.EstadoRegistro > 0 ? habilitacao.EstadoRegistro : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_visto_crea", DbType.String, 12, habilitacao.NumeroVistoCREA);
				comando.AdicionarParametroEntrada("numero_crea", DbType.String, 12, habilitacao.NumeroCREA);
				comando.AdicionarParametroEntrada("telefone_comercial", habilitacao.TelefoneComercial, DbType.String);
				comando.AdicionarParametroEntrada("telefone_residencial", habilitacao.TelefoneResidencial, DbType.String);
				comando.AdicionarParametroEntrada("telefone_celular", habilitacao.TelefoneCelular, DbType.String);
				comando.AdicionarParametroEntrada("cep", habilitacao.Endereco.Cep, DbType.String);
				comando.AdicionarParametroEntrada("logradouro", habilitacao.Endereco.Logradouro, DbType.String);
				comando.AdicionarParametroEntrada("bairro_gleba", habilitacao.Endereco.Bairro, DbType.String);
				comando.AdicionarParametroEntrada("estado", habilitacao.Endereco.EstadoId > 0 ? habilitacao.Endereco.EstadoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", habilitacao.Endereco.MunicipioId > 0 ? habilitacao.Endereco.MunicipioId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", habilitacao.Endereco.Numero, DbType.String);
				comando.AdicionarParametroEntrada("distrito_localidade", habilitacao.Endereco.DistritoLocalizacao, DbType.String);
				comando.AdicionarParametroEntrada("complemento", habilitacao.Endereco.Complemento, DbType.String);
				comando.AdicionarParametroEntrada("id", habilitacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from tab_hab_emi_ptv_operador ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where habilitacao = :habilitacao {0}", comando.AdicionarNotIn("and", "id", DbType.Int32, habilitacao.Operadores.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("habilitacao", habilitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				habilitacao.Operadores.ForEach(operador =>
				{
					if (operador.Id <= 0)
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_hab_emi_ptv_operador (id, tid, habilitacao, funcionario) 
						values (seq_tab_hab_emi_ptv_operador.nextval, :tid, :habilitacao, :funcionario)", EsquemaBanco);
						comando.AdicionarParametroEntrada("habilitacao", habilitacao.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"update tab_hab_emi_ptv_operador set funcionario =:funcionario, tid =:tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", operador.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("funcionario", operador.FuncionarioId > 0 ? operador.FuncionarioId : (object)DBNull.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				});

				Historico.Gerar(habilitacao.Id, eHistoricoArtefato.habilitacaoemissaoptv, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(int id, int situacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"update tab_hab_emi_ptv set situacao = :situacao where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.habilitacaoemissaoptv, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public HabilitacaoEmissaoPTV Obter(int id)
		{
			HabilitacaoEmissaoPTV retorno = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.situacao, t.funcionario, f.nome funcionario_nome, f.cpf funcionario_cpf, t.arquivo, t.numero_habilitacao, 
				t.rg, t.numero_matricula, t.profissao, p.texto profissao_texto, t.orgao_classe, t.registro_orgao_classe, t.uf_habilitacao, t.numero_visto_crea, t.numero_crea, t.telefone_comercial, 
				t.telefone_residencial, t.telefone_celular, t.cep, t.logradouro, t.bairro_gleba, t.estado, t.municipio, t.numero, t.distrito_localidade, t.complemento 
				from tab_hab_emi_ptv t, tab_funcionario f, tab_profissao p where f.id = t.funcionario and p.id(+) = t.profissao and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno = new HabilitacaoEmissaoPTV();
						retorno.Id = id;
						retorno.Tid = reader.GetValue<string>("tid");
						retorno.SituacaoId = reader.GetValue<int>("situacao");
						retorno.Funcionario.Id = reader.GetValue<int>("funcionario");
						retorno.Funcionario.Nome = reader.GetValue<string>("funcionario_nome");
						retorno.Funcionario.Cpf = reader.GetValue<string>("funcionario_cpf");
						retorno.Arquivo.Id = reader.GetValue<int>("arquivo");
						retorno.NumeroHabilitacao = reader.GetValue<string>("numero_habilitacao");
						retorno.RG = reader.GetValue<string>("rg");
						retorno.NumeroMatricula = reader.GetValue<string>("numero_matricula");
						retorno.Profissao.Id = reader.GetValue<int>("profissao");
						retorno.Profissao.ProfissaoTexto = reader.GetValue<string>("profissao_texto");
						retorno.Profissao.OrgaoClasseId = reader.GetValue<int>("orgao_classe");
						retorno.Profissao.Registro = reader.GetValue<string>("registro_orgao_classe");
						retorno.EstadoRegistro = reader.GetValue<int>("uf_habilitacao");
						retorno.NumeroVistoCREA = reader.GetValue<string>("numero_visto_crea");
						retorno.NumeroCREA = reader.GetValue<string>("numero_crea");
						retorno.Endereco.Cep = reader.GetValue<string>("cep");
						retorno.Endereco.Logradouro = reader.GetValue<string>("logradouro");
						retorno.Endereco.Bairro = reader.GetValue<string>("bairro_gleba");
						retorno.Endereco.EstadoId = reader.GetValue<int>("estado");
						retorno.Endereco.MunicipioId = reader.GetValue<int>("municipio");
						retorno.Endereco.Numero = reader.GetValue<string>("numero");
						retorno.Endereco.Complemento = reader.GetValue<string>("complemento");
						retorno.Endereco.DistritoLocalizacao = reader.GetValue<string>("distrito_localidade");

						if (!string.IsNullOrEmpty(reader.GetValue<string>("telefone_comercial")))
						{
							retorno.Telefones.Add(new Contato() { TipoContato = eTipoContato.TelefoneComercial, Valor = reader.GetValue<string>("telefone_comercial") });
						}

						if (!string.IsNullOrEmpty(reader.GetValue<string>("telefone_residencial")))
						{
							retorno.Telefones.Add(new Contato() { TipoContato = eTipoContato.TelefoneResidencial, Valor = reader.GetValue<string>("telefone_residencial") });
						}

						if (!string.IsNullOrEmpty(reader.GetValue<string>("telefone_celular")))
						{
							retorno.Telefones.Add(new Contato() { TipoContato = eTipoContato.TelefoneCelular, Valor = reader.GetValue<string>("telefone_celular") });
						}
					}
					reader.Close();
				}

				retorno.Operadores = ObterOperadores(retorno.Id);
			}

			return retorno;
		}

		public List<HabilitacaoEmissaoPTVOperador> ObterOperadores(int habilitacaoId)
		{
			List<HabilitacaoEmissaoPTVOperador> retorno = new List<HabilitacaoEmissaoPTVOperador>();

			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select t.*, f.nome funcionario_nome from tab_hab_emi_ptv_operador t, tab_funcionario f where f.id = t.funcionario and habilitacao =:id");

				comando.AdicionarParametroEntrada("id", habilitacaoId, DbType.Int32);

				using (IDataReader reader = banco.ExecutarReader(comando))
				{
					HabilitacaoEmissaoPTVOperador operador = null;

					while (reader.Read())
					{
						operador = new HabilitacaoEmissaoPTVOperador();
						operador.Id = reader.GetValue<int>("id");
						operador.FuncionarioId = reader.GetValue<int>("funcionario");
						operador.FuncionarioNome = reader.GetValue<string>("funcionario_nome");
						operador.Tid = reader.GetValue<string>("tid");
						retorno.Add(operador);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal Resultados<HabilitacaoEmissaoPTVFiltro> Filtrar(Filtro<HabilitacaoEmissaoPTVFiltro> filtros)
		{
			Resultados<HabilitacaoEmissaoPTVFiltro> retorno = new Resultados<HabilitacaoEmissaoPTVFiltro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("f.nome", "funcionario", filtros.Dados.Funcionario, true, true);

				comandtxt += comando.FiltroAnd("f.cpf", "cpf", filtros.Dados.CPF);

				comandtxt += comando.FiltroAnd("t.numero_habilitacao", "numero_habilitacao", filtros.Dados.NumeroHabilitacao);

				comandtxt += comando.FiltroIn("t.funcionario", "select s.funcionario from tab_funcionario_setor s where s.setor = :setor", "setor", filtros.Dados.SetorId);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "funcionario", "numero_habilitacao", "situacao" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("funcionario");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format(@"select count(t.id) from tab_hab_emi_ptv t, tab_funcionario f where f.id = t.funcionario " + comandtxt);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select t.id, f.nome funcionario, f.cpf, t.numero_habilitacao, t.situacao 
				from tab_hab_emi_ptv t, tab_funcionario f where f.id = t.funcionario {0} {1}",
				comandtxt, DaHelper.Ordenar(colunas, ordenar));
				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					HabilitacaoEmissaoPTVFiltro habilitacao;
					while (reader.Read())
					{
						habilitacao = new HabilitacaoEmissaoPTVFiltro();
						habilitacao.Id = reader.GetValue<int>("id");
						habilitacao.Funcionario = reader.GetValue<string>("funcionario");
						habilitacao.CPF = reader.GetValue<string>("cpf");
						habilitacao.NumeroHabilitacao = reader.GetValue<string>("numero_habilitacao");
						habilitacao.SituacaoTexto = reader.GetValue<int>("situacao") == 1 ? "Ativo" : "Inativo";
						retorno.Itens.Add(habilitacao);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		internal bool CPFAssociadoFuncionario(string cpf)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando("select count(*) from tab_funcionario where cpf =:cpf");
				comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);

				return banco.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool FuncionarioDesabilitado(string cpf)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando("select count(*) from tab_hab_emi_ptv t, tab_funcionario f where f.id = t.funcionario and f.cpf =:cpf");
				comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);

				return banco.ExecutarScalar<int>(comando) <= 0;
			}
		}

		internal bool FuncionarioHabilitadoValido(int funcionario)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select sum(total) from (select count(*) total from tab_hab_emi_ptv t where t.situacao = 1 and t.funcionario = :funcionario
					union all
					select count(*) total from tab_hab_emi_ptv t, tab_hab_emi_ptv_operador o where t.situacao = 1 and t.id = o.habilitacao and o.funcionario = :funcionario )");
				
				comando.AdicionarParametroEntrada("funcionario", funcionario, DbType.Int32);

				return banco.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool ExisteNumeroHabilitacao(string n_habilitacao, int id_funcionario)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(*) from tab_hab_emi_ptv t where t.numero_habilitacao = :n_habilitacao and t.funcionario <> :id_funcionario");
				comando.AdicionarParametroEntrada("n_habilitacao", n_habilitacao, DbType.String);
				comando.AdicionarParametroEntrada("id_funcionario", id_funcionario, DbType.Int32);

				return (banco.ExecutarScalar<int>(comando) > 0);
			}
		}

		//		internal bool ExisteOperador(int id, out string nome)
		//		{
		//			nome = "";
		//			int retorno = 0;
		//			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
		//			{
		//				//Comando comando = banco.CriarComando(@"select count(*) from tab_hab_emi_ptv_operador o, tab_hab_emi_ptv e
		//				//									   where e.id = o.habilitacao(+) and  (o.funcionario = :id or e.funcionario = :id)");
		//				//return (banco.ExecutarScalar<int>(comando) > 0);

		//				Comando comando = banco.CriarComando(@"
		//				select f.nome from tab_hab_emi_ptv_operador o, tab_hab_emi_ptv e, tab_funcionario f 
		//				where e.id = o.habilitacao(+) and f.id = e.funcionario and (o.funcionario = :id or e.funcionario = :id)", EsquemaBanco);

		//				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

		//				using (IDataReader reader = banco.ExecutarReader(comando))
		//				{ 
		//					if(reader.Read())
		//					{
		//						nome = reader.GetValue<string>("nome");
		//						retorno = 1;
		//					}			

		//					reader.Close();
		//				}

		//				return (retorno > 0);
		//			}
		//		}

		internal string ExisteOperador(int id)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"
				select f.nome from tab_hab_emi_ptv_operador o, tab_hab_emi_ptv e, tab_funcionario f 
				where e.id = o.habilitacao(+) and f.id = e.funcionario and (o.funcionario = :id or e.funcionario = :id)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return banco.ExecutarScalar<string>(comando);
			}
		}

		#endregion
	}
}