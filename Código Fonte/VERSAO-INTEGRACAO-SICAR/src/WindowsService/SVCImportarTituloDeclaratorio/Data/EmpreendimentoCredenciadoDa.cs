using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class EmpreendimentoCredenciadoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();
        
        Historico _historico = new Historico();

        public Historico Historico { get { return _historico; } }

        public String UsuarioCredenciado
        {
            get { return _configSys.UsuarioCredenciado; }
        }

        public List<Pessoa> ObterResponsaveis(int empreendimento, BancoDeDados bancoCredenciado)
        {
            var retorno = new List<Pessoa>();
            var conj = new List<string>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado,UsuarioCredenciado))
            {
                bancoDeDados.IniciarTransacao();

                #region Responsáveis do empreendimento

                Comando comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, p.interno InternoId, nvl(p.nome, p.razao_social) NomeRazaoSocial,
				nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
				from {0}tab_pessoa p, {0}tab_empreendimento_responsavel pc where p.id = pc.responsavel and pc.empreendimento = :empreendimento", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

                Pessoa pessoa;

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        pessoa = new Pessoa();
                        pessoa.Id = reader.GetValue<int>("Id");
                        pessoa.Tipo = reader.GetValue<int>("Tipo");
                        pessoa.InternoId = reader.GetValue<int>("InternoId");
                        pessoa.NomeFantasia = reader.GetValue<string>("NomeRazaoSocial");
                        pessoa.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
                        if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
                        {
                            conj = reader.GetValue<string>("conjuge").Split('@').ToList();
                            pessoa.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pessoa.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
                        }
                        retorno.Add(pessoa);
                    }

                    reader.Close();
                }

                #endregion

                //Obter CPF Conjuges
                comando = bancoDeDados.CriarComando(@"select p.id, p.cpf from {0}tab_pessoa p ", UsuarioCredenciado);
                comando.DbCommand.CommandText += comando.AdicionarIn("where", "p.id", DbType.Int32, retorno.Where(x => x.Fisica.ConjugeId > 0).Select(x => x.Fisica.ConjugeId).ToList());

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        pessoa = retorno.FirstOrDefault(x => x.Fisica.ConjugeId == reader.GetValue<int>("id"));

                        if (pessoa != null)
                        {
                            pessoa.Fisica.ConjugeCPF = reader.GetValue<string>("cpf");
                        }
                    }

                    reader.Close();
                }
            }
            return retorno;
        }

        public Empreendimento Obter(int id, BancoDeDados bancoCredenciado, bool simplificado = false)
        {
            Empreendimento empreendimento = new Empreendimento();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
            {
                bancoDeDados.IniciarTransacao();

                #region Empreendimento

                Comando comando = bancoDeDados.CriarComando(@"select e.id, e.segmento, e.cnpj, e.denominador, e.nome_fantasia, e.atividade, e.interno, e.interno_tid, 
				e.credenciado, e.tid, e.codigo from {0}tab_empreendimento e where e.id = :id", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        empreendimento.Id = id;
                        empreendimento.Tid = reader["tid"].ToString();
                        empreendimento.CredenciadoId = reader.GetValue<int?>("credenciado");
                        empreendimento.InternoId = reader.GetValue<int?>("interno");
                        empreendimento.InternoTid = reader.GetValue<string>("interno_tid");
                        empreendimento.Codigo = reader.GetValue<long?>("codigo");
                        if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
                        {
                            empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
                        }

                        empreendimento.CNPJ = reader["cnpj"].ToString();
                        empreendimento.Denominador = reader["denominador"].ToString();
                        empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

                        if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
                        {
                            empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
                        }
                    }

                    reader.Close();
                }

                #endregion

                if (empreendimento.Id <= 0 || simplificado)
                {
                    return empreendimento;
                }

                #region Responsáveis

                comando = bancoDeDados.CriarComando(@"select pr.id, pr.empreendimento, pr.responsavel, nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf_cnpj, pr.tipo, pr.data_vencimento, pr.especificar 
				from {0}tab_empreendimento_responsavel pr, {0}tab_pessoa p where pr.responsavel = p.id and pr.empreendimento = :empreendimento", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Responsavel responsavel;

                    while (reader.Read())
                    {
                        responsavel = new Responsavel();
                        responsavel.Id = Convert.ToInt32(reader["responsavel"]);
                        responsavel.IdRelacionamento = Convert.ToInt32(reader["id"]);
                        responsavel.Tipo = Convert.ToInt32(reader["tipo"]);

                        if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
                        {
                            responsavel.DataVencimento = Convert.ToDateTime(reader["data_vencimento"]);
                        }

                        responsavel.NomeRazao = reader["nome"].ToString();
                        responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
                        responsavel.EspecificarTexto = reader["especificar"].ToString();
                        empreendimento.Responsaveis.Add(responsavel);
                    }

                    reader.Close();
                }

                #endregion

                #region Endereços

                comando = bancoDeDados.CriarComando(@"select te.id, te.empreendimento, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro, te.estado, te.municipio, 
				te.numero, te.distrito, te.corrego, te.caixa_postal, te.complemento, te.tid from {0}tab_empreendimento_endereco te 
				where te.empreendimento = :empreendimento order by te.correspondencia", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Endereco end;

                    while (reader.Read())
                    {
                        end = new Endereco();
                        end.Id = Convert.ToInt32(reader["id"]);
                        end.Tid = reader["tid"].ToString();
                        end.Correspondencia = Convert.IsDBNull(reader["correspondencia"]) ? 0 : Convert.ToInt32(reader["correspondencia"]);
                        end.ZonaLocalizacaoId = Convert.IsDBNull(reader["zona"]) ? 0 : Convert.ToInt32(reader["zona"]);
                        end.Cep = reader["cep"].ToString();
                        end.Logradouro = reader["logradouro"].ToString();
                        end.Bairro = reader["bairro"].ToString();
                        end.EstadoId = Convert.IsDBNull(reader["estado"]) ? 0 : Convert.ToInt32(reader["estado"]);
                        end.MunicipioId = Convert.IsDBNull(reader["municipio"]) ? 0 : Convert.ToInt32(reader["municipio"]);
                        end.Numero = reader["numero"].ToString();
                        end.DistritoLocalizacao = reader["distrito"].ToString();
                        end.Corrego = reader["corrego"].ToString();
                        end.CaixaPostal = reader["caixa_postal"].ToString();
                        end.Complemento = reader["complemento"].ToString();
                        empreendimento.Enderecos.Add(end);
                    }

                    reader.Close();
                }

                #endregion

                #region Coordenada

                comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, aec.datum, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec, 
				aec.forma_coleta, aec.local_coleta from {0}tab_empreendimento_coord aec where aec.empreendimento = :empreendimentoid", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("empreendimentoid", empreendimento.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
                        empreendimento.Coordenada.Tid = reader["tid"].ToString();

                        if (!Convert.IsDBNull(reader["easting_utm"]))
                        {
                            empreendimento.Coordenada.EastingUtm = Convert.ToDouble(reader["easting_utm"]);
                            empreendimento.Coordenada.EastingUtmTexto = empreendimento.Coordenada.EastingUtm.ToString();
                        }

                        if (!Convert.IsDBNull(reader["northing_utm"]))
                        {
                            empreendimento.Coordenada.NorthingUtm = Convert.ToDouble(reader["northing_utm"]);
                            empreendimento.Coordenada.NorthingUtmTexto = empreendimento.Coordenada.NorthingUtm.ToString();
                        }

                        empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
                        empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
                        empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
                        empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();

                        if (!Convert.IsDBNull(reader["latitude_gdec"]))
                        {
                            empreendimento.Coordenada.LatitudeGdec = Convert.ToDouble(reader["latitude_gdec"]);
                            empreendimento.Coordenada.LatitudeGdecTexto = empreendimento.Coordenada.LatitudeGdec.ToString();
                        }

                        if (!Convert.IsDBNull(reader["longitude_gdec"]))
                        {
                            empreendimento.Coordenada.LongitudeGdec = Convert.ToDouble(reader["longitude_gdec"]);
                            empreendimento.Coordenada.LongitudeGdecTexto = empreendimento.Coordenada.LongitudeGdec.ToString();
                        }

                        empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
                        empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);

                        if (!Convert.IsDBNull(reader["forma_coleta"]))
                        {
                            empreendimento.Coordenada.FormaColeta = Convert.ToInt32(reader["forma_coleta"]);
                        }

                        if (!Convert.IsDBNull(reader["local_coleta"]))
                        {
                            empreendimento.Coordenada.LocalColeta = Convert.ToInt32(reader["local_coleta"]);
                        }

                    }

                    reader.Close();
                }

                #endregion

                #region Meio de Contato

                comando = bancoDeDados.CriarComando(@"select a.id, a.empreendimento, a.meio_contato tipo_contato_id,
				a.valor, a.tid from {0}tab_empreendimento_contato a where  a.empreendimento = :empreendimento", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Contato contato;

                    while (reader.Read())
                    {
                        contato = new Contato();
                        contato.Id = Convert.ToInt32(reader["id"]);
                        contato.PessoaId = Convert.ToInt32(reader["empreendimento"]);
                        contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
                        contato.Valor = reader["valor"].ToString();
                        contato.Tid = reader["tid"].ToString();
                        empreendimento.MeiosContatos.Add(contato);
                    }

                    reader.Close();
                }

                #endregion

                empreendimento.TemCorrespondencia = (empreendimento.Enderecos.Any(x => x.Correspondencia == 1) ? 1 : 0);
            }

            return empreendimento;
        }

        internal void SalvarInternoId(Empreendimento empreendimento, BancoDeDados bancoCredenciado)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
            {
                bancoDeDados.IniciarTransacao();

                #region Empreendimento

                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento e set e.interno = :interno, e.codigo = :codigo, e.interno_tid = :interno_tid, e.tid = :tid 
				where e.id = :id", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("id", empreendimento.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("interno", empreendimento.InternoId, DbType.Int32);
                comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, empreendimento.InternoTid);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("codigo", empreendimento.Codigo, DbType.Int64);

                bancoDeDados.ExecutarNonQuery(comando);

                #endregion
                
                Historico.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        public Municipio ObterMunicipio(int MunicipioId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                bancoDeDados.IniciarTransacao();

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

        internal List<Segmento> ObterSegmentos()
        {
            List<Segmento> lst = new List<Segmento>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.denominador from lov_empreendimento_segmento t order by t.texto");
            foreach (var item in daReader)
            {
                lst.Add(new Segmento()
                {
                    Id = item["id"].ToString(),
                    Texto = item["texto"].ToString(),
                    Denominador = item["denominador"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

        internal List<CoordenadaTipo> ObterTiposCoordenada()
        {
            List<CoordenadaTipo> lst = new List<CoordenadaTipo>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_coordenada_tipo c");
            foreach (var item in daReader)
            {
                lst.Add(new CoordenadaTipo()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

        internal List<Datum> ObterDatuns()
        {
            List<Datum> lst = new List<Datum>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.sigla, c.texto from lov_coordenada_datum c");
            foreach (var item in daReader)
            {
                lst.Add(new Datum()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Sigla = item["sigla"].ToString(),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

        internal List<CoordenadaHemisferio> ObterHemisferios()
        {
            List<CoordenadaHemisferio> lst = new List<CoordenadaHemisferio>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select h.id, h.texto from lov_coordenada_hemisferio h");
            foreach (var item in daReader)
            {
                lst.Add(new CoordenadaHemisferio()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }
            return lst;
        }

        internal List<Lista> ObterFormaColetaPonto()
        {
            List<Lista> lst = new List<Lista>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_empreendimento_forma_colet c order by c.id");

            foreach (var item in daReader)
            {
                lst.Add(new Lista()
                {
                    Id = item["id"].ToString(),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
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

        internal List<TipoResponsavel> ObterEmpTipoResponsavel()
        {
            List<TipoResponsavel> lst = new List<TipoResponsavel>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_empreendimento_tipo_resp t order by t.texto");
            foreach (var item in daReader)
            {
                lst.Add(new TipoResponsavel()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Nome = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

        internal List<ContatoLst> ObterMeiosContato()
        {
            List<ContatoLst> lst = new List<ContatoLst>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.mascara, t.tid from tab_meio_contato t order by t.id");
            foreach (var item in daReader)
            {
                lst.Add(new ContatoLst()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    Mascara = item["mascara"].ToString(),
                    Tid = item["tid"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }
    }
}
