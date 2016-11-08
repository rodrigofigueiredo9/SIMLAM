using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data
{
	public class EntregaDa
	{
		#region Propriedade

		private string EsquemaBanco { get; set; }
		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		#endregion

		public EntregaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Entrega entrega, BancoDeDados banco = null)
		{
			if (entrega == null)
			{
				throw new Exception("A Entrega de Título é nula.");
			}

			Criar(entrega, banco);
		}

		internal void Criar(Entrega entrega, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Entrega de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_entrega e (id, protocolo, pessoa, nome, cpf, data_entrega, tid)
				values ({0}seq_titulo_entrega.nextval, :protocolo, :pessoa, :nome, :cpf, :data_entrega, :tid) returning e.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", entrega.Protocolo.Id, DbType.Int32);			
				comando.AdicionarParametroEntrada("pessoa", entrega.PessoaId, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, entrega.Nome);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 14, entrega.CPF);
				comando.AdicionarParametroEntrada("data_entrega", entrega.DataEntrega.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				entrega.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Títulos

				if (entrega.Titulos != null && entrega.Titulos.Count > 0)
				{
					foreach (int item in entrega.Titulos)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_entrega_titulos (id, entrega, titulo, tid)
						values ({0}seq_titulo_entrega_titulo.nextval, :entrega, :titulo, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("entrega", entrega.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo", item, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(entrega.Id, eHistoricoArtefato.tituloentrega, eHistoricoAcao.entregar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Resultados<Entrega> Filtrar(Filtro<ListarEntregaFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Entrega> retorno = new Resultados<Entrega>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");
				ProtocoloNumero protocolo;

				#region Adicionando Filtros

				//Protocolo Número
				if (!String.IsNullOrWhiteSpace(filtros.Dados.NumeroProtocolo))
				{
					comandtxt += "e.protocolo in (select p.id from {0}tab_protocolo p where p.numero :numero and p.ano = :ano)";
					protocolo = new ProtocoloNumero(filtros.Dados.NumeroProtocolo);
					comando.AdicionarParametroEntrada("numero", protocolo.Numero, DbType.Int32);
					comando.AdicionarParametroEntrada("ano", protocolo.Ano, DbType.Int32);
				}

				//Título Número
				comandtxt += comando.FiltroIn("e.id", String.Format(@"select e.entrega from {0}tab_titulo_entrega_titulos e, {0}tab_titulo t where e.titulo = t.id
            and t.numero like upper(:titulo_numero)", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "titulo_numero", filtros.Dados.NumeroTitulo + "%");

				//Empreendimento Denominador
				comandtxt += comando.FiltroIn("e.id", String.Format(@"select e.entrega from {0}tab_titulo_entrega_titulos e, {0}tab_titulo t, {0}tab_empreendimento d where e.titulo = t.id
            and t.empreendimento = d.id and d.denominador like upper(:empreendimento_denominador)", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "empreendimento_denominador", filtros.Dados.Empreendimento + "%");

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero,ano", "empreendimento", "data_entrega" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("protocolo_numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_titulo_entrega e where e.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select e.id, p.protocolo, p.id protocolo_id, p.numero, p.ano,
				(select te.denominador from {0}tab_empreendimento te where te.id = p.empreendimento) empreendimento,
				e.pessoa pessoa_id, e.nome, e.cpf, e.data_entrega, e.tid from {0}tab_titulo_entrega e, {0}tab_protocolo p where e.protocolo = p.id" 
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Entrega entrega;

					while (reader.Read())
					{
						entrega = new Entrega();
						entrega.Id = Convert.ToInt32(reader["id"]);
						entrega.Tid = reader["tid"].ToString();
						entrega.Nome = reader["nome"].ToString();
						entrega.CPF = reader["cpf"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							entrega.Protocolo.IsProcesso = reader["protocolo"].ToString() =="1";
						}

						entrega.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						entrega.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["numero"]);
						entrega.Protocolo.Ano = Convert.ToInt32(reader["ano"]);

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							entrega.EmpreendimentoDenominador = reader["empreendimento"].ToString();
						}

						if (reader["pessoa_id"] != null && !Convert.IsDBNull(reader["pessoa_id"]))
						{
							entrega.PessoaId = Convert.ToInt32(reader["pessoa_id"]);
						}

						if (reader["data_entrega"] != null && !Convert.IsDBNull(reader["data_entrega"]))
						{
							entrega.DataEntrega.Data = Convert.ToDateTime(reader["data_entrega"]);
						}

						retorno.Itens.Add(entrega);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Entrega Obter(int id, BancoDeDados banco = null)
		{
			Entrega entrega = new Entrega();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Entrega de Título

				Comando comando = bancoDeDados.CriarComando(@"select e.id, p.protocolo, p.id protocolo_id, p.numero, p.ano,
					(select te.denominador from {0}tab_empreendimento te where te.id = p.empreendimento) empreendimento,
					e.pessoa pessoa_id, e.nome, e.cpf, e.data_entrega, e.tid 
					from {0}tab_titulo_entrega e, {0}tab_protocolo p where e.protocolo = p.id and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entrega = new Entrega();
						entrega.Id = Convert.ToInt32(reader["id"]);
						entrega.Tid = reader["tid"].ToString();
						entrega.Nome = reader["nome"].ToString();
						entrega.CPF = reader["cpf"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							entrega.Protocolo.IsProcesso = reader["protocolo"].ToString() == "1";
						}

						entrega.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						entrega.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["numero"]);
						entrega.Protocolo.Ano = Convert.ToInt32(reader["ano"]);

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							entrega.EmpreendimentoDenominador = reader["empreendimento"].ToString();
						}

						if (reader["pessoa_id"] != null && !Convert.IsDBNull(reader["pessoa_id"]))
						{
							entrega.PessoaId = Convert.ToInt32(reader["pessoa_id"]);
						}

						if (reader["data_entrega"] != null && !Convert.IsDBNull(reader["data_entrega"]))
						{
							entrega.DataEntrega.Data = Convert.ToDateTime(reader["data_entrega"]);
						}
					}

					reader.Close();
				}

				#endregion

				#region Títulos

				comando = bancoDeDados.CriarComando(@"select t.titulo from {0}tab_titulo_entrega_titulos t where t.entrega = :entrega", EsquemaBanco);

				comando.AdicionarParametroEntrada("entrega", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						entrega.Titulos.Add(Convert.ToInt32(reader["titulo"]));
					}

					reader.Close();
				}

				#endregion
			}

			return entrega;
		}
	
		#endregion

		#region Validações

		public bool TituloEntregue(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_titulo_entrega_titulos t where t.titulo = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar(comando).ToString() != "0";
			}
		}

		#endregion
	}
}