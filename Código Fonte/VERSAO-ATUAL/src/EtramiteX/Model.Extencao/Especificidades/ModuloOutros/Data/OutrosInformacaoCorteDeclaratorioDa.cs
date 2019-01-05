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
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
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
			int empreendimentoId = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(151048, bancoDeDados);

				outros.Titulo = dados.Titulo;
				outros.Titulo.SetorEndereco = DaEsp.ObterEndSetor(outros.Titulo.SetorId);
				outros.Protocolo = dados.Protocolo;
				outros.Empreendimento = dados.Empreendimento;

				#endregion

				#region Interessado
				Comando comando = bancoDeDados.CriarComando(@"
					select tt.requerimento, r.empreendimento, r.interessado, 
					nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj, 
					nvl(lv.texto, ' -- ') vinculoPropriedade, nvl(p.rg, ' -- ') rg
					from tab_titulo                             tt 
					  inner join {0}tab_requerimento               r   on tt.requerimento = r.id
					  inner join {0}tab_pessoa                     p   on r.interessado = p.id
					  inner join {0}tab_empreendimento_responsavel er  on p.id = er.responsavel
					  inner join lov_empreendimento_tipo_resp   lv  on er.tipo = lv.id
					where tt.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", 151048, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						outros.Interessado.NomeRazaoSocial = reader.GetValue<string>("nome_razao");
						outros.Interessado.CPFCNPJ = reader.GetValue<string>("cpf_cnpj");
						outros.Interessado.VinculoTipoTexto = reader.GetValue<string>("vinculoPropriedade");
						outros.Interessado.RGIE = reader.GetValue<string>("rg");
						empreendimentoId = reader.GetValue<int>("empreendimento");

						//_daEsp.ObterDadosPessoa(reader.GetValue<int>("destinatario"), outros.Empreendimento.Id, bancoDeDados);
					}

					reader.Close();

				}

				#endregion

				#region Empreendimento


				comando = bancoDeDados.CriarComando(@"
					select e.codigo, lv.texto segmento, e.denominador, e.cnpj, ee.bairro, ee.distrito, lvm.texto municipio, ee.complemento,

						nvl((select cs.codigo_imovel from tab_controle_sicar cs
								where cs.empreendimento = e.id and cs.solicitacao_car_esquema = 1 and codigo_imovel is not null),
							'') codigo_imovel,
						(select sum(dd.area_croqui) from {0}crt_dominialidade_dominio dd
							where exists (select 1 from {0}crt_dominialidade d
								where d.id = dd.dominialidade and d.empreendimento = e.id)) area_croqui,
						(case ee.zona when 1 then 'Zona Urbana' when 2 then 'Zona Rural' end) zona,
						(select max(c.area_flor_plantada) from {0}crt_informacao_corte c inner join esp_out_informacao_corte es 
						  on c.id = es.informacao_corte where es.titulo = :titulo) area_plantada
						
					from {0}tab_empreendimento e
					inner join {0}tab_empreendimento_endereco ee on e.id = ee.empreendimento
					inner join lov_empreendimento_segmento lv on lv.id = e.segmento
					inner join lov_municipio              lvm on lvm.id = ee.municipio

					where ee.correspondencia = 0 and e.id = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						outros.Empreendimento.Codigo = reader.GetValue<string>("codigo");
						outros.Empreendimento.CodigoImovel = reader.GetValue<string>("codigo_imovel");
						outros.Empreendimento.EndZona = reader.GetValue<string>("zona");
						outros.Empreendimento.Segmento = reader.GetValue<string>("segmento");
						outros.Empreendimento.Nome = reader.GetValue<string>("denominador");
						outros.Empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						outros.Empreendimento.EndBairro = reader.GetValue<string>("bairro");
						outros.Empreendimento.EndDistrito = reader.GetValue<string>("distrito");
						outros.Empreendimento.EndMunicipio = reader.GetValue<string>("municipio");
						outros.Empreendimento.EndComplemento = reader.GetValue<string>("complemento");
						outros.Empreendimento.ATPCroquiDecimal = reader.GetValue<decimal>("area_croqui");
						outros.InformacaoCorte.AreaPlantada = reader.GetValue<decimal>("area_plantada");
					}

					reader.Close();

				}

				#endregion

				#region Informação de corte

				comando = bancoDeDados.CriarComando(@"
					select  oic.titulo, ict.tipo_corte, ict.especie, ict.area_corte, ict.idade_plantio, icd.dest_material,
					lvd.texto dest_mat, lvp.texto produto, icd.quantidade
						from {0}crt_informacao_corte ic
							inner join esp_out_informacao_corte       oic on ic.id = oic.informacao_corte
							inner join {0}crt_inf_corte_tipo             ict on ic.id = ict.corte_id
							inner join {0}crt_inf_corte_dest_material    icd on ict.id = icd.tipo_corte_id
							inner join lov_crt_inf_corte_inf_dest_mat lvd on lvd.id = icd.dest_material
							inner join lov_crt_produto                lvp on lvp.id = icd.produto
						where oic.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						InformacaoCorteInfoPDF ic = new InformacaoCorteInfoPDF();
						ic.TipoCorte = Enum.ToObject(typeof(eTipoCorte), reader.GetValue<int>("tipo_corte")).ToDescription();
						ic.Especie = Enum.ToObject(typeof(eEspecieInformada), reader.GetValue<int>("especie")).ToDescription();
						ic.AreaCorte = reader.GetValue<decimal>("area_corte");
						ic.IdadePlantio = reader.GetValue<int>("idade_plantio");
						ic.DestMaterial = reader.GetValue<string>("dest_mat");
						ic.Produto = reader.GetValue<string>("produto");
						ic.Quantidade = reader.GetValue<decimal>("quantidade");

						outros.InformacaoCorte.InformacoesDeCorte.Add(ic);
					}

					reader.Close();

				}

				//infoCorte = infoCorteBus.ObterPorEmpreendimento(outros.Empreendimento.Id.GetValueOrDefault(), banco: bancoDeDados);

				//if (infoCorte != null)
				//{
				//	infoCorteInfo = infoCorte.SingleOrDefault(x => x.Id == infoCorteInfoId);

				//	if (infoCorteInfo != null)
				//	{
				//		outros.InformacaoCorteInfo = new InformacaoCorteInfoPDF(infoCorteInfo);
				//	}
				//}

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