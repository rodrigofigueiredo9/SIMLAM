using GeoJSON.Net.Geometry;
using Ionic.Zip;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Tecnomapas.EtramiteX.Scheduler.misc;
using Tecnomapas.EtramiteX.Scheduler.misc.WKT;
using Tecnomapas.EtramiteX.Scheduler.models;
using Tecnomapas.EtramiteX.Scheduler.models.misc;
using Tecnomapas.EtramiteX.Scheduler.models.simlam;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	[DisallowConcurrentExecution]
	public class GerarArquivoCarJob : IJob
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static readonly string STRING_VAZIA = "Não Informado";
		public static readonly DateTime DATA_VAZIA = new DateTime(1900, 01, 01);
		public static readonly string DATA_STR_VAZIA = DATA_VAZIA.ToString("dd/MM/yyyy");

		/// <summary>
		/// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
		/// fires that is associated with the <see cref="T:Quartz.IJob" />.
		/// </summary>
		/// <param name="context">The execution context.</param>
		/// <remarks>
		/// The implementation may wish to set a  result object on the
		/// JobExecutionContext before this method exits.  The result itself
		/// is meaningless to Quartz, but may be informative to
		/// <see cref="T:Quartz.IJobListener" />s or
		/// <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
		/// execution.
		/// </remarks>
		public void Execute(IJobExecutionContext context)
		{
			//logging
            //var jobKey = context.JobDetail.Key;
            //Log.InfoFormat("BEGIN {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));

            //using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
            //{
            //    conn.Open();
            //    string tid = string.Empty;

            //    //Veja se 
            //    var nextItem = LocalDB.PegarProximoItemFila(conn, "gerar-car");

            //    while (nextItem != null)
            //    {
            //        //Update item as Started
            //        //LocalDB.MarcarItemFilaIniciado(conn, nextItem.Id);

            //        var requisicao = JsonConvert.DeserializeObject<RequisicaoJobCar>(nextItem.Requisicao);
            //        //var controleSicar = ControleCarDB.ObterItemControleCar(conn, requisicao);

            //        //TODO:PROCESSAR APENAS SOLICITACOES PASSIVOS
            //        if (controleSicar != null && controleSicar.solicitacao_passivo == 0)
            //        {
            //            break;
            //        }

            //        ObterDadosRequisicao(conn, requisicao);
            //        tid = Blocos.Data.GerenciadorTransacao.ObterIDAtual();

            //        try
            //        {
            //            CAR car;

            //            if (requisicao.tem_titulo || requisicao.origem == RequisicaoJobCar.INSTITUCIONAL)
            //            {
            //                car = ObterDadosCar(conn, requisicao, CarUtils.GetEsquemaInstitucional());
            //            }
            //            else
            //            {
            //                using (var connCredendicado = new OracleConnection(CarUtils.GetBancoCredenciado()))
            //                {
            //                    connCredendicado.Open();

            //                    car = ObterDadosCar(connCredendicado, requisicao, CarUtils.GetEsquemaCredenciado());
            //                }
            //            }

            //            Pessoa declarante = null;

            //            if (requisicao.origem == RequisicaoJobCar.INSTITUCIONAL)
            //                declarante = ObterDadosDeclarante(conn, CarUtils.GetEsquemaInstitucional(), requisicao.empreendimento, requisicao.empreendimento_tid, requisicao.solicitacao_car, requisicao.solicitacao_car_tid);
            //            else
            //                using (var connCredendicado = new OracleConnection(CarUtils.GetBancoCredenciado()))
            //                {
            //                    connCredendicado.Open();
            //                    declarante = ObterDadosDeclarante(connCredendicado, CarUtils.GetEsquemaCredenciado(), requisicao.empreendimento, requisicao.empreendimento_tid, requisicao.solicitacao_car, requisicao.solicitacao_car_tid);
            //                }

            //            car.cadastrante = new Cadastrante()
            //            {
            //                cpf = declarante.cpf,
            //                dataNascimento = declarante.dataNascimento,
            //                nome = declarante.nome,
            //                nomeMae = declarante.nomeMae
            //            };


            //            if (controleSicar.solicitacao_passivo > 0)
            //                PreencherCampos(car);
            //            else
            //                ValidarCampos(car);

            //            //Salvar o arquivo .CAR
            //            var arquivoCar = GerarArquivoCAR(car, conn);

            //            //Marcar como processado
            //            LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, true, "Processado");

            //            //Atualizar o Controle do SICAR
            //            //var idControleSicar = ControleCarDB.InserirControleSICAR(conn, nextItem, arquivoCar);
            //            var idControleSicar = ControleCarDB.AtualizarControleSICAR(conn, null, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_GERADO, tid);

            //            //var idControleSicar = ControleCarDB.InserirControleSICAR(conn, nextItem, arquivoCar);

            //            //Adicionar na fila pedido para Enviar Arquivo SICAR
            //            LocalDB.AdicionarItemFila(conn, "enviar-car", nextItem.Id, arquivoCar, requisicao.empreendimento);
            //        }
            //        catch (Exception ex)
            //        {
            //            //Marcar como processado registrando a mensagem de erro
            //            LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, false, ex.Message);
            //            ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, ControleCarDB.SITUACAO_SOLICITACAO_PENDENTE, tid);
            //            ControleCarDB.AtualizarControleSICAR(conn, new MensagemRetorno() { mensagensResposta = new List<string> { ex.Message } }, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_REPROVADO, tid);
            //        }

            //        nextItem = LocalDB.PegarProximoItemFila(conn, "gerar-car");
            //    }
            //}

            //Log.InfoFormat("ENDING {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));
		}

		private void ValidarCampos(CAR car)
		{
			StringBuilder mensagens = new StringBuilder();

			if (String.IsNullOrWhiteSpace(car.imovel.cep))


				if (String.IsNullOrWhiteSpace(car.imovel.enderecoCorrespondencia.cep))
					mensagens.AppendLine("Campo CEP do endereço de correspondência deve ser preenchido;");

			#region [ DOCUMENTO.RESERVALEGAL.DADOSRESERVA ]

			if (car.documentos != null && car.documentos.Count > 0)
			{
				foreach (var documento in car.documentos)
				{
					if (documento.reservaLegal.dadosReserva != null && documento.reservaLegal.dadosReserva.Count > 0)
					{
						foreach (var dadosReserva in documento.reservaLegal.dadosReserva)
						{
							if (dadosReserva.reservaDentroImovel == "Não" && String.IsNullOrWhiteSpace(dadosReserva.numeroCAR))
								mensagens.AppendLine("O Empreendimento possui reserva legal compensada, é necessário enviar o CAR do empreendimento cedente primeiro;");
						}
					}
				}
			}

			#endregion


			if (mensagens.Length > 0)
				throw new Exception(mensagens.ToString());


		}

		private void PreencherCampos(CAR car)
		{
			#region [ CADASTRANTE ]

			if (String.IsNullOrWhiteSpace(car.cadastrante.nome))
				car.cadastrante.nome = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.cadastrante.cpf))
				car.cadastrante.cpf = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.cadastrante.nomeMae))
				car.cadastrante.nomeMae = STRING_VAZIA;

			if (car.cadastrante.dataNascimento == DateTime.MinValue)
				car.cadastrante.dataNascimento = DATA_VAZIA;

			#endregion

			#region [ IMOVEL ]

			if (String.IsNullOrWhiteSpace(car.imovel.nome))
				car.imovel.nome = STRING_VAZIA;

			//TODO:CAMPO NUMERICO
			//if (String.IsNullOrWhiteSpace(car.imovel.codigoMunicipio))
			//	car.imovel.codigoMunicipio = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.imovel.cep))
				car.imovel.cep = "29000-000";

			if (String.IsNullOrWhiteSpace(car.imovel.descricaoAcesso))
				car.imovel.descricaoAcesso = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.imovel.zonaLocalizacao))
				car.imovel.zonaLocalizacao = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.imovel.email))
				car.imovel.email = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.imovel.telefone))
				car.imovel.telefone = STRING_VAZIA;

			#endregion

			#region [ IMOVEL.ENDERECOCORESPONDENCIA ]

			if (String.IsNullOrWhiteSpace(car.imovel.enderecoCorrespondencia.logradouro))
				car.imovel.enderecoCorrespondencia.logradouro = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.imovel.enderecoCorrespondencia.numero))
				car.imovel.enderecoCorrespondencia.numero = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.imovel.enderecoCorrespondencia.bairro))
				car.imovel.enderecoCorrespondencia.bairro = STRING_VAZIA;

			if (String.IsNullOrWhiteSpace(car.imovel.enderecoCorrespondencia.cep))
				car.imovel.enderecoCorrespondencia.cep = "29000-000";

			//TODO:CAMPO NUMERICO
			//if (String.IsNullOrWhiteSpace(car.imovel.enderecoCorrespondencia.codigoMunicipio))
			//	car.imovel.enderecoCorrespondencia.codigoMunicipio = STRING_VAZIA;

			#endregion

			#region [ PROPRIETARIOSPOSSEIROSCONCESSIONARIOS ]

			if (car.proprietariosPosseirosConcessionarios != null && car.proprietariosPosseirosConcessionarios.Count > 0)
			{
				foreach (var proprietario in car.proprietariosPosseirosConcessionarios)
				{
					if (String.IsNullOrWhiteSpace(proprietario.tipo))
						proprietario.tipo = STRING_VAZIA;

					if (String.IsNullOrWhiteSpace(proprietario.cpfCnpj))
						proprietario.cpfCnpj = STRING_VAZIA;

					if (String.IsNullOrWhiteSpace(proprietario.nome))
						proprietario.nome = STRING_VAZIA;

					if (proprietario.tipo == ProprietariosPosseirosConcessionario.TipoPessoaJuridica)
					{
						if (String.IsNullOrWhiteSpace(proprietario.nomeFantasia))
							proprietario.nomeFantasia = STRING_VAZIA;
					}

					if (proprietario.tipo == ProprietariosPosseirosConcessionario.TipoPessoaFisica)
					{
						if (String.IsNullOrWhiteSpace(proprietario.dataNascimento))
							proprietario.dataNascimento = DATA_STR_VAZIA;

						if (String.IsNullOrWhiteSpace(proprietario.nomeMae))
							proprietario.nomeMae = STRING_VAZIA;
					}
				}
			}

			#endregion

			#region [ DOCUMENTOS ]

			if (car.documentos != null && car.documentos.Count > 0)
			{
				foreach (var documento in car.documentos)
				{
					if (String.IsNullOrWhiteSpace(documento.tipo))
						documento.tipo = STRING_VAZIA;

					if (String.IsNullOrWhiteSpace(documento.denominacao))
						documento.denominacao = STRING_VAZIA;

					//TODO:CAMPO NUMERICO
					//if (String.IsNullOrWhiteSpace(documento.area))
					//	documento.area = STRING_VAZIA;

					//if (documento.proprietariosPosseirosConcessionarios != null && documento.proprietariosPosseirosConcessionarios.Count > 0)
					//	documento.proprietariosPosseirosConcessionarios = new List<string> { STRING_VAZIA };

					//if (String.IsNullOrWhiteSpace(documento.tipoDocumentoPropriedade))
					//	documento.tipoDocumentoPropriedade = STRING_VAZIA;

					//if (String.IsNullOrWhiteSpace(documento.tipoDocumentoPosse))
					//	documento.tipoDocumentoPosse = STRING_VAZIA;

					#region [ DOCUMENTO.DETALHEDOCUMENTOPROPRIEDADE ]

					if (documento.tipo == Documento.TipoPropriedade)
					{
						if (String.IsNullOrWhiteSpace(documento.detalheDocumentoPropriedade.numeroMatricula))
							documento.detalheDocumentoPropriedade.numeroMatricula = STRING_VAZIA;

						if (!documento.detalheDocumentoPropriedade.dataRegistro.HasValue && documento.detalheDocumentoPropriedade.dataRegistro.Value == DateTime.MinValue)
							documento.detalheDocumentoPropriedade.dataRegistro = DATA_VAZIA;

						if (String.IsNullOrWhiteSpace(documento.detalheDocumentoPropriedade.livro))
							documento.detalheDocumentoPropriedade.livro = STRING_VAZIA;

						if (String.IsNullOrWhiteSpace(documento.detalheDocumentoPropriedade.folha))
							documento.detalheDocumentoPropriedade.folha = STRING_VAZIA;

						//TODO:CAMPO NUMERICO
						//if (String.IsNullOrWhiteSpace(documento.detalheDocumentoPropriedade.municipioCartorio))
						//	documento.detalheDocumentoPropriedade.municipioCartorio = STRING_VAZIA;
					}

					#endregion

					#region [ DOCUMENTO.DETALHEDOCUMENTOPOSSE ]

					if (documento.tipo == Documento.TipoPosse)
					{
						if (String.IsNullOrWhiteSpace(documento.detalheDocumentoPosse.emissorDocumento))
							documento.detalheDocumentoPosse.emissorDocumento = STRING_VAZIA;

						if (!documento.detalheDocumentoPosse.dataDocumento.HasValue && documento.detalheDocumentoPosse.dataDocumento.Value == DateTime.MinValue)
							documento.detalheDocumentoPosse.dataDocumento = DATA_VAZIA;
					}

					#endregion

					#region [ DOCUMENTO.RESERVALEGAL ]

					if (String.IsNullOrWhiteSpace(documento.reservaLegal.resposta))
						documento.reservaLegal.resposta = STRING_VAZIA;

					#endregion

					#region [ DOCUMENTO.RESERVALEGAL.DADOSRESERVA ]

					if (documento.reservaLegal.dadosReserva != null && documento.reservaLegal.dadosReserva.Count > 0)
					{
						foreach (var dadosReserva in documento.reservaLegal.dadosReserva)
						{
							if (String.IsNullOrWhiteSpace(dadosReserva.numero))
								dadosReserva.numero = STRING_VAZIA;

							if (dadosReserva.data == DateTime.MinValue)
								dadosReserva.data = DATA_VAZIA;

							if (dadosReserva.reservaDentroImovel == "Não" && String.IsNullOrWhiteSpace(dadosReserva.numeroCAR))
								dadosReserva.numeroCAR = "ES-0000001-00000000000000000000000000000001";
						}
					}

					#endregion
				}
			}

			#endregion
		}

		private void ObterDadosRequisicao(OracleConnection conn, RequisicaoJobCar requisicao)
		{
			var tituloId = 0;
			var tituloTid = string.Empty;
			var query = string.Empty;

			if (requisicao.origem == RequisicaoJobCar.INSTITUCIONAL)
				query = @"select tt.id, tt.tid from tab_car_solicitacao c, tab_titulo tt where c.empreendimento=tt.empreendimento
					and tt.modelo=(select ttm.id from tab_titulo_modelo ttm where ttm.codigo = 49) and tt.situacao=3 and c.id = :solicitacao_car";
			else
				query = @"select tt.id, tt.tid from tab_car_solicitacao_cred cc, cre_empreendimento tec,tab_titulo tt, tab_empreendimento te where cc.empreendimento = tec.id
					and tt.modelo=(select ttm.id from tab_titulo_modelo ttm where ttm.codigo = 49) and tt.situacao=3 and tt.empreendimento = te.id and te.codigo = tec.codigo and cc.id = :solicitacao_car";

			using (var cmd = new OracleCommand(query, conn))
			{
				cmd.Parameters.Add(new OracleParameter("solicitacao_car", requisicao.solicitacao_car));

				using (var dr = cmd.ExecuteReader())
					if (dr.Read())
					{
						tituloId = Convert.ToInt32(dr["id"]);
						tituloTid = Convert.ToString(dr["tid"]);
					}
			}

			requisicao.tem_titulo = tituloId > 0;

			if (requisicao.tem_titulo)
				query = @" select ttd.dependencia_id caract_id, ttd.dependencia_tid caract_tid, ttd.dependencia_projeto_id projeto_id, ttd.dependencia_projeto_tid projeto_tid
					from tab_titulo_dependencia ttd where ttd.titulo=:id and ttd.dependencia_caracterizacao=1";
			else if (requisicao.origem == RequisicaoJobCar.INSTITUCIONAL)
				query = @" select c.dominialidade_id caract_id, c.dominialidade_tid caract_tid, c.projeto_geo_id projeto_id, c.projeto_geo_tid projeto_tid 
					from hst_car_solicitacao c where c.solicitacao_id=:id and c.tid=:tid ";
			else
				query = @" select c.dominialidade_id caract_id, c.dominialidade_tid caract_tid, c.projeto_geo_id projeto_id, c.projeto_geo_tid projeto_tid 
					from hst_car_solicitacao_cred c where c.solicitacao_id=:id and c.tid=:tid ";

			using (var cmd = new OracleCommand(query, conn))
			{
				if (requisicao.tem_titulo)
				{
					cmd.Parameters.Add(new OracleParameter("id", tituloId));
					//cmd.Parameters.Add(new OracleParameter("tid", tituloTid));
				}
				else
				{
					cmd.Parameters.Add(new OracleParameter("id", requisicao.solicitacao_car));
					cmd.Parameters.Add(new OracleParameter("tid", requisicao.solicitacao_car_tid));
				}

				using (var dr = cmd.ExecuteReader())
					if (dr.Read())
					{
						requisicao.caracterizacao_id = Convert.ToInt32(dr["caract_id"]);
						requisicao.caracterizacao_tid = Convert.ToString(dr["caract_tid"]);
						requisicao.projeto_geografico_id = Convert.ToInt32(dr["projeto_id"]);
						requisicao.projeto_geografico_tid = Convert.ToString(dr["projeto_tid"]);
					}
			}
		}

		/// <summary>
		/// Obters the dados car.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="requisicao">The requisicao.</param>
		/// <param name="schema">The schema.</param>
		/// <returns></returns>
		private static CAR ObterDadosCar(OracleConnection conn, RequisicaoJobCar requisicao, string schema)
		{
			var car = new CAR();

			var imovel = ObterDadosImovel(conn, schema, requisicao.empreendimento, requisicao.empreendimento_tid);

			var builder = new StringBuilder();
			builder.Append(imovel.logradouro);
			builder.Append(",");
			builder.Append(imovel.numero);
			builder.Append(",");
			builder.Append(imovel.complemento);
			builder.Append(" - ");
			builder.Append(imovel.bairro);
			builder.Append(",");
			builder.Append(imovel.distrito);
			builder.Append(",");
			builder.Append(imovel.corrego);

			var descricaoAcesso = builder.ToString();

			var complementoCorrespondencia = imovel.complemento + " C.P.:" + imovel.caixaPostal;
			var bairroCorrespondencia = imovel.bairro + ", " + imovel.distrito + ", " + imovel.corrego;

			car.imovel = new Imovel()
			{
				nome = imovel.denominador,
				codigoMunicipio = imovel.municipio,
				cep = imovel.cep,
				descricaoAcesso = (descricaoAcesso.Length > 1000 ? descricaoAcesso.Substring(0, 1000) : descricaoAcesso),
				zonaLocalizacao = (imovel.zona == 1 ? Imovel.ZonaUrbana : Imovel.ZonaRural),
				email = (imovel.email.Length > 100 ? imovel.email.Substring(0, 100) : imovel.email),
				telefone = (imovel.telefone.Length > 14 ? imovel.telefone.Substring(0, 14) : imovel.telefone),
				modulosFiscais = ObterModuloFiscal(conn, imovel.id, schema),
				enderecoCorrespondencia = new EnderecoCorrespondencia()
				{
					logradouro = (imovel.logradouro.Length > 100 ? imovel.logradouro.Substring(0, 100) : imovel.logradouro),
					numero = (String.IsNullOrEmpty(imovel.numero) ? "S/N" : imovel.numero),
					complemento =
						(complementoCorrespondencia.Length > 100
							? complementoCorrespondencia.Substring(0, 100)
							: complementoCorrespondencia),
					bairro = (bairroCorrespondencia.Length > 100 ? bairroCorrespondencia.Substring(0, 100) : bairroCorrespondencia),
					cep = imovel.cep,
					codigoMunicipio = imovel.municipio
				}
			};

			var proprietarios = ObterProprietariosPosseirosConcessionarios(conn, schema, requisicao.empreendimento, requisicao.empreendimento_tid);
			car.proprietariosPosseirosConcessionarios = proprietarios;

			car.documentos = ObterDocumentos(conn, schema, requisicao.origem, requisicao.empreendimento, requisicao.empreendimento_tid, imovel.municipio, requisicao.caracterizacao_id, requisicao.caracterizacao_tid);

			var cpfCpnjProprietarios = proprietarios.Select(proprietario => proprietario.cpfCnpj).ToList();
			foreach (var documento in car.documentos)
			{
				documento.proprietariosPosseirosConcessionarios = cpfCpnjProprietarios;
			}

			//car.Informações já auto-geradas ao instanciar um objeto CAR

			car.geo = ObterGeometriasImovel(conn, schema, requisicao.empreendimento, requisicao.empreendimento_tid, requisicao.caracterizacao_id, requisicao.caracterizacao_tid, requisicao.projeto_geografico_id, requisicao.projeto_geografico_tid);

			//Atualizar código do protocolo com as informações do código do município do empreendimento
			car.origem.codigoProtocolo = string.Format("{0}-{1}-{2}", CarUtils.ObterCodigoUF(car.imovel.codigoMunicipio), car.imovel.codigoMunicipio, car.origem.codigoProtocolo);

			return car;
		}

		/// <summary>
		/// Obters the dados imovel.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="empreendimentoId">The empreendimento identifier.</param>
		/// <param name="empreendimentoTid">The empreendimento tid.</param>
		/// <returns></returns>
		private static Empreendimento ObterDadosImovel(OracleConnection conn, string schema, int empreendimentoId, string empreendimentoTid)
		{
			var empreendimento = new Empreendimento();

			using (
				var cmd =
					new OracleCommand(
						"SELECT * FROM " + schema +
						".HST_EMPREENDIMENTO t WHERE t.empreendimento_id = :empreendimento_id AND tid = :empreendimento_tid", conn))
			{
				cmd.Parameters.Add(new OracleParameter("empreendimento_id", empreendimentoId));
				cmd.Parameters.Add(new OracleParameter("empreendimento_tid", empreendimentoTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						empreendimento.id = Convert.ToInt32(dr["empreendimento_id"]);
						empreendimento.cnpj = Convert.ToString(dr["cnpj"]);
						empreendimento.denominador = Convert.ToString(dr["denominador"]);
						empreendimento.nomeFantasia = Convert.ToString(dr["nome_fantasia"]);
					}
				}
			}

			//Endereço do Empreendimento - Se existir, usar o de localizacao do empreendimento (correspondencia = 0), senao, usar um dos de correspondencia (correspondencia = 1)
			var enderecos = new List<EnderecoEmpreendimento>();

			using (
				var cmd =
					new OracleCommand(
						"SELECT correspondencia,zona,cep,logradouro,bairro,municipio_id,numero,caixa_postal,distrito,corrego,complemento FROM " +
						schema +
						".HST_EMPREENDIMENTO_ENDERECO t WHERE t.tid = :tid AND correspondencia IN (0,1) ORDER BY correspondencia ASC",
						conn))
			{
				cmd.Parameters.Add(new OracleParameter("tid", empreendimentoTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						var empTemporario = new EnderecoEmpreendimento
						{
							correspondencia = dr.GetValue<Int32>("correspondencia"),
							zona = dr.GetValue<Int32>("zona"),
							logradouro = dr.GetValue<string>("logradouro"),
							bairro = dr.GetValue<string>("bairro"),
							municipio = dr.GetValue<Int32>("municipio_id"),
							cep = dr.GetValue<string>("cep"),
							numero = dr.GetValue<string>("numero"),
							caixaPostal = dr.GetValue<string>("caixa_postal"),
							distrito = dr.GetValue<string>("distrito"),
							corrego = dr.GetValue<string>("corrego"),
							complemento = dr.GetValue<string>("complemento")
						};

						enderecos.Add(empTemporario);
					}
				}
			}

			if (enderecos.Count > 0)
			{
				var enderecoEscolhido = 0;
				for (var i = 0; i < enderecos.Count; i++)
				{
					if (enderecos[i].correspondencia == 0)
					{
						enderecoEscolhido = i;
						break;
					}
				}

				empreendimento.zona = enderecos[enderecoEscolhido].zona;
				empreendimento.cep = enderecos[enderecoEscolhido].cep;
				empreendimento.logradouro = enderecos[enderecoEscolhido].logradouro;
				empreendimento.bairro = enderecos[enderecoEscolhido].bairro;
				empreendimento.municipio = enderecos[enderecoEscolhido].municipio;
				empreendimento.numero = enderecos[enderecoEscolhido].numero;
				empreendimento.caixaPostal = enderecos[enderecoEscolhido].caixaPostal;
				empreendimento.distrito = enderecos[enderecoEscolhido].distrito;
				empreendimento.corrego = enderecos[enderecoEscolhido].corrego;
				empreendimento.complemento = enderecos[enderecoEscolhido].complemento;
			}

			/*if (empreendimento.logradouro == null)
			{
				throw new Exception("Logradouro do endereço é um valor obrigatório!");
			}
			if (empreendimento.numero == null)
			{
				throw new Exception("Número do endereço é um valor obrigatório!");
			}
			if (empreendimento.bairro == null)
			{
				throw new Exception("Nome do bairro é um valor obrigatório!");
			}*/

			if (!String.IsNullOrEmpty(empreendimento.cep))
				empreendimento.cep = empreendimento.cep.Replace(".", "").Replace("-", "");

			//Buscar código do IBGE
			using (var cmd = new OracleCommand("SELECT ibge FROM " + schema + ".LOV_MUNICIPIO t WHERE t.id = :id", conn))
			{
				cmd.Parameters.Add(new OracleParameter("id", empreendimento.municipio));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						empreendimento.municipio = Convert.ToInt32(dr["ibge"]);
					}
				}
			}

			//Buscar meios de contato
			using (
				var cmd =
					new OracleCommand(
						"SELECT meio_contato_id,valor FROM " + schema +
						".HST_EMPREENDIMENTO_CONTATO t WHERE t.emp_contato_id = :emp_contato_id AND t.tid = :tid", conn))
			{
				cmd.Parameters.Add(new OracleParameter("emp_contato_id", empreendimentoId));
				cmd.Parameters.Add(new OracleParameter("tid", empreendimentoTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						var meioContato = Convert.ToInt32(dr["meio_contato_id"]);
						if (meioContato == 5)
						{
							empreendimento.email += dr.GetValue<string>("valor") + " ";
						}
						else
						{
							empreendimento.telefone += dr.GetValue<string>("valor") + " ";
						}
					}
				}
			}

			if (empreendimento.email == null)
			{
				empreendimento.email = string.Empty;
			}

			if (empreendimento.telefone == null)
			{
				empreendimento.telefone = string.Empty;
			}

			return empreendimento;
		}

		/// <summary>
		/// Obters the modulo fiscal.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="empreendimento">The empreendimento.</param>
		/// <param name="schema">The schema.</param>
		/// <returns></returns>
		private static double ObterModuloFiscal(OracleConnection conn, int empreendimento, string schema)
		{
			double area = 0;
			double moduloHa = 0;

			//Obter area do documento convertida em HA
			using (
				var cmd =
					new OracleCommand(
						"SELECT (croqui_area/10000) as area FROM " + schema +
						".CRT_DOMINIALIDADE t WHERE t.empreendimento = :empreendimento", conn))
			{
				cmd.Parameters.Add(new OracleParameter("empreendimento", empreendimento));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						area = Convert.ToDouble(dr["area"]);
					}
				}
			}

			//Obter modulo fiscal do municipio do empreendimento
			using (
				var cmd =
					new OracleCommand(
						"SELECT modulo_ha FROM " + schema + ".CNF_MUNICIPIO_MOD_FISCAL t WHERE t.municipio = (SELECT municipio FROM " +
						schema + ".TAB_EMPREENDIMENTO_ENDERECO e WHERE e.empreendimento = :empreendimento AND correspondencia = 0)", conn)
				)
			{
				cmd.Parameters.Add(new OracleParameter("empreendimento", empreendimento));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						moduloHa = Convert.ToDouble(dr["modulo_ha"]);
					}
				}
			}

			return Math.Round(area / moduloHa, 2);
		}

		/// <summary>
		/// Obters the dados declarante.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="empreendimentoId">The empreendimento identifier.</param>
		/// <param name="empreendimentoTid">The empreendimento tid.</param>
		/// <param name="solicitacaoCar">The solicitacao car.</param>
		/// <param name="solicitacaoCarTid">The solicitacao car tid.</param>
		/// <returns></returns>
		private static Pessoa ObterDadosDeclarante(OracleConnection conn, string schema, int empreendimentoId, string empreendimentoTid, int solicitacaoCar, string solicitacaoCarTid)
		{
			var declarante = 0;
			var declaranteTid = string.Empty;
			var pessoa = new Pessoa();

			using (
				var cmd =
					new OracleCommand(
						"SELECT declarante_id,declarante_tid FROM " + schema +
						".HST_CAR_SOLICITACAO t WHERE t.empreendimento_id = :empreendimento_id AND situacao_id IN (1,2,5,6) AND tid = :tid",
						conn))
			{
				cmd.Parameters.Add(new OracleParameter("empreendimento_id", empreendimentoId));
				cmd.Parameters.Add(new OracleParameter("tid", solicitacaoCarTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						declarante = Convert.ToInt32(dr["declarante_id"]);
						declaranteTid = Convert.ToString(dr["declarante_tid"]);
					}
				}
			}

			//Buscar Pessoa. Se for PF, ele é o declarante
			using (
				var cmd =
					new OracleCommand(
						"SELECT pessoa_id,tipo,nome,mae,cpf,data_nascimento FROM " + schema +
						".HST_PESSOA t WHERE t.pessoa_id = :pessoa_id AND t.tid = :tid", conn))
			{
				cmd.Parameters.Add(new OracleParameter("pessoa_id", declarante));
				cmd.Parameters.Add(new OracleParameter("tid", declaranteTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						pessoa.id = Convert.ToInt32(dr["pessoa_id"]);
						pessoa.tipo = Convert.ToInt32(dr["tipo"]);
						pessoa.nome = Convert.ToString(dr["nome"]);
						pessoa.nomeMae = Convert.ToString(dr["mae"]);
						pessoa.cpf = Convert.ToString(dr["cpf"]).Replace(".", "").Replace("-", "");
						pessoa.dataNascimento = dr.GetValue<DateTime>("data_nascimento");
					}
				}
			}

			//Se for PJ, buscar o representante da PJ. Ele será o declarante
			if (pessoa.tipo == 2)
			{
				var representanteId = 0;
				var representanteTid = string.Empty;

				using (
					var cmd =
						new OracleCommand(
							"SELECT representante_id,representante_tid FROM " + schema +
							".HST_PESSOA_REPRESENTANTE t WHERE t.pessoa_id = :pessoa_id AND (t.tid = :tid OR t.representante_tid = :tid)", conn))
				{
					cmd.Parameters.Add(new OracleParameter("pessoa_id", pessoa.id));
					cmd.Parameters.Add(new OracleParameter("tid", declaranteTid));

					using (var dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							representanteId = Convert.ToInt32(dr["representante_id"]);
							representanteTid = Convert.ToString(dr["representante_tid"]);
						}
					}
				}

				pessoa = new Pessoa();

				using (
					var cmd =
						new OracleCommand(
							"SELECT pessoa_id,tipo,nome,mae,cpf,data_nascimento FROM " + schema +
							".HST_PESSOA t WHERE t.pessoa_id = :pessoa_id AND t.tid = :tid", conn))
				{
					cmd.Parameters.Add(new OracleParameter("pessoa_id", representanteId));
					cmd.Parameters.Add(new OracleParameter("tid", representanteTid));

					using (var dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							pessoa.id = Convert.ToInt32(dr["pessoa_id"]);
							pessoa.tipo = Convert.ToInt32(dr["tipo"]);
							pessoa.nome = Convert.ToString(dr["nome"]);
							pessoa.nomeMae = Convert.ToString(dr["mae"]);
							pessoa.cpf = Convert.ToString(dr["cpf"]).Replace(".", "").Replace("-", "");
							pessoa.dataNascimento = dr.GetValue<DateTime>("data_nascimento");
							if (pessoa.dataNascimento == DateTime.MinValue)
							{
								pessoa.dataNascimento = new DateTime(1900, 01, 01);
							}

						}
					}
				}
			}

			if (String.IsNullOrEmpty(pessoa.nomeMae))
			{
				pessoa.nomeMae = "Não informado";
			}

			return pessoa;
		}

		/// <summary>
		/// Obters the proprietarios posseiros concessionarios.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="empreendimentoId">The empreendimento identifier.</param>
		/// <param name="empreendimentoTid">The empreendimento tid.</param>
		/// <returns></returns>
		private static List<ProprietariosPosseirosConcessionario> ObterProprietariosPosseirosConcessionarios(OracleConnection conn, string schema, int empreendimentoId, string empreendimentoTid)
		{
			var resultado = new List<ProprietariosPosseirosConcessionario>();
			var responsaveis = new Dictionary<int, string>();

			using (
				var cmd =
					new OracleCommand(
						"SELECT responsavel_id,responsavel_tid FROM " + schema +
						".HST_EMPREENDIMENTO_RESPONSAVEL t WHERE t.empreendimento_id = :empreendimento_id AND empreendimento_tid = :empreendimento_tid",
						conn))
			{
				cmd.Parameters.Add(new OracleParameter("empreendimento_id", empreendimentoId));
				cmd.Parameters.Add(new OracleParameter("empreendimento_tid", empreendimentoTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						responsaveis.Add(Convert.ToInt32(dr["responsavel_id"]), Convert.ToString(dr["responsavel_tid"]));
					}
				}
			}

			foreach (var responsavel in responsaveis)
			{
				resultado.Add(ObterResponsavel(conn, schema, responsavel.Key, responsavel.Value));
			}

			return resultado;
		}

		/// <summary>
		/// Obters the responsavel.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="responsavelId">The responsavel identifier.</param>
		/// <param name="responsavelTid">The responsavel tid.</param>
		/// <returns></returns>
		private static ProprietariosPosseirosConcessionario ObterResponsavel(OracleConnection conn, string schema, int responsavelId, string responsavelTid)
		{
			var ppc = new ProprietariosPosseirosConcessionario();

			using (
				var cmd =
					new OracleCommand(
						"SELECT tipo,nome,mae,cpf,cnpj,to_char(data_nascimento,'DD/MM/YYYY') as data_nascimento,nome_fantasia,razao_social FROM " +
						schema +
						".HST_PESSOA t WHERE t.pessoa_id = :pessoa_id AND t.tid = :tid", conn))
			{
				cmd.Parameters.Add(new OracleParameter("pessoa_id", responsavelId));
				cmd.Parameters.Add(new OracleParameter("tid", responsavelTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						var tipo = Convert.ToInt32(dr["tipo"]);

						if (tipo == 1)
						{
							ppc.tipo = ProprietariosPosseirosConcessionario.TipoPessoaFisica;
							ppc.cpfCnpj = Convert.ToString(dr["cpf"]).Replace(".", "").Replace("-", "");
							ppc.nome = Convert.ToString(dr["nome"]);
							ppc.nomeFantasia = null;
							ppc.dataNascimento = Convert.ToString(dr["data_nascimento"]);
							if (String.IsNullOrEmpty(ppc.dataNascimento))
							{
								ppc.dataNascimento = "01/01/1900";
							}
							ppc.nomeMae = Convert.ToString(dr["mae"]);
							if (String.IsNullOrEmpty(ppc.nomeMae))
							{
								ppc.nomeMae = "Não informado";
							}
						}
						else
						{
							ppc.tipo = ProprietariosPosseirosConcessionario.TipoPessoaJuridica;
							ppc.cpfCnpj = Convert.ToString(dr["cnpj"]).Replace(".", "").Replace("-", "").Replace("/", "");
							ppc.nome = Convert.ToString(dr["razao_social"]);
							ppc.nomeFantasia = Convert.ToString(dr["nome_fantasia"]);
							ppc.dataNascimento = string.Empty;
							ppc.nomeMae = string.Empty;
						}
					}
				}
			}

			return ppc;
		}

		/// <summary>
		/// Obters the documentos.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="empreendimentoId">The empreendimento identifier.</param>
		/// <param name="empreendimentoTid">The empreendimento tid.</param>
		/// <param name="empreendimentoMunicipio">The empreendimento municipio.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">Empreendimento não possui Dominialidade. Faça o cadastro e envie novamente.</exception>
		private static List<Documento> ObterDocumentos(OracleConnection conn, string schema, string requisicaoOrigem, int empreendimentoId, string empreendimentoTid, int empreendimentoMunicipio, int dominialidadeId, string dominialidadeTid)
		{
			var resultado = new List<Documento>();

			//Se não encontrar a dominialidade no banco
			if (dominialidadeId == 0)
			{
				throw new Exception("Empreendimento não possui Dominialidade. Faça o cadastro e envie novamente.");
			}

			using (
				var cmd =
					new OracleCommand(
						"SELECT dominialidade_dominio_id,tid,tipo_id,identificacao,area_documento,matricula,folha,livro,numero_ccri,registro,comprovacao_texto FROM " +
						schema +
						".HST_CRT_DOMINIALIDADE_DOMINIO t WHERE t.dominialidade_id = :dominialidade_id AND dominialidade_tid = :dominialidade_tid",
						conn))
			{
				cmd.Parameters.Add(new OracleParameter("dominialidade_id", dominialidadeId));
				cmd.Parameters.Add(new OracleParameter("dominialidade_tid", dominialidadeTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						var doc = new Documento
						{
							detalheDocumentoPosse = new DetalheDocumentoPosse
							{
								enderecoDeclarante = new EnderecoDeclarante()
							},
							detalheDocumentoPropriedade = new DetalheDocumentoPropriedade()
						};


						var tipo = Convert.ToInt32(dr["tipo_id"]);
						doc.area = Math.Round(Convert.ToDouble(dr["area_documento"]) / 10000, 2); //Converter de m2 para ha

						if ((tipo == 1) || (tipo == 2))
						{
							doc.denominacao = Convert.ToString(dr["identificacao"]); // E SE FOR OUTRO TIPO?
						}

						if (tipo == 1)
						{
							doc.tipo = Documento.TipoPropriedade;

							doc.tipoDocumentoPropriedade = Documento.TipoDocPropCertidaoRegistro;
							doc.detalheDocumentoPropriedade = new DetalheDocumentoPropriedade
							{
								numeroMatricula = Convert.ToString(dr["matricula"]),
								livro = Convert.ToString(dr["livro"]),
								folha = Convert.ToString(dr["folha"]),
								dataRegistro = new DateTime(1900, 01, 01),
								municipioCartorio = empreendimentoMunicipio
							};
						}
						else
						{
							doc.tipo = Documento.TipoPosse;

							doc.tipoDocumentoPosse = Documento.TipoDocPosseTituloDominio;
							doc.detalheDocumentoPosse = new DetalheDocumentoPosse();

							var emissorDocumento = Convert.ToString(dr["comprovacao_texto"]) + " " + Convert.ToString(dr["registro"]);
							doc.detalheDocumentoPosse.emissorDocumento = (emissorDocumento.Length > 100
								? emissorDocumento.Substring(0, 100)
								: emissorDocumento);
							doc.detalheDocumentoPosse.dataDocumento = new DateTime(1900, 01, 01);
						}

						doc.ccir = Convert.ToString(dr["numero_ccri"]);
						doc.reservaLegal = ObterDadosReservaLegal(conn, schema, requisicaoOrigem, Convert.ToInt32(dr["dominialidade_dominio_id"]),
							Convert.ToString(dr["tid"]));

						resultado.Add(doc);
					}
				}
			}

			return resultado;
		}

		/// <summary>
		/// Obters the dados reserva legal.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="dominioId">The dominio identifier.</param>
		/// <param name="dominioTid">The dominio tid.</param>
		/// <returns></returns>
		private static ReservaLegal ObterDadosReservaLegal(OracleConnection conn, string schema, string requisicaoOrigem, int dominioId, string dominioTid)
		{
			var resultado = new ReservaLegal();

			using (
				var cmd =
					new OracleCommand(
						"SELECT situacao_id,numero_termo,arl_croqui,(case when t.compensada = 1 and t.cedente_receptor = 2 then 1 else 0 end) compensada,cedente_receptor,emp_compensacao_id FROM " + schema +
						".HST_CRT_DOMINIALIDADE_RESERVA t WHERE t.dominio_id = :dominio_id AND t.dominio_tid = :dominio_tid", conn))
			{
				cmd.Parameters.Add(new OracleParameter("dominio_id", dominioId));
				cmd.Parameters.Add(new OracleParameter("dominio_tid", dominioTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						if (Convert.ToInt32(dr["situacao_id"]) == 1)
						{
							resultado.resposta = "Não";
						}
						else
						{
							resultado.resposta = "Sim";

							var dados = new DadosReserva()
							{
								numero = dr.GetValue<string>("numero_termo"),
								data = new DateTime(1900, 01, 01),
								reservaDentroImovel = (Convert.ToInt32(dr["compensada"]) == 0 ? "Sim" : "Não")
							};
							if (string.IsNullOrEmpty(dados.numero))
							{
								dados.numero = "Não informado";
							}

							var area = dr.GetValue<double>("arl_croqui");
							dados.area = area > 0 ? Convert.ToString(Math.Round(area / 10000, 2), CultureInfo.InvariantCulture) : "0";
							var empreendimentoCedente = dr["emp_compensacao_id"];
							if (dados.reservaDentroImovel == "Não" && !Convert.IsDBNull(empreendimentoCedente) && empreendimentoCedente != null)
							{
								//TODO:Pendente da ISSUE http://hercules:8080/browse/SIMLAMIDAF-2297
								//dados.numeroCAR = "ES-0000001-00000000000000000000000000000001";
								dados.numeroCAR = ObterNumeroSICAR(conn, schema, Convert.ToInt32(empreendimentoCedente));
							}
							resultado.dadosReserva = new List<DadosReserva>() { dados };
						}
					}
				}
			}

			return resultado;
		}

		private static string ObterNumeroSICAR(OracleConnection conn, string schema, int empreendimentoCedente)
		{
			using (var cmd = new OracleCommand("select s.codigo_imovel from " + schema + ".tab_controle_sicar s where s.empreendimento=:empreendimento and s.solicitacao_car_esquema=1", conn))
			{
				cmd.Parameters.Add(new OracleParameter("empreendimento", empreendimentoCedente));

				using (var dr = cmd.ExecuteReader())
					if (dr.Read())
						return Convert.ToString(dr["codigo_imovel"]);
			}
			return String.Empty;
		}

		/// <summary>
		/// Obters the geometrias imovel.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="empreendimentoId">The empreendimento identifier.</param>
		/// <param name="empreendimentoTid">The empreendimento tid.</param>
		/// <returns></returns>
		private static List<Geo> ObterGeometriasImovel(OracleConnection conn, string schema, int empreendimentoId, string empreendimentoTid, int dominialidadeId, string dominialidadeTid, int projetoGeoId, string projetoGeoTid)
		{
			var geo = new List<Geo>();

			#region Buscar dominialdiade para ARL_...

			//var dominioId = 0;
			//var dominioTid = string.Empty;

			//using (
			//	var cmd =
			//		new OracleCommand(
			//			"SELECT dominialidade_dominio_id,tid FROM " + schema +
			//			".HST_CRT_DOMINIALIDADE_DOMINIO t WHERE t.dominialidade_id = :dominialidade_id AND dominialidade_tid = :dominialidade_tid",
			//			conn))
			//{
			//	cmd.Parameters.Add(new OracleParameter("dominialidade_id", dominialidadeId));
			//	cmd.Parameters.Add(new OracleParameter("dominialidade_tid", dominialidadeTid));

			//	using (var dr = cmd.ExecuteReader())
			//	{
			//		while (dr.Read())
			//		{
			//			dominioId = Convert.ToInt32(dr["dominialidade_dominio_id"]);
			//			dominioTid = Convert.ToString(dr["tid"]);
			//		}
			//	}
			//}

			//var identificacaoReserva = new List<Tuple<int, string>>();

			//using (
			//	var cmd =
			//		new OracleCommand(
			//			"SELECT identificacao,situacao_id FROM " + schema +
			//			".HST_CRT_DOMINIALIDADE_RESERVA t WHERE t.dominio_id = :dominio_id AND t.dominio_tid = :dominio_tid AND t.situacao_id IN (2,3)", conn))
			//{
			//	cmd.Parameters.Add(new OracleParameter("dominio_id", dominioId));
			//	cmd.Parameters.Add(new OracleParameter("dominio_tid", dominioTid));

			//	using (var dr = cmd.ExecuteReader())  
			//	{
			//		while (dr.Read())
			//		{
			//			identificacaoReserva.Add(new Tuple<int, string>(Convert.ToInt32(dr["situacao_id"]), Convert.ToString(dr["identificacao"])));
			//		}
			//	}
			//}


			var identificacaoReserva = new List<Tuple<int, string>>();

			using (
				var cmd =
					new OracleCommand(
						"SELECT identificacao,situacao_id FROM " + schema +
						".HST_CRT_DOMINIALIDADE_RESERVA t WHERE t.situacao_id IN (2,3) and (t.dominio_id, t.dominio_tid) in (SELECT dominialidade_dominio_id,tid FROM " + schema +
						".HST_CRT_DOMINIALIDADE_DOMINIO t WHERE t.dominialidade_id = :dominialidade_id AND dominialidade_tid = :dominialidade_tid)", conn))
			{
				cmd.Parameters.Add(new OracleParameter("dominialidade_id", dominialidadeId));
				cmd.Parameters.Add(new OracleParameter("dominialidade_tid", dominialidadeTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						identificacaoReserva.Add(new Tuple<int, string>(Convert.ToInt32(dr["situacao_id"]), Convert.ToString(dr["identificacao"])));
					}
				}
			}


			#endregion


			var bancoGeo = (schema == CarUtils.GetEsquemaInstitucional()
				? CarUtils.GetBancoInstitucionalGeo()
				: CarUtils.GetBancoCredenciadoGeo());

			var schemaGeo = (schema == CarUtils.GetEsquemaInstitucional()
				? CarUtils.GetEsquemaInstitucionalGeo()
				: CarUtils.GetEsquemaCredenciadoGeo());

			using (var connGeo = new OracleConnection(bancoGeo))
			{
				connGeo.Open();

				//Geometria Obrigatória.
				geo.Add(ObterGeometriaAreaImovel(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				//Opcionais
				geo.AddRange(ObterGeometriaArlARecuperar(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaReservatorioArtifical(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaRioAte10(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaRio10A50(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaRio50A200(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaRio200A600(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaRioAcima600(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaNascenteOlhoDagua(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaLagoNatural(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaManguezal(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaVegetacaoNativa(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaAreaDeclividadeMaior45(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaBordaChapada(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaAreaConsolidada(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));
				geo.AddRange(ObterGeometriaAppTotal(connGeo, schemaGeo, projetoGeoId, projetoGeoTid));

				//Areas que dependem da identificacao da Reserva Legal
				identificacaoReserva.ForEach((item) =>
				{
					if (item.Item1 == 2)
					{
						geo.AddRange(ObterGeometriaArlProposta(connGeo, schemaGeo, projetoGeoId, projetoGeoTid, item.Item2));
					}

					if (item.Item1 == 3)
					{
						geo.AddRange(ObterGeometriaArlAverbada(connGeo, schemaGeo, projetoGeoId, projetoGeoTid, item.Item2));
					}
				});
			}

			//Remover geometrias nulas para envio
			geo.RemoveAll(x => x == null);

			return geo;
		}

		/// <summary>
		/// Obtem a geometrias do Banco, passando os parâmetros do empreendimetno
		/// </summary>
		/// <param name="conn">A conexão de banco já iniciada.</param>
		/// <param name="tabela">A tabela geo.</param>
		/// <param name="projetoGeoId">O id do projeto geo.</param>
		/// <param name="projetoGeoTid">o tid do projeto geo.</param>
		/// <param name="tipoGeometriaGeoJson">Qual o tipo da geometria geo json.</param>
		/// <param name="tipoGeometriaCar">Qual o tipo da geometria car.</param>
		/// <param name="filtro">Opcional: Filtro SQL adicional para a consulta.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">
		/// Geometria  + tipoGeometriaCar + inválida:  + exception.Message
		/// or
		/// Geometria  + tipoGeometriaCar +  ID  + id + inválida:  + exception.Message
		/// </exception>
		private static IEnumerable<Geo> ObterGeometrias(OracleConnection conn, string tabela, int projetoGeoId, string projetoGeoTid, string tipoGeometriaGeoJson, string tipoGeometriaCar, string filtro = "")
		{
			var geometrias = new List<Geo>();

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("SELECT id,");
			switch (tipoGeometriaGeoJson)
			{
				case Geometria.POLYGON:
					sqlBuilder.Append("area_m2,");
					break;
				case Geometria.LINESTRING:
					sqlBuilder.Append("largura,");
					break;
			}
			sqlBuilder.Append(" (CASE WHEN TEMARCO(t.geometry)='TRUE' THEN SDO_CS.TRANSFORM(sdo_geom.sdo_arc_densify(t.geometry, 0.01001, 'arc_tolerance=0,00001'), 4674).get_wkt() ");
			sqlBuilder.Append(" ELSE SDO_CS.TRANSFORM(t.geometry, 4674).get_wkt() END) as wkt FROM ");
			sqlBuilder.Append(tabela + " t WHERE t.projeto = :projeto AND (t.tid = :tid or t.projeto_tid = :tid)");
			if (filtro != "")
			{
				sqlBuilder.Append(" AND " + filtro);
			}

			using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
			{
				cmd.Parameters.Add(new OracleParameter("projeto", projetoGeoId));
				cmd.Parameters.Add(new OracleParameter("tid", projetoGeoTid));

				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						var geo = new Geo() { tipo = tipoGeometriaCar };

						try
						{
							//geo.geoJson = GeometryFromWKT.Parse(Convert.ToString(dr["wkt"])).ToGeoJson();
							var multipolygon = GeoJSONFromWKT.Parse(Convert.ToString(dr["wkt"]));
							geo.geoJson = multipolygon;
						}
						catch (Exception exception)
						{
							var id = Convert.ToString(dr["id"]);

							throw new Exception("Geometria " + tipoGeometriaCar + " ID " + id + "inválida: " + exception.Message);
						}

						switch (tipoGeometriaGeoJson)
						{
							case Geometria.LINESTRING:
								//geo.largura = Math.Round(Convert.ToDouble(dr["largura"]), 2);
								geo.area = 0;
								break;
							case Geometria.POINT:
								//geo.largura = 0;
								geo.area = 0;
								break;
							case Geometria.POLYGON:
								//geo.largura = 0;
								geo.area = Math.Round(Convert.ToDouble(dr["area_m2"]) / 10000, 2); //Converter para hectare
								//geo.geoJson.type = Geometria.MULTIPOLYGON;
								geo.geoJson = new MultiPolygon(new List<Polygon>() { geo.geoJson as Polygon });
								break;
						}

						//Criar arquivo geojson para testes
						/*var id = Convert.ToString(dr["id"]);

						try
						{
							var multipolygon = GeoJSONFromWKT.Parse(Convert.ToString(dr["wkt"]));

							//var newFeature = new Feature(multipolygon);

							var serializer = new JsonSerializer();

							using (var sw = new StreamWriter(tipoGeometriaCar + "-" + id + "-" + Guid.NewGuid() + ".geojson"))
							{
								using (var writer = new JsonTextWriter(sw))
								{
									serializer.Serialize(writer, multipolygon);
								}
							}
						}
						catch (Exception exception)
						{
							throw new Exception("Geometria " + tipoGeometriaCar + " ID " + id + "inválida: " + exception.Message);
						}*/
						//Fim do Teste

						geometrias.Add(geo);
					}
				}
			}

			return geometrias;
		}


		/// <summary>
		/// Obters the geometria area imovel.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">Erro ao buscar os dados Geo - ATP do imóvel inexistente.</exception>
		private static Geo ObterGeometriaAreaImovel(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_ATP";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON, Geo.TipoAreaImovel));

			if (geometrias.Count == 0)
			{
				throw new Exception("Erro ao buscar os dados Geo - ATP do imóvel inexistente.");
			}

			return geometrias[0];
		}

		/// <summary>
		/// Obters the geometria arl a recuperar.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaArlARecuperar(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_ARL";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoArlARecuperar, "situacao = 'USO'"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria rio ate10.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaRioAte10(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_RIO_AREA";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON, Geo.TipoRioAte10, "largura <= 10"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria rio10 a50.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaRio10A50(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_RIO_AREA";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON, Geo.TipoRio10A50,
				"largura > 10 AND largura <= 50"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria reservatorio artifical.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaReservatorioArtifical(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_REPRESA";

			var geometrias = new List<Geo>();


			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON, Geo.TipoReservArtificialDecorBarramento));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria rio50 a200.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaRio50A200(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_RIO_AREA";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoRio50A200,
				"largura > 50 AND largura <= 200"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria rio200 a600.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaRio200A600(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_RIO_AREA";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoRio200A600,
				"largura > 200 AND largura <= 600"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria rio acima600.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaRioAcima600(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_RIO_AREA";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoRioAcima600,
				"largura > 600"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria nascente olho dagua.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaNascenteOlhoDagua(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_NASCENTE";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POINT, Geo.TipoNascenteOlhoDagua));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria lago natural.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaLagoNatural(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_LAGOA";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON, Geo.TipoLagoNatural));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria area declividade maior45.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaAreaDeclividadeMaior45(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_REST_DECLIVIDADE";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoAreaDeclivMaior45));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria borda chapada.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaBordaChapada(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_ESCARPA";

			var geometrias = new List<Geo>();

			//A Tabela é área, mas como não tem coluna AREA_M2 nem LARGURA, pedir coluna de dimensão sendo POINT para não usar nenhuma
			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POINT, Geo.TipoBordaChapada));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria manguezal.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaManguezal(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_AVN";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON, Geo.TipoManguezal,
				"vegetacao = 'MANGUE'"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria vegetacao nativa.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaVegetacaoNativa(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_AVN";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoVegetacaoNativa,
				"vegetacao NOT IN ('MANGUE','BREJO','RESTINGA','RESTINGA-APP')"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria application total.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaAppTotal(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_AREAS_CALCULADAS";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON, Geo.TipoAppTotal,
				"tipo = 'APP_APMP'"));

			return geometrias;
		}

		/// <summary>
		/// Obters the geometria area consolidada.
		/// </summary>
		/// <param name="conn">The connection.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="projetoGeoId">The projeto geo identifier.</param>
		/// <param name="projetoGeoTid">The projeto geo tid.</param>
		/// <returns></returns>
		private static IEnumerable<Geo> ObterGeometriaAreaConsolidada(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid)
		{
			var tabela = schema + ".HST_AA";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoAreaConsolidada));

			return geometrias;
		}

		private static IEnumerable<Geo> ObterGeometriaArlAverbada(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid, string identificacaoReserva)
		{
			var tabela = schema + ".HST_ARL";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoArlAverbada, "codigo = '" + identificacaoReserva + "'"));

			return geometrias;
		}

		private static IEnumerable<Geo> ObterGeometriaArlProposta(OracleConnection conn, string schema, int projetoGeoId, string projetoGeoTid, string identificacaoReserva)
		{
			var tabela = schema + ".HST_ARL";

			var geometrias = new List<Geo>();

			geometrias.AddRange(ObterGeometrias(conn, tabela, projetoGeoId, projetoGeoTid, Geometria.POLYGON,
				Geo.TipoArlProposta, "codigo = '" + identificacaoReserva + "'"));

			return geometrias;
		}

		/// <summary>
		/// Gerars the arquivo car.
		/// </summary>
		/// <param name="car">The car object.</param>
		/// <param name="conn">The database connection already in use.</param>
		/// <returns></returns>
		private static string GerarArquivoCAR(CAR car, OracleConnection conn)
		{
			var pathArquivoTemporario = new ArquivoManager().BuscarDiretorioArquivoTemporario(conn);

			if (!pathArquivoTemporario.EndsWith("\\"))
				pathArquivoTemporario += "\\";
			
			pathArquivoTemporario += "SICAR\\";
			if (!Directory.Exists(Path.GetDirectoryName(pathArquivoTemporario)))
				Directory.CreateDirectory(Path.GetDirectoryName(pathArquivoTemporario));

			var serializer = new JsonSerializer();

			serializer.Converters.Add(new JavaScriptDateTimeConverter());
			serializer.NullValueHandling = NullValueHandling.Include;
			serializer.Formatting = Formatting.Indented;

			var carPath = car.origem.codigoProtocolo + ".car";

			if(File.Exists(pathArquivoTemporario + carPath))
				File.Delete(pathArquivoTemporario + carPath);

			using (var sw = new StreamWriter(pathArquivoTemporario + car.origem.codigoProtocolo))
			using (var writer = new JsonTextWriter(sw))
			{
				serializer.Serialize(writer, car);
			}
			

			using (var zip = new ZipFile(pathArquivoTemporario + carPath))
			{
				zip.AddFile(pathArquivoTemporario + car.origem.codigoProtocolo, "");
				zip.Save(pathArquivoTemporario + carPath);
			}

			File.Delete(pathArquivoTemporario + car.origem.codigoProtocolo);

			return carPath;
		}
	}
}