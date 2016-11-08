using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Data
{
	public class OficioUsucapiaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }
		#endregion

		public OficioUsucapiaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(OficioUsucapiao oficio, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Ofício de Usucapião

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_oficio_usucapiao e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", oficio.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_oficio_usucapiao e set e.titulo = :titulo, e.protocolo = :protocolo, 
														e.dimensao = :dimensao, e.destinatario = :destinatario, e.descricao = :descricao, 
														e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					oficio.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_oficio_usucapiao e (id, titulo, protocolo, dimensao, destinatario, descricao, tid) 
														values ({0}seq_esp_oficio_usucapiao.nextval, :titulo, :protocolo, :dimensao, :destinatario, :descricao, 
														:tid) returning e.id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", oficio.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", oficio.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("dimensao", oficio.Dimensao, DbType.Decimal);
				comando.AdicionarParametroEntrada("destinatario", oficio.Destinatario, DbType.String);
				comando.AdicionarParametroEntrada("descricao", oficio.Descricao, DbType.String);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					oficio = oficio ?? new OficioUsucapiao();
					oficio.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(oficio.Titulo.Id, eHistoricoArtefatoEspecificidade.oficiousucapiao, acao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_oficio_usucapiao c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.oficiousucapiao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_oficio_usucapiao e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal OficioUsucapiao Obter(int titulo, BancoDeDados banco = null)
		{
			OficioUsucapiao especificidade = new OficioUsucapiao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Ofício de Usucapião

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, e.dimensao, e.destinatario, e.descricao, n.numero,
															n.ano, ee.zona empreendimento_tipo, p.requerimento, p.protocolo protocolo_tipo
															from {0}esp_oficio_usucapiao e, {0}tab_titulo_numero n, {0}tab_protocolo p,
															{0}tab_empreendimento_endereco ee where n.titulo(+) = e.titulo and e.protocolo = p.id(+)
															and ee.empreendimento(+) = p.empreendimento and ee.correspondencia = 0 and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Titulo.Id = titulo;
						especificidade.Destinatario = reader["destinatario"].ToString();
						especificidade.EmpreendimentoTipo = reader.GetValue<Int32?>("empreendimento_tipo");
						especificidade.Descricao = reader["descricao"].ToString();
						especificidade.Tid = reader["tid"].ToString();

						if (reader["dimensao"] != null && !Convert.IsDBNull(reader["dimensao"]))
						{
							especificidade.Dimensao = Convert.ToDecimal(reader["dimensao"]).ToStringTrunc();
						}


						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							especificidade.ProtocoloReq.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);
							especificidade.ProtocoloReq.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
							especificidade.ProtocoloReq.Id = Convert.ToInt32(reader["protocolo"]);
						}

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							especificidade.Titulo.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							especificidade.Titulo.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}
					}
					reader.Close();
				}
				#endregion

			}
			return especificidade;
		}

		internal Oficio ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Oficio oficio = new Oficio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				oficio.Titulo = dados.Titulo;
				oficio.Titulo.SetorEndereco = DaEsp.ObterEndSetor(oficio.Titulo.SetorId);
				oficio.Protocolo = dados.Protocolo;
				oficio.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.dimensao, e.destinatario, e.descricao, ee.zona empreendimento_tipo
															from {0}esp_oficio_usucapiao e, {0}tab_protocolo p, {0}tab_empreendimento_endereco ee
															where e.protocolo = p.id and ee.empreendimento(+) = p.empreendimento and ee.correspondencia = 0 
															and e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						oficio.DestinatarioPGE = reader["destinatario"].ToString();
						oficio.Descricao = reader["descricao"].ToString();
						oficio.Dimensao = Convert.ToDecimal(reader["dimensao"]).ToStringTrunc();

						if (reader["empreendimento_tipo"] != null && !Convert.IsDBNull(reader["empreendimento_tipo"]))
						{
							oficio.EmpreendimentoTipo = reader.GetValue<Int32>("empreendimento_tipo");
						}
					}

					reader.Close();
				}

				#endregion

				oficio.Destinatario = DaEsp.ObterDadosPessoa(oficio.Destinatario.Id, oficio.Empreendimento.Id, bancoDeDados);

				oficio.Anexos = DaEsp.ObterAnexos(titulo, bancoDeDados);
			}

			return oficio;
		}

		internal Int32? ObterZonaLocalizacaoEmpreendimento(int protocolo, BancoDeDados banco = null)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select ee.zona from {0}tab_empreendimento_endereco ee, 
															{0}tab_protocolo p where ee.empreendimento = p.empreendimento 
															and ee.correspondencia = 0 and p.id = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["zona"] != null && !Convert.IsDBNull(reader["zona"]))
						{
							return Convert.ToInt32(reader["zona"]);
						}
					}

					reader.Close();
				}
			}

			return null;

		}

		#endregion
	}
}