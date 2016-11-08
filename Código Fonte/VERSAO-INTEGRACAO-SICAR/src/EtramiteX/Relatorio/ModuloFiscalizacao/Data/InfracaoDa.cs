using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class InfracaoDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }

		#endregion

		public InfracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}

		}

		#region Obter

		public InfracaoRelatorio Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			InfracaoRelatorio objeto = new InfracaoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select i.id Id, i.gerado_sistema IsGeradoSistema, nvl(i.numero_auto_infracao_bloco, f.autos) NumeroAI, ls.texto SerieTexto,
				   TO_CHAR(i.data_lavratura_auto, 'DD/MM/YYYY') DataLavraturaAI, i.descricao_infracao DescricaoInfracao,
				   c.texto Classificacao, t.texto Tipo, it.texto Item, s.texto Subitem 
				from {0}tab_fiscalizacao f,
				   {0}tab_fisc_infracao i, 
				   {0}lov_cnf_fisc_infracao_classif c, 
				   {0}cnf_fisc_infracao_tipo t, 
				   {0}cnf_fisc_infracao_item it,
				   {0}lov_fiscalizacao_serie ls, 
				   {0}cnf_fisc_infracao_subitem s          
				where i.serie = ls.id(+)
				   and i.subitem = s.id(+) 
				   and i.item = it.id
				   and i.tipo = t.id
				   and i.classificacao = c.id
				   and i.fiscalizacao = f.id 
				   and f.id = :fiscalizacaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<InfracaoRelatorio>(comando);
				objeto.SerieTexto = String.IsNullOrWhiteSpace(objeto.NumeroAI) ? String.Empty : objeto.SerieTexto;
				objeto.Campos = ObterCampos(objeto.Id, bancoDeDados);
				objeto.Perguntas = ObterPerguntas(objeto.Id, bancoDeDados);

				if (objeto.DataLavraturaAI == null)
				{
					objeto.DataLavraturaAI = new FiscalizacaoDa().ObterDataConclusao(fiscalizacaoId, bancoDeDados).DataTexto;
				}

				if (!objeto.IsGeradoSistema.HasValue)
				{
					objeto.NumeroAI = null;
					objeto.DataLavraturaAI = null;
				}
			}

			return objeto;
		}

		public InfracaoRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
		{
			InfracaoRelatorio objeto = new InfracaoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"
					select i.id HistoricoId, i.infracao_id Id, i.gerado_sistema IsGeradoSistema, nvl(i.numero_auto_infracao_bloco, f.autos) NumeroAI, i.serie_texto SerieTexto, 
					   TO_CHAR(nvl(i.data_lavratura_auto, f.situacao_data), 'DD/MM/YYYY') DataLavraturaAI, i.descricao_infracao DescricaoInfracao,
					   i.classificacao_texto Classificacao, t.texto Tipo, it.texto Item, s.texto Subitem                     
					from 
					   {0}hst_fiscalizacao f,
					   {0}hst_fisc_infracao i, 
					   {0}hst_cnf_fisc_infracao_tipo t, 
					   {0}hst_cnf_fisc_infracao_item  it,
					   {0}hst_cnf_fisc_infracao_subitem s
					where 
					   i.tipo_id = t.infracao_tipo_id
					   and i.tipo_tid = t.tid
					   and i.item_id = it.infracao_item_id
					   and i.item_tid = it.tid
					   and i.subitem_id = s.infracao_subitem_id(+)
					   and i.subitem_tid = s.tid(+)
					   and i.fiscalizacao_id_hst = f.id 
					   and f.id = :historicoId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<InfracaoRelatorio>(comando);
				objeto.SerieTexto = String.IsNullOrWhiteSpace(objeto.NumeroAI) ? String.Empty : objeto.SerieTexto;
				objeto.Campos = ObterCamposHistorico(objeto.HistoricoId, bancoDeDados);
				objeto.Perguntas = ObterPerguntasHistorico(objeto.HistoricoId, bancoDeDados);

				if (!objeto.IsGeradoSistema.HasValue)
				{
					objeto.NumeroAI = null;
					objeto.DataLavraturaAI = null;
				}
			}

			return objeto;
		}

		private List<InfracaoCampoRelatorio> ObterCampos(int infracaoid, BancoDeDados banco = null)
		{
			List<InfracaoCampoRelatorio> colecao = new List<InfracaoCampoRelatorio>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select (case when cc.unidade is null then cc.texto else cc.texto || ' (' || lu.texto || ')' end) Texto, c.texto valor 
													from {0}tab_fisc_infracao_campo c, {0}cnf_fisc_infracao_campo cc, {0}lov_cnf_fisc_infracao_camp_uni lu 
													where c.campo = cc.id and c.infracao = :infracaoid and lu.id = cc.unidade", EsquemaBanco);

				comando.AdicionarParametroEntrada("infracaoid", infracaoid, DbType.Int32);

				colecao = bancoDeDados.ObterEntityList<InfracaoCampoRelatorio>(comando);
			}

			return colecao;
		}

		private List<InfracaoCampoRelatorio> ObterCamposHistorico(int historicoInfracaoid, BancoDeDados banco = null)
		{
			List<InfracaoCampoRelatorio> colecao = new List<InfracaoCampoRelatorio>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select (case when cc.unidade is null then cc.texto else cc.texto || ' (' || cc.unidade_texto || ')' end) Texto, c.texto valor 
					from 
						{0}hst_fisc_infracao_campo c, 
						{0}hst_cnf_fisc_infracao_campo cc
					where 
						c.campo_id = cc.infracao_campo_id
						and c.campo_tid = cc.tid
						and c.infracao_id_hst = :historicoInfracaoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoInfracaoid", historicoInfracaoid, DbType.Int32);

				colecao = bancoDeDados.ObterEntityList<InfracaoCampoRelatorio>(comando);
			}

			return colecao;
		}

		private List<InfracaoPerguntaRelatorio> ObterPerguntas(int infracaoid, BancoDeDados banco = null)
		{
			List<InfracaoPerguntaRelatorio> colecao = new List<InfracaoPerguntaRelatorio>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select cp.texto Pergunta, r.texto Resposta, p.especificacao Especificacao from {0}tab_fisc_infracao_pergunta p, 
					{0}cnf_fisc_infracao_pergunta cp, {0}cnf_fisc_infracao_resposta r where p.pergunta = cp.id and p.resposta = r.id and p.infracao = :infracaoId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("infracaoid", infracaoid, DbType.Int32);

				colecao = bancoDeDados.ObterEntityList<InfracaoPerguntaRelatorio>(comando);
			}

			return colecao;
		}

		private List<InfracaoPerguntaRelatorio> ObterPerguntasHistorico(int historicoInfracaoid, BancoDeDados banco = null)
		{
			List<InfracaoPerguntaRelatorio> colecao = new List<InfracaoPerguntaRelatorio>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select cp.texto Pergunta, r.texto Resposta, p.especificacao Especificacao  
					from 
					   {0}hst_fisc_infracao_pergunta p, 
					   {0}hst_cnf_fisc_infracao_pergunta cp, 
					   {0}hst_cnf_fisc_infracao_resposta r 
					where 
					   p.pergunta_id = cp.pergunta_id
					   and p.pergunta_tid = cp.tid
					   and p.resposta_id = r.resposta_id
					   and p.resposta_tid = r.tid
					   and p.infracao_id_hst = :historicoInfracaoid 
					order by p.pergunta_id, p.resposta_id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoInfracaoid", historicoInfracaoid, DbType.Int32);

				colecao = bancoDeDados.ObterEntityList<InfracaoPerguntaRelatorio>(comando);
			}

			return colecao;
		}

		#endregion
	}
}
