using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Data
{
	public class ComunicacaoInternaDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public ComunicacaoInternaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal ProtocoloRelatorio Obter(int id, BancoDeDados banco = null)
		{
			ProtocoloRelatorio protocolo = new ProtocoloRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Protocolo
				Comando comando = bancoDeDados.CriarComando(@"
				select d.*, f.nome funcionario_nome, lm.texto municipio, le.texto estado, le.sigla uf,
						(select stragg(c.nome) from {0}tab_funcionario_cargo fc, {0}tab_cargo c where fc.funcionario = f.id and c.id = fc.cargo) funcionario_cargo
					from (select p.nome protocolo_nome,
								e.codigo emp_codigo,
								p.protocolo,
								lp.texto protocolo_texto,
								p.tipo protocolo_tipo,
								t.texto protocolo_tipo_texto,
								p.numero || '/' || p.ano protocolo_numero,
								p.fiscalizacao fiscalizacao_numero,
								to_char(p.data_criacao, 'DD/MM/yyyy') data_criacao,
								a.tipo interessado_tipo,
								coalesce(a.nome, a.razao_social, p.interessado_livre) interessado_nomerazaosocial,
								nvl(a.cpf, a.cnpj) interessado_cpfcnpj,
								e.denominador empreendimento_nome,
								e.cnpj empreendimento_cnpj,
								r.numero requerimento_numero,
								nvl(p.checagem, p.checagem_pendencia) checagem,
								nvl(p.setor, (select a.setor from {0}tab_protocolo_associado pa, {0}tab_protocolo a where pa.protocolo = a.id and pa.associado = :id)) setor,
								nvl(p.emposse, (select a.emposse from {0}tab_protocolo_associado pa, {0}tab_protocolo a where pa.protocolo = a.id and pa.associado = :id)) emposse,
								pro_ass.numero numero_associado,
								pro_ass.tipo tipo_associado,
								p.assunto, (select fd.nome from tab_funcionario fd where fd.id = p.destinatario) destinatario, (select s.nome from tab_setor s where s.id = p.destinatario_setor) destinatario_setor, p.descricao
							from {0}tab_protocolo p,
								(select pa.id, pa.numero || '/' || pa.ano numero, ta.texto tipo from {0}tab_protocolo pa, {0}lov_protocolo ta where pa.protocolo = ta.id) pro_ass,
								{0}lov_protocolo_tipo t,
								{0}lov_protocolo lp,
								{0}tab_pessoa a,
								{0}tab_empreendimento e,
								{0}tab_requerimento r
							where t.id = p.tipo
							and p.protocolo = lp.id
							and p.interessado = a.id(+)
							and p.empreendimento = e.id(+)
							and p.requerimento = r.id(+)
							and p.protocolo_associado = pro_ass.id(+)
							and p.id = :id) d,
						{0}tab_funcionario f,
						{0}tab_setor s,
						{0}tab_setor_endereco se,
						{0}lov_municipio lm,
						{0}lov_estado le
					where f.id = d.emposse
					and s.id = d.setor
					and se.setor(+) = s.id
					and se.municipio = lm.id(+)
					and se.estado = le.id(+)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						PessoaRelatorio interessado = new PessoaRelatorio();

						interessado.Tipo = reader.GetValue<int>("interessado_tipo");
						if (interessado.IsFisica)
						{
							interessado.Fisica.Nome = reader.GetValue<String>("interessado_nomerazaosocial");
							interessado.Fisica.CPF = reader.GetValue<String>("interessado_cpfcnpj");
						}
						else
						{
							interessado.Juridica.RazaoSocial = reader.GetValue<String>("interessado_nomerazaosocial");
							interessado.Juridica.CNPJ = reader.GetValue<String>("interessado_cpfcnpj");
						}

						EmpreendimentoRelatorio empreendimento = new EmpreendimentoRelatorio();

						empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						empreendimento.Codigo = reader.GetValue<Int32>("emp_codigo");
						empreendimento.CNPJ = reader.GetValue<String>("empreendimento_cnpj");
						protocolo.Nome = reader.GetValue<String>("protocolo_nome");
						protocolo.ProtocoloProcDoc = reader.GetValue<Int32>("protocolo");
						protocolo.ProtocoloTexto = reader.GetValue<String>("protocolo_texto");
						protocolo.Numero = reader.GetValue<String>("protocolo_numero");
						protocolo.ProtocoloTipo = reader.GetValue<Int32>("protocolo_tipo");
						protocolo.TipoTexto = reader.GetValue<String>("protocolo_tipo_texto");
						protocolo.ChecagemNumero = reader.GetValue<String>("checagem");
						protocolo.RequerimentoNumero = reader.GetValue<String>("requerimento_numero");
						protocolo.FiscalizacaoNumero = reader.GetValue<String>("fiscalizacao_numero");
						protocolo.Data = reader.GetValue<String>("data_criacao");
						protocolo.SetorId = reader.GetValue<Int32>("setor");
						protocolo.OrgaoMunicipio = reader.GetValue<String>("municipio");
						protocolo.OrgaoUF = reader.GetValue<String>("uf");
						protocolo.UsuarioNome = reader.GetValue<String>("funcionario_nome");
						protocolo.UsuarioCargo = reader.GetValue<String>("funcionario_cargo");
						protocolo.ProtocoloAssociadoNumero = reader.GetValue<String>("numero_associado");
						protocolo.ProtocoloAssociadoTipo = reader.GetValue<String>("tipo_associado");
						protocolo.Destinatario = reader.GetValue<String>("destinatario");
						protocolo.SetorDestinatario = reader.GetValue<String>("destinatario_setor");
						protocolo.Assunto = reader.GetValue<String>("assunto");
						protocolo.Descricao = reader.GetValue<String>("descricao");
						protocolo.Interessado = interessado;
						protocolo.Empreendimento = empreendimento;

					}

					reader.Close();
				}

				#endregion
			}

			return protocolo;
		}
	}
}