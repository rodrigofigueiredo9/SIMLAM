using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class PessoaCredenciadoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        private string EsquemaBanco { get { return _configSys.UsuarioCredenciado; } }

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

        internal List<Estado> ObterEstados()
        {
            List<Estado> lst = new List<Estado>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.sigla from lov_estado c");
            foreach (var item in daReader)
            {
                lst.Add(new Estado()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["sigla"].ToString(),
                    Sigla = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }
        
        internal Dictionary<int, List<Municipio>> ObterMunicipios(List<Estado> estados)
        {
            Dictionary<int, List<Municipio>> lst = new Dictionary<int, List<Municipio>>();
            List<Municipio> lstMun = null;

            foreach (var estado in estados)
            {
                IEnumerable<IDataReader> daReader = DaHelper.ObterLista(string.Format(@"select c.id, c.texto, c.estado, c.cep, c.ibge from lov_municipio c where c.estado = {0} order by c.texto", estado.Id.ToString()));

                lstMun = new List<Municipio>();
                foreach (var item in daReader)
                {
                    lstMun.Add(new Municipio()
                    {
                        Id = Convert.ToInt32(item["id"]),
                        Texto = item["texto"].ToString(),
                        Estado = new Estado() { Id = Convert.ToInt32(item["estado"]) },
                        IsAtivo = true,
                        Ibge = Convert.ToInt32(item["ibge"])
                    });
                }

                lst.Add(estado.Id, lstMun);

            }

            return lst;
        }

        internal List<EstadoCivil> ObterEstadosCivis()
        {
            List<EstadoCivil> lst = new List<EstadoCivil>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_pessoa_estado_civil c");
            foreach (var item in daReader)
            {
                lst.Add(new EstadoCivil()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

        internal List<Sexo> ObterSexos()
        {
            List<Sexo> lst = new List<Sexo>();

            lst.Add(new Sexo()
            {
                Id = 1,
                Texto = "Masculino",
                IsAtivo = true
            });

            lst.Add(new Sexo()
            {
                Id = 2,
                Texto = "Feminino",
                IsAtivo = true
            });

            return lst;
        }

        internal List<ProfissaoLst> ObterProfissoes()
        {
            List<ProfissaoLst> lst = new List<ProfissaoLst>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from tab_profissao c");
            foreach (var item in daReader)
            {
                lst.Add(new ProfissaoLst()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

        internal List<OrgaoClasse> ObterOrgaoClasses()
        {
            List<OrgaoClasse> lst = new List<OrgaoClasse>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.orgao_sigla from tab_orgao_classe c");
            foreach (var item in daReader)
            {
                lst.Add(new OrgaoClasse()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["orgao_sigla"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

    }
}
