using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloSobre;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloSobre.Data
{
	public class SobreDa
	{
		#region Propriedades
		
		private String EsquemaBanco { get; set; }

		#endregion

		public SobreDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public Sobre Obter()
		{
			Sobre sobre = new Sobre();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				sobre = bancoDeDados.ObterEntity<Sobre>(bancoDeDados.CriarComando(@"
					select *
					  from (select t.id Id,
								   t.numero Versao,
								   to_char(t.data, 'dd/MM/yyyy') Data,
								   (select to_char(c.valor) from {0}cnf_sistema c where c.campo = 'orgaosigla') || ' - ' ||
								   (select initcap(c.valor) from {0}cnf_sistema c where c.campo = 'orgaonome') Licenciado
							  from {0}lov_controle_versao t
							 order by t.data desc, t.id desc)
					 where rownum = 1", EsquemaBanco));
			}

			return sobre;
		}

		public List<SobreItem> ObterSobreItens(int versaoId)
		{
			List<SobreItem> lstSobreItem = new List<SobreItem>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select ti.id        Id,
						   ti.versao    VersaoId,
						   ti.numero_tp NumeroTP,
						   ti.descricao Descricao,
						   tt.texto     Tipo
					  from {0}lov_controle_versao_item ti, 
						   {0}lov_controle_versao_tipo tt
					 where ti.tipo = tt.id(+)
					   and ti.versao = :versaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("versaoId", versaoId, DbType.Int32);

				lstSobreItem = bancoDeDados.ObterEntityList<SobreItem>(comando);
			}

			return lstSobreItem;
		}

		public List<Sobre> ObterVersoes()
		{
			List<Sobre> lstVesoes = new List<Sobre>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				lstVesoes = bancoDeDados.ObterEntityList<Sobre>(bancoDeDados.CriarComando(@"
					select t.id Id,
							t.numero Versao,
							to_char(t.data, 'dd/MM/yyyy') Data,
							(select to_char(c.valor) from {0}cnf_sistema c where c.campo = 'orgaosigla') || ' - ' ||
							(select initcap(c.valor) from {0}cnf_sistema c where c.campo = 'orgaonome') Licenciado
						from {0}lov_controle_versao t
						order by t.data desc, t.id  desc", EsquemaBanco));
			}

			return lstVesoes;
		}
	}
}
