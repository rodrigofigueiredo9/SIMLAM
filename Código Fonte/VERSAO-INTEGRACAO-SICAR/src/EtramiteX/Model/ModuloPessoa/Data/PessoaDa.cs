using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Data
{
	public class PessoaDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }
		
		Consulta _consulta = new Consulta();
		internal Consulta Consulta { get { return _consulta; } }
		
		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}
		
		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public PessoaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal int Criar(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Pessoa

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa a
																	(id, ativa, tipo, nome, apelido, cpf, rg, 
																	estado_civil, sexo, nacionalidade, naturalidade,
																	data_nascimento, mae, pai, cnpj, razao_social,
																	nome_fantasia, ie, tid)
																values
																	({0}seq_pessoa.nextval,
																	:ativa, :tipo, :nome, :apelido, :cpf, :rg,
																	:estado_civil, :sexo, :nacionalidade, :naturalidade,
																	:data_nascimento, :mae, :pai, :cnpj, :razao_social,
																	:nome_fantasia, :ie, :tid)
																returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("ativa", pessoa.Ativa, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", pessoa.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, pessoa.Fisica.Nome);
				comando.AdicionarParametroEntrada("apelido", DbType.String, 80, pessoa.Fisica.Apelido);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 15, pessoa.Fisica.CPF);
				comando.AdicionarParametroEntrada("rg", DbType.String, 30, pessoa.Fisica.RG);
				comando.AdicionarParametroEntrada("estado_civil", (pessoa.Fisica.EstadoCivil.HasValue && pessoa.Fisica.EstadoCivil > 0) ? pessoa.Fisica.EstadoCivil : null, DbType.Int32);
				comando.AdicionarParametroEntrada("sexo", (pessoa.Fisica.Sexo.HasValue && pessoa.Fisica.Sexo > 0) ? pessoa.Fisica.Sexo : null, DbType.Int32);
				comando.AdicionarParametroEntrada("nacionalidade", DbType.String, 80, pessoa.Fisica.Nacionalidade);
				comando.AdicionarParametroEntrada("naturalidade", DbType.String, 80, pessoa.Fisica.Naturalidade);
				comando.AdicionarParametroEntrada("mae", DbType.String, 80, pessoa.Fisica.NomeMae);
				comando.AdicionarParametroEntrada("pai", DbType.String, 80, pessoa.Fisica.NomePai);
				comando.AdicionarParametroEntrada("data_nascimento", (pessoa.Fisica.DataNascimento.HasValue && pessoa.Fisica.DataNascimento != DateTime.MinValue) ? pessoa.Fisica.DataNascimento : null, DbType.DateTime);
				comando.AdicionarParametroEntrada("cnpj", DbType.String, 20, pessoa.Juridica.CNPJ);
				comando.AdicionarParametroEntrada("razao_social", DbType.String, 80, pessoa.Juridica.RazaoSocial);
				comando.AdicionarParametroEntrada("nome_fantasia", DbType.String, 80, pessoa.Juridica.NomeFantasia);
				comando.AdicionarParametroEntrada("ie", DbType.String, 80, pessoa.Juridica.IE);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				pessoa.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Meios de contato -  Ambos

				if (pessoa.MeiosContatos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_meio_contato(id, pessoa,meio_contato, valor, tid) values
					({0}seq_pessoa_meio_contato.nextval, :pessoa, :meio_contato, :valor, : tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("meio_contato", DbType.Int32);
					comando.AdicionarParametroEntrada("valor", DbType.String, 1000);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Contato item in pessoa.MeiosContatos)
					{
						comando.SetarValorParametro("meio_contato", Convert.ToInt32(item.TipoContato));
						comando.SetarValorParametro("valor", item.Valor);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Endereço - Ambos

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_endereco (id, pessoa, cep, logradouro, bairro, estado, municipio, numero, distrito, complemento, tid)
				values({0}seq_pessoa_endereco.nextval, :pessoa, :cep, :logradouro, :bairro, :estado, :municipio, :numero, :distrito, :complemento, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("cep", DbType.String, 15, pessoa.Endereco.Cep);
				comando.AdicionarParametroEntrada("logradouro", DbType.String, 500, pessoa.Endereco.Logradouro);
				comando.AdicionarParametroEntrada("bairro", DbType.String, 100, pessoa.Endereco.Bairro);

				if (pessoa.Endereco.EstadoId > 0)
				{
					comando.AdicionarParametroEntrada("estado", pessoa.Endereco.EstadoId, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("estado", null, DbType.Int32);
				}

				if (pessoa.Endereco.MunicipioId > 0)
				{
					comando.AdicionarParametroEntrada("municipio", pessoa.Endereco.MunicipioId, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("municipio", null, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("numero", DbType.String, 6, pessoa.Endereco.Numero);
				comando.AdicionarParametroEntrada("distrito", DbType.String, 100, pessoa.Endereco.DistritoLocalizacao);
				comando.AdicionarParametroEntrada("complemento", DbType.String, 80, pessoa.Endereco.Complemento);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Profissão - Física

				if (pessoa.IsFisica && pessoa.Fisica.Profissao.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_profissao (id, pessoa, profissao, orgao_classe, registro, tid) values
					({0}seq_pessoa_profissao.nextval, :pessoa, :profissao, :orgao_classe, :registro, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("profissao", pessoa.Fisica.Profissao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("orgao_classe", (pessoa.Fisica.Profissao.OrgaoClasseId.HasValue && pessoa.Fisica.Profissao.OrgaoClasseId > 0) ? pessoa.Fisica.Profissao.OrgaoClasseId : null, DbType.Int32);
					comando.AdicionarParametroEntrada("registro", DbType.String, 15, pessoa.Fisica.Profissao.Registro);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Cônjuge - Física

				if (pessoa.IsFisica && pessoa.Fisica.ConjugeId > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_conjuge (id, pessoa, conjuge, tid)
					values({0}seq_pessoa_conjuge.nextval, :pessoa, :conjuge, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("conjuge", pessoa.Fisica.ConjugeId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);					
				}

				#endregion

				#region Representantes - Jurídica

				if (pessoa.IsJuridica && pessoa.Juridica.Representantes.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_representante(id, pessoa, representante, tid)
					values({0}seq_pessoa_representante.nextval, :pessoa, :representante, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("representante", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Pessoa item in pessoa.Juridica.Representantes)
					{
						comando.SetarValorParametro("representante", item.Id);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico e Consulta

				Historico.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, eHistoricoAcao.criar, bancoDeDados, executor);
				Consulta.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return pessoa.Id;
			}
		}

		internal void Editar(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Pessoa

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
									update {0}tab_pessoa p
										set p.tipo            = :tipo,
											p.nome            = :nome,
											p.apelido         = :apelido,
											p.cpf             = :cpf,
											p.rg              = :rg,
											p.estado_civil    = :estado_civil,
											p.sexo            = :sexo,
											p.nacionalidade   = :nacionalidade,
											p.naturalidade    = :naturalidade,
											p.data_nascimento = :data_nascimento,
											p.mae             = :mae,
											p.pai             = :pai,
											p.cnpj            = :cnpj,
											p.razao_social    = :razao_social,
											p.nome_fantasia   = :nome_fantasia,
											p.ie              = :ie,
											p.tid             = :tid
										where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", pessoa.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, pessoa.Fisica.Nome);
				comando.AdicionarParametroEntrada("apelido", DbType.String, 80, pessoa.Fisica.Apelido);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 15, pessoa.Fisica.CPF);
				comando.AdicionarParametroEntrada("rg", DbType.String, 30, pessoa.Fisica.RG);
				comando.AdicionarParametroEntrada("estado_civil", (pessoa.Fisica.EstadoCivil.HasValue && pessoa.Fisica.EstadoCivil > 0) ? pessoa.Fisica.EstadoCivil : null, DbType.Int32);
				comando.AdicionarParametroEntrada("sexo", (pessoa.Fisica.Sexo.HasValue && pessoa.Fisica.Sexo > 0) ? pessoa.Fisica.Sexo : null, DbType.Int32);
				comando.AdicionarParametroEntrada("nacionalidade", DbType.String, 80, pessoa.Fisica.Nacionalidade);
				comando.AdicionarParametroEntrada("naturalidade", DbType.String, 80, pessoa.Fisica.Naturalidade);
				comando.AdicionarParametroEntrada("data_nascimento", (pessoa.Fisica.DataNascimento.HasValue && pessoa.Fisica.DataNascimento != DateTime.MinValue) ? pessoa.Fisica.DataNascimento : null, DbType.DateTime);
				comando.AdicionarParametroEntrada("mae", DbType.String, 80, pessoa.Fisica.NomeMae);
				comando.AdicionarParametroEntrada("pai", DbType.String, 80, pessoa.Fisica.NomePai);
				comando.AdicionarParametroEntrada("cnpj", DbType.String, 20, pessoa.Juridica.CNPJ);
				comando.AdicionarParametroEntrada("razao_social", DbType.String, 80, pessoa.Juridica.RazaoSocial);
				comando.AdicionarParametroEntrada("nome_fantasia", DbType.String, 80, pessoa.Juridica.NomeFantasia);
				comando.AdicionarParametroEntrada("ie", DbType.String, 80, pessoa.Juridica.IE);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco
				//Meios de Contato
				comando = bancoDeDados.CriarComando("delete from {0}tab_pessoa_meio_contato c where c.pessoa = :pessoa", EsquemaBanco);
				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Representante
				comando = bancoDeDados.CriarComando("delete from {0}tab_pessoa_representante c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.pessoa = :pessoa{0}",
				comando.AdicionarNotIn("and", "c.representante", DbType.Int32, pessoa.Juridica.Representantes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Profissao
				comando = bancoDeDados.CriarComando(@"delete {0}tab_pessoa_profissao p where p.pessoa = :id and p.profissao != :profissao ", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("profissao", pessoa.Fisica.Profissao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Endereço
				comando = bancoDeDados.CriarComando(@"delete {0}tab_pessoa_endereco p where p.pessoa = :id and p.id != :endereco ", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("endereco", pessoa.Endereco.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Meios de contato - Ambos

				if (pessoa.MeiosContatos.Count > 0)
				{
					foreach (Contato item in pessoa.MeiosContatos)
					{
						
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_meio_contato(id, pessoa, meio_contato, valor, tid) values
						({0}seq_pessoa_meio_contato.nextval, :pessoa, :meio_contato, :valor, : tid)", EsquemaBanco);						

						comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("meio_contato", item.TipoContato, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", DbType.String, 1000, item.Valor);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Endereço - Ambos

				if (pessoa.Endereco.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa_endereco e set e.pessoa = :pessoa, e.cep = :cep, e.logradouro = :logradouro, e.bairro = :bairro, 
					e.estado = :estado, e.municipio = :municipio, e.numero = :numero, e.distrito = :distrito, e.complemento = :complemento, e.tid =:tid where e.id = :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", pessoa.Endereco.Id, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_endereco (id, pessoa, cep, logradouro, bairro, estado, municipio, numero, distrito, complemento, tid)
					values({0}seq_pessoa_endereco.nextval, :pessoa, :cep, :logradouro, :bairro, :estado, :municipio, :numero, :distrito, :complemento, :tid )", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("cep", DbType.String, 15, pessoa.Endereco.Cep);
				comando.AdicionarParametroEntrada("logradouro", DbType.String, 500, pessoa.Endereco.Logradouro);
				comando.AdicionarParametroEntrada("bairro", DbType.String, 100, pessoa.Endereco.Bairro);
				comando.AdicionarParametroEntrada("estado", DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", DbType.Int32);

				if (pessoa.Endereco != null && pessoa.Endereco.EstadoId > 0)
				{
					comando.SetarValorParametro("estado", pessoa.Endereco.EstadoId);
				}

				if (pessoa.Endereco != null && pessoa.Endereco.MunicipioId > 0)
				{
					comando.SetarValorParametro("municipio", pessoa.Endereco.MunicipioId);
				}

				comando.AdicionarParametroEntrada("numero", DbType.String, 6, pessoa.Endereco.Numero);
				comando.AdicionarParametroEntrada("distrito", DbType.String, 100, pessoa.Endereco.DistritoLocalizacao);
				comando.AdicionarParametroEntrada("complemento", DbType.String, 80, pessoa.Endereco.Complemento);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Profissão - Física

				if (pessoa.IsFisica && pessoa.Fisica.Profissao.Id > 0)
				{
					object pro = 0;
					comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_pessoa_profissao p where p.pessoa = :pessoa and p.profissao = :profissao", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("profissao", pessoa.Fisica.Profissao.Id, DbType.Int32);

					pro = bancoDeDados.ExecutarScalar(comando);

					if (pro != null && !Convert.IsDBNull(pro))
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa_profissao p set p.pessoa = :pessoa, p.profissao =:profissao,
						p.orgao_classe =:orgao_classe, p.registro =:registro, p.tid=:tid where p.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", pro, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_profissao(id, pessoa, profissao, orgao_classe, registro, tid) 
						(select {0}seq_pessoa_profissao.nextval, :pessoa, :profissao, :orgao_classe, :registro, :tid from dual where not exists
						(select id from {0}tab_pessoa_profissao r where r.pessoa = :pessoa and r.profissao = :profissao))", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("profissao", pessoa.Fisica.Profissao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("orgao_classe", (pessoa.Fisica.Profissao.OrgaoClasseId.HasValue && pessoa.Fisica.Profissao.OrgaoClasseId > 0) ? pessoa.Fisica.Profissao.OrgaoClasseId : null, DbType.Int32);
					comando.AdicionarParametroEntrada("registro", DbType.String, 15, pessoa.Fisica.Profissao.Registro);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarScalar(comando);
				}

				#endregion

				#region Cônjuge - Física

				if (pessoa.IsFisica && pessoa.Fisica.ConjugeId > 0)
				{
					comando = bancoDeDados.CriarComando(@"select id from {0}tab_pessoa_conjuge r where r.pessoa = :pessoa or r.conjuge = :pessoa", EsquemaBanco);
					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					object aux = bancoDeDados.ExecutarScalar(comando);

					if (aux != null && !Convert.IsDBNull(aux))
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa_conjuge t set t.pessoa = :pessoa, t.conjuge = :conjuge, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", aux, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_conjuge(id, pessoa, conjuge, tid) values 
						({0}seq_pessoa_conjuge.nextval, :pessoa, :conjuge, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("conjuge", pessoa.Fisica.ConjugeId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"begin delete {0}tab_pessoa_conjuge p where p.pessoa = :id; delete {0}tab_pessoa_conjuge p where p.conjuge = :id; end;", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", pessoa.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Representantes - Jurídica

				if (pessoa.IsJuridica && pessoa.Juridica.Representantes.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_representante(id, pessoa, representante, tid) 
					(select {0}seq_pessoa_representante.nextval, :pessoa, :representante, :tid from dual where not exists
					(select id from {0}tab_pessoa_representante r where r.pessoa = :pessoa and r.representante = :representante))", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("representante", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Pessoa item in pessoa.Juridica.Representantes)
					{
						comando.SetarValorParametro("representante", item.Id);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete {0}tab_pessoa_representante p where p.pessoa = :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", pessoa.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Histórico e Consulta

				Historico.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, eHistoricoAcao.atualizar, bancoDeDados, executor);
				Consulta.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void Salvar(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			if (pessoa == null)
			{
				throw new Exception("Pessoa é nula.");
			}

			if (pessoa.Id <= 0)
			{
				Criar(pessoa, banco, executor);
			}
			else
			{
				Editar(pessoa, banco, executor);
			}
		}

		internal void Excluir(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação
				
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico / Consulta

				Historico.Gerar(id, eHistoricoArtefato.pessoa, eHistoricoAcao.excluir, bancoDeDados);
				Consulta.Deletar(id, eHistoricoArtefato.pessoa, bancoDeDados);

				#endregion

				List<String> listaDeletes = new List<String>();
				listaDeletes.Add("delete {0}tab_pessoa_conjuge a where a.pessoa = :pessoa;");
				listaDeletes.Add("delete {0}tab_pessoa_representante a where a.pessoa = :pessoa;");
				listaDeletes.Add("delete {0}tab_pessoa_profissao a where a.pessoa = :pessoa;");
				listaDeletes.Add("delete {0}tab_pessoa_endereco a where a.pessoa = :pessoa;");
				listaDeletes.Add("delete {0}tab_pessoa_meio_contato a where a.pessoa = :pessoa;");
				listaDeletes.Add("delete {0}tab_pessoa a where a.id = :pessoa;");

				#region Apaga os dados de pessoa
				comando = bancoDeDados.CriarComando("begin " + string.Join(" ", listaDeletes) + " end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("pessoa", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
				#endregion
			}
		}

		internal void AlterarEstadoCivil(int id, int estadoCivilId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa p set p.estado_civil = :estadoCivil, p.tid = :tid where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("estadoCivil", estadoCivilId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Histórico e Consulta
				Historico.Gerar(id, eHistoricoArtefato.pessoa, eHistoricoAcao.alterarestadocivil, bancoDeDados);
				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarConjuge(int id, BancoDeDados banco = null)
		{
			int conjugeId = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
					select c.conjuge conjugeId from {0}tab_pessoa_conjuge c where c.pessoa = :id 
					union all 
					select c.pessoa conjugeId from {0}tab_pessoa_conjuge c where c.conjuge =: id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarReader(comando, (reader) => {
					conjugeId = Convert.ToInt32(reader["conjugeId"]);
				});

				if (conjugeId == 0)
				{
					bancoDeDados.Commit();
					return;
				}

				comando = bancoDeDados.CriarComandoPlSql(@"
					begin 
						update {0}tab_pessoa p set p.estado_civil = :estadoCivil, p.tid = :tid where p.id in (:id,:conjugeId);
						delete {0}tab_pessoa_conjuge p where p.pessoa = :id; 
						delete {0}tab_pessoa_conjuge p where p.conjuge = :id; 
					end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("conjugeId", conjugeId, DbType.Int32);
				comando.AdicionarParametroEntrada("estadoCivil", 1, DbType.Int32); // 1-Solteiro(a)
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				
				bancoDeDados.ExecutarNonQuery(comando);

				#region Histórico e Consulta
				Historico.Gerar(id, eHistoricoArtefato.pessoa, eHistoricoAcao.alterarestadocivil, bancoDeDados);
				Historico.Gerar(conjugeId, eHistoricoArtefato.pessoa, eHistoricoAcao.alterarestadocivil, bancoDeDados);
				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarConjugeEstadoCivil(int id, int conjugeId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa p set p.estado_civil = :estadoCivil, p.tid = :tid where p.id = :id or p.id =:conjugeId", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("conjugeId", id, DbType.Int32);
				comando.AdicionarParametroEntrada("estadoCivil", 2, DbType.Int32); // 2-Casado(a)
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_conjuge(id, pessoa, conjuge, tid) 
					(select {0}seq_pessoa_conjuge.nextval, :pessoa, :conjuge, :tid from dual where not exists
					(select id from {0}tab_pessoa_conjuge r where r.pessoa = :pessoa and r.conjuge = :conjuge
					union all select id from {0}tab_pessoa_conjuge r where r.pessoa = :conjuge and r.conjuge = :pessoa))", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", id, DbType.Int32);
				comando.AdicionarParametroEntrada("conjuge", conjugeId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#region Histórico e Consulta
				Historico.Gerar(id, eHistoricoArtefato.pessoa, eHistoricoAcao.alterarestadocivil, bancoDeDados);
				Historico.Gerar(conjugeId, eHistoricoArtefato.pessoa, eHistoricoAcao.alterarestadocivil, bancoDeDados);
				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Validações

		internal bool ValidarConjugeAssociado(int? pessoaId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select sum(qtd) from (
					select count(*) qtd from tab_pessoa_conjuge t where t.conjuge = :pessoa_id
					union all 
					select count(*) qtd from tab_pessoa_conjuge t where t.pessoa = :pessoa_id)");

				comando.AdicionarParametroEntrada("pessoa_id", pessoaId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ValidarConjugeAssociado(int? conjugeId, int pessoaId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) 
					from (	select  c.pessoa pessoa1, c.conjuge pessoa2 from tab_pessoa_conjuge c
							union all
							select  c.conjuge pessoa1, c.pessoa pessoa2 from tab_pessoa_conjuge c  ) con 
					where con.pessoa1 = :conjuge_id and con.pessoa2 <> :pessoa_id");

				comando.AdicionarParametroEntrada("conjuge_id", conjugeId, DbType.Int32);
				comando.AdicionarParametroEntrada("pessoa_id", pessoaId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ExisteMunicipio(int MunicipioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from lov_municipio lm where lm.id = :id");
				comando.AdicionarParametroEntrada("id", MunicipioId, DbType.Int32);
				return bancoDeDados.ExecutarScalar(comando) != null;
			}
		}

		public bool ExisteProfissao(int pessoa, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select (case 
				when pext.tipo = 1 then (select count(*) from tab_pessoa p, tab_pessoa_profissao pro where pro.pessoa = p.id and p.id = pext.id)
				when pext.tipo = 2 then (select count(*) from tab_pessoa_representante pr, tab_pessoa_profissao pro where pr.pessoa = pext.id and pr.representante = pro.pessoa)
				end) profissoes from tab_pessoa pext where pext.id = :id");
				comando.AdicionarParametroEntrada("id", pessoa, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ExisteEndereco(int pessoa, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(pe.id) from tab_pessoa_endereco pe where pe.pessoa = :pessoa");

				comando.AdicionarParametroEntrada("pessoa", pessoa, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}
		public bool ExisteEstado(int EstadoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from lov_estado le where le.id = :id");
				comando.AdicionarParametroEntrada("id", EstadoId, DbType.Int32);
				return bancoDeDados.ExecutarScalar(comando) != null;
			}
		}

		internal bool ExistePessoa(String cpfCnpj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl(p.cpf, p.cnpj) from {0}tab_pessoa p where p.cpf = :cpf_cnpj or p.cnpj = :cpf_cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 50, cpfCnpj);
				return bancoDeDados.ExecutarScalar(comando) != null;
			}
		}

		internal bool ExistePessoa(Int32 id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(p.id) from {0}tab_pessoa p where p.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public List<String> VerificarPessoaEmpreendimento(int id, BancoDeDados banco = null)
		{
			List<String> pessoas = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.denominador from {0}tab_empreendimento e, {0}tab_empreendimento_responsavel r 
				where r.empreendimento = e.id and r.responsavel = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoas.Add(reader["denominador"].ToString());
					}
					reader.Close();
				}
			}
			return pessoas;
		}

		public List<String> VerificarPessoaRequerimento(int id, BancoDeDados banco = null)
		{
			List<String> pessoas = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select r.numero from {0}tab_requerimento r where r.interessado = :id union
				select r.numero from {0}tab_requerimento r, {0}tab_requerimento_responsavel re where re.requerimento = r.id and re.responsavel = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoas.Add(reader["numero"].ToString());
					}
					reader.Close();
				}
			}
			return pessoas;
		}

		public List<String> VerificarPessoaDocumento(int id, BancoDeDados banco = null)
		{
			List<String> pessoas = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
												select d.numero || '/' || d.ano numero
												  from {0}tab_protocolo d
												 where d.interessado = :id
												  and d.protocolo = 2
												union
												select d.numero || '/' || d.ano numero
												  from {0}tab_protocolo_responsavel dr, {0}tab_protocolo d
												 where dr.id = d.id
												   and d.protocolo = 2
												   and dr.responsavel = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoas.Add(reader["numero"].ToString());
					}
					reader.Close();
				}
			}
			return pessoas;
		}

		public List<String> VerificarPessoaProcesso(int id, BancoDeDados banco = null)
		{
			List<String> pessoas = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
										select d.numero || '/' || d.ano numero
										  from {0}tab_protocolo d
										 where d.interessado = :id
										 and d.protocolo = 1
										union
										select d.numero || '/' || d.ano numero
										  from {0}tab_protocolo_responsavel dr, {0}tab_protocolo d
										 where dr.id = d.id
										   and d.protocolo = 1
										   and dr.responsavel = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoas.Add(reader["numero"].ToString());
					}
					reader.Close();
				}
			}
			return pessoas;
		}

		public List<String> VerificarPessoaTitulo(int id, BancoDeDados banco = null)
		{
			List<String> titulos = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.numero ||case when t.ano is not null then '/'||t.ano else null end numero 
				from {0}tab_titulo_pessoas e, {0}tab_titulo_numero t where e.titulo = t.titulo(+) and e.pessoa = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						titulos.Add(reader["numero"].ToString());
					}
					reader.Close();
				}
			}
			return titulos;
		}

		public List<String> VerificarEmpreendimentoTituloEntrega(int id, BancoDeDados banco = null)
		{
			List<String> emp = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.id numero from {0}tab_titulo_entrega e where e.pessoa = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						emp.Add(reader["numero"].ToString());
					}
					reader.Close();
				}
			}
			return emp;
		}

		internal bool AssociadoRequerimentoProtocolo(int pessoaId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
								select (select count(*) valor
										  from tab_requerimento_responsavel
										 where responsavel = :pessoa_id) +									   
									   (select count(*) valor
										  from tab_protocolo_responsavel
										 where responsavel = :pessoa_id)
								  from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa_id", pessoaId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		#region Obter / Filtrar

		public List<Pessoa> ObterPessoasRelacionadas(List<int> pessoas)
		{
			List<Pessoa> retorno = new List<Pessoa>();
			
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Pessoas relacionadas

				if (pessoas != null && pessoas.Count > 0)
				{
					//Representantes
					Comando comando = bancoDeDados.CriarComando(@"select pc.pessoa, p.tipo Tipo, p.id Id, nvl(p.nome, p.razao_social) NomeRazaoSocial,
					nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
					from {0}tab_pessoa p, {0}tab_pessoa_representante pc where p.id = pc.representante", EsquemaBanco);

					comando.DbCommand.CommandText += comando.AdicionarIn("and", "pc.pessoa", DbType.Int32, pessoas);

					List<string> conj = new List<string>();

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Pessoa pes;
						while (reader.Read())
						{
							pes = new Pessoa();
							pes.Id = reader.GetValue<int>("Id");
							pes.InternoId = reader.GetValue<int>("Id");
							pes.IdRelacionamento = reader.GetValue<int>("pessoa");
							pes.Tipo = reader.GetValue<int>("Tipo");
							pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
							pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
							if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
							{
								conj = reader.GetValue<string>("conjuge").Split('@').ToList();
								pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
							}
							retorno.Add(pes);
						}
						reader.Close();
					}

					pessoas.AddRange(retorno.Select(x=>x.Id).ToList());

					//Conjuges
					comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, nvl(p.nome, p.razao_social) NomeRazaoSocial,
					nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
					from {0}tab_pessoa p, {0}tab_pessoa_conjuge pc where p.id = pc.conjuge", EsquemaBanco);

					comando.DbCommand.CommandText += comando.AdicionarIn("and", "pc.pessoa", DbType.Int32, pessoas);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Pessoa pes;
						while (reader.Read())
						{
							pes = new Pessoa();
							pes.Id = reader.GetValue<int>("Id");
							pes.InternoId = reader.GetValue<int>("Id");
							pes.Tipo = reader.GetValue<int>("Tipo");
							pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
							pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
							if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
							{
								conj = reader.GetValue<string>("conjuge").Split('@').ToList();
								pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
							}

							retorno.Add(pes);
						}
						reader.Close();
					}

					//Obter CPF Conjuges
					comando = bancoDeDados.CriarComando(@"select p.id, p.cpf from {0}tab_pessoa p ", EsquemaBanco);
					comando.DbCommand.CommandText += comando.AdicionarIn("where", "p.id", DbType.Int32, retorno.Where(x => x.Fisica.ConjugeId > 0).Select(x => x.Fisica.ConjugeId).ToList());

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Pessoa pes;
						while (reader.Read())
						{
							pes = retorno.FirstOrDefault(x => x.Fisica.ConjugeId == reader.GetValue<int>("id"));

							if (pes != null)
							{
								pes.Fisica.ConjugeCPF = reader.GetValue<string>("cpf");
							}
						}

						reader.Close();
					}
				}

				#endregion
			}

			retorno = retorno.GroupBy(x => x.Id).Select(y => new Pessoa
			{
				Id = y.First().Id,
				Tipo = y.First().Tipo,
				InternoId = y.First().InternoId,
				NomeRazaoSocial = y.First().NomeRazaoSocial,
				CPFCNPJ = y.First().CPFCNPJ,
				Fisica = y.First().Fisica,
			}).ToList();


			return retorno;
		}

		internal int ObterId(String cpfCnpj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_pessoa p where p.cpf = :cpf_cnpj or p.cnpj = :cpf_cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 50, cpfCnpj);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		public Municipio ObterMunicipio(int MunicipioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Municipio municipio = new Municipio();
				Comando comando = bancoDeDados.CriarComando(@"select m.id, m.texto, m.estado, m.cep, m.ibge from lov_municipio m where m.id = :id");
				comando.AdicionarParametroEntrada("id", MunicipioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						municipio.Id = Convert.ToInt32(reader["id"]);
						municipio.Estado.Id = Convert.ToInt32(reader["estado"]);
						municipio.Ibge = Convert.IsDBNull(reader["ibge"]) ? 0 : Convert.ToInt32(reader["ibge"]);
						municipio.Texto = reader["texto"].ToString();
						municipio.Cep = reader["texto"].ToString();
						municipio.IsAtivo = true;
					}
					reader.Close();
				}
				return municipio;
			}
		}

		internal Pessoa Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = new Pessoa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Pessoa
				Comando comando = bancoDeDados.CriarComando(@"select id, tipo, nome, apelido, cpf, rg, estado_civil, sexo, nacionalidade, naturalidade, data_nascimento, mae, pai, 
				cnpj, razao_social, nome_fantasia, ie, tid from {0}tab_pessoa p where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pessoa.Id = id;
						pessoa.Tid = reader["tid"].ToString();
						pessoa.Tipo = Convert.ToInt32(reader["tipo"]);

						if (pessoa.IsFisica)
						{
							pessoa.Fisica.Nome = reader["nome"].ToString();
							pessoa.Fisica.Apelido = reader["apelido"].ToString();
							pessoa.Fisica.NomeMae = reader["mae"].ToString();
							pessoa.Fisica.NomePai = reader["pai"].ToString();
							pessoa.Fisica.CPF = reader["cpf"].ToString();
							pessoa.Fisica.RG = reader["rg"].ToString();

							if (reader["estado_civil"] != null && !Convert.IsDBNull(reader["estado_civil"]))
							{
								pessoa.Fisica.EstadoCivil = Convert.ToInt32(reader["estado_civil"]);
							}

							if (reader["sexo"] != null && !Convert.IsDBNull(reader["sexo"]))
							{
								pessoa.Fisica.Sexo = Convert.ToInt32(reader["sexo"]);
							}

							pessoa.Fisica.Naturalidade = reader["naturalidade"].ToString();
							pessoa.Fisica.Nacionalidade = reader["nacionalidade"].ToString();

							if (reader["data_nascimento"] != null && !Convert.IsDBNull(reader["data_nascimento"]))
							{
								pessoa.Fisica.DataNascimento = Convert.ToDateTime(reader["data_nascimento"]);
							}
						}
						else // juridica
						{
							pessoa.Juridica.CNPJ = reader["cnpj"].ToString();
							pessoa.Juridica.RazaoSocial = reader["razao_social"].ToString();
							pessoa.Juridica.NomeFantasia = reader["nome_fantasia"].ToString();
							pessoa.Juridica.IE = reader["ie"].ToString();							
						}
					}

					reader.Close();
				}

				#endregion

				if (pessoa.Id <= 0 || simplificado)
				{
					return pessoa;
				}

				#region Meio de Contato
				comando = bancoDeDados.CriarComando(@"select a.id, b.mascara, a.pessoa, b.id tipo_contato_id, b.texto contato_texto,
				a.valor, a.tid from {0}tab_pessoa_meio_contato a, {0}tab_meio_contato b where a.meio_contato = b.id and a.pessoa = :pessoa", EsquemaBanco);
				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Contato contato;
					while (reader.Read())
					{
						contato = new Contato();
						contato.Id = Convert.ToInt32(reader["id"]);
						contato.PessoaId = Convert.ToInt32(reader["pessoa"]);
						contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
						contato.TipoTexto = reader["contato_texto"].ToString();
						contato.Mascara = reader["mascara"].ToString();
						contato.Valor = reader["valor"].ToString();
						contato.Tid = reader["tid"].ToString();
						pessoa.MeiosContatos.Add(contato);
					}
					reader.Close();
				}
				#endregion

				#region Endereços
				comando = bancoDeDados.CriarComando(@"
									select te.id,
										   te.pessoa,
										   te.cep,
										   te.logradouro,
										   te.bairro,
										   le.id          estado_id,
										   le.texto       estado_texto,
										   lm.id          municipio_id,
										   lm.texto       municipio_texto,
										   te.numero,
										   te.complemento,
										   te.tid,
										   te.distrito
									  from {0}tab_pessoa_endereco te, {0}lov_estado le, {0}lov_municipio lm
									 where te.estado = le.id(+)
									   and te.municipio = lm.id(+)
									   and te.pessoa = :pessoa", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoa.Endereco.Id = Convert.ToInt32(reader["id"]);
						pessoa.Endereco.Cep = reader["cep"].ToString();
						pessoa.Endereco.Logradouro = reader["logradouro"].ToString();
						pessoa.Endereco.Bairro = reader["bairro"].ToString();
						pessoa.Endereco.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
						pessoa.Endereco.EstadoTexto = reader["estado_texto"].ToString();
						pessoa.Endereco.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
						pessoa.Endereco.MunicipioTexto = reader["municipio_texto"].ToString();
						pessoa.Endereco.Numero = reader["numero"].ToString();
						pessoa.Endereco.Complemento = reader["complemento"].ToString();
						pessoa.Endereco.Tid = reader["tid"].ToString();
						pessoa.Endereco.DistritoLocalizacao = reader["distrito"].ToString();
					}
					reader.Close();
				}
				#endregion

				#region Profissão

				if (pessoa.IsFisica)
				{
					comando = bancoDeDados.CriarComando(@"select pf.id, pf.pessoa, tp.id profissao_id, tp.texto profissao_texto,
					tc.id orgao_classe_id, tc.texto orgao_classe_texto, registro, pf.tid from {0}tab_pessoa_profissao pf, {0}tab_profissao tp, {0}tab_orgao_classe tc
					where pf.profissao = tp.id and pf.orgao_classe = tc.id(+) and pf.pessoa = :pessoa", EsquemaBanco);
					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							pessoa.Fisica.Profissao.IdRelacionamento = Convert.ToInt32(reader["id"]);
							pessoa.Fisica.Profissao.Id = Convert.IsDBNull(reader["profissao_id"]) ? 0 : Convert.ToInt32(reader["profissao_id"]);
							pessoa.Fisica.Profissao.ProfissaoTexto = reader["profissao_texto"].ToString();
							pessoa.Fisica.Profissao.OrgaoClasseId = Convert.IsDBNull(reader["orgao_classe_id"]) ? 0 : Convert.ToInt32(reader["orgao_classe_id"]);
							pessoa.Fisica.Profissao.OrgaoClasseTexto = reader["orgao_classe_texto"].ToString();
							pessoa.Fisica.Profissao.Registro = reader["registro"].ToString();
							pessoa.Fisica.Profissao.Tid = reader["tid"].ToString();
						}
						reader.Close();
					}
				}
				#endregion

				#region Cônjuge

				if (pessoa.IsFisica)
				{
					comando = bancoDeDados.CriarComando(@"select pr.id, pr.pessoa, pr.conjuge, p.nome, p.cpf, p.tipo 
						from {0}tab_pessoa_conjuge pr, {0}tab_pessoa p where pr.conjuge = p.id and pr.pessoa = :pessoa          
						union all select pr.id, pr.conjuge pessoa, pr.pessoa conjuge, p.nome, p.cpf, p.tipo
						from {0}tab_pessoa_conjuge pr, {0}tab_pessoa p where pr.pessoa = p.id and pr.conjuge = :pessoa", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							pessoa.Fisica.ConjugeId = Convert.ToInt32(reader["conjuge"]);
							pessoa.Fisica.ConjugeNome = reader["nome"].ToString();
							pessoa.Fisica.ConjugeCPF = reader["cpf"].ToString();
						}
						reader.Close();
					}
				}
				#endregion

				#region Representantes

				if (pessoa.IsJuridica)
				{
					comando = bancoDeDados.CriarComando(@"select pr.id, pr.pessoa, pr.representante, p.nome, p.cpf, p.tipo, 
					  c.id conjuge_id, c.cpf conjuge_cpf, c.nome conjuge_nome
					  from tab_pessoa_representante pr, tab_pessoa p, tab_pessoa_conjuge pc, tab_pessoa c
					  where pr.representante = p.id and pr.pessoa = :pessoa
					  and p.id = pc.pessoa (+) and pc.conjuge = c.id (+) order by p.nome", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Pessoa representante;
						while (reader.Read())
						{
							representante = new Pessoa();
							representante.Id = Convert.ToInt32(reader["representante"]);
							representante.Tipo = Convert.ToInt32(reader["tipo"]);
							representante.Fisica.Nome = reader["nome"].ToString();
							representante.Fisica.CPF = reader["cpf"].ToString();
							representante.Fisica.ConjugeId = reader.GetValue<int>("conjuge_id");
							representante.Fisica.ConjugeCPF = reader.GetValue<string>("conjuge_cpf");
							representante.Fisica.ConjugeNome = reader.GetValue<string>("conjuge_nome");
							pessoa.Juridica.Representantes.Add(representante);
						}
						reader.Close();
					}
				}
				#endregion
			}
			return pessoa;
		}

		internal Pessoa Obter(String cpfCnpj, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = new Pessoa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_pessoa p where nvl(p.cpf,p.cnpj) = :cpfCnpj", EsquemaBanco);

				comando.AdicionarParametroEntrada("cpfCnpj", cpfCnpj, DbType.String);
				Object id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					pessoa = Obter(Convert.ToInt32(id), bancoDeDados, simplificado);
				}
			}

			return pessoa;
		}

		internal String ObterProfissao(int id, BancoDeDados banco = null)
		{
			String profissaoTexto = "";
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select texto from tab_profissao p where p.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				profissaoTexto = bancoDeDados.ExecutarScalar(comando).ToString();

			}
			return profissaoTexto;
		}

		public List<String> ObterAssociacoesPessoa(int pessoaId, BancoDeDados banco = null)
		{
			List<String> list = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.razao_social from tab_pessoa p,
				tab_pessoa_representante r where r.pessoa = p.id and r.representante = :pessoa_id");

				comando.AdicionarParametroEntrada("pessoa_id", pessoaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						list.Add(reader["razao_social"].ToString());
					}
					reader.Close();
				}

			}
			return list;
		}

		internal Resultados<Pessoa> Filtrar(Filtro<ListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Pessoa> retorno = new Resultados<Pessoa>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if (!String.IsNullOrWhiteSpace(filtros.Dados.CpfCnpj) && filtros.Dados.Tipo == 0)
				{
					filtros.Dados.Tipo = filtros.Dados.IsCpf ? 1 : 2;
				}

				comandtxt += comando.FiltroAnd("tipo", "tipo", filtros.Dados.Tipo);

				comandtxt += comando.FiltroAndLike("p.nome_razao_social", "nome_razao", filtros.Dados.NomeRazaoSocial, true);

				comandtxt += comando.FiltroAndLike("p.cpf_cnpj", "cpf_cnpj", filtros.Dados.CpfCnpj);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome_razao", "cpf_cnpj" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor-1));
				}
				else
				{
					ordenar.Add("nome_razao");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_pessoa p where p.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select  pessoa_id id, nome_razao_social nome_razao, cpf_cnpj, rg_ie, tipo from lst_pessoa p 
				where p.id > 0" + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Pessoa pessoa;
					while (reader.Read())
					{
						pessoa = new Pessoa();
						pessoa.Id = Convert.ToInt32(reader["id"]);
						pessoa.Tipo = Convert.ToInt32(reader["tipo"]);
						pessoa.Fisica.Nome = pessoa.IsFisica ? reader["nome_razao"].ToString() : "";
						pessoa.Juridica.RazaoSocial = pessoa.IsJuridica ? reader["nome_razao"].ToString() : "";
						pessoa.Fisica.CPF = pessoa.IsFisica ? reader["cpf_cnpj"].ToString() : "";
						pessoa.Juridica.CNPJ = pessoa.IsJuridica ? reader["cpf_cnpj"].ToString() : "";
						pessoa.Fisica.RG = pessoa.IsFisica ? reader["rg_ie"].ToString() : "";
						pessoa.Juridica.IE = pessoa.IsJuridica ? reader["rg_ie"].ToString() : "";

						retorno.Itens.Add(pessoa);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Resultados<ProfissaoLst> FiltrarProfissao(Filtro<ProfissaoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<ProfissaoLst> retorno = new Resultados<ProfissaoLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("p.texto", "profissao_texto", filtros.Dados.Profissao, true, true);
				
				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("texto");
				}

				#endregion
								
				if (!string.IsNullOrEmpty(filtros.Dados.Profissao)) 
				{
					comandtxt += @" union all select p.id, p.texto, max(trunc(metaphone.jaro_winkler(:filtro_fonetico,p.texto),5)) similaridade from tab_profissao p
										where p.texto_fonema like upper('%' || upper(metaphone.gerarCodigo(:filtro_fonetico)) || '%') and metaphone.jaro_winkler(:filtro_fonetico,p.texto) >=
										to_number(" + ConfiguracaoSistema.LimiteSimilaridade +@") group by p.id, p.texto";

					comando.AdicionarParametroEntrada("filtro_fonetico", filtros.Dados.Profissao, DbType.String);
					colunas[0] = "similaridade";
					ordenar[0] = "similaridade";
				}

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = "select count(*) from (select p.id, p.texto, 0 similaridade from tab_profissao p where p.id > 0 " + comandtxt + ")";

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comandtxt = @"select p.id, p.texto, 1 similaridade from tab_profissao p where p.id > 0 " + comandtxt;

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt += DaHelper.Ordenar(colunas, ordenar, !string.IsNullOrEmpty(filtros.Dados.Profissao));
							
				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					ProfissaoLst profissao;
					while (reader.Read())
					{
						profissao = new ProfissaoLst();
						profissao.Id = Convert.ToInt32(reader["id"]);
						profissao.Texto = reader["texto"].ToString();
						retorno.Itens.Add(profissao);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal int? ObterEstadoCivilAnterior(int pessoaId, BancoDeDados banco = null)
		{
			int? estadoCivilId = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Pessoa
				Comando comando = bancoDeDados.CriarComando(@"select h.estado_civil_id , max( h.data_execucao)
					from {0}hst_pessoa h  
					where h.pessoa_id = :id and h.estado_civil_id not in (2,5)   
					group by h.estado_civil_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", pessoaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						estadoCivilId = Convert.ToInt32(reader["estado_civil_id"]);
					}
					reader.Close();
				}
				#endregion
			}
			return estadoCivilId;
		}

		internal Pessoa ObterHistorico(int id, string tid, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = new Pessoa();
			Int32 id_hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Pessoa

				Comando comando = bancoDeDados.CriarComando(@"select p.id id_hst, p.pessoa_id id, p.tipo, p.nome, p.apelido, p.cpf, p.rg, p.estado_civil_id, 
															p.sexo, p.nacionalidade, p.naturalidade, p.data_nascimento, p.mae, p.pai, p.cnpj, p.razao_social,
															p.nome_fantasia, p.ie from {0}hst_pessoa p where p.pessoa_id = :pessoa 
															and p.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pessoa.Id = id;
						pessoa.Tid = tid;
						pessoa.Tipo = Convert.ToInt32(reader["tipo"]);
						id_hst = Convert.ToInt32(reader["id_hst"]);

						if (pessoa.IsFisica)
						{
							pessoa.Fisica.Nome = reader["nome"].ToString();
							pessoa.Fisica.Apelido = reader["apelido"].ToString();
							pessoa.Fisica.NomeMae = reader["mae"].ToString();
							pessoa.Fisica.NomePai = reader["pai"].ToString();
							pessoa.Fisica.CPF = reader["cpf"].ToString();
							pessoa.Fisica.RG = reader["rg"].ToString();

							if (reader["estado_civil_id"] != null && !Convert.IsDBNull(reader["estado_civil_id"]))
							{
								pessoa.Fisica.EstadoCivil = Convert.ToInt32(reader["estado_civil_id"]);
							}

							if (reader["sexo"] != null && !Convert.IsDBNull(reader["sexo"]))
							{
								pessoa.Fisica.Sexo = Convert.ToInt32(reader["sexo"]);
							}

							pessoa.Fisica.Naturalidade = reader["naturalidade"].ToString();
							pessoa.Fisica.Nacionalidade = reader["nacionalidade"].ToString();

							if (reader["data_nascimento"] != null && !Convert.IsDBNull(reader["data_nascimento"]))
							{
								pessoa.Fisica.DataNascimento = Convert.ToDateTime(reader["data_nascimento"]);
							}
						}
						else // juridica
						{
							pessoa.Juridica.CNPJ = reader["cnpj"].ToString();
							pessoa.Juridica.RazaoSocial = reader["razao_social"].ToString();
							pessoa.Juridica.NomeFantasia = reader["nome_fantasia"].ToString();
							pessoa.Juridica.IE = reader["ie"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				if (pessoa.Id <= 0 || simplificado)
				{
					return pessoa;
				}

				#region Meio de Contato

				comando = bancoDeDados.CriarComando(@"select a.id, a.meio_contato_id, b.mascara, b.id tipo_contato_id,
													b.texto contato_texto, a.valor, a.tid from {0}hst_pessoa_meio_contato a, 
													{0}tab_meio_contato b where a.meio_contato_id = b.id and a.id_hst = :id_hst
													and a.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", pessoa.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Contato contato;
					while (reader.Read())
					{
						contato = new Contato();
						contato.Id = Convert.ToInt32(reader["meio_contato_id"]);
						contato.PessoaId = id;
						contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
						contato.TipoTexto = reader["contato_texto"].ToString();
						contato.Mascara = reader["mascara"].ToString();
						contato.Valor = reader["valor"].ToString();
						contato.Tid = reader["tid"].ToString();
						pessoa.MeiosContatos.Add(contato);
					}
					reader.Close();
				}
				#endregion

				#region Endereços

				comando = bancoDeDados.CriarComando(@"select te.endereco_id id, te.cep, te.logradouro, te.bairro, te.estado_id, te.estado_texto,
													te.municipio_id, te.municipio_texto, te.numero, te.complemento, te.tid, te.distrito
													from {0}hst_pessoa_endereco te where te.id_hst = :id_hst and te.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", pessoa.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoa.Endereco.Id = Convert.ToInt32(reader["id"]);
						pessoa.Endereco.Cep = reader["cep"].ToString();
						pessoa.Endereco.Logradouro = reader["logradouro"].ToString();
						pessoa.Endereco.Bairro = reader["bairro"].ToString();
						pessoa.Endereco.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
						pessoa.Endereco.EstadoTexto = reader["estado_texto"].ToString();
						pessoa.Endereco.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
						pessoa.Endereco.MunicipioTexto = reader["municipio_texto"].ToString();
						pessoa.Endereco.Numero = reader["numero"].ToString();
						pessoa.Endereco.Complemento = reader["complemento"].ToString();
						pessoa.Endereco.Tid = reader["tid"].ToString();
						pessoa.Endereco.DistritoLocalizacao = reader["distrito"].ToString();
					}
					reader.Close();
				}
				#endregion

				#region Profissão

				if (pessoa.IsFisica)
				{
					comando = bancoDeDados.CriarComando(@"select pf.profissao_id id, pf.pes_profissao_id pessoa, tp.id profissao_id,
														tp.texto  profissao_texto, tc.id orgao_classe_id, tc.texto  orgao_classe_texto,
														registro, pf.tid from {0}hst_pessoa_profissao pf, {0}tab_profissao tp,
														{0}tab_orgao_classe tc where pf.profissao_id = tp.id and pf.orgao_classe_id = tc.id(+)
														and pf.id_hst = :id_hst and pf.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", pessoa.Tid, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							pessoa.Fisica.Profissao.IdRelacionamento = Convert.ToInt32(reader["id"]);
							pessoa.Fisica.Profissao.Id = Convert.IsDBNull(reader["profissao_id"]) ? 0 : Convert.ToInt32(reader["profissao_id"]);
							pessoa.Fisica.Profissao.ProfissaoTexto = reader["profissao_texto"].ToString();
							pessoa.Fisica.Profissao.OrgaoClasseId = Convert.IsDBNull(reader["orgao_classe_id"]) ? 0 : Convert.ToInt32(reader["orgao_classe_id"]);
							pessoa.Fisica.Profissao.OrgaoClasseTexto = reader["orgao_classe_texto"].ToString();
							pessoa.Fisica.Profissao.Registro = reader["registro"].ToString();
							pessoa.Fisica.Profissao.Tid = reader["tid"].ToString();
						}
						reader.Close();
					}
				}
				#endregion

				#region Cônjuge

				if (pessoa.IsFisica)
				{
					comando = bancoDeDados.CriarComando(@"select pr.pessoa_conj_id id, pr.pessoa_id pessoa, pr.conjuge_id conjuge, p.nome, p.cpf, p.tipo
														from {0}hst_pessoa_conjuge pr, {0}hst_pessoa p where pr.conjuge_id = p.pessoa_id and 
														pr.pessoa_id = :pessoa and pr.id_hst = p.id and p.tid = :tid union all select pr.pessoa_conj_id id,
														pr.conjuge_id pessoa, pr.pessoa_id conjuge, p.nome, p.cpf, p.tipo from {0}hst_pessoa_conjuge pr, 
														{0}hst_pessoa p where pr.pessoa_id = p.pessoa_id and pr.conjuge_id = :pessoa and pr.id_hst = p.id
														and p.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", pessoa.Tid, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							pessoa.Fisica.ConjugeId = Convert.ToInt32(reader["conjuge"]);
							pessoa.Fisica.ConjugeNome = reader["nome"].ToString();
							pessoa.Fisica.ConjugeCPF = reader["cpf"].ToString();
						}
						reader.Close();
					}
				}
				#endregion

				#region Representantes

				if (pessoa.IsJuridica)
				{
					comando = bancoDeDados.CriarComando(@"select pr.pessoa_rep_id id, pr.pessoa_id pessoa, pr.pessoa_tid, pr.representante_id representante,
														p.nome, p.cpf, p.tipo, c.id conjuge_id, c.cpf conjuge_cpf, c.nome conjuge_nome from 
														{0}hst_pessoa_representante pr, {0}hst_pessoa p, {0}hst_pessoa_conjuge pc, {0}hst_pessoa c
														where pr.representante_id = p.pessoa_id and pr.representante_tid = p.tid and pr.pessoa_id = :pessoa
														and pr.pessoa_tid = :tid and p.pessoa_id = pc.pessoa_id(+) and p.tid = pc.conjuge_tid(+)
														and pc.conjuge_id = c.pessoa_id(+) and pc.conjuge_tid = c.tid(+) order by p.nome", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", pessoa.Tid, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Pessoa representante;
						while (reader.Read())
						{
							representante = new Pessoa();
							representante.Id = Convert.ToInt32(reader["representante"]);
							representante.Tipo = Convert.ToInt32(reader["tipo"]);
							representante.Fisica.Nome = reader["nome"].ToString();
							representante.Fisica.CPF = reader["cpf"].ToString();
							representante.Fisica.ConjugeId = reader.GetValue<int>("conjuge_id");
							representante.Fisica.ConjugeCPF = reader.GetValue<string>("conjuge_cpf");
							representante.Fisica.ConjugeNome = reader.GetValue<string>("conjuge_nome");
							pessoa.Juridica.Representantes.Add(representante);
						}
						reader.Close();
					}
				}
				#endregion
			}
			return pessoa;
		}

		#endregion
	}
}