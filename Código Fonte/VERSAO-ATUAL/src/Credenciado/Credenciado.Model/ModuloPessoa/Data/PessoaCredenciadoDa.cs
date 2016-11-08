using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Data
{
	public class PessoaCredenciadoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public PessoaCredenciadoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

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

		internal int Criar(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Pessoa

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa a (id, ativa, tipo, nome, apelido, cpf, rg,  estado_civil, sexo, nacionalidade, naturalidade,
				data_nascimento, mae, pai, cnpj, razao_social, nome_fantasia, ie, interno, credenciado, usuario, tid) values ({0}seq_pessoa.nextval, :ativa, :tipo, :nome, :apelido, :cpf, :rg, 
				:estado_civil, :sexo, :nacionalidade, :naturalidade, :data_nascimento,  :mae, :pai, :cnpj, :razao_social, :nome_fantasia, :ie, :interno, :credenciado, :usuario, :tid) 
				returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("interno", (pessoa.InternoId.GetValueOrDefault() > 0) ? pessoa.InternoId : null, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", pessoa.CredenciadoId, DbType.Int32);
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
				comando.AdicionarParametroEntrada("usuario", pessoa.IsCredenciado ? 1 : 0, DbType.Int32);
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

				#region Histórico

				Historico.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, eHistoricoAcao.criar, bancoDeDados, executor);

				#endregion

				bancoDeDados.Commit();

				return pessoa.Id;
			}
		}

		internal void Editar(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Pessoa

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
									update {0}tab_pessoa p
										set p.tipo            = :tipo,
											p.nome            = :nome,
											p.apelido         = :apelido,
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

				if (pessoa.IsFisica)
				{
					//Profissao
					comando = bancoDeDados.CriarComando("delete {0}tab_pessoa_profissao p where p.pessoa = :pessoa and p.id <> :id_relacionamento", EsquemaBanco);
					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("id_relacionamento", pessoa.Fisica.Profissao.IdRelacionamento, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

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
					if (pessoa.Fisica.Profissao.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa_profissao p set p.pessoa = :pessoa, p.profissao = :profissao, p.orgao_classe = :orgao_classe, 
						p.registro = :registro, p.tid = :tid where p.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", pessoa.Fisica.Profissao.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_profissao (id, pessoa, profissao, orgao_classe, registro, tid) values
						({0}seq_pessoa_profissao.nextval, :pessoa, :profissao, :orgao_classe, :registro, :tid)", EsquemaBanco);
					}

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

				#region Histórico

				Historico.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, eHistoricoAcao.atualizar, bancoDeDados, executor);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.pessoa, eHistoricoAcao.excluir, bancoDeDados);

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

		internal void SalvarConjuge(int pessoa, int conjuge, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_pessoa_conjuge (id, pessoa, conjuge, tid)
					values({0}seq_pessoa_conjuge.nextval, :pessoa, :conjuge, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa, DbType.Int32);
				comando.AdicionarParametroEntrada("conjuge", conjuge, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarEstadoCivil(int id, int estadoCivilId, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa p set p.estado_civil = :estadoCivil, p.tid = :tid where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("estadoCivil", estadoCivilId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.pessoa, eHistoricoAcao.alterarestadocivil, bancoDeDados, executor);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarEmail(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					update {0}tab_pessoa_conjuge a set a.tid = :tid where a.pessoa = :pessoa;
					update {0}tab_pessoa_representante a set a.tid = :tid where a.pessoa = :pessoa;
					update {0}tab_pessoa_profissao a set a.tid = :tid where a.pessoa = :pessoa;
					update {0}tab_pessoa_endereco a set a.tid = :tid where a.pessoa = :pessoa;
					update {0}tab_pessoa_meio_contato a set a.tid = :tid where a.pessoa = :pessoa;
					update {0}tab_pessoa a set a.tid = :tid where a.id = :pessoa;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Meios de contato

				Contato email = pessoa.MeiosContatos.Single(x => x.TipoContato == eTipoContato.Email);

				comando = bancoDeDados.CriarComando(@"update {0}tab_pessoa_meio_contato a set a.valor = :valor, a.tid = :tid 
				where a.pessoa = :pessoa and a.meio_contato = :meio_contato", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("meio_contato", email.TipoContatoInteiro, DbType.Int32);
				comando.AdicionarParametroEntrada("valor", DbType.String, 1000, email.Valor);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, eHistoricoAcao.atualizar, bancoDeDados, executor);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Pessoa Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = new Pessoa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Pessoa
				Comando comando = bancoDeDados.CriarComando(@"select id, tipo, nome, apelido, cpf, rg, estado_civil, sexo, nacionalidade, naturalidade, data_nascimento, mae, pai, 
				cnpj, razao_social, nome_fantasia, ie, interno, credenciado, tid from {0}tab_pessoa p where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pessoa.Id = id;
						pessoa.Tid = reader["tid"].ToString();
						pessoa.Tipo = Convert.ToInt32(reader["tipo"]);
						pessoa.CredenciadoId = reader.GetValue<int?>("credenciado");

						if (reader["interno"] != null && !Convert.IsDBNull(reader["interno"]))
						{
							pessoa.InternoId = Convert.ToInt32(reader["interno"]);
						}

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
								pessoa.Fisica.Sexo = Convert.IsDBNull(reader["sexo"]) ? (int?)null : Convert.ToInt32(reader["sexo"]);
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
				comando = bancoDeDados.CriarComando(@"select a.id, a.pessoa, a.meio_contato tipo_contato_id, a.valor, a.tid
					from {0}tab_pessoa_meio_contato a where a.pessoa = :pessoa", EsquemaBanco);
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
										   te.estado      estado_id,
										   te.municipio   municipio_id,
										   te.numero,
										   te.complemento,
										   te.tid,
										   te.distrito
									  from tab_pessoa_endereco te
									 where te.pessoa = :pessoa", EsquemaBanco);

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
						pessoa.Endereco.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
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
					comando = bancoDeDados.CriarComando(@"select pf.id,
																pf.pessoa,
																pf.profissao    profissao_id,
																pf.orgao_classe orgao_classe_id,
																registro,
																pf.tid
															from tab_pessoa_profissao pf
															where pf.pessoa = :pessoa", EsquemaBanco);
					comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							pessoa.Fisica.Profissao.IdRelacionamento = Convert.ToInt32(reader["id"]);
							pessoa.Fisica.Profissao.Id = Convert.IsDBNull(reader["profissao_id"]) ? 0 : Convert.ToInt32(reader["profissao_id"]);
							pessoa.Fisica.Profissao.OrgaoClasseId = Convert.IsDBNull(reader["orgao_classe_id"]) ? 0 : Convert.ToInt32(reader["orgao_classe_id"]);
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
					comando = bancoDeDados.CriarComando(@"select pr.id, pr.pessoa, pr.representante, p.nome, p.cpf, p.tipo, p.rg, p.estado_civil, p.sexo, 
														p.nacionalidade, p.naturalidade, p.data_nascimento, p.mae, p.pai, p.razao_social 
														from {0}tab_pessoa_representante pr, {0}tab_pessoa p 
														where pr.representante = p.id and pr.pessoa = :pessoa order by p.nome", EsquemaBanco);

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


							if (reader["estado_civil"] != null && !Convert.IsDBNull(reader["estado_civil"]))
							{
								representante.Fisica.EstadoCivil = Convert.ToInt32(reader["estado_civil"]);
							}

							if (reader["sexo"] != null && !Convert.IsDBNull(reader["sexo"]))
							{
								representante.Fisica.Sexo = Convert.IsDBNull(reader["sexo"]) ? (int?)null : Convert.ToInt32(reader["sexo"]);
							}

							if (reader["data_nascimento"] != null && !Convert.IsDBNull(reader["data_nascimento"]))
							{
								representante.Fisica.DataNascimento = Convert.ToDateTime(reader["data_nascimento"]);
							}

							representante.Fisica.RG = reader["rg"].ToString();
							representante.Fisica.Nacionalidade = reader["nacionalidade"].ToString();
							representante.Fisica.Naturalidade = reader["naturalidade"].ToString();
							representante.Fisica.NomeMae = reader["mae"].ToString();
							representante.Fisica.NomePai = reader["pai"].ToString();


							#region Endereços

							comando = bancoDeDados.CriarComando(@"
										select te.id,
											   te.pessoa,
											   te.cep,
											   te.logradouro,
											   te.bairro,
											   te.estado      estado_id,
											   te.municipio   municipio_id,
											   te.numero,
											   te.complemento,
											   te.tid,
											   te.distrito
										  from tab_pessoa_endereco te
										 where te.pessoa = :pessoa", EsquemaBanco);

							comando.AdicionarParametroEntrada("pessoa", representante.Id, DbType.Int32);

							using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
							{
								while (readerAux.Read())
								{
									Endereco endereco = new Endereco();
									endereco.Id = Convert.ToInt32(readerAux["id"]);
									endereco.Cep = readerAux["cep"].ToString();
									endereco.Logradouro = readerAux["logradouro"].ToString();
									endereco.Bairro = readerAux["bairro"].ToString();
									endereco.EstadoId = Convert.IsDBNull(readerAux["estado_id"]) ? 0 : Convert.ToInt32(readerAux["estado_id"]);
									endereco.MunicipioId = Convert.IsDBNull(readerAux["municipio_id"]) ? 0 : Convert.ToInt32(readerAux["municipio_id"]);
									endereco.Numero = readerAux["numero"].ToString();
									endereco.Complemento = readerAux["complemento"].ToString();
									endereco.Tid = readerAux["tid"].ToString();
									endereco.DistritoLocalizacao = readerAux["distrito"].ToString();

									representante.Endereco = endereco;
								}
								readerAux.Close();
							}

							#endregion

							pessoa.Juridica.Representantes.Add(representante);
						}
						reader.Close();
					}
				}
				#endregion
			}
			return pessoa;
		}

		internal Pessoa Obter(String cpfCnpj, BancoDeDados banco = null, bool simplificado = false, int? credenciadoId = null)
		{
			Pessoa pessoa = new Pessoa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id from tab_pessoa p where nvl(p.cpf, p.cnpj) = :cpfCnpj and p.credenciado = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("cpfCnpj", cpfCnpj, DbType.String);
				comando.AdicionarParametroEntrada("id", credenciadoId, DbType.Int32);

				Object id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					pessoa = Obter(Convert.ToInt32(id), bancoDeDados, simplificado);
				}
			}

			return pessoa;
		}

		internal String ObterProfissaoTexto(int id)
		{
			String profissaoTexto = "";
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".");

				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if (!String.IsNullOrWhiteSpace(filtros.Dados.CpfCnpj) && filtros.Dados.Tipo == 0)
				{
					filtros.Dados.Tipo = filtros.Dados.IsCpf ? 1 : 2;
				}

				comandtxt += comando.FiltroAnd("tipo", "tipo", filtros.Dados.Tipo);

				if (filtros.Dados.IsCpf)
				{
					comandtxt += comando.FiltroAndLike("p.nome", "nome_razao", filtros.Dados.NomeRazaoSocial, true);
				}
				else
				{
					comandtxt += comando.FiltroAndLike("p.razao_social", "nome_razao", filtros.Dados.NomeRazaoSocial, true);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome_razao", "cpf_cnpj" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome_razao");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_pessoa p where p.credenciado = :credenciado" + comandtxt, esquema);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("credenciado", filtros.Dados.Credenciado);

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select id, nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj, nvl(p.rg, p.ie)  rg_ie, tipo from {0}tab_pessoa p 
				where p.credenciado = :credenciado" + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquema);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Pessoa pessoa;
					while (reader.Read())
					{
						pessoa = new Pessoa();
						pessoa.Id = reader.GetValue<int>("id"); ;
						pessoa.Tipo = reader.GetValue<int>("tipo");
						pessoa.Fisica.Nome = pessoa.IsFisica ? reader.GetValue<string>("nome_razao") : "";
						pessoa.Juridica.RazaoSocial = pessoa.IsJuridica ? reader.GetValue<string>("nome_razao") : "";
						pessoa.Fisica.CPF = pessoa.IsFisica ? reader.GetValue<string>("cpf_cnpj") : "";
						pessoa.Juridica.CNPJ = pessoa.IsJuridica ? reader.GetValue<string>("cpf_cnpj") : "";
						pessoa.Fisica.RG = pessoa.IsFisica ? reader.GetValue<string>("rg_ie") : "";
						pessoa.Juridica.IE = pessoa.IsJuridica ? reader.GetValue<string>("rg_ie") : "";
						retorno.Itens.Add(pessoa);
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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Pessoa

				Comando comando = bancoDeDados.CriarComando(@"select h.estado_civil_id , max( h.data_execucao)
				from {0}hst_pessoa h where h.pessoa_id = :id and h.estado_civil_id not in (2,5) group by h.estado_civil_id", EsquemaBanco);

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

		#endregion

		#region Validações

		internal bool ValidarConjugeAssociado(int? pessoaId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select sum(qtd) from (
					select count(*) qtd from tab_pessoa_conjuge t where t.conjuge = :conjuge_id and t.pessoa <> :pessoa_id
					union all 
					select count(*) qtd from tab_pessoa_conjuge t where t.conjuge = :pessoa_id and t.pessoa <> :conjuge_id )");

				comando.AdicionarParametroEntrada("conjuge_id", conjugeId, DbType.Int32);
				comando.AdicionarParametroEntrada("pessoa_id", pessoaId, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool ValidarConjugeAssociado(string pessoaCPF, string conjugeCPF, int credenciado)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select sum(qtd) qtd from (
				select count(*) qtd from tab_pessoa_conjuge c, tab_pessoa p, tab_pessoa pc
				where c.pessoa = p.id and c.conjuge = pc.id and p.credenciado = :credenciado and pc.credenciado = :credenciado and p.cpf <> :pessoa_cpf and pc.cpf = :conjuge_cpf 
				union all
				select count(*) qtd from tab_pessoa_conjuge c, tab_pessoa p, tab_pessoa pc
				where c.pessoa = p.id and c.conjuge = pc.id and p.credenciado = :credenciado and pc.credenciado = :credenciado and pc.cpf <> :pessoa_cpf and p.cpf = :conjuge_cpf)", EsquemaBanco);

				comando.AdicionarParametroEntrada("credenciado", credenciado, DbType.Int32);
				comando.AdicionarParametroEntrada("pessoa_cpf", DbType.String, 20, pessoaCPF);
				comando.AdicionarParametroEntrada("conjuge_cpf", DbType.String, 20, conjugeCPF);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		public bool ExisteProfissao(int pessoa, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
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

		internal int ExistePessoa(String cpfCnpj, int credenciado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_pessoa p where nvl(p.cpf, p.cnpj) = :cpf_cnpj and p.credenciado = :credenciado", EsquemaBanco);

				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 50, cpfCnpj);
				comando.AdicionarParametroEntrada("credenciado", credenciado, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					return Convert.ToInt32(retorno);
				}

				return 0;
			}
		}

		internal bool ExistePessoa(Int32 id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(p.id) from {0}tab_pessoa p where p.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public List<String> VerificarPessoaEmpreendimento(int id, BancoDeDados banco = null)
		{
			List<String> pessoas = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
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

		#endregion
	}
}