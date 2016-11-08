using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Data
{
	public class FichaFundiariaDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public FichaFundiariaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal FichaFundiaria Salvar(FichaFundiaria ficha, BancoDeDados banco = null)
		{

			if (ficha == null)
			{
				throw new Exception("Objeto Ficha Fundiária não pode ser nulo.");
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Ficha Fundiaria

				bancoDeDados.IniciarTransacao();

				Comando comando = null;
				eHistoricoAcao acao = eHistoricoAcao.criar;

				if (ficha.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_acervo_ficha_fund f set f.codigo = :codigo, f.requerente = :requerente, 
														f.idoc = :idoc, f.ndoc = :ndoc, f.pai = :pai, f.mae = :mae, f.endereco = :endereco, 
														f.municipio = :municipio, f.distrito = :distrito, f.lugar = :lugar, f.tipo_area = 
														:tipo_area, f.data_med = :data_med, f.area_med = :area_med, f.perimetro = :perimetro, 
														f.topografo = :topografo, f.prot_reg = :prot_reg, f.prot_ger = :prot_ger, f.lote =
														:lote, f.quadra = :quadra, f.l_sul = :l_sul, f.l_norte = :l_norte, f.l_leste = :l_leste,
														l_oeste = :l_oeste, f.data_ec = :data_ec, f.lv_ec = :lv_ec, f.fl_ec = :fl_ec, f.data_ed
														= :data_ed, f.lv_ed = :lv_ed, f.fl_ed = :fl_ed, f.observacao = :observacao, f.tid = :tid
														where f.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("codigo", ficha.Codigo, DbType.String);
					comando.AdicionarParametroEntrada("id", ficha.Id, DbType.Int32);
					acao = eHistoricoAcao.atualizar;
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_acervo_ficha_fund(id, codigo, requerente, idoc, ndoc, pai, mae, endereco, municipio, 
														distrito, lugar, tipo_area, data_med, area_med, perimetro, topografo, prot_reg, prot_ger, lote, quadra, 
														l_sul, l_norte, l_leste, l_oeste, data_ec, lv_ec, fl_ec, data_ed, lv_ed, fl_ed, observacao, tid) values
														({0}seq_tab_acervo_ficha_fund.nextval, {0}seq_tab_acervo_ficha_fund.currval, :requerente, :idoc, :ndoc, 
														:pai, :mae, :endereco, :municipio, :distrito, :lugar, :tipo_area, :data_med, :area_med, :perimetro, 
														:topografo, :prot_reg, :prot_ger, :lote, :quadra, :l_sul, :l_norte, :l_leste, :l_oeste, :data_ec, :lv_ec,
														:fl_ec, :data_ed, :lv_ed, :fl_ed, :observacao, :tid) returning id into :id", EsquemaBanco);

					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				comando.AdicionarParametroEntrada("prot_reg", ficha.ProtocoloRegional, DbType.String);
				comando.AdicionarParametroEntrada("prot_ger", ficha.ProtocoloGeral, DbType.String);
				comando.AdicionarParametroEntrada("l_sul", ficha.ConfrontanteSul, DbType.String);
				comando.AdicionarParametroEntrada("l_norte", ficha.ConfrontanteNorte, DbType.String);
				comando.AdicionarParametroEntrada("l_leste", ficha.ConfrontanteLeste, DbType.String);
				comando.AdicionarParametroEntrada("l_oeste", ficha.ConfrontanteOeste, DbType.String);
				comando.AdicionarParametroEntrada("observacao", ficha.Observacoes, DbType.String);

				comando.AdicionarParametroEntrada("requerente", ficha.Requerente.Nome, DbType.String);
				comando.AdicionarParametroEntrada("idoc", ficha.Requerente.DocumentoTipo, DbType.String);
				comando.AdicionarParametroEntrada("ndoc", ficha.Requerente.DocumentoNumero, DbType.String);
				comando.AdicionarParametroEntrada("pai", ficha.Requerente.NomePai, DbType.String);
				comando.AdicionarParametroEntrada("mae", ficha.Requerente.NomeMae, DbType.String);
				comando.AdicionarParametroEntrada("endereco", ficha.Requerente.Endereco, DbType.String);

				comando.AdicionarParametroEntrada("municipio", ficha.Terreno.Municipio, DbType.String);
				comando.AdicionarParametroEntrada("distrito", ficha.Terreno.Distrito, DbType.String);
				comando.AdicionarParametroEntrada("lugar", ficha.Terreno.Lugar, DbType.String);
				comando.AdicionarParametroEntrada("tipo_area", ficha.Terreno.Tipo, DbType.String);
				comando.AdicionarParametroEntrada("data_med", ficha.Terreno.DataMedicao, DbType.String);
				comando.AdicionarParametroEntrada("area_med", ficha.Terreno.Area, DbType.String);
				comando.AdicionarParametroEntrada("perimetro", ficha.Terreno.Perimetro, DbType.String);
				comando.AdicionarParametroEntrada("topografo", ficha.Terreno.NomeTopografo, DbType.String);
				comando.AdicionarParametroEntrada("lote", ficha.Terreno.Lote, DbType.String);
				comando.AdicionarParametroEntrada("quadra", ficha.Terreno.Quadra, DbType.String);

				comando.AdicionarParametroEntrada("data_ec", ficha.EscrituraCondicional.Data, DbType.String);
				comando.AdicionarParametroEntrada("lv_ec", ficha.EscrituraCondicional.Livro, DbType.String);
				comando.AdicionarParametroEntrada("fl_ec", ficha.EscrituraCondicional.Folha, DbType.String);

				comando.AdicionarParametroEntrada("data_ed", ficha.EscrituraDefinitiva.Data, DbType.String);
				comando.AdicionarParametroEntrada("lv_ed", ficha.EscrituraDefinitiva.Livro, DbType.String);
				comando.AdicionarParametroEntrada("fl_ed", ficha.EscrituraDefinitiva.Folha, DbType.String);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (ficha.Id <= 0) 
				{
					ficha.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion


				#region Histórico

				Historico.Gerar(ficha.Id, eHistoricoArtefato.fichafundiaria, acao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}

			return ficha;
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_acervo_ficha_fund f set f.tid = :tid where f.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.fichafundiaria, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_acervo_ficha_fund f where f.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal FichaFundiaria Obter(int id)
		{
			FichaFundiaria ficha = new FichaFundiaria();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Ficha Fundiaria

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.prot_ger protocolo_geral, e.prot_reg protocolo_regional, 
															e.requerente nome_requerente, e.idoc documento_tipo, e.ndoc documento_numero, e.pai nome_pai,
															e.mae nome_mae, e.endereco, e.municipio, e.distrito, e.lugar, e.tipo_area tipo_terreno,
															e.data_med, e.area_med, e.perimetro, e.lote, e.quadra, e.topografo nome_topografo, e.l_norte,
															e.l_sul, e.l_leste, e.l_oeste, e.data_ec escritura_condicional_data, e.lv_ec escritura_condicional_livro,
															e.fl_ec escritura_condicional_folha, e.data_ed escritura_definitiva_data, e.lv_ed escritura_definitiva_livro,
															e.fl_ed escritura_definitiva_folha, e.observacao from {0}tab_acervo_ficha_fund e where e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						ficha.Id = reader.GetValue<Int32>("id");
						ficha.Codigo = reader.GetValue<String>("codigo");
						ficha.ProtocoloGeral = reader.GetValue<String>("protocolo_geral");
						ficha.ProtocoloRegional = reader.GetValue<String>("protocolo_regional");
						
						ficha.Requerente.Nome = reader.GetValue<String>("nome_requerente");
						ficha.Requerente.DocumentoTipo = reader.GetValue<String>("documento_tipo");
						ficha.Requerente.DocumentoNumero = reader.GetValue<String>("documento_numero");
						ficha.Requerente.NomePai = reader.GetValue<String>("nome_pai");
						ficha.Requerente.NomeMae = reader.GetValue<String>("nome_mae");
						ficha.Requerente.Endereco = reader.GetValue<String>("endereco");

						ficha.Terreno.Municipio = reader.GetValue<String>("municipio");
						ficha.Terreno.Distrito = reader.GetValue<String>("distrito");
						ficha.Terreno.Lugar = reader.GetValue<String>("lugar");
						ficha.Terreno.Tipo = reader.GetValue<String>("tipo_terreno");
						ficha.Terreno.DataMedicao = reader.GetValue<String>("data_med");
						ficha.Terreno.Area = reader.GetValue<String>("area_med");
						ficha.Terreno.Perimetro = reader.GetValue<String>("perimetro");
						ficha.Terreno.Lote = reader.GetValue<String>("lote");
						ficha.Terreno.Quadra = reader.GetValue<String>("quadra");
						ficha.Terreno.NomeTopografo = reader.GetValue<String>("nome_topografo");

						ficha.ConfrontanteNorte = reader.GetValue<String>("l_norte");
						ficha.ConfrontanteSul = reader.GetValue<String>("l_sul");
						ficha.ConfrontanteLeste = reader.GetValue<String>("l_leste");
						ficha.ConfrontanteOeste = reader.GetValue<String>("l_oeste");

						ficha.EscrituraCondicional.Data = reader.GetValue<String>("escritura_condicional_data");
						ficha.EscrituraCondicional.Livro = reader.GetValue<String>("escritura_condicional_livro");
						ficha.EscrituraCondicional.Folha = reader.GetValue<String>("escritura_condicional_folha");

						ficha.EscrituraDefinitiva.Data = reader.GetValue<String>("escritura_definitiva_data");
						ficha.EscrituraDefinitiva.Livro = reader.GetValue<String>("escritura_definitiva_livro");
						ficha.EscrituraDefinitiva.Folha = reader.GetValue<String>("escritura_definitiva_folha");

						ficha.Observacoes = reader.GetValue<String>("observacao");
					}
					reader.Close();
				}

				#endregion
			}
			return ficha;
		}

		internal Resultados<FichaFundiaria> Filtrar(Filtro<ListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<FichaFundiaria> retorno = new Resultados<FichaFundiaria>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("e.prot_ger", "protocolo_geral", filtros.Dados.ProtocoloGeral, true, true);

				comandtxt += comando.FiltroAndLike("e.prot_reg", "protocolo_regional", filtros.Dados.ProtocoloRegional, true, true);

				comandtxt += comando.FiltroAndLike("e.idoc", "documento_tipo", filtros.Dados.DocumentoTipo, true, true);

				comandtxt += comando.FiltroAndLike("e.ndoc", "documento_numero", filtros.Dados.DocumentoNumero, true, true);

				comandtxt += comando.FiltroAndLike("e.municipio", "municipio", filtros.Dados.Municipio, true, true);

				comandtxt += comando.FiltroAndLike("e.distrito", "distrito", filtros.Dados.Distrito, true, true);

				comandtxt += comando.FiltroAndLike("e.requerente", "requerente", filtros.Dados.Requerente, true, true);


				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "requerente", "documento_tipo", "municipio", "protocolo_geral", "protocolo_regional" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("requerente");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_acervo_ficha_fund e where e.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select e.id, e.codigo, e.requerente, e.idoc documento_tipo, e.ndoc documento_numero,
											e.municipio, e.distrito, e.prot_ger protocolo_geral, e.prot_reg protocolo_regional 
											from {0}tab_acervo_ficha_fund e where e.id > 0"

				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					FichaFundiaria ficha;

					while (reader.Read())
					{
						ficha = new FichaFundiaria();
						ficha.Id = Convert.ToInt32(reader["id"].ToString());
						ficha.Requerente.Nome = reader["requerente"].ToString();
						ficha.Requerente.DocumentoTipo = reader["documento_tipo"].ToString();
						ficha.Requerente.DocumentoNumero = reader["documento_numero"].ToString();
						ficha.Terreno.Municipio = reader["municipio"].ToString();
						ficha.ProtocoloGeral = reader["protocolo_geral"].ToString();
						ficha.ProtocoloRegional = reader["protocolo_regional"].ToString();

						retorno.Itens.Add(ficha);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		#endregion
	}
}
