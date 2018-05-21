using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFuncionario.Data;
using Tecnomapas.Utilitarios.Numero;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class FiscalizacaoDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }
		private FuncionarioDa _funcionarioDa { get; set; }
		private ComplementacaoDadosDa _complementacaoDadosDa { get; set; }
		private ConsideracoesFinaisDa _consideracoesFinaisDa { get; set; }
		private InfracaoDa _infracaoDa { get; set; }
		private LocalInfracaoDa _localInfracaoDa { get; set; }
		private MaterialApreendidoDa _materialApreendidoDa { get; set; }
		private ObjetoInfracaoDa _objetoInfracaoDa { get; set; }
        private OutrasPenalidadesDa _outrasPenalidadesDa { get; set; }
        private MultaDa _multaDa { get; set; }
		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		public FiscalizacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}

			_funcionarioDa = new FuncionarioDa();
			_complementacaoDadosDa = new ComplementacaoDadosDa();
			_consideracoesFinaisDa = new ConsideracoesFinaisDa();
			_infracaoDa = new InfracaoDa();
			_localInfracaoDa = new LocalInfracaoDa();
			_materialApreendidoDa = new MaterialApreendidoDa();
			_objetoInfracaoDa = new ObjetoInfracaoDa();
            _multaDa = new MultaDa();
            _outrasPenalidadesDa = new OutrasPenalidadesDa();
		}

		#region Obter

		public AutoInfracaoRelatorio ObterAutoTermoFiscalizacao(int id, BancoDeDados banco = null)
		{
			AutoInfracaoRelatorio fiscalizacao = new AutoInfracaoRelatorio();
			Comando comando = null;
			DateTime dt;
			List<Hashtable> lista = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Cabeçalho

				comando = bancoDeDados.CriarComando(@" select (select 'X' from tab_fisc_infracao tfi where tfi.fiscalizacao = tf.id and tfi.gerado_sistema = '1') is_auto_infracao, (select 'X' from 
													tab_fisc_material_apreendido tfma where tfma.fiscalizacao = tf.id and tfma.tad_gerado = '1') is_termo_apreensao_deposito, (select 'X' from tab_fisc_obj_infracao tfoi where 
													tfoi.fiscalizacao = tf.id and tfoi.tei_gerado_pelo_sist = '1') is_termo_embargo_interdicao, tfli.setor, tf.situacao, tf.autos, 
													(select f.vencimento from tab_fiscalizacao f where f.id = :id)data_termo from tab_fiscalizacao tf, tab_fisc_local_infracao tfli where tf.id = 
													tfli.fiscalizacao and tf.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.Serie = "C";
						fiscalizacao.CodigoUnidadeConvenio = string.Empty;
						fiscalizacao.SetorId = reader.GetValue<int>("setor");
						fiscalizacao.SituacaoId = reader.GetValue<int>("situacao");
						fiscalizacao.NumeroAutoTermo = reader.GetValue<string>("autos");
						fiscalizacao.IsAI = reader.GetValue<string>("is_auto_infracao");
						fiscalizacao.IsTAD = reader.GetValue<string>("is_termo_apreensao_deposito");
						fiscalizacao.IsTEI = reader.GetValue<string>("is_termo_embargo_interdicao");


						if (reader["data_termo"] != null && !Convert.IsDBNull(reader["data_termo"]))
						{
							fiscalizacao.DataVencimento = Convert.ToDateTime(reader["data_termo"]).ToShortDateString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Identificação do autuado

				comando = bancoDeDados.CriarComando(@"
					 select nvl(tp.cpf, tp.cnpj) cpf_cnpj,
							nvl(tp.nome, tp.razao_social) nome_razao_social,
							lpec.texto estado_civil,
							tp.naturalidade,
							tp.rg,
							tpe.logradouro,
							tpe.numero,
							lm.texto municipio,
							le.sigla estado,
							tpe.bairro,
							tpe.distrito,
							tpe.cep,
							tpe.complemento
					   from {0}tab_pessoa              tp,
							{0}lov_pessoa_estado_civil lpec,
							{0}tab_pessoa_endereco     tpe,
							{0}lov_municipio           lm,
							{0}lov_estado              le,
							{0}tab_fisc_local_infracao tfli
					  where tp.estado_civil = lpec.id(+)
						and tp.id = tpe.pessoa(+)
						and tpe.municipio = lm.id(+)
						and tpe.estado = le.id(+)
						and tp.id = nvl(tfli.pessoa, tfli.responsavel)
						and tfli.fiscalizacao = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.AutuadoCPFCNPJ = reader.GetValue<string>("cpf_cnpj");
						fiscalizacao.AutuadoEndBairro = reader.GetValue<string>("bairro");
						fiscalizacao.AutuadoEndCEP = reader.GetValue<string>("cep");
						fiscalizacao.AutuadoEndComplemento = reader.GetValue<string>("complemento");
						fiscalizacao.AutuadoEndDistrito = reader.GetValue<string>("distrito");
						fiscalizacao.AutuadoEndLogradouro = reader.GetValue<string>("logradouro");
						fiscalizacao.AutuadoEndMunicipio = reader.GetValue<string>("municipio");
						fiscalizacao.AutuadoEndNumero = reader.GetValue<string>("numero");
						fiscalizacao.AutuadoEndUF = reader.GetValue<string>("estado");
						fiscalizacao.AutuadoEstadoCivil = reader.GetValue<string>("estado_civil");
						fiscalizacao.AutuadoNaturalidade = reader.GetValue<string>("naturalidade");
						fiscalizacao.AutuadoNomeRazaoSocial = reader.GetValue<string>("nome_razao_social");
						fiscalizacao.AutuadoRG = reader.GetValue<string>("rg");
					}

					reader.Close();
				}

				#endregion

				#region Enquadramento

				comando = bancoDeDados.CriarComando(@"select (case
													 when tfi.gerado_sistema = 0 and tfo.gerado_sistema <> 1 and tfm.gerado_sistema <> 1 then
													  tfi.data_lav
													 when tfo.gerado_sistema = 0 and tfi.gerado_sistema <> 1 and tfm.gerado_sistema <> 1 then
													  tfo.data_lav
													 when tfm.gerado_sistema = 0 and tfi.gerado_sistema <> 1 and tfo.gerado_sistema <> 1 then
													  tfm.data_lav
													 else
													  (f.situacao_data)
												   end) data,
												   tfli.lat_northing,
												   tfli.lon_easting,
												   tfli.local
											  from {0}tab_fisc_local_infracao tfli,
												   {0}tab_fiscalizacao f,
												   (select tfi.gerado_sistema,
														   tfi.data_lavratura_auto data_lav,
														   tfi.fiscalizacao
													  from {0}tab_fisc_infracao tfi
													 where tfi.fiscalizacao = :id) tfi,
												   (select tfo.tei_gerado_pelo_sist gerado_sistema,
														   tfo.data_lavratura_termo data_lav,
														   tfo.fiscalizacao
													  from {0}tab_fisc_obj_infracao tfo
													 where tfo.fiscalizacao = :id) tfo,
												   (select tfm.tad_gerado   gerado_sistema,
														   tfm.tad_data     data_lav,
														   tfm.fiscalizacao
													  from {0}tab_fisc_material_apreendido tfm
													 where tfm.fiscalizacao = :id) tfm
											 where tfli.fiscalizacao = tfi.fiscalizacao(+)
											   and tfli.fiscalizacao = tfo.fiscalizacao(+)
											   and tfli.fiscalizacao = tfm.fiscalizacao(+)
											   and f.id = tfli.fiscalizacao
											   and f.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.CoordenadaNorthing = reader.GetValue<string>("lat_northing");
						fiscalizacao.CoordenadaEasting = reader.GetValue<string>("lon_easting");
						fiscalizacao.Local = reader.GetValue<string>("local");

						dt = reader.GetValue<DateTime>("data");

						if (dt.ToShortDateString() != "01/01/0001")
						{
							fiscalizacao.DiaAutuacao = dt.Day.ToString();
							fiscalizacao.MesAutuacao = _configSys.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(dt.Month - 1);
							fiscalizacao.AnoAtuacao = dt.Year.ToString();
						}

					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@" select tfea.artigo, tfea.artigo_paragrafo, tfea.combinado_artigo, tfea.combinado_artigo_paragrafo, tfea.da_do norma_legal from tab_fisc_enquadramento
					tfe, tab_fisc_enquadr_artig tfea where tfe.id = tfea.enquadramento_id  and tfe.fiscalizacao = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				lista = bancoDeDados.ExecutarHashtable(comando);

				if (lista != null && lista.Count > 0)
				{
					if (lista[0] != null)
					{
						fiscalizacao.EnquadramentoArtigo1 = lista[0]["ARTIGO"].ToString();
						fiscalizacao.EnquadramentoArtigoItemParagrafo1 = lista[0]["ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigo1 = lista[0]["COMBINADO_ARTIGO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigoItemParagrafo1 = lista[0]["COMBINADO_ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCitarNormaLegal1 = lista[0]["NORMA_LEGAL"].ToString();
					}

					if (lista[1] != null)
					{
						fiscalizacao.EnquadramentoArtigo2 = lista[1]["ARTIGO"].ToString();
						fiscalizacao.EnquadramentoArtigoItemParagrafo2 = lista[1]["ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigo2 = lista[1]["COMBINADO_ARTIGO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigoItemParagrafo2 = lista[1]["COMBINADO_ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCitarNormaLegal2 = lista[1]["NORMA_LEGAL"].ToString();
					}

					if (lista[2] != null)
					{
						fiscalizacao.EnquadramentoArtigo3 = lista[2]["ARTIGO"].ToString();
						fiscalizacao.EnquadramentoArtigoItemParagrafo3 = lista[2]["ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigo3 = lista[2]["COMBINADO_ARTIGO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigoItemParagrafo3 = lista[2]["COMBINADO_ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCitarNormaLegal3 = lista[2]["NORMA_LEGAL"].ToString();
					}
				}
				#endregion

				#region Descrição da infração

				comando = bancoDeDados.CriarComando(" select tfi.descricao_infracao from tab_fisc_infracao tfi where tfi.fiscalizacao = :id ");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				fiscalizacao.DescricaoInfracao = bancoDeDados.ExecutarScalar<string>(comando);

				#endregion

				#region Valor

				comando = bancoDeDados.CriarComando(@" select tfi.valor_multa, lficr.texto codigo_receita from tab_fisc_infracao tfi, lov_fisc_infracao_codigo_rece lficr where tfi.codigo_receita = 
					lficr.id and tfi.fiscalizacao = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.ValorMulta = reader.GetValue<decimal>("valor_multa").ToString("N2");
						fiscalizacao.CodigoReceita = reader.GetValue<string>("codigo_receita");
						fiscalizacao.ValorMultaPorExtenso = Escrita.PorExtenso(Convert.ToDecimal(fiscalizacao.ValorMulta), ModoEscrita.Monetario);
					}

					reader.Close();
				}

				#endregion

				#region Descrição da apreensão

				comando = bancoDeDados.CriarComando(@"
					 select tfma.descricao,
							tfma.valor_produtos,
							tfma.endereco_logradouro,
							tfma.endereco_bairro,
							tfma.endereco_distrito,
							led.sigla endereco_estado,
							lmd.texto endereco_municipio,
							nvl(tp.nome, tp.razao_social) nome,
							lpec.texto estado_civil,
							tp.naturalidade,
							tp.rg,
							tpe.logradouro,
							tpe.bairro,
							tpe.distrito,
							lm.texto municipio
					   from {0}tab_fisc_material_apreendido tfma,
							{0}tab_pessoa                   tp,
							{0}tab_pessoa_endereco          tpe,
							{0}lov_pessoa_estado_civil      lpec,
							{0}lov_municipio                lmd,
							{0}lov_municipio                lm,
							{0}lov_estado                   led
					  where tfma.depositario = tp.id
						and tp.id = tpe.pessoa
						and tp.estado_civil = lpec.id(+)
						and tfma.endereco_municipio = lmd.id(+)
						and tfma.endereco_estado = led.id(+)
						and tpe.municipio = lm.id(+)
						and tfma.fiscalizacao = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.DescreverApreensao = reader.GetValue<string>("descricao");
						fiscalizacao.ValorBemProdutoArbitrado = reader.GetValue<decimal>("valor_produtos").ToString("N2");
						fiscalizacao.DepositarioLogradouro = reader.GetValue<string>("endereco_logradouro");
						fiscalizacao.DepositarioBairro = reader.GetValue<string>("endereco_bairro");
						fiscalizacao.DepositarioDistrito = reader.GetValue<string>("endereco_distrito");
						fiscalizacao.DepositarioUF = reader.GetValue<string>("endereco_estado");
						fiscalizacao.DepositarioMunicipio = reader.GetValue<string>("endereco_municipio");
						fiscalizacao.DepositarioNome = reader.GetValue<string>("nome");
						fiscalizacao.DepositarioEstadoCivil = reader.GetValue<string>("estado_civil");
						fiscalizacao.DepositarioNaturalidade = reader.GetValue<string>("naturalidade");
						fiscalizacao.DepositarioRG = reader.GetValue<string>("rg");
						fiscalizacao.DepositarioEndLogradouro = reader.GetValue<string>("logradouro");
						fiscalizacao.DepositarioEndBairro = reader.GetValue<string>("bairro");
						fiscalizacao.DepositarioEndDistrito = reader.GetValue<string>("distrito");
						fiscalizacao.DepositarioEndMunicipio = reader.GetValue<string>("municipio");

						fiscalizacao.ValorBemPorExtenso = Escrita.PorExtenso(Convert.ToDecimal(fiscalizacao.ValorBemProdutoArbitrado), ModoEscrita.Monetario);
					}

					reader.Close();
				}

				#endregion

				#region Descrição do embargo/interdição

				comando = bancoDeDados.CriarComando(" select tfoi.desc_termo_embargo from tab_fisc_obj_infracao tfoi where tfoi.fiscalizacao = :id ");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				fiscalizacao.DescricaoTermoEmbargo = bancoDeDados.ExecutarScalar<string>(comando);

				#endregion

				#region Firmas

				comando = bancoDeDados.CriarComando(@"
					select nvl(tp.nome, tp.razao_social) nome,
						   nvl(tp.cpf, tp.cnpj) cpf,
						   tu.nome autuante
					  from tab_fisc_local_infracao tfli,
						   tab_pessoa              tp,
						   tab_fiscalizacao        tf,
						   tab_funcionario         tu
					 where tp.id = nvl(tfli.responsavel, tfli.pessoa)
					   and tf.autuante = tu.id
					   and tfli.fiscalizacao = tf.id
					   and tf.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.ResponsavelEmpNomeRazaoSocial = reader.GetValue<string>("nome");
						fiscalizacao.ResponsavelEmpCPFCNPJ = reader.GetValue<string>("cpf");
						fiscalizacao.NomeUsuarioCadastro = reader.GetValue<string>("autuante");
					}

					reader.Close();
				}

				#endregion

				#region Testemunhas

				comando = bancoDeDados.CriarComando(@"
					select (case
							 when tfcft.testemunha_setor is null then
							  tfcft.endereco
							 else
							  te.endereco
						   end) endereco,
						   nvl(tfcft.nome, tf.nome) nome
					  from {0}tab_fisc_consid_final tfcf,
						   {0}tab_fisc_consid_final_test tfcft,
						   {0}tab_funcionario tf,
						   (select t.logradouro || ', ' || t.numero ||
								   nvl2(t.complemento,
										' - (Complemento: ' || t.complemento || ')',
										'') || ' - Bairro: ' || t.bairro || ' - Cep: ' || t.cep ||
								   ' - ' || lm.texto || '/' || le.sigla endereco,
								   t.setor
							  from {0}tab_setor_endereco t, {0}lov_estado le, {0}lov_municipio lm
							 where t.municipio = lm.id(+)
							   and lm.estado = le.id(+)) te
					 where tfcf.id = tfcft.consid_final
					   and tfcft.testemunha = tf.id(+)
					   and tfcft.testemunha_setor = te.setor(+)
					   and tfcf.fiscalizacao = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				lista = bancoDeDados.ExecutarHashtable(comando);

				if (lista != null && lista.Count > 0)
				{
					if (lista[0] != null)
					{
						fiscalizacao.TestemunhaNome1 = lista[0]["NOME"].ToString();
						fiscalizacao.TestemunhaEnd1 = lista[0]["ENDERECO"].ToString();
					}

					if (lista[1] != null)
					{
						fiscalizacao.TestemunhaNome2 = lista[1]["NOME"].ToString();
						fiscalizacao.TestemunhaEnd2 = lista[1]["ENDERECO"].ToString();
					}
				}

				#endregion
			}

			return fiscalizacao;
		}

		public AutoInfracaoRelatorio ObterAutoTermoFiscalizacaoHistorico(int historicoId, BancoDeDados banco = null)
		{
			AutoInfracaoRelatorio fiscalizacao = new AutoInfracaoRelatorio();
			Comando comando = null;
			DateTime dt;
			List<Hashtable> lista = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Cabeçalho

				comando = bancoDeDados.CriarComando(@"select 
					(select 'X' from hst_fisc_infracao tfi where tfi.fiscalizacao_id_hst = tf.id and tfi.gerado_sistema = '1') is_auto_infracao, 
					(select 'X' from hst_fisc_material_apreendido tfma where tfma.fiscalizacao_id_hst = tf.id and tfma.tad_gerado = '1') is_termo_apreensao_deposito, 
					(select 'X' from hst_fisc_obj_infracao tfoi where tfoi.fiscalizacao_id_hst = tf.id and tfoi.tei_gerado_pelo_sist = '1') is_termo_embargo_interdicao, 
					tfli.setor_id setor, tf.situacao_id situacao, tf.autos, 
					tf.vencimento data_termo 
					from 
						{0}hst_fiscalizacao tf, 
						{0}hst_fisc_local_infracao tfli 
					where tf.id = tfli.id_hst
					  and tf.id = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.Serie = "C";
						fiscalizacao.CodigoUnidadeConvenio = string.Empty;
						fiscalizacao.SetorId = reader.GetValue<int>("setor");
						fiscalizacao.SituacaoId = reader.GetValue<int>("situacao");
						fiscalizacao.NumeroAutoTermo = reader.GetValue<string>("autos");
						fiscalizacao.IsAI = reader.GetValue<string>("is_auto_infracao");
						fiscalizacao.IsTAD = reader.GetValue<string>("is_termo_apreensao_deposito");
						fiscalizacao.IsTEI = reader.GetValue<string>("is_termo_embargo_interdicao");


						if (reader["data_termo"] != null && !Convert.IsDBNull(reader["data_termo"]))
						{
							fiscalizacao.DataVencimento = Convert.ToDateTime(reader["data_termo"]).ToShortDateString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Identificação do autuado

				comando = bancoDeDados.CriarComando(@"
							 select nvl(tp.cpf, tp.cnpj) cpf_cnpj,
									nvl(tp.nome, tp.razao_social) nome_razao_social,
									tp.estado_civil_texto estado_civil,
									tp.naturalidade,
									tp.rg,
									tpe.logradouro,
									tpe.numero,
									tpe.municipio_texto municipio,
									le.sigla estado,
									tpe.bairro,
									tpe.distrito,
									tpe.cep,
									tpe.complemento
							   from {0}hst_fisc_local_infracao tfli,
									{0}hst_pessoa              tp,
									{0}hst_pessoa_endereco     tpe,              
									{0}lov_estado              le
							  where tpe.estado_id = le.id(+)            
					and tp.id = tpe.id_hst(+)
					and tp.pessoa_id = nvl(tfli.pessoa_id, tfli.resp_propriedade_id)
					and tp.tid = nvl(tfli.pessoa_tid, tfli.resp_propriedade_tid)
					and tfli.id_hst = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.AutuadoCPFCNPJ = reader.GetValue<string>("cpf_cnpj");
						fiscalizacao.AutuadoEndBairro = reader.GetValue<string>("bairro");
						fiscalizacao.AutuadoEndCEP = reader.GetValue<string>("cep");
						fiscalizacao.AutuadoEndComplemento = reader.GetValue<string>("complemento");
						fiscalizacao.AutuadoEndDistrito = reader.GetValue<string>("distrito");
						fiscalizacao.AutuadoEndLogradouro = reader.GetValue<string>("logradouro");
						fiscalizacao.AutuadoEndMunicipio = reader.GetValue<string>("municipio");
						fiscalizacao.AutuadoEndNumero = reader.GetValue<string>("numero");
						fiscalizacao.AutuadoEndUF = reader.GetValue<string>("estado");
						fiscalizacao.AutuadoEstadoCivil = reader.GetValue<string>("estado_civil");
						fiscalizacao.AutuadoNaturalidade = reader.GetValue<string>("naturalidade");
						fiscalizacao.AutuadoNomeRazaoSocial = reader.GetValue<string>("nome_razao_social");
						fiscalizacao.AutuadoRG = reader.GetValue<string>("rg");
					}

					reader.Close();
				}

				#endregion

				#region Enquadramento

				comando = bancoDeDados.CriarComando(@"select (case
													   when tfi.gerado_sistema = 0 and tfo.tei_gerado_pelo_sist <> 1 and tfm.tad_gerado <> 1 then
													   tfi.data_lavratura_auto
													   when tfo.tei_gerado_pelo_sist = 0 and tfi.gerado_sistema <> 1 and tfm.tad_gerado <> 1 then
													   tfo.data_lavratura_termo
													   when tfm.tad_gerado = 0 and tfi.gerado_sistema <> 1 and tfo.tei_gerado_pelo_sist <> 1 then
													   tfm.tad_data
													   else
													   (f.situacao_data)
													   end) data,
													   tfli.lat_northing,
													   tfli.lon_easting,
													   tfli.local                              
													from 
													   {0}hst_fiscalizacao f,
													   {0}hst_fisc_local_infracao tfli,
													   {0}hst_fisc_infracao tfi, 
													   {0}hst_fisc_obj_infracao tfo, 
													   {0}hst_fisc_material_apreendido tfm                           
													where f.id = tfi.fiscalizacao_id_hst(+)
													   and f.id = tfo.fiscalizacao_id_hst(+)
													   and f.id = tfm.fiscalizacao_id_hst(+)
													   and f.id = tfli.id_hst
													   and f.id = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.CoordenadaNorthing = reader.GetValue<string>("lat_northing");
						fiscalizacao.CoordenadaEasting = reader.GetValue<string>("lon_easting");
						fiscalizacao.Local = reader.GetValue<string>("local");

						dt = reader.GetValue<DateTime>("data");

						if (dt.ToShortDateString() != "01/01/0001")
						{
							fiscalizacao.DiaAutuacao = dt.Day.ToString();
							fiscalizacao.MesAutuacao = _configSys.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(dt.Month - 1);
							fiscalizacao.AnoAtuacao = dt.Year.ToString();
						}

					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@" select tfea.artigo, tfea.artigo_paragrafo, tfea.combinado_artigo, tfea.combinado_artigo_paragrafo, tfea.da_do norma_legal 
					 from {0}hst_fisc_enquadramento tfe, 
						  {0}hst_fisc_enquadr_artig tfea 
					where tfe.id = tfea.id_hst
					  and tfe.fiscalizacao_id_hst = :historicoId
					order by tfea.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				lista = bancoDeDados.ExecutarHashtable(comando);

				if (lista != null && lista.Count > 0)
				{
					if (lista[0] != null)
					{
						fiscalizacao.EnquadramentoArtigo1 = lista[0]["ARTIGO"].ToString();
						fiscalizacao.EnquadramentoArtigoItemParagrafo1 = lista[0]["ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigo1 = lista[0]["COMBINADO_ARTIGO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigoItemParagrafo1 = lista[0]["COMBINADO_ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCitarNormaLegal1 = lista[0]["NORMA_LEGAL"].ToString();
					}

					if (lista[1] != null)
					{
						fiscalizacao.EnquadramentoArtigo2 = lista[1]["ARTIGO"].ToString();
						fiscalizacao.EnquadramentoArtigoItemParagrafo2 = lista[1]["ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigo2 = lista[1]["COMBINADO_ARTIGO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigoItemParagrafo2 = lista[1]["COMBINADO_ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCitarNormaLegal2 = lista[1]["NORMA_LEGAL"].ToString();
					}

					if (lista[2] != null)
					{
						fiscalizacao.EnquadramentoArtigo3 = lista[2]["ARTIGO"].ToString();
						fiscalizacao.EnquadramentoArtigoItemParagrafo3 = lista[2]["ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigo3 = lista[2]["COMBINADO_ARTIGO"].ToString();
						fiscalizacao.EnquadramentoCombinadoArtigoItemParagrafo3 = lista[2]["COMBINADO_ARTIGO_PARAGRAFO"].ToString();
						fiscalizacao.EnquadramentoCitarNormaLegal3 = lista[2]["NORMA_LEGAL"].ToString();
					}
				}
				#endregion

				#region Descrição da infração

				comando = bancoDeDados.CriarComando(" select tfi.descricao_infracao from hst_fisc_infracao tfi where tfi.fiscalizacao_id_hst = :historicoId");

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				fiscalizacao.DescricaoInfracao = bancoDeDados.ExecutarScalar<string>(comando);

				#endregion

				#region Valor

				comando = bancoDeDados.CriarComando(@" select tfi.valor_multa, tfi.codigo_receita_texto codigo_receita 
					from {0}hst_fisc_infracao tfi 
					where tfi.fiscalizacao_id_hst = :historicoId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.ValorMulta = reader.GetValue<decimal>("valor_multa").ToString("N2");
						fiscalizacao.CodigoReceita = reader.GetValue<string>("codigo_receita");
						fiscalizacao.ValorMultaPorExtenso = Escrita.PorExtenso(Convert.ToDecimal(fiscalizacao.ValorMulta), ModoEscrita.Monetario);
					}

					reader.Close();
				}

				#endregion

				#region Descrição da apreensão

				comando = bancoDeDados.CriarComando(@"
					select tfma.descricao,
						tfma.valor_produtos,
						tfma.endereco_logradouro,
						tfma.endereco_bairro,
						tfma.endereco_distrito,
						led.sigla endereco_estado,
						tfma.endereco_municipio_texto endereco_municipio,
						nvl(tp.nome, tp.razao_social) nome,
						tp.estado_civil_texto estado_civil,
						tp.naturalidade,
						tp.rg,
						tpe.logradouro,
						tpe.bairro,
						tpe.distrito,
						tpe.municipio_texto municipio
					from 
						{0}hst_fisc_material_apreendido tfma,
						{0}hst_pessoa                   tp,
						{0}hst_pessoa_endereco          tpe,
						{0}lov_estado                   led
					where 
						tfma.endereco_estado_id = led.id(+)
						and tfma.depositario_id = tp.pessoa_id
						and tfma.depositario_tid = tp.tid
						and tp.id = tpe.id_hst
						and tfma.fiscalizacao_id_hst = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada(":historicoId", historicoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.DescreverApreensao = reader.GetValue<string>("descricao");
						fiscalizacao.ValorBemProdutoArbitrado = reader.GetValue<decimal>("valor_produtos").ToString("N2");
						fiscalizacao.DepositarioLogradouro = reader.GetValue<string>("endereco_logradouro");
						fiscalizacao.DepositarioBairro = reader.GetValue<string>("endereco_bairro");
						fiscalizacao.DepositarioDistrito = reader.GetValue<string>("endereco_distrito");
						fiscalizacao.DepositarioUF = reader.GetValue<string>("endereco_estado");
						fiscalizacao.DepositarioMunicipio = reader.GetValue<string>("endereco_municipio");
						fiscalizacao.DepositarioNome = reader.GetValue<string>("nome");
						fiscalizacao.DepositarioEstadoCivil = reader.GetValue<string>("estado_civil");
						fiscalizacao.DepositarioNaturalidade = reader.GetValue<string>("naturalidade");
						fiscalizacao.DepositarioRG = reader.GetValue<string>("rg");
						fiscalizacao.DepositarioEndLogradouro = reader.GetValue<string>("logradouro");
						fiscalizacao.DepositarioEndBairro = reader.GetValue<string>("bairro");
						fiscalizacao.DepositarioEndDistrito = reader.GetValue<string>("distrito");
						fiscalizacao.DepositarioEndMunicipio = reader.GetValue<string>("municipio");

						fiscalizacao.ValorBemPorExtenso = Escrita.PorExtenso(Convert.ToDecimal(fiscalizacao.ValorBemProdutoArbitrado), ModoEscrita.Monetario);
					}

					reader.Close();
				}

				#endregion

				#region Descrição do embargo/interdição

				comando = bancoDeDados.CriarComando(" select tfoi.desc_termo_embargo from hst_fisc_obj_infracao tfoi where tfoi.fiscalizacao_id_hst = :historicoId ");

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				fiscalizacao.DescricaoTermoEmbargo = bancoDeDados.ExecutarScalar<string>(comando);

				#endregion

				#region Firmas

				comando = bancoDeDados.CriarComando(@"
							select distinct nvl(tp.nome, tp.razao_social) nome,
					   nvl(tp.cpf, tp.cnpj) cpf,
					   tu.nome autuante, tu.acao_executada
					from {0}hst_fisc_local_infracao tfli,
					   {0}hst_pessoa              tp,
					   {0}hst_fiscalizacao        tf,
					   {0}hst_funcionario         tu
				   where tp.pessoa_id = nvl(tfli.resp_propriedade_id, tfli.pessoa_id)
					 and tp.tid = nvl(tfli.resp_propriedade_tid, tfli.pessoa_tid)					 
					 and tf.autuante_id = tu.funcionario_id
					 and tf.autuante_tid = tu.tid					 
					 and tfli.id_hst = tf.id
					 and tf.id = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						fiscalizacao.ResponsavelEmpNomeRazaoSocial = reader.GetValue<string>("nome");
						fiscalizacao.ResponsavelEmpCPFCNPJ = reader.GetValue<string>("cpf");
						fiscalizacao.NomeUsuarioCadastro = reader.GetValue<string>("autuante");
					}

					reader.Close();
				}

				#endregion

				#region Testemunhas

				comando = bancoDeDados.CriarComando(@"
					select tfcft.testemunha_id, (case
						when tfcft.testemunha_setor_id is null then
						tfcft.endereco
						else
						te.endereco
						end) endereco,
						nvl(tfcft.nome, tf.nome) nome
						from hst_fisc_consid_final tfcf,
						hst_fisc_consid_final_test tfcft,
						hst_funcionario tf,
						hst_fiscalizacao hf,
						(select t.tid, t.logradouro || ', ' || t.numero ||
							nvl2(t.complemento,
							' - (Complemento: ' || t.complemento || ')',
							'') || ' - Bairro: ' || t.bairro || ' - Cep: ' || t.cep ||
							' - ' || lm.texto || '/' || le.sigla endereco,
							t.endereco_id setor
						from hst_setor_endereco t, lov_estado le, lov_municipio lm
						where t.municipio_id = lm.id(+)
							and lm.estado = le.id(+)) te
						where tfcf.id = tfcft.id_hst
						and tfcft.testemunha_setor_id = te.setor(+)
						and tfcft.testemunha_setor_tid = te.tid(+)
						and tfcf.id_hst = hf.id
						and tf.id = (select max(r.id) from hst_funcionario r 
										where r.funcionario_id = tfcft.testemunha_id 
										and r.tid = tfcft.testemunha_tid
										and r.acao_executada <= hf.acao_executada )
						and tfcf.id_hst = :historicoId 
				union all           
					select tfcft.testemunha_id, tfcft.endereco endereco,
					tfcft.nome nome
					from hst_fisc_consid_final tfcf,
					hst_fisc_consid_final_test tfcft
					where tfcf.id = tfcft.id_hst
					and tfcft.testemunha_id is null
					and tfcf.id_hst = :historicoId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				lista = bancoDeDados.ExecutarHashtable(comando);

				if (lista != null && lista.Count > 0)
				{
					lista = lista.OrderBy(x => ( (!x.ContainsKey("NOME")) || x["NOME"] == null || x["NOME"] == DBNull.Value ) ? String.Empty : x["NOME"]).ToList();

					if (lista[0] != null)
					{
						fiscalizacao.TestemunhaNome1 = lista[0]["NOME"].ToString();
						fiscalizacao.TestemunhaEnd1 = lista[0]["ENDERECO"].ToString();
					}

					if (lista[1] != null)
					{
						fiscalizacao.TestemunhaNome2 = lista[1]["NOME"].ToString();
						fiscalizacao.TestemunhaEnd2 = lista[1]["ENDERECO"].ToString();
					}
				}

				#endregion
			}

			return fiscalizacao;
		}

        public InstrumentoUnicoFiscalizacaoRelatorio ObterInstrumentoUnicoFiscalizacao(int id, BancoDeDados banco = null)
        {
            InstrumentoUnicoFiscalizacaoRelatorio fiscalizacao = new InstrumentoUnicoFiscalizacaoRelatorio();
            Comando comando = null;
            List<Hashtable> lista = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                #region Cabeçalho

                comando = bancoDeDados.CriarComando(@"
                            select ( select 'X'
                                      from tab_fisc_local_infracao tfi
                                      where tfi.fiscalizacao = tf.id
                                            and tfi.area_fiscalizacao = 0 ) is_ddsia,
                                    ( select 'X'
                                      from tab_fisc_local_infracao tfi
                                      where tfi.fiscalizacao = tf.id
                                            and tfi.area_fiscalizacao = 1 ) is_ddsiv,
                                    ( select 'X'
                                      from tab_fisc_local_infracao tfi
                                      where tfi.fiscalizacao = tf.id
                                            and tfi.area_fiscalizacao = 2 ) is_drnre,
                                    tfli.setor,
                                    tf.situacao,
                                    tf.autos,
                                    ( select f.situacao_data
                                      from tab_fiscalizacao f
                                      where f.id = :id
                                            and f.situacao != 1 ) data_termo,
                                    ( select distinct texto
                                      from lov_fiscalizacao_serie lfs
                                      where lfs.id = tfa.serie
                                            or lfs.id = tfm.serie
                                            or lfs.id = tfop.serie
                                            or lfs.id = tfoi.serie ) serie,
                                    nvl(tfa.iuf_numero, nvl(tfm.iuf_numero, nvl(tfop.iuf_numero, tfoi.iuf_numero))) numero_iuf
                            from tab_fiscalizacao tf,
                                 tab_fisc_local_infracao tfli,
                                 (select * from tab_fisc_apreensao a where a.iuf_digital = 1) tfa,
                                 (select * from tab_fisc_multa m where m.iuf_digital = 1) tfm,
                                 (select * from tab_fisc_outras_penalidades op where op.iuf_digital = 1) tfop,
                                 (select * from tab_fisc_obj_infracao oi where oi.iuf_digital = 1) tfoi
                            where tf.id = tfli.fiscalizacao
                                  and tfa.fiscalizacao (+)= tf.id
                                  and tfm.fiscalizacao (+)= tf.id
                                  and tfop.fiscalizacao (+)= tf.id
                                  and tfoi.fiscalizacao (+)= tf.id
                                  and tf.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.Serie = reader.GetValue<string>("serie");
                        fiscalizacao.CodigoUnidadeConvenio = string.Empty;
                        fiscalizacao.SetorId = reader.GetValue<int>("setor");
                        fiscalizacao.SituacaoId = reader.GetValue<int>("situacao");
                        //fiscalizacao.NumeroAutoTermo = reader.GetValue<string>("autos");

                        if (fiscalizacao.SituacaoId != (int)eFiscalizacaoSituacaoRelatorio.EmAndamento)
                        {
                            fiscalizacao.NumeroIUF = reader.GetValue<string>("numero_iuf");

                            if (reader["data_termo"] != null && !Convert.IsDBNull(reader["data_termo"]))
                            {
                                fiscalizacao.DataVencimento = Convert.ToDateTime(reader["data_termo"]).ToShortDateString();
                            }
                        }

                        fiscalizacao.IsDDSIA = reader.GetValue<string>("is_ddsia");
                        fiscalizacao.IsDDSIV = reader.GetValue<string>("is_ddsiv");
                        fiscalizacao.IsDRNRE = reader.GetValue<string>("is_drnre");

                        if (!String.IsNullOrWhiteSpace(fiscalizacao.NumeroIUF))
                        {
                            fiscalizacao.NumeroIUF = String.Format("{0:000000}", Convert.ToInt64(fiscalizacao.NumeroIUF));
                        }
                    }

                    reader.Close();
                }

                #endregion

                #region Identificação do autuado

                comando = bancoDeDados.CriarComando(@"
					 select nvl(tp.cpf, tp.cnpj) cpf_cnpj,
							nvl(tp.nome, tp.razao_social) nome_razao_social,
							lpec.texto estado_civil,
							tp.naturalidade,
							tp.rg,
							tpe.logradouro,
							tpe.numero,
							lm.texto municipio,
							le.sigla estado,
							tpe.bairro,
							tpe.distrito,
							tpe.cep,
							tpe.complemento
					   from {0}tab_pessoa              tp,
							{0}lov_pessoa_estado_civil lpec,
							{0}tab_pessoa_endereco     tpe,
							{0}lov_municipio           lm,
							{0}lov_estado              le,
							{0}tab_fisc_local_infracao tfli
					  where tp.estado_civil = lpec.id(+)
						and tp.id = tpe.pessoa(+)
						and tpe.municipio = lm.id(+)
						and tpe.estado = le.id(+)
						and tp.id = nvl(tfli.pessoa, tfli.responsavel)
						and tfli.fiscalizacao = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.AutuadoCPFCNPJ = reader.GetValue<string>("cpf_cnpj");
                        fiscalizacao.AutuadoEndBairro = reader.GetValue<string>("bairro");
                        fiscalizacao.AutuadoEndCEP = reader.GetValue<string>("cep");
                        fiscalizacao.AutuadoEndComplemento = reader.GetValue<string>("complemento");
                        fiscalizacao.AutuadoEndDistrito = reader.GetValue<string>("distrito");
                        fiscalizacao.AutuadoEndLogradouro = reader.GetValue<string>("logradouro");
                        fiscalizacao.AutuadoEndMunicipio = reader.GetValue<string>("municipio");
                        fiscalizacao.AutuadoEndNumero = reader.GetValue<string>("numero");
                        fiscalizacao.AutuadoEndUF = reader.GetValue<string>("estado");
                        fiscalizacao.AutuadoEstadoCivil = reader.GetValue<string>("estado_civil");
                        fiscalizacao.AutuadoNaturalidade = reader.GetValue<string>("naturalidade");
                        fiscalizacao.AutuadoNomeRazaoSocial = reader.GetValue<string>("nome_razao_social");
                        fiscalizacao.AutuadoRG = reader.GetValue<string>("rg");
                    }

                    reader.Close();
                }

                #endregion

                #region Enquadramento

                comando = bancoDeDados.CriarComando(@"
                                select tfli.lat_northing,
                                       tfli.lon_easting,
                                       nvl(tfli.local, tee.logradouro||', '||tee.numero||', '||tee.bairro) local,
                                       ( select lm.texto
                                         from lov_municipio lm
                                         where lm.id = nvl(tfli.municipio, tee.municipio) ) municipio
                                from {0}tab_fisc_local_infracao tfli,
                                     {0}tab_fiscalizacao f,
                                     ( select t.* from {0}tab_empreendimento_endereco t where t.correspondencia = 0 ) tee
                                where f.id = tfli.fiscalizacao
                                      and tee.empreendimento (+)= tfli.empreendimento
                                      and f.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.CoordenadaNorthing = reader.GetValue<string>("lat_northing");
                        fiscalizacao.CoordenadaEasting = reader.GetValue<string>("lon_easting");
                        fiscalizacao.Local = reader.GetValue<string>("local");
                        fiscalizacao.Municipio = reader.GetValue<string>("municipio");
                    }

                    reader.Close();
                }

                //se a área for DDSIA, converte as coordenadas para Grau, Minuto e Segundo
                if (!String.IsNullOrWhiteSpace(fiscalizacao.IsDDSIA))
                {
                    String EsquemaBancoGeo = "idafgeo";

                    comando = bancoDeDados.CriarComandoPlSql(@"
                                begin
                                    {0}coordenada.utm2gms(:datum, :easting, :northing, :fuso, 1, :longitude, :latitude);
                                end;", EsquemaBancoGeo);
                    comando.AdicionarParametroEntrada("datum", "SIRGAS2000", DbType.String);
                    comando.AdicionarParametroEntrada("easting", fiscalizacao.CoordenadaEasting, DbType.Int64);
                    comando.AdicionarParametroEntrada("northing", fiscalizacao.CoordenadaNorthing, DbType.Int64);
                    comando.AdicionarParametroEntrada("fuso", 24, DbType.Int32);
                    comando.AdicionarParametroSaida("longitude", DbType.String, 100);
                    comando.AdicionarParametroSaida("latitude", DbType.String, 100);

                    bancoDeDados.ExecutarNonQuery(comando);

                    fiscalizacao.CoordenadaEasting = comando.ObterValorParametro<string>("longitude");
                    fiscalizacao.CoordenadaNorthing = comando.ObterValorParametro<string>("latitude");
                }

                comando = bancoDeDados.CriarComando(@"
                            select tfea.artigo,
                                   tfea.artigo_paragrafo,
                                   tfea.da_do norma_legal
                            from {0}tab_fisc_enquadramento tfe,
                                 {0}tab_fisc_enquadr_artig tfea
                            where tfe.id = tfea.enquadramento_id
                                  and tfe.fiscalizacao = :id
                            order by tfea.id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                lista = bancoDeDados.ExecutarHashtable(comando);

                if (lista != null && lista.Count > 0)
                {
                    if (lista[0] != null)
                    {
                        fiscalizacao.EnquadramentoArtigo1 = lista[0]["ARTIGO"].ToString();
                        fiscalizacao.EnquadramentoArtigoItemParagrafo1 = lista[0]["ARTIGO_PARAGRAFO"].ToString();
                        fiscalizacao.EnquadramentoCitarNormaLegal1 = lista[0]["NORMA_LEGAL"].ToString();
                    }

                    if (lista.Count > 1 && lista[1] != null)
                    {
                        fiscalizacao.EnquadramentoArtigo2 = lista[1]["ARTIGO"].ToString();
                        fiscalizacao.EnquadramentoArtigoItemParagrafo2 = lista[1]["ARTIGO_PARAGRAFO"].ToString();
                        fiscalizacao.EnquadramentoCitarNormaLegal2 = lista[1]["NORMA_LEGAL"].ToString();
                    }

                    if (lista.Count > 2 && lista[2] != null)
                    {
                        fiscalizacao.EnquadramentoArtigo3 = lista[2]["ARTIGO"].ToString();
                        fiscalizacao.EnquadramentoArtigoItemParagrafo3 = lista[2]["ARTIGO_PARAGRAFO"].ToString();
                        fiscalizacao.EnquadramentoCitarNormaLegal3 = lista[2]["NORMA_LEGAL"].ToString();
                    }
                }
                #endregion

                #region Descrição da infração

                comando = bancoDeDados.CriarComando(@"
                                select tfi.descricao_infracao,
                                       ( select 'X'
                                         from {0}tab_fisc_infracao inf
                                         where inf.fiscalizacao = :id
                                               and tfi.classificacao_infracao = 0 ) is_leve,
                                       ( select 'X'
                                         from {0}tab_fisc_infracao inf
                                         where inf.fiscalizacao = :id
                                               and tfi.classificacao_infracao = 1 ) is_media,
                                       ( select 'X'
                                         from {0}tab_fisc_infracao inf
                                         where inf.fiscalizacao = :id
                                               and tfi.classificacao_infracao = 2 ) is_grave,
                                       ( select 'X'
                                         from {0}tab_fisc_infracao inf
                                         where inf.fiscalizacao = :id
                                               and tfi.classificacao_infracao = 3 ) is_gravissima,
                                       to_char(tfi.data_constatacao, 'DD/MM/YYYY') data_constatacao,
                                       tfi.hora_constatacao
                                from {0}tab_fisc_infracao tfi
                                where tfi.fiscalizacao = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.DescricaoInfracao = reader.GetValue<string>("descricao_infracao");
                        fiscalizacao.DataInfracao = reader.GetValue<string>("data_constatacao");
                        fiscalizacao.HoraInfracao = reader.GetValue<string>("hora_constatacao");
                        fiscalizacao.InfrLeve = reader.GetValue<string>("is_leve");
                        fiscalizacao.InfrMedia = reader.GetValue<string>("is_media");
                        fiscalizacao.InfrGrave = reader.GetValue<string>("is_grave");
                        fiscalizacao.InfrGravissima = reader.GetValue<string>("is_gravissima");
                    }

                    reader.Close();
                }

                #endregion

                #region Penalidades

                comando = bancoDeDados.CriarComando(@"
                            select ( select 'X'
                                     from {0}tab_fisc_penalidades_infr p,
                                          {0}lov_fisc_penalidades_fixas l,
                                          {0}tab_fisc_infracao i
                                     where p.infracao = i.id
                                           and p.penalidade = l.id
                                           and l.texto like '%Advertência%'
                                           and i.fiscalizacao = :id ) TemAdvertencia,
                                   ( select 'X'
                                     from {0}tab_fisc_penalidades_infr p,
                                          {0}lov_fisc_penalidades_fixas l,
                                          {0}tab_fisc_infracao i
                                     where p.infracao = i.id
                                           and p.penalidade = l.id
                                           and l.texto like '%Multa%'
                                           and i.fiscalizacao = :id ) TemMulta,
                                   ( select 'X'
                                     from {0}tab_fisc_penalidades_infr p,
                                          {0}lov_fisc_penalidades_fixas l,
                                          {0}tab_fisc_infracao i
                                     where p.infracao = i.id
                                           and p.penalidade = l.id
                                           and l.texto like '%Apreensão%'
                                           and i.fiscalizacao = :id ) TemApreensao,
                                   ( select 'X'
                                     from {0}tab_fisc_penalidades_infr p,
                                          {0}lov_fisc_penalidades_fixas l,
                                          {0}tab_fisc_infracao i
                                     where p.infracao = i.id
                                           and p.penalidade = l.id
                                           and l.texto like '%Interdição%'
                                           and i.fiscalizacao = :id ) TemInterdicao
                            from dual", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.TemAdvertencia = reader.GetValue<string>("TemAdvertencia");
                        fiscalizacao.TemMulta = reader.GetValue<string>("TemMulta");
                        fiscalizacao.TemApreensao = reader.GetValue<string>("TemApreensao");
                        fiscalizacao.TemInterdicao = reader.GetValue<string>("TemInterdicao");
                    }

                    reader.Close();
                }

                comando = bancoDeDados.CriarComando(@"
                            select cfip.item item,
                                   cfip.descricao
                            from tab_fisc_infracao tfi,
                                 tab_fisc_outras_penalidad_infr tfopi,
                                 cnf_fisc_infracao_penalidade cfip
                            where tfopi.infracao = tfi.id
                                  and tfopi.penalidade_outra = cfip.id
                                  and tfi.fiscalizacao = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.TemOutra01 = "X";
                        fiscalizacao.Outra01 = reader.GetValue<string>("item");
                        fiscalizacao.DescricaoOutra01 = reader.GetValue<string>("descricao");
                        fiscalizacao.DescricaoOutra01 = fiscalizacao.DescricaoOutra01.Count() <= 80 ? fiscalizacao.DescricaoOutra01 : fiscalizacao.DescricaoOutra01.Substring(0, 80);
                    }
                    if (reader.Read())
                    {
                        fiscalizacao.TemOutra02 = "X";
                        fiscalizacao.Outra02 = reader.GetValue<string>("item");
                        fiscalizacao.DescricaoOutra02 = reader.GetValue<string>("descricao");
                        fiscalizacao.DescricaoOutra02 = fiscalizacao.DescricaoOutra02.Count() <= 80 ? fiscalizacao.DescricaoOutra02 : fiscalizacao.DescricaoOutra02.Substring(0, 80);
                    }
                    if (reader.Read())
                    {
                        fiscalizacao.TemOutra03 = "X";
                        fiscalizacao.Outra03 = reader.GetValue<string>("item");
                        fiscalizacao.DescricaoOutra03 = reader.GetValue<string>("descricao");
                        fiscalizacao.DescricaoOutra03 = fiscalizacao.DescricaoOutra03.Count() <= 80 ? fiscalizacao.DescricaoOutra03 : fiscalizacao.DescricaoOutra03.Substring(0, 80);
                    }
                    if (reader.Read())
                    {
                        fiscalizacao.TemOutra04 = "X";
                        fiscalizacao.Outra04 = reader.GetValue<string>("item");
                        fiscalizacao.DescricaoOutra04 = reader.GetValue<string>("descricao");
                        fiscalizacao.DescricaoOutra04 = fiscalizacao.DescricaoOutra04.Count() <= 80 ? fiscalizacao.DescricaoOutra04 : fiscalizacao.DescricaoOutra04.Substring(0, 80);
                    }

                    reader.Close();
                }

                #endregion Penalidades

                #region Multa

                comando = bancoDeDados.CriarComando(@"
                            select tfm.valor_multa,
                                   (lficr.texto || ' - ' || lficr.descricao) codigo_receita
                            from {0}tab_fisc_multa tfm,
                                 {0}lov_fisc_infracao_codigo_rece lficr
                            where tfm.codigo_receita = lficr.id
                                  and tfm.fiscalizacao = :id
                                  and tfm.iuf_digital = 1", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.ValorMulta = reader.GetValue<decimal>("valor_multa").ToString("N2");
                        fiscalizacao.CodigoReceita = reader.GetValue<string>("codigo_receita");
                        fiscalizacao.ValorMultaPorExtenso = Escrita.PorExtenso(Convert.ToDecimal(fiscalizacao.ValorMulta), ModoEscrita.Monetario);

                        fiscalizacao.ValorMultaPorExtenso = fiscalizacao.ValorMultaPorExtenso.First().ToString().ToUpper() + fiscalizacao.ValorMultaPorExtenso.Substring(1);
                    }
                    else
                    {
                        fiscalizacao.TemMulta = null;
                    }

                    reader.Close();
                }

                #endregion Multa

                #region Apreensão

                comando = bancoDeDados.CriarComando(@"
                            select tfa.descricao,
                                   tfa.valor_produtos_reais valor_reais,
                                   tp.nome nome_depositario,
                                   tp.cpf cpf_depositario,
                                   tfa.endereco_logradouro logradouro,
                                   tfa.endereco_bairro bairro,
                                   tfa.endereco_distrito distrito,
                                   lm.texto municipio,
                                   le.sigla uf,
                                   tfa.numero_lacres lacres
                            from {0}tab_fisc_apreensao tfa,
                                 {0}tab_pessoa tp,
                                 {0}lov_municipio lm,
                                 {0}lov_estado le
                            where tp.id (+)= tfa.depositario
                                  and lm.id (+)= tfa.endereco_municipio
                                  and le.id (+)= tfa.endereco_estado
                                  and tfa.fiscalizacao = :id
                                  and tfa.iuf_digital = 1", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.DescreverApreensao = reader.GetValue<string>("descricao");
                        fiscalizacao.ValorBemProdutoArbitrado = reader.GetValue<decimal>("valor_reais").ToString("N2");
                        fiscalizacao.DepositarioNome = reader.GetValue<string>("nome_depositario");
                        fiscalizacao.DepositarioCPF = reader.GetValue<string>("cpf_depositario");
                        fiscalizacao.DepositarioLogradouro = reader.GetValue<string>("logradouro");
                        fiscalizacao.DepositarioBairro = reader.GetValue<string>("bairro");
                        fiscalizacao.DepositarioDistrito = reader.GetValue<string>("distrito");
                        fiscalizacao.DepositarioMunicipio = reader.GetValue<string>("municipio");
                        fiscalizacao.DepositarioUF = reader.GetValue<string>("uf");
                        fiscalizacao.ApreensaoLacres = reader.GetValue<string>("lacres");

                        fiscalizacao.ValorBemPorExtenso = Escrita.PorExtenso(Convert.ToDecimal(fiscalizacao.ValorBemProdutoArbitrado), ModoEscrita.Monetario);
						if(!string.IsNullOrWhiteSpace(fiscalizacao.ValorBemPorExtenso))
							fiscalizacao.ValorBemPorExtenso = fiscalizacao.ValorBemPorExtenso.First().ToString().ToUpper() + fiscalizacao.ValorBemPorExtenso.Substring(1);
                    }
                    else
                    {
                        fiscalizacao.TemApreensao = null;
                    }

                    reader.Close();
                }

                #endregion Apreensão

                #region Interdição / Embargo

                comando = bancoDeDados.CriarComando(@"
                            select tfoi.desc_termo_embargo,
                                   ( select 'X'
                                     from {0}tab_fisc_obj_infracao t
                                     where t.fiscalizacao = :id
                                           and t.interditado = 1) IsInterditado,
                                   ( select 'X'
                                     from {0}tab_fisc_obj_infracao t
                                     where t.fiscalizacao = :id
                                           and t.interditado = 0) IsEmbargado
                            from {0}tab_fisc_obj_infracao tfoi
                            where tfoi.fiscalizacao = :id
                                  and tfoi.iuf_digital = 1", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.DescricaoTermoEmbargo = reader.GetValue<string>("desc_termo_embargo");
                        fiscalizacao.IsInterditado = reader.GetValue<string>("IsInterditado");
                        fiscalizacao.IsEmbargado = reader.GetValue<string>("IsEmbargado");
                    }
                    else
                    {
                        fiscalizacao.TemInterdicao = null;
                    }

                    reader.Close();
                }

                #endregion Interdição / Embargo

                #region Descrição de outras penalidades

                comando = bancoDeDados.CriarComando(@"
                            select tfop.descricao
                            from {0}tab_fisc_outras_penalidades tfop
                            where tfop.fiscalizacao = :id
                                  and tfop.iuf_digital = 1", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.DescricaoOutrasPenalidades = reader.GetValue<string>("descricao");
                        fiscalizacao.DescricaoOutrasPenalidades = fiscalizacao.DescricaoOutrasPenalidades.Count() <= 465 ? fiscalizacao.DescricaoOutrasPenalidades : fiscalizacao.DescricaoOutrasPenalidades.Substring(0, 465);
                    }
                    else
                    {
                        fiscalizacao.TemAdvertencia = null;
                        fiscalizacao.TemOutra01 = null;
                        fiscalizacao.TemOutra02 = null;
                        fiscalizacao.TemOutra03 = null;
                        fiscalizacao.TemOutra04 = null;
                    }

                    reader.Close();
                }

                #endregion Interdição / Embargo

                #region Firmas

                comando = bancoDeDados.CriarComando(@"
					select nvl(tp.nome, tp.razao_social) nome,
						   nvl(tp.cpf, tp.cnpj) cpf,
						   tu.nome autuante
					  from tab_fisc_local_infracao tfli,
						   tab_pessoa              tp,
						   tab_fiscalizacao        tf,
						   tab_funcionario         tu
					 where tp.id = nvl(tfli.responsavel, tfli.pessoa)
					   and tf.autuante = tu.id
					   and tfli.fiscalizacao = tf.id
					   and tf.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.ResponsavelEmpNomeRazaoSocial = reader.GetValue<string>("nome");
                        fiscalizacao.ResponsavelEmpCPFCNPJ = reader.GetValue<string>("cpf");
                        fiscalizacao.NomeUsuarioCadastro = reader.GetValue<string>("autuante");
                    }

                    reader.Close();
                }

                #endregion

                #region Testemunhas

                comando = bancoDeDados.CriarComando(@"
                            select ( case when tfcft.idaf = 1 then tf.nome
                                          when (tfcft.idaf = 0 and tfcft.nome is null) then tp.nome
                                          else tfcft.nome
                                     end) nome,
                                   ( case when tfcft.idaf = 1 then tf.cpf
                                          when (tfcft.idaf = 0 and tfcft.cpf is null) then tp.cpf
                                          else tfcft.cpf
                                     end) cpf
                            from {0}tab_fisc_consid_final tfcf,
                                 {0}tab_fisc_consid_final_test tfcft,
                                 {0}tab_funcionario tf,
                                 {0}tab_pessoa tp
                            where tfcf.fiscalizacao = :id
                                  and tfcft.consid_final = tfcf.id
                                  and tf.id (+)= tfcft.testemunha
                                  and tp.id (+)= tfcft.testemunha", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        fiscalizacao.TestemunhaNome = reader.GetValue<string>("nome");
                        fiscalizacao.TestemunhaCPF = reader.GetValue<string>("cpf");
                    }

                    reader.Close();
                }

                #endregion
            }

            return fiscalizacao;
        }

		public FiscalizacaoRelatorio Obter(int id, BancoDeDados banco = null)
		{
			FiscalizacaoRelatorio objeto = new FiscalizacaoRelatorio();
			Comando comando = null;
			int autuanteId = 0;

			objeto.NumeroFiscalizacao = id.ToString();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Fiscalizacao

				comando = bancoDeDados.CriarComando(@"select f.autuante, f.situacao, f.situacao_data from {0}tab_fiscalizacao f where f.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						autuanteId = reader.GetValue<Int32>("autuante");
						objeto.SituacaoId = reader.GetValue<Int32>("situacao");

						if (objeto.SituacaoId != (int)eFiscalizacaoSituacaoRelatorio.EmAndamento) 
						{
							if (objeto.SituacaoId == (int)eFiscalizacaoSituacaoRelatorio.CadastroConcluido)
							{
								objeto.DataConclusao = reader.GetValue<DateTime>("situacao_data").ToShortDateString();
							}
							else
							{
								comando = bancoDeDados.CriarComando(@"select f.situacao_data from hst_fiscalizacao f where f.fiscalizacao_id = :fiscalizacao 
																	and f.situacao_id = 2/*Cadastro Concluido*/ and f.data_execucao = (select max(h.data_execucao) 
																	from hst_fiscalizacao h where h.fiscalizacao_id = :fiscalizacao and 
																	h.situacao_id = 2/*Cadastro Concluido*/)", EsquemaBanco);

								comando.AdicionarParametroEntrada("fiscalizacao", id, DbType.Int32);

								using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
								{
									if (readerAux.Read())
									{
										objeto.DataConclusao = readerAux.GetValue<DateTime>("situacao_data").ToShortDateString();
									}
									readerAux.Close();
								}
							}
						}
						
					}

					reader.Close();
				}

				objeto.UsuarioCadastro = _funcionarioDa.Obter(autuanteId, bancoDeDados);

				#endregion

				objeto.LocalInfracao = _localInfracaoDa.Obter(id, bancoDeDados);
				objeto.ComplementacaoDados = _complementacaoDadosDa.Obter(id, bancoDeDados);

				objeto.Infracao = _infracaoDa.Obter(id, bancoDeDados);
				objeto.ObjetoInfracao = _objetoInfracaoDa.Obter(id, bancoDeDados);
				objeto.MaterialApreendido = _materialApreendidoDa.Obter(id, bancoDeDados);

				objeto.ConsideracoesFinais = _consideracoesFinaisDa.Obter(id, bancoDeDados);
			}

			return objeto;
		}

        public FiscalizacaoRelatorioNovo ObterNovo(int id, BancoDeDados banco = null)
        {
            FiscalizacaoRelatorioNovo objeto = new FiscalizacaoRelatorioNovo();
            Comando comando = null;
            int autuanteId = 0;

            objeto.NumeroFiscalizacao = id.ToString();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                #region Fiscalizacao

                comando = bancoDeDados.CriarComando(@"select f.autuante, f.situacao, f.situacao_data from {0}tab_fiscalizacao f where f.id = :id ", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        autuanteId = reader.GetValue<Int32>("autuante");
                        objeto.SituacaoId = reader.GetValue<Int32>("situacao");

                        if (objeto.SituacaoId != (int)eFiscalizacaoSituacaoRelatorio.EmAndamento)
                        {
                            if (objeto.SituacaoId == (int)eFiscalizacaoSituacaoRelatorio.CadastroConcluido)
                            {
                                objeto.DataConclusao = reader.GetValue<DateTime>("situacao_data").ToShortDateString();
                            }
                            else
                            {
                                comando = bancoDeDados.CriarComando(@"select f.situacao_data from hst_fiscalizacao f where f.fiscalizacao_id = :fiscalizacao 
																	and f.situacao_id = 2/*Cadastro Concluido*/ and f.data_execucao = (select max(h.data_execucao) 
																	from hst_fiscalizacao h where h.fiscalizacao_id = :fiscalizacao and 
																	h.situacao_id = 2/*Cadastro Concluido*/)", EsquemaBanco);

                                comando.AdicionarParametroEntrada("fiscalizacao", id, DbType.Int32);

                                using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
                                {
                                    if (readerAux.Read())
                                    {
                                        objeto.DataConclusao = readerAux.GetValue<DateTime>("situacao_data").ToShortDateString();
                                    }
                                    readerAux.Close();
                                }
                            }
                        }

                    }

                    reader.Close();
                }

                objeto.UsuarioCadastro = _funcionarioDa.Obter(autuanteId, bancoDeDados);

                #endregion

                objeto.LocalInfracao = _localInfracaoDa.ObterNovo(id, bancoDeDados);
                objeto.Infracao = _infracaoDa.Obter(id, bancoDeDados);

                objeto.ObjetoInfracao = _objetoInfracaoDa.ObterNovo(id, bancoDeDados);
                objeto.Multa = _multaDa.Obter(id, bancoDeDados);
                objeto.MaterialApreendido = _materialApreendidoDa.ObterNovo(id, bancoDeDados);
                objeto.OutrasPenalidades = _outrasPenalidadesDa.Obter(id, bancoDeDados);

                //Se a fiscalização está em andamento, não pode haver números e datas de IUF digital
                if (objeto.SituacaoId == (int)eFiscalizacaoSituacaoRelatorio.EmAndamento)
                {
                    if (objeto.ObjetoInfracao != null && objeto.ObjetoInfracao.SerieTexto == "E")
                    {
                        objeto.ObjetoInfracao.NumeroIUF = null;
                        objeto.ObjetoInfracao.DataLavraturaIUF = null;
                    }

                    if (objeto.Multa != null && objeto.Multa.SerieTexto == "E")
                    {
                        objeto.Multa.NumeroIUF = null;
                        objeto.Multa.DataLavraturaIUF = null;
                    }

                    if (objeto.MaterialApreendido != null && objeto.MaterialApreendido.SerieTexto == "E")
                    {
                        objeto.MaterialApreendido.NumeroIUF = null;
                        objeto.MaterialApreendido.DataLavraturaIUF = null;
                    }

                    if (objeto.OutrasPenalidades != null && objeto.OutrasPenalidades.SerieTexto == "E")
                    {
                        objeto.OutrasPenalidades.NumeroIUF = null;
                        objeto.OutrasPenalidades.DataLavraturaIUF = null;
                    }
                }

                objeto.ConsideracoesFinais = _consideracoesFinaisDa.Obter(id, bancoDeDados);

                objeto.ComplementacaoDados = new ComplementacaoDadosRelatorio();
                objeto.ComplementacaoDados.VinculoPropriedade = objeto.LocalInfracao.Autuado.TipoTexto;
            }

            return objeto;
        }

		public FiscalizacaoRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
		{
			FiscalizacaoRelatorio objeto = new FiscalizacaoRelatorio();
			Comando comando = null;
			int autuanteId = 0;
			string autuanteTid = string.Empty;

			

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Fiscalizacao

				comando = bancoDeDados.CriarComando(@"select fiscalizacao_id, f.autuante_id autuante, f.autuante_tid , f.situacao_id situacao, f.situacao_data from hst_fiscalizacao f where f.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", historicoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						autuanteId = reader.GetValue<Int32>("autuante");
						autuanteTid = reader.GetValue<String>("autuante_tid");
						objeto.NumeroFiscalizacao = reader.GetValue<String>("fiscalizacao_id");
						objeto.SituacaoId = reader.GetValue<Int32>("situacao");
						objeto.DataConclusao = reader.GetValue<DateTime>("situacao_data").ToShortDateString();
					}

					reader.Close();
				}

				objeto.UsuarioCadastro = _funcionarioDa.ObterHistorico(autuanteId, autuanteTid, bancoDeDados);

				#endregion

				objeto.LocalInfracao = _localInfracaoDa.ObterHistorico(historicoId, bancoDeDados);
				objeto.ComplementacaoDados = _complementacaoDadosDa.ObterHistorico(historicoId, bancoDeDados);

				objeto.Infracao = _infracaoDa.ObterHistorico(historicoId, bancoDeDados);
				objeto.ObjetoInfracao = _objetoInfracaoDa.ObterHistorico(historicoId, bancoDeDados);
				objeto.MaterialApreendido = _materialApreendidoDa.ObterHistorico(historicoId, bancoDeDados);

				objeto.ConsideracoesFinais = _consideracoesFinaisDa.ObterHistorico(historicoId, bancoDeDados);
			}

			return objeto;
		}

		#endregion

		#region Auxiliares

		internal DateTecno ObterDataConclusao(int fiscalizacaoId, BancoDeDados banco = null)
		{
			DateTecno dataConclusao = new DateTecno();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select fisc.situacao_data data_conclusao from {0}tab_fiscalizacao fisc where fisc.situacao = 2 and fisc.id = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["data_conclusao"] != null && !Convert.IsDBNull(reader["data_conclusao"]))
						{
							dataConclusao.Data = reader.GetValue<DateTime>("data_conclusao");
						}
					}

					reader.Close();
				}
			}

			return dataConclusao;
		}


		public List<FiscalizacaoRelatorio> ObterHistoricoConcluidos()
		{
			List<FiscalizacaoRelatorio> lstFiscalizacao = new List<FiscalizacaoRelatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.fiscalizacao_id, to_char(f.situacao_data, 'yyyy.mm.DD_hh24.mi') DataConclusao , f.*
					from hst_fiscalizacao f 
					where f.situacao_id = 2
					  and f.pdf_laudo is null", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstFiscalizacao.Add(new FiscalizacaoRelatorio() {
							Id = reader.GetValue<int>("fiscalizacao_id"),
							HistoricoId = reader.GetValue<int>("id"),
							DataConclusao = reader.GetValue<String>("DataConclusao")
							
						});
					}

					reader.Close();
				}
			}

			return lstFiscalizacao;
		}

		public List<FiscalizacaoRelatorio> ObterHistoricoTarja()
		{
			List<FiscalizacaoRelatorio> lstFiscalizacao = new List<FiscalizacaoRelatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				/*Comando comando = bancoDeDados.CriarComando(@"select f.id, f.fiscalizacao_id, to_char(f.situacao_data, 'yyyy.mm.DD_hh24.mi') DataConclusao , f.*
          from hst_fiscalizacao f where f.data_execucao > to_date('16/11/2012') and f.pdf_laudo is not null ", EsquemaBanco);*/

				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.fiscalizacao_id, to_char(f.situacao_data, 'yyyy.mm.DD_hh24.mi') DataConclusao , f.*
					  from hst_fiscalizacao f where f.id in (
			select min(f.id) id_hst
					  from hst_fiscalizacao f where f.data_execucao > to_date('16/11/2012') and f.pdf_laudo is not null 
					  group by f.pdf_laudo )  ", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstFiscalizacao.Add(new FiscalizacaoRelatorio()
						{
							Id = reader.GetValue<int>("fiscalizacao_id"),
							HistoricoId = reader.GetValue<int>("id"),
							DataConclusao = reader.GetValue<String>("DataConclusao")

						});
					}

					reader.Close();
				}
			}

			return lstFiscalizacao;
		}

		#endregion

		internal void AtualizarHistorico(int historicoId, int? laudoId, int? autoId, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update hst_fiscalizacao hf set hf.pdf_laudo = :laudoId, hf.pdf_auto_termo = :autoId where hf.id = :historicoId", EsquemaBanco);


				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);
				comando.AdicionarParametroEntrada("laudoId", laudoId, DbType.Int32);
				comando.AdicionarParametroEntrada("autoId", autoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void CorrigirHistoricoSubSequente(BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
				  for i in (select h1.id, h1.pdf_laudo pdf_laudo_ant, h1.pdf_auto_termo pdf_auto_termo_ant, h2.id fisc_id_des,  h2.pdf_laudo, h2.pdf_auto_termo 
					   from hst_fiscalizacao h1, hst_fiscalizacao h2 , tab_arquivo aa 
					   where h1.fiscalizacao_id = h2.fiscalizacao_id
					   and h2.id = ( select max(hc.id)
							   from hst_fiscalizacao hc 
							   where hc.data_execucao < h1.data_execucao 
							   and hc.situacao_id = 2
							   and hc.fiscalizacao_id = h1.fiscalizacao_id)
					   and h1.situacao_id <> 2                 
					   and h2.pdf_laudo = aa.id
					   and aa.executor_login = 'Path') 
				  loop
				   update hst_fiscalizacao f set f.pdf_laudo = i.pdf_laudo where f.id = i.id and f.pdf_laudo is null;
				   update hst_fiscalizacao f set f.pdf_auto_termo = i.pdf_auto_termo where f.id = i.id and f.pdf_auto_termo is null;
				  end loop;              
				end;  ", EsquemaBanco);
				
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
				   for i in (select h.fiscalizacao_id, h.pdf_auto_termo, h.pdf_laudo
							   from tab_fiscalizacao f, hst_fiscalizacao h, (select hf.id, hf.fiscalizacao_id, hf.tid, max(hf.data_execucao) data_execucao
							   from hst_fiscalizacao hf 
							   where hf.situacao_id <> 1
							   group by hf.id, hf.fiscalizacao_id, hf.tid) hstf
							   where f.id = hstf.fiscalizacao_id
							   and f.tid = hstf.tid
							   and h.id = hstf.id
							   and f.pdf_laudo is null) 
				   loop
					  update tab_fiscalizacao f set f.pdf_laudo = i.pdf_laudo where f.id = i.fiscalizacao_id and f.pdf_laudo is null;
					  update tab_fiscalizacao f set f.pdf_auto_termo = i.pdf_auto_termo where f.id = i.fiscalizacao_id and f.pdf_auto_termo is null;
				   end loop;
				end;  ", EsquemaBanco);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal int ObterHistoricoIdConcluido(int fiscalizacao, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select max(fisc.id) from {0}hst_fiscalizacao fisc 
				where fisc.situacao_id = 2 and fisc.fiscalizacao_id = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				object objeto = bancoDeDados.ExecutarScalar(comando);

				if (objeto != null && !Convert.IsDBNull(objeto))
				{
					return Convert.ToInt32(objeto);
				}

				return 0;
			}
		}
	}
}