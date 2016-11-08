using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
    public class PessoaCredenciadoBus
    {
        PessoaCredenciadoDa _da = new PessoaCredenciadoDa();

        public Pessoa Obter(int id, BancoDeDados banco = null, bool simplificado = false)
        {
            Pessoa pessoa = _da.Obter(id, banco, simplificado);

            if (!simplificado)
            {
                CarregarDadosListas(pessoa);
            }

            return pessoa;
        }

        void CarregarDadosListas(Pessoa pessoa)
        {
            if (pessoa.Id > 0)
            {
                var listaContatos = ListaCredenciadoBus.ListarMeiosContato;
                ContatoLst contatoAux;

                pessoa.MeiosContatos.ForEach(x =>
                {
                    contatoAux = listaContatos.SingleOrDefault(y => y.Id == (int)x.TipoContato) ?? new ContatoLst();
                    x.Mascara = contatoAux.Mascara;
                    x.TipoTexto = contatoAux.Texto;
                });

                if (pessoa.Endereco.EstadoId > 0)
                {
                    List<Estado> estados = _da.ObterEstados();
                    pessoa.Endereco.EstadoTexto = (estados.SingleOrDefault(x => x.Id == pessoa.Endereco.EstadoId) ?? new Estado()).Sigla;

                    string texto = (_da.ObterMunicipios(estados)[pessoa.Endereco.EstadoId].SingleOrDefault(x => x.Id == pessoa.Endereco.MunicipioId) ?? new Municipio()).Texto;
                }

                if (pessoa.IsFisica)
                {
                    pessoa.Fisica.EstadoCivilTexto = (_da.ObterEstadosCivis().SingleOrDefault(x => x.Id == pessoa.Fisica.EstadoCivil) ?? new EstadoCivil()).Texto;
                    pessoa.Fisica.SexoTexto = (_da.ObterSexos().SingleOrDefault(x => x.Id == pessoa.Fisica.Sexo) ?? new Sexo()).Texto;
                    pessoa.Fisica.Profissao.ProfissaoTexto = (_da.ObterProfissoes().SingleOrDefault(x => x.Id == pessoa.Fisica.Profissao.Id) ?? new ProfissaoLst()).Texto;
                    pessoa.Fisica.Profissao.OrgaoClasseTexto = (_da.ObterOrgaoClasses().SingleOrDefault(x => x.Id == pessoa.Fisica.Profissao.OrgaoClasseId) ?? new OrgaoClasse()).Texto;
                }
            }
        }
    }
}
