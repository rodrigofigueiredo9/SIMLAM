﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Data
{
	public class OutrosInformacaoCorteDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public OutrosInformacaoCorteDa(string strBancoDeDados = null)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

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
					t.crt_informacao_corte = :informacao_corte,
					t.validade = :validade,
					t.tid = :tid where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					certidao.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into esp_out_informacao_corte (id, tid, titulo, crt_informacao_corte, validade, informacao_corte)
					values (seq_esp_out_informacao_corte.nextval, :tid, :titulo, :informacao_corte, :validade, 0) returning id into :id", EsquemaBanco);

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_out_informacao_corte c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.outrosinformacaocorte, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_out_informacao_corte e where e.titulo = :titulo", EsquemaBanco);

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
					e.crt_informacao_corte InformacaoCorte,
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

		internal Outros ObterDadosPDF(int titulo, int user, BancoDeDados banco = null)
		{
			Outros outros = new Outros();
			InformacaoCorteBus infoCorteBus = new InformacaoCorteBus();
			List<InformacaoCorte> infoCorte = null;
			InformacaoCorte infoCorteInfo = null;
			int infoCorteInfoId = 0;
			int empreendimentoId = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo);

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
					where tt.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						outros.Destinatario.NomeRazaoSocial = reader.GetValue<string>("nome_razao");
						outros.Destinatario.CPFCNPJ = reader.GetValue<string>("cpf_cnpj");
						outros.Destinatario.VinculoTipoTexto = reader.GetValue<string>("vinculoPropriedade");
						outros.Destinatario.RGIE = reader.GetValue<string>("rg");
						empreendimentoId = reader.GetValue<int>("empreendimento");
					}

					reader.Close();

				}

				#endregion

				#region Assinantes

				outros.Titulo.Assinante = new AssinanteDefault { Cargo = "Proprietário", Nome = outros.Destinatario.NomeRazaoSocial };

				comando = bancoDeDados.CriarComando(@"
					SELECT NVL(P.NOME, P.RAZAO_SOCIAL) nome
						FROM TAB_PESSOA p 
					INNER JOIN TAB_CREDENCIADO C ON P.ID = C.PESSOA
					WHERE C.USUARIO = :usuario", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("usuario", user, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
						outros.Autor.NomeRazaoSocial = reader.GetValue<string>("nome");

					reader.Close();
				}

				#endregion

				#region Empreendimento


				comando = bancoDeDados.CriarComando(@"
					select i.area_flor_plantada,
							nvl(  (select sum(dd.area_croqui) from {0}crt_dominialidade_dominio dd
										where exists (select 1 from {0}crt_dominialidade d
											where d.id = dd.dominialidade and d.empreendimento = i.empreendimento)), 
								  i.area_imovel
								) area_croqui,
							'IC / ' || i.id || ' - ' || i.data_informacao carac
						from {0}crt_informacao_corte i 
						inner join esp_out_informacao_corte o on o.crt_informacao_corte = i.id
						where o.titulo = :titulo ", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						outros.InformacaoCorte.AreaPlantada = reader.GetValue<decimal>("area_flor_plantada");
						outros.InformacaoCorte.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						outros.InformacaoCorte.Caracterizacao = reader.GetValue<string>("carac");
					}

					reader.Close();

				}

				#endregion

				#region Licença

				comando = bancoDeDados.CriarComando(@"
					select tm.sigla || '-' || t.data_vencimento licenca
						from tab_titulo t 
							inner join tab_titulo_modelo tm on tm.id = t.modelo
					where t.modelo in (23, 24) and t.id = :titulo and rownum <= 1
					order by t.data_vencimento desc 
						", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
						outros.InformacaoCorte.LicençaAmbiental = reader.GetValue<string>("licenca");

					reader.Close();
				}

				if (String.IsNullOrWhiteSpace(outros.InformacaoCorte.LicençaAmbiental))
				{
					comando = bancoDeDados.CriarComando(@"
					select c.tipo_licenca || ' - ' || c.data_vencimento licenca
						from {0}crt_inf_corte_licenca c 
						inner join esp_out_informacao_corte o on o.crt_informacao_corte = c.corte_id
						where o.titulo = :titulo and rownum <= 1
					order by c.data_vencimento desc
						", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
							outros.InformacaoCorte.LicençaAmbiental = reader.GetValue<string>("licenca");

						reader.Close();
					}
				}

				#endregion

				#region Informação de corte

				comando = bancoDeDados.CriarComando(@"
					select  oic.titulo, ict.tipo_corte, ict.especie, ict.area_corte, ict.idade_plantio, icd.dest_material,
					lvd.texto dest_mat, lvp.texto produto, icd.quantidade
						from {0}crt_informacao_corte ic
							inner join esp_out_informacao_corte       oic on ic.id = oic.crt_informacao_corte
							inner join {0}crt_inf_corte_tipo             ict on ic.id = ict.corte_id
							inner join {0}crt_inf_corte_dest_material    icd on ict.id = icd.tipo_corte_id
							inner join lov_crt_inf_corte_inf_dest_mat lvd on lvd.id = icd.dest_material
							inner join idaf.lov_crt_produto                lvp on lvp.id = icd.produto
						where oic.titulo = :titulo", UsuarioCredenciado);

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

				#endregion
			}

			return outros;
		}


		#endregion
	}
}