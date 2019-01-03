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
	public class OutrosInformacaoCorteDeclaratorioDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public OutrosInformacaoCorteDeclaratorioDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(OutrosInformacaoCorte certidao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Certidao de Anuencia

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_out_informacao_corte e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update esp_out_informacao_corte t set
					t.informacao_corte = :informacao_corte,
					t.validade = :validade,
					t.tid = :tid where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					certidao.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into esp_out_informacao_corte (id, tid, titulo, informacao_corte, validade)
					values (seq_esp_out_informacao_corte.nextval, :tid, :titulo, :informacao_corte, :validade) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("informacao_corte", certidao.InformacaoCorte, DbType.Int32);
				comando.AdicionarParametroEntrada("validade", certidao.Validade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					certidao = certidao ?? new OutrosInformacaoCorte();
					certidao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				Historico.Gerar(Convert.ToInt32(certidao.Titulo.Id), eHistoricoArtefatoEspecificidade.outrosinformacaocorte, acao, bancoDeDados);

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
				#region Título de Certidao de Anuencia

				Comando comando = bancoDeDados.CriarComando(@"
				select
					e.id Id,
					e.tid Tid,
					tta.atividade Atividade,
					e.informacao_corte InformacaoCorte,
					e.validade Validade,
					tt.requerimento RequerimentoId,
					(case when tt.credenciado is null then (select nvl(p.nome, p.razao_social) from tab_requerimento r, tab_pessoa p where p.id = r.interessado and r.id = tt.requerimento) else 
					(select nvl(p.nome, p.razao_social) from cre_requerimento r, cre_pessoa p where p.id = r.interessado and r.id = tt.requerimento) end) Interessado
				from
					esp_out_informacao_corte     e,
					tab_titulo            tt,
					tab_titulo_atividades tta
				where
					e.titulo = tt.id
				and
					tta.titulo = tt.id(+)
				and
					e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade = bancoDeDados.ObterEntity<OutrosInformacaoCorte>(comando);

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