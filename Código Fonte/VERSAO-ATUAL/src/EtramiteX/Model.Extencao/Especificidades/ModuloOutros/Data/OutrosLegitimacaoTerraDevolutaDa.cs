using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Data
{
	public class OutrosLegitimacaoTerraDevolutaDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBancoGeo { get; set; }
		private string EsquemaBanco { get; set; }

		#endregion

		public OutrosLegitimacaoTerraDevolutaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioGeo);

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(OutrosLegitimacaoTerraDevoluta outro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_out_legitima_terr_devolut e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", outro.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"
					update {0}esp_out_legitima_terr_devolut t set t.protocolo = :protocolo, t.dominio = :dominio, t.tid = :tid, t.valor_terreno = :valor_terreno, 
					t.is_inalienabilidade = :is_inalienabilidade, t.municipio_gleba = :municipio_gleba where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					outro.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}esp_out_legitima_terr_devolut (id, titulo, protocolo, dominio, tid, valor_terreno, is_inalienabilidade, municipio_gleba) values 
					({0}seq_esp_out_legitim_ter_devol.nextval, :titulo, :protocolo, :dominio, :tid, :valor_terreno, :is_inalienabilidade, :municipio_gleba) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", outro.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", outro.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("dominio", outro.Dominio, DbType.Int32);
				comando.AdicionarParametroEntrada("is_inalienabilidade", (outro.IsInalienabilidade.Value ? 1 : 0), DbType.Int32);
				comando.AdicionarParametroEntrada("valor_terreno", outro.ValorTerreno, DbType.Decimal);
				comando.AdicionarParametroEntrada("municipio_gleba", outro.MunicipioGlebaId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					outro = outro ?? new OutrosLegitimacaoTerraDevoluta();
					outro.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Destinatário

				comando = bancoDeDados.CriarComando("delete from {0}esp_out_legitima_destinatario t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, outro.Destinatarios.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", outro.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in outro.Destinatarios)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_out_legitima_destinatario t set t.destinatario = :destinatario, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_out_legitima_destinatario (id, especificidade, destinatario, tid) values ({0}seq_esp_out_legitima_destinat.nextval, 
							:especificidade, :destinatario, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", outro.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("destinatario", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(outro.Titulo.Id, eHistoricoArtefatoEspecificidade.outroslegitimacaoterradevoluta, acao, bancoDeDados);

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
						"update {0}esp_out_legitima_terr_devolut c set c.tid = :tid where c.titulo = :titulo; " +
						"update {0}esp_out_legitima_destinatario c set c.tid = :tid where c.especificidade = (select id from esp_out_legitima_terr_devolut where titulo = :titulo); " +
					"end; ", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.outroslegitimacaoterradevoluta, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(
					"begin " +
						"delete from {0}esp_out_legitima_destinatario c where c.especificidade = (select id from esp_out_legitima_terr_devolut where titulo = :titulo); " +
						"delete from {0}esp_out_legitima_terr_devolut e where e.titulo = :titulo; " +
					"end; ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal OutrosLegitimacaoTerraDevoluta Obter(int titulo, BancoDeDados banco = null)
		{
			OutrosLegitimacaoTerraDevoluta especificidade = new OutrosLegitimacaoTerraDevoluta();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"
				select e.id, e.titulo, e.protocolo, e.dominio, e.tid, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.is_inalienabilidade, e.valor_terreno, e.municipio_gleba 
				from {0}esp_out_legitima_terr_devolut e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id and e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("id");
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.Dominio = reader.GetValue<int>("dominio");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.Tipo = reader.GetValue<int>("protocolo_tipo");
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
						especificidade.IsInalienabilidade = reader.GetValue<bool?>("is_inalienabilidade");
						especificidade.ValorTerreno = reader.GetValue<string>("valor_terreno");
						especificidade.MunicipioGlebaId = reader.GetValue<int>("municipio_gleba");
					}

					reader.Close();
				}

				#region Destinatário

				comando = bancoDeDados.CriarComando(@" select p.id Id, t.id IdRelacionamento, nvl(p.nome, p.razao_social) Nome from {0}esp_out_legitima_destinatario t, {0}esp_out_legitima_terr_devolut e, {0}tab_pessoa p where 
					t.especificidade = e.id and e.titulo = :titulo and p.id = t.destinatario", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.Destinatarios = bancoDeDados.ObterEntityList<OutrosLegitimacaoDestinatario>(comando);

				#endregion

				#endregion
			}

			return especificidade;
		}

		internal OutrosLegitimacaoTerraDevoluta ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			OutrosLegitimacaoTerraDevoluta especificidade = new OutrosLegitimacaoTerraDevoluta();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				if (situacao > 0)
				{
					comando = bancoDeDados.CriarComando(@"
					select e.id,
						e.especificidade_id,
						e.tid,
						p.protocolo_id protocolo_tipo,
						e.protocolo_id,
						p.requerimento_id, 
						n.numero,
						n.ano,
						e.dominio_id, 
						e.is_inalienabilidade,
						e.valor_terreno,
						e.municipio_gleba_id,
						e.municipio_gleba_texto
					from {0}hst_esp_out_legit_terr_devolu e,
						 {0}hst_titulo_numero             n,
						 {0}hst_protocolo                 p
					where e.titulo_id = n.titulo_id(+)
						and e.titulo_tid = n.titulo_tid(+)
						and e.protocolo_id = p.id_protocolo(+)
						and e.protocolo_tid = p.tid(+)
						and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
						and e.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
						(select max(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo_id and htt.situacao_id = :situacao))
						and e.titulo_id = :titulo", EsquemaBanco);

					comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				}
				else
				{
					//Pegar a proxima linha de historico apos a ultima situacao de cadastrado
					comando = bancoDeDados.CriarComando(@"
					select e.id,
						e.especificidade_id,
						e.tid,
						p.protocolo_id protocolo_tipo,
						e.protocolo_id,
						p.requerimento_id,
						n.numero,
						n.ano,
						e.dominio_id, 
						e.is_inalienabilidade,
						e.valor_terreno,
						e.municipio_gleba_id,
						e.municipio_gleba_texto
					from {0}hst_esp_out_legit_terr_devolu e,
						 {0}hst_titulo_numero             n,
						 {0}hst_protocolo                 p
					where e.titulo_id = n.titulo_id(+)
						and e.titulo_tid = n.titulo_tid(+)
						and e.protocolo_id = p.id_protocolo(+)
						and e.protocolo_tid = p.tid(+)
						and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
						and e.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
						(select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo_id and htt.data_execucao >
						(select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo_id and httt.situacao_id = 1)))
						and e.titulo_id = :titulo", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("especificidade_id");
						especificidade.Tid = reader.GetValue<string>("tid");

						especificidade.ProtocoloReq.Tipo = reader.GetValue<int>("protocolo_tipo");
						especificidade.ProtocoloReq.IsProcesso = especificidade.ProtocoloReq.Tipo == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento_id");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo_id");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
						especificidade.Dominio = reader.GetValue<int>("dominio_id");
						especificidade.IsInalienabilidade = reader.GetValue<bool?>("is_inalienabilidade");
						especificidade.ValorTerreno = reader.GetValue<string>("valor_terreno");
						especificidade.MunicipioGlebaId = reader.GetValue<int>("municipio_gleba_id");
						especificidade.MunicipioGlebaTexto = reader.GetValue<string>("municipio_gleba_texto");
					}

					reader.Close();
				}

				#endregion

				#region Destinatário

				comando = bancoDeDados.CriarComando(@"select distinct d.legit_desti IdRelacionamento, p.pessoa_id Id, nvl(p.nome, p.razao_social) Nome
				from {0}hst_esp_out_legit_destinatari d, {0}hst_pessoa p where d.destinatario_id = p.pessoa_id and d.destinatario_tid = p.tid
				and p.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = p.pessoa_id and h.tid = p.tid) and d.hst_id = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);

				especificidade.Destinatarios = bancoDeDados.ObterEntityList<OutrosLegitimacaoDestinatario>(comando);

				#endregion
			}

			return especificidade;
		}

		internal Outros ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Outros outros = new Outros();
			int dominioId = 0;

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

				Comando comando = bancoDeDados.CriarComando(@"
				select e.valor_terreno, e.dominio, e.is_inalienabilidade, lm.texto municipio_gleba_texto 
				from {0}esp_out_legitima_terr_devolut e, {0}lov_municipio lm where lm.id(+) = e.municipio_gleba and e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						dominioId = reader.GetValue<int>("dominio");
						outros.ValorTerreno = reader.GetValue<decimal>("valor_terreno").ToString("N2");
						outros.IsInalienabilidade = reader.GetValue<bool>("is_inalienabilidade");
						outros.Municipio = reader.GetValue<string>("municipio_gleba_texto");
					}

					reader.Close();
				}

				#endregion

				//#region Destinatarios

				//comando = bancoDeDados.CriarComando(@" select d.destinatario from {0}esp_out_legitima_destinatario d, {0}esp_out_legitima_terr_devolut e 
				//where d.especificidade = e.id and e.titulo = :tituloId ", EsquemaBanco);
				//comando.AdicionarParametroEntrada("tituloId", titulo, DbType.Int32);

				//using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				//{
				//	while (reader.Read())
				//	{
				//		var pessoa = DaEsp.ObterDadosPessoa(reader.GetValue<int>("destinatario"), banco: bancoDeDados);
				//		outros.Destinatarios.Add(pessoa);
				//		outros.Titulo.AssinanteSource.Add(new AssinanteDefault { Cargo = "Titulado", Nome = pessoa.NomeRazaoSocial });
				//	}

				//	reader.Close();
				//}

				//#endregion

				#region Dominialidade

				DominialidadeBus dominialidadeBus = new DominialidadeBus();
				outros.Dominialidade = new DominialidadePDF(dominialidadeBus.ObterPorEmpreendimento(outros.Empreendimento.Id.GetValueOrDefault(), banco: bancoDeDados));

				#endregion

				#region Regularizacao Fundiaria

				RegularizacaoFundiaria regularizacao = new RegularizacaoFundiariaBus().ObterPorEmpreendimento(outros.Empreendimento.Id.GetValueOrDefault());
				Posse posse = regularizacao.Posses.SingleOrDefault(x => x.Id.GetValueOrDefault() == dominioId);

				if (posse != null)
				{
					PossePDF possePDF = new PossePDF(posse);
					outros.RegularizacaoFundiaria.Posses.Add(possePDF);
					outros.RegularizacaoFundiaria.Posse = possePDF;
					outros.Dominio = outros.Dominialidade.Dominios.SingleOrDefault(x => x.Id == possePDF.DominioId);
				}

				#endregion
			}

			return outros;
		}

		internal List<ListaValor> ObterDominios(int protocoloId)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@" select distinct d.id, d.identificacao || '/' || dc.texto || ' - ' texto, d.area_croqui  from {0}crt_regularizacao_dominio d, {0}crt_dominialidade_dominio dd,
					{0}lov_crt_domin_comprovacao dc, {0}tab_protocolo p, {0}crt_regularizacao cr where d.dominio = dd.id and dd.comprovacao = dc.id and p.empreendimento = cr.empreendimento and cr.id = d.regularizacao
					and p.id = :protocoloId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocoloId", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ListaValor item;

					while (reader.Read())
					{
						item = new ListaValor();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["texto"].ToString() + Convert.ToDecimal(reader["area_croqui"]).ToStringTrunc();
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

		internal bool IsDominioCadastrado(int dominioId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(" select count(1) from {0}crt_regularizacao_dominio c where c.id = :dominioId ", EsquemaBanco);
				comando.AdicionarParametroEntrada("dominioId", dominioId, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal TituloEsp IsDominioAssociado(int dominioId, int tituloID)
		{
			TituloEsp titulo = new TituloEsp();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select n.numero, n.ano, t.situacao, s.texto situacao_texto 
				from {0}esp_out_legitima_terr_devolut e, {0}tab_titulo t, {0}tab_titulo_numero n, {0}lov_titulo_situacao s 
				where t.id = e.titulo and n.titulo(+) = t.id and s.id = t.situacao and t.situacao in (1, 2, 3, 4, 6) and e.titulo != :titulo and e.dominio = :dominioId", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", tituloID, DbType.Int32);
				comando.AdicionarParametroEntrada("dominioId", dominioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						titulo.Numero.Ano = reader.GetValue<int>("ano");
						titulo.SituacaoId = reader.GetValue<int>("situacao");
						titulo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
					}

					reader.Close();
				}
			}

			return titulo;
		}

		#endregion
	}
}