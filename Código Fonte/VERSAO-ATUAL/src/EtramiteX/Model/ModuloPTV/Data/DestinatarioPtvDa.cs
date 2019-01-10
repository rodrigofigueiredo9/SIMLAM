using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Data;
using System.Linq;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Data
{
	public class DestinatarioPTVDa
	{
		#region Propriedades

		Historico _historico = new Historico();

		public Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		#region DMLs

		internal void Salvar(DestinatarioPTV destinatario, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;
				eHistoricoAcao historicoAcao;

				if (destinatario.ID == 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_destinatario_ptv(id, tipo_pessoa, cpf_cnpj, nome, endereco, uf, municipio, itinerario, tid, pais) 
															values (seq_tab_destinatario_ptv.nextval,:tipo_pessoa, :cpf_cnpj, :nome, :endereco, :uf, :municipio, :itinerario, :tid, :pais)
															returning id into :id", EsquemaBanco);

					comando.AdicionarParametroSaida("id", DbType.Int32);
					historicoAcao = eHistoricoAcao.criar;
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_destinatario_ptv d set d.tipo_pessoa = :tipo_pessoa, d.cpf_cnpj = :cpf_cnpj, d.nome = :nome, d.endereco = :endereco, 
														d.uf = :uf, d.municipio = :municipio, d.itinerario = :itinerario, d.tid = :tid, d.pais = :pais where d.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", destinatario.ID, DbType.Int32);
					historicoAcao = eHistoricoAcao.atualizar;
				}

				comando.AdicionarParametroEntrada("tipo_pessoa", destinatario.PessoaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("cpf_cnpj", destinatario.CPFCNPJ, DbType.String);
				comando.AdicionarParametroEntrada("nome", destinatario.NomeRazaoSocial, DbType.String);
				comando.AdicionarParametroEntrada("endereco", destinatario.Endereco, DbType.String);
				comando.AdicionarParametroEntrada("uf", destinatario.EstadoID > 0 ? destinatario.EstadoID : null, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", destinatario.MunicipioID > 0 ? destinatario.MunicipioID : null, DbType.Int32);
				comando.AdicionarParametroEntrada("itinerario", destinatario.Itinerario, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("pais", destinatario.Pais, DbType.String);

				bancoDeDados.ExecutarScalar(comando);

				if (destinatario.ID == 0)
				{
					destinatario.ID = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				Historico.Gerar(destinatario.ID, eHistoricoArtefato.destinatarioptv, historicoAcao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				#region Atualizando

				comando = bancoDeDados.CriarComando(@"update {0}tab_destinatario_ptv d set d.tid = :tid where d.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				#endregion

				Historico.Gerar(id, eHistoricoArtefato.destinatarioptv, eHistoricoAcao.excluir, bancoDeDados);

				#region Excluir

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_destinatario_ptv d where d.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarScalar(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal DestinatarioPTV Obter(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				DestinatarioPTV destinatario = new DestinatarioPTV();

				#region SQL
				Comando comando = bancoDeDados.CriarComando(@"select d.id,
																d.tipo_pessoa,
																d.cpf_cnpj,
																d.nome,
																d.endereco,
																d.uf,
									                            e.sigla as estadoSigla,
																e.texto as estadoTexto,
																d.municipio,
																m.texto as municipioTexto,
																d.pais,
																d.itinerario,
																d.tid
															from tab_destinatario_ptv d
															LEFT JOIN lov_estado e ON d.uf = e.id
															LEFT JOIN lov_municipio m ON d.MUNICIPIO = m.id
															where d.id = :id", EsquemaBanco);
				#endregion

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						destinatario.ID = reader.GetValue<int>("id");
						destinatario.PessoaTipo = reader.GetValue<int>("tipo_pessoa");
						destinatario.CPFCNPJ = reader.GetValue<string>("cpf_cnpj");
						destinatario.NomeRazaoSocial = reader.GetValue<string>("nome");
						destinatario.Endereco = reader.GetValue<string>("endereco");
						destinatario.EstadoID = reader.GetValue<int>("uf");
						destinatario.EstadoSigla = reader.GetValue<string>("estadoSigla");
						destinatario.EstadoTexto = reader.GetValue<string>("estadoTexto");
						destinatario.MunicipioID = reader.GetValue<int>("municipio");
						destinatario.MunicipioTexto = reader.GetValue<string>("municipioTexto");
						destinatario.Pais = reader.GetValue<string>("pais");
						destinatario.Itinerario = reader.GetValue<string>("itinerario");
						destinatario.TID = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				return destinatario;
			}
		}

		internal Resultados<DestinatarioListarResultado> Filtrar(Filtro<DestinatarioListarFiltro> filtro)
		{
			Resultados<DestinatarioListarResultado> retorno = new Resultados<DestinatarioListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");
				List<int> culturas = new List<int>();

				#region Adicionando Filtros

				comandtxt = comando.FiltroAndLike("d.nome", "nome", filtro.Dados.Nome, true, true);

				comandtxt += comando.FiltroAndLike("d.cpf_cnpj", "cpfcnpj", filtro.Dados.CPFCNPJ, true, true);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nomerazaosocial", "cpfcnpj" };

				if (filtro.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nomerazaosocial");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_destinatario_ptv d where 0=0 " + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				comandtxt = String.Format(@"select d.id, d.nome NomeRazaoSocial, d.cpf_cnpj CpfCnpj 
										   from {0}tab_destinatario_ptv d where 0=0 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					DestinatarioListarResultado item;

					while (reader.Read())
					{
						item = new DestinatarioListarResultado();
						item.ID = reader.GetValue<int>("id");
						item.Nome = reader.GetValue<string>("NomeRazaoSocial");
						item.CPFCNPJ = reader.GetValue<string>("CpfCnpj");

						retorno.Itens.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal int ObterId(String cpfCnpj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_destinatario_ptv p where p.cpf_cnpj = :cpf_cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 50, cpfCnpj);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		internal int ObterIdExportacao(String nomeRazaoSocial, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_destinatario_ptv p where p.tipo_pessoa = 3 and upper(p.nome) = :nome", EsquemaBanco);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, nomeRazaoSocial);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		/// <summary>
		///	Recebe o ID do destinatário e retorna Verdadeiro se houver Associado ao PTV.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="banco"></param>
		/// <returns></returns>

		internal bool DestinatarioAssociacaoPTV(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_destinatario_ptv d, tab_ptv p where d.id = p.destinatario and d.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal bool DestinatarioAssociacaoPTVOutro(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_destinatario_ptv d, tab_ptv_outrouf p where d.id = p.destinatario and d.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		#endregion
	}
}