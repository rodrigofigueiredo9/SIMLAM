using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Data
{
	public class OutrosInformacaoCorteDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public OutrosInformacaoCorteDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(OutrosInformacaoCorte outro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_out_informacao_corte e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", outro.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@" update {0}esp_out_informacao_corte t set t.protocolo = :protocolo, t.informacao_corte = :informacao_corte, t.tid = :tid, t.destinatario = :destinatario
						where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					outro.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@" insert into {0}esp_out_informacao_corte (id, titulo, protocolo, informacao_corte, tid, destinatario) values 
						({0}seq_esp_out_informacao_corte.nextval, :titulo, :protocolo, :informacao_corte, :tid, :destinatario) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", outro.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", outro.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("informacao_corte", outro.InformacaoCorte, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", outro.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					outro = outro ?? new OutrosInformacaoCorte();
					outro.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(outro.Titulo.Id, eHistoricoArtefatoEspecificidade.outrosinformacaocorte, acao, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(
					"begin " +
					  "update {0}esp_out_informacao_corte c set c.tid = :tid where c.titulo = :titulo; " +
					"end; ", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.outrosinformacaocorte, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(
					"begin " +
					  "delete from {0}esp_out_informacao_corte e where e.titulo = :titulo; " +
					"end; ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal OutrosInformacaoCorte Obter(int titulo, BancoDeDados banco = null)
		{
			OutrosInformacaoCorte especificidade = new OutrosInformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@" select e.id, e.titulo, e.protocolo, e.informacao_corte, e.tid, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.destinatario, 
				(select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_out_informacao_corte he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
				and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
				and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao 
				from {0}esp_out_informacao_corte e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id and e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("id");
						especificidade.InformacaoCorte = reader.GetValue<int>("informacao_corte");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
						especificidade.Destinatario = reader.GetValue<int>("destinatario");
						especificidade.DestinatarioNomeRazao = reader.GetValue<string>("destinatario_nome_razao");
						especificidade.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Outros ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Outros outros = new Outros();
			InformacaoCorteBus infoCorteBus = new InformacaoCorteBus();
			List<InformacaoCorte> infoCorte = null;
			InformacaoCorte infoCorteInfo = null;
			int infoCorteInfoId = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				outros.Titulo = dados.Titulo;
				outros.Titulo.SetorEndereco = DaEsp.ObterEndSetor(outros.Titulo.SetorId);
				outros.Protocolo = dados.Protocolo;
				outros.Empreendimento = dados.Empreendimento;

				#endregion

				#region Especificidade

				Comando comando = bancoDeDados.CriarComando(@" select e.destinatario, e.informacao_corte from {0}esp_out_informacao_corte e where e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						outros.Destinatario = _daEsp.ObterDadosPessoa(reader.GetValue<int>("destinatario"), outros.Empreendimento.Id, bancoDeDados);
						infoCorteInfoId = reader.GetValue<int>("informacao_corte");
					}

					reader.Close();

				}

				#endregion

				#region Dominialidade

				DominialidadeBus _dominialidadeBus = new DominialidadeBus();

				outros.Dominialidade = new DominialidadePDF(_dominialidadeBus.ObterPorEmpreendimento(outros.Empreendimento.Id.GetValueOrDefault(), banco: bancoDeDados));

				#endregion

				#region Informação de corte

				infoCorte = infoCorteBus.ObterPorEmpreendimento(outros.Empreendimento.Id.GetValueOrDefault(), banco: bancoDeDados);

				if (infoCorte != null)
				{
					infoCorteInfo = infoCorte.SingleOrDefault(x => x.Id == infoCorteInfoId);

					if (infoCorteInfo != null)
					{
						outros.InformacaoCorteInfo = new InformacaoCorteInfoPDF(infoCorteInfo);
					}
				}

				#endregion
			}

			return outros;
		}

		internal List<ListaValor> ObterInformacaoCortes(int protocoloId)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@" select distinct i.id, to_char(i.data_informacao, 'DD/MM/YYYY') texto from {0}crt_inf_corte_inf i, {0}crt_informacao_corte c,
					{0}tab_protocolo p where i.caracterizacao = c.id and p.empreendimento = c.empreendimento and p.id = :protocoloId order by to_char(i.data_informacao, 'DD/MM/YYYY') ", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocoloId", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ListaValor item;

					while (reader.Read())
					{
						item = new ListaValor();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["texto"].ToString();
						item.IsAtivo = true;
						lst.Add(item);
					}

					reader.Close();
				}
			}

			return lst;
		}

		#endregion

		#region Validação

		internal bool IsInformacaoCorteCadastrado(int informacaoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(" select count(1) from {0}crt_inf_corte_inf c where c.id = :informacaoId ", EsquemaBanco);
				comando.AdicionarParametroEntrada("informacaoId", informacaoId, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal bool IsInformacaoCorteAssociado(int informacaoId, int especificidadeId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}esp_out_informacao_corte e, {0}tab_titulo t where e.id != :especificidadeId
					 and e.informacao_corte = :informacaoId and t.id = e.titulo and t.situacao in (3, 6) ", EsquemaBanco);

				comando.AdicionarParametroEntrada("especificidadeId", especificidadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("informacaoId", informacaoId, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);

			}
		}	

		#endregion
	}
}