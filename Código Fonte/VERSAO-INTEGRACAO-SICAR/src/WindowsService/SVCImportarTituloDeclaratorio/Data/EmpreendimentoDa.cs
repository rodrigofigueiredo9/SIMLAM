using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class EmpreendimentoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        Consulta _consulta = new Consulta();
        public Consulta Consulta { get { return _consulta; } }

        Historico _historico = new Historico();

        public Historico Historico {get { return _historico; } }

        private string EsquemaBanco { get { return _configSys.UsuarioInterno; } }
        private string EsquemaBancoGeo { get { return _configSys.UsuarioGeo; } }

        public EmpreendimentoDa() { }

        public Empreendimento Obter(int id, BancoDeDados banco)
        {
            Empreendimento empreendimento = new Empreendimento();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Empreendimento

                Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.segmento, ls.texto segmento_texto, ls.denominador segmento_denominador, e.cnpj, e.denominador, 
				e.nome_fantasia, e.atividade, a.atividade atividade_texto, e.tid from {0}tab_empreendimento e, {0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento ls 
				where e.atividade = a.id(+) and e.segmento = ls.id and e.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        empreendimento.Id = id;
                        empreendimento.Tid = reader.GetValue<string>("tid");
                        empreendimento.Codigo = reader.GetValue<long>("codigo");

                        if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
                        {
                            empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
                            empreendimento.SegmentoTexto = reader["segmento_texto"].ToString();
                            empreendimento.SegmentoDenominador = reader["segmento_denominador"].ToString();
                        }

                        empreendimento.CNPJ = reader["cnpj"].ToString();
                        empreendimento.Denominador = reader["denominador"].ToString();
                        empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

                        if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
                        {
                            empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
                            empreendimento.Atividade.Atividade = reader["atividade_texto"].ToString();
                        }
                    }

                    reader.Close();
                }

                #endregion

                #region Responsáveis

                comando = bancoDeDados.CriarComando(@"select pr.id, pr.empreendimento, pr.responsavel, nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf_cnpj, pr.tipo, 
				lr.texto tipo_texto, pr.data_vencimento, pr.especificar, pr.origem, pr.origem_texto, pr.credenciado_usuario_id 
				from {0}tab_empreendimento_responsavel pr, {0}tab_pessoa p, {0}lov_empreendimento_tipo_resp lr where pr.responsavel = p.id and pr.tipo = lr.id 
				and pr.empreendimento = :empreendimento", EsquemaBanco);

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
                        responsavel.TipoTexto = reader["tipo_texto"].ToString();

                        if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
                        {
                            responsavel.DataVencimento = Convert.ToDateTime(reader["data_vencimento"]);
                        }

                        responsavel.NomeRazao = reader["nome"].ToString();
                        responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
                        responsavel.EspecificarTexto = reader["especificar"].ToString();

                        responsavel.Origem = reader.GetValue<int>("origem");
                        responsavel.OrigemTexto = reader.GetValue<string>("origem_texto");
                        responsavel.CredenciadoUsuarioId = reader.GetValue<int>("credenciado_usuario_id");

                        empreendimento.Responsaveis.Add(responsavel);
                    }

                    reader.Close();
                }

                #endregion

                #region Endereços

                comando = bancoDeDados.CriarComando(@"select te.id, te.empreendimento, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro, le.id estado_id, le.texto estado_texto, lm.id municipio_id, 
				lm.texto municipio_texto, te.numero, te.distrito, te.corrego, te.caixa_postal, te.complemento, te.tid from {0}tab_empreendimento_endereco te, {0}lov_estado le, {0}lov_municipio lm where te.estado = le.id(+) 
				and te.municipio = lm.id(+) and te.empreendimento = :empreendimento order by te.correspondencia", EsquemaBanco);

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
                        end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
                        end.EstadoTexto = reader["estado_texto"].ToString();
                        end.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
                        end.MunicipioTexto = reader["municipio_texto"].ToString();
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

                comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, lct.texto tipoTexto, aec.datum, lcd.texto datumTexto, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, lch.texto hemisferioTexto, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec,
				aec.forma_coleta,   aec.local_coleta  from tab_empreendimento_coord aec, lov_coordenada_datum lcd, lov_coordenada_tipo lct, lov_coordenada_hemisferio lch
				where aec.datum = lcd.id(+) and aec.hemisferio_utm = lch.id(+) and aec.tipo_coordenada = lct.id(+) and aec.empreendimento = :empreendimentoid", EsquemaBanco);

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

                        if (!Convert.IsDBNull(reader["datumtexto"]))
                        {
                            empreendimento.Coordenada.Datum.Texto = reader["datumtexto"].ToString();
                        }

                        if (!Convert.IsDBNull(reader["tipotexto"]))
                        {
                            empreendimento.Coordenada.Tipo.Texto = reader["tipotexto"].ToString();
                        }

                        if (!Convert.IsDBNull(reader["hemisferiotexto"]))
                        {
                            empreendimento.Coordenada.HemisferioUtmTexto = reader["hemisferiotexto"].ToString();
                        }
                    }

                    reader.Close();
                }

                #endregion

                #region Meio de Contato

                comando = bancoDeDados.CriarComando(@"select a.id, b.mascara, a.empreendimento, b.id tipo_contato_id, b.texto contato_texto,
				a.valor, a.tid from {0}tab_empreendimento_contato a, {0}tab_meio_contato b where a.meio_contato = b.id and a.empreendimento = :empreendimento", EsquemaBanco);
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
                        contato.TipoTexto = reader["contato_texto"].ToString();
                        contato.Mascara = reader["mascara"].ToString();
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

        public Empreendimento ObterHistorico(int id, string tid, BancoDeDados bancoInterno)
        {
            Empreendimento empreendimento = new Empreendimento();

            int id_hst = 0;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Empreendimento

                Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.segmento_id segmento, e.segmento_texto, e.cnpj, ls.denominador 
															segmento_denominador, e.denominador, e.nome_fantasia, e.atividade_id atividade, 
															a.atividade atividade_texto from {0}hst_empreendimento e, {0}hst_empreendimento_atividade a, 
															lov_empreendimento_segmento  ls where e.atividade_id = a.atividade_id(+) 
															and ls.id = e.segmento_id and e.empreendimento_id = :id and e.tid = :tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", tid, DbType.String);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        empreendimento.Id = id;
                        empreendimento.Tid = tid;
                        id_hst = reader.GetValue<Int32>("id");
                        empreendimento.Codigo = reader.GetValue<Int64>("codigo");

                        if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
                        {
                            empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
                            empreendimento.SegmentoTexto = reader["segmento_texto"].ToString();
                            empreendimento.SegmentoDenominador = reader["segmento_denominador"].ToString();
                        }

                        empreendimento.CNPJ = reader["cnpj"].ToString();
                        empreendimento.Denominador = reader["denominador"].ToString();
                        empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

                        if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
                        {
                            empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
                            empreendimento.Atividade.Atividade = reader["atividade_texto"].ToString();
                        }
                    }

                    reader.Close();
                }

                #endregion

                #region Responsáveis

                comando = bancoDeDados.CriarComando(@"select pr.tid, pr.emp_resp_id id, pr.empreendimento_id, pr.responsavel_id responsavel, pr.responsavel_tid,
				nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf_cnpj, pr.tipo_id tipo, pr.tipo_texto, pr.data_vencimento, pr.especificar, pr.origem, pr.origem_texto, 
				pr.credenciado_usuario_id from {0}hst_empreendimento_responsavel pr, {0}hst_pessoa p where pr.responsavel_id = p.pessoa_id and p.tid = pr.responsavel_tid 
				and pr.empreendimento_id = :empreendimento and pr.tid = :tid and pr.id_hst = :id_hst", EsquemaBanco);

                comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Responsavel responsavel;

                    while (reader.Read())
                    {
                        responsavel = new Responsavel();
                        responsavel.Id = Convert.ToInt32(reader["responsavel"]);
                        responsavel.IdRelacionamento = Convert.ToInt32(reader["id"]);
                        responsavel.Tipo = Convert.ToInt32(reader["tipo"]);
                        responsavel.TipoTexto = reader["tipo_texto"].ToString();
                        responsavel.Tid = reader["responsavel_tid"].ToString();

                        if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
                        {
                            responsavel.DataVencimento = Convert.ToDateTime(reader["data_vencimento"]);
                        }

                        responsavel.NomeRazao = reader["nome"].ToString();
                        responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
                        responsavel.EspecificarTexto = reader["especificar"].ToString();

                        responsavel.Origem = reader.GetValue<int>("origem");
                        responsavel.OrigemTexto = reader.GetValue<string>("origem_texto");
                        responsavel.CredenciadoUsuarioId = reader.GetValue<int>("credenciado_usuario_id");

                        empreendimento.Responsaveis.Add(responsavel);
                    }

                    reader.Close();
                }

                #endregion

                #region Endereços

                comando = bancoDeDados.CriarComando(@"select te.endereco_id id, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro,
													te.estado_id, te.estado_texto, te.municipio_id, te.municipio_texto, te.numero, te.distrito,
													te.corrego, te.caixa_postal, te.complemento, te.tid from {0}hst_empreendimento_endereco te
													where te.id_hst = :id_hst and te.tid = :tid order by te.correspondencia", EsquemaBanco);

                comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

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
                        end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
                        end.EstadoTexto = reader["estado_texto"].ToString();
                        end.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
                        end.MunicipioTexto = reader["municipio_texto"].ToString();
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

                comando = bancoDeDados.CriarComando(@"select aec.coordenada_id id, aec.tid, aec.tipo_coordenada_id tipo_coordenada, aec.tipo_coordenada_texto tipotexto,
													aec.datum_id datum, aec.datum_texto datumTexto, aec.easting_utm, aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm_id hemisferio_utm,
													aec.hemisferio_utm_texto hemisferioTexto, aec.latitude_gms, aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec,
													aec.forma_coleta_id forma_coleta, aec.local_coleta_id local_coleta from {0}hst_empreendimento_coord  aec where aec.id_hst = :id_hst
													and aec.tid = :tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

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

                        if (!Convert.IsDBNull(reader["datumtexto"]))
                        {
                            empreendimento.Coordenada.Datum.Texto = reader["datumtexto"].ToString();
                        }

                        if (!Convert.IsDBNull(reader["tipotexto"]))
                        {
                            empreendimento.Coordenada.Tipo.Texto = reader["tipotexto"].ToString();
                        }

                        if (!Convert.IsDBNull(reader["hemisferiotexto"]))
                        {
                            empreendimento.Coordenada.HemisferioUtmTexto = reader["hemisferiotexto"].ToString();
                        }
                    }

                    reader.Close();
                }

                #endregion

                #region Meio de Contato

                comando = bancoDeDados.CriarComando(@"select a.emp_contato_id id, b.mascara, b.id tipo_contato_id, b.texto contato_texto,
													a.valor, a.tid from {0}hst_empreendimento_contato a, {0}tab_meio_contato b
													where a.meio_contato_id = b.id and a.id_hst = :id_hst and a.tid = :tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Contato contato;

                    while (reader.Read())
                    {
                        contato = new Contato();
                        contato.Id = Convert.ToInt32(reader["id"]);
                        contato.PessoaId = empreendimento.Id;
                        contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
                        contato.TipoTexto = reader["contato_texto"].ToString();
                        contato.Mascara = reader["mascara"].ToString();
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

        public int ObterId(string cnpj, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select te.id from {0}tab_empreendimento te where te.cnpj = :cnpj", EsquemaBanco);

                comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);

                object ret = bancoDeDados.ExecutarScalar(comando);

                return (ret != null && !Convert.IsDBNull(ret) && Convert.ToInt32(ret) > 0) ? Convert.ToInt32(ret) : 0;
            }
        }

        internal int Criar(Empreendimento empreendimento, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                #region Empreendimento

                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento e (id, codigo, segmento, cnpj, denominador, nome_fantasia, atividade, tid) 
				values ({0}seq_empreendimento.nextval, {0}seq_empreendimento_codigo.nextval, :segmento, :cnpj, :denominador, :nome_fantasia, :atividade, :tid) 
				returning e.id, e.codigo into :id, :codigo", EsquemaBanco);

                comando.AdicionarParametroEntrada("cnpj", DbType.String, 20, empreendimento.CNPJ);
                comando.AdicionarParametroEntrada("denominador", DbType.String, 100, empreendimento.Denominador);
                comando.AdicionarParametroEntrada("nome_fantasia", DbType.String, 100, empreendimento.NomeFantasia);
                comando.AdicionarParametroEntrada("segmento", empreendimento.Segmento, DbType.Int32);
                comando.AdicionarParametroEntrada("atividade", (empreendimento.Atividade.Id > 0) ? empreendimento.Atividade.Id : (object)DBNull.Value, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);
                comando.AdicionarParametroSaida("codigo", DbType.Int64);

                bancoDeDados.ExecutarNonQuery(comando);

                empreendimento.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
                empreendimento.Tid = GerenciadorTransacao.ObterIDAtual();
                empreendimento.Codigo = Convert.ToInt64(comando.ObterValorParametro("codigo"));

                #endregion

                #region Responsáveis

                if (empreendimento.Responsaveis.Count > 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_responsavel(id, empreendimento, responsavel, tipo, data_vencimento, especificar, 
					origem, origem_texto, credenciado_usuario_id, tid) values({0}seq_empreendimento_responsavel.nextval, :empreendimento, :responsavel, :tipo, :data_vencimento, 
					:especificar, :origem, :origem_texto, :credenciado_usuario_id, :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("responsavel", DbType.Int32);
                    comando.AdicionarParametroEntrada("tipo", DbType.Int32);
                    comando.AdicionarParametroEntrada("data_vencimento", DbType.DateTime);
                    comando.AdicionarParametroEntrada("especificar", DbType.String);
                    comando.AdicionarParametroEntrada("credenciado_usuario_id", DbType.Int32);
                    comando.AdicionarParametroEntrada("origem", DbType.Int32);
                    comando.AdicionarParametroEntrada("origem_texto", DbType.String);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    foreach (Responsavel item in empreendimento.Responsaveis)
                    {
                        comando.SetarValorParametro("responsavel", item.Id);
                        comando.SetarValorParametro("tipo", item.Tipo);
                        comando.SetarValorParametro("data_vencimento", (item.DataVencimento.HasValue && item.DataVencimento.Value != DateTime.MinValue) ? item.DataVencimento : null);
                        comando.SetarValorParametro("especificar", (String.IsNullOrWhiteSpace(item.EspecificarTexto) ? (Object)DBNull.Value : item.EspecificarTexto));
                        comando.SetarValorParametro("credenciado_usuario_id", item.CredenciadoUsuarioId);
                        comando.SetarValorParametro("origem", item.Origem);
                        comando.SetarValorParametro("origem_texto", item.OrigemTexto);
                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                #endregion

                #region Endereço

                if (empreendimento.Enderecos != null && empreendimento.Enderecos.Count > 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_endereco (id, empreendimento, correspondencia, zona, cep, logradouro, bairro, estado, municipio, numero, distrito, corrego, caixa_postal, complemento, tid)
					values({0}seq_empreendimento_endereco.nextval, :empreendimento, :correspondencia, :zona, :cep, :logradouro, :bairro, :estado, :municipio, :numero, :distrito, :corrego, :caixa_postal, :complemento, :tid )", EsquemaBanco);

                    comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("correspondencia", DbType.Int32);
                    comando.AdicionarParametroEntrada("zona", DbType.Int32);
                    comando.AdicionarParametroEntrada("cep", DbType.String, 15);
                    comando.AdicionarParametroEntrada("logradouro", DbType.String, 500);
                    comando.AdicionarParametroEntrada("bairro", DbType.String, 100);
                    comando.AdicionarParametroEntrada("estado", DbType.Int32);
                    comando.AdicionarParametroEntrada("municipio", DbType.Int32);
                    comando.AdicionarParametroEntrada("numero", DbType.String, 6);
                    comando.AdicionarParametroEntrada("distrito", DbType.String, 100);
                    comando.AdicionarParametroEntrada("corrego", DbType.String, 100);
                    comando.AdicionarParametroEntrada("caixa_postal", DbType.String, 50);
                    comando.AdicionarParametroEntrada("complemento", DbType.String, 500);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    foreach (Endereco item in empreendimento.Enderecos)
                    {
                        comando.SetarValorParametro("correspondencia", item.Correspondencia);
                        comando.SetarValorParametro("zona", item.ZonaLocalizacaoId);
                        comando.SetarValorParametro("cep", item.Cep);
                        comando.SetarValorParametro("logradouro", item.Logradouro);
                        comando.SetarValorParametro("bairro", item.Bairro);
                        comando.SetarValorParametro("estado", item.EstadoId);
                        comando.SetarValorParametro("municipio", item.MunicipioId);
                        comando.SetarValorParametro("numero", item.Numero);
                        comando.SetarValorParametro("distrito", item.DistritoLocalizacao);
                        comando.SetarValorParametro("corrego", item.Corrego);
                        comando.SetarValorParametro("caixa_postal", item.CaixaPostal);
                        comando.SetarValorParametro("complemento", item.Complemento);

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                #endregion

                #region Coordenadas/Geometria

                if (empreendimento.Coordenada.Tipo.Id > 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_coord
					(id, empreendimento, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, latitude_gms, longitude_gms, latitude_gdec, longitude_gdec, forma_coleta, local_coleta, tid)
					values ({0}seq_empreendimento_coord.nextval, :empreendimento, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :latitude_gms, 
					:longitude_gms, :latitude_gdec, :longitude_gdec, :forma_coleta, :local_coleta, :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tipo_coordenada", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("easting_utm", empreendimento.Coordenada.EastingUtm, DbType.Double);
                    comando.AdicionarParametroEntrada("northing_utm", empreendimento.Coordenada.NorthingUtm, DbType.Double);
                    comando.AdicionarParametroEntrada("fuso_utm", empreendimento.Coordenada.FusoUtm, DbType.Int32);
                    comando.AdicionarParametroEntrada("hemisferio_utm", empreendimento.Coordenada.HemisferioUtm, DbType.Int32);
                    comando.AdicionarParametroEntrada("latitude_gms", DbType.String, 15, empreendimento.Coordenada.LatitudeGms);
                    comando.AdicionarParametroEntrada("longitude_gms", DbType.String, 15, empreendimento.Coordenada.LongitudeGms);
                    comando.AdicionarParametroEntrada("latitude_gdec", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
                    comando.AdicionarParametroEntrada("longitude_gdec", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
                    comando.AdicionarParametroEntrada("forma_coleta", empreendimento.Coordenada.FormaColeta, DbType.Int32);
                    comando.AdicionarParametroEntrada("local_coleta", empreendimento.Coordenada.LocalColeta, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);

                    #region Geometria

                    comando = bancoDeDados.CriarComando(@"declare"
                        + " v_x number;"
                        + " v_y number;"
                        + " v_fuso number;"
                        + " v_hemisferio number;"
                        + " geo mdsys.sdo_geometry;"
                    + " begin"
                        + " case :tipo"
                            + " when 1 then /*Grau, minuto e Segundo : Geografico*/"
                                + " {0}coordenada.gms2gdec({0}coordenada.formatagms(:x), {0}coordenada.formatagms(:y), v_x, v_y);"
                            + " when 2 then /*Grau decimal: Geografico*/"
                                + " v_x := :x;"
                                + " v_y := :y;"
                            + " when 3 then /*UTM: Projetado*/"
                                + " {0}coordenada.utm2gdec(:datum_sigla, :x, :y, :fuso, :hemisferio, v_x, v_y);"
                        + " end case;"
                        + " geo := {0}coordenada.gdec2spatialpoint(v_x, v_y, :datum);"
                        + " insert into {0}geo_emp_localizacao(id,empreendimento,geometry,tid) values ({0}seq_geo_emp_localizacao.nextval, :empreendimento, geo, :tid);"
                    + " end;", EsquemaBancoGeo);

                    comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tipo", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("datum_sigla", empreendimento.Coordenada.Datum.Sigla, DbType.String);
                    comando.AdicionarParametroEntrada("fuso", DbType.Int32);
                    comando.AdicionarParametroEntrada("hemisferio", DbType.Int32);

                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    if (empreendimento.Coordenada.Tipo.Id == 1)
                    {
                        comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGms, DbType.String);
                        comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGms, DbType.String);
                    }
                    else if (empreendimento.Coordenada.Tipo.Id == 2)
                    {
                        comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
                        comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
                    }
                    else
                    {
                        comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.EastingUtm, DbType.Double);
                        comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.NorthingUtm, DbType.Double);

                        comando.SetarValorParametro("fuso", empreendimento.Coordenada.FusoUtm.Value);
                        comando.SetarValorParametro("hemisferio", empreendimento.Coordenada.HemisferioUtm.Value);
                    }

                    bancoDeDados.ExecutarNonQuery(comando);
                    Comando cmd = bancoDeDados.CriarComando("alter session set NLS_NUMERIC_CHARACTERS = ',.'");
                    bancoDeDados.ExecutarNonQuery(cmd);

                    #endregion
                }

                #endregion

                #region Meios de contato

                if (empreendimento.MeiosContatos != null && empreendimento.MeiosContatos.Count > 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_contato(id, empreendimento, meio_contato, valor, tid) values
					({0}seq_empreendimento_contato.nextval, :empreendimento, :meio_contato, :valor, : tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("meio_contato", DbType.Int32);
                    comando.AdicionarParametroEntrada("valor", DbType.String, 1000);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    foreach (Contato item in empreendimento.MeiosContatos)
                    {
                        comando.SetarValorParametro("meio_contato", Convert.ToInt32(item.TipoContato));
                        comando.SetarValorParametro("valor", item.Valor);
                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                #endregion

                #region Histórico/Consulta/Posse
                //Historico
                Historico.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, eHistoricoAcao.criar, bancoDeDados);
                Consulta.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, bancoDeDados);

                #endregion

                bancoDeDados.Commit();

                return empreendimento.Id;
            }
        }

        internal void Editar(Empreendimento empreendimento, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Empreendimento
                

                Comando comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento e set e.segmento = :segmento, e.cnpj = :cnpj, e.denominador = :denominador, 
				e.nome_fantasia = :nome_fantasia, e.atividade = :atividade, e.tid = :tid where e.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("cnpj", DbType.String, 20, empreendimento.CNPJ);
                comando.AdicionarParametroEntrada("denominador", DbType.String, 100, empreendimento.Denominador);
                comando.AdicionarParametroEntrada("nome_fantasia", DbType.String, 100, empreendimento.NomeFantasia);
                comando.AdicionarParametroEntrada("segmento", empreendimento.Segmento ?? (object)DBNull.Value, DbType.Int32);
                comando.AdicionarParametroEntrada("atividade", (empreendimento.Atividade.Id > 0) ? empreendimento.Atividade.Id : (object)DBNull.Value, DbType.Int32);
                comando.AdicionarParametroEntrada("id", empreendimento.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);
                empreendimento.Tid = GerenciadorTransacao.ObterIDAtual();

                #endregion

                #region Limpar os dados do banco

                //Meios de Contato
                comando = bancoDeDados.CriarComando("delete from {0}tab_empreendimento_contato c ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where c.empreendimento = :empreendimento{0}",
                comando.AdicionarNotIn("and", "c.id", DbType.Int32, empreendimento.MeiosContatos.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                //Responsavel
                comando = bancoDeDados.CriarComando("delete from {0}tab_empreendimento_responsavel c ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where c.empreendimento = :empreendimento{0}",
                comando.AdicionarNotIn("and", "c.id", DbType.Int32, empreendimento.Responsaveis.Select(x => x.IdRelacionamento).ToList()));
                comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                //Endereço
                comando = bancoDeDados.CriarComando("delete from {0}tab_empreendimento_endereco c ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where c.empreendimento = :empreendimento{0}",
                comando.AdicionarNotIn("and", "c.id", DbType.Int32, empreendimento.Enderecos.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                #region Responsáveis

                if (empreendimento.Responsaveis != null && empreendimento.Responsaveis.Count > 0)
                {
                    foreach (Responsavel item in empreendimento.Responsaveis)
                    {
                        if (item.IdRelacionamento > 0)
                        {
                            comando = bancoDeDados.CriarComando(@"update tab_empreendimento_responsavel r set r.empreendimento = :empreendimento, r.responsavel = :responsavel, 
							r.tipo = :tipo, r.data_vencimento = :data_vencimento, r.especificar = :especificar, r.origem = :origem, r.origem_texto = :origem_texto, 
							r.credenciado_usuario_id = :credenciado_usuario_id, r.tid = :tid where r.id = :id", EsquemaBanco);

                            comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
                        }
                        else
                        {
                            comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_responsavel(id, empreendimento, responsavel, tipo, data_vencimento, especificar, 
							origem, origem_texto, credenciado_usuario_id, tid) values ({0}seq_empreendimento_responsavel.nextval, :empreendimento, :responsavel, :tipo, 
							:data_vencimento, :especificar, :origem, :origem_texto, :credenciado_usuario_id, :tid )", EsquemaBanco);
                        }

                        comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("responsavel", item.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
                        comando.AdicionarParametroEntrada("data_vencimento", (item.DataVencimento.HasValue && item.DataVencimento.Value != DateTime.MinValue) ? item.DataVencimento : null, DbType.DateTime);
                        comando.AdicionarParametroEntrada("especificar", (String.IsNullOrWhiteSpace(item.EspecificarTexto) ? (Object)DBNull.Value : item.EspecificarTexto), DbType.String);
                        comando.AdicionarParametroEntrada("credenciado_usuario_id", item.CredenciadoUsuarioId, DbType.Int32);
                        comando.AdicionarParametroEntrada("origem", item.Origem, DbType.Int32);
                        comando.AdicionarParametroEntrada("origem_texto", DbType.String, 200, item.OrigemTexto);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"delete {0}tab_empreendimento_responsavel p where p.empreendimento = :id", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id", empreendimento.Id, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Endereço

                if (empreendimento.Enderecos != null && empreendimento.Enderecos.Count > 0)
                {
                    foreach (Endereco item in empreendimento.Enderecos)
                    {
                        if (item.Id > 0)
                        {
                            comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento_endereco e set e.empreendimento = :empreendimento, e.correspondencia = :correspondencia, e.zona = :zona, 
							e.cep = :cep, e.logradouro = :logradouro, e.bairro = :bairro, e.estado = :estado, e.municipio = :municipio, e.numero = :numero, e.distrito =:distrito, e.corrego = :corrego, 
							e.caixa_postal = :caixa_postal, e.complemento = :complemento, e.tid =:tid where e.id = :id", EsquemaBanco);
                            comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
                        }
                        else
                        {
                            comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_endereco (id, empreendimento, correspondencia, zona, cep, logradouro, bairro, estado, municipio, numero, distrito, corrego,
							caixa_postal, complemento, tid) values({0}seq_empreendimento_endereco.nextval, :empreendimento, :correspondencia, :zona, :cep, :logradouro, :bairro, :estado, :municipio, :numero, :distrito, :corrego, :caixa_postal, :complemento, :tid )", EsquemaBanco);
                        }

                        comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("correspondencia", item.Correspondencia, DbType.Int32);
                        comando.AdicionarParametroEntrada("zona", item.ZonaLocalizacaoId, DbType.Int32);
                        comando.AdicionarParametroEntrada("cep", DbType.String, 15, item.Cep);
                        comando.AdicionarParametroEntrada("logradouro", DbType.String, 500, item.Logradouro);
                        comando.AdicionarParametroEntrada("bairro", DbType.String, 100, item.Bairro);
                        comando.AdicionarParametroEntrada("estado", item.EstadoId, DbType.Int32);
                        comando.AdicionarParametroEntrada("municipio", item.MunicipioId, DbType.Int32);
                        comando.AdicionarParametroEntrada("numero", DbType.String, 6, item.Numero);
                        comando.AdicionarParametroEntrada("distrito", DbType.String, 100, item.DistritoLocalizacao);
                        comando.AdicionarParametroEntrada("corrego", DbType.String, 100, item.Corrego);
                        comando.AdicionarParametroEntrada("caixa_postal", DbType.String, 50, item.CaixaPostal);
                        comando.AdicionarParametroEntrada("complemento", DbType.String, 500, item.Complemento);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                #endregion

                #region Coordenadas/Geometria

                if (empreendimento.Coordenada.Tipo.Id > 0)
                {
                    if (empreendimento.Coordenada.Id > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento_coord tc set tipo_coordenada = :tipo_coordenada, datum = :datum, easting_utm = :easting_utm, 
						northing_utm = :northing_utm, fuso_utm = :fuso_utm, hemisferio_utm = :hemisferio_utm, latitude_gms = :latitude_gms, longitude_gms = :longitude_gms, latitude_gdec = :latitude_gdec, 
						longitude_gdec = :longitude_gdec, forma_coleta = :forma_coleta, local_coleta = :local_coleta, tid = :tid where empreendimento = :empreendimento", EsquemaBanco);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_coord
						(id, empreendimento, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, latitude_gms, longitude_gms, latitude_gdec, longitude_gdec, forma_coleta, local_coleta, tid)
						values ({0}seq_empreendimento_coord.nextval, :empreendimento, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :latitude_gms, 
						:longitude_gms, :latitude_gdec, :longitude_gdec, :forma_coleta, :local_coleta, :tid)", EsquemaBanco);
                    }

                    comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tipo_coordenada", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("easting_utm", empreendimento.Coordenada.EastingUtm, DbType.Double);
                    comando.AdicionarParametroEntrada("northing_utm", empreendimento.Coordenada.NorthingUtm, DbType.Double);
                    comando.AdicionarParametroEntrada("fuso_utm", empreendimento.Coordenada.FusoUtm, DbType.Int32);
                    comando.AdicionarParametroEntrada("hemisferio_utm", empreendimento.Coordenada.HemisferioUtm, DbType.Int32);
                    comando.AdicionarParametroEntrada("latitude_gms", DbType.String, 15, empreendimento.Coordenada.LatitudeGms);
                    comando.AdicionarParametroEntrada("longitude_gms", DbType.String, 15, empreendimento.Coordenada.LongitudeGms);
                    comando.AdicionarParametroEntrada("latitude_gdec", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
                    comando.AdicionarParametroEntrada("longitude_gdec", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
                    comando.AdicionarParametroEntrada("forma_coleta", empreendimento.Coordenada.FormaColeta, DbType.Int32);
                    comando.AdicionarParametroEntrada("local_coleta", empreendimento.Coordenada.LocalColeta, DbType.Int32);

                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);

                    #region Geometria

                    comando = bancoDeDados.CriarComando(@"declare"
                        + " v_x number;"
                        + " v_y number;"
                        + " v_id number;"
                        + " v_fuso number;"
                        + " v_hemisferio number;"
                        + " geo mdsys.sdo_geometry;"
                    + " begin"
                        + " case :tipo"
                            + " when 1 then /*Grau, minuto e Segundo : Geografico*/"
                                + " {0}coordenada.gms2gdec({0}coordenada.formatagms(:x), {0}coordenada.formatagms(:y), v_x, v_y);"
                            + " when 2 then /*Grau decimal: Geografico*/"
                                + " v_x := :x;"
                                + " v_y := :y;"
                            + " when 3 then /*UTM: Projetado*/"
                                + " {0}coordenada.utm2gdec(:datum_sigla, :x, :y, :fuso, :hemisferio, v_x, v_y);"
                        + " end case;"
                        + " geo := {0}coordenada.gdec2spatialpoint(v_x, v_y, :datum);"
                        + " select count(*) into v_id from {0}geo_emp_localizacao g where g.empreendimento = :empreendimento;"
                        + " if(v_id=0)then"
                            + " insert into {0}geo_emp_localizacao(id,empreendimento,geometry,tid) values ({0}seq_geo_emp_localizacao.nextval, :empreendimento, geo, :tid);"
                        + " else"
                            + " update {0}geo_emp_localizacao g set g.geometry = geo, g.tid = tid where g.empreendimento = :empreendimento;"
                        + " end if;"
                    + " end;", EsquemaBancoGeo);

                    comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tipo", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("datum_sigla", empreendimento.Coordenada.Datum.Sigla, DbType.String);
                    comando.AdicionarParametroEntrada("fuso", DbType.Int32);
                    comando.AdicionarParametroEntrada("hemisferio", DbType.Int32);

                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    if (empreendimento.Coordenada.Tipo.Id == 1)
                    {
                        comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGms, DbType.String);
                        comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGms, DbType.String);
                    }
                    else if (empreendimento.Coordenada.Tipo.Id == 2)
                    {
                        comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
                        comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
                    }
                    else
                    {
                        comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.EastingUtm, DbType.Double);
                        comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.NorthingUtm, DbType.Double);

                        comando.SetarValorParametro("fuso", empreendimento.Coordenada.FusoUtm.Value);
                        comando.SetarValorParametro("hemisferio", empreendimento.Coordenada.HemisferioUtm.Value);

                    }

                    bancoDeDados.ExecutarNonQuery(comando);
                    Comando cmd = bancoDeDados.CriarComando("alter session set NLS_NUMERIC_CHARACTERS = ',.'");
                    bancoDeDados.ExecutarNonQuery(cmd);

                    #endregion
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"delete from {0}tab_empreendimento_coord ae where ae.empreendimento = :id", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id", empreendimento.Id, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Meios de contato

                if (empreendimento.MeiosContatos != null && empreendimento.MeiosContatos.Count > 0)
                {
                    foreach (Contato item in empreendimento.MeiosContatos)
                    {
                        if (item.Id > 0)
                        {
                            comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento_contato tec set tec.meio_contato = :meio_contato, tec.valor = :valor, tec.tid = :tid
							where tec.empreendimento = :empreendimento and tec.id = :id", EsquemaBanco);
                            comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
                        }
                        else
                        {
                            comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_contato(id, empreendimento, meio_contato, valor, tid) values 
							({0}seq_empreendimento_contato.nextval, :empreendimento, :meio_contato, :valor, :tid)", EsquemaBanco);
                        }

                        comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("meio_contato", Convert.ToInt32(item.TipoContato), DbType.Int32);
                        comando.AdicionarParametroEntrada("valor", DbType.String, 1000, item.Valor);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                #endregion

                #region Histórico/Consulta/Posse
                //Historico
                Historico.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, eHistoricoAcao.atualizar, bancoDeDados);
                Consulta.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, bancoDeDados);

                #endregion

                bancoDeDados.Commit();
            }
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
    }
}
