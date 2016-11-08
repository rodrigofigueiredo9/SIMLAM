using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class PessoaDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        Consulta _consulta = new Consulta();
        public Consulta Consulta { get { return _consulta; } }

        Historico _historico = new Historico();

        public Historico Historico { get { return _historico; } }

        private string EsquemaBanco { get { return _configSys.UsuarioInterno; } }

        public PessoaDa() { }

        internal Pessoa Obter(String cpfCnpj, BancoDeDados bancoInterno, bool simplificado = false)
        {
            Pessoa pessoa = new Pessoa();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

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

        internal Pessoa Obter(int id, BancoDeDados bancoInterno, bool simplificado = false)
        {
            Pessoa pessoa = new Pessoa();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

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

        internal void AlterarConjuge(int id, BancoDeDados bancoInterno)
        {
            int conjugeId = 0;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
					select c.conjuge conjugeId from {0}tab_pessoa_conjuge c where c.pessoa = :id 
					union all 
					select c.pessoa conjugeId from {0}tab_pessoa_conjuge c where c.conjuge =: id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarReader(comando, (reader) =>
                {
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

        internal int ObterId(String cpfCnpj, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_pessoa p where p.cpf = :cpf_cnpj or p.cnpj = :cpf_cnpj", EsquemaBanco);
                comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 50, cpfCnpj);

                object retorno = bancoDeDados.ExecutarScalar(comando);

                return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
            }
        }

        internal int Criar(Pessoa pessoa, BancoDeDados banco, Executor executor = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Pessoa

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
                //Historico
                Historico.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, eHistoricoAcao.criar, bancoDeDados, executor);
                Consulta.Gerar(pessoa.Id, eHistoricoArtefato.pessoa, bancoDeDados);

                #endregion

                bancoDeDados.Commit();

                return pessoa.Id;
            }
        }
        internal void Editar(Pessoa pessoa, BancoDeDados bancoInterno, Executor executor = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
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

        internal void AlterarConjugeEstadoCivil(int id, int conjugeId, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
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
    
    }
}
