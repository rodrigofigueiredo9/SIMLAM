using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{	
	public class LocalInfracaoDa
	{		
		#region Propriedade e Atributos

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public String EsquemaBancoGeo { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }

		#endregion

		public LocalInfracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}
		
		#region Ações de DML

		public int Salvar(LocalInfracao localInfracao, BancoDeDados banco = null)
		{
			if (localInfracao == null)
			{
				throw new Exception("Local Infração é nulo.");
			}

			if (localInfracao.Id <= 0)
			{
				Criar(localInfracao, banco);
			}
			else
			{
				Editar(localInfracao, banco);
			}

			return localInfracao.Id;
		}
		
		public int Criar(LocalInfracao localInfracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}tab_fisc_local_infracao (id, fiscalizacao, setor, sis_coord, datum, area_abrang, lat_northing, lon_easting, hemisferio, 
				municipio, pessoa, empreendimento, tid, local, responsavel, resp_propriedade, area_fiscalizacao, assinante)
				values
				({0}seq_tab_fisc_local_infracao.nextval, :fiscalizacao, :setor, :sis_coord, :datum, :area_abrang, :lat_northing, :lon_easting, :hemisferio, 
				:municipio, :pessoa, :empreendimento, :tid, :local, :responsavel, :resp_propriedade, :area_fiscalizacao, :assinante) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", localInfracao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", localInfracao.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("sis_coord", localInfracao.SistemaCoordId, DbType.Int32);
				comando.AdicionarParametroEntrada("datum", localInfracao.Datum, DbType.Int32);
				comando.AdicionarParametroEntrada("area_abrang", localInfracao.AreaAbrangencia, DbType.Int32);
				comando.AdicionarParametroEntrada("lat_northing", localInfracao.LatNorthingToDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("lon_easting", localInfracao.LonEastingToDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("hemisferio", localInfracao.Hemisferio, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", (localInfracao.MunicipioId == 0 ? null : (int?)localInfracao.MunicipioId), DbType.Int32);
				comando.AdicionarParametroEntrada("pessoa", localInfracao.PessoaId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", localInfracao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                //comando.AdicionarParametroEntrada("data", localInfracao.Data.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("local", DbType.String, 150, localInfracao.Local);
				comando.AdicionarParametroEntrada("responsavel", localInfracao.ResponsavelId, DbType.Int32);
				comando.AdicionarParametroEntrada("resp_propriedade", localInfracao.ResponsavelPropriedadeId, DbType.Int32);
                comando.AdicionarParametroEntrada("area_fiscalizacao", localInfracao.AreaFiscalizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("assinante", (localInfracao.AssinantePropriedadeId == 0 ? null : localInfracao.AssinantePropriedadeId), DbType.Int32);

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				localInfracao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

                if (localInfracao.MunicipioId != 0 && localInfracao.MunicipioId != null)
                {
                    AtualizarGeoLocalizacao(localInfracao, bancoDeDados);
                }

				bancoDeDados.Commit();
			}

			return localInfracao.Id;
		}

		public void Editar(LocalInfracao localInfracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
					update {0}tab_fisc_local_infracao t
					   set t.setor             = :setor,
						   t.sis_coord         = :sis_coord,
						   t.datum             = :datum,
						   t.area_abrang       = :area_abrang,
						   t.lat_northing      = :lat_northing,
						   t.lon_easting       = :lon_easting,
						   t.hemisferio        = :hemisferio,
						   t.municipio         = :municipio,
						   t.pessoa            = :pessoa,
						   t.empreendimento    = :empreendimento,
						   t.tid               = :tid,
						   t.local             = :local,
						   t.responsavel       = :responsavel,
						   t.resp_propriedade  = :resp_propriedade,
                           t.area_fiscalizacao = :area_fiscalizacao,
						   t.assinante         = :assinante
					 where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("setor", localInfracao.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("sis_coord", localInfracao.SistemaCoordId, DbType.Int32);
				comando.AdicionarParametroEntrada("datum", localInfracao.Datum, DbType.Int32);
				comando.AdicionarParametroEntrada("area_abrang", localInfracao.AreaAbrangencia, DbType.Int32);
				comando.AdicionarParametroEntrada("lat_northing", localInfracao.LatNorthingToDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("lon_easting", localInfracao.LonEastingToDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("hemisferio", localInfracao.Hemisferio, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", (localInfracao.MunicipioId == 0 ? null : (int?)localInfracao.MunicipioId), DbType.Int32);
				comando.AdicionarParametroEntrada("pessoa", localInfracao.PessoaId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", localInfracao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                //comando.AdicionarParametroEntrada("data", localInfracao.Data.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("local", DbType.String, 150, localInfracao.Local);
				comando.AdicionarParametroEntrada("responsavel", localInfracao.ResponsavelId, DbType.Int32);
				comando.AdicionarParametroEntrada("resp_propriedade", localInfracao.ResponsavelPropriedadeId, DbType.Int32);
                comando.AdicionarParametroEntrada("area_fiscalizacao", localInfracao.AreaFiscalizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("assinante", localInfracao.AssinantePropriedadeId == 0 ? null : localInfracao.AssinantePropriedadeId, DbType.Int32);

				comando.AdicionarParametroEntrada("id", localInfracao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

                if (localInfracao.MunicipioId != 0 && localInfracao.MunicipioId != null)
                {
                    AtualizarGeoLocalizacao(localInfracao, bancoDeDados);
                }

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete {0}tab_fisc_local_infracao t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);				

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}
		
		#endregion

		#region Obter / Filtrar

		public LocalInfracao Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			LocalInfracao localInfracao = new LocalInfracao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select t.id					Id,
						   t.fiscalizacao		FiscalizacaoId,
						   t.setor				SetorId,
						   t.sis_coord			SistemaCoordId,
						   t.datum				Datum,
						   t.area_abrang		AreaAbrangencia,
						   t.lat_northing		LatNorthing,
						   t.lon_easting		LonEasting,
						   t.hemisferio			Hemisferio,
						   t.municipio			MunicipioId,
						   l.texto				MunicipioTexto,
						   t.pessoa				PessoaId,
						   t.empreendimento		EmpreendimentoId,
						   t.tid				Tid,
						   t.local				Local,
						   t.responsavel		ResponsavelId,
						   t.resp_propriedade	ResponsavelPropriedadeId,
						   t.assinante			AssinantePropriedadeId,
                           t.area_fiscalizacao  AreaFiscalizacao          
					  from {0}tab_fisc_local_infracao t, 
						   {0}lov_municipio l
					 where t.municipio = l.id(+)
					   and t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				localInfracao = bancoDeDados.ObterEntity<LocalInfracao>(comando/*, (IDataReader reader, LocalInfracao item) => { item.Data.DataTexto = reader.GetValue<DateTime>("dataFisc").ToString("dd/MM/yyyy"); }*/);
			}
			return localInfracao;
		}

		internal List<PessoaLst> ObterResponsaveis(int empreendimentoId)
		{
			List<PessoaLst> lst = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Responsaveis do Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select tp.id, (case when lv.id != 9/*Outro*/ then lv.texto else ter.especificar end) 
															|| ' - ' || nvl(tp.nome, tp.razao_social) nome_razao, tp.cpf from tab_empreendimento_responsavel ter,
															tab_pessoa tp, lov_empreendimento_tipo_resp lv where ter.tipo = lv.id(+) and ter.responsavel = tp.id
															and ter.empreendimento = :empreendimento order by nome_razao", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst item;

					while (reader.Read())
					{
						item = new PessoaLst();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["nome_razao"].ToString();
						item.CPFCNPJ = reader["cpf"].ToString();
						item.IsAtivo = true;
						lst.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return lst;
		}

		internal List<PessoaLst> ObterAssinantes(int pessoaId)
		{
			List<PessoaLst> lst = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Responsaveis do Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select p.id, nvl(p.nome, p.razao_social) nome_razao, p.cpf
					  from tab_pessoa_representante pr, tab_pessoa p, tab_pessoa_conjuge pc, tab_pessoa c
					  where pr.representante = p.id and pr.pessoa = :pessoa_id
					  and p.id = pc.pessoa (+) and pc.conjuge = c.id (+) order by p.nome", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa_id", pessoaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst item;

					while (reader.Read())
					{
						item = new PessoaLst();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["nome_razao"].ToString();
						item.CPFCNPJ = reader["cpf"].ToString();
						item.IsAtivo = true;
						lst.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return lst;
		}

		internal List<PessoaLst> ObterResponsaveisHistorico(int empreendimentoId, string empreendimentoTid)
		{
			List<PessoaLst> lst = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Responsaveis do Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select tp.pessoa_id id, (case when ter.tipo_id != 9 /*Outro*/ then ter.tipo_texto 
															else ter.especificar end) || ' - ' || nvl(tp.nome, tp.razao_social) nome_razao, tp.cpf from 
															{0}hst_empreendimento_responsavel ter, {0}hst_pessoa tp where ter.responsavel_id = 
															tp.pessoa_id and ter.responsavel_tid = tp.tid and ter.empreendimento_id = :empreendimento
															and ter.empreendimento_tid = :tid order by nome_razao", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", empreendimentoTid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst item;

					while (reader.Read())
					{
						item = new PessoaLst();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["nome_razao"].ToString();
						item.CPFCNPJ = reader["cpf"].ToString();
						item.IsAtivo = true;
						lst.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return lst;
		}

		public LocalInfracao ObterHistorico(int fiscalizacaoId, BancoDeDados banco = null)
		{
			LocalInfracao localInfracao = new LocalInfracao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id         Id,
															t.fiscalizacao_id     FiscalizacaoId,
															t.setor_id            SetorId,
                                                            t.area_fiscalizacao   AreaFiscalizacao,
															t.sis_coord_id        SistemaCoordId,
															t.datum_id            Datum,
															t.area_abrang         AreaAbrangencia,
															t.lat_northing        LatNorthing,
															t.lon_easting         LonEasting,
															t.hemisferio_id       Hemisferio,
															t.municipio_id        MunicipioId,
															t.municipio_texto     MunicipioTexto,
															t.pessoa_id           PessoaId,
															t.pessoa_tid          PessoaTid,
															t.empreendimento_id   EmpreendimentoId,
															t.empreendimento_tid  EmpreendimentoTid,
															t.tid                 Tid,
															--t.data                dataFisc,
															t.local               Local,
															t.responsavel_id      ResponsavelId,
                                                            t.responsavel_tid     ResponsavelTid,
															t.resp_propriedade_id ResponsavelPropriedadeId,
															t.assinante_id		  AssinantePropriedadeId
														from {0}hst_fisc_local_infracao t
														where t.fiscalizacao_id = :fiscalizacao
														and t.id_hst = (select max(id)
																		 from {0}hst_fiscalizacao
																		where fiscalizacao_id = :fiscalizacao
																		  and situacao_id = 2 /*Cadastro Concluido*/
																	   )", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                //localInfracao = bancoDeDados.ObterEntity<LocalInfracao>(comando/*, (IDataReader reader, LocalInfracao item) => { item.Data.DataTexto = reader.GetValue<DateTime>("dataFisc").ToString("dd/MM/yyyy"); }*/);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        localInfracao.Id = reader.GetValue<int>("Id");
                        localInfracao.FiscalizacaoId = reader.GetValue<int>("FiscalizacaoId");
                        localInfracao.SetorId = reader.GetValue<int>("SetorId");
                        localInfracao.AreaFiscalizacao  = reader.GetValue<int?>("AreaFiscalizacao");
                        localInfracao.SistemaCoordId = reader.GetValue<int>("SistemaCoordId");
                        localInfracao.Datum  = reader.GetValue<int>("Datum");
                        localInfracao.AreaAbrangencia  = reader.GetValue<string>("AreaAbrangencia");
                        localInfracao.LatNorthing = reader.GetValue<string>("LatNorthing");
                        localInfracao.LonEasting = reader.GetValue<string>("LonEasting");
                        localInfracao.Hemisferio = reader.GetValue<int>("Hemisferio");
                        localInfracao.MunicipioId  = reader.GetValue<int>("MunicipioId");
                        localInfracao.MunicipioTexto = reader.GetValue<string>("MunicipioTexto");
                        localInfracao.PessoaId  = reader.GetValue<int>("PessoaId");
                        localInfracao.PessoaTid = reader.GetValue<string>("PessoaTid");
                        localInfracao.EmpreendimentoId  = reader.GetValue<int>("EmpreendimentoId");
                        localInfracao.EmpreendimentoTid = reader.GetValue<string>("EmpreendimentoTid");
                        localInfracao.Tid = reader.GetValue<string>("Tid");
                        localInfracao.Local = reader.GetValue<string>("Local");
                        localInfracao.ResponsavelId  = reader.GetValue<int>("ResponsavelId");
                        localInfracao.ResponsavelPropriedadeId = reader.GetValue<int>("ResponsavelPropriedadeId");
                        localInfracao.AssinantePropriedadeId = reader.GetValue<int>("AssinantePropriedadeId");

                        if (string.IsNullOrWhiteSpace(localInfracao.PessoaTid))
                        {
                            localInfracao.PessoaTid = reader.GetValue<string>("ResponsavelTid");
                        }
                    }

                    reader.Close();
                }
			}
			return localInfracao;
		}

		public Pessoa ObterPessoaSimplificadaPorHistorico(int id, string tid, BancoDeDados banco = null)
		{
			Pessoa pessoa = new Pessoa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.pessoa_id id, p.tipo, p.nome, p.apelido, p.cpf, p.rg, p.estado_civil_id estado_civil, p.sexo,
															p.nacionalidade, p.naturalidade, p.data_nascimento, p.mae, p.pai, p.cnpj, p.razao_social,
															p.nome_fantasia, p.ie, p.tid from {0}hst_pessoa p where p.pessoa_id = :id 
															and p.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

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
			}
			return pessoa;
		}

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_local_infracao t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

		#endregion

		#region Coordenandas

		private void AtualizarGeoLocalizacao(LocalInfracao localInfracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(
				@"declare "+
				   "v_existe number:=0; "+
				   "v_geo mdsys.sdo_geometry; "+
				"begin "+
				   "select count(*) into v_existe from {0}geo_fisc_localizacao f where f.fiscalizacao = :fiscalizacao; " +
				   "v_geo := mdsys.sdo_geometry(2001,31984, sdo_point_type(:lon_easting, :lat_northing, null), null, null); " +
				   "if v_existe > 0 then  "+
					  "update {0}geo_fisc_localizacao f set f.geometry = v_geo, f.tid = :tid where f.fiscalizacao = :fiscalizacao; " +
				   "else "+
					  "insert into {0}geo_fisc_localizacao (id, fiscalizacao, geometry, tid) values ({0}seq_geo_fisc_localizacao.nextval, :fiscalizacao, v_geo, :tid); " +
				   "end if; " +
				"end;", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("fiscalizacao", localInfracao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("lat_northing", localInfracao.LatNorthingToDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("lon_easting", localInfracao.LonEastingToDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion
	}
}