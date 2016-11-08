using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Data
{
	public class SetorLocalizacaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		ListaBus _busLista = new ListaBus();

		#endregion

		public SetorLocalizacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal SetorLocalizacao Editar(SetorLocalizacao setor, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Setor

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_setor s set s.nome = :nome, s.sigla = :sigla,
															s.responsavel = :responsavel, s.tid = :tid where s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", setor.Nome, DbType.String);
				comando.AdicionarParametroEntrada("sigla", setor.Sigla, DbType.String);
				comando.AdicionarParametroEntrada("responsavel", setor.Responsavel, DbType.Int32);
				comando.AdicionarParametroEntrada("id", setor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Endereco

				comando = bancoDeDados.CriarComando(@" update {0}tab_setor_endereco se set se.logradouro = :logradouro, se.numero = :numero,
													se.bairro = :bairro, se.cep = :cep, se.estado = :estado, se.municipio = :municipio,
													se.fone = :fone, se.fone_fax = :fax, se.complemento = :complemento , se.tid = :tid
													where se.setor = :setor", EsquemaBanco);

				comando.AdicionarParametroEntrada("logradouro", setor.Endereco.Logradouro, DbType.String);
				comando.AdicionarParametroEntrada("numero", setor.Endereco.Numero, DbType.String);
				comando.AdicionarParametroEntrada("bairro", setor.Endereco.Bairro, DbType.String);
				comando.AdicionarParametroEntrada("cep", setor.Endereco.Cep, DbType.String);
				comando.AdicionarParametroEntrada("estado", setor.Endereco.EstadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", setor.Endereco.MunicipioId, DbType.Int32);
				comando.AdicionarParametroEntrada("fone", setor.Endereco.Fone, DbType.String);
				comando.AdicionarParametroEntrada("fax", setor.Endereco.Fax, DbType.String);
				comando.AdicionarParametroEntrada("complemento", setor.Endereco.Complemento, DbType.String);
				comando.AdicionarParametroEntrada("setor", setor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(setor.Id, eHistoricoArtefato.setor, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}

			return setor;
		}

		public SetorLocalizacao Salvar(SetorLocalizacao setor, BancoDeDados banco = null)
		{
			if (setor == null)
			{
				throw new Exception("Setor é nulo.");
			}

			if (setor.Id > 0)
			{
				setor = Editar(setor, banco);
			}

			return setor;
		}

		#endregion

		#region Obter / Filtrar

		internal SetorLocalizacao Obter(int id)
		{
			SetorLocalizacao setor = new SetorLocalizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Setor
				Comando comando = bancoDeDados.CriarComando(@"select s.id setorId, s.sigla, s.tid, g.id agrupadorId, g.nome agrupadorTexto, s.nome setorTexto,
															lm.id municipioId, lm.texto municipioTexto, le.id estadoId, le.sigla estadoSigla, le.texto estadoTexto,
															se.id endereco_id, se.logradouro, se.numero, se.bairro, se.cep, se.fone, se.fone_fax, se.complemento from {0}tab_setor s,
															{0}tab_setor_agrupador g, {0}tab_setor_grupo sg, {0}tab_setor_endereco  se, {0}lov_municipio lm, {0}lov_estado le
															where g.id(+) = sg.grupo and sg.setor(+) = s.id and se.setor(+) = s.id and lm.id(+) = se.municipio and le.id(+) = lm.estado
															and s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						setor.Id = Convert.ToInt32(reader["setorId"]);
						setor.Sigla = reader["sigla"].ToString();
						setor.Nome = reader["setorTexto"].ToString();
						setor.Tid = reader["tid"].ToString();

						if (reader["agrupadorId"] != null && !Convert.IsDBNull(reader["agrupadorId"]))
						{
							setor.Agrupador = Convert.ToInt32(reader["agrupadorId"]);
							setor.AgrupadorTexto = reader["agrupadorTexto"].ToString();
						}

						if (reader["endereco_id"] != null && !Convert.IsDBNull(reader["endereco_id"]))
						{
							Endereco endereco = new Endereco();
							endereco.MunicipioId = Convert.ToInt32(reader["municipioId"]);
							endereco.MunicipioTexto = reader["municipioTexto"].ToString();
							endereco.EstadoId = Convert.ToInt32(reader["estadoId"]);
							endereco.EstadoTexto = reader["estadoSigla"].ToString();
							endereco.Logradouro = reader["logradouro"].ToString();
							endereco.Numero = reader["numero"].ToString();

							endereco.Bairro = reader["bairro"].ToString();
							endereco.Cep = reader["cep"].ToString();
							endereco.Fone = reader["fone"].ToString();
							endereco.Fax = reader["fone_fax"].ToString();
							endereco.Complemento = reader["complemento"].ToString();

							setor.Endereco = endereco;
						}


					}
					reader.Close();
				}

				#endregion
			}
			return setor;
		}

		internal Resultados<SetorLocalizacao> Filtrar(Filtro<ListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<SetorLocalizacao> retorno = new Resultados<SetorLocalizacao>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroIn("s.id", String.Format("select g.setor from {0}tab_setor_grupo g where g.grupo = :agrupador",
					EsquemaBanco), "agrupador", filtros.Dados.Agrupador);

				comandtxt += comando.FiltroAnd("s.id", "setor", filtros.Dados.Setor);

				comandtxt += comando.FiltroIn("s.id", String.Format("select e.setor from {0}tab_setor_endereco e where e.municipio = :municipio",
					EsquemaBanco), "municipio", filtros.Dados.Municipio);


				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "agrupadorTexto", "setorTexto", "municipioTexto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("agrupadorTexto");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_setor s where s.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select s.id setorId, s.tid, g.id agrupadorId, g.nome agrupadorTexto, s.nome setorTexto, lm.id municipioId, lm.texto municipioTexto 
							from {0}tab_setor s, {0}tab_setor_agrupador g, {0}tab_setor_grupo sg, {0}tab_setor_endereco se, {0}lov_municipio lm
							where g.id(+) = sg.grupo and sg.setor(+) = s.id and se.setor(+) = s.id and lm.id(+) = se.municipio" + comandtxt + DaHelper.Ordenar(colunas, ordenar);

				comando.DbCommand.CommandText = String.Format(@"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					SetorLocalizacao setor;

					while (reader.Read())
					{
						setor = new SetorLocalizacao();
						setor.Id = Convert.ToInt32(reader["setorId"]);
						setor.Nome = reader["setorTexto"].ToString();

						if (reader["agrupadorId"] != null && !Convert.IsDBNull(reader["agrupadorId"]))
						{
							setor.Agrupador = Convert.ToInt32(reader["agrupadorId"]);
							setor.AgrupadorTexto = reader["agrupadorTexto"].ToString();
						}

						if (reader["municipioId"] != null && !Convert.IsDBNull(reader["municipioId"])) 
						{
							setor.Endereco.MunicipioId = Convert.ToInt32(reader["municipioId"]);
							setor.Endereco.MunicipioTexto = reader["municipioTexto"].ToString();
						}

						setor.Tid = reader["tid"].ToString();

						retorno.Itens.Add(setor);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		#endregion

		#region Validacoes

		internal bool VerificarTidAtual(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.tid from {0}tab_setor s where s.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				string tidAtual = bancoDeDados.ExecutarScalar<string>(comando);

				return tidAtual.Equals(tid, StringComparison.InvariantCultureIgnoreCase);
			}
		}

		internal bool ExisteSigla(int setor, string sigla)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				
				Comando comando = bancoDeDados.CriarComando(@"select s.id setor from {0}tab_setor s where s.sigla = :sigla", EsquemaBanco);
				comando.AdicionarParametroEntrada("sigla", sigla, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (Convert.ToInt32(reader["setor"]) == setor)
						{
							reader.Close();
							return false;
						}
					}
					else 
					{
						reader.Close();
						return false;
					}

					reader.Close();
				}
			}

			return true;
		}

		#endregion
	}
}
