﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data
{
	public class LiberacaoNumeroCFOCFOCDa
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

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}
		#endregion

		public LiberacaoNumeroCFOCFOCDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region DML's

		public void Salvar(LiberaracaoNumeroCFOCFOC liberacao, BancoDeDados banco = null)
		{
			if (liberacao.Id > 0)
			{
			}
			else
			{
				Criar(liberacao, banco);
			}
		}

		private void Criar(LiberaracaoNumeroCFOCFOC liberacao, BancoDeDados banco = null)
		{
			IDataReader reader = null;

			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					Comando comando = bancoDeDados.CriarComando(@"
					insert into tab_liberacao_cfo_cfoc (id, tid, responsavel_tecnico, liberar_bloco_cfo, numero_inicial_cfo, numero_final_cfo, 
					liberar_bloco_cfoc, numero_inicial_cfoc, numero_final_cfoc, liberar_num_digital_cfo, qtd_num_cfo, liberar_num_digital_cfoc, qtd_num_cfoc)
					values (seq_tab_liberacao_cfo_cfoc.nextval, :tid, :responsavel_tecnico, :liberar_bloco_cfo, :numero_inicial_cfo, :numero_final_cfo, 
					:liberar_bloco_cfoc, :numero_inicial_cfoc, :numero_final_cfoc, :liberar_num_digital_cfo, :qtd_num_cfo, :liberar_num_digital_cfoc, 
					:qtd_num_cfoc) returning id into :liberacao_id", EsquemaBanco);

					comando.AdicionarParametroEntrada("responsavel_tecnico", liberacao.CredenciadoId, DbType.Int32);
					comando.AdicionarParametroEntrada("liberar_bloco_cfo", liberacao.LiberarBlocoCFO, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_inicial_cfo", liberacao.NumeroInicialCFO > 0 ? liberacao.NumeroInicialCFO : (object)DBNull.Value, DbType.Int64);
					comando.AdicionarParametroEntrada("numero_final_cfo", liberacao.NumeroFinalCFO > 0 ? liberacao.NumeroFinalCFO : (object)DBNull.Value, DbType.Int64);
					comando.AdicionarParametroEntrada("liberar_bloco_cfoc", liberacao.LiberarBlocoCFOC, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_inicial_cfoc", liberacao.NumeroInicialCFOC > 0 ? liberacao.NumeroInicialCFOC : (object)DBNull.Value, DbType.Int64);
					comando.AdicionarParametroEntrada("numero_final_cfoc", liberacao.NumeroFinalCFOC > 0 ? liberacao.NumeroFinalCFOC : (object)DBNull.Value, DbType.Int64);
					comando.AdicionarParametroEntrada("liberar_num_digital_cfo", liberacao.LiberarDigitalCFO, DbType.Int32);
					comando.AdicionarParametroEntrada("qtd_num_cfo", liberacao.QuantidadeDigitalCFO, DbType.Int32);
					comando.AdicionarParametroEntrada("liberar_num_digital_cfoc", liberacao.LiberarDigitalCFOC, DbType.Int32);
					comando.AdicionarParametroEntrada("qtd_num_cfoc", liberacao.QuantidadeDigitalCFOC, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("liberacao_id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					liberacao.Id = comando.ObterValorParametro<int>("liberacao_id");

					//Alocar a tabela
					comando = bancoDeDados.CriarComando(@"select * from {0}tab_numero_cfo_cfoc for update", EsquemaBanco);
					reader = bancoDeDados.ExecutarReader(comando);

					#region CFO

					if (liberacao.NumeroInicialCFO > 0)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid)
													values (seq_tab_numero_cfo_cfoc.nextval, :numero, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("liberacao", liberacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_documento", eCFOCFOCTipo.CFO, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_numero", eCFOCFOCTipoNumero.Bloco, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", GerenciadorTransacao.ObterIDAtual(), DbType.String);
						comando.AdicionarParametroEntrada("numero", DbType.Int64);

						for (var i = liberacao.NumeroInicialCFO; i <= liberacao.NumeroFinalCFO; i++)
						{
							comando.SetarValorParametro("numero", i);

							bancoDeDados.ExecutarNonQuery(comando);
						}
					}

					comando = bancoDeDados.CriarComandoPlSql(@"
					declare
						v_aux            number := 0;
						v_maior          number := 0;
						v_quantidade_lib number := :quantidade_lib;
					begin
						select nvl((select max(d.numero) from tab_numero_cfo_cfoc d where d.tipo_documento = :tipo_documento and d.tipo_numero = :tipo_numero),
							(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento and c.tipo = :tipo_numero))
						into v_maior from dual;

						for j in 1..v_quantidade_lib loop 
							v_maior := v_maior + 1;

							select count(1) into v_aux from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento 
							and c.tipo = :tipo_numero and (v_maior between c.numero_inicial and c.numero_final);

							if (v_aux > 0) then
								insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
								values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
							else 
								v_aux := v_maior;

								select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
								where t.tipo_documento = :tipo_documento and t.tipo = :tipo_numero and t.numero_inicial > v_maior;

								if(v_maior is null or v_aux = v_maior) then 
									--Tratamento de exceção
									Raise_application_error(-20023, 'Número não configurado');
								else 
									insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
									values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
								end if;
							end if;
						end loop;
					end;", EsquemaBanco);

					comando.AdicionarParametroEntrada("quantidade_lib", liberacao.QuantidadeDigitalCFO, DbType.Int32);
					comando.AdicionarParametroEntrada("liberacao", liberacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_documento", eCFOCFOCTipo.CFO, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_numero", eCFOCFOCTipoNumero.Digital, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", GerenciadorTransacao.ObterIDAtual(), DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					#endregion

					#region CFOC

					if (liberacao.NumeroInicialCFOC > 0)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
													values (seq_tab_numero_cfo_cfoc.nextval, :numero, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("liberacao", liberacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_documento", eCFOCFOCTipo.CFOC, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_numero", eCFOCFOCTipoNumero.Bloco, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", GerenciadorTransacao.ObterIDAtual(), DbType.String);
						comando.AdicionarParametroEntrada("numero", DbType.Int64);

						for (var i = liberacao.NumeroInicialCFOC; i <= liberacao.NumeroFinalCFOC; i++)
						{
							comando.SetarValorParametro("numero", i);

							bancoDeDados.ExecutarNonQuery(comando);
						}
					}

					comando = bancoDeDados.CriarComandoPlSql(@"
					declare
						v_aux            number := 0;
						v_maior          number := 0;
						v_quantidade_lib number := :quantidade_lib;
					begin
						select nvl((select max(d.numero) from tab_numero_cfo_cfoc d where d.tipo_documento = :tipo_documento and d.tipo_numero = :tipo_numero),
							(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento and c.tipo = :tipo_numero))
						into v_maior from dual;

						for j in 1..v_quantidade_lib loop 
							v_maior := v_maior + 1;

							select count(1) into v_aux from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento 
							and c.tipo = :tipo_numero and (v_maior between c.numero_inicial and c.numero_final);

							if (v_aux > 0) then
								insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
								values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
							else 
								v_aux := v_maior;

								select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
								where t.tipo_documento = :tipo_documento and t.tipo = :tipo_numero and t.numero_inicial > v_maior;

								if(v_maior is null or v_aux = v_maior) then 
									--Tratamento de exceção
									Raise_application_error(-20023, 'Número não configurado');
								else 
									insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
									values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
								end if;
							end if;
						end loop;
					end;", EsquemaBanco);

					comando.AdicionarParametroEntrada("quantidade_lib", liberacao.QuantidadeDigitalCFOC, DbType.Int32);
					comando.AdicionarParametroEntrada("liberacao", liberacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_documento", eCFOCFOCTipo.CFOC, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_numero", eCFOCFOCTipoNumero.Digital, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", GerenciadorTransacao.ObterIDAtual(), DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					#endregion

					Historico.Gerar(liberacao.Id, eHistoricoArtefato.liberacaocfocfoc, eHistoricoAcao.criar, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
					reader.Dispose();
				}
			}
		}

		internal void Cancelar(NumeroCFOCFOC objeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("update tab_numero_cfo_cfoc set motivo =:motivo, situacao = 0, tid = :tid where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", objeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", objeto.Motivo, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				Historico.Gerar(objeto.Id, eHistoricoArtefato.numerocfocfoc, eHistoricoAcao.cancelar, bancoDeDados);
				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		internal LiberaracaoNumeroCFOCFOC Obter(int id, BancoDeDados banco = null)
		{
			LiberaracaoNumeroCFOCFOC retorno = new LiberaracaoNumeroCFOCFOC();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select l.id, l.tid, l.responsavel_tecnico, p.nome, p.cpf, l.liberar_bloco_cfo, l.numero_inicial_cfo, l.numero_final_cfo,
				l.liberar_bloco_cfoc, l.numero_inicial_cfoc, l.numero_final_cfoc, l.liberar_num_digital_cfo, l.qtd_num_cfo,
				 l.liberar_num_digital_cfoc, l.qtd_num_cfoc from tab_liberacao_cfo_cfoc l, tab_credenciado c, 
				{1}tab_pessoa p where l.responsavel_tecnico = c.id and c.pessoa = p.id and l.id = :id", UsuarioInterno, UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Id = id;
						retorno.LiberarBlocoCFO = reader.GetValue<bool>("liberar_bloco_cfo");
						retorno.NumeroInicialCFO = reader.GetValue<long>("numero_inicial_cfo");
						retorno.NumeroFinalCFO = reader.GetValue<long>("numero_final_cfo");
						retorno.LiberarBlocoCFOC = reader.GetValue<bool>("liberar_bloco_cfoc");
						retorno.NumeroInicialCFOC = reader.GetValue<long>("numero_inicial_cfoc");
						retorno.NumeroFinalCFOC = reader.GetValue<long>("numero_final_cfoc");
						retorno.LiberarDigitalCFO = reader.GetValue<bool>("liberar_num_digital_cfo");
						retorno.QuantidadeDigitalCFO = reader.GetValue<int>("qtd_num_cfo");
						retorno.LiberarDigitalCFOC = reader.GetValue<bool>("liberar_num_digital_cfo");
						retorno.QuantidadeDigitalCFOC = reader.GetValue<int>("qtd_num_cfoc");
						retorno.Nome = reader.GetValue<string>("nome");
						retorno.CPF = reader.GetValue<string>("cpf");
					}

					reader.Close();
				}
			}
			return retorno;
		}

		internal Resultados<ListarResultados> Filtrar(Filtro<ListarFiltro> filtro)
		{
			Resultados<ListarResultados> retorno = new Resultados<ListarResultados>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("p.nome", "nome_responsavel", filtro.Dados.ResponsavelNome, upper: true, likeInicio: true);

				if (filtro.Dados.TipoNumero == 1)
				{
					comandtxt += @" and (l.liberar_bloco_cfo = 1 or l.liberar_bloco_cfoc = 1) ";
				}

				if (filtro.Dados.TipoNumero == 2)
				{
					comandtxt += @" and (l.liberar_num_digital_cfo = 1 or l.liberar_num_digital_cfoc = 1)";
				}

				if (filtro.Dados.TipoDocumento == 1)
				{
					comandtxt += @" and (l.liberar_bloco_cfo = 1 or l.liberar_num_digital_cfo = 1)";
				}

				if (filtro.Dados.TipoDocumento == 2)
				{
					comandtxt += @" and (l.liberar_bloco_cfoc = 1 or l.liberar_num_digital_cfoc = 1)";
				}

				if (filtro.Dados.Numero > 0)
				{
					//comandtxt += @"and ((:numero between l.numero_inicial_cfo and l.numero_final_cfo) or (:numero between l.numero_inicial_cfoc and l.numero_final_cfoc))";
					comandtxt += @" and exists ( select 1 from tab_numero_cfo_cfoc t where t.numero = :numero and t.liberacao = l.id )";
					comando.AdicionarParametroEntrada("numero", filtro.Dados.Numero, DbType.Int64);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome" };

				if (filtro.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = DaHelper.FormatarSql(@"select count(*) from {0}tab_liberacao_cfo_cfoc l, {0}tab_credenciado c, {1}tab_pessoa p 
				where l.responsavel_tecnico = c.id and c.pessoa = p.id " + comandtxt, UsuarioInterno, UsuarioCredenciado);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				comandtxt = String.Format(@"select l.id, p.nome from tab_liberacao_cfo_cfoc l, tab_credenciado c, {2}tab_pessoa p where l.responsavel_tecnico = c.id and 
				c.pessoa = p.id {0} {1}",
					comandtxt, DaHelper.Ordenar(colunas, ordenar),
					String.IsNullOrEmpty(UsuarioCredenciado) ? "" : UsuarioCredenciado + ".",
					String.IsNullOrEmpty(UsuarioInterno) ? "" : UsuarioInterno + ".");

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ListarResultados item = null;
					while (reader.Read())
					{
						item = new ListarResultados();
						item.LiberacaoId = reader.GetValue<int>("id");
						item.ResponsavelNome = reader.GetValue<String>("nome");
						retorno.Itens.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal List<NumeroCFOCFOC> FiltrarConsulta(ConsultaFiltro filtro, BancoDeDados banco = null)
		{
			List<NumeroCFOCFOC> retorno = new List<NumeroCFOCFOC>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.* from tab_numero_cfo_cfoc t, hst_liberacao_cfo_cfoc h 
				where h.responsavel_tecnico_id = :credenciado_id and h.liberacao_id = t.liberacao and t.tipo_numero = :tipo_numero ");

				comando.AdicionarParametroEntrada("tipo_numero", filtro.TipoNumero, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado_id", filtro.CredenciadoId, DbType.Int32);

				comando.DbCommand.CommandText += comando.FiltroAnd("t.numero", "numero", filtro.Numero);

				comando.DbCommand.CommandText += comando.FiltroAnd("t.tipo_documento", "tipo_documento", filtro.TipoDocumento);

				if (!string.IsNullOrEmpty(filtro.DataInicialEmissao))
				{
					comando.DbCommand.CommandText += " and h.data_execucao >= :data_inicial ";
					comando.AdicionarParametroEntrada("data_inicial", filtro.DataInicialEmissao, DbType.DateTime);
				}

				if (!string.IsNullOrEmpty(filtro.DataFinalEmissao))
				{
					comando.DbCommand.CommandText += " and h.data_execucao <= :data_final";
					comando.AdicionarParametroEntrada("data_final", filtro.DataFinalEmissao, DbType.DateTime);
				}

				comando.DbCommand.CommandText += " order by t.numero";

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					NumeroCFOCFOC item = null;
					while (reader.Read())
					{
						item = new NumeroCFOCFOC();

						item.Id = reader.GetValue<int>("id");
						item.Numero = reader.GetValue<long>("numero");
						item.Situacao = reader.GetValue<bool>("situacao");
						item.Tipo = reader.GetValue<int>("tipo_documento");
						item.TipoNumero = filtro.TipoNumero;
						item.Utilizado = reader.GetValue<bool>("utilizado");
						item.Motivo = reader.GetValue<string>("motivo");
						retorno.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		internal bool VerificarCPFAssociadoALiberacao(string cpf)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_liberacao_cfo_cfoc l, tab_credenciado c, {0}tab_pessoa p 
				where l.responsavel_tecnico = c.id and c.pessoa = p.id and p.cpf =:cpf", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool VerificarCPFAssociadoCredenciado(string cpf)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando("select count(*) from {0}tab_credenciado c , {0}tab_pessoa p where c.pessoa = p.id and p.cpf =:cpf", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool PossuiRegistroOrgaoClasse(string cpf)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioInterno))
			{
				Comando comando = bancoDeDados.CriarComando("select count(*) from {0}tab_credenciado c, {1}tab_pessoa_profissao pp, {1}tab_pessoa p where p.id = c.pessoa and  p.id = pp.pessoa and p.cpf = :cpf", UsuarioInterno, UsuarioCredenciado);
				comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal int ObterIDHabilitacaoCFOCFOC(string cpf)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select h.id from {0}tab_hab_emi_cfo_cfoc h, {0}tab_credenciado c, {1}tab_pessoa p where 
				c.id = h.responsavel and p.id = c.pessoa and p.cpf = :cpf", UsuarioInterno, UsuarioCredenciado);
				comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);
				object retorno = bancoDeDados.ExecutarScalar(comando);
				int habilitacaoId = retorno != null ? Convert.ToInt32(retorno) : 0;
				return habilitacaoId;
			}
		}

		internal bool BlocoPossuiNumeroCFONaoConfigurado(long numeroInicial, long numeroFinal)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComandoPlSql(@"
				declare 
					iterator number;
					v_aux    number := 1;
					v_saida  number := 0;
				begin 
					iterator := :numero_inicial;

					while iterator <= :numero_final loop
						select count(*) into v_aux from cnf_doc_fito_intervalo c where c.tipo = 1 and c.tipo_documento = 1 and iterator >= c.numero_inicial and iterator <= c.numero_final;
						iterator := iterator + 1;

						if v_aux = 0 then
							v_saida := 1;/*Possui 1 não configurado*/
							exit;
						end if;
					end loop;

					:retorno := v_saida;
				end;");

				comando.AdicionarParametroEntrada("numero_inicial", numeroInicial, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_final", numeroFinal, DbType.Int64);
				comando.AdicionarParametroSaida("retorno", DbType.Int32);

				banco.ExecutarNonQuery(comando);

				return Convert.ToInt32(comando.ObterValorParametro("retorno")) > 0;
			}
		}

		internal bool BlocoPossuiNumeroCFOCNaoConfigurado(long numeroInicial, long numeroFinal)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComandoPlSql(@"
				declare 
					iterator number;
					v_aux    number := 1;
					v_saida  number := 0;
				begin 
					iterator := :numero_inicial;

					while iterator <= :numero_final loop
						select count(*) into v_aux from cnf_doc_fito_intervalo c where c.tipo = 1 and c.tipo_documento = 2 and iterator >= c.numero_inicial and iterator <= c.numero_final;
						iterator := iterator + 1;

						if v_aux = 0 then
							v_saida := 1;/*Possui 1 não configurado*/
							exit;
						end if;
					end loop;

					:retorno := v_saida;
				end;");

				comando.AdicionarParametroEntrada("numero_inicial", numeroInicial, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_final", numeroFinal, DbType.Int64);
				comando.AdicionarParametroSaida("retorno", DbType.Int32);

				banco.ExecutarNonQuery(comando);

				return Convert.ToInt32(comando.ObterValorParametro("retorno")) > 0;
			}
		}

		internal bool VerificarNumeroCFOJaAtribuido(long numeroInicial, long numeroFinal)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(*) from tab_liberacao_cfo_cfoc c 
				where ((:numero_inicial between c.numero_inicial_cfo and c.numero_final_cfo) or (:numero_final between c.numero_inicial_cfo and c.numero_final_cfo))");

				comando.AdicionarParametroEntrada("numero_inicial", numeroInicial, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_final", numeroFinal, DbType.Int64);

				return banco.ExecutarScalar<int>(comando) <= 0;
			}
		}

		internal bool VerificarNumeroCFOCJaAtribuido(long numeroInicial, long numeroFinal)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(*) from tab_liberacao_cfo_cfoc c 
				where ((:numero_inicial between c.numero_inicial_cfoC and c.numero_final_cfoc) or (:numero_final between c.numero_inicial_cfoc and c.numero_final_cfoc))");

				comando.AdicionarParametroEntrada("numero_inicial", numeroInicial, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_final", numeroFinal, DbType.Int64);

				return banco.ExecutarScalar<int>(comando) <= 0;
			}
		}

		internal bool ValidarBlocoQuantidadeCadastradaCFO(LiberaracaoNumeroCFOCFOC liberacao)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(*) from tab_numero_cfo_cfoc n, tab_liberacao_cfo_cfoc l, tab_credenciado c where l.id = n.liberacao and 
				c.id = l.responsavel_tecnico and n.tipo_documento = 1 and n.tipo_numero = 1 and n.situacao = 1 and n.utilizado = 0 and c.id = :credenciado_id");

				comando.AdicionarParametroEntrada("credenciado_id", liberacao.CredenciadoId, DbType.Int32);

				return (banco.ExecutarScalar<int>(comando) + ((liberacao.NumeroFinalCFO - liberacao.NumeroInicialCFO) + 1)) <= 25;
			}
		}

		internal bool ValidarBlocoQuantidadeCadastradaCFOC(LiberaracaoNumeroCFOCFOC liberacao)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(*) from tab_numero_cfo_cfoc n, tab_liberacao_cfo_cfoc l, tab_credenciado c where l.id = n.liberacao and 
				c.id = l.responsavel_tecnico and n.tipo_documento = 2 and n.tipo_numero = 1 and n.situacao = 1 and n.utilizado = 0 and c.id = :credenciado_id");

				comando.AdicionarParametroEntrada("credenciado_id", liberacao.CredenciadoId, DbType.Int32);

				return (banco.ExecutarScalar<int>(comando) + ((liberacao.NumeroFinalCFOC - liberacao.NumeroInicialCFOC)) + 1) <= 25;
			}
		}

		internal bool DigitalPossuiNumeroCFONaoConfigurado(int quantidadeDigitalCFO, int credenciado)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"
				select nvl((select max(t.numero) from tab_liberacao_cfo_cfoc l, tab_numero_cfo_cfoc t where t.liberacao = l.id and t.tipo_documento = 1 and t.tipo_numero = 2), 
				(select min(t.numero_inicial) - 1 from cnf_doc_fito_intervalo t where t.tipo_documento = 1 and t.tipo = 2)) from dual");

				object objeto = banco.ExecutarScalar(comando);

				long ultimoAdicionado = ((objeto != null && objeto != DBNull.Value) ? Convert.ToInt64(objeto) : 0);

				comando = banco.CriarComandoPlSql(@"
				declare 
					proximo number;
					v_aux   number := 1;
					v_saida number := 0;
				begin 
					proximo := :numero_inicial;

					for i in 1..:quantidadeDigital loop
						select count(*) into v_aux from cnf_doc_fito_intervalo c 
						where c.tipo = 2 and c.tipo_documento = 1 and proximo >= c.numero_inicial and proximo <= c.numero_final;

						if v_aux = 0 then
							select nvl(min(t.numero_inicial), 0) into proximo from cnf_doc_fito_intervalo t 
							where t.tipo_documento = 1 and t.tipo = 2 and t.numero_inicial > proximo;

							if proximo = 0 then
								v_saida := 1;/*Possui 1 não configurado*/
								exit;
							end if;
						else
							proximo := proximo + 1;
						end if;
					end loop;

					:retorno := v_saida;
				end;");

				comando.AdicionarParametroEntrada("numero_inicial", ultimoAdicionado + 1, DbType.Int64);
				comando.AdicionarParametroEntrada("quantidadeDigital", quantidadeDigitalCFO, DbType.Int32);
				comando.AdicionarParametroSaida("retorno", DbType.Int32);

				banco.ExecutarNonQuery(comando);

				return Convert.ToInt32(comando.ObterValorParametro("retorno")) > 0;
			}
		}

		internal bool DigitalPossuiNumeroCFOCNaoConfigurado(int quantidadeDigitalCFOC, int credenciado)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"
				select nvl((select max(t.numero) from tab_liberacao_cfo_cfoc l, tab_numero_cfo_cfoc t where t.liberacao = l.id and t.tipo_documento = 2 and t.tipo_numero = 2), 
				(select min(t.numero_inicial) - 1 from cnf_doc_fito_intervalo t where t.tipo_documento = 2 and t.tipo = 2)) from dual");

				object objeto = banco.ExecutarScalar(comando);

				long ultimoAdicionado = ((objeto != null && objeto != DBNull.Value) ? Convert.ToInt64(objeto) : 0);

				comando = banco.CriarComandoPlSql(@"
				declare 
					proximo number;
					v_aux   number := 1;
					v_saida number := 0;
				begin 
					proximo := :numero_inicial;

					for i in 1..:quantidadeDigital loop
						select count(*) into v_aux from cnf_doc_fito_intervalo c 
						where c.tipo = 2 and c.tipo_documento = 2 and proximo >= c.numero_inicial and proximo <= c.numero_final;

						if v_aux = 0 then
							select nvl(min(t.numero_inicial), 0) into proximo from cnf_doc_fito_intervalo t 
							where t.tipo_documento = 2 and t.tipo = 2 and t.numero_inicial > proximo;

							if proximo = 0 then
								v_saida := 1;/*Possui 1 não configurado*/
								exit;
							end if;
						else
							proximo := proximo + 1;
						end if;
					end loop;

					:retorno := v_saida;
				end;");

				comando.AdicionarParametroEntrada("numero_inicial", ultimoAdicionado + 1, DbType.Int64);
				comando.AdicionarParametroEntrada("quantidadeDigital", quantidadeDigitalCFOC, DbType.Int32);
				comando.AdicionarParametroSaida("retorno", DbType.Int32);

				banco.ExecutarNonQuery(comando);

				return Convert.ToInt32(comando.ObterValorParametro("retorno")) > 0;
			}
		}

		internal bool VerificarQuantidadeMaximaNumDigitalCadastradoCFO(LiberaracaoNumeroCFOCFOC liberacao)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(t.id) from tab_numero_cfo_cfoc t, tab_liberacao_cfo_cfoc l, tab_credenciado c where l.id = t.liberacao and c.id = l.responsavel_tecnico
				and t.tipo_documento = 1 and t.tipo_numero = 2 and t.situacao = 1 and t.utilizado = 0 and c.id = :credenciado_id");

				comando.AdicionarParametroEntrada("credenciado_id", liberacao.CredenciadoId, DbType.Int32);

				int qtdCadastrada = banco.ExecutarScalar<int>(comando);

				return (qtdCadastrada + liberacao.QuantidadeDigitalCFO) <= 50;
			}
		}

		internal bool VerificarQuantidadeMaximaNumDigitalCadastradoCFOC(LiberaracaoNumeroCFOCFOC liberacao)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(t.id) from tab_numero_cfo_cfoc t, tab_liberacao_cfo_cfoc l, tab_credenciado c where l.id = t.liberacao and c.id = l.responsavel_tecnico
				and t.tipo_documento = 2 and t.tipo_numero = 2 and t.situacao = 1 and t.utilizado = 0 and c.id = :credenciado_id");

				comando.AdicionarParametroEntrada("credenciado_id", liberacao.CredenciadoId, DbType.Int32);

				int qtdCadastrada = banco.ExecutarScalar<int>(comando);

				return (qtdCadastrada + liberacao.QuantidadeDigitalCFOC) <= 50;
			}
		}

		internal bool NumeroCancelado(int id)
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(*) from tab_numero_cfo_cfoc t where t.situacao = 0 and t.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToInt32(banco.ExecutarScalar(comando)) > 0;
			}
		}

		internal Dictionary<string, string> CFOCFOCJaAssociado(int origemTipo, int origem, BancoDeDados banco = null)
		{
			Dictionary<string, string> retorno = new Dictionary<string, string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				select t.codigo_uc || t.ano || lpad(t.numero, 4, '0') numero, 'Lote' tipo from tab_lote t, tab_lote_item i where i.lote = t.id and i.origem_tipo = :origem_tipo and i.origem = :origem 
				union all 
				select to_char(t.numero), 'EPTV' tipo from tab_ptv t, tab_ptv_produto i where i.ptv = t.id and i.origem_tipo = :origem_tipo and i.origem = :origem 
				union all 
				select to_char(t.numero), 'PTV' tipo from ins_ptv t, ins_ptv_produto i where i.ptv = t.id and i.origem_tipo = :origem_tipo and i.origem = :origem");

				comando.AdicionarParametroEntrada("origem_tipo", origemTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("origem", origem, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						string key = reader.GetValue<string>("tipo");
						if (retorno.ContainsKey(key))
						{
							key = key + retorno.Where(x => x.Key.Contains(key)).Count();
						}
						retorno.Add(key, reader.GetValue<string>("numero"));
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion
	}
}