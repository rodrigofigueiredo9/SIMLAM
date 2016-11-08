using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloAnalise;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Data
{
	class PecaTecnicaDa
	{
		private string EsquemaBanco { get; set; }

		public PecaTecnicaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal PecaTecnicaRelatorio Obter(int id, BancoDeDados banco = null)
		{
			PecaTecnicaRelatorio objeto = new PecaTecnicaRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.numero || '/' || p.ano numero, le.sigla estado_sigla, ee.bairro, ee.distrito, lm.texto municipio, 
					   nvl(pt.setor_cadastro, 0) setor_cadastro, case when pt.elaborador_tipo = 1 then (select f.nome from tab_funcionario f where f.id = pt.elaborador_tecnico) 
						  else (select nvl(e.nome, e.razao_social) from tab_pessoa e where e.id = pt.elaborador_pessoa) end elaborador, case when pt.elaborador_tipo = 1 then 
						null else (select pr.numero_art from tab_protocolo_responsavel pr where pr.protocolo = p.id and pr.responsavel = nvl(pt.elaborador_pessoa, pt.elaborador_tecnico)) 
						 end elaborador_numero_art, case when pt.elaborador_tipo = 1 then null else (select pp.texto from tab_pessoa_profissao ep, tab_profissao pp where ep.profissao = pp.id 
						and ep.pessoa = pt.elaborador_pessoa) end elaborador_profissao, case when pt.elaborador_tipo = 1 then null else (select oc.orgao_sigla from tab_pessoa_profissao ep, 
						tab_orgao_classe oc where ep.orgao_classe = oc.id and ep.pessoa = pt.elaborador_pessoa) end elaborador_orgao_classe, case when pt.elaborador_tipo = 1 then 
						null else (select ep.registro from tab_pessoa_profissao ep where ep.pessoa = pt.elaborador_pessoa) end elaborador_registro, (select count(*)  from crt_dominialidade d, 
						crt_dominialidade_dominio dom where d.empreendimento = p.empreendimento and d.id = dom.dominialidade) dominio_qtd 
					  from tab_peca_tecnica pt,
						   tab_protocolo               p,
						   tab_empreendimento_endereco ee,
						   lov_estado                  le,
						   lov_municipio               lm
					 where pt.protocolo = p.id
					   and p.empreendimento = ee.empreendimento
					   and ee.correspondencia = 0
					   and ee.estado = le.id
					   and ee.municipio = lm.id
					   and pt.id = :peca_tecnica ", EsquemaBanco);

				comando.AdicionarParametroEntrada("peca_tecnica", id, DbType.Int32);


				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						objeto.Protocolo = reader.GetValue<string>("numero");
						objeto.Municipio = reader.GetValue<string>("municipio");
						objeto.Uf = reader.GetValue<string>("estado_sigla");
						objeto.Elaborador = reader.GetValue<string>("elaborador");

						objeto.Bairro = reader.GetValue<string>("bairro");
						objeto.Distrito = reader.GetValue<string>("distrito");

						objeto.ElaboradorArt = reader.GetValue<string>("elaborador_numero_art");
						objeto.ElaboradorProfissao = reader.GetValue<string>("elaborador_profissao");
						objeto.ElaboradorOrgaoClasse = reader.GetValue<string>("elaborador_orgao_classe");
						objeto.ElaboradorRegistro = reader.GetValue<string>("elaborador_registro");
						objeto.SetorId = reader.GetValue<Int32>("setor_cadastro");
						objeto.DominioQtd = reader.GetValue<Int32>("dominio_qtd");
					}
					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome from tab_peca_tecnica_dest d, tab_pessoa p where d.destinatario=p.id and d.peca_tecnica =  :peca_tecnica", EsquemaBanco);
				comando.AdicionarParametroEntrada("peca_tecnica", id, DbType.Int32);


				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						objeto.Destinatarios.Add(reader.GetValue<string>("nome"));
					}
					reader.Close();
				}

			}

			return objeto;
		}
	}
}