using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data
{
	public class DuaDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public string UsuarioCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}
		public string UsuarioInterno
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}
		public string UsuarioConsulta
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioConsulta); }
		}

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		#endregion

		public Dua Obter(int id, BancoDeDados banco = null)
		{
			Dua solicitacao = new Dua();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando("select s.tid," +
					  " s.numero," +
					  " s.data_emissao," +
					  " s.situacao_data," +
					  " l.id situacao," +
					  " l.texto situacao_texto," +
					  " s.situacao_anterior," +
					  " la.texto situacao_anterior_texto," +
					  " s.situacao_anterior_data," +
					  " nvl(pes.nome, pes.razao_social) declarante_nome_razao," +
					  " s.requerimento," +
					  " s.atividade," +
					  " e.id empreendimento_id," +
					  " e.denominador empreendimento_nome," +
					  " e.codigo empreendimento_codigo," +
					  " s.declarante," +

					  " p.id protocolo_id," +
					  " p.protocolo," +
					  " p.numero protocolo_numero," +
					  " p.ano protocolo_ano," +

					  " s.credenciado autor_id," +
					  " nvl(f.nome, f.razao_social) autor_nome," +
					  " lct.texto  autor_tipo," +
					  " 'Credenciado' autor_modulo," +

					  " s.motivo," +
					  " tr.data_criacao requerimento_data_cadastro," +
					  " s.projeto_digital" +
					" from " +
					  " tab_car_solicitacao          s," +
					  " lov_car_solicitacao_situacao l," +
					  " lov_car_solicitacao_situacao la," +
					  " tab_empreendimento           e," +
					  " tab_pessoa                   pes," +
					  " tab_requerimento             tr," +
					  " tab_credenciado              tc," +
					  " tab_pessoa                   f," +
					  " lov_credenciado_tipo         lct," +
					  " ins_protocolo                p" +
					" where s.situacao = l.id" +
					" and s.situacao_anterior = la.id(+)" +
					" and s.empreendimento = e.id" +
					" and s.declarante = pes.id" +
					" and s.requerimento = tr.id" +
					" and s.empreendimento = e.id" +
					" and tc.id = s.credenciado" +
					" and f.id = tc.pessoa" +
					" and lct.id = tc.tipo" +
					" and s.requerimento=p.requerimento(+)" +
					" and s.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						//solicitacao.Id = id;
						//solicitacao.Tid = reader.GetValue<String>("tid");
						//solicitacao.Numero = reader.GetValue<String>("numero");
						//solicitacao.DataEmissao.DataTexto = reader.GetValue<String>("data_emissao");
						//solicitacao.SituacaoId = reader.GetValue<Int32>("situacao");
						//solicitacao.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						//solicitacao.DataSituacao.DataTexto = reader.GetValue<String>("situacao_data");
						//solicitacao.SituacaoAnteriorId = reader.GetValue<Int32>("situacao_anterior");
						//solicitacao.SituacaoAnteriorTexto = reader.GetValue<String>("situacao_anterior_texto");
						//solicitacao.DataSituacaoAnterior.DataTexto = reader.GetValue<String>("situacao_anterior_data");
						//solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
						//solicitacao.Requerimento.DataCadastro = reader.GetValue<DateTime>("requerimento_data_cadastro");
						//solicitacao.Atividade.Id = reader.GetValue<Int32>("atividade");
						//solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						//solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						//solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
						//solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante");
						//solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nome_razao");

						//solicitacao.Protocolo.Id = reader.GetValue<Int32>("protocolo_id");
						//solicitacao.Protocolo.IsProcesso = reader.GetValue<Int32>("protocolo") == 1;
						//solicitacao.Protocolo.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_numero");
						//solicitacao.Protocolo.Ano = reader.GetValue<Int32>("protocolo_ano");

						//solicitacao.AutorId = reader.GetValue<Int32>("autor_id");
						//solicitacao.AutorNome = reader.GetValue<String>("autor_nome");
						//solicitacao.AutorTipoTexto = reader.GetValue<String>("autor_tipo");
						//solicitacao.AutorModuloTexto = reader.GetValue<String>("autor_modulo");

						//solicitacao.Motivo = reader.GetValue<String>("motivo");
						//solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_digital");
					}

					reader.Close();
				}

				#endregion
			}

			return solicitacao;
		}
		
	}
}