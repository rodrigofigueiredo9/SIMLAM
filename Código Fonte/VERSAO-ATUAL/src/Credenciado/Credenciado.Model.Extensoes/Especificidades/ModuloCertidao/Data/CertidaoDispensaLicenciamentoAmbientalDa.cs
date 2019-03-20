using Exiges.Negocios.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloCertidao.Data
{
	public class CertidaoDispensaLicenciamentoAmbientalDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private String EsquemaCredenciadoBanco { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }
		private string EsquemaBanco { get; set; }

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public CertidaoDispensaLicenciamentoAmbientalDa(string strBancoDeDados = null)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(CertidaoDispensaLicenciamentoAmbiental certidao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Certidao de Anuencia

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_cert_disp_amb e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update esp_cert_disp_amb t set t.vinculo_propriedade = :vinculo_propriedade, t.vinculo_propriedade_outro = :vinculo_propriedade_outro, t.tid = :tid where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					certidao.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into esp_cert_disp_amb (id, tid, titulo, vinculo_propriedade, vinculo_propriedade_outro)
					values (seq_esp_cert_disp_amb.nextval, :tid, :titulo, :vinculo_propriedade, :vinculo_propriedade_outro) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("vinculo_propriedade", certidao.VinculoPropriedade, DbType.Int32);
				comando.AdicionarParametroEntrada("vinculo_propriedade_outro", DbType.String, 50, certidao.VinculoPropriedadeOutro);

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					certidao = certidao ?? new CertidaoDispensaLicenciamentoAmbiental();
					certidao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				Historico.Gerar(Convert.ToInt32(certidao.Titulo.Id), eHistoricoArtefatoEspecificidade.certdisplicenamb, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_cert_disp_amb c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.certdisplicenamb, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_cert_disp_amb e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal CertidaoDispensaLicenciamentoAmbiental Obter(int titulo, BancoDeDados banco = null)
		{
			CertidaoDispensaLicenciamentoAmbiental especificidade = new CertidaoDispensaLicenciamentoAmbiental();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Certidao de Anuencia

				Comando comando = bancoDeDados.CriarComando(@"
				select
					e.id Id,
					e.tid Tid,
					tta.atividade Atividade,
					e.vinculo_propriedade VinculoPropriedade,
					e.vinculo_propriedade_outro VinculoPropriedadeOutro,
					tt.requerimento RequerimentoId,
					(case when tt.credenciado is null then (select nvl(p.nome, p.razao_social) from tab_requerimento r, tab_pessoa p where p.id = r.interessado and r.id = tt.requerimento) else 
					(select nvl(p.nome, p.razao_social) from cre_requerimento r, cre_pessoa p where p.id = r.interessado and r.id = tt.requerimento) end) Interessado
				from
					esp_cert_disp_amb     e,
					tab_titulo            tt,
					tab_titulo_atividades tta
				where
					e.titulo = tt.id
				and
					tta.titulo = tt.id(+)
				and
					e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade = bancoDeDados.ObterEntity<CertidaoDispensaLicenciamentoAmbiental>(comando);

				#endregion
			}

			return especificidade;
		}

		internal CertidaoDispensaLicenciamentoAmbientalPDF ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			CertidaoDispensaLicenciamentoAmbientalPDF certidao = new CertidaoDispensaLicenciamentoAmbientalPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				certidao.Titulo = dados.Titulo;
				certidao.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select l.texto vinculo_propriedade, vinculo_propriedade_outro 
				from esp_cert_disp_amb e, tab_titulo t, lov_esp_cert_disp_amb l where t.id = e.titulo and l.id  = e.vinculo_propriedade and e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						certidao.VinculoPropriedade = reader.GetValue<string>("vinculo_propriedade");
						certidao.VinculoPropriedadeOutro = reader.GetValue<string>("vinculo_propriedade_outro");
					}

					reader.Close();
				}

				#endregion
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf 
				from tab_requerimento r, tab_pessoa p where p.id = r.interessado and r.id = :requerimento", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("requerimento", certidao.Titulo.Requerimento.Numero, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						certidao.Interessado.NomeRazaoSocial = reader.GetValue<string>("nome");
						certidao.Interessado.CPFCNPJ = reader.GetValue<string>("cpf");
					}

					reader.Close();
				}

				#endregion

				#region Pessoas

				List<PessoaPDF> pessoas = new List<PessoaPDF>();

				comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome_razao, 'Interessado' vinculo_tipo 
				from tab_requerimento r, tab_pessoa p where p.id = r.interessado and r.id = :requerimento
				union all 
				select nvl(p.nome, p.razao_social) nome_razao, 'Responsável Técnico' vinculo_tipo 
				from tab_requerimento_responsavel r, tab_pessoa p where p.id = r.responsavel and r.requerimento = :requerimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", certidao.Titulo.Requerimento.Numero, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaPDF pessoa = null;

					while (reader.Read())
					{
						pessoa = new PessoaPDF();
						pessoa.VinculoTipoTexto = reader.GetValue<string>("vinculo_tipo");
						pessoa.NomeRazaoSocial = reader.GetValue<string>("nome_razao");
						pessoas.Add(pessoa);
					}

					reader.Close();
				}

				pessoas.ForEach(item =>
				{
					certidao.Titulo.AssinanteSource.Add(new AssinanteDefault { Cargo = item.VinculoTipoTexto, Nome = item.NomeRazaoSocial });
				});

				#endregion Pesssoas
			}

			#region Barragem

			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();
			int barragemId;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select bdl.id from crt_barragem_dispensa_lic bdl
																inner join TAB_PROJ_DIGITAL_DEPENDENCIAS pdd on pdd.DEPENDENCIA_ID = bdl.ID
																inner join TAB_PROJETO_DIGITAL pd on pd.ID = pdd.PROJETO_DIGITAL_ID
																inner join IDAF.TAB_TITULO t on t.REQUERIMENTO = pd.REQUERIMENTO
															where bdl.empreendimento = :empreendimento and t.REQUERIMENTO = :requerimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", certidao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", certidao.Titulo.Requerimento.Numero, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);
				barragemId = Convert.ToInt32(valor);
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select  b.id, b.tid, b.empreendimento, b.atividade, b.tipo_barragem, b.curso_hidrico, 
							b.vazao_enchente, b.area_bacia_contribuicao, b.precipitacao, b.periodo_retorno, 
							b.coeficiente_escoamento, b.tempo_concentracao, b.equacao_calculo, b.area_alagada, 
							b.volume_armazenado, b.fase, b.interno_id, b.interno_tid, b.possui_barragem_contigua, 
							b.altura_barramento, b.comprimento_barramento, b.largura_base_barramento, b.largura_crista_barramento,
							b.fonte_precipitacao, b.fonte_coeficiente_escoamento, b.fonte_vazao_enchente,
							c.id contruida_construir, c.supressao_app, c.largura_demarcada, c.largura_demarcada_legislacao,
							c.faixa_cercada, c.descricao_desen_app, c.demarcacao_app, c.barramento_normas, 
							c.barramento_adequacoes, c.vazao_min_tipo, c.vazao_min_diametro, c.vazao_min_instalado, 
							c.vazao_min_normas, c.vazao_min_adequacoes, c.vazao_max_tipo, c.vazao_max_diametro,
							c.vazao_max_instalado, c.vazao_max_normas, c.vazao_max_adequacoes, c.periodo_inicio_obra, c.periodo_termino_obra
					from crt_barragem_dispensa_lic b
					inner join crt_barragem_construida_con c on b.id = c.barragem
					where b.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", barragemId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{ 
						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.CredenciadoID = barragemId;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");

						caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento");
						caracterizacao.AtividadeID = reader.GetValue<int>("atividade");
						caracterizacao.BarragemTipo = (eBarragemTipo)reader.GetValue<int>("tipo_barragem");
						caracterizacao.cursoHidrico = reader.GetValue<string>("curso_hidrico");
						caracterizacao.vazaoEnchente = reader.GetValue<decimal>("vazao_enchente");
						caracterizacao.areaBaciaContribuicao = reader.GetValue<decimal>("area_bacia_contribuicao");
						caracterizacao.precipitacao = reader.GetValue<decimal>("precipitacao");
						caracterizacao.periodoRetorno = reader.GetValue<decimal>("periodo_retorno");
						caracterizacao.coeficienteEscoamento = reader.GetValue<decimal>("coeficiente_escoamento");
						caracterizacao.tempoConcentracao = reader.GetValue<decimal>("tempo_concentracao");
						caracterizacao.tempoConcentracaoEquacaoUtilizada = reader.GetValue<string>("equacao_calculo");
						caracterizacao.areaAlagada = reader.GetValue<decimal>("area_alagada");
						caracterizacao.volumeArmazanado = reader.GetValue<decimal>("volume_armazenado");
						caracterizacao.faseInstalacao = (eFase)reader.GetValue<int>("fase");

						caracterizacao.barragemContiguaMesmoNivel = reader.GetValue<bool>("possui_barragem_contigua");
						caracterizacao.alturaBarramento = reader.GetValue<decimal>("altura_barramento");
						caracterizacao.comprimentoBarramento = reader.GetValue<decimal>("comprimento_barramento");
						caracterizacao.larguraBaseBarramento = reader.GetValue<decimal>("largura_base_barramento");
						caracterizacao.larguraCristaBarramento = reader.GetValue<decimal>("largura_crista_barramento");
						caracterizacao.fonteDadosPrecipitacao = reader.GetValue<string>("fonte_precipitacao");
						caracterizacao.fonteDadosCoeficienteEscoamento = reader.GetValue<string>("fonte_coeficiente_escoamento");
						caracterizacao.fonteDadosVazaoEnchente = reader.GetValue<string>("fonte_vazao_enchente");
						caracterizacao.construidaConstruir.id = reader.GetValue<int>("contruida_construir");
						caracterizacao.construidaConstruir.isSupressaoAPP = reader.GetValue<bool>("supressao_app");
						caracterizacao.construidaConstruir.larguraDemarcada = reader.GetValue<decimal>("largura_demarcada");
						caracterizacao.construidaConstruir.larguraDemarcadaLegislacao = reader.GetValue<bool>("largura_demarcada_legislacao");
						caracterizacao.construidaConstruir.faixaCercada = reader.GetValue<int>("faixa_cercada");
						caracterizacao.construidaConstruir.descricaoDesenvolvimentoAPP = reader.GetValue<string>("descricao_desen_app");
						caracterizacao.construidaConstruir.isDemarcacaoAPP = reader.GetValue<int>("demarcacao_app");
						caracterizacao.construidaConstruir.barramentoNormas = reader.GetValue<bool>("barramento_normas");
						caracterizacao.construidaConstruir.barramentoAdequacoes = reader.GetValue<string>("barramento_adequacoes");
						caracterizacao.construidaConstruir.vazaoMinTipo = reader.GetValue<int>("vazao_min_tipo");
						caracterizacao.construidaConstruir.vazaoMinDiametro = reader.GetValue<decimal>("vazao_min_diametro");
						caracterizacao.construidaConstruir.vazaoMinInstalado = reader.GetValue<bool>("vazao_min_instalado");
						caracterizacao.construidaConstruir.vazaoMinNormas = reader.GetValue<bool>("vazao_min_normas");
						caracterizacao.construidaConstruir.vazaoMinAdequacoes = reader.GetValue<string>("vazao_min_adequacoes");
						caracterizacao.construidaConstruir.vazaoMaxTipo = reader.GetValue<int>("vazao_max_tipo");
						caracterizacao.construidaConstruir.vazaoMaxDiametro = reader.GetValue<string>("vazao_max_diametro");
						caracterizacao.construidaConstruir.vazaoMaxInstalado = reader.GetValue<bool>("vazao_max_instalado");
						caracterizacao.construidaConstruir.vazaoMaxNormas = reader.GetValue<bool>("vazao_max_normas");
						caracterizacao.construidaConstruir.vazaoMaxAdequacoes = reader.GetValue<string>("vazao_max_adequacoes");
						caracterizacao.construidaConstruir.periodoInicioObra = reader.GetValue<string>("periodo_inicio_obra");
						caracterizacao.construidaConstruir.periodoTerminoObra = reader.GetValue<string>("periodo_termino_obra");
					}

					reader.Close();
				}

				#region Coordenadas
				comando = bancoDeDados.CriarComando(@"
					select c.id, c.tipo, c.northing, c.easting from crt_barragem_coordenada c
						where c.barragem = :barragem order by tipo", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);
				

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var obj = caracterizacao.coordenadas.FirstOrDefault(x => (int)x.tipo == reader.GetValue<int>("tipo"));
						obj.id = reader.GetValue<int>("id");
						obj.tipo = (eTipoCoordenadaBarragem)reader.GetValue<int>("tipo");
						obj.northing = reader.GetValue<int>("northing");
						obj.easting = reader.GetValue<int>("easting");
					}
				}
				#endregion

				#region Responsaveis Tecnicos
				comando = bancoDeDados.CriarComando(@"
					select r.id, r.tipo, r.nome, r.profissao, r.registro_crea, r.numero_art, r.autorizacao_crea, r.proprio_declarante
						from crt_barragem_responsavel r where r.barragem = :barragem", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					List<BarragemRT> rts = new List<BarragemRT>();

					while (reader.Read())
					{
						var obj = caracterizacao.responsaveisTecnicos.FirstOrDefault(x => (int)x.tipo == reader.GetValue<int>("tipo"));
						obj.id = reader.GetValue<int>("id");
						obj.tipo = (eTipoRT)reader.GetValue<int>("tipo");
						obj.nome = reader.GetValue<string>("nome");
						obj.profissao.Id = reader.GetValue<int>("profissao");
						obj.registroCREA = reader.GetValue<string>("registro_crea");
						obj.numeroART = reader.GetValue<string>("numero_art");
						if (obj.tipo == eTipoRT.ElaboracaoProjeto)
							obj.autorizacaoCREA.Id = reader.GetValue<int>("autorizacao_crea");
						obj.proprioDeclarante = reader.GetValue<bool>("proprio_declarante");
					}
				}
				#endregion

				#region Finalidade Atividade
				comando = bancoDeDados.CriarComando(@"
					select  f.atividade from crt_barragem_finalidade_ativ f where f.barragem = :barragem", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);

				caracterizacao.finalidade = bancoDeDados.ExecutarList<int>(comando);
				#endregion

				certidao.Caracterizacao.barragemEntity = caracterizacao;
			}
			#endregion

			return certidao;
		}

		internal List<Lista> ObterFinalidadesTexto(int barragemId, BancoDeDados banco = null)
		{
			List<Lista> finalidades = new List<Lista>();
			Lista finalidade;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select b.ATIVIDADE, b.BARRAGEM, atv.id, atv.texto from crt_barragem_finalidade_ativ b " +
					"inner join lov_crt_bdla_finalidade_atv atv on b.ATIVIDADE = atv.id " +
					"where BARRAGEM = :barragem ", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						finalidade = new Lista();

						finalidade.Tipo = reader.GetValue<int>("id");
						finalidade.Texto = reader.GetValue<string>("texto");

						finalidades.Add(finalidade);
					}

					reader.Close();
				}
			}

			return finalidades;
		}

		internal string ObterVazaoMinimaTipoTexto(int barragemId, BancoDeDados banco = null)
		{
			string vazao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select mt.texto from CRT_BARRAGEM_CONSTRUIDA_CON b " +
					"inner join LOV_CRT_BDLA_MONGE_TIPO mt on b.VAZAO_MIN_TIPO = mt.id " +
					"where b.BARRAGEM = :barragem", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);

				vazao = bancoDeDados.ExecutarScalar<string>(comando);
			}
			return vazao;
		}

		internal string ObterVazaoMaximaTipoTexto(int barragemId, BancoDeDados banco = null)
		{
			string vazao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select mt.texto from CRT_BARRAGEM_CONSTRUIDA_CON b " +
					"inner join LOV_CRT_BDLA_VERTEDOURO_TIPO mt on b.VAZAO_MAX_TIPO = mt.id " +
					"where b.BARRAGEM = :barragem", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);

				vazao = bancoDeDados.ExecutarScalar<string>(comando);
			}
			return vazao;
		}

		internal string ObterTextoProfissao(int barragemId, int tipoRT = 1, BancoDeDados banco= null)
		{
			string TextoProfissao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.texto from CRT_BARRAGEM_DISPENSA_LIC b
					inner join CRT_BARRAGEM_RESPONSAVEL br on b.ID = br.BARRAGEM
					inner join TAB_PROFISSAO p on br.PROFISSAO = p.id
					where b.id = :barragem and br.tipo = :tipoRT");

				comando.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipoRT", tipoRT, DbType.Int32);

				TextoProfissao = bancoDeDados.ExecutarScalar<string>(comando);
			}

			return TextoProfissao;
		}

		#endregion
	}
}